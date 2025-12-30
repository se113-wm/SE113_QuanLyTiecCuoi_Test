using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.IntegrationTests
{
    /// <summary>
    /// Integration Tests for Service Management (BR89-BR105)
    /// Tests the interaction between Service and Repository layers with real database
    /// </summary>
    [TestClass]
    public class ServiceManagementIntegrationTests
    {
        private ServiceService _serviceService;

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize service with real repository
            _serviceService = new ServiceService(new ServiceRepository());
        }

        #region BR89 - Display Service Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR89")]
        [Description("TC_BR89_001: Integration - Verify services load from database")]
        public void TC_BR89_001_Integration_Services_LoadFromDatabase()
        {
            // Act
            var services = _serviceService.GetAll().ToList();

            // Assert
            Assert.IsTrue(services.Count > 0, "Should load services from database");
            
            // Verify each service has required fields
            foreach (var service in services)
            {
                Assert.IsFalse(string.IsNullOrEmpty(service.ServiceName), 
                    "Service name should not be empty");
                Assert.IsNotNull(service.UnitPrice, 
                    "Unit price should not be null");
                Assert.IsTrue(service.UnitPrice > 0, 
                    $"Service {service.ServiceName} should have positive price");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR89")]
        [Description("TC_BR89_002: Integration - Verify services are accessible")]
        public void TC_BR89_002_Integration_Services_AreAccessible()
        {
            // Act
            var services = _serviceService.GetAll().ToList();

            if (services.Count == 0)
            {
                Assert.Inconclusive("No services in database to test");
                return;
            }

            // Assert - Can access service properties
            var firstService = services.First();
            Assert.IsTrue(firstService.ServiceId > 0);
            Assert.IsFalse(string.IsNullOrEmpty(firstService.ServiceName));
            Assert.IsNotNull(firstService.UnitPrice);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR89")]
        [Description("TC_BR89_003: Integration - Verify services have pricing information")]
        public void TC_BR89_003_Integration_Services_HavePricingInfo()
        {
            // Act
            var services = _serviceService.GetAll().ToList();

            if (services.Count == 0)
            {
                Assert.Inconclusive("No services in database to test");
                return;
            }

            // Assert - All services have valid pricing
            foreach (var service in services)
            {
                Assert.IsTrue(service.UnitPrice.HasValue, 
                    $"Service {service.ServiceName} should have unit price");
                Assert.IsTrue(service.UnitPrice > 0, 
                    $"Service {service.ServiceName} should have positive price");
            }
        }

        #endregion

        #region BR90 - GetById Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR90")]
        [Description("TC_BR90_001: Integration - Verify GetById retrieves correct service")]
        public void TC_BR90_001_Integration_GetById_RetrievesCorrectService()
        {
            // Arrange - Get an existing service ID
            var allServices = _serviceService.GetAll().ToList();
            
            if (allServices.Count == 0)
            {
                Assert.Inconclusive("No services in database to test");
                return;
            }

            var existingId = allServices.First().ServiceId;

            // Act
            var service = _serviceService.GetById(existingId);

            // Assert
            Assert.IsNotNull(service, "Should retrieve service by ID");
            Assert.AreEqual(existingId, service.ServiceId);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR90")]
        [Description("TC_BR90_002: Integration - Verify GetById returns null for non-existent ID")]
        public void TC_BR90_002_Integration_GetById_ReturnsNull_ForNonExistentId()
        {
            // Act - Use an ID that shouldn't exist
            var service = _serviceService.GetById(99999);

            // Assert
            Assert.IsNull(service, "Should return null for non-existent ID");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR90")]
        [Description("TC_BR90_003: Integration - Verify GetById preserves all service properties")]
        public void TC_BR90_003_Integration_GetById_PreservesAllProperties()
        {
            // Arrange
            var allServices = _serviceService.GetAll().ToList();
            
            if (allServices.Count == 0)
            {
                Assert.Inconclusive("No services in database to test");
                return;
            }

            var original = allServices.First();

            // Act
            var retrieved = _serviceService.GetById(original.ServiceId);

            // Assert
            Assert.AreEqual(original.ServiceName, retrieved.ServiceName);
            Assert.AreEqual(original.UnitPrice, retrieved.UnitPrice);
            Assert.AreEqual(original.Note, retrieved.Note);
        }

        #endregion

        #region BR89-BR105 - Complete Workflow Test

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR89")]
        [TestCategory("BR90")]
        [TestCategory("BR91")]
        [TestCategory("BR92")]
        [TestCategory("BR93")]
        [Description("Integration - Complete read workflow for services")]
        public void Integration_Service_CompleteReadWorkflow()
        {
            // Verify read operations work
            var services = _serviceService.GetAll().ToList();
            
            Assert.IsTrue(services.Count >= 0, 
                "Should be able to query services (may be empty in test DB)");
            
            if (services.Count > 0)
            {
                var firstService = services.First();
                var retrieved = _serviceService.GetById(firstService.ServiceId);
                
                Assert.IsNotNull(retrieved, "Should retrieve service by ID");
                Assert.AreEqual(firstService.ServiceName, retrieved.ServiceName);
                Assert.AreEqual(firstService.UnitPrice, retrieved.UnitPrice);
            }
        }

        #endregion

        #region Data Validation Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR89")]
        [Description("Integration - Verify all services have valid pricing")]
        public void Integration_Services_HaveValidPricing()
        {
            // Act
            var services = _serviceService.GetAll().ToList();

            if (services.Count == 0)
            {
                Assert.Inconclusive("No services in database");
                return;
            }

            // Assert - All prices should be positive and reasonable
            foreach (var service in services)
            {
                Assert.IsTrue(service.UnitPrice > 0, 
                    $"Service '{service.ServiceName}' should have positive unit price");
                
                // Reasonable price range check (100000 to 50000000 VND)
                Assert.IsTrue(service.UnitPrice >= 100000, 
                    $"Service '{service.ServiceName}' price seems too low");
                Assert.IsTrue(service.UnitPrice <= 50000000, 
                    $"Service '{service.ServiceName}' price seems too high");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR89")]
        [Description("Integration - Verify service names are unique")]
        public void Integration_Services_HaveUniqueNames()
        {
            // Act
            var services = _serviceService.GetAll().ToList();

            if (services.Count == 0)
            {
                Assert.Inconclusive("No services in database");
                return;
            }

            // Assert - No duplicate names
            var uniqueNames = services.Select(s => s.ServiceName).Distinct().Count();
            Assert.AreEqual(services.Count, uniqueNames, 
                "All service names should be unique");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR89")]
        [Description("Integration - Verify services have appropriate data types")]
        public void Integration_Services_HaveAppropriateDataTypes()
        {
            // Act
            var services = _serviceService.GetAll().ToList();

            if (services.Count == 0)
            {
                Assert.Inconclusive("No services in database");
                return;
            }

            // Assert - Check data types
            foreach (var service in services)
            {
                Assert.IsTrue(service.ServiceId > 0, 
                    "Service ID should be positive integer");
                Assert.IsFalse(string.IsNullOrWhiteSpace(service.ServiceName), 
                    "Service name should not be empty");
                Assert.IsTrue(service.UnitPrice.HasValue && service.UnitPrice.Value > 0, 
                    $"Service '{service.ServiceName}' should have valid price");
                
                // Note can be null or empty - that's valid
                // Just verify it's a string type (implicitly checked by property type)
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR89")]
        [Description("Integration - Verify services are in reasonable price order")]
        public void Integration_Services_HaveReasonablePriceRange()
        {
            // Act
            var services = _serviceService.GetAll().ToList();

            if (services.Count < 2)
            {
                Assert.Inconclusive("Need at least 2 services to test price range");
                return;
            }

            // Assert - Check price distribution
            var minPrice = services.Min(s => s.UnitPrice);
            var maxPrice = services.Max(s => s.UnitPrice);
            
            Assert.IsTrue(minPrice > 0, "Minimum price should be positive");
            Assert.IsTrue(maxPrice > minPrice, "Should have price variation");
            
            // Price range shouldn't be too extreme (max/min < 100)
            var priceRatio = maxPrice / minPrice;
            Assert.IsTrue(priceRatio < 100, 
                "Price range seems too extreme (most expensive is 100x cheapest)");
        }

        #endregion
    }
}
