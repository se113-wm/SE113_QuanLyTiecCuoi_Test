using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace QuanLyTiecCuoi.Presentation.View
{
    public partial class WeddingView : UserControl
    {
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public WeddingView()
        {
            InitializeComponent();
        }

        private void WeddingListView_ColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked?.Column != null)
            {
                // Map header to property name
                string sortBy = null;
                switch (headerClicked.Column.Header.ToString())
                {
                    case "Số lượng bàn":
                        sortBy = "SoLuongBan";
                        break;
                    case "Tên chú rể":
                        sortBy = "TenChuRe";
                        break;
                    case "Tên cô dâu":
                        sortBy = "TenCoDau";
                        break;
                    case "Tên sảnh":
                        sortBy = "Sanh.TenSanh";
                        break;
                    case "Ngày đãi tiệc":
                        sortBy = "NgayDaiTiec";
                        break;
                    case "Giờ đãi tiệc":
                        sortBy = "NgayDaiTiec";
                        break;
                    case "Trạng thái":
                        sortBy = "TrangThai";
                        break;
                        // Add more as needed
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    ListSortDirection direction = ListSortDirection.Ascending;
                    if (_lastHeaderClicked == headerClicked && _lastDirection == ListSortDirection.Ascending)
                        direction = ListSortDirection.Descending;

                    var collectionView = CollectionViewSource.GetDefaultView(WeddingListView.ItemsSource);
                    collectionView.SortDescriptions.Clear();
                    collectionView.SortDescriptions.Add(new SortDescription(sortBy, direction));
                    collectionView.Refresh();

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }
    }
}