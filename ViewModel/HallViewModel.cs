using QuanLyTiecCuoi.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    public class HallViewModel : BaseViewModel
    {
        private ObservableCollection<SANH> _List;
        public ObservableCollection<SANH> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<SANH> _OriginalList;
        public ObservableCollection<SANH> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private ObservableCollection<LOAISANH> _HallTypes;
        public ObservableCollection<LOAISANH> HallTypes { get => _HallTypes; set { _HallTypes = value; OnPropertyChanged(); } }

        private SANH _SelectedItem;
        public SANH SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    TenSanh = SelectedItem.TenSanh;
                    SoLuongBanToiDa = SelectedItem.SoLuongBanToiDa;
                    DonGiaBanToiThieu = SelectedItem.LOAISANH?.DonGiaBanToiThieu;
                    GhiChu = SelectedItem.GhiChu;
                    SelectedHallType = SelectedItem.LOAISANH;
                }
            }
        }

        private LOAISANH _SelectedHallType;
        public LOAISANH SelectedHallType
        {
            get => _SelectedHallType;
            set
            {
                _SelectedHallType = value;
                OnPropertyChanged();
                // Khi chọn loại sảnh, cập nhật đơn giá bàn tối thiểu (readonly ở view)
                DonGiaBanToiThieu = _SelectedHallType?.DonGiaBanToiThieu;
            }
        }

        private string _TenSanh;
        public string TenSanh { get => _TenSanh; set { _TenSanh = value; OnPropertyChanged(); } }

        private int? _SoLuongBanToiDa;
        public int? SoLuongBanToiDa { get => _SoLuongBanToiDa; set { _SoLuongBanToiDa = value; OnPropertyChanged(); } }

        private string _GhiChu;
        public string GhiChu { get => _GhiChu; set { _GhiChu = value; OnPropertyChanged(); } }

        private decimal? _DonGiaBanToiThieu;
        public decimal? DonGiaBanToiThieu { get => _DonGiaBanToiThieu; set { _DonGiaBanToiThieu = value; OnPropertyChanged(); } }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        private ObservableCollection<string> _SearchProperties;
        public ObservableCollection<string> SearchProperties
        {
            get => _SearchProperties;
            set { _SearchProperties = value; OnPropertyChanged(); }
        }

        private string _SelectedSearchProperty;
        public string SelectedSearchProperty
        {
            get => _SelectedSearchProperty;
            set
            {
                _SelectedSearchProperty = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }
        private void PerformSearch()
        {
            try
            {
                SelectedItem = null;
                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    List = OriginalList;
                    return;
                }

                switch (SelectedSearchProperty)
                {
                    case "Tên sảnh":
                        List = new ObservableCollection<SANH>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.TenSanh) &&
                                x.TenSanh.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Tên loại sảnh":
                        List = new ObservableCollection<SANH>(
                            OriginalList.Where(x => x.LOAISANH != null && !string.IsNullOrEmpty(x.LOAISANH.TenLoaiSanh) &&
                                x.LOAISANH.TenLoaiSanh.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Đơn giá bàn tối thiểu":
                        if (decimal.TryParse(SearchText, out var price))
                        {
                            List = new ObservableCollection<SANH>(
                                OriginalList.Where(x => x.LOAISANH != null && x.LOAISANH.DonGiaBanToiThieu == price));
                        }
                        else
                        {
                            List = new ObservableCollection<SANH>();
                        }
                        break;
                    case "Số lượng bàn tối đa":
                        if (int.TryParse(SearchText, out var sl))
                        {
                            List = new ObservableCollection<SANH>(
                                OriginalList.Where(x => x.SoLuongBanToiDa == sl));
                        }
                        else
                        {
                            List = new ObservableCollection<SANH>();
                        }
                        break;
                    case "Ghi chú":
                        List = new ObservableCollection<SANH>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.GhiChu) &&
                                x.GhiChu.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    default:
                        List = new ObservableCollection<SANH>(OriginalList);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public HallViewModel()
        {
            List = new ObservableCollection<SANH>(DataProvider.Ins.DB.SANHs);
            OriginalList = new ObservableCollection<SANH>(List);
            HallTypes = new ObservableCollection<LOAISANH>(DataProvider.Ins.DB.LOAISANHs);

            SearchProperties = new ObservableCollection<string>
            {
                "Tên sảnh",
                "Tên loại sảnh",
                "Đơn giá bàn tối thiểu",
                "Số lượng bàn tối đa",
                "Ghi chú"
            };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();


            AddCommand = new RelayCommand<object>((p) => CanAddOrEdit(), (p) =>
            {
                try
                {
                    var newHall = new SANH()
                    {
                        TenSanh = TenSanh,
                        SoLuongBanToiDa = SoLuongBanToiDa,
                        GhiChu = GhiChu,
                        MaLoaiSanh = SelectedHallType?.MaLoaiSanh
                    };

                    DataProvider.Ins.DB.SANHs.Add(newHall);
                    DataProvider.Ins.DB.SaveChanges();

                    // Load lại để cập nhật navigation property
                    List = new ObservableCollection<SANH>(DataProvider.Ins.DB.SANHs);
                    OriginalList = new ObservableCollection<SANH>(List);

                    Reset();
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            EditCommand = new RelayCommand<object>((p) => CanAddOrEdit(true), (p) =>
            {
                try
                {
                    var hall = DataProvider.Ins.DB.SANHs.SingleOrDefault(x => x.MaSanh == SelectedItem.MaSanh);
                    if (hall != null)
                    {
                        hall.TenSanh = TenSanh;
                        hall.SoLuongBanToiDa = SoLuongBanToiDa;
                        hall.GhiChu = GhiChu;
                        hall.MaLoaiSanh = SelectedHallType?.MaLoaiSanh;
                        DataProvider.Ins.DB.SaveChanges();

                        // Load lại để cập nhật navigation property
                        List = new ObservableCollection<SANH>(DataProvider.Ins.DB.SANHs);
                        OriginalList = new ObservableCollection<SANH>(List);

                        Reset();
                        MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                return SelectedItem != null;
            }, (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sảnh này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        DataProvider.Ins.DB.SANHs.Remove(SelectedItem);
                        DataProvider.Ins.DB.SaveChanges();

                        OriginalList.Remove(SelectedItem);
                        List.Remove(SelectedItem);
                        

                        Reset();
                        MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private bool CanAddOrEdit(bool isEdit = false)
        {
            if (string.IsNullOrWhiteSpace(TenSanh))
                return false;
            if (!SoLuongBanToiDa.HasValue || SoLuongBanToiDa <= 0)
                return false;
            if (SelectedHallType == null)
                return false;

            // Không cho trùng tên sảnh trong cùng loại sảnh
            string normalizedName = TenSanh.Trim();
            var query = DataProvider.Ins.DB.SANHs
                .Where(x => x.TenSanh.Trim() == normalizedName && x.MaLoaiSanh == SelectedHallType.MaLoaiSanh);

            //if (isEdit && SelectedItem != null)
            //    query = query.Where(x => x.MaSanh != SelectedItem.MaSanh);

            if (query.Any())
                return false;

            return true;
        }
        private void Reset()
        {
            SelectedItem = null;
            TenSanh = string.Empty;
            SoLuongBanToiDa = null;
            GhiChu = string.Empty;
            SelectedHallType = null;
        }
    }
}
