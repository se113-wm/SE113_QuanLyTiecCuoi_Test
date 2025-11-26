using QuanLyTiecCuoi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
//using static MaterialDesignThemes.Wpf.Theme;
using System.Windows.Controls;

namespace QuanLyTiecCuoi.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand UserNameChangedCommand { get; set; }
        public LoginViewModel()
        {
            var a = DataProvider.Ins.DB.NGUOIDUNGs.ToList();
            Password = "";
            UserName = "";
            // username: Fartiel; pass: admin
            // username: Neith; pass: staff
            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Login(p); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            UserNameChangedCommand = new RelayCommand<TextBox>((p) => { return true; }, (p) => { UserName = p.Text; });
        }
        void Login(Window p)
        {
            if (p == null)
                return;
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("Vui lòng nhập tài khoản!");
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                return;
            }
            string passEncode = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(Password));
            var Account = DataProvider.Ins.DB.NGUOIDUNGs
                .Where(x => x.TenDangNhap == UserName && x.MatKhauHash == passEncode);
            if (Account.Count() > 0)
            {
                MessageBox.Show("Đăng nhập thành công!");
                DataProvider.Ins.CurrentUser = Account.First();
                // Gọi Main Window
                MainWindow mainWindow = new MainWindow()
                {
                    DataContext = Infrastructure.ServiceContainer.GetService<MainViewModel>()
                };
                mainWindow.Show();
                p.Close();
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
            }
        }
        
    }
}
