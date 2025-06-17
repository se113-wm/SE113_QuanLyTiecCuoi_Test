using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows.Data;

namespace QuanLyTiecCuoi.Presentation.Helpers {
    public class MultiplyConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Length < 2) return "0";

            bool parseA = decimal.TryParse(values[0]?.ToString(), out decimal a);
            bool parseC = decimal.TryParse(values[1]?.ToString(), out decimal c);

            if (parseA && parseC) {
                decimal mult = a * c;
                return $"{mult:N0}";
            }
            return "0";  // Trả về 0 nếu nhập sai
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            return new object[] { Binding.DoNothing, Binding.DoNothing }; ;  //Không cần ConvertBack
        }
    }
}
