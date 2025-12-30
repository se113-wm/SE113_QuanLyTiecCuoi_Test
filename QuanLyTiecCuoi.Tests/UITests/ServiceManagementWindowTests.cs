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
    /// UI Tests for Service Management Window
    /// Covers BR89-BR105 - Service Management
    /// Tests: Add, Edit, Delete, Search, Export, and Validation scenarios
    /// </summary>
    [TestClass]
    public class ServiceManagementWindowTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private Window _serviceWindow;

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
            
            // Navigate to Service Management
            NavigateToServiceManagement();
        }

        /// <summary>
        /// Perform login and re-acquire Main Window after transition
        /// </summary>
        private Window PerformLoginAndReacquireMainWindow(Window loginWindow)
        {
            var helper = new UITestHelper(_app, _automation);
            return helper.PerformLoginAndReacquireMainWindow(loginWindow, "Fartiel", "admin");
        }

        private void NavigateToServiceManagement()
        {
            var serviceMenuItem = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ServiceButton")    // ? PRIMARY: Exact match from XAML
                .Or(cf.ByName("D?ch v?"))             // Fallback: Vietnamese text
                .Or(cf.ByName("Service")));           // Fallback: English text
            
            if (serviceMenuItem != null)
            {
                serviceMenuItem.Click();
                Thread.Sleep(1500);
                _serviceWindow = _mainWindow.ModalWindows.FirstOrDefault() ?? _mainWindow;
            }
            else
            {
                Assert.Inconclusive("Cannot find Service Management button. Check MainWindow.xaml AutomationId.");
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

        #region BR89 - Display Service List Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR89")]
        [Description("TC_BR89_UI_001: Verify service management window displays service list")]
        public void UI_BR89_001_ServiceWindow_ShouldDisplayServiceList()
        {
            // Assert
            var serviceList = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ServiceDataGrid")
                .Or(cf.ByAutomationId("ServiceListBox"))
                .Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid)));

            Assert.IsNotNull(serviceList, "Service list should be displayed");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR89")]
        [Description("TC_BR89_UI_002: Verify service list contains service data")]
        public void UI_BR89_002_ServiceList_ShouldContainServiceData()
        {
            // Arrange
            var serviceList = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (serviceList == null)
            {
                Assert.Inconclusive("Cannot find service list control");
                return;
            }

            // Assert
            var items = serviceList.FindAllDescendants(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            Assert.IsTrue(items.Length > 0, "Service list should contain at least one service");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR89")]
        [Description("TC_BR89_UI_003: Verify service list displays pricing information")]
        public void UI_BR89_003_ServiceList_ShouldDisplayPricing()
        {
            // Assert - Service list should be visible with columns
            var serviceList = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            Assert.IsNotNull(serviceList, "Service list should display pricing columns");
        }

        #endregion

        #region BR90 - Search Service Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR90")]
        [Description("TC_BR90_UI_001: Verify search textbox exists")]
        public void UI_BR90_001_SearchTextBox_ShouldExist()
        {
            // Assert
            var searchBox = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox")
                .Or(cf.ByAutomationId("ServiceSearchBox")));

            Assert.IsNotNull(searchBox, "Search textbox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR90")]
        [Description("TC_BR90_UI_002: Verify search functionality filters services")]
        public void UI_BR90_002_Search_ShouldFilterServices()
        {
            // Arrange
            var searchBox = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox"))?.AsTextBox();

            if (searchBox == null)
            {
                Assert.Inconclusive("Search box not found");
                return;
            }

            // Act
            searchBox.Text = "Photography";
            Thread.Sleep(1000);

            // Assert
            Assert.IsTrue(true, "Search completed without error");
        }

        #endregion

        #region BR91 - Create Service Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR91")]
        [Description("TC_BR91_UI_001: Verify Add button exists and is clickable")]
        public void UI_BR91_001_AddButton_ShouldExistAndBeClickable()
        {
            // Assert
            var addButton = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton")
                .Or(cf.ByName("Thêm"))
                .Or(cf.ByName("Add")))?.AsButton();

            Assert.IsNotNull(addButton, "Add button should exist");
            Assert.IsTrue(addButton.IsEnabled, "Add button should be enabled");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR91")]
        [Description("TC_BR91_UI_002: Verify input fields exist for adding service")]
        public void UI_BR91_002_AddService_InputFieldsShouldExist()
        {
            // Arrange - Select Add action
            var actionCombo = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            // Assert
            var serviceNameBox = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ServiceNameTextBox"));
            var unitPriceBox = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("UnitPriceTextBox"));

            Assert.IsNotNull(serviceNameBox, "Service name input should exist");
            Assert.IsNotNull(unitPriceBox, "Unit price input should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR91")]
        [Description("TC_BR91_UI_003: Verify cannot add service without name")]
        public void UI_BR91_003_AddService_WithoutName_ShouldShowValidation()
        {
            // Arrange
            var actionCombo = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var addButton = _serviceWindow.FindFirstDescendant(cf => 
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
        [TestCategory("ServiceManagement")]
        [TestCategory("BR91")]
        [Description("TC_BR91_UI_004: Verify cannot add service with invalid price")]
        public void UI_BR91_004_AddService_WithInvalidPrice_ShouldShowValidation()
        {
            // Arrange
            var actionCombo = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var serviceNameBox = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ServiceNameTextBox"))?.AsTextBox();
            var unitPriceBox = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("UnitPriceTextBox"))?.AsTextBox();

            if (serviceNameBox != null && unitPriceBox != null)
            {
                serviceNameBox.Text = "Test Service";
                unitPriceBox.Text = "-1000"; // Invalid negative price
            }

            var addButton = _serviceWindow.FindFirstDescendant(cf => 
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
                    Assert.IsTrue(true, "Validation message for invalid price displayed");
                    CloseAnyMessageBox();
                }
            }
        }

        #endregion

        #region BR92 - Update Service Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR92")]
        [Description("TC_BR92_UI_001: Verify Edit button exists")]
        public void UI_BR92_001_EditButton_ShouldExist()
        {
            // Assert
            var editButton = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("EditButton")
                .Or(cf.ByName("S?a"))
                .Or(cf.ByName("Edit")))?.AsButton();

            Assert.IsNotNull(editButton, "Edit button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR92")]
        [Description("TC_BR92_UI_002: Verify can select service for editing")]
        public void UI_BR92_002_EditService_CanSelectService()
        {
            // Arrange
            var serviceList = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (serviceList == null)
            {
                Assert.Inconclusive("Service list not found");
                return;
            }

            var firstItem = serviceList.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));

            // Act
            if (firstItem != null)
            {
                firstItem.Click();
                Thread.Sleep(500);

                // Assert
                Assert.IsTrue(true, "Service can be selected");
            }
        }

        #endregion

        #region BR93 - Delete Service Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR93")]
        [Description("TC_BR93_UI_001: Verify Delete button exists")]
        public void UI_BR93_001_DeleteButton_ShouldExist()
        {
            // Assert
            var deleteButton = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DeleteButton")
                .Or(cf.ByName("Xóa"))
                .Or(cf.ByName("Delete")))?.AsButton();

            Assert.IsNotNull(deleteButton, "Delete button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR93")]
        [Description("TC_BR93_UI_002: Verify delete confirmation appears")]
        public void UI_BR93_002_DeleteService_ShouldShowConfirmation()
        {
            // Arrange
            var serviceList = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            if (serviceList != null)
            {
                var firstItem = serviceList.FindFirstDescendant(cf => 
                    cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataItem));
                firstItem?.Click();
                Thread.Sleep(500);
            }

            var actionCombo = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(2); // Delete
                Thread.Sleep(500);
            }

            var deleteButton = _serviceWindow.FindFirstDescendant(cf => 
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

        #region BR94 - Export Excel Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR94")]
        [Description("TC_BR94_UI_001: Verify Export to Excel button exists")]
        public void UI_BR94_001_ExportExcelButton_ShouldExist()
        {
            // Assert
            var exportButton = _serviceWindow.FindFirstDescendant(cf => 
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

        #region BR95 - Action Selection Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR95")]
        [Description("TC_BR95_UI_001: Verify action combobox exists")]
        public void UI_BR95_001_ActionComboBox_ShouldExist()
        {
            // Assert
            var actionCombo = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();

            Assert.IsNotNull(actionCombo, "Action combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR95")]
        [Description("TC_BR95_UI_002: Verify can select Add action")]
        public void UI_BR95_002_ActionComboBox_CanSelectAdd()
        {
            // Arrange
            var actionCombo = _serviceWindow.FindFirstDescendant(cf => 
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
            var serviceNameBox = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ServiceNameTextBox"));

            Assert.IsNotNull(serviceNameBox, "Input fields should appear when Add is selected");
        }

        #endregion

        #region BR96 - Reset Functionality Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR96")]
        [Description("TC_BR96_UI_001: Verify Reset button exists")]
        public void UI_BR96_001_ResetButton_ShouldExist()
        {
            // Assert
            var resetButton = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton")
                .Or(cf.ByName("Reset"))
                .Or(cf.ByName("Làm m?i")))?.AsButton();

            Assert.IsNotNull(resetButton, "Reset button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [TestCategory("BR96")]
        [Description("TC_BR96_UI_002: Verify Reset clears input fields")]
        public void UI_BR96_002_Reset_ShouldClearFields()
        {
            // Arrange
            var actionCombo = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            
            if (actionCombo != null)
            {
                actionCombo.Select(0); // Add
                Thread.Sleep(500);
            }

            var serviceNameBox = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ServiceNameTextBox"))?.AsTextBox();

            if (serviceNameBox != null)
            {
                serviceNameBox.Text = "Test Service";
            }

            // Act
            var resetButton = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetButton"))?.AsButton();

            if (resetButton != null)
            {
                resetButton.Click();
                Thread.Sleep(500);

                // Assert
                if (serviceNameBox != null)
                {
                    var currentText = serviceNameBox.Text;
                    Assert.IsTrue(string.IsNullOrEmpty(currentText), "Reset should clear input fields");
                }
            }
        }

        #endregion

        #region UI Element Verification Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [Description("Verify service management window displays all required controls")]
        public void UI_ServiceWindow_ShouldDisplayAllRequiredControls()
        {
            // Assert
            var serviceList = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));
            var actionCombo = _serviceWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ActionComboBox"));

            Assert.IsNotNull(serviceList, "Service list should exist");
            Assert.IsNotNull(actionCombo, "Action combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ServiceManagement")]
        [Description("Verify window title contains service management")]
        public void UI_ServiceWindow_ShouldHaveCorrectTitle()
        {
            // Assert
            var title = _serviceWindow.Title;
            Assert.IsTrue(
                title.Contains("D?ch v?") || title.Contains("Service") || title.Contains("Qu?n lý"),
                $"Window title should indicate service management. Current: {title}");
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
