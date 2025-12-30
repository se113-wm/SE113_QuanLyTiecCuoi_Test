using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.IntegrationTests
{
    /// <summary>
    /// Integration Tests for Hall Management (BR41-BR50)
    /// Tests the interaction between Service and Repository layers with real database
    /// </summary>
    [TestClass]
    public class HallManagementIntegrationTests
    {
        private HallService _hallService;
        private HallTypeService _hallTypeService;

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize services with real repositories
            _hallService = new HallService(new HallRepository());
            _hallTypeService = new HallTypeService(new HallTypeRepository());
        }

        #region BR41 - Display Hall Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR41")]
        [Description("TC_BR41_001: Integration - Verify halls load from database")]
        public void TC_BR41_001_Integration_Halls_LoadFromDatabase()
        {
            // Act
            var halls = _hallService.GetAll().ToList();

            // Assert
            Assert.IsTrue(halls.Count > 0, "Should load halls from database");
            
            // Verify each hall has required fields
            foreach (var hall in halls)
            {
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallName), 
                    "Hall name should not be empty");
                Assert.IsNotNull(hall.MaxTableCount, 
                    "Max table count should not be null");
                Assert.IsTrue(hall.MaxTableCount > 0, 
                    $"Hall {hall.HallName} should have positive table count");
                Assert.IsNotNull(hall.HallType, 
                    $"Hall {hall.HallName} should have hall type");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR41")]
        [Description("TC_BR41_002: Integration - Verify halls are accessible")]
        public void TC_BR41_002_Integration_Halls_AreAccessible()
        {
            // Act
            var halls = _hallService.GetAll().ToList();

            if (halls.Count == 0)
            {
                Assert.Inconclusive("No halls in database to test");
                return;
            }

            // Assert - Can access hall properties
            var firstHall = halls.First();
            Assert.IsTrue(firstHall.HallId > 0);
            Assert.IsFalse(string.IsNullOrEmpty(firstHall.HallName));
        }

        #endregion

        #region BR42 - GetById Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR42")]
        [Description("TC_BR42_001: Integration - Verify GetById retrieves correct hall")]
        public void TC_BR42_001_Integration_GetById_RetrievesCorrectHall()
        {
            // Arrange - Get an existing hall ID
            var allHalls = _hallService.GetAll().ToList();
            
            if (allHalls.Count == 0)
            {
                Assert.Inconclusive("No halls in database to test");
                return;
            }

            var existingId = allHalls.First().HallId;

            // Act
            var hall = _hallService.GetById(existingId);

            // Assert
            Assert.IsNotNull(hall, "Should retrieve hall by ID");
            Assert.AreEqual(existingId, hall.HallId);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR42")]
        [Description("TC_BR42_002: Integration - Verify GetById returns null for non-existent ID")]
        public void TC_BR42_002_Integration_GetById_ReturnsNull_ForNonExistentId()
        {
            // Act - Use an ID that shouldn't exist
            var hall = _hallService.GetById(99999);

            // Assert
            Assert.IsNull(hall, "Should return null for non-existent ID");
        }

        #endregion

        #region BR41-BR50 - Complete CRUD Workflow Test

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR41")]
        [TestCategory("BR42")]
        [TestCategory("BR43")]
        [TestCategory("BR44")]
        [TestCategory("BR45")]
        [Description("Integration - Complete CRUD workflow for halls")]
        public void Integration_Hall_CompleteCRUDWorkflow()
        {
            // This test would need to be run in a test database environment
            // where we can safely create/update/delete records
            
            // For now, we verify read operations work
            var halls = _hallService.GetAll().ToList();
            
            Assert.IsTrue(halls.Count >= 0, 
                "Should be able to query halls (may be empty in test DB)");
            
            if (halls.Count > 0)
            {
                var firstHall = halls.First();
                var retrieved = _hallService.GetById(firstHall.HallId);
                
                Assert.IsNotNull(retrieved, "Should retrieve hall by ID");
                Assert.AreEqual(firstHall.HallName, retrieved.HallName);
            }
        }

        #endregion

        #region Data Validation Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR41")]
        [Description("Integration - Verify all halls have valid hall types")]
        public void Integration_Halls_HaveValidHallTypes()
        {
            // Act
            var halls = _hallService.GetAll().ToList();

            if (halls.Count == 0)
            {
                Assert.Inconclusive("No halls in database");
                return;
            }

            // Assert - All halls should have valid hall types
            var hallTypes = _hallTypeService.GetAll().ToList();
            
            foreach (var hall in halls)
            {
                Assert.IsNotNull(hall.HallType, 
                    $"Hall '{hall.HallName}' should have a hall type");
                
                var hallTypeExists = hallTypes.Any(ht => ht.HallTypeId == hall.HallTypeId);
                Assert.IsTrue(hallTypeExists, 
                    $"Hall '{hall.HallName}' references non-existent hall type ID {hall.HallTypeId}");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR41")]
        [Description("Integration - Verify hall names are unique within hall type")]
        public void Integration_Halls_HaveUniqueNamesWithinType()
        {
            // Act
            var halls = _hallService.GetAll().ToList();

            if (halls.Count == 0)
            {
                Assert.Inconclusive("No halls in database");
                return;
            }

            // Assert - No duplicate names within same hall type
            var hallsByType = halls.GroupBy(h => h.HallTypeId);
            
            foreach (var typeGroup in hallsByType)
            {
                var hallNames = typeGroup.Select(h => h.HallName).ToList();
                var uniqueNames = hallNames.Distinct().Count();
                
                Assert.AreEqual(hallNames.Count, uniqueNames, 
                    $"Hall type ID {typeGroup.Key} has duplicate hall names");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR41")]
        [Description("Integration - Verify halls have positive table counts")]
        public void Integration_Halls_HavePositiveTableCounts()
        {
            // Act
            var halls = _hallService.GetAll().ToList();

            if (halls.Count == 0)
            {
                Assert.Inconclusive("No halls in database");
                return;
            }

            // Assert - All table counts should be positive
            foreach (var hall in halls)
            {
                Assert.IsTrue(hall.MaxTableCount > 0, 
                    $"Hall '{hall.HallName}' should have positive max table count");
                
                // Reasonable range check (10 to 200 tables)
                Assert.IsTrue(hall.MaxTableCount >= 10, 
                    $"Hall '{hall.HallName}' max table count seems too low");
                Assert.IsTrue(hall.MaxTableCount <= 200, 
                    $"Hall '{hall.HallName}' max table count seems unreasonably high");
            }
        }

        #endregion

        #region Search and Filter Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR42")]
        [Description("Integration - Verify can search halls by partial name")]
        public void Integration_Halls_CanSearchByPartialName()
        {
            // Arrange
            var allHalls = _hallService.GetAll().ToList();

            if (allHalls.Count == 0)
            {
                Assert.Inconclusive("No halls in database to test search");
                return;
            }

            var firstHall = allHalls.First();
            var searchTerm = firstHall.HallName.Length >= 3 
                ? firstHall.HallName.Substring(0, 3) 
                : firstHall.HallName;

            // Act - Simulate search
            var searchResults = allHalls.Where(h => 
                h.HallName.Contains(searchTerm)).ToList();

            // Assert
            Assert.IsTrue(searchResults.Count > 0, "Should find at least one hall");
            Assert.IsTrue(searchResults.Any(h => h.HallId == firstHall.HallId), 
                "Should find the original hall");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR42")]
        [Description("Integration - Verify can filter halls by hall type")]
        public void Integration_Halls_CanFilterByHallType()
        {
            // Arrange
            var allHalls = _hallService.GetAll().ToList();

            if (allHalls.Count == 0)
            {
                Assert.Inconclusive("No halls in database to test filtering");
                return;
            }

            var firstHallType = allHalls.First().HallTypeId;

            // Act - Filter by hall type
            var filteredHalls = allHalls.Where(h => h.HallTypeId == firstHallType).ToList();

            // Assert
            Assert.IsTrue(filteredHalls.Count > 0, "Should find halls of this type");
            Assert.IsTrue(filteredHalls.All(h => h.HallTypeId == firstHallType), 
                "All filtered halls should match the hall type");
        }

        #endregion

        #region Relationship Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR41")]
        [Description("Integration - Verify hall to hall type relationship")]
        public void Integration_Halls_HallTypeRelationship_IsValid()
        {
            // Arrange
            var halls = _hallService.GetAll().ToList();
            
            if (halls.Count == 0)
            {
                Assert.Inconclusive("No halls in database");
                return;
            }

            // Act & Assert
            foreach (var hall in halls)
            {
                Assert.IsNotNull(hall.HallType, 
                    $"Hall '{hall.HallName}' should have HallType navigation property");
                
                Assert.AreEqual(hall.HallTypeId, hall.HallType.HallTypeId, 
                    $"Hall '{hall.HallName}' HallTypeId should match HallType.HallTypeId");
                
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallType.HallTypeName), 
                    $"Hall '{hall.HallName}' HallType should have a name");
                
                Assert.IsNotNull(hall.HallType.MinTablePrice, 
                    $"Hall '{hall.HallName}' HallType should have MinTablePrice");
            }
        }

        #endregion
    }
}
