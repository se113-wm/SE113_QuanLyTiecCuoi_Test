using ClosedXML.Excel;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    public class HallTypeViewModel : BaseViewModel
    {
        private readonly ILoaiSanhService _loaiSanhService;
        private readonly ISanhService _sanhService;

        private ObservableCollection<LOAISANHDTO> _List;
        public ObservableCollection<LOAISANHDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<LOAISANHDTO> _OriginalList;
        public ObservableCollection<LOAISANHDTO> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private LOAISANHDTO _SelectedItem;
        public LOAISANHDTO SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    TenLoaiSanh = SelectedItem.TenLoaiSanh;
                    DonGiaBanToiThieu = SelectedItem.DonGiaBanToiThieu?.ToString("G29") ?? string.Empty;
                }
                else
                {
                    AddMessage = string.Empty;
                    EditMessage = string.Empty;
                    DeleteMessage = string.Empty;
                }
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }
        private bool _isAdding;
        public bool IsAdding
        {
            get => _isAdding;
            set { _isAdding = value; OnPropertyChanged(); }
        }
        private bool _isDeleting;
        public bool IsDeleting
        {
            get => _isDeleting;
            set { _isDeleting = value; OnPropertyChanged(); }
        }
        //Add ExportingExcel
        private bool _isExporting;
        public bool IsExporting
        {
            get => _isExporting;
            set { _isExporting = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> ActionList { get; } = new ObservableCollection<string> { "Thêm", "Sửa", "Xóa", "Xuất Excel", "Chọn thao tác" };
        private string _selectedAction;
        public string SelectedAction
        {
            get => _selectedAction;
            set
            {
                _selectedAction = value;
                OnPropertyChanged();
                switch (value)
                {
                    case "Thêm":
                        IsAdding = true;
                        IsEditing = false;
                        IsDeleting = false;
                        IsExporting = false;
                        Reset(); // reset các trường nhập liệu
                        break;
                    case "Sửa":
                        IsAdding = false;
                        IsEditing = true;
                        IsDeleting = false;
                        IsExporting = false;
                        Reset();
                        //if (SelectedItem == null)
                        //{
                        //    MessageBox.Show("Vui lòng chọn một sảnh để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    return;
                        //}
                        break;
                    case "Xóa":
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = true;
                        IsExporting = false;
                        Reset();
                        //if (SelectedItem == null)
                        //{
                        //    MessageBox.Show("Vui lòng chọn một sảnh để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    return;
                        //}
                        break;
                    case "Xuất Excel":
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = false;
                        IsExporting = true;
                        Reset();
                        break;
                    default:
                        _selectedAction = null;
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = false;
                        IsExporting = false;
                        Reset(); // reset các trường nhập liệu
                        break;
                }
            }
        }
        private string _TenLoaiSanh;
        public string TenLoaiSanh { get => _TenLoaiSanh; set { _TenLoaiSanh = value; OnPropertyChanged(); } }

        private string _DonGiaBanToiThieu;
        public string DonGiaBanToiThieu { get => _DonGiaBanToiThieu; set { _DonGiaBanToiThieu = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        private string _AddMessage;
        public string AddMessage { get => _AddMessage; set { _AddMessage = value; OnPropertyChanged(); } }

        public ICommand EditCommand { get; set; }
        private string _EditMessage;
        public string EditMessage { get => _EditMessage; set { _EditMessage = value; OnPropertyChanged(); } }
        public ICommand DeleteCommand { get; set; }
        private string _DeleteMessage;
        public string DeleteMessage { get => _DeleteMessage; set { _DeleteMessage = value; OnPropertyChanged(); } }
        public ICommand ExportToExcelCommand { get; set; }

        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => {
            Reset();
        });

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
                        List = new ObservableCollection<LOAISANHDTO>(OriginalList.Where(x => x.TenLoaiSanh != null && x.TenLoaiSanh.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Đơn giá bàn tối thiểu":
                        List = new ObservableCollection<LOAISANHDTO>(
                            OriginalList.Where(x =>
                                x.DonGiaBanToiThieu != null &&
                                x.DonGiaBanToiThieu.Value.ToString("N0").Replace(",", "").Contains(SearchText.Replace(",", "").Trim())
                            )
                        );
                        break;
                    default:
                        List = new ObservableCollection<LOAISANHDTO>(OriginalList);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public HallTypeViewModel(ILoaiSanhService loaiSanhService, ISanhService sanhService )
        {
            _loaiSanhService = loaiSanhService;
            _sanhService = sanhService;

            List = new ObservableCollection<LOAISANHDTO>(_loaiSanhService.GetAll().ToList());
            OriginalList = new ObservableCollection<LOAISANHDTO>(List);

            SearchProperties = new ObservableCollection<string> { "Tên loại sảnh", "Đơn giá bàn tối thiểu" };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrWhiteSpace(TenLoaiSanh))
                {
                    if (SelectedItem != null)
                    {
                        AddMessage = "Vui lòng nhập tên loại sảnh";
                    }
                    else
                    {
                        AddMessage = string.Empty;
                    }
                    return false;
                }
                var exists = OriginalList.Any(x => x.TenLoaiSanh == TenLoaiSanh);
                if (exists)
                {
                    AddMessage = "Tên loại sảnh đã tồn tại";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(DonGiaBanToiThieu) || !decimal.TryParse(DonGiaBanToiThieu, out var gia) || gia % 1 != 0)
                {
                    AddMessage = "Vui lòng nhập đơn giá hợp lệ";
                    return false;
                }
                if (gia < 10000)
                {
                    AddMessage = "Đơn giá bàn tối thiểu >= 10.000";
                    return false;
                }
                AddMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var newHallType = new LOAISANHDTO()
                    {
                        TenLoaiSanh = TenLoaiSanh,
                        DonGiaBanToiThieu = string.IsNullOrWhiteSpace(DonGiaBanToiThieu)
                                                    ? (decimal?)null
                                                    : decimal.Parse(DonGiaBanToiThieu)
                    };

                    _loaiSanhService.Create(newHallType);

                    List.Add(newHallType);

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
                    return false;
                }
                if (SelectedItem.TenLoaiSanh == TenLoaiSanh && SelectedItem.DonGiaBanToiThieu?.ToString("G29") == DonGiaBanToiThieu)
                {
                    EditMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(TenLoaiSanh))
                {
                    EditMessage = "Tên loại sảnh không được để trống";
                    return false;
                }
                var exists = OriginalList.Any(x => x.TenLoaiSanh == TenLoaiSanh && x.MaLoaiSanh != SelectedItem.MaLoaiSanh);
                if (exists)
                {
                    EditMessage = "Tên loại sảnh đã tồn tại";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(DonGiaBanToiThieu) || !decimal.TryParse(DonGiaBanToiThieu, out var gia) || gia % 1 != 0)
                {
                    EditMessage = "Vui lòng nhập đơn giá hợp lệ";
                    return false;
                }
                if (gia < 10000)
                {
                    EditMessage = "Đơn giá bàn tối thiểu >= 10.000";
                    return false;
                }
                EditMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var updateDto = new LOAISANHDTO()
                    {
                        MaLoaiSanh = SelectedItem.MaLoaiSanh,
                        TenLoaiSanh = TenLoaiSanh,
                        DonGiaBanToiThieu = string.IsNullOrWhiteSpace(DonGiaBanToiThieu)
                                                ? (decimal?)null
                                                : decimal.Parse(DonGiaBanToiThieu)
                    };

                    _loaiSanhService.Update(updateDto);

                    var index = List.IndexOf(SelectedItem);
                    List[index] = null;
                    List[index] = updateDto;

                    Reset();
                    MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
                // Tìm xem có sảnh nào có mã loại sảnh này không, bằng cách gọi sảnh service để lấy danh sách sảnh
                var hasSanh = _sanhService.GetAll().Any(s => s.MaLoaiSanh == SelectedItem.MaLoaiSanh);
                if (hasSanh)
                {
                    DeleteMessage = "Đang có sảnh thuộc loại sảnh này";
                    return false;
                }
                DeleteMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa loại sảnh này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _loaiSanhService.Delete(SelectedItem.MaLoaiSanh);

                        List.Remove(SelectedItem);

                        Reset();

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
            ExportToExcelCommand = new RelayCommand<object>((p) => true, (p) => ExportToExcel());
            _sanhService = sanhService;
        }
        //Exporting excel function
        private void ExportToExcel()
        {
            if (List == null || List.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách loại sảnh");

            // Tiêu đề cột
            worksheet.Cell(1, 1).Value = "Tên loại sảnh";
            worksheet.Cell(1, 2).Value = "Đơn giá bàn tối thiểu";

            // Format tiêu đề
            var headerRange = worksheet.Range("A1:B1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Ghi dữ liệu
            int row = 2;
            foreach (var item in List)
            {
                worksheet.Cell(row, 1).Value = item.TenLoaiSanh;
                worksheet.Cell(row, 2).Value = item.DonGiaBanToiThieu;

                // Format dòng dữ liệu
                for (int col = 1; col <= 2; col++)
                {
                    worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0"; // Format tiền
                row++;
            }

            // Tự động điều chỉnh độ rộng
            worksheet.Columns().AdjustToContents();

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"DanhSachLoaiSanh_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                workbook.SaveAs(dialog.FileName);
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở file: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Reset()
        {
            SelectedItem = null;
            TenLoaiSanh = string.Empty;
            DonGiaBanToiThieu = null;
            SearchText = string.Empty;
        }
    }
}