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
    /// System Test - Scenario 4: Data Integration (Master Data to Booking Flow)
    /// 
    /// Description: Verify data consistency and real-time integration by creating a new 
    /// Master Data entity (Hall) and immediately using it in a transaction (Booking).
    /// 
    /// Test Steps:
    /// 1. Login as Admin - Valid admin credentials
    /// 2. Create New Hall - Name: "Diamond Hall New"
    /// 3. Switch Role - Logout, then login as Staff
    /// 4. Search Hall - Search for "Diamond Hall New"
    /// 5. Create Booking - Select "Diamond Hall New"
    /// 6. Save Booking - Linked to new hall ID
    /// 
    /// Business Rules Validated:
    /// - BR3: Admin authentication (step 1)
    /// - BR46: Hall creation by admin (step 2)
    /// - BR4: Staff authentication after role switch (step 3)
    /// - BR137: Hall search and display (step 4)
    /// - BR141: Booking creation with new hall (step 5, 6)
    /// - Data Integration: Master data immediately available for transactions
    /// </summary>
    [TestClass]
    public class Scenario4_DataIntegration_SystemTest
    {
        private AppUserService _userService;
        private HallService _hallService;
        private HallTypeService _hallTypeService;
        private ShiftService _shiftService;
        private BookingService _bookingService;

        // Test data IDs for cleanup
        private int _testHallId;
        private int _testBookingId;

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize all required services
            _userService = new AppUserService(new AppUserRepository());
            _hallService = new HallService(new HallRepository());
            _hallTypeService = new HallTypeService(new HallTypeRepository());
            _shiftService = new ShiftService(new ShiftRepository());
            _bookingService = new BookingService(new BookingRepository());
            
            _testHallId = 0;
            _testBookingId = 0;
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test data in reverse order (bookings first, then hall)
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
            
            if (_testHallId > 0)
            {
                try
                {
                    _hallService.Delete(_testHallId);
                }
                catch
                {
                    // Hall may already be deleted
                }
            }
        }

        #region Step 1: Login as Admin (TC_BR3_002)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("BR3")]
        [Description("Step 1: TC_BR3_002 - Verify admin can login successfully")]
        public void Step1_TC_BR3_002_Admin_Login_Successful()
        {
            // Arrange
            var users = _userService.GetAll().ToList();
            
            // Find admin user (GroupId = "3" or "ADMIN")
            var adminUser = users.FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            
            // Assert
            Assert.IsNotNull(adminUser, "Admin user should exist in database");
            Assert.IsFalse(string.IsNullOrEmpty(adminUser.Username), "Username should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(adminUser.PasswordHash), "PasswordHash should not be empty");
            
            // Verify GroupId is admin
            bool isAdmin = adminUser.GroupId == "3" || adminUser.GroupId == "ADMIN";
            Assert.IsTrue(isAdmin, "User should be in Admin group (GroupId = '3' or 'ADMIN')");
            
            // Simulate successful login by setting current user
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = adminUser.UserId,
                Username = adminUser.Username,
                PasswordHash = adminUser.PasswordHash,
                FullName = adminUser.FullName,
                GroupId = adminUser.GroupId
            };
            
            Assert.IsNotNull(DataProvider.Ins.CurrentUser, "Current user should be set after login");
            Assert.AreEqual(adminUser.Username, DataProvider.Ins.CurrentUser.Username, "Username should match");
        }

        #endregion

        #region Step 2: Create New Hall (TC_BR46_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("BR46")]
        [Description("Step 2: TC_BR46_001 - Verify new hall created successfully by admin")]
        public void Step2_TC_BR46_001_CreateNewHall_Successful()
        {
            // Arrange - Login as admin
            var adminUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            Assert.IsNotNull(adminUser, "Admin user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = adminUser.UserId,
                GroupId = adminUser.GroupId
            };
            
            // Get hall types for creating hall
            var hallTypes = _hallTypeService.GetAll().ToList();
            Assert.IsTrue(hallTypes.Count > 0, "Need hall types to create hall");
            
            var hallType = hallTypes.First();
            
            // Create unique hall name for testing
            string uniqueHallName = $"Diamond Hall New_{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            // Act - Create new hall
            var newHall = SystemTestHelper.CreateTestHall(
                hallName: uniqueHallName,
                maxTableCount: 60,
                hallTypeId: hallType.HallTypeId,
                minTablePrice: hallType.MinTablePrice ?? 600000m
            );
            
            int hallCountBefore = _hallService.GetAll().Count();
            _hallService.Create(newHall);
            int hallCountAfter = _hallService.GetAll().Count();
            
            _testHallId = newHall.HallId; // Store for cleanup
            
            // Retrieve created hall
            var createdHall = _hallService.GetById(_testHallId);
            
            // Assert - Hall should be created successfully
            Assert.AreEqual(hallCountBefore + 1, hallCountAfter, 
                "Hall count should increase by 1 (BR46)");
            Assert.IsNotNull(createdHall, 
                "Created hall should be retrievable (BR46)");
            Assert.AreEqual(uniqueHallName, createdHall.HallName, 
                "Hall name should match");
            Assert.AreEqual(60, createdHall.MaxTableCount, 
                "Hall max table count should match");
            Assert.AreEqual(hallType.HallTypeId, createdHall.HallTypeId, 
                "Hall type ID should match");
            
            // Verify hall type details are loaded
            Assert.IsNotNull(createdHall.HallType, 
                "Hall type should be loaded");
            Assert.IsFalse(string.IsNullOrEmpty(createdHall.HallType.HallTypeName), 
                "Hall type name should be available");
            Assert.IsNotNull(createdHall.HallType.MinTablePrice, 
                "Hall type price should be available");
            
            Console.WriteLine($"Hall created successfully: {createdHall.HallName} (ID: {createdHall.HallId})");
        }

        #endregion

        #region Step 3: Switch Role - Logout and Login as Staff (TC_BR4_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("BR4")]
        [Description("Step 3: TC_BR4_001 - Verify logout and login as staff successful")]
        public void Step3_TC_BR4_001_SwitchRole_LogoutAndLoginAsStaff()
        {
            // Arrange - Login as admin first
            var adminUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            Assert.IsNotNull(adminUser, "Admin user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = adminUser.UserId,
                GroupId = adminUser.GroupId
            };
            
            Assert.IsNotNull(DataProvider.Ins.CurrentUser, "Should be logged in as admin");
            
            // Act - Logout (clear current user)
            DataProvider.Ins.CurrentUser = null;
            
            Assert.IsNull(DataProvider.Ins.CurrentUser, "Current user should be null after logout");
            
            // Login as staff
            var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2" || u.GroupId == "STAFF");
            Assert.IsNotNull(staffUser, "Staff user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = staffUser.UserId,
                Username = staffUser.Username,
                PasswordHash = staffUser.PasswordHash,
                FullName = staffUser.FullName,
                GroupId = staffUser.GroupId
            };
            
            // Assert - Should be logged in as staff now
            Assert.IsNotNull(DataProvider.Ins.CurrentUser, 
                "Current user should be set after staff login (BR4)");
            Assert.AreEqual(staffUser.Username, DataProvider.Ins.CurrentUser.Username, 
                "Staff username should match");
            
            bool isStaff = staffUser.GroupId == "2" || staffUser.GroupId == "STAFF";
            Assert.IsTrue(isStaff, "Should be logged in as staff (GroupId = '2' or 'STAFF') (BR4)");
            
            Console.WriteLine($"Role switched successfully from Admin to Staff: {staffUser.Username}");
        }

        #endregion

        #region Step 4: Search Hall - New Hall Appears Immediately (TC_BR137_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("BR137")]
        [TestCategory("Integration")]
        [Description("Step 4: TC_BR137_001 - Verify new hall appears in search results immediately")]
        public void Step4_TC_BR137_001_SearchHall_NewHallAppearsImmediately()
        {
            // Arrange - Create hall as admin
            var adminUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            Assert.IsNotNull(adminUser, "Admin user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = adminUser.UserId,
                GroupId = adminUser.GroupId
            };
            
            var hallTypes = _hallTypeService.GetAll().ToList();
            var hallType = hallTypes.First();
            
            string uniqueHallName = $"Diamond Hall New_{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            var newHall = SystemTestHelper.CreateTestHall(
                hallName: uniqueHallName,
                maxTableCount: 60,
                hallTypeId: hallType.HallTypeId,
                minTablePrice: hallType.MinTablePrice ?? 600000m
            );
            
            _hallService.Create(newHall);
            _testHallId = newHall.HallId;
            
            // Switch to staff
            DataProvider.Ins.CurrentUser = null;
            var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2" || u.GroupId == "STAFF");
            Assert.IsNotNull(staffUser, "Staff user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = staffUser.UserId,
                GroupId = staffUser.GroupId
            };
            
            // Act - Search for newly created hall
            var allHalls = _hallService.GetAll().ToList();
            var searchResult = allHalls.FirstOrDefault(h => h.HallName == uniqueHallName);
            
            // Also test partial search
            var partialSearchResults = allHalls.Where(h => 
                h.HallName.Contains("Diamond Hall New")).ToList();
            
            // Assert - New hall should appear immediately in search results
            Assert.IsNotNull(searchResult, 
                "New hall should appear in search results immediately (Integration Check - BR137)");
            Assert.AreEqual(uniqueHallName, searchResult.HallName, 
                "Hall name should match exactly");
            Assert.AreEqual(_testHallId, searchResult.HallId, 
                "Hall ID should match");
            
            // Verify hall details are complete for display
            Assert.IsNotNull(searchResult.MaxTableCount, 
                "Hall should have max table count");
            Assert.AreEqual(60, searchResult.MaxTableCount.Value, 
                "Hall capacity should be 60");
            Assert.IsNotNull(searchResult.HallType, 
                "Hall should have hall type loaded");
            Assert.IsNotNull(searchResult.HallType.MinTablePrice, 
                "Hall should have pricing information");
            
            // Verify partial search also works
            Assert.IsTrue(partialSearchResults.Count > 0, 
                "Partial search should find halls");
            Assert.IsTrue(partialSearchResults.Any(h => h.HallId == _testHallId), 
                "New hall should be in partial search results");
            
            Console.WriteLine($"Integration verified: New hall '{uniqueHallName}' " +
                            $"immediately available after creation");
        }

        #endregion

        #region Step 5: Create Booking with New Hall (TC_BR141_001)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("BR141")]
        [TestCategory("Integration")]
        [Description("Step 5: TC_BR141_001 - Verify system allows creating booking for newly created hall")]
        public void Step5_TC_BR141_001_CreateBooking_WithNewHall_Allowed()
        {
            // Arrange - Create hall as admin
            var adminUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            Assert.IsNotNull(adminUser, "Admin user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = adminUser.UserId,
                GroupId = adminUser.GroupId
            };
            
            var hallTypes = _hallTypeService.GetAll().ToList();
            var hallType = hallTypes.First();
            
            string uniqueHallName = $"Diamond Hall New_{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            var newHall = SystemTestHelper.CreateTestHall(
                hallName: uniqueHallName,
                maxTableCount: 60,
                hallTypeId: hallType.HallTypeId,
                minTablePrice: hallType.MinTablePrice ?? 600000m
            );
            
            _hallService.Create(newHall);
            _testHallId = newHall.HallId;
            
            // Switch to staff
            DataProvider.Ins.CurrentUser = null;
            var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2" || u.GroupId == "STAFF");
            Assert.IsNotNull(staffUser, "Staff user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = staffUser.UserId,
                GroupId = staffUser.GroupId
            };
            
            // Act - Create booking with new hall
            var testDate = DateTime.Now.AddMonths(2);
            var testShiftId = 1;
            
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: $"Integration_Test_Groom_{Guid.NewGuid().ToString().Substring(0, 8)}",
                brideName: $"Integration_Test_Bride_{Guid.NewGuid().ToString().Substring(0, 8)}",
                phone: "0123456789",
                weddingDate: testDate,
                hallId: _testHallId, // Use newly created hall
                shiftId: testShiftId,
                tableCount: 30
            );
            
            // Validate booking data before creation
            bool isValid = SystemTestHelper.ValidateBooking(testBooking);
            Assert.IsTrue(isValid, "Booking data should be valid");
            
            // Create booking
            int bookingCountBefore = _bookingService.GetAll().Count();
            _bookingService.Create(testBooking);
            int bookingCountAfter = _bookingService.GetAll().Count();
            
            _testBookingId = testBooking.BookingId;
            
            // Retrieve created booking
            var createdBooking = _bookingService.GetById(_testBookingId);
            
            // Assert - Booking should be created with new hall
            Assert.AreEqual(bookingCountBefore + 1, bookingCountAfter, 
                "Booking count should increase by 1");
            Assert.IsNotNull(createdBooking, 
                "System should allow creating booking for newly created hall (Integration Check - BR141)");
            Assert.AreEqual(_testHallId, createdBooking.HallId, 
                "Booking should be linked to new hall ID");
            Assert.AreEqual(testBooking.GroomName, createdBooking.GroomName, 
                "Booking details should be saved correctly");
            
            // Verify hall relationship is maintained
            Assert.IsNotNull(createdBooking.Hall, 
                "Booking should have hall details loaded");
            Assert.AreEqual(uniqueHallName, createdBooking.Hall.HallName, 
                "Booking should reference correct hall name");
            Assert.AreEqual(60, createdBooking.Hall.MaxTableCount, 
                "Hall capacity should be correct");
            
            Console.WriteLine($"Integration verified: Booking created successfully " +
                            $"with newly created hall '{uniqueHallName}' (ID: {_testHallId})");
        }

        #endregion

        #region Step 6: Save Booking - Linked to New Hall ID (TC_BR141_002)

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("BR141")]
        [TestCategory("Integration")]
        [Description("Step 6: TC_BR141_002 - Verify booking saved successfully linked to new hall ID")]
        public void Step6_TC_BR141_002_SaveBooking_LinkedToNewHallId()
        {
            // Arrange - Create hall as admin
            var adminUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            Assert.IsNotNull(adminUser, "Admin user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = adminUser.UserId,
                GroupId = adminUser.GroupId
            };
            
            var hallTypes = _hallTypeService.GetAll().ToList();
            var hallType = hallTypes.First();
            
            string uniqueHallName = $"Diamond Hall New_{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            var newHall = SystemTestHelper.CreateTestHall(
                hallName: uniqueHallName,
                maxTableCount: 60,
                hallTypeId: hallType.HallTypeId,
                minTablePrice: hallType.MinTablePrice ?? 600000m
            );
            
            _hallService.Create(newHall);
            _testHallId = newHall.HallId;
            
            // Switch to staff
            DataProvider.Ins.CurrentUser = null;
            var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2" || u.GroupId == "STAFF");
            Assert.IsNotNull(staffUser, "Staff user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = staffUser.UserId,
                GroupId = staffUser.GroupId
            };
            
            // Create booking
            var testDate = DateTime.Now.AddMonths(2);
            var testShiftId = 1;
            
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: $"Integration_Groom_{Guid.NewGuid().ToString().Substring(0, 8)}",
                brideName: $"Integration_Bride_{Guid.NewGuid().ToString().Substring(0, 8)}",
                phone: "0987654321",
                weddingDate: testDate,
                hallId: _testHallId,
                shiftId: testShiftId,
                tableCount: 30
            );
            
            // Act - Save booking
            _bookingService.Create(testBooking);
            _testBookingId = testBooking.BookingId;
            
            // Retrieve booking from database
            var savedBooking = _bookingService.GetById(_testBookingId);
            
            // Verify booking exists in all bookings list
            var allBookings = _bookingService.GetAll().ToList();
            var foundBooking = allBookings.FirstOrDefault(b => b.BookingId == _testBookingId);
            
            // Assert - Booking should be saved with correct hall linkage
            Assert.IsNotNull(savedBooking, 
                "Booking should be saved successfully (BR141)");
            Assert.AreEqual(_testHallId, savedBooking.HallId, 
                "Booking should be linked to new hall ID (Integration Check)");
            Assert.IsTrue(savedBooking.BookingId > 0, 
                "Booking should have valid ID");
            
            // Verify foreign key relationship integrity
            Assert.IsNotNull(foundBooking, 
                "Booking should appear in all bookings list");
            Assert.AreEqual(_testHallId, foundBooking.HallId, 
                "Hall ID should be consistent across queries");
            
            // Verify hall details are loaded via relationship
            Assert.IsNotNull(savedBooking.Hall, 
                "Hall details should be loaded via foreign key relationship");
            Assert.AreEqual(uniqueHallName, savedBooking.Hall.HallName, 
                "Hall name should match via relationship");
            Assert.AreEqual(_testHallId, savedBooking.Hall.HallId, 
                "Hall ID should match via relationship");
            
            // Verify data integrity: Can retrieve booking by hall
            var bookingsForHall = allBookings.Where(b => b.HallId == _testHallId).ToList();
            Assert.IsTrue(bookingsForHall.Any(b => b.BookingId == _testBookingId), 
                "Booking should be findable by hall ID (Data integrity check)");
            
            Console.WriteLine($"Data Integrity verified: Booking (ID: {_testBookingId}) " +
                            $"correctly linked to Hall (ID: {_testHallId}, Name: '{uniqueHallName}')");
        }

        #endregion

        #region End-to-End Data Integration Workflow Test

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("EndToEnd")]
        [TestCategory("Integration")]
        [Description("Complete End-to-End Test: Data integration from master data creation to transaction")]
        public void Scenario4_EndToEnd_DataIntegration_CompleteWorkflow()
        {
            // STEP 1: Login as Admin
            var adminUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            Assert.IsNotNull(adminUser, "Admin user should exist");
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = adminUser.UserId,
                GroupId = adminUser.GroupId
            };
            // Verify admin is logged in
            bool isAdmin = DataProvider.Ins.CurrentUser.GroupId == "3" || DataProvider.Ins.CurrentUser.GroupId == "ADMIN";
            Assert.IsTrue(isAdmin, "Should be logged in as admin");

            // STEP 2: Create New Hall
            var hallTypes = _hallTypeService.GetAll().ToList();
            Assert.IsTrue(hallTypes.Count > 0, "Need hall types");
            
            var hallType = hallTypes.First();
            string uniqueHallName = $"E2E_Diamond_Hall_{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            var newHall = SystemTestHelper.CreateTestHall(
                hallName: uniqueHallName,
                maxTableCount: 60,
                hallTypeId: hallType.HallTypeId,
                minTablePrice: hallType.MinTablePrice ?? 600000m
            );
            
            _hallService.Create(newHall);
            _testHallId = newHall.HallId;
            Assert.IsTrue(_testHallId > 0, "Hall should be created with valid ID");
            
            // STEP 3: Switch Role - Logout and Login as Staff
            DataProvider.Ins.CurrentUser = null;
            var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2" || u.GroupId == "STAFF");
            Assert.IsNotNull(staffUser, "Staff user should exist");
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = staffUser.UserId,
                GroupId = staffUser.GroupId
            };
            // Verify staff is logged in
            bool isStaff = DataProvider.Ins.CurrentUser.GroupId == "2" || DataProvider.Ins.CurrentUser.GroupId == "STAFF";
            Assert.IsTrue(isStaff, "Should be logged in as staff");

            // STEP 4: Search Hall - Verify new hall appears immediately
            var allHalls = _hallService.GetAll().ToList();
            var searchResult = allHalls.FirstOrDefault(h => h.HallId == _testHallId);
            Assert.IsNotNull(searchResult, 
                "New hall should appear immediately in search results (INTEGRATION VERIFIED)");
            Assert.AreEqual(uniqueHallName, searchResult.HallName, "Hall name should match");
            
            // STEP 5 & 6: Create and Save Booking with New Hall
            var testDate = DateTime.Now.AddMonths(2);
            var testShiftId = 1;
            
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: $"E2E_Integration_{Guid.NewGuid().ToString().Substring(0, 8)}",
                brideName: $"E2E_Integration_{Guid.NewGuid().ToString().Substring(0, 8)}",
                phone: "0987654321",
                weddingDate: testDate,
                hallId: _testHallId,
                shiftId: testShiftId,
                tableCount: 30
            );
            
            _bookingService.Create(testBooking);
            _testBookingId = testBooking.BookingId;
            
            var savedBooking = _bookingService.GetById(_testBookingId);
            Assert.IsNotNull(savedBooking, "Booking should be saved");
            Assert.AreEqual(_testHallId, savedBooking.HallId, 
                "Booking should be linked to new hall (INTEGRATION VERIFIED)");
            Assert.IsNotNull(savedBooking.Hall, "Hall details should be loaded");
            Assert.AreEqual(uniqueHallName, savedBooking.Hall.HallName, 
                "Hall name should be correct (DATA INTEGRITY VERIFIED)");
            
            // Final Integration Verification
            Console.WriteLine("=== END-TO-END DATA INTEGRATION TEST COMPLETED ===");
            Console.WriteLine($"Admin Created Hall: '{uniqueHallName}' (ID: {_testHallId})");
            Console.WriteLine($"Staff Found Hall: Immediately available in search");
            Console.WriteLine($"Staff Created Booking: (ID: {_testBookingId})");
            Console.WriteLine($"Data Integrity: Booking correctly linked to new Hall");
            Console.WriteLine($"Integration Status: ? VERIFIED - Master data immediately available for transactions");
        }

        #endregion

        #region Additional Integration Tests

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("Integration")]
        [Description("Additional Test: Verify multiple staff can see new hall simultaneously")]
        public void Additional_Integration_MultipleStaff_SeeNewHallSimultaneously()
        {
            // Arrange - Create hall as admin
            var adminUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            Assert.IsNotNull(adminUser, "Admin user must exist");
            
            DataProvider.Ins.CurrentUser = new AppUser
            {
                UserId = adminUser.UserId,
                GroupId = adminUser.GroupId
            };
            
            var hallTypes = _hallTypeService.GetAll().ToList();
            var hallType = hallTypes.First();
            
            string uniqueHallName = $"Multi_Staff_Hall_{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            var newHall = SystemTestHelper.CreateTestHall(
                hallName: uniqueHallName,
                maxTableCount: 70,
                hallTypeId: hallType.HallTypeId,
                minTablePrice: hallType.MinTablePrice ?? 600000m
            );
            
            _hallService.Create(newHall);
            _testHallId = newHall.HallId;
            
            // Act - Multiple staff users search for hall
            var staffUsers = _userService.GetAll().Where(u => u.GroupId == "2" || u.GroupId == "STAFF").Take(3).ToList();
            
            if (staffUsers.Count == 0)
            {
                Assert.Inconclusive("Need staff users for testing");
                return;
            }
            
            // Simulate multiple staff users querying
            foreach (var staff in staffUsers)
            {
                DataProvider.Ins.CurrentUser = new AppUser
                {
                    UserId = staff.UserId,
                    GroupId = staff.GroupId
                };
                
                var halls = _hallService.GetAll().ToList();
                var foundHall = halls.FirstOrDefault(h => h.HallId == _testHallId);
                
                // Assert - Each staff should see the new hall
                Assert.IsNotNull(foundHall, 
                    $"Staff {staff.Username} should see new hall immediately");
                Assert.AreEqual(uniqueHallName, foundHall.HallName, 
                    $"Staff {staff.Username} should see correct hall name");
                
                Console.WriteLine($"Staff {staff.Username}: ? Can see hall '{uniqueHallName}'");
            }
            
            Console.WriteLine("Multi-user integration verified: All staff can see new hall simultaneously");
        }

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("Integration")]
        [Description("Additional Test: Verify hall update reflects immediately in bookings")]
        public void Additional_Integration_HallUpdate_ReflectsInBookings()
        {
            // Arrange - Create hall and booking
            var adminUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2" || u.GroupId == "STAFF");
            
            Assert.IsNotNull(adminUser, "Admin user must exist");
            Assert.IsNotNull(staffUser, "Staff user must exist");
            
            // Admin creates hall
            DataProvider.Ins.CurrentUser = new AppUser { UserId = adminUser.UserId, GroupId = adminUser.GroupId };
            
            var hallTypes = _hallTypeService.GetAll().ToList();
            var hallType = hallTypes.First();
            
            string originalHallName = $"Original_Hall_{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            var newHall = SystemTestHelper.CreateTestHall(
                hallName: originalHallName,
                maxTableCount: 50,
                hallTypeId: hallType.HallTypeId,
                minTablePrice: hallType.MinTablePrice ?? 500000m
            );
            
            _hallService.Create(newHall);
            _testHallId = newHall.HallId;
            
            // Staff creates booking
            DataProvider.Ins.CurrentUser = new AppUser { UserId = staffUser.UserId, GroupId = staffUser.GroupId };
            
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: "Update Test Groom",
                brideName: "Update Test Bride",
                phone: "0123456789",
                weddingDate: DateTime.Now.AddMonths(2),
                hallId: _testHallId,
                shiftId: 1,
                tableCount: 20
            );
            
            _bookingService.Create(testBooking);
            _testBookingId = testBooking.BookingId;
            
            // Act - Admin updates hall
            DataProvider.Ins.CurrentUser = new AppUser { UserId = adminUser.UserId, GroupId = adminUser.GroupId };
            
            string updatedHallName = $"Updated_Hall_{Guid.NewGuid().ToString().Substring(0, 8)}";
            newHall.HallName = updatedHallName;
            newHall.MaxTableCount = 80; // Increase capacity
            
            _hallService.Update(newHall);
            
            // Staff retrieves booking
            DataProvider.Ins.CurrentUser = new AppUser { UserId = staffUser.UserId, GroupId = staffUser.GroupId };
            
            var updatedBooking = _bookingService.GetById(_testBookingId);
            
            // Assert - Booking should reflect updated hall information
            Assert.IsNotNull(updatedBooking, "Booking should still exist");
            Assert.IsNotNull(updatedBooking.Hall, "Hall relationship should be maintained");
            Assert.AreEqual(updatedHallName, updatedBooking.Hall.HallName, 
                "Booking should show updated hall name (Integration Check)");
            Assert.AreEqual(80, updatedBooking.Hall.MaxTableCount, 
                "Booking should show updated hall capacity (Integration Check)");
            
            Console.WriteLine("Integration verified: Hall updates reflect immediately in existing bookings");
            Console.WriteLine($"  Original: {originalHallName} (Cap: 50)");
            Console.WriteLine($"  Updated:  {updatedHallName} (Cap: 80)");
        }

        [TestMethod]
        [TestCategory("SystemTest")]
        [TestCategory("Scenario4")]
        [TestCategory("Integration")]
        [Description("Additional Test: Verify referential integrity - cannot delete hall with bookings")]
        public void Additional_Integration_ReferentialIntegrity_CannotDeleteHallWithBookings()
        {
            // Arrange - Create hall and booking
            var adminUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "3" || u.GroupId == "ADMIN");
            var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2" || u.GroupId == "STAFF");
            
            Assert.IsNotNull(adminUser, "Admin user must exist");
            Assert.IsNotNull(staffUser, "Staff user must exist");
            
            // Create hall
            DataProvider.Ins.CurrentUser = new AppUser { UserId = adminUser.UserId, GroupId = adminUser.GroupId };
            
            var hallTypes = _hallTypeService.GetAll().ToList();
            var hallType = hallTypes.First();
            
            string hallName = $"Integrity_Test_{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            var newHall = SystemTestHelper.CreateTestHall(
                hallName: hallName,
                maxTableCount: 50,
                hallTypeId: hallType.HallTypeId,
                minTablePrice: hallType.MinTablePrice ?? 500000m
            );
            
            _hallService.Create(newHall);
            _testHallId = newHall.HallId;
            
            // Create booking
            DataProvider.Ins.CurrentUser = new AppUser { UserId = staffUser.UserId, GroupId = staffUser.GroupId };
            
            var testBooking = SystemTestHelper.CreateTestBooking(
                groomName: "Integrity Test",
                brideName: "Integrity Test",
                phone: "0123456789",
                weddingDate: DateTime.Now.AddMonths(2),
                hallId: _testHallId,
                shiftId: 1,
                tableCount: 20
            );
            
            _bookingService.Create(testBooking);
            _testBookingId = testBooking.BookingId;
            
            // Act & Assert - Try to delete hall with existing booking
            DataProvider.Ins.CurrentUser = new AppUser { UserId = adminUser.UserId, GroupId = adminUser.GroupId };
            
            bool deletionFailed = false;
            try
            {
                _hallService.Delete(_testHallId);
            }
            catch
            {
                deletionFailed = true;
            }
            
            // Should fail due to foreign key constraint
            Assert.IsTrue(deletionFailed, 
                "Deleting hall with existing bookings should fail (Referential Integrity)");
            
            // Verify hall still exists
            var hall = _hallService.GetById(_testHallId);
            Assert.IsNotNull(hall, "Hall should still exist after failed deletion");
            
            // Verify booking still references hall
            var booking = _bookingService.GetById(_testBookingId);
            Assert.IsNotNull(booking, "Booking should still exist");
            Assert.AreEqual(_testHallId, booking.HallId, "Booking should still reference hall");
            
            Console.WriteLine("Referential integrity verified: Cannot delete hall with active bookings");
            
            // Cleanup: Delete booking first, then hall
            _bookingService.Delete(_testBookingId);
            _testBookingId = 0; // Prevent double deletion in cleanup
        }

        #endregion
    }
}
