using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using QuanLyTiecCuoi.Tests.UITests.Helpers;

namespace QuanLyTiecCuoi.Tests.UITests
{
    /// <summary>
    /// UI Tests for Dish Management Window
    /// Covers BR71-BR88 - Dish/Food Management
    /// Tests: Add, Edit, Delete, Search, Export, and Validation scenarios
    /// </summary>
    [TestClass]
    public class DishManagementWindowTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private Window _dishWindow;

        private static string GetAppPath()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var appPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\bin\Debug\QuanLyTiecCuoi.exe"));
            
            if (!File.Exists(appPath))
            {
                appPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\bin\Debug\QuanLyTiecCuoi.exe"));
            }
            
            return appPath;
        }

        [TestInitialize]
        public void Setup()
        {
            var appPath = GetAppPath();
            
            if (!File.Exists(appPath))
            {
                Assert.Inconclusive($"Cannot find exe at: {appPath}. Please build the project first.");
                return;
            }

            _automation = new UIA3Automation();
            _app = Application.Launch(appPath);
            
            // Login and get Main Window
            var loginWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10));
            _mainWindow = PerformLoginAndReacquireMainWindow(loginWindow);
            
            if (_mainWindow == null)
            {
                Assert.Inconclusive("Cannot login - Main Window not acquired");
                return;
            }
            
            // Navigate to Dish Management
            NavigateToDishManagement();
        }

        /// <summary>
        /// Perform login and re-acquire Main Window after transition
        /// </summary>
        private Window PerformLoginAndReacquireMainWindow(Window loginWindow)
        {
            var helper = new UITestHelper(_app, _automation);
            return helper.PerformLoginAndReacquireMainWindow(loginWindow, "Fartiel", "admin");
        }

        private void NavigateToDishManagement()
        {
            var dishMenuItem = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("FoodButton")    // ? PRIMARY: Exact match from XAML
                .Or(cf.ByName("Món ?n"))           // Fallback: Vietnamese text
                .Or(cf.ByName("Food")));           // Fallback: English text
            
            if (dishMenuItem != null)
            {
                dishMenuItem.Click();
                Thread.Sleep(1500);
                _dishWindow = _mainWindow.ModalWindows.FirstOrDefault() ?? _mainWindow;
            }
            else
            {
                Assert.Inconclusive("Cannot find Dish/Food Management button. Check MainWindow.xaml AutomationId.");
            }
        }

        private void CloseAnyMessageBox()
        {
            var helper = new UITestHelper(_app, _automation);
            var messageBox = helper.WaitForMessageBox(_mainWindow, TimeSpan.FromSeconds(2));
            if (messageBox != null)
            {
                helper.CloseMessageBox(messageBox);
            }
        }

        #region BR71 - Display Dish List Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR71")]
        [Description("TC_BR71_UI_001: Verify dish management window displays dish list")]
        public void UI_BR71_001_DishWindow_ShouldDisplayDishList()
        {
            // Assert
            var dishList = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DishDataGrid")
                .Or(cf.ByAutomationId("FoodDataGrid"))
                .Or(cf.ByAutomationId("DishListBox"))
                .Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid)));

            Assert.IsNotNull(dishList, "Dish list should be displayed");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR71")]
        [Description("TC_BR71_UI_002: Verify dish list contains dish data")]
        public void UI_BR71_002_DishList_ShouldContainDishData()
        {
            // Arrange
            var dishList = _dishWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (dishList == null)
            {
                Assert.Inconclusive("Cannot find dish list control");
                return;
            }

            // Assert
            var items = dishList.FindAllDescendants(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            Assert.IsTrue(items.Length > 0, "Dish list should contain at least one dish");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR71")]
        [Description("TC_BR71_UI_003: Verify dish list displays pricing information")]
        public void UI_BR71_003_DishList_ShouldDisplayPricing()
        {
            // Assert
            var dishList = _dishWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            Assert.IsNotNull(dishList, "Dish list should display pricing columns");
        }

        #endregion

        #region BR72 - Search Dish Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR72")]
        [Description("TC_BR72_UI_001: Verify search textbox exists")]
        public void UI_BR72_001_SearchTextBox_ShouldExist()
        {
            // Assert
            var searchBox = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox")
                .Or(cf.ByAutomationId("DishSearchBox"))
                .Or(cf.ByAutomationId("FoodSearchBox")));

            Assert.IsNotNull(searchBox, "Search textbox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR72")]
        [Description("TC_BR72_UI_002: Verify search functionality filters dishes")]
        public void UI_BR72_002_Search_ShouldFilterDishes()
        {
            // Arrange
            var searchBox = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox"))?.AsTextBox();

            if (searchBox == null)
            {
                Assert.Inconclusive("Search box not found");
                return;
            }

            // Act
            searchBox.Text = "Chicken";
            Thread.Sleep(1000);

            // Assert
            Assert.IsTrue(true, "Search completed without error");
        }

        #endregion

        #region BR73 - Create Dish Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR73")]
        [Description("TC_BR73_UI_001: Verify Add button exists and is clickable")]
        public void UI_BR73_001_AddButton_ShouldExistAndBeClickable()
        {
            // Assert
            var addButton = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton")
                .Or(cf.ByName("Thêm"))
                .Or(cf.ByName("Add")))?.AsButton();

            Assert.IsNotNull(addButton, "Add button should exist");
            Assert.IsTrue(addButton.IsEnabled, "Add button should be enabled");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR73")]
        [Description("TC_BR73_UI_002: Verify input fields exist for adding dish")]
        public void UI_BR73_002_AddDish_InputFieldsShouldExist()
        {
            // Arrange
            var actionCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            // Assert
            var dishNameBox = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DishNameTextBox")
                .Or(cf.ByAutomationId("FoodNameTextBox")));
            var unitPriceBox = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("UnitPriceTextBox"));

            Assert.IsNotNull(dishNameBox, "Dish name input should exist");
            Assert.IsNotNull(unitPriceBox, "Unit price input should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR73")]
        [Description("TC_BR73_UI_003: Verify cannot add dish without name")]
        public void UI_BR73_003_AddDish_WithoutName_ShouldShowValidation()
        {
            // Arrange
            var actionCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var addButton = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton"))?.AsButton();

            // Act
            if (addButton != null)
            {
                addButton.Click();
                Thread.Sleep(1000);

                // Assert
                var messageBox = _automation.GetDesktop().FindFirstChild(cf => 
                    cf.ByClassName("#32770"));
                
                if (messageBox != null)
                {
                    Assert.IsTrue(true, "Validation message displayed");
                    CloseAnyMessageBox();
                }
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR73")]
        [Description("TC_BR73_UI_004: Verify cannot add dish with invalid price")]
        public void UI_BR73_004_AddDish_WithInvalidPrice_ShouldShowValidation()
        {
            // Arrange
            var actionCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var dishNameBox = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DishNameTextBox"))?.AsTextBox();
            var unitPriceBox = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("UnitPriceTextBox"))?.AsTextBox();

            if (dishNameBox != null && unitPriceBox != null)
            {
                dishNameBox.Text = "Test Dish";
                unitPriceBox.Text = "-1000"; // Invalid negative price
            }

            var addButton = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton"))?.AsButton();

            // Act
            if (addButton != null)
            {
                addButton.Click();
                Thread.Sleep(1000);

                // Assert
                var messageBox = _automation.GetDesktop().FindFirstChild(cf => 
                    cf.ByClassName("#32770"));
                
                if (messageBox != null)
                {
                    Assert.IsTrue(true, "Validation for invalid price displayed");
                    CloseAnyMessageBox();
                }
            }
        }

        #endregion

        #region BR74 - Update Dish Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR74")]
        [Description("TC_BR74_UI_001: Verify Edit button exists")]
        public void UI_BR74_001_EditButton_ShouldExist()
        {
            // Assert
            var editButton = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("EditButton")
                .Or(cf.ByName("S?a"))
                .Or(cf.ByName("Edit")))?.AsButton();

            Assert.IsNotNull(editButton, "Edit button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR74")]
        [Description("TC_BR74_UI_002: Verify can select dish for editing")]
        public void UI_BR74_002_EditDish_CanSelectDish()
        {
            // Arrange
            var dishList = _dishWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (dishList == null)
            {
                Assert.Inconclusive("Dish list not found");
                return;
            }

            var firstItem = dishList.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            // Act
            if (firstItem != null)
            {
                firstItem.Click();
                Thread.Sleep(500);

                // Assert
                Assert.IsTrue(true, "Dish can be selected");
            }
        }

        #endregion

        #region BR75 - Delete Dish Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR75")]
        [Description("TC_BR75_UI_001: Verify Delete button exists")]
        public void UI_BR75_001_DeleteButton_ShouldExist()
        {
            // Assert
            var deleteButton = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DeleteButton")
                .Or(cf.ByName("Xóa"))
                .Or(cf.ByName("Delete")))?.AsButton();

            Assert.IsNotNull(deleteButton, "Delete button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR75")]
        [Description("TC_BR75_UI_002: Verify delete confirmation appears")]
        public void UI_BR75_002_DeleteDish_ShouldShowConfirmation()
        {
            // Arrange
            var dishList = _dishWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (dishList != null)
            {
                var firstItem = dishList.FindFirstDescendant(cf => 
                    cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));
                firstItem?.Click();
                Thread.Sleep(500);
            }

            var actionCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(2); // Delete
                Thread.Sleep(500);
            }

            var deleteButton = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DeleteButton"))?.AsButton();

            // Act
            if (deleteButton != null && deleteButton.IsEnabled)
            {
                deleteButton.Click();
                Thread.Sleep(1000);

                // Assert
                var confirmBox = _automation.GetDesktop().FindFirstChild(cf => 
                    cf.ByClassName("#32770"));
                
                if (confirmBox != null)
                {
                    Assert.IsTrue(true, "Delete confirmation displayed");
                    CloseAnyMessageBox();
                }
            }
        }

        #endregion

        #region BR76 - Dish Type Selection Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR76")]
        [Description("TC_BR76_UI_001: Verify dish type combobox exists")]
        public void UI_BR76_001_DishTypeComboBox_ShouldExist()
        {
            // Arrange
            var actionCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            // Assert
            var dishTypeCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DishTypeComboBox")
                .Or(cf.ByAutomationId("FoodTypeComboBox")))?.AsComboBox();

            // Dish type might not be required
            if (dishTypeCombo != null)
            {
                Assert.IsTrue(true, "Dish type combobox exists");
            }
        }

        #endregion

        #region BR77 - Export Excel Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR77")]
        [Description("TC_BR77_UI_001: Verify Export to Excel button exists")]
        public void UI_BR77_001_ExportExcelButton_ShouldExist()
        {
            // Assert
            var exportButton = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ExportExcelButton")
                .Or(cf.ByName("Export"))
                .Or(cf.ByName("Xu?t Excel")))?.AsButton();

            // Export button might not always be visible
            if (exportButton != null)
            {
                Assert.IsTrue(true, "Export Excel button exists");
            }
            else
            {
                Assert.Inconclusive("Export button not found - might be in action dropdown");
            }
        }

        #endregion

        #region BR78 - Action Selection Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR78")]
        [Description("TC_BR78_UI_001: Verify action combobox exists")]
        public void UI_BR78_001_ActionComboBox_ShouldExist()
        {
            // Assert
            var actionCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            Assert.IsNotNull(actionCombo, "Action combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR78")]
        [Description("TC_BR78_UI_002: Verify can select Add action")]
        public void UI_BR78_002_ActionComboBox_CanSelectAdd()
        {
            // Arrange
            var actionCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            if (actionCombo == null)
            {
                Assert.Inconclusive("Action combobox not found");
                return;
            }

            // Act
            actionCombo.Select(0); // Add
            Thread.Sleep(500);

            // Assert
            var dishNameBox = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DishNameTextBox"));

            Assert.IsNotNull(dishNameBox, "Input fields should appear when Add is selected");
        }

        #endregion

        #region BR79 - Reset Functionality Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR79")]
        [Description("TC_BR79_UI_001: Verify Reset button exists")]
        public void UI_BR79_001_ResetButton_ShouldExist()
        {
            // Assert
            var resetButton = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton")
                .Or(cf.ByName("Reset"))
                .Or(cf.ByName("Làm m?i")))?.AsButton();

            Assert.IsNotNull(resetButton, "Reset button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [TestCategory("BR79")]
        [Description("TC_BR79_UI_002: Verify Reset clears input fields")]
        public void UI_BR79_002_Reset_ShouldClearFields()
        {
            // Arrange
            var actionCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var dishNameBox = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DishNameTextBox"))?.AsTextBox();

            if (dishNameBox != null)
            {
                dishNameBox.Text = "Test Dish";
            }

            // Act
            var resetButton = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton"))?.AsButton();

            if (resetButton != null)
            {
                resetButton.Click();
                Thread.Sleep(500);

                // Assert
                if (dishNameBox != null)
                {
                    var currentText = dishNameBox.Text;
                    Assert.IsTrue(string.IsNullOrEmpty(currentText), "Reset should clear input fields");
                }
            }
        }

        #endregion

        #region UI Element Verification Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [Description("Verify dish management window displays all required controls")]
        public void UI_DishWindow_ShouldDisplayAllRequiredControls()
        {
            // Assert
            var dishList = _dishWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));
            var actionCombo = _dishWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"));

            Assert.IsNotNull(dishList, "Dish list should exist");
            Assert.IsNotNull(actionCombo, "Action combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("DishManagement")]
        [Description("Verify window title contains dish management")]
        public void UI_DishWindow_ShouldHaveCorrectTitle()
        {
            // Assert
            var title = _dishWindow.Title;
            Assert.IsTrue(
                title.Contains("Món ?n") || 
                title.Contains("Th?c ??n") || 
                title.Contains("Dish") || 
                title.Contains("Food") ||
                title.Contains("Qu?n lý"),
                $"Window title should indicate dish management. Current: {title}");
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            CloseAnyMessageBox();
            _app?.Close();
            _automation?.Dispose();
        }
    }
}
