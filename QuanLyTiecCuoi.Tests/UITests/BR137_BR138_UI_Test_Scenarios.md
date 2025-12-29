# UI Test Scenarios for BR137 & BR138
# Booking Management - Staff Hall Availability Check

## Prerequisites for ALL UI Tests
1. WMS application installed and can be launched
2. Test database populated with sample data:
   - At least 3 halls (Diamond, Gold, Silver)
   - At least 2 shifts (Tr?a, T?i)
   - Sample bookings for various dates
3. Test user account with booking management permissions
4. Test credentials: Username = "staff_test", Password = "Test@123"

---

## TC_BR137_001: Verify Booking Management Screen Displays for Authorized Users

### Test Type: UI Test - Manual
### Priority: Critical
### Test Category: BR137

### Test Steps:
1. **Launch Application**
   - Open QuanLyTiecCuoi.exe
   - **Expected**: Login window displays

2. **Login as Authorized Staff**
   - Enter username: "staff_test"
   - Enter password: "Test@123"
   - Click "??ng nh?p" button
   - **Expected**: 
     - Login successful
     - Main dashboard displays
     - Navigation menu shows available functions

3. **Navigate to Booking Management**
   - Click on "Qu?n lý ??t ti?c" menu item
   - **Expected**:
     - Booking management screen displays
     - Screen shows booking list
     - Filter controls are visible
     - All UI elements load properly

### Pass Criteria:
- ? Authorized user can access booking management
- ? All UI controls are visible and functional
- ? No errors or crashes occur

### Fail Scenarios:
- ? Login fails with valid credentials
- ? Booking management screen not accessible
- ? UI elements missing or not displayed

---

## TC_BR137_002: Verify Calendar View Control for Date Selection

### Test Type: UI Test - Manual
### Priority: High
### Test Category: BR137

### Test Steps:
1. **Navigate to Booking Management Screen**
   - Follow TC_BR137_001 steps 1-3
   - **Expected**: Booking management screen is displayed

2. **Locate Calendar Control**
   - Find the date filter/calendar control on the screen
   - **Expected**: Calendar/DatePicker control is visible

3. **Click on Date Picker**
   - Click on the calendar icon or date field
   - **Expected**: 
     - Calendar popup/dropdown opens
     - Current month is displayed
     - Dates are selectable

4. **Select a Future Date**
   - Click on a date 30 days from today
   - **Expected**:
     - Date is selected
     - Selected date displays in the date field
     - Calendar closes

5. **Verify Date Filter Applied**
   - Check if booking list updates
   - **Expected**: List shows only bookings for selected date

### Pass Criteria:
- ? Calendar control is accessible
- ? Past dates are disabled/grayed out
- ? Future dates are selectable
- ? Selected date filters booking list

### Test Data to Verify:
- Past dates: Should be disabled
- Today: Should be selectable
- Future dates: Should be selectable

---

## TC_BR137_003: Verify Shift Selection Dropdown Available

### Test Type: UI Test - Manual
### Priority: High
### Test Category: BR137

### Test Steps:
1. **Navigate to Booking Management Screen**
   - Follow TC_BR137_001 steps 1-3

2. **Locate Shift Dropdown**
   - Find the shift selection dropdown/combobox
   - **Expected**: Shift dropdown is visible with label "Ca"

3. **Click on Shift Dropdown**
   - Click on the dropdown control
   - **Expected**: Dropdown expands showing options

4. **View Available Shifts**
   - Check the shift options displayed
   - **Expected**:
     - "Tr?a" option is available
     - "T?i" option is available
     - All shifts from database are shown

5. **Select a Shift**
   - Click on "Tr?a" shift
   - **Expected**:
     - Shift is selected
     - Dropdown closes
     - Booking list filters by selected shift

### Pass Criteria:
- ? Shift dropdown displays all available shifts
- ? Shift names are in Vietnamese (Tr?a, T?i)
- ? Selection filters booking list correctly

---

## TC_BR137_004: Verify Hall Capacity Filter Option Available

### Test Type: UI Test - Manual
### Priority: Medium
### Test Category: BR137

### Test Steps:
1. **Navigate to Booking Management Screen**
   - Follow TC_BR137_001 steps 1-3

2. **Locate Capacity Filter**
   - Find the table count/capacity filter control
   - **Expected**: Filter control is visible (TextBox or NumericUpDown)

3. **Enter Minimum Table Count**
   - Type "20" into the capacity filter field
   - **Expected**: Value is accepted

4. **Apply Filter**
   - Press Enter or click apply/search button
   - **Expected**:
     - Filter is applied
     - Booking list updates
     - Only bookings with ?20 tables are shown

5. **Verify Filtered Results**
   - Check each booking in the list
   - **Expected**: All visible bookings have TableCount ? 20

### Pass Criteria:
- ? Capacity filter accepts numeric input
- ? Filter correctly filters by table count
- ? Results match filter criteria

---

## TC_BR138_001: Verify Query Excludes Cancelled Bookings from Availability

### Test Type: UI Test - Manual
### Priority: Critical
### Test Category: BR138

### Test Steps:
1. **Setup Test Data**
   - Ensure database has:
     - Hall A with a CANCELLED booking on Date X, Shift Tr?a
     - No other bookings for Hall A on Date X, Shift Tr?a

2. **Navigate to Hall Availability Check**
   - Open "Ki?m tra s?nh tr?ng" function
   - **Expected**: Availability check screen displays

3. **Select Date X**
   - Use calendar to select the test date
   - **Expected**: Date is selected

4. **Select Shift "Tr?a"**
   - Choose "Tr?a" from shift dropdown
   - **Expected**: Shift is selected

5. **Click Search/Query Button**
   - Click "Tìm ki?m" or "Ki?m tra" button
   - **Expected**: 
     - Query executes
     - Results display

6. **Check Hall A Availability**
   - Look for Hall A in the results
   - **Expected**: 
     - Hall A shows as AVAILABLE
     - Cancelled booking is NOT blocking the hall
     - "T?o phi?u ??t" button is enabled for Hall A

### Pass Criteria:
- ? Cancelled bookings don't block hall availability
- ? Hall with cancelled booking shows as available
- ? Can create new booking for hall with cancelled booking

---

## TC_BR138_002: Verify Hall Details Display (Name, Type, Capacity, Pricing)

### Test Type: UI Test - Manual
### Priority: High
### Test Category: BR138

### Test Steps:
1. **Navigate to Hall Availability Check**
   - Open "Ki?m tra s?nh tr?ng" function

2. **Select Any Date and Shift**
   - Select a date with available halls
   - Select any shift

3. **Execute Query**
   - Click search/query button
   - **Expected**: Available halls list displays

4. **Verify Hall Details Columns**
   - Check the displayed information for each hall
   - **Expected Columns**:
     - **Tên s?nh** (Hall Name): e.g., "S?nh Diamond"
     - **Lo?i s?nh** (Hall Type): e.g., "VIP"
     - **S? bàn t?i ?a** (Max Table Count): e.g., "50"
     - **??n giá t?i thi?u** (Min Table Price): e.g., "2,000,000 ?"

5. **Verify Data Accuracy**
   - Compare with known hall data
   - **Expected**: All fields match database values

### Pass Criteria:
- ? All 4 required fields are displayed for each hall
- ? Hall names are correct
- ? Hall types are correct
- ? Capacity numbers are accurate
- ? Prices are formatted correctly in VND

### Test Data:
| Hall Name | Hall Type | Max Tables | Min Price |
|-----------|-----------|------------|-----------|
| S?nh Diamond | VIP | 50 | 2,000,000 |
| S?nh Gold | Standard | 40 | 1,500,000 |
| S?nh Silver | Standard | 30 | 1,500,000 |

---

## TC_BR138_003: Verify MSG90 Displays When No Halls Available

### Test Type: UI Test - Manual
### Priority: High
### Test Category: BR138

### Test Steps:
1. **Setup Test Condition**
   - Identify a date/shift combination where ALL halls are booked
   - OR manually book all halls for a specific test date/shift

2. **Navigate to Hall Availability Check**
   - Open "Ki?m tra s?nh tr?ng" function

3. **Select Fully Booked Date**
   - Select the date where all halls are occupied
   - **Expected**: Date is selected

4. **Select Fully Booked Shift**
   - Select the shift where all halls are occupied
   - **Expected**: Shift is selected

5. **Click Search Button**
   - Click "Tìm ki?m" button
   - **Expected**: Query executes

6. **Verify MSG90 Message**
   - Check for message display
   - **Expected**:
     - Message displays: "Không có s?nh tr?ng cho ngày và ca ?ã ch?n"
     - OR English: "No halls available for selected date and shift"
     - Message is prominently displayed (MessageBox or status label)
     - No halls are listed in results

### Pass Criteria:
- ? MSG90 message displays when no halls available
- ? Message text is correct
- ? No hall list is shown (empty result)
- ? User understands no availability

---

## TC_BR138_004: Verify "Create Booking" Button Available for Each Hall

### Test Type: UI Test - Manual
### Priority: High
### Test Category: BR138

### Test Steps:
1. **Navigate to Hall Availability Check**
   - Open "Ki?m tra s?nh tr?ng" function

2. **Select Date and Shift with Multiple Available Halls**
   - Choose criteria that returns 2+ available halls
   - Click search

3. **View Available Halls List**
   - Examine the results table/list
   - **Expected**: Multiple halls are displayed

4. **Check Each Hall Row**
   - For each available hall, look for action button
   - **Expected**:
     - Each row has a "T?o phi?u ??t" button
     - Button is enabled (not grayed out)
     - Button is clickable

5. **Click "T?o phi?u ??t" for First Hall**
   - Click the button for any available hall
   - **Expected**:
     - Booking creation form opens
     - Selected hall is pre-filled
     - Selected date and shift are pre-filled

6. **Verify Pre-filled Information**
   - Check the booking form fields
   - **Expected**:
     - Hall name matches selected hall
     - Date matches selected date
     - Shift matches selected shift

### Pass Criteria:
- ? "T?o phi?u ??t" button present for ALL available halls
- ? Button is enabled and clickable
- ? Clicking button opens booking form
- ? Booking form pre-fills hall, date, shift correctly

---

## TC_BR138_005: Verify Existing Bookings Shown for Occupied Halls

### Test Type: UI Test - Manual
### Priority: Medium
### Test Category: BR138

### Test Steps:
1. **Setup Test Data**
   - Ensure some halls have CONFIRMED bookings for test date/shift
   - Note the booking details (customer name, status)

2. **Navigate to Hall Availability Check**
   - Open "Ki?m tra s?nh tr?ng" function

3. **Select Date/Shift with Mixed Availability**
   - Choose date/shift where:
     - Some halls are AVAILABLE
     - Some halls are OCCUPIED with bookings

4. **Execute Query**
   - Click search button
   - **Expected**: Results show both available and occupied halls

5. **Check Occupied Hall Display**
   - For each occupied hall, verify:
     - **Expected to Display**:
       - Hall is marked as OCCUPIED (visual indicator)
       - Customer name (Groom/Bride names)
       - Booking status (e.g., "?ã ??t c?c", "?ã xác nh?n")
       - Contact phone number
       - "T?o phi?u ??t" button is DISABLED or hidden

6. **Check Available Hall Display**
   - For each available hall, verify:
     - Hall is marked as AVAILABLE
     - No booking info displayed
     - "T?o phi?u ??t" button is ENABLED

### Pass Criteria:
- ? Occupied halls clearly distinguished from available halls
- ? Booking information displayed for occupied halls:
  - Customer name
  - Booking status
  - Contact information
- ? Cannot create new booking for occupied hall
- ? Available halls show create booking button

### Visual Distinction Examples:
- Occupied: Red/Orange background, "?ã ??t" badge
- Available: Green background, "Tr?ng" badge

---

## UI Test Execution Notes

### Test Environment Setup:
1. **Application**: QuanLyTiecCuoi.exe (Debug or Release build)
2. **Database**: Test database with sample data
3. **Test User**: staff_test / Test@123
4. **Screen Resolution**: 1920x1080 (recommended)

### Test Data Requirements:
- At least 5 halls with various types and capacities
- At least 2 shifts (Tr?a, T?i)
- Sample bookings covering:
  - Past dates
  - Future dates
  - Various statuses (Pending, Confirmed, Cancelled, Paid)
  - Different halls and shifts

### Recording Test Results:
For each test case, record:
- ? PASS or ? FAIL
- Screenshots of key steps
- Any error messages or unexpected behavior
- Actual vs Expected results
- Date and time of test execution
- Tester name

### Common Issues to Watch For:
- UI elements not loading
- Calendar not showing correct dates
- Filters not working
- Data not matching database
- Messages not displaying
- Buttons not responding to clicks
- Performance issues (slow loading)

---

## Automated UI Testing (Future Enhancement)

For future automation, consider:
- **WinAppDriver**: Microsoft's UI automation tool for WPF
- **White**: Open source UI automation framework
- **Coded UI Tests**: Visual Studio feature
- **FlaUI**: Modern UI automation library

### Example Automated UI Test Structure (Pseudocode):
```csharp
[TestMethod]
public void AutomatedUI_TC_BR137_001_BookingManagementScreen()
{
    // Launch application
    var app = Application.Launch("QuanLyTiecCuoi.exe");
    
    // Find and click login button
    var loginButton = app.FindElement(By.Name("LoginButton"));
    loginButton.Click();
    
    // Navigate to booking management
    var menuItem = app.FindElement(By.Name("MenuBooking"));
    menuItem.Click();
    
    // Assert screen is displayed
    Assert.IsTrue(app.FindElement(By.Name("BookingGrid")).Displayed);
}
```

---

## Test Execution Checklist

Before running UI tests:
- [ ] Application builds successfully
- [ ] Test database is populated
- [ ] Test user account exists
- [ ] All prerequisite test data is in place
- [ ] Screen recording software is ready (optional)
- [ ] Bug tracking system is available

During test execution:
- [ ] Follow test steps exactly as written
- [ ] Take screenshots at each step
- [ ] Note any deviations from expected results
- [ ] Record performance issues
- [ ] Document any workarounds used

After test execution:
- [ ] Update test results in test management system
- [ ] File bugs for any failures
- [ ] Attach screenshots and logs
- [ ] Suggest test improvements
- [ ] Report to test lead/QA manager
