using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
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
    public class AccountViewModel : BaseViewModel, IDataErrorInfo{
        private readonly INguoiDungService _nguoiDungService;

        private ObservableCollection<NGUOIDUNGDTO> _List;
        public ObservableCollection<NGUOIDUNGDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private string _TenDangNhap;
        public string TenDangNhap { get => _TenDangNhap; set { _TenDangNhap = value; OnPropertyChanged(); } }

        private string _HoTen;
        public string HoTen { get => _HoTen; set { _HoTen = value; OnPropertyChanged(); } }

        private string _Email;
        public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }
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

        private string _TenNhom;
        public string TenNhom { get => _TenNhom; set { _TenNhom = value; OnPropertyChanged(); } }
        public ICommand SaveCommand { get; set; }
        private string _SaveMessage;
        public string SaveMessage { get => _SaveMessage; set { _SaveMessage = value; OnPropertyChanged(); } }
        private string _CurrentPassword;
        public string CurrentPassword { get => _CurrentPassword; set { _CurrentPassword = value; OnPropertyChanged(); } }
        private string _NewPassword;
        public string NewPassword { get => _NewPassword; set { _NewPassword = value; OnPropertyChanged(); } }
        private string _NewPassword1;
        public string NewPassword1 { get => _NewPassword1; set { _NewPassword1 = value; OnPropertyChanged(); } }
        public ICommand ChangePasswordCommand { get; set; }
        //private string _ChangePasswordMessage;
        public string ChangePasswordMessage { get => _SaveMessage; set { _SaveMessage = value; OnPropertyChanged(); } }
        public ICommand CurrentPasswordChangedCommand { get; set; }
        public ICommand NewPasswordChangedCommand { get; set; }
        public ICommand NewPassword1ChangedCommand { get; set; }
        public ICommand ResetPasswordCommand { get; set; }
        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => {
            Reset();
        });
        public AccountViewModel() {
            var currentUser = DataProvider.Ins.CurrentUser;
            _nguoiDungService = new NguoiDungService();
            List = new ObservableCollection<NGUOIDUNGDTO>(_nguoiDungService.GetAll().ToList());

            TenDangNhap = currentUser.TenDangNhap;
            HoTen = currentUser.HoTen;
            Email = currentUser.Email;
            TenNhom = currentUser.NHOMNGUOIDUNG.TenNhom;

            CurrentPassword = "";
            NewPassword = "";
            NewPassword1 = "";
            CurrentPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { CurrentPassword = p.Password; });
            NewPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { NewPassword = p.Password; });
            NewPassword1ChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { NewPassword1 = p.Password; });
            OnPropertyChanged();

            SaveCommand = new RelayCommand<object>((p) => {
                if (TenDangNhap == currentUser.TenDangNhap && HoTen == currentUser.HoTen && Email == currentUser.Email) {
                    SaveMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(TenDangNhap)) {
                    SaveMessage = "Tên đăng nhập không được để trống";
                    return false;
                }
                var exists = List.Any(x =>
                    x.TenDangNhap == TenDangNhap && x.MaNguoiDung != currentUser.MaNguoiDung);
                if (exists) {
                    SaveMessage = "Tên đăng nhập này đã tồn tại";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(HoTen)) {
                    SaveMessage = "Họ tên không được để trống";
                    return false;
                }
                if (!ValidationHelper.IsValidEmail(Email)) {
                    SaveMessage = "Vui lòng nhập đúng định dạng email";
                    return false;
                }
                SaveMessage = string.Empty;
                return true;
            }, (p) => {
                try {
                    var updateDto = new NGUOIDUNGDTO() {
                        MaNguoiDung = currentUser.MaNguoiDung,
                        TenDangNhap = TenDangNhap,
                        MatKhauHash = currentUser.MatKhauHash,
                        HoTen = HoTen,
                        Email = Email,
                        MaNhom = currentUser.MaNhom,
                        NhomNguoiDung = _nguoiDungService.GetById(currentUser.MaNguoiDung).NhomNguoiDung
                    };
                    _nguoiDungService.Update(updateDto);
                    DataProvider.Ins.CurrentUser.Email = Email;
                    DataProvider.Ins.CurrentUser.TenDangNhap = TenDangNhap;
                    DataProvider.Ins.CurrentUser.HoTen = HoTen;

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
                if(string.IsNullOrWhiteSpace(NewPassword1)) {
                    ChangePasswordMessage = "Vui lòng nhập lại mật khẩu mới";
                    return false;
                }
                if (NewPassword != NewPassword1) {
                    ChangePasswordMessage = "Xác nhận mật khẩu không trùng khớp";
                    return false;
                }
                ChangePasswordMessage = "";
                return true;
            }, (p) => {
                try {
                    if (currentUser.MatKhauHash != PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(CurrentPassword))) {
                        MessageBox.Show("Mật khẩu hiện tại không đúng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else {
                        var updateDto = new NGUOIDUNGDTO() {
                            MaNguoiDung = currentUser.MaNguoiDung,
                            TenDangNhap = currentUser.TenDangNhap,
                            MatKhauHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(NewPassword)),
                            HoTen = currentUser.HoTen,
                            Email = currentUser.Email,
                            MaNhom = currentUser.MaNhom,
                            NhomNguoiDung = _nguoiDungService.GetById(currentUser.MaNguoiDung).NhomNguoiDung
                        };
                        _nguoiDungService.Update(updateDto);
                        Reset();
                        MessageBox.Show("Đổi mật khẩu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
        private void Reset() {
            var currentUser = DataProvider.Ins.CurrentUser;
            TenDangNhap = currentUser.TenDangNhap;
            HoTen = currentUser.HoTen;
            Email = currentUser.Email;
            TenNhom = currentUser.NHOMNGUOIDUNG.TenNhom;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            NewPassword1 = string.Empty;
            ChangePasswordMessage = string.Empty;
            OnPropertyChanged();
        }
    }
}
