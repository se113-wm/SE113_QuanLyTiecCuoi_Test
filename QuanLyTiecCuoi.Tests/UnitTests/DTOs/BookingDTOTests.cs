using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.Tests.UnitTests.DTOs
{
    [TestClass]
    public class BookingDTOTests
    {
        #region Status Property Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Status tr? v? 'Paid' khi ?ã thanh toán")]
        public void Status_WhenPaymentDateIsSet_ReturnsPaid()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = DateTime.Now,
                WeddingDate = DateTime.Now.AddDays(-1)
            };

            // Act
            var status = booking.Status;

            // Assert
            Assert.AreEqual("Paid", status);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Status tr? v? 'Not Organized' khi ngày c??i trong t??ng lai")]
        public void Status_WhenWeddingDateInFuture_ReturnsNotOrganized()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = null,
                WeddingDate = DateTime.Now.AddDays(7)
            };

            // Act
            var status = booking.Status;

            // Assert
            Assert.AreEqual("Not Organized", status);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Status tr? v? 'Not Paid' khi ngày c??i là hôm nay")]
        public void Status_WhenWeddingDateIsToday_ReturnsNotPaid()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = null,
                WeddingDate = DateTime.Today
            };

            // Act
            var status = booking.Status;

            // Assert
            Assert.AreEqual("Not Paid", status);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Status tr? v? 'Late Payment' khi ngày c??i ?ã qua")]
        public void Status_WhenWeddingDateInPast_ReturnsLatePayment()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = null,
                WeddingDate = DateTime.Now.AddDays(-7)
            };

            // Act
            var status = booking.Status;

            // Assert
            Assert.AreEqual("Late Payment", status);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Status tr? v? empty khi WeddingDate null")]
        public void Status_WhenWeddingDateIsNull_ReturnsEmpty()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = null,
                WeddingDate = null
            };

            // Act
            var status = booking.Status;

            // Assert
            Assert.AreEqual("", status);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("PaymentDate có ?u tiên cao h?n WeddingDate")]
        public void Status_PaymentDateTakesPrecedence_ReturnsPaid()
        {
            // Arrange - Even if wedding is in future, if paid -> Paid
            var booking = new BookingDTO
            {
                PaymentDate = DateTime.Now,
                WeddingDate = DateTime.Now.AddDays(30)
            };

            // Act
            var status = booking.Status;

            // Assert
            Assert.AreEqual("Paid", status);
        }

        #endregion

        #region StatusBrush Property Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("StatusBrush tr? v? Green khi ?ã thanh toán")]
        public void StatusBrush_WhenPaid_ReturnsGreen()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = DateTime.Now,
                WeddingDate = DateTime.Now.AddDays(-1)
            };

            // Act
            var brush = booking.StatusBrush;

            // Assert
            Assert.AreEqual(System.Windows.Media.Brushes.Green, brush);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("StatusBrush tr? v? Blue khi ch?a t? ch?c")]
        public void StatusBrush_WhenNotOrganized_ReturnsBlue()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = null,
                WeddingDate = DateTime.Now.AddDays(7)
            };

            // Act
            var brush = booking.StatusBrush;

            // Assert
            Assert.AreEqual(System.Windows.Media.Brushes.Blue, brush);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("StatusBrush tr? v? Orange khi ch?a thanh toán (hôm nay)")]
        public void StatusBrush_WhenNotPaid_ReturnsOrange()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = null,
                WeddingDate = DateTime.Today
            };

            // Act
            var brush = booking.StatusBrush;

            // Assert
            Assert.AreEqual(System.Windows.Media.Brushes.Orange, brush);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("StatusBrush tr? v? Red khi thanh toán tr?")]
        public void StatusBrush_WhenLatePayment_ReturnsRed()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = null,
                WeddingDate = DateTime.Now.AddDays(-7)
            };

            // Act
            var brush = booking.StatusBrush;

            // Assert
            Assert.AreEqual(System.Windows.Media.Brushes.Red, brush);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("StatusBrush tr? v? Black khi WeddingDate null")]
        public void StatusBrush_WhenWeddingDateNull_ReturnsBlack()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PaymentDate = null,
                WeddingDate = null
            };

            // Act
            var brush = booking.StatusBrush;

            // Assert
            Assert.AreEqual(System.Windows.Media.Brushes.Black, brush);
        }

        #endregion

        #region Property Value Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Ki?m tra t?t c? properties ???c set ?úng")]
        public void BookingDTO_AllProperties_SetCorrectly()
        {
            // Arrange & Act
            var booking = new BookingDTO
            {
                BookingId = 1,
                GroomName = "Nguy?n V?n A",
                BrideName = "Tr?n Th? B",
                Phone = "0901234567",
                BookingDate = new DateTime(2024, 1, 1),
                WeddingDate = new DateTime(2024, 2, 14),
                ShiftId = 1,
                HallId = 2,
                Deposit = 5000000,
                TableCount = 20,
                ReserveTableCount = 5,
                PaymentDate = new DateTime(2024, 2, 14),
                TablePrice = 1500000,
                TotalTableAmount = 30000000,
                TotalServiceAmount = 5000000,
                TotalInvoiceAmount = 35000000,
                RemainingAmount = 30000000,
                AdditionalCost = 500000,
                PenaltyAmount = 0
            };

            // Assert
            Assert.AreEqual(1, booking.BookingId);
            Assert.AreEqual("Nguy?n V?n A", booking.GroomName);
            Assert.AreEqual("Tr?n Th? B", booking.BrideName);
            Assert.AreEqual("0901234567", booking.Phone);
            Assert.AreEqual(new DateTime(2024, 1, 1), booking.BookingDate);
            Assert.AreEqual(new DateTime(2024, 2, 14), booking.WeddingDate);
            Assert.AreEqual(1, booking.ShiftId);
            Assert.AreEqual(2, booking.HallId);
            Assert.AreEqual(5000000, booking.Deposit);
            Assert.AreEqual(20, booking.TableCount);
            Assert.AreEqual(5, booking.ReserveTableCount);
            Assert.AreEqual(new DateTime(2024, 2, 14), booking.PaymentDate);
            Assert.AreEqual(1500000, booking.TablePrice);
            Assert.AreEqual(30000000, booking.TotalTableAmount);
            Assert.AreEqual(5000000, booking.TotalServiceAmount);
            Assert.AreEqual(35000000, booking.TotalInvoiceAmount);
            Assert.AreEqual(30000000, booking.RemainingAmount);
            Assert.AreEqual(500000, booking.AdditionalCost);
            Assert.AreEqual(0, booking.PenaltyAmount);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Navigation properties có th? ???c set")]
        public void BookingDTO_NavigationProperties_CanBeSet()
        {
            // Arrange & Act
            var booking = new BookingDTO
            {
                Shift = new ShiftDTO { ShiftId = 1, ShiftName = "Tr?a" },
                Hall = new HallDTO 
                { 
                    HallId = 1, 
                    HallName = "S?nh A",
                    HallType = new HallTypeDTO { HallTypeId = 1, HallTypeName = "VIP" }
                }
            };

            // Assert
            Assert.IsNotNull(booking.Shift);
            Assert.AreEqual("Tr?a", booking.Shift.ShiftName);
            Assert.IsNotNull(booking.Hall);
            Assert.AreEqual("S?nh A", booking.Hall.HallName);
            Assert.IsNotNull(booking.Hall.HallType);
            Assert.AreEqual("VIP", booking.Hall.HallType.HallTypeName);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Nullable properties có th? null")]
        public void BookingDTO_NullableProperties_CanBeNull()
        {
            // Arrange & Act
            var booking = new BookingDTO();

            // Assert
            Assert.IsNull(booking.BookingDate);
            Assert.IsNull(booking.WeddingDate);
            Assert.IsNull(booking.ShiftId);
            Assert.IsNull(booking.HallId);
            Assert.IsNull(booking.Deposit);
            Assert.IsNull(booking.TableCount);
            Assert.IsNull(booking.ReserveTableCount);
            Assert.IsNull(booking.PaymentDate);
            Assert.IsNull(booking.TablePrice);
            Assert.IsNull(booking.TotalTableAmount);
            Assert.IsNull(booking.TotalServiceAmount);
            Assert.IsNull(booking.TotalInvoiceAmount);
            Assert.IsNull(booking.RemainingAmount);
            Assert.IsNull(booking.AdditionalCost);
            Assert.IsNull(booking.PenaltyAmount);
            Assert.IsNull(booking.Shift);
            Assert.IsNull(booking.Hall);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Status v?i ngày c??i ? ranh gi?i n?a ?êm")]
        public void Status_WeddingDateAtMidnight_HandlesCorrectly()
        {
            // Arrange - Wedding date at exactly midnight tomorrow
            var tomorrow = DateTime.Today.AddDays(1);
            var booking = new BookingDTO
            {
                PaymentDate = null,
                WeddingDate = tomorrow
            };

            // Act
            var status = booking.Status;

            // Assert
            Assert.AreEqual("Not Organized", status);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Giá tr? decimal l?n")]
        public void BookingDTO_LargeDecimalValues_HandlesCorrectly()
        {
            // Arrange
            var booking = new BookingDTO
            {
                Deposit = 999999999.99m,
                TotalInvoiceAmount = 9999999999.99m,
                TablePrice = 99999999.99m
            };

            // Assert
            Assert.AreEqual(999999999.99m, booking.Deposit);
            Assert.AreEqual(9999999999.99m, booking.TotalInvoiceAmount);
            Assert.AreEqual(99999999.99m, booking.TablePrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingDTO")]
        [Description("Giá tr? decimal âm (cho penalty)")]
        public void BookingDTO_NegativeValues_HandlesCorrectly()
        {
            // Arrange
            var booking = new BookingDTO
            {
                PenaltyAmount = -1000000m, // Edge case - negative penalty
                RemainingAmount = -500000m  // Overpaid
            };

            // Assert
            Assert.AreEqual(-1000000m, booking.PenaltyAmount);
            Assert.AreEqual(-500000m, booking.RemainingAmount);
        }

        #endregion
    }
}
