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
        //public bool IsLogin { get; set; }
        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand UserNameChangedCommand { get; set; }
        //public NGUOIDUNG _CurrentUser { get; set; }
        //public void Reset()
        //{
        //    //IsLogin = false;
        //    UserName = string.Empty;
        //    Password = string.Empty;
        //    _CurrentUser = null;
        //}
        public LoginViewModel()
        {
            var a = DataProvider.Ins.DB.NGUOIDUNGs.First();
            //IsLogin = false;
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
            string passEncode = MD5Hash(Base64Encode(Password));
            var Account = DataProvider.Ins.DB.NGUOIDUNGs
                .ToList()
                .Where(x => x.TenDangNhap == UserName && x.MatKhauHash == passEncode);
            if (Account.Count() > 0)
            {
                //_CurrentUser = Account.First();
                //IsLogin = true;
                MessageBox.Show("Đăng nhập thành công!");
                DataProvider.Ins.CurrentUser = Account.First();
                // Gọi Main Window
                MainWindow mainWindow = new MainWindow();
                mainWindow.DataContext = new MainViewModel();
                mainWindow.Show();
                p.Close();
            }
            else
            {
                //IsLogin = false;
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
