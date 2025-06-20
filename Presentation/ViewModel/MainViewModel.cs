using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.Presentation.ViewModel;


namespace QuanLyTiecCuoi.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
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


        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());

        private object _CurrentView;
        public object CurrentView
        {
            get => _CurrentView;
            set { _CurrentView = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            LoadButtonVisibility();
            #region Command
            HomeCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new HomeView()
                {
                    DataContext = new HomeViewModel(this)
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Home"] = new SolidColorBrush(Colors.DarkBlue); 
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            HallTypeCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new HallTypeView
                {
                    DataContext = new HallTypeViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["HallType"] = new SolidColorBrush(Colors.DarkBlue); 
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            HallCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new HallView()
                {
                    DataContext = new HallViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Hall"] = new SolidColorBrush(Colors.DarkBlue); 
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            ShiftCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new ShiftView()
                {
                    DataContext = new ShiftViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Shift"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            ServiceCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new ServiceView()
                {
                    DataContext = new ServiceViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Service"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            WeddingCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new WeddingView()
                {
                    DataContext = new WeddingViewModel(this)
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
                    DataContext = new ParameterViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Parameter"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            PermissionCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new PermissionView() {
                    DataContext = new PermissionViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Permission"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            HallTypeCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                CurrentView = new HallTypeView {
                    DataContext = new HallTypeViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["HallType"] = new SolidColorBrush(Colors.DarkBlue); 
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            UserCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                CurrentView = new UserView() {
                    DataContext = new UserViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["User"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            AccountCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                CurrentView = new AccountView() {
                    DataContext = new AccountViewModel()
                };
                ResetButtonBackgrounds();
                ButtonBackgrounds["Account"] = new SolidColorBrush(Colors.DarkBlue);
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            FoodCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                CurrentView = new FoodView();
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

                var dataContext = new WeddingViewModel(this);
                CurrentView = new WeddingView()
                {
                    DataContext = dataContext
                };

                // Đặt màu nền cho nút "Sảnh" là màu được chọn
                ResetButtonBackgrounds();
                ButtonBackgrounds["Wedding"] = new SolidColorBrush(Colors.DarkBlue); // Màu khi được chọn
                // Gọi OnPropertyChanged để cập nhật giao diện
                OnPropertyChanged(nameof(ButtonBackgrounds));
                //dataContext.AddCommandFunc();
            }
        }
        // Chuyển tab sang Chi tiết Đặt tiệc cưới
        public void SwitchToWeddingDetailTab(int weddingId)
        {
            //if (ButtonVisibilities.ContainsKey("Wedding") && ButtonVisibilities["Wedding"] == Visibility.Visible)
            //{
                var dataContext = new WeddingDetailViewModel(weddingId);
                CurrentView = new WeddingDetailView()
                {
                    DataContext = dataContext
                };
                // Đặt màu nền cho nút "Sảnh" là màu được chọn
            //    ResetButtonBackgrounds();
            //    ButtonBackgrounds["Wedding"] = new SolidColorBrush(Colors.DarkBlue); // Màu khi được chọn
            //    // Gọi OnPropertyChanged để cập nhật giao diện
            //    OnPropertyChanged(nameof(ButtonBackgrounds));
            //}
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
                DataContext = new HomeViewModel(this)
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
