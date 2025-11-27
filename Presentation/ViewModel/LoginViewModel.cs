using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace QuanLyTiecCuoi.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAppUserService _appUserService;

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand UsernameChangedCommand { get; set; }

        public LoginViewModel(IAppUserService appUserService)
        {
            _appUserService = appUserService;

            Password = string.Empty;
            Username = string.Empty;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Login(p); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            UsernameChangedCommand = new RelayCommand<TextBox>((p) => { return true; }, (p) => { Username = p.Text; });
        }

        void Login(Window window)
        {
            if (window == null)
                return;

            if (string.IsNullOrEmpty(Username))
            {
                MessageBox.Show("Please enter username!", "Notice", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please enter password!", "Notice", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string passwordHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(Password));
            
            var users = DataProvider.Ins.DB.AppUsers
                .Where(x => x.Username == Username && x.PasswordHash == passwordHash);

            if (users.Count() > 0)
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DataProvider.Ins.CurrentUser = users.First();

                MainWindow mainWindow = new MainWindow()
                {
                    DataContext = Infrastructure.ServiceContainer.GetService<MainViewModel>()
                };
                mainWindow.Show();
                window.Close();
            }
            else
            {
                MessageBox.Show("Incorrect username or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
