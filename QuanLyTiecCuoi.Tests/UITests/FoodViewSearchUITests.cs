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
    /// UI Tests for BR74 - FoodView Search/Filter
    /// Covers TC_BR74_001 - TC_BR74_005
    /// </summary>
    [TestClass]
    public class FoodViewSearchUITests
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

        private void OpenFoodView()
        {
            var foodButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("FoodButton")
                .Or(cf.ByName("Food"))
                .Or(cf.ByName("Món ăn"))
                .Or(cf.ByAutomationId("btnFood"))
            )?.AsButton();
            Assert.IsNotNull(foodButton, "Food navigation button not found");
            foodButton.Click();
            Thread.Sleep(1200);
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR74")]
        [Priority(1)]
        public void TC_BR74_001_Search_By_DishName_Filters_Correctly()
        {
            OpenFoodView();
            var searchCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchPropertyComboBox"))?.AsComboBox();
            Assert.IsNotNull(searchCombo, "SearchPropertyComboBox not found");
            searchCombo.Select(0); // "Tên món ăn"
            var searchBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchTextBox"))?.AsTextBox();
            Assert.IsNotNull(searchBox, "SearchTextBox not found");
            searchBox.Text = "G";
            Thread.Sleep(800);
            var dishListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DishListView"));
            Assert.IsNotNull(dishListView, "DishListView not found");
            var items = dishListView.FindAllChildren();
            Assert.IsTrue(items.All(i => i.Name.Contains("G", StringComparison.OrdinalIgnoreCase)), "All results should contain 'Gà' in name");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR74")]
        [Priority(1)]
        public void TC_BR74_002_Search_By_UnitPrice_Filters_Correctly()
        {
            OpenFoodView();
            var searchCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchPropertyComboBox"))?.AsComboBox();
            Assert.IsNotNull(searchCombo, "SearchPropertyComboBox not found");
            searchCombo.Select(1); // "Đơn giá"
            var searchBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchTextBox"))?.AsTextBox();
            Assert.IsNotNull(searchBox, "SearchTextBox not found");
            searchBox.Text = "150000";
            Thread.Sleep(800);
            var dishListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DishListView"));
            Assert.IsNotNull(dishListView, "DishListView not found");
            var items = dishListView.FindAllChildren();
            Assert.IsTrue(items.All(i => i.Name.Contains("150000")), "All results should contain '150000' in price");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR74")]
        [Priority(1)]
        public void TC_BR74_003_Search_By_Note_Filters_Correctly()
        {
            OpenFoodView();
            var searchCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchPropertyComboBox"))?.AsComboBox();
            Assert.IsNotNull(searchCombo, "SearchPropertyComboBox not found");
            searchCombo.Select(2); // "Ghi chú"
            var searchBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchTextBox"))?.AsTextBox();
            Assert.IsNotNull(searchBox, "SearchTextBox not found");
            searchBox.Text = "đặc biệt";
            Thread.Sleep(800);
            var dishListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DishListView"));
            Assert.IsNotNull(dishListView, "DishListView not found");
            var items = dishListView.FindAllChildren();
            Assert.IsTrue(items.All(i => i.Name.Contains("đặc biệt", StringComparison.OrdinalIgnoreCase)), "All results should contain 'đặc biệt' in note");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR74")]
        [Priority(1)]
        public void TC_BR74_004_Search_Is_Case_Insensitive()
        {
            OpenFoodView();
            var searchCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchPropertyComboBox"))?.AsComboBox();
            Assert.IsNotNull(searchCombo, "SearchPropertyComboBox not found");
            searchCombo.Select(0); // "Tên món ăn"
            var searchBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchTextBox"))?.AsTextBox();
            Assert.IsNotNull(searchBox, "SearchTextBox not found");
            searchBox.Text = "gà nướng";
            Thread.Sleep(800);
            var dishListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DishListView"));
            Assert.IsNotNull(dishListView, "DishListView not found");
            var items = dishListView.FindAllChildren();
            Assert.IsTrue(items.Any(i => i.Name.Contains("Gà Nướng", StringComparison.OrdinalIgnoreCase)), "Should match regardless of case");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR74")]
        [Priority(1)]
        public void TC_BR74_005_Empty_Search_Returns_All_Dishes()
        {
            OpenFoodView();
            var searchCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchPropertyComboBox"))?.AsComboBox();
            Assert.IsNotNull(searchCombo, "SearchPropertyComboBox not found");
            searchCombo.Select(0); // "Tên món ăn"
            var searchBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchTextBox"))?.AsTextBox();
            Assert.IsNotNull(searchBox, "SearchTextBox not found");
            searchBox.Text = "Gà";
            Thread.Sleep(800);
            var dishListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DishListView"));
            Assert.IsNotNull(dishListView, "DishListView not found");
            var filteredCount = dishListView.FindAllChildren().Length;
            searchBox.Text = string.Empty;
            Thread.Sleep(800);
            var allCount = dishListView.FindAllChildren().Length;
            Assert.IsTrue(allCount > filteredCount, "Clearing search should show more or all dishes");
        }
    }
}
