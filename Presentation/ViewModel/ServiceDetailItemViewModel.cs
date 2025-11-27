using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
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
        private readonly IServiceService _ServiceService;

        // Original list (unchanged)
        private ObservableCollection<ServiceDTO> _originalServiceList;
        public ObservableCollection<ServiceDTO> OriginalServiceList
        {
            get => _originalServiceList;
            set { _originalServiceList = value; OnPropertyChanged(); }
        }

        // Displayed list (after filtering)
        private ObservableCollection<ServiceDTO> _serviceList;
        public ObservableCollection<ServiceDTO> ServiceList
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
        private ServiceDTO _selectedService;
        public ServiceDTO SelectedService
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

        // Constructor với Dependency Injection
        public ServiceDetailItemViewModel(IServiceService ServiceService)
        {
            _ServiceService = ServiceService;
            
            OriginalServiceList = new ObservableCollection<ServiceDTO>(_ServiceService.GetAll());
            ServiceList = new ObservableCollection<ServiceDTO>(OriginalServiceList);

            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            ConfirmCommand = new RelayCommand<Window>(
                (p) => SelectedService != null,
                (p) =>
                {
                    MessageBox.Show($"Bạn đã chọn dịch vụ: {SelectedService.ServiceName}", "Xác nhận", MessageBoxButton.OK, MessageBoxImage.Information);

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
                ServiceList = new ObservableCollection<ServiceDTO>(OriginalServiceList);
                return;
            }

            string search = SearchText.Trim().ToLowerInvariant();

            switch (SelectedSearchProperty)
            {
                case "Tên dịch vụ":
                    ServiceList = new ObservableCollection<ServiceDTO>(
                        OriginalServiceList.Where(x => !string.IsNullOrWhiteSpace(x.ServiceName) &&
                            x.ServiceName.ToLowerInvariant().Contains(search)));
                    break;
                case "Đơn giá":
                    ServiceList = new ObservableCollection<ServiceDTO>(
                        OriginalServiceList.Where(x =>
                            x.UnitPrice != null &&
                            x.UnitPrice.Value.ToString("N0").Replace(",", "").Contains(SearchText.Replace(",", "").Trim())
                        )
                    );
                    break;
                case "Ghi chú":
                    ServiceList = new ObservableCollection<ServiceDTO>(
                        OriginalServiceList.Where(x => !string.IsNullOrWhiteSpace(x.Note) &&
                            x.Note.ToLowerInvariant().Contains(search)));
                    break;
                default:
                    ServiceList = new ObservableCollection<ServiceDTO>(OriginalServiceList);
                    break;
            }
        }
    }
}