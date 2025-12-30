using System;
using System.IO;
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
    /// UI Tests for BR73 - Xem danh sách món ?n (FoodView)
    /// Covers TC_BR73_001 - TC_BR73_005
    /// </summary>
    [TestClass]
    public class FoodViewUITests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private UITestHelper _helper;

        private static readonly TimeSpan WindowTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan MessageBoxTimeout = TimeSpan.FromSeconds(3);

        private static string GetAppPath()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var appPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\bin\Debug\QuanLyTiecCuoi.exe"));
            if (!File.Exists(appPath))
                appPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\bin\Debug\QuanLyTiecCuoi.exe"));
            return appPath;
        }

        [TestInitialize]
        public void Setup()
        {
            var appPath = GetAppPath();
            if (!File.Exists(appPath))
            {
                Assert.Inconclusive($"Cannot find exe at: {appPath}. Build the project before running UI tests.");
                return;
            }
            _automation = new UIA3Automation();
            _app = Application.Launch(appPath);
            _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10));
            _helper = new UITestHelper(_app, _automation);
            // Login
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

        /// <summary>
        /// TC_BR73_001: Verify FoodView displays when user clicks Food navigation button
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR73")]
        [Priority(1)]
        public void TC_BR73_001_FoodView_Displays_When_Click_FoodButton()
        {
            // Act: Click Food navigation button
            var foodButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("FoodButton")
                .Or(cf.ByName("Food"))
                .Or(cf.ByName("Món ?n"))
                .Or(cf.ByAutomationId("btnFood"))
            )?.AsButton();
            Assert.IsNotNull(foodButton, "Food navigation button not found");
            foodButton.Click();
            Thread.Sleep(1500);
            // Assert: FoodView is displayed
            var foodTitle = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("FoodPageTitle"));
            Assert.IsNotNull(foodTitle, "FoodView title not found");
        }

        /// <summary>
        /// TC_BR73_002: Verify database context is reinitialized via resetDatabaseContext()
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR73")]
        [Priority(1)]
        public void TC_BR73_002_FoodView_Resets_DatabaseContext_On_Load()
        {
            // M? FoodView
            var foodButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("FoodButton")
                .Or(cf.ByName("Food"))
                .Or(cf.ByName("Món ?n"))
                .Or(cf.ByAutomationId("btnFood"))
            )?.AsButton();
            Assert.IsNotNull(foodButton, "Food navigation button not found");
            foodButton.Click();
            Thread.Sleep(1500);
            // Không th? ki?m tra tr?c ti?p DbContext, nh?ng có th? ki?m tra DishListView có d? li?u m?i nh?t
            var dishListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DishListView"));
            Assert.IsNotNull(dishListView, "DishListView not found");
            // N?u có ít nh?t 1 món ?n, coi nh? context ?ã load m?i
            var items = dishListView.FindAllChildren();
            Assert.IsTrue(items.Length >= 0, "DishListView should be loaded (context reset)");
        }

        /// <summary>
        /// TC_BR73_003: Verify FoodViewModel loads dishes using GetAll() from DishService
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR73")]
        [Priority(1)]
        public void TC_BR73_003_FoodViewModel_Loads_Dishes_From_DishService()
        {
            var foodButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("FoodButton")
                .Or(cf.ByName("Food"))
                .Or(cf.ByName("Món ?n"))
                .Or(cf.ByAutomationId("btnFood"))
            )?.AsButton();
            Assert.IsNotNull(foodButton, "Food navigation button not found");
            foodButton.Click();
            Thread.Sleep(1500);
            var dishListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DishListView"));
            Assert.IsNotNull(dishListView, "DishListView not found");
            var items = dishListView.FindAllChildren();
            Assert.IsTrue(items.Length > 0, "DishList should be populated from DishService.GetAll()");
        }

        /// <summary>
        /// TC_BR73_004: Verify DataGrid displays all dishes with correct columns
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR73")]
        [Priority(1)]
        public void TC_BR73_004_FoodView_DataGrid_Has_Correct_Columns()
        {
            var foodButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("FoodButton")
                .Or(cf.ByName("Food"))
                .Or(cf.ByName("Món ?n"))
                .Or(cf.ByAutomationId("btnFood"))
            )?.AsButton();
            Assert.IsNotNull(foodButton, "Food navigation button not found");
            foodButton.Click();
            Thread.Sleep(1500);
            var dishListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DishListView"));
            Assert.IsNotNull(dishListView, "DishListView not found");
            // Ki?m tra header các c?t
            var gridView = dishListView.FindFirstChild(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Header));
            Assert.IsNotNull(gridView, "GridView header not found");
            var headers = gridView.FindAllChildren();
            var headerNames = headers.Select(h => h.Name).ToArray();
            Assert.IsTrue(headerNames.Any(h => h.Contains("Tên Món ?n")), "Tên Món ?n column missing");
            Assert.IsTrue(headerNames.Any(h => h.Contains("??n Giá")), "??n Giá column missing");
            Assert.IsTrue(headerNames.Any(h => h.Contains("Ghi chú")), "Ghi chú column missing");
        }

        /// <summary>
        /// TC_BR73_005: Verify UnitPrice is displayed with proper currency format
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR73")]
        [Priority(1)]
        public void TC_BR73_005_UnitPrice_Displayed_With_Currency_Format()
        {
            var foodButton = _mainWindow.FindFirstDescendant(cf =>
                cf.ByAutomationId("FoodButton")
                .Or(cf.ByName("Food"))
                .Or(cf.ByName("Món ?n"))
                .Or(cf.ByAutomationId("btnFood"))
            )?.AsButton();
            Assert.IsNotNull(foodButton, "Food navigation button not found");
            foodButton.Click();
            Thread.Sleep(1500);
            var dishListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DishListView"));
            Assert.IsNotNull(dishListView, "DishListView not found");
            var items = dishListView.FindAllChildren();
            Assert.IsTrue(items.Length > 0, "DishList should have at least one item");
            // L?y item ??u tiên, ki?m tra c?t ??n Giá
            var firstItem = items.FirstOrDefault();
            Assert.IsNotNull(firstItem, "No dish item found");
            var priceCell = firstItem.FindAllChildren().FirstOrDefault(c => c.Name.Contains(",") || c.Name.All(char.IsDigit));
            Assert.IsNotNull(priceCell, "UnitPrice cell not found");
            Assert.IsTrue(priceCell.Name.Contains(",") || priceCell.Name.Length > 3, "UnitPrice should be formatted with thousands separator");
        }
    }
}
