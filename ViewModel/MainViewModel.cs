using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using QuanLyTiecCuoi.Model;


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


        private object _CurrentView;
        public object CurrentView
        {
            get => _CurrentView;
            set { _CurrentView = value; OnPropertyChanged(); }
        }
        public MainViewModel()
        {
            //LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {
            //    if (p == null)
            //        return;
            //    p.Hide();
            //    LoginWindow loginWindow = new LoginWindow();
            //    loginWindow.ShowDialog();

            //    if (loginWindow.DataContext == null)
            //        return;
            //    var loginVM = loginWindow.DataContext as LoginViewModel;
            //    if (loginVM.IsLogin)
            //    {
            //        _CurrentUser = loginVM._CurrentUser;
            //        LoadButtonVisibility();
            //        p.Show();
            //    }
            //    else
            //    {
            //        p.Close();
            //    }
            //}
            //  );
            LoadButtonVisibility();
            #region Command
            HomeCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new HomeView();
                // Đặt màu nền cho nút "Trang chủ" là màu được chọn
                ResetButtonBackgrounds();
                ButtonBackgrounds["Home"] = new SolidColorBrush(Colors.DarkBlue); // Màu khi được chọn

                // Gọi OnPropertyChanged để cập nhật giao diện
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            HallTypeCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                // Giả sử HallTypeView có thuộc tính DataContext
                CurrentView = new HallTypeView
                {
                    DataContext = new HallTypeViewModel()
                };


                // Đặt màu nền cho nút "Trang chủ" là màu được chọn
                ResetButtonBackgrounds();
                ButtonBackgrounds["HallType"] = new SolidColorBrush(Colors.DarkBlue); // Màu khi được chọn

                // Gọi OnPropertyChanged để cập nhật giao diện
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            HallCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new HallView()
                {
                    DataContext = new HallViewModel()
                };

                // Đặt màu nền cho nút "Sảnh" là màu được chọn
                ResetButtonBackgrounds();
                ButtonBackgrounds["Hall"] = new SolidColorBrush(Colors.DarkBlue); // Màu khi được chọn

                // Gọi OnPropertyChanged để cập nhật giao diện
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });

            //ShiftCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentView = new ShiftView(); });
            //HallTypeCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            //{
            //    CurrentView = new FoodView();

            //    // Đặt màu nền cho nút "Trang chủ" là màu được chọn
            //    ResetButtonBackgrounds();
            //    ButtonBackgrounds["HallType"] = new SolidColorBrush(Colors.DarkBlue); // Màu khi được chọn

            //    // Gọi OnPropertyChanged để cập nhật giao diện
            //    OnPropertyChanged(nameof(ButtonBackgrounds));
            //});
            //ServiceCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentView = new ServiceView(); });
            //WeddingCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentView = new WeddingView(); });

            //ReportCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentView = new ReportView(); });
            //ParameterCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentView = new ParameterView(); });
            //PermissionCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentView = new PermissionView(); });
            PermissionCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentView = new PermissionView()
                {
                    DataContext = new PermissionViewModel()
                };

                // Đặt màu nền cho nút "Sảnh" là màu được chọn
                ResetButtonBackgrounds();
                ButtonBackgrounds["Permission"] = new SolidColorBrush(Colors.DarkBlue); // Màu khi được chọn

                // Gọi OnPropertyChanged để cập nhật giao diện
                OnPropertyChanged(nameof(ButtonBackgrounds));
            });
            //UserCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentView = new UserView(); });
            //AccountCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CurrentView = new AccountView(); });
            #endregion

            LogoutCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.DataContext = new LoginViewModel();
                loginWindow.Show();
                LoadButtonVisibility();
                p.Close();
            });
        }
        private void LoadButtonVisibility()
        {
            // Lấy danh sách chức năng của nhóm người dùng hiện tại
            var userPermissions = DataProvider.Ins.DB.NHOMNGUOIDUNGs
                .Where(nhom => nhom.MaNhom == DataProvider.Ins.CurrentUser.MaNhom) // Lọc theo mã nhóm của người dùng hiện tại
                .SelectMany(nhom => nhom.CHUCNANGs) // Lấy danh sách CHUCNANG từ NHOMNGUOIDUNG
                .ToList();

            // Thiết lập Visibility dựa trên quyền
            var buttonKeys = new List<string> { "Home", "HallType", "Hall", "Shift", "Food", "Service", "Wedding", "Report", "Parameter", "Permission", "User" };

            foreach (var key in buttonKeys)
            {
                ButtonVisibilities[key] = userPermissions.Any(cn => cn.MaChucNang == key) ? Visibility.Visible : Visibility.Collapsed;
            }

            // Gọi OnPropertyChanged để cập nhật giao diện
            OnPropertyChanged(nameof(ButtonVisibilities));
            ResetButtonBackgrounds();
            // Đặt màu nền cho nút "Trang chủ" là màu được chọn
            CurrentView = new HomeView();
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
