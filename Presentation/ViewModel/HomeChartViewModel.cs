using LiveCharts;
using LiveCharts.Wpf;
using QuanLyTiecCuoi.DataTransferObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class HomeChartViewModel : INotifyPropertyChanged
    {
        private SeriesCollection _seriesCollection;
        private List<string> _labels;
        private Func<double, string> _formatter;

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set { _seriesCollection = value; OnPropertyChanged(); }
        }

        public List<string> Labels
        {
            get => _labels;
            set { _labels = value; OnPropertyChanged(); }
        }

        public Func<double, string> Formatter
        {
            get => _formatter;
            set { _formatter = value; OnPropertyChanged(); }
        }

        public HomeChartViewModel()
        {
            // Empty constructor for XAML designer
        }

        public HomeChartViewModel(IEnumerable<RevenueReportDetailDTO> reportList, bool isAreaChart = false)
        {
            LoadChart(reportList, isAreaChart);
        }

        private void LoadChart(IEnumerable<RevenueReportDetailDTO> reportList, bool isAreaChart)
        {
            var values = new ChartValues<decimal>();
            var labels = new List<string>();

            foreach (var item in reportList)
            {
                values.Add(item.Revenue ?? 0);
                labels.Add(item.Day.ToString());
            }

            if (isAreaChart)
            {
                SeriesCollection = new SeriesCollection
                {
                    new StackedAreaSeries
                    {
                        Title = "Doanh thu",
                        Values = values,
                        Fill = new SolidColorBrush(Color.FromArgb(120, 72, 61, 139)), // semi-transparent DarkSlateBlue
                        Stroke = new SolidColorBrush(Colors.DarkSlateBlue),
                        LineSmoothness = 0.5
                    }
                };
            }
            else
            {
                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Doanh thu",
                        Values = values,
                        Stroke = new SolidColorBrush(Colors.Green),
                        Fill = Brushes.Transparent,
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8,
                        LineSmoothness = 0.5
                    }
                };
            }

            Labels = labels;
            Formatter = value => value.ToString("C", CultureInfo.GetCultureInfo("vi-VN"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}