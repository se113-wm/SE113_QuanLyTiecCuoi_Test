using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Helpers
{
    public static class DatePickerHelper
    {
        public static readonly DependencyProperty BlackoutDatesSourceProperty =
            DependencyProperty.RegisterAttached("BlackoutDatesSource", typeof(IEnumerable<CalendarDateRange>),
                typeof(DatePickerHelper), new PropertyMetadata(null, OnBlackoutDatesSourceChanged));

        public static IEnumerable<CalendarDateRange> GetBlackoutDatesSource(DependencyObject obj)
            => (IEnumerable<CalendarDateRange>)obj.GetValue(BlackoutDatesSourceProperty);

        public static void SetBlackoutDatesSource(DependencyObject obj, IEnumerable<CalendarDateRange> value)
            => obj.SetValue(BlackoutDatesSourceProperty, value);

        private static void OnBlackoutDatesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker)
            {
                datePicker.Loaded += (s, _) =>
                {
                    if (e.NewValue is IEnumerable<CalendarDateRange> ranges)
                    {
                        datePicker.BlackoutDates.Clear();
                        foreach (var range in ranges)
                        {
                            datePicker.BlackoutDates.Add(range);
                        }
                    }
                };
            }
        }
    }

}
