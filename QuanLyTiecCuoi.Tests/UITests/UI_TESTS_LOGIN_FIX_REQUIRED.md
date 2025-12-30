# ?? UI TESTS LOGIN FIX REQUIRED
## Critical Issue: MessageBox Handling & Window Re-acquisition

### ?? Date: 2024
### ?? Status: ?? FIX NEEDED

---

## ?? PROBLEM IDENTIFIED

### Issue Description
After login success, the application shows a **MessageBox with "Login successful!"**. The user must click **OK** to dismiss it before the Main Window appears.

**Current Problem:**
- 6 new UI test files **DO NOT HANDLE** this correctly
- They call `CloseAnyMessageBox()` but **DO NOT re-acquire** the Main Window
- This causes tests to **FAIL** because they're using a reference to the old Login Window

### Affected Files (6 files)
1. ? `HallManagementWindowTests.cs`
2. ? `HallTypeManagementWindowTests.cs`
3. ? `DishManagementWindowTests.cs`
4. ? `ShiftManagementWindowTests.cs`
5. ? `ServiceManagementWindowTests.cs`
6. ? `ReportManagementWindowTests.cs`

### Working Reference
? `BookingManagementWindowTests.cs` - Already handles this correctly!

---

## ?? Root Cause Analysis

### Login Flow in Application

```
1. User enters credentials
2. Click "Login" button
3. ? Login successful
4. ?? MessageBox appears: "Login successful!"
5. User clicks "OK"
6. ?? Window transition: Login Window closes ? Main Window opens
7. ? Main Window displayed
```

### What Tests Currently Do (WRONG ?)

```csharp
[TestInitialize]
public void Setup()
{
    _automation = new UIA3Automation();
    _app = Application.Launch(appPath);
    
    var loginWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10));
    PerformLogin(loginWindow);  // ? PROBLEM HERE!
    
    // _mainWindow still references the OLD Login Window!
    _mainWindow = _app.GetMainWindow(...); // ? Might get wrong window
    
    NavigateToXXXManagement(); // ? Uses old window reference
}

private void PerformLogin(Window loginWindow)
{
    // ... enter credentials ...
    btnLogin.Click();
    Thread.Sleep(2000);
    
    CloseAnyMessageBox(); // ? Closes MessageBox
    // ? MISSING: Re-acquire Main Window!
}
```

### What Tests Should Do (CORRECT ?)

```csharp
[TestInitialize]
public void Setup()
{
    _automation = new UIA3Automation();
    _app = Application.Launch(appPath);
    
    var loginWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(10));
    
    // ? Use helper that returns NEW Main Window reference
    _mainWindow = PerformLoginAndReacquireMainWindow(loginWindow);
    
    if (_mainWindow == null)
    {
        Assert.Inconclusive("Cannot login");
        return;
    }
    
    NavigateToXXXManagement(); // ? Uses correct window reference
}

private Window PerformLoginAndReacquireMainWindow(Window loginWindow)
{
    // 1. Enter credentials
    txtUsername.Text = "Fartiel";
    pwdPassword.Text = "admin";
    btnLogin.Click();
    Thread.Sleep(2000);

    // 2. Handle MessageBox
    var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
    if (messageBox != null)
    {
        CloseMessageBox(messageBox);
        Thread.Sleep(1000);
    }

    // 3. ?? CRITICAL: Re-acquire Main Window
    Thread.Sleep(2000); // Wait for transition
    
    var desktop = _automation.GetDesktop();
    var appWindows = desktop.FindAllChildren(cf => cf.ByProcessId(_app.ProcessId));
    
    foreach (var window in appWindows)
    {
        // Verify this is Main Window by finding WeddingButton
        var weddingButton = window.FindFirstDescendant(cf => 
            cf.ByAutomationId("WeddingButton"));
        
        if (weddingButton != null)
        {
            return window.AsWindow(); // ? Return NEW window reference
        }
    }
    
    return null;
}
```

---

## ? SOLUTION

### 1. Created UITestHelper with Correct Login Method

File: `QuanLyTiecCuoi.Tests\UITests\Helpers\UITestHelper.cs`

Added method:
```csharp
public Window PerformLoginAndReacquireMainWindow(
    Window loginWindow, 
    string username, 
    string password)
{
    // ? Handles credentials
    // ? Handles MessageBox
    // ? Re-acquires Main Window
    // ? Returns NEW window reference
}
```

### 2. Fix Required for Each Test File

**Pattern to Apply:**

```csharp
// BEFORE (WRONG ?)
[TestInitialize]
public void Setup()
{
    _automation = new UIA3Automation();
    _app = Application.Launch(appPath);
    
    var loginWindow = _app.GetMainWindow(...);
    PerformLogin(loginWindow);
    
    Thread.Sleep(2000);
    _mainWindow = _app.GetMainWindow(...);
    
    NavigateToXXXManagement();
}

// AFTER (CORRECT ?)
[TestInitialize]
public void Setup()
{
    _automation = new UIA3Automation();
    _app = Application.Launch(appPath);
    
    var loginWindow = _app.GetMainWindow(...);
    
    // ? Use helper and get NEW window reference
    _mainWindow = PerformLoginAndReacquireMainWindow(loginWindow);
    
    if (_mainWindow == null)
    {
        Assert.Inconclusive("Cannot login");
        return;
    }
    
    NavigateToXXXManagement();
}

// ? Add this method
private Window PerformLoginAndReacquireMainWindow(Window loginWindow)
{
    var helper = new UITestHelper(_app, _automation);
    return helper.PerformLoginAndReacquireMainWindow(
        loginWindow, "Fartiel", "admin");
}

// ? Remove old PerformLogin method
```

---

## ?? Files Needing Updates

### 1. HallManagementWindowTests.cs
- Lines to change: ~35-65 (Setup + PerformLogin)
- Add: PerformLoginAndReacquireMainWindow method
- Remove: Old PerformLogin method

### 2. HallTypeManagementWindowTests.cs
- Lines to change: ~35-65 (Setup + PerformLogin)
- Add: PerformLoginAndReacquireMainWindow method
- Remove: Old PerformLogin method

### 3. DishManagementWindowTests.cs
- Lines to change: ~35-65 (Setup + PerformLogin)
- Add: PerformLoginAndReacquireMainWindow method
- Remove: Old PerformLogin method

### 4. ShiftManagementWindowTests.cs
- Lines to change: ~35-65 (Setup + PerformLogin)
- Add: PerformLoginAndReacquireMainWindow method
- Remove: Old PerformLogin method

### 5. ServiceManagementWindowTests.cs
- Lines to change: ~35-65 (Setup + PerformLogin)
- Add: PerformLoginAndReacquireMainWindow method
- Remove: Old PerformLogin method

### 6. ReportManagementWindowTests.cs
- Lines to change: ~35-65 (Setup + PerformLogin)
- Add: PerformLoginAndReacquireMainWindow method
- Remove: Old PerformLogin method

---

## ?? Expected Results After Fix

### Before Fix ?
```
Login ? MessageBox appears ? Click OK ? Window transition
Tests use old window reference ? Tests FAIL
```

### After Fix ?
```
Login ? MessageBox appears ? Click OK ? Window transition
Tests re-acquire new window ? Tests PASS
```

---

## ?? Implementation Steps

### Option 1: Manual Fix (Recommended for Learning)
1. Open each test file
2. Locate `Setup()` method
3. Replace `PerformLogin` logic with helper call
4. Add `PerformLoginAndReacquireMainWindow` wrapper method
5. Test each file individually

### Option 2: Automated Fix (Faster)
1. Run bulk edit script
2. Verify all 6 files updated
3. Run build
4. Run UI tests to verify

---

## ?? Testing the Fix

### Run Individual Test
```bash
# Test one file after fix
dotnet test --filter "TestCategory=HallManagement"
```

### Run All UI Tests
```bash
# Test all after all files fixed
dotnet test --filter "TestCategory=UI"
```

### Expected Output
```
? All tests should find Main Window successfully
? Navigation to management pages should work
? No "Element not found" errors
? No "Window is closed" errors
```

---

## ?? Impact Assessment

### Test Count Affected
- **6 test files**
- **~118 UI tests** potentially affected
- **High priority** fix needed

### Severity
- ?? **CRITICAL** - Tests cannot run without this fix
- Tests will fail at Setup phase
- Cannot verify UI functionality

### Time to Fix
- **Per file:** ~5-10 minutes
- **Total:** ~30-60 minutes for all 6 files
- **Build + verify:** ~10 minutes

**Total estimated time: 40-70 minutes**

---

## ? Success Criteria

After applying fix, all UI tests should:

1. ? **Login successfully**
2. ? **Handle MessageBox correctly**
3. ? **Re-acquire Main Window**
4. ? **Navigate to management pages**
5. ? **Find UI elements**
6. ? **Complete test scenarios**

---

## ?? References

### Working Example
- `BookingManagementWindowTests.cs` (lines 35-125)
- Shows correct login + window re-acquisition

### Helper Class
- `UITestHelper.cs` 
- Method: `PerformLoginAndReacquireMainWindow`

### FlaUI Documentation
- Window lifecycle: https://github.com/FlaUI/FlaUI/wiki
- MessageBox handling: Modal windows

---

**Status:** ?? **FIX REQUIRED**  
**Priority:** ?? **CRITICAL**  
**Assigned:** Team/Developer  
**Due:** ASAP

---

*"The best time to fix a bug was when it was created. The second best time is now."*

---

**Date Created:** 2024  
**Last Updated:** 2024  
**Version:** 1.0
