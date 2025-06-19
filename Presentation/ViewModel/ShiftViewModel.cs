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
    public class ShiftViewModel : BaseViewModel
    {
        private readonly ICaService _caService;

        private ObservableCollection<CADTO> _List;
        public ObservableCollection<CADTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<CADTO> _OriginalList;
        public ObservableCollection<CADTO> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private CADTO _SelectedItem;
        public CADTO SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    TenCa = SelectedItem.TenCa;
                    ThoiGianBatDauCa = SelectedItem.ThoiGianBatDauCa;
                    ThoiGianKetThucCa = SelectedItem.ThoiGianKetThucCa;
                }
                else
                {
                    AddMessage = string.Empty;
                    EditMessage = string.Empty;
                    DeleteMessage = string.Empty;
                }
            }
        }

        private string _TenCa;
        public string TenCa { get => _TenCa; set { _TenCa = value; OnPropertyChanged(); } }

        private TimeSpan? _thoiGianBatDauCa;
        public TimeSpan? ThoiGianBatDauCa
        {
            get => _thoiGianBatDauCa;
            set { _thoiGianBatDauCa = value; OnPropertyChanged(); OnPropertyChanged(nameof(ThoiGianBatDauCaDateTime)); }
        }

        public DateTime? ThoiGianBatDauCaDateTime
        {
            get => _thoiGianBatDauCa.HasValue ? DateTime.Today.Add(_thoiGianBatDauCa.Value) : (DateTime?)null;
            set
            {
                if (value.HasValue)
                    ThoiGianBatDauCa = value.Value.TimeOfDay;
                else
                    ThoiGianBatDauCa = null;
                OnPropertyChanged();
            }
        }

        private TimeSpan? _thoiGianKetThucCa;
        public TimeSpan? ThoiGianKetThucCa
        {
            get => _thoiGianKetThucCa;
            set { _thoiGianKetThucCa = value; OnPropertyChanged(); OnPropertyChanged(nameof(ThoiGianKetThucCaDateTime)); }
        }

        public DateTime? ThoiGianKetThucCaDateTime
        {
            get => _thoiGianKetThucCa.HasValue ? DateTime.Today.Add(_thoiGianKetThucCa.Value) : (DateTime?)null;
            set
            {
                if (value.HasValue)
                    ThoiGianKetThucCa = value.Value.TimeOfDay;
                else
                    ThoiGianKetThucCa = null;
                OnPropertyChanged();
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

        public ICommand ResetCommand { get; set; }

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
                TenCa = string.Empty;
                ThoiGianBatDauCa = null;
                ThoiGianKetThucCa = null;

                if (string.IsNullOrWhiteSpace(SearchText) || string.IsNullOrWhiteSpace(SelectedSearchProperty))
                {
                    List = OriginalList;
                    return;
                }

                string search = SearchText.Trim();

                switch (SelectedSearchProperty)
                {
                    case "Tên ca":
                        List = new ObservableCollection<CADTO>(
                            OriginalList.Where(x => !string.IsNullOrWhiteSpace(x.TenCa) &&
                                                    x.TenCa.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Thời gian bắt đầu":
                        if (TimeSpan.TryParse(search, out TimeSpan startTime))
                        {
                            List = new ObservableCollection<CADTO>(
                                OriginalList.Where(x => x.ThoiGianBatDauCa.HasValue && x.ThoiGianBatDauCa.Value == startTime));
                        }
                        else
                        {
                            List = new ObservableCollection<CADTO>();
                        }
                        break;
                    case "Thời gian kết thúc":
                        if (TimeSpan.TryParse(search, out TimeSpan endTime))
                        {
                            List = new ObservableCollection<CADTO>(
                                OriginalList.Where(x => x.ThoiGianKetThucCa.HasValue && x.ThoiGianKetThucCa.Value == endTime));
                        }
                        else
                        {
                            List = new ObservableCollection<CADTO>();
                        }
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

        public ShiftViewModel()
        {
            _caService = new CaService();
            List = new ObservableCollection<CADTO>(_caService.GetAll().ToList());
            OriginalList = new ObservableCollection<CADTO>(List);

            SearchProperties = new ObservableCollection<string> { "Tên ca", "Thời gian bắt đầu", "Thời gian kết thúc" };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>((p) =>
            {
                AddMessage = string.Empty;
                if (string.IsNullOrWhiteSpace(TenCa))
                {
                    AddMessage = "Vui lòng nhập tên ca";
                    return false;
                }
                if (!ThoiGianBatDauCa.HasValue)
                {
                    AddMessage = "Vui lòng nhập thời gian bắt đầu ca";
                    return false;
                }
                if (!ThoiGianKetThucCa.HasValue)
                {
                    AddMessage = "Vui lòng nhập thời gian kết thúc ca";
                    return false;
                }
                if (ThoiGianBatDauCa.Value < new TimeSpan(7, 30, 0) || ThoiGianBatDauCa.Value >= new TimeSpan(24, 0, 0))
                {
                    AddMessage = "Thời gian bắt đầu ca phải sau 7h30 và trước 12h đêm.";
                    return false;
                }
                var exists = OriginalList.Any(x => x.TenCa == TenCa);
                if (exists)
                {
                    AddMessage = "Tên ca đã tồn tại";
                    return false;
                }
                return true;
            }, (p) =>
            {
                try
                {
                    var newShift = new CADTO()
                    {
                        TenCa = TenCa,
                        ThoiGianBatDauCa = ThoiGianBatDauCa,
                        ThoiGianKetThucCa = ThoiGianKetThucCa
                    };

                    _caService.Create(newShift);
                    List.Add(newShift);
                    MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    Reset();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                EditMessage = string.Empty;
                if (SelectedItem == null)
                {
                    return false;
                }
                if (string.IsNullOrWhiteSpace(TenCa))
                {
                    EditMessage = "Vui lòng nhập tên ca";
                    return false;
                }
                if (!ThoiGianBatDauCa.HasValue)
                {
                    EditMessage = "Vui lòng nhập thời gian bắt đầu ca";
                    return false;
                }
                if (!ThoiGianKetThucCa.HasValue)
                {
                    EditMessage = "Vui lòng nhập thời gian kết thúc ca";
                    return false;
                }
                if (ThoiGianBatDauCa.Value < new TimeSpan(7, 30, 0) || ThoiGianBatDauCa.Value >= new TimeSpan(24, 0, 0))
                {
                    EditMessage = "Thời gian bắt đầu ca phải sau 7h30 và trước 12h đêm.";
                    return false;
                }
                var exists = OriginalList.Any(x => x.TenCa == TenCa && x.MaCa != SelectedItem.MaCa);
                if (exists)
                {
                    EditMessage = "Tên ca đã tồn tại";
                    return false;
                }
                if (SelectedItem.TenCa == TenCa &&
                    SelectedItem.ThoiGianBatDauCa == ThoiGianBatDauCa &&
                    SelectedItem.ThoiGianKetThucCa == ThoiGianKetThucCa)
                {
                    EditMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                return true;
            }, (p) =>
            {
                try
                {
                    var updateDto = new CADTO()
                    {
                        MaCa = SelectedItem.MaCa,
                        TenCa = TenCa,
                        ThoiGianBatDauCa = ThoiGianBatDauCa,
                        ThoiGianKetThucCa = ThoiGianKetThucCa
                    };

                    _caService.Update(updateDto);

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

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                DeleteMessage = string.Empty;
                if (SelectedItem == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa ca này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _caService.Delete(SelectedItem.MaCa);
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
            TenCa = string.Empty;
            ThoiGianBatDauCa = null;
            ThoiGianKetThucCa = null;
            SearchText = string.Empty;
        }
    }
}