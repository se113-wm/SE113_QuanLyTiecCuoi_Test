using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation.ViewModel;

namespace QuanLyTiecCuoi.Tests.UnitTests.ViewModels
{
    /// <summary>
    /// Unit Tests for AddWeddingViewModel
    /// Covers BR137/BR138 - Staff Hall Availability and Booking Creation
    /// </summary>
    [TestClass]
    public class AddWeddingViewModelTests
    {
        private Mock<IHallService> _mockHallService;
        private Mock<IShiftService> _mockShiftService;
        private Mock<IBookingService> _mockBookingService;
        private Mock<IDishService> _mockDishService;
        private Mock<IServiceService> _mockServiceService;
        private Mock<IMenuService> _mockMenuService;
        private Mock<IServiceDetailService> _mockServiceDetailService;
        private Mock<IParameterService> _mockParameterService;

        [TestInitialize]
        public void Setup()
        {
            _mockHallService = new Mock<IHallService>();
            _mockShiftService = new Mock<IShiftService>();
            _mockBookingService = new Mock<IBookingService>();
            _mockDishService = new Mock<IDishService>();
            _mockServiceService = new Mock<IServiceService>();
            _mockMenuService = new Mock<IMenuService>();
            _mockServiceDetailService = new Mock<IServiceDetailService>();
            _mockParameterService = new Mock<IParameterService>();

            // Default setup
            _mockHallService.Setup(s => s.GetAll()).Returns(CreateSampleHalls());
            _mockShiftService.Setup(s => s.GetAll()).Returns(CreateSampleShifts());
        }

        #region BR137_001 - Constructor and Initialization Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_001: Verify AddWeddingViewModel initializes correctly")]
        public void TC_BR137_001_Constructor_InitializesCorrectly()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.HallList);
            Assert.IsNotNull(viewModel.ShiftList);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_001: Verify HallList is populated from HallService")]
        public void TC_BR137_001_HallList_IsPopulated_FromHallService()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.AreEqual(3, viewModel.HallList.Count);
            Assert.IsTrue(viewModel.HallList.Any(h => h.HallName == "S?nh Diamond"));
            Assert.IsTrue(viewModel.HallList.Any(h => h.HallName == "S?nh Gold"));
            Assert.IsTrue(viewModel.HallList.Any(h => h.HallName == "S?nh Silver"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_001: Verify ShiftList is populated from ShiftService")]
        public void TC_BR137_001_ShiftList_IsPopulated_FromShiftService()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.AreEqual(2, viewModel.ShiftList.Count);
            Assert.IsTrue(viewModel.ShiftList.Any(s => s.ShiftName == "Tr?a"));
            Assert.IsTrue(viewModel.ShiftList.Any(s => s.ShiftName == "T?i"));
        }

        #endregion

        #region BR137_002 - Calendar/Date Selection Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_002: Verify WeddingDate can be set and retrieved")]
        public void TC_BR137_002_WeddingDate_CanBeSetAndRetrieved()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var testDate = DateTime.Now.AddDays(30);

            // Act
            viewModel.WeddingDate = testDate;

            // Assert
            Assert.AreEqual(testDate, viewModel.WeddingDate);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_002: Verify UnavailableDates includes past dates")]
        public void TC_BR137_002_UnavailableDates_IncludesPastDates()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.UnavailableDates);
            Assert.IsTrue(viewModel.UnavailableDates.Count > 0);
            
            // Check that past dates are blocked (up to today)
            var blockedRange = viewModel.UnavailableDates.First();
            Assert.IsTrue(blockedRange.End >= DateTime.Today);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_002: Verify WeddingDate triggers PropertyChanged")]
        public void TC_BR137_002_WeddingDate_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "WeddingDate")
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.WeddingDate = DateTime.Now.AddDays(30);

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        #endregion

        #region BR137_003 - Shift Selection Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_003: Verify SelectedShift can be set")]
        public void TC_BR137_003_SelectedShift_CanBeSet()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var shift = viewModel.ShiftList.First();

            // Act
            viewModel.SelectedShift = shift;

            // Assert
            Assert.AreEqual(shift, viewModel.SelectedShift);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_003: Verify ShiftList contains correct shift times")]
        public void TC_BR137_003_ShiftList_ContainsCorrectShiftTimes()
        {
            // Act
            var viewModel = CreateViewModel();
            var lunchShift = viewModel.ShiftList.FirstOrDefault(s => s.ShiftName == "Tr?a");
            var eveningShift = viewModel.ShiftList.FirstOrDefault(s => s.ShiftName == "T?i");

            // Assert
            Assert.IsNotNull(lunchShift);
            Assert.AreEqual(new TimeSpan(11, 0, 0), lunchShift.StartTime);
            Assert.AreEqual(new TimeSpan(14, 0, 0), lunchShift.EndTime);

            Assert.IsNotNull(eveningShift);
            Assert.AreEqual(new TimeSpan(18, 0, 0), eveningShift.StartTime);
            Assert.AreEqual(new TimeSpan(22, 0, 0), eveningShift.EndTime);
        }

        #endregion

        #region BR137_004 - Hall Capacity Filter Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_004: Verify HallList contains halls with capacity info")]
        public void TC_BR137_004_HallList_ContainsCapacityInfo()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var hall in viewModel.HallList)
            {
                Assert.IsNotNull(hall.MaxTableCount);
                Assert.IsTrue(hall.MaxTableCount > 0);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_004: Verify SelectedHall can be set")]
        public void TC_BR137_004_SelectedHall_CanBeSet()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var hall = viewModel.HallList.First();

            // Act
            viewModel.SelectedHall = hall;

            // Assert
            Assert.AreEqual(hall, viewModel.SelectedHall);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("TC_BR137_004: Verify TableCount property works correctly")]
        public void TC_BR137_004_TableCount_CanBeSet()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.TableCount = "25";

            // Assert
            Assert.AreEqual("25", viewModel.TableCount);
        }

        #endregion

        #region BR138_002 - Hall Details Display Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_002: Verify halls include HallType information")]
        public void TC_BR138_002_Halls_IncludeHallTypeInformation()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var hall in viewModel.HallList)
            {
                Assert.IsNotNull(hall.HallType);
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallType.HallTypeName));
                Assert.IsNotNull(hall.HallType.MinTablePrice);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_002: Verify halls have pricing information")]
        public void TC_BR138_002_Halls_HavePricingInformation()
        {
            // Act
            var viewModel = CreateViewModel();
            var vipHall = viewModel.HallList.FirstOrDefault(h => h.HallType?.HallTypeName == "VIP");

            // Assert
            Assert.IsNotNull(vipHall);
            Assert.AreEqual(2000000, vipHall.HallType.MinTablePrice);
        }

        #endregion

        #region BR138_004 - Create Booking Command Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_004: Verify ConfirmCommand is initialized")]
        public void TC_BR138_004_ConfirmCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ConfirmCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_004: Verify CancelCommand is initialized")]
        public void TC_BR138_004_CancelCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.CancelCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_004: Verify ResetWeddingCommand is initialized")]
        public void TC_BR138_004_ResetWeddingCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ResetWeddingCommand);
        }

        #endregion

        #region BR138_005 - Booking Info Display Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_005: Verify customer info properties can be set")]
        public void TC_BR138_005_CustomerInfoProperties_CanBeSet()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.GroomName = "Nguy?n V?n A";
            viewModel.BrideName = "Tr?n Th? B";
            viewModel.Phone = "0901234567";

            // Assert
            Assert.AreEqual("Nguy?n V?n A", viewModel.GroomName);
            Assert.AreEqual("Tr?n Th? B", viewModel.BrideName);
            Assert.AreEqual("0901234567", viewModel.Phone);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("TC_BR138_005: Verify booking properties raise PropertyChanged")]
        public void TC_BR138_005_BookingProperties_RaisePropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var changedProperties = new List<string>();
            viewModel.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName);

            // Act
            viewModel.GroomName = "Test";
            viewModel.BrideName = "Test";
            viewModel.Phone = "Test";
            viewModel.Deposit = "1000000";

            // Assert
            Assert.IsTrue(changedProperties.Contains("GroomName"));
            Assert.IsTrue(changedProperties.Contains("BrideName"));
            Assert.IsTrue(changedProperties.Contains("Phone"));
            Assert.IsTrue(changedProperties.Contains("Deposit"));
        }

        #endregion

        #region Menu and Service Tests (Related to Booking)

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("Verify MenuList is initialized as empty")]
        public void MenuList_IsInitialized_AsEmpty()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.MenuList);
            Assert.AreEqual(0, viewModel.MenuList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("Verify ServiceList is initialized as empty")]
        public void ServiceList_IsInitialized_AsEmpty()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ServiceList);
            Assert.AreEqual(0, viewModel.ServiceList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("Verify menu commands are initialized")]
        public void MenuCommands_AreInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.AddMenuCommand);
            Assert.IsNotNull(viewModel.EditMenuCommand);
            Assert.IsNotNull(viewModel.DeleteMenuCommand);
            Assert.IsNotNull(viewModel.SelectDishCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("Verify service commands are initialized")]
        public void ServiceCommands_AreInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.AddServiceCommand);
            Assert.IsNotNull(viewModel.EditServiceCommand);
            Assert.IsNotNull(viewModel.DeleteServiceCommand);
            Assert.IsNotNull(viewModel.SelectServiceCommand);
        }

        #endregion

        #region Reset Functionality Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("Verify ResetWeddingCommand clears all wedding fields")]
        public void ResetWeddingCommand_ClearsAllFields()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.GroomName = "Test";
            viewModel.BrideName = "Test";
            viewModel.Phone = "0901234567";
            viewModel.WeddingDate = DateTime.Now.AddDays(30);
            viewModel.SelectedShift = viewModel.ShiftList.First();
            viewModel.SelectedHall = viewModel.HallList.First();

            // Act
            viewModel.ResetWeddingCommand.Execute(null);

            // Assert
            Assert.AreEqual(string.Empty, viewModel.GroomName);
            Assert.AreEqual(string.Empty, viewModel.BrideName);
            Assert.AreEqual(string.Empty, viewModel.Phone);
            Assert.IsNull(viewModel.WeddingDate);
            Assert.IsNull(viewModel.SelectedShift);
            Assert.IsNull(viewModel.SelectedHall);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("Verify ResetMenuCommand clears menu fields")]
        public void ResetMenuCommand_ClearsMenuFields()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.MenuQuantity = "5";
            viewModel.MenuNote = "Test Note";

            // Act
            viewModel.ResetMenuCommand.Execute(null);

            // Assert
            Assert.AreEqual(string.Empty, viewModel.MenuQuantity);
            Assert.AreEqual(string.Empty, viewModel.MenuNote);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("Verify ResetServiceCommand clears service fields")]
        public void ResetServiceCommand_ClearsServiceFields()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.ServiceQuantity = "3";
            viewModel.ServiceNote = "Service Note";

            // Act
            viewModel.ResetServiceCommand.Execute(null);

            // Assert
            Assert.AreEqual(string.Empty, viewModel.ServiceQuantity);
            Assert.AreEqual(string.Empty, viewModel.ServiceNote);
        }

        #endregion

        #region Total Calculation Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("Verify MenuTotal starts at 0")]
        public void MenuTotal_StartsAtZero()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.AreEqual(0, viewModel.MenuTotal);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR138")]
        [Description("Verify ServiceTotal starts at 0")]
        public void ServiceTotal_StartsAtZero()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.AreEqual(0, viewModel.ServiceTotal);
        }

        #endregion

        #region Property Change Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("Verify SelectedHall raises PropertyChanged")]
        public void SelectedHall_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "SelectedHall")
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedHall = viewModel.HallList.First();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AddWeddingViewModel")]
        [TestCategory("BR137")]
        [Description("Verify SelectedShift raises PropertyChanged")]
        public void SelectedShift_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "SelectedShift")
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedShift = viewModel.ShiftList.First();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        #endregion

        #region Helper Methods

        private AddWeddingViewModel CreateViewModel()
        {
            return new AddWeddingViewModel(
                _mockHallService.Object,
                _mockShiftService.Object,
                _mockBookingService.Object,
                _mockDishService.Object,
                _mockServiceService.Object,
                _mockMenuService.Object,
                _mockServiceDetailService.Object,
                _mockParameterService.Object);
        }

        private List<HallDTO> CreateSampleHalls()
        {
            return new List<HallDTO>
            {
                new HallDTO
                {
                    HallId = 1,
                    HallName = "S?nh Diamond",
                    MaxTableCount = 50,
                    HallTypeId = 1,
                    HallType = new HallTypeDTO
                    {
                        HallTypeId = 1,
                        HallTypeName = "VIP",
                        MinTablePrice = 2000000
                    }
                },
                new HallDTO
                {
                    HallId = 2,
                    HallName = "S?nh Gold",
                    MaxTableCount = 40,
                    HallTypeId = 2,
                    HallType = new HallTypeDTO
                    {
                        HallTypeId = 2,
                        HallTypeName = "Standard",
                        MinTablePrice = 1500000
                    }
                },
                new HallDTO
                {
                    HallId = 3,
                    HallName = "S?nh Silver",
                    MaxTableCount = 30,
                    HallTypeId = 2,
                    HallType = new HallTypeDTO
                    {
                        HallTypeId = 2,
                        HallTypeName = "Standard",
                        MinTablePrice = 1500000
                    }
                }
            };
        }

        private List<ShiftDTO> CreateSampleShifts()
        {
            return new List<ShiftDTO>
            {
                new ShiftDTO
                {
                    ShiftId = 1,
                    ShiftName = "Tr?a",
                    StartTime = new TimeSpan(11, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0)
                },
                new ShiftDTO
                {
                    ShiftId = 2,
                    ShiftName = "T?i",
                    StartTime = new TimeSpan(18, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0)
                }
            };
        }

        #endregion
    }
}
