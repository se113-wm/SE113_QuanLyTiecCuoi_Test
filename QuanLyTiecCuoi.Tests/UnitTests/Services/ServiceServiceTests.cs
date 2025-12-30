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
    /// Unit Tests for ServiceService
    /// Covers BR89-BR105 - Service Management
    /// </summary>
    [TestClass]
    public class ServiceServiceTests
    {
        private Mock<IServiceRepository> _mockRepository;
        private ServiceService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IServiceRepository>();
            _service = new ServiceService(_mockRepository.Object);
        }

        #region BR89 - Get All Services Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR89")]
        [Description("TC_BR89_001: Verify GetAll returns all services")]
        public void TC_BR89_001_GetAll_ReturnsAllServices()
        {
            // Arrange
            var services = CreateSampleServices();
            _mockRepository.Setup(r => r.GetAll()).Returns(services);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Any(s => s.ServiceName == "Wedding Photography"));
            Assert.IsTrue(result.Any(s => s.ServiceName == "Flower Decoration"));
            Assert.IsTrue(result.Any(s => s.ServiceName == "Wedding MC"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR89")]
        [Description("TC_BR89_002: Verify GetAll returns DTOs with correct pricing")]
        public void TC_BR89_002_GetAll_ReturnsDTOsWithCorrectPricing()
        {
            // Arrange
            var services = CreateSampleServices();
            _mockRepository.Setup(r => r.GetAll()).Returns(services);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            var photography = result.First(s => s.ServiceName == "Wedding Photography");
            Assert.AreEqual(4500000, photography.UnitPrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR89")]
        [Description("TC_BR89_003: Verify GetAll returns empty list when no services exist")]
        public void TC_BR89_003_GetAll_ReturnsEmptyList_WhenNoServices()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll()).Returns(new List<Service>());

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR89")]
        [Description("TC_BR89_004: Verify GetAll preserves notes")]
        public void TC_BR89_004_GetAll_PreservesNotes()
        {
            // Arrange
            var services = CreateSampleServices();
            _mockRepository.Setup(r => r.GetAll()).Returns(services);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            var photography = result.First(s => s.ServiceName == "Wedding Photography");
            Assert.AreEqual("Professional photographer", photography.Note);
        }

        #endregion

        #region BR90 - Get Service By ID Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR90")]
        [Description("TC_BR90_001: Verify GetById returns correct service")]
        public void TC_BR90_001_GetById_ReturnsCorrectService()
        {
            // Arrange
            var service = new Service
            {
                ServiceId = 1,
                ServiceName = "Wedding Photography",
                UnitPrice = 4500000,
                Note = "Professional photographer"
            };
            _mockRepository.Setup(r => r.GetById(1)).Returns(service);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ServiceId);
            Assert.AreEqual("Wedding Photography", result.ServiceName);
            Assert.AreEqual(4500000, result.UnitPrice);
            Assert.AreEqual("Professional photographer", result.Note);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR90")]
        [Description("TC_BR90_002: Verify GetById returns null for non-existent ID")]
        public void TC_BR90_002_GetById_ReturnsNull_ForNonExistentId()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(999)).Returns((Service)null);

            // Act
            var result = _service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR90")]
        [Description("TC_BR90_003: Verify GetById maps properties correctly")]
        public void TC_BR90_003_GetById_MapsPropertiesCorrectly()
        {
            // Arrange
            var service = new Service
            {
                ServiceId = 2,
                ServiceName = "Flower Decoration",
                UnitPrice = 3000000,
                Note = "Fresh flowers"
            };
            _mockRepository.Setup(r => r.GetById(2)).Returns(service);

            // Act
            var result = _service.GetById(2);

            // Assert
            Assert.AreEqual(service.ServiceId, result.ServiceId);
            Assert.AreEqual(service.ServiceName, result.ServiceName);
            Assert.AreEqual(service.UnitPrice, result.UnitPrice);
            Assert.AreEqual(service.Note, result.Note);
        }

        #endregion

        #region BR91 - Create Service Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR91")]
        [Description("TC_BR91_001: Verify Create calls repository Create with correct data")]
        public void TC_BR91_001_Create_CallsRepositoryCreate()
        {
            // Arrange
            var dto = new ServiceDTO
            {
                ServiceName = "New Service",
                UnitPrice = 2000000,
                Note = "Test note"
            };

            // Act
            _service.Create(dto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<Service>(s =>
                s.ServiceName == "New Service" &&
                s.UnitPrice == 2000000 &&
                s.Note == "Test note"
            )), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR91")]
        [Description("TC_BR91_002: Verify Create maps DTO to entity correctly")]
        public void TC_BR91_002_Create_MapsDTOToEntityCorrectly()
        {
            // Arrange
            var dto = new ServiceDTO
            {
                ServiceName = "Premium Service",
                UnitPrice = 5000000,
                Note = "Premium package"
            };

            Service capturedEntity = null;
            _mockRepository.Setup(r => r.Create(It.IsAny<Service>()))
                .Callback<Service>(s => capturedEntity = s);

            // Act
            _service.Create(dto);

            // Assert
            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(dto.ServiceName, capturedEntity.ServiceName);
            Assert.AreEqual(dto.UnitPrice, capturedEntity.UnitPrice);
            Assert.AreEqual(dto.Note, capturedEntity.Note);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR91")]
        [Description("TC_BR91_003: Verify Create updates DTO with generated ID")]
        public void TC_BR91_003_Create_UpdatesDTOWithGeneratedId()
        {
            // Arrange
            var dto = new ServiceDTO
            {
                ServiceName = "Test Service",
                UnitPrice = 1500000
            };

            _mockRepository.Setup(r => r.Create(It.IsAny<Service>()))
                .Callback<Service>(s => s.ServiceId = 123);

            // Act
            _service.Create(dto);

            // Assert
            Assert.AreEqual(123, dto.ServiceId);
        }

        #endregion

        #region BR92 - Update Service Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR92")]
        [Description("TC_BR92_001: Verify Update calls repository Update")]
        public void TC_BR92_001_Update_CallsRepositoryUpdate()
        {
            // Arrange
            var dto = new ServiceDTO
            {
                ServiceId = 1,
                ServiceName = "Updated Service",
                UnitPrice = 3500000,
                Note = "Updated note"
            };

            // Act
            _service.Update(dto);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<Service>(s =>
                s.ServiceId == 1 &&
                s.ServiceName == "Updated Service" &&
                s.UnitPrice == 3500000 &&
                s.Note == "Updated note"
            )), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR92")]
        [Description("TC_BR92_002: Verify Update maps all properties")]
        public void TC_BR92_002_Update_MapsAllProperties()
        {
            // Arrange
            var dto = new ServiceDTO
            {
                ServiceId = 2,
                ServiceName = "Modified Service",
                UnitPrice = 4000000,
                Note = "Modified"
            };

            Service capturedEntity = null;
            _mockRepository.Setup(r => r.Update(It.IsAny<Service>()))
                .Callback<Service>(s => capturedEntity = s);

            // Act
            _service.Update(dto);

            // Assert
            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(dto.ServiceId, capturedEntity.ServiceId);
            Assert.AreEqual(dto.ServiceName, capturedEntity.ServiceName);
            Assert.AreEqual(dto.UnitPrice, capturedEntity.UnitPrice);
            Assert.AreEqual(dto.Note, capturedEntity.Note);
        }

        #endregion

        #region BR93 - Delete Service Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR93")]
        [Description("TC_BR93_001: Verify Delete calls repository Delete with correct ID")]
        public void TC_BR93_001_Delete_CallsRepositoryDelete()
        {
            // Act
            _service.Delete(1);

            // Assert
            _mockRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [TestCategory("BR93")]
        [Description("TC_BR93_002: Verify Delete with different IDs")]
        public void TC_BR93_002_Delete_WithDifferentIds()
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
        [TestCategory("ServiceService")]
        [Description("Verify service handles operations with mock repository")]
        public void Service_HandlesOperations_WithMockRepository()
        {
            // Arrange & Act
            var service = new ServiceService(_mockRepository.Object);

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [Description("Verify GetAll handles large dataset")]
        public void GetAll_HandlesLargeDataset()
        {
            // Arrange
            var largeList = new List<Service>();
            for (int i = 1; i <= 100; i++)
            {
                largeList.Add(new Service
                {
                    ServiceId = i,
                    ServiceName = $"Service {i}",
                    UnitPrice = 1000000 + (i * 50000),
                    Note = $"Note {i}"
                });
            }
            _mockRepository.Setup(r => r.GetAll()).Returns(largeList);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(100, result.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceService")]
        [Description("Verify service handles null notes")]
        public void Service_HandlesNullNotes()
        {
            // Arrange
            var services = new List<Service>
            {
                new Service
                {
                    ServiceId = 1,
                    ServiceName = "Service Without Note",
                    UnitPrice = 2000000,
                    Note = null
                }
            };
            _mockRepository.Setup(r => r.GetAll()).Returns(services);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsNull(result[0].Note);
        }

        #endregion

        #region Helper Methods

        private List<Service> CreateSampleServices()
        {
            return new List<Service>
            {
                new Service
                {
                    ServiceId = 1,
                    ServiceName = "Wedding Photography",
                    UnitPrice = 4500000,
                    Note = "Professional photographer"
                },
                new Service
                {
                    ServiceId = 2,
                    ServiceName = "Flower Decoration",
                    UnitPrice = 3000000,
                    Note = "Fresh flowers"
                },
                new Service
                {
                    ServiceId = 3,
                    ServiceName = "Wedding MC",
                    UnitPrice = 3500000,
                    Note = "Professional MC"
                },
                new Service
                {
                    ServiceId = 4,
                    ServiceName = "Sound System",
                    UnitPrice = 2000000,
                    Note = "High quality audio"
                },
                new Service
                {
                    ServiceId = 5,
                    ServiceName = "Lighting System",
                    UnitPrice = 2500000,
                    Note = "Professional lighting"
                }
            };
        }

        #endregion
    }
}
