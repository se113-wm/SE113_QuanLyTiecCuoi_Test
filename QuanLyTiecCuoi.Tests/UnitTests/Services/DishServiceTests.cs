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
    /// <summary>
    /// Unit Tests for DishService
    /// Covers BR71-BR88 - Dish Management
    /// </summary>
    [TestClass]
    public class DishServiceTests
    {
        private Mock<IDishRepository> _mockRepository;
        private DishService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IDishRepository>();
            _service = new DishService(_mockRepository.Object);
        }

        #region BR71 - Get All Dishes Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR71")]
        [Description("TC_BR71_001: Verify GetAll returns all dishes")]
        public void TC_BR71_001_GetAll_ReturnsAllDishes()
        {
            // Arrange
            var dishes = CreateSampleDishes();
            _mockRepository.Setup(r => r.GetAll()).Returns(dishes);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Any(d => d.DishName == "Grilled Chicken"));
            Assert.IsTrue(result.Any(d => d.DishName == "Steamed Fish"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR71")]
        [Description("TC_BR71_002: Verify GetAll returns DTOs with correct mapping")]
        public void TC_BR71_002_GetAll_ReturnsDTOsWithCorrectMapping()
        {
            // Arrange
            var dishes = CreateSampleDishes();
            _mockRepository.Setup(r => r.GetAll()).Returns(dishes);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            var chicken = result.First(d => d.DishName == "Grilled Chicken");
            Assert.AreEqual(1, chicken.DishId);
            Assert.AreEqual(150000, chicken.UnitPrice);
            Assert.AreEqual("Main course", chicken.Note);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR71")]
        [Description("TC_BR71_003: Verify GetAll returns empty list when no dishes exist")]
        public void TC_BR71_003_GetAll_ReturnsEmptyList_WhenNoDishes()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll()).Returns(new List<Dish>());

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region BR72 - Get Dish By ID Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR72")]
        [Description("TC_BR72_001: Verify GetById returns correct dish")]
        public void TC_BR72_001_GetById_ReturnsCorrectDish()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 1,
                DishName = "Grilled Chicken",
                UnitPrice = 150000,
                Note = "Main course"
            };
            _mockRepository.Setup(r => r.GetById(1)).Returns(dish);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.DishId);
            Assert.AreEqual("Grilled Chicken", result.DishName);
            Assert.AreEqual(150000, result.UnitPrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR72")]
        [Description("TC_BR72_002: Verify GetById returns null for non-existent ID")]
        public void TC_BR72_002_GetById_ReturnsNull_ForNonExistentId()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(999)).Returns((Dish)null);

            // Act
            var result = _service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR72")]
        [Description("TC_BR72_003: Verify GetById maps all properties correctly")]
        public void TC_BR72_003_GetById_MapsPropertiesCorrectly()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = 3,
                DishName = "Vegetable Soup",
                UnitPrice = 80000,
                Note = "Appetizer"
            };
            _mockRepository.Setup(r => r.GetById(3)).Returns(dish);

            // Act
            var result = _service.GetById(3);

            // Assert
            Assert.AreEqual(dish.DishId, result.DishId);
            Assert.AreEqual(dish.DishName, result.DishName);
            Assert.AreEqual(dish.UnitPrice, result.UnitPrice);
            Assert.AreEqual(dish.Note, result.Note);
        }

        #endregion

        #region BR73 - Create Dish Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR73")]
        [Description("TC_BR73_001: Verify Create calls repository Create with correct data")]
        public void TC_BR73_001_Create_CallsRepositoryCreate()
        {
            // Arrange
            var dto = new DishDTO
            {
                DishName = "New Dish",
                UnitPrice = 200000,
                Note = "Special"
            };

            // Act
            _service.Create(dto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<Dish>(d =>
                d.DishName == "New Dish" &&
                d.UnitPrice == 200000 &&
                d.Note == "Special"
            )), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR73")]
        [Description("TC_BR73_002: Verify Create maps DTO to entity correctly")]
        public void TC_BR73_002_Create_MapsDTOToEntityCorrectly()
        {
            // Arrange
            var dto = new DishDTO
            {
                DishName = "Premium Steak",
                UnitPrice = 500000,
                Note = "High quality"
            };

            Dish capturedEntity = null;
            _mockRepository.Setup(r => r.Create(It.IsAny<Dish>()))
                .Callback<Dish>(d => capturedEntity = d);

            // Act
            _service.Create(dto);

            // Assert
            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(dto.DishName, capturedEntity.DishName);
            Assert.AreEqual(dto.UnitPrice, capturedEntity.UnitPrice);
            Assert.AreEqual(dto.Note, capturedEntity.Note);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR73")]
        [Description("TC_BR73_003: Verify Create handles dish without note")]
        public void TC_BR73_003_Create_HandlesNullNote()
        {
            // Arrange
            var dto = new DishDTO
            {
                DishName = "Simple Dish",
                UnitPrice = 100000,
                Note = null
            };

            // Act
            _service.Create(dto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<Dish>(d =>
                d.DishName == "Simple Dish" &&
                d.Note == null
            )), Times.Once);
        }

        #endregion

        #region BR74 - Update Dish Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR74")]
        [Description("TC_BR74_001: Verify Update calls repository Update")]
        public void TC_BR74_001_Update_CallsRepositoryUpdate()
        {
            // Arrange
            var dto = new DishDTO
            {
                DishId = 1,
                DishName = "Updated Dish",
                UnitPrice = 180000,
                Note = "Modified"
            };

            // Act
            _service.Update(dto);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<Dish>(d =>
                d.DishId == 1 &&
                d.DishName == "Updated Dish" &&
                d.UnitPrice == 180000 &&
                d.Note == "Modified"
            )), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR74")]
        [Description("TC_BR74_002: Verify Update maps all properties")]
        public void TC_BR74_002_Update_MapsAllProperties()
        {
            // Arrange
            var dto = new DishDTO
            {
                DishId = 2,
                DishName = "Super Fish",
                UnitPrice = 250000,
                Note = "Fresh daily"
            };

            Dish capturedEntity = null;
            _mockRepository.Setup(r => r.Update(It.IsAny<Dish>()))
                .Callback<Dish>(d => capturedEntity = d);

            // Act
            _service.Update(dto);

            // Assert
            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(dto.DishId, capturedEntity.DishId);
            Assert.AreEqual(dto.DishName, capturedEntity.DishName);
            Assert.AreEqual(dto.UnitPrice, capturedEntity.UnitPrice);
            Assert.AreEqual(dto.Note, capturedEntity.Note);
        }

        #endregion

        #region BR75 - Delete Dish Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR75")]
        [Description("TC_BR75_001: Verify Delete calls repository Delete with correct ID")]
        public void TC_BR75_001_Delete_CallsRepositoryDelete()
        {
            // Act
            _service.Delete(1);

            // Assert
            _mockRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [TestCategory("BR75")]
        [Description("TC_BR75_002: Verify Delete with different IDs")]
        public void TC_BR75_002_Delete_WithDifferentIds()
        {
            // Act
            _service.Delete(5);
            _service.Delete(10);
            _service.Delete(15);

            // Assert
            _mockRepository.Verify(r => r.Delete(5), Times.Once);
            _mockRepository.Verify(r => r.Delete(10), Times.Once);
            _mockRepository.Verify(r => r.Delete(15), Times.Once);
        }

        #endregion

        #region Additional Validation Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [Description("Verify service instantiation")]
        public void Service_CanBeInstantiated()
        {
            // Arrange & Act
            var service = new DishService(_mockRepository.Object);

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [Description("Verify GetAll handles large dataset")]
        public void GetAll_HandlesLargeDataset()
        {
            // Arrange
            var largelist = new List<Dish>();
            for (int i = 1; i <= 100; i++)
            {
                largelist.Add(new Dish
                {
                    DishId = i,
                    DishName = $"Dish {i}",
                    UnitPrice = 50000 + (i * 10000),
                    Note = $"Dish {i} description"
                });
            }
            _mockRepository.Setup(r => r.GetAll()).Returns(largelist);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(100, result.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("DishService")]
        [Description("Verify dishes have valid unit prices")]
        public void GetAll_DishesHaveValidUnitPrices()
        {
            // Arrange
            var dishes = CreateSampleDishes();
            _mockRepository.Setup(r => r.GetAll()).Returns(dishes);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            foreach (var dish in result)
            {
                Assert.IsNotNull(dish.UnitPrice);
                Assert.IsTrue(dish.UnitPrice > 0, $"Dish {dish.DishName} should have positive unit price");
            }
        }

        #endregion

        #region Helper Methods

        private List<Dish> CreateSampleDishes()
        {
            return new List<Dish>
            {
                new Dish
                {
                    DishId = 1,
                    DishName = "Grilled Chicken",
                    UnitPrice = 150000,
                    Note = "Main course"
                },
                new Dish
                {
                    DishId = 2,
                    DishName = "Steamed Fish",
                    UnitPrice = 200000,
                    Note = "Fresh fish daily"
                },
                new Dish
                {
                    DishId = 3,
                    DishName = "Vegetable Soup",
                    UnitPrice = 80000,
                    Note = "Appetizer"
                },
                new Dish
                {
                    DishId = 4,
                    DishName = "Beef Steak",
                    UnitPrice = 350000,
                    Note = "Premium quality"
                },
                new Dish
                {
                    DishId = 5,
                    DishName = "Fried Rice",
                    UnitPrice = 70000,
                    Note = "Side dish"
                }
            };
        }

        #endregion
    }
}
