using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Helpers;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.Presentation.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel {
    public class BillViewModel : BaseViewModel {
        private IPhieuDatTiecService _phieuDatTiecService;
        private IThucDonService _thucDonService;
        private IChiTietDVService _chiTietDichVuService;

        private ObservableCollection<PHIEUDATTIECDTO> _List;
        public ObservableCollection<PHIEUDATTIECDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<PHIEUDATTIECDTO> _OriginalList;
        public ObservableCollection<PHIEUDATTIECDTO> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private PHIEUDATTIECDTO _SelectedItem;
        public PHIEUDATTIECDTO SelectedItem {
            get => _SelectedItem;
            set { _SelectedItem = value; OnPropertyChanged(); }
        }

        // Filter properties
        public ObservableCollection<string> TenChuReFilterList { get; set; }
        public string SelectedTenChuRe { get => _selectedTenChuRe; set { _selectedTenChuRe = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedTenChuRe;

        public ObservableCollection<string> TenCoDauFilterList { get; set; }
        public string SelectedTenCoDau { get => _selectedTenCoDau; set { _selectedTenCoDau = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedTenCoDau;

        public ObservableCollection<string> TenSanhFilterList { get; set; }
        public string SelectedTenSanh { get => _selectedTenSanh; set { _selectedTenSanh = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedTenSanh;

        public ObservableCollection<int> SoLuongBanFilterList { get; set; }
        public int? SelectedSoLuongBan { get => _selectedSoLuongBan; set { _selectedSoLuongBan = value; OnPropertyChanged(); PerformSearch(); } }
        private int? _selectedSoLuongBan;

        public DateTime? SelectedNgayDaiTiec { get => _selectedNgayDaiTiec; set { _selectedNgayDaiTiec = value; OnPropertyChanged(); PerformSearch(); } }
        private DateTime? _selectedNgayDaiTiec;

        // Search properties
        public ObservableCollection<string> SearchProperties { get; set; }
        public string SelectedSearchProperty { get => _selectedSearchProperty; set { _selectedSearchProperty = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedSearchProperty;

        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); PerformSearch(); } }
        private string _searchText;

        // Command properties
        public ICommand CancelCommand { get; set; }
        public ICommand ExportCommand { get; set; }

        private MainViewModel mainViewModel;

        public BillViewModel() {

            _phieuDatTiecService = new PhieuDatTiecService();
            _thucDonService = new ThucDonService();
            _chiTietDichVuService = new ChiTietDVService();

            var all = _phieuDatTiecService.GetAll().ToList();
            List = new ObservableCollection<PHIEUDATTIECDTO>(all);
            OriginalList = new ObservableCollection<PHIEUDATTIECDTO>(all);

            // Build filter lists
            TenChuReFilterList = new ObservableCollection<string>(all.Select(x => x.TenChuRe).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
            TenCoDauFilterList = new ObservableCollection<string>(all.Select(x => x.TenCoDau).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
            TenSanhFilterList = new ObservableCollection<string>(all.Select(x => x.Sanh?.TenSanh).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
            SoLuongBanFilterList = new ObservableCollection<int>(all.Select(x => x.SoLuongBan ?? 0).Where(x => x > 0).Distinct().OrderBy(x => x));

            SearchProperties = new ObservableCollection<string>
            {
                "Tên chú rể",
                "Tên cô dâu",
                "Tên sảnh",
                "Ngày đãi tiệc",
                "Số lượng bàn"
            };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            ExportCommand = new RelayCommand<Window>((p) => true, (p) => {
                try {
                    if (SelectedItem == null) {
                        return;
                    }
                    int maPhieuDat = SelectedItem.MaPhieuDat;
                    var dialog = new Microsoft.Win32.SaveFileDialog {
                        FileName = $"HoaDon_{maPhieuDat}",
                        DefaultExt = ".pdf",
                        Filter = "PDF documents (.pdf)|*.pdf"
                    };
                    bool? result = dialog.ShowDialog();
                    if (result == true) {
                        string filePath = dialog.FileName;
                        // Xuất PDF ở filePath
                        PdfExportHelper.ExportInvoice(SelectedItem, filePath);
                        MessageBox.Show("Xuất hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi xuất hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            CancelCommand = new RelayCommand<Window>((p) => true, (p) => {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn hủy không?", "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) {
                    if (p is Window window) {
                        window.Close();
                    }
                }
            });
        }

        private void PerformSearch() {
            try {
                var filtered = OriginalList.AsEnumerable();

                // Filter by ComboBox filters
                if (!string.IsNullOrEmpty(SelectedTenChuRe))
                    filtered = filtered.Where(x => x.TenChuRe == SelectedTenChuRe);
                if (!string.IsNullOrEmpty(SelectedTenCoDau))
                    filtered = filtered.Where(x => x.TenCoDau == SelectedTenCoDau);
                if (!string.IsNullOrEmpty(SelectedTenSanh))
                    filtered = filtered.Where(x => x.Sanh != null && x.Sanh.TenSanh == SelectedTenSanh);
                if (SelectedNgayDaiTiec.HasValue)
                    filtered = filtered.Where(x => x.NgayDaiTiec.HasValue && x.NgayDaiTiec.Value.Date == SelectedNgayDaiTiec.Value.Date);
                if (SelectedSoLuongBan.HasValue)
                    filtered = filtered.Where(x => x.SoLuongBan == SelectedSoLuongBan);

                // Search by text
                if (!string.IsNullOrEmpty(SearchText) && !string.IsNullOrEmpty(SelectedSearchProperty)) {
                    var search = SearchText.Trim().ToLower();
                    switch (SelectedSearchProperty) {
                        case "Tên chú rể":
                            filtered = filtered.Where(x => !string.IsNullOrEmpty(x.TenChuRe) && x.TenChuRe.ToLower().Contains(search));
                            break;
                        case "Tên cô dâu":
                            filtered = filtered.Where(x => !string.IsNullOrEmpty(x.TenCoDau) && x.TenCoDau.ToLower().Contains(search));
                            break;
                        case "Tên sảnh":
                            filtered = filtered.Where(x => x.Sanh != null && !string.IsNullOrEmpty(x.Sanh.TenSanh) && x.Sanh.TenSanh.ToLower().Contains(search));
                            break;
                        case "Ngày đãi tiệc":
                            filtered = filtered.Where(x => x.NgayDaiTiec.HasValue && x.NgayDaiTiec.Value.ToString("dd/MM/yyyy").Contains(search));
                            break;
                        case "Số lượng bàn":
                            filtered = filtered.Where(x => x.SoLuongBan.HasValue && x.SoLuongBan.Value.ToString().Contains(search));
                            break;
                        default:
                            break;
                    }
                }

                List = new ObservableCollection<PHIEUDATTIECDTO>(filtered);
            }
            catch (Exception ex) {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
