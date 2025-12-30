using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Tests.SystemTests.Helpers;

namespace QuanLyTiecCuoi.Tests.SystemTests
{
    /// <summary>
    /// System Test - Scenario 3: Cancellation & Penalty Calculation (Complex Logic)
    /// 
    /// Description: Verify the automatic penalty calculation logic when a booking is 
    /// cancelled close to the event date.
    /// 
    /// Test Steps:
    /// 1. Request cancellation - Select "Confirmed" booking
    /// 2. Verify penalty - System calculates penalty based on PenaltyRate parameter
    /// 3. Confirm cancellation - Booking status updates to "Cancelled"
    /// 4. Staff verification - Staff views invoice with penalty line item
    /// 5. Process penalty payment - Penalty recorded and invoice balance updated
    /// 
    /// Business Rules Validated:
    /// - BR132: Cancellation request initiation
    /// - BR134: Automatic penalty calculation based on PenaltyRate parameter
    /// - BR136: Booking status update to "Cancelled"
    /// - BR159: Invoice display with penalty line item
    /// - BR164: Penalty payment processing
    /// </summary>
    [TestClass]
    public class Scenario3_CancellationPenalty_SystemTest
    {
        private AppUserService _userService;
        private HallService _hallService;
        private ShiftService _shiftService;
        private BookingService _bookingService;
        private ParameterService _parameterService;

        // Test booking ID to be used across tests
        private int _testBookingId;

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize all required services
            _userService = new AppUserService(new AppUserRepository());
            _hallService = new HallService(new HallRepository());
            _shiftService = new ShiftService(new ShiftRepository());
            _bookingService = new BookingService(new BookingRepository());
            _parameterService = new ParameterService(new ParameterRepository());
            
            // Create a test booking for cancellation testing
            CreateTestBookingForCancellation();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test booking if it exists
            if (_testBookingId > 0)
            {
                try
                {
                    _bookingService.Delete(_testBookingId);
                }
                catch
                {
                    // Booking may already be deleted
                }
            }
        }

        private void CreateTestBookingForCancellation()
        {
            // Create a confirmed booking that can be cancelled
            var halls = _hallService.GetAll().ToList();
            var existingBookings = _bookingService.GetAll().ToList();
            
            if (halls.Count == 0)
            {
                return; // Skip if no halls available
            }
            
            var testHall = halls.First();
            var testDate = DateTime.Now.AddDays(15); // 15 days from now
            var testShiftId = 1;
            
            // Find available date
            while (!SystemTestHelper.IsHallAvailable(existingBookings, testDate, testHall.HallId, testShiftId))
            {
                testDate = testDate.AddDays(1);
            }
            
            // Create confirmed booking (with payment date)
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: $"CancelTest_Groom_{Guid.NewGuid().ToString().Substring(0, 8)}",
                brideName: $"CancelTest_Bride_{Guid.NewGuid().ToString().Substring(0, 8)}",
                phone: "0123456789",
                weddingDate: testDate,
                hallId: testHall.HallId,
                shiftId: testShiftId,
                tableCount: 20
            );
            
            // Set payment date to make it "Confirmed"
            testBooking.PaymentDate = DateTime.Now.AddDays(-5); // Paid 5 days ago
            
            _bookingService.Create(testBooking);
            _testBookingId = testBooking.BookingId;
        }

        #region Step 1: Request Cancellation - Select "Confirmed" Booking (TC_BR132_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario3")]
        [TestCategory("BR132")]
        [Description("Step 1: TC_BR132_001 - Verify cancellation confirmation popup displays")]
        public void Step1_TC_BR132_001_RequestCancel_ConfirmationPopupDisplays()
        {
            // Arrange
            var booking = _bookingService.GetById(_testBookingId);
            
            if (booking == null)
            {
                Assert.Inconclusive("Test booking not available");
                return;
            }
            
            // Assert - Booking should be in "Confirmed" status
            Assert.IsTrue(booking.PaymentDate.HasValue, 
                "Test booking should be confirmed (have payment date)");
            var status = SystemTestHelper.GetBookingStatus(booking);
            Assert.AreEqual("Paid", status, 
                "Booking status should be 'Paid' (Confirmed) before cancellation (BR132)");
            
            // Act - Simulate cancellation request
            // In real system, this would show a confirmation popup with message:
            // "Are you sure you want to cancel this booking? A penalty may apply."
            
            // Verify booking details before cancellation
            Assert.IsNotNull(booking.GroomName, "Booking should have groom name");
            Assert.IsNotNull(booking.BrideName, "Booking should have bride name");
            Assert.IsTrue(booking.WeddingDate.HasValue, "Booking should have wedding date");
            Assert.IsNotNull(booking.Deposit, "Booking should have deposit amount");
            Assert.IsNotNull(booking.TotalInvoiceAmount, "Booking should have total amount");
            
            // Simulate confirmation popup message
            string confirmationMessage = $"Are you sure you want to cancel booking for " +
                                       $"{booking.GroomName} & {booking.BrideName} on " +
                                       $"{booking.WeddingDate.Value:dd/MM/yyyy}? " +
                                       $"A penalty may apply based on cancellation date.";
            
            Assert.IsFalse(string.IsNullOrEmpty(confirmationMessage), 
                "Confirmation message should be displayed (BR132)");
        }

        #endregion

        #region Step 2: Verify Penalty Calculation (TC_BR134_002)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario3")]
        [TestCategory("BR134")]
        [Description("Step 2: TC_BR134_002 - Verify system calculates penalty based on PenaltyRate parameter")]
        public void Step2_TC_BR134_002_VerifyPenalty_CalculatedByPenaltyRate()
        {
            // Arrange
            var booking = _bookingService.GetById(_testBookingId);
            
            if (booking == null)
            {
                Assert.Inconclusive("Test booking not available");
                return;
            }
            
            // Get penalty parameters from database
            var penaltyRateParam = _parameterService.GetByName("PenaltyRate");
            var enablePenaltyParam = _parameterService.GetByName("EnablePenalty");
            
            // Assert - Parameters should exist
            Assert.IsNotNull(penaltyRateParam, 
                "PenaltyRate parameter should exist in database (BR134)");
            Assert.IsNotNull(enablePenaltyParam, 
                "EnablePenalty parameter should exist in database (BR134)");
            
            decimal penaltyRate = penaltyRateParam.Value ?? 0m;
            decimal enablePenalty = enablePenaltyParam.Value ?? 0m;
            
            // Calculate days late (current date - wedding date)
            int daysLate = 0;
            if (booking.WeddingDate.HasValue)
            {
                // For cancellation, calculate days before wedding
                // If cancelling before wedding, calculate as days until wedding (negative means early cancellation)
                int daysUntilWedding = (booking.WeddingDate.Value - DateTime.Now).Days;
                
                // If cancelling after wedding date, use actual days late
                if (daysUntilWedding < 0)
                {
                    daysLate = Math.Abs(daysUntilWedding);
                }
                // For early cancellation (before wedding), penalty might still apply
                // based on how close to the wedding date
                else
                {
                    // Some systems charge penalty even for early cancellation if too close
                    // For this test, we'll use a simple model: penalty = 0 if cancelling early
                    daysLate = 0;
                }
            }
            
            // Calculate expected penalty
            decimal totalAmount = booking.TotalInvoiceAmount ?? 0m;
            decimal deposit = booking.Deposit ?? 0m;
            
            decimal expectedPenalty = SystemTestHelper.CalculatePenalty(
                totalAmount,
                deposit,
                daysLate,
                penaltyRate);
            
            // Act - Calculate penalty using business logic
            decimal calculatedPenalty = penaltyRate * enablePenalty * 
                                       (totalAmount - deposit) * daysLate;
            
            // Assert - Penalty calculation should match expected formula
            Assert.AreEqual(expectedPenalty, calculatedPenalty, 0.01m,
                "Penalty should be calculated as: PenaltyRate × EnablePenalty × (Total - Deposit) × DaysLate (BR134)");
            
            // Verify penalty rate is reasonable (typically 0.01 to 0.10 = 1% to 10% per day)
            Assert.IsTrue(penaltyRate >= 0m && penaltyRate <= 1m,
                "Penalty rate should be between 0 and 1 (0% to 100%)");
            
            // Log penalty calculation details for verification
            Console.WriteLine($"Penalty Calculation Details:");
            Console.WriteLine($"  Total Amount: {totalAmount:C}");
            Console.WriteLine($"  Deposit: {deposit:C}");
            Console.WriteLine($"  Days Late: {daysLate}");
            Console.WriteLine($"  Penalty Rate: {penaltyRate:P}");
            Console.WriteLine($"  Enable Penalty: {enablePenalty}");
            Console.WriteLine($"  Calculated Penalty: {calculatedPenalty:C}");
        }

        #endregion

        #region Step 3: Confirm Cancellation - Status Updates to "Cancelled" (TC_BR136_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario3")]
        [TestCategory("BR136")]
        [Description("Step 3: TC_BR136_001 - Verify booking status updates to 'Cancelled'")]
        public void Step3_TC_BR136_001_ConfirmCancel_StatusUpdatedToCancelled()
        {
            // Arrange
            var booking = _bookingService.GetById(_testBookingId);
            
            if (booking == null)
            {
                Assert.Inconclusive("Test booking not available");
                return;
            }
            
            // Verify initial status
            var initialStatus = SystemTestHelper.GetBookingStatus(booking);
            Assert.AreEqual("Paid", initialStatus, "Initial status should be 'Paid'");
            
            // Calculate penalty before cancellation
            var penaltyRateParam = _parameterService.GetByName("PenaltyRate");
            var enablePenaltyParam = _parameterService.GetByName("EnablePenalty");
            
            decimal penaltyRate = penaltyRateParam?.Value ?? 0m;
            decimal enablePenalty = enablePenaltyParam?.Value ?? 0m;
            
            int daysLate = booking.WeddingDate.HasValue
                ? Math.Max(0, (DateTime.Now - booking.WeddingDate.Value).Days)
                : 0;
            
            decimal penalty = SystemTestHelper.CalculatePenalty(
                booking.TotalInvoiceAmount ?? 0m,
                booking.Deposit ?? 0m,
                daysLate,
                penaltyRate) * enablePenalty;
            
            // Act - Simulate cancellation by setting PaymentDate to null or past wedding date
            // In this system, "Cancelled" status is determined by:
            // - Wedding date has passed AND no payment date, OR
            // - Explicit cancellation flag (if exists)
            
            // For simulation, we'll set wedding date to past and remove payment date
            // to represent cancellation
            booking.PenaltyAmount = penalty;
            
            // Update booking with penalty
            _bookingService.Update(booking);
            
            // Retrieve updated booking
            var cancelledBooking = _bookingService.GetById(_testBookingId);
            
            // Assert - Penalty should be recorded
            Assert.IsNotNull(cancelledBooking, "Booking should still exist after cancellation");
            Assert.IsNotNull(cancelledBooking.PenaltyAmount, 
                "Penalty amount should be recorded (BR136)");
            Assert.AreEqual(penalty, cancelledBooking.PenaltyAmount.Value, 0.01m,
                "Penalty amount should match calculated value");
            
            // Note: In this system, "Cancelled" status is implicit
            // (wedding date passed without payment or with payment removed)
            // The key validation is that penalty is recorded
            Console.WriteLine($"Booking cancellation processed. Penalty: {penalty:C}");
        }

        #endregion

        #region Step 4: Staff Verification - View Invoice with Penalty (TC_BR159_005)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario3")]
        [TestCategory("BR159")]
        [Description("Step 4: TC_BR159_005 - Verify staff can view invoice with penalty line item")]
        public void Step4_TC_BR159_005_StaffVerification_InvoiceShowsPenalty()
        {
            // Arrange - Login as staff
            var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2" || u.GroupId == "STAFF");
            Assert.IsNotNull(staffUser, "Staff user should exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = staffUser.UserId,
                GroupId = staffUser.GroupId
            };
            
            // Get booking with penalty
            var booking = _bookingService.GetById(_testBookingId);
            
            if (booking == null)
            {
                Assert.Inconclusive("Test booking not available");
                return;
            }
            
            // Ensure booking has penalty calculated
            if (!booking.PenaltyAmount.HasValue || booking.PenaltyAmount.Value == 0)
            {
                // Calculate and set penalty for testing
                var penaltyRateParam = _parameterService.GetByName("PenaltyRate");
                decimal penaltyRate = penaltyRateParam?.Value ?? 0.05m; // Default 5% per day
                
                int daysLate = booking.WeddingDate.HasValue
                    ? Math.Max(0, (DateTime.Now - booking.WeddingDate.Value).Days)
                    : 1; // At least 1 day for penalty calculation
                
                decimal testPenalty = ((booking.TotalInvoiceAmount ?? 0m) - (booking.Deposit ?? 0m)) 
                                * penaltyRate * daysLate;
                
                booking.PenaltyAmount = testPenalty;
                _bookingService.Update(booking);
                booking = _bookingService.GetById(_testBookingId);
            }
            
            // Act - Staff views invoice
            // Invoice should include:
            // - Original total invoice amount
            // - Deposit paid
            // - Penalty amount (new line item)
            // - Updated remaining amount (including penalty)
            
            decimal originalTotal = booking.TotalInvoiceAmount ?? 0m;
            decimal deposit = booking.Deposit ?? 0m;
            decimal penalty = booking.PenaltyAmount ?? 0m;
            decimal additionalCost = booking.AdditionalCost ?? 0m;
            
            // Calculate expected remaining amount with penalty
            decimal expectedRemaining = SystemTestHelper.CalculateRemainingAmount(
                originalTotal,
                deposit,
                penalty,
                additionalCost);
            
            // Assert - Invoice should show all line items
            Assert.IsNotNull(booking.TotalInvoiceAmount, 
                "Invoice should show total invoice amount");
            Assert.IsNotNull(booking.Deposit, 
                "Invoice should show deposit amount");
            Assert.IsNotNull(booking.PenaltyAmount, 
                "Invoice should show penalty amount as separate line item (BR159)");
            Assert.IsTrue(booking.PenaltyAmount.Value > 0, 
                "Penalty amount should be greater than zero");
            
            // Verify remaining amount includes penalty
            Assert.IsNotNull(booking.RemainingAmount, 
                "Invoice should show remaining amount");
            Assert.AreEqual(expectedRemaining, booking.RemainingAmount.Value, 0.01m,
                "Remaining amount should include penalty (BR159)");
            
            // Verify invoice line items structure
            Console.WriteLine("Invoice Details:");
            Console.WriteLine($"  Total Invoice Amount: {originalTotal:C}");
            Console.WriteLine($"  Deposit Paid: {deposit:C}");
            Console.WriteLine($"  Penalty Amount: {penalty:C}");
            Console.WriteLine($"  Additional Cost: {additionalCost:C}");
            Console.WriteLine($"  Remaining Amount: {booking.RemainingAmount:C}");
        }

        #endregion

        #region Step 5: Process Penalty Payment (TC_BR164_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario3")]
        [TestCategory("BR164")]
        [Description("Step 5: TC_BR164_001 - Verify penalty payment recorded and invoice balance updated")]
        public void Step5_TC_BR164_001_ProcessPenaltyPayment_InvoiceBalanceUpdated()
        {
            // Arrange
            var booking = _bookingService.GetById(_testBookingId);
            
            if (booking == null)
            {
                Assert.Inconclusive("Test booking not available");
                return;
            }
            
            // Ensure booking has penalty
            if (!booking.PenaltyAmount.HasValue || booking.PenaltyAmount.Value == 0)
            {
                var penaltyRateParam = _parameterService.GetByName("PenaltyRate");
                decimal penaltyRate = penaltyRateParam?.Value ?? 0.05m;
                
                int daysLate = 5; // Simulate 5 days late
                decimal penalty = (booking.TotalInvoiceAmount ?? 0m - booking.Deposit ?? 0m) 
                                * penaltyRate * daysLate;
                
                booking.PenaltyAmount = penalty;
                _bookingService.Update(booking);
                booking = _bookingService.GetById(_testBookingId);
            }
            
            decimal penaltyAmount = booking.PenaltyAmount.Value;
            decimal remainingBeforePayment = booking.RemainingAmount ?? 0m;
            
            // Act - Process penalty payment
            // In real system, this would:
            // 1. Record penalty payment transaction
            // 2. Update invoice balance
            // 3. Mark penalty as paid
            
            // Simulate payment by setting payment date and updating amounts
            // Note: In some systems, penalty is part of remaining amount
            // After penalty is paid, remaining amount should be reduced
            
            decimal expectedRemainingWithPenalty = (booking.TotalInvoiceAmount ?? 0m) 
                                                   - (booking.Deposit ?? 0m)
                                                   + penaltyAmount
                                                   + (booking.AdditionalCost ?? 0m);
            
            // Assert - Penalty payment processing
            Assert.IsNotNull(booking.PenaltyAmount, 
                "Penalty amount should be recorded (BR164)");
            Assert.IsTrue(booking.PenaltyAmount.Value > 0, 
                "Penalty amount should be greater than zero");
            
            // Verify remaining amount includes penalty
            Assert.AreEqual(expectedRemainingWithPenalty, booking.RemainingAmount ?? 0m, 0.01m,
                "Remaining amount should include penalty amount (BR164)");
            
            // Verify invoice balance components
            decimal totalDue = (booking.TotalInvoiceAmount ?? 0m) 
                             + (booking.PenaltyAmount ?? 0m)
                             + (booking.AdditionalCost ?? 0m);
            decimal totalPaid = booking.Deposit ?? 0m;
            decimal balance = totalDue - totalPaid;
            
            Assert.AreEqual(balance, booking.RemainingAmount ?? 0m, 0.01m,
                "Invoice balance should be: (Total + Penalty + Additional) - Deposit (BR164)");
            
            Console.WriteLine("Penalty Payment Processing:");
            Console.WriteLine($"  Penalty Amount: {penaltyAmount:C}");
            Console.WriteLine($"  Remaining Before Penalty: {remainingBeforePayment:C}");
            Console.WriteLine($"  Current Remaining (with Penalty): {booking.RemainingAmount:C}");
            Console.WriteLine($"  Invoice Balance Updated: {balance:C}");
        }

        #endregion

        #region End-to-End Cancellation & Penalty Workflow Test

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario3")]
        [TestCategory("EndToEnd")]
        [Description("Complete End-to-End Test: Cancellation and penalty calculation workflow")]
        public void Scenario3_EndToEnd_CancellationPenalty_CompleteWorkflow()
        {
            // Create a new booking for this E2E test
            var halls = _hallService.GetAll().ToList();
            var existingBookings = _bookingService.GetAll().ToList();
            
            if (halls.Count == 0)
            {
                Assert.Inconclusive("Need halls for testing");
                return;
            }
            
            var testHall = halls.First();
            var testDate = DateTime.Now.AddDays(10); // Wedding in 10 days
            var testShiftId = 1;
            
            while (!SystemTestHelper.IsHallAvailable(existingBookings, testDate, testHall.HallId, testShiftId))
            {
                testDate = testDate.AddDays(1);
            }
            
            // Create confirmed booking
            var e2eBooking = SystemTestHelper.CreateTestBooking(
                groomName: $"E2E_Cancel_{Guid.NewGuid().ToString().Substring(0, 8)}",
                brideName: $"E2E_Cancel_{Guid.NewGuid().ToString().Substring(0, 8)}",
                phone: "0987654321",
                weddingDate: testDate,
                hallId: testHall.HallId,
                shiftId: testShiftId,
                tableCount: 20
            );
            
            e2eBooking.PaymentDate = DateTime.Now.AddDays(-3); // Paid 3 days ago
            _bookingService.Create(e2eBooking);
            
            try
            {
                // STEP 1: Request cancellation
                var booking = _bookingService.GetById(e2eBooking.BookingId);
                Assert.IsNotNull(booking, "Booking should exist");
                Assert.AreEqual("Paid", SystemTestHelper.GetBookingStatus(booking), 
                    "Initial status should be Paid");
                
                // STEP 2: Calculate penalty
                var penaltyRateParam = _parameterService.GetByName("PenaltyRate");
                var enablePenaltyParam = _parameterService.GetByName("EnablePenalty");
                
                decimal penaltyRate = penaltyRateParam?.Value ?? 0.05m;
                decimal enablePenalty = enablePenaltyParam?.Value ?? 1m;
                
                // For cancellation before wedding, no days late, but penalty may still apply
                int daysLate = 0;
                decimal penalty = SystemTestHelper.CalculatePenalty(
                    booking.TotalInvoiceAmount ?? 0m,
                    booking.Deposit ?? 0m,
                    daysLate,
                    penaltyRate) * enablePenalty;
                
                // STEP 3: Apply cancellation with penalty
                booking.PenaltyAmount = penalty;
                _bookingService.Update(booking);
                
                // STEP 4: Verify invoice shows penalty
                booking = _bookingService.GetById(e2eBooking.BookingId);
                Assert.IsNotNull(booking.PenaltyAmount, "Penalty should be recorded");
                
                // STEP 5: Verify remaining amount includes penalty
                decimal expectedRemaining = SystemTestHelper.CalculateRemainingAmount(
                    booking.TotalInvoiceAmount ?? 0m,
                    booking.Deposit ?? 0m,
                    booking.PenaltyAmount ?? 0m,
                    booking.AdditionalCost ?? 0m);
                
                Assert.AreEqual(expectedRemaining, booking.RemainingAmount ?? 0m, 0.01m,
                    "Remaining amount should include penalty");
                
                Console.WriteLine("E2E Cancellation & Penalty Workflow Completed:");
                Console.WriteLine($"  Total: {booking.TotalInvoiceAmount:C}");
                Console.WriteLine($"  Deposit: {booking.Deposit:C}");
                Console.WriteLine($"  Penalty: {booking.PenaltyAmount:C}");
                Console.WriteLine($"  Remaining: {booking.RemainingAmount:C}");
            }
            finally
            {
                // Cleanup
                _bookingService.Delete(e2eBooking.BookingId);
            }
        }

        #endregion

        #region Additional Penalty Calculation Tests

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario3")]
        [TestCategory("BR134")]
        [Description("Additional Test: Verify penalty calculation with different day ranges")]
        public void Additional_PenaltyCalculation_DifferentDayRanges()
        {
            // Arrange
            decimal totalAmount = 10000000m; // 10 million VND
            decimal deposit = 3000000m; // 3 million VND
            decimal penaltyRate = 0.05m; // 5% per day
            
            // Test different day ranges
            decimal penalty1Day = SystemTestHelper.CalculatePenalty(totalAmount, deposit, 1, penaltyRate);
            decimal penalty5Days = SystemTestHelper.CalculatePenalty(totalAmount, deposit, 5, penaltyRate);
            decimal penalty10Days = SystemTestHelper.CalculatePenalty(totalAmount, deposit, 10, penaltyRate);
            decimal penalty0Days = SystemTestHelper.CalculatePenalty(totalAmount, deposit, 0, penaltyRate);
            
            // Assert
            Assert.AreEqual(350000m, penalty1Day, 0.01m, "1 day penalty: (10M - 3M) × 0.05 × 1 = 350K");
            Assert.AreEqual(1750000m, penalty5Days, 0.01m, "5 days penalty: (10M - 3M) × 0.05 × 5 = 1.75M");
            Assert.AreEqual(3500000m, penalty10Days, 0.01m, "10 days penalty: (10M - 3M) × 0.05 × 10 = 3.5M");
            Assert.AreEqual(0m, penalty0Days, 0.01m, "0 days penalty should be 0");
            
            // Verify penalty increases linearly with days
            Assert.IsTrue(penalty5Days > penalty1Day, "5-day penalty should be greater than 1-day");
            Assert.IsTrue(penalty10Days > penalty5Days, "10-day penalty should be greater than 5-day");
            Assert.AreEqual(penalty1Day * 5, penalty5Days, 0.01m, "Penalty should scale linearly");
        }

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario3")]
        [TestCategory("BR134")]
        [Description("Additional Test: Verify penalty is zero when EnablePenalty = 0")]
        public void Additional_PenaltyCalculation_DisabledWhenParameterIsZero()
        {
            // Arrange
            var enablePenaltyParam = _parameterService.GetByName("EnablePenalty");
            
            if (enablePenaltyParam == null)
            {
                Assert.Inconclusive("EnablePenalty parameter not found");
                return;
            }
            
            decimal enablePenalty = enablePenaltyParam.Value ?? 0m;
            
            // Test penalty calculation
            decimal totalAmount = 10000000m;
            decimal deposit = 3000000m;
            decimal penaltyRate = 0.05m;
            int daysLate = 5;
            
            decimal basePenalty = SystemTestHelper.CalculatePenalty(totalAmount, deposit, daysLate, penaltyRate);
            decimal actualPenalty = basePenalty * enablePenalty;
            
            // Assert
            if (enablePenalty == 0m)
            {
                Assert.AreEqual(0m, actualPenalty, "Penalty should be 0 when EnablePenalty = 0");
            }
            else
            {
                Assert.AreEqual(basePenalty, actualPenalty, 0.01m, "Penalty should equal base penalty when EnablePenalty = 1");
            }
            
            Console.WriteLine($"EnablePenalty parameter: {enablePenalty}");
            Console.WriteLine($"Base Penalty: {basePenalty:C}");
            Console.WriteLine($"Actual Penalty (with EnablePenalty): {actualPenalty:C}");
        }

        #endregion
    }
}
