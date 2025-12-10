using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.IO;
using QuanLyTiecCuoi.Tests.Helpers;

namespace QuanLyTiecCuoi.Tests.UITests
{
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

            // Wait for navigation
            System.Threading.Thread.Sleep(3000);

            // Assert - Kiểm tra đã chuyển sang MainWindow
            var currentWindow = _app.GetMainWindow(_automation);
            Assert.IsTrue(
                currentWindow.Title.Contains("Main") ||
                currentWindow.Title.Contains("Quản lý") ||
                !currentWindow.Title.Contains("Login"),
                $"Không navigate được đến màn hình chính. Title hiện tại: {currentWindow.Title}");
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
            System.Threading.Thread.Sleep(2000);

            // Assert - Tìm MessageBox
            //var desktop = _automation.GetDesktop();
            //var messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770")); // MessageBox class name

            // wait up to 5s for a message box (polling)
            var desktop = _automation.GetDesktop();
            AutomationElement messageBox = null;
            var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            while (DateTime.UtcNow < timeout)
            {
                // 1) ưu tiên modal windows của main window
                try
                {
                    var modalWindows = _mainWindow?.ModalWindows;
                    if (modalWindows != null && modalWindows.Length > 0)
                    {
                        messageBox = modalWindows[0];
                        break;
                    }
                }
                catch { /* ignore transient UIA errors */ }

                // 2) fallback: tìm dialog tiêu chuẩn trên desktop (class #32770) hoặc theo title/content
                messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                //.Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window))
                //.And(cf.ByName("Notice").Or(cf.ByName("Please enter username!")).Or(cf.ByName("Thông báo"))));
                if (messageBox != null) break;

                System.Threading.Thread.Sleep(200);
            }

            Assert.IsNotNull(messageBox, "Không hiển thị MessageBox thông báo lỗi khi đăng nhập sai");
            
            // Đóng MessageBox nếu có
            var okButton = messageBox?.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
            okButton?.Click();
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

            System.Threading.Thread.Sleep(1500);

            // Assert - Tìm MessageBox cảnh báo
            //var desktop = _automation.GetDesktop();
            //var messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));

            // wait up to 5s for a message box (polling)
            var desktop = _automation.GetDesktop();
            AutomationElement messageBox = null;
            var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            while (DateTime.UtcNow < timeout)
            {
                // 1) ưu tiên modal windows của main window
                try
                {
                    var modalWindows = _mainWindow?.ModalWindows;
                    if (modalWindows != null && modalWindows.Length > 0)
                    {
                        messageBox = modalWindows[0];
                        break;
                    }
                }
                catch { /* ignore transient UIA errors */ }

                // 2) fallback: tìm dialog tiêu chuẩn trên desktop (class #32770) hoặc theo title/content
                messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                                                           //.Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window))
                                                           //.And(cf.ByName("Notice").Or(cf.ByName("Please enter username!")).Or(cf.ByName("Thông báo"))));
                if (messageBox != null) break;

                System.Threading.Thread.Sleep(200);
            }

            Assert.IsNotNull(messageBox, "Phải hiển thị thông báo khi username trống");
            
            // Đóng MessageBox
            var okButton = messageBox?.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
            okButton?.Click();
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

            System.Threading.Thread.Sleep(1500);

            // Assert
            //var desktop = _automation.GetDesktop();
            //var messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));

            // wait up to 5s for a message box (polling)
            var desktop = _automation.GetDesktop();
            AutomationElement messageBox = null;
            var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            while (DateTime.UtcNow < timeout)
            {
                // 1) ưu tiên modal windows của main window
                try
                {
                    var modalWindows = _mainWindow?.ModalWindows;
                    if (modalWindows != null && modalWindows.Length > 0)
                    {
                        messageBox = modalWindows[0];
                        break;
                    }
                }
                catch { /* ignore transient UIA errors */ }

                // 2) fallback: tìm dialog tiêu chuẩn trên desktop (class #32770) hoặc theo title/content
                messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                //.Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window))
                //.And(cf.ByName("Notice").Or(cf.ByName("Please enter username!")).Or(cf.ByName("Thông báo"))));
                if (messageBox != null) break;

                System.Threading.Thread.Sleep(200);
            }

            Assert.IsNotNull(messageBox, "Phải hiển thị thông báo khi password trống");
            
            var okButton = messageBox?.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
            okButton?.Click();
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

            System.Threading.Thread.Sleep(1500);

            // Assert
            //var desktop = _automation.GetDesktop();
            //var messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));

            // wait up to 5s for a message box (polling)
            var desktop = _automation.GetDesktop();
            AutomationElement messageBox = null;
            var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            while (DateTime.UtcNow < timeout)
            {
                // 1) ưu tiên modal windows của main window
                try
                {
                    var modalWindows = _mainWindow?.ModalWindows;
                    if (modalWindows != null && modalWindows.Length > 0)
                    {
                        messageBox = modalWindows[0];
                        break;
                    }
                }
                catch { /* ignore transient UIA errors */ }

                // 2) fallback: tìm dialog tiêu chuẩn trên desktop (class #32770) hoặc theo title/content
                messageBox = desktop.FindFirstChild(cf => cf.ByClassName("#32770"));
                //.Or(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window))
                //.And(cf.ByName("Notice").Or(cf.ByName("Please enter username!")).Or(cf.ByName("Thông báo"))));
                if (messageBox != null) break;

                System.Threading.Thread.Sleep(200);
            }

            Assert.IsNotNull(messageBox, "Phải hiển thị thông báo khi cả username và password đều trống");
            
            var okButton = messageBox?.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
            okButton?.Click();
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
            Assert.IsNotNull(welcomeText, "Welcome text phải hiển thị");
        }

        [TestMethod]
        [TestCategory("UI")]
        [TestCategory("Login")]
        [Description("Kiểm tra window title")]
        public void LoginWindow_ShouldHaveCorrectTitle()
        {
            // Assert
            Assert.AreEqual("Login", _mainWindow.Title, "Window title phải là 'Login'");
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
                    var okButton = messageBox.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
                    okButton?.Click();
                }
            }
            catch { }

            _app?.Close();
            _automation?.Dispose();
        }
    }
}