using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    public class ShiftViewModel : BaseViewModel
    {
        #region Service & Collections
        private readonly IShiftService _shiftService;
        private readonly IBookingService _bookingService;

        private ObservableCollection<ShiftDTO> _shiftList;
        public ObservableCollection<ShiftDTO> ShiftList
        {
            get => _shiftList;
            set { _shiftList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ShiftDTO> _originalList;
        public ObservableCollection<ShiftDTO> OriginalList
        {
            get => _originalList;
            set { _originalList = value; OnPropertyChanged(); }
        }
        #endregion

        #region Selected Item
        private ShiftDTO _selectedItem;
        public ShiftDTO SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();

                if (SelectedItem != null)
                {
                    ShiftName = SelectedItem.ShiftName;
                    StartTime = SelectedItem.StartTime;
                    EndTime = SelectedItem.EndTime;
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
        private string _shiftName;
        public string ShiftName
        {
            get => _shiftName;
            set { _shiftName = value; OnPropertyChanged(); }
        }

        private TimeSpan? _startTime;
        public TimeSpan? StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StartTimeDateTime));
            }
        }

        public DateTime? StartTimeDateTime
        {
            get => _startTime.HasValue ? DateTime.Today.Add(_startTime.Value) : (DateTime?)null;
            set
            {
                if (value.HasValue)
                    StartTime = value.Value.TimeOfDay;
                else
                    StartTime = null;
                OnPropertyChanged();
            }
        }

        private TimeSpan? _endTime;
        public TimeSpan? EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EndTimeDateTime));
            }
        }

        public DateTime? EndTimeDateTime
        {
            get => _endTime.HasValue ? DateTime.Today.Add(_endTime.Value) : (DateTime?)null;
            set
            {
                if (value.HasValue)
                    EndTime = value.Value.TimeOfDay;
                else
                    EndTime = null;
                OnPropertyChanged();
            }
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

        public ObservableCollection<string> ActionList { get; } = new ObservableCollection<string> { "Thêm", "Sửa", "Xóa", "Chọn thao tác" };

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
                        Reset();
                        break;
                    case "Sửa":
                        IsAdding = false;
                        IsEditing = true;
                        IsDeleting = false;
                        Reset();
                        break;
                    case "Xóa":
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
        public ICommand ResetCommand { get; set; }
        #endregion

        #region Constructor
        public ShiftViewModel(IShiftService shiftService, IBookingService bookingService)
        {
            _shiftService = shiftService;
            _bookingService = bookingService;

            ShiftList = new ObservableCollection<ShiftDTO>(_shiftService.GetAll().ToList());
            OriginalList = new ObservableCollection<ShiftDTO>(ShiftList);

            SearchProperties = new ObservableCollection<string> { "Tên ca", "Thời gian bắt đầu", "Thời gian kết thúc" };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            AddCommand = new RelayCommand<object>(
                (p) => CanAdd(),
                (p) => AddShift()
            );

            EditCommand = new RelayCommand<object>(
                (p) => CanEdit(),
                (p) => EditShift()
            );

            DeleteCommand = new RelayCommand<object>(
                (p) => CanDelete(),
                (p) => DeleteShift()
            );

            ResetCommand = new RelayCommand<object>(
                (p) => true,
                (p) => Reset()
            );
        }
        #endregion

        #region Validation Helpers
        private bool ValidateTimeRange(TimeSpan? time, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!time.HasValue)
                return false;

            if (time.Value < new TimeSpan(7, 30, 0) || time.Value >= new TimeSpan(24, 0, 0))
            {
                errorMessage = "Thời gian phải sau 7h30 và trước 12h đêm.";
                return false;
            }
            return true;
        }
        #endregion

        #region Add
        private bool CanAdd()
        {
            AddMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(ShiftName))
            {
                AddMessage = "Vui lòng nhập tên ca";
                return false;
            }

            if (!StartTime.HasValue)
            {
                AddMessage = "Vui lòng nhập thời gian bắt đầu ca";
                return false;
            }

            if (!EndTime.HasValue)
            {
                AddMessage = "Vui lòng nhập thời gian kết thúc ca";
                return false;
            }

            if (!ValidateTimeRange(StartTime, out string startError))
            {
                AddMessage = "Thời gian bắt đầu ca phải sau 7h30 và trước 12h đêm.";
                return false;
            }

            if (!ValidateTimeRange(EndTime, out string endError))
            {
                AddMessage = "Thời gian kết thúc ca phải sau 7h30 và trước 12h đêm.";
                return false;
            }

            if (EndTime <= StartTime)
            {
                AddMessage = "Thời gian kết thúc ca phải sau thời gian bắt đầu ca.";
                return false;
            }

            var exists = OriginalList.Any(x => x.ShiftName == ShiftName);
            if (exists)
            {
                AddMessage = "Tên ca đã tồn tại";
                return false;
            }

            return true;
        }

        private void AddShift()
        {
            try
            {
                var newShift = new ShiftDTO()
                {
                    ShiftName = ShiftName.Trim(),
                    StartTime = StartTime,
                    EndTime = EndTime
                };

                _shiftService.Create(newShift);
                OriginalList.Add(newShift);
                ShiftList = new ObservableCollection<ShiftDTO>(OriginalList);

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

            if (string.IsNullOrWhiteSpace(ShiftName))
            {
                EditMessage = "Vui lòng nhập tên ca";
                return false;
            }

            if (!StartTime.HasValue)
            {
                EditMessage = "Vui lòng nhập thời gian bắt đầu ca";
                return false;
            }

            if (!EndTime.HasValue)
            {
                EditMessage = "Vui lòng nhập thời gian kết thúc ca";
                return false;
            }

            if (!ValidateTimeRange(StartTime, out string startError))
            {
                EditMessage = "Thời gian bắt đầu ca phải sau 7h30 và trước 12h đêm.";
                return false;
            }

            if (!ValidateTimeRange(EndTime, out string endError))
            {
                EditMessage = "Thời gian kết thúc ca phải sau 7h30 và trước 12h đêm.";
                return false;
            }

            if (EndTime <= StartTime)
            {
                EditMessage = "Thời gian kết thúc ca phải sau thời gian bắt đầu ca.";
                return false;
            }

            var exists = OriginalList.Any(x => x.ShiftName == ShiftName && x.ShiftId != SelectedItem.ShiftId);
            if (exists)
            {
                EditMessage = "Tên ca đã tồn tại";
                return false;
            }

            if (SelectedItem.ShiftName == ShiftName &&
                SelectedItem.StartTime == StartTime &&
                SelectedItem.EndTime == EndTime)
            {
                EditMessage = "Không có thay đổi nào để cập nhật";
                return false;
            }

            return true;
        }

        private void EditShift()
        {
            try
            {
                var updateDto = new ShiftDTO()
                {
                    ShiftId = SelectedItem.ShiftId,
                    ShiftName = ShiftName.Trim(),
                    StartTime = StartTime,
                    EndTime = EndTime
                };

                _shiftService.Update(updateDto);

                var index = ShiftList.IndexOf(SelectedItem);
                ShiftList[index] = null;
                ShiftList[index] = updateDto;
                OriginalList[index] = updateDto;

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
                return false;
            }

            if (_bookingService.GetAll().Any(x => x.ShiftId == SelectedItem.ShiftId))
            {
                DeleteMessage = "Không thể xóa ca này vì nó đang được sử dụng trong phiếu đặt tiệc";
                return false;
            }

            DeleteMessage = string.Empty;
            return true;
        }

        private void DeleteShift()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa ca này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                _shiftService.Delete(SelectedItem.ShiftId);
                ShiftList.Remove(SelectedItem);
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

        #region Helpers
        private void PerformSearch()
        {
            try
            {
                SelectedItem = null;
                ShiftName = string.Empty;
                StartTime = null;
                EndTime = null;

                if (string.IsNullOrWhiteSpace(SearchText) || string.IsNullOrWhiteSpace(SelectedSearchProperty))
                {
                    ShiftList = OriginalList;
                    return;
                }

                string search = SearchText.Trim();

                switch (SelectedSearchProperty)
                {
                    case "Tên ca":
                        ShiftList = new ObservableCollection<ShiftDTO>(
                            OriginalList.Where(x => !string.IsNullOrWhiteSpace(x.ShiftName) &&
                                                    x.ShiftName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
                        break;
                    case "Thời gian bắt đầu":
                        if (TimeSpan.TryParse(search, out TimeSpan startTime))
                        {
                            ShiftList = new ObservableCollection<ShiftDTO>(
                                OriginalList.Where(x => x.StartTime.HasValue && x.StartTime.Value == startTime));
                        }
                        else
                        {
                            ShiftList = new ObservableCollection<ShiftDTO>();
                        }
                        break;
                    case "Thời gian kết thúc":
                        if (TimeSpan.TryParse(search, out TimeSpan endTime))
                        {
                            ShiftList = new ObservableCollection<ShiftDTO>(
                                OriginalList.Where(x => x.EndTime.HasValue && x.EndTime.Value == endTime));
                        }
                        else
                        {
                            ShiftList = new ObservableCollection<ShiftDTO>();
                        }
                        break;
                    default:
                        ShiftList = OriginalList;
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
            ShiftName = string.Empty;
            StartTime = null;
            EndTime = null;
            SearchText = string.Empty;
        }
        #endregion
    }
}