using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace QuanLyTiecCuoi.Tests.UnitTests.Validators
{
    [TestClass]
    public class InputValidatorTests
    {
        #region Phone Number Validation Tests

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra số điện thoại hợp lệ với 10 số")]
        public void ValidatePhoneNumber_ValidPhone10Digits_ReturnsTrue()
        {
            // Arrange
            string phoneNumber = "0901234567";

            // Act
            bool result = IsValidPhoneNumber(phoneNumber);

            // Assert
            Assert.IsTrue(result, "Số điện thoại 10 số phải hợp lệ");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra số điện thoại không hợp lệ - có chữ cái")]
        public void ValidatePhoneNumber_ContainsLetters_ReturnsFalse()
        {
            // Arrange
            string phoneNumber = "090ABC4567";

            // Act
            bool result = IsValidPhoneNumber(phoneNumber);

            // Assert
            Assert.IsFalse(result, "Số điện thoại có chữ cái phải không hợp lệ");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra số điện thoại rỗng")]
        public void ValidatePhoneNumber_EmptyString_ReturnsFalse()
        {
            // Arrange
            string phoneNumber = "";

            // Act
            bool result = IsValidPhoneNumber(phoneNumber);

            // Assert
            Assert.IsFalse(result, "Số điện thoại rỗng phải không hợp lệ");
        }

        #endregion

        #region Email Validation Tests

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra email hợp lệ")]
        public void ValidateEmail_ValidEmail_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";

            // Act
            bool result = IsValidEmail(email);

            // Assert
            Assert.IsTrue(result, "Email hợp lệ phải trả về true");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra email không có @")]
        public void ValidateEmail_MissingAtSymbol_ReturnsFalse()
        {
            // Arrange
            string email = "testexample.com";

            // Act
            bool result = IsValidEmail(email);

            // Assert
            Assert.IsFalse(result, "Email thiếu @ phải không hợp lệ");
        }

        #endregion

        #region Table Count Validation Tests

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra số bàn hợp lệ")]
        public void ValidateTableCount_ValidCount_ReturnsTrue()
        {
            // Arrange
            int tableCount = 20;
            int minTables = 10;
            int maxTables = 100;

            // Act
            bool result = IsValidTableCount(tableCount, minTables, maxTables);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra số bàn nhỏ hơn minimum")]
        public void ValidateTableCount_LessThanMin_ReturnsFalse()
        {
            // Arrange
            int tableCount = 5;
            int minTables = 10;
            int maxTables = 100;

            // Act
            bool result = IsValidTableCount(tableCount, minTables, maxTables);

            // Assert
            Assert.IsFalse(result, "Số bàn nhỏ hơn minimum phải không hợp lệ");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra số bàn lớn hơn maximum")]
        public void ValidateTableCount_GreaterThanMax_ReturnsFalse()
        {
            // Arrange
            int tableCount = 150;
            int minTables = 10;
            int maxTables = 100;

            // Act
            bool result = IsValidTableCount(tableCount, minTables, maxTables);

            // Assert
            Assert.IsFalse(result, "Số bàn lớn hơn maximum phải không hợp lệ");
        }

        #endregion

        #region Booking Date Validation Tests

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra ngày đặt tiệc trong tương lai")]
        public void ValidateBookingDate_FutureDate_ReturnsTrue()
        {
            // Arrange
            DateTime bookingDate = DateTime.Now.AddDays(7);

            // Act
            bool result = IsValidBookingDate(bookingDate);

            // Assert
            Assert.IsTrue(result, "Ngày trong tương lai phải hợp lệ");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra ngày đặt tiệc trong quá khứ")]
        public void ValidateBookingDate_PastDate_ReturnsFalse()
        {
            // Arrange
            DateTime bookingDate = DateTime.Now.AddDays(-1);

            // Act
            bool result = IsValidBookingDate(bookingDate);

            // Assert
            Assert.IsFalse(result, "Ngày trong quá khứ phải không hợp lệ");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [Description("Kiểm tra ngày đặt tiệc hôm nay")]
        public void ValidateBookingDate_Today_ReturnsFalse()
        {
            // Arrange
            DateTime bookingDate = DateTime.Today;

            // Act
            bool result = IsValidBookingDate(bookingDate);

            // Assert
            Assert.IsFalse(result, "Ngày hôm nay phải không hợp lệ (cần đặt trước)");
        }

        #endregion

        #region Private Validation Methods (Simulating actual validators)

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Kiểm tra chỉ chứa số và có độ dài 10-11
            if (phoneNumber.Length < 10 || phoneNumber.Length > 11)
                return false;

            foreach (char c in phoneNumber)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidTableCount(int tableCount, int min, int max)
        {
            return tableCount >= min && tableCount <= max;
        }

        private bool IsValidBookingDate(DateTime bookingDate)
        {
            return bookingDate.Date > DateTime.Today;
        }

        #endregion
    }
}