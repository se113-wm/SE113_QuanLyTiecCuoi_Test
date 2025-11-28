using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class WeddingDetailViewModel : BaseViewModel
    {
        // Services
        private readonly IHallService _hallService;
        private readonly IShiftService _shiftService;
        private readonly IBookingService _bookingService;
        private readonly IDishService _dishService;
        private readonly IServiceService _serviceService;
        private readonly IMenuService _menuService;
        private readonly IServiceDetailService _serviceDetailService;
        private readonly IParameterService _parameterService;

        private int _bookingId;

        // Constructor with Dependency Injection
        public WeddingDetailViewModel(
            int bookingId,
            IHallService hallService,
            IShiftService shiftService,
            IBookingService bookingService,
            IDishService dishService,
            IServiceService serviceService,
            IMenuService menuService,
            IServiceDetailService serviceDetailService,
            IParameterService parameterService)
        {
            _hallService = hallService;
            _shiftService = shiftService;
            _bookingService = bookingService;
            _dishService = dishService;
            _serviceService = serviceService;
            _menuService = menuService;
            _serviceDetailService = serviceDetailService;
            _parameterService = parameterService;

            _bookingId = bookingId;

            ShiftList = new ObservableCollection<ShiftDTO>(_shiftService.GetAll());
            HallList = new ObservableCollection<HallDTO>(_hallService.GetAll());

            if (bookingId > 0)
            {
                LoadWeddingDetail(bookingId);
            }

            var from = new DateTime(2000, 1, 1);
            var to = DateTime.Today;

            if (from > to)
            {
                to = from;
            }
            UnavailableDates = new ObservableCollection<CalendarDateRange>();

            InitializeCommands();
        }

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

        private ObservableCollection<HallDTO> _hallList;
        public ObservableCollection<HallDTO> HallList
        {
            get => _hallList;
            set
            {
                if (_hallList != value)
                {
                    _hallList = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private HallDTO _selectedHall;
        public HallDTO SelectedHall { get => _selectedHall; set { _selectedHall = value; OnPropertyChanged(); } }

        private string _deposit;
        public string Deposit { get => _deposit; set { _deposit = value; OnPropertyChanged(); } }

        private string _tableCount;
        public string TableCount { get => _tableCount; set { _tableCount = value; OnPropertyChanged(); } }

        private string _reserveTableCount;
        public string ReserveTableCount { get => _reserveTableCount; set { _reserveTableCount = value; OnPropertyChanged(); } }

        // Edit mode
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (value == _isEditing) return;

                if (value)
                {
                    if (CurrentWedding?.PaymentDate != null)
                    {
                        MessageBox.Show("Tiệc cưới đã thanh toán, không thể chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (WeddingDate != null && DateTime.Now.Date >= WeddingDate.Value.Date)
                    {
                        MessageBox.Show("Đã qua ngày đãi tiệc, không thể chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        public BookingDTO CurrentWedding => _bookingService.GetById(_bookingId);

        // Menu Section
        private ObservableCollection<MenuDTO> _menuList; 
        public ObservableCollection<MenuDTO> MenuList
        {
            get => _menuList; 
            set
            {
                if (_menuList != value)
                {
                    if (_menuList != null) _menuList.CollectionChanged -= MenuList_CollectionChanged;
                    _menuList = value;
                    if (_menuList != null)
                        _menuList.CollectionChanged += MenuList_CollectionChanged;

                    UpdateMenuTotal();
                    OnPropertyChanged();
                }
            }
        }

        private decimal _menuTotal;
        public decimal MenuTotal
        {
            get => _menuTotal;
            set
            {
                if (_menuTotal != value)
                {
                    _menuTotal = value;
                    OnPropertyChanged();
                }
            }
        }

        private void MenuList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MenuDTO item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged notifyItem)
                        notifyItem.PropertyChanged += MenuItem_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (MenuDTO item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged notifyItem)
                        notifyItem.PropertyChanged -= MenuItem_PropertyChanged;
                }
            }
            UpdateMenuTotal();
        }

        private void MenuItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MenuDTO.Quantity) || e.PropertyName == nameof(MenuDTO.UnitPrice))
                UpdateMenuTotal();
        }

        private void UpdateMenuTotal()
        {
            MenuTotal = MenuList?.Where(item => item != null).Sum(item => (item.Quantity ?? 0) * (item.UnitPrice ?? 0)) ?? 0;
        }

        private MenuDTO _selectedMenuItem;
        public MenuDTO SelectedMenuItem { get => _selectedMenuItem; set { _selectedMenuItem = value; OnPropertyChanged(); LoadMenuDetail(); } }

        public DishDTO Dish { get; set; } = new DishDTO();

        private string _menuUnitPrice;
        public string MenuUnitPrice { get => _menuUnitPrice; set { _menuUnitPrice = value; OnPropertyChanged(); } }

        private string _menuQuantity;
        public string MenuQuantity { get => _menuQuantity; set { _menuQuantity = value; OnPropertyChanged(); } }
        
        private string _menuNote;
        public string MenuNote { get => _menuNote; set { _menuNote = value; OnPropertyChanged(); } }

        // Service Section
        private ObservableCollection<ServiceDetailDTO> _serviceList;
        public ObservableCollection<ServiceDetailDTO> ServiceList
        {
            get => _serviceList;
            set
            {
                if (_serviceList != value)
                {
                    if (_serviceList != null)
                        _serviceList.CollectionChanged -= ServiceList_CollectionChanged;
                    _serviceList = value;
                    if (_serviceList != null)
                        _serviceList.CollectionChanged += ServiceList_CollectionChanged;

                    UpdateServiceTotal();
                    OnPropertyChanged();
                }
            }
        }

        private decimal _serviceTotal;
        public decimal ServiceTotal
        {
            get => _serviceTotal;
            set
            {
                if (_serviceTotal != value)
                {
                    _serviceTotal = value;
                    OnPropertyChanged();
                }
            }
        }

        private void ServiceList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ServiceDetailDTO item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged notifyItem)
                        notifyItem.PropertyChanged += ServiceItem_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (ServiceDetailDTO item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged notifyItem)
                        notifyItem.PropertyChanged -= ServiceItem_PropertyChanged;
                }
            }
            UpdateServiceTotal();
        }

        private void ServiceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ServiceDetailDTO.Quantity) || e.PropertyName == nameof(ServiceDetailDTO.UnitPrice))
                UpdateServiceTotal();
        }

        private void UpdateServiceTotal()
        {
            ServiceTotal = ServiceList?.Where(item => item != null)
                .Sum(item => (item.Quantity ?? 0) * (item.UnitPrice ?? 0)) ?? 0;
        }

        private ServiceDetailDTO _selectedServiceItem;
        public ServiceDetailDTO SelectedServiceItem { get => _selectedServiceItem; set { _selectedServiceItem = value; OnPropertyChanged(); LoadServiceDetail(); } }

        public ServiceDTO Service { get; set; } = new ServiceDTO();

        private string _serviceUnitPrice;
        public string ServiceUnitPrice { get => _serviceUnitPrice; set { _serviceUnitPrice = value; OnPropertyChanged(); } }

        private string _serviceQuantity;
        public string ServiceQuantity { get => _serviceQuantity; set { _serviceQuantity = value; OnPropertyChanged(); } }
        
        private string _serviceNote;
        public string ServiceNote { get => _serviceNote; set { _serviceNote = value; OnPropertyChanged(); } }

        // Commands
        public ICommand ResetWeddingCommand { get; set; }
        public ICommand ConfirmEditCommand { get; set; }
        public ICommand CancelEditCommand { get; set; }
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
        public ICommand ShowInvoiceCommand { get; set; }

        private void InitializeCommands()
        {
            ResetWeddingCommand = new RelayCommand<object>((p) => true, (p) => 
            {
                var wedding = _bookingService.GetById(_bookingId);
                if (wedding != null)
                {
                    GroomName = wedding.GroomName;
                    BrideName = wedding.BrideName;
                    Phone = wedding.Phone;
                    WeddingDate = wedding.WeddingDate;
                    BookingDate = (DateTime)wedding.BookingDate;
                    SelectedShift = ShiftList.FirstOrDefault(c => c.ShiftId == wedding.ShiftId);
                    SelectedHall = HallList.FirstOrDefault(s => s.HallId == wedding.HallId);
                    Deposit = wedding.Deposit.ToString();
                    TableCount = wedding.TableCount.ToString();
                    ReserveTableCount = wedding.ReserveTableCount.ToString();
                    
                    MenuList = new ObservableCollection<MenuDTO>(_menuService.GetByBookingId(_bookingId));
                    ServiceList = new ObservableCollection<ServiceDetailDTO>(_serviceDetailService.GetByBookingId(_bookingId));
                }
            });
            
            ConfirmEditCommand = new RelayCommand<object>((p) => IsEditing, (p) => ConfirmEdit());
            CancelEditCommand = new RelayCommand<object>((p) => IsEditing, (p) => CancelEdit());
            ResetMenuCommand = new RelayCommand<object>((p) => true, (p) => ResetMenu());
            ResetServiceCommand = new RelayCommand<object>((p) => true, (p) => ResetService());

            SelectDishCommand = new RelayCommand<object>((p) => IsEditing, (p) => SelectDish());
            
            AddMenuCommand = new RelayCommand<object>((p) =>
            {
                if (!IsEditing) return false;
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
                    DishId = Dish.DishId,
                    BookingId = _bookingId,
                    Dish = Dish,
                    UnitPrice = Dish.UnitPrice,
                    Quantity = int.Parse(MenuQuantity),
                    Note = MenuNote
                });
                MenuQuantity = string.Empty;
                MenuNote = string.Empty;
                MenuUnitPrice = string.Empty;
                Dish = new DishDTO();
                OnPropertyChanged(nameof(Dish));
            });

            EditMenuCommand = new RelayCommand<object>((p) =>
            {
                if (!IsEditing) return false;
                if (SelectedMenuItem == null) return false;
                if (Dish == null || string.IsNullOrWhiteSpace(Dish.DishName) || !int.TryParse(MenuQuantity, out int sl) || sl <= 0)
                {
                    return false;
                }
                if (
                    SelectedMenuItem.Dish.DishName == Dish.DishName &&
                    SelectedMenuItem.Quantity?.ToString() == MenuQuantity &&
                    SelectedMenuItem.Note == MenuNote &&
                    (
                        (SelectedMenuItem.UnitPrice ?? 0) == (decimal.TryParse(MenuUnitPrice.Replace(",", "").Replace(".", ""), out var unitPrice) ? unitPrice : 0)
                    )
                )
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
                if (SelectedMenuItem.Dish.DishId == Dish.DishId && SelectedMenuItem.UnitPrice != Dish.UnitPrice)
                {
                    var result = MessageBox.Show($"Món ăn {Dish.DishName} đã có trong thực đơn với đơn giá {SelectedMenuItem.UnitPrice:N0}. Bạn có muốn cập nhật đơn giá mới là {Dish.UnitPrice:N0} không?", "Cập nhật đơn giá", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                var newMenuItem = new MenuDTO
                {
                    BookingId = _bookingId,
                    DishId = Dish.DishId,
                    Dish = Dish,
                    UnitPrice = Dish.UnitPrice,
                    Quantity = int.Parse(MenuQuantity),
                    Note = MenuNote
                };
                int index = MenuList.IndexOf(SelectedMenuItem);
                MenuList[index] = newMenuItem;
            });

            DeleteMenuCommand = new RelayCommand<object>((p) => SelectedMenuItem != null, (p) =>
            {
                MenuList.Remove(SelectedMenuItem);
                SelectedMenuItem = null;
            });
            
            SelectServiceCommand = new RelayCommand<object>((p) => true, (p) => SelectService());
            
            AddServiceCommand = new RelayCommand<Object>((p) =>
            {
                if (!IsEditing) return false;
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
                    BookingId = _bookingId,
                    ServiceId = Service.ServiceId,
                    Service = Service,
                    UnitPrice = Service.UnitPrice,
                    Quantity = int.Parse(ServiceQuantity),
                    Note = ServiceNote
                });
                ServiceQuantity = string.Empty;
                ServiceNote = string.Empty;
                ServiceUnitPrice = string.Empty;
                Service = new ServiceDTO();
                OnPropertyChanged(nameof(Service));
            });

            EditServiceCommand = new RelayCommand<object>((p) =>
            {
                if (!IsEditing) return false;
                if (SelectedServiceItem == null) return false;
                if (Service == null || string.IsNullOrWhiteSpace(Service.ServiceName) || !int.TryParse(ServiceQuantity, out int sl) || sl <= 0)
                {
                    return false;
                }
                if (
                    SelectedServiceItem.Service.ServiceName == Service.ServiceName &&
                    SelectedServiceItem.Quantity?.ToString() == ServiceQuantity &&
                    SelectedServiceItem.Note == ServiceNote &&
                    (
                        (SelectedServiceItem.UnitPrice ?? 0) == (decimal.TryParse(ServiceUnitPrice.Replace(",", "").Replace(".", ""), out var unitPrice) ? unitPrice : 0)
                    )
                )
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
                if (SelectedServiceItem.Service.ServiceId == Service.ServiceId && SelectedServiceItem.UnitPrice != Service.UnitPrice)
                {
                    var result = MessageBox.Show($"Dịch vụ {Service.ServiceName} đã có trong danh sách với đơn giá {SelectedServiceItem.UnitPrice:N0}. Bạn có muốn cập nhật đơn giá mới là {Service.UnitPrice:N0} không?", "Cập nhật đơn giá", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                var newServiceItem = new ServiceDetailDTO
                {
                    BookingId = _bookingId,
                    ServiceId = Service.ServiceId,
                    Service = Service,
                    UnitPrice = Service.UnitPrice,
                    Quantity = int.Parse(ServiceQuantity),
                    Note = ServiceNote
                };
                int index = ServiceList.IndexOf(SelectedServiceItem);
                ServiceList[index] = newServiceItem;
            });

            DeleteServiceCommand = new RelayCommand<object>((p) =>
            {
                return SelectedServiceItem != null;
            }
            , (p) =>
            {
                ServiceList.Remove(SelectedServiceItem);
                SelectedServiceItem = null;
            });

            ShowInvoiceCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                var invoiceView = new InvoiceView() {
                    DataContext = Infrastructure.ServiceContainer.CreateInvoiceViewModel(_bookingId)
                };
                invoiceView.ShowDialog();
                    
                IsEditing = false;
                LoadWeddingDetail(_bookingId);
            });
        }

        private void LoadWeddingDetail(int bookingId)
        {
            var wedding = _bookingService.GetById(bookingId);
            if (wedding != null)
            {
                GroomName = wedding.GroomName;
                BrideName = wedding.BrideName;
                Phone = wedding.Phone;
                WeddingDate = wedding.WeddingDate;
                BookingDate = (DateTime)wedding.BookingDate;
                SelectedShift = ShiftList.FirstOrDefault(c => c.ShiftId == wedding.ShiftId);
                HallList = new ObservableCollection<HallDTO>(_hallService.GetAll());
                
                var hall = HallList.FirstOrDefault(s => s.HallId == wedding.HallId);
                if (hall.HallType.MinTablePrice != wedding.TablePrice)
                {
                    var oldHall = new HallDTO
                    {
                        HallId = hall.HallId,
                        HallName = hall.HallName + " (old price)",
                        HallType = new HallTypeDTO
                        {
                            HallTypeId = hall.HallType.HallTypeId,
                            HallTypeName = hall.HallType.HallTypeName,
                            MinTablePrice = wedding.TablePrice
                        },
                        MaxTableCount = hall.MaxTableCount,
                    };
                    HallList.Insert(0, oldHall);
                    SelectedHall = oldHall;
                }
                else
                {
                    SelectedHall = hall;
                }
                Deposit = wedding.Deposit.ToString();
                TableCount = wedding.TableCount.ToString();
                ReserveTableCount = wedding.ReserveTableCount.ToString();
                
                MenuList = new ObservableCollection<MenuDTO>(_menuService.GetByBookingId(bookingId));

                var allDishes = _dishService.GetAll().ToList();
                foreach (var menu in MenuList)
                {
                    if (menu.Dish == null)
                        menu.Dish = allDishes.FirstOrDefault(m => m.DishId == menu.DishId);
                }
                
                ServiceList = new ObservableCollection<ServiceDetailDTO>(_serviceDetailService.GetByBookingId(bookingId));

                var allServices = _serviceService.GetAll().ToList();
                foreach (var service in ServiceList)
                {
                    if (service.Service == null)
                        service.Service = allServices.FirstOrDefault(d => d.ServiceId == service.ServiceId);
                }
            }
        }

        private void LoadMenuDetail()
        {
            if (SelectedMenuItem != null)
            {
                Dish = SelectedMenuItem.Dish;
                MenuUnitPrice = SelectedMenuItem.UnitPrice?.ToString("N0");
                MenuQuantity = SelectedMenuItem.Quantity?.ToString();
                MenuNote = SelectedMenuItem.Note;
                OnPropertyChanged(nameof(Dish));
            }
            else
            {
                Dish = new DishDTO();
                MenuUnitPrice = string.Empty;
                MenuQuantity = string.Empty;
                MenuNote = string.Empty;
                OnPropertyChanged(nameof(Dish));
            }
        }

        private void ResetMenu()
        {
            Dish = new DishDTO();
            MenuUnitPrice = string.Empty;
            MenuQuantity = string.Empty;
            MenuNote = string.Empty;
            SelectedMenuItem = null;
            OnPropertyChanged(nameof(Dish));
        }

        private void SelectDish()
        {
            var dishDialog = new MenuItemView();
            var viewModel = Infrastructure.ServiceContainer.GetService<MenuItemViewModel>();
            dishDialog.DataContext = viewModel;
            if (dishDialog.ShowDialog() == true)
            {
                Dish = viewModel.SelectedFood;
                MenuUnitPrice = Dish.UnitPrice?.ToString("N0");
                MenuQuantity = "1";
                OnPropertyChanged(nameof(Dish));
            }
        }

        private void LoadServiceDetail()
        {
            if (SelectedServiceItem != null)
            {
                Service = SelectedServiceItem.Service;
                ServiceUnitPrice = SelectedServiceItem.UnitPrice?.ToString("N0");
                ServiceQuantity = SelectedServiceItem.Quantity?.ToString();
                ServiceNote = SelectedServiceItem.Note;
                OnPropertyChanged(nameof(Service));
            }
            else
            {
                Service = new ServiceDTO();
                ServiceUnitPrice = string.Empty;
                ServiceQuantity = string.Empty;
                ServiceNote = string.Empty;
                OnPropertyChanged(nameof(Service));
            }
        }

        private void ResetService()
        {
            Service = new ServiceDTO();
            ServiceUnitPrice = string.Empty;
            ServiceQuantity = string.Empty;
            ServiceNote = string.Empty;
            SelectedServiceItem = null;
            OnPropertyChanged(nameof(Service));
        }

        private void SelectService()
        {
            var serviceDialog = new ServiceDetailItemView();
            var viewModel = Infrastructure.ServiceContainer.GetService<ServiceDetailItemViewModel>();
            serviceDialog.DataContext = viewModel;
            if (serviceDialog.ShowDialog() == true)
            {
                Service = viewModel.SelectedService;
                ServiceUnitPrice = Service.UnitPrice?.ToString("N0");
                ServiceQuantity = "1";
                OnPropertyChanged(nameof(Service));
            }
        }

        private void ConfirmEdit()
        {
            // 1. Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(GroomName) || string.IsNullOrWhiteSpace(BrideName) || string.IsNullOrWhiteSpace(Phone) ||
                WeddingDate == null || SelectedShift == null || SelectedHall == null || string.IsNullOrWhiteSpace(Deposit) ||
                string.IsNullOrWhiteSpace(TableCount) || string.IsNullOrWhiteSpace(ReserveTableCount) ||
                MenuList.Count == 0 || ServiceList.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc và chọn thực đơn, dịch vụ.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (WeddingDate.Value.Date < DateTime.Today.AddDays(1))
            {
                MessageBox.Show("Ngày đãi tiệc phải là ngày mai hoặc ngày sau đó.", "Lỗi ngày", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existingWedding = _bookingService.GetById(_bookingId);
            var existingMenu = _menuService.GetByBookingId(_bookingId);
            var existingServices = _serviceDetailService.GetByBookingId(_bookingId);
            
            var updatedMenu = MenuList.Select(m => new MenuDTO
            {
                BookingId = _bookingId,
                Dish = m.Dish,
                UnitPrice = m.UnitPrice,
                Quantity = m.Quantity,
                Note = m.Note
            }).ToList();

            var updatedServices = ServiceList.Select(s => new ServiceDetailDTO
            {
                BookingId = _bookingId,
                UnitPrice = s.UnitPrice,
                Service = s.Service,
                Quantity = s.Quantity,
                Note = s.Note
            }).ToList();

            if (existingWedding != null &&
                existingWedding.GroomName == GroomName &&
                existingWedding.BrideName == BrideName &&
                existingWedding.Phone == Phone &&
                existingWedding.WeddingDate == WeddingDate &&
                existingWedding.BookingDate.Value.Date == BookingDate.Date &&
                existingWedding.ShiftId == SelectedShift.ShiftId &&
                SelectedHall.HallId == existingWedding.HallId &&
                SelectedHall.HallType.MinTablePrice == existingWedding.TablePrice &&
                existingWedding.Deposit.ToString() == Deposit &&
                existingWedding.TableCount.ToString() == TableCount &&
                existingWedding.ReserveTableCount.ToString() == ReserveTableCount &&
                updatedMenu.SequenceEqual(existingMenu) &&
                updatedServices.SequenceEqual(existingServices))
            {
                MessageBox.Show("Không có thay đổi nào để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 2. Kiểm tra trùng ngày, sảnh (trừ chính mình)
            var duplicateWedding = _bookingService.GetAll()
                .FirstOrDefault(w => w.BookingId != _bookingId &&
                                     w.WeddingDate.HasValue && WeddingDate.HasValue &&
                                     w.WeddingDate.Value.Date == WeddingDate.Value.Date &&
                                     w.HallId == SelectedHall.HallId);
            if (duplicateWedding != null)
            {
                MessageBox.Show("Đã có tiệc cưới được đặt cho ngày và sảnh này. Vui lòng chọn ngày hoặc sảnh khác.", "Trùng lịch", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Kiểm tra số lượng bàn hợp lệ
            var minTableRatio = _parameterService.GetByName("MinReserveTableRate")?.Value ?? 0.5m;
            var maxTableCount = _hallService.GetById(SelectedHall.HallId)?.MaxTableCount ?? 0;
            
            if (!int.TryParse(TableCount, out int tableCount) || tableCount <= 0)
            {
                MessageBox.Show("Số lượng bàn phải là số nguyên dương.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (tableCount > maxTableCount)
            {
                MessageBox.Show($"Số lượng bàn vượt quá số lượng tối đa của sảnh ({maxTableCount}).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                TableCount = maxTableCount.ToString();
                return;
            }
            if (tableCount < (minTableRatio * maxTableCount))
            {
                MessageBox.Show($"Số lượng bàn phải lớn hơn hoặc bằng {Math.Ceiling(minTableRatio * maxTableCount)} (tỉ lệ đặt trước tối thiểu).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                TableCount = Math.Ceiling(minTableRatio * maxTableCount).ToString();
                return;
            }

            // 4. Kiểm tra số lượng bàn dự trữ hợp lệ
            var maxReserveCount = maxTableCount - tableCount;
            if (!int.TryParse(ReserveTableCount, out int reserveCount) || reserveCount < 0 || reserveCount > maxReserveCount)
            {
                MessageBox.Show($"Số bàn dự trữ phải là số nguyên dương và không vượt quá {maxReserveCount}.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                ReserveTableCount = maxReserveCount.ToString();
                return;
            }

            if (!string.IsNullOrWhiteSpace(Phone) && !System.Text.RegularExpressions.Regex.IsMatch(Phone, @"^(0|\+84)[0-9]{9,10}$"))
            {
                MessageBox.Show("Số điện thoại phải là 10 hoặc 11 chữ số và bắt đầu bằng 0 hoặc +84.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                Phone = "0123456789";
                return;
            }

            // 5. Kiểm tra tiền đặt cọc hợp lệ
            var totalDishPrice = MenuList.Sum(m => (m.UnitPrice ?? 0) * (m.Quantity ?? 0));
            var totalServicePrice = ServiceList.Sum(s => (s.UnitPrice ?? 0) * (s.Quantity ?? 0));
            var minTablePrice = existingWedding.TablePrice ?? 0;
            var estimatedTotal = tableCount * (minTablePrice + totalDishPrice) + totalServicePrice;
            var minDepositRatio = _parameterService.GetByName("MinDepositRate")?.Value ?? 0.3m;

            var minDeposit = (decimal)Math.Ceiling(minDepositRatio * estimatedTotal);
            var maxDeposit = (decimal)Math.Ceiling(estimatedTotal);

            if (!decimal.TryParse(Deposit, out decimal deposit))
            {
                MessageBox.Show("Tiền đặt cọc phải là số hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (deposit < minDeposit)
            {
                MessageBox.Show($"Tiền đặt cọc phải lớn hơn hoặc bằng {minDeposit:N0} (tỉ lệ tối thiểu).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                Deposit = minDeposit.ToString("N0");
                return;
            }
            if (deposit > maxDeposit)
            {
                MessageBox.Show("Tiền đặt cọc không được vượt quá tổng chi phí ước tính.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                Deposit = maxDeposit.ToString("N0");
                return;
            }

            // 6. Lưu dữ liệu
            try
            {
                var booking = _bookingService.GetById(_bookingId);
                if (booking == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin tiệc cưới để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                booking.GroomName = GroomName;
                booking.BrideName = BrideName;
                booking.Phone = Phone;
                booking.WeddingDate = WeddingDate.Value.Date.Add(SelectedShift.StartTime.Value);
                booking.BookingDate = BookingDate;
                booking.ShiftId = SelectedShift.ShiftId;
                booking.HallId = SelectedHall.HallId;
                booking.TablePrice = SelectedHall.HallType.MinTablePrice;
                booking.Deposit = deposit;
                booking.TableCount = tableCount;
                booking.ReserveTableCount = reserveCount;

                _bookingService.Update(booking);

                var oldMenus = _menuService.GetByBookingId(_bookingId).ToList();
                foreach (var old in oldMenus)
                    _menuService.Delete(_bookingId, old.DishId);
                foreach (var item in MenuList)
                {
                    var menu = new MenuDTO
                    {
                        BookingId = _bookingId,
                        DishId = item.Dish.DishId,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        Note = item.Note,
                        Dish = item.Dish
                    };
                    _menuService.Create(menu);
                }

                var oldServices = _serviceDetailService.GetByBookingId(_bookingId).ToList();
                foreach (var old in oldServices)
                    _serviceDetailService.Delete(_bookingId, old.ServiceId);
                foreach (var item in ServiceList)
                {
                    var serviceDetail = new ServiceDetailDTO
                    {
                        BookingId = _bookingId,
                        ServiceId = item.Service.ServiceId,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        TotalAmount = (item.UnitPrice ?? 0) * (item.Quantity ?? 0),
                        Note = item.Note,
                        Service = item.Service
                    };
                    _serviceDetailService.Create(serviceDetail);
                }

                IsEditing = false;
                LoadWeddingDetail(_bookingId);
                MessageBox.Show("Cập nhật thông tin tiệc cưới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Dữ liệu nhập vào không hợp lệ. Vui lòng kiểm tra lại số lượng, tiền đặt cọc, ...\nChi tiết: " + ex.Message, "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Có lỗi khi lưu dữ liệu. Vui lòng thử lại.\nChi tiết: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi không xác định: " + ex.Message +
                    (ex.InnerException != null ? "\nChi tiết: " + ex.InnerException.Message : ""),
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
            LoadWeddingDetail(_bookingId);
            MessageBox.Show("Đã hủy chỉnh sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}