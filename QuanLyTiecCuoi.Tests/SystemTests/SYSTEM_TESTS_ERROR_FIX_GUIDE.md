# System Tests - Error Fix Guide

## Overview

?ây là h??ng d?n chi ti?t ?? fix t?t c? các l?i compilation trong System Tests. Các l?i chính:

1. **GroupId là `string`, không ph?i `int`** - Trong database c?a b?n, GroupId ???c ??nh ngh?a là string (ví d?: "CUSTOMER", "STAFF", "ADMIN")
2. **PasswordHelper reference sai namespace** - PasswordHelper n?m ? global namespace, không ph?i trong QuanLyTiecCuoi.Tests.SystemTests.Helpers
3. **Duplicate variable penalty** - Trong Scenario3, có bi?n penalty b? declare 2 l?n

## Errors Summary

```
Total Errors: ~45 errors
- GroupId type mismatch: ~40 errors
- PasswordHelper namespace: 2 errors  
- Duplicate variable: 1 error
- Missing HallTypeService: ~2 errors
```

---

## Fix 1: GroupId Type Issues

### Problem

```csharp
// ? SAI - GroupId là string, không th? so sánh v?i int
var staffUser = users.FirstOrDefault(u => u.GroupId == 2);
Assert.AreEqual(2, DataProvider.Ins.CurrentUser.GroupId);
```

### Solution

D?a vào database c?a b?n, GroupId có 3 giá tr?:
- **"1"** ho?c **"CUSTOMER"** = Customer
- **"2"** ho?c **"STAFF"** = Staff  
- **"3"** ho?c **"ADMIN"** = Admin

```csharp
// ? ?ÚNG - So sánh string
var staffUser = users.FirstOrDefault(u => u.GroupId == "2"); // ho?c "STAFF"
var adminUser = users.FirstOrDefault(u => u.GroupId == "3"); // ho?c "ADMIN"
var customerUser = users.FirstOrDefault(u => u.GroupId == "1"); // ho?c "CUSTOMER"

// Assert v?i string
Assert.AreEqual("2", DataProvider.Ins.CurrentUser.GroupId, "Should be staff");
```

---

## Fix 2: PasswordHelper Reference

### Problem

```csharp
// ? SAI - Namespace incorrect
PasswordHash = Helpers.PasswordHelper.MD5Hash(Helpers.PasswordHelper.Base64Encode(password))
```

### Solution

PasswordHelper n?m ? global namespace (không có namespace khai báo):

```csharp
// ? ?ÚNG - Global namespace
PasswordHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(password))
```

---

## Fix 3: Duplicate Variable 'penalty'

### Problem (Scenario3, line 361)

```csharp
// ? SAI - Variable 'penalty' ?ã ???c declare ? scope bên ngoài
decimal penalty = (booking.TotalInvoiceAmount ?? 0m - booking.Deposit ?? 0m) * penaltyRate * daysLate;
```

### Solution

```csharp
// ? ?ÚNG - ??i tên bi?n ho?c remove duplicate
decimal calculatedPenalty = (booking.TotalInvoiceAmount ?? 0m - booking.Deposit ?? 0m) * penaltyRate * daysLate;
```

---

## Fix 4: Missing HallTypeService

Scenario 4 c?n HallTypeService nh?ng ch?a ???c kh?i t?o.

```csharp
// SystemTests/Scenario4_DataIntegration_SystemTest.cs

[TestInitialize]
public void Setup()
{
    DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
    
    _userService = new AppUserService(new AppUserRepository());
    _hallService = new HallService(new HallRepository());
    _hallTypeService = new HallTypeService(new HallTypeRepository()); // ? ADD THIS
    _shiftService = new ShiftService(new ShiftRepository());
    _bookingService = new BookingService(new BookingRepository());
}
```

---

## Complete Fix Instructions

### Step 1: Fix SystemTestHelper.cs

File: `QuanLyTiecCuoi.Tests\SystemTests\Helpers\SystemTestHelper.cs`

**Changes:**

1. Fix PasswordHelper reference (line 63)
2. Fix GroupId type from `int` to `string` (lines 60-71)

```csharp
/// <summary>
/// Creates a test user DTO with specified role
/// </summary>
public static AppUserDTO CreateTestUser(
    string username,
    string password,
    string fullName,
    string groupId = "1") // ? Changed from int to string
{
    return new AppUserDTO
    {
        Username = username,
        PasswordHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(password)), // ? Removed Helpers.
        FullName = fullName,
        Email = $"{username}@test.com",
        PhoneNumber = "0123456789",
        GroupId = groupId, // ? Now string
        UserGroup = new UserGroupDTO
        {
            GroupId = groupId, // ? Now string
            GroupName = groupId == "1" ? "Customer" : groupId == "2" ? "Staff" : "Admin" // ? String comparison
        }
    };
}
```

---

### Step 2: Fix Scenario1_SuccessfulBooking_SystemTest.cs

**All GroupId comparisons need to use strings:**

```csharp
// Line 87 - Fix GroupId comparison
var testUser = users.FirstOrDefault(u => u.Username == testUsername) 
            ?? users.FirstOrDefault(u => u.GroupId == "1"); // ? String "1"

// Line 90 - Fix Assert
Assert.AreEqual("1", testUser.GroupId, "User should be in Customer group (GroupId = '1')"); // ? String

// Line 490 - Fix GroupId comparison
var testUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "1"); // ? String "1"
```

---

### Step 3: Fix Scenario2_BookingValidation_SystemTest.cs

**Fix all GroupId comparisons (5 locations):**

```csharp
// Line 76, 110, 416, 502 - Fix GroupId comparison
var staffUser = users.FirstOrDefault(u => u.GroupId == "2"); // ? String "2"
var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2"); // ? String "2"

// Line 79, 95 - Fix Assert
Assert.AreEqual("2", staffUser.GroupId, "User should be in Staff group (GroupId = '2')"); // ? String
Assert.AreEqual("2", DataProvider.Ins.CurrentUser.GroupId, "Logged in user should be staff"); // ? String
```

---

### Step 4: Fix Scenario3_CancellationPenalty_SystemTest.cs

**Fix 2 issues:**

1. **GroupId comparison** (line 332)
2. **Duplicate variable 'penalty'** (line 361)

```csharp
// Line 332 - Fix GroupId
var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2"); // ? String "2"

// Line 361 - Fix duplicate variable name
// ? OLD:
decimal penalty = (booking.TotalInvoiceAmount ?? 0m - booking.Deposit ?? 0m) * penaltyRate * daysLate;

// ? NEW: Rename to avoid conflict
decimal testPenalty = (booking.TotalInvoiceAmount ?? 0m - booking.Deposit ?? 0m) * penaltyRate * daysLate;

booking.PenaltyAmount = testPenalty; // ? Use new variable name
_bookingService.Update(booking);
```

---

### Step 5: Fix Scenario4_DataIntegration_SystemTest.cs

**Fix many GroupId comparisons:**

```csharp
// Add HallTypeService field
private HallTypeService _hallTypeService;

// Initialize in Setup
[TestInitialize]
public void Setup()
{
    DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
    
    _userService = new AppUserService(new AppUserRepository());
    _hallService = new HallService(new HallRepository());
    _hallTypeService = new HallTypeService(new HallTypeRepository()); // ? ADD THIS
    _shiftService = new ShiftService(new ShiftRepository());
    _bookingService = new BookingService(new BookingRepository());
}

// Fix all GroupId comparisons:
// Lines: 106, 125, 141, 211, 220, 228, 243, 264, 290, 348, 374, 446, 472, 550, 557, 579, 586, 641, 666, 706, 707, 781, 782

// ? Examples:
var adminUser = users.FirstOrDefault(u => u.GroupId == "3"); // Admin
var staffUser = _userService.GetAll().FirstOrDefault(u => u.GroupId == "2"); // Staff

Assert.AreEqual("3", DataProvider.Ins.CurrentUser.GroupId, "Should be admin");
Assert.AreEqual("2", DataProvider.Ins.CurrentUser.GroupId, "Should be staff");
```

---

## Quick Reference: GroupId Values

Based on your database, GroupId should be:

| Role | GroupId Value (string) | Alternative Values |
|------|------------------------|--------------------|
| Customer | `"1"` | or `"CUSTOMER"` |
| Staff | `"2"` | or `"STAFF"` |
| Admin | `"3"` | or `"ADMIN"` |

**Check your database** to verify which format is used:

```sql
SELECT * FROM UserGroup;
SELECT * FROM AppUser;
```

If your database uses:
- **Numeric strings**: `"1"`, `"2"`, `"3"` ? Use as shown above
- **Text strings**: `"CUSTOMER"`, `"STAFF"`, `"ADMIN"` ? Change all comparisons to use these values

---

## Database Connection

Your tests correctly use:

```csharp
[TestInitialize]
public void Setup()
{
    // ? CORRECT - Fresh database context
    DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
    
    // ? CORRECT - Initialize services with repositories
    _userService = new AppUserService(new AppUserRepository());
    _hallService = new HallService(new HallRepository());
    _bookingService = new BookingService(new BookingRepository());
}
```

**Connection String** is read from `App.config` in test project:

```xml
<!-- QuanLyTiecCuoi.Tests\app.config -->
<connectionStrings>
  <add name="QuanLyTiecCuoiEntities" 
       connectionString="metadata=res://*/Model.Model1.csdl|res://*/Model.Model1.ssdl|res://*/Model.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=YOUR_SERVER;initial catalog=QuanLyTiecCuoi;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

---

## Test Data Requirements

### Required Database Setup

Before running tests, ensure your database has:

#### 1. User Groups
```sql
INSERT INTO UserGroup (GroupId, GroupName) VALUES
('1', 'Customer'),
('2', 'Staff'),
('3', 'Admin');
```

#### 2. Test Users
```sql
-- Password: "password123" (Base64 + MD5 hashed)
INSERT INTO AppUser (Username, PasswordHash, FullName, Email, GroupId) VALUES
('customer_test', 'hashed_password_here', 'Test Customer', 'customer@test.com', '1'),
('staff_test', 'hashed_password_here', 'Test Staff', 'staff@test.com', '2'),
('admin_test', 'hashed_password_here', 'Test Admin', 'admin@test.com', '3');
```

#### 3. Hall Types
```sql
INSERT INTO HallType (HallTypeId, HallTypeName, MinTablePrice) VALUES
(1, 'Class A', 500000),
(2, 'Class B', 400000),
(3, 'Class C', 300000);
```

#### 4. Halls
```sql
INSERT INTO Hall (HallName, HallTypeId, MaxTableCount, Note) VALUES
('Diamond Hall', 1, 50, 'Luxury hall'),
('Gold Hall', 2, 40, 'Premium hall'),
('Silver Hall', 3, 30, 'Standard hall');
```

#### 5. Shifts
```sql
INSERT INTO Shift (ShiftName, StartTime, EndTime) VALUES
('Morning', '08:00:00', '12:00:00'),
('Afternoon', '13:00:00', '17:00:00'),
('Evening', '18:00:00', '22:00:00');
```

#### 6. Parameters
```sql
INSERT INTO Parameter (ParameterName, Value) VALUES
('PenaltyRate', 0.05),    -- 5% per day
('EnablePenalty', 1);      -- Enabled
```

---

## Verification Steps

After fixing all files:

### 1. Build Solution
```powershell
# From Visual Studio
Build ? Rebuild Solution (Ctrl+Shift+B)

# Or from command line
msbuild QuanLyTiecCuoi.sln /t:Rebuild /p:Configuration=Debug
```

### 2. Run Tests
```powershell
# Run all system tests
dotnet test --filter "TestCategory=SystemTest"

# Run specific scenario
dotnet test --filter "TestCategory=Scenario1"
```

### 3. Check Test Explorer
```
Visual Studio ? Test ? Test Explorer (Ctrl+E, T)
Filter: TestCategory=SystemTest
Click "Run All"
```

---

## Common Test Errors After Fix

### Error: "Test user should exist in database"
**Fix:** Add test users to your database (see Test Data Requirements above)

### Error: "System should have halls in database"
**Fix:** Add halls and hall types to your database

### Error: "PenaltyRate parameter should exist"
**Fix:** Add parameters to your database

### Error: "Connection string not found"
**Fix:** Check `App.config` in test project has correct connection string

---

## Summary of Changes

| File | Lines to Fix | Change Type |
|------|--------------|-------------|
| SystemTestHelper.cs | 60, 63, 67, 70, 71 | GroupId `int` ? `string`, PasswordHelper namespace |
| Scenario1_SuccessfulBooking_SystemTest.cs | 87, 90, 490 | GroupId comparisons |
| Scenario2_BookingValidation_SystemTest.cs | 76, 79, 95, 110, 416, 502 | GroupId comparisons |
| Scenario3_CancellationPenalty_SystemTest.cs | 332, 361 | GroupId + duplicate variable |
| Scenario4_DataIntegration_SystemTest.cs | ~25 locations | GroupId comparisons + HallTypeService |

---

## Next Steps

1. ? Apply all fixes from this guide
2. ? Verify database has required test data
3. ? Build solution to check for errors
4. ? Run system tests
5. ? Review test results

**Expected Result:** All 36 system test methods should compile successfully. Tests may fail if database doesn't have required data, but there should be **ZERO compilation errors**.

---

**Document Version:** 1.0  
**Last Updated:** 2024  
**Status:** ? Ready for Implementation

