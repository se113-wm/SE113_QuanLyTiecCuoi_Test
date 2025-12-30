using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.UnitTests.Services
{
    /// <summary>
    /// Unit Tests for ShiftService
    /// Covers BR51-BR59 - Shift Management
    /// </summary>
    [TestClass]
    public class ShiftServiceTests
    {
        private Mock<IShiftRepository> _mockRepository;
        private ShiftService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IShiftRepository>();
            _service = new ShiftService(_mockRepository.Object);
        }

        #region BR51 - Get All Shifts Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR51")]
        [Description("TC_BR51_001: Verify GetAll returns all shifts")]
        public void TC_BR51_001_GetAll_ReturnsAllShifts()
        {
            // Arrange
            var shifts = CreateSampleShifts();
            _mockRepository.Setup(r => r.GetAll()).Returns(shifts);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(s => s.ShiftName == "Morning Shift"));
            Assert.IsTrue(result.Any(s => s.ShiftName == "Afternoon Shift"));
            Assert.IsTrue(result.Any(s => s.ShiftName == "Evening Shift"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR51")]
        [Description("TC_BR51_002: Verify GetAll returns DTOs with correct time mapping")]
        public void TC_BR51_002_GetAll_ReturnsDTOsWithCorrectTimeMapping()
        {
            // Arrange
            var shifts = CreateSampleShifts();
            _mockRepository.Setup(r => r.GetAll()).Returns(shifts);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            var morningShift = result.First(s => s.ShiftName == "Morning Shift");
            Assert.AreEqual(new TimeSpan(7, 30, 0), morningShift.StartTime);
            Assert.AreEqual(new TimeSpan(12, 0, 0), morningShift.EndTime);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR51")]
        [Description("TC_BR51_003: Verify GetAll returns empty list when no shifts exist")]
        public void TC_BR51_003_GetAll_ReturnsEmptyList_WhenNoShifts()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll()).Returns(new List<Shift>());

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR51")]
        [Description("TC_BR51_004: Verify GetAll preserves all shift properties")]
        public void TC_BR51_004_GetAll_PreservesAllProperties()
        {
            // Arrange
            var shifts = CreateSampleShifts();
            _mockRepository.Setup(r => r.GetAll()).Returns(shifts);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            foreach (var shift in result)
            {
                Assert.IsNotNull(shift.ShiftName);
                Assert.IsNotNull(shift.StartTime);
                Assert.IsNotNull(shift.EndTime);
                Assert.IsTrue(shift.ShiftId > 0);
            }
        }

        #endregion

        #region BR52 - Get Shift By ID Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR52")]
        [Description("TC_BR52_001: Verify GetById returns correct shift")]
        public void TC_BR52_001_GetById_ReturnsCorrectShift()
        {
            // Arrange
            var shift = new Shift
            {
                ShiftId = 1,
                ShiftName = "Morning Shift",
                StartTime = new TimeSpan(7, 30, 0),
                EndTime = new TimeSpan(12, 0, 0)
            };
            _mockRepository.Setup(r => r.GetById(1)).Returns(shift);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ShiftId);
            Assert.AreEqual("Morning Shift", result.ShiftName);
            Assert.AreEqual(new TimeSpan(7, 30, 0), result.StartTime);
            Assert.AreEqual(new TimeSpan(12, 0, 0), result.EndTime);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR52")]
        [Description("TC_BR52_002: Verify GetById returns null for non-existent ID")]
        public void TC_BR52_002_GetById_ReturnsNull_ForNonExistentId()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(999)).Returns((Shift)null);

            // Act
            var result = _service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR52")]
        [Description("TC_BR52_003: Verify GetById maps time properties correctly")]
        public void TC_BR52_003_GetById_MapsTimePropertiesCorrectly()
        {
            // Arrange
            var shift = new Shift
            {
                ShiftId = 2,
                ShiftName = "Evening Shift",
                StartTime = new TimeSpan(18, 0, 0),
                EndTime = new TimeSpan(22, 0, 0)
            };
            _mockRepository.Setup(r => r.GetById(2)).Returns(shift);

            // Act
            var result = _service.GetById(2);

            // Assert
            Assert.AreEqual(shift.StartTime, result.StartTime);
            Assert.AreEqual(shift.EndTime, result.EndTime);
        }

        #endregion

        #region BR53 - Create Shift Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR53")]
        [Description("TC_BR53_001: Verify Create calls repository Create with correct data")]
        public void TC_BR53_001_Create_CallsRepositoryCreate()
        {
            // Arrange
            var dto = new ShiftDTO
            {
                ShiftName = "New Shift",
                StartTime = new TimeSpan(13, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            };

            // Act
            _service.Create(dto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<Shift>(s =>
                s.ShiftName == "New Shift" &&
                s.StartTime == new TimeSpan(13, 0, 0) &&
                s.EndTime == new TimeSpan(17, 0, 0)
            )), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR53")]
        [Description("TC_BR53_002: Verify Create maps DTO to entity correctly")]
        public void TC_BR53_002_Create_MapsDTOToEntityCorrectly()
        {
            // Arrange
            var dto = new ShiftDTO
            {
                ShiftName = "Night Shift",
                StartTime = new TimeSpan(22, 0, 0),
                EndTime = new TimeSpan(23, 59, 0)
            };

            Shift capturedEntity = null;
            _mockRepository.Setup(r => r.Create(It.IsAny<Shift>()))
                .Callback<Shift>(s => capturedEntity = s);

            // Act
            _service.Create(dto);

            // Assert
            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(dto.ShiftName, capturedEntity.ShiftName);
            Assert.AreEqual(dto.StartTime, capturedEntity.StartTime);
            Assert.AreEqual(dto.EndTime, capturedEntity.EndTime);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR53")]
        [Description("TC_BR53_003: Verify Create preserves all time components")]
        public void TC_BR53_003_Create_PreservesAllTimeComponents()
        {
            // Arrange
            var dto = new ShiftDTO
            {
                ShiftName = "Test Shift",
                StartTime = new TimeSpan(15, 30, 45),
                EndTime = new TimeSpan(19, 45, 30)
            };

            Shift capturedEntity = null;
            _mockRepository.Setup(r => r.Create(It.IsAny<Shift>()))
                .Callback<Shift>(s => capturedEntity = s);

            // Act
            _service.Create(dto);

            // Assert
            Assert.AreEqual(15, capturedEntity.StartTime.Value.Hours);
            Assert.AreEqual(30, capturedEntity.StartTime.Value.Minutes);
            Assert.AreEqual(19, capturedEntity.EndTime.Value.Hours);
            Assert.AreEqual(45, capturedEntity.EndTime.Value.Minutes);
        }

        #endregion

        #region BR54 - Update Shift Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR54")]
        [Description("TC_BR54_001: Verify Update calls repository Update")]
        public void TC_BR54_001_Update_CallsRepositoryUpdate()
        {
            // Arrange
            var dto = new ShiftDTO
            {
                ShiftId = 1,
                ShiftName = "Updated Shift",
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(12, 30, 0)
            };

            // Act
            _service.Update(dto);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<Shift>(s =>
                s.ShiftId == 1 &&
                s.ShiftName == "Updated Shift" &&
                s.StartTime == new TimeSpan(8, 0, 0) &&
                s.EndTime == new TimeSpan(12, 30, 0)
            )), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR54")]
        [Description("TC_BR54_002: Verify Update maps all properties")]
        public void TC_BR54_002_Update_MapsAllProperties()
        {
            // Arrange
            var dto = new ShiftDTO
            {
                ShiftId = 2,
                ShiftName = "Modified Shift",
                StartTime = new TimeSpan(14, 0, 0),
                EndTime = new TimeSpan(18, 0, 0)
            };

            Shift capturedEntity = null;
            _mockRepository.Setup(r => r.Update(It.IsAny<Shift>()))
                .Callback<Shift>(s => capturedEntity = s);

            // Act
            _service.Update(dto);

            // Assert
            Assert.IsNotNull(capturedEntity);
            Assert.AreEqual(dto.ShiftId, capturedEntity.ShiftId);
            Assert.AreEqual(dto.ShiftName, capturedEntity.ShiftName);
            Assert.AreEqual(dto.StartTime, capturedEntity.StartTime);
            Assert.AreEqual(dto.EndTime, capturedEntity.EndTime);
        }

        #endregion

        #region BR55 - Delete Shift Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR55")]
        [Description("TC_BR55_001: Verify Delete calls repository Delete with correct ID")]
        public void TC_BR55_001_Delete_CallsRepositoryDelete()
        {
            // Act
            _service.Delete(1);

            // Assert
            _mockRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [TestCategory("BR55")]
        [Description("TC_BR55_002: Verify Delete with different IDs")]
        public void TC_BR55_002_Delete_WithDifferentIds()
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
        [TestCategory("ShiftService")]
        [Description("Verify service handles operations with mock repository")]
        public void Service_HandlesOperations_WithMockRepository()
        {
            // Arrange & Act
            var service = new ShiftService(_mockRepository.Object);

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [Description("Verify GetAll handles large dataset")]
        public void GetAll_HandlesLargeDataset()
        {
            // Arrange
            var largeList = new List<Shift>();
            for (int i = 1; i <= 50; i++)
            {
                largeList.Add(new Shift
                {
                    ShiftId = i,
                    ShiftName = $"Shift {i}",
                    StartTime = new TimeSpan(7 + (i % 15), 0, 0),
                    EndTime = new TimeSpan(11 + (i % 15), 0, 0)
                });
            }
            _mockRepository.Setup(r => r.GetAll()).Returns(largeList);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(50, result.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ShiftService")]
        [Description("Verify service maintains time ordering")]
        public void Service_MaintainsTimeOrdering()
        {
            // Arrange
            var shifts = CreateSampleShifts();
            _mockRepository.Setup(r => r.GetAll()).Returns(shifts);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            foreach (var shift in result)
            {
                if (shift.StartTime.HasValue && shift.EndTime.HasValue)
                {
                    Assert.IsTrue(shift.EndTime > shift.StartTime, 
                        $"Shift {shift.ShiftName} has invalid time range");
                }
            }
        }

        #endregion

        #region Helper Methods

        private List<Shift> CreateSampleShifts()
        {
            return new List<Shift>
            {
                new Shift
                {
                    ShiftId = 1,
                    ShiftName = "Morning Shift",
                    StartTime = new TimeSpan(7, 30, 0),
                    EndTime = new TimeSpan(12, 0, 0)
                },
                new Shift
                {
                    ShiftId = 2,
                    ShiftName = "Afternoon Shift",
                    StartTime = new TimeSpan(13, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                },
                new Shift
                {
                    ShiftId = 3,
                    ShiftName = "Evening Shift",
                    StartTime = new TimeSpan(18, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0)
                }
            };
        }

        #endregion
    }
}
