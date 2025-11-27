using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace QuanLyTiecCuoi.ViewModel {
    public class AccountViewModel : BaseViewModel, IDataErrorInfo {
        private readonly IAppUserService _appUserService;

        private ObservableCollection<AppUserDTO> _userList;
        public ObservableCollection<AppUserDTO> UserList {
            get => _userList;
            set { _userList = value; OnPropertyChanged(); }
        }

        private string _username;
        public string Username {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _fullName;
        public string FullName {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string this[string columnName] {
            get {
                if (columnName == nameof(Email)) {
                    if (string.IsNullOrWhiteSpace(Email))
                        return "Email không được để trống";
                    if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        return "Email không đúng định dạng";
                }
                return null;
            }
        }

        public string Error => null;

        private string _groupName;
        public string GroupName {
            get => _groupName;
            set { _groupName = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; set; }

        private string _saveMessage;
        public string SaveMessage {
            get => _saveMessage;
            set { _saveMessage = value; OnPropertyChanged(); }
        }

        private string _currentPassword;
        public string CurrentPassword {
            get => _currentPassword;
            set { _currentPassword = value; OnPropertyChanged(); }
        }

        private string _newPassword;
        public string NewPassword {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        private string _confirmNewPassword;
        public string ConfirmNewPassword {
            get => _confirmNewPassword;
            set { _confirmNewPassword = value; OnPropertyChanged(); }
        }

        public ICommand ChangePasswordCommand { get; set; }

        private string _changePasswordMessage;
        public string ChangePasswordMessage {
            get => _changePasswordMessage;
            set { _changePasswordMessage = value; OnPropertyChanged(); }
        }

        public ICommand CurrentPasswordChangedCommand { get; set; }
        public ICommand NewPasswordChangedCommand { get; set; }
        public ICommand ConfirmNewPasswordChangedCommand { get; set; }
        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => { Reset(); });

        public AccountViewModel(IAppUserService appUserService) {
            _appUserService = appUserService;

            var currentUser = DataProvider.Ins.CurrentUser;
            UserList = new ObservableCollection<AppUserDTO>(_appUserService.GetAll().ToList());

            Username = currentUser.Username;
            FullName = currentUser.FullName;
            Email = currentUser.Email;
            GroupName = currentUser.UserGroup.GroupName;

            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;

            CurrentPasswordChangedCommand = new RelayCommand<PasswordBox>(
                (p) => true,
                (p) => { CurrentPassword = p.Password; });

            NewPasswordChangedCommand = new RelayCommand<PasswordBox>(
                (p) => true,
                (p) => { NewPassword = p.Password; });

            ConfirmNewPasswordChangedCommand = new RelayCommand<PasswordBox>(
                (p) => true,
                (p) => { ConfirmNewPassword = p.Password; });

            OnPropertyChanged();

            SaveCommand = new RelayCommand<object>((p) => {
                if (Username == currentUser.Username && FullName == currentUser.FullName && Email == currentUser.Email) {
                    SaveMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(Username)) {
                    SaveMessage = "Tên đăng nhập không được để trống";
                    return false;
                }
                var exists = UserList.Any(x => x.Username == Username && x.UserId != currentUser.UserId);
                if (exists) {
                    SaveMessage = "Tên đăng nhập này đã tồn tại";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(FullName)) {
                    SaveMessage = "Họ tên không được để trống";
                    return false;
                }
                if (!EmailValidationHelper.IsValidEmail(Email)) {
                    SaveMessage = "Vui lòng nhập đúng định dạng email";
                    return false;
                }
                SaveMessage = string.Empty;
                return true;
            }, (p) => {
                try {
                    var updateDto = new AppUserDTO() {
                        UserId = currentUser.UserId,
                        Username = Username,
                        PasswordHash = currentUser.PasswordHash,
                        FullName = FullName,
                        Email = Email,
                        GroupId = currentUser.GroupId,
                        UserGroup = _appUserService.GetById(currentUser.UserId).UserGroup
                    };
                    _appUserService.Update(updateDto);
                    DataProvider.Ins.CurrentUser.Email = Email;
                    DataProvider.Ins.CurrentUser.Username = Username;
                    DataProvider.Ins.CurrentUser.FullName = FullName;

                    MessageBox.Show("Cập nhật thông tin thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            ChangePasswordCommand = new RelayCommand<object>((p) => {
                if (string.IsNullOrWhiteSpace(CurrentPassword)) {
                    ChangePasswordMessage = "Vui lòng nhập mật khẩu hiện tại";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(NewPassword)) {
                    ChangePasswordMessage = "Vui lòng nhập mật khẩu mới";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(ConfirmNewPassword)) {
                    ChangePasswordMessage = "Vui lòng nhập lại mật khẩu mới";
                    return false;
                }
                if (NewPassword != ConfirmNewPassword) {
                    ChangePasswordMessage = "Xác nhận mật khẩu không trùng khớp";
                    return false;
                }
                ChangePasswordMessage = string.Empty;
                return true;
            }, (p) => {
                try {
                    if (currentUser.PasswordHash != PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(CurrentPassword))) {
                        MessageBox.Show("Mật khẩu hiện tại không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    } else {
                        var updateDto = new AppUserDTO() {
                            UserId = currentUser.UserId,
                            Username = currentUser.Username,
                            PasswordHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(NewPassword)),
                            FullName = currentUser.FullName,
                            Email = currentUser.Email,
                            GroupId = currentUser.GroupId,
                            UserGroup = _appUserService.GetById(currentUser.UserId).UserGroup
                        };
                        _appUserService.Update(updateDto);
                        currentUser.PasswordHash = updateDto.PasswordHash;
                        Reset();
                        MessageBox.Show("Đổi mật khẩu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void Reset() {
            var currentUser = DataProvider.Ins.CurrentUser;
            Username = currentUser.Username;
            FullName = currentUser.FullName;
            Email = currentUser.Email;
            GroupName = currentUser.UserGroup.GroupName;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            ChangePasswordMessage = string.Empty;
            SaveMessage = string.Empty;
            OnPropertyChanged();
        }
    }
}
