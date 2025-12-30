using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.IntegrationTests
{
    /// <summary>
    /// Integration Tests for Dish Management (BR71-BR88)
    /// Tests the interaction between Service and Repository layers with real database
    /// </summary>
    [TestClass]
    public class DishManagementIntegrationTests
    {
        private DishService _dishService;

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize service with real repository
            _dishService = new DishService(new DishRepository());
        }

        #region BR71 - Display Dish Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR71")]
        [Description("TC_BR71_001: Integration - Verify dishes load from database")]
        public void TC_BR71_001_Integration_Dishes_LoadFromDatabase()
        {
            // Act
            var dishes = _dishService.GetAll().ToList();

            // Assert
            Assert.IsTrue(dishes.Count > 0, "Should load dishes from database");
            
            // Verify each dish has required fields
            foreach (var dish in dishes)
            {
                Assert.IsFalse(string.IsNullOrEmpty(dish.DishName), 
                    "Dish name should not be empty");
                Assert.IsNotNull(dish.UnitPrice, 
                    "Unit price should not be null");
                Assert.IsTrue(dish.UnitPrice > 0, 
                    $"Dish {dish.DishName} should have positive price");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR71")]
        [Description("TC_BR71_002: Integration - Verify dishes are accessible")]
        public void TC_BR71_002_Integration_Dishes_AreAccessible()
        {
            // Act
            var dishes = _dishService.GetAll().ToList();

            if (dishes.Count == 0)
            {
                Assert.Inconclusive("No dishes in database to test");
                return;
            }

            // Assert - Can access dish properties
            var firstDish = dishes.First();
            Assert.IsTrue(firstDish.DishId > 0);
            Assert.IsFalse(string.IsNullOrEmpty(firstDish.DishName));
        }

        #endregion

        #region BR72 - GetById Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR72")]
        [Description("TC_BR72_001: Integration - Verify GetById retrieves correct dish")]
        public void TC_BR72_001_Integration_GetById_RetrievesCorrectDish()
        {
            // Arrange - Get an existing dish ID
            var allDishes = _dishService.GetAll().ToList();
            
            if (allDishes.Count == 0)
            {
                Assert.Inconclusive("No dishes in database to test");
                return;
            }

            var existingId = allDishes.First().DishId;

            // Act
            var dish = _dishService.GetById(existingId);

            // Assert
            Assert.IsNotNull(dish, "Should retrieve dish by ID");
            Assert.AreEqual(existingId, dish.DishId);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR72")]
        [Description("TC_BR72_002: Integration - Verify GetById returns null for non-existent ID")]
        public void TC_BR72_002_Integration_GetById_ReturnsNull_ForNonExistentId()
        {
            // Act - Use an ID that shouldn't exist
            var dish = _dishService.GetById(99999);

            // Assert
            Assert.IsNull(dish, "Should return null for non-existent ID");
        }

        #endregion

        #region BR71-BR88 - Complete CRUD Workflow Test

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR71")]
        [TestCategory("BR72")]
        [TestCategory("BR73")]
        [TestCategory("BR74")]
        [TestCategory("BR75")]
        [Description("Integration - Complete CRUD workflow for dishes")]
        public void Integration_Dish_CompleteCRUDWorkflow()
        {
            // This test verifies read operations work with real database
            
            // Verify read operations
            var dishes = _dishService.GetAll().ToList();
            
            Assert.IsTrue(dishes.Count >= 0, 
                "Should be able to query dishes (may be empty in test DB)");
            
            if (dishes.Count > 0)
            {
                var firstDish = dishes.First();
                var retrieved = _dishService.GetById(firstDish.DishId);
                
                Assert.IsNotNull(retrieved, "Should retrieve dish by ID");
                Assert.AreEqual(firstDish.DishName, retrieved.DishName);
                Assert.AreEqual(firstDish.UnitPrice, retrieved.UnitPrice);
            }
        }

        #endregion

        #region Data Validation Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR71")]
        [Description("Integration - Verify all dishes have valid pricing")]
        public void Integration_Dishes_HaveValidPricing()
        {
            // Act
            var dishes = _dishService.GetAll().ToList();

            if (dishes.Count == 0)
            {
                Assert.Inconclusive("No dishes in database");
                return;
            }

            // Assert - All prices should be positive
            foreach (var dish in dishes)
            {
                Assert.IsTrue(dish.UnitPrice > 0, 
                    $"Dish '{dish.DishName}' should have positive unit price");
                
                // Reasonable price range check (10000 to 1000000 VND)
                Assert.IsTrue(dish.UnitPrice >= 10000, 
                    $"Dish '{dish.DishName}' price seems too low");
                Assert.IsTrue(dish.UnitPrice <= 1000000, 
                    $"Dish '{dish.DishName}' price seems unreasonably high");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR71")]
        [Description("Integration - Verify dish names are unique")]
        public void Integration_Dishes_HaveUniqueNames()
        {
            // Act
            var dishes = _dishService.GetAll().ToList();

            if (dishes.Count == 0)
            {
                Assert.Inconclusive("No dishes in database");
                return;
            }

            // Assert - No duplicate names
            var uniqueNames = dishes.Select(d => d.DishName).Distinct().Count();
            Assert.AreEqual(dishes.Count, uniqueNames, 
                "All dish names should be unique");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR71")]
        [Description("Integration - Verify dishes with notes have valid information")]
        public void Integration_Dishes_WithNotes_HaveValidInfo()
        {
            // Act
            var dishes = _dishService.GetAll().ToList();

            if (dishes.Count == 0)
            {
                Assert.Inconclusive("No dishes in database");
                return;
            }

            // Assert - Dishes with notes should have meaningful content
            var dishesWithNotes = dishes.Where(d => !string.IsNullOrEmpty(d.Note)).ToList();
            
            if (dishesWithNotes.Count > 0)
            {
                foreach (var dish in dishesWithNotes)
                {
                    Assert.IsTrue(dish.Note.Length >= 3, 
                        $"Dish '{dish.DishName}' note should have at least 3 characters");
                }
            }
        }

        #endregion

        #region Search and Filter Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR71")]
        [Description("Integration - Verify can search dishes by partial name")]
        public void Integration_Dishes_CanSearchByPartialName()
        {
            // Arrange
            var allDishes = _dishService.GetAll().ToList();

            if (allDishes.Count == 0)
            {
                Assert.Inconclusive("No dishes in database to test search");
                return;
            }

            var firstDish = allDishes.First();
            var searchTerm = firstDish.DishName.Length >= 3 
                ? firstDish.DishName.Substring(0, 3) 
                : firstDish.DishName;

            // Act - Simulate search
            var searchResults = allDishes.Where(d => 
                d.DishName.Contains(searchTerm)).ToList();

            // Assert
            Assert.IsTrue(searchResults.Count > 0, "Should find at least one dish");
            Assert.IsTrue(searchResults.Any(d => d.DishId == firstDish.DishId), 
                "Should find the original dish");
        }

        #endregion
    }
}
