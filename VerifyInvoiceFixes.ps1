# Verify Invoice Method Fixes
# Quick test script to verify the fixes

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "INVOICE METHOD FIXES VERIFICATION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$projectRoot = $PSScriptRoot
Write-Host "`nProject Root: $projectRoot" -ForegroundColor Yellow

# Check for old method names
Write-Host "`n1. Checking for old method names..." -ForegroundColor Yellow

$oldMethods = @(
    "GetByPhieuDat",
    "GetByMaPhieuDat"
)

$foundIssues = $false

foreach ($method in $oldMethods) {
    Write-Host "   Searching for: $method" -ForegroundColor Gray
    
    $files = Get-ChildItem -Path $projectRoot -Include "*.cs" -Recurse -Exclude "*.Designer.cs","*.Context.cs","Model1.cs" |
        Where-Object { $_.FullName -notlike "*\bin\*" -and $_.FullName -notlike "*\obj\*" -and $_.FullName -notlike "*\Model\*" }
    
    foreach ($file in $files) {
        $content = Get-Content -Path $file.FullName -Raw
        if ($content -match $method) {
            Write-Host "   ? FOUND in: $($file.FullName.Replace($projectRoot, ''))" -ForegroundColor Red
            $foundIssues = $true
        }
    }
}

if (!$foundIssues) {
    Write-Host "   ? No old method names found!" -ForegroundColor Green
}

# Check InvoiceViewModel specifically
Write-Host "`n2. Verifying InvoiceViewModel..." -ForegroundColor Yellow

$invoiceVM = Join-Path $projectRoot "Presentation\ViewModel\InvoiceViewModel.cs"
if (Test-Path $invoiceVM) {
    $content = Get-Content -Path $invoiceVM -Raw
    
    $checks = @{
        "GetByBookingId" = $content -match "GetByBookingId"
        "No GetByPhieuDat" = $content -notmatch "GetByPhieuDat"
    }
    
    foreach ($check in $checks.Keys) {
        if ($checks[$check]) {
            Write-Host "   ? $check" -ForegroundColor Green
        } else {
            Write-Host "   ? $check FAILED" -ForegroundColor Red
        }
    }
} else {
    Write-Host "   ? InvoiceViewModel.cs not found!" -ForegroundColor Red
}

# Check interfaces
Write-Host "`n3. Verifying Service Interfaces..." -ForegroundColor Yellow

$interfaces = @(
    "BusinessLogicLayer\IService\IMenuService.cs",
    "BusinessLogicLayer\IService\IServiceDetailService.cs"
)

foreach ($interface in $interfaces) {
    $path = Join-Path $projectRoot $interface
    if (Test-Path $path) {
        $content = Get-Content -Path $path -Raw
        
        if ($content -match "GetByBookingId") {
            Write-Host "   ? $interface has GetByBookingId" -ForegroundColor Green
        } else {
            Write-Host "   ? $interface missing GetByBookingId" -ForegroundColor Red
        }
    } else {
        Write-Host "   ? $interface not found!" -ForegroundColor Red
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "VERIFICATION COMPLETE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if (!$foundIssues) {
    Write-Host "`n? ALL CHECKS PASSED!" -ForegroundColor Green
    Write-Host "   Ready to build and test." -ForegroundColor White
} else {
    Write-Host "`n??  ISSUES FOUND" -ForegroundColor Yellow
    Write-Host "   Please review and fix the issues above." -ForegroundColor White
}

Write-Host "`n?? Next Steps:" -ForegroundColor Magenta
Write-Host "1. Build solution (Ctrl+Shift+B)" -ForegroundColor White
Write-Host "2. Fix any compilation errors" -ForegroundColor White
Write-Host "3. Test Invoice functionality" -ForegroundColor White
Write-Host "4. Verify PDF export" -ForegroundColor White

Write-Host "`nPress any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
