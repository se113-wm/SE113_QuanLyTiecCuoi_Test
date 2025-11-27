# Wedding Modules Quick Fix Script
# Automatically fixes common issues in Wedding ViewModels and Views

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "WEDDING MODULES QUICK FIX" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$projectRoot = $PSScriptRoot

# Define all Wedding-specific mappings
$weddingMappings = @{
    # Service injections
    '_caService' = '_shiftService'
    '_BookingService' = '_bookingService'
    '_DishService' = '_dishService'
    '_ServiceService' = '_serviceService'
    '_thucDonService' = '_menuService'
    '_chiTietServiceService' = '_serviceDetailService'
    '_thamSoService' = '_parameterService'
    '_sanhService' = '_hallService'
    
    # Properties
    'SelectedCa' = 'SelectedShift'
    'SelectedSanh' = 'SelectedHall'
    'CaList' = 'ShiftList'
    'SanhList' = 'HallList'
    'NgayDaiTiec' = 'WeddingDate'
    'NgayDatTiec' = 'BookingDate'
    'NgayKhongChoChon' = 'UnavailableDates'
    'TD_SoLuong' = 'MenuQuantity'
    'TD_GhiChu' = 'MenuNote'
    'DV_SoLuong' = 'ServiceQuantity'
    'DV_GhiChu' = 'ServiceNote'
    'MonAn' = 'Dish'
    'DichVu' = 'Service'
    
    # Commands
    'ResetTDCommand' = 'ResetMenuCommand'
    'ResetCTDVCommand' = 'ResetServiceCommand'
    'ChonMonAnCommand' = 'SelectDishCommand'
    'ChonDichVuCommand' = 'SelectServiceCommand'
    
    # Methods
    'ResetTD' = 'ResetMenu'
    'ResetCTDV' = 'ResetService'
    'ChonMonAn' = 'SelectDish'
    'ChonDichVu' = 'SelectService'
    
    # Variables
    'tiLeSoBanDatTruocToiThieu' = 'minTableRatio'
    'soLuongBanToiDa' = 'maxTableCount'
    'soLuongBan' = 'tableCount'
    'soBanDuTru' = 'reserveCount'
    'soBanDuTruToiDa' = 'maxReserveCount'
    'tienDatCoc' = 'deposit'
    'tongDonGiaMonAn' = 'totalDishPrice'
    'tongDonGiaDichVu' = 'totalServicePrice'
    'donGiaBanToiThieu' = 'minTablePrice'
    'tongChiPhiUocTinh' = 'estimatedTotal'
    'tiLeTienDatCocToiThieu' = 'minDepositRatio'
    'phieuDatTiec' = 'booking'
    'thucDon' = 'menu'
    'chiTietDV' = 'serviceDetail'
}

# Find Wedding-related files
$weddingFiles = @(
    "Presentation\ViewModel\WeddingViewModel.cs",
    "Presentation\ViewModel\WeddingDetailViewModel.cs",
    "Presentation\View\AddWeddingView.xaml",
    "Presentation\View\WeddingView.xaml",
    "Presentation\View\WeddingDetailView.xaml"
)

$filesChanged = 0
$totalChanges = 0

foreach ($relativeFile in $weddingFiles) {
    $filePath = Join-Path $projectRoot $relativeFile
    
    if (!(Test-Path $filePath)) {
        Write-Host "`n??  File not found: $relativeFile" -ForegroundColor Yellow
        continue
    }
    
    Write-Host "`nProcessing: $relativeFile" -ForegroundColor Cyan
    
    $content = Get-Content -Path $filePath -Raw -Encoding UTF8
    $originalContent = $content
    $fileChanges = 0
    
    # Apply Wedding-specific replacements
    foreach ($oldText in $weddingMappings.Keys) {
        $newText = $weddingMappings[$oldText]
        $pattern = "\b$([regex]::Escape($oldText))\b"
        $matches = [regex]::Matches($content, $pattern)
        
        if ($matches.Count -gt 0) {
            $content = [regex]::Replace($content, $pattern, $newText)
            $fileChanges += $matches.Count
            Write-Host "  - $oldText ? $newText ($($matches.Count)x)" -ForegroundColor DarkYellow
        }
    }
    
    # Save if changed
    if ($content -ne $originalContent) {
        Set-Content -Path $filePath -Value $content -Encoding UTF8 -NoNewline
        $filesChanged++
        $totalChanges += $fileChanges
        Write-Host "  ? SAVED - $fileChanges changes" -ForegroundColor Green
    }
    else {
        Write-Host "  - No changes needed" -ForegroundColor Gray
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "QUICK FIX COMPLETE" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Files changed: $filesChanged" -ForegroundColor Yellow
Write-Host "Total replacements: $totalChanges" -ForegroundColor Yellow

Write-Host "`n?? Next Manual Steps:" -ForegroundColor Magenta
Write-Host "1. Add AutomationId to ALL XAML controls" -ForegroundColor White
Write-Host "2. Translate Vietnamese labels to English" -ForegroundColor White
Write-Host "3. Build solution (Ctrl+Shift+B)" -ForegroundColor White
Write-Host "4. Test all Wedding functionality" -ForegroundColor White

Write-Host "`nPress any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
