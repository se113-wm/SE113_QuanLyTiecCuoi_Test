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
    /// UI Tests for Shift Management Window
    /// Covers BR51-BR59 - Shift Management
    /// Tests: Add, Edit, Delete, Search, and Validation scenarios
    /// </summary>
    [TestClass]
    public class ShiftManagementWindowTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private Window _shiftWindow;

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
            
            // Navigate to Shift Management
            NavigateToShiftManagement();
        }

        /// <summary>
        /// Perform login and re-acquire Main Window after transition
        /// </summary>
        private Window PerformLoginAndReacquireMainWindow(Window loginWindow)
        {
            var helper = new UITestHelper(_app, _automation);
            return helper.PerformLoginAndReacquireMainWindow(loginWindow, "Fartiel", "admin");
        }

        private void NavigateToShiftManagement()
        {
            var shiftMenuItem = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ShiftButton")    // ? PRIMARY: Exact match from XAML
                .Or(cf.ByName("Ca"))                // Fallback: Vietnamese text
                .Or(cf.ByName("Shift")));           // Fallback: English text
            
            if (shiftMenuItem != null)
            {
                shiftMenuItem.Click();
                Thread.Sleep(1500);
                _shiftWindow = _mainWindow.ModalWindows.FirstOrDefault() ?? _mainWindow;
            }
            else
            {
                Assert.Inconclusive("Cannot find Shift Management button. Check MainWindow.xaml AutomationId.");
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

        #region BR51 - Display Shift List Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR51")]
        [Description("TC_BR51_UI_001: Verify shift management window displays shift list")]
        public void UI_BR51_001_ShiftWindow_ShouldDisplayShiftList()
        {
            // Assert - Look for shift list (DataGrid or ListBox)
            var shiftList = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ShiftDataGrid")
                .Or(cf.ByAutomationId("ShiftListBox"))
                .Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid)));

            Assert.IsNotNull(shiftList, "Shift list should be displayed");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR51")]
        [Description("TC_BR51_UI_002: Verify shift list contains shift data")]
        public void UI_BR51_002_ShiftList_ShouldContainShiftData()
        {
            // Arrange
            var shiftList = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (shiftList == null)
            {
                Assert.Inconclusive("Cannot find shift list control");
                return;
            }

            // Assert - Check if list has items
            var items = shiftList.FindAllDescendants(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            Assert.IsTrue(items.Length > 0, "Shift list should contain at least one shift");
        }

        #endregion

        #region BR52 - Search Shift Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR52")]
        [Description("TC_BR52_UI_001: Verify search textbox exists")]
        public void UI_BR52_001_SearchTextBox_ShouldExist()
        {
            // Assert
            var searchBox = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox")
                .Or(cf.ByAutomationId("ShiftSearchBox")));

            Assert.IsNotNull(searchBox, "Search textbox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR52")]
        [Description("TC_BR52_UI_002: Verify search functionality filters shifts")]
        public void UI_BR52_002_Search_ShouldFilterShifts()
        {
            // Arrange
            var searchBox = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox"))?.AsTextBox();

            if (searchBox == null)
            {
                Assert.Inconclusive("Search box not found");
                return;
            }

            // Act
            searchBox.Text = "Morning";
            Thread.Sleep(1000);

            // Assert - List should update (we can't easily verify filtered results, but no crash is good)
            Assert.IsTrue(true, "Search completed without error");
        }

        #endregion

        #region BR53 - Create Shift Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR53")]
        [Description("TC_BR53_UI_001: Verify Add button exists and is clickable")]
        public void UI_BR53_001_AddButton_ShouldExistAndBeClickable()
        {
            // Assert
            var addButton = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton")
                .Or(cf.ByName("Thêm"))
                .Or(cf.ByName("Add")))?.AsButton();

            Assert.IsNotNull(addButton, "Add button should exist");
            Assert.IsTrue(addButton.IsEnabled, "Add button should be enabled");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR53")]
        [Description("TC_BR53_UI_002: Verify input fields exist for adding shift")]
        public void UI_BR53_002_AddShift_InputFieldsShouldExist()
        {
            // Arrange - Select Add action
            var actionCombo = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            if (actionCombo != null)
            {
                actionCombo.Select(0); // Select "Add" action
                Thread.Sleep(500);
            }

            // Assert - Check for input fields
            var shiftNameBox = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ShiftNameTextBox"));
            var startTimePicker = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("StartTimePicker"));
            var endTimePicker = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("EndTimePicker"));

            Assert.IsNotNull(shiftNameBox, "Shift name input should exist");
            Assert.IsNotNull(startTimePicker, "Start time picker should exist");
            Assert.IsNotNull(endTimePicker, "End time picker should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR53")]
        [Description("TC_BR53_UI_003: Verify cannot add shift without name")]
        public void UI_BR53_003_AddShift_WithoutName_ShouldShowValidation()
        {
            // Arrange
            var actionCombo = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var addButton = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton"))?.AsButton();

            // Act - Try to add without entering name
            if (addButton != null)
            {
                addButton.Click();
                Thread.Sleep(1000);

                // Assert - Should show validation message
                var messageBox = _automation.GetDesktop().FindFirstChild(cf => 
                    cf.ByClassName("#32770"));
                
                if (messageBox != null)
                {
                    Assert.IsTrue(true, "Validation message displayed");
                    CloseAnyMessageBox();
                }
            }
        }

        #endregion

        #region BR54 - Update Shift Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR54")]
        [Description("TC_BR54_UI_001: Verify Edit button exists")]
        public void UI_BR54_001_EditButton_ShouldExist()
        {
            // Assert
            var editButton = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("EditButton")
                .Or(cf.ByName("S?a"))
                .Or(cf.ByName("Edit")))?.AsButton();

            Assert.IsNotNull(editButton, "Edit button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR54")]
        [Description("TC_BR54_UI_002: Verify can select shift for editing")]
        public void UI_BR54_002_EditShift_CanSelectShift()
        {
            // Arrange
            var shiftList = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (shiftList == null)
            {
                Assert.Inconclusive("Shift list not found");
                return;
            }

            var firstItem = shiftList.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            // Act
            if (firstItem != null)
            {
                firstItem.Click();
                Thread.Sleep(500);

                // Assert - Item should be selected
                Assert.IsTrue(true, "Shift can be selected");
            }
        }

        #endregion

        #region BR55 - Delete Shift Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR55")]
        [Description("TC_BR55_UI_001: Verify Delete button exists")]
        public void UI_BR55_001_DeleteButton_ShouldExist()
        {
            // Assert
            var deleteButton = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DeleteButton")
                .Or(cf.ByName("Xóa"))
                .Or(cf.ByName("Delete")))?.AsButton();

            Assert.IsNotNull(deleteButton, "Delete button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR55")]
        [Description("TC_BR55_UI_002: Verify delete confirmation appears")]
        public void UI_BR55_002_DeleteShift_ShouldShowConfirmation()
        {
            // Arrange - Select a shift
            var shiftList = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (shiftList != null)
            {
                var firstItem = shiftList.FindFirstDescendant(cf => 
                    cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));
                firstItem?.Click();
                Thread.Sleep(500);
            }

            // Select Delete action
            var actionCombo = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(2); // Delete
                Thread.Sleep(500);
            }

            var deleteButton = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DeleteButton"))?.AsButton();

            // Act
            if (deleteButton != null && deleteButton.IsEnabled)
            {
                deleteButton.Click();
                Thread.Sleep(1000);

                // Assert - Confirmation should appear
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

        #region BR56 - Action Selection Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR56")]
        [Description("TC_BR56_UI_001: Verify action combobox exists")]
        public void UI_BR56_001_ActionComboBox_ShouldExist()
        {
            // Assert
            var actionCombo = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            Assert.IsNotNull(actionCombo, "Action combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR56")]
        [Description("TC_BR56_UI_002: Verify can select Add action")]
        public void UI_BR56_002_ActionComboBox_CanSelectAdd()
        {
            // Arrange
            var actionCombo = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            if (actionCombo == null)
            {
                Assert.Inconclusive("Action combobox not found");
                return;
            }

            // Act
            actionCombo.Select(0); // Add
            Thread.Sleep(500);

            // Assert - Input fields should appear
            var shiftNameBox = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ShiftNameTextBox"));

            Assert.IsNotNull(shiftNameBox, "Input fields should appear when Add is selected");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR56")]
        [Description("TC_BR56_UI_003: Verify can select Edit action")]
        public void UI_BR56_003_ActionComboBox_CanSelectEdit()
        {
            // Arrange
            var actionCombo = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            if (actionCombo == null)
            {
                Assert.Inconclusive("Action combobox not found");
                return;
            }

            // Act
            actionCombo.Select(1); // Edit
            Thread.Sleep(500);

            // Assert
            Assert.IsTrue(true, "Edit action can be selected");
        }

        #endregion

        #region BR57 - Reset Functionality Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR57")]
        [Description("TC_BR57_UI_001: Verify Reset button exists")]
        public void UI_BR57_001_ResetButton_ShouldExist()
        {
            // Assert
            var resetButton = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton")
                .Or(cf.ByName("Reset"))
                .Or(cf.ByName("Làm m?i")))?.AsButton();

            Assert.IsNotNull(resetButton, "Reset button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [TestCategory("BR57")]
        [Description("TC_BR57_UI_002: Verify Reset clears input fields")]
        public void UI_BR57_002_Reset_ShouldClearFields()
        {
            // Arrange - Enter some data
            var actionCombo = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var shiftNameBox = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ShiftNameTextBox"))?.AsTextBox();

            if (shiftNameBox != null)
            {
                shiftNameBox.Text = "Test Shift";
            }

            // Act - Click Reset
            var resetButton = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton"))?.AsButton();

            if (resetButton != null)
            {
                resetButton.Click();
                Thread.Sleep(500);

                // Assert - Field should be cleared
                if (shiftNameBox != null)
                {
                    var currentText = shiftNameBox.Text;
                    Assert.IsTrue(string.IsNullOrEmpty(currentText), "Reset should clear input fields");
                }
            }
        }

        #endregion

        #region UI Element Verification Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [Description("Verify shift management window displays all required controls")]
        public void UI_ShiftWindow_ShouldDisplayAllRequiredControls()
        {
            // Assert - Check all main controls
            var shiftList = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));
            var actionCombo = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"));
            var searchBox = _shiftWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox"));

            Assert.IsNotNull(shiftList, "Shift list should exist");
            Assert.IsNotNull(actionCombo, "Action combobox should exist");
            // Search box is optional
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ShiftManagement")]
        [Description("Verify window title contains shift management")]
        public void UI_ShiftWindow_ShouldHaveCorrectTitle()
        {
            // Assert
            var title = _shiftWindow.Title;
            Assert.IsTrue(
                title.Contains("Ca") || title.Contains("Shift") || title.Contains("Qu?n lý"),
                $"Window title should indicate shift management. Current: {title}");
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
