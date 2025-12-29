using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Threading;

namespace QuanLyTiecCuoi.Tests.UITests.Helpers
{
    /// <summary>
    /// Helper class for UI testing with FlaUI
    /// Provides common UI automation utilities for Wedding Management System
    /// </summary>
    public class UITestHelper
    {
        private readonly Application _app;
        private readonly UIA3Automation _automation;

        public UITestHelper(Application app, UIA3Automation automation)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            _automation = automation ?? throw new ArgumentNullException(nameof(automation));
        }

        #region Element Finding

        /// <summary>
        /// Wait for element to appear with timeout
        /// </summary>
        public AutomationElement WaitForElement(Window window, Func<ConditionFactory, ConditionBase> condition, 
            TimeSpan timeout)
        {
            var endTime = DateTime.UtcNow + timeout;
            AutomationElement element = null;

            while (DateTime.UtcNow < endTime)
            {
                element = window.FindFirstDescendant(condition);
                if (element != null)
                    return element;

                Thread.Sleep(200);
            }

            return null;
        }

        /// <summary>
        /// Find element by multiple possible AutomationIds
        /// </summary>
        public AutomationElement FindElementByIds(Window window, params string[] automationIds)
        {
            foreach (var id in automationIds)
            {
                var element = window.FindFirstDescendant(cf => cf.ByAutomationId(id));
                if (element != null)
                    return element;
            }

            return null;
        }

        #endregion

        #region MessageBox Handling

        /// <summary>
        /// Wait for MessageBox to appear (modal or standard dialog)
        /// </summary>
        public AutomationElement WaitForMessageBox(Window mainWindow, TimeSpan timeout)
        {
            var endTime = DateTime.UtcNow + timeout;
            var desktop = _automation.GetDesktop();
            AutomationElement messageBox = null;

            while (DateTime.UtcNow < endTime)
            {
                // Try modal windows first (more reliable for WPF)
                try
                {
                    var modalWindows = mainWindow?.ModalWindows;
                    if (modalWindows != null && modalWindows.Length > 0)
                    {
                        return modalWindows[0];
                    }
                }
                catch { }

                // Fallback to standard Win32 dialog (class #32770)
                messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                if (messageBox != null)
                    return messageBox;

                Thread.Sleep(200);
            }

            return null;
        }

        /// <summary>
        /// Close MessageBox by clicking OK or Yes button
        /// </summary>
        public void CloseMessageBox(AutomationElement messageBox, string preferredButton = "OK")
        {
            if (messageBox == null)
                return;

            // Try preferred button first
            var button = messageBox.FindFirstDescendant(cf => cf.ByName(preferredButton))?.AsButton();
            
            // Fallback to other common buttons
            if (button == null)
            {
                button = messageBox.FindFirstDescendant(cf => 
                    cf.ByName("OK")
                    .Or(cf.ByName("Yes"))
                    .Or(cf.ByName("Có"))
                    .Or(cf.ByName("??ng ý")))?.AsButton();
            }
            
            button?.Click();
            Thread.Sleep(300);
        }

        /// <summary>
        /// Close cancel confirmation dialog by clicking Yes
        /// </summary>
        public void ConfirmCancel(AutomationElement confirmDialog)
        {
            if (confirmDialog == null) return;

            var yesButton = confirmDialog.FindFirstDescendant(cf => 
                cf.ByName("Yes")
                .Or(cf.ByName("Có")))?.AsButton();
            
            yesButton?.Click();
            Thread.Sleep(300);
        }

        #endregion

        #region Login Helpers

        /// <summary>
        /// Perform login with credentials and handle success MessageBox
        /// </summary>
        /// <returns>Main window after login, or null if failed</returns>
        public Window LoginAndNavigateToMain(Window loginWindow, string username, string password)
        {
            try
            {
                var txtUsername = loginWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
                
                var pwdPassword = loginWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("PasswordBox"));
                
                var btnLogin = loginWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("LoginButton"))?.AsButton();

                if (txtUsername == null || pwdPassword == null || btnLogin == null)
                    return null;

                // Enter credentials
                txtUsername.Text = username;
                SetPasswordBoxText(pwdPassword, password);
                btnLogin.Click();

                Thread.Sleep(2000);

                // Handle success MessageBox
                var messageBox = WaitForMessageBox(loginWindow, TimeSpan.FromSeconds(5));
                if (messageBox != null)
                {
                    CloseMessageBox(messageBox);
                    Thread.Sleep(1000);
                }

                // Return updated main window
                return _app.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Set text in PasswordBox (requires special handling)
        /// </summary>
        public void SetPasswordBoxText(AutomationElement passwordBox, string password)
        {
            if (passwordBox == null)
                throw new ArgumentNullException(nameof(passwordBox));

            passwordBox.Focus();
            Thread.Sleep(100);
            
            var textBox = passwordBox.AsTextBox();
            if (textBox != null)
            {
                textBox.Text = password;
            }
        }

        #endregion

        #region Navigation Helpers

        /// <summary>
        /// Navigate to a tab by clicking navigation button
        /// </summary>
        public bool NavigateToTab(Window mainWindow, string buttonAutomationId)
        {
            try
            {
                var button = mainWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId(buttonAutomationId))?.AsButton();

                if (button == null || !button.IsEnabled)
                    return false;

                button.Click();
                Thread.Sleep(1500);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Navigate to Wedding tab (click WeddingButton)
        /// </summary>
        public bool NavigateToWeddingTab(Window mainWindow)
        {
            return NavigateToTab(mainWindow, "WeddingButton");
        }

        /// <summary>
        /// Navigate to Home tab (click HomeButton)
        /// </summary>
        public bool NavigateToHomeTab(Window mainWindow)
        {
            return NavigateToTab(mainWindow, "HomeButton");
        }

        /// <summary>
        /// Open Add Wedding form by clicking AddButton in Wedding view
        /// </summary>
        public Window OpenAddWeddingForm(Window mainWindow)
        {
            try
            {
                var addButton = mainWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("AddButton"))?.AsButton();

                if (addButton == null || !addButton.IsEnabled)
                    return null;

                addButton.Click();
                Thread.Sleep(2000);

                // Find the Add Wedding window
                var desktop = _automation.GetDesktop();
                var addWeddingWindow = desktop.FindFirstChild(cf => 
                    cf.ByName("??t Ti?c C??i")
                    .Or(cf.ByAutomationId("DatTiecCuoi")))?.AsWindow();

                return addWeddingWindow;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Close Add Wedding form with cancel confirmation
        /// </summary>
        public void CloseAddWeddingForm(Window addWeddingWindow)
        {
            if (addWeddingWindow == null) return;

            try
            {
                var cancelButton = addWeddingWindow.FindFirstDescendant(cf => 
                    cf.ByAutomationId("CancelButton"))?.AsButton();
                cancelButton?.Click();

                Thread.Sleep(500);

                // Handle cancel confirmation
                var confirmBox = WaitForMessageBox(addWeddingWindow, TimeSpan.FromSeconds(2));
                if (confirmBox != null)
                {
                    ConfirmCancel(confirmBox);
                }
            }
            catch
            {
                try
                {
                    addWeddingWindow.Close();
                }
                catch { }
            }
        }

        #endregion

        #region ComboBox Helpers

        /// <summary>
        /// Select item in ComboBox by text (partial match)
        /// </summary>
        public bool SelectComboBoxItem(AutomationElement comboBoxElement, string itemText)
        {
            if (comboBoxElement == null)
                return false;

            var comboBox = comboBoxElement.AsComboBox();
            if (comboBox == null)
                return false;

            try
            {
                comboBox.Expand();
                Thread.Sleep(300);

                foreach (var item in comboBox.Items)
                {
                    if (item.Text != null && item.Text.Contains(itemText))
                    {
                        item.Select();
                        Thread.Sleep(300);
                        return true;
                    }
                }

                comboBox.Collapse();
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get ComboBox item count
        /// </summary>
        public int GetComboBoxItemCount(AutomationElement comboBoxElement)
        {
            if (comboBoxElement == null)
                return 0;

            var comboBox = comboBoxElement.AsComboBox();
            if (comboBox == null)
                return 0;

            try
            {
                comboBox.Expand();
                Thread.Sleep(300);
                var count = comboBox.Items.Length;
                comboBox.Collapse();
                return count;
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region TextBox Helpers

        /// <summary>
        /// Get text from TextBox or Label
        /// </summary>
        public string GetText(AutomationElement element)
        {
            if (element == null)
                return null;

            try
            {
                var textBox = element.AsTextBox();
                if (textBox != null)
                    return textBox.Text;

                var label = element.AsLabel();
                if (label != null)
                    return label.Text;

                return element.Name;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Set text in TextBox
        /// </summary>
        public bool SetText(AutomationElement element, string text)
        {
            if (element == null)
                return false;

            try
            {
                var textBox = element.AsTextBox();
                if (textBox != null)
                {
                    textBox.Text = text;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Button Helpers

        /// <summary>
        /// Click button with retry logic
        /// </summary>
        public bool ClickButton(AutomationElement buttonElement, int maxRetries = 3)
        {
            if (buttonElement == null)
                return false;

            var button = buttonElement.AsButton();
            if (button == null)
                return false;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    if (button.IsEnabled)
                    {
                        button.Click();
                        Thread.Sleep(300);
                        return true;
                    }
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if element is visible and enabled
        /// </summary>
        public bool IsElementReady(AutomationElement element)
        {
            if (element == null)
                return false;

            try
            {
                return element.IsEnabled && element.IsOffscreen == false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region DataGrid Helpers

        /// <summary>
        /// Get row count from DataGrid/ListView
        /// </summary>
        public int GetDataGridRowCount(AutomationElement gridElement)
        {
            if (gridElement == null)
                return 0;

            try
            {
                var grid = gridElement.AsDataGridView();
                if (grid != null)
                    return grid.Rows.Length;
            }
            catch { }

            return 0;
        }

        /// <summary>
        /// Select row in DataGrid/ListView by index
        /// </summary>
        public bool SelectDataGridRow(AutomationElement gridElement, int rowIndex)
        {
            if (gridElement == null)
                return false;

            try
            {
                var grid = gridElement.AsDataGridView();
                if (grid != null && rowIndex < grid.Rows.Length)
                {
                    var row = grid.Rows[rowIndex];
                    row.Click();
                    Thread.Sleep(300);
                    return true;
                }
            }
            catch { }

            return false;
        }

        #endregion

        #region Screenshot Helpers

        /// <summary>
        /// Take screenshot of element
        /// </summary>
        public void CaptureScreenshot(AutomationElement element, string fileName)
        {
            if (element == null)
                return;

            try
            {
                var capture = FlaUI.Core.Capturing.Capture.Element(element);
                capture.ToFile($"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            }
            catch
            {
                // Screenshot failed, continue test
            }
        }

        #endregion

        #region Cleanup Helpers

        /// <summary>
        /// Close all open dialogs
        /// </summary>
        public void CloseAllDialogs()
        {
            try
            {
                var desktop = _automation.GetDesktop();
                
                // Close MessageBoxes
                var messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                if (messageBox != null)
                {
                    CloseMessageBox(messageBox);
                }

                // Close Add Wedding windows
                var addWeddingWindow = desktop.FindFirstChild(cf => 
                    cf.ByName("??t Ti?c C??i"))?.AsWindow();
                if (addWeddingWindow != null)
                {
                    CloseAddWeddingForm(addWeddingWindow);
                }
            }
            catch { }
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for arrays (compatibility with .NET Framework)
    /// </summary>
    public static class LinqExtensions
    {
        public static T FirstOrDefault<T>(this T[] array, Func<T, bool> predicate)
        {
            if (array == null)
                return default(T);

            foreach (var item in array)
            {
                if (predicate(item))
                    return item;
            }

            return default(T);
        }

        public static bool Any<T>(this T[] array, Func<T, bool> predicate)
        {
            if (array == null)
                return false;

            foreach (var item in array)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }

        public static bool Contains(this string source, string value, StringComparison comparison)
        {
            if (source == null || value == null)
                return false;

            return source.IndexOf(value, comparison) >= 0;
        }
    }
}
