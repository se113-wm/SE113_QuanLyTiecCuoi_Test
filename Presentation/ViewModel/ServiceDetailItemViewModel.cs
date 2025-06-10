using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class ServiceDetailItemViewModel : BaseViewModel
    {
        // Original list (unchanged)
        private ObservableCollection<DICHVUDTO> _originalServiceList;
        public ObservableCollection<DICHVUDTO> OriginalServiceList
        {
            get => _originalServiceList;
            set { _originalServiceList = value; OnPropertyChanged(); }
        }

        // Displayed list (after filtering)
        private ObservableCollection<DICHVUDTO> _serviceList;
        public ObservableCollection<DICHVUDTO> ServiceList
        {
            get => _serviceList;
            set { _serviceList = value; OnPropertyChanged(); }
        }

        // Search text property
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

        // Search fields
        public ObservableCollection<string> SearchProperties { get; set; } = new ObservableCollection<string> { "Tên dịch vụ", "Đơn giá", "Ghi chú" };

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

        // Selected service
        private DICHVUDTO _selectedService;
        public DICHVUDTO SelectedService
        {
            get => _selectedService;
            set
            {
                if (_selectedService != value)
                {
                    _selectedService = value;
                    OnPropertyChanged();
                }
            }
        }

        // Confirm command
        public ICommand ConfirmCommand { get; set; }
        // Cancel command
        public ICommand CancelCommand { get; set; }

        public ServiceDetailItemViewModel()
        {
            var service = new DichVuService();
            OriginalServiceList = new ObservableCollection<DICHVUDTO>(service.GetAll());
            ServiceList = new ObservableCollection<DICHVUDTO>(OriginalServiceList);

            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            ConfirmCommand = new RelayCommand<Window>(
                (p) => SelectedService != null,
                (p) =>
                {
                    MessageBox.Show($"Bạn đã chọn dịch vụ: {SelectedService.TenDichVu}", "Xác nhận", MessageBoxButton.OK, MessageBoxImage.Information);

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
                ServiceList = new ObservableCollection<DICHVUDTO>(OriginalServiceList);
                return;
            }

            string search = SearchText.Trim().ToLowerInvariant();

            switch (SelectedSearchProperty)
            {
                case "Tên dịch vụ":
                    ServiceList = new ObservableCollection<DICHVUDTO>(
                        OriginalServiceList.Where(x => !string.IsNullOrWhiteSpace(x.TenDichVu) &&
                            x.TenDichVu.ToLowerInvariant().Contains(search)));
                    break;
                case "Đơn giá":
                    ServiceList = new ObservableCollection<DICHVUDTO>(
                        OriginalServiceList.Where(x =>
                            x.DonGia != null &&
                            x.DonGia.Value.ToString("N0").Replace(",", "").Contains(SearchText.Replace(",", "").Trim())
                        )
                    );
                    break;
                case "Ghi chú":
                    ServiceList = new ObservableCollection<DICHVUDTO>(
                        OriginalServiceList.Where(x => !string.IsNullOrWhiteSpace(x.GhiChu) &&
                            x.GhiChu.ToLowerInvariant().Contains(search)));
                    break;
                default:
                    ServiceList = new ObservableCollection<DICHVUDTO>(OriginalServiceList);
                    break;
            }
        }
    }
}