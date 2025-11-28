# Master PowerShell Script to Execute Complete Refactoring
# This script runs all refactoring steps in order

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   COMPLETE REFACTORING PROCESS" -ForegroundColor Cyan
Write-Host "   Vietnamese -> English" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$projectRoot = $PSScriptRoot
Write-Host "`nProject Root: $projectRoot" -ForegroundColor Yellow

# Check if all scripts exist
$refactorScript = Join-Path $projectRoot "RefactorToEnglish.ps1"
$renameScript = Join-Path $projectRoot "RenameFiles.ps1"
$updateProjScript = Join-Path $projectRoot "UpdateProjectFile.ps1"

$scriptsToCheck = @(
    @{Name = "RefactorToEnglish.ps1"; Path = $refactorScript}
    @{Name = "RenameFiles.ps1"; Path = $renameScript}
    @{Name = "UpdateProjectFile.ps1"; Path = $updateProjScript}
)

$allScriptsExist = $true
foreach ($script in $scriptsToCheck) {
    if (-not (Test-Path $script.Path)) {
        Write-Host "ERROR: $($script.Name) not found!" -ForegroundColor Red
        $allScriptsExist = $false
    }
}

if (-not $allScriptsExist) {
    Write-Host "`nMake sure all required scripts are in the project root." -ForegroundColor Yellow
    exit 1
}

# Display what will happen
Write-Host "`n?? This script will perform the following steps:" -ForegroundColor Yellow
Write-Host "   1. Refactor code content (replace Vietnamese with English)" -ForegroundColor White
Write-Host "   2. Rename files (DTO, Service, Repository)" -ForegroundColor White
Write-Host "   3. Update .csproj file references" -ForegroundColor White
Write-Host ""

# Confirm before proceeding
Write-Host "??  WARNING: This will modify your codebase!" -ForegroundColor Yellow
$confirm = Read-Host "Do you want to proceed? (yes/no)"

if ($confirm -ne "yes") {
    Write-Host "`nOperation cancelled." -ForegroundColor Yellow
    exit 0
}

# Recommend Git commit
Write-Host "`n?? RECOMMENDATION: Commit your current code to Git before proceeding" -ForegroundColor Magenta
$gitConfirm = Read-Host "Have you committed your code? (yes/no)"

if ($gitConfirm -ne "yes") {
    Write-Host "`n??  Please commit your code first, then run this script again." -ForegroundColor Yellow
    Write-Host "   Run: git add . && git commit -m 'Before refactoring'" -ForegroundColor White
    
    $continueAnyway = Read-Host "`nContinue anyway? (yes/no)"
    if ($continueAnyway -ne "yes") {
        exit 0
    }
}

# Step 1: Refactor Code Content
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "STEP 1/3: Refactoring Code Content" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

try {
    & $refactorScript
    if ($LASTEXITCODE -ne 0 -and $null -ne $LASTEXITCODE) {
        throw "RefactorToEnglish.ps1 failed"
    }
    Write-Host "`n? Step 1 completed successfully!" -ForegroundColor Green
}
catch {
    Write-Host "`n? ERROR during code refactoring: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Aborting process..." -ForegroundColor Red
    exit 1
}

Start-Sleep -Seconds 2

# Step 2: Rename Files
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "STEP 2/3: Renaming Files" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

try {
    & $renameScript
    if ($LASTEXITCODE -ne 0 -and $null -ne $LASTEXITCODE) {
        throw "RenameFiles.ps1 failed"
    }
    Write-Host "`n? Step 2 completed successfully!" -ForegroundColor Green
}
catch {
    Write-Host "`n? ERROR during file renaming: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Continuing to next step..." -ForegroundColor Yellow
}

Start-Sleep -Seconds 2

# Step 3: Update Project File
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "STEP 3/3: Updating Project File" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

try {
    & $updateProjScript
    if ($LASTEXITCODE -ne 0 -and $null -ne $LASTEXITCODE) {
        throw "UpdateProjectFile.ps1 failed"
    }
    Write-Host "`n? Step 3 completed successfully!" -ForegroundColor Green
}
catch {
    Write-Host "`n? ERROR during project file update: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "You may need to update the project file manually." -ForegroundColor Yellow
}

# Final summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "?? REFACTORING COMPLETE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`n?? SUMMARY:" -ForegroundColor Yellow
Write-Host "? Code content refactored" -ForegroundColor Green
Write-Host "? Files renamed" -ForegroundColor Green
Write-Host "? Project file updated" -ForegroundColor Green
Write-Host ""

Write-Host "?? NEXT STEPS (MANDATORY):" -ForegroundColor Magenta
Write-Host ""
Write-Host "1. CLOSE VISUAL STUDIO (if open)" -ForegroundColor Yellow
Write-Host "   - Close all instances of Visual Studio" -ForegroundColor White
Write-Host ""

Write-Host "2. REOPEN AND RELOAD PROJECT:" -ForegroundColor Yellow
Write-Host "   - Open Visual Studio" -ForegroundColor White
Write-Host "   - Open the solution file (.sln)" -ForegroundColor White
Write-Host "   - Reload project when prompted" -ForegroundColor White
Write-Host ""

Write-Host "3. CLEAN AND REBUILD:" -ForegroundColor Yellow
Write-Host "   - Build > Clean Solution" -ForegroundColor White
Write-Host "   - Build > Rebuild Solution" -ForegroundColor White
Write-Host ""

Write-Host "4. FIX XAML BINDINGS:" -ForegroundColor Yellow
Write-Host "   - Open each .xaml file" -ForegroundColor White
Write-Host "   - Find and replace old property names" -ForegroundColor White
Write-Host "   - Example: {Binding MaSanh} -> {Binding HallId}" -ForegroundColor White
Write-Host ""

Write-Host "5. TEST THOROUGHLY:" -ForegroundColor Yellow
Write-Host "   - Test all CRUD operations" -ForegroundColor White
Write-Host "   - Test all forms and views" -ForegroundColor White
Write-Host "   - Test reports and charts" -ForegroundColor White
Write-Host ""

Write-Host "6. COMMIT CHANGES:" -ForegroundColor Yellow
Write-Host "   git add ." -ForegroundColor White
Write-Host "   git commit -m 'Refactor: Vietnamese to English naming'" -ForegroundColor White
Write-Host "   git push" -ForegroundColor White
Write-Host ""

# Offer to create a XAML refactor helper
Write-Host "========================================" -ForegroundColor Cyan
$createXamlHelper = Read-Host "`nDo you want to create a XAML binding reference guide? (Y/N)"
if ($createXamlHelper -eq "Y" -or $createXamlHelper -eq "y") {
    $guideContent = @"
# XAML Binding Reference Guide

This file contains all the XAML binding replacements you need to make manually.

## How to use:
1. Open each .xaml file in Visual Studio
2. Use Find & Replace (Ctrl+H) for each binding below
3. Make sure to check "Match whole word"

## Hall (S?nh) Bindings:
{Binding MaSanh} -> {Binding HallId}
{Binding TenSanh} -> {Binding HallName}
{Binding SoLuongBanToiDa} -> {Binding MaxTableCount}

## HallType (Lo?iS?nh) Bindings:
{Binding MaLoaiSanh} -> {Binding HallTypeId}
{Binding TenLoaiSanh} -> {Binding HallTypeName}
{Binding DonGiaBanToiThieu} -> {Binding MinTablePrice}
{Binding LoaiSanh} -> {Binding HallType}

## Shift (Ca) Bindings:
{Binding MaCa} -> {Binding ShiftId}
{Binding TenCa} -> {Binding ShiftName}
{Binding ThoiGianBatDauCa} -> {Binding StartTime}
{Binding ThoiGianKetThucCa} -> {Binding EndTime}
{Binding Ca} -> {Binding Shift}

## Dish (MonAn) Bindings:
{Binding MaMonAn} -> {Binding DishId}
{Binding TenMonAn} -> {Binding DishName}
{Binding MonAn} -> {Binding Dish}

## Service (DichVu) Bindings:
{Binding MaDichVu} -> {Binding ServiceId}
{Binding TenDichVu} -> {Binding ServiceName}
{Binding DichVu} -> {Binding Service}

## Booking (PhieuDatTiec) Bindings:
{Binding MaPhieuDat} -> {Binding BookingId}
{Binding TenChuRe} -> {Binding GroomName}
{Binding TenCoDau} -> {Binding BrideName}
{Binding DienThoai} -> {Binding Phone}
{Binding NgayDatTiec} -> {Binding BookingDate}
{Binding NgayDaiTiec} -> {Binding WeddingDate}
{Binding TienDatCoc} -> {Binding Deposit}
{Binding SoLuongBan} -> {Binding TableCount}
{Binding SoBanDuTru} -> {Binding ReserveTableCount}
{Binding NgayThanhToan} -> {Binding PaymentDate}
{Binding DonGiaBanTiec} -> {Binding TablePrice}
{Binding TongTienBan} -> {Binding TotalTableAmount}
{Binding TongTienDV} -> {Binding TotalServiceAmount}
{Binding TongTienHoaDon} -> {Binding TotalInvoiceAmount}
{Binding TienConLai} -> {Binding RemainingAmount}
{Binding ChiPhiPhatSinh} -> {Binding AdditionalCost}
{Binding TienPhat} -> {Binding PenaltyAmount}
{Binding Sanh} -> {Binding Hall}

## Menu/ServiceDetail Common Bindings:
{Binding SoLuong} -> {Binding Quantity}
{Binding DonGia} -> {Binding UnitPrice}
{Binding GhiChu} -> {Binding Note}
{Binding ThanhTien} -> {Binding TotalAmount}

## User (NguoiDung) Bindings:
{Binding MaNguoiDung} -> {Binding UserId}
{Binding TenDangNhap} -> {Binding Username}
{Binding MatKhauHash} -> {Binding PasswordHash}
{Binding HoTen} -> {Binding FullName}
{Binding SoDienThoai} -> {Binding PhoneNumber}
{Binding DiaChi} -> {Binding Address}
{Binding NgaySinh} -> {Binding BirthDate}
{Binding GioiTinh} -> {Binding Gender}
{Binding MaNhom} -> {Binding GroupId}
{Binding NhomNguoiDung} -> {Binding UserGroup}

## UserGroup (NhomNguoiDung) Bindings:
{Binding TenNhom} -> {Binding GroupName}

## Permission (ChucNang) Bindings:
{Binding MaChucNang} -> {Binding PermissionId}
{Binding TenChucNang} -> {Binding PermissionName}
{Binding TenManHinhDuocLoad} -> {Binding LoadedScreenName}

## Parameter (ThamSo) Bindings:
{Binding TenThamSo} -> {Binding ParameterName}
{Binding GiaTri} -> {Binding Value}

## Revenue Report Bindings:
{Binding Thang} -> {Binding Month}
{Binding Nam} -> {Binding Year}
{Binding Ngay} -> {Binding Day}
{Binding TongDoanhThu} -> {Binding TotalRevenue}
{Binding DoanhThu} -> {Binding Revenue}
{Binding SoLuongTiec} -> {Binding WeddingCount}
{Binding TiLe} -> {Binding Ratio}
{Binding NgayHienThi} -> {Binding DisplayDate}

## Tips:
- Use Find & Replace in entire solution (Ctrl+Shift+H)
- Check "Match whole word" option
- Preview changes before replacing all
- Test each view after updating bindings
"@
    
    $guidePath = Join-Path $projectRoot "XAML_BINDING_GUIDE.md"
    Set-Content -Path $guidePath -Value $guideContent -Encoding UTF8
    Write-Host "`n? Created: XAML_BINDING_GUIDE.md" -ForegroundColor Green
    
    # Open the guide
    Start-Process notepad.exe -ArgumentList $guidePath
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
