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
    /// Unit Tests for RevenueReportService and RevenueReportDetailService
    /// Covers BR106-BR115 - Report Management
    /// </summary>
    [TestClass]
    public class RevenueReportServiceTests
    {
        private Mock<IRevenueReportRepository> _mockReportRepository;
        private Mock<IRevenueReportDetailRepository> _mockDetailRepository;
        private RevenueReportService _reportService;
        private RevenueReportDetailService _detailService;

        [TestInitialize]
        public void Setup()
        {
            _mockReportRepository = new Mock<IRevenueReportRepository>();
            _mockDetailRepository = new Mock<IRevenueReportDetailRepository>();
            _reportService = new RevenueReportService(_mockReportRepository.Object);
            _detailService = new RevenueReportDetailService(_mockDetailRepository.Object);
        }

        #region BR106 - Get All Revenue Reports Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportService")]
        [TestCategory("BR106")]
        [Description("TC_BR106_001: Verify GetAll returns all revenue reports")]
        public void TC_BR106_001_GetAll_ReturnsAllReports()
        {
            // Arrange
            var reports = CreateSampleReports();
            _mockReportRepository.Setup(r => r.GetAll()).Returns(reports);

            // Act
            var result = _reportService.GetAll().ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(r => r.Month == 1 && r.Year == 2024));
            Assert.IsTrue(result.Any(r => r.Month == 2 && r.Year == 2024));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportService")]
        [TestCategory("BR106")]
        [Description("TC_BR106_002: Verify GetAll returns DTOs with revenue")]
        public void TC_BR106_002_GetAll_ReturnsDTOsWithRevenue()
        {
            // Arrange
            var reports = CreateSampleReports();
            _mockReportRepository.Setup(r => r.GetAll()).Returns(reports);

            // Act
            var result = _reportService.GetAll().ToList();

            // Assert
            var januaryReport = result.First(r => r.Month == 1);
            Assert.AreEqual(50000000, januaryReport.TotalRevenue);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportService")]
        [TestCategory("BR106")]
        [Description("TC_BR106_003: Verify GetAll includes report details")]
        public void TC_BR106_003_GetAll_IncludesReportDetails()
        {
            // Arrange
            var reports = CreateSampleReports();
            _mockReportRepository.Setup(r => r.GetAll()).Returns(reports);

            // Act
            var result = _reportService.GetAll().ToList();

            // Assert
            var reportWithDetails = result.First(r => r.Month == 1);
            Assert.IsNotNull(reportWithDetails.RevenueReportDetails);
            Assert.IsTrue(reportWithDetails.RevenueReportDetails.Count > 0);
        }

        #endregion

        #region BR107 - Get Revenue Report By Month/Year Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportService")]
        [TestCategory("BR107")]
        [Description("TC_BR107_001: Verify GetByMonthYear returns correct report")]
        public void TC_BR107_001_GetByMonthYear_ReturnsCorrectReport()
        {
            // Arrange
            var report = new RevenueReport
            {
                Month = 1,
                Year = 2024,
                TotalRevenue = 50000000,
                RevenueReportDetails = (ICollection<RevenueReportDetail>)CreateSampleDetails(1, 2024).ToList()
            };
            _mockReportRepository.Setup(r => r.GetByMonthYear(1, 2024)).Returns(report);

            // Act
            var result = _reportService.GetByMonthYear(1, 2024);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Month);
            Assert.AreEqual(2024, result.Year);
            Assert.AreEqual(50000000, result.TotalRevenue);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportService")]
        [TestCategory("BR107")]
        [Description("TC_BR107_002: Verify GetByMonthYear returns null for non-existent report")]
        public void TC_BR107_002_GetByMonthYear_ReturnsNull_ForNonExistent()
        {
            // Arrange
            _mockReportRepository.Setup(r => r.GetByMonthYear(13, 2024)).Returns((RevenueReport)null);

            // Act
            var result = _reportService.GetByMonthYear(13, 2024);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportService")]
        [TestCategory("BR107")]
        [Description("TC_BR107_003: Verify GetByMonthYear includes all details")]
        public void TC_BR107_003_GetByMonthYear_IncludesAllDetails()
        {
            // Arrange
            var report = new RevenueReport
            {
                Month = 6,
                Year = 2024,
                TotalRevenue = 75000000,
                RevenueReportDetails = (ICollection<RevenueReportDetail>)CreateSampleDetails(6, 2024).ToList()
            };
            _mockReportRepository.Setup(r => r.GetByMonthYear(6, 2024)).Returns(report);

            // Act
            var result = _reportService.GetByMonthYear(6, 2024);

            // Assert
            Assert.IsNotNull(result.RevenueReportDetails);
            Assert.AreEqual(5, result.RevenueReportDetails.Count);
        }

        #endregion

        #region BR108 - Get Revenue Report Details Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportDetailService")]
        [TestCategory("BR108")]
        [Description("TC_BR108_001: Verify GetByMonthYear returns all details for month")]
        public void TC_BR108_001_GetByMonthYear_ReturnsAllDetails()
        {
            // Arrange
            var details = CreateSampleDetails(3, 2024).ToList();
            _mockDetailRepository.Setup(r => r.GetByMonthYear(3, 2024)).Returns(details);

            // Act
            var result = _detailService.GetByMonthYear(3, 2024).ToList();

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.All(d => d.Month == 3 && d.Year == 2024));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportDetailService")]
        [TestCategory("BR108")]
        [Description("TC_BR108_002: Verify GetByDate returns correct detail")]
        public void TC_BR108_002_GetByDate_ReturnsCorrectDetail()
        {
            // Arrange
            var detail = new RevenueReportDetail
            {
                Day = 15,
                Month = 6,
                Year = 2024,
                WeddingCount = 3,
                Revenue = 15000000,
                Ratio = 20.5m
            };
            _mockDetailRepository.Setup(r => r.GetByDate(15, 6, 2024)).Returns(detail);

            // Act
            var result = _detailService.GetByDate(15, 6, 2024);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.Day);
            Assert.AreEqual(6, result.Month);
            Assert.AreEqual(2024, result.Year);
            Assert.AreEqual(3, result.WeddingCount);
            Assert.AreEqual(15000000, result.Revenue);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportDetailService")]
        [TestCategory("BR108")]
        [Description("TC_BR108_003: Verify GetByDate returns null for non-existent date")]
        public void TC_BR108_003_GetByDate_ReturnsNull_ForNonExistent()
        {
            // Arrange
            _mockDetailRepository.Setup(r => r.GetByDate(32, 1, 2024)).Returns((RevenueReportDetail)null);

            // Act
            var result = _detailService.GetByDate(32, 1, 2024);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region BR109 - Revenue Calculation Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportService")]
        [TestCategory("BR109")]
        [Description("TC_BR109_001: Verify report calculates total revenue correctly")]
        public void TC_BR109_001_Report_CalculatesTotalRevenue()
        {
            // Arrange
            var details = new List<RevenueReportDetail>
            {
                new RevenueReportDetail { Day = 1, Month = 1, Year = 2024, Revenue = 10000000 },
                new RevenueReportDetail { Day = 5, Month = 1, Year = 2024, Revenue = 15000000 },
                new RevenueReportDetail { Day = 10, Month = 1, Year = 2024, Revenue = 25000000 }
            };
            
            var report = new RevenueReport
            {
                Month = 1,
                Year = 2024,
                TotalRevenue = 50000000,
                RevenueReportDetails = details
            };
            _mockReportRepository.Setup(r => r.GetByMonthYear(1, 2024)).Returns(report);

            // Act
            var result = _reportService.GetByMonthYear(1, 2024);

            // Assert
            Assert.AreEqual(50000000, result.TotalRevenue);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportDetailService")]
        [TestCategory("BR109")]
        [Description("TC_BR109_002: Verify detail includes ratio")]
        public void TC_BR109_002_Detail_IncludesRatio()
        {
            // Arrange
            var detail = new RevenueReportDetail
            {
                Day = 1,
                Month = 1,
                Year = 2024,
                Revenue = 10000000,
                Ratio = 20m
            };
            _mockDetailRepository.Setup(r => r.GetByDate(1, 1, 2024)).Returns(detail);

            // Act
            var result = _detailService.GetByDate(1, 1, 2024);

            // Assert
            Assert.IsNotNull(result.Ratio);
            Assert.AreEqual(20m, result.Ratio);
        }

        #endregion

        #region Additional Service Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportService")]
        [Description("Verify report service handles operations")]
        public void ReportService_HandlesOperations()
        {
            // Arrange & Act
            var service = new RevenueReportService(_mockReportRepository.Object);

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportDetailService")]
        [Description("Verify detail service handles operations")]
        public void DetailService_HandlesOperations()
        {
            // Arrange & Act
            var service = new RevenueReportDetailService(_mockDetailRepository.Object);

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("RevenueReportDetailService")]
        [Description("Verify GetAll returns all details")]
        public void DetailService_GetAll_ReturnsAllDetails()
        {
            // Arrange
            var allDetails = new List<RevenueReportDetail>();
            for (int i = 1; i <= 30; i++)
            {
                allDetails.Add(new RevenueReportDetail
                {
                    Day = i,
                    Month = 6,
                    Year = 2024,
                    WeddingCount = i % 5,
                    Revenue = i * 1000000
                });
            }
            _mockDetailRepository.Setup(r => r.GetAll()).Returns(allDetails);

            // Act
            var result = _detailService.GetAll().ToList();

            // Assert
            Assert.AreEqual(30, result.Count);
        }

        #endregion

        #region Helper Methods

        private List<RevenueReport> CreateSampleReports()
        {
            return new List<RevenueReport>
            {
                new RevenueReport
                {
                    Month = 1,
                    Year = 2024,
                    TotalRevenue = 50000000,
                    RevenueReportDetails = (ICollection<RevenueReportDetail>)CreateSampleDetails(1, 2024).ToList()
                },
                new RevenueReport
                {
                    Month = 2,
                    Year = 2024,
                    TotalRevenue = 60000000,
                    RevenueReportDetails = (ICollection<RevenueReportDetail>)CreateSampleDetails(2, 2024).ToList()
                },
                new RevenueReport
                {
                    Month = 3,
                    Year = 2024,
                    TotalRevenue = 75000000,
                    RevenueReportDetails = (ICollection<RevenueReportDetail>)CreateSampleDetails(3, 2024).ToList()
                }
            };
        }

        private IEnumerable<RevenueReportDetail> CreateSampleDetails(int month, int year)
        {
            return new List<RevenueReportDetail>
            {
                new RevenueReportDetail
                {
                    Day = 1,
                    Month = month,
                    Year = year,
                    WeddingCount = 2,
                    Revenue = 10000000,
                    Ratio = 20m
                },
                new RevenueReportDetail
                {
                    Day = 5,
                    Month = month,
                    Year = year,
                    WeddingCount = 3,
                    Revenue = 15000000,
                    Ratio = 30m
                },
                new RevenueReportDetail
                {
                    Day = 10,
                    Month = month,
                    Year = year,
                    WeddingCount = 4,
                    Revenue = 20000000,
                    Ratio = 40m
                },
                new RevenueReportDetail
                {
                    Day = 15,
                    Month = month,
                    Year = year,
                    WeddingCount = 1,
                    Revenue = 5000000,
                    Ratio = 10m
                },
                new RevenueReportDetail
                {
                    Day = 20,
                    Month = month,
                    Year = year,
                    WeddingCount = 0,
                    Revenue = 0,
                    Ratio = 0m
                }
            };
        }

        #endregion
    }
}
