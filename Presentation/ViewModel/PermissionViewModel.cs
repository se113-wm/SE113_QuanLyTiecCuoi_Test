using QuanLyTiecCuoi.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    public class PermissionViewModel : BaseViewModel
    {
        private ObservableCollection<NHOMNGUOIDUNG> _List;
        public ObservableCollection<NHOMNGUOIDUNG> List
        {
            get => _List;
            set { _List = value; OnPropertyChanged(); }
        }

        private ObservableCollection<NHOMNGUOIDUNG> _OriginalList;
        public ObservableCollection<NHOMNGUOIDUNG> OriginalList
        {
            get => _OriginalList;
            set { _OriginalList = value; OnPropertyChanged(); }
        }

        // Trong PermissionViewModel:
        public Dictionary<string, ChucNangState> ChucNangStates { get; set; }

        private void InitChucNangStates()
        {
            ChucNangStates = new Dictionary<string, ChucNangState>
            {
                ["Home"] = new ChucNangState { MaChucNang = "Home", TenManHinhDuocLoad = "HomeView" },
                ["HallType"] = new ChucNangState { MaChucNang = "HallType", TenManHinhDuocLoad = "HallTypeView" },
                ["Hall"] = new ChucNangState { MaChucNang = "Hall", TenManHinhDuocLoad = "HallView" },
                ["Shift"] = new ChucNangState { MaChucNang = "Shift", TenManHinhDuocLoad = "ShiftView" },
                ["Food"] = new ChucNangState { MaChucNang = "Food", TenManHinhDuocLoad = "FoodView" },
                ["Service"] = new ChucNangState { MaChucNang = "Service", TenManHinhDuocLoad = "ServiceView" },
                ["Wedding"] = new ChucNangState { MaChucNang = "Wedding", TenManHinhDuocLoad = "WeddingView" },
                ["Report"] = new ChucNangState { MaChucNang = "Report", TenManHinhDuocLoad = "ReportView" },
                ["Parameter"] = new ChucNangState { MaChucNang = "Parameter", TenManHinhDuocLoad = "ParameterView" },
                ["Permission"] = new ChucNangState { MaChucNang = "Permission", TenManHinhDuocLoad = "PermissionView" },
                ["User"] = new ChucNangState { MaChucNang = "User", TenManHinhDuocLoad = "UserView" }
            };
            foreach (var state in ChucNangStates.Values)
                state.UpdatePermission += ChucNangState_UpdatePermission;
        }

        private void ChucNangState_UpdatePermission(object sender, EventArgs e)
        {
            if (SelectedItem == null) return;
            var state = sender as ChucNangState;
            var db = DataProvider.Ins.DB;
            var chucNang = db.CHUCNANGs.FirstOrDefault(cn => cn.MaChucNang == state.MaChucNang);
            if (chucNang == null) return;
            if (state.IsChecked)
            {
                if (!SelectedItem.CHUCNANGs.Any(cn => cn.MaChucNang == state.MaChucNang))
                    SelectedItem.CHUCNANGs.Add(chucNang);
            }
            else
            {
                var remove = SelectedItem.CHUCNANGs.FirstOrDefault(cn => cn.MaChucNang == state.MaChucNang);
                if (remove != null)
                    SelectedItem.CHUCNANGs.Remove(remove);
            }
            db.SaveChanges();
        }

        // Khi đổi SelectedItem (nhóm người dùng), cập nhật trạng thái check:
        private void UpdateChucNangStates()
        {
            if (SelectedItem == null)
            {
                foreach (var state in ChucNangStates.Values)
                    state.IsChecked = false;
                return;
            }
            var maChucNangSet = new HashSet<string>(SelectedItem.CHUCNANGs.Select(cn => cn.MaChucNang));
            foreach (var state in ChucNangStates.Values)
                state.IsChecked = maChucNangSet.Contains(state.MaChucNang);
        }

        private NHOMNGUOIDUNG _SelectedItem;
        public NHOMNGUOIDUNG SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    TenNhom = SelectedItem.TenNhom;
                }
                else
                {
                    AddMessage = string.Empty;
                    EditMessage = string.Empty;
                    DeleteMessage = string.Empty;
                }
            }
        }

        private string _TenNhom;
        public string TenNhom
        {
            get => _TenNhom;
            set { _TenNhom = value; OnPropertyChanged(); }
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

        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => {
            Reset();
        });

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
                TenNhom = string.Empty;

                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    List = OriginalList;
                    return;
                }

                switch (SelectedSearchProperty)
                {
                    case "Tên nhóm":
                        List = new ObservableCollection<NHOMNGUOIDUNG>(
                            OriginalList.Where(x => x.TenNhom != null && x.TenNhom.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    default:
                        List = new ObservableCollection<NHOMNGUOIDUNG>(OriginalList);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public PermissionViewModel()
        {
            // Load dữ liệu từ database, không hiển thị nhóm 'ADMIN' và nhóm của người dùng hiện tại
            List = new ObservableCollection<NHOMNGUOIDUNG>(
                DataProvider.Ins.DB.NHOMNGUOIDUNGs
                    .Where(x => x.TenNhom != "Quản trị viên" && x.MaNhom != DataProvider.Ins.CurrentUser.MaNhom)
                    .ToList()
            );
            // Lưu danh sách gốc để tìm kiếm
            OriginalList = new ObservableCollection<NHOMNGUOIDUNG>(List);


            SearchProperties = new ObservableCollection<string> { "Tên nhóm" };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();
            // Khởi tạo trạng thái chức năng
            InitChucNangStates();
            // Đăng ký sự kiện khi thay đổi SelectedItem
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedItem))
                {
                    UpdateChucNangStates();
                }
            };

            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrWhiteSpace(TenNhom))
                {
                    if (SelectedItem != null)
                    {
                        AddMessage = "Vui lòng nhập tên nhóm";
                    }
                    else
                    {
                        AddMessage = string.Empty;
                    }
                    return false;
                }
                // Nếu tên nhóm chọn là 'Quản trị viên' thì không cho phép sửa
                if (TenNhom == "Quản trị viên" || TenNhom.ToLower().Contains("quản trị viên") || TenNhom.ToLower().Contains("admin"))
                {
                    AddMessage = "Không thể thêm 'Quản trị viên'";
                    return false;
                }
                var exists = DataProvider.Ins.DB.NHOMNGUOIDUNGs.Any(x => x.TenNhom == TenNhom);
                if (exists)
                {
                    AddMessage = "Tên nhóm đã tồn tại";
                    return false;
                }
                AddMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    // Tạo mã nhóm mới (rút gọn từ GUID, 8 ký tự)
                    string maNhom = "GR" + Guid.NewGuid().ToString("N").Substring(0, 8);

                    // Kiểm tra trùng mã nhóm trong DB (hiếm nhưng nên có)
                    while (DataProvider.Ins.DB.NHOMNGUOIDUNGs.Any(x => x.MaNhom == maNhom))
                    {
                        maNhom = "GR" + Guid.NewGuid().ToString("N").Substring(0, 8);
                    }

                    var newPermission = new NHOMNGUOIDUNG()
                    {
                        TenNhom = TenNhom.Trim(),
                        MaNhom = maNhom
                    };

                    DataProvider.Ins.DB.NHOMNGUOIDUNGs.Add(newPermission);
                    DataProvider.Ins.DB.SaveChanges();

                    List.Add(newPermission);

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
                if (SelectedItem.TenNhom == TenNhom)
                {
                    EditMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                // Kiểm tra tính hợp lệ của dữ liệu
                if (string.IsNullOrWhiteSpace(TenNhom))
                {
                    EditMessage = "Tên nhóm không được để trống";
                    return false;
                }
                if (TenNhom == "Quản trị viên" || TenNhom.ToLower().Contains("quản trị viên") || TenNhom.ToLower().Contains("admin"))
                {
                    EditMessage = "Không thể sửa thành 'Quản trị viên'";
                    return false;
                }
                var exists = DataProvider.Ins.DB.NHOMNGUOIDUNGs.Any(x => x.TenNhom == TenNhom);
                if (exists)
                {
                    EditMessage = "Tên nhóm đã tồn tại";
                    return false;
                }
                EditMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var permission = DataProvider.Ins.DB.NHOMNGUOIDUNGs.Where(x => x.MaNhom == SelectedItem.MaNhom).SingleOrDefault();
                    if (permission != null)
                    {
                        permission.TenNhom = TenNhom.Trim();
                        DataProvider.Ins.DB.SaveChanges();

                        SelectedItem.TenNhom = TenNhom.Trim();

                        var index = List.IndexOf(SelectedItem);
                        List[index] = null;
                        List[index] = permission;

                        Reset();
                        MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
                // Kiểm tra tham chiếu nếu có (ví dụ: kiểm tra bảng người dùng có nhóm quyền này không)
                var hasReferences = DataProvider.Ins.DB.NGUOIDUNGs.Any(u => u.MaNhom == SelectedItem.MaNhom);
                if (hasReferences)
                {
                    DeleteMessage = "Đối tượng đang được tham chiếu.";
                    return false;
                }
                else
                {
                    DeleteMessage = string.Empty;
                    return true;
                }
            }, (p) =>
            {
                try
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhóm phân quyền này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        DataProvider.Ins.DB.NHOMNGUOIDUNGs.Remove(SelectedItem);
                        DataProvider.Ins.DB.SaveChanges();

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
        }
        private void Reset()
        {
            SelectedItem = null;
            TenNhom = string.Empty;
            SearchText = string.Empty;
        }
    }
}
