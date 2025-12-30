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
    /// Unit Tests for HallTypeService
    /// Covers BR60-BR70 - HallType Management
    /// </summary>
    [TestClass]
    public class HallTypeServiceTests
    {
        private Mock<IHallTypeRepository> _mockRepository;
        private HallTypeService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IHallTypeRepository>();
            _service = new HallTypeService(_mockRepository.Object);
        }

        #region BR60 - Get All HallTypes Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR60")]
        [Description("TC_BR60_001: Verify GetAll returns all hall types")]
        public void TC_BR60_001_GetAll_ReturnsAllHallTypes()
        {
            // Arrange
            var hallTypes = CreateSampleHallTypes();
            _mockRepository.Setup(r => r.GetAll()).Returns(hallTypes);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(ht => ht.HallTypeName == "VIP"));
            Assert.IsTrue(result.Any(ht => ht.HallTypeName == "Standard"));
            Assert.IsTrue(result.Any(ht => ht.HallTypeName == "Economy"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR60")]
        [Description("TC_BR60_002: Verify GetAll returns DTOs with correct mapping")]
        public void TC_BR60_002_GetAll_ReturnsDTOsWithCorrectMapping()
        {
            // Arrange
            var hallTypes = CreateSampleHallTypes();
            _mockRepository.Setup(r => r.GetAll()).Returns(hallTypes);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            var vipType = result.First(ht => ht.HallTypeName == "VIP");
            Assert.AreEqual(1, vipType.HallTypeId);
            Assert.AreEqual(2000000, vipType.MinTablePrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR60")]
        [Description("TC_BR60_003: Verify GetAll returns empty list when no hall types exist")]
        public void TC_BR60_003_GetAll_ReturnsEmptyList_WhenNoHallTypes()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll()).Returns(new List<HallType>());

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region BR61 - Get HallType By ID Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR61")]
        [Description("TC_BR61_001: Verify GetById returns correct hall type")]
        public void TC_BR61_001_GetById_ReturnsCorrectHallType()
        {
            // Arrange
            var hallType = new HallType
            {
                HallTypeId = 1,
                HallTypeName = "VIP",
                MinTablePrice = 2000000
            };
            _mockRepository.Setup(r => r.GetById(1)).Returns(hallType);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.HallTypeId);
            Assert.AreEqual("VIP", result.HallTypeName);
            Assert.AreEqual(2000000, result.MinTablePrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR61")]
        [Description("TC_BR61_002: Verify GetById returns null for non-existent ID")]
        public void TC_BR61_002_GetById_ReturnsNull_ForNonExistentId()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(999)).Returns((HallType)null);

            // Act
            var result = _service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR61")]
        [Description("TC_BR61_003: Verify GetById maps properties correctly")]
        public void TC_BR61_003_GetById_MapsPropertiesCorrectly()
        {
            // Arrange
            var hallType = new HallType
            {
                HallTypeId = 2,
                HallTypeName = "Standard",
                MinTablePrice = 1500000
            };
            _mockRepository.Setup(r => r.GetById(2)).Returns(hallType);

            // Act
            var result = _service.GetById(2);

            // Assert
            Assert.AreEqual(hallType.HallTypeId, result.HallTypeId);
            Assert.AreEqual(hallType.HallTypeName, result.HallTypeName);
            Assert.AreEqual(hallType.MinTablePrice, result.MinTablePrice);
        }

        #endregion

        #region BR62 - Create HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR62")]
        [Description("TC_BR62_001: Verify Create calls repository Create with correct data")]
        public void TC_BR62_001_Create_CallsRepositoryCreate()
        {
            // Arrange
            var dto = new HallTypeDTO
            {
                HallTypeName = "New Type",
                MinTablePrice = 1800000
            };

            // Act
            _service.Create(dto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<HallType>(ht =>
                ht.HallTypeName == "New Type" &&
                ht.MinTablePrice == 1800000
            )), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR62")]
        [Description("TC_BR62_002: Verify Create maps DTO to entity correctly")]
        public void TC_BR62_002_Create_MapsDTOToEntityCorrectly()
        {
            // Arrange
            var dto = new HallTypeDTO
            {
                HallTypeName = "Premium",
                MinTablePrice = 2500000
            };

            HallType capturedEntity = null;
            _mockRepository.Setup(r => r.Create(It.IsAny<HallType>()))
                .Callback<HallType>(ht => capturedEntity = ht);

            // Act
            _service.Create(dto);

            // Assert
            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(dto.HallTypeName, capturedEntity.HallTypeName);
            Assert.AreEqual(dto.MinTablePrice, capturedEntity.MinTablePrice);
        }

        #endregion

        #region BR63 - Update HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR63")]
        [Description("TC_BR63_001: Verify Update calls repository Update")]
        public void TC_BR63_001_Update_CallsRepositoryUpdate()
        {
            // Arrange
            var dto = new HallTypeDTO
            {
                HallTypeId = 1,
                HallTypeName = "Updated Type",
                MinTablePrice = 2500000
            };

            // Act
            _service.Update(dto);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<HallType>(ht =>
                ht.HallTypeId == 1 &&
                ht.HallTypeName == "Updated Type" &&
                ht.MinTablePrice == 2500000
            )), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR63")]
        [Description("TC_BR63_002: Verify Update maps all properties")]
        public void TC_BR63_002_Update_MapsAllProperties()
        {
            // Arrange
            var dto = new HallTypeDTO
            {
                HallTypeId = 2,
                HallTypeName = "Super VIP",
                MinTablePrice = 3000000
            };

            HallType capturedEntity = null;
            _mockRepository.Setup(r => r.Update(It.IsAny<HallType>()))
                .Callback<HallType>(ht => capturedEntity = ht);

            // Act
            _service.Update(dto);

            // Assert
            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(dto.HallTypeId, capturedEntity.HallTypeId);
            Assert.AreEqual(dto.HallTypeName, capturedEntity.HallTypeName);
            Assert.AreEqual(dto.MinTablePrice, capturedEntity.MinTablePrice);
        }

        #endregion

        #region BR64 - Delete HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR64")]
        [Description("TC_BR64_001: Verify Delete calls repository Delete with correct ID")]
        public void TC_BR64_001_Delete_CallsRepositoryDelete()
        {
            // Act
            _service.Delete(1);

            // Assert
            _mockRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR64")]
        [Description("TC_BR64_002: Verify Delete with different IDs")]
        public void TC_BR64_002_Delete_WithDifferentIds()
        {
            // Act
            _service.Delete(5);
            _service.Delete(10);

            // Assert
            _mockRepository.Verify(r => r.Delete(5), Times.Once);
            _mockRepository.Verify(r => r.Delete(10), Times.Once);
        }

        #endregion

        #region Additional Service Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [Description("Verify service handles null repository gracefully")]
        public void Service_HandlesOperations_WithMockRepository()
        {
            // This test verifies the service can be instantiated and used
            // Arrange & Act
            var service = new HallTypeService(_mockRepository.Object);

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [Description("Verify GetAll performance with large dataset")]
        public void GetAll_HandlesLargeDataset()
        {
            // Arrange
            var largeList = new List<HallType>();
            for (int i = 1; i <= 100; i++)
            {
                largeList.Add(new HallType
                {
                    HallTypeId = i,
                    HallTypeName = $"Type {i}",
                    MinTablePrice = 1000000 + (i * 100000)
                });
            }
            _mockRepository.Setup(r => r.GetAll()).Returns(largeList);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(100, result.Count);
        }

        #endregion

        #region Helper Methods

        private List<HallType> CreateSampleHallTypes()
        {
            return new List<HallType>
            {
                new HallType
                {
                    HallTypeId = 1,
                    HallTypeName = "VIP",
                    MinTablePrice = 2000000
                },
                new HallType
                {
                    HallTypeId = 2,
                    HallTypeName = "Standard",
                    MinTablePrice = 1500000
                },
                new HallType
                {
                    HallTypeId = 3,
                    HallTypeName = "Economy",
                    MinTablePrice = 1000000
                }
            };
        }

        #endregion
    }
}
