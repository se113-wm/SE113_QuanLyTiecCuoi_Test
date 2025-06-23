using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class WeddingDetailViewModel : BaseViewModel
    {
        // Services
        private readonly ISanhService _sanhService;
        private readonly ICaService _caService;
        private IPhieuDatTiecService _phieuDatTiecService;
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

        private ObservableCollection<SANHDTO> _sanhList;
        public ObservableCollection<SANHDTO> SanhList
        {
            get => _sanhList;
            set
            {
                if (_sanhList != value)
                {
                    _sanhList = value;
                    OnPropertyChanged();
                }
            }
        }
        private SANHDTO _selectedSanh;
        public SANHDTO SelectedSanh { get => _selectedSanh; set { _selectedSanh = value; OnPropertyChanged(); } }

        private string _tienDatCoc;
        public string TienDatCoc { get => _tienDatCoc; set { _tienDatCoc = value; OnPropertyChanged(); } }

        private string _soLuongBan;
        public string SoLuongBan { get => _soLuongBan; set { _soLuongBan = value; OnPropertyChanged(); } }

        private string _soBanDuTru;
        public string SoBanDuTru { get => _soBanDuTru; set { _soBanDuTru = value; OnPropertyChanged(); } }

        // Edit mode
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (value == _isEditing) return;

                // Kiểm tra điều kiện trước khi cho phép chỉnh sửa
                if (value) // Chỉ kiểm tra khi chuyển sang chế độ chỉnh sửa
                {
                    // Đã thanh toán hoặc đã qua ngày đãi tiệc thì không cho chỉnh sửa
                    if (CurrentWedding?.NgayThanhToan != null)
                    {
                        MessageBox.Show("Tiệc cưới đã thanh toán, không thể chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (NgayDaiTiec != null && DateTime.Now > NgayDaiTiec)
                    {
                        MessageBox.Show("Đã qua ngày đãi tiệc, không thể chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        // Thêm property để lấy thông tin tiệc hiện tại (nếu chưa có)
        public PHIEUDATTIECDTO CurrentWedding => _phieuDatTiecService.GetById(_maPhieuDat);

        // Menu Section
        private ObservableCollection<THUCDONDTO> _menuList;
        public ObservableCollection<THUCDONDTO> MenuList
        {
            get => _menuList;
            set
            {
                if (_menuList != value)
                {
                    _menuList = value;
                    OnPropertyChanged();
                }
            }
        }

        private THUCDONDTO _selectedMenuItem;
        public THUCDONDTO SelectedMenuItem { get => _selectedMenuItem; set { _selectedMenuItem = value; OnPropertyChanged(); LoadMenuDetail(); } }

        public MONANDTO MonAn { get; set; } = new MONANDTO();

        private string _td_DonGia;
        public string TD_DonGia { get => _td_DonGia; set { _td_DonGia = value; OnPropertyChanged(); } }

        private string _td_SoLuong;
        public string TD_SoLuong { get => _td_SoLuong; set { _td_SoLuong = value; OnPropertyChanged(); } }
        private string _td_GhiChu;
        public string TD_GhiChu { get => _td_GhiChu; set { _td_GhiChu = value; OnPropertyChanged(); } }

        // Service Section
        private ObservableCollection<CHITIETDVDTO> _serviceList;
        public ObservableCollection<CHITIETDVDTO> ServiceList
        {
            get => _serviceList;
            set
            {
                if (_serviceList != value)
                {
                    _serviceList = value;
                    OnPropertyChanged();
                }
            }
        }

        private CHITIETDVDTO _selectedServiceItem;
        public CHITIETDVDTO SelectedServiceItem { get => _selectedServiceItem; set { _selectedServiceItem = value; OnPropertyChanged(); LoadServiceDetail(); } }

        public DICHVUDTO DichVu { get; set; } = new DICHVUDTO();

        private string _dv_DonGia;
        public string DV_DonGia { get => _dv_DonGia; set { _dv_DonGia = value; OnPropertyChanged(); } }

        private string _dv_SoLuong;
        public string DV_SoLuong { get => _dv_SoLuong; set { _dv_SoLuong = value; OnPropertyChanged(); } }
        private string _dv_GhiChu;
        public string DV_GhiChu { get => _dv_GhiChu; set { _dv_GhiChu = value; OnPropertyChanged(); } }

        // Commands
        public ICommand ResetTCCommand { get; set; }
        public ICommand ConfirmEditCommand { get; set; }
        public ICommand CancelEditCommand { get; set; }
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
        public ICommand ShowInvoiceCommand { get; set; }

        private int _maPhieuDat;

        public WeddingDetailViewModel(int maPhieuDat)
        {
            // Initialize services
            _sanhService = new SanhService();
            _caService = new CaService();
            _phieuDatTiecService = new PhieuDatTiecService();
            _monAnService = new MonAnService();
            _dichVuService = new DichVuService();
            _thucDonService = new ThucDonService();
            _chiTietDichVuService = new ChiTietDVService();
            _thamSoService = new ThamSoService();

            // Load data
            CaList = new ObservableCollection<CADTO>(_caService.GetAll());
            SanhList = new ObservableCollection<SANHDTO>(_sanhService.GetAll());

            _maPhieuDat = maPhieuDat;
            // Example: Load wedding detail here if needed
            if (maPhieuDat > 0)
            {
                LoadWeddingDetail(maPhieuDat);
            }
            var from = new DateTime(2000, 1, 1);
            var to = DateTime.Today;

            // Nếu có ngày đãi tiệc, kiểm tra kỹ
            //if (NgayDaiTiec.HasValue)
            //{
            //    var date = NgayDaiTiec.Value.Date;

            //    // Nếu ngày đãi tiệc nằm trong phạm vi hợp lệ và lớn hơn "from"
            //    if (date >= from && date <= to)
            //    {
            //        to = date;
            //    }
            //}

            //// Không được để from > to
            //if (from > to)
            //{
            //    // Log để kiểm tra
            //    MessageBox.Show($"LỖI PHẠM VI: from = {from}, to = {to}");
            //    // Đặt lại to = from để tránh lỗi
            //    to = from;
            if (from > to)
            {
                // Đặt lại to = from để tránh lỗi
                to = from;
            }
            NgayKhongChoChon = new ObservableCollection<CalendarDateRange>();


            ResetTCCommand = new RelayCommand<object>((p) => true, (p) => 
            {
                // Lấy lại thông tin ban đầu
                var wedding = _phieuDatTiecService.GetById(_maPhieuDat);
                if (wedding != null)
                {
                    TenChuRe = wedding.TenChuRe;
                    TenCoDau = wedding.TenCoDau;
                    DienThoai = wedding.DienThoai;
                    NgayDaiTiec = wedding.NgayDaiTiec;
                    NgayDatTiec = (DateTime)wedding.NgayDatTiec;
                    SelectedCa = CaList.FirstOrDefault(c => c.MaCa == wedding.MaCa);
                    SelectedSanh = SanhList.FirstOrDefault(s => s.MaSanh == wedding.MaSanh);
                    TienDatCoc = wedding.TienDatCoc.ToString();
                    SoLuongBan = wedding.SoLuongBan.ToString();
                    SoBanDuTru = wedding.SoBanDuTru.ToString();
                    
                    // Load menu and service details
                    MenuList = new ObservableCollection<THUCDONDTO>(_thucDonService.GetByPhieuDat(_maPhieuDat));
                    ServiceList = new ObservableCollection<CHITIETDVDTO>(_chiTietDichVuService.GetByPhieuDat(_maPhieuDat));
                }
            });
            ConfirmEditCommand = new RelayCommand<object>((p) => IsEditing, (p) => ConfirmEdit());
            CancelEditCommand = new RelayCommand<object>((p) => IsEditing, (p) => CancelEdit());
            ResetTDCommand = new RelayCommand<object>((p) => true, (p) => ResetTD());
            ResetCTDVCommand = new RelayCommand<object>((p) => true, (p) => ResetCTDV());

            ChonMonAnCommand = new RelayCommand<object>((p) => IsEditing, (p) => ChonMonAn());
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
                    DonGia = MonAn.DonGia,
                    SoLuong = int.Parse(TD_SoLuong),
                    GhiChu = TD_GhiChu
                });
                // Reset input fields after adding
                TD_SoLuong = string.Empty;
                TD_GhiChu = string.Empty;
                TD_DonGia = string.Empty;
                MonAn = new MONANDTO();
                OnPropertyChanged(nameof(MonAn));
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
                if (
                    SelectedMenuItem.MonAn.TenMonAn == MonAn.TenMonAn &&
                    SelectedMenuItem.SoLuong?.ToString() == TD_SoLuong &&
                    SelectedMenuItem.GhiChu == TD_GhiChu &&
                    (
                        (SelectedMenuItem.DonGia ?? 0) == (decimal.TryParse(TD_DonGia.Replace(",", "").Replace(".", ""), out var tdDonGia) ? tdDonGia : 0)
                    )
                )
                {
                    // Mess, cho tui xem giá trị của SelectedMenuItem.MonAn.DonGia và MonAn.DonGia
                     
                    return false;
                }
                //MessageBox.Show($"SelectedMenuItem.MonAn.DonGia: {SelectedMenuItem.MonAn.DonGia}, MonAn.DonGia: {TD_DonGia}");
                // Kiểm tra xem món ăn có tồn tại trong danh sách chưa
                var existingItem = MenuList.FirstOrDefault(m => m.MonAn.MaMonAn == MonAn.MaMonAn);
                if (existingItem != null && existingItem != SelectedMenuItem)
                {
                    // Nếu món ăn đã tồn tại và không phải là món ăn đang chỉnh sửa, không thể chỉnh sửa
                    return false;
                }
                // Kiểm tra xem món ăn có tồn tại trong danh sách chưa, nếu rồi thì kiểm tra xem có khác đơn giá trong menu ko

                return true;
            }, (p) =>
            {
                // Nếu giá mã món ăn giống nhau, mà đơn giá món ăn khác đơn giá trong menu
                // thì báo lên message hỏi có muốn cập nhật theo giá mới (giá món ăn)
                // Nếu có thì cho sửa bình thường, không thì kết thúc
                if (SelectedMenuItem.MonAn.MaMonAn == MonAn.MaMonAn && SelectedMenuItem.DonGia != MonAn.DonGia)
                {
                    var result = MessageBox.Show($"Món ăn {MonAn.TenMonAn} đã có trong thực đơn với đơn giá {SelectedMenuItem.DonGia:N0}. Bạn có muốn cập nhật đơn giá mới là {MonAn.DonGia:N0} không?", "Cập nhật đơn giá", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return; // Không cập nhật, kết thúc lệnh
                    }
                }

                var NewMenuItem = new THUCDONDTO
                {
                    MonAn = MonAn,
                    DonGia = MonAn.DonGia,
                    SoLuong = int.Parse(TD_SoLuong),
                    GhiChu = TD_GhiChu
                };
                int index = MenuList.IndexOf(SelectedMenuItem);
                MenuList[index] = null; // Remove the old item to trigger UI update
                MenuList[index] = NewMenuItem; // Add the updated item back
            });
            DeleteMenuCommand = new RelayCommand<object>((p) => SelectedMenuItem != null, (p) =>
            {
                MenuList.Remove(SelectedMenuItem);
                SelectedMenuItem = null;
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
                    DonGia = DichVu.DonGia,
                    SoLuong = int.Parse(DV_SoLuong),
                    GhiChu = DV_GhiChu
                });
                DV_SoLuong = string.Empty;
                DV_GhiChu = string.Empty;
                DV_DonGia = string.Empty;
                DichVu = new DICHVUDTO();
                OnPropertyChanged(nameof(DichVu));
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
                if (
                    SelectedServiceItem.DichVu.TenDichVu == DichVu.TenDichVu &&
                    SelectedServiceItem.SoLuong?.ToString() == DV_SoLuong &&
                    SelectedServiceItem.GhiChu == DV_GhiChu &&
                    (
                        (SelectedServiceItem.DonGia ?? 0) == (decimal.TryParse(DV_DonGia.Replace(",", "").Replace(".", ""), out var dvDonGia) ? dvDonGia : 0)
                    )
                )
                {
                    
                    return false;
                }
                //// Nếu không có gì thay đổi, không cần chỉnh sửa
                               //if (
                //    SelectedMenuItem.MonAn.TenMonAn == MonAn.TenMonAn &&
                //    SelectedMenuItem.SoLuong?.ToString() == TD_SoLuong &&
                //    SelectedMenuItem.GhiChu == TD_GhiChu &&
                //    (
                //        (SelectedMenuItem.MonAn.DonGia ?? 0) == (decimal.TryParse(TD_DonGia.Replace(",", "").Replace(".", ""), out var tdDonGia) ? tdDonGia : 0)
                //    )
                //)
                //{
                //    return false;
                //}
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
                // Nếu giá mã dịch vụ giống nhau, mà đơn giá dịch vụ khác đơn giá trong danh sách
                // thì báo lên message hỏi có muốn cập nhật theo giá mới (giá dịch vụ)
                // Nếu có thì cho sửa bình thường, không thì kết thúc
                if (SelectedServiceItem.DichVu.MaDichVu == DichVu.MaDichVu && SelectedServiceItem.DonGia != DichVu.DonGia)
                {
                    var result = MessageBox.Show($"Dịch vụ {DichVu.TenDichVu} đã có trong danh sách với đơn giá {SelectedServiceItem.DonGia:N0}. Bạn có muốn cập nhật đơn giá mới là {DichVu.DonGia:N0} không?", "Cập nhật đơn giá", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return; // Không cập nhật, kết thúc lệnh
                    }
                }

                var NewServiceItem = new CHITIETDVDTO
                {
                    DichVu = DichVu,
                    DonGia = DichVu.DonGia,
                    SoLuong = int.Parse(DV_SoLuong),
                    GhiChu = DV_GhiChu
                };
                int index = ServiceList.IndexOf(SelectedServiceItem);
                ServiceList[index] = null; // Remove the old item to trigger UI update
                ServiceList[index] = NewServiceItem; // Add the updated item back

            });
            DeleteServiceCommand = new RelayCommand<object>((p) =>
            {
                // Nếu SelectedServiceItem là null, không thể xóa
                return SelectedServiceItem != null;
            }
            , (p) =>
            {
                ServiceList.Remove(SelectedServiceItem);
                SelectedServiceItem = null;
            });

            ShowInvoiceCommand = new RelayCommand<object>((p) => true, (p) =>
            {

                var invoiceView = new InvoiceView() {
                    DataContext = new InvoiceViewModel(_maPhieuDat)
                };
                invoiceView.ShowDialog();
                    
                IsEditing = false; // Reset editing mode after showing invoice
                LoadWeddingDetail(maPhieuDat); // Reload wedding details to reflect any changes
              
            });

        }

        // Load wedding detail by ID (if needed)
        private void LoadWeddingDetail(int maPhieuDat)
        {
            _phieuDatTiecService = new PhieuDatTiecService(); // Refresh service to get updated data
            var wedding = _phieuDatTiecService.GetById(maPhieuDat);
            if (wedding != null)
            {
                TenChuRe = wedding.TenChuRe;
                TenCoDau = wedding.TenCoDau;
                DienThoai = wedding.DienThoai;
                NgayDaiTiec = wedding.NgayDaiTiec;
                NgayDatTiec = (DateTime)wedding.NgayDatTiec;
                SelectedCa = CaList.FirstOrDefault(c => c.MaCa == wedding.MaCa);
                //SelectedSanh = SanhList.FirstOrDefault(s => s.MaSanh == wedding.MaSanh);
                SanhList = new ObservableCollection<SANHDTO>(_sanhService.GetAll()); // Refresh SanhList
                
                var sanh = SanhList.FirstOrDefault(s => s.MaSanh == wedding.MaSanh);
                if (sanh.LoaiSanh.DonGiaBanToiThieu != wedding.DonGiaBanTiec)
                {
                    // thêm vào 1 sảnh tương tự với đơn giá là đơn gián bàn tiệc đã đặt
                    var sanhMoi = new SANHDTO
                    {
                        MaSanh = sanh.MaSanh,
                        TenSanh = sanh.TenSanh + " cũ",
                        LoaiSanh = new LOAISANHDTO
                        {
                            MaLoaiSanh = sanh.LoaiSanh.MaLoaiSanh,
                            TenLoaiSanh = sanh.LoaiSanh.TenLoaiSanh,
                            DonGiaBanToiThieu = wedding.DonGiaBanTiec
                        },
                        SoLuongBanToiDa = sanh.SoLuongBanToiDa,
                    };
                    // Thêm vào đầu danh sách
                    SanhList.Insert(0, sanhMoi);
                    SelectedSanh = sanhMoi;
                }
                else
                {
                    SelectedSanh = sanh;
                }
                TienDatCoc = wedding.TienDatCoc.ToString();
                SoLuongBan = wedding.SoLuongBan.ToString();
                SoBanDuTru = wedding.SoBanDuTru.ToString();
                // Load menu and service details
                MenuList = new ObservableCollection<THUCDONDTO>(_thucDonService.GetByPhieuDat(maPhieuDat));
                ServiceList = new ObservableCollection<CHITIETDVDTO>(_chiTietDichVuService.GetByPhieuDat(maPhieuDat));
            }
        }

        // Menu logic
        private void LoadMenuDetail()
        {
            if (SelectedMenuItem != null)
            {
                MonAn = SelectedMenuItem.MonAn;
                TD_DonGia = SelectedMenuItem.DonGia?.ToString("N0"); // Format as currency
                TD_SoLuong = SelectedMenuItem.SoLuong?.ToString();
                TD_GhiChu = SelectedMenuItem.GhiChu;
                OnPropertyChanged(nameof(MonAn));
            }
            else
            {
                MonAn = new MONANDTO();
                TD_DonGia = string.Empty;
                TD_SoLuong = string.Empty;
                TD_GhiChu = string.Empty;
                OnPropertyChanged(nameof(MonAn));
            }
        }

        private void ResetTD()
        {
            MonAn = new MONANDTO();
            TD_DonGia = string.Empty;
            TD_SoLuong = string.Empty;
            TD_GhiChu = string.Empty;
            SelectedMenuItem = null;
            OnPropertyChanged(nameof(MonAn));
        }

        private bool CanAddMenu()
        {
            return MonAn != null && !string.IsNullOrWhiteSpace(MonAn.TenMonAn)
                && int.TryParse(TD_SoLuong, out int sl) && sl > 0
                && MenuList.All(m => m.MonAn.MaMonAn != MonAn.MaMonAn);
        }

        private void AddMenu()
        {
            MenuList.Add(new THUCDONDTO
            {
                MonAn = MonAn,
                SoLuong = int.Parse(TD_SoLuong),
                GhiChu = TD_GhiChu
            });
            ResetTD();
        }

        private bool CanEditMenu()
        {
            if (SelectedMenuItem == null) return false;
            if (MonAn == null || string.IsNullOrWhiteSpace(MonAn.TenMonAn) || !int.TryParse(TD_SoLuong, out int sl) || sl <= 0)
                return false;
            var existing = MenuList.FirstOrDefault(m => m.MonAn.MaMonAn == MonAn.MaMonAn);
            if (existing != null && existing != SelectedMenuItem) return false;
            return true;
        }

        private void EditMenu()
        {
            if (SelectedMenuItem == null) return;
            SelectedMenuItem.MonAn = MonAn;
            SelectedMenuItem.SoLuong = int.Parse(TD_SoLuong);
            SelectedMenuItem.GhiChu = TD_GhiChu;
            // Force UI update
            var idx = MenuList.IndexOf(SelectedMenuItem);
            MenuList[idx] = SelectedMenuItem;
            ResetTD();
        }

        private void DeleteMenu()
        {
            if (SelectedMenuItem != null)
            {
                MenuList.Remove(SelectedMenuItem);
                ResetTD();
            }
        }

        private void ChonMonAn()
        {
            var monAnDialog = new MenuItemView();
            var viewModel = new MenuItemViewModel();
            monAnDialog.DataContext = viewModel;
            if (monAnDialog.ShowDialog() == true)
            {
                MonAn = viewModel.SelectedFood;
                TD_DonGia = MonAn.DonGia?.ToString("N0"); // Format as currency
                TD_SoLuong = "1";
                OnPropertyChanged(nameof(MonAn));
            }
        }

        // Service logic
        private void LoadServiceDetail()
        {
            if (SelectedServiceItem != null)
            {
                DichVu = SelectedServiceItem.DichVu;
                DV_DonGia = SelectedServiceItem.DonGia?.ToString("N0"); // Format as currency
                DV_SoLuong = SelectedServiceItem.SoLuong?.ToString();
                DV_GhiChu = SelectedServiceItem.GhiChu;
                OnPropertyChanged(nameof(DichVu));
            }
            else
            {
                DichVu = new DICHVUDTO();
                DV_DonGia = string.Empty;
                DV_SoLuong = string.Empty;
                DV_GhiChu = string.Empty;
                OnPropertyChanged(nameof(DichVu));
            }
        }

        private void ResetCTDV()
        {
            DichVu = new DICHVUDTO();
            DV_DonGia = string.Empty;
            DV_SoLuong = string.Empty;
            DV_GhiChu = string.Empty;
            SelectedServiceItem = null;
            OnPropertyChanged(nameof(DichVu));
        }

        private bool CanAddService()
        {
            return DichVu != null && !string.IsNullOrWhiteSpace(DichVu.TenDichVu)
                && int.TryParse(DV_SoLuong, out int sl) && sl > 0
                && ServiceList.All(s => s.DichVu.MaDichVu != DichVu.MaDichVu);
        }

        private void AddService()
        {
            ServiceList.Add(new CHITIETDVDTO
            {
                DichVu = DichVu,
                SoLuong = int.Parse(DV_SoLuong),
                GhiChu = DV_GhiChu
            });
            ResetCTDV();
        }

        private bool CanEditService()
        {
            if (SelectedServiceItem == null) return false;
            if (DichVu == null || string.IsNullOrWhiteSpace(DichVu.TenDichVu) || !int.TryParse(DV_SoLuong, out int sl) || sl <= 0)
                return false;
            var existing = ServiceList.FirstOrDefault(s => s.DichVu.MaDichVu == DichVu.MaDichVu);
            if (existing != null && existing != SelectedServiceItem) return false;
            return true;
        }

        private void EditService()
        {
            if (SelectedServiceItem == null) return;
            SelectedServiceItem.DichVu = DichVu;
            SelectedServiceItem.SoLuong = int.Parse(DV_SoLuong);
            SelectedServiceItem.GhiChu = DV_GhiChu;
            var idx = ServiceList.IndexOf(SelectedServiceItem);
            ServiceList[idx] = SelectedServiceItem;
            ResetCTDV();
        }

        private void DeleteService()
        {
            if (SelectedServiceItem != null)
            {
                ServiceList.Remove(SelectedServiceItem);
                ResetCTDV();
            }
        }

        private void ChonDichVu()
        {
            var dichVuDialog = new ServiceDetailItemView();
            var viewModel = new ServiceDetailItemViewModel();
            dichVuDialog.DataContext = viewModel;
            if (dichVuDialog.ShowDialog() == true)
            {
                DichVu = viewModel.SelectedService;
                DV_DonGia = DichVu.DonGia?.ToString("N0"); // Format as currency
                DV_SoLuong = "1";
                OnPropertyChanged(nameof(DichVu));
            }
        }

        // Edit/Cancel logic
        private void ConfirmEdit()
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
            // Kiểm tra ngày đãi tiệc phải là ngày mai hoặc ngày sau đó
            if (NgayDaiTiec.Value.Date < DateTime.Today.AddDays(1))
            {
                MessageBox.Show("Ngày đãi tiệc phải là ngày mai hoặc ngày sau đó.", "Lỗi ngày", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra nếu không có gì thay đổi (bao gồm cả menu và dịch vụ) thì không cần cập nhật
            var existingWedding = _phieuDatTiecService.GetById(_maPhieuDat);
            var existingMenu = _thucDonService.GetByPhieuDat(_maPhieuDat);
            var existingServices = _chiTietDichVuService.GetByPhieuDat(_maPhieuDat);
            // Xử lý kiểu dữ liệu của 2 list đang khác nhau giữa list cần so sánh và list đã sửa, nên cần chuyển đổi
            var updatedMenu = MenuList.Select(m => new THUCDONDTO
            {
                MaPhieuDat = _maPhieuDat,
                MonAn = m.MonAn,
                DonGia = m.DonGia,
                SoLuong = m.SoLuong,
                GhiChu = m.GhiChu
            }).ToList();

            var updatedServices = ServiceList.Select(s => new CHITIETDVDTO
            {
                MaPhieuDat = _maPhieuDat,
                DonGia = s.DonGia,
                DichVu = s.DichVu,
                SoLuong = s.SoLuong,
                GhiChu = s.GhiChu
            }).ToList();

            // Để kiểm tra xem sảnh có đổi hay không, ta cần so sánh MaSanh, nếu khác thì  nghĩa là đã đổi sảnh
            // Nếu giống, tiếp tục kiểm tra thử sảnh mới chọn có khác giá với đơn giá bàn tiệc đã đặt hay không, nếu không khác thì không cần cập nhật,
            // nếu khác thì cần cập nhật đơn giá bàn tiệc mới
            //var sanhKhongBiDoi = false;
            //if (SelectedSanh.MaSanh == existingWedding.MaSanh)
            //{
            //    // Kiểm tra đơn giá bàn tiệc có khác không
            //    if (SelectedSanh.LoaiSanh.DonGiaBanToiThieu == existingWedding.DonGiaBanTiec)
            //    {
            //        sanhKhongBiDoi = true;
            //    }
            //}
            if (existingWedding != null &&
                existingWedding.TenChuRe == TenChuRe &&
                existingWedding.TenCoDau == TenCoDau &&
                existingWedding.DienThoai == DienThoai &&
                existingWedding.NgayDaiTiec == NgayDaiTiec &&
                existingWedding.NgayDatTiec.Value.Date == NgayDatTiec.Date &&
                existingWedding.MaCa == SelectedCa.MaCa &&
                //existingWedding.MaSanh == SelectedSanh.MaSanh &&
                SelectedSanh.MaSanh == existingWedding.MaSanh &&
                SelectedSanh.LoaiSanh.DonGiaBanToiThieu == existingWedding.DonGiaBanTiec &&
                existingWedding.TienDatCoc.ToString() == TienDatCoc &&
                existingWedding.SoLuongBan.ToString() == SoLuongBan &&
                existingWedding.SoBanDuTru.ToString() == SoBanDuTru &&
                updatedMenu.SequenceEqual(existingMenu) &&
                updatedServices.SequenceEqual(existingServices))
            {
                MessageBox.Show("Không có thay đổi nào để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 2. Kiểm tra trùng ngày, sảnh (trừ chính mình)
            var existingWedding1 = _phieuDatTiecService.GetAll()
                .FirstOrDefault(w => w.MaPhieuDat != _maPhieuDat &&
                                     w.NgayDaiTiec.HasValue && NgayDaiTiec.HasValue &&
                                     w.NgayDaiTiec.Value.Date == NgayDaiTiec.Value.Date &&
                                     w.MaSanh == SelectedSanh.MaSanh);
            if (existingWedding1 != null)
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
                SoLuongBan = soLuongBanToiDa.ToString();
                return;
            }
            if (soLuongBan < (tiLeSoBanDatTruocToiThieu * soLuongBanToiDa))
            {
                MessageBox.Show($"Số lượng bàn phải lớn hơn hoặc bằng {Math.Ceiling(tiLeSoBanDatTruocToiThieu * soLuongBanToiDa)} (tỉ lệ đặt trước tối thiểu).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                SoLuongBan = Math.Ceiling(tiLeSoBanDatTruocToiThieu * soLuongBanToiDa).ToString();
                return;
            }

            // 4. Kiểm tra số lượng bàn dự trữ hợp lệ
            var soBanDuTruToiDa = soLuongBanToiDa - soLuongBan;
            if (!int.TryParse(SoBanDuTru, out int soBanDuTru) || soBanDuTru < 0 || soBanDuTru > soBanDuTruToiDa)
            {
                MessageBox.Show($"Số bàn dự trữ phải là số nguyên dương và không vượt quá {soBanDuTruToiDa}.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                SoBanDuTru = soBanDuTruToiDa.ToString();
                return;
            }

            // Kiểm tra formatter số điện thoại (10 số hoặc 11 số bắt đầu bằng 0 hoặc 84)
            if (!string.IsNullOrWhiteSpace(DienThoai) && !System.Text.RegularExpressions.Regex.IsMatch(DienThoai, @"^(0|\+84)[0-9]{9,10}$"))
            {
                MessageBox.Show("Số điện thoại phải là 10 hoặc 11 chữ số và bắt đầu bằng 0 hoặc +84.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                DienThoai = "0123456789";
                return;
            }

            // 5. Kiểm tra tiền đặt cọc hợp lệ
            var tongDonGiaMonAn = MenuList.Sum(m => (m.DonGia ?? 0) * (m.SoLuong ?? 0));
            var tongDonGiaDichVu = ServiceList.Sum(s => (s.DonGia ?? 0) * (s.SoLuong ?? 0));
            var donGiaBanToiThieu = existingWedding.DonGiaBanTiec ?? 0;
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

            // 6. Lưu dữ liệu
            try
            {
                // Cập nhật thông tin tiệc cưới
                var phieuDatTiec = _phieuDatTiecService.GetById(_maPhieuDat);
                if (phieuDatTiec == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin tiệc cưới để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                phieuDatTiec.TenChuRe = TenChuRe;
                phieuDatTiec.TenCoDau = TenCoDau;
                phieuDatTiec.DienThoai = DienThoai;
                //phieuDatTiec.NgayDaiTiec = NgayDaiTiec;
                // giờ đãi tiệc là thời gian bắt đầu của ca
                phieuDatTiec.NgayDaiTiec = NgayDaiTiec.Value.Date.Add(SelectedCa.ThoiGianBatDauCa.Value);
                phieuDatTiec.NgayDatTiec = NgayDatTiec;
                phieuDatTiec.MaCa = SelectedCa.MaCa;
                phieuDatTiec.MaSanh = SelectedSanh.MaSanh;
                phieuDatTiec.DonGiaBanTiec = SelectedSanh.LoaiSanh.DonGiaBanToiThieu;
                phieuDatTiec.TienDatCoc = tienDatCoc;
                phieuDatTiec.SoLuongBan = soLuongBan;
                phieuDatTiec.SoBanDuTru = soBanDuTru;

                _phieuDatTiecService.Update(phieuDatTiec);

                // Cập nhật thực đơn
                var oldMenus = _thucDonService.GetByPhieuDat(_maPhieuDat).ToList();
                foreach (var old in oldMenus)
                    _thucDonService.Delete(_maPhieuDat, old.MaMonAn);
                foreach (var item in MenuList)
                {
                    var thucDon = new THUCDONDTO
                    {
                        MaPhieuDat = _maPhieuDat,
                        MaMonAn = item.MonAn.MaMonAn,
                        DonGia = item.DonGia,
                        SoLuong = item.SoLuong,
                        GhiChu = item.GhiChu,
                        MonAn = item.MonAn
                    };
                    _thucDonService.Create(thucDon);
                }

                // Cập nhật dịch vụ
                var oldServices = _chiTietDichVuService.GetByPhieuDat(_maPhieuDat).ToList();
                foreach (var old in oldServices)
                    _chiTietDichVuService.Delete(_maPhieuDat, old.MaDichVu);
                foreach (var item in ServiceList)
                {
                    var chiTietDV = new CHITIETDVDTO
                    {
                        MaPhieuDat = _maPhieuDat,
                        MaDichVu = item.DichVu.MaDichVu,
                        DonGia = item.DonGia,
                        SoLuong = item.SoLuong,
                        ThanhTien = (item.DonGia ?? 0) * (item.SoLuong ?? 0),
                        GhiChu = item.GhiChu,
                        DichVu = item.DichVu
                    };
                    _chiTietDichVuService.Create(chiTietDV);
                }

                IsEditing = false;
                LoadWeddingDetail(_maPhieuDat); // Reload wedding details to reflect any changes
                MessageBox.Show("Cập nhật thông tin tiệc cưới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
        }

        private void CancelEdit()
        {
            // Reload original data or revert changes
            IsEditing = false;
            LoadWeddingDetail(_maPhieuDat);
            MessageBox.Show("Đã hủy chỉnh sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}