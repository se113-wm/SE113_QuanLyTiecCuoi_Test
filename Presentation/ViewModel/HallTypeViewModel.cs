using ClosedXML.Excel;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    public class HallTypeViewModel : BaseViewModel
    {
        #region Service & Collections
        private readonly IHallTypeService _hallTypeService;
        private readonly IHallService _hallService;

        private ObservableCollection<HallTypeDTO> _hallTypeList;
        public ObservableCollection<HallTypeDTO> HallTypeList
        {
            get => _hallTypeList;
            set { _hallTypeList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<HallTypeDTO> _originalList;
        public ObservableCollection<HallTypeDTO> OriginalList
        {
            get => _originalList;
            set { _originalList = value; OnPropertyChanged(); }
        }
        #endregion

        #region Selected Item
        private HallTypeDTO _selectedItem;
        public HallTypeDTO SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();

                if (SelectedItem != null)
                {
                    HallTypeName = SelectedItem.HallTypeName;
                    MinTablePrice = SelectedItem.MinTablePrice?.ToString("G29") ?? string.Empty;
                }
                else
                {
                    AddMessage = string.Empty;
                    EditMessage = string.Empty;
                    DeleteMessage = string.Empty;
                }
            }
        }
        #endregion

        #region Bindable Fields
        private string _hallTypeName;
        public string HallTypeName
        {
            get => _hallTypeName;
            set { _hallTypeName = value; OnPropertyChanged(); }
        }

        private string _minTablePrice;
        public string MinTablePrice
        {
            get => _minTablePrice;
            set { _minTablePrice = value; OnPropertyChanged(); }
        }
        #endregion

        #region Search
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

        private ObservableCollection<string> _searchProperties;
        public ObservableCollection<string> SearchProperties
        {
            get => _searchProperties;
            set { _searchProperties = value; OnPropertyChanged(); }
        }

        private string _selectedSearchProperty;
        public string SelectedSearchProperty
        {
            get => _selectedSearchProperty;
            set
            {
                _selectedSearchProperty = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }
        #endregion

        #region Action Selection
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
                        Reset();
                        break;
                    case "Sửa":
                        IsAdding = false;
                        IsEditing = true;
                        IsDeleting = false;
                        IsExporting = false;
                        Reset();
                        break;
                    case "Xóa":
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = true;
                        IsExporting = false;
                        Reset();
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
                        Reset();
                        break;
                }
            }
        }
        #endregion

        #region Messages & Commands
        private string _addMessage;
        public string AddMessage
        {
            get => _addMessage;
            set { _addMessage = value; OnPropertyChanged(); }
        }

        private string _editMessage;
        public string EditMessage
        {
            get => _editMessage;
            set { _editMessage = value; OnPropertyChanged(); }
        }

        private string _deleteMessage;
        public string DeleteMessage
        {
            get => _deleteMessage;
            set { _deleteMessage = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ExportToExcelCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        #endregion

        #region Constructor
        public HallTypeViewModel(IHallTypeService hallTypeService, IHallService hallService)
        {
            _hallTypeService = hallTypeService;
            _hallService = hallService;

            HallTypeList = new ObservableCollection<HallTypeDTO>(_hallTypeService.GetAll().ToList());
            OriginalList = new ObservableCollection<HallTypeDTO>(HallTypeList);

            SearchProperties = new ObservableCollection<string> { "Tên loại sảnh", "Đơn giá bàn tối thiểu" };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>(
                (p) => CanAdd(),
                (p) => AddHallType()
            );

            EditCommand = new RelayCommand<object>(
                (p) => CanEdit(),
                (p) => EditHallType()
            );

            DeleteCommand = new RelayCommand<object>(
                (p) => CanDelete(),
                (p) => DeleteHallType()
            );

            ResetCommand = new RelayCommand<object>(
                (p) => true,
                (p) => Reset()
            );

            ExportToExcelCommand = new RelayCommand<object>((p) => true, (p) => ExportToExcel());
        }
        #endregion

        #region Validation Helpers
        private bool TryParsePrice(string input, out decimal val)
            => decimal.TryParse(input, out val);
        #endregion

        #region Add
        private bool CanAdd()
        {
            if (string.IsNullOrWhiteSpace(HallTypeName))
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

            var exists = OriginalList.Any(x => x.HallTypeName == HallTypeName);
            if (exists)
            {
                AddMessage = "Tên loại sảnh đã tồn tại";
                return false;
            }

            if (string.IsNullOrWhiteSpace(MinTablePrice) || !TryParsePrice(MinTablePrice, out var price) || price % 1 != 0)
            {
                AddMessage = "Vui lòng nhập đơn giá hợp lệ";
                return false;
            }

            if (price < 10000)
            {
                AddMessage = "Đơn giá bàn tối thiểu >= 10.000";
                return false;
            }

            AddMessage = string.Empty;
            return true;
        }

        private void AddHallType()
        {
            try
            {
                var newHallType = new HallTypeDTO()
                {
                    HallTypeName = HallTypeName.Trim(),
                    MinTablePrice = string.IsNullOrWhiteSpace(MinTablePrice)
                                                ? (decimal?)null
                                                : decimal.Parse(MinTablePrice)
                };

                _hallTypeService.Create(newHallType);
                OriginalList.Add(newHallType);
                HallTypeList = new ObservableCollection<HallTypeDTO>(OriginalList);

                Reset();
                MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Edit
        private bool CanEdit()
        {
            if (SelectedItem == null)
            {
                return false;
            }

            if (SelectedItem.HallTypeName == HallTypeName && SelectedItem.MinTablePrice?.ToString("G29") == MinTablePrice)
            {
                EditMessage = "Không có thay đổi nào để cập nhật";
                return false;
            }

            if (string.IsNullOrWhiteSpace(HallTypeName))
            {
                EditMessage = "Tên loại sảnh không được để trống";
                return false;
            }

            var exists = OriginalList.Any(x => x.HallTypeName == HallTypeName && x.HallTypeId != SelectedItem.HallTypeId);
            if (exists)
            {
                EditMessage = "Tên loại sảnh đã tồn tại";
                return false;
            }

            if (string.IsNullOrWhiteSpace(MinTablePrice) || !TryParsePrice(MinTablePrice, out var price) || price % 1 != 0)
            {
                EditMessage = "Vui lòng nhập đơn giá hợp lệ";
                return false;
            }

            if (price < 10000)
            {
                EditMessage = "Đơn giá bàn tối thiểu >= 10.000";
                return false;
            }

            EditMessage = string.Empty;
            return true;
        }

        private void EditHallType()
        {
            try
            {
                var updateDto = new HallTypeDTO()
                {
                    HallTypeId = SelectedItem.HallTypeId,
                    HallTypeName = HallTypeName.Trim(),
                    MinTablePrice = string.IsNullOrWhiteSpace(MinTablePrice)
                                            ? (decimal?)null
                                            : decimal.Parse(MinTablePrice)
                };

                _hallTypeService.Update(updateDto);

                var index = HallTypeList.IndexOf(SelectedItem);
                HallTypeList[index] = null;
                HallTypeList[index] = updateDto;
                OriginalList[index] = updateDto;

                Reset();
                MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Delete
        private bool CanDelete()
        {
            if (SelectedItem == null)
                return false;

            var hasHalls = _hallService.GetAll().Any(s => s.HallTypeId == SelectedItem.HallTypeId);
            if (hasHalls)
            {
                DeleteMessage = "Đang có sảnh thuộc loại sảnh này";
                return false;
            }

            DeleteMessage = string.Empty;
            return true;
        }

        private void DeleteHallType()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa loại sảnh này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                _hallTypeService.Delete(SelectedItem.HallTypeId);
                HallTypeList.Remove(SelectedItem);
                OriginalList.Remove(SelectedItem);

                Reset();
                MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region ExportToExcel
        private void ExportToExcel()
        {
            if (HallTypeList == null || HallTypeList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách loại sảnh");

            worksheet.Cell(1, 1).Value = "Tên loại sảnh";
            worksheet.Cell(1, 2).Value = "Đơn giá bàn tối thiểu";

            var headerRange = worksheet.Range("A1:B1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            int row = 2;
            foreach (var item in HallTypeList)
            {
                worksheet.Cell(row, 1).Value = item.HallTypeName;
                worksheet.Cell(row, 2).Value = item.MinTablePrice;

                for (int col = 1; col <= 2; col++)
                {
                    worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0";
                row++;
            }

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
        #endregion

        #region Helpers
        private void PerformSearch()
        {
            try
            {
                SelectedItem = null;
                HallTypeName = string.Empty;
                MinTablePrice = null;

                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    HallTypeList = OriginalList;
                    return;
                }

                switch (SelectedSearchProperty)
                {
                    case "Tên loại sảnh":
                        HallTypeList = new ObservableCollection<HallTypeDTO>(OriginalList.Where(x => x.HallTypeName != null && x.HallTypeName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Đơn giá bàn tối thiểu":
                        HallTypeList = new ObservableCollection<HallTypeDTO>(
                            OriginalList.Where(x =>
                                x.MinTablePrice != null &&
                                x.MinTablePrice.Value.ToString("N0").Replace(",", "").Contains(SearchText.Replace(",", "").Trim())
                            )
                        );
                        break;
                    default:
                        HallTypeList = new ObservableCollection<HallTypeDTO>(OriginalList);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Reset()
        {
            //SelectedItem = null;
            //HallTypeName = string.Empty;
            //MinTablePrice = null;
            //SearchText = string.Empty;
        }
        #endregion
    }
}