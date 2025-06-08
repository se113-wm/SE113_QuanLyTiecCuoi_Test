using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class AddWeddingViewModel : BaseViewModel
    {
        private readonly ISanhService _sanhService;
        private readonly ICaService _caService;
        private readonly IPhieuDatTiecService _phieuDatTiecService;
        private readonly IMonAnService _monAnService;
        private readonly IDichVuService _dichVuService;
        private readonly IThucDonService _thucDonService;
        private readonly IChiTietDVService _chiTietDichVuService;
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

        public AddWeddingViewModel()
        {
            // Initialize services
            _sanhService = new SanhService();
            _caService = new CaService();
            _phieuDatTiecService = new PhieuDatTiecService();
            _monAnService = new MonAnService();
            _dichVuService = new DichVuService();
            _thucDonService = new ThucDonService();
            _chiTietDichVuService = new ChiTietDVService();

            // Example data for Ca and Sanh
            CaList = new ObservableCollection<CADTO>(_caService.GetAll());
            SanhList = new ObservableCollection<SANHDTO>(_sanhService.GetAll());

            ResetWeddingCommand = new RelayCommand<object>((p) => true, (p) => ResetWedding());
            ResetTDCommand = new RelayCommand<object>((p) => true, (p) => ResetTD());
            ResetCTDVCommand = new RelayCommand<object>((p) => true, (p) => ResetCTDV());

            // Menu commands
            ChonMonAnCommand = new RelayCommand<object>((p) => true, (p) => ChonMonAn());
            AddMenuCommand = new RelayCommand<object>((p) => CanAddMenu(), (p) => AddMenu());
            EditMenuCommand = new RelayCommand<object>((p) => SelectedMenuItem != null, (p) => EditMenu());
            DeleteMenuCommand = new RelayCommand<object>((p) => SelectedMenuItem != null, (p) => DeleteMenu());

            // Service commands
            ChonDichVuCommand = new RelayCommand<object>((p) => true, (p) => ChonDichVu());
            AddServiceCommand = new RelayCommand<object>((p) => CanAddService(), (p) => AddService());
            EditServiceCommand = new RelayCommand<object>((p) => SelectedServiceItem != null, (p) => EditService());
            DeleteServiceCommand = new RelayCommand<object>((p) => SelectedServiceItem != null, (p) => DeleteService());

            // Confirm/Cancel
            ConfirmCommand = new RelayCommand<object>((p) => CanConfirm(), (p) => Confirm());
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
            var viewModel = new MenuItemViewModel();
            monAnDialog.DataContext = viewModel;
            if (monAnDialog.ShowDialog() == true)
            {
                MonAn = viewModel.SelectedFood;
                TD_SoLuong = "1"; // Default quantity
                OnPropertyChanged(nameof(MonAn));
            }
        }
        private bool CanAddMenu()
        {
            return MonAn != null && !string.IsNullOrWhiteSpace(MonAn.TenMonAn) && int.TryParse(TD_SoLuong, out int sl) && sl > 0;
        }
        private void AddMenu()
        {
            MenuList.Add(new THUCDONDTO
            {
                MonAn = MonAn,
                SoLuong = int.Parse(TD_SoLuong),
                GhiChu = TD_GhiChu
            });
            TD_SoLuong = string.Empty;
            TD_GhiChu = string.Empty;
            MonAn = new MONANDTO();
            OnPropertyChanged(nameof(MonAn));
        }
        private void EditMenu()
        {
            if (SelectedMenuItem != null)
            {
                SelectedMenuItem.MonAn = MonAn;
                SelectedMenuItem.SoLuong = int.Parse(TD_SoLuong);
                SelectedMenuItem.GhiChu = TD_GhiChu;
            }
        }
        private void DeleteMenu()
        {
            if (SelectedMenuItem != null)
            {
                MenuList.Remove(SelectedMenuItem);
                SelectedMenuItem = null;
            }
        }

        // Service logic
        private void ChonDichVu()
        {
            // Open service selection dialog (implement as needed)
        }
        private bool CanAddService()
        {
            return DichVu != null && !string.IsNullOrWhiteSpace(DichVu.TenDichVu) && int.TryParse(DV_SoLuong, out int sl) && sl > 0;
        }
        private void AddService()
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
        }
        private void EditService()
        {
            if (SelectedServiceItem != null)
            {
                SelectedServiceItem.DichVu = DichVu;
                SelectedServiceItem.SoLuong = int.Parse(DV_SoLuong);
                SelectedServiceItem.GhiChu = DV_GhiChu;
            }
        }
        private void DeleteService()
        {
            if (SelectedServiceItem != null)
            {
                ServiceList.Remove(SelectedServiceItem);
                SelectedServiceItem = null;
            }
        }

        // Confirm/Cancel logic
        private bool CanConfirm()
        {
            return !string.IsNullOrWhiteSpace(TenChuRe)
                && !string.IsNullOrWhiteSpace(TenCoDau)
                && !string.IsNullOrWhiteSpace(DienThoai)
                && NgayDaiTiec != null
                && SelectedCa != null
                && SelectedSanh != null
                && int.TryParse(TienDatCoc, out _)
                && int.TryParse(SoLuongBan, out _)
                && int.TryParse(SoBanDuTru, out _)
                && MenuList.Count > 0;
        }
        private void Confirm()
        {
            // Save wedding logic here
            MessageBox.Show("Đặt tiệc cưới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}