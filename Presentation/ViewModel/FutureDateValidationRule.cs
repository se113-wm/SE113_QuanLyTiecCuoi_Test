using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class FutureDateValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is DateTime date && date.Date < DateTime.Today)
                return new ValidationResult(false, "Chỉ chọn ngày hiện tại hoặc tương lai.");
            return ValidationResult.ValidResult;
        }
    }
}
