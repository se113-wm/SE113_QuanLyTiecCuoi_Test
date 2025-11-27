using LiveCharts;
using LiveCharts.Wpf;
using QuanLyTiecCuoi.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class ChartViewModel : INotifyPropertyChanged
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

        public ChartViewModel()
        {
            // Empty constructor for XAML designer
        }

        public ChartViewModel(IEnumerable<RevenueReportDetailDTO> reportList)
        {
            LoadChart(reportList);
        }

        private void LoadChart(IEnumerable<RevenueReportDetailDTO> reportList)
        {
            var values = new ChartValues<decimal>();
            var labels = new List<string>();

            foreach (var item in reportList)
            {
                values.Add(item.Revenue ?? 0);
                labels.Add(item.Day.ToString());
            }

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Doanh thu ",
                    Values = values,
                    Fill = new SolidColorBrush(Colors.DarkSlateBlue)
                }
            };

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
