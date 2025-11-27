using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class WeddingViewModel : BaseViewModel
    {
        private readonly IBookingService _bookingService;
        private readonly IMenuService _menuService;
        private readonly IServiceDetailService _serviceDetailService;
        private readonly MainViewModel _mainViewModel;

        private ObservableCollection<BookingDTO> _list;
        public ObservableCollection<BookingDTO> List { get => _list; set { _list = value; OnPropertyChanged(); } }

        private ObservableCollection<BookingDTO> _originalList;
        public ObservableCollection<BookingDTO> OriginalList { get => _originalList; set { _originalList = value; OnPropertyChanged(); } }

        private BookingDTO _selectedItem;
        public BookingDTO SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        // Filter properties
        public List<string> StatusFilterList { get; } = new List<string>
        {
            "All",
            "Not Organized",
            "Not Paid",
            "Late Payment",
            "Paid"
        };

        private string _selectedStatus = "All";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }
        
        public ObservableCollection<string> GroomNameFilterList { get; set; }
        public string SelectedGroomName { get => _selectedGroomName; set { _selectedGroomName = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedGroomName;

        public ObservableCollection<string> BrideNameFilterList { get; set; }
        public string SelectedBrideName { get => _selectedBrideName; set { _selectedBrideName = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedBrideName;

        public ObservableCollection<string> HallNameFilterList { get; set; }
        public string SelectedHallName { get => _selectedHallName; set { _selectedHallName = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedHallName;

        public ObservableCollection<int> TableCountFilterList { get; set; }
        public int? SelectedTableCount { get => _selectedTableCount; set { _selectedTableCount = value; OnPropertyChanged(); PerformSearch(); } }
        private int? _selectedTableCount;

        public DateTime? SelectedWeddingDate { get => _selectedWeddingDate; set { _selectedWeddingDate = value; OnPropertyChanged(); PerformSearch(); } }
        private DateTime? _selectedWeddingDate;

        // Search properties
        public ObservableCollection<string> SearchProperties { get; set; }
        public string SelectedSearchProperty { get => _selectedSearchProperty; set { _selectedSearchProperty = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedSearchProperty;

        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); PerformSearch(); } }
        private string _searchText;

        // Command properties
        public ICommand AddCommand { get; set; }
        public ICommand DetailCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => Reset());

        private string _deleteMessage;
        public string DeleteMessage { get => _deleteMessage; set { _deleteMessage = value; OnPropertyChanged(); } }

        // Constructor with Dependency Injection
        public WeddingViewModel(
            MainViewModel mainViewModel, 
            IBookingService bookingService, 
            IMenuService menuService, 
            IServiceDetailService serviceDetailService)
        {
            _mainViewModel = mainViewModel;
            _bookingService = bookingService;
            _menuService = menuService;
            _serviceDetailService = serviceDetailService;

            var all = _bookingService.GetAll().ToList();
            List = new ObservableCollection<BookingDTO>(all);
            OriginalList = new ObservableCollection<BookingDTO>(all);

            // Build filter lists
            GroomNameFilterList = new ObservableCollection<string>(all.Select(x => x.GroomName).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
            BrideNameFilterList = new ObservableCollection<string>(all.Select(x => x.BrideName).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
            HallNameFilterList = new ObservableCollection<string>(all.Select(x => x.Hall?.HallName).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
            TableCountFilterList = new ObservableCollection<int>(all.Select(x => x.TableCount ?? 0).Where(x => x > 0).Distinct().OrderBy(x => x));

            SearchProperties = new ObservableCollection<string>
            {
                "Groom Name",
                "Bride Name",
                "Hall Name",
                "Wedding Date",
                "Table Count",
                "Status"
            };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            AddCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                AddWedding();
            });

            DetailCommand = new RelayCommand<object>((p) => SelectedItem != null, (p) =>
            {
                _mainViewModel.CurrentView = new WeddingDetailView
                {
                    DataContext = Infrastructure.ServiceContainer.CreateWeddingDetailViewModel(SelectedItem.BookingId)
                };
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                {
                    DeleteMessage = "Please select a wedding to delete.";
                    return false;
                }
                if (SelectedItem.PaymentDate.HasValue)
                {
                    DeleteMessage = "Cannot delete paid wedding.";
                    return false;
                }
                if (SelectedItem.WeddingDate.HasValue && SelectedItem.WeddingDate.Value.Date < DateTime.Today.AddDays(1))
                {
                    DeleteMessage = "Cannot delete wedding organized today or in the past.";
                    return false;
                }
                DeleteMessage = string.Empty;
                return true;
            }
            , (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Are you sure you want to delete this wedding?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        var menuList = _menuService.GetByBookingId(SelectedItem.BookingId).ToList();
                        foreach (var menu in menuList)
                        {
                            _menuService.Delete(SelectedItem.BookingId, menu.DishId);
                        }
                        
                        var serviceList = _serviceDetailService.GetByBookingId(SelectedItem.BookingId).ToList();
                        foreach (var service in serviceList)
                        {
                            _serviceDetailService.Delete(SelectedItem.BookingId, service.ServiceId);
                        }
                        
                        _bookingService.Delete(SelectedItem.BookingId);
                        List.Remove(SelectedItem);
                        RefreshList();
                        MessageBox.Show("Wedding deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    DeleteMessage = $"Delete error: {ex.Message}";
                }
            });
        }

        private void PerformSearch()
        {
            try
            {
                var filtered = OriginalList.AsEnumerable();

                // Filter by ComboBox filters
                if (!string.IsNullOrEmpty(SelectedGroomName))
                    filtered = filtered.Where(x => x.GroomName == SelectedGroomName);
                if (!string.IsNullOrEmpty(SelectedBrideName))
                    filtered = filtered.Where(x => x.BrideName == SelectedBrideName);
                if (!string.IsNullOrEmpty(SelectedHallName))
                    filtered = filtered.Where(x => x.Hall != null && x.Hall.HallName == SelectedHallName);
                if (SelectedWeddingDate.HasValue)
                    filtered = filtered.Where(x => x.WeddingDate.HasValue && x.WeddingDate.Value.Date == SelectedWeddingDate.Value.Date);
                if (SelectedTableCount.HasValue)
                    filtered = filtered.Where(x => x.TableCount == SelectedTableCount);

                // Filter by Status
                if (!string.IsNullOrEmpty(SelectedStatus) && SelectedStatus != "All")
                    filtered = filtered.Where(x => x.Status == SelectedStatus);

                // Search by text
                if (!string.IsNullOrEmpty(SearchText) && !string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    var search = SearchText.Trim().ToLower();
                    switch (SelectedSearchProperty)
                    {
                        case "Groom Name":
                            filtered = filtered.Where(x => !string.IsNullOrEmpty(x.GroomName) && x.GroomName.ToLower().Contains(search));
                            break;
                        case "Bride Name":
                            filtered = filtered.Where(x => !string.IsNullOrEmpty(x.BrideName) && x.BrideName.ToLower().Contains(search));
                            break;
                        case "Hall Name":
                            filtered = filtered.Where(x => x.Hall != null && !string.IsNullOrEmpty(x.Hall.HallName) && x.Hall.HallName.ToLower().Contains(search));
                            break;
                        case "Wedding Date":
                            filtered = filtered.Where(x => x.WeddingDate.HasValue && x.WeddingDate.Value.ToString("dd/MM/yyyy").Contains(search));
                            break;
                        case "Table Count":
                            filtered = filtered.Where(x => x.TableCount.HasValue && x.TableCount.Value.ToString().Contains(search));
                            break;
                        case "Status":
                            filtered = filtered.Where(x => !string.IsNullOrEmpty(x.Status) && x.Status.ToLower().Contains(search));
                            break;
                        default:
                            break;
                    }
                }

                List = new ObservableCollection<BookingDTO>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddWedding()
        {
            var addView = new Presentation.View.AddWeddingView()
            {
                DataContext = Infrastructure.ServiceContainer.CreateAddWeddingViewModel()
            };
            addView.ShowDialog();
            RefreshList();
        }

        private void RefreshList()
        {
            var all = _bookingService.GetAll().ToList();
            List = new ObservableCollection<BookingDTO>(all);
            OriginalList = new ObservableCollection<BookingDTO>(all);
        }

        private void Reset()
        {
            SelectedItem = null;
            SelectedGroomName = null;
            SelectedBrideName = null;
            SelectedHallName = null;
            SelectedWeddingDate = null;
            SelectedTableCount = null;
            SearchText = string.Empty;
            DeleteMessage = string.Empty;
            PerformSearch();
        }
    }
}