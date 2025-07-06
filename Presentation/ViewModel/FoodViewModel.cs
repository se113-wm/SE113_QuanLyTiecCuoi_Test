using ClosedXML.Excel;
using Microsoft.Win32;
using QuanLyTiecCuoi.BusinessLogicLayer.Helpers;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model; // Để kiểm tra FK trực tiếp khi xoá
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
    /// <summary>
    /// ViewModel quản lý bảng MONAN – viết theo cùng phong cách FoodViewModel.
    /// Hỗ trợ: giới hạn 100 món, tìm kiếm (Tên, Đơn giá, Ghi chú), Thêm / Sửa / Xoá.
    /// </summary>
    public class FoodViewModel : BaseViewModel
    {
        #region Service & Collections
        private readonly IMonAnService _foodService;

        private ObservableCollection<MONANDTO> _List;
        public ObservableCollection<MONANDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<MONANDTO> _OriginalList;
        public ObservableCollection<MONANDTO> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }
        #endregion

        #region Selected Item
        private MONANDTO _SelectedItem;
        public MONANDTO SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();

                if (SelectedItem != null)
                {
                    TenMonAn = SelectedItem.TenMonAn;
                    DonGia = SelectedItem.DonGia?.ToString("N0");
                    GhiChu = SelectedItem.GhiChu;
                    if (!IsAdding)
                    {
                        RenderImageAsync(SelectedItem.MaMonAn.ToString(), "Food");
                    }
                    if (IsEditing)
                    {
                        // Clean up cache files
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
        private string _TenMonAn;
        public string TenMonAn { get => _TenMonAn; set { _TenMonAn = value; OnPropertyChanged(); } }

        private string _DonGia;
        public string DonGia { get => _DonGia; set { _DonGia = value; OnPropertyChanged(); } }

        private string _GhiChu;
        public string GhiChu { get => _GhiChu; set { _GhiChu = value; OnPropertyChanged(); } }
        #endregion

        #region Search
        private string _SearchText;
        public string SearchText
        {
            get => _SearchText;
            set
            {
                if (_SearchText != value)
                {
                    _SearchText = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        private ObservableCollection<string> _SearchProperties;
        public ObservableCollection<string> SearchProperties { get => _SearchProperties; set { _SearchProperties = value; OnPropertyChanged(); } }

        private string _SelectedSearchProperty;
        public string SelectedSearchProperty
        {
            get => _SelectedSearchProperty;
            set { _SelectedSearchProperty = value; OnPropertyChanged(); PerformSearch(); }
        }
        #endregion

        #region selecte action

        private bool _nullImage;
        public bool nullImage
        {
            get => _nullImage;
            set { _nullImage = value; OnPropertyChanged(); }
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
                        // reset ảnh về ko có ảnh
                        Image = null;
                        break;
                    case "Sửa":
                        IsAdding = false;
                        IsEditing = true;
                        IsDeleting = false;
                        IsExporting = false;
                        Reset(); // reset các trường nhập liệu
                        //if (SelectedItem == null)
                        //{
                        //    MessageBox.Show("Vui lòng chọn một dịch vụ để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    return;
                        //}
                        break;
                    case "Xóa":
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = true;
                        IsExporting = false;
                        Reset(); // reset các trường nhập liệu
                        //if (SelectedItem == null)
                        //{
                        //    MessageBox.Show("Vui lòng chọn một dịch vụ để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
        #endregion

        #region Image Handling


        private ImageSource _Image;
        public ImageSource Image
        {
            get => _Image;
            set { _Image = value; OnPropertyChanged(); nullImage = Image == null ? true : false; }
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
                //// Allow only when adding or editing
                //return IsAdding || IsEditing;
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
                    string currentImagePath = Path.Combine(folder, SelectedItem.MaMonAn + ".jpg");
                    string cachePath = Path.Combine(folder, "Editcache.jpg");

                    if (File.Exists(currentImagePath) || File.Exists(cachePath))
                    {
                        var result = MessageBox.Show(
                            "Bạn đã chọn ảnh. Bạn muốn đổi ảnh (Yes) hay xóa ảnh (No)?",
                            "Ảnh món ăn",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);

                        if (result == MessageBoxResult.Yes) // Đổi ảnh
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
                        else if (result == MessageBoxResult.No) // Xóa ảnh
                        {
                            try
                            {
                                //if (File.Exists(currentImagePath)) File.Delete(currentImagePath);
                                if (File.Exists(cachePath)) File.Delete(cachePath);
                                Image = null;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Không thể xóa ảnh: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        // Cancel: do nothing
                    }
                    else // Chưa có ảnh, chọn ảnh như bình thường
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

                        if (result == MessageBoxResult.Yes) // Đổi ảnh
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
                        else if (result == MessageBoxResult.No) // Xóa ảnh
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
                        // Cancel: do nothing
                    }
                    else // Chưa có ảnh, chọn ảnh như bình thường
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
                // Load original image
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

                // Calculate crop rectangle (centered)
                int x = (original.PixelWidth - cropSize) / 2;
                int y = (original.PixelHeight - cropSize) / 2;

                // Crop to square
                var cropped = new CroppedBitmap(original, new Int32Rect(x, y, cropSize, cropSize));

                // Resize to 100x100
                var targetSize = 500;
                var resized = new TransformedBitmap(cropped, new ScaleTransform(
                    (double)targetSize / cropSize,
                    (double)targetSize / cropSize
                ));

                // Encode and overwrite the file
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(resized));
                using (var outStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(outStream);
                }

                // Reload the resized image for display
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

        private void RenderImageAsync(string id, string loai)
        {
            var path = ImageHelper.GetImagePath(loai, id);
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
        private string _AddMessage;
        public string AddMessage { get => _AddMessage; set { _AddMessage = value; OnPropertyChanged(); } }

        private string _EditMessage;
        public string EditMessage { get => _EditMessage; set { _EditMessage = value; OnPropertyChanged(); } }

        private string _DeleteMessage;
        public string DeleteMessage { get => _DeleteMessage; set { _DeleteMessage = value; OnPropertyChanged(); } }

        private void ClearMessages() => (AddMessage, EditMessage, DeleteMessage) = (string.Empty, string.Empty, string.Empty);

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ExportToExcelCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        #endregion

        #region Constructor
        public FoodViewModel()
        {
            Image = null;
            _foodService = new MonAnService();

            List = new ObservableCollection<MONANDTO>(_foodService.GetAll().ToList());
            OriginalList = new ObservableCollection<MONANDTO>(List);

            SearchProperties = new ObservableCollection<string> { "Tên món ăn", "Đơn giá", "Ghi chú" };
            SelectedSearchProperty = SearchProperties.First();

            AddCommand = new RelayCommand<object>(
                (p) => CanAdd(),
                (p) => AddFood()
            );

            EditCommand = new RelayCommand<object>(
                (p) => CanEdit(),
                (p) => EditFood()
            );

            DeleteCommand = new RelayCommand<object>(
                (p) => CanDelete(),
                (p) => DeleteFood()
            );
            ResetCommand = new RelayCommand<object>(
                (p) => true,
                (p) => Reset()
            );
            ExportToExcelCommand = new RelayCommand<object>((p) => true, (p) => ExportToExcel());
        }
        #endregion

        #region Validation Helpers
        private bool TryParseDonGia(string input, out decimal val)
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
            if (string.IsNullOrWhiteSpace(TenMonAn)) { AddMessage = "Tên món ăn không được để trống"; return false; }
            if (!TryParseDonGia(DonGia, out var price) || price <= 0) { AddMessage = "Đơn giá phải là số dương"; return false; }
            if (!string.IsNullOrEmpty(GhiChu) && GhiChu.Length > 100) { AddMessage = "Ghi chú tối đa 100 ký tự"; return false; }
            var exists = OriginalList.Any(x => x.TenMonAn.Trim().Equals(TenMonAn.Trim(), StringComparison.OrdinalIgnoreCase));
            if (exists) { AddMessage = "Tên món ăn đã tồn tại"; return false; }
            AddMessage = string.Empty; return true;
        }

        private void AddFood()
        {
            try
            {
                var dto = new MONANDTO
                {
                    TenMonAn = TenMonAn.Trim(),
                    DonGia = decimal.Parse(DonGia),
                    GhiChu = GhiChu
                };
                _foodService.Create(dto);

                OriginalList.Add(dto); // Thêm vào danh sách gốc
                List = new ObservableCollection<MONANDTO>(OriginalList); // Cập nhật lại danh sách hiển thị

                // Move Addcache.jpg to real image
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                string cachePath = Path.Combine(folder, "Addcache.jpg");
                if (File.Exists(cachePath))
                {
                    string newImagePath = Path.Combine(folder, dto.MaMonAn + ".jpg");
                    File.Copy(cachePath, newImagePath, true);
                    File.Delete(cachePath);
                }
                SelectedAction = null; // Reset action selection

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
                EditMessage = "Vui lòng chọn món ăn";
                return false;
            }
            var folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
            var imagePath = Path.Combine(folder, SelectedItem.MaMonAn + ".jpg");
            var imageExists = File.Exists(imagePath);
            // So sánh dữ liệu gốc với dữ liệu hiện tại
            if (
                SelectedItem.TenMonAn == TenMonAn?.Trim() &&
                SelectedItem.GhiChu == GhiChu &&
                decimal.TryParse(DonGia?.Replace(".", "").Replace(",", ""), out var newPrice) &&
                SelectedItem.DonGia == newPrice &&
                 !(imageExists && Image == null) &&
                ImageHelper.IsEditCacheImageSameAsCurrent(SelectedItem.MaMonAn, "Food")
            )
            {
                EditMessage = "Chưa có thông tin nào thay đổi";
                return false;
            }

            if (string.IsNullOrWhiteSpace(TenMonAn))
            {
                EditMessage = "Tên món ăn không được để trống";
                return false;
            }

            if (!TryParseDonGia(DonGia, out var price) || price <= 0)
            {
                EditMessage = "Đơn giá phải là số dương";
                return false;
            }

            if (!string.IsNullOrEmpty(GhiChu) && GhiChu.Length > 100)
            {
                EditMessage = "Ghi chú tối đa 100 ký tự";
                return false;
            }

            var exists = OriginalList.Any(x => x.MaMonAn != SelectedItem.MaMonAn && x.TenMonAn.Trim().Equals(TenMonAn.Trim(), StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                EditMessage = "Tên món ăn đã tồn tại";
                return false;
            }

            EditMessage = string.Empty;
            return true;
        }


        private void EditFood()
        {
            try
            {

                string folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                if (Image == null)
                {
                    string imagePath = Path.Combine(folder, SelectedItem.MaMonAn + ".jpg");

                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                var dto = new MONANDTO
                {
                    MaMonAn = SelectedItem.MaMonAn,
                    TenMonAn = TenMonAn.Trim(),
                    DonGia = decimal.Parse(DonGia),
                    GhiChu = GhiChu
                };
                _foodService.Update(dto);

                var idx = List.IndexOf(SelectedItem);
                List[idx] = dto; OriginalList[idx] = dto;
                // Move Editcache.jpg to real image
                string cachePath = Path.Combine(folder, "Editcache.jpg");
                if (File.Exists(cachePath))
                {
                    string newImagePath = Path.Combine(folder, dto.MaMonAn + ".jpg");
                    File.Copy(cachePath, newImagePath, true);
                    File.Delete(cachePath);
                }
                SelectedAction = null; // Reset action selection
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
        private bool CanDelete() => SelectedItem != null;

        private void DeleteFood()
        {
            if (MessageBox.Show("Bạn có chắc muốn xoá món ăn này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                // Kiểm tra khoá ngoại trực tiếp
                using (var ctx = new QuanLyTiecCuoiEntities())
                {
                    bool isUsed = ctx.THUCDONs.Any(t => t.MaMonAn == SelectedItem.MaMonAn);
                    if (isUsed)
                    {
                        MessageBox.Show("Món ăn đang được sử dụng trong thực đơn – không thể xoá.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Xóa ảnh của món ăn
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Food");
                string imagePath = Path.Combine(folder, SelectedItem.MaMonAn + ".jpg");
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                _foodService.Delete(SelectedItem.MaMonAn);
                List.Remove(SelectedItem); OriginalList.Remove(SelectedItem);
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
            if (List == null || List.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách Món ăn");

            // Tiêu đề cột
            worksheet.Cell(1, 1).Value = "Tên món ăn";
            worksheet.Cell(1, 2).Value = "Đơn giá";
            worksheet.Cell(1, 3).Value = "Ghi chú";

            // Format tiêu đề
            var headerRange = worksheet.Range("A1:C1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Ghi dữ liệu
            int row = 2;
            foreach (var item in List)
            {
                worksheet.Cell(row, 1).Value = item.TenMonAn;
                worksheet.Cell(row, 2).Value = item.DonGia;
                worksheet.Cell(row, 3).Value = item.GhiChu;

                // Format dòng dữ liệu
                for (int col = 1; col <= 3; col++)
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
#endregion ExportToExcel
        #region Helpers
        private void PerformSearch()
        {
            if (string.IsNullOrEmpty(SearchText)) { List = OriginalList; return; }

            switch (SelectedSearchProperty)
            {
                case "Tên món ăn":
                    List = new ObservableCollection<MONANDTO>(OriginalList.Where(x => !string.IsNullOrEmpty(x.TenMonAn) && x.TenMonAn.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                    break;
                case "Đơn giá":
                    List = new ObservableCollection<MONANDTO>(OriginalList.Where(x => x.DonGia.HasValue && x.DonGia.Value.ToString().Contains(SearchText)));
                    break;
                case "Ghi chú":
                    List = new ObservableCollection<MONANDTO>(OriginalList.Where(x => !string.IsNullOrEmpty(x.GhiChu) && x.GhiChu.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                    break;
                default:
                    List = OriginalList; break;
            }
        }

        private void Reset()
        {
            SelectedItem = null;
            TenMonAn = string.Empty;
            DonGia = string.Empty;
            GhiChu = string.Empty;
            SearchText = string.Empty;

            // Clean up cache files
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
