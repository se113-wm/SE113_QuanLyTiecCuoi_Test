using ClosedXML.Excel;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    public class UserViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly IAppUserService _appUserService;
        private readonly IUserGroupService _userGroupService;

        private ObservableCollection<AppUserDTO> _userList;
        public ObservableCollection<AppUserDTO> UserList
        {
            get => _userList;
            set { _userList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<AppUserDTO> _originalList;
        public ObservableCollection<AppUserDTO> OriginalList
        {
            get => _originalList;
            set { _originalList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<UserGroupDTO> _userTypes;
        public ObservableCollection<UserGroupDTO> UserTypes
        {
            get => _userTypes;
            set { _userTypes = value; OnPropertyChanged(); }
        }

        private AppUserDTO _selectedItem;
        public AppUserDTO SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    Username = SelectedItem.Username;
                    NewPassword = string.Empty;
                    FullName = SelectedItem.FullName;
                    Email = SelectedItem.Email;
                    SelectedUserType = UserTypes?.FirstOrDefault(ht => ht.GroupId == SelectedItem.GroupId);
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

        private UserGroupDTO _selectedUserType;
        public UserGroupDTO SelectedUserType
        {
            get => _selectedUserType;
            set
            {
                _selectedUserType = value;
                OnPropertyChanged();
                GroupName = _selectedUserType?.GroupName;
            }
        }

        public ICommand AddCommand { get; set; }

        private string _addMessage;
        public string AddMessage
        {
            get => _addMessage;
            set { _addMessage = value; OnPropertyChanged(); }
        }

        public ICommand EditCommand { get; set; }

        private string _editMessage;
        public string EditMessage
        {
            get => _editMessage;
            set { _editMessage = value; OnPropertyChanged(); }
        }

        public ICommand DeleteCommand { get; set; }

        private string _deleteMessage;
        public string DeleteMessage
        {
            get => _deleteMessage;
            set { _deleteMessage = value; OnPropertyChanged(); }
        }

        public ICommand ExportToExcelCommand { get; set; }

        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => { Reset(); });

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Email))
                {
                    if (string.IsNullOrWhiteSpace(Email))
                        return "Email không được để trống";
                    if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        return "Email không đúng định dạng";
                }
                return null;
            }
        }

        public string Error => null;

        private string _groupName;
        public string GroupName
        {
            get => _groupName;
            set { _groupName = value; OnPropertyChanged(); }
        }

        private bool _isPasswordChangeEnabled;
        public bool IsPasswordChangeEnabled
        {
            get => _isPasswordChangeEnabled;
            set { _isPasswordChangeEnabled = value; OnPropertyChanged(); NewPassword = string.Empty; }
        }

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

        public UserViewModel(IAppUserService appUserService, IUserGroupService userGroupService)
        {
            _appUserService = appUserService;
            _userGroupService = userGroupService;

            var currentGroupId = DataProvider.Ins.CurrentUser.GroupId;
            IsPasswordChangeEnabled = false;

            UserList = new ObservableCollection<AppUserDTO>(_appUserService.GetAll().Where(u => u.GroupId != currentGroupId && u.GroupId != "ADMIN").ToList());
            OriginalList = new ObservableCollection<AppUserDTO>(UserList);
            UserTypes = new ObservableCollection<UserGroupDTO>(
                _userGroupService.GetAll()
                .Where(g => g.GroupId != currentGroupId && g.GroupId != "ADMIN")
                .ToList());

            SearchProperties = new ObservableCollection<string>
            {
                "Tên đăng nhập",
                "Họ tên",
                "Nhóm người dùng",
                "Email"
            };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    AddMessage = "Vui lòng nhập tên đăng nhập";
                    return false;
                }
                if (!IsPasswordChangeEnabled || string.IsNullOrWhiteSpace(NewPassword))
                {
                    AddMessage = "Vui lòng nhập mật khẩu";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(FullName))
                {
                    AddMessage = "Vui lòng nhập họ tên";
                    return false;
                }
                if (SelectedUserType == null)
                {
                    AddMessage = "Vui lòng chọn loại nhóm người dùng";
                    return false;
                }
                if (!EmailValidationHelper.IsValidEmail(Email))
                {
                    AddMessage = "Vui lòng nhập email đúng định dạng";
                    return false;
                }
                var exists = OriginalList.Any(x => x.Username == Username);
                if (exists)
                {
                    AddMessage = "Người dùng này đã tồn tại";
                    return false;
                }
                AddMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var newUser = new AppUserDTO()
                    {
                        Username = Username,
                        PasswordHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(NewPassword)),
                        FullName = FullName,
                        Email = Email,
                        GroupId = SelectedUserType?.GroupId,
                        UserGroup = SelectedUserType
                    };
                    _appUserService.Create(newUser);
                    UserList.Add(newUser);

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
                if (Username == SelectedItem.Username && FullName == SelectedItem.FullName && Email == SelectedItem.Email
                && SelectedUserType.GroupId == SelectedItem.GroupId && NewPassword == string.Empty)
                {
                    EditMessage = "Không có thay đổi nào để cập nhật";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(Username))
                {
                    EditMessage = "Tên đăng nhập không được để trống";
                    return false;
                }
                var exists = UserList.Any(x => x.Username == Username && x.UserId != SelectedItem.UserId);
                if (exists)
                {
                    EditMessage = "Tên đăng nhập này đã tồn tại";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(FullName))
                {
                    EditMessage = "Họ tên không được để trống";
                    return false;
                }
                if (!EmailValidationHelper.IsValidEmail(Email))
                {
                    EditMessage = "Vui lòng nhập email đúng định dạng";
                    return false;
                }
                if (IsPasswordChangeEnabled && string.IsNullOrWhiteSpace(NewPassword))
                {
                    EditMessage = "Mật khẩu không được để trống";
                    return false;
                }

                EditMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var updateDto = new AppUserDTO()
                    {
                        UserId = SelectedItem.UserId,
                        Username = Username,
                        FullName = FullName,
                        Email = Email,
                        GroupId = SelectedUserType?.GroupId,
                        UserGroup = SelectedUserType
                    };

                    if (!string.IsNullOrWhiteSpace(NewPassword))
                    {
                        updateDto.PasswordHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(NewPassword));
                    }
                    else
                    {
                        updateDto.PasswordHash = _appUserService.GetById(SelectedItem.UserId).PasswordHash;
                    }

                    _appUserService.Update(updateDto);

                    var index = UserList.IndexOf(SelectedItem);
                    UserList[index] = null;
                    UserList[index] = updateDto;

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
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa người dùng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _appUserService.Delete(SelectedItem.UserId);
                        UserList.Remove(SelectedItem);

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

        private void PerformSearch()
        {
            try
            {
                SelectedItem = null;
                Username = string.Empty;
                FullName = string.Empty;
                Email = string.Empty;
                SelectedUserType = null;

                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    UserList = OriginalList;
                    return;
                }

                switch (SelectedSearchProperty)
                {
                    case "Tên đăng nhập":
                        UserList = new ObservableCollection<AppUserDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.Username) &&
                                x.Username.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Họ tên":
                        UserList = new ObservableCollection<AppUserDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.FullName) &&
                                x.FullName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Nhóm người dùng":
                        UserList = new ObservableCollection<AppUserDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.GroupId) &&
                                x.GroupId.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Email":
                        UserList = new ObservableCollection<AppUserDTO>(
                            OriginalList.Where(x => !string.IsNullOrEmpty(x.Email) &&
                                x.Email.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    default:
                        UserList = new ObservableCollection<AppUserDTO>(OriginalList);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToExcel()
        {
            if (UserList == null || UserList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách Người dùng");

            worksheet.Cell(1, 1).Value = "Tên đăng nhập";
            worksheet.Cell(1, 2).Value = "Họ tên";
            worksheet.Cell(1, 3).Value = "Nhóm người dùng";
            worksheet.Cell(1, 4).Value = "Email";

            var headerRange = worksheet.Range("A1:D1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            int row = 2;
            foreach (var item in UserList)
            {
                worksheet.Cell(row, 1).Value = item.Username;
                worksheet.Cell(row, 2).Value = item.FullName;
                worksheet.Cell(row, 3).Value = item.UserGroup?.GroupName ?? "Không xác định";
                worksheet.Cell(row, 4).Value = item.Email;

                for (int col = 1; col <= 4; col++)
                {
                    worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"DanhSachNguoiDung_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
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
            Username = string.Empty;
            FullName = string.Empty;
            NewPassword = string.Empty;
            Email = string.Empty;
            SelectedUserType = null;
            SearchText = string.Empty;
        }
    }
}
