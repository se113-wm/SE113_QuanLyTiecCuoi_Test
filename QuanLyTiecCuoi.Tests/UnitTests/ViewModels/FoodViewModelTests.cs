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
    /// Unit Tests for FoodViewModel (DishViewModel)
    /// Covers BR71-BR87 (skip BR72, BR83, BR88 - manual UI tests)
    /// </summary>
    [TestClass]
    public class FoodViewModelTests
    {
        private Mock<IDishService> _mockDishService;
        private Mock<IMenuService> _mockMenuService;

        [TestInitialize]
        public void Setup()
        {
            _mockDishService = new Mock<IDishService>();
            _mockMenuService = new Mock<IMenuService>();

            // Setup default mock returns
            _mockDishService.Setup(s => s.GetAll()).Returns(CreateSampleDishes());
            _mockMenuService.Setup(s => s.GetAll()).Returns(new List<MenuDTO>());
        }

        #region BR71 - Display Dish List Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR71")]
        [Description("TC_BR71_001: Verify FoodViewModel initializes and loads dish list")]
        public void TC_BR71_001_Constructor_LoadsDishList()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.DishList);
            Assert.IsTrue(viewModel.DishList.Count > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR71")]
        [Description("TC_BR71_002: Verify dish list contains pricing information")]
        public void TC_BR71_002_DishList_ContainsPricingInfo()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var dish in viewModel.DishList)
            {
                Assert.IsNotNull(dish.UnitPrice);
                Assert.IsTrue(dish.UnitPrice > 0);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR71")]
        [Description("TC_BR71_003: Verify original list is preserved")]
        public void TC_BR71_003_OriginalList_IsPreserved()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.OriginalList);
            Assert.AreEqual(viewModel.DishList.Count, viewModel.OriginalList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR71")]
        [Description("TC_BR71_004: Verify dish names are loaded")]
        public void TC_BR71_004_DishList_ContainsNames()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var dish in viewModel.DishList)
            {
                Assert.IsFalse(string.IsNullOrEmpty(dish.DishName));
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR71")]
        [Description("TC_BR71_005: Verify dishes are distinct")]
        public void TC_BR71_005_DishList_ContainsDistinctDishes()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            var uniqueNames = viewModel.DishList.Select(d => d.DishName).Distinct().Count();
            Assert.AreEqual(viewModel.DishList.Count, uniqueNames);
        }

        #endregion

        #region BR73 - Create Dish Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR73")]
        [Description("TC_BR73_001: Verify AddCommand is initialized")]
        public void TC_BR73_001_AddCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.AddCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR73")]
        [Description("TC_BR73_002: Verify cannot add without dish name")]
        public void TC_BR73_002_AddCommand_RequiresDishName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.DishName = "";
            viewModel.UnitPrice = "100000";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR73")]
        [Description("TC_BR73_003: Verify cannot add with invalid unit price")]
        public void TC_BR73_003_AddCommand_RequiresValidUnitPrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.DishName = "New Dish";
            viewModel.UnitPrice = "abc"; // Invalid

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR73")]
        [Description("TC_BR73_004: Verify cannot add with zero or negative price")]
        public void TC_BR73_004_AddCommand_RequiresPositivePrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.DishName = "New Dish";
            viewModel.UnitPrice = "0";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR73")]
        [Description("TC_BR73_005: Verify cannot add duplicate dish name")]
        public void TC_BR73_005_AddCommand_PreventsDuplicateDishName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var existingDish = viewModel.DishList.First();
            viewModel.DishName = existingDish.DishName;
            viewModel.UnitPrice = "100000";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR73")]
        [Description("TC_BR73_006: Verify cannot add when limit reached (100 dishes)")]
        public void TC_BR73_006_AddCommand_PreventsExceedingLimit()
        {
            // Arrange
            var largeDishList = new List<DishDTO>();
            for (int i = 0; i < 100; i++)
            {
                largeDishList.Add(new DishDTO { DishId = i, DishName = $"Dish {i}", UnitPrice = 100000 });
            }
            _mockDishService.Setup(s => s.GetAll()).Returns(largeDishList);

            var viewModel = CreateViewModel();
            viewModel.DishName = "New Dish";
            viewModel.UnitPrice = "100000";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR73")]
        [Description("TC_BR73_007: Verify cannot add with note exceeding 100 characters")]
        public void TC_BR73_007_AddCommand_ValidatesNoteLength()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.DishName = "New Dish";
            viewModel.UnitPrice = "100000";
            viewModel.Note = new string('a', 101); // 101 characters

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR74 - Update Dish Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR74")]
        [Description("TC_BR74_001: Verify EditCommand is initialized")]
        public void TC_BR74_001_EditCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.EditCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR74")]
        [Description("TC_BR74_002: Verify cannot edit without selection")]
        public void TC_BR74_002_EditCommand_RequiresSelection()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.SelectedItem = null;

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR74")]
        [Description("TC_BR74_003: Verify cannot edit without changes")]
        public void TC_BR74_003_EditCommand_RequiresChanges()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var dish = viewModel.DishList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = dish;

            // Act - No changes made
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR74")]
        [Description("TC_BR74_004: Verify cannot edit with empty name")]
        public void TC_BR74_004_EditCommand_ValidatesName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var dish = viewModel.DishList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = dish;
            viewModel.DishName = ""; // Invalid

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR74")]
        [Description("TC_BR74_005: Verify cannot edit to duplicate name")]
        public void TC_BR74_005_EditCommand_PreventsDuplicateName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var dish = viewModel.DishList.First();
            var otherDish = viewModel.DishList.Last();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = dish;
            viewModel.DishName = otherDish.DishName; // Try to use another's name

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR74")]
        [Description("TC_BR74_006: Verify cannot edit with invalid price")]
        public void TC_BR74_006_EditCommand_ValidatesPrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var dish = viewModel.DishList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = dish;
            viewModel.DishName = "Updated Name";
            viewModel.UnitPrice = "invalid";

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR75 - Delete Dish Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR75")]
        [Description("TC_BR75_001: Verify DeleteCommand is initialized")]
        public void TC_BR75_001_DeleteCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.DeleteCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR75")]
        [Description("TC_BR75_002: Verify cannot delete without selection")]
        public void TC_BR75_002_DeleteCommand_RequiresSelection()
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
        [TestCategory("FoodViewModel")]
        [TestCategory("BR75")]
        [Description("TC_BR75_003: Verify cannot delete dish in use")]
        public void TC_BR75_003_DeleteCommand_PreventsDeletionWhenInUse()
        {
            // Arrange
            var dish = CreateSampleDishes().First();
            _mockMenuService.Setup(s => s.GetAll()).Returns(new List<MenuDTO>
            {
                new MenuDTO { DishId = dish.DishId }
            });

            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = dish;

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR75")]
        [Description("TC_BR75_004: Verify can delete dish not in use")]
        public void TC_BR75_004_DeleteCommand_AllowsDeletionWhenNotInUse()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = viewModel.DishList.First();

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR75")]
        [Description("TC_BR75_005: Verify delete message displays when dish is in use")]
        public void TC_BR75_005_DeleteMessage_DisplaysForDishInUse()
        {
            // Arrange
            var dish = CreateSampleDishes().First();
            _mockMenuService.Setup(s => s.GetAll()).Returns(new List<MenuDTO>
            {
                new MenuDTO { DishId = dish.DishId }
            });

            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = dish;

            // Act
            var canDelete = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canDelete);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.DeleteMessage));
        }

        #endregion

        #region BR76 - Search/Filter Dish Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR76")]
        [Description("TC_BR76_001: Verify search by dish name works")]
        public void TC_BR76_001_SearchByDishName_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "Chicken";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0]; // Instead of hardcoding
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.DishList.Count > 0, "Should find dishes with search term");
            Assert.IsTrue(viewModel.DishList.All(d => 
                d.DishName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0),
                "All results should contain search term (case-insensitive)");
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR76")]
        [Description("TC_BR76_002: Verify search by unit price works")]
        public void TC_BR76_002_SearchByUnitPrice_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "150000";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[1]; // "Đơn giá"
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.DishList.Count > 0, "Should find dishes with matching price");
            Assert.IsTrue(viewModel.DishList.All(d => 
                d.UnitPrice?.ToString().Contains(searchTerm) == true),
                "All results should have matching price");
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR76")]
        [Description("TC_BR76_003: Verify search by note works")]
        public void TC_BR76_003_SearchByNote_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "Special";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[2]; // "Ghi chú"
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.DishList.Count > 0, "Should find dishes with search term in note");
            Assert.IsTrue(viewModel.DishList.All(d => 
                !string.IsNullOrEmpty(d.Note) && 
                d.Note.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0),
                "All results should have search term in note (case-insensitive)");
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR76")]
        [Description("TC_BR76_004: Verify clearing search restores full list")]
        public void TC_BR76_004_ClearSearch_RestoresFullList()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var originalCount = viewModel.DishList.Count;
            viewModel.SearchText = "Chicken";

            // Act
            viewModel.SearchText = "";

            // Assert
            Assert.AreEqual(originalCount, viewModel.DishList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR76")]
        [Description("TC_BR76_005: Verify search is case insensitive")]
        public void TC_BR76_005_Search_IsCaseInsensitive()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0]; // "Tên món ăn"
            viewModel.SearchText = "chicken"; // lowercase

            // Assert
            Assert.IsTrue(viewModel.DishList.Count > 0, "Should find dishes regardless of case");
            Assert.IsTrue(viewModel.DishList.All(d => 
                d.DishName.IndexOf("chicken", StringComparison.OrdinalIgnoreCase) >= 0),
                "Should match case-insensitively");
        }

        #endregion

        #region BR77 - Action Selection Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR77")]
        [Description("TC_BR77_001: Verify action list contains all actions")]
        public void TC_BR77_001_ActionList_ContainsAllActions()
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
        [TestCategory("FoodViewModel")]
        [TestCategory("BR77")]
        [Description("TC_BR77_002: Verify selecting Add action sets IsAdding")]
        public void TC_BR77_002_SelectedAction_Add_SetsIsAdding()
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
        [TestCategory("FoodViewModel")]
        [TestCategory("BR77")]
        [Description("TC_BR77_003: Verify selecting Edit action sets IsEditing")]
        public void TC_BR77_003_SelectedAction_Edit_SetsIsEditing()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[1]; // "Sửa"

            // Assert
            Assert.IsFalse(viewModel.IsAdding);
            Assert.IsTrue(viewModel.IsEditing);
            Assert.IsFalse(viewModel.IsDeleting);
            Assert.IsFalse(viewModel.IsExporting);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR77")]
        [Description("TC_BR77_004: Verify selecting Delete action sets IsDeleting")]
        public void TC_BR77_004_SelectedAction_Delete_SetsIsDeleting()
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
        [TestCategory("FoodViewModel")]
        [TestCategory("BR77")]
        [Description("TC_BR77_005: Verify selecting Export action sets IsExporting")]
        public void TC_BR77_005_SelectedAction_Export_SetsIsExporting()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[3]; // "Xuất Excel"

            // Assert
            Assert.IsFalse(viewModel.IsAdding);
            Assert.IsFalse(viewModel.IsEditing);
            Assert.IsFalse(viewModel.IsDeleting);
            Assert.IsTrue(viewModel.IsExporting);
        }

        #endregion

        #region BR78-BR87 - Additional Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR78")]
        [Description("TC_BR78_001: Verify ResetCommand is initialized")]
        public void TC_BR78_001_ResetCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ResetCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR79")]
        [Description("TC_BR79_001: Verify reset clears all fields")]
        public void TC_BR79_001_ResetCommand_ClearsAllFields()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.DishName = "Test";
            viewModel.UnitPrice = "100000";
            viewModel.Note = "Note";
            viewModel.SelectedItem = viewModel.DishList.First();

            // Act
            viewModel.ResetCommand.Execute(null);

            // Assert
            Assert.IsNull(viewModel.SelectedItem);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.DishName));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.UnitPrice));
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.Note));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR80")]
        [Description("TC_BR80_001: Verify DishName raises PropertyChanged")]
        public void TC_BR80_001_DishName_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.DishName))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.DishName = "New Dish";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR81")]
        [Description("TC_BR81_001: Verify UnitPrice raises PropertyChanged")]
        public void TC_BR81_001_UnitPrice_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.UnitPrice))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.UnitPrice = "200000";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR82")]
        [Description("TC_BR82_001: Verify SelectedItem raises PropertyChanged")]
        public void TC_BR82_001_SelectedItem_RaisesPropertyChanged()
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
            viewModel.SelectedItem = viewModel.DishList.First();

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR84")]
        [Description("TC_BR84_001: Verify SearchText raises PropertyChanged")]
        public void TC_BR84_001_SearchText_RaisesPropertyChanged()
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
        [TestCategory("FoodViewModel")]
        [TestCategory("BR85")]
        [Description("TC_BR85_001: Verify ExportToExcelCommand is initialized")]
        public void TC_BR85_001_ExportToExcelCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ExportToExcelCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR86")]
        [Description("TC_BR86_001: Verify search properties list is initialized")]
        public void TC_BR86_001_SearchProperties_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SearchProperties);
            Assert.AreEqual(3, viewModel.SearchProperties.Count, "Should have 3 search properties");
            // Verify properties exist without hardcoding Vietnamese strings
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[0]), "First search property should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[1]), "Second search property should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[2]), "Third search property should not be empty");
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR87")]
        [Description("TC_BR87_001: Verify default search property is selected")]
        public void TC_BR87_001_SelectedSearchProperty_HasDefault()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SelectedSearchProperty);
            Assert.IsTrue(viewModel.SearchProperties.Contains(viewModel.SelectedSearchProperty));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("FoodViewModel")]
        [TestCategory("BR87")]
        [Description("TC_BR87_002: Verify reset when changing actions")]
        public void TC_BR87_002_SelectedAction_TriggersReset()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.DishName = "Test";
            viewModel.SelectedAction = viewModel.ActionList[0]; // "Thêm"

            // Act
            viewModel.SelectedAction = viewModel.ActionList[1]; // "Sửa"

            // Assert - Should reset when action changes
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.DishName));
        }

        #endregion

        #region Helper Methods

        private FoodViewModel CreateViewModel()
        {
            return new FoodViewModel(
                _mockDishService.Object,
                _mockMenuService.Object);
        }

        private List<DishDTO> CreateSampleDishes()
        {
            return new List<DishDTO>
            {
                new DishDTO
                {
                    DishId = 1,
                    DishName = "Grilled Chicken",
                    UnitPrice = 150000,
                    Note = "Special dish"
                },
                new DishDTO
                {
                    DishId = 2,
                    DishName = "Steamed Fish",
                    UnitPrice = 200000,
                    Note = "Fresh daily"
                },
                new DishDTO
                {
                    DishId = 3,
                    DishName = "Vegetable Soup",
                    UnitPrice = 80000,
                    Note = "Healthy option"
                },
                new DishDTO
                {
                    DishId = 4,
                    DishName = "Beef Steak",
                    UnitPrice = 350000,
                    Note = "Premium quality"
                },
                new DishDTO
                {
                    DishId = 5,
                    DishName = "Fried Rice",
                    UnitPrice = 70000,
                    Note = "Popular choice"
                }
            };
        }

        #endregion
    }
}
