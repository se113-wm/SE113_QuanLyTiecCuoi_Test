using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation.ViewModel;

namespace QuanLyTiecCuoi.Tests.UnitTests.ViewModels
{
    /// <summary>
    /// Unit Tests for ReportViewModel
    /// Covers BR106-BR115 - Revenue Report Management
    /// </summary>
    [TestClass]
    public class ReportViewModelTests
    {
        private Mock<IRevenueReportDetailService> _mockRevenueReportDetailService;

        [TestInitialize]
        public void Setup()
        {
            _mockRevenueReportDetailService = new Mock<IRevenueReportDetailService>();

            // Setup default mock returns
            _mockRevenueReportDetailService
                .Setup(s => s.GetByMonthYear(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(CreateSampleReportDetails());
        }

        #region BR106 - Display Report Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR106")]
        [Description("TC_BR106_001: Verify ReportViewModel initializes correctly")]
        public void TC_BR106_001_Constructor_InitializesCorrectly()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.ReportList);
            Assert.IsNotNull(viewModel.Months);
            Assert.IsNotNull(viewModel.Years);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR106")]
        [Description("TC_BR106_002: Verify months collection contains 12 months")]
        public void TC_BR106_002_Months_Contains12Months()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.AreEqual(12, viewModel.Months.Count);
            Assert.AreEqual(1, viewModel.Months.First());
            Assert.AreEqual(12, viewModel.Months.Last());
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR106")]
        [Description("TC_BR106_003: Verify years collection contains appropriate range")]
        public void TC_BR106_003_Years_ContainsAppropriateRange()
        {
            // Act
            var viewModel = CreateViewModel();
            var currentYear = DateTime.Now.Year;

            // Assert
            Assert.IsTrue(viewModel.Years.Count > 0);
            Assert.IsTrue(viewModel.Years.Contains(currentYear));
            Assert.IsTrue(viewModel.Years.Min() <= currentYear - 5);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR106")]
        [Description("TC_BR106_004: Verify default month is current month")]
        public void TC_BR106_004_SelectedMonth_DefaultsToCurrentMonth()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.AreEqual(DateTime.Now.Month, viewModel.SelectedMonth);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR106")]
        [Description("TC_BR106_005: Verify default year is current year")]
        public void TC_BR106_005_SelectedYear_DefaultsToCurrentYear()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.AreEqual(DateTime.Now.Year, viewModel.SelectedYear);
        }

        #endregion

        #region BR107 - Load Report Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR107")]
        [Description("TC_BR107_001: Verify LoadReportCommand is initialized")]
        public void TC_BR107_001_LoadReportCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.LoadReportCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR107")]
        [Description("TC_BR107_002: Verify report loads data on initialization")]
        public void TC_BR107_002_Constructor_LoadsReportData()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsTrue(viewModel.ReportList.Count > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR107")]
        [Description("TC_BR107_003: Verify report calculates total revenue")]
        public void TC_BR107_003_LoadReport_CalculatesTotalRevenue()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsTrue(viewModel.TotalRevenue > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR107")]
        [Description("TC_BR107_004: Verify report calculates ratios correctly")]
        public void TC_BR107_004_LoadReport_CalculatesRatiosCorrectly()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            decimal totalRatio = 0;
            foreach (var item in viewModel.ReportList)
            {
                Assert.IsNotNull(item.Ratio);
                Assert.IsTrue(item.Ratio >= 0 && item.Ratio <= 100);
                totalRatio += item.Ratio ?? 0;
            }
            // Total ratios should be approximately 100%
            Assert.IsTrue(Math.Abs(totalRatio - 100) < 0.1m);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR107")]
        [Description("TC_BR107_005: Verify report assigns row numbers")]
        public void TC_BR107_005_LoadReport_AssignsRowNumbers()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            int expectedRow = 1;
            foreach (var item in viewModel.ReportList)
            {
                Assert.AreEqual(expectedRow, item.RowNumber);
                expectedRow++;
            }
        }

        #endregion

        #region BR108 - Month/Year Selection Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR108")]
        [Description("TC_BR108_001: Verify SelectedMonth can be changed")]
        public void TC_BR108_001_SelectedMonth_CanBeChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var newMonth = 6;

            // Act
            viewModel.SelectedMonth = newMonth;

            // Assert
            Assert.AreEqual(newMonth, viewModel.SelectedMonth);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR108")]
        [Description("TC_BR108_002: Verify SelectedYear can be changed")]
        public void TC_BR108_002_SelectedYear_CanBeChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var newYear = DateTime.Now.Year - 1;

            // Act
            viewModel.SelectedYear = newYear;

            // Assert
            Assert.AreEqual(newYear, viewModel.SelectedYear);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR108")]
        [Description("TC_BR108_003: Verify SelectedMonth raises PropertyChanged")]
        public void TC_BR108_003_SelectedMonth_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SelectedMonth))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedMonth = 6;

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR108")]
        [Description("TC_BR108_004: Verify SelectedYear raises PropertyChanged")]
        public void TC_BR108_004_SelectedYear_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SelectedYear))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedYear = DateTime.Now.Year - 1;

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        #endregion

        #region BR109 - Report Data Display Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR109")]
        [Description("TC_BR109_001: Verify report list contains date information")]
        public void TC_BR109_001_ReportList_ContainsDateInfo()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var item in viewModel.ReportList)
            {
                Assert.IsTrue(item.Day > 0 && item.Day <= 31);
                Assert.IsTrue(item.Month > 0 && item.Month <= 12);
                Assert.IsTrue(item.Year > 0);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR109")]
        [Description("TC_BR109_002: Verify report list contains wedding count")]
        public void TC_BR109_002_ReportList_ContainsWeddingCount()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var item in viewModel.ReportList)
            {
                Assert.IsNotNull(item.WeddingCount);
                Assert.IsTrue(item.WeddingCount >= 0);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR109")]
        [Description("TC_BR109_003: Verify report list contains revenue")]
        public void TC_BR109_003_ReportList_ContainsRevenue()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var item in viewModel.ReportList)
            {
                Assert.IsNotNull(item.Revenue);
                Assert.IsTrue(item.Revenue >= 0);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR109")]
        [Description("TC_BR109_004: Verify report filters out zero revenue entries")]
        public void TC_BR109_004_ReportList_FiltersZeroRevenue()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert - All items should have positive revenue and wedding count
            foreach (var item in viewModel.ReportList)
            {
                Assert.IsTrue(item.WeddingCount > 0);
                Assert.IsTrue(item.Revenue > 0);
            }
        }

        #endregion

        #region BR110 - Export PDF Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR110")]
        [Description("TC_BR110_001: Verify ExportPdfCommand is initialized")]
        public void TC_BR110_001_ExportPdfCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ExportPdfCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR110")]
        [Description("TC_BR110_002: Verify ExportPdfCommand can always execute")]
        public void TC_BR110_002_ExportPdfCommand_CanAlwaysExecute()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            bool canExecute = viewModel.ExportPdfCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        #endregion

        #region BR111 - Export Excel Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR111")]
        [Description("TC_BR111_001: Verify ExportExcelCommand is initialized")]
        public void TC_BR111_001_ExportExcelCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ExportExcelCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR111")]
        [Description("TC_BR111_002: Verify ExportExcelCommand can always execute")]
        public void TC_BR111_002_ExportExcelCommand_CanAlwaysExecute()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            bool canExecute = viewModel.ExportExcelCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        #endregion

        #region BR112 - Show Chart Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR112")]
        [Description("TC_BR112_001: Verify ShowChartCommand is initialized")]
        public void TC_BR112_001_ShowChartCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ShowChartCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR112")]
        [Description("TC_BR112_002: Verify ShowChartCommand can always execute")]
        public void TC_BR112_002_ShowChartCommand_CanAlwaysExecute()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            bool canExecute = viewModel.ShowChartCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        #endregion

        #region BR113 - Total Revenue Calculation Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR113")]
        [Description("TC_BR113_001: Verify total revenue is calculated correctly")]
        public void TC_BR113_001_TotalRevenue_CalculatedCorrectly()
        {
            // Act
            var viewModel = CreateViewModel();
            var expectedTotal = viewModel.ReportList.Sum(x => x.Revenue ?? 0);

            // Assert
            Assert.AreEqual(expectedTotal, viewModel.TotalRevenue);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR113")]
        [Description("TC_BR113_002: Verify TotalRevenue raises PropertyChanged")]
        public void TC_BR113_002_TotalRevenue_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.TotalRevenue))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.TotalRevenue = 1000000;

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        #endregion

        #region BR114 - Report with No Data Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR114")]
        [Description("TC_BR114_001: Verify report handles no data gracefully")]
        public void TC_BR114_001_LoadReport_HandlesNoDataGracefully()
        {
            // Arrange
            _mockRevenueReportDetailService
                .Setup(s => s.GetByMonthYear(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<RevenueReportDetailDTO>());

            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.AreEqual(0, viewModel.ReportList.Count);
            Assert.AreEqual(0, viewModel.TotalRevenue);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR114")]
        [Description("TC_BR114_002: Verify report filters entries with zero wedding count")]
        public void TC_BR114_002_LoadReport_FiltersZeroWeddingCount()
        {
            // Arrange
            var testData = new List<RevenueReportDetailDTO>
            {
                new RevenueReportDetailDTO { Day = 1, Month = 6, Year = 2024, WeddingCount = 0, Revenue = 0 },
                new RevenueReportDetailDTO { Day = 2, Month = 6, Year = 2024, WeddingCount = 2, Revenue = 5000000 }
            };

            _mockRevenueReportDetailService
                .Setup(s => s.GetByMonthYear(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(testData);

            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.AreEqual(1, viewModel.ReportList.Count);
            Assert.IsTrue(viewModel.ReportList.All(x => x.WeddingCount > 0));
        }

        #endregion

        #region BR115 - Report Display Format Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR115")]
        [Description("TC_BR115_001: Verify DisplayDate property formats correctly")]
        public void TC_BR115_001_DisplayDate_FormatsCorrectly()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var item in viewModel.ReportList)
            {
                string expected = $"{item.Day:D2}/{item.Month:D2}/{item.Year}";
                Assert.AreEqual(expected, item.DisplayDate);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ReportViewModel")]
        [TestCategory("BR115")]
        [Description("TC_BR115_002: Verify report data is sorted by date")]
        public void TC_BR115_002_ReportList_IsSortedByDate()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            for (int i = 0; i < viewModel.ReportList.Count - 1; i++)
            {
                var current = viewModel.ReportList[i];
                var next = viewModel.ReportList[i + 1];
                Assert.IsTrue(current.Day <= next.Day);
            }
        }

        #endregion

        #region Helper Methods

        private ReportViewModel CreateViewModel()
        {
            return new ReportViewModel(_mockRevenueReportDetailService.Object);
        }

        private List<RevenueReportDetailDTO> CreateSampleReportDetails()
        {
            var currentDate = DateTime.Now;
            return new List<RevenueReportDetailDTO>
            {
                new RevenueReportDetailDTO
                {
                    Day = 1,
                    Month = currentDate.Month,
                    Year = currentDate.Year,
                    WeddingCount = 3,
                    Revenue = 15000000
                },
                new RevenueReportDetailDTO
                {
                    Day = 5,
                    Month = currentDate.Month,
                    Year = currentDate.Year,
                    WeddingCount = 2,
                    Revenue = 10000000
                },
                new RevenueReportDetailDTO
                {
                    Day = 10,
                    Month = currentDate.Month,
                    Year = currentDate.Year,
                    WeddingCount = 4,
                    Revenue = 20000000
                },
                new RevenueReportDetailDTO
                {
                    Day = 15,
                    Month = currentDate.Month,
                    Year = currentDate.Year,
                    WeddingCount = 1,
                    Revenue = 5000000
                },
                new RevenueReportDetailDTO
                {
                    Day = 20,
                    Month = currentDate.Month,
                    Year = currentDate.Year,
                    WeddingCount = 5,
                    Revenue = 25000000
                }
            };
        }

        #endregion
    }
}
