# Refactor ViewModels Script
# Focuses on ViewModel layer refactoring
# Run AFTER repository and service layers are refactored

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "VIEWMODEL REFACTORING SCRIPT" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$projectRoot = $PSScriptRoot
Write-Host "Project Root: $projectRoot" -ForegroundColor Yellow

# ViewModel-specific mappings
$viewModelMappings = @{
    # ===== FIELD NAMES (private) =====
    '_tenChuRe' = '_groomName'
    '_tenCoDau' = '_brideName'
    '_dienThoai' = '_phone'
    '_ngayDatTiec' = '_bookingDate'
    '_ngayDaiTiec' = '_weddingDate'
    '_tienDatCoc' = '_deposit'
    '_soLuongBan' = '_tableCount'
    '_soBanDuTru' = '_reserveTableCount'
    '_ngayThanhToan' = '_paymentDate'
    '_donGiaBanTiec' = '_tablePrice'
    
    # Hall/HallType fields
    '_tenSanh' = '_hallName'
    '_tenLoaiSanh' = '_hallTypeName'
    '_soLuongBanToiDa' = '_maxTableCount'
    '_donGiaBanToiThieu' = '_minTablePrice'
    
    # Shift fields
    '_tenCa' = '_shiftName'
    '_thoiGianBatDauCa' = '_startTime'
    '_thoiGianKetThucCa' = '_endTime'
    
    # Dish fields
    '_tenMonAn' = '_dishName'
    
    # Service fields
    '_tenDichVu' = '_serviceName'
    
    # Common fields
    '_donGia' = '_unitPrice'
    '_soLuong' = '_quantity'
    '_ghiChu' = '_note'
    '_thanhTien' = '_totalAmount'
    
    # ===== PROPERTY NAMES (public) =====
    'TenChuRe' = 'GroomName'
    'TenCoDau' = 'BrideName'
    'DienThoai' = 'Phone'
    'NgayDatTiec' = 'BookingDate'
    'NgayDaiTiec' = 'WeddingDate'
    'TienDatCoc' = 'Deposit'
    'SoLuongBan' = 'TableCount'
    'SoBanDuTru' = 'ReserveTableCount'
    'NgayThanhToan' = 'PaymentDate'
    'DonGiaBanTiec' = 'TablePrice'
    'TongTienBan' = 'TotalTableAmount'
    'TongTienDV' = 'TotalServiceAmount'
    'TongTienHoaDon' = 'TotalInvoiceAmount'
    'TienConLai' = 'RemainingAmount'
    'ChiPhiPhatSinh' = 'AdditionalCost'
    'TienPhat' = 'PenaltyAmount'
    
    # Hall/HallType
    'TenSanh' = 'HallName'
    'TenLoaiSanh' = 'HallTypeName'
    'SoLuongBanToiDa' = 'MaxTableCount'
    'DonGiaBanToiThieu' = 'MinTablePrice'
    
    # Shift
    'TenCa' = 'ShiftName'
    'ThoiGianBatDauCa' = 'StartTime'
    'ThoiGianKetThucCa' = 'EndTime'
    
    # Dish
    'TenMonAn' = 'DishName'
    
    # Service
    'TenDichVu' = 'ServiceName'
    
    # Common
    'DonGia' = 'UnitPrice'
    'SoLuong' = 'Quantity'
    'GhiChu' = 'Note'
    'ThanhTien' = 'TotalAmount'
    
    # ===== COLLECTION PROPERTIES =====
    'CaList' = 'ShiftList'
    'SanhList' = 'HallList'
    'LoaiSanhList' = 'HallTypeList'
    'MonAnList' = 'DishList'
    'DichVuList' = 'ServiceList'
    'PhieuDatTiecList' = 'BookingList'
    
    # Collection fields
    '_caList' = '_shiftList'
    '_sanhList' = '_hallList'
    '_loaiSanhList' = '_hallTypeList'
    '_monAnList' = '_dishList'
    '_dichVuList' = '_serviceList'
    
    # ===== SELECTED ITEMS =====
    'SelectedCa' = 'SelectedShift'
    'SelectedSanh' = 'SelectedHall'
    'SelectedLoaiSanh' = 'SelectedHallType'
    'SelectedMonAn' = 'SelectedDish'
    'SelectedDichVu' = 'SelectedService'
    'SelectedPhieuDat' = 'SelectedBooking'
    
    '_selectedCa' = '_selectedShift'
    '_selectedSanh' = '_selectedHall'
    '_selectedLoaiSanh' = '_selectedHallType'
    '_selectedMonAn' = '_selectedDish'
    '_selectedDichVu' = '_selectedService'
    
    # ===== SERVICE INJECTIONS =====
    '_caService' = '_shiftService'
    '_sanhService' = '_hallService'
    '_loaiSanhService' = '_hallTypeService'
    '_monAnService' = '_dishService'
    '_dichVuService' = '_serviceService'
    '_phieuDatTiecService' = '_bookingService'
    '_thucDonService' = '_menuService'
    '_chiTietDVService' = '_serviceDetailService'
    '_chiTietServiceService' = '_serviceDetailService'
    '_thamSoService' = '_parameterService'
    '_nguoiDungService' = '_appUserService'
    '_nhomNguoiDungService' = '_userGroupService'
    '_chucNangService' = '_permissionService'
    
    # Interface parameters
    'ICaService' = 'IShiftService'
    'ISanhService' = 'IHallService'
    'ILoaiSanhService' = 'IHallTypeService'
    'IMonAnService' = 'IDishService'
    'IDichVuService' = 'IServiceService'
    'IPhieuDatTiecService' = 'IBookingService'
    'IThucDonService' = 'IMenuService'
    'IChiTietDVService' = 'IServiceDetailService'
    'IThamSoService' = 'IParameterService'
    'INguoiDungService' = 'IAppUserService'
    'INhomNguoiDungService' = 'IUserGroupService'
    'IChucNangService' = 'IPermissionService'
    
    # ===== UI SPECIFIC =====
    'NgayKhongChoChon' = 'UnavailableDates'
    'TD_SoLuong' = 'MenuQuantity'
    'TD_GhiChu' = 'MenuNote'
    'DV_SoLuong' = 'ServiceQuantity'
    'DV_GhiChu' = 'ServiceNote'
    '_td_SoLuong' = '_menuQuantity'
    '_td_GhiChu' = '_menuNote'
    '_dv_SoLuong' = '_serviceQuantity'
    '_dv_GhiChu' = '_serviceNote'
    
    # ===== METHOD NAMES =====
    'ChonMonAn' = 'SelectDish'
    'ChonDichVu' = 'SelectService'
    'ChonMonAnCommand' = 'SelectDishCommand'
    'ChonDichVuCommand' = 'SelectServiceCommand'
    'ResetTD' = 'ResetMenu'
    'ResetCTDV' = 'ResetServiceDetail'
    
    # ===== DISPLAY PROPERTIES =====
    'TrangThai' = 'Status'
    'TrangThaiBrush' = 'StatusBrush'
    'NgayHienThi' = 'DisplayDate'
    
    # ===== OBJECTS =====
    'MonAn' = 'Dish'
    'DichVu' = 'Service'
    'Ca' = 'Shift'
    'Sanh' = 'Hall'
    'LoaiSanh' = 'HallType'
    'PhieuDatTiec' = 'Booking'
}

# Find ViewModel files
$viewModelDir = Join-Path $projectRoot "Presentation\ViewModel"
if (!(Test-Path $viewModelDir)) {
    $viewModelDir = Join-Path $projectRoot "ViewModel"
}

$files = Get-ChildItem -Path $viewModelDir -Filter "*.cs" -Recurse

Write-Host "`nFound $($files.Count) ViewModel files" -ForegroundColor Green
Write-Host "Starting refactoring...`n" -ForegroundColor Yellow

$totalChanges = 0
$filesChanged = 0

foreach ($file in $files) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Gray
    
    $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    $fileChanges = 0
    
    # Apply replacements with proper word boundaries
    foreach ($oldText in $viewModelMappings.Keys) {
        $newText = $viewModelMappings[$oldText]
        
        # Handle different contexts
        if ($oldText.StartsWith("_")) {
            # Field declaration: private string _tenChuRe
            $pattern = "(\b)(private|protected|public)\s+\w+\s+$oldText(\s*[;=])"
            $content = $content -replace $pattern, "`$1`$2 $newText`$3"
            
            # Field usage: _tenChuRe = value
            $pattern = "(\b)$oldText(\s*[;=,)\]])"
            $content = $content -replace $pattern, "`$1$newText`$2"
        }
        else {
            # Property: public string TenChuRe
            $pattern = "(\b)(public|private|protected)\s+\w+\s+$oldText(\s*\{)"
            $content = $content -replace $pattern, "`$1`$2 $newText`$3"
            
            # Property access: obj.TenChuRe or TenChuRe =
            $pattern = "(\.|^|\s)$oldText(\s*[;=,)\.\s])"
            $content = $content -replace $pattern, "`$1$newText`$2"
        }
        
        # Count changes
        $beforeLength = $content.Length
        $afterLength = $content.Length
        # This is approximate, just for display
    }
    
    # Calculate actual changes
    $differences = 0
    $oldLines = $originalContent -split "`n"
    $newLines = $content -split "`n"
    for ($i = 0; $i -lt [Math]::Min($oldLines.Length, $newLines.Length); $i++) {
        if ($oldLines[$i] -ne $newLines[$i]) {
            $differences++
        }
    }
    $fileChanges = $differences
    
    # Save if changes were made
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -NoNewline
        $totalChanges += $fileChanges
        $filesChanged++
        Write-Host "  ? Changes: ~$fileChanges lines" -ForegroundColor Green
    }
    else {
        Write-Host "  - No changes" -ForegroundColor DarkGray
    }
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "VIEWMODEL REFACTORING COMPLETE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Files processed: $($files.Count)" -ForegroundColor Yellow
Write-Host "Files changed: $filesChanged" -ForegroundColor Yellow
Write-Host "Approximate changes: ~$totalChanges lines" -ForegroundColor Yellow

Write-Host "`n?? NEXT STEPS:" -ForegroundColor Magenta
Write-Host "1. Build solution (Ctrl+Shift+B)" -ForegroundColor White
Write-Host "2. Fix any compilation errors" -ForegroundColor White
Write-Host "3. Update XAML bindings" -ForegroundColor White
Write-Host "4. Test ViewModels" -ForegroundColor White

Write-Host "`nPress any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
