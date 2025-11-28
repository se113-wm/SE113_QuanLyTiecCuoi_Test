# PowerShell Script to Update .csproj file references
# This script updates <Compile Include='...'> paths in the project file

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Updating .csproj File References" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

$projectRoot = $PSScriptRoot
Write-Host "Project Root: $projectRoot" -ForegroundColor Yellow

# Find .csproj file
$csprojFile = Get-ChildItem -Path $projectRoot -Filter "*.csproj" -File | Select-Object -First 1

if (-not $csprojFile) {
    Write-Host "ERROR: No .csproj file found in project root!" -ForegroundColor Red
    exit 1
}

Write-Host "Found project file: $($csprojFile.Name)" -ForegroundColor Green

# Define file path mappings (old -> new)
$pathMappings = @{
    # DTO Files
    'DataTransferObject\\SANHDTO.cs' = 'DataTransferObject\\HallDTO.cs'
    'DataTransferObject\\LOAISANHDTO.cs' = 'DataTransferObject\\HallTypeDTO.cs'
    'DataTransferObject\\CADTO.cs' = 'DataTransferObject\\ShiftDTO.cs'
    'DataTransferObject\\MONANDTO.cs' = 'DataTransferObject\\DishDTO.cs'
    'DataTransferObject\\DICHVUDTO.cs' = 'DataTransferObject\\ServiceDTO.cs'
    'DataTransferObject\\THUCDONDTO.cs' = 'DataTransferObject\\MenuDTO.cs'
    'DataTransferObject\\CHITIETDVDTO.cs' = 'DataTransferObject\\ServiceDetailDTO.cs'
    'DataTransferObject\\PHIEUDATTIECDTO.cs' = 'DataTransferObject\\BookingDTO.cs'
    'DataTransferObject\\NGUOIDUNGDTO.cs' = 'DataTransferObject\\AppUserDTO.cs'
    'DataTransferObject\\NHOMNGUOIDUNGDTO.cs' = 'DataTransferObject\\UserGroupDTO.cs'
    'DataTransferObject\\CHUCNANGDTO.cs' = 'DataTransferObject\\PermissionDTO.cs'
    'DataTransferObject\\THAMSODTO.cs' = 'DataTransferObject\\ParameterDTO.cs'
    'DataTransferObject\\BAOCAODDTO.cs' = 'DataTransferObject\\RevenueReportDTO.cs'
    'DataTransferObject\\CTBAOCAODDTO.cs' = 'DataTransferObject\\RevenueReportDetailDTO.cs'
    
    # Repository Files
    'DataAccessLayer\\Repository\\SanhRepository.cs' = 'DataAccessLayer\\Repository\\HallRepository.cs'
    'DataAccessLayer\\IRepository\\ISanhRepository.cs' = 'DataAccessLayer\\IRepository\\IHallRepository.cs'
    'DataAccessLayer\\Repository\\LoaiSanhRepository.cs' = 'DataAccessLayer\\Repository\\HallTypeRepository.cs'
    'DataAccessLayer\\IRepository\\ILoaiSanhRepository.cs' = 'DataAccessLayer\\IRepository\\IHallTypeRepository.cs'
    'DataAccessLayer\\Repository\\CaRepository.cs' = 'DataAccessLayer\\Repository\\ShiftRepository.cs'
    'DataAccessLayer\\IRepository\\ICaRepository.cs' = 'DataAccessLayer\\IRepository\\IShiftRepository.cs'
    'DataAccessLayer\\Repository\\MonAnRepository.cs' = 'DataAccessLayer\\Repository\\DishRepository.cs'
    'DataAccessLayer\\IRepository\\IMonAnRepository.cs' = 'DataAccessLayer\\IRepository\\IDishRepository.cs'
    'DataAccessLayer\\Repository\\DichVuRepository.cs' = 'DataAccessLayer\\Repository\\ServiceRepository.cs'
    'DataAccessLayer\\IRepository\\IDichVuRepository.cs' = 'DataAccessLayer\\IRepository\\IServiceRepository.cs'
    'DataAccessLayer\\Repository\\ThucDonRepository.cs' = 'DataAccessLayer\\Repository\\MenuRepository.cs'
    'DataAccessLayer\\IRepository\\IThucDonRepository.cs' = 'DataAccessLayer\\IRepository\\IMenuRepository.cs'
    'DataAccessLayer\\Repository\\ChiTietDVRepository.cs' = 'DataAccessLayer\\Repository\\ServiceDetailRepository.cs'
    'DataAccessLayer\\IRepository\\IChiTietDVRepository.cs' = 'DataAccessLayer\\IRepository\\IServiceDetailRepository.cs'
    'DataAccessLayer\\Repository\\PhieuDatTiecRepository.cs' = 'DataAccessLayer\\Repository\\BookingRepository.cs'
    'DataAccessLayer\\IRepository\\IPhieuDatTiecRepository.cs' = 'DataAccessLayer\\IRepository\\IBookingRepository.cs'
    'DataAccessLayer\\Repository\\NguoiDungRepository.cs' = 'DataAccessLayer\\Repository\\AppUserRepository.cs'
    'DataAccessLayer\\IRepository\\INguoiDungRepository.cs' = 'DataAccessLayer\\IRepository\\IAppUserRepository.cs'
    'DataAccessLayer\\Repository\\NhomNguoiDungRepository.cs' = 'DataAccessLayer\\Repository\\UserGroupRepository.cs'
    'DataAccessLayer\\IRepository\\INhomNguoiDungRepository.cs' = 'DataAccessLayer\\IRepository\\IUserGroupRepository.cs'
    'DataAccessLayer\\Repository\\ChucNangRepository.cs' = 'DataAccessLayer\\Repository\\PermissionRepository.cs'
    'DataAccessLayer\\IRepository\\IChucNangRepository.cs' = 'DataAccessLayer\\IRepository\\IPermissionRepository.cs'
    'DataAccessLayer\\Repository\\ThamSoRepository.cs' = 'DataAccessLayer\\Repository\\ParameterRepository.cs'
    'DataAccessLayer\\IRepository\\IThamSoRepository.cs' = 'DataAccessLayer\\IRepository\\IParameterRepository.cs'
    'DataAccessLayer\\Repository\\BaoCaoDsRepository.cs' = 'DataAccessLayer\\Repository\\RevenueReportRepository.cs'
    'DataAccessLayer\\IRepository\\IBaoCaoDsRepository.cs' = 'DataAccessLayer\\IRepository\\IRevenueReportRepository.cs'
    'DataAccessLayer\\Repository\\CtBaoCaoDsRepository.cs' = 'DataAccessLayer\\Repository\\RevenueReportDetailRepository.cs'
    'DataAccessLayer\\IRepository\\ICtBaoCaoDsRepository.cs' = 'DataAccessLayer\\IRepository\\IRevenueReportDetailRepository.cs'
    
    # Service Files
    'BusinessLogicLayer\\Service\\SanhService.cs' = 'BusinessLogicLayer\\Service\\HallService.cs'
    'BusinessLogicLayer\\IService\\ISanhService.cs' = 'BusinessLogicLayer\\IService\\IHallService.cs'
    'BusinessLogicLayer\\Service\\LoaiSanhService.cs' = 'BusinessLogicLayer\\Service\\HallTypeService.cs'
    'BusinessLogicLayer\\IService\\ILoaiSanhService.cs' = 'BusinessLogicLayer\\IService\\IHallTypeService.cs'
    'BusinessLogicLayer\\Service\\CaService.cs' = 'BusinessLogicLayer\\Service\\ShiftService.cs'
    'BusinessLogicLayer\\IService\\ICaService.cs' = 'BusinessLogicLayer\\IService\\IShiftService.cs'
    'BusinessLogicLayer\\Service\\MonAnService.cs' = 'BusinessLogicLayer\\Service\\DishService.cs'
    'BusinessLogicLayer\\IService\\IMonAnService.cs' = 'BusinessLogicLayer\\IService\\IDishService.cs'
    'BusinessLogicLayer\\Service\\DichVuService.cs' = 'BusinessLogicLayer\\Service\\ServiceService.cs'
    'BusinessLogicLayer\\IService\\IDichVuService.cs' = 'BusinessLogicLayer\\IService\\IServiceService.cs'
    'BusinessLogicLayer\\Service\\ThucDonService.cs' = 'BusinessLogicLayer\\Service\\MenuService.cs'
    'BusinessLogicLayer\\IService\\IThucDonService.cs' = 'BusinessLogicLayer\\IService\\IMenuService.cs'
    'BusinessLogicLayer\\Service\\ChiTietDVService.cs' = 'BusinessLogicLayer\\Service\\ServiceDetailService.cs'
    'BusinessLogicLayer\\IService\\IChiTietDVService.cs' = 'BusinessLogicLayer\\IService\\IServiceDetailService.cs'
    'BusinessLogicLayer\\Service\\PhieuDatTiecService.cs' = 'BusinessLogicLayer\\Service\\BookingService.cs'
    'BusinessLogicLayer\\IService\\IPhieuDatTiecService.cs' = 'BusinessLogicLayer\\IService\\IBookingService.cs'
    'BusinessLogicLayer\\Service\\NguoiDungService.cs' = 'BusinessLogicLayer\\Service\\AppUserService.cs'
    'BusinessLogicLayer\\IService\\INguoiDungService.cs' = 'BusinessLogicLayer\\IService\\IAppUserService.cs'
    'BusinessLogicLayer\\Service\\NhomNguoiDungService.cs' = 'BusinessLogicLayer\\Service\\UserGroupService.cs'
    'BusinessLogicLayer\\IService\\INhomNguoiDungService.cs' = 'BusinessLogicLayer\\IService\\IUserGroupService.cs'
    'BusinessLogicLayer\\Service\\ChucNangService.cs' = 'BusinessLogicLayer\\Service\\PermissionService.cs'
    'BusinessLogicLayer\\IService\\IChucNangService.cs' = 'BusinessLogicLayer\\IService\\IPermissionService.cs'
    'BusinessLogicLayer\\Service\\ThamSoService.cs' = 'BusinessLogicLayer\\Service\\ParameterService.cs'
    'BusinessLogicLayer\\IService\\IThamSoService.cs' = 'BusinessLogicLayer\\IService\\IParameterService.cs'
    'BusinessLogicLayer\\Service\\BaoCaoDsService.cs' = 'BusinessLogicLayer\\Service\\RevenueReportService.cs'
    'BusinessLogicLayer\\IService\\IBaoCaoDsService.cs' = 'BusinessLogicLayer\\IService\\IRevenueReportService.cs'
    'BusinessLogicLayer\\Service\\CtBaoCaoDsService.cs' = 'BusinessLogicLayer\\Service\\RevenueReportDetailService.cs'
    'BusinessLogicLayer\\IService\\ICtBaoCaoDsService.cs' = 'BusinessLogicLayer\\IService\\IRevenueReportDetailService.cs'
}

# Backup .csproj file
$backupPath = "$($csprojFile.FullName).backup"
Copy-Item -Path $csprojFile.FullName -Destination $backupPath -Force
Write-Host "`nBackup created: $backupPath" -ForegroundColor Green

# Read .csproj content
Write-Host "`nReading project file..." -ForegroundColor Yellow
$content = Get-Content -Path $csprojFile.FullName -Raw -Encoding UTF8

# Apply replacements
$changesCount = 0
foreach ($oldPath in $pathMappings.Keys) {
    $newPath = $pathMappings[$oldPath]
    
    # Escape backslashes for regex
    $oldPathEscaped = [regex]::Escape($oldPath)
    
    # Check if path exists in content
    if ($content -match $oldPathEscaped) {
        $content = $content -replace $oldPathEscaped, $newPath
        $changesCount++
        Write-Host "  ? Updated: $oldPath -> $newPath" -ForegroundColor Green
    }
}

# Save updated content
if ($changesCount -gt 0) {
    Set-Content -Path $csprojFile.FullName -Value $content -Encoding UTF8 -NoNewline
    Write-Host "`n? Project file updated successfully!" -ForegroundColor Green
    Write-Host "Total path updates: $changesCount" -ForegroundColor Yellow
}
else {
    Write-Host "`n??  No changes made to project file." -ForegroundColor Yellow
    Write-Host "This might mean:" -ForegroundColor Gray
    Write-Host "  - Files were already renamed in the project" -ForegroundColor Gray
    Write-Host "  - Project uses wildcard includes" -ForegroundColor Gray
    Write-Host "  - Files need to be manually updated in Visual Studio" -ForegroundColor Gray
}

Write-Host "`n=====================================" -ForegroundColor Cyan
Write-Host "Project File Update Complete!" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

if ($changesCount -gt 0) {
    Write-Host "`n?? Next Steps:" -ForegroundColor Magenta
    Write-Host "1. Open Visual Studio" -ForegroundColor White
    Write-Host "2. Reload the project when prompted" -ForegroundColor White
    Write-Host "3. Rebuild the solution" -ForegroundColor White
    Write-Host "4. Fix any remaining errors" -ForegroundColor White
}

Write-Host "`nPress any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
