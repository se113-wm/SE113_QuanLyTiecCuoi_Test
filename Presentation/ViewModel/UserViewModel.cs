using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;



namespace QuanLyTiecCuoi.ViewModel {
    public class UserViewModel : BaseViewModel{
        private readonly INguoiDungService _nguoiDungService;
        private readonly INhomNguoiDungService _nhomNguoiDungService;

        private ObservableCollection<NGUOIDUNGDTO> _List;
        public ObservableCollection<NGUOIDUNGDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<NGUOIDUNGDTO> _OriginalList;
        public ObservableCollection<NGUOIDUNGDTO> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private ObservableCollection<NHOMNGUOIDUNGDTO> _UserTypes;
        public ObservableCollection<NHOMNGUOIDUNGDTO> UserTypes { get => _UserTypes; set { _UserTypes = value; OnPropertyChanged(); } }

        private NGUOIDUNGDTO _SelectedItem;
        public NGUOIDUNGDTO SelectedItem {
            get => _SelectedItem;
            set {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null) {
                    TenDangNhap = SelectedItem.TenDangNhap;
                    MatKhau = "";
                    HoTen = SelectedItem.HoTen;
                    Email = SelectedItem.Email;
                    // Tìm loại nguoidung theo ID để giữ instance trùng (combobox)
                    SelectedUserType = UserTypes?.FirstOrDefault(ht => ht.MaNhom == SelectedItem.MaNhom);
                }
                else {
                    AddMessage = string.Empty;
                    EditMessage = string.Empty;
                    DeleteMessage = string.Empty;
                }
            }
        }
        private NHOMNGUOIDUNGDTO _SelectedUserType;
        public NHOMNGUOIDUNGDTO SelectedUserType {
            get => _SelectedUserType;
            set {
                _SelectedUserType = value;
                OnPropertyChanged();
                TenNhom = _SelectedUserType?.TenNhom;
            }
        }

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

        private string _searchText;
        public string SearchText {
            get => _searchText;
            set {
                if (_searchText != value) {
                    _searchText = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        private ObservableCollection<string> _SearchProperties;
        public ObservableCollection<string> SearchProperties {
            get => _SearchProperties;
            set { _SearchProperties = value; OnPropertyChanged(); }
        }

        private string _SelectedSearchProperty;
        public string SelectedSearchProperty {
            get => _SelectedSearchProperty;
            set {
                _SelectedSearchProperty = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }

        // Tìm kiếm
        private void PerformSearch() {
            try {
                SelectedItem = null;
                TenDangNhap = string.Empty;
                Email = string.Empty;
                SelectedUserType = null;
                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchProperty)) {
                    List = OriginalList;
                    return;
                }

                switch (SelectedSearchProperty) {
                    case "Tên đăng nhập":
                        List = new ObservableCollection<NGUOIDUNGDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.TenDangNhap) &&
                                x.TenDangNhap.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Họ Tên":
                        List = new ObservableCollection<NGUOIDUNGDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.HoTen) &&
                                x.HoTen.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Nhóm người dùng":
                        List = new ObservableCollection<NGUOIDUNGDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.MaNhom) &&
                                x.MaNhom.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Email":
                        List = new ObservableCollection<NGUOIDUNGDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.Email) &&
                                x.Email.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    default:
                        List = new ObservableCollection<NGUOIDUNGDTO>(OriginalList);
                        break;
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ICommand AddCommand { get; set; }
        private string _AddMessage;
        public string AddMessage { get => _AddMessage; set { _AddMessage = value; OnPropertyChanged(); } }
        public ICommand EditCommand { get; set; }
        private string _EditMessage;
        public string EditMessage { get => _EditMessage; set { _EditMessage = value; OnPropertyChanged(); } }
        public ICommand DeleteCommand { get; set; }
        private string _DeleteMessage;
        public string DeleteMessage { get => _DeleteMessage; set { _DeleteMessage = value; OnPropertyChanged(); } }

        public UserViewModel() {
            _nguoiDungService = new NguoiDungService();
            _nhomNguoiDungService = new NhomNguoiDungService();

            var maNhomHienTai = DataProvider.Ins.CurrentUser.MaNhom;

            List = new ObservableCollection<NGUOIDUNGDTO>(_nguoiDungService.GetAll().Where(nd => nd.MaNhom != maNhomHienTai && nd.MaNhom != "ADMIN").ToList());
            OriginalList = new ObservableCollection<NGUOIDUNGDTO>(List);
            UserTypes = new ObservableCollection<NHOMNGUOIDUNGDTO>(_nhomNguoiDungService.GetAll().ToList());

            SearchProperties = new ObservableCollection<string>
            {
                "Tên đăng nhập",
                "Họ tên",
                "Nhóm người dùng",
                "Email"
            };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            // Nút "Thêm"
            AddCommand = new RelayCommand<object>((p) => {
                if (string.IsNullOrWhiteSpace(TenDangNhap)) {
                    if (SelectedItem != null) {
                        AddMessage = "Vui lòng nhập tên đăng nhập";
                    }
                    else {
                        AddMessage = string.Empty;
                    }
                    return false;
                }
                if (string.IsNullOrWhiteSpace(MatKhau)) {
                    if (SelectedItem != null) {
                        AddMessage = "Vui lòng nhập mật khẩu";
                    }
                    else {
                        AddMessage = string.Empty;
                    }
                    return false;
                }
                if (string.IsNullOrWhiteSpace(HoTen)) {
                    if (SelectedItem != null) {
                        AddMessage = "Vui lòng nhập họ tên";
                    }
                    else {
                        AddMessage = string.Empty;
                    }
                    return false;
                }
                if (SelectedUserType == null) {
                    AddMessage = "Vui lòng chọn loại nhóm người dùng";
                    return false;
                }
                if(string.IsNullOrWhiteSpace(Email)) {
                    if (SelectedItem != null) {
                        AddMessage = "Vui lòng nhập email";
                    }
                    else {
                        AddMessage = string.Empty;
                    }
                    return false;
                }
                var exists = OriginalList.Any(x => x.TenDangNhap == TenDangNhap);
                if (exists) {
                    AddMessage = "Người dùng này đã tồn tại";
                    return false;
                }
                AddMessage = string.Empty;
                return true;
            }, (p) => {
                try {
                    var newUser = new NGUOIDUNGDTO() {
                        TenDangNhap = TenDangNhap,
                        MatKhauHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(MatKhau)),
                        HoTen = HoTen,
                        Email = Email,
                        MaNhom = SelectedUserType?.MaNhom,
                        NhomNguoiDung = SelectedUserType
                    };

                    _nguoiDungService.Create(newUser);

                    List.Add(newUser);

                    Reset();
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            // Nút "Sửa"
            EditCommand = new RelayCommand<object>((p) => {
                if (SelectedItem == null) {
                    EditMessage = string.Empty;
                    return false;
                }
                if (string.IsNullOrWhiteSpace(TenDangNhap)) {
                    EditMessage = "Tên đăng nhập không được để trống";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(MatKhau)) {
                    EditMessage = "Mật khẩu không được để trống";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(HoTen)) {
                    EditMessage = "Họ tên không được để trống";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(Email)) {
                    EditMessage = "Email không được để trống";
                    return false;
                }
                var exists = OriginalList.Any(x =>
                    x.TenDangNhap == TenDangNhap);
                if (exists) {
                    EditMessage = "Người dùng này đã tồn tại";
                    return false;
                }
                EditMessage = string.Empty;
                return true;
            }, (p) => {
                try {
                    var updateDto = new NGUOIDUNGDTO() {
                        MaNguoiDung = SelectedItem.MaNguoiDung,
                        TenDangNhap = TenDangNhap,
                        MatKhauHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(MatKhau)),
                        HoTen = HoTen,
                        Email = Email,
                        MaNhom = SelectedUserType?.MaNhom,
                        NhomNguoiDung = SelectedUserType
                    };

                    _nguoiDungService.Update(updateDto);

                    var index = List.IndexOf(SelectedItem);
                    List[index] = null;
                    List[index] = updateDto;

                    Reset();
                    MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            // Nút "Xoá"
            DeleteCommand = new RelayCommand<object>((p) => {
                return SelectedItem != null;
            }, (p) => {
                try {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa người dùng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes) {
                        _nguoiDungService.Delete(SelectedItem.MaNguoiDung);
                        List.Remove(SelectedItem);

                        Reset();
                        MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void Reset() {
            SelectedItem = null;
            TenDangNhap = string.Empty;
            MatKhau = string.Empty;
            Email = string.Empty;
            SelectedUserType = null;
            SearchText = string.Empty;
        }
    }
}
