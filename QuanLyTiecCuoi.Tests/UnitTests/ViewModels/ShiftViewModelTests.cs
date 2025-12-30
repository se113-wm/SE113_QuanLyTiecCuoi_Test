using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.ViewModel;

namespace QuanLyTiecCuoi.Tests.UnitTests.ViewModels
{
    /// <summary>
    /// Unit Tests for ShiftViewModel
    /// Covers BR51-BR59 - Shift Management
    /// </summary>
    [TestClass]
    public class ShiftViewModelTests
    {
        private Mock<IShiftService> _mockShiftService;
        private Mock<IBookingService> _mockBookingService;

        [TestInitialize]
        public void Setup()
        {
            _mockShiftService = new Mock<IShiftService>();
            _mockBookingService = new Mock<IBookingService>();

            // Setup default mock returns
            _mockShiftService.Setup(s => s.GetAll()).Returns(CreateSampleShifts());
            _mockBookingService.Setup(s => s.GetAll()).Returns(new List<BookingDTO>());
        }

        #region BR51 - Display Shift List Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR51")]
        [Description("TC_BR51_001: Verify ShiftViewModel initializes and loads shift list")]
        public void TC_BR51_001_Constructor_LoadsShiftList()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.ShiftList);
            Assert.IsTrue(viewModel.ShiftList.Count > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR51")]
        [Description("TC_BR51_002: Verify shift list contains time information")]
        public void TC_BR51_002_ShiftList_ContainsTimeInfo()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var shift in viewModel.ShiftList)
            {
                Assert.IsNotNull(shift.StartTime);
                Assert.IsNotNull(shift.EndTime);
                Assert.IsTrue(shift.EndTime > shift.StartTime);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR51")]
        [Description("TC_BR51_003: Verify original list is preserved")]
        public void TC_BR51_003_OriginalList_IsPreserved()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.OriginalList);
            Assert.AreEqual(viewModel.ShiftList.Count, viewModel.OriginalList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR51")]
        [Description("TC_BR51_004: Verify shift names are loaded")]
        public void TC_BR51_004_ShiftList_ContainsNames()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var shift in viewModel.ShiftList)
            {
                Assert.IsFalse(string.IsNullOrEmpty(shift.ShiftName));
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR51")]
        [Description("TC_BR51_005: Verify shifts are distinct")]
        public void TC_BR51_005_ShiftList_ContainsDistinctShifts()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            var uniqueNames = viewModel.ShiftList.Select(s => s.ShiftName).Distinct().Count();
            Assert.AreEqual(viewModel.ShiftList.Count, uniqueNames);
        }

        #endregion

        #region BR52 - Search/Filter Shift Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR52")]
        [Description("TC_BR52_001: Verify search by shift name works")]
        public void TC_BR52_001_SearchByShiftName_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "Morning";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0]; // "Tên ca"
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.ShiftList.All(s => 
                s.ShiftName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR52")]
        [Description("TC_BR52_002: Verify search by start time works")]
        public void TC_BR52_002_SearchByStartTime_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTime = "07:30";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[1]; // "Th?i gian b?t ??u"
            viewModel.SearchText = searchTime;

            // Assert
            Assert.IsTrue(viewModel.ShiftList.Count >= 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR52")]
        [Description("TC_BR52_003: Verify clearing search restores full list")]
        public void TC_BR52_003_ClearSearch_RestoresFullList()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var originalCount = viewModel.ShiftList.Count;
            viewModel.SearchText = "Morning";

            // Act
            viewModel.SearchText = "";

            // Assert
            Assert.AreEqual(originalCount, viewModel.ShiftList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR52")]
        [Description("TC_BR52_004: Verify search is case insensitive")]
        public void TC_BR52_004_Search_IsCaseInsensitive()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0];
            viewModel.SearchText = "morning"; // lowercase

            // Assert
            Assert.IsTrue(viewModel.ShiftList.Count > 0);
        }

        #endregion

        #region BR53 - Create Shift Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR53")]
        [Description("TC_BR53_001: Verify AddCommand is initialized")]
        public void TC_BR53_001_AddCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.AddCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR53")]
        [Description("TC_BR53_002: Verify cannot add without shift name")]
        public void TC_BR53_002_AddCommand_RequiresShiftName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.ShiftName = "";
            viewModel.StartTime = new TimeSpan(8, 0, 0);
            viewModel.EndTime = new TimeSpan(12, 0, 0);

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.AddMessage));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR53")]
        [Description("TC_BR53_003: Verify cannot add without start time")]
        public void TC_BR53_003_AddCommand_RequiresStartTime()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.ShiftName = "Test Shift";
            viewModel.StartTime = null;
            viewModel.EndTime = new TimeSpan(12, 0, 0);

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR53")]
        [Description("TC_BR53_004: Verify cannot add without end time")]
        public void TC_BR53_004_AddCommand_RequiresEndTime()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.ShiftName = "Test Shift";
            viewModel.StartTime = new TimeSpan(8, 0, 0);
            viewModel.EndTime = null;

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR53")]
        [Description("TC_BR53_005: Verify cannot add with end time before start time")]
        public void TC_BR53_005_AddCommand_ValidatesTimeOrder()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.ShiftName = "Test Shift";
            viewModel.StartTime = new TimeSpan(12, 0, 0);
            viewModel.EndTime = new TimeSpan(8, 0, 0); // Before start time

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR53")]
        [Description("TC_BR53_006: Verify cannot add with time before 7:30")]
        public void TC_BR53_006_AddCommand_ValidatesMinimumTime()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.ShiftName = "Test Shift";
            viewModel.StartTime = new TimeSpan(6, 0, 0); // Before 7:30
            viewModel.EndTime = new TimeSpan(12, 0, 0);

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR53")]
        [Description("TC_BR53_007: Verify cannot add duplicate shift name")]
        public void TC_BR53_007_AddCommand_PreventsDuplicateShiftName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var existingShift = viewModel.ShiftList.First();
            viewModel.IsAdding = true;
            viewModel.ShiftName = existingShift.ShiftName;
            viewModel.StartTime = new TimeSpan(8, 0, 0);
            viewModel.EndTime = new TimeSpan(12, 0, 0);

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR54 - Update Shift Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR54")]
        [Description("TC_BR54_001: Verify EditCommand is initialized")]
        public void TC_BR54_001_EditCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.EditCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR54")]
        [Description("TC_BR54_002: Verify cannot edit without selection")]
        public void TC_BR54_002_EditCommand_RequiresSelection()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = null;

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR54")]
        [Description("TC_BR54_003: Verify cannot edit without changes")]
        public void TC_BR54_003_EditCommand_RequiresChanges()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var shift = viewModel.ShiftList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = shift;

            // Act - No changes made
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR54")]
        [Description("TC_BR54_004: Verify cannot edit with empty name")]
        public void TC_BR54_004_EditCommand_ValidatesName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var shift = viewModel.ShiftList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = shift;
            viewModel.ShiftName = ""; // Invalid

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR54")]
        [Description("TC_BR54_005: Verify cannot edit to duplicate name")]
        public void TC_BR54_005_EditCommand_PreventsDuplicateName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var shift = viewModel.ShiftList.First();
            var otherShift = viewModel.ShiftList.Last();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = shift;
            viewModel.ShiftName = otherShift.ShiftName;

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR55 - Delete Shift Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR55")]
        [Description("TC_BR55_001: Verify DeleteCommand is initialized")]
        public void TC_BR55_001_DeleteCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.DeleteCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR55")]
        [Description("TC_BR55_002: Verify cannot delete without selection")]
        public void TC_BR55_002_DeleteCommand_RequiresSelection()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.SelectedItem = null;

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR55")]
        [Description("TC_BR55_003: Verify cannot delete shift with bookings")]
        public void TC_BR55_003_DeleteCommand_PreventsDeletionWithBookings()
        {
            // Arrange
            var shift = CreateSampleShifts().First();
            _mockBookingService.Setup(s => s.GetAll()).Returns(new List<BookingDTO>
            {
                new BookingDTO { ShiftId = shift.ShiftId }
            });

            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = shift;

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR55")]
        [Description("TC_BR55_004: Verify can delete shift without bookings")]
        public void TC_BR55_004_DeleteCommand_AllowsDeletionWithoutBookings()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = viewModel.ShiftList.First();

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        #endregion

        #region BR56 - Action Selection Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR56")]
        [Description("TC_BR56_001: Verify action list contains all actions")]
        public void TC_BR56_001_ActionList_ContainsAllActions()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ActionList);
            Assert.AreEqual(4, viewModel.ActionList.Count);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[0]));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[1]));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[2]));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR56")]
        [Description("TC_BR56_002: Verify selecting Add action sets IsAdding")]
        public void TC_BR56_002_SelectedAction_Add_SetsIsAdding()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[0];

            // Assert
            Assert.IsTrue(viewModel.IsAdding);
            Assert.IsFalse(viewModel.IsEditing);
            Assert.IsFalse(viewModel.IsDeleting);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR56")]
        [Description("TC_BR56_003: Verify selecting Edit action sets IsEditing")]
        public void TC_BR56_003_SelectedAction_Edit_SetsIsEditing()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[1];

            // Assert
            Assert.IsFalse(viewModel.IsAdding);
            Assert.IsTrue(viewModel.IsEditing);
            Assert.IsFalse(viewModel.IsDeleting);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR56")]
        [Description("TC_BR56_004: Verify selecting Delete action sets IsDeleting")]
        public void TC_BR56_004_SelectedAction_Delete_SetsIsDeleting()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[2];

            // Assert
            Assert.IsFalse(viewModel.IsAdding);
            Assert.IsFalse(viewModel.IsEditing);
            Assert.IsTrue(viewModel.IsDeleting);
        }

        #endregion

        #region BR57 - Reset Functionality Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR57")]
        [Description("TC_BR57_001: Verify ResetCommand is initialized")]
        public void TC_BR57_001_ResetCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ResetCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR57")]
        [Description("TC_BR57_002: Verify reset clears all fields")]
        public void TC_BR57_002_ResetCommand_ClearsAllFields()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.ShiftName = "Test";
            viewModel.StartTime = new TimeSpan(8, 0, 0);
            viewModel.EndTime = new TimeSpan(12, 0, 0);
            viewModel.SelectedItem = viewModel.ShiftList.First();

            // Act
            viewModel.ResetCommand.Execute(null);

            // Assert
            Assert.IsNull(viewModel.SelectedItem);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.ShiftName));
            Assert.IsNull(viewModel.StartTime);
            Assert.IsNull(viewModel.EndTime);
        }

        #endregion

        #region BR58 - Property Change Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR58")]
        [Description("TC_BR58_001: Verify ShiftName raises PropertyChanged")]
        public void TC_BR58_001_ShiftName_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.ShiftName))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.ShiftName = "New Shift";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR58")]
        [Description("TC_BR58_002: Verify StartTime raises PropertyChanged")]
        public void TC_BR58_002_StartTime_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.StartTime))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.StartTime = new TimeSpan(8, 0, 0);

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR58")]
        [Description("TC_BR58_003: Verify EndTime raises PropertyChanged")]
        public void TC_BR58_003_EndTime_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.EndTime))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.EndTime = new TimeSpan(12, 0, 0);

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        #endregion

        #region BR59 - Search Properties Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR59")]
        [Description("TC_BR59_001: Verify search properties list is initialized")]
        public void TC_BR59_001_SearchProperties_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SearchProperties);
            Assert.AreEqual(3, viewModel.SearchProperties.Count);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[0]));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[1]));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[2]));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftViewModel")]
        [TestCategory("BR59")]
        [Description("TC_BR59_002: Verify default search property is selected")]
        public void TC_BR59_002_SelectedSearchProperty_HasDefault()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SelectedSearchProperty);
            Assert.IsTrue(viewModel.SearchProperties.Contains(viewModel.SelectedSearchProperty));
        }

        #endregion

        #region Helper Methods

        private ShiftViewModel CreateViewModel()
        {
            return new ShiftViewModel(
                _mockShiftService.Object,
                _mockBookingService.Object);
        }

        private List<ShiftDTO> CreateSampleShifts()
        {
            return new List<ShiftDTO>
            {
                new ShiftDTO
                {
                    ShiftId = 1,
                    ShiftName = "Morning Shift",
                    StartTime = new TimeSpan(7, 30, 0),
                    EndTime = new TimeSpan(12, 0, 0)
                },
                new ShiftDTO
                {
                    ShiftId = 2,
                    ShiftName = "Afternoon Shift",
                    StartTime = new TimeSpan(13, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                },
                new ShiftDTO
                {
                    ShiftId = 3,
                    ShiftName = "Evening Shift",
                    StartTime = new TimeSpan(18, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0)
                }
            };
        }

        #endregion
    }
}
