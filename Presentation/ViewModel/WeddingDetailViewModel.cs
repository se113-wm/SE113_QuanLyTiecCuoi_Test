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

        // Edit mode
        private bool _isEditing;
        public bool IsEditing { get => _isEditing; set { _isEditing = value; OnPropertyChanged(); } }

        // Menu Section
        public ObservableCollection<THUCDONDTO> MenuList { get; set; } = new ObservableCollection<THUCDONDTO>();
        private THUCDONDTO _selectedMenuItem;
        public THUCDONDTO SelectedMenuItem { get => _selectedMenuItem; set { _selectedMenuItem = value; OnPropertyChanged(); LoadMenuDetail(); } }

        public MONANDTO MonAn { get; set; } = new MONANDTO();
        private string _td_SoLuong;
        public string TD_SoLuong { get => _td_SoLuong; set { _td_SoLuong = value; OnPropertyChanged(); } }
        private string _td_GhiChu;
        public string TD_GhiChu { get => _td_GhiChu; set { _td_GhiChu = value; OnPropertyChanged(); } }

        // Service Section
        public ObservableCollection<CHITIETDVDTO> ServiceList { get; set; } = new ObservableCollection<CHITIETDVDTO>();
        private CHITIETDVDTO _selectedServiceItem;
        public CHITIETDVDTO SelectedServiceItem { get => _selectedServiceItem; set { _selectedServiceItem = value; OnPropertyChanged(); LoadServiceDetail(); } }

        public DICHVUDTO DichVu { get; set; } = new DICHVUDTO();
        private string _dv_SoLuong;
        public string DV_SoLuong { get => _dv_SoLuong; set { _dv_SoLuong = value; OnPropertyChanged(); } }
        private string _dv_GhiChu;
        public string DV_GhiChu { get => _dv_GhiChu; set { _dv_GhiChu = value; OnPropertyChanged(); } }

        // Commands
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
            // Commands
            ConfirmEditCommand = new RelayCommand<object>((p) => IsEditing, (p) => ConfirmEdit());
            CancelEditCommand = new RelayCommand<object>((p) => IsEditing, (p) => CancelEdit());
            ResetTDCommand = new RelayCommand<object>((p) => true, (p) => ResetTD());
            ResetCTDVCommand = new RelayCommand<object>((p) => true, (p) => ResetCTDV());

            ChonMonAnCommand = new RelayCommand<object>((p) => IsEditing, (p) => ChonMonAn());
            AddMenuCommand = new RelayCommand<object>((p) => IsEditing && CanAddMenu(), (p) => AddMenu());
            EditMenuCommand = new RelayCommand<object>((p) => IsEditing && CanEditMenu(), (p) => EditMenu());
            DeleteMenuCommand = new RelayCommand<object>((p) => IsEditing && SelectedMenuItem != null, (p) => DeleteMenu());

            ChonDichVuCommand = new RelayCommand<object>((p) => IsEditing, (p) => ChonDichVu());
            AddServiceCommand = new RelayCommand<object>((p) => IsEditing && CanAddService(), (p) => AddService());
            EditServiceCommand = new RelayCommand<object>((p) => IsEditing && CanEditService(), (p) => EditService());
            DeleteServiceCommand = new RelayCommand<object>((p) => IsEditing && SelectedServiceItem != null, (p) => DeleteService());
            
        }

        // Load wedding detail by ID (if needed)
        private void LoadWeddingDetail(int maPhieuDat)
        {
            var wedding = _phieuDatTiecService.GetById(maPhieuDat);
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
                TD_SoLuong = SelectedMenuItem.SoLuong?.ToString();
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

        private void ResetTD()
        {
            MonAn = new MONANDTO();
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
                DV_SoLuong = SelectedServiceItem.SoLuong?.ToString();
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

        private void ResetCTDV()
        {
            DichVu = new DICHVUDTO();
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
                DV_SoLuong = "1";
                OnPropertyChanged(nameof(DichVu));
            }
        }

        // Edit/Cancel logic
        private void ConfirmEdit()
        {
            // Save changes logic here (update to DB or service)
            IsEditing = false;
            MessageBox.Show("Cập nhật thông tin tiệc cưới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CancelEdit()
        {
            // Reload original data or revert changes
            IsEditing = false;
            MessageBox.Show("Đã hủy chỉnh sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}