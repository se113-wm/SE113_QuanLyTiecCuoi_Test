using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class AddWeddingViewModel : BaseViewModel
    {
        private readonly IHallService _hallService;
        private readonly ICaService _caService;
        private readonly IPhieuDatTiecService _phieuDatTiecService;
        private readonly IMonAnService _monAnService;
        private readonly IDichVuService _dichVuService;
        private readonly IThucDonService _thucDonService;
        private readonly IChiTietDVService _chiTietDichVuService;
        private readonly IThamSoService _thamSoService;
        
        // Wedding Info
        private string _tenChuRe;
        public string TenChuRe { get => _tenChuRe; set { _tenChuRe = value; OnPropertyChanged(); } }

        private string _tenCoDau;
        public string TenCoDau { get => _tenCoDau; set { _tenCoDau = value; OnPropertyChanged(); } }

        private string _dienThoai;
        public string DienThoai { get => _dienThoai; set { _dienThoai = value; OnPropertyChanged(); } }

        private DateTime? _ngayDaiTiec;
        public DateTime? NgayDaiTiec { get => _ngayDaiTiec; set { _ngayDaiTiec = value; OnPropertyChanged(); } }

        public ObservableCollection<CalendarDateRange> NgayKhongChoChon { get; set; }

        private DateTime _ngayDatTiec = DateTime.Now;
        public DateTime NgayDatTiec { get => _ngayDatTiec; set { _ngayDatTiec = value; OnPropertyChanged(); } }

        // Session (Ca) & Hall (Sanh)
        public ObservableCollection<CADTO> CaList { get; set; }
        private CADTO _selectedCa;
        public CADTO SelectedCa { get => _selectedCa; set { _selectedCa = value; OnPropertyChanged(); } }

        public ObservableCollection<SANHDTO> SanhList { get; set; }
        private SANHDTO _selectedSanh;
        public SANHDTO SelectedSanh { get => _selectedSanh; set { _selectedSanh = value; OnPropertyChanged(); } }

        private string _tienDatCoc;
        public string TienDatCoc { get => _tienDatCoc; set { _tienDatCoc = value; OnPropertyChanged(); } }

        private string _soLuongBan;
        public string SoLuongBan { get => _soLuongBan; set { _soLuongBan = value; OnPropertyChanged(); } }

        private string _soBanDuTru;
        public string SoBanDuTru { get => _soBanDuTru; set { _soBanDuTru = value; OnPropertyChanged(); } }

        // Menu Section
        public ObservableCollection<THUCDONDTO> MenuList { get; set; } = new ObservableCollection<THUCDONDTO>();
        private decimal _menuTotal;
        public decimal MenuTotal { get => _menuTotal; set { _menuTotal = value; OnPropertyChanged(); } }

        private THUCDONDTO _selectedMenuItem;
        public THUCDONDTO SelectedMenuItem { get => _selectedMenuItem; set { _selectedMenuItem = value; OnPropertyChanged(); LoadMenuDetail(); } }

        public MONANDTO MonAn { get; set; } = new MONANDTO();
        private string _td_SoLuong;
        public string TD_SoLuong { get => _td_SoLuong; set { _td_SoLuong = value; OnPropertyChanged(); } }
        private string _td_GhiChu;
        public string TD_GhiChu { get => _td_GhiChu; set { _td_GhiChu = value; OnPropertyChanged(); } }

        // Service Section
        public ObservableCollection<CHITIETDVDTO> ServiceList { get; set; } = new ObservableCollection<CHITIETDVDTO>();
        private decimal _serviceTotal;
        public decimal ServiceTotal { get => _serviceTotal; set { _serviceTotal = value; OnPropertyChanged(); } }
        private CHITIETDVDTO _selectedServiceItem;
        public CHITIETDVDTO SelectedServiceItem { get => _selectedServiceItem; set { _selectedServiceItem = value; OnPropertyChanged(); LoadServiceDetail(); } }

        public DICHVUDTO DichVu { get; set; } = new DICHVUDTO();
        private string _dv_SoLuong;
        public string DV_SoLuong { get => _dv_SoLuong; set { _dv_SoLuong = value; OnPropertyChanged(); } }
        private string _dv_GhiChu;
        public string DV_GhiChu { get => _dv_GhiChu; set { _dv_GhiChu = value; OnPropertyChanged(); } }

        // Commands
        public ICommand ResetWeddingCommand { get; set; }
        public ICommand ResetTDCommand { get; set; }
        public ICommand ResetCTDVCommand { get; set; }
        public ICommand ChonMonAnCommand { get; set; }
        public ICommand AddMenuCommand { get; set; }
        public ICommand EditMenuCommand { get; set; }
        public ICommand DeleteMenuCommand { get; set; }

        public ICommand ChonDichVuCommand { get; set; }
        public ICommand AddServiceCommand { get; set; }
        public ICommand EditServiceCommand { get; set; }
        public ICommand DeleteServiceCommand { get; set; }

        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        // Constructor với Dependency Injection
        public AddWeddingViewModel(
            IHallService hallService,
            ICaService caService,
            IPhieuDatTiecService phieuDatTiecService,
            IMonAnService monAnService,
            IDichVuService dichVuService,
            IThucDonService thucDonService,
            IChiTietDVService chiTietDichVuService,
            IThamSoService thamSoService)
        {
            // Inject services
            _hallService = hallService;
            _caService = caService;
            _phieuDatTiecService = phieuDatTiecService;
            _monAnService = monAnService;
            _dichVuService = dichVuService;
            _thucDonService = thucDonService;
            _chiTietDichVuService = chiTietDichVuService;
            _thamSoService = thamSoService;

            NgayKhongChoChon = new ObservableCollection<CalendarDateRange>
            {
                new CalendarDateRange(DateTime.MinValue, DateTime.Today)
            };

            // Load data from services
            CaList = new ObservableCollection<CADTO>(_caService.GetAll());
            SanhList = new ObservableCollection<SANHDTO>(_hallService.GetAll());

            // Initialize commands
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ResetWeddingCommand = new RelayCommand<object>((p) => true, (p) => ResetWedding());
            ResetTDCommand = new RelayCommand<object>((p) => true, (p) => ResetTD());
            ResetCTDVCommand = new RelayCommand<object>((p) => true, (p) => ResetCTDV());

            // Menu commands
            ChonMonAnCommand = new RelayCommand<object>((p) => true, (p) => ChonMonAn());
            AddMenuCommand = new RelayCommand<object>((p) => 
            {
                // Nếu MonAn là null hoặc không có tên món ăn, không thể thêm
                if (MonAn == null || string.IsNullOrWhiteSpace(MonAn.TenMonAn) || !int.TryParse(TD_SoLuong, out int sl) || sl <= 0)
                {
                    return false;
                }
                // Kiểm tra xem món ăn đã tồn tại trong danh sách chưa
                var existingItem = MenuList.FirstOrDefault(m => m.MonAn.MaMonAn == MonAn.MaMonAn);
                if (existingItem != null)
                {
                    // Nếu món ăn đã tồn tại, không thể thêm
                    return false;
                }
                return true;
            }
            , (p) => 
            {
                MenuList.Add(new THUCDONDTO
                {
                    MonAn = MonAn,
                    SoLuong = int.Parse(TD_SoLuong),
                    GhiChu = TD_GhiChu
                });
                // Reset input fields after adding
                TD_SoLuong = string.Empty;
                TD_GhiChu = string.Empty;
                MonAn = new MONANDTO();
                OnPropertyChanged(nameof(MonAn));
                // Update total price
                MenuTotal = MenuList.Sum(m => (m.MonAn.DonGia ?? 0) * (m.SoLuong ?? 0));
            });
            EditMenuCommand = new RelayCommand<object>((p) =>
            {
                // Nếu SelectedMenuItem là null, không thể chỉnh sửa
                if (SelectedMenuItem == null) return false;
                // Kiểm tra tính hợp lệ của dữ liệu
                if (MonAn == null || string.IsNullOrWhiteSpace(MonAn.TenMonAn) || !int.TryParse(TD_SoLuong, out int sl) || sl <= 0)
                {
                    return false;
                }
                // Nếu không có gì thay đổi, không cần chỉnh sửa
                if (SelectedMenuItem.MonAn.TenMonAn == MonAn.TenMonAn && SelectedMenuItem.SoLuong.ToString() == TD_SoLuong && SelectedMenuItem.GhiChu == TD_GhiChu)
                {
                    return false;
                }
                // Kiểm tra xem món ăn có tồn tại trong danh sách chưa
                var existingItem = MenuList.FirstOrDefault(m => m.MonAn.MaMonAn == MonAn.MaMonAn);
                if (existingItem != null && existingItem != SelectedMenuItem)
                {
                    // Nếu món ăn đã tồn tại và không phải là món ăn đang chỉnh sửa, không thể chỉnh sửa
                    return false;
                }
                return true;
            }, (p) =>
            {
                var NewMenuItem = new THUCDONDTO
                {
                    MonAn = MonAn,
                    SoLuong = int.Parse(TD_SoLuong),
                    GhiChu = TD_GhiChu
                };
                int index = MenuList.IndexOf(SelectedMenuItem);
                MenuList[index] = null; // Remove the old item to trigger UI update
                MenuList[index] = NewMenuItem; // Add the updated item back
                // Update total price
                MenuTotal = MenuList.Sum(m => (m.MonAn.DonGia ?? 0) * (m.SoLuong ?? 0));
            });
            DeleteMenuCommand = new RelayCommand<object>((p) => SelectedMenuItem != null, (p) =>
            {
                MenuList.Remove(SelectedMenuItem);
                SelectedMenuItem = null;
                // Update total price
                MenuTotal = MenuList.Sum(m => (m.MonAn.DonGia ?? 0) * (m.SoLuong ?? 0));
            });
            // Service commands
            ChonDichVuCommand = new RelayCommand<object>((p) => true, (p) => ChonDichVu());
            AddServiceCommand = new RelayCommand<object>((p) =>
            {
                // Nếu DichVu là null hoặc không có tên dịch vụ, không thể thêm
                if (DichVu == null || string.IsNullOrWhiteSpace(DichVu.TenDichVu) || !int.TryParse(DV_SoLuong, out int sl) || sl <= 0)
                {
                    return false;
                }
                // Kiểm tra xem dịch vụ đã tồn tại trong danh sách chưa
                var existingService = ServiceList.FirstOrDefault(s => s.DichVu.MaDichVu == DichVu.MaDichVu);
                if (existingService != null)
                {
                    // Nếu dịch vụ đã tồn tại, không thể thêm
                    return false;
                }
                return true;

            }
            , (p) =>
            {
                ServiceList.Add(new CHITIETDVDTO
                {
                    DichVu = DichVu,
                    SoLuong = int.Parse(DV_SoLuong),
                    GhiChu = DV_GhiChu
                });
                DV_SoLuong = string.Empty;
                DV_GhiChu = string.Empty;
                DichVu = new DICHVUDTO();
                OnPropertyChanged(nameof(DichVu));
                // Update total price
                ServiceTotal = ServiceList.Sum(s => (s.DichVu.DonGia ?? 0) * (s.SoLuong ?? 0));
            });
            EditServiceCommand = new RelayCommand<object>((p) =>
            {
                // Nếu SelectedServiceItem là null, không thể chỉnh sửa
                if (SelectedServiceItem == null) return false;
                // Kiểm tra tính hợp lệ của dữ liệu
                if (DichVu == null || string.IsNullOrWhiteSpace(DichVu.TenDichVu) || !int.TryParse(DV_SoLuong, out int sl) || sl <= 0)
                {
                    return false;
                }
                // Nếu không có gì thay đổi, không cần chỉnh sửa
                if (SelectedServiceItem.DichVu.TenDichVu == DichVu.TenDichVu && SelectedServiceItem.SoLuong.ToString() == DV_SoLuong && SelectedServiceItem.GhiChu == DV_GhiChu)
                {
                    return false;
                }
                // Kiểm tra xem dịch vụ có tồn tại trong danh sách chưa
                var existingService = ServiceList.FirstOrDefault(s => s.DichVu.MaDichVu == DichVu.MaDichVu);
                if (existingService != null && existingService != SelectedServiceItem)
                {
                    // Nếu dịch vụ đã tồn tại và không phải là dịch vụ đang chỉnh sửa, không thể chỉnh sửa
                    return false;
                }
                return true;
            }, (p) =>
            {
                var NewServiceItem = new CHITIETDVDTO
                {
                    DichVu = DichVu,
                    SoLuong = int.Parse(DV_SoLuong),
                    GhiChu = DV_GhiChu
                };
                int index = ServiceList.IndexOf(SelectedServiceItem);
                ServiceList[index] = null; // Remove the old item to trigger UI update
                ServiceList[index] = NewServiceItem; // Add the updated item back
                // Update total price
                ServiceTotal = ServiceList.Sum(s => (s.DichVu.DonGia ?? 0) * (s.SoLuong ?? 0));
            });
            DeleteServiceCommand = new RelayCommand<object>((p) => SelectedServiceItem != null, (p) => 
            {
                ServiceList.Remove(SelectedServiceItem);
                SelectedServiceItem = null;
                // Update total price
                ServiceTotal = ServiceList.Sum(s => (s.DichVu.DonGia ?? 0) * (s.SoLuong ?? 0));
            });

            // Confirm/Cancel
            ConfirmCommand = new RelayCommand<Window>((p) => CanConfirm(), (p) =>
            {
                // 1. Kiểm tra các trường bắt buộc
                if (string.IsNullOrWhiteSpace(TenChuRe) || string.IsNullOrWhiteSpace(TenCoDau) || string.IsNullOrWhiteSpace(DienThoai) ||
                    NgayDaiTiec == null || SelectedCa == null || SelectedSanh == null || string.IsNullOrWhiteSpace(TienDatCoc) ||
                    string.IsNullOrWhiteSpace(SoLuongBan) || string.IsNullOrWhiteSpace(SoBanDuTru) ||
                    MenuList.Count == 0 || ServiceList.Count == 0)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc và chọn thực đơn, dịch vụ.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 2. Kiểm tra trùng ngày, sảnh
                var existingWedding = _phieuDatTiecService.GetAll()
                    .FirstOrDefault(w => w.NgayDaiTiec.HasValue && NgayDaiTiec.HasValue &&
                             w.NgayDaiTiec.Value.Date == NgayDaiTiec.Value.Date &&
                             w.Sanh != null && w.Sanh.MaSanh == SelectedSanh.MaSanh);
                if (existingWedding != null)
                {
                    MessageBox.Show("Đã có tiệc cưới được đặt cho ngày và sảnh này. Vui lòng chọn ngày hoặc sảnh khác.", "Trùng lịch", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 3. Kiểm tra số lượng bàn hợp lệ
                var tiLeSoBanDatTruocToiThieu = _thamSoService.GetByName("TiLeSoBanDatTruocToiThieu")?.GiaTri ?? 0.5m;
                var soLuongBanToiDa = _sanhService.GetById(SelectedSanh.MaSanh)?.SoLuongBanToiDa ?? 0;
                if (!int.TryParse(SoLuongBan, out int soLuongBan) || soLuongBan <= 0)
                {
                    MessageBox.Show("Số lượng bàn phải là số nguyên dương.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (soLuongBan > soLuongBanToiDa)
                {
                    MessageBox.Show($"Số lượng bàn vượt quá số lượng tối đa của sảnh ({soLuongBanToiDa}).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Sửa lại giúp người dùng
                    SoLuongBan = soLuongBanToiDa.ToString();
                    return;
                }
                if (soLuongBan < (tiLeSoBanDatTruocToiThieu * soLuongBanToiDa))
                {
                    // làm tròn lên
                    MessageBox.Show($"Số lượng bàn phải lớn hơn hoặc bằng {Math.Ceiling(tiLeSoBanDatTruocToiThieu * soLuongBanToiDa)} (tỉ lệ đặt trước tối thiểu).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Sửa lại giúp người dùng
                    SoLuongBan = Math.Ceiling(tiLeSoBanDatTruocToiThieu * soLuongBanToiDa).ToString();
                    //MessageBox.Show($"Số lượng bàn phải lớn hơn hoặc bằng {tiLeSoBanDatTruocToiThieu * soLuongBanToiDa} (tỉ lệ đặt trước tối thiểu).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 4. Kiểm tra số lượng bàn dự trữ hợp lệ 
//                Kiểm tra 0 <= Số bàn dự trữ(D1)   và
                //Số bàn dự trữ(D1) <= Số lượng bàn tối đa của sảnh tương ứng(D3)
                //– Số lượng bàn(D1) ?
                var soBanDuTruToiDa = soLuongBanToiDa - soLuongBan;
                if (!int.TryParse(SoBanDuTru, out int soBanDuTru) || soBanDuTru < 0 || soBanDuTru > soBanDuTruToiDa)
                {
                    MessageBox.Show($"Số bàn dự trữ phải là số nguyên dương và không vượt quá {soBanDuTruToiDa}.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Sửa lại giúp người dùng
                    SoBanDuTru = soBanDuTruToiDa.ToString();
                    return;
                }
                // Kiểm tra formatter số điện thoại (10 số hoặc 11 số bắt đầu bằng 0 hoặc 84)
                if (!string.IsNullOrWhiteSpace(DienThoai) && !System.Text.RegularExpressions.Regex.IsMatch(DienThoai, @"^(0|\+84)[0-9]{9,10}$"))
                {
                    MessageBox.Show("Số điện thoại phải là 10 hoặc 11 chữ số và bắt đầu bằng 0 hoặc +84.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Sửa lại giúp người dùng
                    DienThoai = "0123456789"; // Ví dụ sửa lại
                    return;
                }

                // 4. Kiểm tra tiền đặt cọc hợp lệ
                var tongDonGiaMonAn = MenuList.Sum(m => (m.MonAn.DonGia ?? 0) * (m.SoLuong ?? 0));
                var tongDonGiaDichVu = ServiceList.Sum(s => (s.DichVu.DonGia ?? 0) * (s.SoLuong ?? 0));
                var donGiaBanToiThieu = _sanhService.GetById(SelectedSanh.MaSanh)?.LoaiSanh.DonGiaBanToiThieu ?? 0;
                var tongChiPhiUocTinh = soLuongBan * (donGiaBanToiThieu + tongDonGiaMonAn) + tongDonGiaDichVu;
                var tiLeTienDatCocToiThieu = _thamSoService.GetByName("TiLeTienDatCocToiThieu")?.GiaTri ?? 0.3m;

                var minDeposit = (decimal)Math.Ceiling(tiLeTienDatCocToiThieu * tongChiPhiUocTinh);
                var maxDeposit = (decimal)Math.Ceiling(tongChiPhiUocTinh);

                if (!decimal.TryParse(TienDatCoc, out decimal tienDatCoc))
                {
                    MessageBox.Show("Tiền đặt cọc phải là số hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (tienDatCoc < minDeposit)
                {
                    MessageBox.Show($"Tiền đặt cọc phải lớn hơn hoặc bằng {minDeposit:N0} (tỉ lệ tối thiểu).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TienDatCoc = minDeposit.ToString("N0");
                    return;
                }
                if (tienDatCoc > maxDeposit)
                {
                    MessageBox.Show("Tiền đặt cọc không được vượt quá tổng chi phí ước tính.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TienDatCoc = maxDeposit.ToString("N0");
                    return;
                }

                // 5. Lưu dữ liệu
                try
                {
                    var phieuDatTiec = new PHIEUDATTIECDTO
                    {
                        TenChuRe = TenChuRe,
                        TenCoDau = TenCoDau,
                        DienThoai = DienThoai,
                        NgayDaiTiec = NgayDaiTiec.Value.Date.Add(SelectedCa.ThoiGianBatDauCa.Value),
                        NgayDatTiec = NgayDatTiec,
                        Ca = SelectedCa,
                        Sanh = SelectedSanh,
                        DonGiaBanTiec = SelectedSanh.LoaiSanh.DonGiaBanToiThieu,
                        TienDatCoc = tienDatCoc,
                        SoLuongBan = soLuongBan,
                        SoBanDuTru = int.Parse(SoBanDuTru),
                        MaCa = SelectedCa.MaCa,
                        MaSanh = SelectedSanh.MaSanh,

                    };

                    _phieuDatTiecService.Create(phieuDatTiec);
                    
                    // Lấy phiếu vừa tạo
                    phieuDatTiec = _phieuDatTiecService.GetAll().LastOrDefault();
                    for (int i = 0; i < MenuList.Count; i++)
                    {
                        var item = MenuList[i];
                        var thucDon = new THUCDONDTO
                        {
                            MaPhieuDat = phieuDatTiec.MaPhieuDat,
                            MaMonAn = item.MonAn.MaMonAn,
                            DonGia = item.MonAn.DonGia,
                            SoLuong = item.SoLuong,
                            GhiChu = item.GhiChu,
                            MonAn = item.MonAn,
                            
                        };
                        _thucDonService.Create(thucDon);
                    }

                    for (int i = 0; i < ServiceList.Count; i++)
                    {
                        var item = ServiceList[i];
                        var chiTietDV = new CHITIETDVDTO
                        {
                            MaPhieuDat = phieuDatTiec.MaPhieuDat,
                            MaDichVu = item.DichVu.MaDichVu,                            
                            DonGia = item.DichVu.DonGia,
                            SoLuong = item.SoLuong,
                            ThanhTien = (item.DichVu.DonGia ?? 0) * (item.SoLuong ?? 0),
                            GhiChu = item.GhiChu,
                            DichVu = item.DichVu,
                        };
                        _chiTietDichVuService.Create(chiTietDV);
                    }
                    if (p is Window window)
                    {
                        MessageBox.Show("Đặt tiệc cưới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        window.Close(); // Close the window
                    }

                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Dữ liệu nhập vào không hợp lệ. Vui lòng kiểm tra lại số lượng, tiền đặt cọc, ...\nChi tiết: " + ex.Message, "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Có lỗi khi lưu dữ liệu. Vui lòng thử lại.\nChi tiết: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi không xác định: " + ex.Message +
                        (ex.InnerException != null ? "\nChi tiết: " + ex.InnerException.Message : ""),
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            CancelCommand = new RelayCommand<Window>((p) => true, (p) => 
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn hủy không?", "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Close the window
                    if (p is Window window)
                    {
                        window.Close();
                    }
                }
            });
        }

        private void LoadMenuDetail()
        {
            if (SelectedMenuItem != null)
            {
                MonAn = SelectedMenuItem.MonAn;
                TD_SoLuong = SelectedMenuItem.SoLuong.ToString();
                TD_GhiChu = SelectedMenuItem.GhiChu;
                OnPropertyChanged(nameof(MonAn));
            }
            else
            {
                MonAn = new MONANDTO();
                TD_SoLuong = string.Empty;
                TD_GhiChu = string.Empty;
                OnPropertyChanged(nameof(MonAn));
            }
        }

        private void LoadServiceDetail()
        {
            if (SelectedServiceItem != null)
            {
                DichVu = SelectedServiceItem.DichVu;
                DV_SoLuong = SelectedServiceItem.SoLuong.ToString();
                DV_GhiChu = SelectedServiceItem.GhiChu;
                OnPropertyChanged(nameof(DichVu));
            }
            else
            {
                DichVu = new DICHVUDTO();
                DV_SoLuong = string.Empty;
                DV_GhiChu = string.Empty;
                OnPropertyChanged(nameof(DichVu));
            }
        }
        private void ResetWedding()
        {
            TenChuRe = string.Empty;
            TenCoDau = string.Empty;
            DienThoai = string.Empty;
            NgayDaiTiec = null;
            NgayDatTiec = DateTime.Now;
            SelectedCa = null;
            SelectedSanh = null;
            TienDatCoc = string.Empty;
            SoLuongBan = string.Empty;
            SoBanDuTru = string.Empty;
        }
        private void ResetTD()
        {
            MonAn = new MONANDTO();
            TD_SoLuong = string.Empty;
            TD_GhiChu = string.Empty;
            SelectedMenuItem = null;
        }

        private void ResetCTDV()
        {
            DichVu = new DICHVUDTO();
            DV_SoLuong = string.Empty;
            DV_GhiChu = string.Empty;
            SelectedServiceItem = null;
        }

        // Menu logic
        private void ChonMonAn()
        {
            var monAnDialog = new MenuItemView();
            var viewModel = Infrastructure.ServiceContainer.GetService<MenuItemViewModel>();
            monAnDialog.DataContext = viewModel;
            if (monAnDialog.ShowDialog() == true)
            {
                MonAn = viewModel.SelectedFood;
                TD_SoLuong = "1"; // Default quantity
                OnPropertyChanged(nameof(MonAn));
            }
        }
        
        // Service logic
        private void ChonDichVu()
        {
            var dichVuDialog = new ServiceDetailItemView();
            var viewModel = Infrastructure.ServiceContainer.GetService<ServiceDetailItemViewModel>();
            dichVuDialog.DataContext = viewModel;
            if (dichVuDialog.ShowDialog() == true)
            {
                DichVu = viewModel.SelectedService;
                DV_SoLuong = "1"; // Default quantity
                OnPropertyChanged(nameof(DichVu));
            }
        }

        // Confirm/Cancel logic
        private bool CanConfirm()
        {
            return true;
        }
    }
}