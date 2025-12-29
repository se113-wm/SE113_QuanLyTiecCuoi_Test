# Running UI Tests for BR137 & BR138

## Overview

Automated UI tests for **BR137 (Staff Hall Availability)** and **BR138 (Query Available Halls)** using **FlaUI** framework.

---

## Application Flow (Important!)

Understanding the application flow is critical for UI tests:

```
???????????????    Login    ?????????????????    Click OK    ????????????????
? Login       ? ??????????? ? Success       ? ?????????????? ? Main Window  ?
? Window      ?  (Enter     ? MessageBox    ?   (Dismiss)    ? (Home View)  ?
???????????????  creds)     ?????????????????                ????????????????
                                                                    ?
                    ?????????????????????????????????????????????????
                    ?                                               ?
                    ?                                               ?
         Click "WeddingButton"                           Click "BookNowButton"
         in side menu                                    on Home View
                    ?                                               ?
                    ?                                               ?
            ????????????????                               ????????????????
            ? Wedding View ? ??????? Click "AddButton" ??? ? Add Wedding  ?
            ? (List)       ?                               ? Window       ?
            ????????????????                               ????????????????
```

### Key Navigation Paths:

1. **Login ? Main Window**
   - Enter username + password ? Click Login
   - **MessageBox appears ? Click OK**
   - Main Window with Home View displays

2. **Home View ? Wedding View**
   - Click "??t Ti?c Ngay" (BookNowButton) ? Wedding View
   - OR Click "Ti?c c??i" (WeddingButton) in side menu

3. **Wedding View ? Add Wedding Form**
   - Click "Thêm" (AddButton) ? Add Wedding Window opens

---

## Prerequisites

### 1. Software Requirements
- ? Windows 10/11
- ? .NET Framework 4.8
- ? Visual Studio 2019/2022
- ? MSTest Test Framework
- ? FlaUI.UIA3 (installed via NuGet)

### 2. Application Build
```bash
# Build the main application first
cd C:\Users\Dang Phu Thien\source\repos\SE113_QuanLyTiecCuoi_Test
msbuild QuanLyTiecCuoi.csproj /p:Configuration=Debug
```

Ensure `QuanLyTiecCuoi.exe` exists at:
- `bin\Debug\QuanLyTiecCuoi.exe`

### 3. Test Database
- SQL Server running
- Database `QuanLyTiecCuoiEntities` available
- Connection string configured in `App.config`
- Test data populated (halls, shifts, bookings)

### 4. Test User Account
- Username: `Fartiel`
- Password: `admin`
- Role: Staff with booking management permissions

---

## AutomationId Reference

### Login Window
```
UsernameTextBox     - TextBox for username input
PasswordBox         - PasswordBox for password input
LoginButton         - Login button
```

### Main Window (Navigation)
```
MainWindow          - Main application window
NavigationPanel     - Side navigation menu
HomeButton          - Navigate to Home
WeddingButton       - Navigate to Wedding list
HallButton          - Navigate to Hall management
ShiftButton         - Navigate to Shift management
LogoutButton        - Logout
```

### Home View
```
HomePageTitle       - "H? th?ng qu?n lý ti?c c??i"
BookNowButton       - "??t Ti?c Ngay" button
RecentBookingsDataGrid - Recent bookings table
```

### Wedding View
```
WeddingPageTitle    - "Qu?n lý danh sách ti?c c??i"
FilterCard          - Filter panel card
ActionsCard         - Actions panel card
WeddingListCard     - Wedding list card

# Filters
GroomNameFilterComboBox    - Filter by groom name
BrideNameFilterComboBox    - Filter by bride name
HallNameFilterComboBox     - Filter by hall name
WeddingDateFilterPicker    - Filter by wedding date
TableCountFilterComboBox   - Filter by table count
StatusFilterComboBox       - Filter by status
SearchPropertyComboBox     - Search property selector
SearchTextBox              - Search text input
ResetFilterButton          - Reset all filters

# Actions
AddButton           - Open Add Wedding form
DetailButton        - View booking details
DeleteButton        - Delete selected booking

# List
WeddingListView     - ListView showing all weddings
```

### Add Wedding Window
```
DatTiecCuoi         - Window name "??t Ti?c C??i"

# Customer Info
GroomNameTextBox    - Groom name input
BrideNameTextBox    - Bride name input
PhoneTextBox        - Phone number input
WeddingDatePicker   - Wedding date picker
BookingDateTextBox  - Booking date (read-only)

# Hall & Shift
ShiftComboBox       - Shift selection dropdown
HallComboBox        - Hall selection dropdown
DepositTextBox      - Deposit amount input
TableCountTextBox   - Table count input
ReserveTableCountTextBox - Reserve table count

# Menu Section
DishNameTextBox     - Selected dish name
MenuQuantityTextBox - Menu item quantity
MenuNoteTextBox     - Menu item note
SelectDishButton    - Open dish selection dialog
AddMenuButton       - Add menu item
EditMenuButton      - Edit selected menu item
DeleteMenuButton    - Delete selected menu item
ResetMenuButton     - Reset menu fields
MenuListView        - List of menu items
MenuTotalTextBlock  - Menu total amount

# Service Section
ServiceNameTextBox  - Selected service name
ServiceQuantityTextBox - Service quantity
ServiceNoteTextBox  - Service note
SelectServiceButton - Open service selection dialog
AddServiceButton    - Add service item
EditServiceButton   - Edit selected service
DeleteServiceButton - Delete selected service
ResetServiceButton  - Reset service fields
ServiceListView     - List of service items
ServiceTotalTextBlock - Service total amount

# Actions
ConfirmButton       - Confirm and create booking
CancelButton        - Cancel and close window
ResetWeddingButton  - Reset all wedding info
```

---

## Test Files Structure

```
QuanLyTiecCuoi.Tests\
??? UITests\
?   ??? LoginWindowTests.cs              (10 tests - Login functionality)
?   ??? BookingManagementWindowTests.cs  (15+ tests - BR137/BR138)
?   ??? Helpers\
?       ??? UITestHelper.cs              (UI automation utilities)
```

---

## Running Tests

### Option 1: Visual Studio Test Explorer

1. **Open Test Explorer**
   - Menu: `Test` ? `Test Explorer`
   - Or press: `Ctrl + E, T`

2. **Run All UI Tests**
   ```
   Right-click "UITests" folder ? Run
   ```

3. **Run Specific Category**
   - Filter by: `TestCategory=UI`, `TestCategory=BR137`, or `TestCategory=BR138`

### Option 2: Command Line

```bash
cd QuanLyTiecCuoi.Tests

# Run all UI tests
dotnet test --filter "TestCategory=UI"

# Run Login tests only
dotnet test --filter "TestCategory=Login"

# Run BR137 tests only
dotnet test --filter "TestCategory=BR137"

# Run BR138 tests only
dotnet test --filter "TestCategory=BR138"
```

---

## Test Coverage

### LoginWindowTests.cs (10 tests)

| Test Name | Description |
|-----------|-------------|
| Login_WithValidCredentials_ShouldNavigateToMainScreen | Valid login navigates to main |
| Login_WithValidCredentials_ShouldShowSuccessMessageBox | Success shows MessageBox |
| Login_WithInvalidCredentials_ShouldShowErrorMessage | Invalid login shows error |
| Login_WithEmptyUsername_ShouldShowValidationError | Empty username validation |
| Login_WithEmptyPassword_ShouldShowValidationError | Empty password validation |
| Login_WithEmptyUsernameAndPassword_ShouldShowValidationError | Both empty validation |
| LoginWindow_ShouldDisplayAllRequiredElements | UI elements exist |
| LoginWindow_ShouldHaveCorrectTitle | Window title check |
| LoginButton_ShouldBeEnabled | Button enabled check |
| UsernameTextBox_ShouldAcceptInput | TextBox accepts input |

### BookingManagementWindowTests.cs (15+ tests)

| Test ID | Test Name | Category |
|---------|-----------|----------|
| TC_BR137_001 | BookingManagementScreen_DisplaysForAuthorizedUsers | BR137 |
| TC_BR137_002 | CalendarControl_IsAvailableForDateSelection | BR137 |
| TC_BR137_002 | AddWedding_DatePicker_Works | BR137 |
| TC_BR137_003 | ShiftDropdown_IsAvailable | BR137 |
| TC_BR137_003 | ShiftDropdown_ContainsOptions | BR137 |
| TC_BR137_004 | HallDropdown_IsAvailable | BR137 |
| TC_BR137_004 | TableCount_CanBeEntered | BR137 |
| TC_BR138_002 | HallDetails_DisplayAllInfo | BR138 |
| TC_BR138_004 | AddWeddingButton_OpensForm | BR138 |
| TC_BR138_004 | ConfirmButton_ExistsInAddWeddingForm | BR138 |
| TC_BR138_005 | CustomerInfoFields_ExistInAddWeddingForm | BR138 |
| TC_BR138_005 | WeddingList_ShowsBookingInfo | BR138 |
| - | Filter_AllFilterControls_Exist | BR137 |
| - | ResetFilterButton_ClearsFilters | BR137 |
| - | BookNowButton_NavigatesToWedding | BR137 |

---

## UITestHelper Methods

```csharp
var helper = new UITestHelper(_app, _automation);

// Login and navigate
var mainWindow = helper.LoginAndNavigateToMain(loginWindow, "username", "password");

// Navigate to tabs
helper.NavigateToWeddingTab(mainWindow);
helper.NavigateToHomeTab(mainWindow);

// Open/Close Add Wedding form
var addWeddingWindow = helper.OpenAddWeddingForm(mainWindow);
helper.CloseAddWeddingForm(addWeddingWindow);

// MessageBox handling
var messageBox = helper.WaitForMessageBox(mainWindow, TimeSpan.FromSeconds(5));
helper.CloseMessageBox(messageBox);
helper.ConfirmCancel(confirmDialog);

// ComboBox
helper.SelectComboBoxItem(comboBox, "item text");
int count = helper.GetComboBoxItemCount(comboBox);

// TextBox
helper.SetText(textBox, "value");
string text = helper.GetText(element);

// Button
helper.ClickButton(button, maxRetries: 3);
bool ready = helper.IsElementReady(element);

// DataGrid
int rows = helper.GetDataGridRowCount(grid);
helper.SelectDataGridRow(grid, 0);

// Cleanup
helper.CloseAllDialogs();
```

---

## Common Issues & Solutions

### Issue 1: "Login MessageBox not found"

**Cause:** Login success MessageBox not detected

**Solution:** The test now handles the MessageBox that appears after successful login:
```csharp
// After clicking login
var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
if (messageBox != null)
{
    var okButton = messageBox.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
    okButton?.Click();
}
```

### Issue 2: "Cannot find Wedding view elements"

**Cause:** Not navigated to Wedding tab

**Solution:** Navigate to Wedding tab first:
```csharp
// Click WeddingButton in side menu
var weddingButton = _mainWindow.FindFirstDescendant(cf => 
    cf.ByAutomationId("WeddingButton"))?.AsButton();
weddingButton.Click();
Thread.Sleep(1500);
```

### Issue 3: "Add Wedding window not opening"

**Cause:** Need to be on Wedding view first

**Solution:** 
1. Navigate to Wedding tab
2. Click AddButton
3. Wait for window to open

```csharp
// Navigate first
NavigateToWeddingTab();

// Then open form
var addButton = _mainWindow.FindFirstDescendant(cf => 
    cf.ByAutomationId("AddButton"))?.AsButton();
addButton.Click();
Thread.Sleep(2000);

// Find the new window
var addWeddingWindow = desktop.FindFirstChild(cf => 
    cf.ByName("??t Ti?c C??i"))?.AsWindow();
```

### Issue 4: "Cannot close Add Wedding window"

**Cause:** Cancel confirmation dialog not handled

**Solution:**
```csharp
// Click Cancel
var cancelButton = addWeddingWindow.FindFirstDescendant(cf => 
    cf.ByAutomationId("CancelButton"))?.AsButton();
cancelButton?.Click();

// Handle confirmation
Thread.Sleep(500);
var confirmBox = WaitForMessageBox(TimeSpan.FromSeconds(2));
if (confirmBox != null)
{
    var yesButton = confirmBox.FindFirstDescendant(cf => 
        cf.ByName("Yes"))?.AsButton();
    yesButton?.Click();
}
```

---

## Test Execution Flow

```
[TestInitialize]
1. Launch QuanLyTiecCuoi.exe
2. Get Login window
3. Login with "Fartiel" / "admin"
4. Handle success MessageBox (click OK)
5. Get Main window

[TestMethod]
1. Navigate to Wedding tab (if needed)
2. Perform test actions
3. Assert results
4. Cleanup (close dialogs/windows)

[TestCleanup]
1. Close any open MessageBoxes
2. Close any open AddWedding windows
3. Close application
4. Dispose automation
```

---

## Best Practices

### 1. Always handle MessageBoxes
```csharp
var messageBox = WaitForMessageBox(TimeSpan.FromSeconds(5));
if (messageBox != null)
{
    CloseMessageBox(messageBox);
}
```

### 2. Use proper waits
```csharp
// After clicking buttons, wait for UI to update
button.Click();
Thread.Sleep(1500); // Wait for navigation/dialog
```

### 3. Navigate before testing
```csharp
// Ensure you're on correct view before finding elements
Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");
```

### 4. Clean up in finally blocks
```csharp
try
{
    // Test code
}
finally
{
    CloseAddWeddingWindow(addWeddingWindow);
}
```

### 5. Use Assert.Inconclusive for missing prerequisites
```csharp
var element = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SomeId"));
if (element == null)
{
    Assert.Inconclusive("Element not found - check AutomationId");
    return;
}
```

---

## Contact & Support

For issues with UI tests:
1. Check this documentation
2. Verify application flow understanding
3. Check AutomationId reference
4. Review `UITestHelper.cs` for utilities
5. Contact QA team for assistance

---

**Document Version:** 2.0  
**Last Updated:** 2024  
**Status:** ? Production Ready
