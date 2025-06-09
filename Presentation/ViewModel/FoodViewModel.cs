using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model; // Để kiểm tra FK trực tiếp khi xoá
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    /// <summary>
    /// ViewModel quản lý bảng MONAN – viết theo cùng phong cách HallViewModel.
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
                }
                else
                {
                    ClearMessages();
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
        #endregion

        #region Constructor
        public FoodViewModel()
        {
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

            // So sánh dữ liệu gốc với dữ liệu hiện tại
            if (
                SelectedItem.TenMonAn == TenMonAn?.Trim() &&
                SelectedItem.GhiChu == GhiChu &&
                decimal.TryParse(DonGia?.Replace(".", "").Replace(",", ""), out var newPrice) &&
                SelectedItem.DonGia == newPrice
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
        }

        private Exception GetInnermost(Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            return ex;
        }
        #endregion
    }
}
