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
using static QuanLyTiecCuoi.Presentation.ViewModel.ReportViewModel;

namespace QuanLyTiecCuoi.ViewModel
{
    public class HallViewModel : BaseViewModel
    {
        private readonly ISanhService _sanhService;
        private readonly ILoaiSanhService _loaiSanhService;

        private ObservableCollection<SANHDTO> _List;
        public ObservableCollection<SANHDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<SANHDTO> _OriginalList;
        public ObservableCollection<SANHDTO> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private ObservableCollection<LOAISANHDTO> _HallTypes;
        public ObservableCollection<LOAISANHDTO> HallTypes { get => _HallTypes; set { _HallTypes = value; OnPropertyChanged(); } }

        private SANHDTO _SelectedItem;
        public SANHDTO SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    TenSanh = SelectedItem.TenSanh;
                    SoLuongBanToiDa = SelectedItem.SoLuongBanToiDa?.ToString();
                    DonGiaBanToiThieu = SelectedItem.LoaiSanh?.DonGiaBanToiThieu;
                    GhiChu = SelectedItem.GhiChu;
                    SelectedHallType = HallTypes?.FirstOrDefault(ht => ht.MaLoaiSanh == SelectedItem.MaLoaiSanh);
                    if (!IsAdding)
                    {
                        RenderImageAsync(SelectedItem.MaSanh.ToString(), "Hall");
                    }
                    if (IsEditing)
                    {
                        // Clean up cache files
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

        public ObservableCollection<string> ActionList { get; } = new ObservableCollection<string> { "Thêm", "Sửa", "Xóa", "Xuất Excel", "Chọn thao tác"};
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
                        Reset(); // reset các trường nhập liệu
                        break;
                }
            }
        }

        private LOAISANHDTO _SelectedHallType;
        public LOAISANHDTO SelectedHallType
        {
            get => _SelectedHallType;
            set
            {
                _SelectedHallType = value;
                OnPropertyChanged();
                DonGiaBanToiThieu = _SelectedHallType?.DonGiaBanToiThieu;
            }
        }

        private string _TenSanh;
        public string TenSanh { get => _TenSanh; set { _TenSanh = value; OnPropertyChanged(); } }

        private string _SoLuongBanToiDa;
        public string SoLuongBanToiDa { get => _SoLuongBanToiDa; set { _SoLuongBanToiDa = value; OnPropertyChanged(); } }

        private string _GhiChu;
        public string GhiChu { get => _GhiChu; set { _GhiChu = value; OnPropertyChanged(); } }

        private decimal? _DonGiaBanToiThieu;
        public decimal? DonGiaBanToiThieu { get => _DonGiaBanToiThieu; set { _DonGiaBanToiThieu = value; OnPropertyChanged(); } }

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
                string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                if (IsEditing && SelectedItem != null)
                {
                    string currentImagePath = Path.Combine(folder, SelectedItem.MaSanh + ".jpg");
                    string cachePath = Path.Combine(folder, "Editcache.jpg");

                    if (File.Exists(currentImagePath) || File.Exists(cachePath))
                    {
                        var result = MessageBox.Show(
                            "Bạn đã chọn ảnh. Bạn muốn đổi ảnh (Yes) hay xóa ảnh (No)?",
                            "Ảnh sảnh",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question,
                            MessageBoxResult.Cancel);

                        if (result == MessageBoxResult.Yes) // Đổi ảnh
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

                        if (result == MessageBoxResult.Yes) // Đổi ảnh
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
                TenSanh = string.Empty;
                SoLuongBanToiDa = null;
                GhiChu = string.Empty;
                SelectedHallType = null;
                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    List = OriginalList;
                    return;
                }

                switch (SelectedSearchProperty)
                {
                    case "Tên sảnh":
                        List = new ObservableCollection<SANHDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.TenSanh) &&
                                x.TenSanh.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Tên loại sảnh":
                        List = new ObservableCollection<SANHDTO>(
                            OriginalList.Where(x => x.LoaiSanh != null && !string.IsNullOrEmpty(x.LoaiSanh.TenLoaiSanh) &&
                                x.LoaiSanh.TenLoaiSanh.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Đơn giá bàn tối thiểu":
                        List = new ObservableCollection<SANHDTO>(
                            OriginalList.Where(x =>
                                x.LoaiSanh != null &&
                                x.LoaiSanh.DonGiaBanToiThieu != null &&
                                x.LoaiSanh.DonGiaBanToiThieu.Value.ToString("N0").Replace(",", "").Contains(SearchText.Replace(",", "").Trim())
                            )
                        );
                        break;
                    case "Số lượng bàn tối đa":
                        List = new ObservableCollection<SANHDTO>(
                            OriginalList.Where(x =>
                                x.SoLuongBanToiDa != null &&
                                x.SoLuongBanToiDa.Value.ToString().Contains(SearchText.Trim())
                            )
                        );
                        break;
                    case "Ghi chú":
                        List = new ObservableCollection<SANHDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.GhiChu) &&
                                x.GhiChu.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    default:
                        List = new ObservableCollection<SANHDTO>(OriginalList);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
        public ICommand ExportToExcelCommand { get; set; }


        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => {
            Reset();
        });

        public HallViewModel()
        {
            _sanhService = new SanhService();
            _loaiSanhService = new LoaiSanhService();

            List = new ObservableCollection<SANHDTO>(_sanhService.GetAll().ToList());
            OriginalList = new ObservableCollection<SANHDTO>(List);
            HallTypes = new ObservableCollection<LOAISANHDTO>(_loaiSanhService.GetAll().ToList());

            SearchProperties = new ObservableCollection<string>
            {
                "Tên sảnh",
                "Tên loại sảnh",
                "Đơn giá bàn tối thiểu",
                "Số lượng bàn tối đa",
                "Ghi chú"
            };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrWhiteSpace(TenSanh))
                {
                    if (SelectedItem != null)
                    {
                        AddMessage = "Vui lòng nhập tên sảnh";
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
                if (SoLuongBanToiDa == null || !int.TryParse(SoLuongBanToiDa, out int soLuong) || soLuong <= 0)
                {
                    AddMessage = "Số lượng bàn tối đa phải là số nguyên dương";
                    return false;
                }
                var exists = OriginalList.Any(x => x.TenSanh == TenSanh && x.MaLoaiSanh == SelectedHallType.MaLoaiSanh);
                if (exists)
                {
                    AddMessage = "Tên sảnh đã tồn tại trong loại sảnh này";
                    return false;
                }
                AddMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var newHall = new SANHDTO()
                    {
                        TenSanh = TenSanh,
                        SoLuongBanToiDa = int.TryParse(SoLuongBanToiDa, out int soLuong) ? soLuong : (int?)null,
                        GhiChu = GhiChu,
                        MaLoaiSanh = SelectedHallType?.MaLoaiSanh,
                        LoaiSanh = SelectedHallType
                    };

                    _sanhService.Create(newHall);

                    List.Add(newHall);

                    // Move Addcache.jpg to real image
                    string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
                    string cachePath = Path.Combine(folder, "Addcache.jpg");
                    if (File.Exists(cachePath))
                    {
                        string newImagePath = Path.Combine(folder, newHall.MaSanh + ".jpg");
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
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                {
                    EditMessage = string.Empty;
                    return false;
                }
                var folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
                var imagePath = Path.Combine(folder, SelectedItem.MaSanh + ".jpg");
                var imageExists = File.Exists(imagePath);
                if (SelectedItem.TenSanh == TenSanh
                    && SelectedItem.SoLuongBanToiDa?.ToString() == SoLuongBanToiDa
                    && SelectedItem.GhiChu == GhiChu
                    && SelectedItem.MaLoaiSanh == (SelectedHallType?.MaLoaiSanh ?? 0)
                    && !(imageExists && Image == null)
                    && ImageHelper.IsEditCacheImageSameAsCurrent(SelectedItem.MaSanh, "Hall"))
                {
                    EditMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(TenSanh))
                {
                    EditMessage = "Tên sảnh không được để trống";
                    return false;
                }
                if (!int.TryParse(SoLuongBanToiDa, out int soLuong) || soLuong <= 0)
                {
                    EditMessage = "Số lượng bàn tối đa phải là số nguyên dương";
                    return false;
                }
                var exists = OriginalList.Any(x =>
                    x.TenSanh == TenSanh &&
                    x.MaLoaiSanh == SelectedHallType.MaLoaiSanh &&
                    x.MaSanh != SelectedItem.MaSanh);
                if (exists)
                {
                    EditMessage = "Tên sảnh đã tồn tại trong loại sảnh này";
                    return false;
                }
                EditMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {

                    string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
                    if (Image == null)
                    {
                        string imagePath = Path.Combine(folder, SelectedItem.MaSanh + ".jpg");

                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }
                    }
                 


                    var updateDto = new SANHDTO()
                    {
                        MaSanh = SelectedItem.MaSanh,
                        TenSanh = TenSanh,
                        SoLuongBanToiDa = int.TryParse(SoLuongBanToiDa, out int soLuong) ? soLuong : (int?)null,
                        GhiChu = GhiChu,
                        MaLoaiSanh = SelectedHallType?.MaLoaiSanh,
                        LoaiSanh = SelectedHallType
                    };

                    _sanhService.Update(updateDto);

                    var index = List.IndexOf(SelectedItem);
                    List[index] = null;
                    List[index] = updateDto;


                    // Move Editcache.jpg to real image
                    string cachePath = Path.Combine(folder, "Editcache.jpg");
                    if (File.Exists(cachePath))
                    {
                        string newImagePath = Path.Combine(folder, updateDto.MaSanh + ".jpg");
                        File.Copy(cachePath, newImagePath, true);
                        File.Delete(cachePath);
                    }
                    SelectedAction = null; // Reset action selection
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
                return SelectedItem != null;
            }, (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sảnh này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Xóa ảnh của sảnh
                        string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
                        string imagePath = Path.Combine(folder, SelectedItem.MaSanh + ".jpg");
                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }

                        _sanhService.Delete(SelectedItem.MaSanh);
                        List.Remove(SelectedItem);
                        SelectedAction = null; // Reset action selection
                        Reset();
                        MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            ExportToExcelCommand = new RelayCommand<object>((p) => true, (p) => ExportToExcel());
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
            var worksheet = workbook.Worksheets.Add("Danh sách sảnh");

            // Tiêu đề cột
            worksheet.Cell(1, 1).Value = "Tên sảnh";
            worksheet.Cell(1, 2).Value = "Loại sảnh";
            worksheet.Cell(1, 3).Value = "Đơn giá bàn tối thiểu";
            worksheet.Cell(1, 4).Value = "Số lượng bàn tối đa";
            worksheet.Cell(1, 5).Value = "Ghi chú";

            // Format tiêu đề
            var headerRange = worksheet.Range("A1:E1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Ghi dữ liệu
            int row = 2;
            foreach (var item in List)
            {
                worksheet.Cell(row, 1).Value = item.TenSanh;
                worksheet.Cell(row, 2).Value = item.LoaiSanh?.TenLoaiSanh;
                worksheet.Cell(row, 3).Value = item.LoaiSanh?.DonGiaBanToiThieu ?? 0;
                worksheet.Cell(row, 4).Value = item.SoLuongBanToiDa ?? 0;
                worksheet.Cell(row, 5).Value = item.GhiChu;

                // Format dòng dữ liệu
                for (int col = 1; col <= 5; col++)
                {
                    worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0"; // Format tiền
                row++;
            }

            // Tự động điều chỉnh độ rộng
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



        private void Reset()
        {
            SelectedItem = null;
            TenSanh = string.Empty;
            SoLuongBanToiDa = null;
            GhiChu = string.Empty;
            SelectedHallType = null;
            SearchText = string.Empty;

            // Clean up cache files
            string folder = Path.Combine(ImageHelper.BaseImagePath, "Hall");
            string addCache = Path.Combine(folder, "Addcache.jpg");
            string editCache = Path.Combine(folder, "Editcache.jpg");
            if (File.Exists(addCache)) File.Delete(addCache);
            if (File.Exists(editCache)) File.Delete(editCache);
        }
    }
}