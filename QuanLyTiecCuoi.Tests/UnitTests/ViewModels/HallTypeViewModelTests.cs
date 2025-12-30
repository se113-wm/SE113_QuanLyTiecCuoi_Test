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
    /// Unit Tests for HallTypeViewModel
    /// Covers BR60-BR66 (skip BR67 - manual UI tests)
    /// </summary>
    [TestClass]
    public class HallTypeViewModelTests
    {
        private Mock<IHallTypeService> _mockHallTypeService;
        private Mock<IHallService> _mockHallService;

        [TestInitialize]
        public void Setup()
        {
            _mockHallTypeService = new Mock<IHallTypeService>();
            _mockHallService = new Mock<IHallService>();

            // Setup default mock returns
            _mockHallTypeService.Setup(s => s.GetAll()).Returns(CreateSampleHallTypes());
            _mockHallService.Setup(s => s.GetAll()).Returns(new List<HallDTO>());
        }

        #region BR60 - Display HallType List Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR60")]
        [Description("TC_BR60_001: Verify HallTypeViewModel initializes and loads hall type list")]
        public void TC_BR60_001_Constructor_LoadsHallTypeList()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.HallTypeList);
            Assert.IsTrue(viewModel.HallTypeList.Count > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR60")]
        [Description("TC_BR60_002: Verify hall type list contains pricing information")]
        public void TC_BR60_002_HallTypeList_ContainsPricingInfo()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var hallType in viewModel.HallTypeList)
            {
                Assert.IsNotNull(hallType.MinTablePrice);
                Assert.IsTrue(hallType.MinTablePrice > 0);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR60")]
        [Description("TC_BR60_003: Verify original list is preserved")]
        public void TC_BR60_003_OriginalList_IsPreserved()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.OriginalList);
            Assert.AreEqual(viewModel.HallTypeList.Count, viewModel.OriginalList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR60")]
        [Description("TC_BR60_004: Verify hall type names are loaded")]
        public void TC_BR60_004_HallTypeList_ContainsNames()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var hallType in viewModel.HallTypeList)
            {
                Assert.IsFalse(string.IsNullOrEmpty(hallType.HallTypeName));
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR60")]
        [Description("TC_BR60_005: Verify hall types are distinct")]
        public void TC_BR60_005_HallTypeList_ContainsDistinctTypes()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            var uniqueNames = viewModel.HallTypeList.Select(ht => ht.HallTypeName).Distinct().Count();
            Assert.AreEqual(viewModel.HallTypeList.Count, uniqueNames);
        }

        #endregion

        #region BR61 - Search/Filter HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR61")]
        [Description("TC_BR61_001: Verify search by hall type name works")]
        public void TC_BR61_001_SearchByHallTypeName_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "VIP";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0]; // "Tên lo?i s?nh"
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.HallTypeList.All(ht => 
                ht.HallTypeName.Contains(searchTerm)));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR61")]
        [Description("TC_BR61_002: Verify search by min table price works")]
        public void TC_BR61_002_SearchByMinTablePrice_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "2000000";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[1]; // "??n giá bàn t?i thi?u"
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.HallTypeList.Count > 0);
            Assert.IsTrue(viewModel.HallTypeList.All(ht => 
                ht.MinTablePrice?.ToString().Contains(searchTerm.Replace(",", "")) == true));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR61")]
        [Description("TC_BR61_003: Verify clearing search restores full list")]
        public void TC_BR61_003_ClearSearch_RestoresFullList()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var originalCount = viewModel.HallTypeList.Count;
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0];
            viewModel.SearchText = "VIP";

            // Act
            viewModel.SearchText = "";

            // Assert
            Assert.AreEqual(originalCount, viewModel.HallTypeList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR61")]
        [Description("TC_BR61_004: Verify search is case insensitive")]
        public void TC_BR61_004_Search_IsCaseInsensitive()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0]; // "Tên lo?i s?nh"
            viewModel.SearchText = "vip"; // lowercase

            // Assert
            Assert.IsTrue(viewModel.HallTypeList.Count > 0);
            Assert.IsTrue(viewModel.HallTypeList.All(ht => 
                ht.HallTypeName.IndexOf("vip", StringComparison.OrdinalIgnoreCase) >= 0));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR61")]
        [Description("TC_BR61_005: Verify search with no results returns empty list")]
        public void TC_BR61_005_Search_WithNoResults_ReturnsEmpty()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SearchText = "NonExistentType123";

            // Assert
            Assert.AreEqual(0, viewModel.HallTypeList.Count);
        }

        #endregion

        #region BR62 - Create HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR62")]
        [Description("TC_BR62_001: Verify AddCommand is initialized")]
        public void TC_BR62_001_AddCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.AddCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR62")]
        [Description("TC_BR62_002: Verify cannot add without hall type name")]
        public void TC_BR62_002_AddCommand_RequiresHallTypeName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.HallTypeName = "";
            viewModel.MinTablePrice = "2000000";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR62")]
        [Description("TC_BR62_003: Verify cannot add duplicate hall type name")]
        public void TC_BR62_003_AddCommand_PreventsDuplicateHallTypeName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var existingType = viewModel.HallTypeList.First();
            viewModel.IsAdding = true;
            viewModel.HallTypeName = existingType.HallTypeName;
            viewModel.MinTablePrice = "2000000";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR62")]
        [Description("TC_BR62_004: Verify cannot add with invalid price")]
        public void TC_BR62_004_AddCommand_RequiresValidPrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.HallTypeName = "New Type";
            viewModel.MinTablePrice = "abc"; // Invalid

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR62")]
        [Description("TC_BR62_005: Verify cannot add with price less than 10000")]
        public void TC_BR62_005_AddCommand_RequiresMinimumPrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.HallTypeName = "New Type";
            viewModel.MinTablePrice = "5000"; // Too low

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR62")]
        [Description("TC_BR62_006: Verify cannot add with decimal price")]
        public void TC_BR62_006_AddCommand_RequiresIntegerPrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsAdding = true;
            viewModel.HallTypeName = "New Type";
            viewModel.MinTablePrice = "2000000.5"; // Has decimal

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR63 - Update HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR63")]
        [Description("TC_BR63_001: Verify EditCommand is initialized")]
        public void TC_BR63_001_EditCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.EditCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR63")]
        [Description("TC_BR63_002: Verify cannot edit without selection")]
        public void TC_BR63_002_EditCommand_RequiresSelection()
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
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR63")]
        [Description("TC_BR63_003: Verify cannot edit without changes")]
        public void TC_BR63_003_EditCommand_RequiresChanges()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var hallType = viewModel.HallTypeList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = hallType;

            // Act - No changes made
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR63")]
        [Description("TC_BR63_004: Verify cannot edit with empty name")]
        public void TC_BR63_004_EditCommand_ValidatesName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var hallType = viewModel.HallTypeList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = hallType;
            viewModel.HallTypeName = ""; // Invalid

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR63")]
        [Description("TC_BR63_005: Verify cannot edit to duplicate name")]
        public void TC_BR63_005_EditCommand_PreventsDuplicateName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var hallType = viewModel.HallTypeList.First();
            var otherType = viewModel.HallTypeList.Last();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = hallType;
            viewModel.HallTypeName = otherType.HallTypeName; // Try to use another's name

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR63")]
        [Description("TC_BR63_006: Verify cannot edit with invalid price")]
        public void TC_BR63_006_EditCommand_ValidatesPrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var hallType = viewModel.HallTypeList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = hallType;
            viewModel.HallTypeName = "Updated Name";
            viewModel.MinTablePrice = "invalid";

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR64 - Delete HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR64")]
        [Description("TC_BR64_001: Verify DeleteCommand is initialized")]
        public void TC_BR64_001_DeleteCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.DeleteCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR64")]
        [Description("TC_BR64_002: Verify cannot delete without selection")]
        public void TC_BR64_002_DeleteCommand_RequiresSelection()
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
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR64")]
        [Description("TC_BR64_003: Verify cannot delete hall type with halls")]
        public void TC_BR64_003_DeleteCommand_PreventsDeletionWithHalls()
        {
            // Arrange
            var hallType = CreateSampleHallTypes().First();
            _mockHallService.Setup(s => s.GetAll()).Returns(new List<HallDTO>
            {
                new HallDTO { HallTypeId = hallType.HallTypeId }
            });

            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = hallType;

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR64")]
        [Description("TC_BR64_004: Verify can delete hall type without halls")]
        public void TC_BR64_004_DeleteCommand_AllowsDeletionWithoutHalls()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = viewModel.HallTypeList.First();

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR64")]
        [Description("TC_BR64_005: Verify delete message displays when hall type has halls")]
        public void TC_BR64_005_DeleteMessage_DisplaysForHallTypeWithHalls()
        {
            // Arrange
            var hallType = CreateSampleHallTypes().First();
            _mockHallService.Setup(s => s.GetAll()).Returns(new List<HallDTO>
            {
                new HallDTO { HallTypeId = hallType.HallTypeId }
            });

            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = hallType;

            // Act
            var canDelete = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canDelete);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.DeleteMessage));
        }

        #endregion

        #region BR65 - Action Selection Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR65")]
        [Description("TC_BR65_001: Verify action list contains all actions")]
        public void TC_BR65_001_ActionList_ContainsAllActions()
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
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR65")]
        [Description("TC_BR65_002: Verify selecting Add action sets IsAdding")]
        public void TC_BR65_002_SelectedAction_Add_SetsIsAdding()
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
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR65")]
        [Description("TC_BR65_003: Verify selecting Edit action sets IsEditing")]
        public void TC_BR65_003_SelectedAction_Edit_SetsIsEditing()
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
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR65")]
        [Description("TC_BR65_004: Verify selecting Delete action sets IsDeleting")]
        public void TC_BR65_004_SelectedAction_Delete_SetsIsDeleting()
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
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR65")]
        [Description("TC_BR65_005: Verify selecting Export action sets IsExporting")]
        public void TC_BR65_005_SelectedAction_Export_SetsIsExporting()
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

        #region BR66 - Reset and Property Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_001: Verify ResetCommand is initialized")]
        public void TC_BR66_001_ResetCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ResetCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_002: Verify reset clears all fields")]
        public void TC_BR66_002_ResetCommand_ClearsAllFields()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.HallTypeName = "Test";
            viewModel.MinTablePrice = "2000000";
            viewModel.SelectedItem = viewModel.HallTypeList.First();

            // Act
            viewModel.ResetCommand.Execute(null);

            // Assert
            Assert.IsNull(viewModel.SelectedItem);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.HallTypeName));
            Assert.IsNull(viewModel.MinTablePrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_003: Verify HallTypeName raises PropertyChanged")]
        public void TC_BR66_003_HallTypeName_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.HallTypeName))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.HallTypeName = "New Type";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_004: Verify MinTablePrice raises PropertyChanged")]
        public void TC_BR66_004_MinTablePrice_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.MinTablePrice))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.MinTablePrice = "2000000";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_005: Verify SelectedItem raises PropertyChanged")]
        public void TC_BR66_005_SelectedItem_RaisesPropertyChanged()
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
            viewModel.SelectedItem = viewModel.HallTypeList.First();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_006: Verify SearchText raises PropertyChanged")]
        public void TC_BR66_006_SearchText_RaisesPropertyChanged()
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
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_007: Verify ExportToExcelCommand is initialized")]
        public void TC_BR66_007_ExportToExcelCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ExportToExcelCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_008: Verify search properties list is initialized")]
        public void TC_BR66_008_SearchProperties_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SearchProperties);
            Assert.AreEqual(2, viewModel.SearchProperties.Count, "Should have 2 search properties");
            // Verify properties exist without hardcoding Vietnamese strings
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[0]), "First search property should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[1]), "Second search property should not be empty");
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_009: Verify default search property is selected")]
        public void TC_BR66_009_SelectedSearchProperty_HasDefault()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SelectedSearchProperty);
            Assert.IsTrue(viewModel.SearchProperties.Contains(viewModel.SelectedSearchProperty));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeViewModel")]
        [TestCategory("BR66")]
        [Description("TC_BR66_010: Verify reset when changing actions")]
        public void TC_BR66_010_SelectedAction_TriggersReset()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.HallTypeName = "Test";
            viewModel.SelectedAction = viewModel.ActionList[0]; // "Thêm"

            // Act
            viewModel.SelectedAction = viewModel.ActionList[1]; // "S?a"

            // Assert - Should reset when action changes
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.HallTypeName));
        }

        #endregion

        #region Helper Methods

        private HallTypeViewModel CreateViewModel()
        {
            return new HallTypeViewModel(
                _mockHallTypeService.Object,
                _mockHallService.Object);
        }

        private List<HallTypeDTO> CreateSampleHallTypes()
        {
            return new List<HallTypeDTO>
            {
                new HallTypeDTO
                {
                    HallTypeId = 1,
                    HallTypeName = "VIP",
                    MinTablePrice = 2000000
                },
                new HallTypeDTO
                {
                    HallTypeId = 2,
                    HallTypeName = "Standard",
                    MinTablePrice = 1500000
                },
                new HallTypeDTO
                {
                    HallTypeId = 3,
                    HallTypeName = "Economy",
                    MinTablePrice = 1000000
                }
            };
        }

        #endregion
    }
}
