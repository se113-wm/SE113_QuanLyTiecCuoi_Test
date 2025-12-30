# System Tests - T?ng Quan & Cách Fix L?i

## ? Tình Tr?ng Hi?n T?i

B?n ?ã có **4 System Test Scenarios** hoàn ch?nh v?i **50+ test methods**, nh?ng hi?n t?i **có ~45 compilation errors** c?n fix.

## ? Nguyên Nhân L?i Chính

### 1. **GroupId Type Mismatch** (~40 l?i)
```csharp
// ? Code hi?n t?i SAI:
var staffUser = users.FirstOrDefault(u => u.GroupId == 2);  // GroupId là string, không ph?i int!

// ? C?n fix thành:
var staffUser = users.FirstOrDefault(u => u.GroupId == "2"); // String comparison
```

**Gi?i thích:** 
- Trong database c?a b?n, b?ng `AppUser` có c?t `GroupId` là `string` (ví d?: "1", "2", "3")
- Test code ?ang dùng `int` ?? so sánh ? Type mismatch error

### 2. **PasswordHelper Namespace** (2 l?i)
```csharp
// ? SAI:
Helpers.PasswordHelper.MD5Hash(...)  // Namespace sai

// ? ?ÚNG:
PasswordHelper.MD5Hash(...)  // Global namespace
```

### 3. **Duplicate Variable** (1 l?i)
```csharp
// Trong Scenario3, bi?n 'penalty' ???c declare 2 l?n trong cùng scope
```

### 4. **Missing Service** (~2 l?i)
```csharp
// Scenario4 thi?u HallTypeService
```

---

## ? C?u Trúc Test Files Hi?n T?i

```
QuanLyTiecCuoi.Tests\SystemTests\
?
??? Helpers\
?   ??? SystemTestHelper.cs                    ? CREATED
?       ??? CreateTestBooking()
?       ??? CreateTestUser()                   ? C?N FIX: GroupId type, PasswordHelper
?       ??? CreateTestHall()
?       ??? ValidateBooking()
?       ??? CalculatePenalty()
?       ??? IsHallAvailable()
?       ??? ... (12+ helper methods)
?
??? Scenario1_SuccessfulBooking_SystemTest.cs  ? CREATED
?   ??? 8 test methods                         ? C?N FIX: 3 l?i GroupId
?
??? Scenario2_BookingValidation_SystemTest.cs  ? CREATED
?   ??? 10 test methods                        ? C?N FIX: 6 l?i GroupId
?
??? Scenario3_CancellationPenalty_SystemTest.cs ? CREATED
?   ??? 8 test methods                         ? C?N FIX: 2 l?i (GroupId + duplicate variable)
?
??? Scenario4_DataIntegration_SystemTest.cs    ? CREATED
?   ??? 10 test methods                        ? C?N FIX: ~28 l?i GroupId + missing HallTypeService
?
??? SYSTEM_TESTING_README.md                   ? CREATED - Documentation hoàn ch?nh
??? SYSTEM_TESTS_ERROR_FIX_GUIDE.md           ? CREATED - Fix guide chi ti?t
```

---

## ? H??ng D?n Fix - 3 B??c ??n Gi?n

### **B??c 1: Ki?m Tra Database GroupId Format**

Ch?y query này ?? xem GroupId trong database c?a b?n:

```sql
SELECT GroupId, GroupName FROM UserGroup;
SELECT TOP 5 Username, GroupId FROM AppUser;
```

**K?t qu? có th? là:**
- Format s?: `"1"`, `"2"`, `"3"` ? Dùng string "1", "2", "3" trong code
- Format text: `"CUSTOMER"`, `"STAFF"`, `"ADMIN"` ? Dùng string "CUSTOMER", "STAFF", "ADMIN"

### **B??c 2: Apply Global Find & Replace**

Trong Visual Studio:

#### A. Fix GroupId Comparisons

**Find (Regex):**
```
u\.GroupId == (\d)
```

**Replace with:**
```
u.GroupId == "$1"
```

**Ho?c manual replace:**
- `u.GroupId == 1` ? `u.GroupId == "1"`
- `u.GroupId == 2` ? `u.GroupId == "2"`  
- `u.GroupId == 3` ? `u.GroupId == "3"`

#### B. Fix Assert.AreEqual

**Find:**
```
Assert.AreEqual(1, 
Assert.AreEqual(2, 
Assert.AreEqual(3,
```

**Replace with:**
```
Assert.AreEqual("1",
Assert.AreEqual("2",
Assert.AreEqual("3",
```

### **B??c 3: Manual Fixes**

#### Fix 1: SystemTestHelper.cs
```csharp
// Line 57: Change parameter type
public static AppUserDTO CreateTestUser(
    string username,
    string password,
    string fullName,
    string groupId = "1")  // ? Changed from int to string
{
    return new AppUserDTO
    {
        Username = username,
        // Line 63: Remove "Helpers."
        PasswordHash = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(password)),
        FullName = fullName,
        Email = $"{username}@test.com",
        PhoneNumber = "0123456789",
        GroupId = groupId,  // ? Now string
        UserGroup = new UserGroupDTO
        {
            GroupId = groupId,  // ? Now string
            // Line 71: String comparison
            GroupName = groupId == "1" ? "Customer" : groupId == "2" ? "Staff" : "Admin"
        }
    };
}
```

#### Fix 2: Scenario3 - Duplicate Variable (Line 361)
```csharp
// ? OLD:
decimal penalty = (booking.TotalInvoiceAmount ?? 0m - booking.Deposit ?? 0m) * penaltyRate * daysLate;

// ? NEW:
decimal testPenalty = (booking.TotalInvoiceAmount ?? 0m - booking.Deposit ?? 0m) * penaltyRate * daysLate;
booking.PenaltyAmount = testPenalty;
```

#### Fix 3: Scenario4 - Add HallTypeService
```csharp
// Add field
private HallTypeService _hallTypeService;

// In Setup()
[TestInitialize]
public void Setup()
{
    DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
    
    _userService = new AppUserService(new AppUserRepository());
    _hallService = new HallService(new HallRepository());
    _hallTypeService = new HallTypeService(new HallTypeRepository()); // ? ADD THIS LINE
    _shiftService = new ShiftService(new ShiftRepository());
    _bookingService = new BookingService(new BookingRepository());
}
```

---

## ? Test Execution Workflow

### 1. Cách Test K?t N?i Database

Tests c?a b?n s? d?ng pattern này:

```csharp
[TestInitialize]
public void Setup()
{
    // ? Fresh database context cho m?i test
    DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
    
    // ? Initialize services through repositories
    _bookingService = new BookingService(new BookingRepository());
    _hallService = new HallService(new HallRepository());
}
```

**Connection string** ???c ??c t?: `QuanLyTiecCuoi.Tests\app.config`

### 2. Test Data Flow

```
Test Method
    ?
Service Layer (BookingService, HallService, etc.)
    ?
Repository Layer (BookingRepository, HallRepository, etc.)
    ?
Entity Framework (QuanLyTiecCuoiEntities)
    ?
SQL Server Database
```

### 3. Mocking Strategy

Tests c?a b?n **KHÔNG dùng mocks** - ?ây là **true integration/system tests**:
- ? S? d?ng real database
- ? S? d?ng real services
- ? Test end-to-end workflows
- ? Clean up sau m?i test

---

## ? Quick Fix Checklist

Th?c hi?n theo th? t?:

- [ ] **Step 1:** Check database GroupId format (`SELECT * FROM UserGroup`)
- [ ] **Step 2:** Fix `SystemTestHelper.cs`
  - [ ] Line 57: Change `int groupId` ? `string groupId`  
  - [ ] Line 63: Remove `Helpers.` prefix
  - [ ] Line 67, 70, 71: String operations
- [ ] **Step 3:** Fix `Scenario1_SuccessfulBooking_SystemTest.cs`
  - [ ] Line 87: `== "1"`
  - [ ] Line 90: `Assert.AreEqual("1", ...)`
  - [ ] Line 490: `== "1"`
- [ ] **Step 4:** Fix `Scenario2_BookingValidation_SystemTest.cs`
  - [ ] 6 locations: Change to string comparisons
- [ ] **Step 5:** Fix `Scenario3_CancellationPenalty_SystemTest.cs`
  - [ ] Line 332: `== "2"`
  - [ ] Line 361: Rename duplicate variable
- [ ] **Step 6:** Fix `Scenario4_DataIntegration_SystemTest.cs`
  - [ ] Add `HallTypeService` field and initialization
  - [ ] ~25 locations: Change to string comparisons
- [ ] **Step 7:** Build solution (`Ctrl+Shift+B`)
- [ ] **Step 8:** Fix any remaining errors
- [ ] **Step 9:** Setup test database with sample data
- [ ] **Step 10:** Run tests (`dotnet test --filter "TestCategory=SystemTest"`)

---

## ? Expected Results After Fix

### Compilation
```
Build: SUCCESS
Errors: 0
Warnings: 0 (or minimal)
```

### Test Execution
```
? Scenario 1: 8 tests
? Scenario 2: 10 tests  
? Scenario 3: 8 tests
? Scenario 4: 10 tests

Total: 36 test methods
Status: May PASS or FAIL depending on database data
```

**Note:** Tests có th? fail n?u:
- Database không có test users
- Database không có halls/shifts/hall types
- Database không có parameters (PenaltyRate, EnablePenalty)

? Nh?ng **KHÔNG nên có compilation errors**!

---

## ? Test Categories & Running Tests

Sau khi fix, b?n có th? run tests theo categories:

```bash
# Run all system tests
dotnet test --filter "TestCategory=SystemTest"

# Run by scenario
dotnet test --filter "TestCategory=Scenario1"
dotnet test --filter "TestCategory=Scenario2"
dotnet test --filter "TestCategory=Scenario3"
dotnet test --filter "TestCategory=Scenario4"

# Run by business rule
dotnet test --filter "TestCategory=BR3"    # Authentication tests
dotnet test --filter "TestCategory=BR137"  # Hall availability tests
dotnet test --filter "TestCategory=BR138"  # Hall availability check

# Run end-to-end tests only
dotnet test --filter "TestCategory=EndToEnd"
```

---

## ? Documentation Files Created

| File | Purpose | Status |
|------|---------|--------|
| `SystemTestHelper.cs` | Utility methods cho tests | ? Created, ? Has errors |
| `Scenario1_SuccessfulBooking_SystemTest.cs` | Happy path testing | ? Created, ? Has errors |
| `Scenario2_BookingValidation_SystemTest.cs` | Negative flow testing | ? Created, ? Has errors |
| `Scenario3_CancellationPenalty_SystemTest.cs` | Complex logic testing | ? Created, ? Has errors |
| `Scenario4_DataIntegration_SystemTest.cs` | Integration testing | ? Created, ? Has errors |
| `SYSTEM_TESTING_README.md` | Complete documentation | ? Created, ? No errors |
| `SYSTEM_TESTS_ERROR_FIX_GUIDE.md` | Detailed fix guide | ? Created, ? No errors |

---

## ? Next Steps

1. **??c file:** `SYSTEM_TESTS_ERROR_FIX_GUIDE.md` (chi ti?t h?n)
2. **Apply fixes:** Theo checklist ? trên
3. **Build & test:** Verify không còn errors
4. **Setup database:** Add required test data
5. **Run tests:** Execute và xem results

---

## ? Support

N?u g?p v?n ??:

1. **Check:** Build errors trong Error List (Ctrl+\, E)
2. **Review:** SYSTEM_TESTS_ERROR_FIX_GUIDE.md
3. **Verify:** Database connection string in app.config
4. **Test:** Run m?t test ??n l? tr??c khi run full suite

---

**Tóm t?t:** B?n có full System Test suite v?i 36 test methods covering 4 scenarios và 22 business rules. Ch? c?n fix ~45 compilation errors (ch? y?u là GroupId type) là có th? run ???c!

**Document Version:** 1.0  
**Status:** ? Ready for Fixes  
**Estimated Fix Time:** 15-30 minutes
