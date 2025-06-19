using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace QuanLyTiecCuoi.Presentation.Helpers {
    public class DecimalValidationRule : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, "Không được để trống.");

            string input = value.ToString();

            // Xoá dấu phân cách hàng nghìn (,) hoặc (.) tùy theo Culture
            var thousandsSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
            var cleanedInput = input.Replace(thousandsSeparator, "");

            if (decimal.TryParse(cleanedInput, NumberStyles.Number, cultureInfo, out _))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Giá trị không hợp lệ.");
        }
    }
}
