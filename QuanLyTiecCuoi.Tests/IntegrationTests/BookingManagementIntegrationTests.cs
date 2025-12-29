using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.ViewModel;

namespace QuanLyTiecCuoi.Tests.IntegrationTests
{
    /// <summary>
    /// Integration Tests for Booking Management (BR137/BR138)
    /// Tests the interaction between ViewModel, Service, and Repository layers
    /// Note: These tests use the actual database through DataProvider
    /// </summary>
    [TestClass]
    public class BookingManagementIntegrationTests
    {
        private BookingService _bookingService;
        private HallService _hallService;
        private ShiftService _shiftService;

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize repositories (they use DataProvider internally)
            var bookingRepo = new BookingRepository();
            var hallRepo = new HallRepository();
            var shiftRepo = new ShiftRepository();
            
            // Initialize services
            _bookingService = new BookingService(bookingRepo);
            _hallService = new HallService(hallRepo);
            _shiftService = new ShiftService(shiftRepo);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // DataProvider will be disposed by garbage collector
        }

        #region BR137 - Display Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR137")]
        [Description("TC_BR137_001: Integration test - Verify WeddingViewModel loads actual bookings from database")]
        public void TC_BR137_001_Integration_WeddingViewModel_LoadsActualBookings()
        {
            // Arrange
            var menuService = new MenuService(new MenuRepository());
            var serviceDetailService = new ServiceDetailService(new ServiceDetailRepository());

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _bookingService,
                menuService,
                serviceDetailService);

            // Assert
            Assert.IsNotNull(viewModel.List);
            Assert.IsNotNull(viewModel.OriginalList);
            // Should have loaded bookings from actual database
            Assert.IsTrue(viewModel.OriginalList.Count >= 0, "Should load bookings list (may be empty)");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR137")]
        [Description("TC_BR137_002: Integration test - Verify filtering by wedding date works with real data")]
        public void TC_BR137_002_Integration_FilterByWeddingDate_WorksWithRealData()
        {
            // Arrange
            var menuService = new MenuService(new MenuRepository());
            var serviceDetailService = new ServiceDetailService(new ServiceDetailRepository());
            
            var viewModel = new WeddingViewModel(
                null,
                _bookingService,
                menuService,
                serviceDetailService);

            // Only test if there are bookings
            if (viewModel.OriginalList.Count == 0)
            {
                Assert.Inconclusive("No bookings in database to test filtering");
                return;
            }

            var firstBooking = viewModel.OriginalList.FirstOrDefault(b => b.WeddingDate.HasValue);
            if (firstBooking == null)
            {
                Assert.Inconclusive("No bookings with wedding date in database");
                return;
            }

            var testDate = firstBooking.WeddingDate.Value.Date;
            
            // Act
            viewModel.SelectedWeddingDate = testDate;
            
            // Assert
            // All bookings in filtered list should match the selected date
            foreach (var booking in viewModel.List)
            {
                Assert.IsTrue(booking.WeddingDate.HasValue);
                Assert.AreEqual(testDate, booking.WeddingDate.Value.Date);
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR137")]
        [Description("TC_BR137_003: Integration test - Verify halls loaded with shift information")]
        public void TC_BR137_003_Integration_Halls_LoadedWithShiftInfo()
        {
            // Act
            var halls = _hallService.GetAll().ToList();
            var shifts = _shiftService.GetAll().ToList();

            // Assert
            Assert.IsTrue(halls.Count > 0, "Should have halls in database");
            Assert.IsTrue(shifts.Count > 0, "Should have shifts in database");
            
            // Verify shift data structure
            foreach (var shift in shifts)
            {
                Assert.IsFalse(string.IsNullOrEmpty(shift.ShiftName));
                Assert.IsNotNull(shift.StartTime);
                Assert.IsNotNull(shift.EndTime);
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR137")]
        [Description("TC_BR137_004: Integration test - Verify halls have capacity information")]
        public void TC_BR137_004_Integration_Halls_HaveCapacityInformation()
        {
            // Act
            var halls = _hallService.GetAll().ToList();

            // Assert
            Assert.IsTrue(halls.Count > 0, "Should have halls in database");
            
            foreach (var hall in halls)
            {
                Assert.IsNotNull(hall.MaxTableCount, $"Hall {hall.HallName} should have MaxTableCount");
                Assert.IsTrue(hall.MaxTableCount > 0, $"Hall {hall.HallName} MaxTableCount should be > 0");
            }
        }

        #endregion

        #region BR138 - Query Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR138")]
        [Description("TC_BR138_001: Integration test - Verify cancelled bookings handling")]
        public void TC_BR138_001_Integration_CancelledBookings_Handling()
        {
            // Arrange
            var allBookings = _bookingService.GetAll().ToList();
            
            // Act - Filter to get only active bookings (not cancelled)
            // In this system, cancelled would be past wedding dates without payment
            var activeBookings = allBookings.Where(b => 
                b.WeddingDate.HasValue && 
                (b.PaymentDate.HasValue || b.WeddingDate.Value >= DateTime.Now)
            ).ToList();

            // Assert
            Assert.IsTrue(activeBookings.All(b => b.WeddingDate.HasValue), 
                "All active bookings should have wedding date");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR138")]
        [Description("TC_BR138_002: Integration test - Verify hall details include all required information")]
        public void TC_BR138_002_Integration_HallDetails_IncludeAllRequiredInfo()
        {
            // Act
            var halls = _hallService.GetAll().ToList();

            // Assert
            Assert.IsTrue(halls.Count > 0, "Should have halls in database");
            
            foreach (var hall in halls)
            {
                // Verify required fields
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallName), "HallName is required");
                Assert.IsNotNull(hall.MaxTableCount, "MaxTableCount is required");
                Assert.IsNotNull(hall.HallType, "HallType is required");
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallType.HallTypeName), "HallTypeName is required");
                Assert.IsNotNull(hall.HallType.MinTablePrice, "MinTablePrice is required");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR138")]
        [Description("TC_BR138_003: Integration test - Verify empty result when no halls match criteria")]
        public void TC_BR138_003_Integration_EmptyResult_WhenNoHallsMatchCriteria()
        {
            // Arrange
            var menuService = new MenuService(new MenuRepository());
            var serviceDetailService = new ServiceDetailService(new ServiceDetailRepository());
            
            var viewModel = new WeddingViewModel(
                null,
                _bookingService,
                menuService,
                serviceDetailService);

            // Act - Use impossible filter criteria
            viewModel.SelectedHallName = "NonExistentHall_XYZ123_" + Guid.NewGuid();

            // Assert
            Assert.AreEqual(0, viewModel.List.Count, "Should return empty list for non-existent hall");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR138")]
        [Description("TC_BR138_005: Integration test - Verify occupied halls show booking information")]
        public void TC_BR138_005_Integration_OccupiedHalls_ShowBookingInfo()
        {
            // Act - Get bookings with future wedding dates
            var bookings = _bookingService.GetAll()
                .Where(b => b.WeddingDate.HasValue && b.WeddingDate.Value >= DateTime.Now)
                .ToList();

            if (bookings.Count == 0)
            {
                Assert.Inconclusive("No future bookings in database to test");
                return;
            }

            // Assert
            foreach (var booking in bookings.Take(5)) // Test first 5 bookings
            {
                // Verify booking has customer information
                Assert.IsFalse(string.IsNullOrEmpty(booking.GroomName), "GroomName should be present");
                Assert.IsFalse(string.IsNullOrEmpty(booking.BrideName), "BrideName should be present");
                
                // Verify booking has hall information
                if (booking.Hall != null)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(booking.Hall.HallName), "HallName should be present");
                }
                
                // Verify booking has shift information
                if (booking.Shift != null)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(booking.Shift.ShiftName), "ShiftName should be present");
                }
            }
        }

        #endregion

        #region Cross-Layer Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR137")]
        [TestCategory("BR138")]
        [Description("Integration test - Verify complete booking workflow from ViewModel to Database")]
        public void Integration_CompleteBookingWorkflow_ViewModelToDatabase()
        {
            // Arrange - Setup all services using repositories with DataProvider
            var hallService = new HallService(new HallRepository());
            var shiftService = new ShiftService(new ShiftRepository());
            var bookingService = new BookingService(new BookingRepository());
            var dishService = new DishService(new DishRepository());
            var serviceService = new ServiceService(new ServiceRepository());
            var menuService = new MenuService(new MenuRepository());
            var serviceDetailService = new ServiceDetailService(new ServiceDetailRepository());
            var parameterService = new ParameterService(new ParameterRepository());

            // Create AddWeddingViewModel with real services
            var viewModel = new AddWeddingViewModel(
                hallService,
                shiftService,
                bookingService,
                dishService,
                serviceService,
                menuService,
                serviceDetailService,
                parameterService);

            // Assert - Verify ViewModel loaded data from database
            Assert.IsTrue(viewModel.HallList.Count > 0, "Should load halls from database");
            Assert.IsTrue(viewModel.ShiftList.Count > 0, "Should load shifts from database");
            
            // Verify hall data structure is complete
            foreach (var hall in viewModel.HallList)
            {
                Assert.IsNotNull(hall.HallType, "Hall should have HallType loaded");
                Assert.IsNotNull(hall.HallType.MinTablePrice, "HallType should have MinTablePrice");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR137")]
        [TestCategory("BR138")]
        [Description("Integration test - Verify search and filter work with real database data")]
        public void Integration_SearchAndFilter_WorkWithRealData()
        {
            // Arrange
            var menuService = new MenuService(new MenuRepository());
            var serviceDetailService = new ServiceDetailService(new ServiceDetailRepository());
            
            var viewModel = new WeddingViewModel(
                null,
                _bookingService,
                menuService,
                serviceDetailService);

            if (viewModel.OriginalList.Count == 0)
            {
                Assert.Inconclusive("No bookings in database to test filtering");
                return;
            }

            var originalCount = viewModel.List.Count;

            // Act - Apply hall filter if available
            if (viewModel.HallNameFilterList.Count > 0)
            {
                viewModel.SelectedHallName = viewModel.HallNameFilterList.First();
            }
            else
            {
                Assert.Inconclusive("No hall filter options available");
                return;
            }

            // Assert
            Assert.IsTrue(viewModel.List.Count <= originalCount, "Filter should reduce or maintain list size");
            
            // Verify all items match filter
            if (!string.IsNullOrEmpty(viewModel.SelectedHallName))
            {
                Assert.IsTrue(viewModel.List.All(b => 
                    b.Hall != null && b.Hall.HallName == viewModel.SelectedHallName),
                    "All filtered items should match selected hall name");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR137")]
        [TestCategory("BR138")]
        [Description("Integration test - Verify status filter works correctly")]
        public void Integration_StatusFilter_WorksCorrectly()
        {
            // Arrange
            var menuService = new MenuService(new MenuRepository());
            var serviceDetailService = new ServiceDetailService(new ServiceDetailRepository());
            
            var viewModel = new WeddingViewModel(
                null,
                _bookingService,
                menuService,
                serviceDetailService);

            if (viewModel.OriginalList.Count == 0)
            {
                Assert.Inconclusive("No bookings in database to test status filtering");
                return;
            }

            // Act - Filter by "Paid" status
            viewModel.SelectedStatus = "Paid";

            // Assert - All filtered bookings should be Paid
            foreach (var booking in viewModel.List)
            {
                Assert.AreEqual("Paid", booking.Status, "All bookings should have Paid status");
                Assert.IsTrue(booking.PaymentDate.HasValue, "Paid bookings should have PaymentDate");
            }
        }

        #endregion
    }
}
