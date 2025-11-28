# PowerShell Script to Refactor Vietnamese naming to English
# Run this script from the project root directory

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Starting Refactoring Process..." -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# Get the script's directory (project root)
$projectRoot = $PSScriptRoot
Write-Host "Project Root: $projectRoot" -ForegroundColor Yellow

# Define file patterns to search (excluding auto-generated files and certain directories)
$includePatterns = @("*.cs", "*.xaml")
$excludeDirs = @("bin", "obj", "packages", ".vs", ".git")

# Get all files to process
$files = Get-ChildItem -Path $projectRoot -Include $includePatterns -Recurse | 
    Where-Object { 
        $exclude = $false
        foreach ($dir in $excludeDirs) {
            if ($_.FullName -like "*\$dir\*") {
                $exclude = $true
                break
            }
        }
        # Exclude auto-generated EDMX files
        if ($_.Name -like "*.Designer.cs" -or $_.Name -like "*.Context.cs" -or $_.Name -like "Model1.cs") {
            $exclude = $true
        }
        !$exclude
    }

Write-Host "`nFound $($files.Count) files to process" -ForegroundColor Green

# Define replacement mappings
# Format: @{OldText = "NewText"}
$replacements = @{
    # ===== DTO Classes =====
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
    'PHANQUYENDTO' = 'GroupPermissionDTO'
    
    # ===== Service/Repository Classes =====
    'ISanhService' = 'IHallService'
    'SanhService' = 'HallService'
    'ISanhRepository' = 'IHallRepository'
    'SanhRepository' = 'HallRepository'
    
    'ILoaiSanhService' = 'IHallTypeService'
    'LoaiSanhService' = 'HallTypeService'
    'ILoaiSanhRepository' = 'IHallTypeRepository'
    'LoaiSanhRepository' = 'HallTypeRepository'
    
    'ICaService' = 'IShiftService'
    'CaService' = 'ShiftService'
    'ICaRepository' = 'IShiftRepository'
    'CaRepository' = 'ShiftRepository'
    
    'IMonAnService' = 'IDishService'
    'MonAnService' = 'DishService'
    'IMonAnRepository' = 'IDishRepository'
    'MonAnRepository' = 'DishRepository'
    
    'IDichVuService' = 'IServiceService'
    'DichVuService' = 'ServiceService'
    'IDichVuRepository' = 'IServiceRepository'
    'DichVuRepository' = 'ServiceRepository'
    
    'IThucDonService' = 'IMenuService'
    'ThucDonService' = 'MenuService'
    'IThucDonRepository' = 'IMenuRepository'
    'ThucDonRepository' = 'MenuRepository'
    
    'IChiTietDVService' = 'IServiceDetailService'
    'ChiTietDVService' = 'ServiceDetailService'
    'IChiTietDVRepository' = 'IServiceDetailRepository'
    'ChiTietDVRepository' = 'ServiceDetailRepository'
    
    'IPhieuDatTiecService' = 'IBookingService'
    'PhieuDatTiecService' = 'BookingService'
    'IPhieuDatTiecRepository' = 'IBookingRepository'
    'PhieuDatTiecRepository' = 'BookingRepository'
    
    'INguoiDungService' = 'IAppUserService'
    'NguoiDungService' = 'AppUserService'
    'INguoiDungRepository' = 'IAppUserRepository'
    'NguoiDungRepository' = 'AppUserRepository'
    
    'INhomNguoiDungService' = 'IUserGroupService'
    'NhomNguoiDungService' = 'UserGroupService'
    'INhomNguoiDungRepository' = 'IUserGroupRepository'
    'NhomNguoiDungRepository' = 'UserGroupRepository'
    
    'IChucNangService' = 'IPermissionService'
    'ChucNangService' = 'PermissionService'
    'IChucNangRepository' = 'IPermissionRepository'
    'ChucNangRepository' = 'PermissionRepository'
    
    'IThamSoService' = 'IParameterService'
    'ThamSoService' = 'ParameterService'
    'IThamSoRepository' = 'IParameterRepository'
    'ThamSoRepository' = 'ParameterRepository'
    
    'IBaoCaoDsService' = 'IRevenueReportService'
    'BaoCaoDsService' = 'RevenueReportService'
    'IBaoCaoDsRepository' = 'IRevenueReportRepository'
    'BaoCaoDsRepository' = 'RevenueReportRepository'
    
    'ICtBaoCaoDsService' = 'IRevenueReportDetailService'
    'CtBaoCaoDsService' = 'RevenueReportDetailService'
    'ICtBaoCaoDsRepository' = 'IRevenueReportDetailRepository'
    'CtBaoCaoDsRepository' = 'RevenueReportDetailRepository'
    
    # ===== Hall (S?nh) properties =====
    '\.MaSanh\b' = '.HallId'
    '\bMaSanh\b(?=\s*[,;=)])' = 'HallId'
    '\.TenSanh\b' = '.HallName'
    '\bTenSanh\b(?=\s*[,;=)])' = 'HallName'
    '\.SoLuongBanToiDa\b' = '.MaxTableCount'
    '\bSoLuongBanToiDa\b(?=\s*[,;=)])' = 'MaxTableCount'
    
    # ===== HallType (Lo?iS?nh) properties =====
    '\.MaLoaiSanh\b' = '.HallTypeId'
    '\bMaLoaiSanh\b(?=\s*[,;=)])' = 'HallTypeId'
    '\.TenLoaiSanh\b' = '.HallTypeName'
    '\bTenLoaiSanh\b(?=\s*[,;=)])' = 'HallTypeName'
    '\.DonGiaBanToiThieu\b' = '.MinTablePrice'
    '\bDonGiaBanToiThieu\b(?=\s*[,;=)])' = 'MinTablePrice'
    '\.LoaiSanh\b' = '.HallType'
    '\bLoaiSanh\b(?=\s*[,;=)])' = 'HallType'
    
    # ===== Shift (Ca) properties =====
    '\.MaCa\b' = '.ShiftId'
    '\bMaCa\b(?=\s*[,;=)])' = 'ShiftId'
    '\.TenCa\b' = '.ShiftName'
    '\bTenCa\b(?=\s*[,;=)])' = 'ShiftName'
    '\.ThoiGianBatDauCa\b' = '.StartTime'
    '\bThoiGianBatDauCa\b(?=\s*[,;=)])' = 'StartTime'
    '\.ThoiGianKetThucCa\b' = '.EndTime'
    '\bThoiGianKetThucCa\b(?=\s*[,;=)])' = 'EndTime'
    
    # ===== Dish (MonAn) properties =====
    '\.MaMonAn\b' = '.DishId'
    '\bMaMonAn\b(?=\s*[,;=)])' = 'DishId'
    '\.TenMonAn\b' = '.DishName'
    '\bTenMonAn\b(?=\s*[,;=)])' = 'DishName'
    '\.MonAn\b' = '.Dish'
    '\bMonAn\b(?=\s*[,;=)])' = 'Dish'
    
    # ===== Service (DichVu) properties =====
    '\.MaDichVu\b' = '.ServiceId'
    '\bMaDichVu\b(?=\s*[,;=)])' = 'ServiceId'
    '\.TenDichVu\b' = '.ServiceName'
    '\bTenDichVu\b(?=\s*[,;=)])' = 'ServiceName'
    '\.DichVu\b' = '.Service'
    '\bDichVu\b(?=\s*[,;=)])' = 'Service'
    
    # ===== Booking (PhieuDatTiec) properties =====
    '\.MaPhieuDat\b' = '.BookingId'
    '\bMaPhieuDat\b(?=\s*[,;=)])' = 'BookingId'
    '\.TenChuRe\b' = '.GroomName'
    '\bTenChuRe\b(?=\s*[,;=)])' = 'GroomName'
    '\.TenCoDau\b' = '.BrideName'
    '\bTenCoDau\b(?=\s*[,;=)])' = 'BrideName'
    '\.DienThoai\b' = '.Phone'
    '\bDienThoai\b(?=\s*[,;=)])' = 'Phone'
    '\.NgayDatTiec\b' = '.BookingDate'
    '\bNgayDatTiec\b(?=\s*[,;=)])' = 'BookingDate'
    '\.NgayDaiTiec\b' = '.WeddingDate'
    '\bNgayDaiTiec\b(?=\s*[,;=)])' = 'WeddingDate'
    '\.TienDatCoc\b' = '.Deposit'
    '\bTienDatCoc\b(?=\s*[,;=)])' = 'Deposit'
    '\.SoLuongBan\b' = '.TableCount'
    '\bSoLuongBan\b(?=\s*[,;=)])' = 'TableCount'
    '\.SoBanDuTru\b' = '.ReserveTableCount'
    '\bSoBanDuTru\b(?=\s*[,;=)])' = 'ReserveTableCount'
    '\.NgayThanhToan\b' = '.PaymentDate'
    '\bNgayThanhToan\b(?=\s*[,;=)])' = 'PaymentDate'
    '\.DonGiaBanTiec\b' = '.TablePrice'
    '\bDonGiaBanTiec\b(?=\s*[,;=)])' = 'TablePrice'
    '\.TongTienBan\b' = '.TotalTableAmount'
    '\bTongTienBan\b(?=\s*[,;=)])' = 'TotalTableAmount'
    '\.TongTienDV\b' = '.TotalServiceAmount'
    '\bTongTienDV\b(?=\s*[,;=)])' = 'TotalServiceAmount'
    '\.TongTienHoaDon\b' = '.TotalInvoiceAmount'
    '\bTongTienHoaDon\b(?=\s*[,;=)])' = 'TotalInvoiceAmount'
    '\.TienConLai\b' = '.RemainingAmount'
    '\bTienConLai\b(?=\s*[,;=)])' = 'RemainingAmount'
    '\.ChiPhiPhatSinh\b' = '.AdditionalCost'
    '\bChiPhiPhatSinh\b(?=\s*[,;=)])' = 'AdditionalCost'
    '\.TienPhat\b' = '.PenaltyAmount'
    '\bTienPhat\b(?=\s*[,;=)])' = 'PenaltyAmount'
    '\.Sanh\b' = '.Hall'
    '\bSanh\b(?=\s*[,;=)])' = 'Hall'
    
    # ===== Menu (ThucDon) properties =====
    '\.SoLuong\b' = '.Quantity'
    '\bSoLuong\b(?=\s*[,;=)])' = 'Quantity'
    '\.DonGia\b' = '.UnitPrice'
    '\bDonGia\b(?=\s*[,;=)])' = 'UnitPrice'
    '\.GhiChu\b' = '.Note'
    '\bGhiChu\b(?=\s*[,;=)])' = 'Note'
    
    # ===== ServiceDetail (ChiTietDV) properties =====
    '\.ThanhTien\b' = '.TotalAmount'
    '\bThanhTien\b(?=\s*[,;=)])' = 'TotalAmount'
    
    # ===== AppUser (NguoiDung) properties =====
    '\.MaNguoiDung\b' = '.UserId'
    '\bMaNguoiDung\b(?=\s*[,;=)])' = 'UserId'
    '\.TenDangNhap\b' = '.Username'
    '\bTenDangNhap\b(?=\s*[,;=)])' = 'Username'
    '\.MatKhauHash\b' = '.PasswordHash'
    '\bMatKhauHash\b(?=\s*[,;=)])' = 'PasswordHash'
    '\.HoTen\b' = '.FullName'
    '\bHoTen\b(?=\s*[,;=)])' = 'FullName'
    '\.Email\b' = '.Email'
    '\.SoDienThoai\b' = '.PhoneNumber'
    '\bSoDienThoai\b(?=\s*[,;=)])' = 'PhoneNumber'
    '\.DiaChi\b' = '.Address'
    '\bDiaChi\b(?=\s*[,;=)])' = 'Address'
    '\.NgaySinh\b' = '.BirthDate'
    '\bNgaySinh\b(?=\s*[,;=)])' = 'BirthDate'
    '\.GioiTinh\b' = '.Gender'
    '\bGioiTinh\b(?=\s*[,;=)])' = 'Gender'
    '\.MaNhom\b' = '.GroupId'
    '\bMaNhom\b(?=\s*[,;=)])' = 'GroupId'
    '\.NhomNguoiDung\b' = '.UserGroup'
    '\bNhomNguoiDung\b(?=\s*[,;=)])' = 'UserGroup'
    
    # ===== UserGroup (NhomNguoiDung) properties =====
    '\.TenNhom\b' = '.GroupName'
    '\bTenNhom\b(?=\s*[,;=)])' = 'GroupName'
    
    # ===== Permission (ChucNang) properties =====
    '\.MaChucNang\b' = '.PermissionId'
    '\bMaChucNang\b(?=\s*[,;=)])' = 'PermissionId'
    '\.TenChucNang\b' = '.PermissionName'
    '\bTenChucNang\b(?=\s*[,;=)])' = 'PermissionName'
    '\.TenManHinhDuocLoad\b' = '.LoadedScreenName'
    '\bTenManHinhDuocLoad\b(?=\s*[,;=)])' = 'LoadedScreenName'
    
    # ===== Parameter (ThamSo) properties =====
    '\.TenThamSo\b' = '.ParameterName'
    '\bTenThamSo\b(?=\s*[,;=)])' = 'ParameterName'
    '\.GiaTri\b' = '.Value'
    '\bGiaTri\b(?=\s*[,;=)])' = 'Value'
    
    # ===== RevenueReport (BaoCaoDS) properties =====
    '\.Thang\b' = '.Month'
    '\bThang\b(?=\s*[,;=)])' = 'Month'
    '\.Nam\b' = '.Year'
    '\bNam\b(?=\s*[,;=)])' = 'Year'
    '\.TongDoanhThu\b' = '.TotalRevenue'
    '\bTongDoanhThu\b(?=\s*[,;=)])' = 'TotalRevenue'
    '\.CTBAOCAODS\b' = '.RevenueReportDetails'
    '\bCTBAOCAODS\b(?=\s*[,;=)])' = 'RevenueReportDetails'
    '\.BaoCaoD\b' = '.RevenueReport'
    '\bBaoCaoD\b(?=\s*[,;=)])' = 'RevenueReport'
    
    # ===== RevenueReportDetail (CTBaoCaoDS) properties =====
    '\.Ngay\b' = '.Day'
    '\bNgay\b(?=\s*[,;=)])' = 'Day'
    '\.SoLuongTiec\b' = '.WeddingCount'
    '\bSoLuongTiec\b(?=\s*[,;=)])' = 'WeddingCount'
    '\.DoanhThu\b' = '.Revenue'
    '\bDoanhThu\b(?=\s*[,;=)])' = 'Revenue'
    '\.TiLe\b' = '.Ratio'
    '\bTiLe\b(?=\s*[,;=)])' = 'Ratio'
    '\.NgayHienThi\b' = '.DisplayDate'
    '\bNgayHienThi\b(?=\s*[,;=)])' = 'DisplayDate'
    
    # ===== Common Navigation Properties =====
    '\.ThucDons\b' = '.Menus'
    '\bThucDons\b(?=\s*[,;=)])' = 'Menus'
    '\.ChiTietDVs\b' = '.ServiceDetails'
    '\bChiTietDVs\b(?=\s*[,;=)])' = 'ServiceDetails'
    '\.PhieuDatTiecs\b' = '.Bookings'
    '\bPhieuDatTiecs\b(?=\s*[,;=)])' = 'Bookings'
    '\.Sanhs\b' = '.Halls'
    '\bSanhs\b(?=\s*[,;=)])' = 'Halls'
    '\.Cas\b' = '.Shifts'
    '\bCas\b(?=\s*[,;=)])' = 'Shifts'
    '\.MonAns\b' = '.Dishes'
    '\bMonAns\b(?=\s*[,;=)])' = 'Dishes'
    '\.DichVus\b' = '.Services'
    '\bDichVus\b(?=\s*[,;=)])' = 'Services'
    '\.NguoiDungs\b' = '.AppUsers'
    '\bNguoiDungs\b(?=\s*[,;=)])' = 'AppUsers'
    '\.ChucNangs\b' = '.Permissions'
    '\bChucNangs\b(?=\s*[,;=)])' = 'Permissions'
}

# Counter for changes
$totalChanges = 0
$filesChanged = 0

# Backup option
$createBackup = Read-Host "`nDo you want to create backup files? (Y/N)"
$shouldBackup = $createBackup -eq "Y" -or $createBackup -eq "y"

if ($shouldBackup) {
    $backupDir = Join-Path $projectRoot "Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
    Write-Host "Backup directory created: $backupDir" -ForegroundColor Green
}

# Process each file
foreach ($file in $files) {
    Write-Host "`nProcessing: $($file.FullName.Replace($projectRoot, ''))" -ForegroundColor Gray
    
    # Read file content
    $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    $fileChanges = 0
    
    # Apply all replacements
    foreach ($oldText in $replacements.Keys) {
        $newText = $replacements[$oldText]
        
        # Use regex for word boundary patterns
        if ($oldText -match '\\b') {
            $matches = [regex]::Matches($content, $oldText)
            if ($matches.Count -gt 0) {
                $content = [regex]::Replace($content, $oldText, $newText)
                $fileChanges += $matches.Count
                Write-Host "  - Replaced '$oldText' -> '$newText' ($($matches.Count) times)" -ForegroundColor DarkYellow
            }
        }
        else {
            $count = ([regex]::Matches($content, [regex]::Escape($oldText))).Count
            if ($count -gt 0) {
                $content = $content -replace [regex]::Escape($oldText), $newText
                $fileChanges += $count
                Write-Host "  - Replaced '$oldText' -> '$newText' ($count times)" -ForegroundColor DarkYellow
            }
        }
    }
    
    # If changes were made, save the file
    if ($content -ne $originalContent) {
        # Create backup if requested
        if ($shouldBackup) {
            $relativePath = $file.FullName.Replace($projectRoot, "").TrimStart("\")
            $backupPath = Join-Path $backupDir $relativePath
            $backupFileDir = Split-Path -Parent $backupPath
            if (!(Test-Path $backupFileDir)) {
                New-Item -ItemType Directory -Path $backupFileDir -Force | Out-Null
            }
            Copy-Item -Path $file.FullName -Destination $backupPath -Force
        }
        
        # Save modified content
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -NoNewline
        
        $totalChanges += $fileChanges
        $filesChanged++
        Write-Host "  SAVED - Total changes in this file: $fileChanges" -ForegroundColor Green
    }
}

# Summary
Write-Host "`n=====================================" -ForegroundColor Cyan
Write-Host "Refactoring Complete!" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Files processed: $($files.Count)" -ForegroundColor Yellow
Write-Host "Files changed: $filesChanged" -ForegroundColor Yellow
Write-Host "Total replacements: $totalChanges" -ForegroundColor Yellow

if ($shouldBackup) {
    Write-Host "`nBackup location: $backupDir" -ForegroundColor Green
}

Write-Host "`nIMPORTANT NEXT STEPS:" -ForegroundColor Magenta
Write-Host "1. Build your solution to check for any compilation errors" -ForegroundColor White
Write-Host "2. Review changes using Git diff" -ForegroundColor White
Write-Host "3. Update any XAML bindings manually if needed" -ForegroundColor White
Write-Host "4. Test the application thoroughly" -ForegroundColor White

Write-Host "`nPress any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
