using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
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
        private readonly IUserGroupService _userGroupService;
        private readonly IPermissionService _permissionService;
        private readonly IAppUserService _appUserService;

        private ObservableCollection<UserGroupDTO> _groupList;
        public ObservableCollection<UserGroupDTO> GroupList
        {
            get => _groupList;
            set { _groupList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<UserGroupDTO> _originalList;
        public ObservableCollection<UserGroupDTO> OriginalList
        {
            get => _originalList;
            set { _originalList = value; OnPropertyChanged(); }
        }

        public Dictionary<string, PermissionState> PermissionStates { get; set; }

        private void InitPermissionStates()
        {
            PermissionStates = new Dictionary<string, PermissionState>
            {
                ["Home"] = new PermissionState { PermissionId = "Home", LoadedScreenName = "HomeView" },
                ["HallType"] = new PermissionState { PermissionId = "HallType", LoadedScreenName = "HallTypeView" },
                ["Hall"] = new PermissionState { PermissionId = "Hall", LoadedScreenName = "HallView" },
                ["Shift"] = new PermissionState { PermissionId = "Shift", LoadedScreenName = "ShiftView" },
                ["Food"] = new PermissionState { PermissionId = "Food", LoadedScreenName = "FoodView" },
                ["Service"] = new PermissionState { PermissionId = "Service", LoadedScreenName = "ServiceView" },
                ["Wedding"] = new PermissionState { PermissionId = "Wedding", LoadedScreenName = "WeddingView" },
                ["Report"] = new PermissionState { PermissionId = "Report", LoadedScreenName = "ReportView" },
                ["Parameter"] = new PermissionState { PermissionId = "Parameter", LoadedScreenName = "ParameterView" },
                ["Permission"] = new PermissionState { PermissionId = "Permission", LoadedScreenName = "PermissionView" },
                ["User"] = new PermissionState { PermissionId = "User", LoadedScreenName = "UserView" }
            };
            foreach (var state in PermissionStates.Values)
                state.UpdatePermission += PermissionState_UpdatePermission;
        }

        private void PermissionState_UpdatePermission(object sender, EventArgs e)
        {
            if (SelectedItem == null) return;
            var state = sender as PermissionState;
            var db = DataProvider.Ins.DB;
            var permission = db.Permissions.FirstOrDefault(p => p.PermissionId == state.PermissionId);
            if (permission == null) return;

            var userGroup = db.UserGroups.FirstOrDefault(g => g.GroupId == SelectedItem.GroupId);
            if (userGroup == null) return;

            if (state.IsChecked)
            {
                if (!userGroup.Permissions.Any(p => p.PermissionId == state.PermissionId))
                    userGroup.Permissions.Add(permission);
            }
            else
            {
                var remove = userGroup.Permissions.FirstOrDefault(p => p.PermissionId == state.PermissionId);
                if (remove != null)
                    userGroup.Permissions.Remove(remove);
            }
            db.SaveChanges();
        }

        private void UpdatePermissionStates()
        {
            if (SelectedItem == null)
            {
                foreach (var state in PermissionStates.Values)
                    state.IsChecked = false;
                return;
            }

            var db = DataProvider.Ins.DB;
            var userGroup = db.UserGroups.FirstOrDefault(g => g.GroupId == SelectedItem.GroupId);
            if (userGroup == null) return;

            var permissionIdSet = new HashSet<string>(userGroup.Permissions.Select(p => p.PermissionId));
            foreach (var state in PermissionStates.Values)
                state.IsChecked = permissionIdSet.Contains(state.PermissionId);
        }

        private UserGroupDTO _selectedItem;
        public UserGroupDTO SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    GroupName = SelectedItem.GroupName;
                    IsSelected = true;
                    UpdatePermissionStates();
                }
                else
                {
                    AddMessage = string.Empty;
                    EditMessage = string.Empty;
                    DeleteMessage = string.Empty;
                    IsSelected = false;
                }
            }
        }

        private string _groupName;
        public string GroupName
        {
            get => _groupName;
            set { _groupName = value; OnPropertyChanged(); }
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

        public ICommand ResetCommand => new RelayCommand<object>((p) => true, (p) => { Reset(); });

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

        private void PerformSearch()
        {
            try
            {
                SelectedItem = null;
                GroupName = string.Empty;

                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchProperty))
                {
                    GroupList = OriginalList;
                    return;
                }

                switch (SelectedSearchProperty)
                {
                    case "Group Name":
                        GroupList = new ObservableCollection<UserGroupDTO>(
                            OriginalList.Where(x => x.GroupName != null && x.GroupName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    default:
                        GroupList = new ObservableCollection<UserGroupDTO>(OriginalList);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
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

        public ObservableCollection<string> ActionList { get; } = new ObservableCollection<string> { "Add", "Edit", "Delete", "Select Action" };

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
                    case "Add":
                        IsAdding = true;
                        IsEditing = false;
                        IsDeleting = false;
                        Reset();
                        break;
                    case "Edit":
                        IsAdding = false;
                        IsEditing = true;
                        IsDeleting = false;
                        Reset();
                        break;
                    case "Delete":
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = true;
                        Reset();
                        break;
                    default:
                        _selectedAction = null;
                        IsAdding = false;
                        IsEditing = false;
                        IsDeleting = false;
                        Reset();
                        break;
                }
            }
        }

        public PermissionViewModel(IUserGroupService userGroupService, IPermissionService permissionService, IAppUserService appUserService)
        {
            _userGroupService = userGroupService;
            _permissionService = permissionService;
            _appUserService = appUserService;

            var currentGroupId = DataProvider.Ins.CurrentUser.GroupId;

            GroupList = new ObservableCollection<UserGroupDTO>(
                _userGroupService.GetAll()
                    .Where(x => x.GroupName != "Administrator" && x.GroupId != currentGroupId)
                    .ToList()
            );
            OriginalList = new ObservableCollection<UserGroupDTO>(GroupList);

            SearchProperties = new ObservableCollection<string> { "Group Name" };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            InitPermissionStates();
            InitializeCommands();

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedItem))
                {
                    UpdatePermissionStates();
                }
            };
        }

        private void InitializeCommands()
        {
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrWhiteSpace(GroupName))
                {
                    AddMessage = "Please enter group name";
                    return false;
                }
                if (GroupName == "Administrator" || GroupName.ToLower().Contains("administrator") || GroupName.ToLower().Contains("admin"))
                {
                    AddMessage = "Cannot add 'Administrator'";
                    return false;
                }
                var exists = GroupList.Any(x => x.GroupName == GroupName);
                if (exists)
                {
                    AddMessage = "Group name already exists";
                    return false;
                }
                AddMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    string groupId = "GR" + Guid.NewGuid().ToString("N").Substring(0, 8);

                    while (GroupList.Any(x => x.GroupId == groupId))
                    {
                        groupId = "GR" + Guid.NewGuid().ToString("N").Substring(0, 8);
                    }

                    var newGroup = new UserGroupDTO()
                    {
                        GroupName = GroupName.Trim(),
                        GroupId = groupId
                    };

                    _userGroupService.Create(newGroup);
                    GroupList.Add(newGroup);

                    Reset();
                    MessageBox.Show("Added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Add error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                {
                    return false;
                }
                if (SelectedItem.GroupName == GroupName)
                {
                    EditMessage = "No changes to update";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(GroupName))
                {
                    EditMessage = "Group name cannot be empty";
                    return false;
                }
                if (GroupName == "Administrator" || GroupName.ToLower().Contains("administrator") || GroupName.ToLower().Contains("admin"))
                {
                    EditMessage = "Cannot change to 'Administrator'";
                    return false;
                }
                var exists = GroupList.Any(x => x.GroupName == GroupName && x.GroupId != SelectedItem.GroupId);
                if (exists)
                {
                    EditMessage = "Group name already exists";
                    return false;
                }
                EditMessage = string.Empty;
                return true;
            }, (p) =>
            {
                try
                {
                    var updateDto = new UserGroupDTO
                    {
                        GroupId = SelectedItem.GroupId,
                        GroupName = GroupName.Trim()
                    };

                    _userGroupService.Update(updateDto);

                    var index = GroupList.IndexOf(SelectedItem);
                    GroupList[index] = null;
                    GroupList[index] = updateDto;

                    Reset();
                    MessageBox.Show("Updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Update error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var hasReferences = _appUserService.GetAll().Any(u => u.GroupId == SelectedItem.GroupId);
                if (hasReferences)
                {
                    DeleteMessage = "Object is being referenced.";
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
                    var result = MessageBox.Show("Are you sure you want to delete this permission group?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _userGroupService.Delete(SelectedItem.GroupId);
                        GroupList.Remove(SelectedItem);
                        Reset();

                        MessageBox.Show("Deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Delete cancelled", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Delete error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void Reset()
        {
            SelectedItem = null;
            GroupName = string.Empty;
            SearchText = string.Empty;
        }
    }

    public class PermissionState : BaseViewModel
    {
        public string PermissionId { get; set; }
        public string LoadedScreenName { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
                UpdatePermission?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler UpdatePermission;
    }
}
