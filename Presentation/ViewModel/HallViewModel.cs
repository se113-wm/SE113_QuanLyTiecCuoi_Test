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
    public class HallViewModel : BaseViewModel
    {
        #region Service & Collections
        private readonly IHallService _hallService;
        private readonly IHallTypeService _hallTypeService;
        private readonly IBookingService _bookingService;

        private ObservableCollection<HallDTO> _hallList;
        public ObservableCollection<HallDTO> HallList
        {
            get => _hallList;
            set { _hallList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<HallDTO> _originalList;
        public ObservableCollection<HallDTO> OriginalList
        {
            get => _originalList;
            set { _originalList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<HallTypeDTO> _hallTypes;
        public ObservableCollection<HallTypeDTO> HallTypes
        {
            get => _hallTypes;
            set { _hallTypes = value; OnPropertyChanged(); }
        }
        #endregion

        #region Selected Item
        private HallDTO _selectedItem;
        public HallDTO SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();

                if (SelectedItem != null)
                {
                    HallName = SelectedItem.HallName;
                    MaxTableCount = SelectedItem.MaxTableCount?.ToString();
                    MinTablePrice = SelectedItem.HallType?.MinTablePrice;
                    Note = SelectedItem.Note;
                    SelectedHallType = HallTypes?.FirstOrDefault(ht => ht.HallTypeId == SelectedItem.HallTypeId);
                    if (!IsAdding)
                    {
                        RenderImageAsync(SelectedItem.HallId.ToString(), "Hall");
                    }
                    if (IsEditing)
                    {
                        string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
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
        private string _hallName;
        public string HallName
        {
            get => _hallName;
            set { _hallName = value; OnPropertyChanged(); }
        }

        private string _maxTableCount;
        public string MaxTableCount
        {
            get => _maxTableCount;
            set { _maxTableCount = value; OnPropertyChanged(); }
        }

        private string _note;
        public string Note
        {
            get => _note;
            set { _note = value; OnPropertyChanged(); }
        }

        private decimal? _minTablePrice;
        public decimal? MinTablePrice
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

        private HallTypeDTO _selectedHallType;
        public HallTypeDTO SelectedHallType
        {
            get => _selectedHallType;
            set
            {
                _selectedHallType = value;
                OnPropertyChanged();
                MinTablePrice = _selectedHallType?.MinTablePrice;
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
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                if (IsEditing && SelectedItem != null)
                {
                    string currentImagePath = Path.Combine(folder, SelectedItem.HallId + ".jpg");
                    string cachePath = Path.Combine(folder, "Editcache.jpg");

                    if (File.Exists(currentImagePath) || File.Exists(cachePath))
                    {
                        var result = MessageBox.Show(
                            "Bạn đã chọn ảnh. Bạn muốn đổi ảnh (Yes) hay xóa ảnh (No)?",
                            "Ảnh sảnh",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);

                        if (result == MessageBoxResult.Yes)
                        {
                            var dlg = new OpenFileDialog
                            {
                                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                                Title = "Chọn ảnh sảnh"
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
                            Title = "Chọn ảnh sảnh"
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
                            "Ảnh sảnh",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);

                        if (result == MessageBoxResult.Yes)
                        {
                            var dlg = new OpenFileDialog
                            {
                                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                                Title = "Chọn ảnh sảnh"
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
                            Title = "Chọn ảnh sảnh"
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
        public HallViewModel(IHallService hallService, IHallTypeService hallTypeService, IBookingService bookingService)
        {
            Image = null;
            _hallService = hallService;
            _hallTypeService = hallTypeService;
            _bookingService = bookingService;

            HallList = new ObservableCollection<HallDTO>(_hallService.GetAll().ToList());
            OriginalList = new ObservableCollection<HallDTO>(HallList);
            HallTypes = new ObservableCollection<HallTypeDTO>(_hallTypeService.GetAll().ToList());

            SearchProperties = new ObservableCollection<string>
            {
                "Tên sảnh",
                "Tên loại sảnh",
                "Đơn giá bàn tối thiểu",
                "Số lượng bàn tối đa",
                "Ghi chú"
            };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>(
                (p) => CanAdd(),
                (p) => AddHall()
            );

            EditCommand = new RelayCommand<object>(
                (p) => CanEdit(),
                (p) => EditHall()
            );

            DeleteCommand = new RelayCommand<object>(
                (p) => CanDelete(),
                (p) => DeleteHall()
            );

            ResetCommand = new RelayCommand<object>(
                (p) => true,
                (p) => Reset()
            );

            ExportToExcelCommand = new RelayCommand<object>((p) => true, (p) => ExportToExcel());
        }
        #endregion

        #region Validation Helpers
        private bool TryParseTableCount(string input, out int val)
            => int.TryParse(input, out val);
        #endregion

        #region Add
        private bool CanAdd()
        {
            if (string.IsNullOrWhiteSpace(HallName))
            {
                if (SelectedItem != null)
                {
                    //AddMessage = "Vui lòng nhập tên sảnh";
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

            if (MaxTableCount == null || !TryParseTableCount(MaxTableCount, out int count) || count <= 0)
            {
                AddMessage = "Số lượng bàn tối đa phải là số nguyên dương";
                return false;
            }

            var exists = OriginalList.Any(x => x.HallName == HallName && x.HallTypeId == SelectedHallType.HallTypeId);
            if (exists)
            {
                AddMessage = "Tên sảnh đã tồn tại trong loại sảnh này";
                return false;
            }

            AddMessage = string.Empty;
            return true;
        }

        private void AddHall()
        {
            try
            {
                var newHall = new HallDTO()
                {
                    HallName = HallName.Trim(),
                    MaxTableCount = TryParseTableCount(MaxTableCount, out int count) ? count : (int?)null,
                    Note = Note,
                    HallTypeId = SelectedHallType?.HallTypeId,
                    HallType = SelectedHallType
                };

                _hallService.Create(newHall);
                OriginalList.Add(newHall);
                HallList = new ObservableCollection<HallDTO>(OriginalList);

                string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
                string cachePath = Path.Combine(folder, "Addcache.jpg");
                if (File.Exists(cachePath))
                {
                    string newImagePath = Path.Combine(folder, newHall.HallId + ".jpg");
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
                EditMessage = string.Empty;
                return false;
            }

            var folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
            var imagePath = Path.Combine(folder, SelectedItem.HallId + ".jpg");
            var imageExists = File.Exists(imagePath);

            if (SelectedItem.HallName == HallName
                && SelectedItem.MaxTableCount?.ToString() == MaxTableCount
                && SelectedItem.Note == Note
                && SelectedItem.HallTypeId == (SelectedHallType?.HallTypeId ?? 0)
                && !(imageExists && Image == null)
                && ImageHelper.IsEditCacheImageSameAsCurrent(SelectedItem.HallId, "Hall"))
            {
                EditMessage = "Không có thay đổi nào để cập nhật";
                return false;
            }

            if (string.IsNullOrWhiteSpace(HallName))
            {
                EditMessage = "Tên sảnh không được để trống";
                return false;
            }

            if (!TryParseTableCount(MaxTableCount, out int count) || count <= 0)
            {
                EditMessage = "Số lượng bàn tối đa phải là số nguyên dương";
                return false;
            }

            var exists = OriginalList.Any(x =>
                x.HallName == HallName &&
                x.HallTypeId == SelectedHallType.HallTypeId &&
                x.HallId != SelectedItem.HallId);
            if (exists)
            {
                EditMessage = "Tên sảnh đã tồn tại trong loại sảnh này";
                return false;
            }

            if (_bookingService.GetAll()
                    .Any(t => t.HallId == SelectedItem.HallId
                           && t.WeddingDate.HasValue
                           && t.WeddingDate.Value.Date >= DateTime.Today.AddDays(1)))
            {
                if (TryParseTableCount(MaxTableCount, out int newMax) && SelectedItem.MaxTableCount != newMax)
                {
                    EditMessage = "Đang có tiệc chưa tổ chức, không sửa số lượng bàn tối đa";
                    return false;
                }
            }

            EditMessage = string.Empty;
            return true;
        }

        private void EditHall()
        {
            try
            {
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
                if (Image == null)
                {
                    string imagePath = Path.Combine(folder, SelectedItem.HallId + ".jpg");
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                var updateDto = new HallDTO()
                {
                    HallId = SelectedItem.HallId,
                    HallName = HallName.Trim(),
                    MaxTableCount = TryParseTableCount(MaxTableCount, out int count) ? count : (int?)null,
                    Note = Note,
                    HallTypeId = SelectedHallType?.HallTypeId,
                    HallType = SelectedHallType
                };

                _hallService.Update(updateDto);

                var index = HallList.IndexOf(SelectedItem);
                HallList[index] = null;
                HallList[index] = updateDto;
                OriginalList[index] = updateDto;

                string cachePath = Path.Combine(folder, "Editcache.jpg");
                if (File.Exists(cachePath))
                {
                    string newImagePath = Path.Combine(folder, updateDto.HallId + ".jpg");
                    File.Copy(cachePath, newImagePath, true);
                    File.Delete(cachePath);
                }
                SelectedAction = null;

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
            {
                return false;
            }

            if (_bookingService.GetAll().Any(t => t.HallId == SelectedItem.HallId))
            {
                DeleteMessage = "Không thể xóa sảnh này vì đang có phiếu đặt tiệc sử dụng nó.";
                return false;
            }

            DeleteMessage = string.Empty;
            return true;
        }

        private void DeleteHall()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sảnh này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
                string imagePath = Path.Combine(folder, SelectedItem.HallId + ".jpg");
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                _hallService.Delete(SelectedItem.HallId);
                HallList.Remove(SelectedItem);
                OriginalList.Remove(SelectedItem);

                SelectedAction = null;
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
            if (HallList == null || HallList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách sảnh");

            worksheet.Cell(1, 1).Value = "Tên sảnh";
            worksheet.Cell(1, 2).Value = "Loại sảnh";
            worksheet.Cell(1, 3).Value = "Đơn giá bàn tối thiểu";
            worksheet.Cell(1, 4).Value = "Số lượng bàn tối đa";
            worksheet.Cell(1, 5).Value = "Ghi chú";

            var headerRange = worksheet.Range("A1:E1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            int row = 2;
            foreach (var item in HallList)
            {
                worksheet.Cell(row, 1).Value = item.HallName;
                worksheet.Cell(row, 2).Value = item.HallType?.HallTypeName;
                worksheet.Cell(row, 3).Value = item.HallType?.MinTablePrice ?? 0;
                worksheet.Cell(row, 4).Value = item.MaxTableCount ?? 0;
                worksheet.Cell(row, 5).Value = item.Note;

                for (int col = 1; col <= 5; col++)
                {
                    worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"DanhSachSanh_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
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
                HallName = string.Empty;
                MaxTableCount = null;
                Note = string.Empty;
                SelectedHallType = null;

                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    HallList = OriginalList;
                    return;
                }

                switch (SelectedSearchProperty)
                {
                    case "Tên sảnh":
                        HallList = new ObservableCollection<HallDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.HallName) &&
                                x.HallName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Tên loại sảnh":
                        HallList = new ObservableCollection<HallDTO>(
                            OriginalList.Where(x => x.HallType != null && !string.IsNullOrEmpty(x.HallType.HallTypeName) &&
                                x.HallType.HallTypeName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Đơn giá bàn tối thiểu":
                        HallList = new ObservableCollection<HallDTO>(
                            OriginalList.Where(x =>
                                x.HallType != null &&
                                x.HallType.MinTablePrice != null &&
                                x.HallType.MinTablePrice.Value.ToString("N0").Replace(",", "").Contains(SearchText.Replace(",", "").Trim())
                            )
                        );
                        break;
                    case "Số lượng bàn tối đa":
                        HallList = new ObservableCollection<HallDTO>(
                            OriginalList.Where(x =>
                                x.MaxTableCount != null &&
                                x.MaxTableCount.Value.ToString().Contains(SearchText.Trim())
                            )
                        );
                        break;
                    case "Ghi chú":
                        HallList = new ObservableCollection<HallDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.Note) &&
                                x.Note.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    default:
                        HallList = new ObservableCollection<HallDTO>(OriginalList);
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
            SelectedItem = null;
            HallName = string.Empty;
            MaxTableCount = null;
            Note = string.Empty;
            SelectedHallType = null;
            SearchText = string.Empty;

            string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
            string addCache = Path.Combine(folder, "Addcache.jpg");
            string editCache = Path.Combine(folder, "Editcache.jpg");
            if (File.Exists(addCache)) File.Delete(addCache);
            if (File.Exists(editCache)) File.Delete(editCache);
        }
        #endregion
    }
}