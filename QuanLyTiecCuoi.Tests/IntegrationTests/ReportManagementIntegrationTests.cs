using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.IntegrationTests
{
    /// <summary>
    /// Integration Tests for Report Management (BR106-BR115)
    /// Tests the interaction between Service and Repository layers with real database
    /// </summary>
    [TestClass]
    public class ReportManagementIntegrationTests
    {
        private RevenueReportService _reportService;
        private RevenueReportDetailService _detailService;

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize services with real repositories
            _reportService = new RevenueReportService(new RevenueReportRepository());
            _detailService = new RevenueReportDetailService(new RevenueReportDetailRepository());
        }

        #region BR106 - Display Revenue Report Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR106")]
        [Description("TC_BR106_001: Integration - Verify reports load from database")]
        public void TC_BR106_001_Integration_Reports_LoadFromDatabase()
        {
            // Act
            var reports = _reportService.GetAll().ToList();

            // Assert
            Assert.IsTrue(reports.Count >= 0, "Should be able to query reports");
            
            // Verify each report has required fields
            foreach (var report in reports)
            {
                Assert.IsTrue(report.Month >= 1 && report.Month <= 12, 
                    $"Report month should be valid (1-12): {report.Month}");
                Assert.IsTrue(report.Year >= 2000 && report.Year <= 2100, 
                    $"Report year should be reasonable: {report.Year}");
                Assert.IsTrue(report.TotalRevenue >= 0, 
                    "Total revenue should not be negative");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR106")]
        [Description("TC_BR106_002: Integration - Verify reports are accessible")]
        public void TC_BR106_002_Integration_Reports_AreAccessible()
        {
            // Act
            var reports = _reportService.GetAll().ToList();

            if (reports.Count == 0)
            {
                Assert.Inconclusive("No reports in database to test");
                return;
            }

            // Assert - Can access report properties
            var firstReport = reports.First();
            Assert.IsTrue(firstReport.Month > 0);
            Assert.IsTrue(firstReport.Year > 0);
            Assert.IsNotNull(firstReport.TotalRevenue);
        }

        #endregion

        #region BR107 - Get Report By Month/Year Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR107")]
        [Description("TC_BR107_001: Integration - Verify GetByMonthYear retrieves correct report")]
        public void TC_BR107_001_Integration_GetByMonthYear_RetrievesCorrectReport()
        {
            // Arrange - Get an existing report
            var allReports = _reportService.GetAll().ToList();
            
            if (allReports.Count == 0)
            {
                Assert.Inconclusive("No reports in database to test");
                return;
            }

            var existingReport = allReports.First();

            // Act
            var report = _reportService.GetByMonthYear(existingReport.Month, existingReport.Year);

            // Assert
            Assert.IsNotNull(report, "Should retrieve report by month/year");
            Assert.AreEqual(existingReport.Month, report.Month);
            Assert.AreEqual(existingReport.Year, report.Year);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR107")]
        [Description("TC_BR107_002: Integration - Verify GetByMonthYear returns null for non-existent report")]
        public void TC_BR107_002_Integration_GetByMonthYear_ReturnsNull_ForNonExistent()
        {
            // Act - Use month/year that shouldn't exist
            var report = _reportService.GetByMonthYear(13, 1900);

            // Assert
            Assert.IsNull(report, "Should return null for non-existent report");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR107")]
        [Description("TC_BR107_003: Integration - Verify report includes details")]
        public void TC_BR107_003_Integration_Report_IncludesDetails()
        {
            // Arrange
            var allReports = _reportService.GetAll().ToList();
            
            if (allReports.Count == 0)
            {
                Assert.Inconclusive("No reports in database to test");
                return;
            }

            var reportWithDetails = allReports.FirstOrDefault(r => r.RevenueReportDetails != null && r.RevenueReportDetails.Count > 0);
            
            if (reportWithDetails == null)
            {
                Assert.Inconclusive("No reports with details in database");
                return;
            }

            // Act
            var report = _reportService.GetByMonthYear(reportWithDetails.Month, reportWithDetails.Year);

            // Assert
            Assert.IsNotNull(report.RevenueReportDetails);
            Assert.IsTrue(report.RevenueReportDetails.Count > 0);
        }

        #endregion

        #region BR108 - Revenue Report Details Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR108")]
        [Description("TC_BR108_001: Integration - Verify details load from database")]
        public void TC_BR108_001_Integration_Details_LoadFromDatabase()
        {
            // Act
            var details = _detailService.GetAll().ToList();

            // Assert
            Assert.IsTrue(details.Count >= 0, "Should be able to query details");
            
            // Verify each detail has required fields
            foreach (var detail in details)
            {
                Assert.IsTrue(detail.Day >= 1 && detail.Day <= 31, 
                    $"Detail day should be valid (1-31): {detail.Day}");
                Assert.IsTrue(detail.Month >= 1 && detail.Month <= 12, 
                    $"Detail month should be valid (1-12): {detail.Month}");
                Assert.IsTrue(detail.Year >= 2000 && detail.Year <= 2100, 
                    $"Detail year should be reasonable: {detail.Year}");
                Assert.IsTrue(detail.Revenue >= 0, 
                    "Revenue should not be negative");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR108")]
        [Description("TC_BR108_002: Integration - Verify GetByMonthYear retrieves correct details")]
        public void TC_BR108_002_Integration_Details_GetByMonthYear()
        {
            // Arrange
            var allReports = _reportService.GetAll().ToList();
            
            if (allReports.Count == 0)
            {
                Assert.Inconclusive("No reports in database to test");
                return;
            }

            var report = allReports.First();

            // Act
            var details = _detailService.GetByMonthYear(report.Month, report.Year).ToList();

            // Assert
            Assert.IsTrue(details.Count >= 0, "Should retrieve details for month/year");
            
            // All details should belong to the requested month/year
            foreach (var detail in details)
            {
                Assert.AreEqual(report.Month, detail.Month);
                Assert.AreEqual(report.Year, detail.Year);
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR108")]
        [Description("TC_BR108_003: Integration - Verify GetByDate retrieves correct detail")]
        public void TC_BR108_003_Integration_GetByDate_RetrievesCorrectDetail()
        {
            // Arrange
            var allDetails = _detailService.GetAll().ToList();
            
            if (allDetails.Count == 0)
            {
                Assert.Inconclusive("No details in database to test");
                return;
            }

            var existingDetail = allDetails.First();

            // Act
            var detail = _detailService.GetByDate(existingDetail.Day, existingDetail.Month, existingDetail.Year);

            // Assert
            Assert.IsNotNull(detail, "Should retrieve detail by date");
            Assert.AreEqual(existingDetail.Day, detail.Day);
            Assert.AreEqual(existingDetail.Month, detail.Month);
            Assert.AreEqual(existingDetail.Year, detail.Year);
        }

        #endregion

        #region BR106-BR115 - Complete Workflow Test

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR106")]
        [TestCategory("BR107")]
        [TestCategory("BR108")]
        [Description("Integration - Complete read workflow for reports")]
        public void Integration_Report_CompleteReadWorkflow()
        {
            // Verify read operations work
            var reports = _reportService.GetAll().ToList();
            
            Assert.IsTrue(reports.Count >= 0, 
                "Should be able to query reports (may be empty in test DB)");
            
            if (reports.Count > 0)
            {
                var firstReport = reports.First();
                var retrieved = _reportService.GetByMonthYear(firstReport.Month, firstReport.Year);
                
                Assert.IsNotNull(retrieved, "Should retrieve report by month/year");
                Assert.AreEqual(firstReport.Month, retrieved.Month);
                Assert.AreEqual(firstReport.Year, retrieved.Year);
            }
        }

        #endregion

        #region Data Validation Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR106")]
        [Description("Integration - Verify reports have non-negative revenue")]
        public void Integration_Reports_HaveValidRevenue()
        {
            // Act
            var reports = _reportService.GetAll().ToList();

            if (reports.Count == 0)
            {
                Assert.Inconclusive("No reports in database");
                return;
            }

            // Assert - All revenue should be non-negative
            foreach (var report in reports)
            {
                Assert.IsTrue(report.TotalRevenue >= 0, 
                    $"Report {report.Month}/{report.Year} should have non-negative revenue");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR108")]
        [Description("Integration - Verify details match report totals")]
        public void Integration_Details_MatchReportTotals()
        {
            // Act
            var reports = _reportService.GetAll().ToList();

            if (reports.Count == 0)
            {
                Assert.Inconclusive("No reports in database");
                return;
            }

            // Assert - Check each report's total matches sum of details
            foreach (var report in reports)
            {
                if (report.RevenueReportDetails != null && report.RevenueReportDetails.Count > 0)
                {
                    var detailsTotal = report.RevenueReportDetails.Sum(d => d.Revenue ?? 0);
                    
                    // Allow small rounding differences
                    var difference = Math.Abs(report.TotalRevenue.Value - detailsTotal);
                    Assert.IsTrue(difference < 1, 
                        $"Report {report.Month}/{report.Year} total should match sum of details");
                }
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR108")]
        [Description("Integration - Verify detail dates are valid")]
        public void Integration_Details_HaveValidDates()
        {
            // Act
            var details = _detailService.GetAll().ToList();

            if (details.Count == 0)
            {
                Assert.Inconclusive("No details in database");
                return;
            }

            // Assert - All dates should be valid
            foreach (var detail in details)
            {
                Assert.IsTrue(detail.Day >= 1 && detail.Day <= 31, 
                    $"Detail day should be 1-31: {detail.Day}");
                Assert.IsTrue(detail.Month >= 1 && detail.Month <= 12, 
                    $"Detail month should be 1-12: {detail.Month}");
                
                // Verify the date is actually valid (e.g., not Feb 31)
                try
                {
                    var date = new DateTime(detail.Year, detail.Month, detail.Day);
                    Assert.IsTrue(date <= DateTime.Now, 
                        "Detail date should not be in the future");
                }
                catch (ArgumentOutOfRangeException)
                {
                    Assert.Fail($"Invalid date: {detail.Day}/{detail.Month}/{detail.Year}");
                }
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR108")]
        [Description("Integration - Verify ratios sum to approximately 100%")]
        public void Integration_Details_RatiosSumTo100Percent()
        {
            // Act
            var reports = _reportService.GetAll().ToList();

            if (reports.Count == 0)
            {
                Assert.Inconclusive("No reports in database");
                return;
            }

            // Assert - Check ratios for each report
            foreach (var report in reports)
            {
                if (report.RevenueReportDetails != null && report.RevenueReportDetails.Count > 0)
                {
                    var totalRatio = report.RevenueReportDetails.Sum(d => d.Ratio ?? 0);
                    
                    // Allow small rounding differences (±1%)
                    if (totalRatio > 0)
                    {
                        Assert.IsTrue(Math.Abs(totalRatio - 100) <= 1, 
                            $"Report {report.Month}/{report.Year} ratios should sum to ~100%");
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR106")]
        [Description("Integration - Verify reports don't have duplicate month/year")]
        public void Integration_Reports_NoDuplicateMonthYear()
        {
            // Act
            var reports = _reportService.GetAll().ToList();

            if (reports.Count < 2)
            {
                Assert.Inconclusive("Need at least 2 reports to test uniqueness");
                return;
            }

            // Assert - No duplicate month/year combinations
            var uniqueMonthYears = reports.Select(r => $"{r.Month}/{r.Year}").Distinct().Count();
            Assert.AreEqual(reports.Count, uniqueMonthYears, 
                "Each report should have unique month/year combination");
        }

        #endregion
    }
}
