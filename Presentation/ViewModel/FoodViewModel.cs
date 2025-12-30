using ClosedXML.Excel;
using Microsoft.Win32;
using QuanLyTiecCuoi.BusinessLogicLayer.Helpers;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuanLyTiecCuoi.ViewModel
{
    public class FoodViewModel : BaseViewModel
    {
        #region Service & Collections
        private readonly IDishService _dishService;
        private readonly IMenuService _menuService;

        private ObservableCollection<DishDTO> _dishList;
        public ObservableCollection<DishDTO> DishList
        {
            get => _dishList;
            set { _dishList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<DishDTO> _originalList;
        public ObservableCollection<DishDTO> OriginalList
        {
            get => _originalList;
            set { _originalList = value; OnPropertyChanged(); }
        }
        #endregion

        #region Selected Item
        private DishDTO _selectedItem;
        public DishDTO SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();

                if (SelectedItem != null)
                {
                    DishName = SelectedItem.DishName;
                    UnitPrice = SelectedItem.UnitPrice?.ToString("N0");
                    Note = SelectedItem.Note;
                    if (!IsAdding)
                    {
                        RenderImageAsync(SelectedItem.DishId.ToString(), "Food");
                    }
                    if (IsEditing)
                    {
                        string folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                        string addCache = Path.Combine(folder, "Addcache.jpg");
                        string editCache = Path.Combine(folder, "Editcache.jpg");
                        if (File.Exists(addCache)) File.Delete(addCache);
                        if (File.Exists(editCache)) File.Delete(editCache);
                    }
                }
                else
                {
                    AddMessage = string.Empty;
                    EditMessage = string.Empty;
                    DeleteMessage = string.Empty;
                    Image = null;
                }
            }
        }
        #endregion

        #region Bindable Fields
        private string _dishName;
        public string DishName
        {
            get => _dishName;
            set { _dishName = value; OnPropertyChanged(); }
        }

        private string _unitPrice;
        public string UnitPrice
        {
            get => _unitPrice;
            set { _unitPrice = value; OnPropertyChanged(); }
        }

        private string _note;
        public string Note
        {
            get => _note;
            set { _note = value; OnPropertyChanged(); }
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
        private bool _hasNoImage;
        public bool HasNoImage
        {
            get => _hasNoImage;
            set { _hasNoImage = value; OnPropertyChanged(); }
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
                        Image = null;
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

        #region Image Handling
        private ImageSource _image;
        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
                HasNoImage = Image == null;
            }
        }

        private string _imageBtnToolTip;
        public string ImageBtnToolTip
        {
            get => _imageBtnToolTip;
            set { _imageBtnToolTip = value; OnPropertyChanged(); }
        }

        public ICommand SelectImageCommand => new RelayCommand<object>(
            (p) =>
            {
                if (IsAdding)
                {
                    return true;
                }
                if (IsEditing && SelectedItem != null)
                {
                    return true;
                }
                return false;
            },
            (p) =>
            {
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                if (IsEditing && SelectedItem != null)
                {
                    string currentImagePath = Path.Combine(folder, SelectedItem.DishId + ".jpg");
                    string cachePath = Path.Combine(folder, "Editcache.jpg");

                    if (File.Exists(currentImagePath) || File.Exists(cachePath))
                    {
                        var result = MessageBox.Show(
                            "Bạn đã chọn ảnh. Bạn muốn đổi ảnh (Yes) hay xóa ảnh (No)?",
                            "Ảnh món ăn",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);

                        if (result == MessageBoxResult.Yes)
                        {
                            var dlg = new OpenFileDialog
                            {
                                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                                Title = "Chọn ảnh món ăn"
                            };
                            if (dlg.ShowDialog() == true)
                            {
                                try
                                {
                                    File.Copy(dlg.FileName, cachePath, true);
                                    UpdateImageFromPath(cachePath);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Không thể lưu ảnh tạm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            try
                            {
                                if (File.Exists(cachePath)) File.Delete(cachePath);
                                Image = null;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Không thể xóa ảnh: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        var dlg = new OpenFileDialog
                        {
                            Filter = "Image Files|*.jpg;*.jpeg;*.png",
                            Title = "Chọn ảnh món ăn"
                        };
                        if (dlg.ShowDialog() == true)
                        {
                            try
                            {
                                File.Copy(dlg.FileName, cachePath, true);
                                UpdateImageFromPath(cachePath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Không thể lưu ảnh tạm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                else if (IsAdding)
                {
                    string cachePath = Path.Combine(folder, "Addcache.jpg");
                    if (File.Exists(cachePath))
                    {
                        var result = MessageBox.Show(
                            "Bạn đã chọn ảnh. Bạn muốn đổi ảnh (Yes) hay xóa ảnh (No)?",
                            "Ảnh món ăn",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);

                        if (result == MessageBoxResult.Yes)
                        {
                            var dlg = new OpenFileDialog
                            {
                                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                                Title = "Chọn ảnh món ăn"
                            };
                            if (dlg.ShowDialog() == true)
                            {
                                try
                                {
                                    File.Copy(dlg.FileName, cachePath, true);
                                    UpdateImageFromPath(cachePath);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Không thể lưu ảnh tạm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            try
                            {
                                File.Delete(cachePath);
                                Image = null;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Không thể xóa ảnh: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        var dlg = new OpenFileDialog
                        {
                            Filter = "Image Files|*.jpg;*.jpeg;*.png",
                            Title = "Chọn ảnh món ăn"
                        };
                        if (dlg.ShowDialog() == true)
                        {
                            try
                            {
                                File.Copy(dlg.FileName, cachePath, true);
                                UpdateImageFromPath(cachePath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Không thể lưu ảnh tạm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
        );

        private void UpdateImageFromPath(string path)
        {
            if (!File.Exists(path))
            {
                Image = null;
                return;
            }
            try
            {
                BitmapImage original = new BitmapImage();
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    original.BeginInit();
                    original.CacheOption = BitmapCacheOption.OnLoad;
                    original.StreamSource = stream;
                    original.EndInit();
                    original.Freeze();
                }

                int cropSize = Math.Min(original.PixelWidth, original.PixelHeight);
                int x = (original.PixelWidth - cropSize) / 2;
                int y = (original.PixelHeight - cropSize) / 2;

                var cropped = new CroppedBitmap(original, new Int32Rect(x, y, cropSize, cropSize));

                var targetSize = 500;
                var resized = new TransformedBitmap(cropped, new ScaleTransform(
                    (double)targetSize / cropSize,
                    (double)targetSize / cropSize
                ));

                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(resized));
                using (var outStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(outStream);
                }

                BitmapImage bitmap = new BitmapImage();
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
                Image = bitmap;
            }
            catch
            {
                Image = null;
            }
        }

        private void RenderImageAsync(string id, string type)
        {
            var path = ImageHelper.GetImagePath(type, id);
            if (!File.Exists(path))
            {
                Image = null;
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    BitmapImage bitmap = null;
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Image = bitmap;
                    });
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Image = null;
                    });
                }
            });
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
        public FoodViewModel(IDishService dishService, IMenuService menuService)
        {
            Image = null;
            _dishService = dishService;
            _menuService = menuService;

            DishList = new ObservableCollection<DishDTO>(_dishService.GetAll().ToList());
            OriginalList = new ObservableCollection<DishDTO>(DishList);

            SearchProperties = new ObservableCollection<string> { "Tên món ăn", "Đơn giá", "Ghi chú" };
            SelectedSearchProperty = SearchProperties.First();

            AddCommand = new RelayCommand<object>(
                (p) => CanAdd(),
                (p) => AddDish()
            );

            EditCommand = new RelayCommand<object>(
                (p) => CanEdit(),
                (p) => EditDish()
            );

            DeleteCommand = new RelayCommand<object>(
                (p) => CanDelete(),
                (p) => DeleteDish()
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
            => decimal.TryParse(input?.Replace(".", "").Replace(",", ""), out val);
        #endregion

        #region Add
        private bool CanAdd()
        {
            if (OriginalList.Count >= 100)
            {
                AddMessage = "Đã đạt giới hạn 100 món ăn";
                return false;
            }
            if (string.IsNullOrWhiteSpace(DishName))
            {
                AddMessage = "Tên món ăn không được để trống";
                return false;
            }
            if (!TryParsePrice(UnitPrice, out var price) || price <= 0)
            {
                AddMessage = "Đơn giá phải là số dương";
                return false;
            }
            if (!string.IsNullOrEmpty(Note) && Note.Length > 100)
            {
                AddMessage = "Ghi chú tối đa 100 ký tự";
                return false;
            }
            var exists = OriginalList.Any(x => x.DishName.Trim().Equals(DishName.Trim(), StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                AddMessage = "Tên món ăn đã tồn tại";
                return false;
            }
            AddMessage = string.Empty;
            return true;
        }

        private void AddDish()
        {
            try
            {
                var dto = new DishDTO
                {
                    DishName = DishName.Trim(),
                    UnitPrice = decimal.Parse(UnitPrice.Replace(".", "").Replace(",", "")),
                    Note = Note
                };
                _dishService.Create(dto);

                OriginalList.Add(dto);
                DishList = new ObservableCollection<DishDTO>(OriginalList);

                string folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                string cachePath = Path.Combine(folder, "Addcache.jpg");
                if (File.Exists(cachePath))
                {
                    string newImagePath = Path.Combine(folder, dto.DishId + ".jpg");
                    File.Copy(cachePath, newImagePath, true);
                    File.Delete(cachePath);
                }
                SelectedAction = null;

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

            var folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
            var imagePath = Path.Combine(folder, SelectedItem.DishId + ".jpg");
            var imageExists = File.Exists(imagePath);

            if (
                SelectedItem.DishName == DishName?.Trim() &&
                SelectedItem.Note == Note &&
                decimal.TryParse(UnitPrice?.Replace(".", "").Replace(",", ""), out var newPrice) &&
                SelectedItem.UnitPrice == newPrice &&
                !(imageExists && Image == null) &&
                ImageHelper.IsEditCacheImageSameAsCurrent(SelectedItem.DishId, "Food")
            )
            {
                EditMessage = "Chưa có thông tin nào thay đổi";
                return false;
            }

            if (string.IsNullOrWhiteSpace(DishName))
            {
                EditMessage = "Tên món ăn không được để trống";
                return false;
            }

            if (!TryParsePrice(UnitPrice, out var price) || price <= 0)
            {
                EditMessage = "Đơn giá phải là số dương";
                return false;
            }

            if (!string.IsNullOrEmpty(Note) && Note.Length > 100)
            {
                EditMessage = "Ghi chú tối đa 100 ký tự";
                return false;
            }

            var exists = OriginalList.Any(x => x.DishId != SelectedItem.DishId && x.DishName.Trim().Equals(DishName.Trim(), StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                EditMessage = "Tên món ăn đã tồn tại";
                return false;
            }

            EditMessage = string.Empty;
            return true;
        }

        private void EditDish()
        {
            try
            {
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                if (Image == null)
                {
                    string imagePath = Path.Combine(folder, SelectedItem.DishId + ".jpg");
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                var dto = new DishDTO
                {
                    DishId = SelectedItem.DishId,
                    DishName = DishName.Trim(),
                    UnitPrice = decimal.Parse(UnitPrice.Replace(".", "").Replace(",", "")),
                    Note = Note
                };
                _dishService.Update(dto);

                var idx = DishList.IndexOf(SelectedItem);
                DishList[idx] = dto;
                OriginalList[idx] = dto;

                string cachePath = Path.Combine(folder, "Editcache.jpg");
                if (File.Exists(cachePath))
                {
                    string newImagePath = Path.Combine(folder, dto.DishId + ".jpg");
                    File.Copy(cachePath, newImagePath, true);
                    File.Delete(cachePath);
                }
                SelectedAction = null;
                Reset();
                MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.Message.Contains("UNIQUE") == true)
                    MessageBox.Show("Tên món ăn đã tồn tại. Chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Delete
        private bool CanDelete()
        {
            if (SelectedItem == null)
            {
                return false;
            }

            var existsInMenu = _menuService.GetAll().Any(t => t.DishId == SelectedItem.DishId);
            if (existsInMenu)
            {
                DeleteMessage = "Món ăn này đang được sử dụng trong phiếu đặt tiệc";
                return false;
            }
            DeleteMessage = string.Empty;
            return true;
        }

        private void DeleteDish()
        {
            if (MessageBox.Show("Bạn có chắc muốn xoá món ăn này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                string imagePath = Path.Combine(folder, SelectedItem.DishId + ".jpg");
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                _dishService.Delete(SelectedItem.DishId);
                DishList.Remove(SelectedItem);
                OriginalList.Remove(SelectedItem);
                Reset();
                MessageBox.Show("Xoá thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể xoá do: {GetInnermost(ex).Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region ExportToExcel
        private void ExportToExcel()
        {
            if (DishList == null || DishList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách Món ăn");

            worksheet.Cell(1, 1).Value = "Tên món ăn";
            worksheet.Cell(1, 2).Value = "Đơn giá";
            worksheet.Cell(1, 3).Value = "Ghi chú";

            var headerRange = worksheet.Range("A1:C1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            int row = 2;
            foreach (var item in DishList)
            {
                worksheet.Cell(row, 1).Value = item.DishName;
                worksheet.Cell(row, 2).Value = item.UnitPrice;
                worksheet.Cell(row, 3).Value = item.Note;

                for (int col = 1; col <= 3; col++)
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
                FileName = $"DanhSachMonAn_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
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
            if (string.IsNullOrEmpty(SearchText))
            {
                DishList = OriginalList;
                return;
            }

            switch (SelectedSearchProperty)
            {
                case "Tên món ăn":
                    DishList = new ObservableCollection<DishDTO>(OriginalList.Where(x => !string.IsNullOrEmpty(x.DishName) && x.DishName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                    break;
                case "Đơn giá":
                    DishList = OriginalList;
                    break;
                case "Ghi chú":
                    DishList = new ObservableCollection<DishDTO>(OriginalList.Where(x => !string.IsNullOrEmpty(x.Note) && x.Note.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                    break;
                default:
                    DishList = OriginalList;
                    break;
            }
        }

        private void Reset()
        {
            SelectedItem = null;
            DishName = string.Empty;
            UnitPrice = string.Empty;
            Note = string.Empty;
            SearchText = string.Empty;

            string folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
            string addCache = Path.Combine(folder, "Addcache.jpg");
            string editCache = Path.Combine(folder, "Editcache.jpg");
            if (File.Exists(addCache)) File.Delete(addCache);
            if (File.Exists(editCache)) File.Delete(editCache);
        }

        private Exception GetInnermost(Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            return ex;
        }
        #endregion
    }
}
