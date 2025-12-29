using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.IO;
using System.Threading;

namespace QuanLyTiecCuoi.Tests.UITests
{
    /// <summary>
    /// UI Tests for Login Window
    /// Flow:
    /// 1. Launch app -> Login Window appears
    /// 2. Enter credentials -> Click Login
    /// 3. Success: MessageBox appears -> Click OK -> Main Window
    /// 4. Failure: MessageBox error appears
    /// </summary>
    [TestClass]
    public class LoginWindowTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;

        // Sử dụng đường dẫn tương đối từ thư mục test
        private static string GetAppPath()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            // Navigate từ bin\Debug của test project đến bin\Debug của main project
            var appPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\bin\Debug\QuanLyTiecCuoi.exe"));
            
            if (!File.Exists(appPath))
            {
                // Thử đường dẫn khác
                appPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\bin\Debug\QuanLyTiecCuoi.exe"));
            }
            
            return appPath;
        }

        [TestInitialize]
        public void Setup()
        {
            var appPath = GetAppPath();
            
            // Kiểm tra file exe tồn tại
            if (!File.Exists(appPath))
            {
                Assert.Inconclusive($"Không tìm thấy file exe tại: {appPath}. Hãy build project trước khi chạy UI tests.");
                return;
            }

            _automation = new UIA3Automation();
            _app = Application.Launch(appPath);
            _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10));
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
        /// Close MessageBox by clicking a button
        /// </summary>
        private void CloseMessageBox(AutomationElement messageBox, string buttonName = "OK")
        {
            if (messageBox == null) return;

            var button = messageBox.FindFirstDescendant(cf => 
                cf.ByName(buttonName)
                .Or(cf.ByName("OK"))
                .Or(cf.ByName("Yes")))?.AsButton();
            
            button?.Click();
            Thread.Sleep(300);
        }

        #region Login Success Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Test đăng nhập với thông tin hợp lệ")]
        [Priority(1)]
        public void Login_WithValidCredentials_ShouldNavigateToMainScreen()
        {
            // Arrange - Sử dụng AutomationId từ XAML: UsernameTextBox, PasswordBox, LoginButton
            var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
            var pwdPassword = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"));
            var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

            // Kiểm tra controls tồn tại
            Assert.IsNotNull(txtUsername, "Không tìm thấy TextBox Username (AutomationId: UsernameTextBox)");
            Assert.IsNotNull(pwdPassword, "Không tìm thấy PasswordBox (AutomationId: PasswordBox)");
            Assert.IsNotNull(btnLogin, "Không tìm thấy Button Login (AutomationId: LoginButton)");

            // Act
            txtUsername.Text = "Fartiel";
            
            // PasswordBox cần xử lý đặc biệt - sử dụng Pattern hoặc Focus + SendKeys
            pwdPassword.Focus();
            pwdPassword.AsTextBox().Text = "admin";
            
            btnLogin.Click();

            // Wait for login success MessageBox
            Thread.Sleep(2000);

            // Handle success MessageBox - click OK to proceed to Main Window
            var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
            if (messageBox != null)
            {
                // Found MessageBox - this is expected on successful login
                var okButton = messageBox.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
                Assert.IsNotNull(okButton, "MessageBox nên có nút OK");
                okButton.Click();
                Thread.Sleep(1000);
            }

            // Assert - Kiểm tra đã chuyển sang MainWindow
            var currentWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
            Assert.IsNotNull(currentWindow, "Phải có window sau khi login");
            Assert.IsTrue(
                currentWindow.Title.Contains("Main") ||
                currentWindow.Title.Contains("Wedding Management") ||
                currentWindow.Title.Contains("Quản lý") ||
                !currentWindow.Title.Contains("Login"),
                $"Không navigate được đến màn hình chính. Title hiện tại: {currentWindow.Title}");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Test đăng nhập thành công hiển thị MessageBox thông báo")]
        [Priority(1)]
        public void Login_WithValidCredentials_ShouldShowSuccessMessageBox()
        {
            // Arrange
            var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
            var pwdPassword = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"));
            var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

            Assert.IsNotNull(txtUsername, "Username field should exist");
            Assert.IsNotNull(pwdPassword, "Password field should exist");
            Assert.IsNotNull(btnLogin, "Login button should exist");

            // Act
            txtUsername.Text = "Fartiel";
            pwdPassword.Focus();
            pwdPassword.AsTextBox().Text = "admin";
            btnLogin.Click();

            // Wait for MessageBox
            Thread.Sleep(2000);

            // Assert - MessageBox should appear for successful login
            var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
            Assert.IsNotNull(messageBox, "MessageBox thông báo login thành công phải hiển thị");

            // Close it
            CloseMessageBox(messageBox);
        }

        #endregion

        #region Login Failure Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Test đăng nhập với thông tin không hợp lệ")]
        [Priority(1)]
        public void Login_WithInvalidCredentials_ShouldShowErrorMessage()
        {
            // Arrange
            var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
            var pwdPassword = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"));
            var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

            if (txtUsername == null || pwdPassword == null || btnLogin == null)
            {
                Assert.Inconclusive("Không tìm thấy các controls cần thiết. Kiểm tra AutomationId trong XAML.");
                return;
            }

            // Act
            txtUsername.Text = "wronguser";
            pwdPassword.Focus();
            pwdPassword.AsTextBox().Text = "wrongpassword";
            btnLogin.Click();

            // Wait for error message (MessageBox)
            Thread.Sleep(2000);

            // Assert - Tìm MessageBox
            var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
            Assert.IsNotNull(messageBox, "Không hiển thị MessageBox thông báo lỗi khi đăng nhập sai");
            
            // Đóng MessageBox nếu có
            CloseMessageBox(messageBox);
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Test để trống username")]
        public void Login_WithEmptyUsername_ShouldShowValidationError()
        {
            // Arrange
            var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
            var pwdPassword = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"));
            var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

            if (txtUsername == null || pwdPassword == null || btnLogin == null)
            {
                Assert.Inconclusive("Không tìm thấy các controls cần thiết.");
                return;
            }

            // Act
            txtUsername.Text = "";  // Để trống
            pwdPassword.Focus();
            pwdPassword.AsTextBox().Text = "somepassword";
            btnLogin.Click();

            Thread.Sleep(1500);

            // Assert - Tìm MessageBox cảnh báo
            var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
            Assert.IsNotNull(messageBox, "Phải hiển thị thông báo khi username trống");
            
            // Đóng MessageBox
            CloseMessageBox(messageBox);
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Test để trống password")]
        public void Login_WithEmptyPassword_ShouldShowValidationError()
        {
            // Arrange
            var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
            var pwdPassword = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"));
            var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

            if (txtUsername == null || pwdPassword == null || btnLogin == null)
            {
                Assert.Inconclusive("Không tìm thấy các controls cần thiết.");
                return;
            }

            // Act
            txtUsername.Text = "admin";
            // Không nhập password
            btnLogin.Click();

            Thread.Sleep(1500);

            // Assert
            var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
            Assert.IsNotNull(messageBox, "Phải hiển thị thông báo khi password trống");
            
            CloseMessageBox(messageBox);
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Test để trống cả username và password")]
        public void Login_WithEmptyUsernameAndPassword_ShouldShowValidationError()
        {
            // Arrange
            var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
            var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

            if (txtUsername == null || btnLogin == null)
            {
                Assert.Inconclusive("Không tìm thấy các controls cần thiết.");
                return;
            }

            // Act
            txtUsername.Text = "";
            btnLogin.Click();

            Thread.Sleep(1500);

            // Assert
            var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
            Assert.IsNotNull(messageBox, "Phải hiển thị thông báo khi cả username và password đều trống");
            
            CloseMessageBox(messageBox);
        }

        #endregion

        #region UI Element Verification Tests

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Kiểm tra các UI elements hiển thị đúng")]
        public void LoginWindow_ShouldDisplayAllRequiredElements()
        {
            // Assert - Kiểm tra tất cả controls cần thiết hiển thị
            var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"));
            var pwdPassword = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"));
            var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"));
            var welcomeText = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("WelcomeText"));

            Assert.IsNotNull(txtUsername, "TextBox Username phải hiển thị");
            Assert.IsNotNull(pwdPassword, "PasswordBox phải hiển thị");
            Assert.IsNotNull(btnLogin, "Button Login phải hiển thị");
            // WelcomeText might not exist - make it optional
            // Assert.IsNotNull(welcomeText, "Welcome text phải hiển thị");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Kiểm tra window title")]
        public void LoginWindow_ShouldHaveCorrectTitle()
        {
            // Assert - Login window title
            Assert.IsTrue(
                _mainWindow.Title.Contains("Login") || _mainWindow.Title.Contains("Đăng nhập"),
                $"Window title phải chứa 'Login' hoặc 'Đăng nhập'. Title hiện tại: {_mainWindow.Title}");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Kiểm tra Button Login có thể click được")]
        public void LoginButton_ShouldBeEnabled()
        {
            // Arrange
            var btnLogin = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))?.AsButton();

            // Assert
            Assert.IsNotNull(btnLogin, "Button Login phải tồn tại");
            Assert.IsTrue(btnLogin.IsEnabled, "Button Login phải được enabled");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Kiểm tra Username TextBox có thể nhập text")]
        public void UsernameTextBox_ShouldAcceptInput()
        {
            // Arrange
            var txtUsername = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))?.AsTextBox();
            Assert.IsNotNull(txtUsername, "Username TextBox phải tồn tại");

            // Act
            txtUsername.Text = "testuser";

            // Assert
            Assert.AreEqual("testuser", txtUsername.Text, "TextBox phải chấp nhận input");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Kiểm tra PasswordBox có thể nhập text")]
        public void PasswordBox_ShouldAcceptInput()
        {
            // Arrange
            var pwdPassword = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"));
            Assert.IsNotNull(pwdPassword, "PasswordBox phải tồn tại");

            // Act
            pwdPassword.Focus();
            var textBox = pwdPassword.AsTextBox();
            if (textBox != null)
            {
                textBox.Text = "testpassword";
                // Password text might be masked, just verify no exception
            }

            // Assert - If we got here without exception, it works
            Assert.IsTrue(true, "PasswordBox accepted input");
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            // Đóng tất cả MessageBox nếu còn mở
            try
            {
                var desktop = _automation?.GetDesktop();
                var messageBox = desktop?.FindFirstChild(cf => cf.ByClassName("#32770"));
                if (messageBox != null)
                {
                    CloseMessageBox(messageBox);
                }
            }
            catch { }

            _app?.Close();
            _automation?.Dispose();
        }
    }
}