using QuanLyTiecCuoi.BusinessLogicLayer.IService;
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
        private readonly IRevenueReportDetailService _revenueReportDetailService;
        private readonly IBookingService _BookingService;
        private MainViewModel _mainVM;

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

        private ObservableCollection<string> _imagePaths = new ObservableCollection<string>
        {
            "/Image/1.jpg",
            "/Image/2.jpg",
            "/Image/4.jpg",
            "/Image/5.jpg",
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

        // Constructor với Dependency Injection
        public HomeViewModel(MainViewModel mainVM, IBookingService BookingService, IRevenueReportDetailService revenueReportDetailService)
        {
            _revenueReportDetailService = revenueReportDetailService;
            _BookingService = BookingService;
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
            _timer.Interval = TimeSpan.FromSeconds(3);
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
            var bookings = _BookingService.GetAll()
                .Where(x => x.BookingDate.HasValue)
                .OrderByDescending(x => x.BookingDate.Value)
                .Take(5)
                .Select(x => new RecentBookingViewModel
                {
                    BrideGroom = $"{x.BrideName} - {x.GroomName}",
                    Hall = x.Hall?.HallName ?? "",
                    TableCount = x.TableCount ?? 0,
                    BookingDate = x.BookingDate ?? DateTime.MinValue
                });

            RecentBookings = new ObservableCollection<RecentBookingViewModel>(bookings);
            OnPropertyChanged(nameof(RecentBookings));
        }

        private void LoadWeddingDays()
        {
            var now = DateTime.Today;
            var weddings = _BookingService.GetAll()
                .Where(x => x.WeddingDate.HasValue && x.WeddingDate.Value >= now)
                .OrderBy(x => x.WeddingDate.Value)
                .Select(x => new SpecialDateInfo
                {
                    Date = x.WeddingDate.Value,
                    Tooltip = $"{x.BrideName} - {x.GroomName}\nSảnh: {x.Hall?.HallName ?? ""}\nBàn: {x.TableCount ?? 0}"
                });

            WeddingDays = new ObservableCollection<SpecialDateInfo>(weddings);
            OnPropertyChanged(nameof(WeddingDays));
        }

        private void LoadStatisticChart()
        {
            var allReports = _revenueReportDetailService.GetAll()
                    .OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .ThenByDescending(x => x.Day)
                    .ToList();

            if (allReports.Count == 0)
            {
                StatisticChartControl = null;
                return;
            }

            // Doanh thu tháng hiện tại
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var currentMonthReports = allReports
                .Where(x => x.Month == currentMonth && x.Year == currentYear)
                .OrderBy(x => x.Day)
                .ToList();

            var chartVM = new HomeChartViewModel(currentMonthReports);

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