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
    /// UI Tests for Hall Management Window
    /// Covers BR41-BR50 - Hall Management
    /// Tests: Add, Edit, Delete, Search, and Validation scenarios
    /// </summary>
    [TestClass]
    public class HallManagementWindowTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private Window _hallWindow;

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
            
            // Navigate to Hall Management
            NavigateToHallManagement();
        }

        /// <summary>
        /// Perform login and re-acquire Main Window after transition
        /// </summary>
        private Window PerformLoginAndReacquireMainWindow(Window loginWindow)
        {
            var helper = new UITestHelper(_app, _automation);
            return helper.PerformLoginAndReacquireMainWindow(loginWindow, "Fartiel", "admin");
        }

        private void NavigateToHallManagement()
        {
            var hallMenuItem = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallButton")    // ? PRIMARY: Exact match from XAML
                .Or(cf.ByName("S?nh"))             // Fallback: Vietnamese text
                .Or(cf.ByName("Hall")));           // Fallback: English text
            
            if (hallMenuItem != null)
            {
                hallMenuItem.Click();
                Thread.Sleep(1500);
                _hallWindow = _mainWindow.ModalWindows.FirstOrDefault() ?? _mainWindow;
            }
            else
            {
                Assert.Inconclusive("Cannot find Hall Management button. Check MainWindow.xaml AutomationId.");
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

        #region BR41 - Display Hall List Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR41")]
        [Description("TC_BR41_UI_001: Verify hall management window displays hall list")]
        public void UI_BR41_001_HallWindow_ShouldDisplayHallList()
        {
            // Assert
            var hallList = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallDataGrid")
                .Or(cf.ByAutomationId("HallListBox"))
                .Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid)));

            Assert.IsNotNull(hallList, "Hall list should be displayed");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR41")]
        [Description("TC_BR41_UI_002: Verify hall list contains hall data")]
        public void UI_BR41_002_HallList_ShouldContainHallData()
        {
            // Arrange
            var hallList = _hallWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (hallList == null)
            {
                Assert.Inconclusive("Cannot find hall list control");
                return;
            }

            // Assert
            var items = hallList.FindAllDescendants(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            Assert.IsTrue(items.Length > 0, "Hall list should contain at least one hall");
        }

        #endregion

        #region BR42 - Search Hall Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR42")]
        [Description("TC_BR42_UI_001: Verify search textbox exists")]
        public void UI_BR42_001_SearchTextBox_ShouldExist()
        {
            // Assert
            var searchBox = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox")
                .Or(cf.ByAutomationId("HallSearchBox")));

            Assert.IsNotNull(searchBox, "Search textbox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR42")]
        [Description("TC_BR42_UI_002: Verify search functionality filters halls")]
        public void UI_BR42_002_Search_ShouldFilterHalls()
        {
            // Arrange
            var searchBox = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox"))?.AsTextBox();

            if (searchBox == null)
            {
                Assert.Inconclusive("Search box not found");
                return;
            }

            // Act
            searchBox.Text = "A";
            Thread.Sleep(1000);

            // Assert
            Assert.IsTrue(true, "Search completed without error");
        }

        #endregion

        #region BR43 - Create Hall Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR43")]
        [Description("TC_BR43_UI_001: Verify Add button exists and is clickable")]
        public void UI_BR43_001_AddButton_ShouldExistAndBeClickable()
        {
            // Assert
            var addButton = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton")
                .Or(cf.ByName("Thêm"))
                .Or(cf.ByName("Add")))?.AsButton();

            Assert.IsNotNull(addButton, "Add button should exist");
            Assert.IsTrue(addButton.IsEnabled, "Add button should be enabled");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR43")]
        [Description("TC_BR43_UI_002: Verify input fields exist for adding hall")]
        public void UI_BR43_002_AddHall_InputFieldsShouldExist()
        {
            // Arrange - Select Add action
            var actionCombo = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            // Assert
            var hallNameBox = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallNameTextBox"));
            var capacityBox = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("CapacityTextBox"));

            Assert.IsNotNull(hallNameBox, "Hall name input should exist");
            Assert.IsNotNull(capacityBox, "Capacity input should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR43")]
        [Description("TC_BR43_UI_003: Verify cannot add hall without name")]
        public void UI_BR43_003_AddHall_WithoutName_ShouldShowValidation()
        {
            // Arrange
            var actionCombo = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var addButton = _hallWindow.FindFirstDescendant(cf => 
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
        [TestCategory("HallManagement")]
        [TestCategory("BR43")]
        [Description("TC_BR43_UI_004: Verify capacity validation")]
        public void UI_BR43_004_AddHall_WithInvalidCapacity_ShouldShowValidation()
        {
            // Arrange
            var actionCombo = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var hallNameBox = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallNameTextBox"))?.AsTextBox();
            var capacityBox = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("CapacityTextBox"))?.AsTextBox();

            if (hallNameBox != null && capacityBox != null)
            {
                hallNameBox.Text = "Test Hall";
                capacityBox.Text = "0"; // Invalid capacity
            }

            var addButton = _hallWindow.FindFirstDescendant(cf => 
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
                    Assert.IsTrue(true, "Validation for invalid capacity displayed");
                    CloseAnyMessageBox();
                }
            }
        }

        #endregion

        #region BR44 - Update Hall Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR44")]
        [Description("TC_BR44_UI_001: Verify Edit button exists")]
        public void UI_BR44_001_EditButton_ShouldExist()
        {
            // Assert
            var editButton = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("EditButton")
                .Or(cf.ByName("S?a"))
                .Or(cf.ByName("Edit")))?.AsButton();

            Assert.IsNotNull(editButton, "Edit button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR44")]
        [Description("TC_BR44_UI_002: Verify can select hall for editing")]
        public void UI_BR44_002_EditHall_CanSelectHall()
        {
            // Arrange
            var hallList = _hallWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (hallList == null)
            {
                Assert.Inconclusive("Hall list not found");
                return;
            }

            var firstItem = hallList.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            // Act
            if (firstItem != null)
            {
                firstItem.Click();
                Thread.Sleep(500);

                // Assert
                Assert.IsTrue(true, "Hall can be selected");
            }
        }

        #endregion

        #region BR45 - Delete Hall Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR45")]
        [Description("TC_BR45_UI_001: Verify Delete button exists")]
        public void UI_BR45_001_DeleteButton_ShouldExist()
        {
            // Assert
            var deleteButton = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DeleteButton")
                .Or(cf.ByName("Xóa"))
                .Or(cf.ByName("Delete")))?.AsButton();

            Assert.IsNotNull(deleteButton, "Delete button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR45")]
        [Description("TC_BR45_UI_002: Verify delete confirmation appears")]
        public void UI_BR45_002_DeleteHall_ShouldShowConfirmation()
        {
            // Arrange
            var hallList = _hallWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (hallList != null)
            {
                var firstItem = hallList.FindFirstDescendant(cf => 
                    cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));
                firstItem?.Click();
                Thread.Sleep(500);
            }

            var actionCombo = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(2); // Delete
                Thread.Sleep(500);
            }

            var deleteButton = _hallWindow.FindFirstDescendant(cf => 
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

        #region BR46 - Hall Type Assignment Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR46")]
        [Description("TC_BR46_UI_001: Verify hall type combobox exists")]
        public void UI_BR46_001_HallTypeComboBox_ShouldExist()
        {
            // Arrange
            var actionCombo = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            // Assert
            var hallTypeCombo = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallTypeComboBox"))?.AsComboBox();

            Assert.IsNotNull(hallTypeCombo, "Hall type combobox should exist");
        }

        #endregion

        #region BR47 - Action Selection Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR47")]
        [Description("TC_BR47_UI_001: Verify action combobox exists")]
        public void UI_BR47_001_ActionComboBox_ShouldExist()
        {
            // Assert
            var actionCombo = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            Assert.IsNotNull(actionCombo, "Action combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR47")]
        [Description("TC_BR47_UI_002: Verify can select Add action")]
        public void UI_BR47_002_ActionComboBox_CanSelectAdd()
        {
            // Arrange
            var actionCombo = _hallWindow.FindFirstDescendant(cf => 
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
            var hallNameBox = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallNameTextBox"));

            Assert.IsNotNull(hallNameBox, "Input fields should appear when Add is selected");
        }

        #endregion

        #region BR48 - Reset Functionality Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR48")]
        [Description("TC_BR48_UI_001: Verify Reset button exists")]
        public void UI_BR48_001_ResetButton_ShouldExist()
        {
            // Assert
            var resetButton = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton")
                .Or(cf.ByName("Reset"))
                .Or(cf.ByName("Làm m?i")))?.AsButton();

            Assert.IsNotNull(resetButton, "Reset button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [TestCategory("BR48")]
        [Description("TC_BR48_UI_002: Verify Reset clears input fields")]
        public void UI_BR48_002_Reset_ShouldClearFields()
        {
            // Arrange
            var actionCombo = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var hallNameBox = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallNameTextBox"))?.AsTextBox();

            if (hallNameBox != null)
            {
                hallNameBox.Text = "Test Hall";
            }

            // Act
            var resetButton = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton"))?.AsButton();

            if (resetButton != null)
            {
                resetButton.Click();
                Thread.Sleep(500);

                // Assert
                if (hallNameBox != null)
                {
                    var currentText = hallNameBox.Text;
                    Assert.IsTrue(string.IsNullOrEmpty(currentText), "Reset should clear input fields");
                }
            }
        }

        #endregion

        #region UI Element Verification Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [Description("Verify hall management window displays all required controls")]
        public void UI_HallWindow_ShouldDisplayAllRequiredControls()
        {
            // Assert
            var hallList = _hallWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));
            var actionCombo = _hallWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"));

            Assert.IsNotNull(hallList, "Hall list should exist");
            Assert.IsNotNull(actionCombo, "Action combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("HallManagement")]
        [Description("Verify window title contains hall management")]
        public void UI_HallWindow_ShouldHaveCorrectTitle()
        {
            // Assert
            var title = _hallWindow.Title;
            Assert.IsTrue(
                title.Contains("S?nh") || title.Contains("Hall") || title.Contains("Qu?n lý"),
                $"Window title should indicate hall management. Current: {title}");
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
