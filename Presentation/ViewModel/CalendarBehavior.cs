using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Controls;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public static class CalendarBehavior
    {
        public static readonly DependencyProperty HighlightedDatesProperty =
            DependencyProperty.RegisterAttached(
                "HighlightedDates",
                typeof(IEnumerable<SpecialDateInfo>),
                typeof(CalendarBehavior),
                new PropertyMetadata(null, OnHighlightedDatesChanged));

        public static IEnumerable<SpecialDateInfo> GetHighlightedDates(DependencyObject obj) =>
            (IEnumerable<SpecialDateInfo>)obj.GetValue(HighlightedDatesProperty);

        public static void SetHighlightedDates(DependencyObject obj, IEnumerable<SpecialDateInfo> value) =>
            obj.SetValue(HighlightedDatesProperty, value);

        private static void OnHighlightedDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Calendar calendar)
            {
                calendar.Loaded -= Calendar_Refresh;
                calendar.Loaded += Calendar_Refresh;
                calendar.DisplayDateChanged -= Calendar_Refresh;
                calendar.DisplayDateChanged += Calendar_Refresh;
                calendar.DisplayModeChanged -= Calendar_Refresh;
                calendar.DisplayModeChanged += Calendar_Refresh;
            }
        }

        private static void Calendar_Refresh(object sender, EventArgs e)
        {
            if (sender is Calendar calendar)
            {
                calendar.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var dayButtons = FindVisualChildren<CalendarDayButton>(calendar);
                    var highlighted = GetHighlightedDates(calendar)?.ToList() ?? new List<SpecialDateInfo>();

                    foreach (var btn in dayButtons)
                    {
                        if (btn.DataContext is DateTime date)
                        {
                            var info = highlighted.FirstOrDefault(x => x.Date.Date == date.Date);
                            if (info != null)
                            {
                                btn.Background = new SolidColorBrush(Color.FromRgb(178, 243, 155)); // #B2F39B
                                btn.ToolTip = info.Tooltip;
                                // DEBUG: thử đổi màu Foreground để dễ nhận biết
                                btn.Foreground = Brushes.Red;
                            }
                            else
                            {
                                btn.ClearValue(Control.BackgroundProperty);
                                btn.ClearValue(Control.ToolTipProperty);
                                btn.ClearValue(Control.ForegroundProperty);
                            }
                        }
                        else
                        {
                            btn.ClearValue(Control.BackgroundProperty);
                            btn.ClearValue(Control.ToolTipProperty);
                            btn.ClearValue(Control.ForegroundProperty);
                        }
                    }
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t) yield return t;
                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
    }
}
