using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.UnitTests.Services
{
    [TestClass]
    public class AppUserServiceTests
    {
        private Mock<IAppUserRepository> _mockRepository;
        private AppUserService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IAppUserRepository>();
            _service = new AppUserService(_mockRepository.Object);
        }

        #region GetAll Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("GetAll tr? v? danh sách users ?úng")]
        public void GetAll_WhenUsersExist_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<AppUser>
            {
                new AppUser 
                { 
                    UserId = 1, 
                    Username = "admin", 
                    FullName = "Administrator",
                    Email = "admin@test.com",
                    PhoneNumber = "0901234567",
                    UserGroup = new UserGroup { GroupId = "1", GroupName = "Admin" }
                },
                new AppUser 
                { 
                    UserId = 2, 
                    Username = "user1", 
                    FullName = "User One",
                    Email = "user1@test.com",
                    PhoneNumber = "0907654321",
                    UserGroup = new UserGroup { GroupId = "2", GroupName = "User" }
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(users);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("admin", result[0].Username);
            Assert.AreEqual("user1", result[1].Username);
            _mockRepository.Verify(r => r.GetAll(), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("GetAll tr? v? danh sách r?ng khi không có users")]
        public void GetAll_WhenNoUsers_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll()).Returns(new List<AppUser>());

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("GetAll map ?úng UserGroup navigation property")]
        public void GetAll_ShouldMapUserGroupCorrectly()
        {
            // Arrange
            var users = new List<AppUser>
            {
                new AppUser 
                { 
                    UserId = 1, 
                    Username = "admin",
                    UserGroup = new UserGroup { GroupId = "ADMIN", GroupName = "Administrators" }
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(users);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNotNull(result.UserGroup);
            Assert.AreEqual("ADMIN", result.UserGroup.GroupId);
            Assert.AreEqual("Administrators", result.UserGroup.GroupName);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("GetAll x? lý UserGroup null ?úng cách")]
        public void GetAll_WhenUserGroupIsNull_ShouldHandleGracefully()
        {
            // Arrange
            var users = new List<AppUser>
            {
                new AppUser 
                { 
                    UserId = 1, 
                    Username = "admin",
                    UserGroup = null
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(users);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNull(result.UserGroup);
        }

        #endregion

        #region GetById Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("GetById tr? v? user ?úng khi tìm th?y")]
        public void GetById_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var user = new AppUser 
            { 
                UserId = 1, 
                Username = "admin", 
                FullName = "Administrator",
                Email = "admin@test.com",
                UserGroup = new UserGroup { GroupId = "1", GroupName = "Admin" }
            };

            _mockRepository.Setup(r => r.GetById(1)).Returns(user);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.UserId);
            Assert.AreEqual("admin", result.Username);
            Assert.AreEqual("Administrator", result.FullName);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("GetById tr? v? null khi không tìm th?y")]
        public void GetById_WhenUserNotExists_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((AppUser)null);

            // Act
            var result = _service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("GetById v?i id âm tr? v? null")]
        public void GetById_WithNegativeId_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(-1)).Returns((AppUser)null);

            // Act
            var result = _service.GetById(-1);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Create Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("Create g?i repository ?úng cách")]
        public void Create_ValidUser_CallsRepositoryCreate()
        {
            // Arrange
            var userDto = new AppUserDTO 
            { 
                Username = "newuser", 
                FullName = "New User",
                Email = "newuser@test.com",
                PhoneNumber = "0901234567",
                PasswordHash = "hashedpassword"
            };

            _mockRepository.Setup(r => r.Create(It.IsAny<AppUser>()));

            // Act
            _service.Create(userDto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<AppUser>(u => 
                u.Username == "newuser" && 
                u.FullName == "New User" &&
                u.Email == "newuser@test.com")), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("Create map t?t c? properties ?úng")]
        public void Create_ShouldMapAllPropertiesCorrectly()
        {
            // Arrange
            var userDto = new AppUserDTO 
            { 
                UserId = 0,
                Username = "testuser", 
                PasswordHash = "hash123",
                FullName = "Test User",
                Email = "test@example.com",
                PhoneNumber = "0909090909",
                Address = "123 Test Street",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                GroupId = "USER"
            };

            AppUser capturedUser = null;
            _mockRepository.Setup(r => r.Create(It.IsAny<AppUser>()))
                .Callback<AppUser>(u => capturedUser = u);

            // Act
            _service.Create(userDto);

            // Assert
            Assert.IsNotNull(capturedUser);
            Assert.AreEqual("testuser", capturedUser.Username);
            Assert.AreEqual("hash123", capturedUser.PasswordHash);
            Assert.AreEqual("Test User", capturedUser.FullName);
            Assert.AreEqual("test@example.com", capturedUser.Email);
            Assert.AreEqual("0909090909", capturedUser.PhoneNumber);
            Assert.AreEqual("123 Test Street", capturedUser.Address);
            Assert.AreEqual(new DateTime(1990, 1, 1), capturedUser.BirthDate);
            Assert.AreEqual("Male", capturedUser.Gender);
            Assert.AreEqual("USER", capturedUser.GroupId);
        }

        #endregion

        #region Update Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("Update g?i repository ?úng cách")]
        public void Update_ValidUser_CallsRepositoryUpdate()
        {
            // Arrange
            var userDto = new AppUserDTO 
            { 
                UserId = 1,
                Username = "updateduser", 
                FullName = "Updated User"
            };

            _mockRepository.Setup(r => r.Update(It.IsAny<AppUser>()));

            // Act
            _service.Update(userDto);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<AppUser>(u => 
                u.UserId == 1 && 
                u.Username == "updateduser")), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("Update map t?t c? properties ?úng")]
        public void Update_ShouldMapAllPropertiesCorrectly()
        {
            // Arrange
            var userDto = new AppUserDTO 
            { 
                UserId = 5,
                Username = "updateduser", 
                PasswordHash = "newhash",
                FullName = "Updated Full Name",
                Email = "updated@example.com",
                PhoneNumber = "0911111111",
                Address = "456 Updated St",
                BirthDate = new DateTime(1995, 5, 15),
                Gender = "Female",
                GroupId = "ADMIN"
            };

            AppUser capturedUser = null;
            _mockRepository.Setup(r => r.Update(It.IsAny<AppUser>()))
                .Callback<AppUser>(u => capturedUser = u);

            // Act
            _service.Update(userDto);

            // Assert
            Assert.IsNotNull(capturedUser);
            Assert.AreEqual(5, capturedUser.UserId);
            Assert.AreEqual("updateduser", capturedUser.Username);
            Assert.AreEqual("newhash", capturedUser.PasswordHash);
            Assert.AreEqual("Updated Full Name", capturedUser.FullName);
            Assert.AreEqual("updated@example.com", capturedUser.Email);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("Delete g?i repository ?úng cách")]
        public void Delete_ValidId_CallsRepositoryDelete()
        {
            // Arrange
            _mockRepository.Setup(r => r.Delete(It.IsAny<int>()));

            // Act
            _service.Delete(1);

            // Assert
            _mockRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("AppUserService")]
        [Description("Delete v?i id = 0")]
        public void Delete_WithZeroId_StillCallsRepository()
        {
            // Arrange
            _mockRepository.Setup(r => r.Delete(It.IsAny<int>()));

            // Act
            _service.Delete(0);

            // Assert
            _mockRepository.Verify(r => r.Delete(0), Times.Once);
        }

        #endregion
    }
}
