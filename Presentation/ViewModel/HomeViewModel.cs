using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class SpecialDateInfo
    {
        public DateTime Date { get; set; }
        public string Tooltip { get; set; }
    }
    public class HomeViewModel : INotifyPropertyChanged
    {
        // Recent bookings for DataGrid
        public ObservableCollection<RecentBookingViewModel> RecentBookings { get; set; }

        // Highlighted wedding days for Calendar
        public ObservableCollection<SpecialDateInfo> WeddingDays { get; set; }

        // Selected date in Calendar
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }

        // Statistic chart control (can be a UserControl or ViewModel)
        private object _statisticChartControl;
        public object StatisticChartControl
        {
            get => _statisticChartControl;
            set
            {
                if (_statisticChartControl != value)
                {
                    _statisticChartControl = value;
                    OnPropertyChanged(nameof(StatisticChartControl));
                }
            }
        }

        private readonly PhieuDatTiecService _phieuDatTiecService;

        public HomeViewModel()
        {
            _phieuDatTiecService = new PhieuDatTiecService();

            // Load data
            LoadRecentBookings();
            LoadWeddingDays();
            // thử dữ liệu tĩnh cho ngày cưới
            //WeddingDays = new ObservableCollection<SpecialDateInfo>
            //{
            //    new SpecialDateInfo { Date = DateTime.Today.AddDays(1), Tooltip = "Cưới ngày mai" },
            //    new SpecialDateInfo { Date = DateTime.Today.AddDays(2), Tooltip = "Cưới ngày kia" },
            //    new SpecialDateInfo { Date = DateTime.Today.AddDays(3), Tooltip = "Cưới sau 3 ngày" }
            //};
            LoadStatisticChart();
        }

        private void LoadRecentBookings()
        {
            // Lấy 5 phiếu đặt tiệc gần nhất (theo ngày đặt tiệc giảm dần)
            var bookings = _phieuDatTiecService.GetAll()
                .Where(x => x.NgayDatTiec.HasValue)
                .OrderByDescending(x => x.NgayDatTiec.Value)
                .Take(5)
                .Select(x => new RecentBookingViewModel
                {
                    BrideGroom = $"{x.TenCoDau} - {x.TenChuRe}",
                    Hall = x.Sanh?.TenSanh ?? "",
                    TableCount = x.SoLuongBan ?? 0,
                    BookingDate = x.NgayDatTiec ?? DateTime.MinValue
                });

            RecentBookings = new ObservableCollection<RecentBookingViewModel>(bookings);
            OnPropertyChanged(nameof(RecentBookings));
        }

        private void LoadWeddingDays()
        {
            // Lấy các ngày đãi tiệc sắp tới (trong tương lai)
            var now = DateTime.Today;
            var weddings = _phieuDatTiecService.GetAll()
                .Where(x => x.NgayDaiTiec.HasValue && x.NgayDaiTiec.Value >= now)
                .OrderBy(x => x.NgayDaiTiec.Value)
                .Select(x => new SpecialDateInfo
                {
                    Date = x.NgayDaiTiec.Value,
                    Tooltip = $"{x.TenCoDau} - {x.TenChuRe}\nSảnh: {x.Sanh?.TenSanh ?? ""}\nBàn: {x.SoLuongBan ?? 0}"
                });

            WeddingDays = new ObservableCollection<SpecialDateInfo>(weddings);
            OnPropertyChanged(nameof(WeddingDays));
        }

        private void LoadStatisticChart()
        {
            // TODO: Replace with your actual chart control or ViewModel
            // For demo, just set to null or a placeholder string
            StatisticChartControl = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // ViewModel for DataGrid row
    public class RecentBookingViewModel
    {
        public string BrideGroom { get; set; }
        public string Hall { get; set; }
        public int TableCount { get; set; }
        public DateTime BookingDate { get; set; }
    }
}