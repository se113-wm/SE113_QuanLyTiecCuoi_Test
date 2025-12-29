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
    [TestClass]
    public class BookingServiceTests
    {
        private Mock<IBookingRepository> _mockRepository;
        private BookingService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IBookingRepository>();
            _service = new BookingService(_mockRepository.Object);
        }

        #region GetAll Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("GetAll tr? v? danh sách bookings ?úng")]
        public void GetAll_WhenBookingsExist_ReturnsAllBookings()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1, 
                    GroomName = "Nguy?n V?n A", 
                    BrideName = "Tr?n Th? B",
                    Phone = "0901234567",
                    BookingDate = DateTime.Now,
                    WeddingDate = DateTime.Now.AddDays(30),
                    TableCount = 20,
                    Deposit = 5000000,
                    Shift = new Shift { ShiftId = 1, ShiftName = "Tr?a" },
                    Hall = new Hall 
                    { 
                        HallId = 1, 
                        HallName = "S?nh A",
                        HallType = new HallType { HallTypeId = 1, HallTypeName = "VIP" }
                    }
                },
                new Booking 
                { 
                    BookingId = 2, 
                    GroomName = "Lê V?n C", 
                    BrideName = "Ph?m Th? D",
                    Phone = "0907654321",
                    TableCount = 30
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Nguy?n V?n A", result[0].GroomName);
            Assert.AreEqual("Lê V?n C", result[1].GroomName);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("GetAll tr? v? danh sách r?ng khi không có bookings")]
        public void GetAll_WhenNoBookings_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll()).Returns(new List<Booking>());

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("GetAll map ?úng Shift navigation property")]
        public void GetAll_ShouldMapShiftCorrectly()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Test",
                    Shift = new Shift 
                    { 
                        ShiftId = 1, 
                        ShiftName = "Tr?a",
                        StartTime = new TimeSpan(11, 0, 0),
                        EndTime = new TimeSpan(14, 0, 0)
                    }
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNotNull(result.Shift);
            Assert.AreEqual(1, result.Shift.ShiftId);
            Assert.AreEqual("Tr?a", result.Shift.ShiftName);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("GetAll map ?úng Hall và HallType")]
        public void GetAll_ShouldMapHallAndHallTypeCorrectly()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Test",
                    Hall = new Hall 
                    { 
                        HallId = 1, 
                        HallName = "S?nh Diamond",
                        MaxTableCount = 50,
                        HallType = new HallType 
                        { 
                            HallTypeId = 1, 
                            HallTypeName = "VIP",
                            MinTablePrice = 1500000
                        }
                    }
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNotNull(result.Hall);
            Assert.AreEqual("S?nh Diamond", result.Hall.HallName);
            Assert.IsNotNull(result.Hall.HallType);
            Assert.AreEqual("VIP", result.Hall.HallType.HallTypeName);
        }

        #endregion

        #region GetById Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("GetById tr? v? booking ?úng khi tìm th?y")]
        public void GetById_WhenBookingExists_ReturnsBooking()
        {
            // Arrange
            var booking = new Booking 
            { 
                BookingId = 1, 
                GroomName = "Nguy?n V?n A", 
                BrideName = "Tr?n Th? B",
                Phone = "0901234567",
                TableCount = 25,
                Deposit = 10000000
            };

            _mockRepository.Setup(r => r.GetById(1)).Returns(booking);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.BookingId);
            Assert.AreEqual("Nguy?n V?n A", result.GroomName);
            Assert.AreEqual("Tr?n Th? B", result.BrideName);
            Assert.AreEqual(25, result.TableCount);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("GetById tr? v? null khi không tìm th?y")]
        public void GetById_WhenBookingNotExists_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Booking)null);

            // Act
            var result = _service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("GetById map t?t c? financial fields ?úng")]
        public void GetById_ShouldMapFinancialFieldsCorrectly()
        {
            // Arrange
            var booking = new Booking 
            { 
                BookingId = 1,
                GroomName = "Test",
                Deposit = 5000000,
                TablePrice = 1500000,
                TotalTableAmount = 30000000,
                TotalServiceAmount = 5000000,
                TotalInvoiceAmount = 35000000,
                RemainingAmount = 30000000,
                AdditionalCost = 500000,
                PenaltyAmount = 0
            };

            _mockRepository.Setup(r => r.GetById(1)).Returns(booking);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.AreEqual(5000000, result.Deposit);
            Assert.AreEqual(1500000, result.TablePrice);
            Assert.AreEqual(30000000, result.TotalTableAmount);
            Assert.AreEqual(5000000, result.TotalServiceAmount);
            Assert.AreEqual(35000000, result.TotalInvoiceAmount);
            Assert.AreEqual(30000000, result.RemainingAmount);
            Assert.AreEqual(500000, result.AdditionalCost);
            Assert.AreEqual(0, result.PenaltyAmount);
        }

        #endregion

        #region Create Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("Create g?i repository ?úng cách")]
        public void Create_ValidBooking_CallsRepositoryCreate()
        {
            // Arrange
            var bookingDto = new BookingDTO 
            { 
                GroomName = "Nguy?n V?n A", 
                BrideName = "Tr?n Th? B",
                Phone = "0901234567",
                WeddingDate = DateTime.Now.AddDays(30),
                ShiftId = 1,
                HallId = 1,
                TableCount = 20,
                Deposit = 5000000
            };

            _mockRepository.Setup(r => r.Create(It.IsAny<Booking>()));

            // Act
            _service.Create(bookingDto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<Booking>(b => 
                b.GroomName == "Nguy?n V?n A" && 
                b.BrideName == "Tr?n Th? B" &&
                b.TableCount == 20)), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("Create map t?t c? properties ?úng")]
        public void Create_ShouldMapAllPropertiesCorrectly()
        {
            // Arrange
            var weddingDate = DateTime.Now.AddDays(30);
            var bookingDate = DateTime.Now;
            var paymentDate = DateTime.Now.AddDays(29);

            var bookingDto = new BookingDTO 
            { 
                GroomName = "Chú r?", 
                BrideName = "Cô dâu",
                Phone = "0909090909",
                BookingDate = bookingDate,
                WeddingDate = weddingDate,
                ShiftId = 2,
                HallId = 3,
                Deposit = 10000000,
                TableCount = 30,
                ReserveTableCount = 5,
                PaymentDate = paymentDate,
                TablePrice = 1500000,
                TotalTableAmount = 45000000,
                TotalServiceAmount = 10000000,
                TotalInvoiceAmount = 55000000,
                RemainingAmount = 45000000,
                AdditionalCost = 1000000,
                PenaltyAmount = 0
            };

            Booking capturedBooking = null;
            _mockRepository.Setup(r => r.Create(It.IsAny<Booking>()))
                .Callback<Booking>(b => capturedBooking = b);

            // Act
            _service.Create(bookingDto);

            // Assert
            Assert.IsNotNull(capturedBooking);
            Assert.AreEqual("Chú r?", capturedBooking.GroomName);
            Assert.AreEqual("Cô dâu", capturedBooking.BrideName);
            Assert.AreEqual("0909090909", capturedBooking.Phone);
            Assert.AreEqual(30, capturedBooking.TableCount);
            Assert.AreEqual(5, capturedBooking.ReserveTableCount);
            Assert.AreEqual(10000000, capturedBooking.Deposit);
        }

        #endregion

        #region Update Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("Update g?i repository ?úng cách")]
        public void Update_ValidBooking_CallsRepositoryUpdate()
        {
            // Arrange
            var bookingDto = new BookingDTO 
            { 
                BookingId = 1,
                GroomName = "Updated Name", 
                TableCount = 25
            };

            _mockRepository.Setup(r => r.Update(It.IsAny<Booking>()));

            // Act
            _service.Update(bookingDto);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<Booking>(b => 
                b.BookingId == 1 && 
                b.GroomName == "Updated Name")), Times.Once);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
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

        #endregion

        #region Edge Cases

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("GetAll x? lý null navigation properties")]
        public void GetAll_WithNullNavigationProperties_HandlesGracefully()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Test",
                    Shift = null,
                    Hall = null
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNull(result.Shift);
            Assert.IsNull(result.Hall);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [Description("GetAll x? lý Hall có HallType null")]
        public void GetAll_WithHallButNullHallType_HandlesGracefully()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Test",
                    Hall = new Hall 
                    { 
                        HallId = 1, 
                        HallName = "Test Hall",
                        HallType = null
                    }
                }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNotNull(result.Hall);
            Assert.IsNull(result.Hall.HallType);
        }

        #endregion

        #region BR137 - Staff Hall Availability Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [TestCategory("BR137")]
        [Description("TC_BR137_001: Verify GetAll returns bookings for authorized query")]
        public void TC_BR137_001_GetAll_ReturnsBookingsForAuthorizedQuery()
        {
            // Arrange
            var bookings = CreateBookingsWithHallInfo();
            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            _mockRepository.Verify(r => r.GetAll(), Times.Once);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [TestCategory("BR137")]
        [Description("TC_BR137_002: Verify bookings include wedding date for calendar display")]
        public void TC_BR137_002_GetAll_ReturnsBookingsWithWeddingDate()
        {
            // Arrange
            var weddingDate = DateTime.Now.AddDays(30);
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Test",
                    WeddingDate = weddingDate
                }
            };
            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNotNull(result.WeddingDate);
            Assert.AreEqual(weddingDate, result.WeddingDate);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [TestCategory("BR137")]
        [Description("TC_BR137_003: Verify shift information is mapped for shift dropdown")]
        public void TC_BR137_003_GetAll_ReturnsShiftInfoForDropdown()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Test",
                    ShiftId = 1,
                    Shift = new Shift 
                    { 
                        ShiftId = 1, 
                        ShiftName = "Tr?a",
                        StartTime = new TimeSpan(11, 0, 0),
                        EndTime = new TimeSpan(14, 0, 0)
                    }
                }
            };
            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNotNull(result.Shift);
            Assert.AreEqual("Tr?a", result.Shift.ShiftName);
            Assert.AreEqual(new TimeSpan(11, 0, 0), result.Shift.StartTime);
            Assert.AreEqual(new TimeSpan(14, 0, 0), result.Shift.EndTime);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [TestCategory("BR137")]
        [Description("TC_BR137_004: Verify hall capacity info is mapped for filter")]
        public void TC_BR137_004_GetAll_ReturnsHallCapacityForFilter()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Test",
                    TableCount = 25,
                    Hall = new Hall 
                    { 
                        HallId = 1, 
                        HallName = "S?nh Diamond",
                        MaxTableCount = 50
                    }
                }
            };
            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNotNull(result.Hall);
            Assert.AreEqual(50, result.Hall.MaxTableCount);
            Assert.AreEqual(25, result.TableCount);
        }

        #endregion

        #region BR138 - Query Available Halls Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [TestCategory("BR138")]
        [Description("TC_BR138_001: Verify bookings with different payment status are returned")]
        public void TC_BR138_001_GetAll_ReturnsBookingsWithPaymentStatus()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Paid",
                    WeddingDate = DateTime.Now.AddDays(-5),
                    PaymentDate = DateTime.Now // Paid
                },
                new Booking 
                { 
                    BookingId = 2,
                    GroomName = "Unpaid",
                    WeddingDate = DateTime.Now.AddDays(30),
                    PaymentDate = null // Not paid
                }
            };
            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsNotNull(result[0].PaymentDate); // Paid
            Assert.IsNull(result[1].PaymentDate); // Not paid
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [TestCategory("BR138")]
        [Description("TC_BR138_002: Verify hall details are mapped correctly")]
        public void TC_BR138_002_GetAll_ReturnsHallDetailsWithPricing()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Test",
                    Hall = new Hall 
                    { 
                        HallId = 1, 
                        HallName = "S?nh VIP",
                        MaxTableCount = 50,
                        HallType = new HallType 
                        { 
                            HallTypeId = 1, 
                            HallTypeName = "VIP",
                            MinTablePrice = 2000000
                        }
                    }
                }
            };
            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.IsNotNull(result.Hall);
            Assert.AreEqual("S?nh VIP", result.Hall.HallName);
            Assert.AreEqual(50, result.Hall.MaxTableCount);
            Assert.IsNotNull(result.Hall.HallType);
            Assert.AreEqual("VIP", result.Hall.HallType.HallTypeName);
            Assert.AreEqual(2000000, result.Hall.HallType.MinTablePrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [TestCategory("BR138")]
        [Description("TC_BR138_003: Verify empty list when no bookings exist")]
        public void TC_BR138_003_GetAll_ReturnsEmptyWhenNoBookings()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll()).Returns(new List<Booking>());

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [TestCategory("BR138")]
        [Description("TC_BR138_004: Verify booking can be created (Create Booking button)")]
        public void TC_BR138_004_Create_SuccessfullyCreatesBooking()
        {
            // Arrange
            var bookingDto = new BookingDTO
            {
                GroomName = "Nguy?n V?n A",
                BrideName = "Tr?n Th? B",
                Phone = "0901234567",
                WeddingDate = DateTime.Now.AddDays(30),
                ShiftId = 1,
                HallId = 1,
                TableCount = 25,
                Deposit = 10000000
            };

            Booking createdBooking = null;
            _mockRepository.Setup(r => r.Create(It.IsAny<Booking>()))
                .Callback<Booking>(b => createdBooking = b);

            // Act
            _service.Create(bookingDto);

            // Assert
            Assert.IsNotNull(createdBooking);
            Assert.AreEqual("Nguy?n V?n A", createdBooking.GroomName);
            Assert.AreEqual("Tr?n Th? B", createdBooking.BrideName);
            Assert.AreEqual(25, createdBooking.TableCount);
            Assert.AreEqual(1, createdBooking.HallId);
            Assert.AreEqual(1, createdBooking.ShiftId);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("BookingService")]
        [TestCategory("BR138")]
        [Description("TC_BR138_005: Verify bookings include customer info for occupied halls")]
        public void TC_BR138_005_GetAll_ReturnsCustomerInfoForOccupiedHalls()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking 
                { 
                    BookingId = 1,
                    GroomName = "Nguy?n V?n A",
                    BrideName = "Tr?n Th? B",
                    Phone = "0901234567",
                    WeddingDate = DateTime.Now.AddDays(30),
                    Hall = new Hall 
                    { 
                        HallId = 1, 
                        HallName = "S?nh Diamond"
                    },
                    Shift = new Shift
                    {
                        ShiftId = 1,
                        ShiftName = "Tr?a"
                    }
                }
            };
            _mockRepository.Setup(r => r.GetAll()).Returns(bookings);

            // Act
            var result = _service.GetAll().First();

            // Assert
            Assert.AreEqual("Nguy?n V?n A", result.GroomName);
            Assert.AreEqual("Tr?n Th? B", result.BrideName);
            Assert.AreEqual("0901234567", result.Phone);
            Assert.AreEqual("S?nh Diamond", result.Hall.HallName);
            Assert.AreEqual("Tr?a", result.Shift.ShiftName);
        }

        #endregion

        #region Helper Methods

        private List<Booking> CreateBookingsWithHallInfo()
        {
            return new List<Booking>
            {
                new Booking
                {
                    BookingId = 1,
                    GroomName = "Nguy?n V?n A",
                    BrideName = "Tr?n Th? B",
                    Phone = "0901234567",
                    WeddingDate = DateTime.Now.AddDays(30),
                    TableCount = 20,
                    Hall = new Hall
                    {
                        HallId = 1,
                        HallName = "S?nh Diamond",
                        MaxTableCount = 50,
                        HallType = new HallType { HallTypeName = "VIP", MinTablePrice = 2000000 }
                    },
                    Shift = new Shift { ShiftId = 1, ShiftName = "Tr?a" }
                },
                new Booking
                {
                    BookingId = 2,
                    GroomName = "Lê V?n C",
                    BrideName = "Ph?m Th? D",
                    Phone = "0907654321",
                    WeddingDate = DateTime.Now.AddDays(45),
                    TableCount = 30,
                    Hall = new Hall
                    {
                        HallId = 2,
                        HallName = "S?nh Gold",
                        MaxTableCount = 40,
                        HallType = new HallType { HallTypeName = "Standard", MinTablePrice = 1500000 }
                    },
                    Shift = new Shift { ShiftId = 2, ShiftName = "T?i" }
                },
                new Booking
                {
                    BookingId = 3,
                    GroomName = "Hoàng V?n E",
                    BrideName = "V? Th? F",
                    Phone = "0909876543",
                    WeddingDate = DateTime.Now.AddDays(60),
                    TableCount = 25,
                    PaymentDate = DateTime.Now, // Paid
                    Hall = new Hall
                    {
                        HallId = 1,
                        HallName = "S?nh Diamond",
                        MaxTableCount = 50,
                        HallType = new HallType { HallTypeName = "VIP", MinTablePrice = 2000000 }
                    },
                    Shift = new Shift { ShiftId = 1, ShiftName = "Tr?a" }
                }
            };
        }

        #endregion
    }
}
