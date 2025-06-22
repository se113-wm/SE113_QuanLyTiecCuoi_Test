using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class SpecialDateInfo
    {
        public DateTime Date { get; set; }
        public string Tooltip { get; set; }
    }

    public class HomeViewModel : BaseViewModel
    {
        private readonly CtBaoCaoDsService _baoCaoService = new CtBaoCaoDsService();
        public ICommand DatTiecNgayCommand { get; set; }
        public ObservableCollection<RecentBookingViewModel> RecentBookings { get; set; }
        public ObservableCollection<SpecialDateInfo> WeddingDays { get; set; }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _statisticChartControl;
        public object StatisticChartControl
        {
            get => _statisticChartControl;
            set
            {
                if (_statisticChartControl != value)
                {
                    _statisticChartControl = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly PhieuDatTiecService _phieuDatTiecService;
        private MainViewModel _mainVM;

        private ObservableCollection<string> _imagePaths = new ObservableCollection<string>
        {
            "/Image/wedding.jpg",
            "/Image/anhcuoi.png",
             "/Image/1.jpg",
              "/Image/2.jpg",
               "/Image/3.jpg",
            // Thêm các đường dẫn ảnh khác nếu có
        };

        private int _currentImageIndex;
        private string _currentImage;
        private DispatcherTimer _timer;

        public string CurrentImage
        {
            get => _currentImage;
            set { _currentImage = value; OnPropertyChanged(); }
        }

        public ICommand NextImageCommand { get; }
        public ICommand PreviousImageCommand { get; }

        public HomeViewModel(MainViewModel mainVM)
        {
            _phieuDatTiecService = new PhieuDatTiecService();
            _mainVM = mainVM;

            LoadRecentBookings();
            LoadWeddingDays();
            LoadStatisticChart();

            DatTiecNgayCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                _mainVM.SwitchToWeddingTab();
            });

            _currentImageIndex = 0;
            CurrentImage = _imagePaths[_currentImageIndex];

            NextImageCommand = new RelayCommand<object>((p) => true, (p) => NextImage());
            PreviousImageCommand = new RelayCommand<object>((p) => true, (p) => PreviousImage());

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // 3 giây dễ quan sát hơn
            _timer.Tick += (s, e) => NextImage();
            _timer.Start();
        }

        private void NextImage()
        {
            _currentImageIndex = (_currentImageIndex + 1) % _imagePaths.Count;
            CurrentImage = _imagePaths[_currentImageIndex];
        }

        private void PreviousImage()
        {
            _currentImageIndex = (_currentImageIndex - 1 + _imagePaths.Count) % _imagePaths.Count;
            CurrentImage = _imagePaths[_currentImageIndex];
        }

        private void LoadRecentBookings()
        {
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
            var allReports = _baoCaoService.GetAll()
                    .OrderByDescending(x => x.Nam)
                    .ThenByDescending(x => x.Thang)
                    .ThenByDescending(x => x.Ngay)
                    .ToList();

            if (allReports.Count == 0)
            {
                StatisticChartControl = null;
                return;
            }

            var latestMonth = allReports.First().Thang;
            var latestYear = allReports.First().Nam;

            var latestMonthReports = allReports
                .Where(x => x.Thang == latestMonth && x.Nam == latestYear)
                .OrderBy(x => x.Ngay)
                .ToList();

            var chartVM = new ChartViewModel(latestMonthReports);

            var homeChart = new HomeChart
            {
                DataContext = chartVM,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch
            };
            StatisticChartControl = homeChart;
        }
    }

    public class RecentBookingViewModel
    {
        public string BrideGroom { get; set; }
        public string Hall { get; set; }
        public int TableCount { get; set; }
        public DateTime BookingDate { get; set; }
    }
}