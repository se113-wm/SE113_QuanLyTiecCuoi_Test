using System;
using System.IO;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinForms = System.Windows.Forms;

namespace QuanLyTiecCuoi.Tests.UITests
{
    /// <summary>
    /// UI Tests for UC 2.1.5.2 - Create Booking for Customer
    /// Covers BR140, BR141, BR142
    /// 
    /// BR140 - Displaying Rules:
    /// When staff clicks "T?o phi?u ??t" button, the system displays booking form with fields:
    /// [GroomName], [BrideName], [PhoneNumber], [EventDate], [Shift], [Hall], [TableCount], [Menu], [Services]
    /// 
    /// BR141 - Validation Rules:
    /// - IF any required field is empty, displays validation message (MSG10)
    /// - IF [TableCount] <= 0 OR [TableCount] > [Hall.MaxTableCount], displays validation message (MSG91)
    /// - IF hall not available for selected date/shift, displays validation message (MSG102)
    /// 
    /// BR142 - Processing Rules:
    /// After validation passes, the system calculates total amount and creates booking with status "Confirmed"
    /// 
    /// IMPORTANT: According to ViewModel validation logic, validations are checked in order:
    /// 1. First check: ALL required fields must be filled INCLUDING MenuList and ServiceList (MSG10)
    /// 2. Then check: Hall availability (MSG102)
    /// 3. Then check: TableCount validation (MSG89/MSG91)
    /// 4. Then check: ReserveTableCount validation
    /// 5. Then check: Phone format validation (MSG88)
    /// 6. Then check: Deposit validation
    /// 7. Finally: Create booking if all validations pass
    /// 
    /// CRITICAL NOTE about MenuItemViewModel and ServiceDetailItemViewModel:
    /// - ConfirmCommand shows MessageBox "B?n ?ã ch?n món/d?ch v?: X" BEFORE closing window
    /// - CancelCommand shows MessageBox "B?n có ch?c ch?n mu?n h?y không?" with Yes/No buttons
    /// So test code must handle these MessageBoxes properly!
    /// </summary>
    [TestClass]
    public class CreateBookingForCustomerUITests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;

        // Timeout constants for better maintainability
        private static readonly TimeSpan WindowTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan MessageBoxTimeout = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan RetryInterval = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan ShortDelay = TimeSpan.FromMilliseconds(300);

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
                Assert.Inconclusive($"Cannot find exe at: {appPath}. Build the project before running UI tests.");
                return;
            }

            _automation = new UIA3Automation();
            _app = Application.Launch(appPath);
            _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10));

            if (!LoginAsStaff())
            {
                Assert.Inconclusive("Login failed. Check credentials/UI.");
                return;
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                var desktop = _automation?.GetDesktop();
                
                // Close any child windows first (MenuItemView, ServiceDetailItemView)
                CloseAllChildWindows();
                
                var messageBox = desktop?.FindFirstChild(cf => cf.ByClassName("#32770"));
                if (messageBox != null)
                {
                    CloseMessageBox(messageBox);
                }

                var addWeddingWindow = desktop?.FindFirstChild(cf => cf.ByAutomationId("DatTiecCuoi"))?.AsWindow();
                if (addWeddingWindow != null)
                {
                    CloseAddWeddingWindow(addWeddingWindow);
                }
            }
            catch { }

            _app?.Close();
            _automation?.Dispose();
        }

        #region BR140 - Display Booking Form Tests (5 tests)

        /// <summary>
        /// TC_BR140_001: Verify booking form displays all required fields when staff clicks Add button
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR140")]
        [TestCategory("Critical")]
        [Priority(1)]
        [Description("TC_BR140_001: Verify booking form displays all required fields")]
        public void TC_BR140_001_DisplayBookingForm_ShowsAllRequiredFields()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            Assert.IsNotNull(addWeddingWindow, "Add Wedding form should open.");

            try
            {
                // Assert - Verify all required fields exist per BR140
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("GroomNameTextBox")), 
                    "GroomName field should be displayed");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("BrideNameTextBox")), 
                    "BrideName field should be displayed");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("PhoneTextBox")), 
                    "PhoneNumber field should be displayed");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingDatePicker")), 
                    "EventDate picker should be displayed");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ShiftComboBox")), 
                    "Shift dropdown should be displayed");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("HallComboBox")), 
                    "Hall dropdown should be displayed");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("TableCountTextBox")), 
                    "TableCount field should be displayed");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("MenuListView")), 
                    "Menu list should be displayed");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ServiceListView")), 
                    "Services list should be displayed");
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR140_002: Verify Shift dropdown has options loaded
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR140")]
        [Priority(1)]
        [Description("TC_BR140_002: Verify ShiftComboBox has options loaded from database")]
        public void TC_BR140_002_ShiftComboBox_HasOptionsLoaded()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            Assert.IsNotNull(addWeddingWindow, "Add Wedding form should open.");

            try
            {
                var shiftComboBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ShiftComboBox"))?.AsComboBox();
                Assert.IsNotNull(shiftComboBox, "Shift ComboBox should exist");

                shiftComboBox.Expand();
                Thread.Sleep(500);
                WaitForCondition(() => shiftComboBox.Items.Length > 0, TimeSpan.FromSeconds(2));

                Assert.IsTrue(shiftComboBox.Items.Length > 0, "Shift ComboBox should have options loaded from database");

                shiftComboBox.Collapse();
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR140_003: Verify Hall dropdown has options loaded with capacity info
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR140")]
        [Priority(1)]
        [Description("TC_BR140_003: Verify HallComboBox has options loaded with capacity info")]
        public void TC_BR140_003_HallComboBox_HasOptionsWithCapacityInfo()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            Assert.IsNotNull(addWeddingWindow, "Add Wedding form should open.");

            try
            {
                var hallComboBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("HallComboBox"))?.AsComboBox();
                Assert.IsNotNull(hallComboBox, "Hall ComboBox should exist");

                hallComboBox.Expand();
                Thread.Sleep(500);
                WaitForCondition(() => hallComboBox.Items.Length > 0, TimeSpan.FromSeconds(2));

                Assert.IsTrue(hallComboBox.Items.Length > 0, "Hall ComboBox should have options");

                hallComboBox.Collapse();
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR140_004: Verify Menu section has all required buttons
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR140")]
        [Priority(2)]
        [Description("TC_BR140_004: Verify Menu section has all required buttons")]
        public void TC_BR140_004_MenuSection_HasRequiredButtons()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            Assert.IsNotNull(addWeddingWindow, "Add Wedding form should open.");

            try
            {
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("SelectDishButton")), 
                    "Select Dish button should exist");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddMenuButton")), 
                    "Add Menu button should exist");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("EditMenuButton")), 
                    "Edit Menu button should exist");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("DeleteMenuButton")), 
                    "Delete Menu button should exist");
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR140_005: Verify Service section has all required buttons
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR140")]
        [Priority(2)]
        [Description("TC_BR140_005: Verify Service section has all required buttons")]
        public void TC_BR140_005_ServiceSection_HasRequiredButtons()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            Assert.IsNotNull(addWeddingWindow, "Add Wedding form should open.");

            try
            {
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("SelectServiceButton")), 
                    "Select Service button should exist");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddServiceButton")), 
                    "Add Service button should exist");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("EditServiceButton")), 
                    "Edit Service button should exist");
                Assert.IsNotNull(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("DeleteServiceButton")), 
                    "Delete Service button should exist");
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        #endregion

        #region BR141 - Validation Rules Tests (7 tests)

        /// <summary>
        /// TC_BR141_001: Verify MSG10 displays when ALL required fields are empty
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR141")]
        [TestCategory("Critical")]
        [Priority(1)]
        [Description("TC_BR141_001: Verify validation message when all required fields are empty (MSG10)")]
        public void TC_BR141_001_Validation_AllFieldsEmpty_ShowsMissingInfoMessage()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Add Wedding form did not open.");
                return;
            }

            try
            {
                // Act - Click confirm with ALL fields empty (no menu/service selected)
                var confirmButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmButton"))?.AsButton();
                Assert.IsNotNull(confirmButton, "Confirm button not found.");
                confirmButton.Click();
                Thread.Sleep(500);

                // Assert - MSG10: "Please fill in all required information and select menu, services."
                var messageBox = WaitForMessageBox(addWeddingWindow, MessageBoxTimeout);
                Assert.IsNotNull(messageBox, "Validation message (MSG10) should appear when required fields are empty.");

                var messageText = GetMessageBoxText(messageBox);
                System.Diagnostics.Debug.WriteLine($"Message: {messageText}");
                Assert.IsTrue(messageText.Contains("Missing") || messageText.Contains("required") || messageText.Contains("fill") || messageText.Contains("Please"),
                    "Should display missing information message");

                CloseMessageBox(messageBox);
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR141_002: Verify MSG10 displays when Menu list is empty
        /// Even if all other fields are filled, MenuList.Count == 0 triggers MSG10
        /// NOTE: This test fills basic fields but does NOT add menu/service items
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR141")]
        [Priority(1)]
        [Description("TC_BR141_002: Verify validation when Menu list is empty")]
        public void TC_BR141_002_Validation_MenuListEmpty_ShowsMissingInfoMessage()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Add Wedding form did not open.");
                return;
            }

            try
            {
                // Arrange - Fill ALL basic fields but leave Menu and Service empty
                FillAllBasicFieldsOnly(addWeddingWindow);
                // Note: MenuList and ServiceList are still empty - this should trigger MSG10

                // Act - Click confirm
                var confirmButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmButton"))?.AsButton();
                confirmButton.Click();
                Thread.Sleep(500);

                // Assert - Should show MSG10 because MenuList.Count == 0
                var messageBox = WaitForMessageBox(addWeddingWindow, MessageBoxTimeout);
                Assert.IsNotNull(messageBox, "Validation message should appear when Menu list is empty.");

                var messageText = GetMessageBoxText(messageBox);
                System.Diagnostics.Debug.WriteLine($"Message: {messageText}");

                CloseMessageBox(messageBox);
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR141_003: Verify TableCount = 0 triggers validation (MSG89)
        /// NOTE: To reach this validation, ALL required fields INCLUDING Menu and Service must be filled first!
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR141")]
        [Priority(1)]
        [Description("TC_BR141_003: Verify TableCount = 0 triggers validation (MSG89)")]
        public void TC_BR141_003_Validation_ZeroTableCount_ShowsValidationMessage()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Add Wedding form did not open.");
                return;
            }

            try
            {
                // Arrange - Fill ALL required fields INCLUDING Menu and Service
                // This is required to pass the first validation check (MSG10)
                FillAllRequiredFieldsComplete(addWeddingWindow);
                
                // Override TableCount to 0 (invalid) - AFTER filling everything else
                var tableCountTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("TableCountTextBox"))?.AsTextBox();
                if (tableCountTextBox != null)
                {
                    SetTextBoxValue(tableCountTextBox, "0");
                }

                // Act - Click confirm
                var confirmButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmButton"))?.AsButton();
                confirmButton.Click();
                Thread.Sleep(500);

                // Assert - Should show MSG89: "Table count must be a positive integer."
                var messageBox = WaitForMessageBox(addWeddingWindow, MessageBoxTimeout);
                Assert.IsNotNull(messageBox, "Validation message (MSG89) should appear for TableCount = 0.");

                var messageText = GetMessageBoxText(messageBox);
                System.Diagnostics.Debug.WriteLine($"Message: {messageText}");
                Assert.IsTrue(messageText.Contains("Table") || messageText.Contains("positive") || messageText.Contains("bàn"),
                    "Should display table count validation message");

                CloseMessageBox(messageBox);
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR141_004: Verify TableCount exceeds hall capacity triggers validation (MSG91)
        /// NOTE: Must have Menu and Service selected first to pass MSG10 validation
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR141")]
        [Priority(1)]
        [Description("TC_BR141_004: Verify TableCount exceeds hall capacity triggers validation (MSG91)")]
        public void TC_BR141_004_Validation_TableCountExceedsCapacity_ShowsValidationMessage()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Add Wedding form did not open.");
                return;
            }

            try
            {
                // Arrange - Fill ALL required fields INCLUDING Menu and Service
                FillAllRequiredFieldsComplete(addWeddingWindow);
                
                // Set TableCount to very high value to exceed any hall capacity
                var tableCountTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("TableCountTextBox"))?.AsTextBox();
                if (tableCountTextBox != null)
                {
                    SetTextBoxValue(tableCountTextBox, "999");
                }

                // Act - Click confirm
                var confirmButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmButton"))?.AsButton();
                confirmButton.Click();
                Thread.Sleep(500);

                // Assert - Should show MSG91: "Table count exceeds hall maximum"
                var messageBox = WaitForMessageBox(addWeddingWindow, MessageBoxTimeout);
                Assert.IsNotNull(messageBox, "Validation message (MSG91) should appear when TableCount exceeds hall capacity.");

                var messageText = GetMessageBoxText(messageBox);
                System.Diagnostics.Debug.WriteLine($"Message: {messageText}");
                Assert.IsTrue(messageText.Contains("exceed") || messageText.Contains("maximum") || messageText.Contains("v??t"),
                    "Should display table count exceeds capacity message");

                CloseMessageBox(messageBox);
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR141_005: Verify invalid phone format triggers validation (MSG88)
        /// NOTE: Phone validation is checked AFTER TableCount validation!
        /// Must have Menu and Service selected first to pass MSG10 validation
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR141")]
        [Priority(1)]
        [Description("TC_BR141_005: Verify invalid phone format triggers validation (MSG88)")]
        public void TC_BR141_005_Validation_InvalidPhoneFormat_ShowsValidationMessage()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Add Wedding form did not open.");
                return;
            }

            try
            {
                // Arrange - Fill ALL required fields INCLUDING Menu and Service with VALID data
                FillAllRequiredFieldsComplete(addWeddingWindow);
                
                // Override Phone with INVALID format (letters instead of digits)
                var phoneTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("PhoneTextBox"))?.AsTextBox();
                if (phoneTextBox != null)
                {
                    SetTextBoxValue(phoneTextBox, "abc123invalid");
                }

                // Act - Click confirm
                var confirmButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmButton"))?.AsButton();
                confirmButton.Click();
                Thread.Sleep(500);

                // Assert - Should show MSG88: "Phone number must be 10 or 11 digits starting with 0 or +84."
                var messageBox = WaitForMessageBox(addWeddingWindow, MessageBoxTimeout);
                Assert.IsNotNull(messageBox, "Validation message (MSG88) should appear for invalid phone format.");

                var messageText = GetMessageBoxText(messageBox);
                System.Diagnostics.Debug.WriteLine($"Message: {messageText}");
                Assert.IsTrue(messageText.Contains("Phone") || messageText.Contains("digit") || messageText.Contains("?i?n tho?i"),
                    "Should display phone format validation message");

                CloseMessageBox(messageBox);
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR141_006: Verify invalid Deposit format triggers validation
        /// Must have Menu and Service selected first to pass MSG10 validation
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR141")]
        [Priority(1)]
        [Description("TC_BR141_006: Verify invalid Deposit format triggers validation")]
        public void TC_BR141_006_Validation_InvalidDepositFormat_ShowsValidationMessage()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Add Wedding form did not open.");
                return;
            }

            try
            {
                // Arrange - Fill ALL required fields INCLUDING Menu and Service
                FillAllRequiredFieldsComplete(addWeddingWindow);
                
                // Set Deposit to invalid format (letters)
                var depositTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("DepositTextBox"))?.AsTextBox();
                if (depositTextBox != null)
                {
                    SetTextBoxValue(depositTextBox, "abc");
                }

                // Act - Click confirm
                var confirmButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmButton"))?.AsButton();
                confirmButton.Click();
                Thread.Sleep(500);

                // Assert - Should show validation message for invalid Deposit
                var messageBox = WaitForMessageBox(addWeddingWindow, MessageBoxTimeout);
                Assert.IsNotNull(messageBox, "Validation message should appear for invalid Deposit format.");

                var messageText = GetMessageBoxText(messageBox);
                System.Diagnostics.Debug.WriteLine($"Message: {messageText}");
                Assert.IsTrue(messageText.Contains("Deposit") || messageText.Contains("valid") || messageText.Contains("c?c") || messageText.Contains("number"),
                    "Should display deposit validation message");

                CloseMessageBox(messageBox);
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        /// <summary>
        /// TC_BR141_007: Verify Cancel button shows confirmation dialog
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR141")]
        [Priority(2)]
        [Description("TC_BR141_007: Verify Cancel button shows confirmation dialog")]
        public void TC_BR141_007_CancelButton_ShowsConfirmationDialog()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Add Wedding form did not open.");
                return;
            }

            try
            {
                // Act - Click Cancel button
                var cancelButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("CancelButton"))?.AsButton();
                Assert.IsNotNull(cancelButton, "Cancel button should exist");
                cancelButton.Click();
                Thread.Sleep(500);

                // Assert - Confirmation dialog should appear
                var messageBox = WaitForMessageBox(addWeddingWindow, MessageBoxTimeout);
                Assert.IsNotNull(messageBox, "Confirmation dialog should appear when clicking Cancel.");

                // Close dialog by clicking No to stay on form
                var noButton = messageBox.FindFirstDescendant(cf => cf.ByName("No")
                    .Or(cf.ByName("Không")))?.AsButton();
                if (noButton != null)
                {
                    noButton.Click();
                    WaitForMessageBoxToClose(MessageBoxTimeout);
                }
                else
                {
                    CloseMessageBox(messageBox);
                }
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        #endregion

        #region BR142 - Processing Rules Tests (3 tests)

        /// <summary>
        /// TC_BR142_001: Verify booking list is displayed in management screen
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR142")]
        [Priority(1)]
        [Description("TC_BR142_001: Verify booking list is displayed in management screen")]
        public void TC_BR142_001_BookingManagement_DisplaysBookingList()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");

            var weddingListView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingListView"));
            Assert.IsNotNull(weddingListView, "Wedding list view should be visible");

            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddButton"));
            Assert.IsNotNull(addButton, "Add button should be visible for creating bookings");
        }

        /// <summary>
        /// TC_BR142_002: Verify Detail button exists for viewing booking
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR142")]
        [Priority(2)]
        [Description("TC_BR142_002: Verify Detail button exists for viewing booking")]
        public void TC_BR142_002_BookingManagement_HasDetailButton()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");

            var detailButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DetailButton"));
            Assert.IsNotNull(detailButton, "Detail button should be visible for viewing booking details");
        }

        /// <summary>
        /// TC_BR142_003: Verify Delete button exists for removing booking
        /// </summary>
        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR142")]
        [Priority(2)]
        [Description("TC_BR142_003: Verify Delete button exists for removing booking")]
        public void TC_BR142_003_BookingManagement_HasDeleteButton()
        {
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab.");

            var deleteButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DeleteButton"));
            Assert.IsNotNull(deleteButton, "Delete button should be visible for removing bookings");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Chờ MessageBox xuất hiện trên Desktop (MessageBox là cửa sổ hệ thống)
        /// </summary>
        private AutomationElement WaitForMessageBoxOnDesktop(TimeSpan timeout)
        {
            var desktop = _automation.GetDesktop();
            AutomationElement msgBox = null;

            Retry.WhileNull(() =>
            {
                msgBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                return msgBox;
            }, timeout, RetryInterval);

            return msgBox;
        }

        /// <summary>
        /// Wrapper: ưu tiên modal của owner, sau đó tìm trên desktop
        /// </summary>
        private AutomationElement WaitForMessageBox(Window owner, TimeSpan timeout)
        {
            if (owner != null)
            {
                try
                {
                    var modals = owner.ModalWindows;
                    if (modals.Length > 0) return modals[0];
                }
                catch { }
            }
            return WaitForMessageBoxOnDesktop(timeout);
        }

        /// <summary>
        /// Hàm đóng MessageBox thông minh: thử nhiều tên/AutomationId, fallback nút đầu tiên
        /// </summary>
        private void CloseMessageBox(AutomationElement messageBox)
        {
            if (messageBox == null) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"[CloseMessageBox] Handling dialog: {messageBox.Name}");

                var button = messageBox.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button)
                    .And(cf.ByName("OK")
                        .Or(cf.ByName("Yes"))
                        .Or(cf.ByName("Có"))
                        .Or(cf.ByName("Đồng ý"))
                        .Or(cf.ByName("Close"))
                        .Or(cf.ByAutomationId("2"))
                        .Or(cf.ByAutomationId("1"))
                        .Or(cf.ByAutomationId("6"))));

                if (button == null)
                {
                    var allButtons = messageBox.FindAllDescendants(cf => cf.ByControlType(ControlType.Button));
                    if (allButtons.Length > 0)
                    {
                        button = allButtons[0];
                        System.Diagnostics.Debug.WriteLine($"[CloseMessageBox] Fallback clicking first button: {button.Name}");
                    }
                }

                if (button != null)
                {
                    if (button.Patterns.Invoke.IsSupported)
                    {
                        button.Patterns.Invoke.Pattern.Invoke();
                    }
                    else
                    {
                        button.Click();
                    }

                    Thread.Sleep(500);
                    var desktop = _automation.GetDesktop();
                    Retry.WhileNotNull(() => desktop.FindFirstChild(cf => cf.ByClassName("#32770")), TimeSpan.FromSeconds(1), RetryInterval);
                }
                else
                {
                    WinForms.SendKeys.SendWait("{ENTER}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CloseMessageBox] Exception: {ex.Message}");
            }
        }

        private string GetMessageBoxText(AutomationElement messageBox)
        {
            if (messageBox == null) return string.Empty;
            try
            {
                var textElement = messageBox.FindFirstDescendant(cf => cf.ByClassName("Static").Or(cf.ByControlType(ControlType.Text)));
                if (textElement != null)
                {
                    return textElement.Name ?? string.Empty;
                }
                return messageBox.Name ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool WaitForCondition(Func<bool> condition, TimeSpan timeout)
        {
            return Retry.WhileFalse(condition, timeout, RetryInterval).Success;
        }

        private void WaitForMessageBoxToClose(TimeSpan timeout)
        {
            var desktop = _automation.GetDesktop();
            Retry.WhileNotNull(() => desktop.FindFirstChild(cf => cf.ByClassName("#32770")), timeout, RetryInterval);
        }

        private void SetTextBoxValue(TextBox textBox, string value)
        {
            if (textBox == null) return;
            try
            {
                textBox.Focus();
                Thread.Sleep(50);
                textBox.Text = value;
                Thread.Sleep(50);
            }
            catch { }
        }

        private void SetDatePickerValue(AutomationElement datePicker, DateTime date)
        {
            if (datePicker == null) return;
            try
            {
                var textBox = datePicker.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit))?.AsTextBox();
                if (textBox != null)
                {
                    textBox.Focus();
                    Thread.Sleep(100);
                    textBox.Text = date.ToString("dd/MM/yyyy");
                    Thread.Sleep(100);
                }
            }
            catch { }
        }

        private Window WaitForWindow(string automationId, TimeSpan timeout)
        {
            var desktop = _automation.GetDesktop();
            Window foundWindow = null;

            Retry.WhileNull(
                () =>
                {
                    foundWindow = desktop.FindFirstChild(cf => cf.ByAutomationId(automationId))?.AsWindow();
                    return foundWindow;
                },
                timeout,
                RetryInterval);

            return foundWindow;
        }

        private Window WaitForWindowByName(string name, TimeSpan timeout)
        {
            var desktop = _automation.GetDesktop();
            Window foundWindow = null;

            Retry.WhileNull(
                () =>
                {
                    foundWindow = desktop.FindFirstChild(cf => cf.ByName(name))?.AsWindow();
                    return foundWindow;
                },
                timeout,
                RetryInterval);

            return foundWindow;
        }

        private bool LoginAsStaff()
        {
            try
            {
                var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
                var pwdPassword = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"));
                var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

                if (txtUsername == null || pwdPassword == null || btnLogin == null)
                {
                    return _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingButton")) != null;
                }

                txtUsername.Text = "Fartiel";

                try
                {
                    pwdPassword.Focus();
                    Thread.Sleep(200);
                    var pwdTextBox = pwdPassword.AsTextBox();
                    if (pwdTextBox != null)
                    {
                        pwdTextBox.Text = "admin";
                    }
                    else
                    {
                        WinForms.SendKeys.SendWait("admin");
                    }
                }
                catch
                {
                    pwdPassword.Focus();
                    WinForms.SendKeys.SendWait("admin");
                }

                Thread.Sleep(300);
                btnLogin.Click();
                Thread.Sleep(1500);

                var messageBox = WaitForMessageBox(_mainWindow, TimeSpan.FromSeconds(5));
                if (messageBox != null)
                {
                    CloseMessageBox(messageBox);
                }

                Thread.Sleep(1500);

                var desktop = _automation.GetDesktop();
                bool found = false;

                Retry.WhileFalse(
                    () =>
                    {
                        var appWindows = desktop.FindAllChildren(cf => cf.ByProcessId(_app.ProcessId));
                        foreach (var window in appWindows)
                        {
                            var weddingButton = window.FindFirstDescendant(cf => cf.ByAutomationId("WeddingButton"));
                            if (weddingButton != null)
                            {
                                _mainWindow = window.AsWindow();
                                found = true;
                                return true;
                            }
                        }

                        return false;
                    },
                    TimeSpan.FromSeconds(5),
                    RetryInterval);

                return found;
            }
            catch
            {
                return false;
            }
        }

        private bool NavigateToWeddingTab()
        {
            try
            {
                var weddingButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingButton"))?.AsButton();
                if (weddingButton == null)
                    return false;

                weddingButton.Click();
                Thread.Sleep(1500);

                return WaitForCondition(
                    () => _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingPageTitle")) != null,
                    WindowTimeout);
            }
            catch
            {
                return false;
            }
        }

        private Window OpenAddWeddingForm()
        {
            try
            {
                var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddButton"))?.AsButton();
                if (addButton == null || !addButton.IsEnabled)
                    return null;

                addButton.Click();
                Thread.Sleep(1500);

                var desktop = _automation.GetDesktop();
                Window addWeddingWindow = null;

                Retry.WhileNull(
                    () =>
                    {
                        addWeddingWindow = desktop.FindFirstChild(cf => cf.ByAutomationId("DatTiecCuoi")
                            .Or(cf.ByName("Đặt Tiệc Cưới")))?.AsWindow();
                        return addWeddingWindow;
                    },
                    WindowTimeout,
                    RetryInterval);

                return addWeddingWindow;
            }
            catch
            {
                return null;
            }
        }

        private void CloseAllChildWindows()
        {
            try
            {
                var desktop = _automation?.GetDesktop();
                if (desktop == null) return;

                var strayMsg = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                if (strayMsg != null) CloseMessageBox(strayMsg);

                var menuWindow = desktop.FindFirstChild(cf => cf.ByAutomationId("MenuItemWindow").Or(cf.ByName("Chọn món ăn")))?.AsWindow();
                if (menuWindow != null)
                {
                    var cancel = menuWindow.FindFirstDescendant(cf => cf.ByName("Hủy").Or(cf.ByAutomationId("CancelSelectionButton")))?.AsButton();
                    cancel?.Click();
                    var confirm = WaitForMessageBoxOnDesktop(TimeSpan.FromSeconds(1));
                    if (confirm != null) CloseMessageBox(confirm);
                }

                var serviceWindow = desktop.FindFirstChild(cf => cf.ByAutomationId("ServiceItemWindow").Or(cf.ByName("Chọn dịch vụ")))?.AsWindow();
                if (serviceWindow != null)
                {
                    var cancel = serviceWindow.FindFirstDescendant(cf => cf.ByName("Hủy").Or(cf.ByAutomationId("CancelSelectionButton")))?.AsButton();
                    cancel?.Click();
                    var confirm = WaitForMessageBoxOnDesktop(TimeSpan.FromSeconds(1));
                    if (confirm != null) CloseMessageBox(confirm);
                }
            }
            catch { }
        }

        private void CloseAddWeddingWindow(Window addWeddingWindow)
        {
            if (addWeddingWindow == null) return;
            try
            {
                CloseAllChildWindows();

                var cancelButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("CancelButton"))?.AsButton();
                if (cancelButton != null && cancelButton.IsEnabled)
                {
                    cancelButton.Click();
                    Thread.Sleep(300);

                    var confirmBox = WaitForMessageBoxOnDesktop(TimeSpan.FromSeconds(2));
                    if (confirmBox != null)
                    {
                        var yesBtn = confirmBox.FindFirstDescendant(cf => cf.ByName("Yes").Or(cf.ByName("Có")).Or(cf.ByAutomationId("6")))?.AsButton();
                        if (yesBtn != null) yesBtn.Click();
                        else CloseMessageBox(confirmBox);
                    }
                }
                else
                {
                    addWeddingWindow.Close();
                }
            }
            catch
            {
                try { addWeddingWindow.Close(); } catch { }
            }
        }

        private void FillAllBasicFieldsOnly(Window addWeddingWindow)
        {
            SetTextBoxValue(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("GroomNameTextBox"))?.AsTextBox(), "Nguyễn Văn A");
            SetTextBoxValue(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("BrideNameTextBox"))?.AsTextBox(), "Trần Thị B");
            SetTextBoxValue(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("PhoneTextBox"))?.AsTextBox(), "0901234567");
            var dp = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingDatePicker"));
            SetDatePickerValue(dp, DateTime.Now.AddDays(7));
            var shift = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ShiftComboBox"))?.AsComboBox();
            if (shift != null) { shift.Expand(); Thread.Sleep(200); if (shift.Items.Length > 0) shift.Items[0].Select(); shift.Collapse(); }
            var hall = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("HallComboBox"))?.AsComboBox();
            if (hall != null) { hall.Expand(); Thread.Sleep(200); if (hall.Items.Length > 0) hall.Items[0].Select(); hall.Collapse(); }
            SetTextBoxValue(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("DepositTextBox"))?.AsTextBox(), "5000000");
            SetTextBoxValue(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("TableCountTextBox"))?.AsTextBox(), "20");
            SetTextBoxValue(addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ReserveTableCountTextBox"))?.AsTextBox(), "2");
        }

        private void FillAllRequiredFieldsComplete(Window addWeddingWindow)
        {
            FillAllBasicFieldsOnly(addWeddingWindow);
            AddMenuItemToList(addWeddingWindow);
            AddServiceItemToList(addWeddingWindow);
        }

        private void AddMenuItemToList(Window addWeddingWindow)
        {
            // Open menu selection dialog
            var selectDishButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("SelectDishButton"))?.AsButton();
            Assert.IsNotNull(selectDishButton, "SelectDishButton not found");
            selectDishButton.Click();
            Thread.Sleep(500);
            // Wait for menu dialog
            var desktop = _automation.GetDesktop();
            var menuWindow = desktop.FindFirstChild(cf => cf.ByAutomationId("MenuItemWindow").Or(cf.ByName("Chọn món ăn")))?.AsWindow();
            Assert.IsNotNull(menuWindow, "MenuItemWindow not found");
            // Select first dish
            var listBox = menuWindow.FindFirstDescendant(cf => cf.ByClassName("ListBox"))?.AsListBox();
            Assert.IsNotNull(listBox, "Menu ListBox not found");
            Assert.IsTrue(listBox.Items.Length > 0, "No menu items available");
            var firstItem = listBox.Items[0];
            firstItem.Click();
            Thread.Sleep(200);
            // Confirm selection
            var confirmBtn = menuWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmSelectionButton").Or(cf.ByName("Xác nhận")))?.AsButton();
            Assert.IsNotNull(confirmBtn, "Menu confirm button not found");
            confirmBtn.Click();
            Thread.Sleep(500);
            // Handle confirmation message box
            var msgBox = WaitForMessageBoxOnDesktop(TimeSpan.FromSeconds(2));
            if (msgBox != null) CloseMessageBox(msgBox);
            // Set quantity and add
            var menuQuantityTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("MenuQuantityTextBox"))?.AsTextBox();
            SetTextBoxValue(menuQuantityTextBox, "1");
            var addMenuButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddMenuButton"))?.AsButton();
            Assert.IsNotNull(addMenuButton, "AddMenuButton not found");
            addMenuButton.Click();
            Thread.Sleep(300);
        }

        private void AddServiceItemToList(Window addWeddingWindow)
        {
            // Open service selection dialog
            var selectServiceButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("SelectServiceButton"))?.AsButton();
            Assert.IsNotNull(selectServiceButton, "SelectServiceButton not found");
            selectServiceButton.Click();
            Thread.Sleep(500);
            // Wait for service dialog
            var desktop = _automation.GetDesktop();
            var serviceWindow = desktop.FindFirstChild(cf => cf.ByAutomationId("ServiceItemWindow").Or(cf.ByName("Chọn dịch vụ")))?.AsWindow();
            Assert.IsNotNull(serviceWindow, "ServiceItemWindow not found");
            // Select first service
            var listBox = serviceWindow.FindFirstDescendant(cf => cf.ByClassName("ListBox"))?.AsListBox();
            Assert.IsNotNull(listBox, "Service ListBox not found");
            Assert.IsTrue(listBox.Items.Length > 0, "No service items available");
            var firstItem = listBox.Items[0];
            firstItem.Click();
            Thread.Sleep(200);
            // Confirm selection
            var confirmBtn = serviceWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmSelectionButton").Or(cf.ByName("Xác nhận")))?.AsButton();
            Assert.IsNotNull(confirmBtn, "Service confirm button not found");
            confirmBtn.Click();
            Thread.Sleep(500);
            // Handle confirmation message box
            var msgBox = WaitForMessageBoxOnDesktop(TimeSpan.FromSeconds(2));
            if (msgBox != null) CloseMessageBox(msgBox);
            // Set quantity and add
            var serviceQuantityTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ServiceQuantityTextBox"))?.AsTextBox();
            SetTextBoxValue(serviceQuantityTextBox, "1");
            var addServiceButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddServiceButton"))?.AsButton();
            Assert.IsNotNull(addServiceButton, "AddServiceButton not found");
            addServiceButton.Click();
            Thread.Sleep(300);
        }

        #endregion
    }
}
