using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class MenuItemViewModel : BaseViewModel
    {
        private readonly IMonAnService _monAnService;

        // Danh sách gốc (không thay đổi)
        private ObservableCollection<MONANDTO> _originalFoodList;
        public ObservableCollection<MONANDTO> OriginalFoodList
        {
            get => _originalFoodList;
            set { _originalFoodList = value; OnPropertyChanged(); }
        }

        // Danh sách hiển thị (sau khi lọc)
        private ObservableCollection<MONANDTO> _foodList;
        public ObservableCollection<MONANDTO> FoodList
        {
            get => _foodList;
            set { _foodList = value; OnPropertyChanged(); }
        }

        // Thuộc tính tìm kiếm
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        // Các trường tìm kiếm
        public ObservableCollection<string> SearchProperties { get; set; } = new ObservableCollection<string> { "Tên món", "Đơn giá", "Ghi chú" };

        private string _selectedSearchProperty;
        public string SelectedSearchProperty
        {
            get => _selectedSearchProperty;
            set
            {
                if (_selectedSearchProperty != value)
                {
                    _selectedSearchProperty = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        // Món ăn được chọn
        private MONANDTO _selectedFood;
        public MONANDTO SelectedFood
        {
            get => _selectedFood;
            set
            {
                if (_selectedFood != value)
                {
                    _selectedFood = value;
                    OnPropertyChanged();
                }
            }
        }

        // Command xác nhận
        public ICommand ConfirmCommand { get; set; }
        // Command hủy
        public ICommand CancelCommand { get; set; }

        // Constructor với Dependency Injection
        public MenuItemViewModel(IMonAnService monAnService)
        {
            _monAnService = monAnService;
            
            OriginalFoodList = new ObservableCollection<MONANDTO>(_monAnService.GetAll());
            FoodList = new ObservableCollection<MONANDTO>(OriginalFoodList);

            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            ConfirmCommand = new RelayCommand<Window>(
                (p) => SelectedFood != null,
                (p) =>
                {
                    MessageBox.Show($"Bạn đã chọn món: {SelectedFood.TenMonAn}", "Xác nhận", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (p is Window window)
                    {
                        window.DialogResult = true;
                        window.Close();
                    }
                });

            CancelCommand = new RelayCommand<Window>((p) => true, (p) =>
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn hủy không?", "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (p is Window window)
                    {
                        window.Close();
                    }
                }
            });
        }

        private void PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText) || string.IsNullOrWhiteSpace(SelectedSearchProperty))
            {
                FoodList = new ObservableCollection<MONANDTO>(OriginalFoodList);
                return;
            }

            string search = SearchText.Trim().ToLowerInvariant();

            switch (SelectedSearchProperty)
            {
                case "Tên món":
                    FoodList = new ObservableCollection<MONANDTO>(
                        OriginalFoodList.Where(x => !string.IsNullOrWhiteSpace(x.TenMonAn) &&
                            x.TenMonAn.ToLowerInvariant().Contains(search)));
                    break;
                case "Đơn giá":
                    FoodList = new ObservableCollection<MONANDTO>(
                            OriginalFoodList.Where(x =>
                                x.DonGia != null &&
                                x.DonGia.Value.ToString("N0").Replace(",", "").Contains(SearchText.Replace(",", "").Trim())
                            )
                        );
                    break;
                case "Ghi chú":
                    FoodList = new ObservableCollection<MONANDTO>(
                        OriginalFoodList.Where(x => !string.IsNullOrWhiteSpace(x.GhiChu) &&
                            x.GhiChu.ToLowerInvariant().Contains(search)));
                    break;
                default:
                    FoodList = new ObservableCollection<MONANDTO>(OriginalFoodList);
                    break;
            }
        }
    }
}