# System Tests - Test Execution Guide & Troubleshooting

## ?? Current Status

**Build Status:** ? SUCCESS  
**Compilation:** ? 0 errors  
**Logic:** ? Fixed (IsHallAvailable corrected)  
**Test Execution:** ?? NEED DATABASE SETUP

---

## ?? Prerequisites Checklist

### Before Running Tests

- [ ] SQL Server installed and running
- [ ] Database `WeddingManagement` created
- [ ] Connection string configured in `app.config`
- [ ] Test data populated
- [ ] UserGroups with correct GroupId values

---

## ??? Database Setup

### Step 1: Check SQL Server Connection

```sql
-- Test connection
SELECT @@VERSION;
SELECT DB_NAME();
```

**Connection String in `app.config`:**
```
data source=localhost,1433
initial catalog=WeddingManagement
user id=SA
password=Minh1901!
```

### Step 2: Create Database

Run the full SQL script: `QuanLyTiecCuoi.sql`

Or create manually:

```sql
-- Drop existing
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'WeddingManagement')
BEGIN
    ALTER DATABASE WeddingManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE WeddingManagement;
END
GO

-- Create new
CREATE DATABASE WeddingManagement;
GO

USE WeddingManagement;
GO
```

### Step 3: Create Tables

```sql
-- PARAMETER TABLE (Critical for tests!)
CREATE TABLE Parameter (
    ParameterName NVARCHAR(100) PRIMARY KEY,
    Value DECIMAL(5,2)
);

-- USER GROUP TABLE
CREATE TABLE UserGroup (
    GroupId VARCHAR(10) PRIMARY KEY,
    GroupName NVARCHAR(100) UNIQUE NOT NULL
);

-- APP USER TABLE
CREATE TABLE AppUser (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(256) NOT NULL,
    FullName NVARCHAR(100),
    Email VARCHAR(100),
    PhoneNumber VARCHAR(15),
    GroupId VARCHAR(10),
    FOREIGN KEY (GroupId) REFERENCES UserGroup(GroupId)
);

-- HALL TYPE TABLE
CREATE TABLE HallType (
    HallTypeId INT IDENTITY(1,1) PRIMARY KEY,
    HallTypeName NVARCHAR(40) UNIQUE NOT NULL,
    MinTablePrice MONEY
);

-- HALL TABLE
CREATE TABLE Hall (
    HallId INT IDENTITY(1,1) PRIMARY KEY,
    HallTypeId INT,
    HallName NVARCHAR(40) UNIQUE NOT NULL,
    MaxTableCount INT,
    Note NVARCHAR(100),
    FOREIGN KEY (HallTypeId) REFERENCES HallType(HallTypeId)
);

-- SHIFT TABLE
CREATE TABLE Shift (
    ShiftId INT IDENTITY(1,1) PRIMARY KEY,
    ShiftName NVARCHAR(40) UNIQUE NOT NULL,
    StartTime TIME,
    EndTime TIME
);

-- BOOKING TABLE
CREATE TABLE Booking (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    GroomName NVARCHAR(40),
    BrideName NVARCHAR(40),
    Phone VARCHAR(10),
    BookingDate SMALLDATETIME,
    WeddingDate SMALLDATETIME,
    ShiftId INT,
    HallId INT,
    Deposit MONEY,
    TableCount INT,
    ReserveTableCount INT,
    PaymentDate SMALLDATETIME,
    TablePrice MONEY,
    TotalTableAmount MONEY,
    TotalServiceAmount MONEY,
    TotalInvoiceAmount MONEY,
    RemainingAmount MONEY,
    AdditionalCost MONEY,
    PenaltyAmount MONEY,
    FOREIGN KEY (ShiftId) REFERENCES Shift(ShiftId),
    FOREIGN KEY (HallId) REFERENCES Hall(HallId)
);
```

### Step 4: Insert Test Data

**CRITICAL: Parameters (Required for Scenario3!)**

```sql
INSERT INTO Parameter (ParameterName, Value) VALUES
('EnablePenalty', 1),          -- 1 = enabled
('PenaltyRate', 0.05),         -- 5% per day
('MinDepositRate', 0.30),      -- 30% minimum deposit
('MinReserveTableRate', 0.85); -- 85% minimum booking
```

**User Groups (MUST use these exact GroupId values!)**

```sql
INSERT INTO UserGroup (GroupId, GroupName) VALUES
('1', 'Customer'),  -- GroupId = "1" (string!)
('2', 'Staff'),     -- GroupId = "2" (string!)
('3', 'Admin');     -- GroupId = "3" (string!)
```

**Test Users**

```sql
-- Password: admin123 (MD5 hashed)
INSERT INTO AppUser (Username, PasswordHash, FullName, GroupId) VALUES
('customer_test', '0192023a7bbd73250516f069df18b500', 'Test Customer', '1'),
('staff_test', '0192023a7bbd73250516f069df18b500', 'Test Staff', '2'),
('admin_test', '0192023a7bbd73250516f069df18b500', 'Test Admin', '3');
```

**Halls & Hall Types**

```sql
INSERT INTO HallType (HallTypeName, MinTablePrice) VALUES
('Class A', 500000),
('Class B', 400000),
('Class C', 300000);

INSERT INTO Hall (HallName, HallTypeId, MaxTableCount, Note) VALUES
('Ruby Hall', 1, 50, 'Large hall'),
('Sapphire Hall', 1, 45, 'Medium hall'),
('Diamond Hall', 2, 40, 'Small hall'),
('Gold Hall', 2, 35, 'Intimate hall'),
('Silver Hall', 3, 30, 'Budget hall');
```

**Shifts**

```sql
INSERT INTO Shift (ShiftName, StartTime, EndTime) VALUES
('Morning', '08:00:00', '12:00:00'),
('Afternoon', '13:00:00', '17:00:00'),
('Evening', '18:00:00', '22:00:00');
```

---

## ?? Running Tests

### Method 1: Visual Studio Test Explorer

1. Open **Test > Test Explorer** (or `Ctrl+E, T`)
2. **Build Solution** (`Ctrl+Shift+B`)
3. Click **Run All** or select specific tests
4. View results in Test Explorer window

### Method 2: Command Line (dotnet test)

```bash
# All system tests
dotnet test --filter "TestCategory=SystemTest"

# By scenario
dotnet test --filter "TestCategory=Scenario1"
dotnet test --filter "TestCategory=Scenario2"
dotnet test --filter "TestCategory=Scenario3"
dotnet test --filter "TestCategory=Scenario4"

# By business rule
dotnet test --filter "TestCategory=BR137"
dotnet test --filter "TestCategory=BR138"
dotnet test --filter "TestCategory=BR123"

# End-to-end tests only
dotnet test --filter "TestCategory=EndToEnd"

# Verbose output
dotnet test --filter "TestCategory=SystemTest" --logger "console;verbosity=detailed"

# With test result file
dotnet test --filter "TestCategory=SystemTest" --logger "trx;LogFileName=test_results.trx"
```

### Method 3: MSTest Command Line

```bash
# Run with MSTest
vstest.console QuanLyTiecCuoi.Tests\bin\Debug\QuanLyTiecCuoi.Tests.dll /Tests:Scenario1_SuccessfulBooking_SystemTest

# Run specific test
vstest.console QuanLyTiecCuoi.Tests\bin\Debug\QuanLyTiecCuoi.Tests.dll /Tests:Step1_TC_BR123_001_CustomerLogin_Successful
```

---

## ?? Expected Test Results

### Test Suite Structure

```
System Tests (36 tests total)
?
?? Scenario 1: Successful Booking (8 tests)
?  ?? Step1_TC_BR123_001_CustomerLogin_Successful
?  ?? Step2_TC_BR123_002_CheckAvailability_DisplaysAvailableHalls
?  ?? Step3_TC_BR124_001_SelectHall_BookingFormOpensWithHallPreselected
?  ?? Step4_TC_BR125_005_EnterInfo_ValidCapacity_NoValidationErrors
?  ?? Step5_TC_BR126_002_SubmitBooking_ValidationPassed_BookingCreated
?  ?? Step6_TC_BR152_001_ViewInvoice_DisplaysCorrectAmounts
?  ?? Step7_TC_BR155_001_PayDeposit_MinimumDepositAccepted
?  ?? Scenario1_EndToEnd_SuccessfulBooking_CompleteWorkflow
?
?? Scenario 2: Booking Validation (10 tests)
?  ?? Step1_TC_BR123_002_StaffLogin_AccessGranted
?  ?? Step2_TC_BR138_004_OpenBookingForm_CreateButtonAvailable
?  ?? Step3_TC_BR140_003_SelectInvalidDate_ErrorMSG102_HallNotAvailable
?  ?? Step4_TC_BR139_002_SelectValidDate_ErrorClears
?  ?? Step5_TC_BR91_001_ExceedCapacity_ErrorMSG91_ExceededCapacity
?  ?? Step6_TC_BR140_004_CorrectData_NoValidationErrors
?  ?? Step7_TC_BR141_002_SaveBooking_StatusConfirmed
?  ?? Scenario2_EndToEnd_BookingValidation_CompleteWorkflow
?  ?? Additional_BoundaryConditions_MaxCapacityHandling
?  ?? Additional_PhoneValidation_CorrectFormat
?
?? Scenario 3: Cancellation & Penalty (8 tests)
?  ?? Step1_TC_BR132_001_RequestCancel_ConfirmationPopupDisplays
?  ?? Step2_TC_BR134_002_VerifyPenalty_CalculatedByPenaltyRate
?  ?? Step3_TC_BR136_001_ConfirmCancel_StatusUpdatedToCancelled
?  ?? Step4_TC_BR159_005_StaffVerification_InvoiceShowsPenalty
?  ?? Step5_TC_BR164_001_ProcessPenaltyPayment_InvoiceBalanceUpdated
?  ?? Scenario3_EndToEnd_CancellationPenalty_CompleteWorkflow
?  ?? Additional_PenaltyCalculation_DifferentDayRanges
?  ?? Additional_PenaltyCalculation_DisabledWhenParameterIsZero
?
?? Scenario 4: Data Integration (10 tests)
   ?? Step1_TC_BR123_003_AdminLogin_FullAccessGranted
   ?? Step2_TC_BR46_001_CreateHall_SavedToDatabase
   ?? Step3_TC_BR4_002_SwitchToStaffRole_RoleChangeSuccessful
   ?? Step4_TC_BR137_001_SearchHall_NewHallAppearsImmediately
   ?? Step5_TC_BR141_001_CreateBooking_WithNewHall_Allowed
   ?? Step6_TC_BR141_002_SaveBooking_LinkedToNewHallId
   ?? Scenario4_EndToEnd_DataIntegration_CompleteWorkflow
   ?? Additional_MultiUserAccess_ConcurrentCreation
   ?? Additional_UpdatePropagation_CrossServiceConsistency
   ?? Additional_ReferentialIntegrity_ForeignKeyConstraints
```

---

## ?? Common Issues & Solutions

### Issue 1: Database Connection Failed

**Symptoms:**
- Tests skip with "Inconclusive" result
- Error: "Cannot open database"
- Error: "Login failed"

**Solutions:**
```bash
# Check SQL Server is running
services.msc  # Look for SQL Server service

# Test connection
sqlcmd -S localhost,1433 -U SA -P Minh1901!
```

**Fix connection string:**
```xml
<connectionStrings>
  <add name="QuanLyTiecCuoiEntities" 
       connectionString="...data source=localhost,1433;..." />
</connectionStrings>
```

### Issue 2: Parameter Not Found

**Symptoms:**
- Scenario3 tests fail
- Error: "PenaltyRate parameter should exist"
- NullReferenceException in penalty calculation

**Solution:**
```sql
-- Verify parameters exist
SELECT * FROM Parameter;

-- Insert if missing
INSERT INTO Parameter (ParameterName, Value) VALUES
('PenaltyRate', 0.05),
('EnablePenalty', 1);
```

### Issue 3: No Test Users Found

**Symptoms:**
- Tests fail with "Test user should exist"
- Customer/Staff/Admin user not found

**Solution:**
```sql
-- Check users
SELECT * FROM AppUser;
SELECT * FROM UserGroup;

-- Verify GroupId is STRING not INT!
-- WRONG: GroupId = 1 (int)
-- RIGHT: GroupId = '1' (string)

-- Insert test users with correct GroupId
INSERT INTO AppUser (Username, PasswordHash, FullName, GroupId) VALUES
('customer_test', '0192023a7bbd73250516f069df18b500', 'Test Customer', '1'),
('staff_test', '0192023a7bbd73250516f069df18b500', 'Test Staff', '2');
```

### Issue 4: No Halls in Database

**Symptoms:**
- Tests skip with "Need halls for testing"
- "System should have halls" assertion fails

**Solution:**
```sql
-- Check halls
SELECT * FROM Hall;
SELECT * FROM HallType;

-- Insert test halls
INSERT INTO HallType (HallTypeName, MinTablePrice) VALUES ('Test Type', 500000);
INSERT INTO Hall (HallName, HallTypeId, MaxTableCount) VALUES ('Test Hall', 1, 50);
```

### Issue 5: Tests Pass But Create Duplicate Bookings

**Symptoms:**
- Multiple bookings for same date/hall/shift
- Double-booking allowed

**Root Cause:**
- Old `IsHallAvailable` logic was wrong
- Fixed in latest version (removed PaymentDate check)

**Verify Fix:**
```csharp
// Correct logic (no PaymentDate check):
return !existingBookings.Any(b =>
    b.WeddingDate.HasValue &&
    b.WeddingDate.Value.Date == weddingDate.Date &&
    b.HallId == hallId &&
    b.ShiftId == shiftId);
```

### Issue 6: Tests Don't Run (Build Fails)

**Solution:**
```bash
# Clean and rebuild
dotnet clean
dotnet build

# Check for errors
dotnet build > build_output.txt 2>&1
```

### Issue 7: Test Results Not Visible

**Solution in Visual Studio:**
```
1. Test > Test Explorer
2. If empty, rebuild solution (Ctrl+Shift+B)
3. Click refresh icon in Test Explorer
4. Group tests by: Class / Trait / Project
```

**Solution in Command Line:**
```bash
# Use detailed logger
dotnet test --logger "console;verbosity=detailed"

# Output to file
dotnet test > test_results.txt 2>&1
```

---

## ?? Test Execution Checklist

### Before Running Tests

- [ ] SQL Server running
- [ ] Database `WeddingManagement` exists
- [ ] Tables created (Parameter, UserGroup, AppUser, Hall, Shift, Booking)
- [ ] Parameters inserted (PenaltyRate, EnablePenalty)
- [ ] UserGroups inserted with STRING GroupId ('1', '2', '3')
- [ ] Test users created
- [ ] At least 5 halls exist
- [ ] At least 2 shifts exist
- [ ] Connection string correct in `app.config`
- [ ] Solution builds successfully

### Running Tests

- [ ] Run Scenario 1 (Customer booking flow)
- [ ] Run Scenario 2 (Staff validation)
- [ ] Run Scenario 3 (Cancellation & penalty)
- [ ] Run Scenario 4 (Data integration)
- [ ] Review test output
- [ ] Check for inconclusive tests
- [ ] Verify database state after tests

### After Tests

- [ ] Review failed tests
- [ ] Check test data in database
- [ ] Clean up test bookings if needed
- [ ] Document any issues found
- [ ] Update test data if required

---

## ?? Success Criteria

Tests are successful when:

? **All 36 tests RUN** (not skipped)  
? **Pass rate ? 80%** (29+ tests pass)  
? **No compilation errors**  
? **No database connection errors**  
? **Business logic validated** (penalty calculation, validation rules)

---

## ?? Sample Test Run Output

**Expected Success:**
```
Test run for D:\CODE\SE113\SE113_QuanLyTiecCuoi_Test\QuanLyTiecCuoi.Tests\bin\Debug\QuanLyTiecCuoi.Tests.dll (.NETFramework,Version=v4.8)
Microsoft (R) Test Execution Command Line Tool Version 17.x.x

Starting test execution, please wait...
A total of 36 test files matched the specified pattern.

Passed   Step1_TC_BR123_001_CustomerLogin_Successful [50 ms]
Passed   Step2_TC_BR123_002_CheckAvailability_DisplaysAvailableHalls [120 ms]
Passed   Step3_TC_BR124_001_SelectHall_BookingFormOpensWithHallPreselected [80 ms]
...

Test Run Successful.
Total tests: 36
     Passed: 34
     Failed: 0
     Skipped: 2
 Total time: 15.2345 Seconds
```

**Expected Inconclusive (Missing Data):**
```
Skipped  Step2_TC_BR134_002_VerifyPenalty_CalculatedByPenaltyRate
         Message: Test inconclusive: PenaltyRate parameter not found
         
Skipped  Step1_TC_BR123_001_CustomerLogin_Successful
         Message: Test inconclusive: Test user not available
```

---

## ?? Quick Fix Commands

**Reset test database:**
```sql
USE master;
DROP DATABASE WeddingManagement;
-- Then run full setup script
```

**Clear test bookings:**
```sql
DELETE FROM Booking WHERE GroomName LIKE 'Test_%' OR GroomName LIKE 'E2E_%' OR GroomName LIKE 'CancelTest_%';
```

**Check critical data:**
```sql
-- Parameters
SELECT * FROM Parameter;

-- Users
SELECT u.UserId, u.Username, u.GroupId, g.GroupName 
FROM AppUser u 
JOIN UserGroup g ON u.GroupId = g.GroupId;

-- Halls
SELECT h.HallId, h.HallName, h.MaxTableCount, ht.HallTypeName, ht.MinTablePrice
FROM Hall h
JOIN HallType ht ON h.HallTypeId = ht.HallTypeId;
```

---

## ?? Need Help?

If tests still fail after following this guide:

1. **Check build output** for compilation errors
2. **Check test output** for specific error messages
3. **Verify database** has all required data
4. **Check connection string** matches your SQL Server
5. **Review test code** for any assumptions about test data

---

**Document Version:** 1.0  
**Last Updated:** 2024  
**Status:** Ready for Test Execution  
**Prerequisites:** Database setup required

**Next Action:** Setup database using SQL scripts above, then run tests!

```bash
# Ready to run!
dotnet test --filter "TestCategory=SystemTest" --logger "console;verbosity=detailed"
```
