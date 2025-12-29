# Quick Fix Reference - Stale Window Issue

## The Problem in One Line
**After successful login, `_mainWindow` pointed to the closing Login Window, not the new Main Window.**

## The Fix in One Line
**Re-acquire the window by searching all process windows and finding the one with `WeddingButton`.**

## Code Change Summary

### Before (Lines 127-130) - BROKEN
```csharp
// This gets the WRONG window - still points to Login Window
_mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(5));

// This CRASHES because we're accessing a closing window's Title
var loginSuccess = _mainWindow.Title.Contains("Wedding Management");
```

### After (Lines 127-180) - FIXED
```csharp
// Wait for transition
Thread.Sleep(2000);

// Find ALL windows for our process
var appWindows = desktop.FindAllChildren(cf => cf.ByProcessId(_app.ProcessId));

// Find the one with WeddingButton (= Main Window)
foreach (var window in appWindows)
{
    var weddingButton = window.FindFirstDescendant(cf => cf.ByAutomationId("WeddingButton"));
    if (weddingButton != null)
    {
        _mainWindow = window.AsWindow(); // ? Correct window!
        return true;
    }
}
```

## Why It Works
- ? Search by **process ID** (finds all our app's windows)
- ? Verify by **element** (WeddingButton exists only in Main Window)
- ? Never access **.Title** property on transitioning window
- ? Update **_mainWindow** to correct window reference

## Test It
```powershell
dotnet test --filter "FullyQualifiedName~TC_BR137_001" -v detailed
```

Look for:
```
Debug Trace:
Main Window re-acquired successfully by finding WeddingButton
```

## Status
? **FIXED** - Build successful, ready for testing
