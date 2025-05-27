using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QuanLyTiecCuoi.ViewModel
{
    public class CurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (decimal.TryParse(value?.ToString(), out var result))
            {
                return string.Format(culture, "{0:N0}", result);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Chỉ loại bỏ dấu phẩy, giữ lại dấu cách để kiểm tra hợp lệ ở nơi khác
            var str = value?.ToString()?.Replace(",", "");
            return str;
        }
    }
}