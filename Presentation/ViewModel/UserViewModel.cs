using ClosedXML.Excel;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;


namespace QuanLyTiecCuoi.ViewModel {
    public class UserViewModel : BaseViewModel, IDataErrorInfo{
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
                    MatKhauMoi = "";
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
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }
        private bool _isAdding;
        public bool IsAdding
        {
            get => _isAdding;
            set { _isAdding = value; OnPropertyChanged(); }
        }
        private bool _isDeleting;
        public bool IsDeleting
        {
            get => _isDeleting;
            set { _isDeleting = value; OnPropertyChanged(); }
        }
        //Add ExportingExcel
        private bool _isExporting;
        public bool IsExporting
        {
            get => _isExporting;
            set { _isExporting = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> ActionList { get; } = new ObservableCollection<string> { "Thêm", "Sửa", "Xóa", "Xuất Excel", "Chọn thao tác" };
        private string _selectedAction;
        public string SelectedAction
        {
            get => _selectedAction;
            set
            {
                _selectedAction = value;
                OnPropertyChanged();
                switch (value)
                {
                    case "Thêm":
                        IsAdding = true;
                        IsEditing = false;
                        IsDeleting = false;
                        IsExporting = false;
                        Reset(); // reset các trường nhập liệu
                        break;
                    case "Sửa":
                        IsAdding = false;
                        IsEditing = true;
                        IsDeleting = false;
                        IsExporting = false;
                        Reset();
                        //if (SelectedItem == null)
                        //{
                        //    MessageBox.Show("Vui lòng chọn một sảnh để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    return;
                        //}
                        break;
                    case "Xóa":
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = true;
                        IsExporting = false;
                        Reset();
                        //if (SelectedItem == null)
                        //{
                        //    MessageBox.Show("Vui lòng chọn một sảnh để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    return;
                        //}
                        break;
                    case "Xuất Excel":
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = false;
                        IsExporting = true;
                        Reset();
                        break;
                    default:
                        _selectedAction = null;
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = false;
                        IsExporting = false;
                        Reset(); // reset các trường nhập liệu
                        break;
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
        public ICommand AddCommand { get; set; }
        private string _AddMessage;
        public string AddMessage { get => _AddMessage; set { _AddMessage = value; OnPropertyChanged(); } }
        public ICommand EditCommand { get; set; }
        private string _EditMessage;
        public string EditMessage { get => _EditMessage; set { _EditMessage = value; OnPropertyChanged(); } }
        public ICommand DeleteCommand { get; set; }
        private string _DeleteMessage;
        public string DeleteMessage { get => _DeleteMessage; set { _DeleteMessage = value; OnPropertyChanged(); } }
        public ICommand ExportToExcelCommand { get; set; }
        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => {
            Reset();
        });

        private string _TenDangNhap;
        public string TenDangNhap { get => _TenDangNhap; set { _TenDangNhap = value; OnPropertyChanged(); } }

        private string _MatKhauMoi;
        public string MatKhauMoi { get => _MatKhauMoi; set { _MatKhauMoi = value; OnPropertyChanged(); } }

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

        private bool _isChecked; //true -> Doi Mat Khau, false -> Khong doi
        public bool isChecked { get => _isChecked; set { _isChecked = value; OnPropertyChanged(); MatKhauMoi = ""; } }

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
                HoTen = string.Empty;
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

        

        public UserViewModel() {
            _nguoiDungService = new NguoiDungService();
            _nhomNguoiDungService = new NhomNguoiDungService();

            var maNhomHienTai = DataProvider.Ins.CurrentUser.MaNhom;
            isChecked = false;

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
                    AddMessage = "Vui lòng nhập tên đăng nhập";
                    return false;
                    }
                if (!isChecked || string.IsNullOrWhiteSpace(MatKhauMoi)) {
                    AddMessage = "Vui lòng nhập mật khẩu";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(HoTen)) {
                    AddMessage = "Vui lòng nhập họ tên";
                    return false;
                }
                if (SelectedUserType == null) {
                    AddMessage = "Vui lòng chọn loại nhóm người dùng";
                    return false;
                }
                if(!EmailValidationHelper.IsValidEmail(Email)) {
                    AddMessage = "Vui lòng nhập email đúng định dạng";
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
                        MatKhauHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(MatKhauMoi)),
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
                if (TenDangNhap == SelectedItem.TenDangNhap && HoTen == SelectedItem.HoTen && Email == SelectedItem.Email 
                && SelectedUserType.MaNhom == SelectedItem.MaNhom && MatKhauMoi == "") {
                    EditMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(TenDangNhap)) {
                    EditMessage = "Tên đăng nhập không được để trống";
                    return false;
                }
                var exists = List.Any(x =>
                    x.TenDangNhap == TenDangNhap && x.MaNguoiDung != SelectedItem.MaNguoiDung);
                if (exists) {
                    EditMessage = "Tên đăng nhập này đã tồn tại";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(HoTen)) {
                    EditMessage = "Họ tên không được để trống";
                    return false;
                }
                if (!EmailValidationHelper.IsValidEmail(Email)) {
                    EditMessage = "Vui lòng nhập email đúng định dạng";
                    return false;
                }
                if (isChecked && string.IsNullOrWhiteSpace(MatKhauMoi)) {
                    EditMessage = "Mật khẩu không được để trống";
                    return false;
                }

                EditMessage = string.Empty;
                return true;
            }, (p) => {
                try {
                    var updateDto = new NGUOIDUNGDTO() {
                        MaNguoiDung = SelectedItem.MaNguoiDung,
                        TenDangNhap = TenDangNhap,
                        MatKhauHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(MatKhauMoi)),
                        HoTen = HoTen,
                        Email = Email,
                        MaNhom = SelectedUserType?.MaNhom,
                        NhomNguoiDung = SelectedUserType
                    };
                    if (!string.IsNullOrWhiteSpace(MatKhauMoi)) {
                        updateDto.MatKhauHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(MatKhauMoi));
                    }
                    else {
                        updateDto.MatKhauHash = _nguoiDungService.GetById(SelectedItem.MaNguoiDung).MatKhauHash;
                    }

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
            ExportToExcelCommand = new RelayCommand<object>((p) => true, (p) => ExportToExcel());
        }
        //Exporting excel function
        private void ExportToExcel()
        {
            if (List == null || List.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách Người dùng");

            // Tiêu đề cột
            worksheet.Cell(1, 1).Value = "Tên đăng nhập";
            worksheet.Cell(1, 2).Value = "Họ tên";
            worksheet.Cell(1, 3).Value = "Nhóm người dùng";
            worksheet.Cell(1, 4).Value = "Email";

            // Format tiêu đề
            var headerRange = worksheet.Range("A1:D1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Ghi dữ liệu
            int row = 2;
            foreach (var item in List)
            {
                worksheet.Cell(row, 1).Value = item.TenDangNhap;
                worksheet.Cell(row, 2).Value = item.HoTen;
                worksheet.Cell(row, 3).Value = item.NhomNguoiDung?.TenNhom ?? "Không xác định";
                worksheet.Cell(row, 4).Value = item.Email;

                // Format dòng dữ liệu
                for (int col = 1; col <= 4; col++)
                {
                    worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
                row++;
            }

            // Tự động điều chỉnh độ rộng
            worksheet.Columns().AdjustToContents();

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"DanhSachNguoiDung_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                workbook.SaveAs(dialog.FileName);
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở file: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Reset() {
            SelectedItem = null;
            TenDangNhap = string.Empty;
            HoTen = string.Empty;
            MatKhauMoi = string.Empty;
            Email = string.Empty;
            SelectedUserType = null;
            SearchText = string.Empty;
        }
    }
}
