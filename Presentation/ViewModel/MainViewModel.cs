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
        private readonly IPhieuDatTiecService _phieuDatTiecService;

        #region command
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

        private object _CurrentView;
        public object CurrentView
        {
            get => _CurrentView;
            set { _CurrentView = value; OnPropertyChanged(); }
        }

        // Constructor với Dependency Injection
        public MainViewModel(IPhieuDatTiecService phieuDatTiecService)
        {
            _phieuDatTiecService = phieuDatTiecService;
            
            LoadButtonVisibility();
            #region Command
            HomeCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new HomeView()
                {
                    DataContext = new HomeViewModel(this, _phieuDatTiecService)
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
                CurrentView = new ShiftView(); // ShiftView sẽ tự lấy ViewModel từ DI Container
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
                var thucDonService = Infrastructure.ServiceContainer.GetService<IThucDonService>();
                var chiTietDVService = Infrastructure.ServiceContainer.GetService<IChiTietDVService>();
                
                CurrentView = new WeddingView()
                {
                    DataContext = new WeddingViewModel(this, _phieuDatTiecService, thucDonService, chiTietDVService)
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Wedding"] = new SolidColorBrush(Colors.DarkBlue); 
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            ReportCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                CurrentView = new ReportView(); // <- ViewModel được binding ở ContentControl
                ResetButtonBackgrounds();
                ButtonBackgrounds["Report"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            //ParameterCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentView = new ParameterView(); });
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
                    DataContext = new PermissionViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Permission"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            
            UserCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
                CurrentView = new UserView() {
                    DataContext = new UserViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["User"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            AccountCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
                CurrentView = new AccountView() {
                    DataContext = new AccountViewModel()
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
                loginWindow.DataContext = new LoginViewModel();
                DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
                loginWindow.Show();
                LoadButtonVisibility();
                DataProvider.Ins.CurrentUser = null;
                p.Close();
            });
            #endregion
        }
        // Chuyển sang tab Đặt tiệc cưới, nếu có quyền
        public void SwitchToWeddingTab()
        {
            if (ButtonVisibilities.ContainsKey("Wedding") && ButtonVisibilities["Wedding"] == Visibility.Visible)
            {
                var thucDonService = Infrastructure.ServiceContainer.GetService<IThucDonService>();
                var chiTietDVService = Infrastructure.ServiceContainer.GetService<IChiTietDVService>();
                
                var dataContext = new WeddingViewModel(this, _phieuDatTiecService, thucDonService, chiTietDVService);
                CurrentView = new WeddingView()
                {
                    DataContext = dataContext
                };
                
                // Đặt màu nền cho nút "Wedding" là màu được chọn
                ResetButtonBackgrounds();
                ButtonBackgrounds["Wedding"] = new SolidColorBrush(Colors.DarkBlue);
                // Gọi OnPropertyChanged để cập nhật giao diện
                OnPropertyChanged(nameof(ButtonBackgrounds));
                dataContext.AddCommandFunc();
            }
        }
        // Chuyển tab sang Chi tiết Đặt tiệc cưới
        public void SwitchToWeddingDetailTab(int weddingId)
        {
            // Sử dụng ServiceContainer để lấy các dependencies và tạo WeddingDetailViewModel
            var sanhService = Infrastructure.ServiceContainer.GetService<ISanhService>();
            var caService = Infrastructure.ServiceContainer.GetService<ICaService>();
            var phieuDatTiecService = Infrastructure.ServiceContainer.GetService<IPhieuDatTiecService>();
            var monAnService = Infrastructure.ServiceContainer.GetService<IMonAnService>();
            var dichVuService = Infrastructure.ServiceContainer.GetService<IDichVuService>();
            var thucDonService = Infrastructure.ServiceContainer.GetService<IThucDonService>();
            var chiTietDVService = Infrastructure.ServiceContainer.GetService<IChiTietDVService>();
            var thamSoService = Infrastructure.ServiceContainer.GetService<IThamSoService>();

            var dataContext = new WeddingDetailViewModel(
                weddingId,
                sanhService,
                caService,
                phieuDatTiecService,
                monAnService,
                dichVuService,
                thucDonService,
                chiTietDVService,
                thamSoService
            );
            
            CurrentView = new WeddingDetailView()
            {
                DataContext = dataContext
            };
        }
        // Tắt tất cả các view và view model khác (trừ main và view model hiện tại)
        public void CloseAllViewsExceptCurrent()
        {
            // Đặt CurrentView về null để tắt tất cả các view khác
            CurrentView = null;
            // Reset màu nền của tất cả các nút
            ResetButtonBackgrounds();
            OnPropertyChanged(nameof(ButtonBackgrounds));
        }


        private void LoadButtonVisibility()
        {
            // Lấy danh sách chức năng của nhóm người dùng hiện tại
            var userPermissions = DataProvider.Ins.DB.NHOMNGUOIDUNGs
                .Where(nhom => nhom.MaNhom == DataProvider.Ins.CurrentUser.MaNhom) // Lọc theo mã nhóm của người dùng hiện tại
                .SelectMany(nhom => nhom.CHUCNANGs); // Lấy danh sách CHUCNANG từ NHOMNGUOIDUNG

            // Thiết lập Visibility dựa trên quyền
            var buttonKeys = new List<string> { "Home", "HallType", "Hall", "Shift", "Food", "Service", "Wedding", "Report", "Parameter", "Permission", "User", "Account" };

            foreach (var key in buttonKeys)
            {
                ButtonVisibilities[key] = userPermissions.Any(cn => cn.MaChucNang == key) ? Visibility.Visible : Visibility.Collapsed;
            }

            // Gọi OnPropertyChanged để cập nhật giao diện
            OnPropertyChanged(nameof(ButtonVisibilities));
            ResetButtonBackgrounds();
            // Đặt màu nền cho nút "Trang chủ" là màu được chọn
            CurrentView = new HomeView()
            {
                DataContext = new HomeViewModel(this, _phieuDatTiecService)
            };
            ButtonBackgrounds["Home"] = new SolidColorBrush(Colors.DarkBlue);
            // Gọi OnPropertyChanged để cập nhật giao diện
            OnPropertyChanged(nameof(ButtonBackgrounds));
        }
        private void ResetButtonBackgrounds()
        {
            foreach (var key in ButtonBackgrounds.Keys.ToList())
            {
                ButtonBackgrounds[key] = new SolidColorBrush(Colors.Transparent); // Màu mặc định
            }
        }

    }
}
