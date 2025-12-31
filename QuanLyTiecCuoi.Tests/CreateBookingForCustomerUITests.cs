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
    /// - ConfirmCommand shows MessageBox "B?n ?� ch?n m�n/d?ch v?: X" BEFORE closing window
    /// - CancelCommand shows MessageBox "B?n c� ch?c ch?n mu?n h?y kh�ng?" with Yes/No buttons
    /// So test code must handle these MessageBoxes properly!
    /// </summary>
    [TestClass]
    public class 
        Tests
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
                Assert.IsTrue(messageText.Contains("Table") || messageText.Contains("positive") || messageText.Contains("b�n"),
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
                    .Or(cf.ByName("Kh�ng")))?.AsButton();
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
        /// Wait for a condition to be true with retry logic
        /// </summary>
        private bool WaitForCondition(Func<bool> condition, TimeSpan timeout)
        {
            var result = Retry.WhileFalse(condition, timeout, RetryInterval);
            return result.Success;
        }

        /// <summary>
        /// Wait for MessageBox to close
        /// </summary>
        private void WaitForMessageBoxToClose(TimeSpan timeout)
        {
            var desktop = _automation.GetDesktop();
            Retry.WhileNotNull(
                () => desktop.FindFirstChild(cf => cf.ByClassName("#32770")),
                timeout,
                RetryInterval);
        }

        /// <summary>
        /// Set TextBox value with focus and clear existing text
        /// </summary>
        private void SetTextBoxValue(TextBox textBox, string value)
        {
            if (textBox == null) return;
            textBox.Focus();
            Thread.Sleep(100);
            textBox.Text = value;
            Thread.Sleep(100);
        }

        /// <summary>
        /// Wait for a window to appear by AutomationId
        /// </summary>
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

        /// <summary>
        /// Wait for a window to appear by name
        /// </summary>
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

                // Wait for login to complete
                var messageBox = WaitForMessageBox(_mainWindow, TimeSpan.FromSeconds(5));
                if (messageBox != null)
                {
                    CloseMessageBox(messageBox);
                }

                Thread.Sleep(1500);

                // Wait for main window with Wedding button
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
                
                // Wait for Wedding page to load
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

                // Wait for AddWedding window to appear
                var desktop = _automation.GetDesktop();
                Window addWeddingWindow = null;
                
                Retry.WhileNull(
                    () =>
                    {
                        addWeddingWindow = desktop.FindFirstChild(cf => cf.ByAutomationId("DatTiecCuoi")
                            .Or(cf.ByName("??t Ti?c C??i")))?.AsWindow();
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

        private AutomationElement WaitForMessageBox(Window ownerWindow, TimeSpan timeout)
        {
            var endTime = DateTime.UtcNow + timeout;
            var desktop = _automation.GetDesktop();

            while (DateTime.UtcNow < endTime)
            {
                try
                {
                    var modal = ownerWindow?.ModalWindows;
                    if (modal != null && modal.Length > 0)
                        return modal.FirstOrDefault();
                }
                catch { }

                var messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                if (messageBox != null)
                    return messageBox;

                Thread.Sleep((int)RetryInterval.TotalMilliseconds);
            }

            return null;
        }

        /// <summary>
        /// Wait for MessageBox on desktop (not tied to specific owner window)
        /// Used when the owner window might have closed
        /// </summary>
        private AutomationElement WaitForMessageBoxOnDesktop(TimeSpan timeout)
        {
            var desktop = _automation.GetDesktop();
            AutomationElement messageBox = null;
            
            Retry.WhileNull(
                () =>
                {
                    messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                    return messageBox;
                },
                timeout,
                RetryInterval);
            
            return messageBox;
        }

        private string GetMessageBoxText(AutomationElement messageBox)
        {
            if (messageBox == null) return string.Empty;
            
            try
            {
                var textElement = messageBox.FindFirstDescendant(cf => cf.ByClassName("Static"));
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

        private void CloseMessageBox(AutomationElement messageBox)
        {
            if (messageBox == null) return;
            var okButton = messageBox.FindFirstDescendant(cf => cf.ByName("OK")
                .Or(cf.ByName("Yes"))
                .Or(cf.ByName("C�"))
                .Or(cf.ByName("??ng �")))?.AsButton();
            okButton?.Click();
            Thread.Sleep(300);
            WaitForMessageBoxToClose(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Close all child windows (MenuItemView, ServiceDetailItemView) properly
        /// IMPORTANT: These windows show MessageBox on Cancel, so we need to handle them!
        /// </summary>
        private void CloseAllChildWindows()
        {
            try
            {
                var desktop = _automation?.GetDesktop();
                if (desktop == null) return;

                // Close MenuItemView if open
                var menuItemWindow = desktop.FindFirstChild(cf => cf.ByName("Ch?n m�n ?n")
                    .Or(cf.ByAutomationId("MenuItemWindow")))?.AsWindow();
                if (menuItemWindow != null)
                {
                    System.Diagnostics.Debug.WriteLine("Closing MenuItemWindow...");
                    var cancelBtn = menuItemWindow.FindFirstDescendant(cf => cf.ByName("H?y"))?.AsButton();
                    if (cancelBtn != null)
                    {
                        cancelBtn.Click();
                        Thread.Sleep(500);
                        
                        // CRITICAL: Handle the confirmation MessageBox "B?n c� ch?c ch?n mu?n h?y kh�ng?"
                        var confirmBox = WaitForMessageBoxOnDesktop(TimeSpan.FromSeconds(2));
                        if (confirmBox != null)
                        {
                            var yesButton = confirmBox.FindFirstDescendant(cf => cf.ByName("Yes")
                                .Or(cf.ByName("C�")))?.AsButton();
                            if (yesButton != null)
                            {
                                yesButton.Click();
                                WaitForMessageBoxToClose(TimeSpan.FromSeconds(1));
                            }
                            else
                            {
                                CloseMessageBox(confirmBox);
                            }
                        }
                    }
                    else
                    {
                        // Fallback: try to close directly
                        try { menuItemWindow.Close(); } catch { }
                    }
                }

                // Close ServiceDetailItemView if open
                var serviceItemWindow = desktop.FindFirstChild(cf => cf.ByName("Ch?n d?ch v?")
                    .Or(cf.ByAutomationId("ServiceItemWindow")))?.AsWindow();
                if (serviceItemWindow != null)
                {
                    System.Diagnostics.Debug.WriteLine("Closing ServiceItemWindow...");
                    var cancelBtn = serviceItemWindow.FindFirstDescendant(cf => cf.ByName("H?y"))?.AsButton();
                    if (cancelBtn != null)
                    {
                        cancelBtn.Click();
                        Thread.Sleep(500);
                        
                        // CRITICAL: Handle the confirmation MessageBox "B?n c� ch?c ch?n mu?n h?y kh�ng?"
                        var confirmBox = WaitForMessageBoxOnDesktop(TimeSpan.FromSeconds(2));
                        if (confirmBox != null)
                        {
                            var yesButton = confirmBox.FindFirstDescendant(cf => cf.ByName("Yes")
                                .Or(cf.ByName("C�")))?.AsButton();
                            if (yesButton != null)
                            {
                                yesButton.Click();
                                WaitForMessageBoxToClose(TimeSpan.FromSeconds(1));
                            }
                            else
                            {
                                CloseMessageBox(confirmBox);
                            }
                        }
                    }
                    else
                    {
                        // Fallback: try to close directly
                        try { serviceItemWindow.Close(); } catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CloseAllChildWindows: {ex.Message}");
            }
        }

        private void CloseAddWeddingWindow(Window addWeddingWindow)
        {
            if (addWeddingWindow == null) return;
            try
            {
                // First close any child windows
                CloseAllChildWindows();

                var cancelButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("CancelButton"))?.AsButton();
                cancelButton?.Click();
                Thread.Sleep(500);

                var confirmBox = WaitForMessageBox(addWeddingWindow, TimeSpan.FromSeconds(2));
                if (confirmBox != null)
                {
                    var yesButton = confirmBox.FindFirstDescendant(cf => cf.ByName("Yes")
                        .Or(cf.ByName("C�")))?.AsButton();
                    if (yesButton != null)
                    {
                        yesButton.Click();
                        WaitForMessageBoxToClose(TimeSpan.FromSeconds(1));
                    }
                    else
                    {
                        CloseMessageBox(confirmBox);
                    }
                }
            }
            catch
            {
                try { addWeddingWindow.Close(); } catch { }
            }
        }

        /// <summary>
        /// Fill only basic text fields (NOT Menu/Service)
        /// Used for testing MSG10 when Menu/Service are empty
        /// </summary>
        private void FillAllBasicFieldsOnly(Window addWeddingWindow)
        {
            // GroomName
            var groomNameTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("GroomNameTextBox"))?.AsTextBox();
            if (groomNameTextBox != null)
            {
                SetTextBoxValue(groomNameTextBox, "Nguy?n V?n A");
            }

            // BrideName
            var brideNameTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("BrideNameTextBox"))?.AsTextBox();
            if (brideNameTextBox != null)
            {
                SetTextBoxValue(brideNameTextBox, "Tr?n Th? B");
            }

            // Phone
            var phoneTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("PhoneTextBox"))?.AsTextBox();
            if (phoneTextBox != null)
            {
                SetTextBoxValue(phoneTextBox, "0901234567");
            }

            // Wedding date - set to future date to avoid validation issues
            var weddingDatePicker = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingDatePicker"));
            SetDatePickerValue(weddingDatePicker, DateTime.Now.AddDays(7));

            // Shift - Wait for items to load
            var shiftComboBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ShiftComboBox"))?.AsComboBox();
            if (shiftComboBox != null)
            {
                shiftComboBox.Expand();
                Thread.Sleep(500);
                WaitForCondition(() => shiftComboBox.Items.Length > 0, TimeSpan.FromSeconds(2));
                if (shiftComboBox.Items.Length > 0)
                {
                    shiftComboBox.Items[0].Select();
                }
                shiftComboBox.Collapse();
                Thread.Sleep(300);
            }

            // Hall - Wait for items to load
            var hallComboBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("HallComboBox"))?.AsComboBox();
            if (hallComboBox != null)
            {
                hallComboBox.Expand();
                Thread.Sleep(500);
                WaitForCondition(() => hallComboBox.Items.Length > 0, TimeSpan.FromSeconds(2));
                if (hallComboBox.Items.Length > 0)
                {
                    hallComboBox.Items[0].Select();
                }
                hallComboBox.Collapse();
                Thread.Sleep(300);
            }

            // Deposit
            var depositTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("DepositTextBox"))?.AsTextBox();
            if (depositTextBox != null)
            {
                SetTextBoxValue(depositTextBox, "5000000");
            }

            // TableCount
            var tableCountTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("TableCountTextBox"))?.AsTextBox();
            if (tableCountTextBox != null)
            {
                SetTextBoxValue(tableCountTextBox, "20");
            }

            // ReserveTableCount
            var reserveTableCountTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ReserveTableCountTextBox"))?.AsTextBox();
            if (reserveTableCountTextBox != null)
            {
                SetTextBoxValue(reserveTableCountTextBox, "2");
            }

            // NOTE: MenuList and ServiceList are NOT filled here - intentionally left empty
        }

        /// <summary>
        /// Fill ALL required fields INCLUDING Menu and Service items
        /// This is needed to test specific validations like phone format, deposit, etc.
        /// </summary>
        private void FillAllRequiredFieldsComplete(Window addWeddingWindow)
        {
            // First fill all basic fields
            FillAllBasicFieldsOnly(addWeddingWindow);

            // Add a Menu item - REQUIRED for BR141 tests (except BR141_001 and BR141_002)
            AddMenuItemToList(addWeddingWindow);

            // Add a Service item - REQUIRED for BR141 tests (except BR141_001 and BR141_002)
            AddServiceItemToList(addWeddingWindow);
        }

        /// <summary>
        /// Add a menu item to MenuList by:
        /// 1. Click SelectDishButton to open MenuItemView
        /// 2. Select first dish in list
        /// 3. Click "X�c nh?n" to confirm selection
        /// 4. HANDLE the success MessageBox "B?n ?� ch?n m�n: X" - CLICK OK
        /// 5. Window closes and returns to AddWeddingWindow
        /// 6. Enter quantity in MenuQuantityTextBox
        /// 7. Click AddMenuButton to add to MenuList
        /// </summary>
        private void AddMenuItemToList(Window addWeddingWindow)
        {
            try
            {
                Console.WriteLine("--- [Step] B?t ??u ch?n m�n ?n ---");

                var selectDishButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("SelectDishButton"))?.AsButton();
                Assert.IsNotNull(selectDishButton, "Kh�ng t�m th?y n�t 'Ch?n m�n' tr�n m�n h�nh ch�nh");
                selectDishButton.Click();

                var desktop = _automation.GetDesktop();
                var menuItemWindow = Retry.WhileNull(
                    () => desktop.FindFirstChild(cf => cf.ByAutomationId("MenuItemWindow").Or(cf.ByName("Ch?n m�n ?n")))?.AsWindow(),
                    WindowTimeout,
                    RetryInterval).Result;
                Assert.IsNotNull(menuItemWindow, "C?a s? ch?n m�n kh�ng hi?n l�n");

                var listBox = menuItemWindow.FindFirstDescendant(cf => cf.ByClassName("ListBox"))?.AsListBox();
                WaitForCondition(() => listBox != null && listBox.Items.Length > 0, TimeSpan.FromSeconds(5));

                if (listBox != null && listBox.Items.Length > 0)
                {
                    var firstItem = listBox.Items[0];
                    if (firstItem.Patterns.SelectionItem.IsSupported)
                        firstItem.Patterns.SelectionItem.Pattern.Select();
                    else
                        firstItem.Click();
                    Thread.Sleep(300);

                    var confirmBtn = menuItemWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmSelectionButton"))?.AsButton();
                    Assert.IsNotNull(confirmBtn, "Kh�ng t�m th?y n�t X�c nh?n (MenuItemWindow)");

                    WaitForCondition(() => confirmBtn.IsEnabled, TimeSpan.FromSeconds(3));
                    confirmBtn.Click();

                    var successBox = WaitForMessageBoxOnDesktop(MessageBoxTimeout);
                    if (successBox != null)
                    {
                        Console.WriteLine("=> ?� th?y th�ng b�o x�c nh?n m�n. ?ang nh?n OK...");
                        CloseMessageBox(successBox);
                    }

                    Retry.WhileNotNull(
                        () => desktop.FindFirstChild(cf => cf.ByAutomationId("MenuItemWindow")),
                        TimeSpan.FromSeconds(3),
                        RetryInterval);
                }

                var menuQuantityTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("MenuQuantityTextBox"))?.AsTextBox();
                if (menuQuantityTextBox != null)
                {
                    SetTextBoxValue(menuQuantityTextBox, "5");
                }

                var addMenuButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddMenuButton"))?.AsButton();
                if (addMenuButton != null)
                {
                    WaitForCondition(() => addMenuButton.IsEnabled, TimeSpan.FromSeconds(2));
                    if (addMenuButton.IsEnabled)
                    {
                        addMenuButton.Click();
                        Thread.Sleep(500);
                        Console.WriteLine("--- ?� th�m m�n ?n v�o danh s�ch ---");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[L?i AddMenuItemToList]: {ex.Message}");
                throw;
            }
        }

        private void AddServiceItemToList(Window addWeddingWindow)
        {
            try
            {
                Console.WriteLine("--- [Step] B?t ??u ch?n d?ch v? ---");

                var selectServiceButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("SelectServiceButton"))?.AsButton();
                selectServiceButton?.Click();

                var desktop = _automation.GetDesktop();
                var serviceWindow = Retry.WhileNull(
                    () => desktop.FindFirstChild(cf => cf.ByAutomationId("ServiceItemWindow").Or(cf.ByName("Ch?n d?ch v?")))?.AsWindow(),
                    WindowTimeout,
                    RetryInterval).Result;
                Assert.IsNotNull(serviceWindow, "C?a s? ch?n d?ch v? kh�ng hi?n l�n");

                var listBox = serviceWindow.FindFirstDescendant(cf => cf.ByClassName("ListBox"))?.AsListBox();
                WaitForCondition(() => listBox != null && listBox.Items.Length > 0, TimeSpan.FromSeconds(5));

                if (listBox != null && listBox.Items.Length > 0)
                {
                    var firstItem = listBox.Items[0];
                    if (firstItem.Patterns.SelectionItem.IsSupported)
                        firstItem.Patterns.SelectionItem.Pattern.Select();
                    else
                        firstItem.Click();
                    Thread.Sleep(300);

                    var confirmBtn = serviceWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmSelectionButton"))?.AsButton();
                    Assert.IsNotNull(confirmBtn, "Kh�ng t�m th?y n�t X�c nh?n d?ch v?");

                    WaitForCondition(() => confirmBtn.IsEnabled, TimeSpan.FromSeconds(2));
                    confirmBtn.Click();

                    var successBox = WaitForMessageBoxOnDesktop(MessageBoxTimeout);
                    if (successBox != null)
                    {
                        CloseMessageBox(successBox);
                    }

                    Retry.WhileNotNull(
                        () => desktop.FindFirstChild(cf => cf.ByAutomationId("ServiceItemWindow")),
                        TimeSpan.FromSeconds(3),
                        RetryInterval);
                }

                var serviceQuantityTextBox = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("ServiceQuantityTextBox"))?.AsTextBox();
                if (serviceQuantityTextBox != null)
                {
                    SetTextBoxValue(serviceQuantityTextBox, "1");
                }

                var addServiceButton = addWeddingWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddServiceButton"))?.AsButton();
                if (addServiceButton != null)
                {
                    WaitForCondition(() => addServiceButton.IsEnabled, TimeSpan.FromSeconds(2));
                    if (addServiceButton.IsEnabled)
                    {
                        addServiceButton.Click();
                        Thread.Sleep(500);
                        Console.WriteLine("--- ?� th�m d?ch v? v�o danh s�ch ---");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[L?i AddServiceItemToList]: {ex.Message}");
                throw;
            }
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
            catch
            {
            }
        }
        #endregion
    }
}
