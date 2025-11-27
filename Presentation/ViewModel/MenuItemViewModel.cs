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
        private readonly IDishService _DishService;

        // Danh sách gốc (không thay đổi)
        private ObservableCollection<DishDTO> _originalFoodList;
        public ObservableCollection<DishDTO> OriginalFoodList
        {
            get => _originalFoodList;
            set { _originalFoodList = value; OnPropertyChanged(); }
        }

        // Danh sách hiển thị (sau khi lọc)
        private ObservableCollection<DishDTO> _foodList;
        public ObservableCollection<DishDTO> FoodList
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
        private DishDTO _selectedFood;
        public DishDTO SelectedFood
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
        public MenuItemViewModel(IDishService DishService)
        {
            _DishService = DishService;
            
            OriginalFoodList = new ObservableCollection<DishDTO>(_DishService.GetAll());
            FoodList = new ObservableCollection<DishDTO>(OriginalFoodList);

            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            ConfirmCommand = new RelayCommand<Window>(
                (p) => SelectedFood != null,
                (p) =>
                {
                    MessageBox.Show($"Bạn đã chọn món: {SelectedFood.DishName}", "Xác nhận", MessageBoxButton.OK, MessageBoxImage.Information);

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
                FoodList = new ObservableCollection<DishDTO>(OriginalFoodList);
                return;
            }

            string search = SearchText.Trim().ToLowerInvariant();

            switch (SelectedSearchProperty)
            {
                case "Tên món":
                    FoodList = new ObservableCollection<DishDTO>(
                        OriginalFoodList.Where(x => !string.IsNullOrWhiteSpace(x.DishName) &&
                            x.DishName.ToLowerInvariant().Contains(search)));
                    break;
                case "Đơn giá":
                    FoodList = new ObservableCollection<DishDTO>(
                            OriginalFoodList.Where(x =>
                                x.UnitPrice != null &&
                                x.UnitPrice.Value.ToString("N0").Replace(",", "").Contains(SearchText.Replace(",", "").Trim())
                            )
                        );
                    break;
                case "Ghi chú":
                    FoodList = new ObservableCollection<DishDTO>(
                        OriginalFoodList.Where(x => !string.IsNullOrWhiteSpace(x.Note) &&
                            x.Note.ToLowerInvariant().Contains(search)));
                    break;
                default:
                    FoodList = new ObservableCollection<DishDTO>(OriginalFoodList);
                    break;
            }
        }
    }
}