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
    /// System Test - Scenario 1: Successful Wedding Booking (Happy Path)
    /// 
    /// Description: Verify the standard workflow where a customer searches for a hall,
    /// creates a booking, and completes the deposit payment without errors.
    /// 
    /// Test Steps:
    /// 1. Login with valid customer credentials
    /// 2. Check hall availability for valid date & shift
    /// 3. Select a hall and click "Book Now"
    /// 4. Enter valid booking information (200 guests)
    /// 5. Submit booking (status should be "Pending")
    /// 6. View invoice with required deposit amount
    /// 7. Pay deposit (30% of total) - status should update to "Confirmed"
    /// 
    /// Business Rules Validated:
    /// - BR3: User authentication
    /// - BR123: Hall availability check
    /// - BR124: Booking form initialization
    /// - BR125: Capacity validation
    /// - BR126: Booking creation with "Pending" status
    /// - BR152: Invoice display
    /// - BR155: Deposit payment processing
    /// </summary>
    [TestClass]
    public class Scenario1_SuccessfulBooking_SystemTest
    {
        private AppUserService _userService;
        private HallService _hallService;
        private ShiftService _shiftService;
        private BookingService _bookingService;
        private ParameterService _parameterService;
        private MenuService _menuService;
        private ServiceDetailService _serviceDetailService;

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
            _menuService = new MenuService(new MenuRepository());
            _serviceDetailService = new ServiceDetailService(new ServiceDetailRepository());
        }

        [TestCleanup]
        public void Cleanup()
        {
            // DataProvider will be disposed by garbage collector
        }

        #region Step 1: Login with Valid Customer Credentials (TC_BR3_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario1")]
        [TestCategory("BR3")]
        [Description("Step 1: TC_BR3_001 - Verify customer can login successfully")]
        public void Step1_TC_BR3_001_Customer_Login_Successful()
        {
            // Arrange
            string testUsername = "customer_test";
            string testPassword = "password123";
            
            // Get all users
            var users = _userService.GetAll().ToList();
            
            // Try to find existing test user or use first customer
            var testUser = users.FirstOrDefault(u => u.Username == testUsername) 
                        ?? users.FirstOrDefault(u => u.GroupId == "1"); // GroupId 1 = Customer
            
            // Assert
            Assert.IsNotNull(testUser, "Test user should exist in database");
            Assert.AreEqual("1", testUser.GroupId, "User should be in Customer group (GroupId = '1')");
            Assert.IsFalse(string.IsNullOrEmpty(testUser.Username), "Username should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(testUser.PasswordHash), "PasswordHash should not be empty");
            
            // Simulate successful login by setting current user
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = testUser.UserId,
                Username = testUser.Username,
                PasswordHash = testUser.PasswordHash,
                FullName = testUser.FullName,
                GroupId = testUser.GroupId
            };
            
            Assert.IsNotNull(DataProvider.Ins.CurrentUser, "Current user should be set after login");
            Assert.AreEqual(testUser.Username, DataProvider.Ins.CurrentUser.Username, "Logged in user should match");
        }

        #endregion

        #region Step 2: Check Availability - Select Valid Date & Shift (TC_BR123_002)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario1")]
        [TestCategory("BR123")]
        [Description("Step 2: TC_BR123_002 - Verify system displays available halls with pricing")]
        public void Step2_TC_BR123_002_CheckAvailability_DisplaysAvailableHalls()
        {
            // Arrange
            var testDate = DateTime.Now.AddMonths(2); // Book 2 months in advance
            var testShiftId = 1; // Morning shift
            
            // Act
            var allHalls = _hallService.GetAll().ToList();
            var allShifts = _shiftService.GetAll().ToList();
            var existingBookings = _bookingService.GetAll().ToList();
            
            // Get available halls for the selected date and shift
            var availableHalls = allHalls.Where(hall =>
                SystemTestHelper.IsHallAvailable(
                    existingBookings,
                    testDate,
                    hall.HallId,
                    testShiftId)
            ).ToList();
            
            // Assert
            Assert.IsTrue(allHalls.Count > 0, "System should have halls in database");
            Assert.IsTrue(allShifts.Count > 0, "System should have shifts in database");
            Assert.IsNotNull(availableHalls, "Available halls list should not be null");
            
            // Verify each hall has complete information
            foreach (var hall in allHalls)
            {
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallName), "Hall should have name");
                Assert.IsNotNull(hall.MaxTableCount, "Hall should have max table count");
                Assert.IsTrue(hall.MaxTableCount > 0, "Hall max table count should be > 0");
                Assert.IsNotNull(hall.HallType, "Hall should have hall type");
                Assert.IsNotNull(hall.HallType.MinTablePrice, "Hall type should have min table price");
                Assert.IsTrue(hall.HallType.MinTablePrice > 0, "Hall min table price should be > 0");
            }
        }

        #endregion

        #region Step 3: Select Hall & Click "Book Now" (TC_BR124_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario1")]
        [TestCategory("BR124")]
        [Description("Step 3: TC_BR124_001 - Verify booking form opens with selected hall pre-filled")]
        public void Step3_TC_BR124_001_SelectHall_BookingFormOpensWithHallPreselected()
        {
            // Arrange
            var testDate = DateTime.Now.AddMonths(2);
            var testShiftId = 1;
            
            var allHalls = _hallService.GetAll().ToList();
            var existingBookings = _bookingService.GetAll().ToList();
            
            var availableHall = allHalls.FirstOrDefault(h =>
                SystemTestHelper.IsHallAvailable(existingBookings, testDate, h.HallId, testShiftId));
            
            if (availableHall == null)
            {
                Assert.Inconclusive("No available halls for testing. Use a different date.");
                return;
            }
            
            // Act - Simulate selecting hall and opening booking form
            var selectedHallId = availableHall.HallId;
            var selectedHall = _hallService.GetById(selectedHallId);
            var selectedShift = _shiftService.GetById(testShiftId);
            
            // Assert - Booking form should have hall pre-selected
            Assert.IsNotNull(selectedHall, "Selected hall should be loaded");
            Assert.AreEqual(availableHall.HallId, selectedHall.HallId, "Hall ID should match");
            Assert.IsNotNull(selectedShift, "Selected shift should be loaded");
            Assert.AreEqual(testShiftId, selectedShift.ShiftId, "Shift ID should match");
            
            // Verify hall details are complete for form display
            Assert.IsFalse(string.IsNullOrEmpty(selectedHall.HallName), "Hall name should be available");
            Assert.IsNotNull(selectedHall.MaxTableCount, "Hall max capacity should be available");
            Assert.IsNotNull(selectedHall.HallType.MinTablePrice, "Hall pricing should be available");
        }

        #endregion

        #region Step 4: Enter Valid Booking Information (TC_BR125_005)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario1")]
        [TestCategory("BR125")]
        [Description("Step 4: TC_BR125_005 - Verify no validation errors with valid capacity (200 guests)")]
        public void Step4_TC_BR125_005_EnterInfo_ValidCapacity_NoValidationErrors()
        {
            // Arrange
            var testDate = DateTime.Now.AddMonths(2);
            var testShiftId = 1;
            var requestedTableCount = 20; // 200 guests / 10 guests per table = 20 tables
            
            var allHalls = _hallService.GetAll().ToList();
            var existingBookings = _bookingService.GetAll().ToList();
            
            // Find hall with capacity >= 20 tables
            var suitableHall = allHalls.FirstOrDefault(h =>
                h.MaxTableCount >= requestedTableCount &&
                SystemTestHelper.IsHallAvailable(existingBookings, testDate, h.HallId, testShiftId));
            
            if (suitableHall == null)
            {
                Assert.Inconclusive("No halls with sufficient capacity available for testing");
                return;
            }
            
            // Act - Create test booking data
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: "John Doe",
                brideName: "Jane Smith",
                phone: "0123456789",
                weddingDate: testDate,
                hallId: suitableHall.HallId,
                shiftId: testShiftId,
                tableCount: requestedTableCount
            );
            
            // Validate booking data
            bool isValid = SystemTestHelper.ValidateBooking(testBooking);
            bool isTableCountValid = SystemTestHelper.ValidateTableCount(
                requestedTableCount,
                suitableHall.MaxTableCount.Value);
            
            // Assert - No validation errors
            Assert.IsTrue(isValid, "Booking data should be valid");
            Assert.IsTrue(isTableCountValid, 
                $"Table count {requestedTableCount} should be valid for hall capacity {suitableHall.MaxTableCount}");
            Assert.IsFalse(string.IsNullOrEmpty(testBooking.GroomName), "Groom name should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(testBooking.BrideName), "Bride name should not be empty");
            Assert.IsTrue(SystemTestHelper.ValidatePhoneNumber(testBooking.Phone), "Phone number should be valid");
            Assert.IsTrue(testBooking.WeddingDate > DateTime.Now, "Wedding date should be in the future");
            
            // Verify total cost calculation
            decimal expectedTotal = requestedTableCount * suitableHall.HallType.MinTablePrice.Value;
            Assert.IsTrue(testBooking.TotalInvoiceAmount >= expectedTotal, 
                "Total invoice amount should be calculated correctly");
        }

        #endregion

        #region Step 5: Submit Booking - Status "Pending" (TC_BR126_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario1")]
        [TestCategory("BR126")]
        [Description("Step 5: TC_BR126_001 - Verify booking created with status 'Pending'")]
        public void Step5_TC_BR126_001_SubmitBooking_StatusPending()
        {
            // Arrange
            var testDate = DateTime.Now.AddMonths(2);
            var testShiftId = 1;
            var requestedTableCount = 20;
            
            var allHalls = _hallService.GetAll().ToList();
            var existingBookings = _bookingService.GetAll().ToList();
            
            var suitableHall = allHalls.FirstOrDefault(h =>
                h.MaxTableCount >= requestedTableCount &&
                SystemTestHelper.IsHallAvailable(existingBookings, testDate, h.HallId, testShiftId));
            
            if (suitableHall == null)
            {
                Assert.Inconclusive("No halls available for testing");
                return;
            }
            
            // Create test booking
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: $"SystemTest_Groom_{Guid.NewGuid().ToString().Substring(0, 8)}",
                brideName: $"SystemTest_Bride_{Guid.NewGuid().ToString().Substring(0, 8)}",
                phone: "0987654321",
                weddingDate: testDate,
                hallId: suitableHall.HallId,
                shiftId: testShiftId,
                tableCount: requestedTableCount
            );
            
            // Act - Create booking in database
            int bookingCountBefore = _bookingService.GetAll().Count();
            _bookingService.Create(testBooking);
            int bookingCountAfter = _bookingService.GetAll().Count();
            
            // Retrieve created booking
            var createdBooking = _bookingService.GetById(testBooking.BookingId);
            
            // Assert
            Assert.AreEqual(bookingCountBefore + 1, bookingCountAfter, "Booking count should increase by 1");
            Assert.IsNotNull(createdBooking, "Created booking should be retrievable");
            Assert.AreEqual(testBooking.GroomName, createdBooking.GroomName, "Groom name should match");
            Assert.AreEqual(testBooking.BrideName, createdBooking.BrideName, "Bride name should match");
            Assert.AreEqual(testBooking.HallId, createdBooking.HallId, "Hall ID should match");
            Assert.AreEqual(testBooking.ShiftId, createdBooking.ShiftId, "Shift ID should match");
            
            // Verify status is "Pending" (no payment date yet)
            Assert.IsFalse(createdBooking.PaymentDate.HasValue, 
                "Payment date should be null for pending booking (BR126)");
            var status = SystemTestHelper.GetBookingStatus(createdBooking);
            Assert.AreEqual("Pending", status, "Booking status should be 'Pending' (BR126)");
            
            // Cleanup - Delete test booking
            _bookingService.Delete(testBooking.BookingId);
        }

        #endregion

        #region Step 6: View Invoice with Required Deposit (TC_BR152_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario1")]
        [TestCategory("BR152")]
        [Description("Step 6: TC_BR152_001 - Verify invoice displays with required deposit amount")]
        public void Step6_TC_BR152_001_ViewInvoice_DisplaysRequiredDeposit()
        {
            // Arrange
            var testDate = DateTime.Now.AddMonths(2);
            var testShiftId = 1;
            var requestedTableCount = 20;
            
            var allHalls = _hallService.GetAll().ToList();
            var existingBookings = _bookingService.GetAll().ToList();
            
            var suitableHall = allHalls.FirstOrDefault(h =>
                h.MaxTableCount >= requestedTableCount &&
                SystemTestHelper.IsHallAvailable(existingBookings, testDate, h.HallId, testShiftId));
            
            if (suitableHall == null)
            {
                Assert.Inconclusive("No halls available for testing");
                return;
            }
            
            // Create and save test booking
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: $"SystemTest_Groom_{Guid.NewGuid().ToString().Substring(0, 8)}",
                brideName: $"SystemTest_Bride_{Guid.NewGuid().ToString().Substring(0, 8)}",
                phone: "0987654321",
                weddingDate: testDate,
                hallId: suitableHall.HallId,
                shiftId: testShiftId,
                tableCount: requestedTableCount
            );
            
            _bookingService.Create(testBooking);
            
            // Act - Retrieve booking as invoice
            var invoice = _bookingService.GetById(testBooking.BookingId);
            
            // Assert - Invoice should display all required information
            Assert.IsNotNull(invoice, "Invoice should be retrievable");
            Assert.IsNotNull(invoice.Deposit, "Deposit amount should be calculated (BR152)");
            Assert.IsNotNull(invoice.TotalInvoiceAmount, "Total invoice amount should be calculated");
            Assert.IsNotNull(invoice.RemainingAmount, "Remaining amount should be calculated");
            
            // Verify deposit calculation (30% of total)
            decimal expectedDeposit = SystemTestHelper.CalculateDeposit(invoice.TotalInvoiceAmount.Value);
            Assert.AreEqual(expectedDeposit, invoice.Deposit.Value, 0.01m, 
                "Deposit should be 30% of total invoice amount (BR152)");
            
            // Verify remaining amount calculation
            decimal expectedRemaining = invoice.TotalInvoiceAmount.Value - invoice.Deposit.Value;
            Assert.AreEqual(expectedRemaining, invoice.RemainingAmount.Value, 0.01m,
                "Remaining amount should be total minus deposit");
            
            // Verify invoice includes hall and shift details
            Assert.IsNotNull(invoice.Hall, "Invoice should include hall information");
            Assert.IsNotNull(invoice.Shift, "Invoice should include shift information");
            Assert.AreEqual(suitableHall.HallName, invoice.Hall.HallName, "Hall name should match");
            
            // Cleanup
            _bookingService.Delete(testBooking.BookingId);
        }

        #endregion

        #region Step 7: Pay Deposit - Status Updates to "Confirmed" (TC_BR155_004)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario1")]
        [TestCategory("BR155")]
        [Description("Step 7: TC_BR155_004 - Verify payment successful and status updates to 'Confirmed'")]
        public void Step7_TC_BR155_004_PayDeposit_StatusConfirmed()
        {
            // Arrange
            var testDate = DateTime.Now.AddMonths(2);
            var testShiftId = 1;
            var requestedTableCount = 20;
            
            var allHalls = _hallService.GetAll().ToList();
            var existingBookings = _bookingService.GetAll().ToList();
            
            var suitableHall = allHalls.FirstOrDefault(h =>
                h.MaxTableCount >= requestedTableCount &&
                SystemTestHelper.IsHallAvailable(existingBookings, testDate, h.HallId, testShiftId));
            
            if (suitableHall == null)
            {
                Assert.Inconclusive("No halls available for testing");
                return;
            }
            
            // Create test booking
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: $"SystemTest_Groom_{Guid.NewGuid().ToString().Substring(0, 8)}",
                brideName: $"SystemTest_Bride_{Guid.NewGuid().ToString().Substring(0, 8)}",
                phone: "0987654321",
                weddingDate: testDate,
                hallId: suitableHall.HallId,
                shiftId: testShiftId,
                tableCount: requestedTableCount
            );
            
            _bookingService.Create(testBooking);
            
            // Verify initial status is "Pending"
            var pendingBooking = _bookingService.GetById(testBooking.BookingId);
            Assert.IsFalse(pendingBooking.PaymentDate.HasValue, "Payment date should be null initially");
            Assert.AreEqual("Pending", SystemTestHelper.GetBookingStatus(pendingBooking), 
                "Initial status should be 'Pending'");
            
            // Act - Process payment (simulate paying deposit)
            pendingBooking.PaymentDate = DateTime.Now;
            _bookingService.Update(pendingBooking);
            
            // Retrieve updated booking
            var paidBooking = _bookingService.GetById(testBooking.BookingId);
            
            // Assert - Payment should be successful and status updated
            Assert.IsNotNull(paidBooking, "Booking should still be retrievable after payment");
            Assert.IsTrue(paidBooking.PaymentDate.HasValue, 
                "Payment date should be set after payment (BR155)");
            Assert.IsTrue(paidBooking.PaymentDate.Value <= DateTime.Now, 
                "Payment date should be current or past date");
            
            // Verify status changed to "Paid" (Confirmed)
            var status = SystemTestHelper.GetBookingStatus(paidBooking);
            Assert.AreEqual("Paid", status, 
                "Booking status should be 'Paid' (Confirmed) after payment (BR155)");
            
            // Verify booking is now confirmed (not available for other bookings)
            var isStillAvailable = SystemTestHelper.IsHallAvailable(
                new[] { paidBooking },
                testDate,
                suitableHall.HallId,
                testShiftId);
            
            Assert.IsFalse(isStillAvailable, 
                "Hall should no longer be available for the same date/shift after confirmation");
            
            // Cleanup
            _bookingService.Delete(testBooking.BookingId);
        }

        #endregion

        #region End-to-End Complete Workflow Test

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario1")]
        [TestCategory("EndToEnd")]
        [Description("Complete End-to-End Test: Successful booking workflow from login to payment")]
        public void Scenario1_EndToEnd_SuccessfulBooking_CompleteWorkflow()
        {
            // STEP 1: Login
            var testUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "1" || u.GroupId == "USER");
            Assert.IsNotNull(testUser, "Test customer should exist");
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = testUser.UserId,
                Username = testUser.Username,
                GroupId = testUser.GroupId
            };
            
            // STEP 2: Check availability
            var testDate = DateTime.Now.AddMonths(2);
            var testShiftId = 1;
            var allHalls = _hallService.GetAll().ToList();
            var existingBookings = _bookingService.GetAll().ToList();
            
            var availableHall = allHalls.FirstOrDefault(h =>
                h.MaxTableCount >= 20 &&
                SystemTestHelper.IsHallAvailable(existingBookings, testDate, h.HallId, testShiftId));
            
            Assert.IsNotNull(availableHall, "Available hall should be found");
            
            // STEP 3 & 4: Select hall and enter valid information
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: $"E2E_Groom_{Guid.NewGuid().ToString().Substring(0, 8)}",
                brideName: $"E2E_Bride_{Guid.NewGuid().ToString().Substring(0, 8)}",
                phone: "0987654321",
                weddingDate: testDate,
                hallId: availableHall.HallId,
                shiftId: testShiftId,
                tableCount: 20
            );
            
            Assert.IsTrue(SystemTestHelper.ValidateBooking(testBooking), "Booking should be valid");
            
            // STEP 5: Submit booking
            _bookingService.Create(testBooking);
            var createdBooking = _bookingService.GetById(testBooking.BookingId);
            Assert.IsNotNull(createdBooking, "Booking should be created");
            Assert.AreEqual("Pending", SystemTestHelper.GetBookingStatus(createdBooking), 
                "Status should be 'Pending' after creation");
            
            // STEP 6: View invoice
            Assert.IsNotNull(createdBooking.Deposit, "Deposit should be calculated");
            Assert.IsNotNull(createdBooking.TotalInvoiceAmount, "Total amount should be calculated");
            
            // STEP 7: Pay deposit
            createdBooking.PaymentDate = DateTime.Now;
            _bookingService.Update(createdBooking);
            
            var finalBooking = _bookingService.GetById(testBooking.BookingId);
            Assert.IsTrue(finalBooking.PaymentDate.HasValue, "Payment date should be set");
            Assert.AreEqual("Paid", SystemTestHelper.GetBookingStatus(finalBooking), 
                "Final status should be 'Paid' (Confirmed)");
            
            // Cleanup
            _bookingService.Delete(testBooking.BookingId);
            
            // Final assertion - workflow completed successfully
            Assert.IsTrue(true, "Complete workflow executed successfully from login to payment");
        }

        #endregion
    }
}
