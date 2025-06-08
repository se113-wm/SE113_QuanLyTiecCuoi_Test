using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class MenuItemViewModel : BaseViewModel
    {
        // Danh sách tất cả món ăn
        public ObservableCollection<MONANDTO> FoodList { get; set; } = new ObservableCollection<MONANDTO>();

        // Danh sách lọc theo tìm kiếm
        public ICollectionView FilteredFoodList { get; set; }

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
                    
                }
            }
        }

        // Các trường tìm kiếm (ví dụ: Tên món, Ghi chú)
        public ObservableCollection<string> SearchProperties { get; set; } = new ObservableCollection<string> { "Tên món", "Ghi chú" };

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

        public MenuItemViewModel()
        {
            // Load danh sách món ăn từ service
            var monAnService = new MonAnService();
            foreach (var item in monAnService.GetAll())
            {
                FoodList.Add(item);
            }

            // Thiết lập mặc định trường tìm kiếm
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            // Thiết lập CollectionView cho filter
            FilteredFoodList = CollectionViewSource.GetDefaultView(FoodList);
            FilteredFoodList.Filter = FilterFood;

            // Command
            ConfirmCommand = new RelayCommand<Window>(
            (p) => SelectedFood != null,
            (p) =>
            {
                MessageBox.Show($"Bạn đã chọn món: {SelectedFood.TenMonAn}", "Xác nhận", MessageBoxButton.OK, MessageBoxImage.Information);

                if (p is Window window)
                {
                    window.DialogResult = true; // ❗ Thêm dòng này để ShowDialog() trả về true
                    window.Close();
                }
            });

            CancelCommand = new RelayCommand<Window>((p) => true, (p) => 
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn hủy không?", "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Close the window
                    if (p is Window window)
                    {
                        window.Close();
                    }
}
            });
        }

        private bool FilterFood(object obj)
        {
            if (obj is MONANDTO food)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true;

                var search = SearchText.ToLowerInvariant();
                if (SelectedSearchProperty == "Tên món")
                    return (food.TenMonAn ?? "").ToLowerInvariant().Contains(search);
                if (SelectedSearchProperty == "Ghi chú")
                    return (food.GhiChu ?? "").ToLowerInvariant().Contains(search);

                // Mặc định tìm cả hai
                return (food.TenMonAn ?? "").ToLowerInvariant().Contains(search)
                    || (food.GhiChu ?? "").ToLowerInvariant().Contains(search);
            }
            return false;
        }
    }
}