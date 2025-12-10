using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.UnitTests.Services
{
    [TestClass]
    public class HallServiceTests
    {
        private Mock<IHallRepository> _mockRepository;
        private HallService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IHallRepository>();
            _service = new HallService(_mockRepository.Object);
        }

        #region GetAll Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("GetAll tr? v? danh sách halls ?úng")]
        public void GetAll_WhenHallsExist_ReturnsAllHalls()
        {
            // Arrange
            var halls = new List<Hall>
            {
                new Hall 
                { 
                    HallId = 1, 
                    HallName = "S?nh Diamond", 
                    MaxTableCount = 50,
                    Note = "S?nh cao c?p",
                    HallType = new HallType 
                    { 
                        HallTypeId = 1, 
                        HallTypeName = "VIP",
                        MinTablePrice = 2000000 
                    }
                },
                new Hall 
                { 
                    HallId = 2, 
                    HallName = "S?nh Ruby", 
                    MaxTableCount = 30,
                    HallType = new HallType 
                    { 
                        HallTypeId = 2, 
                        HallTypeName = "Standard",
                        MinTablePrice = 1000000 
                    }
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(halls);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("S?nh Diamond", result[0].HallName);
            Assert.AreEqual("S?nh Ruby", result[1].HallName);
            _mockRepository.Verify(r => r.GetAll(), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("GetAll tr? v? danh sách r?ng khi không có halls")]
        public void GetAll_WhenNoHalls_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll()).Returns(new List<Hall>());

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("GetAll map ?úng HallType")]
        public void GetAll_ShouldMapHallTypeCorrectly()
        {
            // Arrange
            var halls = new List<Hall>
            {
                new Hall 
                { 
                    HallId = 1,
                    HallName = "Test Hall",
                    HallType = new HallType 
                    { 
                        HallTypeId = 1, 
                        HallTypeName = "VIP",
                        MinTablePrice = 1500000 
                    }
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(halls);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNotNull(result.HallType);
            Assert.AreEqual(1, result.HallType.HallTypeId);
            Assert.AreEqual("VIP", result.HallType.HallTypeName);
            Assert.AreEqual(1500000, result.HallType.MinTablePrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("GetAll x? lý HallType null")]
        public void GetAll_WhenHallTypeIsNull_HandlesGracefully()
        {
            // Arrange
            var halls = new List<Hall>
            {
                new Hall 
                { 
                    HallId = 1,
                    HallName = "Test Hall",
                    HallType = null
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(halls);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNull(result.HallType);
        }

        #endregion

        #region GetById Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("GetById tr? v? hall ?úng khi tìm th?y")]
        public void GetById_WhenHallExists_ReturnsHall()
        {
            // Arrange
            var hall = new Hall 
            { 
                HallId = 1, 
                HallName = "S?nh Diamond", 
                MaxTableCount = 50,
                Note = "S?nh cao c?p nh?t",
                HallTypeId = 1,
                HallType = new HallType 
                { 
                    HallTypeId = 1, 
                    HallTypeName = "VIP" 
                }
            };

            _mockRepository.Setup(r => r.GetById(1)).Returns(hall);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.HallId);
            Assert.AreEqual("S?nh Diamond", result.HallName);
            Assert.AreEqual(50, result.MaxTableCount);
            Assert.AreEqual("S?nh cao c?p nh?t", result.Note);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("GetById tr? v? null khi không tìm th?y")]
        public void GetById_WhenHallNotExists_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Hall)null);

            // Act
            var result = _service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("GetById v?i id âm")]
        public void GetById_WithNegativeId_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(-1)).Returns((Hall)null);

            // Act
            var result = _service.GetById(-1);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Create Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("Create g?i repository ?úng cách")]
        public void Create_ValidHall_CallsRepositoryCreate()
        {
            // Arrange
            var hallDto = new HallDTO 
            { 
                HallName = "S?nh m?i", 
                MaxTableCount = 40,
                HallTypeId = 1,
                Note = "Ghi chú"
            };

            _mockRepository.Setup(r => r.Create(It.IsAny<Hall>()));

            // Act
            _service.Create(hallDto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<Hall>(h => 
                h.HallName == "S?nh m?i" && 
                h.MaxTableCount == 40 &&
                h.HallTypeId == 1)), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("Create map t?t c? properties ?úng")]
        public void Create_ShouldMapAllPropertiesCorrectly()
        {
            // Arrange
            var hallDto = new HallDTO 
            { 
                HallId = 0,
                HallName = "New Hall", 
                HallTypeId = 2,
                MaxTableCount = 35,
                Note = "New hall note"
            };

            Hall capturedHall = null;
            _mockRepository.Setup(r => r.Create(It.IsAny<Hall>()))
                .Callback<Hall>(h => capturedHall = h);

            // Act
            _service.Create(hallDto);

            // Assert
            Assert.IsNotNull(capturedHall);
            Assert.AreEqual("New Hall", capturedHall.HallName);
            Assert.AreEqual(2, capturedHall.HallTypeId);
            Assert.AreEqual(35, capturedHall.MaxTableCount);
            Assert.AreEqual("New hall note", capturedHall.Note);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("Create v?i MaxTableCount null")]
        public void Create_WithNullMaxTableCount_ShouldWork()
        {
            // Arrange
            var hallDto = new HallDTO 
            { 
                HallName = "Test Hall", 
                MaxTableCount = null
            };

            _mockRepository.Setup(r => r.Create(It.IsAny<Hall>()));

            // Act
            _service.Create(hallDto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<Hall>(h => 
                h.MaxTableCount == null)), Times.Once);
        }

        #endregion

        #region Update Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("Update g?i repository ?úng cách")]
        public void Update_ValidHall_CallsRepositoryUpdate()
        {
            // Arrange
            var hallDto = new HallDTO 
            { 
                HallId = 1,
                HallName = "Updated Hall Name", 
                MaxTableCount = 60
            };

            _mockRepository.Setup(r => r.Update(It.IsAny<Hall>()));

            // Act
            _service.Update(hallDto);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<Hall>(h => 
                h.HallId == 1 && 
                h.HallName == "Updated Hall Name" &&
                h.MaxTableCount == 60)), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("Update map t?t c? properties ?úng")]
        public void Update_ShouldMapAllPropertiesCorrectly()
        {
            // Arrange
            var hallDto = new HallDTO 
            { 
                HallId = 5,
                HallName = "Updated Hall", 
                HallTypeId = 3,
                MaxTableCount = 45,
                Note = "Updated note"
            };

            Hall capturedHall = null;
            _mockRepository.Setup(r => r.Update(It.IsAny<Hall>()))
                .Callback<Hall>(h => capturedHall = h);

            // Act
            _service.Update(hallDto);

            // Assert
            Assert.IsNotNull(capturedHall);
            Assert.AreEqual(5, capturedHall.HallId);
            Assert.AreEqual("Updated Hall", capturedHall.HallName);
            Assert.AreEqual(3, capturedHall.HallTypeId);
            Assert.AreEqual(45, capturedHall.MaxTableCount);
            Assert.AreEqual("Updated note", capturedHall.Note);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
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
        [TestCategory("HallService")]
        [Description("Delete nhi?u l?n")]
        public void Delete_MultipleCalls_CallsRepositoryEachTime()
        {
            // Arrange
            _mockRepository.Setup(r => r.Delete(It.IsAny<int>()));

            // Act
            _service.Delete(1);
            _service.Delete(2);
            _service.Delete(3);

            // Assert
            _mockRepository.Verify(r => r.Delete(It.IsAny<int>()), Times.Exactly(3));
        }

        #endregion

        #region Business Logic Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallService")]
        [Description("Ki?m tra MaxTableCount ???c map ?úng giá tr?")]
        public void GetAll_MaxTableCountValues_MappedCorrectly()
        {
            // Arrange
            var halls = new List<Hall>
            {
                new Hall { HallId = 1, HallName = "Small", MaxTableCount = 10 },
                new Hall { HallId = 2, HallName = "Medium", MaxTableCount = 30 },
                new Hall { HallId = 3, HallName = "Large", MaxTableCount = 100 }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(halls);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(10, result[0].MaxTableCount);
            Assert.AreEqual(30, result[1].MaxTableCount);
            Assert.AreEqual(100, result[2].MaxTableCount);
        }

        #endregion
    }
}
