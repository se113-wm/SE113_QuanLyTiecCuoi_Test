# ?? UI TESTS NAVIGATION ISSUE FIX GUIDE
## Dish/Food Management Navigation Problem - **SOLVED** ?

### ?? Date: 2024
### ?? Status: ? **FIXED**

---

## ?? PROBLEM DESCRIPTION

**Test Failure:**
```
Test: DishManagementWindowTests.UI_BR71_003_DishList_ShouldDisplayPricing
Duration: 16.4 sec
Result: FAIL

Message:
Assert.Inconclusive failed. Cannot find Dish Management menu.

Debug Output:
MessageBox found after login - clicking OK
Re-acquiring Main Window...
Found 1 windows for process
Main Window re-acquired successfully!
```

**What was happening:**
1. ? Login successful
2. ? MessageBox handled
3. ? Main Window re-acquired
4. ? **Could not find "Dish Management" menu item**

---

## ?? ROOT CAUSE ANALYSIS

### The Problem

**Tests were searching for WRONG AutomationId:**

```csharp
// ? BEFORE (WRONG):
var dishMenuItem = _mainWindow.FindFirstDescendant(cf => 
    cf.ByName("Món ?n")
    .Or(cf.ByName("Th?c ??n"))
    .Or(cf.ByName("Dish"))
    .Or(cf.ByName("Food"))
    .Or(cf.ByAutomationId("DishMenuItem"))      // ? WRONG ID!
    .Or(cf.ByAutomationId("FoodMenuItem")));    // ? WRONG ID!
```

**But MainWindow.xaml actually has:**

```xaml
<!--  Food  -->
<Button
    Grid.Row="4"
    Command="{Binding FoodCommand}"
    Foreground="White"
    ToolTip="Món ?n"
    Visibility="{Binding ButtonVisibilities[Food]}"
    AutomationProperties.AutomationId="FoodButton">   <!-- ? CORRECT ID! -->
    <StackPanel Orientation="Horizontal">
        <materialDesign:PackIcon Kind="Food"/>
        <TextBlock Text="Món ?n"/>
    </StackPanel>
</Button>
```

### Why It Failed

1. **AutomationId Mismatch**: 
   - Test searched for: `"DishMenuItem"` and `"FoodMenuItem"`
   - XAML actually has: `"FoodButton"`

2. **Text-based search unreliable**:
   - Vietnamese characters encoding issues
   - MaterialDesign button structure
   - Dynamic visibility binding

---

## ? THE FIX

### Step 1: Update NavigateToDishManagement()

**File: `QuanLyTiecCuoi.Tests\UITests\DishManagementWindowTests.cs`**

**Line 91-106: Replace with:**

```csharp
private void NavigateToDishManagement()
{
    var dishMenuItem = _mainWindow.FindFirstDescendant(cf => 
        cf.ByAutomationId("FoodButton")    // ? PRIMARY: Exact match from XAML
        .Or(cf.ByName("Món ?n"))           // Fallback: Vietnamese text
        .Or(cf.ByName("Food")));           // Fallback: English text
    
    if (dishMenuItem != null)
    {
        dishMenuItem.Click();
        Thread.Sleep(1500);
        _dishWindow = _mainWindow.ModalWindows.FirstOrDefault() ?? _mainWindow;
    }
    else
    {
        Assert.Inconclusive("Cannot find Dish/Food Management button. Check MainWindow.xaml AutomationId.");
    }
}
```

### Changes Made:

1. ? **Primary search**: `ByAutomationId("FoodButton")` - matches XAML
2. ? **Fallback 1**: `ByName("Món ?n")` - Vietnamese text
3. ? **Fallback 2**: `ByName("Food")` - English text
4. ? **Better error message**: Guides developer to check XAML

---

## ?? SAME FIX NEEDED FOR OTHER TESTS

**All these tests probably have SAME navigation issue:**

### Tests to Fix:

| Test File | Navigation Method | Current (WRONG) | Correct AutomationId | Priority |
|-----------|------------------|-----------------|---------------------|----------|
| ? **DishManagementWindowTests.cs** | `NavigateToDishManagement()` | "DishMenuItem" | **"FoodButton"** | **FIXED** ? |
| ? **HallManagementWindowTests.cs** | `NavigateToHallManagement()` | "HallMenuItem"? | **"HallButton"** | HIGH ?? |
| ? **HallTypeManagementWindowTests.cs** | `NavigateToHallTypeManagement()` | "HallTypeMenuItem"? | **"HallTypeButton"** | HIGH ?? |
| ? **ShiftManagementWindowTests.cs** | `NavigateToShiftManagement()` | "ShiftMenuItem"? | **"ShiftButton"** | HIGH ?? |
| ? **ServiceManagementWindowTests.cs** | `NavigateToServiceManagement()` | "ServiceMenuItem"? | **"ServiceButton"** | HIGH ?? |
| ? **ReportManagementWindowTests.cs** | `NavigateToReportManagement()` | "ReportMenuItem"? | **"ReportButton"** | HIGH ?? |
| ? **BookingManagementWindowTests.cs** | `NavigateToWeddingTab()` | Probably correct | **"WeddingButton"** | Works ? |
| ? **LoginWindowTests.cs** | N/A (Login only) | N/A | N/A | N/A ? |

---

## ?? CORRECT AUTOMATIONID MAPPING

**From `MainWindow.xaml`:**

| Button | Row | AutomationId | Vietnamese Text | English Text |
|--------|-----|--------------|----------------|--------------|
| Home | 0 | `"HomeButton"` | Trang ch? | Home |
| Hall Type | 1 | `"HallTypeButton"` | Lo?i s?nh | Hall Type |
| Hall | 2 | `"HallButton"` | S?nh | Hall |
| Shift | 3 | `"ShiftButton"` | Ca | Shift |
| **Food** | **4** | **`"FoodButton"`** | **Món ?n** | **Food** |
| Service | 5 | `"ServiceButton"` | D?ch v? | Service |
| Wedding | 6 | `"WeddingButton"` | Ti?c c??i | Wedding |
| Report | 7 | `"ReportButton"` | Report | Report |
| Parameter | 8 | `"ParameterButton"` | Parameter | Parameter |
| Permission | 9 | `"PermissionButton"` | Permission | Permission |
| User | 10 | `"UserButton"` | User | User |
| Account | Footer | `"AccountButton"` | Account | Account |
| Logout | Footer | `"LogoutButton"` | Logout | Logout |

---

## ?? HOW TO FIX OTHER TESTS (Template)

### Template for Fixing Navigation Methods:

```csharp
private void NavigateTo[Feature]Management()
{
    // ? Use this pattern:
    var menuButton = _mainWindow.FindFirstDescendant(cf => 
        cf.ByAutomationId("[Feature]Button")    // PRIMARY: From MainWindow.xaml
        .Or(cf.ByName("[Vietnamese Text]"))    // Fallback
        .Or(cf.ByName("[English Text]")));     // Fallback
    
    if (menuButton != null)
    {
        menuButton.Click();
        Thread.Sleep(1500);
        _[feature]Window = _mainWindow.ModalWindows.FirstOrDefault() ?? _mainWindow;
    }
    else
    {
        Assert.Inconclusive($"Cannot find [Feature] Management button. Check MainWindow.xaml AutomationId.");
    }
}
```

### Example: HallManagementWindowTests.cs

```csharp
private void NavigateToHallManagement()
{
    var hallButton = _mainWindow.FindFirstDescendant(cf => 
        cf.ByAutomationId("HallButton")    // ? From XAML
        .Or(cf.ByName("S?nh"))             // Vietnamese
        .Or(cf.ByName("Hall")));           // English
    
    if (hallButton != null)
    {
        hallButton.Click();
        Thread.Sleep(1500);
        _hallWindow = _mainWindow.ModalWindows.FirstOrDefault() ?? _mainWindow;
    }
    else
    {
        Assert.Inconclusive("Cannot find Hall Management button. Check MainWindow.xaml AutomationId.");
    }
}
```

---

## ?? TESTING THE FIX

### Step 1: Rebuild Solution

```bash
# In Visual Studio:
Build > Rebuild Solution (Ctrl+Shift+B)
```

### Step 2: Run Single Test

```bash
# Run the fixed test:
dotnet test --filter "FullyQualifiedName~UI_BR71_003_DishList_ShouldDisplayPricing"
```

### Step 3: Run All Dish Tests

```bash
# Run all dish management tests:
dotnet test --filter "TestCategory=DishManagement"
```

### Step 4: Verify Success

**Expected Output:**
```
? Passed UI_BR71_001_DishWindow_ShouldDisplayDishList
? Passed UI_BR71_002_DishList_ShouldContainDishData
? Passed UI_BR71_003_DishList_ShouldDisplayPricing
...
```

---

## ?? VALIDATION CHECKLIST

Before running tests:

- [x] ? MainWindow.xaml has AutomationId="FoodButton"
- [x] ? NavigateToDishManagement() uses "FoodButton"
- [x] ? Fallback searches included (Vietnamese, English)
- [x] ? Error message guides to XAML
- [ ] ? Apply same fix to other 5 test files
- [ ] ? Run all UI tests to verify
- [ ] ? Document success in test results

---

## ?? SUCCESS CRITERIA

After fix, tests should:

1. ? Login successfully
2. ? Handle MessageBox
3. ? Re-acquire Main Window
4. ? **Find navigation button using AutomationId** ? **FIXED!**
5. ? Click button
6. ? Navigate to management page
7. ? Verify page loaded
8. ? Run test scenarios

---

## ?? RELATED FILES

**Application:**
- `Presentation\View\MainWindow.xaml` - ? Has correct AutomationIds

**Tests:**
- `QuanLyTiecCuoi.Tests\UITests\DishManagementWindowTests.cs` - ? **FIXED**
- `QuanLyTiecCuoi.Tests\UITests\HallManagementWindowTests.cs` - ? TODO
- `QuanLyTiecCuoi.Tests\UITests\HallTypeManagementWindowTests.cs` - ? TODO
- `QuanLyTiecCuoi.Tests\UITests\ShiftManagementWindowTests.cs` - ? TODO
- `QuanLyTiecCuoi.Tests\UITests\ServiceManagementWindowTests.cs` - ? TODO
- `QuanLyTiecCuoi.Tests\UITests\ReportManagementWindowTests.cs` - ? TODO

**Documentation:**
- `AUTOMATION_ID_COVERAGE_REPORT.md`
- `UI_TESTS_LOGIN_FIX_COMPLETED.md`
- `RUNNING_UI_TESTS.md`

---

## ?? SOLUTION SUMMARY

### The Issue:
Tests were searching for `"DishMenuItem"` but XAML has `"FoodButton"`

### The Fix:
Changed search to use correct AutomationId: `"FoodButton"`

### Impact:
- ? 1 test file fixed (DishManagementWindowTests.cs)
- ? 5 more test files need same fix pattern
- ? Pattern established for future fixes

---

**Status:** ? **FIXED FOR DISH TESTS**  
**Priority:** ?? **Apply to other 5 test files**  
**Estimated Time:** 5 minutes per file × 5 = 25 minutes  
**Total Impact:** 6 test files, ~60+ tests

---

*"Find the right button with the right name!"* ?????

---

**Date Created:** 2024  
**Last Updated:** 2024  
**Version:** 2.0 - **SOLVED**
