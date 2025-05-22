using QuanLyTiecCuoi.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xaml;

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
                    SoLuongBanToiDa = SelectedItem.SoLuongBanToiDa.ToString();
                    DonGiaBanToiThieu = SelectedItem.LOAISANH?.DonGiaBanToiThieu;
                    GhiChu = SelectedItem.GhiChu;
                    SelectedHallType = SelectedItem.LOAISANH;
                }
                else
                {
                    AddMessage = string.Empty;
                    EditMessage = string.Empty;
                    DeleteMessage = string.Empty;
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

        private string _SoLuongBanToiDa;
        public string SoLuongBanToiDa { get => _SoLuongBanToiDa; set { _SoLuongBanToiDa = value; OnPropertyChanged(); } }

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
                TenSanh = string.Empty;
                SoLuongBanToiDa = null;
                GhiChu = string.Empty;
                SelectedHallType = null;
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
                        List = new ObservableCollection<SANH>(
                            OriginalList.Where(x =>
                                x.LOAISANH != null &&
                                x.LOAISANH.DonGiaBanToiThieu != null &&
                                x.LOAISANH.DonGiaBanToiThieu.Value.ToString("N0").Replace(",", "").Contains(SearchText.Replace(",", "").Trim())
                            )
                        );
                        break;
                    case "Số lượng bàn tối đa":
                        List = new ObservableCollection<SANH>(
                            OriginalList.Where(x =>
                                x.SoLuongBanToiDa != null &&
                                x.SoLuongBanToiDa.Value.ToString().Contains(SearchText.Trim())
                            )
                        );
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
        private string _AddMessage;
        public string AddMessage { get => _AddMessage; set { _AddMessage = value; OnPropertyChanged(); } }
        public ICommand EditCommand { get; set; }
        private string _EditMessage;
        public string EditMessage { get => _EditMessage; set { _EditMessage = value; OnPropertyChanged(); } }
        public ICommand DeleteCommand { get; set; }
        private string _DeleteMessage;
        public string DeleteMessage { get => _DeleteMessage; set { _DeleteMessage = value; OnPropertyChanged(); } }


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


            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrWhiteSpace(TenSanh))
                {
                    if (SelectedItem != null)
                    {
                        AddMessage = "Vui lòng nhập tên sảnh";
                    }
                    else
                    {
                        AddMessage = string.Empty;
                    }
                    return false;
                }
                if (SelectedHallType == null)
                {
                    AddMessage = "Vui lòng chọn loại sảnh";
                    return false;
                }
                if (SoLuongBanToiDa == null || !int.TryParse(SoLuongBanToiDa.ToString(), out int soLuong) || soLuong <= 0)
                {
                    AddMessage = "Số lượng bàn tối đa phải là số nguyên dương";
                    return false;
                }
                // Kiểm tra trùng tên sảnh trong cùng loại sảnh
                var exists = DataProvider.Ins.DB.SANHs.Any(x => x.TenSanh == TenSanh && x.MaLoaiSanh == SelectedHallType.MaLoaiSanh);
                if (exists)
                {
                    AddMessage = "Tên sảnh đã tồn tại trong loại sảnh này";
                    return false;
                }
                AddMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var newHall = new SANH()
                    {
                        TenSanh = TenSanh,
                        SoLuongBanToiDa = int.TryParse(SoLuongBanToiDa, out int soLuong) ? soLuong : (int?)null,
                        GhiChu = GhiChu,
                        MaLoaiSanh = SelectedHallType?.MaLoaiSanh
                    };

                    DataProvider.Ins.DB.SANHs.Add(newHall);
                    DataProvider.Ins.DB.SaveChanges();

                    List.Add(newHall);

                    Reset();
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                {
                    EditMessage = string.Empty;
                    return false;
                }
                // Không có thay đổi nào
                if (SelectedItem.TenSanh == TenSanh
                    && SelectedItem.SoLuongBanToiDa.ToString() == SoLuongBanToiDa
                    && SelectedItem.GhiChu == GhiChu
                    && SelectedItem.MaLoaiSanh == (SelectedHallType?.MaLoaiSanh ?? 0))
                {
                    EditMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                // Kiểm tra tính hợp lệ của dữ liệu
                if (string.IsNullOrWhiteSpace(TenSanh))
                {
                    EditMessage = "Tên sảnh không được để trống";
                    return false;
                }
                if (!int.TryParse(SoLuongBanToiDa, out int soLuong) || soLuong <= 0)
                {
                    EditMessage = "Số lượng bàn tối đa phải là số nguyên dương";
                    return false;
                }
                // Kiểm tra trùng tên sảnh trong cùng loại sảnh (trừ chính nó)
                var exists = DataProvider.Ins.DB.SANHs.Any(x =>
                    x.TenSanh == TenSanh &&
                    x.MaLoaiSanh == SelectedHallType.MaLoaiSanh &&
                    x.MaSanh != SelectedItem.MaSanh);
                if (exists)
                {
                    EditMessage = "Tên sảnh đã tồn tại trong loại sảnh này";
                    return false;
                }
                EditMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var hall = DataProvider.Ins.DB.SANHs.SingleOrDefault(x => x.MaSanh == SelectedItem.MaSanh);
                    if (hall != null)
                    {
                        hall.TenSanh = TenSanh;
                        hall.SoLuongBanToiDa = int.TryParse(SoLuongBanToiDa, out int soLuong) ? soLuong : (int?)null;
                        hall.GhiChu = GhiChu;
                        hall.MaLoaiSanh = SelectedHallType?.MaLoaiSanh;
                        DataProvider.Ins.DB.SaveChanges();

                        var index = List.IndexOf(SelectedItem);
                        List[index] = null;
                        List[index] = hall;

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
        private void Reset()
        {
            SelectedItem = null;
            TenSanh = string.Empty;
            SoLuongBanToiDa = null;
            GhiChu = string.Empty;
            SelectedHallType = null;
            SearchText = string.Empty;
        }
    }
}
