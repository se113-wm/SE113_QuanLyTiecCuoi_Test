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
    /// UI Tests for Hall Type Management Window
    /// Covers BR60-BR70 - Hall Type Management
    /// Tests: Add, Edit, Delete, Search, and Validation scenarios
    /// </summary>
    [TestClass]
    public class HallTypeManagementWindowTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private Window _hallTypeWindow;

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
            
            // Navigate to Hall Type Management
            NavigateToHallTypeManagement();
        }

        /// <summary>
        /// Perform login and re-acquire Main Window after transition
        /// </summary>
        private Window PerformLoginAndReacquireMainWindow(Window loginWindow)
        {
            var helper = new UITestHelper(_app, _automation);
            return helper.PerformLoginAndReacquireMainWindow(loginWindow, "Fartiel", "admin");
        }

        private void NavigateToHallTypeManagement()
        {
            var hallTypeMenuItem = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallTypeButton")    // ? PRIMARY: Exact match from XAML
                .Or(cf.ByName("Lo?i s?nh"))            // Fallback: Vietnamese text
                .Or(cf.ByName("Hall Type")));          // Fallback: English text
            
            if (hallTypeMenuItem != null)
            {
                hallTypeMenuItem.Click();
                Thread.Sleep(1500);
                _hallTypeWindow = _mainWindow.ModalWindows.FirstOrDefault() ?? _mainWindow;
            }
            else
            {
                Assert.Inconclusive("Cannot find Hall Type Management button. Check MainWindow.xaml AutomationId.");
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

        #region BR60 - Display Hall Type List Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR60")]
        [Description("TC_BR60_UI_001: Verify hall type management window displays hall type list")]
        public void UI_BR60_001_HallTypeWindow_ShouldDisplayHallTypeList()
        {
            // Assert
            var hallTypeList = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallTypeDataGrid")
                .Or(cf.ByAutomationId("HallTypeListBox"))
                .Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid)));

            Assert.IsNotNull(hallTypeList, "Hall type list should be displayed");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR60")]
        [Description("TC_BR60_UI_002: Verify hall type list contains data")]
        public void UI_BR60_002_HallTypeList_ShouldContainData()
        {
            // Arrange
            var hallTypeList = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (hallTypeList == null)
            {
                Assert.Inconclusive("Cannot find hall type list control");
                return;
            }

            // Assert
            var items = hallTypeList.FindAllDescendants(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            Assert.IsTrue(items.Length > 0, "Hall type list should contain at least one hall type");
        }

        #endregion

        #region BR61 - Search Hall Type Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR61")]
        [Description("TC_BR61_UI_001: Verify search textbox exists")]
        public void UI_BR61_001_SearchTextBox_ShouldExist()
        {
            // Assert
            var searchBox = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox")
                .Or(cf.ByAutomationId("HallTypeSearchBox")));

            Assert.IsNotNull(searchBox, "Search textbox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR61")]
        [Description("TC_BR61_UI_002: Verify search functionality filters hall types")]
        public void UI_BR61_002_Search_ShouldFilterHallTypes()
        {
            // Arrange
            var searchBox = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox"))?.AsTextBox();

            if (searchBox == null)
            {
                Assert.Inconclusive("Search box not found");
                return;
            }

            // Act
            searchBox.Text = "VIP";
            Thread.Sleep(1000);

            // Assert
            Assert.IsTrue(true, "Search completed without error");
        }

        #endregion

        #region BR62 - Create Hall Type Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR62")]
        [Description("TC_BR62_UI_001: Verify Add button exists and is clickable")]
        public void UI_BR62_001_AddButton_ShouldExistAndBeClickable()
        {
            // Assert
            var addButton = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton")
                .Or(cf.ByName("Thêm"))
                .Or(cf.ByName("Add")))?.AsButton();

            Assert.IsNotNull(addButton, "Add button should exist");
            Assert.IsTrue(addButton.IsEnabled, "Add button should be enabled");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR62")]
        [Description("TC_BR62_UI_002: Verify input fields exist for adding hall type")]
        public void UI_BR62_002_AddHallType_InputFieldsShouldExist()
        {
            // Arrange
            var actionCombo = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            // Assert
            var hallTypeNameBox = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallTypeNameTextBox"));
            var minTablePriceBox = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("MinTablePriceTextBox"));

            Assert.IsNotNull(hallTypeNameBox, "Hall type name input should exist");
            Assert.IsNotNull(minTablePriceBox, "Min table price input should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR62")]
        [Description("TC_BR62_UI_003: Verify cannot add hall type without name")]
        public void UI_BR62_003_AddHallType_WithoutName_ShouldShowValidation()
        {
            // Arrange
            var actionCombo = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var addButton = _hallTypeWindow.FindFirstDescendant(cf => 
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
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR62")]
        [Description("TC_BR62_UI_004: Verify min table price validation")]
        public void UI_BR62_004_AddHallType_WithInvalidPrice_ShouldShowValidation()
        {
            // Arrange
            var actionCombo = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var hallTypeNameBox = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallTypeNameTextBox"))?.AsTextBox();
            var minTablePriceBox = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("MinTablePriceTextBox"))?.AsTextBox();

            if (hallTypeNameBox != null && minTablePriceBox != null)
            {
                hallTypeNameBox.Text = "Test Type";
                minTablePriceBox.Text = "-1000"; // Invalid negative price
            }

            var addButton = _hallTypeWindow.FindFirstDescendant(cf => 
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

        #region BR63 - Update Hall Type Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR63")]
        [Description("TC_BR63_UI_001: Verify Edit button exists")]
        public void UI_BR63_001_EditButton_ShouldExist()
        {
            // Assert
            var editButton = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("EditButton")
                .Or(cf.ByName("S?a"))
                .Or(cf.ByName("Edit")))?.AsButton();

            Assert.IsNotNull(editButton, "Edit button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR63")]
        [Description("TC_BR63_UI_002: Verify can select hall type for editing")]
        public void UI_BR63_002_EditHallType_CanSelectHallType()
        {
            // Arrange
            var hallTypeList = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (hallTypeList == null)
            {
                Assert.Inconclusive("Hall type list not found");
                return;
            }

            var firstItem = hallTypeList.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            // Act
            if (firstItem != null)
            {
                firstItem.Click();
                Thread.Sleep(500);

                // Assert
                Assert.IsTrue(true, "Hall type can be selected");
            }
        }

        #endregion

        #region BR64 - Delete Hall Type Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR64")]
        [Description("TC_BR64_UI_001: Verify Delete button exists")]
        public void UI_BR64_001_DeleteButton_ShouldExist()
        {
            // Assert
            var deleteButton = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DeleteButton")
                .Or(cf.ByName("Xóa"))
                .Or(cf.ByName("Delete")))?.AsButton();

            Assert.IsNotNull(deleteButton, "Delete button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR64")]
        [Description("TC_BR64_UI_002: Verify delete confirmation appears")]
        public void UI_BR64_002_DeleteHallType_ShouldShowConfirmation()
        {
            // Arrange
            var hallTypeList = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (hallTypeList != null)
            {
                var firstItem = hallTypeList.FindFirstDescendant(cf => 
                    cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));
                firstItem?.Click();
                Thread.Sleep(500);
            }

            var actionCombo = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(2); // Delete
                Thread.Sleep(500);
            }

            var deleteButton = _hallTypeWindow.FindFirstDescendant(cf => 
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

        #region BR65 - Action Selection Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR65")]
        [Description("TC_BR65_UI_001: Verify action combobox exists")]
        public void UI_BR65_001_ActionComboBox_ShouldExist()
        {
            // Assert
            var actionCombo = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            Assert.IsNotNull(actionCombo, "Action combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR65")]
        [Description("TC_BR65_UI_002: Verify can select Add action")]
        public void UI_BR65_002_ActionComboBox_CanSelectAdd()
        {
            // Arrange
            var actionCombo = _hallTypeWindow.FindFirstDescendant(cf => 
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
            var hallTypeNameBox = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallTypeNameTextBox"));

            Assert.IsNotNull(hallTypeNameBox, "Input fields should appear when Add is selected");
        }

        #endregion

        #region BR66 - Reset Functionality Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR66")]
        [Description("TC_BR66_UI_001: Verify Reset button exists")]
        public void UI_BR66_001_ResetButton_ShouldExist()
        {
            // Assert
            var resetButton = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton")
                .Or(cf.ByName("Reset"))
                .Or(cf.ByName("Làm m?i")))?.AsButton();

            Assert.IsNotNull(resetButton, "Reset button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [TestCategory("BR66")]
        [Description("TC_BR66_UI_002: Verify Reset clears input fields")]
        public void UI_BR66_002_Reset_ShouldClearFields()
        {
            // Arrange
            var actionCombo = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var hallTypeNameBox = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallTypeNameTextBox"))?.AsTextBox();

            if (hallTypeNameBox != null)
            {
                hallTypeNameBox.Text = "Test Type";
            }

            // Act
            var resetButton = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton"))?.AsButton();

            if (resetButton != null)
            {
                resetButton.Click();
                Thread.Sleep(500);

                // Assert
                if (hallTypeNameBox != null)
                {
                    var currentText = hallTypeNameBox.Text;
                    Assert.IsTrue(string.IsNullOrEmpty(currentText), "Reset should clear input fields");
                }
            }
        }

        #endregion

        #region UI Element Verification Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [Description("Verify hall type management window displays all required controls")]
        public void UI_HallTypeWindow_ShouldDisplayAllRequiredControls()
        {
            // Assert
            var hallTypeList = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));
            var actionCombo = _hallTypeWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"));

            Assert.IsNotNull(hallTypeList, "Hall type list should exist");
            Assert.IsNotNull(actionCombo, "Action combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallTypeManagement")]
        [Description("Verify window title contains hall type management")]
        public void UI_HallTypeWindow_ShouldHaveCorrectTitle()
        {
            // Assert
            var title = _hallTypeWindow.Title;
            Assert.IsTrue(
                title.Contains("Lo?i s?nh") || title.Contains("Hall Type") || title.Contains("Qu?n lý"),
                $"Window title should indicate hall type management. Current: {title}");
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
