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
    public class ServiceViewModel : BaseViewModel
    {
        #region Service & Collections
        private readonly IServiceService _serviceService;
        private readonly IServiceDetailService _serviceDetailService;

        private ObservableCollection<ServiceDTO> _serviceList;
        public ObservableCollection<ServiceDTO> ServiceList
        {
            get => _serviceList;
            set { _serviceList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ServiceDTO> _originalList;
        public ObservableCollection<ServiceDTO> OriginalList
        {
            get => _originalList;
            set { _originalList = value; OnPropertyChanged(); }
        }
        #endregion

        #region Selected Item
        private ServiceDTO _selectedItem;
        public ServiceDTO SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();

                if (SelectedItem != null)
                {
                    ServiceName = SelectedItem.ServiceName;
                    UnitPrice = SelectedItem.UnitPrice?.ToString("G29") ?? string.Empty;
                    Note = SelectedItem.Note;
                    if (!IsAdding)
                    {
                        RenderImageAsync(SelectedItem.ServiceId.ToString(), "Service");
                    }
                    if (IsEditing)
                    {
                        string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
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
        private string _serviceName;
        public string ServiceName
        {
            get => _serviceName;
            set { _serviceName = value; OnPropertyChanged(); }
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
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                if (IsEditing && SelectedItem != null)
                {
                    string currentImagePath = Path.Combine(folder, SelectedItem.ServiceId + ".jpg");
                    string cachePath = Path.Combine(folder, "Editcache.jpg");

                    if (File.Exists(currentImagePath) || File.Exists(cachePath))
                    {
                        var result = MessageBox.Show(
                            "Bạn đã chọn ảnh. Bạn muốn đổi ảnh (Yes) hay xóa ảnh (No)?",
                            "Ảnh dịch vụ",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);

                        if (result == MessageBoxResult.Yes)
                        {
                            var dlg = new OpenFileDialog
                            {
                                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                                Title = "Chọn ảnh dịch vụ"
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
                            Title = "Chọn ảnh dịch vụ"
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
                            "Ảnh dịch vụ",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);

                        if (result == MessageBoxResult.Yes)
                        {
                            var dlg = new OpenFileDialog
                            {
                                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                                Title = "Chọn ảnh dịch vụ"
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
                            Title = "Chọn ảnh dịch vụ"
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
        public ServiceViewModel(IServiceService serviceService, IServiceDetailService serviceDetailService)
        {
            Image = null;
            _serviceService = serviceService;
            _serviceDetailService = serviceDetailService;

            ServiceList = new ObservableCollection<ServiceDTO>(_serviceService.GetAll().ToList());
            OriginalList = new ObservableCollection<ServiceDTO>(ServiceList);

            SearchProperties = new ObservableCollection<string> { "Tên dịch vụ", "Đơn giá", "Ghi chú" };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>(
                (p) => CanAdd(),
                (p) => AddService()
            );

            EditCommand = new RelayCommand<object>(
                (p) => CanEdit(),
                (p) => EditService()
            );

            DeleteCommand = new RelayCommand<object>(
                (p) => CanDelete(),
                (p) => DeleteService()
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
            AddMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(ServiceName))
            {
                if (SelectedItem != null)
                {
                    AddMessage = "Vui lòng nhập tên dịch vụ";
                }
                return false;
            }

            if (string.IsNullOrWhiteSpace(UnitPrice))
            {
                AddMessage = "Vui lòng nhập đơn giá";
                return false;
            }

            if (!TryParsePrice(UnitPrice, out var price) || price < 0)
            {
                AddMessage = "Vui lòng nhập đơn giá hợp lệ";
                return false;
            }

            var exists = OriginalList.Any(x => x.ServiceName == ServiceName);
            if (exists)
            {
                AddMessage = "Tên dịch vụ đã tồn tại";
                return false;
            }

            return true;
        }

        private void AddService()
        {
            try
            {
                var newService = new ServiceDTO()
                {
                    ServiceName = ServiceName.Trim(),
                    UnitPrice = decimal.Parse(UnitPrice.Replace(".", "").Replace(",", "")),
                    Note = Note
                };

                _serviceService.Create(newService);
                OriginalList.Add(newService);
                ServiceList = new ObservableCollection<ServiceDTO>(OriginalList);

                string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                string cachePath = Path.Combine(folder, "Addcache.jpg");
                if (File.Exists(cachePath))
                {
                    string newImagePath = Path.Combine(folder, newService.ServiceId + ".jpg");
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
            EditMessage = string.Empty;

            if (SelectedItem == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ServiceName))
            {
                EditMessage = "Vui lòng nhập tên dịch vụ";
                return false;
            }

            if (string.IsNullOrWhiteSpace(UnitPrice))
            {
                EditMessage = "Vui lòng nhập đơn giá";
                return false;
            }

            if (!TryParsePrice(UnitPrice, out var price) || price < 0)
            {
                EditMessage = "Vui lòng nhập đơn giá hợp lệ";
                return false;
            }

            var exists = OriginalList.Any(x => x.ServiceName == ServiceName && x.ServiceId != SelectedItem.ServiceId);
            if (exists)
            {
                EditMessage = "Tên dịch vụ đã tồn tại";
                return false;
            }

            var folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
            var imagePath = Path.Combine(folder, SelectedItem.ServiceId + ".jpg");
            var imageExists = File.Exists(imagePath);

            if (SelectedItem.ServiceName == ServiceName &&
                SelectedItem.UnitPrice?.ToString("G29") == UnitPrice &&
                SelectedItem.Note == Note &&
                !(imageExists && Image == null) &&
                ImageHelper.IsEditCacheImageSameAsCurrent(SelectedItem.ServiceId, "Service"))
            {
                EditMessage = "Không có thay đổi nào để cập nhật";
                return false;
            }

            return true;
        }

        private void EditService()
        {
            try
            {
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                if (Image == null)
                {
                    string imagePath = Path.Combine(folder, SelectedItem.ServiceId + ".jpg");
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                var updateDto = new ServiceDTO()
                {
                    ServiceId = SelectedItem.ServiceId,
                    ServiceName = ServiceName.Trim(),
                    UnitPrice = decimal.Parse(UnitPrice.Replace(".", "").Replace(",", "")),
                    Note = Note
                };

                _serviceService.Update(updateDto);

                var index = ServiceList.IndexOf(SelectedItem);
                ServiceList[index] = null;
                ServiceList[index] = updateDto;
                OriginalList[index] = updateDto;

                string cachePath = Path.Combine(folder, "Editcache.jpg");
                if (File.Exists(cachePath))
                {
                    string newImagePath = Path.Combine(folder, updateDto.ServiceId + ".jpg");
                    File.Copy(cachePath, newImagePath, true);
                    File.Delete(cachePath);
                }
                SelectedAction = null;

                Reset();
                MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                EditMessage = $"Lỗi khi cập nhật: {ex.Message}";
            }
        }
        #endregion

        #region Delete
        private bool CanDelete()
        {
            if (SelectedItem == null)
            {
                DeleteMessage = "Vui lòng chọn một dịch vụ để xóa.";
                return false;
            }

            var existsInBooking = _serviceDetailService.GetAll().Any(ct => ct.ServiceId == SelectedItem.ServiceId);
            if (existsInBooking)
            {
                DeleteMessage = "Dịch vụ này đang được sử dụng trong phiếu đặt tiệc, không thể xóa.";
                return false;
            }

            DeleteMessage = string.Empty;
            return true;
        }

        private void DeleteService()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa dịch vụ này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                string imagePath = Path.Combine(folder, SelectedItem.ServiceId + ".jpg");
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                _serviceService.Delete(SelectedItem.ServiceId);
                ServiceList.Remove(SelectedItem);
                OriginalList.Remove(SelectedItem);

                Reset();
                MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                DeleteMessage = $"Lỗi khi xóa: {ex.Message}";
            }
        }
        #endregion

        #region ExportToExcel
        private void ExportToExcel()
        {
            if (ServiceList == null || ServiceList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách Dịch vụ");

            worksheet.Cell(1, 1).Value = "Tên dịch vụ";
            worksheet.Cell(1, 2).Value = "Đơn giá";
            worksheet.Cell(1, 3).Value = "Ghi chú";

            var headerRange = worksheet.Range("A1:C1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            int row = 2;
            foreach (var item in ServiceList)
            {
                worksheet.Cell(row, 1).Value = item.ServiceName;
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
                FileName = $"DanhSachDichVu_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
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
                ServiceName = string.Empty;
                UnitPrice = null;
                Note = string.Empty;

                if (string.IsNullOrWhiteSpace(SearchText) || string.IsNullOrWhiteSpace(SelectedSearchProperty))
                {
                    ServiceList = OriginalList;
                    return;
                }

                string search = SearchText.Trim();

                switch (SelectedSearchProperty)
                {
                    case "Tên dịch vụ":
                        ServiceList = new ObservableCollection<ServiceDTO>(
                            OriginalList.Where(x => !string.IsNullOrWhiteSpace(x.ServiceName) &&
                                                    x.ServiceName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;

                    case "Đơn giá":
                        if (decimal.TryParse(search.Replace(",", "").Trim(), out decimal searchPrice))
                        {
                            ServiceList = new ObservableCollection<ServiceDTO>(
                                OriginalList.Where(x => x.UnitPrice.HasValue && x.UnitPrice.Value == searchPrice));
                        }
                        else
                        {
                            ServiceList = new ObservableCollection<ServiceDTO>();
                        }
                        break;

                    case "Ghi chú":
                        ServiceList = new ObservableCollection<ServiceDTO>(
                            OriginalList.Where(x => !string.IsNullOrWhiteSpace(x.Note) &&
                                                    x.Note.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;

                    default:
                        ServiceList = OriginalList;
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
            ServiceName = string.Empty;
            UnitPrice = null;
            Note = string.Empty;
            SearchText = string.Empty;

            string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
            string addCache = Path.Combine(folder, "Addcache.jpg");
            string editCache = Path.Combine(folder, "Editcache.jpg");
            if (File.Exists(addCache)) File.Delete(addCache);
            if (File.Exists(editCache)) File.Delete(editCache);
        }
        #endregion
    }
}