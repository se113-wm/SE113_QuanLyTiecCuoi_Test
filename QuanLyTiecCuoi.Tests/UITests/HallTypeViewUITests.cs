using System;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuanLyTiecCuoi.Tests.UITests.Helpers;

namespace QuanLyTiecCuoi.Tests.UITests
{
    /// <summary>
    /// UI Tests for BR60 - HallTypeView (Qu?n lý lo?i s?nh)
    /// Covers TC_BR60_001 - TC_BR60_004
    /// </summary>
    [TestClass]
    public class HallTypeViewUITests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private UITestHelper _helper;

        [TestInitialize]
        public void Setup()
        {
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var exePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(appPath, "..\\..\\..\\..\\bin\\Debug\\QuanLyTiecCuoi.exe"));
            if (!System.IO.File.Exists(exePath))
                exePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(appPath, "..\\..\\..\\bin\\Debug\\QuanLyTiecCuoi.exe"));
            if (!System.IO.File.Exists(exePath))
                Assert.Inconclusive($"Cannot find exe at: {exePath}. Build the project before running UI tests.");
            _automation = new UIA3Automation();
            _app = Application.Launch(exePath);
            _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10));
            _helper = new UITestHelper(_app, _automation);
            _mainWindow = _helper.LoginAndNavigateToMain(_mainWindow, "Fartiel", "admin");
            Assert.IsNotNull(_mainWindow, "Login failed or main window not found.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _helper?.CloseAllDialogs();
            _app?.Close();
            _automation?.Dispose();
        }

        private void OpenHallTypeView()
        {
            var hallTypeButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("HallTypeButton")
                .Or(cf.ByName("HallType"))
                .Or(cf.ByName("Lo?i s?nh"))
                .Or(cf.ByAutomationId("btnHallType"))
            )?.AsButton();
            Assert.IsNotNull(hallTypeButton, "HallType navigation button not found");
            hallTypeButton.Click();
            Thread.Sleep(1200);
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR60")]
        [Priority(1)]
        public void TC_BR60_001_Select_Add_Action_Sets_IsAdding_True_And_AddButton_Visible()
        {
            OpenHallTypeView();
            var actionCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            Assert.IsNotNull(actionCombo, "ActionComboBox not found");
            actionCombo.Select(0); // "Thêm"
            Thread.Sleep(500);
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddButton"));
            Assert.IsNotNull(addButton, "AddButton should be visible in add mode");
            Assert.IsTrue(addButton.IsEnabled, "AddButton should be enabled in add mode");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR60")]
        [Priority(1)]
        public void TC_BR60_002_Reset_Clears_Form_Fields_When_Add_Action_Selected()
        {
            OpenHallTypeView();
            // Ch?n m?t item ?? ?i?n d? li?u vào form
            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("HallTypeListView"));
            Assert.IsNotNull(listView, "HallTypeListView not found");
            var items = listView.FindAllChildren();
            Assert.IsTrue(items.Length > 0, "Should have at least one hall type to select");
            items[0].Click();
            Thread.Sleep(500);
            var nameBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("HallTypeNameTextBox"))?.AsTextBox();
            var priceBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("MinTablePriceTextBox"))?.AsTextBox();
            Assert.IsFalse(string.IsNullOrWhiteSpace(nameBox.Text), "Name should be filled after selection");
            Assert.IsFalse(string.IsNullOrWhiteSpace(priceBox.Text), "Price should be filled after selection");
            // Chuy?n sang ch? ?? Thêm
            var actionCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            actionCombo.Select(0); // "Thêm"
            Thread.Sleep(500);
            // Ki?m tra các tr??ng ?ã ???c reset
            Assert.IsTrue(string.IsNullOrWhiteSpace(nameBox.Text), "HallTypeName should be cleared in add mode");
            Assert.IsTrue(string.IsNullOrWhiteSpace(priceBox.Text), "MinTablePrice should be cleared in add mode");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR60")]
        [Priority(1)]
        public void TC_BR60_003_Add_Form_Displays_Required_Fields()
        {
            OpenHallTypeView();
            var actionCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            Assert.IsNotNull(actionCombo, "ActionComboBox not found");
            actionCombo.Select(0); // "Thêm"
            Thread.Sleep(500);
            var nameBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("HallTypeNameTextBox"));
            var priceBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("MinTablePriceTextBox"));
            Assert.IsNotNull(nameBox, "HallTypeNameTextBox should be visible");
            Assert.IsTrue(nameBox.IsEnabled, "HallTypeNameTextBox should be editable");
            Assert.IsNotNull(priceBox, "MinTablePriceTextBox should be visible");
            Assert.IsTrue(priceBox.IsEnabled, "MinTablePriceTextBox should be editable");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR60")]
        [Priority(1)]
        public void TC_BR60_004_AddButton_Visible_Only_When_IsAdding_True()
        {
            OpenHallTypeView();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddButton"));
            // Ban ??u ch?a ? ch? ?? thêm, nút Thêm ?n
            Assert.IsNull(addButton, "AddButton should be hidden initially");
            var actionCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ActionComboBox"))?.AsComboBox();
            actionCombo.Select(0); // "Thêm"
            Thread.Sleep(500);
            addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddButton"));
            Assert.IsNotNull(addButton, "AddButton should be visible after selecting add action");
            Assert.IsTrue(addButton.IsEnabled, "AddButton should be enabled in add mode");
        }
    }
}
