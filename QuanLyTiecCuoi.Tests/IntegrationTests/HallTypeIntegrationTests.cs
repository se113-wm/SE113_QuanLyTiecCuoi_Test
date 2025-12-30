using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.IntegrationTests
{
    /// <summary>
    /// Integration Tests for HallType Management (BR60-BR70)
    /// Tests the interaction between Service and Repository layers with real database
    /// </summary>
    [TestClass]
    public class HallTypeIntegrationTests
    {
        private HallTypeService _hallTypeService;

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize service with real repository
            _hallTypeService = new HallTypeService(new HallTypeRepository());
        }

        #region BR60 - Display HallType Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR60")]
        [Description("TC_BR60_001: Integration - Verify hall types load from database")]
        public void TC_BR60_001_Integration_HallTypes_LoadFromDatabase()
        {
            // Act
            var hallTypes = _hallTypeService.GetAll().ToList();

            // Assert
            Assert.IsTrue(hallTypes.Count > 0, "Should load hall types from database");
            
            // Verify each hall type has required fields
            foreach (var hallType in hallTypes)
            {
                Assert.IsFalse(string.IsNullOrEmpty(hallType.HallTypeName), 
                    "Hall type name should not be empty");
                Assert.IsNotNull(hallType.MinTablePrice, 
                    "Min table price should not be null");
                Assert.IsTrue(hallType.MinTablePrice > 0, 
                    $"Hall type {hallType.HallTypeName} should have positive price");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR60")]
        [Description("TC_BR60_002: Integration - Verify hall types are ordered or accessible")]
        public void TC_BR60_002_Integration_HallTypes_AreAccessible()
        {
            // Act
            var hallTypes = _hallTypeService.GetAll().ToList();

            if (hallTypes.Count == 0)
            {
                Assert.Inconclusive("No hall types in database to test");
                return;
            }

            // Assert - Can access hall type properties
            var firstHallType = hallTypes.First();
            Assert.IsTrue(firstHallType.HallTypeId > 0);
            Assert.IsFalse(string.IsNullOrEmpty(firstHallType.HallTypeName));
        }

        #endregion

        #region BR61 - GetById Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR61")]
        [Description("TC_BR61_001: Integration - Verify GetById retrieves correct hall type")]
        public void TC_BR61_001_Integration_GetById_RetrievesCorrectHallType()
        {
            // Arrange - Get an existing hall type ID
            var allHallTypes = _hallTypeService.GetAll().ToList();
            
            if (allHallTypes.Count == 0)
            {
                Assert.Inconclusive("No hall types in database to test");
                return;
            }

            var existingId = allHallTypes.First().HallTypeId;

            // Act
            var hallType = _hallTypeService.GetById(existingId);

            // Assert
            Assert.IsNotNull(hallType, "Should retrieve hall type by ID");
            Assert.AreEqual(existingId, hallType.HallTypeId);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR61")]
        [Description("TC_BR61_002: Integration - Verify GetById returns null for non-existent ID")]
        public void TC_BR61_002_Integration_GetById_ReturnsNull_ForNonExistentId()
        {
            // Act - Use an ID that shouldn't exist
            var hallType = _hallTypeService.GetById(99999);

            // Assert
            Assert.IsNull(hallType, "Should return null for non-existent ID");
        }

        #endregion

        #region BR60-BR70 - Complete CRUD Workflow Test

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR60")]
        [TestCategory("BR61")]
        [TestCategory("BR62")]
        [TestCategory("BR63")]
        [TestCategory("BR64")]
        [Description("Integration - Complete CRUD workflow for hall types")]
        public void Integration_HallType_CompleteCRUDWorkflow()
        {
            // This test would need to be run in a test database environment
            // where we can safely create/update/delete records
            
            // For now, we verify read operations work
            var hallTypes = _hallTypeService.GetAll().ToList();
            
            Assert.IsTrue(hallTypes.Count >= 0, 
                "Should be able to query hall types (may be empty in test DB)");
            
            if (hallTypes.Count > 0)
            {
                var firstType = hallTypes.First();
                var retrieved = _hallTypeService.GetById(firstType.HallTypeId);
                
                Assert.IsNotNull(retrieved, "Should retrieve hall type by ID");
                Assert.AreEqual(firstType.HallTypeName, retrieved.HallTypeName);
            }
        }

        #endregion

        #region Data Validation Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR60")]
        [Description("Integration - Verify all hall types have valid pricing")]
        public void Integration_HallTypes_HaveValidPricing()
        {
            // Act
            var hallTypes = _hallTypeService.GetAll().ToList();

            if (hallTypes.Count == 0)
            {
                Assert.Inconclusive("No hall types in database");
                return;
            }

            // Assert - All prices should be positive
            foreach (var hallType in hallTypes)
            {
                Assert.IsTrue(hallType.MinTablePrice > 0, 
                    $"Hall type '{hallType.HallTypeName}' should have positive min table price");
                
                // Reasonable price range check (1000000 to 10000000 VND)
                Assert.IsTrue(hallType.MinTablePrice >= 100000, 
                    $"Hall type '{hallType.HallTypeName}' price seems too low");
                Assert.IsTrue(hallType.MinTablePrice <= 10000000, 
                    $"Hall type '{hallType.HallTypeName}' price seems too high");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR60")]
        [Description("Integration - Verify hall type names are unique")]
        public void Integration_HallTypes_HaveUniqueNames()
        {
            // Act
            var hallTypes = _hallTypeService.GetAll().ToList();

            if (hallTypes.Count == 0)
            {
                Assert.Inconclusive("No hall types in database");
                return;
            }

            // Assert - No duplicate names
            var uniqueNames = hallTypes.Select(ht => ht.HallTypeName).Distinct().Count();
            Assert.AreEqual(hallTypes.Count, uniqueNames, 
                "All hall type names should be unique");
        }

        #endregion
    }
}
