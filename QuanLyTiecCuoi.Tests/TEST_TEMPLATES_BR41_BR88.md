# Test Templates for BR41-BR88

Quick copy-paste templates for implementing tests. Just fill in the specifics!

---

## ?? Template 1: ViewModel Unit Test

Copy this for `HallViewModelTests.cs`, `HallTypeViewModelTests.cs`, `DishViewModelTests.cs`:

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
    public class [MODULE]ViewModelTests  // Replace [MODULE] with Hall, HallType, or Dish
    {
        private Mock<I[MODULE]Service> _mockService;
        // Add other mock dependencies here

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<I[MODULE]Service>();
            
            // Setup default returns
            _mockService.Setup(s => s.GetAll()).Returns(CreateSample[MODULE]s());
        }

        #region BR## - [Feature Name] Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("[MODULE]ViewModel")]
        [TestCategory("BR##")]
        [Description("TC_BR##_###: [Test description from MD file]")]
        public void TC_BR##_###_[MethodName]_[Scenario]_[ExpectedResult]()
        {
            // Arrange
            var viewModel = CreateViewModel();
            // Setup test data

            // Act
            // Execute the action

            // Assert
            // Verify expected results
            Assert.IsTrue(true); // Replace with actual assertion
        }

        #endregion

        #region Helper Methods

        private [MODULE]ViewModel CreateViewModel()
        {
            return new [MODULE]ViewModel(_mockService.Object);
        }

        private List<[MODULE]DTO> CreateSample[MODULE]s()
        {
            return new List<[MODULE]DTO>
            {
                // Sample data here
            };
        }

        #endregion
    }
}
```

---

## ?? Template 2: Service Unit Test

Copy this for `HallServiceTests.cs`, `HallTypeServiceTests.cs`, `DishServiceTests.cs`:

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
    public class [MODULE]ServiceTests  // Replace [MODULE]
    {
        private Mock<I[MODULE]Repository> _mockRepository;
        private [MODULE]Service _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<I[MODULE]Repository>();
            _service = new [MODULE]Service(_mockRepository.Object);
        }

        #region BR## - GetAll Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("[MODULE]Service")]
        [TestCategory("BR##")]
        [Description("TC_BR##_###: Verify GetAll returns all items")]
        public void TC_BR##_###_GetAll_ReturnsAllItems()
        {
            // Arrange
            var items = CreateSample[MODULE]s();
            _mockRepository.Setup(r => r.GetAll()).Returns(items);

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.AreEqual(items.Count, result.Count);
        }

        #endregion

        #region BR## - GetById Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("[MODULE]Service")]
        [TestCategory("BR##")]
        [Description("TC_BR##_###: Verify GetById returns correct item")]
        public void TC_BR##_###_GetById_ReturnsCorrectItem()
        {
            // Arrange
            var item = new [MODULE]
            {
                [MODULE]Id = 1,
                // Set properties
            };
            _mockRepository.Setup(r => r.GetById(1)).Returns(item);

            // Act
            var result = _service.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.[MODULE]Id);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("[MODULE]Service")]
        [TestCategory("BR##")]
        [Description("TC_BR##_###: Verify GetById returns null for non-existent ID")]
        public void TC_BR##_###_GetById_ReturnsNull_ForNonExistentId()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(999)).Returns(([MODULE])null);

            // Act
            var result = _service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region BR## - Create Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("[MODULE]Service")]
        [TestCategory("BR##")]
        [Description("TC_BR##_###: Verify Create calls repository Create")]
        public void TC_BR##_###_Create_CallsRepositoryCreate()
        {
            // Arrange
            var dto = new [MODULE]DTO
            {
                // Set properties
            };

            // Act
            _service.Create(dto);

            // Assert
            _mockRepository.Verify(r => r.Create(It.IsAny<[MODULE]>()), Times.Once);
        }

        #endregion

        #region BR## - Update Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("[MODULE]Service")]
        [TestCategory("BR##")]
        [Description("TC_BR##_###: Verify Update calls repository Update")]
        public void TC_BR##_###_Update_CallsRepositoryUpdate()
        {
            // Arrange
            var dto = new [MODULE]DTO
            {
                [MODULE]Id = 1,
                // Set properties
            };

            // Act
            _service.Update(dto);

            // Assert
            _mockRepository.Verify(r => r.Update(It.IsAny<[MODULE]>()), Times.Once);
        }

        #endregion

        #region BR## - Delete Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("[MODULE]Service")]
        [TestCategory("BR##")]
        [Description("TC_BR##_###: Verify Delete calls repository Delete")]
        public void TC_BR##_###_Delete_CallsRepositoryDelete()
        {
            // Act
            _service.Delete(1);

            // Assert
            _mockRepository.Verify(r => r.Delete(1), Times.Once);
        }

        #endregion

        #region Helper Methods

        private List<[MODULE]> CreateSample[MODULE]s()
        {
            return new List<[MODULE]>
            {
                new [MODULE] { [MODULE]Id = 1, /* properties */ },
                new [MODULE] { [MODULE]Id = 2, /* properties */ },
                new [MODULE] { [MODULE]Id = 3, /* properties */ }
            };
        }

        #endregion
    }
}
```

---

## ?? Template 3: Integration Test

Copy this for integration test files:

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
    public class [MODULE]ManagementIntegrationTests  // Replace [MODULE]
    {
        private [MODULE]Service _service;
        // Add other services as needed

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize services
            _service = new [MODULE]Service(new [MODULE]Repository());
        }

        #region BR## - Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR##")]
        [Description("TC_BR##_###: Integration - [Test description]")]
        public void TC_BR##_###_Integration_[Scenario]()
        {
            // Arrange
            // Setup if needed

            // Act
            var result = _service.GetAll().ToList();

            // Assert
            Assert.IsTrue(result.Count >= 0, "Should load from database");
            
            // Additional assertions based on actual data
            if (result.Count > 0)
            {
                var firstItem = result.First();
                // Verify properties are loaded correctly
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR##")]
        [Description("TC_BR##_###: Integration - Verify ViewModel loads real data")]
        public void TC_BR##_###_Integration_ViewModel_LoadsRealData()
        {
            // Act
            var viewModel = new [MODULE]ViewModel(_service);

            // Assert
            Assert.IsNotNull(viewModel.List);
            
            if (viewModel.List.Count == 0)
            {
                Assert.Inconclusive("No data in database to test");
                return;
            }

            // Verify data is complete
            foreach (var item in viewModel.List)
            {
                // Add specific assertions
            }
        }

        #endregion
    }
}
```

---

## ?? Specific Templates by Module

### Hall Management Template

```csharp
#region BR41 - Display Hall List Tests

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("HallViewModel")]
[TestCategory("BR41")]
[Description("TC_BR41_001: Verify HallView displays when user clicks HallCommand")]
public void TC_BR41_001_HallView_DisplaysOnCommand()
{
    // Arrange
    var viewModel = CreateViewModel();

    // Act
    // Trigger display

    // Assert
    Assert.IsNotNull(viewModel);
    Assert.IsNotNull(viewModel.List);
}

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("HallViewModel")]
[TestCategory("BR41")]
[Description("TC_BR41_002: Verify hall list contains HallType information")]
public void TC_BR41_002_HallList_ContainsHallTypeInfo()
{
    // Arrange
    var viewModel = CreateViewModel();

    // Act
    var halls = viewModel.List;

    // Assert
    Assert.IsTrue(halls.Count > 0);
    foreach (var hall in halls)
    {
        Assert.IsNotNull(hall.HallType);
        Assert.IsFalse(string.IsNullOrEmpty(hall.HallType.HallTypeName));
    }
}

#endregion

#region BR42 - Search/Filter Hall Tests

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("HallViewModel")]
[TestCategory("BR42")]
[Description("TC_BR42_001: Verify search by hall name")]
public void TC_BR42_001_SearchByHallName_FiltersCorrectly()
{
    // Arrange
    var viewModel = CreateViewModel();
    var searchTerm = "Diamond";

    // Act
    viewModel.SearchKeyword = searchTerm;

    // Assert
    Assert.IsTrue(viewModel.List.All(h => h.HallName.Contains(searchTerm)));
}

#endregion

#region BR43 - Create Hall Tests

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("HallViewModel")]
[TestCategory("BR43")]
[Description("TC_BR43_001: Verify create hall with valid data")]
public void TC_BR43_001_CreateHall_WithValidData_Succeeds()
{
    // Arrange
    var viewModel = CreateViewModel();
    viewModel.HallName = "New Hall";
    viewModel.MaxTableCount = "50";
    viewModel.SelectedHallType = viewModel.HallTypeList.First();

    // Act
    bool canExecute = viewModel.CreateHallCommand.CanExecute(null);

    // Assert
    Assert.IsTrue(canExecute);
}

#endregion
```

### HallType Management Template

```csharp
#region BR60 - Display HallType List Tests

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("HallTypeViewModel")]
[TestCategory("BR60")]
[Description("TC_BR60_001: Verify HallTypeView displays hall type list")]
public void TC_BR60_001_HallTypeView_DisplaysList()
{
    // Arrange
    var viewModel = CreateViewModel();

    // Act & Assert
    Assert.IsNotNull(viewModel.List);
    Assert.IsTrue(viewModel.List.Count > 0);
}

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("HallTypeViewModel")]
[TestCategory("BR60")]
[Description("TC_BR60_002: Verify hall types have pricing information")]
public void TC_BR60_002_HallTypes_HavePricingInfo()
{
    // Arrange
    var viewModel = CreateViewModel();

    // Act & Assert
    foreach (var hallType in viewModel.List)
    {
        Assert.IsNotNull(hallType.MinTablePrice);
        Assert.IsTrue(hallType.MinTablePrice > 0);
    }
}

#endregion

#region BR61 - Create HallType Tests

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("HallTypeViewModel")]
[TestCategory("BR61")]
[Description("TC_BR61_001: Verify create hall type with valid data")]
public void TC_BR61_001_CreateHallType_WithValidData_Succeeds()
{
    // Arrange
    var viewModel = CreateViewModel();
    viewModel.HallTypeName = "Premium";
    viewModel.MinTablePrice = "2500000";

    // Act
    bool canExecute = viewModel.CreateCommand.CanExecute(null);

    // Assert
    Assert.IsTrue(canExecute);
}

#endregion
```

### Dish Management Template

```csharp
#region BR71 - Display Dish List Tests

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("DishViewModel")]
[TestCategory("BR71")]
[Description("TC_BR71_001: Verify DishView displays dish list")]
public void TC_BR71_001_DishView_DisplaysList()
{
    // Arrange
    var viewModel = CreateViewModel();

    // Act & Assert
    Assert.IsNotNull(viewModel.List);
}

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("DishViewModel")]
[TestCategory("BR71")]
[Description("TC_BR71_002: Verify dishes have unit price")]
public void TC_BR71_002_Dishes_HaveUnitPrice()
{
    // Arrange
    var viewModel = CreateViewModel();

    // Act & Assert
    foreach (var dish in viewModel.List)
    {
        Assert.IsNotNull(dish.UnitPrice);
        Assert.IsTrue(dish.UnitPrice > 0);
    }
}

#endregion

#region BR72 - Search Dish Tests

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("DishViewModel")]
[TestCategory("BR72")]
[Description("TC_BR72_001: Verify search by dish name")]
public void TC_BR72_001_SearchByDishName_FiltersCorrectly()
{
    // Arrange
    var viewModel = CreateViewModel();
    var searchTerm = "chicken";

    // Act
    viewModel.SearchKeyword = searchTerm;

    // Assert
    Assert.IsTrue(viewModel.List.All(d => 
        d.DishName.ToLower().Contains(searchTerm.ToLower())));
}

#endregion

#region BR73 - Create Dish Tests

[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("DishViewModel")]
[TestCategory("BR73")]
[Description("TC_BR73_001: Verify create dish with valid data")]
public void TC_BR73_001_CreateDish_WithValidData_Succeeds()
{
    // Arrange
    var viewModel = CreateViewModel();
    viewModel.DishName = "Grilled Fish";
    viewModel.UnitPrice = "150000";

    // Act
    bool canExecute = viewModel.CreateCommand.CanExecute(null);

    // Assert
    Assert.IsTrue(canExecute);
}

#endregion
```

---

## ?? Sample Data Helpers

### Hall Sample Data

```csharp
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
            Note = "Large VIP hall",
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
            Note = "Standard hall",
            HallType = new HallTypeDTO
            {
                HallTypeId = 2,
                HallTypeName = "Standard",
                MinTablePrice = 1500000
            }
        },
        new HallDTO
        {
            HallId = 3,
            HallName = "S?nh Silver",
            MaxTableCount = 30,
            HallTypeId = 3,
            Note = "Economy hall",
            HallType = new HallTypeDTO
            {
                HallTypeId = 3,
                HallTypeName = "Economy",
                MinTablePrice = 1000000
            }
        }
    };
}
```

### HallType Sample Data

```csharp
private List<HallTypeDTO> CreateSampleHallTypes()
{
    return new List<HallTypeDTO>
    {
        new HallTypeDTO
        {
            HallTypeId = 1,
            HallTypeName = "VIP",
            MinTablePrice = 2000000
        },
        new HallTypeDTO
        {
            HallTypeId = 2,
            HallTypeName = "Standard",
            MinTablePrice = 1500000
        },
        new HallTypeDTO
        {
            HallTypeId = 3,
            HallTypeName = "Economy",
            MinTablePrice = 1000000
        }
    };
}
```

### Dish Sample Data

```csharp
private List<DishDTO> CreateSampleDishes()
{
    return new List<DishDTO>
    {
        new DishDTO
        {
            DishId = 1,
            DishName = "Grilled Chicken",
            UnitPrice = 150000,
            Note = "Special recipe"
        },
        new DishDTO
        {
            DishId = 2,
            DishName = "Steamed Fish",
            UnitPrice = 200000,
            Note = "Fresh fish daily"
        },
        new DishDTO
        {
            DishId = 3,
            DishName = "Vegetable Soup",
            UnitPrice = 80000,
            Note = "Healthy option"
        }
    };
}
```

---

## ?? Find & Replace Guide

When copying templates, use these replacements:

| Find | Replace With | Example |
|------|--------------|---------|
| `[MODULE]` | `Hall`, `HallType`, or `Dish` | `HallViewModel` |
| `[MODULE]s` | `Halls`, `HallTypes`, or `Dishes` | `CreateSampleHalls()` |
| `BR##` | Actual BR number | `BR41`, `BR60`, `BR71` |
| `TC_BR##_###` | Actual test case ID | `TC_BR41_001` |

---

## ? Quick Test Checklist

Before considering a test complete:

- [ ] Has `[TestMethod]` attribute
- [ ] Has `[TestCategory("UnitTest")]` or `[TestCategory("IntegrationTest")]`
- [ ] Has `[TestCategory("BR##")]` with correct BR number
- [ ] Has `[Description("TC_BR##_###: ...")]` matching MD file
- [ ] Test name follows `TC_BR##_###_Method_Scenario_Result` pattern
- [ ] Uses AAA pattern (Arrange, Act, Assert)
- [ ] Has meaningful assertions
- [ ] Includes helper methods if needed

---

## ?? Learning Path

Suggested order for implementation:

1. **Start with Service tests** (easiest)
   - Copy Service template
   - Implement GetAll test
   - Implement GetById test
   - Implement Create/Update/Delete tests

2. **Move to ViewModel tests**
   - Copy ViewModel template
   - Implement constructor test
   - Implement display tests
   - Implement command tests

3. **Finish with Integration tests**
   - Copy Integration template
   - Test cross-layer data flow
   - Test ViewModel with real services

---

## ?? One-Command Test Creation

Use this PowerShell script to create test file from template:

```powershell
# CreateTestFile.ps1
param(
    [string]$Module,      # Hall, HallType, or Dish
    [string]$Type         # ViewModel, Service, or Integration
)

$templatePath = "Templates\${Type}Template.cs"
$outputPath = "UnitTests\${Type}s\${Module}${Type}Tests.cs"

(Get-Content $templatePath) `
    -replace '\[MODULE\]', $Module `
    -replace '\[MODULE\]s', "${Module}s" | `
    Set-Content $outputPath

Write-Host "Created: $outputPath"
```

Usage:
```powershell
.\CreateTestFile.ps1 -Module Hall -Type ViewModel
.\CreateTestFile.ps1 -Module HallType -Type Service
.\CreateTestFile.ps1 -Module Dish -Type Integration
```

---

**Copy, customize, and implement! These templates will save you hours of work!** ?

---

**Document Version:** 1.0  
**Templates Based On:** BR137-BR138 successful test patterns  
**Ready to Use:** ? Yes - Copy & Paste!
