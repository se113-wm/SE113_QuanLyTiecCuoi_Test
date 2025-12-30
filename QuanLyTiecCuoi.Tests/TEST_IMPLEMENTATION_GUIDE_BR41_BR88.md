# Test Implementation Guide for BR41-BR88
## Hall, HallType & Dish Management Tests

Based on the successful BR137-BR138 test implementation pattern, this guide shows how to write tests for BR41-BR88 (Hall, HallType, Dish management).

---

## ?? Overview

### Test Coverage for BR41-BR88

| Module | Business Rules | Test Files Needed |
|--------|----------------|-------------------|
| **Hall Management** | BR41-BR59 | HallViewModelTests, HallServiceTests, Hall Integration Tests |
| **HallType Management** | BR60-BR70 | HallTypeViewModelTests, HallTypeServiceTests |
| **Dish Management** | BR71-BR88 | DishViewModelTests, DishServiceTests |

---

## ??? Test Architecture Pattern

Following the **Test Pyramid** from BR137-BR138:

```
                    /\
                   /  \
                  / UI  \        Manual Test Scenarios (MD files)
                 / Tests \       + Automated UI Tests (FlaUI)
                /________\
               /          \
              / Integration\     Cross-layer tests
             /    Tests     \    (ViewModel+Service+DB)
            /______________\
           /                \
          /   Unit Tests     \   ViewModel + Service tests
         /____________________\  (with Moq)
```

### Test Distribution Goal

| Module | Unit Tests | Integration Tests | UI Manual Scenarios |
|--------|-----------|-------------------|---------------------|
| Hall Management | ~40-50 | ~10 | ~10 |
| HallType Management | ~25-30 | ~5 | ~5 |
| Dish Management | ~35-40 | ~8 | ~8 |

---

## ?? Test File Structure

Create these files following BR137-BR138 pattern:

```
QuanLyTiecCuoi.Tests\
?
??? UnitTests\
?   ??? ViewModels\
?   ?   ??? HallViewModelTests.cs           ? Create
?   ?   ??? HallTypeViewModelTests.cs       ? Create
?   ?   ??? DishViewModelTests.cs           ? Create
?   ?
?   ??? Services\
?       ??? HallServiceTests.cs             ? Already exists (expand)
?       ??? HallTypeServiceTests.cs         ? Create
?       ??? DishServiceTests.cs             ? Create
?
??? IntegrationTests\
?   ??? HallManagementIntegrationTests.cs   ? Create
?   ??? HallTypeIntegrationTests.cs         ? Create
?   ??? DishManagementIntegrationTests.cs   ? Create
?
??? hall-management-doc\
?   ??? test-case\                          ? Already has MD files
?   ??? BR41_BR59_TEST_STRATEGY.md          ? Create (like TEST_STRATEGY_BR137_BR138.md)
?
??? hall-type-management-doc\
?   ??? test-case\                          ? Already has MD files
?   ??? BR60_BR70_TEST_STRATEGY.md          ? Create
?
??? dish-management-doc\
    ??? test-case\                          ? Already has MD files
    ??? BR71_BR88_TEST_STRATEGY.md          ? Create
```

---

## ?? Step-by-Step Implementation

### Phase 1: Unit Tests for ViewModels

#### Example: HallViewModelTests.cs

Pattern from `AddWeddingViewModelTests.cs`:

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation.ViewModel;

namespace QuanLyTiecCuoi.Tests.UnitTests.ViewModels
{
    [TestClass]
    public class HallViewModelTests
    {
        private Mock<IHallService> _mockHallService;
        private Mock<IHallTypeService> _mockHallTypeService;

        [TestInitialize]
        public void Setup()
        {
            _mockHallService = new Mock<IHallService>();
            _mockHallTypeService = new Mock<IHallTypeService>();
            
            // Setup default mock returns
            _mockHallService.Setup(s => s.GetAll()).Returns(CreateSampleHalls());
            _mockHallTypeService.Setup(s => s.GetAll()).Returns(CreateSampleHallTypes());
        }

        #region BR41 - Display Hall List Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR41")]
        [Description("TC_BR41_001: Verify HallViewModel initializes and loads hall list")]
        public void TC_BR41_001_Constructor_LoadsHallList()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.List);
            Assert.IsTrue(viewModel.List.Count > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR41")]
        [Description("TC_BR41_002: Verify hall list contains hall type information")]
        public void TC_BR41_002_HallList_ContainsHallTypeInfo()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var hall in viewModel.List)
            {
                Assert.IsNotNull(hall.HallType);
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallType.HallTypeName));
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR41")]
        [Description("TC_BR41_003: Verify hall list contains capacity information")]
        public void TC_BR41_003_HallList_ContainsCapacityInfo()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var hall in viewModel.List)
            {
                Assert.IsNotNull(hall.MaxTableCount);
                Assert.IsTrue(hall.MaxTableCount > 0);
            }
        }

        #endregion

        #region BR42 - Search/Filter Hall Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR42")]
        [Description("TC_BR42_001: Verify search by hall name works")]
        public void TC_BR42_001_SearchByHallName_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "Diamond";

            // Act
            viewModel.SearchKeyword = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.List.All(h => 
                h.HallName.Contains(searchTerm)));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR42")]
        [Description("TC_BR42_002: Verify filter by hall type works")]
        public void TC_BR42_002_FilterByHallType_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var hallTypeName = "VIP";

            // Act
            viewModel.SelectedHallType = hallTypeName;

            // Assert
            Assert.IsTrue(viewModel.List.All(h => 
                h.HallType.HallTypeName == hallTypeName));
        }

        #endregion

        #region BR43 - Create Hall Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR43")]
        [Description("TC_BR43_001: Verify CreateHallCommand is initialized")]
        public void TC_BR43_001_CreateHallCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.CreateHallCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR43")]
        [Description("TC_BR43_002: Verify create hall validates required fields")]
        public void TC_BR43_002_CreateHall_ValidatesRequiredFields()
        {
            // Arrange
            var viewModel = CreateViewModel();
            
            // Act - Try to create without hall name
            viewModel.HallName = "";
            bool canExecute = viewModel.CreateHallCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute, "Should not allow create with empty hall name");
        }

        #endregion

        #region BR44 - Update Hall Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR44")]
        [Description("TC_BR44_001: Verify UpdateHallCommand is initialized")]
        public void TC_BR44_001_UpdateHallCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.UpdateHallCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR44")]
        [Description("TC_BR44_002: Verify selected hall can be updated")]
        public void TC_BR44_002_SelectedHall_CanBeUpdated()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var hall = viewModel.List.First();
            viewModel.SelectedItem = hall;

            // Act
            viewModel.HallName = "Updated Hall Name";
            viewModel.MaxTableCount = "100";

            // Assert
            Assert.AreEqual("Updated Hall Name", viewModel.HallName);
            Assert.AreEqual("100", viewModel.MaxTableCount);
        }

        #endregion

        #region BR45 - Delete Hall Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR45")]
        [Description("TC_BR45_001: Verify DeleteHallCommand is initialized")]
        public void TC_BR45_001_DeleteHallCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.DeleteHallCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [TestCategory("BR45")]
        [Description("TC_BR45_002: Verify delete requires hall selection")]
        public void TC_BR45_002_DeleteHall_RequiresSelection()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.SelectedItem = null;

            // Act
            bool canExecute = viewModel.DeleteHallCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute, "Should not allow delete without selection");
        }

        #endregion

        #region Property Change Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallViewModel")]
        [Description("Verify properties raise PropertyChanged")]
        public void Properties_RaisePropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var changedProperties = new List<string>();
            viewModel.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName);

            // Act
            viewModel.HallName = "Test";
            viewModel.MaxTableCount = "50";
            viewModel.SearchKeyword = "Search";

            // Assert
            Assert.IsTrue(changedProperties.Contains("HallName"));
            Assert.IsTrue(changedProperties.Contains("MaxTableCount"));
            Assert.IsTrue(changedProperties.Contains("SearchKeyword"));
        }

        #endregion

        #region Helper Methods

        private HallViewModel CreateViewModel()
        {
            return new HallViewModel(
                _mockHallService.Object,
                _mockHallTypeService.Object);
        }

        private List<HallDTO> CreateSampleHalls()
        {
            return new List<HallDTO>
            {
                new HallDTO
                {
                    HallId = 1,
                    HallName = "S?nh Diamond",
                    MaxTableCount = 50,
                    HallTypeId = 1,
                    HallType = new HallTypeDTO
                    {
                        HallTypeId = 1,
                        HallTypeName = "VIP",
                        MinTablePrice = 2000000
                    }
                },
                new HallDTO
                {
                    HallId = 2,
                    HallName = "S?nh Gold",
                    MaxTableCount = 40,
                    HallTypeId = 2,
                    HallType = new HallTypeDTO
                    {
                        HallTypeId = 2,
                        HallTypeName = "Standard",
                        MinTablePrice = 1500000
                    }
                }
            };
        }

        private List<HallTypeDTO> CreateSampleHallTypes()
        {
            return new List<HallTypeDTO>
            {
                new HallTypeDTO { HallTypeId = 1, HallTypeName = "VIP", MinTablePrice = 2000000 },
                new HallTypeDTO { HallTypeId = 2, HallTypeName = "Standard", MinTablePrice = 1500000 }
            };
        }

        #endregion
    }
}
```

---

### Phase 2: Unit Tests for Services

#### Example: HallTypeServiceTests.cs

Pattern from `HallServiceTests.cs`:

```csharp
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
    [TestClass]
    public class HallTypeServiceTests
    {
        private Mock<IHallTypeRepository> _mockRepository;
        private HallTypeService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IHallTypeRepository>();
            _service = new HallTypeService(_mockRepository.Object);
        }

        #region BR60 - Get All HallTypes Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR60")]
        [Description("TC_BR60_001: Verify GetAll returns all hall types")]
        public void TC_BR60_001_GetAll_ReturnsAllHallTypes()
        {
            // Arrange
            var hallTypes = CreateSampleHallTypes();
            _mockRepository.Setup(r => r.GetAll()).Returns(hallTypes);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(ht => ht.HallTypeName == "VIP"));
            Assert.IsTrue(result.Any(ht => ht.HallTypeName == "Standard"));
            Assert.IsTrue(result.Any(ht => ht.HallTypeName == "Economy"));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR60")]
        [Description("TC_BR60_002: Verify GetAll returns DTOs with correct mapping")]
        public void TC_BR60_002_GetAll_ReturnsDTOsWithCorrectMapping()
        {
            // Arrange
            var hallTypes = CreateSampleHallTypes();
            _mockRepository.Setup(r => r.GetAll()).Returns(hallTypes);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            var vipType = result.First(ht => ht.HallTypeName == "VIP");
            Assert.AreEqual(1, vipType.HallTypeId);
            Assert.AreEqual(2000000, vipType.MinTablePrice);
        }

        #endregion

        #region BR61 - Get HallType By ID Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR61")]
        [Description("TC_BR61_001: Verify GetById returns correct hall type")]
        public void TC_BR61_001_GetById_ReturnsCorrectHallType()
        {
            // Arrange
            var hallType = new HallType
            {
                HallTypeId = 1,
                HallTypeName = "VIP",
                MinTablePrice = 2000000
            };
            _mockRepository.Setup(r => r.GetById(1)).Returns(hallType);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.HallTypeId);
            Assert.AreEqual("VIP", result.HallTypeName);
            Assert.AreEqual(2000000, result.MinTablePrice);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR61")]
        [Description("TC_BR61_002: Verify GetById returns null for non-existent ID")]
        public void TC_BR61_002_GetById_ReturnsNull_ForNonExistentId()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(999)).Returns((HallType)null);

            // Act
            var result = _service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region BR62 - Create HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR62")]
        [Description("TC_BR62_001: Verify Create calls repository Create")]
        public void TC_BR62_001_Create_CallsRepositoryCreate()
        {
            // Arrange
            var dto = new HallTypeDTO
            {
                HallTypeName = "New Type",
                MinTablePrice = 1800000
            };

            // Act
            _service.Create(dto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.Is<HallType>(ht =>
                ht.HallTypeName == "New Type" &&
                ht.MinTablePrice == 1800000
            )), Times.Once);
        }

        #endregion

        #region BR63 - Update HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR63")]
        [Description("TC_BR63_001: Verify Update calls repository Update")]
        public void TC_BR63_001_Update_CallsRepositoryUpdate()
        {
            // Arrange
            var dto = new HallTypeDTO
            {
                HallTypeId = 1,
                HallTypeName = "Updated Type",
                MinTablePrice = 2500000
            };

            // Act
            _service.Update(dto);

            // Assert
            _mockRepository.Verify(r => r.Update(It.Is<HallType>(ht =>
                ht.HallTypeId == 1 &&
                ht.HallTypeName == "Updated Type" &&
                ht.MinTablePrice == 2500000
            )), Times.Once);
        }

        #endregion

        #region BR64 - Delete HallType Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("HallTypeService")]
        [TestCategory("BR64")]
        [Description("TC_BR64_001: Verify Delete calls repository Delete")]
        public void TC_BR64_001_Delete_CallsRepositoryDelete()
        {
            // Act
            _service.Delete(1);

            // Assert
            _mockRepository.Verify(r => r.Delete(1), Times.Once);
        }

        #endregion

        #region Helper Methods

        private List<HallType> CreateSampleHallTypes()
        {
            return new List<HallType>
            {
                new HallType { HallTypeId = 1, HallTypeName = "VIP", MinTablePrice = 2000000 },
                new HallType { HallTypeId = 2, HallTypeName = "Standard", MinTablePrice = 1500000 },
                new HallType { HallTypeId = 3, HallTypeName = "Economy", MinTablePrice = 1000000 }
            };
        }

        #endregion
    }
}
```

---

### Phase 3: Integration Tests

#### Example: HallManagementIntegrationTests.cs

Pattern from `BookingManagementIntegrationTests.cs`:

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.ViewModel;

namespace QuanLyTiecCuoi.Tests.IntegrationTests
{
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
            
            // Initialize services
            _hallService = new HallService(new HallRepository());
            _hallTypeService = new HallTypeService(new HallTypeRepository());
        }

        #region BR41 - Display Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR41")]
        [Description("TC_BR41_001: Integration - Verify HallViewModel loads actual halls from database")]
        public void TC_BR41_001_Integration_HallViewModel_LoadsActualHalls()
        {
            // Act
            var viewModel = new HallViewModel(_hallService, _hallTypeService);

            // Assert
            Assert.IsNotNull(viewModel.List);
            Assert.IsTrue(viewModel.List.Count > 0, "Should load halls from database");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR41")]
        [Description("TC_BR41_002: Integration - Verify halls have complete information")]
        public void TC_BR41_002_Integration_Halls_HaveCompleteInformation()
        {
            // Act
            var halls = _hallService.GetAll().ToList();

            // Assert
            Assert.IsTrue(halls.Count > 0);
            foreach (var hall in halls)
            {
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallName));
                Assert.IsNotNull(hall.MaxTableCount);
                Assert.IsNotNull(hall.HallType);
                Assert.IsFalse(string.IsNullOrEmpty(hall.HallType.HallTypeName));
            }
        }

        #endregion

        #region BR42 - Search/Filter Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR42")]
        [Description("TC_BR42_001: Integration - Verify search works with real data")]
        public void TC_BR42_001_Integration_Search_WorksWithRealData()
        {
            // Arrange
            var viewModel = new HallViewModel(_hallService, _hallTypeService);
            
            if (viewModel.List.Count == 0)
            {
                Assert.Inconclusive("No halls in database to test");
                return;
            }

            var firstHallName = viewModel.List.First().HallName;
            var searchTerm = firstHallName.Substring(0, 3); // First 3 characters

            // Act
            viewModel.SearchKeyword = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.List.All(h => h.HallName.Contains(searchTerm)));
        }

        #endregion

        #region BR60 - HallType Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR60")]
        [Description("TC_BR60_001: Integration - Verify all hall types load correctly")]
        public void TC_BR60_001_Integration_HallTypes_LoadCorrectly()
        {
            // Act
            var hallTypes = _hallTypeService.GetAll().ToList();

            // Assert
            Assert.IsTrue(hallTypes.Count > 0);
            foreach (var hallType in hallTypes)
            {
                Assert.IsFalse(string.IsNullOrEmpty(hallType.HallTypeName));
                Assert.IsNotNull(hallType.MinTablePrice);
                Assert.IsTrue(hallType.MinTablePrice > 0);
            }
        }

        #endregion
    }
}
```

---

## ?? Test Naming Convention

Follow the exact pattern from BR137-BR138:

```csharp
[TestMethod]
[TestCategory("UnitTest")]           // or "IntegrationTest"
[TestCategory("HallViewModel")]      // Component name
[TestCategory("BR41")]                // Business Rule ID
[Description("TC_BR41_001: Test description")]
public void TC_BR41_001_MethodName_Scenario_ExpectedResult()
{
    // Test implementation
}
```

### Examples:

```csharp
// Unit Test
TC_BR41_001_Constructor_LoadsHallList()

// Integration Test  
TC_BR41_001_Integration_HallViewModel_LoadsActualHalls()

// Service Test
TC_BR60_001_GetAll_ReturnsAllHallTypes()
```

---

## ?? Test Categories Mapping

Map your MD test cases to code tests:

| MD File | Code Test Category | Test File |
|---------|-------------------|-----------|
| `TC_BR41_001.md` | `[TestCategory("BR41")]` | `HallViewModelTests.cs` |
| `TC_BR42_001.md` | `[TestCategory("BR42")]` | `HallViewModelTests.cs` |
| `TC_BR60_001.md` | `[TestCategory("BR60")]` | `HallTypeViewModelTests.cs` |
| `TC_BR71_001.md` | `[TestCategory("BR71")]` | `DishViewModelTests.cs` |

---

## ? Test Implementation Checklist

### For Each Business Rule (BR41-BR88):

- [ ] **Read MD test case** in respective folder
- [ ] **Identify test type**: Unit, Integration, or UI
- [ ] **Create unit test** for ViewModel (if UI operation)
- [ ] **Create unit test** for Service (if business logic)
- [ ] **Create integration test** (for cross-layer validation)
- [ ] **Add test categories**: `[TestCategory("BR##")]`
- [ ] **Follow naming convention**: `TC_BR##_###_Description`
- [ ] **Write AAA pattern**: Arrange, Act, Assert
- [ ] **Add description attribute**: `[Description("...")]`

---

## ?? Quick Start Commands

### Run tests by module:

```bash
# Hall Management tests
dotnet test --filter "TestCategory=BR41|TestCategory=BR42|TestCategory=BR43"

# HallType Management tests
dotnet test --filter "TestCategory=BR60|TestCategory=BR61|TestCategory=BR62"

# Dish Management tests
dotnet test --filter "TestCategory=BR71|TestCategory=BR72|TestCategory=BR73"

# All Hall-related tests
dotnet test --filter "HallViewModel|HallService"

# All integration tests
dotnet test --filter "TestCategory=IntegrationTest"
```

---

## ?? Progress Tracking

Create a simple checklist:

```markdown
## Hall Management (BR41-BR59)
- [ ] HallViewModelTests.cs (40 tests)
  - [ ] BR41: Display (5 tests)
  - [ ] BR42: Search/Filter (5 tests)
  - [ ] BR43: Create (5 tests)
  - [ ] BR44: Update (5 tests)
  - [ ] BR45: Delete (5 tests)
  - [ ] BR46-59: Additional features (15 tests)
- [ ] HallServiceTests.cs (25 tests)
- [ ] HallManagementIntegrationTests.cs (10 tests)

## HallType Management (BR60-BR70)
- [ ] HallTypeViewModelTests.cs (25 tests)
- [ ] HallTypeServiceTests.cs (15 tests)
- [ ] HallTypeIntegrationTests.cs (5 tests)

## Dish Management (BR71-BR88)
- [ ] DishViewModelTests.cs (35 tests)
- [ ] DishServiceTests.cs (20 tests)
- [ ] DishManagementIntegrationTests.cs (8 tests)
```

---

## ?? Tips & Best Practices

### 1. **Start with Service Tests**
Service tests are simpler (just data mapping). Build confidence first.

### 2. **Use Helper Methods**
Create `CreateSampleHalls()`, `CreateSampleDishes()` like in BR137-BR138 tests.

### 3. **Mock Dependencies**
Always mock services in ViewModel tests:
```csharp
Mock<IHallService> _mockHallService;
Mock<IHallTypeService> _mockHallTypeService;
```

### 4. **Test One Thing**
Each test should verify ONE specific behavior.

### 5. **Use Descriptive Names**
```csharp
// Good
TC_BR41_001_Constructor_LoadsHallList()

// Bad
Test1()
```

### 6. **Follow AAA Pattern**
```csharp
// Arrange
var viewModel = CreateViewModel();

// Act
viewModel.LoadHalls();

// Assert
Assert.IsTrue(viewModel.List.Count > 0);
```

### 7. **Group Related Tests**
Use `#region` like in BR137-BR138 tests:
```csharp
#region BR41 - Display Hall List Tests
// Tests here
#endregion
```

---

## ?? Reference Documentation

Study these existing files as templates:

### Unit Tests:
- `AddWeddingViewModelTests.cs` - ViewModel testing pattern
- `WeddingViewModelTests.cs` - List & filter testing
- `HallServiceTests.cs` - Service testing pattern

### Integration Tests:
- `BookingManagementIntegrationTests.cs` - Cross-layer testing pattern

### Documentation:
- `TEST_STRATEGY_BR137_BR138.md` - Test strategy template
- `BR137_BR138_UI_Test_Scenarios.md` - Manual test scenarios

---

## ?? Success Criteria

Your test suite is complete when:

? All BR41-BR88 have corresponding code tests  
? Unit test coverage ? 70% for new ViewModels/Services  
? Integration tests cover happy paths  
? All tests follow naming convention  
? Tests are organized with TestCategory attributes  
? Helper methods reduce code duplication  
? Tests run successfully: `dotnet test`  

---

## ?? Need Help?

If stuck:

1. **Check existing tests**: Look at BR137-BR138 tests for similar patterns
2. **Read MD files**: Your test case documentation is the spec
3. **Start simple**: Begin with `GetAll()` service tests
4. **Build incrementally**: One BR at a time
5. **Run frequently**: `dotnet test` after each test

---

**Ready to start! Follow this guide and you'll have a complete test suite matching the quality of BR137-BR138 tests!** ??

---

**Document Version:** 1.0  
**Based on:** BR137-BR138 test implementation  
**Target:** BR41-BR88 (Hall, HallType, Dish Management)  
**Test Count Goal:** ~200-250 tests total
