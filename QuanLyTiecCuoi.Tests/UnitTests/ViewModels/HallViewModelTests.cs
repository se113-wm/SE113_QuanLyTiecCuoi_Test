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
    /// Unit Tests for HallViewModel
    /// Covers BR41-BR50 (skip BR51 - manual UI tests)
    /// </summary>
    [TestClass]
    public class HallViewModelTests
    {
        private Mock<IHallService> _mockHallService;
        private Mock<IHallTypeService> _mockHallTypeService;
        private Mock<IBookingService> _mockBookingService;

        [TestInitialize]
        public void Setup()
        {
            _mockHallService = new Mock<IHallService>();
            _mockHallTypeService = new Mock<IHallTypeService>();
            _mockBookingService = new Mock<IBookingService>();

            // Setup default mock returns
            _mockHallService.Setup(s => s.GetAll()).Returns(CreateSampleHalls());
            _mockHallTypeService.Setup(s => s.GetAll()).Returns(CreateSampleHallTypes());
            _mockBookingService.Setup(s => s.GetAll()).Returns(new List<BookingDTO>());
        }

        #region BR41 - Display Hall List Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR41")]
        [Description("TC_BR41_001: Verify HallViewModel initializes and loads hall list")]
        public void TC_BR41_001_Constructor_LoadsHallList()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.HallList);
            Assert.IsTrue(viewModel.HallList.Count > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR41")]
        [Description("TC_BR41_002: Verify hall list contains hall type information")]
        public void TC_BR41_002_HallList_ContainsHallTypeInfo()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var hall in viewModel.HallList)
            {
                Assert.IsNotNull(hall.HallType);
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallType.HallTypeName));
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR41")]
        [Description("TC_BR41_003: Verify hall list contains capacity information")]
        public void TC_BR41_003_HallList_ContainsCapacityInfo()
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
        [TestCategory("HallViewModel")]
        [TestCategory("BR41")]
        [Description("TC_BR41_004: Verify original list is preserved")]
        public void TC_BR41_004_OriginalList_IsPreserved()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.OriginalList);
            Assert.AreEqual(viewModel.HallList.Count, viewModel.OriginalList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR41")]
        [Description("TC_BR41_005: Verify hall types list is loaded")]
        public void TC_BR41_005_HallTypes_AreLoaded()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.HallTypes);
            Assert.IsTrue(viewModel.HallTypes.Count > 0);
        }

        #endregion

        #region BR42 - Search/Filter Hall Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR42")]
        [Description("TC_BR42_001: Verify search by hall name works")]
        public void TC_BR42_001_SearchByHallName_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "Diamond";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0]; // "Tên s?nh"
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.HallList.All(h => 
                h.HallName.Contains(searchTerm)));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR42")]
        [Description("TC_BR42_002: Verify search by hall type name works")]
        public void TC_BR42_002_SearchByHallTypeName_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "VIP";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[1]; // "Tên lo?i s?nh"
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.HallList.All(h => 
                h.HallType?.HallTypeName.Contains(searchTerm) == true));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR42")]
        [Description("TC_BR42_003: Verify search by note works")]
        public void TC_BR42_003_SearchByNote_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "Large";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[2]; // "Ghi chú"
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.HallList.All(h => 
                !string.IsNullOrEmpty(h.Note) && h.Note.Contains(searchTerm)));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR42")]
        [Description("TC_BR42_004: Verify clearing search restores full list")]
        public void TC_BR42_004_ClearSearch_RestoresFullList()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var originalCount = viewModel.HallList.Count;
            viewModel.SearchText = "Diamond";

            // Act
            viewModel.SearchText = "";

            // Assert
            Assert.AreEqual(originalCount, viewModel.HallList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR42")]
        [Description("TC_BR42_005: Verify search is case insensitive")]
        public void TC_BR42_005_Search_IsCaseInsensitive()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0]; // "Tên s?nh"
            viewModel.SearchText = "diamond"; // lowercase

            // Assert
            Assert.IsTrue(viewModel.HallList.Count > 0);
            Assert.IsTrue(viewModel.HallList.All(h => 
                h.HallName.IndexOf("diamond", StringComparison.OrdinalIgnoreCase) >= 0));
        }

        #endregion

        #region BR43 - Create Hall Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR43")]
        [Description("TC_BR43_001: Verify AddCommand is initialized")]
        public void TC_BR43_001_AddCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.AddCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR43")]
        [Description("TC_BR43_002: Verify cannot add without hall name")]
        public void TC_BR43_002_AddCommand_RequiresHallName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.HallName = "";
            viewModel.SelectedHallType = viewModel.HallTypes.First();
            viewModel.MaxTableCount = "50";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR43")]
        [Description("TC_BR43_003: Verify cannot add without hall type")]
        public void TC_BR43_003_AddCommand_RequiresHallType()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.HallName = "New Hall";
            viewModel.SelectedHallType = null;
            viewModel.MaxTableCount = "50";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR43")]
        [Description("TC_BR43_004: Verify cannot add with invalid table count")]
        public void TC_BR43_004_AddCommand_RequiresValidTableCount()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.HallName = "New Hall";
            viewModel.SelectedHallType = viewModel.HallTypes.First();
            viewModel.MaxTableCount = "0";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR43")]
        [Description("TC_BR43_005: Verify cannot add duplicate hall name in same type")]
        public void TC_BR43_005_AddCommand_PreventsDuplicateHallName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var existingHall = viewModel.HallList.First();
            viewModel.IsAdding = true;
            viewModel.HallName = existingHall.HallName;
            viewModel.SelectedHallType = viewModel.HallTypes.First(ht => ht.HallTypeId == existingHall.HallTypeId);
            viewModel.MaxTableCount = "50";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR44 - Update Hall Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR44")]
        [Description("TC_BR44_001: Verify EditCommand is initialized")]
        public void TC_BR44_001_EditCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.EditCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR44")]
        [Description("TC_BR44_002: Verify cannot edit without selection")]
        public void TC_BR44_002_EditCommand_RequiresSelection()
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
        [TestCategory("HallViewModel")]
        [TestCategory("BR44")]
        [Description("TC_BR44_003: Verify cannot edit without changes")]
        public void TC_BR44_003_EditCommand_RequiresChanges()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var hall = viewModel.HallList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = hall;

            // Act - No changes made
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR44")]
        [Description("TC_BR44_004: Verify cannot edit with invalid data")]
        public void TC_BR44_004_EditCommand_ValidatesData()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var hall = viewModel.HallList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = hall;
            viewModel.HallName = ""; // Invalid

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR44")]
        [Description("TC_BR44_005: Verify cannot change table count for halls with future bookings")]
        public void TC_BR44_005_EditCommand_PreventTableCountChangeWithFutureBookings()
        {
            // Arrange
            var hall = CreateSampleHalls().First();
            _mockBookingService.Setup(s => s.GetAll()).Returns(new List<BookingDTO>
            {
                new BookingDTO
                {
                    HallId = hall.HallId,
                    WeddingDate = DateTime.Today.AddDays(7)
                }
            });

            var viewModel = CreateViewModel();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = hall;
            viewModel.MaxTableCount = "999"; // Try to change

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR45 - Delete Hall Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR45")]
        [Description("TC_BR45_001: Verify DeleteCommand is initialized")]
        public void TC_BR45_001_DeleteCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.DeleteCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR45")]
        [Description("TC_BR45_002: Verify cannot delete without selection")]
        public void TC_BR45_002_DeleteCommand_RequiresSelection()
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
        [TestCategory("HallViewModel")]
        [TestCategory("BR45")]
        [Description("TC_BR45_003: Verify cannot delete hall with bookings")]
        public void TC_BR45_003_DeleteCommand_PreventsDeletionWithBookings()
        {
            // Arrange
            var hall = CreateSampleHalls().First();
            _mockBookingService.Setup(s => s.GetAll()).Returns(new List<BookingDTO>
            {
                new BookingDTO { HallId = hall.HallId }
            });

            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = hall;

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR45")]
        [Description("TC_BR45_004: Verify can delete hall without bookings")]
        public void TC_BR45_004_DeleteCommand_AllowsDeletionWithoutBookings()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = viewModel.HallList.First();

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR45")]
        [Description("TC_BR45_005: Verify delete message displays when hall has bookings")]
        public void TC_BR45_005_DeleteMessage_DisplaysForHallWithBookings()
        {
            // Arrange
            var hall = CreateSampleHalls().First();
            _mockBookingService.Setup(s => s.GetAll()).Returns(new List<BookingDTO>
            {
                new BookingDTO { HallId = hall.HallId }
            });

            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = hall;

            // Act
            var canDelete = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canDelete);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.DeleteMessage));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR45")]
        [Description("TC_BR45_006: Verify delete clears messages when allowed")]
        public void TC_BR45_006_DeleteMessage_ClearsWhenAllowed()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = viewModel.HallList.First();

            // Act
            var canDelete = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canDelete);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.DeleteMessage));
        }

        #endregion

        #region BR46 - Action Selection Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR46")]
        [Description("TC_BR46_001: Verify action list contains all actions")]
        public void TC_BR46_001_ActionList_ContainsAllActions()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ActionList);
            Assert.AreEqual(5, viewModel.ActionList.Count, "ActionList should contain 5 items");
            // Verify actions exist without hardcoding Vietnamese strings
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[0]), "First action should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[1]), "Second action should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[2]), "Third action should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[3]), "Fourth action should not be empty");
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR46")]
        [Description("TC_BR46_002: Verify selecting Add action sets IsAdding")]
        public void TC_BR46_002_SelectedAction_Add_SetsIsAdding()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[0]; // "Thêm"

            // Assert
            Assert.IsTrue(viewModel.IsAdding);
            Assert.IsFalse(viewModel.IsEditing);
            Assert.IsFalse(viewModel.IsDeleting);
            Assert.IsFalse(viewModel.IsExporting);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR46")]
        [Description("TC_BR46_003: Verify selecting Edit action sets IsEditing")]
        public void TC_BR46_003_SelectedAction_Edit_SetsIsEditing()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[1]; // "S?a"

            // Assert
            Assert.IsFalse(viewModel.IsAdding);
            Assert.IsTrue(viewModel.IsEditing);
            Assert.IsFalse(viewModel.IsDeleting);
            Assert.IsFalse(viewModel.IsExporting);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR46")]
        [Description("TC_BR46_004: Verify selecting Delete action sets IsDeleting")]
        public void TC_BR46_004_SelectedAction_Delete_SetsIsDeleting()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[2]; // "Xóa"

            // Assert
            Assert.IsFalse(viewModel.IsAdding);
            Assert.IsFalse(viewModel.IsEditing);
            Assert.IsTrue(viewModel.IsDeleting);
            Assert.IsFalse(viewModel.IsExporting);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR46")]
        [Description("TC_BR46_005: Verify selecting Export action sets IsExporting")]
        public void TC_BR46_005_SelectedAction_Export_SetsIsExporting()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[3]; // "Xu?t Excel"

            // Assert
            Assert.IsFalse(viewModel.IsAdding);
            Assert.IsFalse(viewModel.IsEditing);
            Assert.IsFalse(viewModel.IsDeleting);
            Assert.IsTrue(viewModel.IsExporting);
        }

        #endregion

        #region BR47 - Reset Functionality Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR47")]
        [Description("TC_BR47_001: Verify ResetCommand is initialized")]
        public void TC_BR47_001_ResetCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ResetCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR47")]
        [Description("TC_BR47_002: Verify reset clears all fields")]
        public void TC_BR47_002_ResetCommand_ClearsAllFields()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.HallName = "Test";
            viewModel.MaxTableCount = "50";
            viewModel.Note = "Note";
            viewModel.SelectedHallType = viewModel.HallTypes.First();
            viewModel.SelectedItem = viewModel.HallList.First();

            // Act
            viewModel.ResetCommand.Execute(null);

            // Assert
            Assert.IsNull(viewModel.SelectedItem);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.HallName));
            Assert.IsNull(viewModel.MaxTableCount);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.Note));
            Assert.IsNull(viewModel.SelectedHallType);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR47")]
        [Description("TC_BR47_003: Verify reset clears search")]
        public void TC_BR47_003_ResetCommand_ClearsSearch()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.SearchText = "Diamond";

            // Act
            viewModel.ResetCommand.Execute(null);

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.SearchText));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR47")]
        [Description("TC_BR47_004: Verify reset can execute anytime")]
        public void TC_BR47_004_ResetCommand_AlwaysCanExecute()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            bool canExecute = viewModel.ResetCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR47")]
        [Description("TC_BR47_005: Verify reset when changing actions")]
        public void TC_BR47_005_SelectedAction_TriggersReset()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.HallName = "Test";
            viewModel.SelectedAction = viewModel.ActionList[0]; // "Thêm"

            // Act
            viewModel.SelectedAction = viewModel.ActionList[1]; // "S?a"

            // Assert - Should reset when action changes
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.HallName));
        }

        #endregion

        #region BR48 - Property Change Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR48")]
        [Description("TC_BR48_001: Verify HallName raises PropertyChanged")]
        public void TC_BR48_001_HallName_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.HallName))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.HallName = "New Name";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR48")]
        [Description("TC_BR48_002: Verify MaxTableCount raises PropertyChanged")]
        public void TC_BR48_002_MaxTableCount_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.MaxTableCount))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.MaxTableCount = "100";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR48")]
        [Description("TC_BR48_003: Verify SelectedHallType raises PropertyChanged")]
        public void TC_BR48_003_SelectedHallType_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SelectedHallType))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedHallType = viewModel.HallTypes.First();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR48")]
        [Description("TC_BR48_004: Verify SelectedItem raises PropertyChanged")]
        public void TC_BR48_004_SelectedItem_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SelectedItem))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedItem = viewModel.HallList.First();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR48")]
        [Description("TC_BR48_005: Verify SearchText raises PropertyChanged")]
        public void TC_BR48_005_SearchText_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SearchText))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SearchText = "test";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR48")]
        [Description("TC_BR48_006: Verify SelectedAction raises PropertyChanged")]
        public void TC_BR48_006_SelectedAction_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SelectedAction))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedAction = viewModel.ActionList[0]; // "Thêm"

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        #endregion

        #region BR49 - Search Properties Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR49")]
        [Description("TC_BR49_001: Verify search properties list is initialized")]
        public void TC_BR49_001_SearchProperties_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SearchProperties);
            Assert.IsTrue(viewModel.SearchProperties.Count > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR49")]
        [Description("TC_BR49_002: Verify default search property is selected")]
        public void TC_BR49_002_SelectedSearchProperty_HasDefault()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SelectedSearchProperty);
            Assert.IsTrue(viewModel.SearchProperties.Contains(viewModel.SelectedSearchProperty));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR49")]
        [Description("TC_BR49_003: Verify changing search property triggers search")]
        public void TC_BR49_003_SelectedSearchProperty_TriggersSearch()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.SearchText = "VIP";
            var property1 = viewModel.SearchProperties[0]; // "Tên s?nh"
            var property2 = viewModel.SearchProperties[1]; // "Tên lo?i s?nh"

            // Act - Change search property
            viewModel.SelectedSearchProperty = property1;
            var count1 = viewModel.HallList.Count;

            viewModel.SelectedSearchProperty = property2;
            var count2 = viewModel.HallList.Count;

            // Assert - Results should be different
            Assert.AreNotEqual(count1, count2);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR49")]
        [Description("TC_BR49_004: Verify search property change raises PropertyChanged")]
        public void TC_BR49_004_SelectedSearchProperty_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SelectedSearchProperty))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties.Last();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR49")]
        [Description("TC_BR49_005: Verify search works with empty search text")]
        public void TC_BR49_005_Search_HandlesEmptyText()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var originalCount = viewModel.HallList.Count;

            // Act
            viewModel.SearchText = "";

            // Assert - Should show all halls
            Assert.AreEqual(originalCount, viewModel.HallList.Count);
        }

        #endregion

        #region BR50 - Export to Excel Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR50")]
        [Description("TC_BR50_001: Verify ExportToExcelCommand is initialized")]
        public void TC_BR50_001_ExportToExcelCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ExportToExcelCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR50")]
        [Description("TC_BR50_002: Verify export command can always execute")]
        public void TC_BR50_002_ExportToExcelCommand_CanAlwaysExecute()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            bool canExecute = viewModel.ExportToExcelCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR50")]
        [Description("TC_BR50_003: Verify export works with empty list")]
        public void TC_BR50_003_ExportToExcelCommand_HandlesEmptyList()
        {
            // Arrange
            _mockHallService.Setup(s => s.GetAll()).Returns(new List<HallDTO>());
            var viewModel = CreateViewModel();

            // Act
            bool canExecute = viewModel.ExportToExcelCommand.CanExecute(null);

            // Assert - Should still be able to execute (will show message)
            Assert.IsTrue(canExecute);
        }

        #endregion

        #region Helper Methods

        private HallViewModel CreateViewModel()
        {
            return new HallViewModel(
                _mockHallService.Object,
                _mockHallTypeService.Object,
                _mockBookingService.Object);
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
                    Note = "Large VIP hall",
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
                    Note = "Standard hall",
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
                    HallTypeId = 3,
                    Note = "Economy hall",
                    HallType = new HallTypeDTO
                    {
                        HallTypeId = 3,
                        HallTypeName = "Economy",
                        MinTablePrice = 1000000
                    }
                }
            };
        }

        private List<HallTypeDTO> CreateSampleHallTypes()
        {
            return new List<HallTypeDTO>
            {
                new HallTypeDTO { HallTypeId = 1, HallTypeName = "VIP", MinTablePrice = 2000000 },
                new HallTypeDTO { HallTypeId = 2, HallTypeName = "Standard", MinTablePrice = 1500000 },
                new HallTypeDTO { HallTypeId = 3, HallTypeName = "Economy", MinTablePrice = 1000000 }
            };
        }

        #endregion
    }
}
