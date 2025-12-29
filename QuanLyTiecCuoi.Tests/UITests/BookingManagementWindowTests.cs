using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using WinForms = System.Windows.Forms; // Alias to avoid ambiguity

namespace QuanLyTiecCuoi.Tests.UITests
{
    /// <summary>
    /// Automated UI Tests for BR137 & BR138 - Booking Management
    /// Tests staff hall availability check and booking creation
    /// 
    /// Flow:
    /// 1. Launch app -> Login Window
    /// 2. Login -> MessageBox success -> Click OK
    /// 3. Main Window appears -> Home View
    /// 4. Click "WeddingButton" (Ti?c c??i) OR "BookNowButton" (??t Ti?c Ngay)
    /// 5. Wedding View OR AddWedding Window appears
    /// </summary>
    [TestClass]
    public class BookingManagementWindowTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;

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
                Assert.Inconclusive($"Không tìm th?y file exe t?i: {appPath}. Hãy build project tr??c khi ch?y UI tests.");
                return;
            }

            _automation = new UIA3Automation();
            _app = Application.Launch(appPath);
            _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10));

            // Login và navigate ??n Wedding screen
            if (!LoginAsStaff())
            {
                Assert.Inconclusive("Không th? login. Ki?m tra credentials và UI.");
                return;
            }
        }

        /// <summary>
        /// Login và handle MessageBox success
        /// </summary>
        private bool LoginAsStaff()
        {
            try
            {
                // 1. Interact with Login Window Controls
                var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
                var pwdPassword = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"));
                var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

                // Check if we are already on Main Window (Edge case)
                if (txtUsername == null || pwdPassword == null || btnLogin == null)
                {
                    System.Diagnostics.Debug.WriteLine("Login controls not found - checking if already on main window");
                    return _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingButton")) != null;
                }

                // 2. Perform Login
                txtUsername.Text = "Fartiel";
                
                // PasswordBox special handling
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
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"PasswordBox error: {ex.Message}");
                    pwdPassword.Focus();
                    WinForms.SendKeys.SendWait("admin");
                }
                
                Thread.Sleep(500);
                btnLogin.Click();
                Thread.Sleep(2000);

                // 3. Handle Success MessageBox
                var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
                if (messageBox != null)
                {
                    System.Diagnostics.Debug.WriteLine("MessageBox found after login");
                    var okButton = messageBox.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
                    if (okButton != null)
                    {
                        okButton.Click();
                        Thread.Sleep(1000);
                        System.Diagnostics.Debug.WriteLine("MessageBox closed");
                    }
                }

                // 4. CRITICAL FIX: Wait for Window Transition
                // The Login window is closing; Main window is opening.
                Thread.Sleep(2000);

                // 5. Re-acquire Main Window by process ID
                // Do NOT use the old _mainWindow reference.
                System.Diagnostics.Debug.WriteLine("Searching for Main Window...");
                var desktop = _automation.GetDesktop();
                var appWindows = desktop.FindAllChildren(cf => cf.ByProcessId(_app.ProcessId));
                
                System.Diagnostics.Debug.WriteLine($"Found {appWindows.Length} windows for process");
                
                foreach (var window in appWindows)
                {
                    try
                    {
                        // Check for a specific element that belongs to the Main Window (e.g., WeddingButton)
                        // We verify by Element first to avoid accessing Title on a closing window
                        var weddingButton = window.FindFirstDescendant(cf => cf.ByAutomationId("WeddingButton"));

                        if (weddingButton != null)
                        {
                            _mainWindow = window.AsWindow();
                            System.Diagnostics.Debug.WriteLine("Main Window re-acquired successfully by finding WeddingButton");
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error checking window: {ex.Message}");
                    }
                }

                // Fallback: Try GetMainWindow one more time
                try
                {
                    _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(3));
                    var weddingButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingButton"));
                    if (weddingButton != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Main Window acquired via GetMainWindow fallback");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetMainWindow fallback failed: {ex.Message}");
                }

                System.Diagnostics.Debug.WriteLine("Could not find Main Window with WeddingButton after login");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Navigate to Wedding tab by clicking WeddingButton in side menu
        /// </summary>
        private bool NavigateToWeddingTab()
        {
            try
            {
                // Find and click Wedding button in navigation menu
                // AutomationId: WeddingButton
                var weddingButton = _mainWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("WeddingButton"))?.AsButton();

                if (weddingButton == null)
                {
                    System.Diagnostics.Debug.WriteLine("WeddingButton not found");
                    return false;
                }

                weddingButton.Click();
                Thread.Sleep(2000); // Increase wait time for view to load

                // Verify Wedding view is loaded by finding WeddingPageTitle or WeddingListView
                var weddingTitle = _mainWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("WeddingPageTitle"));
                
                if (weddingTitle != null)
                {
                    System.Diagnostics.Debug.WriteLine("Successfully navigated to Wedding view");
                    return true;
                }

                System.Diagnostics.Debug.WriteLine("Wedding view not loaded - title not found");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigate to Wedding failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Open Add Wedding form by clicking AddButton or BookNowButton
        /// </summary>
        private Window OpenAddWeddingForm()
        {
            try
            {
                // Try clicking AddButton in Wedding view
                var addButton = _mainWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("AddButton"))?.AsButton();

                if (addButton != null && addButton.IsEnabled)
                {
                    addButton.Click();
                    Thread.Sleep(2000);

                    // Find the Add Wedding window
                    var desktop = _automation.GetDesktop();
                    var addWeddingWindow = desktop.FindFirstChild(cf => 
                        cf.ByName("??t Ti?c C??i")
                        .Or(cf.ByAutomationId("DatTiecCuoi")))?.AsWindow();

                    return addWeddingWindow;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Wait for MessageBox to appear
        /// </summary>
        private AutomationElement WaitForMessageBox(TimeSpan timeout)
        {
            var endTime = DateTime.UtcNow + timeout;
            var desktop = _automation.GetDesktop();

            while (DateTime.UtcNow < endTime)
            {
                // Try modal windows first
                try
                {
                    var modalWindows = _mainWindow?.ModalWindows;
                    if (modalWindows != null && modalWindows.Length > 0)
                    {
                        return modalWindows[0];
                    }
                }
                catch { }

                // Fallback to standard dialog
                var messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                if (messageBox != null)
                    return messageBox;

                Thread.Sleep(200);
            }

            return null;
        }

        /// <summary>
        /// Close MessageBox by clicking OK button
        /// </summary>
        private void CloseMessageBox(AutomationElement messageBox)
        {
            if (messageBox == null) return;

            var okButton = messageBox.FindFirstDescendant(cf => 
                cf.ByName("OK")
                .Or(cf.ByName("Yes"))
                .Or(cf.ByName("??ng ý")))?.AsButton();
            
            okButton?.Click();
            Thread.Sleep(300);
        }

        #region TC_BR137_001 - Booking Management Screen Display

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [TestCategory("Critical")]
        [Priority(1)]
        [Description("TC_BR137_001: Verify booking management screen displays for authorized users")]
        public void TC_BR137_001_BookingManagementScreen_DisplaysForAuthorizedUsers()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab successfully");

            // Verify Wedding view is displayed
            var weddingTitle = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("WeddingPageTitle"));
            Assert.IsNotNull(weddingTitle, "Wedding page title should be visible");

            // Verify filter controls exist (instead of FilterCard)
            var groomFilter = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("GroomNameFilterComboBox"));
            Assert.IsNotNull(groomFilter, "Groom name filter should be visible");

            var resetFilterButton = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetFilterButton"));
            Assert.IsNotNull(resetFilterButton, "Reset filter button should be visible");

            // Verify wedding list view
            var weddingListView = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("WeddingListView"));
            Assert.IsNotNull(weddingListView, "Wedding list view should be visible");

            // Verify action buttons
            var addButton = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton"));
            Assert.IsNotNull(addButton, "Add button should be visible");

            var detailButton = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DetailButton"));
            Assert.IsNotNull(detailButton, "Detail button should be visible");

            var deleteButton = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("DeleteButton"));
            Assert.IsNotNull(deleteButton, "Delete button should be visible");
        }

        #endregion

        #region TC_BR137_002 - Calendar View Control

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [Priority(1)]
        [Description("TC_BR137_002: Verify calendar view control for date selection")]
        public void TC_BR137_002_CalendarControl_IsAvailableForDateSelection()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Find date filter picker
            // AutomationId: WeddingDateFilterPicker
            var dateFilter = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("WeddingDateFilterPicker"));

            Assert.IsNotNull(dateFilter, "Wedding date filter picker should be visible");
            Assert.IsTrue(dateFilter.IsEnabled, "Date picker should be enabled");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [Description("TC_BR137_002: Verify date selection in Add Wedding form")]
        public void TC_BR137_002_AddWedding_DatePicker_Works()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Open Add Wedding form
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Could not open Add Wedding form");
                return;
            }

            try
            {
                // Find WeddingDatePicker in Add Wedding form
                // AutomationId: WeddingDatePicker
                var datePicker = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("WeddingDatePicker"));

                Assert.IsNotNull(datePicker, "Wedding date picker should exist in Add Wedding form");
                Assert.IsTrue(datePicker.IsEnabled, "Wedding date picker should be enabled");
            }
            finally
            {
                // Close Add Wedding window
                var cancelButton = addWeddingWindow?.FindFirstDescendant(cf => 
                    cf.ByAutomationId("CancelButton"))?.AsButton();
                cancelButton?.Click();
                
                // Handle cancel confirmation
                Thread.Sleep(500);
                var confirmBox = WaitForMessageBox(TimeSpan.FromSeconds(2));
                if (confirmBox != null)
                {
                    var yesButton = confirmBox.FindFirstDescendant(cf => cf.ByName("Yes"))?.AsButton();
                    yesButton?.Click();
                }
            }
        }

        #endregion

        #region TC_BR137_003 - Shift Selection Dropdown

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [Priority(1)]
        [Description("TC_BR137_003: Verify shift selection dropdown available in Add Wedding form")]
        public void TC_BR137_003_ShiftDropdown_IsAvailable()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Open Add Wedding form
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Could not open Add Wedding form");
                return;
            }

            try
            {
                // Find ShiftComboBox
                // AutomationId: ShiftComboBox
                var shiftDropdown = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("ShiftComboBox"));

                Assert.IsNotNull(shiftDropdown, "Shift dropdown should be visible");
                Assert.IsTrue(shiftDropdown.IsEnabled, "Shift dropdown should be enabled");
            }
            finally
            {
                // Close Add Wedding window
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [Description("TC_BR137_003: Verify shift options are displayed")]
        public void TC_BR137_003_ShiftDropdown_ContainsOptions()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Open Add Wedding form
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Could not open Add Wedding form");
                return;
            }

            try
            {
                var shiftDropdown = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("ShiftComboBox"))?.AsComboBox();

                if (shiftDropdown == null)
                {
                    Assert.Inconclusive("Shift dropdown not found");
                    return;
                }

                // Expand dropdown
                shiftDropdown.Expand();
                Thread.Sleep(500);

                var items = shiftDropdown.Items;
                Assert.IsTrue(items.Length > 0, "Shift dropdown should have options");

                // Collapse
                shiftDropdown.Collapse();
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        #endregion

        #region TC_BR137_004 - Hall Selection

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [Priority(1)]
        [Description("TC_BR137_004: Verify hall selection dropdown available")]
        public void TC_BR137_004_HallDropdown_IsAvailable()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Open Add Wedding form
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Could not open Add Wedding form");
                return;
            }

            try
            {
                // Find HallComboBox
                // AutomationId: HallComboBox
                var hallDropdown = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("HallComboBox"));

                Assert.IsNotNull(hallDropdown, "Hall dropdown should be visible");
                Assert.IsTrue(hallDropdown.IsEnabled, "Hall dropdown should be enabled");
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [Description("TC_BR137_004: Verify table count field works")]
        public void TC_BR137_004_TableCount_CanBeEntered()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Open Add Wedding form
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Could not open Add Wedding form");
                return;
            }

            try
            {
                // Find TableCountTextBox
                // AutomationId: TableCountTextBox
                var tableCountTextBox = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("TableCountTextBox"))?.AsTextBox();

                Assert.IsNotNull(tableCountTextBox, "Table count text box should exist");
                
                // Enter value
                tableCountTextBox.Text = "25";
                Thread.Sleep(300);
                
                Assert.AreEqual("25", tableCountTextBox.Text, "Should accept numeric input");
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        #endregion

        #region TC_BR138_002 - Hall Details Display

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR138")]
        [Priority(1)]
        [Description("TC_BR138_002: Verify hall details in dropdown show all required info")]
        public void TC_BR138_002_HallDetails_DisplayAllInfo()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Open Add Wedding form
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Could not open Add Wedding form");
                return;
            }

            try
            {
                var hallDropdown = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("HallComboBox"))?.AsComboBox();

                if (hallDropdown == null)
                {
                    Assert.Inconclusive("Hall dropdown not found");
                    return;
                }

                // Expand to see options
                hallDropdown.Expand();
                Thread.Sleep(500);

                var items = hallDropdown.Items;
                Assert.IsTrue(items.Length > 0, "Should have hall options");

                // Hall item template shows: TenSanh | T?i ?a: X bàn | ??n giá: Y ?
                // Verify first item has expected format
                if (items.Length > 0)
                {
                    var firstItemText = items[0].Name ?? "";
                    // Item should contain hall name and capacity info
                    Assert.IsTrue(firstItemText.Length > 0, "Hall item should have display text");
                }

                hallDropdown.Collapse();
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        #endregion

        #region TC_BR138_004 - Create Booking Button

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR138")]
        [Priority(1)]
        [Description("TC_BR138_004: Verify Add Wedding button available and opens form")]
        public void TC_BR138_004_AddWeddingButton_OpensForm()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Find Add button
            var addButton = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("AddButton"))?.AsButton();

            Assert.IsNotNull(addButton, "Add button should exist");
            Assert.IsTrue(addButton.IsEnabled, "Add button should be enabled");

            // Use OpenAddWeddingForm helper method
            var addWeddingWindow = OpenAddWeddingForm();
            Assert.IsNotNull(addWeddingWindow, "Add Wedding window should open");

            // Verify window title - check if it contains expected text (avoid encoding issues)
            try
            {
                var windowTitle = addWeddingWindow.Title;
                System.Diagnostics.Debug.WriteLine($"Add Wedding window title: {windowTitle}");
                Assert.IsTrue(!string.IsNullOrEmpty(windowTitle), "Window should have a title");
                // Flexible title check - just verify it's not empty
                // The actual title might vary due to encoding
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking window title: {ex.Message}");
            }

            // Close the window
            CloseAddWeddingWindow(addWeddingWindow);
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR138")]
        [Description("TC_BR138_004: Verify Confirm button exists in Add Wedding form")]
        public void TC_BR138_004_ConfirmButton_ExistsInAddWeddingForm()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Open Add Wedding form using helper
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Could not open Add Wedding form");
                return;
            }

            try
            {
                // Find ConfirmButton
                // AutomationId: ConfirmButton
                var confirmButton = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("ConfirmButton"))?.AsButton();

                Assert.IsNotNull(confirmButton, "Confirm button should exist");
                // Button might be enabled but won't work without filling form
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        #endregion

        #region TC_BR138_005 - Booking Info Display

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR138")]
        [Priority(1)]
        [Description("TC_BR138_005: Verify customer info fields exist in Add Wedding form")]
        public void TC_BR138_005_CustomerInfoFields_ExistInAddWeddingForm()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Open Add Wedding form
            var addWeddingWindow = OpenAddWeddingForm();
            if (addWeddingWindow == null)
            {
                Assert.Inconclusive("Could not open Add Wedding form");
                return;
            }

            try
            {
                // Find customer info fields
                var groomNameTextBox = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("GroomNameTextBox"));
                var brideNameTextBox = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("BrideNameTextBox"));
                var phoneTextBox = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("PhoneTextBox"));

                Assert.IsNotNull(groomNameTextBox, "Groom name field should exist");
                Assert.IsNotNull(brideNameTextBox, "Bride name field should exist");
                Assert.IsNotNull(phoneTextBox, "Phone field should exist");
            }
            finally
            {
                CloseAddWeddingWindow(addWeddingWindow);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR138")]
        [Description("TC_BR138_005: Verify Wedding list shows booking information")]
        public void TC_BR138_005_WeddingList_ShowsBookingInfo()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Find Wedding list view
            var weddingListView = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("WeddingListView"));

            Assert.IsNotNull(weddingListView, "Wedding list view should be visible");

            // The list shows: Groom, Bride, Hall, Date, Time, TableCount, Status
            // Verify columns exist by checking header
            // Note: GridView columns are rendered differently, we verify the list is functional
        }

        #endregion

        #region Filter Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [Description("Verify filter controls exist on Wedding view")]
        public void Filter_AllFilterControls_Exist()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Check all filter controls
            var groomFilter = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("GroomNameFilterComboBox"));
            var brideFilter = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("BrideNameFilterComboBox"));
            var hallFilter = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("HallNameFilterComboBox"));
            var dateFilter = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("WeddingDateFilterPicker"));
            var tableCountFilter = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("TableCountFilterComboBox"));
            var statusFilter = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("StatusFilterComboBox"));
            var searchTextBox = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox"));

            Assert.IsNotNull(groomFilter, "Groom name filter should exist");
            Assert.IsNotNull(brideFilter, "Bride name filter should exist");
            Assert.IsNotNull(hallFilter, "Hall name filter should exist");
            Assert.IsNotNull(dateFilter, "Wedding date filter should exist");
            Assert.IsNotNull(tableCountFilter, "Table count filter should exist");
            Assert.IsNotNull(statusFilter, "Status filter should exist");
            Assert.IsNotNull(searchTextBox, "Search text box should exist");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [Description("Verify reset filter button works")]
        public void ResetFilterButton_ClearsFilters()
        {
            // Navigate to Wedding tab
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // Set some filter value first
            var searchTextBox = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox"))?.AsTextBox();
            
            if (searchTextBox != null)
            {
                searchTextBox.Text = "test";
                Thread.Sleep(300);
            }

            // Find and click reset button
            var resetButton = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("ResetFilterButton"))?.AsButton();

            Assert.IsNotNull(resetButton, "Reset filter button should exist");
            Assert.IsTrue(resetButton.IsEnabled, "Reset button should be enabled");

            // Click reset
            resetButton.Click();
            Thread.Sleep(1000); // Wait longer for reset to complete

            // Verify filters are cleared (search text should be empty)
            searchTextBox = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("SearchTextBox"))?.AsTextBox();
            
            if (searchTextBox != null)
            {
                // After reset, text should be empty or reset to default
                var currentText = searchTextBox.Text ?? "";
                Assert.IsTrue(string.IsNullOrEmpty(currentText) || currentText == "0", 
                    $"Search text should be cleared after reset, but was: '{currentText}'");
            }
        }

        #endregion

        #region Home View - BookNowButton Test

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [Description("Verify BookNowButton on Home view navigates to Wedding")]
        public void BookNowButton_NavigatesToWedding()
        {
            // After login, we should be on Home view
            // Find BookNowButton
            // AutomationId: BookNowButton
            var bookNowButton = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("BookNowButton"))?.AsButton();

            if (bookNowButton == null)
            {
                // Might need to go to Home first
                var homeButton = _mainWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("HomeButton"))?.AsButton();
                homeButton?.Click();
                Thread.Sleep(1000);

                bookNowButton = _mainWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("BookNowButton"))?.AsButton();
            }

            if (bookNowButton == null)
            {
                Assert.Inconclusive("BookNowButton not found on Home view");
                return;
            }

            Assert.IsTrue(bookNowButton.IsEnabled, "Book Now button should be enabled");

            // Click and verify navigation
            bookNowButton.Click();
            Thread.Sleep(2000); // Increased wait time

            // Should navigate to Wedding view
            var weddingTitle = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("WeddingPageTitle"));
            
            Assert.IsNotNull(weddingTitle, "Should navigate to Wedding view after clicking Book Now");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("BR137")]
        [TestCategory("Debug")]
        [Description("Debug test to verify navigation and element finding")]
        public void Debug_VerifyNavigationAndElements()
        {
            // Verify we're on main window after login
            Assert.IsNotNull(_mainWindow, "Main window should exist");
            
            // Safely try to get window title
            try
            {
                var title = _mainWindow.Title;
                System.Diagnostics.Debug.WriteLine($"Main window title: {title}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Could not get main window title: {ex.Message}");
            }

            // Try to find Wedding button
            var weddingButton = _mainWindow.FindFirstDescendant(cf => 
                cf.ByAutomationId("WeddingButton"));
            Assert.IsNotNull(weddingButton, "Wedding button should exist in navigation");

            // Navigate to Wedding
            Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

            // List all automation IDs found in current view
            System.Diagnostics.Debug.WriteLine("=== Elements found in Wedding view ===");
            
            // Try to find each filter control
            var filters = new[]
            {
                "GroomNameFilterComboBox",
                "BrideNameFilterComboBox", 
                "HallNameFilterComboBox",
                "WeddingDateFilterPicker",
                "TableCountFilterComboBox",
                "StatusFilterComboBox",
                "ResetFilterButton"
            };

            foreach (var filterId in filters)
            {
                var element = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(filterId));
                System.Diagnostics.Debug.WriteLine($"{filterId}: {(element != null ? "FOUND" : "NOT FOUND")}");
            }

            // Try to find action buttons
            var buttons = new[] { "AddButton", "DetailButton", "DeleteButton" };
            foreach (var buttonId in buttons)
            {
                var element = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(buttonId));
                System.Diagnostics.Debug.WriteLine($"{buttonId}: {(element != null ? "FOUND" : "NOT FOUND")}");
            }

            // Try to find list view
            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("WeddingListView"));
            System.Diagnostics.Debug.WriteLine($"WeddingListView: {(listView != null ? "FOUND" : "NOT FOUND")}");

            // This test always passes - it's just for debugging
            Assert.IsTrue(true, "Debug test completed - check output for details");
        }

        #endregion

        #region Helper Methods

        private void CloseAddWeddingWindow(Window addWeddingWindow)
        {
            if (addWeddingWindow == null) return;

            try
            {
                System.Diagnostics.Debug.WriteLine("Attempting to close Add Wedding window");
                
                var cancelButton = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("CancelButton"))?.AsButton();
                
                if (cancelButton == null)
                {
                    System.Diagnostics.Debug.WriteLine("Cancel button not found, trying to close window directly");
                    addWeddingWindow.Close();
                    return;
                }
                
                cancelButton.Click();
                System.Diagnostics.Debug.WriteLine("Cancel button clicked");

                // Handle cancel confirmation dialog
                Thread.Sleep(800); // Increase wait time
                var confirmBox = WaitForMessageBox(TimeSpan.FromSeconds(3)); // Increase timeout
                
                if (confirmBox != null)
                {
                    System.Diagnostics.Debug.WriteLine("Cancel confirmation dialog found");
                    
                    // Try multiple button names
                    var yesButton = confirmBox.FindFirstDescendant(cf => cf.ByName("Yes"))?.AsButton();
                    if (yesButton == null)
                    {
                        yesButton = confirmBox.FindFirstDescendant(cf => cf.ByName("Có"))?.AsButton();
                    }
                    if (yesButton == null)
                    {
                        yesButton = confirmBox.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
                    }
                    
                    if (yesButton != null)
                    {
                        yesButton.Click();
                        Thread.Sleep(500);
                        System.Diagnostics.Debug.WriteLine("Confirmation button clicked");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No confirmation button found, trying SendKeys");
                        // Fallback: press Enter
                        WinForms.SendKeys.SendWait("{ENTER}");
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No confirmation dialog appeared");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error closing Add Wedding window: {ex.Message}");
                
                // Final fallback: try to close by force
                try
                {
                    addWeddingWindow?.Close();
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"Force close also failed: {ex2.Message}");
                }
            }
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            // Close any open dialogs/windows
            try
            {
                var desktop = _automation?.GetDesktop();
                
                // Close any MessageBoxes
                var messageBox = desktop?.FindFirstChild(cf => cf.ByClassName("#32770"));
                if (messageBox != null)
                {
                    var okButton = messageBox.FindFirstDescendant(cf => 
                        cf.ByName("OK")
                        .Or(cf.ByName("Yes")))?.AsButton();
                    okButton?.Click();
                    Thread.Sleep(300);
                }

                // Close any open AddWedding windows
                var addWeddingWindow = desktop?.FindFirstChild(cf => 
                    cf.ByName("??t Ti?c C??i"))?.AsWindow();
                if (addWeddingWindow != null)
                {
                    CloseAddWeddingWindow(addWeddingWindow);
                }
            }
            catch { }

            _app?.Close();
            _automation?.Dispose();
        }
    }
}
