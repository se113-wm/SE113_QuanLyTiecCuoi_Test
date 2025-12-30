using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.SystemTests.Helpers
{
    /// <summary>
    /// Helper class for System Testing - provides test data creation and utility methods
    /// </summary>
    public static class SystemTestHelper
    {
        /// <summary>
        /// Creates a test booking DTO with specified parameters
        /// </summary>
        public static BookingDTO CreateTestBooking(
            string groomName = "John Doe",
            string brideName = "Jane Smith",
            string phone = "0123456789",
            DateTime? weddingDate = null,
            int? hallId = 1,
            int? shiftId = 1,
            int tableCount = 20,
            decimal? deposit = null)
        {
            var booking = new BookingDTO
            {
                GroomName = groomName,
                BrideName = brideName,
                Phone = phone,
                BookingDate = DateTime.Now,
                WeddingDate = weddingDate ?? DateTime.Now.AddMonths(2),
                HallId = hallId,
                ShiftId = shiftId,
                TableCount = tableCount,
                ReserveTableCount = tableCount,
                TablePrice = 500000m, // Default price
                Deposit = deposit ?? (tableCount * 500000m * 0.3m), // 30% deposit
                TotalTableAmount = tableCount * 500000m,
                TotalServiceAmount = 0m,
                TotalInvoiceAmount = tableCount * 500000m,
                RemainingAmount = (tableCount * 500000m * 0.7m),
                AdditionalCost = 0m,
                PenaltyAmount = 0m
            };
            
            return booking;
        }

        /// <summary>
        /// Creates a test user DTO with specified role
        /// </summary>
        public static AppUserDTO CreateTestUser(
            string username,
            string password,
            string fullName,
            string groupId = "1")
        {
            return new AppUserDTO
            {
                Username = username,
                PasswordHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(password)),
                FullName = fullName,
                Email = $"{username}@test.com",
                PhoneNumber = "0123456789",
                GroupId = groupId,
                UserGroup = new UserGroupDTO
                {
                    GroupId = groupId,
                    GroupName = groupId == "1" ? "Customer" : groupId == "2" ? "Staff" : "Admin"
                }
            };
        }

        /// <summary>
        /// Creates a test hall DTO
        /// </summary>
        public static HallDTO CreateTestHall(
            string hallName = "Test Hall",
            int maxTableCount = 50,
            int hallTypeId = 1,
            decimal minTablePrice = 500000m)
        {
            return new HallDTO
            {
                HallName = hallName,
                MaxTableCount = maxTableCount,
                HallTypeId = hallTypeId,
                Note = "Test hall for system testing",
                HallType = new HallTypeDTO
                {
                    HallTypeId = hallTypeId,
                    HallTypeName = "Class A",
                    MinTablePrice = minTablePrice
                }
            };
        }

        /// <summary>
        /// Creates a test shift DTO
        /// </summary>
        public static ShiftDTO CreateTestShift(
            string shiftName = "Morning",
            TimeSpan? startTime = null,
            TimeSpan? endTime = null)
        {
            return new ShiftDTO
            {
                ShiftName = shiftName,
                StartTime = startTime ?? new TimeSpan(8, 0, 0),
                EndTime = endTime ?? new TimeSpan(12, 0, 0)
            };
        }

        /// <summary>
        /// Validates booking data completeness
        /// </summary>
        public static bool ValidateBooking(BookingDTO booking)
        {
            if (booking == null) return false;
            if (string.IsNullOrEmpty(booking.GroomName)) return false;
            if (string.IsNullOrEmpty(booking.BrideName)) return false;
            if (string.IsNullOrEmpty(booking.Phone)) return false;
            if (!booking.WeddingDate.HasValue) return false;
            if (!booking.HallId.HasValue) return false;
            if (!booking.ShiftId.HasValue) return false;
            if (!booking.TableCount.HasValue || booking.TableCount <= 0) return false;
            
            return true;
        }

        /// <summary>
        /// Calculates penalty based on days late and penalty rate
        /// </summary>
        public static decimal CalculatePenalty(
            decimal totalAmount,
            decimal deposit,
            int daysLate,
            decimal penaltyRate)
        {
            if (daysLate <= 0) return 0m;
            
            return (totalAmount - deposit) * penaltyRate * daysLate;
        }

        /// <summary>
        /// Checks if a booking date/shift/hall combination is available.
        /// Hall is unavailable if ANY booking exists for that date/hall/shift,
        /// regardless of payment status.
        /// </summary>
        /// <remarks>
        /// This matches the production logic in AddWeddingViewModel.cs
        /// which prevents double-booking by checking for any existing booking.
        /// </remarks>
        public static bool IsHallAvailable(
            IEnumerable<BookingDTO> existingBookings,
            DateTime weddingDate,
            int hallId,
            int shiftId)
        {
            // Hall unavailable if ANY booking exists
            return !existingBookings.Any(b =>
                b.WeddingDate.HasValue &&
                b.WeddingDate.Value.Date == weddingDate.Date &&
                b.HallId == hallId &&
                b.ShiftId == shiftId);
            // Note: No PaymentDate check - ANY booking blocks the slot
        }

        /// <summary>
        /// Validates table count against hall capacity
        /// </summary>
        public static bool ValidateTableCount(int requestedTables, int hallMaxCapacity)
        {
            return requestedTables > 0 && requestedTables <= hallMaxCapacity;
        }

        /// <summary>
        /// Gets status of a booking
        /// </summary>
        public static string GetBookingStatus(BookingDTO booking)
        {
            if (booking == null) return "Unknown";
            
            if (booking.PaymentDate.HasValue)
                return "Paid";
            
            if (booking.WeddingDate.HasValue && booking.WeddingDate.Value < DateTime.Now)
                return "Cancelled";
            
            return "Pending";
        }

        /// <summary>
        /// Creates a sample list of test bookings
        /// </summary>
        public static List<BookingDTO> CreateSampleBookings(int count = 10)
        {
            var bookings = new List<BookingDTO>();
            var random = new Random();
            
            for (int i = 0; i < count; i++)
            {
                var weddingDate = DateTime.Now.AddMonths(random.Next(1, 6));
                var booking = CreateTestBooking(
                    groomName: $"Groom {i + 1}",
                    brideName: $"Bride {i + 1}",
                    weddingDate: weddingDate,
                    hallId: random.Next(1, 4),
                    shiftId: random.Next(1, 3),
                    tableCount: random.Next(10, 40)
                );
                booking.BookingId = i + 1;
                bookings.Add(booking);
            }
            
            return bookings;
        }

        /// <summary>
        /// Validates phone number format
        /// </summary>
        public static bool ValidatePhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            
            // Vietnamese phone number: 10 digits starting with 0
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{9}$");
        }

        /// <summary>
        /// Calculates deposit amount (30% of total)
        /// </summary>
        public static decimal CalculateDeposit(decimal totalAmount)
        {
            return totalAmount * 0.3m;
        }

        /// <summary>
        /// Calculates remaining amount after deposit
        /// </summary>
        public static decimal CalculateRemainingAmount(
            decimal totalAmount,
            decimal deposit,
            decimal? penalty = null,
            decimal? additionalCost = null)
        {
            decimal remaining = totalAmount - deposit;
            
            if (penalty.HasValue)
                remaining += penalty.Value;
            
            if (additionalCost.HasValue)
                remaining += additionalCost.Value;
            
            return remaining;
        }
    }
}
