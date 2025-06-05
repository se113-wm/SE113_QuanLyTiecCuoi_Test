using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel {
    public class AccountViewModel : BaseViewModel{
        private readonly INguoiDungService _nguoiDungService;

        private ObservableCollection<NGUOIDUNGDTO> _List;
        public ObservableCollection<NGUOIDUNGDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private string _TenDangNhap;
        public string TenDangNhap { get => _TenDangNhap; set { _TenDangNhap = value; OnPropertyChanged(); } }

        private string _MatKhau;
        public string MatKhau { get => _MatKhau; set { _MatKhau = value; OnPropertyChanged(); } }

        private string _HoTen;
        public string HoTen { get => _HoTen; set { _HoTen = value; OnPropertyChanged(); } }

        private string _Email;
        public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }

        private string _TenNhom;
        public string TenNhom { get => _TenNhom; set { _TenNhom = value; OnPropertyChanged(); } }
        public ICommand SaveCommand { get; set; }
        private string _SaveMessage;
        public string SaveMessage { get => _SaveMessage; set { _SaveMessage = value; OnPropertyChanged(); } }

        public AccountViewModel() {
            var currentUser = DataProvider.Ins.CurrentUser;
            _nguoiDungService = new NguoiDungService();
            List = new ObservableCollection<NGUOIDUNGDTO>(_nguoiDungService.GetAll().ToList());

            TenDangNhap = currentUser.TenDangNhap;
            MatKhau = "";
            HoTen = currentUser.HoTen;
            Email = currentUser.Email;
            TenNhom = currentUser.NHOMNGUOIDUNG.TenNhom;
                
            OnPropertyChanged();

            SaveCommand = new RelayCommand<object>((p) => {
                if (TenDangNhap == currentUser.TenDangNhap && HoTen == currentUser.HoTen && Email == currentUser.Email && string.IsNullOrWhiteSpace(MatKhau)) {
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
                if (string.IsNullOrWhiteSpace(Email)) {
                    SaveMessage = "Email không được để trống";
                    return false;
                }
                SaveMessage = string.Empty;
                return true;
            }, (p) => {
                try {
                    var updateDto = new NGUOIDUNGDTO() {
                        MaNguoiDung = currentUser.MaNguoiDung,
                        TenDangNhap = TenDangNhap,
                        HoTen = HoTen,
                        Email = Email,
                        MaNhom = currentUser.MaNhom,
                        NhomNguoiDung = _nguoiDungService.GetById(currentUser.MaNguoiDung).NhomNguoiDung
                    };
                    if (!string.IsNullOrWhiteSpace(MatKhau)) {
                        updateDto.MatKhauHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(MatKhau));
                    }
                    else {
                        updateDto.MatKhauHash = currentUser.MatKhauHash;
                    }
                    _nguoiDungService.Update(updateDto);

                    MessageBox.Show("Cập nhật thông tin thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
    }
}
