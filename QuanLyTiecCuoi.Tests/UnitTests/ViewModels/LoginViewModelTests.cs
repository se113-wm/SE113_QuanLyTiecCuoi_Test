using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.ViewModel;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.Tests.UnitTests.ViewModels
{
    [TestClass]
    public class LoginViewModelTests
    {
        private Mock<IAppUserService> _mockAppUserService;
        private LoginViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _mockAppUserService = new Mock<IAppUserService>();
            _viewModel = new LoginViewModel(_mockAppUserService.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Constructor kh?i t?o Username r?ng")]
        public void Constructor_InitializesUsernameAsEmpty()
        {
            // Assert
            Assert.AreEqual(string.Empty, _viewModel.Username);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Constructor kh?i t?o Password r?ng")]
        public void Constructor_InitializesPasswordAsEmpty()
        {
            // Assert
            Assert.AreEqual(string.Empty, _viewModel.Password);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Constructor kh?i t?o LoginCommand")]
        public void Constructor_InitializesLoginCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.LoginCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Constructor kh?i t?o PasswordChangedCommand")]
        public void Constructor_InitializesPasswordChangedCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.PasswordChangedCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Constructor kh?i t?o UsernameChangedCommand")]
        public void Constructor_InitializesUsernameChangedCommand()
        {
            // Assert
            Assert.IsNotNull(_viewModel.UsernameChangedCommand);
        }

        #endregion

        #region Property Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Username property set và get ?úng")]
        public void Username_SetAndGet_WorksCorrectly()
        {
            // Arrange & Act
            _viewModel.Username = "testuser";

            // Assert
            Assert.AreEqual("testuser", _viewModel.Username);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Password property set và get ?úng")]
        public void Password_SetAndGet_WorksCorrectly()
        {
            // Arrange & Act
            _viewModel.Password = "testpassword";

            // Assert
            Assert.AreEqual("testpassword", _viewModel.Password);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Username có th? set null")]
        public void Username_CanBeSetToNull()
        {
            // Arrange & Act
            _viewModel.Username = null;

            // Assert
            Assert.IsNull(_viewModel.Username);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Password có th? set null")]
        public void Password_CanBeSetToNull()
        {
            // Arrange & Act
            _viewModel.Password = null;

            // Assert
            Assert.IsNull(_viewModel.Password);
        }

        #endregion

        #region PropertyChanged Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Username raise PropertyChanged event")]
        public void Username_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Username")
                    eventRaised = true;
            };

            // Act
            _viewModel.Username = "newuser";

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Password raise PropertyChanged event")]
        public void Password_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Password")
                    eventRaised = true;
            };

            // Act
            _viewModel.Password = "newpassword";

            // Assert
            Assert.IsTrue(eventRaised);
        }

        #endregion

        #region Command CanExecute Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("LoginCommand CanExecute luôn tr? v? true")]
        public void LoginCommand_CanExecute_AlwaysReturnsTrue()
        {
            // Act
            bool canExecute = _viewModel.LoginCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("PasswordChangedCommand CanExecute luôn tr? v? true")]
        public void PasswordChangedCommand_CanExecute_AlwaysReturnsTrue()
        {
            // Act
            bool canExecute = _viewModel.PasswordChangedCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("UsernameChangedCommand CanExecute luôn tr? v? true")]
        public void UsernameChangedCommand_CanExecute_AlwaysReturnsTrue()
        {
            // Act
            bool canExecute = _viewModel.UsernameChangedCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        #endregion

        #region Data Binding Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Thay ??i Username nhi?u l?n")]
        public void Username_MultipleChanges_TracksCorrectly()
        {
            // Act & Assert
            _viewModel.Username = "user1";
            Assert.AreEqual("user1", _viewModel.Username);

            _viewModel.Username = "user2";
            Assert.AreEqual("user2", _viewModel.Username);

            _viewModel.Username = "";
            Assert.AreEqual("", _viewModel.Username);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Username v?i ký t? ??c bi?t")]
        public void Username_WithSpecialCharacters_StoresCorrectly()
        {
            // Arrange
            string usernameWithSpecialChars = "user@domain.com!#$%";

            // Act
            _viewModel.Username = usernameWithSpecialChars;

            // Assert
            Assert.AreEqual(usernameWithSpecialChars, _viewModel.Username);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Password v?i ký t? ??c bi?t")]
        public void Password_WithSpecialCharacters_StoresCorrectly()
        {
            // Arrange
            string passwordWithSpecialChars = "P@$$w0rd!@#$%^&*()";

            // Act
            _viewModel.Password = passwordWithSpecialChars;

            // Assert
            Assert.AreEqual(passwordWithSpecialChars, _viewModel.Password);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Username v?i Unicode (Ti?ng Vi?t)")]
        public void Username_WithUnicode_StoresCorrectly()
        {
            // Arrange
            string unicodeUsername = "nguy?nv?nA";

            // Act
            _viewModel.Username = unicodeUsername;

            // Assert
            Assert.AreEqual(unicodeUsername, _viewModel.Username);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Username r?t dài")]
        public void Username_VeryLong_StoresCorrectly()
        {
            // Arrange
            string longUsername = new string('a', 500);

            // Act
            _viewModel.Username = longUsername;

            // Assert
            Assert.AreEqual(longUsername, _viewModel.Username);
            Assert.AreEqual(500, _viewModel.Username.Length);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Password r?t dài")]
        public void Password_VeryLong_StoresCorrectly()
        {
            // Arrange
            string longPassword = new string('x', 1000);

            // Act
            _viewModel.Password = longPassword;

            // Assert
            Assert.AreEqual(longPassword, _viewModel.Password);
            Assert.AreEqual(1000, _viewModel.Password.Length);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Username v?i kho?ng tr?ng")]
        public void Username_WithWhitespace_StoresAsIs()
        {
            // Arrange
            string usernameWithSpaces = "  user with spaces  ";

            // Act
            _viewModel.Username = usernameWithSpaces;

            // Assert
            Assert.AreEqual(usernameWithSpaces, _viewModel.Username);
        }

        #endregion

        #region Integration-like Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Simulate user flow: nh?p username và password")]
        public void SimulateUserFlow_EnterCredentials_StoresCorrectly()
        {
            // Simulate user entering credentials
            // Step 1: User types username
            _viewModel.Username = "a";
            _viewModel.Username = "ad";
            _viewModel.Username = "adm";
            _viewModel.Username = "admi";
            _viewModel.Username = "admin";

            // Step 2: User types password
            _viewModel.Password = "p";
            _viewModel.Password = "pa";
            _viewModel.Password = "pas";
            _viewModel.Password = "pass";
            _viewModel.Password = "passw";
            _viewModel.Password = "passwo";
            _viewModel.Password = "passwor";
            _viewModel.Password = "password";

            // Assert final values
            Assert.AreEqual("admin", _viewModel.Username);
            Assert.AreEqual("password", _viewModel.Password);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("LoginViewModel")]
        [Description("Simulate user clearing credentials")]
        public void SimulateUserFlow_ClearCredentials_ClearsCorrectly()
        {
            // Arrange
            _viewModel.Username = "admin";
            _viewModel.Password = "password";

            // Act - User clears fields
            _viewModel.Username = "";
            _viewModel.Password = "";

            // Assert
            Assert.AreEqual("", _viewModel.Username);
            Assert.AreEqual("", _viewModel.Password);
        }

        #endregion
    }
}
