using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.Presentation.ViewModel;

namespace QuanLyTiecCuoi.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IBookingService _bookingService;
        private readonly IUserGroupService _userGroupService;
        private readonly IPermissionService _permissionService;
        private readonly IAppUserService _appUserService;
        private readonly IRevenueReportDetailService _revenueReportDetailService;

        #region Commands
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand HomeCommand { get; set; }
        public ICommand HallTypeCommand { get; set; }
        public ICommand HallCommand { get; set; }
        public ICommand ShiftCommand { get; set; }
        public ICommand FoodCommand { get; set; }
        public ICommand ServiceCommand { get; set; }
        public ICommand WeddingCommand { get; set; }
        public ICommand ReportCommand { get; set; }
        public ICommand ParameterCommand { get; set; }
        public ICommand PermissionCommand { get; set; }
        public ICommand UserCommand { get; set; }
        public ICommand AccountCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        #endregion

        public Dictionary<string, Visibility> ButtonVisibilities { get; set; } = new Dictionary<string, Visibility>();
        public Dictionary<string, Brush> ButtonBackgrounds { get; set; } = new Dictionary<string, Brush>();

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        // Constructor with Dependency Injection
        public MainViewModel(
            IBookingService bookingService,
            IUserGroupService userGroupService,
            IPermissionService permissionService,
            IAppUserService appUserService,
            IRevenueReportDetailService revenueReportDetailService)
        {
            _bookingService = bookingService;
            _userGroupService = userGroupService;
            _permissionService = permissionService;
            _appUserService = appUserService;
            _revenueReportDetailService = revenueReportDetailService;
            
            LoadButtonVisibility();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            HomeCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new HomeView()
                {
                    DataContext = new HomeViewModel(this, _bookingService, _revenueReportDetailService)
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Home"] = new SolidColorBrush(Colors.DarkBlue); 
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            HallTypeCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new HallTypeView
                {
                    DataContext = Infrastructure.ServiceContainer.GetService<HallTypeViewModel>()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["HallType"] = new SolidColorBrush(Colors.DarkBlue); 
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            HallCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new HallView()
                {
                    DataContext = Infrastructure.ServiceContainer.GetService<HallViewModel>()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Hall"] = new SolidColorBrush(Colors.DarkBlue); 
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            ShiftCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new ShiftView();
                ResetButtonBackgrounds();
                ButtonBackgrounds["Shift"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            ServiceCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new ServiceView()
                {
                    DataContext = Infrastructure.ServiceContainer.GetService<ServiceViewModel>()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Service"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            WeddingCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                var menuService = Infrastructure.ServiceContainer.GetService<IMenuService>();
                var serviceDetailService = Infrastructure.ServiceContainer.GetService<IServiceDetailService>();
                
                CurrentView = new WeddingView()
                {
                    DataContext = new WeddingViewModel(this, _bookingService, menuService, serviceDetailService)
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Wedding"] = new SolidColorBrush(Colors.DarkBlue); 
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            ReportCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                CurrentView = new ReportView();
                ResetButtonBackgrounds();
                ButtonBackgrounds["Report"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            ParameterCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new ParameterView()
                {
                    DataContext = Infrastructure.ServiceContainer.GetService<ParameterViewModel>()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Parameter"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            PermissionCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
                CurrentView = new PermissionView() {
                    DataContext = new PermissionViewModel(_userGroupService, _permissionService, _appUserService)
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Permission"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            UserCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
                CurrentView = new UserView() {
                    DataContext = Infrastructure.ServiceContainer.GetService<UserViewModel>()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["User"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            AccountCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
                CurrentView = new AccountView() {
                    DataContext = Infrastructure.ServiceContainer.GetService<AccountViewModel>()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Account"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            FoodCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                CurrentView = new FoodView()
                {
                    DataContext = Infrastructure.ServiceContainer.GetService<FoodViewModel>()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Food"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            LogoutCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.DataContext = Infrastructure.ServiceContainer.GetService<LoginViewModel>();
                DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
                loginWindow.Show();
                LoadButtonVisibility();
                DataProvider.Ins.CurrentUser = null;
                p.Close();
            });
        }

        public void SwitchToWeddingTab()
        {
            if (ButtonVisibilities.ContainsKey("Wedding") && ButtonVisibilities["Wedding"] == Visibility.Visible)
            {
                var menuService = Infrastructure.ServiceContainer.GetService<IMenuService>();
                var serviceDetailService = Infrastructure.ServiceContainer.GetService<IServiceDetailService>();
                
                var dataContext = new WeddingViewModel(this, _bookingService, menuService, serviceDetailService);
                CurrentView = new WeddingView()
                {
                    DataContext = dataContext
                };
                
                ResetButtonBackgrounds();
                ButtonBackgrounds["Wedding"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
                dataContext.AddWedding();
            }
        }

        public void SwitchToWeddingDetailTab(int weddingId)
        {
            CurrentView = new WeddingDetailView()
            {
                DataContext = Infrastructure.ServiceContainer.CreateWeddingDetailViewModel(weddingId)
            };
        }

        public void CloseAllViewsExceptCurrent()
        {
            CurrentView = null;
            ResetButtonBackgrounds();
            OnPropertyChanged(nameof(ButtonBackgrounds));
        }

        private void LoadButtonVisibility()
        {
            var userPermissions = DataProvider.Ins.DB.UserGroups
                .Where(group => group.GroupId == DataProvider.Ins.CurrentUser.GroupId)
                .SelectMany(group => group.Permissions);

            var buttonKeys = new List<string> { "Home", "HallType", "Hall", "Shift", "Food", "Service", "Wedding", "Report", "Parameter", "Permission", "User", "Account" };

            foreach (var key in buttonKeys)
            {
                ButtonVisibilities[key] = userPermissions.Any(p => p.PermissionId == key) ? Visibility.Visible : Visibility.Collapsed;
            }

            OnPropertyChanged(nameof(ButtonVisibilities));
            ResetButtonBackgrounds();
            
            CurrentView = new HomeView()
            {
                DataContext = new HomeViewModel(this, _bookingService, _revenueReportDetailService)
            };
            ButtonBackgrounds["Home"] = new SolidColorBrush(Colors.DarkBlue);
            OnPropertyChanged(nameof(ButtonBackgrounds));
        }

        private void ResetButtonBackgrounds()
        {
            foreach (var key in ButtonBackgrounds.Keys.ToList())
            {
                ButtonBackgrounds[key] = new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
