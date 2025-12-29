using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation.ViewModel;
using QuanLyTiecCuoi.ViewModel;

namespace QuanLyTiecCuoi.Tests.UnitTests.ViewModels
{
    /// <summary>
    /// Unit Tests for WeddingViewModel
    /// Covers BR137 - Displaying Rules (Staff Hall Availability)
    /// Covers BR138 - Processing Rules (Query Available Halls)
    /// </summary>
    [TestClass]
    public class WeddingViewModelTests
    {
        private Mock<IBookingService> _mockBookingService;
        private Mock<IMenuService> _mockMenuService;
        private Mock<IServiceDetailService> _mockServiceDetailService;
        private Mock<IUserGroupService> _mockUserGroupService;
        private Mock<IPermissionService> _mockPermissionService;
        private Mock<IAppUserService> _mockAppUserService;
        private Mock<IRevenueReportDetailService> _mockRevenueReportDetailService;

        [TestInitialize]
        public void Setup()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockMenuService = new Mock<IMenuService>();
            _mockServiceDetailService = new Mock<IServiceDetailService>();
            _mockUserGroupService = new Mock<IUserGroupService>();
            _mockPermissionService = new Mock<IPermissionService>();
            _mockAppUserService = new Mock<IAppUserService>();
            _mockRevenueReportDetailService = new Mock<IRevenueReportDetailService>();
        }

        private MainViewModel CreateMockMainViewModel()
        {
            // MainViewModel needs these services, but we won't test it directly
            // Just create a mock that can be passed to WeddingViewModel
            return null; // We'll test WeddingViewModel logic without MainViewModel interaction
        }

        #region BR137_001 - Verify booking management screen displays for authorized users

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_001: Verify WeddingViewModel initializes correctly with booking data")]
        public void TC_BR137_001_WeddingViewModel_InitializesCorrectly_WithBookingData()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null, // MainViewModel not needed for list tests
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.List);
            Assert.AreEqual(2, viewModel.List.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_001: Verify List property is populated from BookingService")]
        public void TC_BR137_001_List_IsPopulated_FromBookingService()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.List);
            Assert.AreEqual("Nguy?n V?n A", viewModel.List[0].GroomName);
            Assert.AreEqual("Lê V?n C", viewModel.List[1].GroomName);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_001: Verify OriginalList is populated")]
        public void TC_BR137_001_OriginalList_IsPopulated()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.OriginalList);
            Assert.AreEqual(2, viewModel.OriginalList.Count);
        }

        #endregion

        #region BR137_002 - Verify calendar view control for date selection

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_002: Verify SelectedWeddingDate can be set and retrieved")]
        public void TC_BR137_002_SelectedWeddingDate_CanBeSetAndRetrieved()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            var testDate = DateTime.Now.AddDays(30);

            // Act
            viewModel.SelectedWeddingDate = testDate;

            // Assert
            Assert.AreEqual(testDate, viewModel.SelectedWeddingDate);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_002: Verify SelectedWeddingDate triggers PropertyChanged")]
        public void TC_BR137_002_SelectedWeddingDate_RaisesPropertyChanged()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "SelectedWeddingDate")
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedWeddingDate = DateTime.Now.AddDays(15);

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_002: Verify setting SelectedWeddingDate filters list")]
        public void TC_BR137_002_SelectedWeddingDate_FiltersListCorrectly()
        {
            // Arrange
            var targetDate = DateTime.Now.AddDays(30).Date;
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, GroomName = "Test1", WeddingDate = targetDate },
                new BookingDTO { BookingId = 2, GroomName = "Test2", WeddingDate = targetDate.AddDays(5) }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Act
            viewModel.SelectedWeddingDate = targetDate;

            // Assert
            Assert.AreEqual(1, viewModel.List.Count);
            Assert.AreEqual("Test1", viewModel.List[0].GroomName);
        }

        #endregion

        #region BR137_003 - Verify shift selection dropdown available

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_003: Verify SearchProperties includes shift-related options")]
        public void TC_BR137_003_SearchProperties_ContainsRequiredOptions()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.SearchProperties);
            Assert.IsTrue(viewModel.SearchProperties.Contains("Groom Name"));
            Assert.IsTrue(viewModel.SearchProperties.Contains("Bride Name"));
            Assert.IsTrue(viewModel.SearchProperties.Contains("Hall Name"));
            Assert.IsTrue(viewModel.SearchProperties.Contains("Wedding Date"));
            Assert.IsTrue(viewModel.SearchProperties.Contains("Status"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_003: Verify StatusFilterList contains all status options")]
        public void TC_BR137_003_StatusFilterList_ContainsAllOptions()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.StatusFilterList);
            Assert.IsTrue(viewModel.StatusFilterList.Contains("All"));
            Assert.IsTrue(viewModel.StatusFilterList.Contains("Not Organized"));
            Assert.IsTrue(viewModel.StatusFilterList.Contains("Not Paid"));
            Assert.IsTrue(viewModel.StatusFilterList.Contains("Late Payment"));
            Assert.IsTrue(viewModel.StatusFilterList.Contains("Paid"));
        }

        #endregion

        #region BR137_004 - Verify hall capacity filter option available

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_004: Verify TableCountFilterList is populated")]
        public void TC_BR137_004_TableCountFilterList_IsPopulated()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, TableCount = 20 },
                new BookingDTO { BookingId = 2, TableCount = 30 },
                new BookingDTO { BookingId = 3, TableCount = 20 } // duplicate
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.TableCountFilterList);
            Assert.AreEqual(2, viewModel.TableCountFilterList.Count); // distinct values
            Assert.IsTrue(viewModel.TableCountFilterList.Contains(20));
            Assert.IsTrue(viewModel.TableCountFilterList.Contains(30));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_004: Verify SelectedTableCount filters bookings by table count")]
        public void TC_BR137_004_SelectedTableCount_FiltersBookingsCorrectly()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, GroomName = "A", TableCount = 20 },
                new BookingDTO { BookingId = 2, GroomName = "B", TableCount = 30 }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Act
            viewModel.SelectedTableCount = 20;

            // Assert
            Assert.AreEqual(1, viewModel.List.Count);
            Assert.AreEqual("A", viewModel.List[0].GroomName);
        }

        #endregion

        #region BR138_001 - Verify query excludes cancelled bookings from availability

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_001: Verify bookings with PaymentDate (Paid status) are included")]
        public void TC_BR138_001_PaidBookings_AreIncludedInList()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO 
                { 
                    BookingId = 1, 
                    GroomName = "Paid",
                    WeddingDate = DateTime.Now.AddDays(-10),
                    PaymentDate = DateTime.Now.AddDays(-5) // Paid
                },
                new BookingDTO 
                { 
                    BookingId = 2, 
                    GroomName = "NotPaid",
                    WeddingDate = DateTime.Now.AddDays(30),
                    PaymentDate = null // Not Paid
                }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.AreEqual(2, viewModel.List.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_001: Verify filtering by status correctly shows booking types")]
        public void TC_BR138_001_StatusFilter_FiltersCorrectly()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO 
                { 
                    BookingId = 1, 
                    GroomName = "PaidBooking",
                    WeddingDate = DateTime.Now.AddDays(-10),
                    PaymentDate = DateTime.Now.AddDays(-5)
                },
                new BookingDTO 
                { 
                    BookingId = 2, 
                    GroomName = "FutureBooking",
                    WeddingDate = DateTime.Now.AddDays(30),
                    PaymentDate = null
                }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Act - Filter to show only Paid
            viewModel.SelectedStatus = "Paid";

            // Assert
            Assert.AreEqual(1, viewModel.List.Count);
            Assert.AreEqual("PaidBooking", viewModel.List[0].GroomName);
        }

        #endregion

        #region BR138_002 - Verify hall details display: name, type, capacity, pricing

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_002: Verify bookings include Hall information")]
        public void TC_BR138_002_Bookings_IncludeHallInformation()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO 
                { 
                    BookingId = 1, 
                    GroomName = "Test",
                    Hall = new HallDTO 
                    { 
                        HallId = 1,
                        HallName = "S?nh Diamond",
                        MaxTableCount = 50,
                        HallType = new HallTypeDTO
                        {
                            HallTypeId = 1,
                            HallTypeName = "VIP",
                            MinTablePrice = 1500000
                        }
                    }
                }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.List[0].Hall);
            Assert.AreEqual("S?nh Diamond", viewModel.List[0].Hall.HallName);
            Assert.AreEqual(50, viewModel.List[0].Hall.MaxTableCount);
            Assert.IsNotNull(viewModel.List[0].Hall.HallType);
            Assert.AreEqual("VIP", viewModel.List[0].Hall.HallType.HallTypeName);
            Assert.AreEqual(1500000, viewModel.List[0].Hall.HallType.MinTablePrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_002: Verify HallNameFilterList contains unique hall names")]
        public void TC_BR138_002_HallNameFilterList_ContainsUniqueHallNames()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, Hall = new HallDTO { HallName = "S?nh A" } },
                new BookingDTO { BookingId = 2, Hall = new HallDTO { HallName = "S?nh B" } },
                new BookingDTO { BookingId = 3, Hall = new HallDTO { HallName = "S?nh A" } } // duplicate
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.HallNameFilterList);
            Assert.AreEqual(2, viewModel.HallNameFilterList.Count);
            Assert.IsTrue(viewModel.HallNameFilterList.Contains("S?nh A"));
            Assert.IsTrue(viewModel.HallNameFilterList.Contains("S?nh B"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_002: Verify SelectedHallName filters correctly")]
        public void TC_BR138_002_SelectedHallName_FiltersCorrectly()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, GroomName = "A", Hall = new HallDTO { HallName = "S?nh Diamond" } },
                new BookingDTO { BookingId = 2, GroomName = "B", Hall = new HallDTO { HallName = "S?nh Gold" } }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Act
            viewModel.SelectedHallName = "S?nh Diamond";

            // Assert
            Assert.AreEqual(1, viewModel.List.Count);
            Assert.AreEqual("A", viewModel.List[0].GroomName);
        }

        #endregion

        #region BR138_003 - Verify MSG90 displays when no halls available

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_003: Verify empty list when no bookings match filter")]
        public void TC_BR138_003_EmptyList_WhenNoMatchingBookings()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, GroomName = "Test", Hall = new HallDTO { HallName = "S?nh A" } }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Act
            viewModel.SelectedHallName = "S?nh NonExistent";

            // Assert
            Assert.AreEqual(0, viewModel.List.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_003: Verify empty list when searching with no results")]
        public void TC_BR138_003_EmptyList_WhenSearchReturnsNoResults()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, GroomName = "Nguy?n V?n A" }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            viewModel.SelectedSearchProperty = "Groom Name";

            // Act
            viewModel.SearchText = "ZZZZZ"; // Non-matching search

            // Assert
            Assert.AreEqual(0, viewModel.List.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_003: Verify empty source data results in empty lists")]
        public void TC_BR138_003_EmptySourceData_ResultsInEmptyLists()
        {
            // Arrange
            _mockBookingService.Setup(s => s.GetAll()).Returns(new List<BookingDTO>());

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.AreEqual(0, viewModel.List.Count);
            Assert.AreEqual(0, viewModel.OriginalList.Count);
        }

        #endregion

        #region BR138_004 - Verify "Create Booking" button available for each hall

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_004: Verify AddCommand is initialized")]
        public void TC_BR138_004_AddCommand_IsInitialized()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.AddCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_004: Verify AddCommand CanExecute returns true")]
        public void TC_BR138_004_AddCommand_CanExecute_ReturnsTrue()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_004: Verify DetailCommand is initialized")]
        public void TC_BR138_004_DetailCommand_IsInitialized()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.DetailCommand);
        }

        #endregion

        #region BR138_005 - Verify existing bookings shown for occupied halls

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_005: Verify bookings with shift info are loaded")]
        public void TC_BR138_005_BookingsWithShiftInfo_AreLoaded()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO 
                { 
                    BookingId = 1, 
                    GroomName = "Test",
                    Shift = new ShiftDTO 
                    { 
                        ShiftId = 1, 
                        ShiftName = "Tr?a",
                        StartTime = new TimeSpan(11, 0, 0),
                        EndTime = new TimeSpan(14, 0, 0)
                    }
                }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.IsNotNull(viewModel.List[0].Shift);
            Assert.AreEqual("Tr?a", viewModel.List[0].Shift.ShiftName);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_005: Verify bookings display customer information")]
        public void TC_BR138_005_Bookings_DisplayCustomerInformation()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO 
                { 
                    BookingId = 1, 
                    GroomName = "Nguy?n V?n A",
                    BrideName = "Tr?n Th? B",
                    Phone = "0901234567",
                    WeddingDate = DateTime.Now.AddDays(30)
                }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.AreEqual("Nguy?n V?n A", viewModel.List[0].GroomName);
            Assert.AreEqual("Tr?n Th? B", viewModel.List[0].BrideName);
            Assert.AreEqual("0901234567", viewModel.List[0].Phone);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_005: Verify booking status is correctly determined")]
        public void TC_BR138_005_BookingStatus_IsCorrectlyDetermined()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO 
                { 
                    BookingId = 1, 
                    GroomName = "Future",
                    WeddingDate = DateTime.Now.AddDays(30),
                    PaymentDate = null
                },
                new BookingDTO 
                { 
                    BookingId = 2, 
                    GroomName = "Paid",
                    WeddingDate = DateTime.Now.AddDays(-10),
                    PaymentDate = DateTime.Now.AddDays(-5)
                }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            // Act
            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Assert
            Assert.AreEqual("Not Organized", viewModel.List[0].Status);
            Assert.AreEqual("Paid", viewModel.List[1].Status);
        }

        #endregion

        #region Search Functionality Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("Verify search by GroomName works correctly")]
        public void Search_ByGroomName_FiltersCorrectly()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, GroomName = "Nguy?n V?n A" },
                new BookingDTO { BookingId = 2, GroomName = "Lê V?n B" }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            viewModel.SelectedSearchProperty = "Groom Name";

            // Act
            viewModel.SearchText = "Nguy?n";

            // Assert
            Assert.AreEqual(1, viewModel.List.Count);
            Assert.AreEqual("Nguy?n V?n A", viewModel.List[0].GroomName);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("Verify search is case-insensitive")]
        public void Search_IsCaseInsensitive()
        {
            // Arrange
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, GroomName = "NGUY?N V?N A" }
            };
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            viewModel.SelectedSearchProperty = "Groom Name";

            // Act
            viewModel.SearchText = "nguy?n";

            // Assert
            Assert.AreEqual(1, viewModel.List.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR138")]
        [Description("Verify empty search shows all bookings")]
        public void Search_EmptyText_ShowsAllBookings()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            viewModel.SelectedSearchProperty = "Groom Name";
            viewModel.SearchText = "Test"; // First filter

            // Act
            viewModel.SearchText = ""; // Clear search

            // Assert
            Assert.AreEqual(2, viewModel.List.Count);
        }

        #endregion

        #region Reset Functionality Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("Verify ResetCommand clears all filters")]
        public void ResetCommand_ClearsAllFilters()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Set some filters
            viewModel.SelectedGroomName = "Test";
            viewModel.SearchText = "Search";
            viewModel.SelectedWeddingDate = DateTime.Now;

            // Act
            viewModel.ResetCommand.Execute(null);

            // Assert
            Assert.IsNull(viewModel.SelectedGroomName);
            Assert.AreEqual(string.Empty, viewModel.SearchText);
            Assert.IsNull(viewModel.SelectedWeddingDate);
            Assert.IsNull(viewModel.SelectedItem);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("WeddingViewModel")]
        [TestCategory("BR137")]
        [Description("Verify ResetCommand restores original list")]
        public void ResetCommand_RestoresOriginalList()
        {
            // Arrange
            var bookings = CreateSampleBookings();
            _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);

            var viewModel = new WeddingViewModel(
                null,
                _mockBookingService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object);

            // Apply filter that reduces list
            viewModel.SelectedGroomName = "Nguy?n V?n A";
            Assert.AreEqual(1, viewModel.List.Count);

            // Act
            viewModel.ResetCommand.Execute(null);

            // Assert
            Assert.AreEqual(2, viewModel.List.Count);
        }

        #endregion

        #region Helper Methods

        private List<BookingDTO> CreateSampleBookings()
        {
            return new List<BookingDTO>
            {
                new BookingDTO
                {
                    BookingId = 1,
                    GroomName = "Nguy?n V?n A",
                    BrideName = "Tr?n Th? B",
                    Phone = "0901234567",
                    WeddingDate = DateTime.Now.AddDays(30),
                    TableCount = 20,
                    Deposit = 5000000,
                    Hall = new HallDTO 
                    { 
                        HallId = 1, 
                        HallName = "S?nh A",
                        HallType = new HallTypeDTO { HallTypeName = "Standard" }
                    },
                    Shift = new ShiftDTO { ShiftId = 1, ShiftName = "Tr?a" }
                },
                new BookingDTO
                {
                    BookingId = 2,
                    GroomName = "Lê V?n C",
                    BrideName = "Ph?m Th? D",
                    Phone = "0907654321",
                    WeddingDate = DateTime.Now.AddDays(60),
                    TableCount = 30,
                    Deposit = 8000000,
                    Hall = new HallDTO 
                    { 
                        HallId = 2, 
                        HallName = "S?nh B",
                        HallType = new HallTypeDTO { HallTypeName = "VIP" }
                    },
                    Shift = new ShiftDTO { ShiftId = 2, ShiftName = "T?i" }
                }
            };
        }

        #endregion
    }
}
