# Complete Refactoring Script for All Entities
# This script refactors: Repository -> Service -> DTO -> ViewModel
# Run from project root directory

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "COMPLETE ENTITY REFACTORING" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$projectRoot = $PSScriptRoot
Write-Host "Project Root: $projectRoot" -ForegroundColor Yellow

# Backup confirmation
$createBackup = Read-Host "`nCreate backup before refactoring? (Y/N)"
$shouldBackup = $createBackup -eq "Y" -or $createBackup -eq "y"

if ($shouldBackup) {
    $backupDir = Join-Path $projectRoot "Backup_Complete_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    Write-Host "Creating backup at: $backupDir" -ForegroundColor Yellow
    
    # Copy entire project except bin, obj, packages
    $excludeDirs = @("bin", "obj", "packages", ".vs", ".git", "Backup_*")
    
    Get-ChildItem -Path $projectRoot -Directory | Where-Object {
        $exclude = $false
        foreach ($dir in $excludeDirs) {
            if ($_.Name -like $dir) {
                $exclude = $true
                break
            }
        }
        !$exclude
    } | ForEach-Object {
        $destPath = Join-Path $backupDir $_.Name
        Copy-Item -Path $_.FullName -Destination $destPath -Recurse -Force
    }
    
    # Copy root files
    Get-ChildItem -Path $projectRoot -File | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination $backupDir -Force
    }
    
    Write-Host "? Backup created successfully!" -ForegroundColor Green
}

# Define ALL entity mappings
$entityMappings = @{
    # ===== COMPLETE PROPERTY MAPPINGS =====
    
    # Booking (PhieuDatTiec) - DONE
    'MaPhieuDat' = 'BookingId'
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
    
    # Hall (Sanh)
    'MaSanh' = 'HallId'
    'TenSanh' = 'HallName'
    'SoLuongBanToiDa' = 'MaxTableCount'
    
    # HallType (LoaiSanh)
    'MaLoaiSanh' = 'HallTypeId'
    'TenLoaiSanh' = 'HallTypeName'
    'DonGiaBanToiThieu' = 'MinTablePrice'
    
    # Shift (Ca)
    'MaCa' = 'ShiftId'
    'TenCa' = 'ShiftName'
    'ThoiGianBatDauCa' = 'StartTime'
    'ThoiGianKetThucCa' = 'EndTime'
    
    # Dish (MonAn)
    'MaMonAn' = 'DishId'
    'TenMonAn' = 'DishName'
    
    # Service (DichVu)
    'MaDichVu' = 'ServiceId'
    'TenDichVu' = 'ServiceName'
    
    # Common Properties
    'DonGia' = 'UnitPrice'
    'SoLuong' = 'Quantity'
    'GhiChu' = 'Note'
    'ThanhTien' = 'TotalAmount'
    
    # User (NguoiDung)
    'MaNguoiDung' = 'UserId'
    'TenDangNhap' = 'Username'
    'MatKhauHash' = 'PasswordHash'
    'HoTen' = 'FullName'
    'SoDienThoai' = 'PhoneNumber'
    'DiaChi' = 'Address'
    'NgaySinh' = 'BirthDate'
    'GioiTinh' = 'Gender'
    'MaNhom' = 'GroupId'
    
    # UserGroup (NhomNguoiDung)
    'TenNhom' = 'GroupName'
    
    # Permission (ChucNang)
    'MaChucNang' = 'PermissionId'
    'TenChucNang' = 'PermissionName'
    'TenManHinhDuocLoad' = 'LoadedScreenName'
    
    # Parameter (ThamSo)
    'TenThamSo' = 'ParameterName'
    'GiaTri' = 'Value'
    
    # Revenue Report (BaoCaoDS)
    'Thang' = 'Month'
    'Nam' = 'Year'
    'Ngay' = 'Day'
    'TongDoanhThu' = 'TotalRevenue'
    'DoanhThu' = 'Revenue'
    'SoLuongTiec' = 'WeddingCount'
    'TiLe' = 'Ratio'
    
    # Navigation Properties
    'LoaiSanh' = 'HallType'
    'Sanh' = 'Hall'
    'Ca' = 'Shift'
    'MonAn' = 'Dish'
    'DichVu' = 'Service'
    'NhomNguoiDung' = 'UserGroup'
    'PhieuDatTiec' = 'Booking'
    
    # Collections
    'Sanhs' = 'Halls'
    'Cas' = 'Shifts'
    'MonAns' = 'Dishes'
    'DichVus' = 'Services'
    'PhieuDatTiecs' = 'Bookings'
    'ThucDons' = 'Menus'
    'ChiTietDVs' = 'ServiceDetails'
    'NguoiDungs' = 'AppUsers'
    'ChucNangs' = 'Permissions'
    
    # ViewModel Specific
    'CaList' = 'ShiftList'
    'SanhList' = 'HallList'
    'LoaiSanhList' = 'HallTypeList'
    'MonAnList' = 'DishList'
    'DichVuList' = 'ServiceList'
    'SelectedCa' = 'SelectedShift'
    'SelectedSanh' = 'SelectedHall'
    'SelectedLoaiSanh' = 'SelectedHallType'
    'SelectedMonAn' = 'SelectedDish'
    'SelectedDichVu' = 'SelectedService'
    
    # Variable names in ViewModels
    '_selectedCa' = '_selectedShift'
    '_selectedSanh' = '_selectedHall'
    '_caList' = '_shiftList'
    '_sanhList' = '_hallList'
    '_tenChuRe' = '_groomName'
    '_tenCoDau' = '_brideName'
    '_dienThoai' = '_phone'
    '_ngayDatTiec' = '_bookingDate'
    '_ngayDaiTiec' = '_weddingDate'
    '_tienDatCoc' = '_deposit'
    '_soLuongBan' = '_tableCount'
    '_soBanDuTru' = '_reserveTableCount'
    
    # Service/Repository variable names
    '_hallService' = '_hallService'
    '_caService' = '_shiftService'
    '_BookingService' = '_bookingService'
    '_DishService' = '_dishService'
    '_ServiceService' = '_serviceService'
    '_loaiSanhService' = '_hallTypeService'
    '_sanhService' = '_hallService'
    
    # Method parameter names
    'maPhieuDat' = 'bookingId'
    'maSanh' = 'hallId'
    'maLoaiSanh' = 'hallTypeId'
    'maCa' = 'shiftId'
    'maMonAn' = 'dishId'
    'maDichVu' = 'serviceId'
    'phieuDatTiec' = 'booking'
    'loaiSanh' = 'hallType'
    
    # UI Element names
    'NgayKhongChoChon' = 'UnavailableDates'
    'TD_SoLuong' = 'MenuQuantity'
    'TD_GhiChu' = 'MenuNote'
    'DV_SoLuong' = 'ServiceQuantity'
    'DV_GhiChu' = 'ServiceNote'
    'ChonMonAn' = 'SelectDish'
    'ChonDichVu' = 'SelectService'
    'ChonMonAnCommand' = 'SelectDishCommand'
    'ChonDichVuCommand' = 'SelectServiceCommand'
    
    # Status/Display
    'TrangThai' = 'Status'
    'TrangThaiBrush' = 'StatusBrush'
    'NgayHienThi' = 'DisplayDate'
}

# Get all C# and XAML files (exclude auto-generated)
$excludeDirs = @("bin", "obj", "packages", ".vs", ".git", "Backup_*")
$files = Get-ChildItem -Path $projectRoot -Include @("*.cs", "*.xaml") -Recurse | 
    Where-Object { 
        $exclude = $false
        foreach ($dir in $excludeDirs) {
            if ($_.FullName -like "*\$dir\*") {
                $exclude = $true
                break
            }
        }
        # Exclude EDMX auto-generated files
        if ($_.Name -like "*.Designer.cs" -or 
            $_.Name -like "*.Context.cs" -or 
            $_.Name -like "Model1.cs" -or
            $_.DirectoryName -like "*\Model" -and $_.Extension -eq ".cs") {
            $exclude = $true
        }
        !$exclude
    }

Write-Host "`nFound $($files.Count) files to process" -ForegroundColor Green
Write-Host "Starting refactoring..." -ForegroundColor Yellow

$totalChanges = 0
$filesChanged = 0

foreach ($file in $files) {
    $relativePath = $file.FullName.Replace($projectRoot, "")
    Write-Host "`nProcessing: $relativePath" -ForegroundColor Gray
    
    $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    $fileChanges = 0
    
    # Apply replacements using word boundaries
    foreach ($oldText in $entityMappings.Keys) {
        $newText = $entityMappings[$oldText]
        
        # Different regex patterns for different contexts
        $patterns = @(
            # Property/Field declarations: public string TenChuRe
            "(\b(public|private|protected|internal)\s+\w+\s+)$oldText(\s*\{)",
            # Variable names: _tenChuRe
            "(\b)$oldText(\b)(?=\s*[;,=)\]])",
            # Method parameters: (string tenChuRe)
            "(\(\s*\w+\s+)$oldText(\s*[,)])",
            # Property access: x.TenChuRe
            "(\.|\s)$oldText(\s*[;,=)\.])",
            # XAML bindings: {Binding TenChuRe}
            "(\{Binding\s+)$oldText(\s*[,}])"
        )
        
        foreach ($pattern in $patterns) {
            $regex = $pattern -replace '\$oldText', [regex]::Escape($oldText)
            $matches = [regex]::Matches($content, $regex)
            
            if ($matches.Count -gt 0) {
                $replacement = "`$1$newText`$2"
                $content = [regex]::Replace($content, $regex, $replacement)
                $fileChanges += $matches.Count
            }
        }
    }
    
    # Save if changes were made
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -NoNewline
        $totalChanges += $fileChanges
        $filesChanged++
        Write-Host "  ? SAVED - Changes: $fileChanges" -ForegroundColor Green
    }
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "REFACTORING COMPLETE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Files processed: $($files.Count)" -ForegroundColor Yellow
Write-Host "Files changed: $filesChanged" -ForegroundColor Yellow
Write-Host "Total replacements: $totalChanges" -ForegroundColor Yellow

if ($shouldBackup) {
    Write-Host "`nBackup location: $backupDir" -ForegroundColor Green
}

Write-Host "`n?? NEXT STEPS:" -ForegroundColor Magenta
Write-Host "1. Build solution (Ctrl+Shift+B)" -ForegroundColor White
Write-Host "2. Fix any compilation errors" -ForegroundColor White
Write-Host "3. Update XAML bindings if needed" -ForegroundColor White
Write-Host "4. Test all functionality" -ForegroundColor White
Write-Host "5. Commit changes to Git" -ForegroundColor White

Write-Host "`nPress any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
