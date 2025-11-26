using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class WeddingViewModel : BaseViewModel
    {
        private readonly IPhieuDatTiecService _phieuDatTiecService;
        private readonly IThucDonService _thucDonService;
        private readonly IChiTietDVService _chiTietDichVuService;
        private readonly MainViewModel _mainViewModel;

        private ObservableCollection<PHIEUDATTIECDTO> _List;
        public ObservableCollection<PHIEUDATTIECDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<PHIEUDATTIECDTO> _OriginalList;
        public ObservableCollection<PHIEUDATTIECDTO> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private PHIEUDATTIECDTO _SelectedItem;
        public PHIEUDATTIECDTO SelectedItem
        {
            get => _SelectedItem;
            set { _SelectedItem = value; OnPropertyChanged(); }
        }

        // Filter properties
        public List<string> TrangThaiFilterList { get; } = new List<string>
        {
            "Tất cả",
            "Chưa tổ chức",
            "Chưa thanh toán",
            "Trễ thanh toán",
            "Đã thanh toán"
        };

        private string _selectedTrangThai = "Tất cả";
        public string SelectedTrangThai
        {
            get => _selectedTrangThai;
            set
            {
                if (_selectedTrangThai != value)
                {
                    _selectedTrangThai = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }
        
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
        public ICommand AddCommand { get; set; }
        public ICommand DetailCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => Reset());

        private string _DeleteMessage;
        public string DeleteMessage { get => _DeleteMessage; set { _DeleteMessage = value; OnPropertyChanged(); } }

        // Constructor với Dependency Injection
        public WeddingViewModel(
            MainViewModel mainViewModel, 
            IPhieuDatTiecService phieuDatTiecService, 
            IThucDonService thucDonService, 
            IChiTietDVService chiTietDVService)
        {
            _mainViewModel = mainViewModel;
            _phieuDatTiecService = phieuDatTiecService;
            _thucDonService = thucDonService;
            _chiTietDichVuService = chiTietDVService;

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
                "Số lượng bàn",
                "Trạng thái"
            };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                AddCommandFunc();
            });

            DetailCommand = new RelayCommand<object>((p) => SelectedItem != null, (p) =>
            {
                // Sử dụng Factory Method để tạo WeddingDetailViewModel
                _mainViewModel.CurrentView = new WeddingDetailView
                {
                    DataContext = Infrastructure.ServiceContainer.CreateWeddingDetailViewModel(SelectedItem.MaPhieuDat)
                };
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                {
                    DeleteMessage = "Vui lòng chọn một tiệc cưới để xóa.";
                    return false;
                }
                if (SelectedItem.NgayThanhToan.HasValue)
                {
                    DeleteMessage = "Không thể xóa tiệc cưới đã thanh toán.";
                    return false;
                }
                // chỉ cho xóa những tiệc cưới vào ngày mai hoặc tương lai
                if (SelectedItem.NgayDaiTiec.HasValue && SelectedItem.NgayDaiTiec.Value.Date < DateTime.Today.AddDays(1))
                {
                    DeleteMessage = "Không thể xóa tiệc cưới tổ chức vào hôm nay và đã tổ chức.";
                    return false;
                }
                DeleteMessage = string.Empty;
                return true;
            }
            , (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa tiệc cưới này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Xóa thực đơn liên quan
                        var thucDonList = _thucDonService.GetByPhieuDat(SelectedItem.MaPhieuDat).ToList();
                        foreach (var thucDon in thucDonList)
                        {
                            _thucDonService.Delete(SelectedItem.MaPhieuDat, thucDon.MaMonAn);
                        }
                        // Xóa chi tiết dịch vụ liên quan
                        var chiTietDVList = _chiTietDichVuService.GetByPhieuDat(SelectedItem.MaPhieuDat).ToList();
                        foreach (var chiTietDV in chiTietDVList)
                        {
                            _chiTietDichVuService.Delete(SelectedItem.MaPhieuDat, chiTietDV.MaDichVu);
                        }
                        // Xóa phiếu đặt tiệc
                        _phieuDatTiecService.Delete(SelectedItem.MaPhieuDat);
                        List.Remove(SelectedItem);
                        RefreshList();
                        MessageBox.Show("Xóa tiệc cưới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    DeleteMessage = $"Lỗi khi xóa: {ex.Message}";
                }
            });
        }

        private void PerformSearch()
        {
            try
            {
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

                // Filter by Trạng thái
                if (!string.IsNullOrEmpty(SelectedTrangThai) && SelectedTrangThai != "Tất cả")
                    filtered = filtered.Where(x => x.TrangThai == SelectedTrangThai);

                // Search by text
                if (!string.IsNullOrEmpty(SearchText) && !string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    var search = SearchText.Trim().ToLower();
                    switch (SelectedSearchProperty)
                    {
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
                        case "Trạng thái":
                            filtered = filtered.Where(x => !string.IsNullOrEmpty(x.TrangThai) && x.TrangThai.ToLower().Contains(search));
                            break;
                        default:
                            break;
                    }
                }

                List = new ObservableCollection<PHIEUDATTIECDTO>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddCommandFunc()
        {
            // Sử dụng Factory Method để tạo AddWeddingViewModel
            var addView = new Presentation.View.AddWeddingView()
            {
                DataContext = Infrastructure.ServiceContainer.CreateAddWeddingViewModel()
            };
            addView.ShowDialog();
            RefreshList();
        }

        private void RefreshList()
        {
            // Sử dụng service đã inject thay vì tạo mới
            var all = _phieuDatTiecService.GetAll().ToList();
            List = new ObservableCollection<PHIEUDATTIECDTO>(all);
            OriginalList = new ObservableCollection<PHIEUDATTIECDTO>(all);
        }

        private void Reset()
        {
            SelectedItem = null;
            SelectedTenChuRe = null;
            SelectedTenCoDau = null;
            SelectedTenSanh = null;
            SelectedNgayDaiTiec = null;
            SelectedSoLuongBan = null;
            SearchText = string.Empty;
            DeleteMessage = string.Empty;
            PerformSearch();
        }
    }
}