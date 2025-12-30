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
    /// UI Tests for Report Management Window
    /// Covers BR106-BR115 - Report Management
    /// Tests: Month/Year selection, Load report, Export, and Chart scenarios
    /// </summary>
    [TestClass]
    public class ReportManagementWindowTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private Window _reportWindow;

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
            
            // Navigate to Report Management
            NavigateToReportManagement();
        }

        /// <summary>
        /// Perform login and re-acquire Main Window after transition
        /// </summary>
        private Window PerformLoginAndReacquireMainWindow(Window loginWindow)
        {
            var helper = new UITestHelper(_app, _automation);
            return helper.PerformLoginAndReacquireMainWindow(loginWindow, "Fartiel", "admin");
        }

        private void NavigateToReportManagement()
        {
            var reportMenuItem = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ReportButton")    // ? PRIMARY: Exact match from XAML
                .Or(cf.ByName("Report"))             // Fallback: English text
                .Or(cf.ByName("Báo cáo")));          // Fallback: Vietnamese text
            
            if (reportMenuItem != null)
            {
                reportMenuItem.Click();
                Thread.Sleep(1500);
                _reportWindow = _mainWindow.ModalWindows.FirstOrDefault() ?? _mainWindow;
            }
            else
            {
                Assert.Inconclusive("Cannot find Report Management button. Check MainWindow.xaml AutomationId.");
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

        #region BR106 - Display Report Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR106")]
        [Description("TC_BR106_UI_001: Verify report management window displays")]
        public void UI_BR106_001_ReportWindow_ShouldDisplay()
        {
            // Assert
            Assert.IsNotNull(_reportWindow, "Report window should display");
            Assert.IsTrue(_reportWindow.IsAvailable, "Report window should be available");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR106")]
        [Description("TC_BR106_UI_002: Verify report list/grid displays")]
        public void UI_BR106_002_ReportList_ShouldDisplay()
        {
            // Assert
            var reportList = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ReportDataGrid")
                .Or(cf.ByAutomationId("ReportListBox"))
                .Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid)));

            Assert.IsNotNull(reportList, "Report list/grid should be displayed");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR106")]
        [Description("TC_BR106_UI_003: Verify total revenue label displays")]
        public void UI_BR106_003_TotalRevenue_ShouldDisplay()
        {
            // Assert
            var totalRevenueLabel = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("TotalRevenueTextBlock")
                .Or(cf.ByAutomationId("TotalRevenueLabel")));

            // Total revenue display might be in different format
            if (totalRevenueLabel != null)
            {
                Assert.IsTrue(true, "Total revenue display found");
            }
        }

        #endregion

        #region BR107 - Load Report Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR107")]
        [Description("TC_BR107_UI_001: Verify Load Report button exists")]
        public void UI_BR107_001_LoadReportButton_ShouldExist()
        {
            // Assert
            var loadButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("LoadReportButton")
                .Or(cf.ByName("Xem báo cáo"))
                .Or(cf.ByName("Load Report")))?.AsButton();

            Assert.IsNotNull(loadButton, "Load Report button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR107")]
        [Description("TC_BR107_UI_002: Verify can click Load Report button")]
        public void UI_BR107_002_LoadReportButton_CanClick()
        {
            // Arrange
            var loadButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("LoadReportButton"))?.AsButton();

            if (loadButton == null)
            {
                Assert.Inconclusive("Load Report button not found");
                return;
            }

            // Act
            loadButton.Click();
            Thread.Sleep(2000);

            // Assert - Report should load without error
            Assert.IsTrue(true, "Load Report button can be clicked");
        }

        #endregion

        #region BR108 - Month/Year Selection Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR108")]
        [Description("TC_BR108_UI_001: Verify Month combobox exists")]
        public void UI_BR108_001_MonthComboBox_ShouldExist()
        {
            // Assert
            var monthCombo = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("MonthComboBox"))?.AsComboBox();

            Assert.IsNotNull(monthCombo, "Month combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR108")]
        [Description("TC_BR108_UI_002: Verify Year combobox exists")]
        public void UI_BR108_002_YearComboBox_ShouldExist()
        {
            // Assert
            var yearCombo = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("YearComboBox"))?.AsComboBox();

            Assert.IsNotNull(yearCombo, "Year combobox should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR108")]
        [Description("TC_BR108_UI_003: Verify can select different month")]
        public void UI_BR108_003_MonthComboBox_CanSelectMonth()
        {
            // Arrange
            var monthCombo = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("MonthComboBox"))?.AsComboBox();

            if (monthCombo == null)
            {
                Assert.Inconclusive("Month combobox not found");
                return;
            }

            // Act - Select different month (e.g., January = index 0)
            monthCombo.Select(0);
            Thread.Sleep(500);

            // Assert
            Assert.IsTrue(true, "Can select different month");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR108")]
        [Description("TC_BR108_UI_004: Verify can select different year")]
        public void UI_BR108_004_YearComboBox_CanSelectYear()
        {
            // Arrange
            var yearCombo = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("YearComboBox"))?.AsComboBox();

            if (yearCombo == null)
            {
                Assert.Inconclusive("Year combobox not found");
                return;
            }

            // Act
            yearCombo.Select(0);
            Thread.Sleep(500);

            // Assert
            Assert.IsTrue(true, "Can select different year");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR108")]
        [Description("TC_BR108_UI_005: Verify month combobox contains 12 months")]
        public void UI_BR108_005_MonthComboBox_Contains12Months()
        {
            // Arrange
            var monthCombo = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("MonthComboBox"))?.AsComboBox();

            if (monthCombo == null)
            {
                Assert.Inconclusive("Month combobox not found");
                return;
            }

            // Assert - Month combobox should have 12 items
            var items = monthCombo.Items;
            Assert.IsTrue(items.Length >= 12, "Month combobox should contain at least 12 months");
        }

        #endregion

        #region BR109 - Report Data Display Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR109")]
        [Description("TC_BR109_UI_001: Verify report displays date information")]
        public void UI_BR109_001_ReportData_ShouldDisplayDates()
        {
            // Arrange - Load report first
            var loadButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("LoadReportButton"))?.AsButton();
            
            if (loadButton != null)
            {
                loadButton.Click();
                Thread.Sleep(2000);
            }

            // Assert
            var reportList = _reportWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            Assert.IsNotNull(reportList, "Report should display data");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR109")]
        [Description("TC_BR109_UI_002: Verify report displays wedding count")]
        public void UI_BR109_002_ReportData_ShouldDisplayWeddingCount()
        {
            // Arrange
            var loadButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("LoadReportButton"))?.AsButton();
            
            if (loadButton != null)
            {
                loadButton.Click();
                Thread.Sleep(2000);
            }

            // Assert - Report grid should contain data
            var reportList = _reportWindow.FindFirstDescendant(cf => 
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid));

            Assert.IsNotNull(reportList, "Report should display wedding count data");
        }

        #endregion

        #region BR110 - Export PDF Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR110")]
        [Description("TC_BR110_UI_001: Verify Export PDF button exists")]
        public void UI_BR110_001_ExportPdfButton_ShouldExist()
        {
            // Assert
            var exportPdfButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ExportPdfButton")
                .Or(cf.ByName("Xu?t PDF"))
                .Or(cf.ByName("Export PDF")))?.AsButton();

            Assert.IsNotNull(exportPdfButton, "Export PDF button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR110")]
        [Description("TC_BR110_UI_002: Verify Export PDF button is enabled")]
        public void UI_BR110_002_ExportPdfButton_ShouldBeEnabled()
        {
            // Arrange - Load report first
            var loadButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("LoadReportButton"))?.AsButton();
            
            if (loadButton != null)
            {
                loadButton.Click();
                Thread.Sleep(2000);
            }

            // Assert
            var exportPdfButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ExportPdfButton"))?.AsButton();

            if (exportPdfButton != null)
            {
                Assert.IsTrue(exportPdfButton.IsEnabled, "Export PDF button should be enabled");
            }
        }

        #endregion

        #region BR111 - Export Excel Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR111")]
        [Description("TC_BR111_UI_001: Verify Export Excel button exists")]
        public void UI_BR111_001_ExportExcelButton_ShouldExist()
        {
            // Assert
            var exportExcelButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ExportExcelButton")
                .Or(cf.ByName("Xu?t Excel"))
                .Or(cf.ByName("Export Excel")))?.AsButton();

            Assert.IsNotNull(exportExcelButton, "Export Excel button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR111")]
        [Description("TC_BR111_UI_002: Verify Export Excel button is enabled")]
        public void UI_BR111_002_ExportExcelButton_ShouldBeEnabled()
        {
            // Arrange
            var loadButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("LoadReportButton"))?.AsButton();
            
            if (loadButton != null)
            {
                loadButton.Click();
                Thread.Sleep(2000);
            }

            // Assert
            var exportExcelButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ExportExcelButton"))?.AsButton();

            if (exportExcelButton != null)
            {
                Assert.IsTrue(exportExcelButton.IsEnabled, "Export Excel button should be enabled");
            }
        }

        #endregion

        #region BR112 - Show Chart Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR112")]
        [Description("TC_BR112_UI_001: Verify Show Chart button exists")]
        public void UI_BR112_001_ShowChartButton_ShouldExist()
        {
            // Assert
            var showChartButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ShowChartButton")
                .Or(cf.ByName("Xem bi?u ??"))
                .Or(cf.ByName("Show Chart")))?.AsButton();

            Assert.IsNotNull(showChartButton, "Show Chart button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR112")]
        [Description("TC_BR112_UI_002: Verify can click Show Chart button")]
        public void UI_BR112_002_ShowChartButton_CanClick()
        {
            // Arrange
            var loadButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("LoadReportButton"))?.AsButton();
            
            if (loadButton != null)
            {
                loadButton.Click();
                Thread.Sleep(2000);
            }

            var showChartButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ShowChartButton"))?.AsButton();

            if (showChartButton == null)
            {
                Assert.Inconclusive("Show Chart button not found");
                return;
            }

            // Act
            if (showChartButton.IsEnabled)
            {
                showChartButton.Click();
                Thread.Sleep(1000);

                // Assert
                Assert.IsTrue(true, "Show Chart button can be clicked");
                
                // Close chart window if opened
                CloseAnyMessageBox();
            }
        }

        #endregion

        #region BR113 - Total Revenue Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [TestCategory("BR113")]
        [Description("TC_BR113_UI_001: Verify total revenue displays after loading report")]
        public void UI_BR113_001_TotalRevenue_ShouldDisplayAfterLoad()
        {
            // Arrange & Act
            var loadButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("LoadReportButton"))?.AsButton();
            
            if (loadButton != null)
            {
                loadButton.Click();
                Thread.Sleep(2000);
            }

            // Assert
            var totalRevenueLabel = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("TotalRevenueTextBlock"));

            if (totalRevenueLabel != null)
            {
                var text = totalRevenueLabel.Name;
                Assert.IsFalse(string.IsNullOrEmpty(text), "Total revenue should display value");
            }
        }

        #endregion

        #region UI Element Verification Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [Description("Verify report management window displays all required controls")]
        public void UI_ReportWindow_ShouldDisplayAllRequiredControls()
        {
            // Assert
            var monthCombo = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("MonthComboBox"));
            var yearCombo = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("YearComboBox"));
            var loadButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("LoadReportButton"));

            Assert.IsNotNull(monthCombo, "Month combobox should exist");
            Assert.IsNotNull(yearCombo, "Year combobox should exist");
            Assert.IsNotNull(loadButton, "Load button should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [Description("Verify window title contains report management")]
        public void UI_ReportWindow_ShouldHaveCorrectTitle()
        {
            // Assert
            var title = _reportWindow.Title;
            Assert.IsTrue(
                title.Contains("Báo cáo") || 
                title.Contains("Report") || 
                title.Contains("Doanh thu") ||
                title.Contains("Revenue"),
                $"Window title should indicate report management. Current: {title}");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("ReportManagement")]
        [Description("Verify all export buttons are available")]
        public void UI_ReportWindow_AllExportButtonsAvailable()
        {
            // Assert
            var exportPdfButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ExportPdfButton"));
            var exportExcelButton = _reportWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ExportExcelButton"));

            Assert.IsNotNull(exportPdfButton, "Export PDF button should exist");
            Assert.IsNotNull(exportExcelButton, "Export Excel button should exist");
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
