# UI Testing Issues & Solutions

## Issue: MaterialDesign Card AutomationId Not Found

### Problem Description

When running UI tests, FlaUI cannot find MaterialDesign Card elements by AutomationId:

```csharp
// This fails:
var filterCard = _mainWindow.FindFirstDescendant(cf => 
    cf.ByAutomationId("FilterCard")); // Returns null
```

Even though the XAML has AutomationId set:

```xml
<materialDesign:Card AutomationProperties.AutomationId="FilterCard">
    <!-- content -->
</materialDesign:Card>
```

### Root Cause

**MaterialDesign controls (Card, etc.) don't reliably expose their AutomationId to UI Automation.**

This is because:
1. MaterialDesign controls are custom controls that wrap standard WPF controls
2. The AutomationId might be set on the wrapper, but UI Automation sees the internal structure
3. FlaUI searches the automation tree, which may not include the Card's automation properties

### Solution: Find Child Elements Instead

Instead of finding the Card container, find the controls **inside** the Card:

#### ? Don't do this:
```csharp
var filterCard = _mainWindow.FindFirstDescendant(cf => 
    cf.ByAutomationId("FilterCard"));
Assert.IsNotNull(filterCard, "Filter card should be visible");
```

#### ? Do this instead:
```csharp
// Find controls inside the card
var groomFilter = _mainWindow.FindFirstDescendant(cf => 
    cf.ByAutomationId("GroomNameFilterComboBox"));
Assert.IsNotNull(groomFilter, "Groom filter should be visible");

var resetButton = _mainWindow.FindFirstDescendant(cf => 
    cf.ByAutomationId("ResetFilterButton"));
Assert.IsNotNull(resetButton, "Reset button should be visible");
```

### Updated Test Example

```csharp
[TestMethod]
public void TC_BR137_001_BookingManagementScreen_DisplaysForAuthorizedUsers()
{
    // Navigate to Wedding tab
    Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

    // Verify Wedding view is displayed
    var weddingTitle = _mainWindow.FindFirstDescendant(cf => 
        cf.ByAutomationId("WeddingPageTitle"));
    Assert.IsNotNull(weddingTitle, "Wedding page title should be visible");

    // ? Verify filter controls (not the Card itself)
    var groomFilter = _mainWindow.FindFirstDescendant(cf => 
        cf.ByAutomationId("GroomNameFilterComboBox"));
    Assert.IsNotNull(groomFilter, "Groom name filter should be visible");

    var resetFilterButton = _mainWindow.FindFirstDescendant(cf => 
        cf.ByAutomationId("ResetFilterButton"));
    Assert.IsNotNull(resetFilterButton, "Reset filter button should be visible");

    // ? Verify action buttons
    var addButton = _mainWindow.FindFirstDescendant(cf => 
        cf.ByAutomationId("AddButton"));
    Assert.IsNotNull(addButton, "Add button should be visible");

    // ? Verify list view
    var weddingListView = _mainWindow.FindFirstDescendant(cf => 
        cf.ByAutomationId("WeddingListView"));
    Assert.IsNotNull(weddingListView, "Wedding list view should be visible");
}
```

---

## Other MaterialDesign Controls That May Have Issues

### Controls to Avoid Finding Directly:
- `materialDesign:Card`
- `materialDesign:ColorZone`
- `materialDesign:DialogHost`
- Custom MaterialDesign container controls

### Controls That Work Well:
- `Button` (even with MaterialDesign styles)
- `TextBox`
- `ComboBox`
- `DatePicker`
- `ListView` / `DataGrid`
- `TextBlock`

---

## Debugging Strategy

### 1. Add Debug Test

Create a test that lists all found elements:

```csharp
[TestMethod]
[TestCategory("Debug")]
public void Debug_VerifyNavigationAndElements()
{
    Assert.IsTrue(NavigateToWeddingTab(), "Should navigate to Wedding tab");

    // List all automation IDs found
    var filters = new[]
    {
        "GroomNameFilterComboBox",
        "BrideNameFilterComboBox", 
        "HallNameFilterComboBox",
        "WeddingDateFilterPicker",
        "FilterCard", // This will show NOT FOUND
        "WeddingListView"
    };

    foreach (var id in filters)
    {
        var element = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(id));
        System.Diagnostics.Debug.WriteLine($"{id}: {(element != null ? "FOUND" : "NOT FOUND")}");
    }
}
```

### 2. Use Inspect.exe

Use Windows SDK's Inspect.exe tool:
1. Launch your application
2. Navigate to the screen you want to test
3. Open Inspect.exe
4. Hover over elements to see their automation properties

**You'll see:**
- MaterialDesign Cards often don't have AutomationId in the tree
- Child controls inside Cards do have AutomationId

### 3. Check Accessibility Tree

Use Accessibility Insights tool:
- Download: https://accessibilityinsights.io/
- Launch app and inspect automation tree
- Verify which elements have AutomationId

---

## Best Practices for UI Testing

### 1. Don't rely on container AutomationIds
```csharp
// ? Bad
var card = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("FilterCard"));

// ? Good  
var firstControl = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("GroomNameFilterComboBox"));
```

### 2. Find interactive controls instead
```csharp
// ? Find buttons, textboxes, comboboxes
var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AddButton"));
var searchBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SearchTextBox"));
```

### 3. Increase wait times for MaterialDesign
```csharp
// MaterialDesign has animations - wait longer
button.Click();
Thread.Sleep(1500); // Instead of 500ms
```

### 4. Use multiple finding strategies
```csharp
// Try AutomationId first
var element = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("MyControl"));

// Fallback to Name
if (element == null)
{
    element = _mainWindow.FindFirstDescendant(cf => cf.ByName("Control Label"));
}

// Fallback to ControlType
if (element == null)
{
    element = _mainWindow.FindFirstDescendant(cf => 
        cf.ByControlType(ControlType.Button)
        .And(cf.ByName("Add")));
}
```

---

## Summary

### Key Takeaways:

1. ? **MaterialDesign Cards don't expose AutomationId reliably**
2. ? **Find child controls instead of container controls**
3. ? **Use Debug tests to verify element finding**
4. ? **Use Inspect.exe to check automation tree**
5. ? **Increase wait times for MaterialDesign animations**
6. ? **Test with interactive controls (Button, TextBox, etc.)**

### Updated Test Pattern:

```csharp
[TestMethod]
public void MyTest()
{
    // Navigate
    NavigateToScreen();
    
    // ? Find interactive controls, not containers
    var control1 = FindControl("Control1AutomationId");
    var control2 = FindControl("Control2AutomationId");
    
    // Assert on actual controls
    Assert.IsNotNull(control1);
    Assert.IsNotNull(control2);
    
    // Interact
    control1.AsButton().Click();
    Thread.Sleep(1500); // Wait for animations
}
```

---

**Document Version:** 1.0  
**Last Updated:** 2024  
**Related:** RUNNING_UI_TESTS.md
