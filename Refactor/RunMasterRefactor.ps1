# MASTER REFACTORING SCRIPT
# Runs all refactoring steps in correct order:
# 1. Repository Layer
# 2. Service Layer  
# 3. DTO Layer
# 4. ViewModel Layer
# 5. Rename Files
# 6. Update .csproj

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   MASTER REFACTORING ORCHESTRATOR" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$projectRoot = $PSScriptRoot
Write-Host "`nProject Root: $projectRoot" -ForegroundColor Yellow

# Check all required scripts exist
$requiredScripts = @(
    "RefactorAllEntities.ps1",
    "RefactorViewModels.ps1",
    "RenameFiles.ps1",
    "UpdateProjectFile.ps1"
)

$allExist = $true
foreach ($script in $requiredScripts) {
    $scriptPath = Join-Path $projectRoot $script
    if (!(Test-Path $scriptPath)) {
        Write-Host "ERROR: Missing required script: $script" -ForegroundColor Red
        $allExist = $false
    }
}

if (!$allExist) {
    Write-Host "`nPlease ensure all refactoring scripts are in the project root." -ForegroundColor Yellow
    exit 1
}

# Confirmation
Write-Host "`n??  WARNING: This will refactor your entire codebase!" -ForegroundColor Yellow
Write-Host "   The following will be executed in order:" -ForegroundColor White
Write-Host "   1. Refactor all entities (Repository, Service, DTO)" -ForegroundColor Gray
Write-Host "   2. Refactor all ViewModels" -ForegroundColor Gray
Write-Host "   3. Rename files" -ForegroundColor Gray
Write-Host "   4. Update .csproj file" -ForegroundColor Gray
Write-Host ""

$confirm = Read-Host "Continue? (yes/no)"
if ($confirm -ne "yes") {
    Write-Host "Operation cancelled." -ForegroundColor Yellow
    exit 0
}

# Git check
Write-Host "`n?? IMPORTANT: Have you committed your code to Git?" -ForegroundColor Magenta
$gitConfirm = Read-Host "Committed to Git? (yes/no)"
if ($gitConfirm -ne "yes") {
    Write-Host "`n??  STRONGLY RECOMMENDED: Commit first!" -ForegroundColor Yellow
    Write-Host "   Run: git add . && git commit -m 'Before refactoring'" -ForegroundColor White
    
    $continueAnyway = Read-Host "`nContinue WITHOUT commit? (yes/no)"
    if ($continueAnyway -ne "yes") {
        exit 0
    }
}

# Create master backup
$masterBackup = Join-Path $projectRoot "MasterBackup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
Write-Host "`nCreating master backup..." -ForegroundColor Yellow
Write-Host "Location: $masterBackup" -ForegroundColor Gray

$excludeDirs = @("bin", "obj", "packages", ".vs", ".git", "Backup_*", "MasterBackup_*")
Get-ChildItem -Path $projectRoot -Directory | Where-Object {
    $exclude = $false
    foreach ($dir in $excludeDirs) {
        if ($_.Name -like $dir) { $exclude = $true; break }
    }
    !$exclude
} | ForEach-Object {
    $destPath = Join-Path $masterBackup $_.Name
    Copy-Item -Path $_.FullName -Destination $destPath -Recurse -Force -ErrorAction SilentlyContinue
}

Get-ChildItem -Path $projectRoot -File | Where-Object {
    $_.Extension -in @(".cs", ".xaml", ".csproj", ".sln", ".config")
} | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination $masterBackup -Force
}

Write-Host "? Master backup created!" -ForegroundColor Green

# Execute refactoring steps
$steps = @(
    @{
        Number = 1
        Name = "Refactor All Entities"
        Script = "RefactorAllEntities.ps1"
        Description = "Repository, Service, DTO layers"
    },
    @{
        Number = 2
        Name = "Refactor ViewModels"
        Script = "RefactorViewModels.ps1"
        Description = "ViewModel layer"
    },
    @{
        Number = 3
        Name = "Rename Files"
        Script = "RenameFiles.ps1"
        Description = "Physical file renaming"
    },
    @{
        Number = 4
        Name = "Update Project File"
        Script = "UpdateProjectFile.ps1"
        Description = ".csproj references"
    }
)

$completedSteps = 0
$failedStep = $null

foreach ($step in $steps) {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "STEP $($step.Number)/4: $($step.Name)" -ForegroundColor Cyan
    Write-Host "$($step.Description)" -ForegroundColor Gray
    Write-Host "========================================" -ForegroundColor Cyan
    
    $scriptPath = Join-Path $projectRoot $step.Script
    
    try {
        # Execute script
        & $scriptPath
        
        if ($LASTEXITCODE -ne 0 -and $null -ne $LASTEXITCODE) {
            throw "Script exited with code $LASTEXITCODE"
        }
        
        $completedSteps++
        Write-Host "`n? Step $($step.Number) completed!" -ForegroundColor Green
        Start-Sleep -Seconds 2
    }
    catch {
        Write-Host "`n? Step $($step.Number) FAILED!" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        $failedStep = $step
        break
    }
}

# Final Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "MASTER REFACTORING SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($completedSteps -eq $steps.Count) {
    Write-Host "?? ALL STEPS COMPLETED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host "Completed: $completedSteps / $($steps.Count)" -ForegroundColor Green
}
else {
    Write-Host "??  PARTIALLY COMPLETED" -ForegroundColor Yellow
    Write-Host "Completed: $completedSteps / $($steps.Count)" -ForegroundColor Yellow
    Write-Host "Failed at: Step $($failedStep.Number) - $($failedStep.Name)" -ForegroundColor Red
}

Write-Host "`nMaster Backup: $masterBackup" -ForegroundColor Gray

Write-Host "`n?? CRITICAL NEXT STEPS:" -ForegroundColor Magenta
Write-Host ""
Write-Host "1. CLOSE VISUAL STUDIO" -ForegroundColor Yellow
Write-Host "   - Close all instances" -ForegroundColor White
Write-Host ""
Write-Host "2. REOPEN SOLUTION" -ForegroundColor Yellow
Write-Host "   - Open .sln file" -ForegroundColor White
Write-Host "   - Reload project if prompted" -ForegroundColor White
Write-Host ""
Write-Host "3. CLEAN AND REBUILD" -ForegroundColor Yellow
Write-Host "   - Clean Solution" -ForegroundColor White
Write-Host "   - Rebuild Solution" -ForegroundColor White
Write-Host ""
Write-Host "4. FIX XAML BINDINGS" -ForegroundColor Yellow
Write-Host "   - Use Find & Replace in XAML files" -ForegroundColor White
Write-Host "   - Reference: XAML_BINDING_GUIDE.md" -ForegroundColor White
Write-Host ""
Write-Host "5. TEST THOROUGHLY" -ForegroundColor Yellow
Write-Host "   - Test all CRUD operations" -ForegroundColor White
Write-Host "   - Test all ViewModels" -ForegroundColor White
Write-Host "   - Test all Views" -ForegroundColor White
Write-Host ""
Write-Host "6. COMMIT CHANGES" -ForegroundColor Yellow
Write-Host "   git add ." -ForegroundColor White
Write-Host "   git commit -m 'Refactor: Complete Vietnamese to English'" -ForegroundColor White
Write-Host "   git push" -ForegroundColor White
Write-Host ""

# Offer to create XAML guide
$createGuide = Read-Host "`nCreate XAML binding reference guide? (Y/N)"
if ($createGuide -eq "Y" -or $createGuide -eq "y") {
    $guideContent = @"
# XAML BINDING REFACTORING GUIDE

## Auto-generated on: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

This guide lists all XAML binding changes needed after refactoring.

## How to Use:
1. Open each .xaml file
2. Use Find & Replace (Ctrl+H)
3. Replace each binding as listed below

## Booking (PhieuDatTiec) Bindings:
{Binding MaPhieuDat} ? {Binding BookingId}
{Binding TenChuRe} ? {Binding GroomName}
{Binding TenCoDau} ? {Binding BrideName}
{Binding DienThoai} ? {Binding Phone}
{Binding NgayDatTiec} ? {Binding BookingDate}
{Binding NgayDaiTiec} ? {Binding WeddingDate}
{Binding TienDatCoc} ? {Binding Deposit}
{Binding SoLuongBan} ? {Binding TableCount}
{Binding SoBanDuTru} ? {Binding ReserveTableCount}
{Binding NgayThanhToan} ? {Binding PaymentDate}
{Binding TongTienHoaDon} ? {Binding TotalInvoiceAmount}
{Binding TrangThai} ? {Binding Status}
{Binding TrangThaiBrush} ? {Binding StatusBrush}

## Hall (Sanh) Bindings:
{Binding MaSanh} ? {Binding HallId}
{Binding TenSanh} ? {Binding HallName}
{Binding SoLuongBanToiDa} ? {Binding MaxTableCount}

## HallType (LoaiSanh) Bindings:
{Binding MaLoaiSanh} ? {Binding HallTypeId}
{Binding TenLoaiSanh} ? {Binding HallTypeName}
{Binding DonGiaBanToiThieu} ? {Binding MinTablePrice}

## Shift (Ca) Bindings:
{Binding MaCa} ? {Binding ShiftId}
{Binding TenCa} ? {Binding ShiftName}
{Binding ThoiGianBatDauCa} ? {Binding StartTime}
{Binding ThoiGianKetThucCa} ? {Binding EndTime}

## Dish (MonAn) Bindings:
{Binding MaMonAn} ? {Binding DishId}
{Binding TenMonAn} ? {Binding DishName}

## Service (DichVu) Bindings:
{Binding MaDichVu} ? {Binding ServiceId}
{Binding TenDichVu} ? {Binding ServiceName}

## Common Bindings:
{Binding DonGia} ? {Binding UnitPrice}
{Binding SoLuong} ? {Binding Quantity}
{Binding GhiChu} ? {Binding Note}
{Binding ThanhTien} ? {Binding TotalAmount}

## Navigation Properties:
{Binding Ca} ? {Binding Shift}
{Binding Sanh} ? {Binding Hall}
{Binding LoaiSanh} ? {Binding HallType}
{Binding MonAn} ? {Binding Dish}
{Binding DichVu} ? {Binding Service}

---
Generated by Master Refactoring Script
"@
    
    $guidePath = Join-Path $projectRoot "XAML_BINDING_COMPLETE_GUIDE.md"
    Set-Content -Path $guidePath -Value $guideContent -Encoding UTF8
    Write-Host "? Created: XAML_BINDING_COMPLETE_GUIDE.md" -ForegroundColor Green
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
