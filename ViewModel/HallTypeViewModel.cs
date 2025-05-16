using QuanLyTiecCuoi.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    public class HallTypeViewModel : BaseViewModel
    {
        private ObservableCollection<LOAISANH> _List;
        public ObservableCollection<LOAISANH> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<LOAISANH> _OriginalList;
        public ObservableCollection<LOAISANH> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private LOAISANH _SelectedItem;
        public LOAISANH SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    TenLoaiSanh = SelectedItem.TenLoaiSanh;
                    DonGiaBanToiThieu = SelectedItem.DonGiaBanToiThieu;
                }
            }
        }

        private string _TenLoaiSanh;
        public string TenLoaiSanh { get => _TenLoaiSanh; set { _TenLoaiSanh = value; OnPropertyChanged(); } }

        private decimal? _DonGiaBanToiThieu;
        public decimal? DonGiaBanToiThieu { get => _DonGiaBanToiThieu; set { _DonGiaBanToiThieu = value; OnPropertyChanged(); } }
        private string _Message;
        public string Message { get => _Message; set { _Message = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

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
                TenLoaiSanh = string.Empty;
                DonGiaBanToiThieu = null;
                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    List = OriginalList;
                    return;
                }

                switch (SelectedSearchProperty)
                {
                    case "Tên loại sảnh":
                        List = new ObservableCollection<LOAISANH>(OriginalList.Where(x => x.TenLoaiSanh != null && x.TenLoaiSanh.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Đơn giá bàn tối thiểu":
                        if (decimal.TryParse(SearchText, out var price))
                        {
                            List = new ObservableCollection<LOAISANH>(OriginalList.Where(x => x.DonGiaBanToiThieu == price));
                        }
                        else
                        {
                            List = new ObservableCollection<LOAISANH>();
                        }
                        break;
                    default:
                        List = new ObservableCollection<LOAISANH>(OriginalList);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public HallTypeViewModel()
        {
            List = new ObservableCollection<LOAISANH>(DataProvider.Ins.DB.LOAISANHs);
            OriginalList = new ObservableCollection<LOAISANH>(List);

            SearchProperties = new ObservableCollection<string> { "Tên loại sảnh", "Đơn giá bàn tối thiểu" };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>((p) =>
            {
                return CanAddOrEdit();
            }, (p) =>
            {
                try
                {
                    var newHallType = new LOAISANH()
                    {
                        TenLoaiSanh = TenLoaiSanh,
                        DonGiaBanToiThieu = DonGiaBanToiThieu
                    };

                    DataProvider.Ins.DB.LOAISANHs.Add(newHallType);
                    DataProvider.Ins.DB.SaveChanges();

                    List.Add(newHallType);

                    reset();
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                //if (SelectedItem == null || string.IsNullOrEmpty(TenLoaiSanh))
                //    return false;
                //var displayList = DataProvider.Ins.DB.LOAISANHs.Where(x => (x.TenLoaiSanh == TenLoaiSanh));
                //if (displayList == null || displayList.Count() != 0)
                //    return false;
                //return true;
                return CanAddOrEdit();
            }, (p) =>
            {
                try
                {
                    var hallType = DataProvider.Ins.DB.LOAISANHs.Where(x => x.MaLoaiSanh == SelectedItem.MaLoaiSanh).SingleOrDefault();
                    if (hallType != null)
                    {
                        hallType.TenLoaiSanh = TenLoaiSanh;
                        hallType.DonGiaBanToiThieu = DonGiaBanToiThieu;
                        DataProvider.Ins.DB.SaveChanges();

                        SelectedItem.TenLoaiSanh = TenLoaiSanh;
                        SelectedItem.DonGiaBanToiThieu = DonGiaBanToiThieu;

                        var index = List.IndexOf(SelectedItem);
                        List[index] = null;
                        List[index] = hallType;

                        reset();
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
                if (SelectedItem == null)
                    return false;
                var hasReferences = DataProvider.Ins.DB.SANHs.Any(s => s.MaLoaiSanh == SelectedItem.MaLoaiSanh);
                return !hasReferences;
            }, (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa loại sảnh này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        DataProvider.Ins.DB.LOAISANHs.Remove(SelectedItem);
                        DataProvider.Ins.DB.SaveChanges();

                        List.Remove(SelectedItem);

                        reset();

                        MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Hủy xóa", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
        private bool CanAddOrEdit()
        {
            // Kiểm tra tên loại sảnh không được để trống
            if (string.IsNullOrWhiteSpace(TenLoaiSanh))
            {
                Message = "Tên loại sảnh không được để trống.";
                return false;
            }

            // Kiểm tra đơn giá phải có và lớn hơn 0
            if (!DonGiaBanToiThieu.HasValue || DonGiaBanToiThieu <= 0)
            {
                Message = "Đơn giá bàn tối thiểu phải lớn hơn 0.";
                return false;
            } 

            // Chuẩn hóa tên loại sảnh để kiểm tra trùng (loại bỏ khoảng trắng, phân biệt hoa thường)
            string normalizedName = TenLoaiSanh.Trim();

            // Không cho phép trùng tên loại sảnh (kể cả khi sửa)
            var query = DataProvider.Ins.DB.LOAISANHs
                .Where(x => x.TenLoaiSanh.Trim() == normalizedName);

            // Nếu đang sửa, không cho phép trùng tên với bất kỳ bản ghi nào (kể cả chính nó)
            if (query.Any())
            {
                
                return false;
            }
            Message = string.Empty; // Xóa thông báo lỗi nếu không có lỗi nào
            return true;
        }
        private void reset()
        {
            SelectedItem = null;
            TenLoaiSanh = string.Empty;
            DonGiaBanToiThieu = null;
            SearchText = string.Empty;
            List = OriginalList;
        }
    }
}
