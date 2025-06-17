using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QuanLyTiecCuoi.Presentation.Helpers {
    public class CurrencyConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) return "";
            if (value is decimal dec)
                return $"{dec:N0}";
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string input) {
                var numberOnly = Regex.Replace(input, @"[^\d]", "");
                if (decimal.TryParse(numberOnly, out var result))
                    return result;
            }
            return 0m;
        }
    }

}
