using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    public class ServiceViewModel : BaseViewModel
    {
        private readonly IDichVuService _dichVuService;

        private ObservableCollection<DICHVUDTO> _List;
        public ObservableCollection<DICHVUDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<DICHVUDTO> _OriginalList;
        public ObservableCollection<DICHVUDTO> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private DICHVUDTO _SelectedItem;
        public DICHVUDTO SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    TenDichVu = SelectedItem.TenDichVu;
                    DonGia = SelectedItem.DonGia?.ToString("G29") ?? string.Empty;
                    GhiChu = SelectedItem.GhiChu;
                }
                else
                {
                    AddMessage = string.Empty;
                    EditMessage = string.Empty;
                    DeleteMessage = string.Empty;
                }
            }
        }

        private string _TenDichVu;
        public string TenDichVu { get => _TenDichVu; set { _TenDichVu = value; OnPropertyChanged(); } }

        private string _DonGia;
        public string DonGia { get => _DonGia; set { _DonGia = value; OnPropertyChanged(); } }

        private string _GhiChu;
        public string GhiChu { get => _GhiChu; set { _GhiChu = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        private string _AddMessage;
        public string AddMessage { get => _AddMessage; set { _AddMessage = value; OnPropertyChanged(); } }

        public ICommand EditCommand { get; set; }
        private string _EditMessage;
        public string EditMessage { get => _EditMessage; set { _EditMessage = value; OnPropertyChanged(); } }

        public ICommand DeleteCommand { get; set; }
        private string _DeleteMessage;

        public ICommand ResetCommand { get; set; }



        public string DeleteMessage { get => _DeleteMessage; set { _DeleteMessage = value; OnPropertyChanged(); } }

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
                TenDichVu = string.Empty;
                DonGia = null;
                GhiChu = string.Empty;

                if (string.IsNullOrWhiteSpace(SearchText) || string.IsNullOrWhiteSpace(SelectedSearchProperty))
                {
                    List = OriginalList;
                    return;
                }

                string search = SearchText.Trim();

                switch (SelectedSearchProperty)
                {
                    case "Tên dịch vụ":
                        List = new ObservableCollection<DICHVUDTO>(
                            OriginalList.Where(x => !string.IsNullOrWhiteSpace(x.TenDichVu) &&
                                                    x.TenDichVu.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;

                    case "Đơn giá":
                        if (decimal.TryParse(search.Replace(",", "").Trim(), out decimal searchPrice))
                        {
                            List = new ObservableCollection<DICHVUDTO>(
                                OriginalList.Where(x => x.DonGia.HasValue && x.DonGia.Value == searchPrice));
                        }
                        else
                        {
                            List = new ObservableCollection<DICHVUDTO>();
                        }
                        break;

                    case "Ghi chú":
                        List = new ObservableCollection<DICHVUDTO>(
                            OriginalList.Where(x => !string.IsNullOrWhiteSpace(x.GhiChu) &&
                                                    x.GhiChu.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;

                    default:
                        List = OriginalList;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public ServiceViewModel()
        {
            //MessageBox.Show("Chào mừng bạn đến với quản lý dịch vụ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            _dichVuService = new DichVuService();
            //MessageBox.Show(_dichVuService.GetAll().First().GhiChu);
            List = new ObservableCollection<DICHVUDTO>(_dichVuService.GetAll().ToList());
            OriginalList = new ObservableCollection<DICHVUDTO>(List);

            SearchProperties = new ObservableCollection<string> { "Tên dịch vụ", "Đơn giá", "Ghi chú" };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>((p) =>
            {
                AddMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(TenDichVu))
                {
                    if (SelectedItem != null)
                    {
                        AddMessage = "Vui lòng nhập tên dịch vụ";
                    }
                    else
                    {
                        AddMessage = string.Empty;
                    }
                    return false;
                }

                if (string.IsNullOrWhiteSpace(DonGia))
                {
                    AddMessage = "Vui lòng nhập đơn giá";
                    return false;
                }
                if (!decimal.TryParse(DonGia, out var gia) || gia < 0)
                {
                    AddMessage = "Vui lòng nhập đơn giá hợp lệ";
                    return false;
                }
                var exists = OriginalList.Any(x => x.TenDichVu == TenDichVu);
                if (exists)
                {
                    AddMessage = "Tên dịch vụ đã tồn tại";
                    return false;
                }
                return true;
            }, (p) =>
            {
                try
                {
                    var newService = new DICHVUDTO()
                    {
                        TenDichVu = TenDichVu,
                        DonGia = decimal.Parse(DonGia),
                        GhiChu = GhiChu
                    };

                    _dichVuService.Create(newService);
                    List.Add(newService);
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    Reset();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            // EditCommand
            EditCommand = new RelayCommand<object>((p) =>
            {
                EditMessage = string.Empty;
                if (SelectedItem == null)
                {
                    // Không cho phép sửa nếu chưa chọn
                    return false;
                }
                if (string.IsNullOrWhiteSpace(TenDichVu))
                {
                    EditMessage = "Vui lòng nhập tên dịch vụ";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(DonGia))
                {
                    EditMessage = "Vui lòng nhập đơn giá";
                    return false;
                }
                if (!decimal.TryParse(DonGia, out var gia) || gia < 0)
                {
                    EditMessage = "Vui lòng nhập đơn giá hợp lệ";
                    return false;
                }
                var exists = OriginalList.Any(x => x.TenDichVu == TenDichVu && x.MaDichVu != SelectedItem.MaDichVu);
                if (exists)
                {
                    EditMessage = "Tên dịch vụ đã tồn tại";
                    return false;
                }
                if (SelectedItem.TenDichVu == TenDichVu &&
                    SelectedItem.DonGia?.ToString("G29") == DonGia &&
                    SelectedItem.GhiChu == GhiChu)
                {
                    EditMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                return true;
            }, (p) =>
            {
                try
                {
                    var updateDto = new DICHVUDTO()
                    {
                        MaDichVu = SelectedItem.MaDichVu,
                        TenDichVu = TenDichVu,
                        DonGia = decimal.Parse(DonGia),
                        GhiChu = GhiChu
                    };

                    _dichVuService.Update(updateDto);

                    var index = List.IndexOf(SelectedItem);
                    List[index] = null;
                    List[index] = updateDto;

                    EditMessage = "Cập nhật thành công";
                    Reset();
                }
                catch (Exception ex)
                {
                    EditMessage = $"Lỗi khi cập nhật: {ex.Message}";
                }
            });

            // DeleteCommand
            DeleteCommand = new RelayCommand<object>((p) =>
            {
                DeleteMessage = string.Empty;
                if (SelectedItem == null)
                {
                    // Không cho phép xóa nếu chưa chọn
                    return false;
                }
                return true;
            }, (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa dịch vụ này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _dichVuService.Delete(SelectedItem.MaDichVu);
                        List.Remove(SelectedItem);
                        DeleteMessage = "Xóa thành công";
                        Reset();
                    }
                }
                catch (Exception ex)
                {
                    DeleteMessage = $"Lỗi khi xóa: {ex.Message}";
                }
            });

            ResetCommand = new RelayCommand<object>((p) => true, (p) => Reset());
        }

        private void Reset()
        {
            SelectedItem = null;
            TenDichVu = string.Empty;
            DonGia = null;
            GhiChu = string.Empty;
            SearchText = string.Empty;
        }
    }
}