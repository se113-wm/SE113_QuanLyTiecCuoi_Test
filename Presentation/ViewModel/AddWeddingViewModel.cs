using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class AddWeddingViewModel : BaseViewModel
    {
        private readonly IHallService _hallService;
        private readonly IShiftService _shiftService;
        private readonly IBookingService _bookingService;
        private readonly IDishService _dishService;
        private readonly IServiceService _serviceService;
        private readonly IMenuService _menuService;
        private readonly IServiceDetailService _serviceDetailService;
        private readonly IParameterService _parameterService;
        
        // Wedding Info
        private string _groomName;
        public string GroomName { get => _groomName; set { _groomName = value; OnPropertyChanged(); } }

        private string _brideName;
        public string BrideName { get => _brideName; set { _brideName = value; OnPropertyChanged(); } }

        private string _phone;
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }

        private DateTime? _weddingDate;
        public DateTime? WeddingDate { get => _weddingDate; set { _weddingDate = value; OnPropertyChanged(); } }

        public ObservableCollection<CalendarDateRange> UnavailableDates { get; set; }

        private DateTime _bookingDate = DateTime.Now;
        public DateTime BookingDate { get => _bookingDate; set { _bookingDate = value; OnPropertyChanged(); } }

        // Shift & Hall
        public ObservableCollection<ShiftDTO> ShiftList { get; set; }
        private ShiftDTO _selectedShift;
        public ShiftDTO SelectedShift { get => _selectedShift; set { _selectedShift = value; OnPropertyChanged(); } }

        public ObservableCollection<HallDTO> HallList { get; set; }
        private HallDTO _selectedHall;
        public HallDTO SelectedHall { get => _selectedHall; set { _selectedHall = value; OnPropertyChanged(); } }

        private string _deposit;
        public string Deposit { get => _deposit; set { _deposit = value; OnPropertyChanged(); } }

        private string _tableCount;
        public string TableCount { get => _tableCount; set { _tableCount = value; OnPropertyChanged(); } }

        private string _reserveTableCount;
        public string ReserveTableCount { get => _reserveTableCount; set { _reserveTableCount = value; OnPropertyChanged(); } }

        // Menu Section
        public ObservableCollection<MenuDTO> MenuList { get; set; } = new ObservableCollection<MenuDTO>();
        private decimal _menuTotal;
        public decimal MenuTotal { get => _menuTotal; set { _menuTotal = value; OnPropertyChanged(); } }

        private MenuDTO _selectedMenuItem;
        public MenuDTO SelectedMenuItem { get => _selectedMenuItem; set { _selectedMenuItem = value; OnPropertyChanged(); LoadMenuDetail(); } }

        public DishDTO Dish { get; set; } = new DishDTO();
        private string _menuQuantity;
        public string MenuQuantity { get => _menuQuantity; set { _menuQuantity = value; OnPropertyChanged(); } }
        private string _menuNote;
        public string MenuNote { get => _menuNote; set { _menuNote = value; OnPropertyChanged(); } }

        // Service Section
        public ObservableCollection<ServiceDetailDTO> ServiceList { get; set; } = new ObservableCollection<ServiceDetailDTO>();
        private decimal _serviceTotal;
        public decimal ServiceTotal { get => _serviceTotal; set { _serviceTotal = value; OnPropertyChanged(); } }
        private ServiceDetailDTO _selectedServiceItem;
        public ServiceDetailDTO SelectedServiceItem { get => _selectedServiceItem; set { _selectedServiceItem = value; OnPropertyChanged(); LoadServiceDetail(); } }

        public ServiceDTO Service { get; set; } = new ServiceDTO();
        private string _serviceQuantity;
        public string ServiceQuantity { get => _serviceQuantity; set { _serviceQuantity = value; OnPropertyChanged(); } }
        private string _serviceNote;
        public string ServiceNote { get => _serviceNote; set { _serviceNote = value; OnPropertyChanged(); } }

        // Commands
        public ICommand ResetWeddingCommand { get; set; }
        public ICommand ResetMenuCommand { get; set; }
        public ICommand ResetServiceCommand { get; set; }
        public ICommand SelectDishCommand { get; set; }
        public ICommand AddMenuCommand { get; set; }
        public ICommand EditMenuCommand { get; set; }
        public ICommand DeleteMenuCommand { get; set; }

        public ICommand SelectServiceCommand { get; set; }
        public ICommand AddServiceCommand { get; set; }
        public ICommand EditServiceCommand { get; set; }
        public ICommand DeleteServiceCommand { get; set; }

        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        // Constructor with Dependency Injection
        public AddWeddingViewModel(
            IHallService hallService,
            IShiftService shiftService,
            IBookingService bookingService,
            IDishService dishService,
            IServiceService serviceService,
            IMenuService menuService,
            IServiceDetailService serviceDetailService,
            IParameterService parameterService)
        {
            // Inject services
            _hallService = hallService;
            _shiftService = shiftService;
            _bookingService = bookingService;
            _dishService = dishService;
            _serviceService = serviceService;
            _menuService = menuService;
            _serviceDetailService = serviceDetailService;
            _parameterService = parameterService;

            UnavailableDates = new ObservableCollection<CalendarDateRange>
            {
                new CalendarDateRange(DateTime.MinValue, DateTime.Today)
            };

            // Load data from services
            ShiftList = new ObservableCollection<ShiftDTO>(_shiftService.GetAll());
            HallList = new ObservableCollection<HallDTO>(_hallService.GetAll());

            // Initialize commands
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ResetWeddingCommand = new RelayCommand<object>((p) => true, (p) => ResetWedding());
            ResetMenuCommand = new RelayCommand<object>((p) => true, (p) => ResetMenu());
            ResetServiceCommand = new RelayCommand<object>((p) => true, (p) => ResetService());

            // Menu commands
            SelectDishCommand = new RelayCommand<object>((p) => true, (p) => SelectDish());
            AddMenuCommand = new RelayCommand<object>((p) => 
            {
                if (Dish == null || string.IsNullOrWhiteSpace(Dish.DishName) || !int.TryParse(MenuQuantity, out int sl) || sl <= 0)
                {
                    return false;
                }
                var existingItem = MenuList.FirstOrDefault(m => m.Dish.DishId == Dish.DishId);
                if (existingItem != null)
                {
                    return false;
                }
                return true;
            }
            , (p) => 
            {
                MenuList.Add(new MenuDTO
                {
                    Dish = Dish,
                    Quantity = int.Parse(MenuQuantity),
                    Note = MenuNote
                });
                MenuQuantity = string.Empty;
                MenuNote = string.Empty;
                Dish = new DishDTO();
                OnPropertyChanged(nameof(Dish));
                MenuTotal = MenuList.Sum(m => (m.Dish.UnitPrice ?? 0) * (m.Quantity ?? 0));
            });
            
            EditMenuCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedMenuItem == null) return false;
                if (Dish == null || string.IsNullOrWhiteSpace(Dish.DishName) || !int.TryParse(MenuQuantity, out int sl) || sl <= 0)
                {
                    return false;
                }
                if (SelectedMenuItem.Dish.DishName == Dish.DishName && SelectedMenuItem.Quantity.ToString() == MenuQuantity && SelectedMenuItem.Note == MenuNote)
                {
                    return false;
                }
                var existingItem = MenuList.FirstOrDefault(m => m.Dish.DishId == Dish.DishId);
                if (existingItem != null && existingItem != SelectedMenuItem)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var newMenuItem = new MenuDTO
                {
                    Dish = Dish,
                    Quantity = int.Parse(MenuQuantity),
                    Note = MenuNote
                };
                int index = MenuList.IndexOf(SelectedMenuItem);
                MenuList[index] = null;
                MenuList[index] = newMenuItem;
                MenuTotal = MenuList.Sum(m => (m.Dish.UnitPrice ?? 0) * (m.Quantity ?? 0));
            });
            
            DeleteMenuCommand = new RelayCommand<object>((p) => SelectedMenuItem != null, (p) =>
            {
                MenuList.Remove(SelectedMenuItem);
                SelectedMenuItem = null;
                MenuTotal = MenuList.Sum(m => (m.Dish.UnitPrice ?? 0) * (m.Quantity ?? 0));
            });
            
            // Service commands
            SelectServiceCommand = new RelayCommand<object>((p) => true, (p) => SelectService());
            AddServiceCommand = new RelayCommand<object>((p) =>
            {
                if (Service == null || string.IsNullOrWhiteSpace(Service.ServiceName) || !int.TryParse(ServiceQuantity, out int sl) || sl <= 0)
                {
                    return false;
                }
                var existingService = ServiceList.FirstOrDefault(s => s.Service.ServiceId == Service.ServiceId);
                if (existingService != null)
                {
                    return false;
                }
                return true;
            }
            , (p) =>
            {
                ServiceList.Add(new ServiceDetailDTO
                {
                    Service = Service,
                    Quantity = int.Parse(ServiceQuantity),
                    Note = ServiceNote
                });
                ServiceQuantity = string.Empty;
                ServiceNote = string.Empty;
                Service = new ServiceDTO();
                OnPropertyChanged(nameof(Service));
                ServiceTotal = ServiceList.Sum(s => (s.Service.UnitPrice ?? 0) * (s.Quantity ?? 0));
            });
            
            EditServiceCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedServiceItem == null) return false;
                if (Service == null || string.IsNullOrWhiteSpace(Service.ServiceName) || !int.TryParse(ServiceQuantity, out int sl) || sl <= 0)
                {
                    return false;
                }
                if (SelectedServiceItem.Service.ServiceName == Service.ServiceName && SelectedServiceItem.Quantity.ToString() == ServiceQuantity && SelectedServiceItem.Note == ServiceNote)
                {
                    return false;
                }
                var existingService = ServiceList.FirstOrDefault(s => s.Service.ServiceId == Service.ServiceId);
                if (existingService != null && existingService != SelectedServiceItem)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var newServiceItem = new ServiceDetailDTO
                {
                    Service = Service,
                    Quantity = int.Parse(ServiceQuantity),
                    Note = ServiceNote
                };
                int index = ServiceList.IndexOf(SelectedServiceItem);
                ServiceList[index] = null;
                ServiceList[index] = newServiceItem;
                ServiceTotal = ServiceList.Sum(s => (s.Service.UnitPrice ?? 0) * (s.Quantity ?? 0));
            });
            
            DeleteServiceCommand = new RelayCommand<object>((p) => SelectedServiceItem != null, (p) => 
            {
                ServiceList.Remove(SelectedServiceItem);
                SelectedServiceItem = null;
                ServiceTotal = ServiceList.Sum(s => (s.Service.UnitPrice ?? 0) * (s.Quantity ?? 0));
            });

            // Confirm/Cancel
            ConfirmCommand = new RelayCommand<Window>((p) => CanConfirm(), (p) =>
            {
                if (string.IsNullOrWhiteSpace(GroomName) || string.IsNullOrWhiteSpace(BrideName) || string.IsNullOrWhiteSpace(Phone) ||
                    WeddingDate == null || SelectedShift == null || SelectedHall == null || string.IsNullOrWhiteSpace(Deposit) ||
                    string.IsNullOrWhiteSpace(TableCount) || string.IsNullOrWhiteSpace(ReserveTableCount) ||
                    MenuList.Count == 0 || ServiceList.Count == 0)
                {
                    MessageBox.Show("Please fill in all required information and select menu, services.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existingWedding = _bookingService.GetAll()
                    .FirstOrDefault(w => w.WeddingDate.HasValue && WeddingDate.HasValue &&
                             w.WeddingDate.Value.Date == WeddingDate.Value.Date &&
                             w.Hall != null && w.Hall.HallId == SelectedHall.HallId);
                if (existingWedding != null)
                {
                    MessageBox.Show("A wedding is already booked for this date and hall. Please choose another date or hall.", "Schedule Conflict", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var minTableRatio = _parameterService.GetByName("TiLeSoBanDatTruocToiThieu")?.Value ?? 0.5m;
                var maxTableCount = _hallService.GetById(SelectedHall.HallId)?.MaxTableCount ?? 0;
                
                if (!int.TryParse(TableCount, out int tableCount) || tableCount <= 0)
                {
                    MessageBox.Show("Table count must be a positive integer.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (tableCount > maxTableCount)
                {
                    MessageBox.Show($"Table count exceeds hall maximum ({maxTableCount}).", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TableCount = maxTableCount.ToString();
                    return;
                }
                if (tableCount < (minTableRatio * maxTableCount))
                {
                    MessageBox.Show($"Table count must be at least {Math.Ceiling(minTableRatio * maxTableCount)} (minimum booking ratio).", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TableCount = Math.Ceiling(minTableRatio * maxTableCount).ToString();
                    return;
                }

                var maxReserveCount = maxTableCount - tableCount;
                if (!int.TryParse(ReserveTableCount, out int reserveCount) || reserveCount < 0 || reserveCount > maxReserveCount)
                {
                    MessageBox.Show($"Reserve table count must be between 0 and {maxReserveCount}.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ReserveTableCount = maxReserveCount.ToString();
                    return;
                }
                
                if (!string.IsNullOrWhiteSpace(Phone) && !System.Text.RegularExpressions.Regex.IsMatch(Phone, @"^(0|\+84)[0-9]{9,10}$"))
                {
                    MessageBox.Show("Phone number must be 10 or 11 digits starting with 0 or +84.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var totalDishPrice = MenuList.Sum(m => (m.Dish.UnitPrice ?? 0) * (m.Quantity ?? 0));
                var totalServicePrice = ServiceList.Sum(s => (s.Service.UnitPrice ?? 0) * (s.Quantity ?? 0));
                var minTablePrice = _hallService.GetById(SelectedHall.HallId)?.HallType.MinTablePrice ?? 0;
                var estimatedTotal = tableCount * (minTablePrice + totalDishPrice) + totalServicePrice;
                var minDepositRatio = _parameterService.GetByName("TiLeTienDatCocToiThieu")?.Value ?? 0.3m;

                var minDeposit = (decimal)Math.Ceiling(minDepositRatio * estimatedTotal);
                var maxDeposit = (decimal)Math.Ceiling(estimatedTotal);

                if (!decimal.TryParse(Deposit, out decimal deposit))
                {
                    MessageBox.Show("Deposit must be a valid number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (deposit < minDeposit)
                {
                    MessageBox.Show($"Deposit must be at least {minDeposit:N0} (minimum ratio).", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Deposit = minDeposit.ToString("N0");
                    return;
                }
                if (deposit > maxDeposit)
                {
                    MessageBox.Show("Deposit cannot exceed estimated total cost.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Deposit = maxDeposit.ToString("N0");
                    return;
                }

                try
                {
                    var booking = new BookingDTO
                    {
                        GroomName = GroomName,
                        BrideName = BrideName,
                        Phone = Phone,
                        WeddingDate = WeddingDate.Value.Date.Add(SelectedShift.StartTime.Value),
                        BookingDate = BookingDate,
                        Shift = SelectedShift,
                        Hall = SelectedHall,
                        TablePrice = SelectedHall.HallType.MinTablePrice,
                        Deposit = deposit,
                        TableCount = tableCount,
                        ReserveTableCount = int.Parse(ReserveTableCount),
                        ShiftId = SelectedShift.ShiftId,
                        HallId = SelectedHall.HallId,
                    };

                    _bookingService.Create(booking);
                    
                    booking = _bookingService.GetAll().LastOrDefault();
                    
                    for (int i = 0; i < MenuList.Count; i++)
                    {
                        var item = MenuList[i];
                        var menu = new MenuDTO
                        {
                            BookingId = booking.BookingId,
                            DishId = item.Dish.DishId,
                            UnitPrice = item.Dish.UnitPrice,
                            Quantity = item.Quantity,
                            Note = item.Note,
                            Dish = item.Dish,
                        };
                        _menuService.Create(menu);
                    }

                    for (int i = 0; i < ServiceList.Count; i++)
                    {
                        var item = ServiceList[i];
                        var serviceDetail = new ServiceDetailDTO
                        {
                            BookingId = booking.BookingId,
                            ServiceId = item.Service.ServiceId,                            
                            UnitPrice = item.Service.UnitPrice,
                            Quantity = item.Quantity,
                            TotalAmount = (item.Service.UnitPrice ?? 0) * (item.Quantity ?? 0),
                            Note = item.Note,
                            Service = item.Service,
                        };
                        _serviceDetailService.Create(serviceDetail);
                    }
                    
                    if (p is Window window)
                    {
                        MessageBox.Show("Wedding booking created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        window.Close();
                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Invalid input data. Please check quantity, deposit, etc.\nDetails: " + ex.Message, "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Error saving data. Please try again.\nDetails: " + ex.Message, "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unknown error occurred: " + ex.Message +
                        (ex.InnerException != null ? "\nDetails: " + ex.InnerException.Message : ""),
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            
            CancelCommand = new RelayCommand<Window>((p) => true, (p) => 
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel?", "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (p is Window window)
                    {
                        window.Close();
                    }
                }
            });
        }

        private void LoadMenuDetail()
        {
            if (SelectedMenuItem != null)
            {
                Dish = SelectedMenuItem.Dish;
                MenuQuantity = SelectedMenuItem.Quantity.ToString();
                MenuNote = SelectedMenuItem.Note;
                OnPropertyChanged(nameof(Dish));
            }
            else
            {
                Dish = new DishDTO();
                MenuQuantity = string.Empty;
                MenuNote = string.Empty;
                OnPropertyChanged(nameof(Dish));
            }
        }

        private void LoadServiceDetail()
        {
            if (SelectedServiceItem != null)
            {
                Service = SelectedServiceItem.Service;
                ServiceQuantity = SelectedServiceItem.Quantity.ToString();
                ServiceNote = SelectedServiceItem.Note;
                OnPropertyChanged(nameof(Service));
            }
            else
            {
                Service = new ServiceDTO();
                ServiceQuantity = string.Empty;
                ServiceNote = string.Empty;
                OnPropertyChanged(nameof(Service));
            }
        }
        
        private void ResetWedding()
        {
            GroomName = string.Empty;
            BrideName = string.Empty;
            Phone = string.Empty;
            WeddingDate = null;
            BookingDate = DateTime.Now;
            SelectedShift = null;
            SelectedHall = null;
            Deposit = string.Empty;
            TableCount = string.Empty;
            ReserveTableCount = string.Empty;
        }
        
        private void ResetMenu()
        {
            Dish = new DishDTO();
            MenuQuantity = string.Empty;
            MenuNote = string.Empty;
            SelectedMenuItem = null;
        }

        private void ResetService()
        {
            Service = new ServiceDTO();
            ServiceQuantity = string.Empty;
            ServiceNote = string.Empty;
            SelectedServiceItem = null;
        }

        private void SelectDish()
        {
            var dishDialog = new MenuItemView();
            var viewModel = Infrastructure.ServiceContainer.GetService<MenuItemViewModel>();
            dishDialog.DataContext = viewModel;
            if (dishDialog.ShowDialog() == true)
            {
                Dish = viewModel.SelectedFood;
                MenuQuantity = "1";
                OnPropertyChanged(nameof(Dish));
            }
        }
        
        private void SelectService()
        {
            var serviceDialog = new ServiceDetailItemView();
            var viewModel = Infrastructure.ServiceContainer.GetService<ServiceDetailItemViewModel>();
            serviceDialog.DataContext = viewModel;
            if (serviceDialog.ShowDialog() == true)
            {
                Service = viewModel.SelectedService;
                ServiceQuantity = "1";
                OnPropertyChanged(nameof(Service));
            }
        }

        private bool CanConfirm()
        {
            return true;
        }
    }
}