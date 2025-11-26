using ClosedXML.Excel;
using Microsoft.Win32;
using QuanLyTiecCuoi.BusinessLogicLayer.Helpers;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
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
        private readonly IDichVuService _dichVuService;
        private readonly IChiTietDVService _chiTietDVService;

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
                    if (!IsAdding)
                    {
                        RenderImageAsync(SelectedItem.MaDichVu.ToString(), "Service");
                    }
                    if (IsEditing)
                    {
                        // Clean up cache files
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
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                if (IsEditing && SelectedItem != null)
                {
                    string currentImagePath = Path.Combine(folder, SelectedItem.MaDichVu + ".jpg");
                    string cachePath = Path.Combine(folder, "Editcache.jpg");

                    if (File.Exists(currentImagePath) || File.Exists(cachePath))
                    {
                        var result = MessageBox.Show(
                            "Bạn đã chọn ảnh. Bạn muốn đổi ảnh (Yes) hay xóa ảnh (No)?",
                            "Ảnh dịch vụ",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);

                        if (result == MessageBoxResult.Yes) // Đổi ảnh
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

                        if (result == MessageBoxResult.Yes) // Đổi ảnh
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
        public ICommand ExportToExcelCommand { get; set; }
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


        public ServiceViewModel(IDichVuService dichVuService, IChiTietDVService chiTietDVService)
        {
            //MessageBox.Show("Chào mừng bạn đến với quản lý dịch vụ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            _dichVuService = dichVuService;
            _chiTietDVService = chiTietDVService;
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

                    // Move Addcache.jpg to real image
                    string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                    string cachePath = Path.Combine(folder, "Addcache.jpg");
                    if (File.Exists(cachePath))
                    {
                        string newImagePath = Path.Combine(folder, newService.MaDichVu + ".jpg");
                        File.Copy(cachePath, newImagePath, true);
                        File.Delete(cachePath);
                    }
                    SelectedAction = null; // Reset action selection

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
                var folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                var imagePath = Path.Combine(folder, SelectedItem.MaDichVu + ".jpg");
                var imageExists = File.Exists(imagePath);
                if (SelectedItem.TenDichVu == TenDichVu &&
                    SelectedItem.DonGia?.ToString("G29") == DonGia &&
                    SelectedItem.GhiChu == GhiChu &&
                    !(imageExists && Image == null) &&
                    ImageHelper.IsEditCacheImageSameAsCurrent(SelectedItem.MaDichVu, "Service"))
                {
                    EditMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                return true;
            }, (p) =>
            {
                try
                {
                    string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                    if (Image == null)
                    {
                        string imagePath = Path.Combine(folder, SelectedItem.MaDichVu + ".jpg");

                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }
                    }

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


                    // Move Editcache.jpg to real image
                    string cachePath = Path.Combine(folder, "Editcache.jpg");
                    if (File.Exists(cachePath))
                    {
                        string newImagePath = Path.Combine(folder, updateDto.MaDichVu + ".jpg");
                        File.Copy(cachePath, newImagePath, true);
                        File.Delete(cachePath);
                    }
                    SelectedAction = null; // Reset action selection

                    // thông báo cập nhật thành công
                    MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
                // Không cho phép xóa nếu chưa chọn
                if (SelectedItem == null)
                {
                    DeleteMessage = "Vui lòng chọn một dịch vụ để xóa.";
                    return false;
                }
                // Không cho phép xóa nếu dịch vụ đã được sử dụng trong phiếu đặt tiệc
                var existsInPhieuDatTiec = _chiTietDVService.GetAll().Any(ct => ct.MaDichVu == SelectedItem.MaDichVu);
                if (existsInPhieuDatTiec)
                {
                    DeleteMessage = "Dịch vụ này đang được sử dụng trong phiếu đặt tiệc, không thể xóa.";
                    return false;
                }
                DeleteMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa dịch vụ này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Xóa ảnh của dịch vụ
                        string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
                        string imagePath = Path.Combine(folder, SelectedItem.MaDichVu + ".jpg");
                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }


                        _dichVuService.Delete(SelectedItem.MaDichVu);
                        List.Remove(SelectedItem);
                        //DeleteMessage = "Xóa thành công";
                        MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        Reset();
                    }
                }
                catch (Exception ex)
                {
                    DeleteMessage = $"Lỗi khi xóa: {ex.Message}";
                }
            });
            ExportToExcelCommand = new RelayCommand<object>((p) => true, (p) => ExportToExcel());
            ResetCommand = new RelayCommand<object>((p) => true, (p) => Reset());
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
            var worksheet = workbook.Worksheets.Add("Danh sách Dịch vụ");

            // Tiêu đề cột
            worksheet.Cell(1, 1).Value = "Tên dịch vụ";
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
                worksheet.Cell(row, 1).Value = item.TenDichVu;
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
        private void Reset()
        {
            SelectedItem = null;
            TenDichVu = string.Empty;
            DonGia = null;
            GhiChu = string.Empty;
            SearchText = string.Empty;

            // Clean up cache files
            string folder = Path.Combine(ImageHelper.BaseImagePath, "Service");
            string addCache = Path.Combine(folder, "Addcache.jpg");
            string editCache = Path.Combine(folder, "Editcache.jpg");
            if (File.Exists(addCache)) File.Delete(addCache);
            if (File.Exists(editCache)) File.Delete(editCache);
        }
    }
}