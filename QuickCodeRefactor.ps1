# Quick Code Refactoring Script
# Only refactors CODE CONTENT (class names, properties, methods)
# Does NOT rename files or update .csproj

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CODE CONTENT REFACTORING" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$projectRoot = $PSScriptRoot
Write-Host "Project Root: $projectRoot" -ForegroundColor Yellow

# All mappings for code refactoring
$mappings = @{
    # ===== DTO CLASS NAMES =====
    'SANHDTO' = 'HallDTO'
    'LOAISANHDTO' = 'HallTypeDTO'
    'CADTO' = 'ShiftDTO'
    'MONANDTO' = 'DishDTO'
    'DICHVUDTO' = 'ServiceDTO'
    'THUCDONDTO' = 'MenuDTO'
    'CHITIETDVDTO' = 'ServiceDetailDTO'
    'PHIEUDATTIECDTO' = 'BookingDTO'
    'NGUOIDUNGDTO' = 'AppUserDTO'
    'NHOMNGUOIDUNGDTO' = 'UserGroupDTO'
    'CHUCNANGDTO' = 'PermissionDTO'
    'THAMSODTO' = 'ParameterDTO'
    'BAOCAODDTO' = 'RevenueReportDTO'
    'CTBAOCAODDTO' = 'RevenueReportDetailDTO'
    
    # ===== SERVICE/REPOSITORY NAMES =====
    'SanhService' = 'HallService'
    'ISanhService' = 'IHallService'
    'SanhRepository' = 'HallRepository'
    'ISanhRepository' = 'IHallRepository'
    
    'LoaiSanhService' = 'HallTypeService'
    'ILoaiSanhService' = 'IHallTypeService'
    'LoaiSanhRepository' = 'HallTypeRepository'
    'ILoaiSanhRepository' = 'IHallTypeRepository'
    
    'CaService' = 'ShiftService'
    'ICaService' = 'IShiftService'
    'CaRepository' = 'ShiftRepository'
    'ICaRepository' = 'IShiftRepository'
    
    'MonAnService' = 'DishService'
    'IMonAnService' = 'IDishService'
    'MonAnRepository' = 'DishRepository'
    'IMonAnRepository' = 'IDishRepository'
    
    'DichVuService' = 'ServiceService'
    'IDichVuService' = 'IServiceService'
    'DichVuRepository' = 'ServiceRepository'
    'IDichVuRepository' = 'IServiceRepository'
    
    'ThucDonService' = 'MenuService'
    'IThucDonService' = 'IMenuService'
    'ThucDonRepository' = 'MenuRepository'
    'IThucDonRepository' = 'IMenuRepository'
    
    'ChiTietDVService' = 'ServiceDetailService'
    'IChiTietDVService' = 'IServiceDetailService'
    'ChiTietDVRepository' = 'ServiceDetailRepository'
    'IChiTietDVRepository' = 'IServiceDetailRepository'
    
    'PhieuDatTiecService' = 'BookingService'
    'IPhieuDatTiecService' = 'IBookingService'
    'PhieuDatTiecRepository' = 'BookingRepository'
    'IPhieuDatTiecRepository' = 'IBookingRepository'
    
    'NguoiDungService' = 'AppUserService'
    'INguoiDungService' = 'IAppUserService'
    'NguoiDungRepository' = 'AppUserRepository'
    'INguoiDungRepository' = 'IAppUserRepository'
    
    'NhomNguoiDungService' = 'UserGroupService'
    'INhomNguoiDungService' = 'IUserGroupService'
    'NhomNguoiDungRepository' = 'UserGroupRepository'
    'INhomNguoiDungRepository' = 'IUserGroupRepository'
    
    'ChucNangService' = 'PermissionService'
    'IChucNangService' = 'IPermissionService'
    'ChucNangRepository' = 'PermissionRepository'
    'IChucNangRepository' = 'IPermissionRepository'
    
    'ThamSoService' = 'ParameterService'
    'IThamSoService' = 'IParameterService'
    'ThamSoRepository' = 'ParameterRepository'
    'IThamSoRepository' = 'IParameterRepository'
    
    'BaoCaoDsService' = 'RevenueReportService'
    'IBaoCaoDsService' = 'IRevenueReportService'
    'BaoCaoDsRepository' = 'RevenueReportRepository'
    'IBaoCaoDsRepository' = 'IRevenueReportRepository'
    
    'CtBaoCaoDsService' = 'RevenueReportDetailService'
    'ICtBaoCaoDsService' = 'IRevenueReportDetailService'
    'CtBaoCaoDsRepository' = 'RevenueReportDetailRepository'
    'ICtBaoCaoDsRepository' = 'IRevenueReportDetailRepository'
    
    # ===== PROPERTIES - Booking =====
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
    
    # ===== PROPERTIES - Hall =====
    'MaSanh' = 'HallId'
    'TenSanh' = 'HallName'
    'SoLuongBanToiDa' = 'MaxTableCount'
    
    # ===== PROPERTIES - HallType =====
    'MaLoaiSanh' = 'HallTypeId'
    'TenLoaiSanh' = 'HallTypeName'
    'DonGiaBanToiThieu' = 'MinTablePrice'
    
    # ===== PROPERTIES - Shift =====
    'MaCa' = 'ShiftId'
    'TenCa' = 'ShiftName'
    'ThoiGianBatDauCa' = 'StartTime'
    'ThoiGianKetThucCa' = 'EndTime'
    
    # ===== PROPERTIES - Dish =====
    'MaMonAn' = 'DishId'
    'TenMonAn' = 'DishName'
    
    # ===== PROPERTIES - Service =====
    'MaDichVu' = 'ServiceId'
    'TenDichVu' = 'ServiceName'
    
    # ===== PROPERTIES - Common =====
    'DonGia' = 'UnitPrice'
    'SoLuong' = 'Quantity'
    'GhiChu' = 'Note'
    'ThanhTien' = 'TotalAmount'
    
    # ===== PROPERTIES - User =====
    'MaNguoiDung' = 'UserId'
    'TenDangNhap' = 'Username'
    'MatKhauHash' = 'PasswordHash'
    'HoTen' = 'FullName'
    'SoDienThoai' = 'PhoneNumber'
    'DiaChi' = 'Address'
    'NgaySinh' = 'BirthDate'
    'GioiTinh' = 'Gender'
    'MaNhom' = 'GroupId'
    
    # ===== PROPERTIES - Others =====
    'TenNhom' = 'GroupName'
    'MaChucNang' = 'PermissionId'
    'TenChucNang' = 'PermissionName'
    'TenManHinhDuocLoad' = 'LoadedScreenName'
    'TenThamSo' = 'ParameterName'
    'GiaTri' = 'Value'
    'Thang' = 'Month'
    'Nam' = 'Year'
    'Ngay' = 'Day'
    'TongDoanhThu' = 'TotalRevenue'
    'DoanhThu' = 'Revenue'
    'SoLuongTiec' = 'WeddingCount'
    'TiLe' = 'Ratio'
    
    # ===== NAVIGATION PROPERTIES (PascalCase) =====
    'LoaiSanh' = 'HallType'
    'Sanh' = 'Hall'
    'Ca' = 'Shift'
    'MonAn' = 'Dish'
    'DichVu' = 'Service'
    'NhomNguoiDung' = 'UserGroup'
    'PhieuDatTiec' = 'Booking'
    
    # ===== COLLECTIONS =====
    'Sanhs' = 'Halls'
    'Cas' = 'Shifts'
    'MonAns' = 'Dishes'
    'DichVus' = 'Services'
    'PhieuDatTiecs' = 'Bookings'
    'ThucDons' = 'Menus'
    'ChiTietDVs' = 'ServiceDetails'
    'NguoiDungs' = 'AppUsers'
    'ChucNangs' = 'Permissions'
    
    # ===== VIEWMODEL VARIABLES (private fields) =====
    '_selectedCa' = '_selectedShift'
    '_selectedSanh' = '_selectedHall'
    '_selectedLoaiSanh' = '_selectedHallType'
    '_selectedMonAn' = '_selectedDish'
    '_selectedDichVu' = '_selectedService'
    '_caList' = '_shiftList'
    '_sanhList' = '_hallList'
    '_loaiSanhList' = '_hallTypeList'
    '_monAnList' = '_dishList'
    '_dichVuList' = '_serviceList'
    '_tenChuRe' = '_groomName'
    '_tenCoDau' = '_brideName'
    '_dienThoai' = '_phone'
    '_ngayDatTiec' = '_bookingDate'
    '_ngayDaiTiec' = '_weddingDate'
    '_tienDatCoc' = '_deposit'
    '_soLuongBan' = '_tableCount'
    '_soBanDuTru' = '_reserveTableCount'
    '_td_SoLuong' = '_menuQuantity'
    '_td_GhiChu' = '_menuNote'
    '_dv_SoLuong' = '_serviceQuantity'
    '_dv_GhiChu' = '_serviceNote'
    
    # ===== VIEWMODEL PROPERTIES (PascalCase) =====
    'SelectedCa' = 'SelectedShift'
    'SelectedSanh' = 'SelectedHall'
    'SelectedLoaiSanh' = 'SelectedHallType'
    'SelectedMonAn' = 'SelectedDish'
    'SelectedDichVu' = 'SelectedService'
    'CaList' = 'ShiftList'
    'SanhList' = 'HallList'
    'LoaiSanhList' = 'HallTypeList'
    'MonAnList' = 'DishList'
    'DichVuList' = 'ServiceList'
    'TD_SoLuong' = 'MenuQuantity'
    'TD_GhiChu' = 'MenuNote'
    'DV_SoLuong' = 'ServiceQuantity'
    'DV_GhiChu' = 'ServiceNote'
    'NgayKhongChoChon' = 'UnavailableDates'
    'TrangThai' = 'Status'
    'TrangThaiBrush' = 'StatusBrush'
    'NgayHienThi' = 'DisplayDate'
    
    # ===== SERVICE INJECTIONS (private fields) =====
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
    '_BookingService' = '_bookingService'
    '_DishService' = '_dishService'
    '_ServiceService' = '_serviceService'
    
    # ===== METHOD PARAMETERS (camelCase) =====
    'maPhieuDat' = 'bookingId'
    'maSanh' = 'hallId'
    'maLoaiSanh' = 'hallTypeId'
    'maCa' = 'shiftId'
    'maMonAn' = 'dishId'
    'maDichVu' = 'serviceId'
    'phieuDatTiec' = 'booking'
    'loaiSanh' = 'hallType'
    
    # ===== METHOD NAMES =====
    'ChonMonAn' = 'SelectDish'
    'ChonDichVu' = 'SelectService'
    'ChonMonAnCommand' = 'SelectDishCommand'
    'ChonDichVuCommand' = 'SelectServiceCommand'
    'ResetTD' = 'ResetMenu'
    'ResetCTDV' = 'ResetServiceDetail'
}

# Get all files (exclude Model folder and auto-generated)
$excludeDirs = @("bin", "obj", "packages", ".vs", ".git", "Backup_*", "Model")
$files = Get-ChildItem -Path $projectRoot -Include @("*.cs") -Recurse | 
    Where-Object { 
        $exclude = $false
        foreach ($dir in $excludeDirs) {
            if ($_.FullName -like "*\$dir\*") {
                $exclude = $true
                break
            }
        }
        if ($_.Name -like "*.Designer.cs" -or 
            $_.Name -like "*.Context.cs" -or 
            $_.Name -like "Model1.cs") {
            $exclude = $true
        }
        !$exclude
    }

Write-Host "`nFound $($files.Count) files to process" -ForegroundColor Green

$totalChanges = 0
$filesChanged = 0

foreach ($file in $files) {
    $relativePath = $file.FullName.Replace($projectRoot, "")
    Write-Host "`nProcessing: $relativePath" -ForegroundColor Gray
    
    $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    
    # Apply replacements
    foreach ($oldText in $mappings.Keys) {
        $newText = $mappings[$oldText]
        
        # Use word boundaries to avoid partial matches
        $pattern = "\b$([regex]::Escape($oldText))\b"
        $matches = [regex]::Matches($content, $pattern)
        
        if ($matches.Count -gt 0) {
            $content = [regex]::Replace($content, $pattern, $newText)
            Write-Host "  - $oldText → $newText ($($matches.Count)x)" -ForegroundColor DarkYellow
        }
    }
    
    # Save if changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -NoNewline
        $filesChanged++
        Write-Host "  ✓ SAVED" -ForegroundColor Green
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "✅ REFACTORING COMPLETE!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Files changed: $filesChanged / $($files.Count)" -ForegroundColor Yellow

Write-Host "`n📋 NEXT STEPS:" -ForegroundColor Magenta
Write-Host "1. Build solution (Ctrl+Shift+B)" -ForegroundColor White
Write-Host "2. Fix any compilation errors" -ForegroundColor White
Write-Host "3. Test the application" -ForegroundColor White

Write-Host "`nPress any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
