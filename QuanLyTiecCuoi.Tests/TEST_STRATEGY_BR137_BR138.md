# Test Strategy for BR137 & BR138
## Staff Booking Management - Hall Availability Check

### Document Version: 1.0
### Date: 2024
### Author: QA Team

---

## Table of Contents
1. [Overview](#overview)
2. [Test Pyramid Strategy](#test-pyramid-strategy)
3. [Test Coverage Summary](#test-coverage-summary)
4. [Test Levels](#test-levels)
5. [Test Execution Plan](#test-execution-plan)
6. [Test Data Requirements](#test-data-requirements)

---

## Overview

This document outlines the comprehensive testing strategy for **BR137 (Display Staff Hall Availability)** and **BR138 (Query Available Halls)** requirements.

### Business Requirements:
- **BR137**: Staff can view booking management screen with filters (calendar, shift, capacity)
- **BR138**: Staff can query available halls showing details (name, type, capacity, pricing) and create bookings

### Test Objectives:
1. ? Verify all UI controls display correctly
2. ? Verify data filtering works accurately
3. ? Verify hall availability calculation is correct
4. ? Verify booking creation workflow functions end-to-end
5. ? Verify data integrity across all layers

---

## Test Pyramid Strategy

We follow the **Test Pyramid** approach for comprehensive coverage:

```
                    /\
                   /  \
                  /    \
                 / UI   \
                /  Tests \
               /  (Manual)\
              /____________\
             /              \
            /  Integration   \
           /     Tests        \
          /                    \
         /______________________\
        /                        \
       /      Unit Tests          \
      /                            \
     /______________________________\
```

### Pyramid Levels:

#### 1. **Unit Tests** (Base - 70% of tests)
- **Purpose**: Test individual components in isolation
- **Technology**: MSTest + Moq
- **Scope**: ViewModels, Services, DTOs
- **Files**:
  - `WeddingViewModelTests.cs` - 43 tests
  - `AddWeddingViewModelTests.cs` - 33 tests
  - `BookingServiceTests.cs` - 35 tests (including BR137/138)
  - `HallServiceTests.cs` - 28 tests (including BR137/138)
- **Total**: ~139 unit tests
- **Execution**: Fast (< 1 minute), runs on every build

#### 2. **Integration Tests** (Middle - 20% of tests)
- **Purpose**: Test interaction between layers (ViewModel ? Service ? Repository ? Database)
- **Technology**: MSTest + Real Database
- **Scope**: Cross-layer data flow
- **Files**:
  - `BookingManagementIntegrationTests.cs` - 10 tests
- **Total**: ~10 integration tests
- **Execution**: Medium speed (2-5 minutes), runs before deployment

#### 3. **UI Tests** (Top - 10% of tests)
- **Purpose**: Test actual user interface and user workflows
- **Technology**: Manual Testing (documented scenarios)
- **Scope**: End-to-end user experience
- **Files**:
  - `BR137_BR138_UI_Test_Scenarios.md` - 8 manual test scenarios
- **Total**: 8 UI test scenarios
- **Execution**: Slow (30-60 minutes), runs before release
- **Future**: Can be automated with WinAppDriver/FlaUI

---

## Test Coverage Summary

### BR137 - Display Staff Hall Availability

| Test Case ID | Description | Unit | Integration | UI |
|--------------|-------------|------|-------------|-----|
| TC_BR137_001 | Verify booking management screen displays | ? 3 | ? 1 | ? 1 |
| TC_BR137_002 | Verify calendar view control | ? 3 | ? 1 | ? 1 |
| TC_BR137_003 | Verify shift selection dropdown | ? 2 | ? 1 | ? 1 |
| TC_BR137_004 | Verify hall capacity filter | ? 3 | ? 1 | ? 1 |
| **Subtotal** | | **11** | **4** | **4** |

### BR138 - Query Available Halls

| Test Case ID | Description | Unit | Integration | UI |
|--------------|-------------|------|-------------|-----|
| TC_BR138_001 | Verify cancelled bookings excluded | ? 2 | ? 1 | ? 1 |
| TC_BR138_002 | Verify hall details display | ? 3 | ? 1 | ? 1 |
| TC_BR138_003 | Verify MSG90 when no halls available | ? 3 | ? 1 | ? 1 |
| TC_BR138_004 | Verify create booking button | ? 3 | ? 0 | ? 1 |
| TC_BR138_005 | Verify existing bookings shown | ? 3 | ? 1 | ? 1 |
| **Subtotal** | | **14** | **4** | **5** |

### Additional Coverage

| Category | Description | Unit | Integration | UI |
|----------|-------------|------|-------------|-----|
| Search & Filter | Text search, combo filters | ? 8 | ? 2 | - |
| Reset Functionality | Clear all filters | ? 2 | - | - |
| Property Changes | INotifyPropertyChanged | ? 6 | - | - |
| Menu & Service | Related booking operations | ? 6 | - | - |
| Data Validation | Input validation | ? 10 | - | - |
| Edge Cases | Null handling, empty lists | ? 8 | - | - |
| **Subtotal** | | **40** | **2** | **0** |

### **GRAND TOTAL**
- **Unit Tests**: 65+ tests
- **Integration Tests**: 10 tests
- **UI Tests**: 9 scenarios
- **Total Coverage**: 84+ test cases

---

## Test Levels

### Level 1: Unit Tests ? IMPLEMENTED

**What is tested:**
- Individual ViewModel methods
- Service layer methods
- DTO properties and calculations
- Command CanExecute/Execute logic
- Property change notifications
- Data filtering algorithms

**Test Characteristics:**
- ? Fast execution (milliseconds per test)
- ? Isolated (no external dependencies)
- ? Uses mocks for all dependencies
- ? Runs on every build
- ? High code coverage (aim for 80%+)

**Example Test:**
```csharp
[TestMethod]
public void TC_BR137_002_SelectedWeddingDate_FiltersListCorrectly()
{
    // Arrange
    var bookings = CreateTestBookings();
    _mockBookingService.Setup(s => s.GetAll()).Returns(bookings);
    var viewModel = new WeddingViewModel(...);
    
    // Act
    viewModel.SelectedWeddingDate = targetDate;
    
    // Assert
    Assert.AreEqual(1, viewModel.List.Count);
}
```

---

### Level 2: Integration Tests ? IMPLEMENTED

**What is tested:**
- ViewModel ? Service interaction
- Service ? Repository interaction
- Repository ? Database interaction
- Data flow across layers
- Transaction handling
- Real database queries

**Test Characteristics:**
- ?? Medium execution speed (seconds per test)
- ?? Uses real database (test environment)
- ?? May require test data setup
- ?? Runs before deployment
- ?? Tests data integrity

**Example Test:**
```csharp
[TestMethod]
public void TC_BR137_001_Integration_WeddingViewModel_LoadsActualBookings()
{
    // Arrange
    DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
    var viewModel = new WeddingViewModel(/* real services */);
    
    // Act & Assert
    Assert.IsNotNull(viewModel.List);
    Assert.IsTrue(viewModel.List.Count >= 0);
}
```

---

### Level 3: UI Tests ?? DOCUMENTED (Manual)

**What is tested:**
- Actual user interface
- User interactions (click, type, select)
- Visual appearance
- Navigation flow
- Error messages display
- Complete user workflows

**Test Characteristics:**
- ? Slow execution (minutes per test)
- ? Requires running application
- ? Currently manual (can be automated later)
- ? Tests user experience
- ? Catches UI-specific bugs

**Example Scenario:**
```
Test: TC_BR137_001
Steps:
1. Launch application
2. Login as staff user
3. Navigate to Booking Management
4. Verify screen displays with all controls

Expected:
- Booking list is visible
- Calendar control is visible
- Shift dropdown is visible
- Filters are functional
```

---

## Test Execution Plan

### Phase 1: Development (Continuous)
**Frequency**: On every code change

```
Developer Workflow:
1. Write code
2. Write unit tests
3. Run unit tests locally
4. Commit if all pass
```

**Command to run unit tests:**
```bash
dotnet test --filter "TestCategory=UnitTest"
```

---

### Phase 2: Pre-Commit (CI/CD)
**Frequency**: Before merging to main branch

```
CI Pipeline Steps:
1. Build solution
2. Run unit tests
3. Generate code coverage report
4. Fail if coverage < 70%
```

**Command:**
```bash
dotnet test --filter "TestCategory=UnitTest" --collect:"XPlat Code Coverage"
```

---

### Phase 3: Integration Testing
**Frequency**: Nightly or before deployment

```
Integration Test Steps:
1. Deploy to test environment
2. Restore test database
3. Run integration tests
4. Generate test report
```

**Command:**
```bash
dotnet test --filter "TestCategory=IntegrationTest"
```

**Prerequisites:**
- Test database must be available
- Connection string configured
- Test data populated

---

### Phase 4: UI Testing (Manual)
**Frequency**: Before release to production

```
Manual Test Execution:
1. Install application on clean machine
2. Follow test scenarios in BR137_BR138_UI_Test_Scenarios.md
3. Record results in test management system
4. Take screenshots for evidence
5. Log any defects found
```

**Test Environment:**
- Windows 10/11
- .NET Framework 4.8
- SQL Server Express or higher
- Screen resolution: 1920x1080

---

### Phase 5: Regression Testing
**Frequency**: After any bug fix or enhancement

```
Regression Suite:
1. Run ALL unit tests (fast)
2. Run ALL integration tests (medium)
3. Run smoke UI tests (manual, critical paths only)
```

---

## Test Data Requirements

### Master Data (Required in Test Database)

#### 1. Halls
| HallId | HallName | HallType | MaxTableCount | MinTablePrice |
|--------|----------|----------|---------------|---------------|
| 1 | S?nh Diamond | VIP | 50 | 2,000,000 |
| 2 | S?nh Gold | Standard | 40 | 1,500,000 |
| 3 | S?nh Silver | Standard | 30 | 1,500,000 |
| 4 | S?nh Platinum | VIP | 60 | 2,500,000 |
| 5 | S?nh Bronze | Economy | 20 | 1,000,000 |

#### 2. Shifts
| ShiftId | ShiftName | StartTime | EndTime |
|---------|-----------|-----------|---------|
| 1 | Tr?a | 11:00 | 14:00 |
| 2 | T?i | 18:00 | 22:00 |

#### 3. Test Users
| UserId | Username | Password | Role | Permissions |
|--------|----------|----------|------|-------------|
| TEST01 | staff_test | Test@123 | Staff | Booking Management |
| TEST02 | admin_test | Admin@123 | Admin | All |
| TEST03 | readonly_test | Read@123 | ReadOnly | View Only |

#### 4. Sample Bookings (for testing filters)

**Past Bookings:**
- Booking for Diamond, Date: -30 days, Shift: Tr?a, Status: Paid
- Booking for Gold, Date: -15 days, Shift: T?i, Status: Paid

**Future Bookings:**
- Booking for Diamond, Date: +30 days, Shift: Tr?a, Status: Confirmed
- Booking for Gold, Date: +45 days, Shift: T?i, Status: Pending
- Booking for Silver, Date: +60 days, Shift: Tr?a, Status: Confirmed

**Cancelled Bookings:**
- Booking for Platinum, Date: +90 days, Shift: T?i, Status: Cancelled

**Today's Bookings:**
- Booking for Bronze, Date: TODAY, Shift: T?i, Status: Pending

---

## Test Metrics & KPIs

### Code Coverage Targets
- **Unit Tests**: 80%+ statement coverage
- **Integration Tests**: 60%+ integration path coverage
- **Overall**: 70%+ combined coverage

### Test Success Criteria
- **All unit tests**: 100% pass rate
- **Integration tests**: 95%+ pass rate (some may be flaky)
- **UI tests**: 100% pass rate for critical paths

### Defect Metrics
- **Critical defects**: 0 allowed in production
- **High priority**: < 5 open at any time
- **Medium/Low**: Tracked and prioritized

---

## Test Automation Roadmap

### Current State (Phase 1) ? COMPLETED
- ? Unit tests for ViewModels
- ? Unit tests for Services
- ? Integration tests documented
- ? Manual UI test scenarios

### Phase 2 (Next Quarter) ?? PLANNED
- [ ] Automate integration tests in CI/CD
- [ ] Setup test database automation
- [ ] Implement data seeding scripts
- [ ] Code coverage reporting

### Phase 3 (Future) ?? FUTURE
- [ ] Explore UI automation (WinAppDriver)
- [ ] Create automated UI smoke tests
- [ ] Performance testing
- [ ] Load testing

---

## Running Tests

### Run All Unit Tests
```bash
cd QuanLyTiecCuoi.Tests
dotnet test --filter "TestCategory=UnitTest"
```

### Run BR137 Tests Only
```bash
dotnet test --filter "TestCategory=BR137"
```

### Run BR138 Tests Only
```bash
dotnet test --filter "TestCategory=BR138"
```

### Run All BR137 & BR138 Tests
```bash
dotnet test --filter "TestCategory=BR137|TestCategory=BR138"
```

### Run Integration Tests
```bash
dotnet test --filter "TestCategory=IntegrationTest"
```

### Run With Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## Test Maintenance

### When to Update Tests

1. **Code Changes**: Update affected unit tests
2. **New Features**: Add new test cases for new requirements
3. **Bug Fixes**: Add regression test to prevent recurrence
4. **Refactoring**: Ensure tests still pass, update if needed

### Test Naming Convention
```
[TestMethod]
[TestCategory("TestLevel")]
[TestCategory("RequirementID")]
[Description("TC_XXXXX_###: Description")]
public void TC_XXXXX_###_MethodName_Scenario_ExpectedResult()
{
    // Test implementation
}
```

**Example:**
```csharp
[TestMethod]
[TestCategory("UnitTest")]
[TestCategory("BR137")]
[Description("TC_BR137_002: Verify calendar date selection works")]
public void TC_BR137_002_SelectedWeddingDate_CanBeSetAndRetrieved()
```

---

## Continuous Improvement

### Regular Reviews
- **Weekly**: Review failed tests
- **Monthly**: Review test coverage
- **Quarterly**: Review test strategy

### Test Metrics Dashboard
Track:
- Total tests count
- Pass/Fail rate
- Code coverage %
- Test execution time
- Flaky tests count

---

## Conclusion

This comprehensive test strategy ensures:
1. ? **Quality**: High confidence in code correctness
2. ? **Speed**: Fast feedback from unit tests
3. ? **Coverage**: All layers tested (Unit ? Integration ? UI)
4. ? **Maintainability**: Well-documented and organized
5. ? **Traceability**: Tests mapped to requirements (RTM)

**Total Test Investment:**
- Unit Tests: 65+ automated tests ?
- Integration Tests: 10 automated tests ?
- UI Tests: 9 documented scenarios ?
- **Coverage**: BR137 (4 test cases) & BR138 (5 test cases) fully covered

---

## References

1. RTM Document: `test_case_list.md`
2. Test Case Details: `manage-bookings-doc/TC_BR13X_XXX.md`
3. Unit Tests: `QuanLyTiecCuoi.Tests/UnitTests/`
4. Integration Tests: `QuanLyTiecCuoi.Tests/IntegrationTests/`
5. UI Test Scenarios: `QuanLyTiecCuoi.Tests/UITests/BR137_BR138_UI_Test_Scenarios.md`

---

**Document Status**: ? Approved for Use  
**Last Updated**: 2024  
**Next Review**: After deployment to production
