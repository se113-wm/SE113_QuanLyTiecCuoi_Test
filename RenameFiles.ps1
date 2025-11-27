# PowerShell Script to Rename Vietnamese files to English
# This script renames physical files and updates project references
# Run this AFTER RefactorToEnglish.ps1

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "File Renaming Process..." -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# Get the script's directory (project root)
$projectRoot = $PSScriptRoot
Write-Host "Project Root: $projectRoot" -ForegroundColor Yellow

# Define file rename mappings
# Format: @{OldFileName = "NewFileName"}
$fileRenameMappings = @{
    # ===== DTO Files =====
    'SANHDTO.cs' = 'HallDTO.cs'
    'LOAISANHDTO.cs' = 'HallTypeDTO.cs'
    'CADTO.cs' = 'ShiftDTO.cs'
    'MONANDTO.cs' = 'DishDTO.cs'
    'DICHVUDTO.cs' = 'ServiceDTO.cs'
    'THUCDONDTO.cs' = 'MenuDTO.cs'
    'CHITIETDVDTO.cs' = 'ServiceDetailDTO.cs'
    'PHIEUDATTIECDTO.cs' = 'BookingDTO.cs'
    'NGUOIDUNGDTO.cs' = 'AppUserDTO.cs'
    'NHOMNGUOIDUNGDTO.cs' = 'UserGroupDTO.cs'
    'CHUCNANGDTO.cs' = 'PermissionDTO.cs'
    'THAMSODTO.cs' = 'ParameterDTO.cs'
    'BAOCAODDTO.cs' = 'RevenueReportDTO.cs'
    'CTBAOCAODDTO.cs' = 'RevenueReportDetailDTO.cs'
    'PHANQUYENDTO.cs' = 'GroupPermissionDTO.cs'
    
    # ===== Repository Files =====
    'SanhRepository.cs' = 'HallRepository.cs'
    'ISanhRepository.cs' = 'IHallRepository.cs'
    'LoaiSanhRepository.cs' = 'HallTypeRepository.cs'
    'ILoaiSanhRepository.cs' = 'IHallTypeRepository.cs'
    'CaRepository.cs' = 'ShiftRepository.cs'
    'ICaRepository.cs' = 'IShiftRepository.cs'
    'MonAnRepository.cs' = 'DishRepository.cs'
    'IMonAnRepository.cs' = 'IDishRepository.cs'
    'DichVuRepository.cs' = 'ServiceRepository.cs'
    'IDichVuRepository.cs' = 'IServiceRepository.cs'
    'ThucDonRepository.cs' = 'MenuRepository.cs'
    'IThucDonRepository.cs' = 'IMenuRepository.cs'
    'ChiTietDVRepository.cs' = 'ServiceDetailRepository.cs'
    'IChiTietDVRepository.cs' = 'IServiceDetailRepository.cs'
    'PhieuDatTiecRepository.cs' = 'BookingRepository.cs'
    'IPhieuDatTiecRepository.cs' = 'IBookingRepository.cs'
    'NguoiDungRepository.cs' = 'AppUserRepository.cs'
    'INguoiDungRepository.cs' = 'IAppUserRepository.cs'
    'NhomNguoiDungRepository.cs' = 'UserGroupRepository.cs'
    'INhomNguoiDungRepository.cs' = 'IUserGroupRepository.cs'
    'ChucNangRepository.cs' = 'PermissionRepository.cs'
    'IChucNangRepository.cs' = 'IPermissionRepository.cs'
    'ThamSoRepository.cs' = 'ParameterRepository.cs'
    'IThamSoRepository.cs' = 'IParameterRepository.cs'
    'BaoCaoDsRepository.cs' = 'RevenueReportRepository.cs'
    'IBaoCaoDsRepository.cs' = 'IRevenueReportRepository.cs'
    'CtBaoCaoDsRepository.cs' = 'RevenueReportDetailRepository.cs'
    'ICtBaoCaoDsRepository.cs' = 'IRevenueReportDetailRepository.cs'
    
    # ===== Service Files =====
    'SanhService.cs' = 'HallService.cs'
    'ISanhService.cs' = 'IHallService.cs'
    'LoaiSanhService.cs' = 'HallTypeService.cs'
    'ILoaiSanhService.cs' = 'IHallTypeService.cs'
    'CaService.cs' = 'ShiftService.cs'
    'ICaService.cs' = 'IShiftService.cs'
    'MonAnService.cs' = 'DishService.cs'
    'IMonAnService.cs' = 'IDishService.cs'
    'DichVuService.cs' = 'ServiceService.cs'
    'IDichVuService.cs' = 'IServiceService.cs'
    'ThucDonService.cs' = 'MenuService.cs'
    'IThucDonService.cs' = 'IMenuService.cs'
    'ChiTietDVService.cs' = 'ServiceDetailService.cs'
    'IChiTietDVService.cs' = 'IServiceDetailService.cs'
    'PhieuDatTiecService.cs' = 'BookingService.cs'
    'IPhieuDatTiecService.cs' = 'IBookingService.cs'
    'NguoiDungService.cs' = 'AppUserService.cs'
    'INguoiDungService.cs' = 'IAppUserService.cs'
    'NhomNguoiDungService.cs' = 'UserGroupService.cs'
    'INhomNguoiDungService.cs' = 'IUserGroupService.cs'
    'ChucNangService.cs' = 'PermissionService.cs'
    'IChucNangService.cs' = 'IPermissionService.cs'
    'ThamSoService.cs' = 'ParameterService.cs'
    'IThamSoService.cs' = 'IParameterService.cs'
    'BaoCaoDsService.cs' = 'RevenueReportService.cs'
    'IBaoCaoDsService.cs' = 'IRevenueReportService.cs'
    'CtBaoCaoDsService.cs' = 'RevenueReportDetailService.cs'
    'ICtBaoCaoDsService.cs' = 'IRevenueReportDetailService.cs'
}

# Exclude directories
$excludeDirs = @("bin", "obj", "packages", ".vs", ".git")

# Counter
$filesRenamed = 0
$errors = 0

Write-Host "`nSearching for files to rename..." -ForegroundColor Yellow

# Find and rename files
foreach ($oldFileName in $fileRenameMappings.Keys) {
    $newFileName = $fileRenameMappings[$oldFileName]
    
    # Search for the file in the project
    $files = Get-ChildItem -Path $projectRoot -Filter $oldFileName -Recurse -File | 
        Where-Object { 
            $exclude = $false
            foreach ($dir in $excludeDirs) {
                if ($_.FullName -like "*\$dir\*") {
                    $exclude = $true
                    break
                }
            }
            !$exclude
        }
    
    foreach ($file in $files) {
        try {
            $directory = $file.Directory.FullName
            $newFilePath = Join-Path $directory $newFileName
            
            # Check if target file already exists
            if (Test-Path $newFilePath) {
                Write-Host "  ??  SKIP: $newFileName already exists in $($file.Directory.FullName.Replace($projectRoot, ''))" -ForegroundColor Yellow
                continue
            }
            
            # Rename the file
            Rename-Item -Path $file.FullName -NewName $newFileName -Force
            $filesRenamed++
            
            $relativePath = $file.Directory.FullName.Replace($projectRoot, "")
            Write-Host "  ? Renamed: $relativePath\$oldFileName -> $newFileName" -ForegroundColor Green
        }
        catch {
            $errors++
            Write-Host "  ? ERROR renaming $($file.FullName): $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

# Summary
Write-Host "`n=====================================" -ForegroundColor Cyan
Write-Host "File Renaming Complete!" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Files renamed: $filesRenamed" -ForegroundColor Green
Write-Host "Errors: $errors" -ForegroundColor $(if ($errors -gt 0) { "Red" } else { "Green" })

if ($filesRenamed -gt 0) {
    Write-Host "`n??  IMPORTANT: You need to update project file references!" -ForegroundColor Yellow
    Write-Host "1. Close Visual Studio if it's open" -ForegroundColor White
    Write-Host "2. Edit the .csproj file to update <Compile Include='...'> paths" -ForegroundColor White
    Write-Host "3. Or use Visual Studio to exclude old files and include new files" -ForegroundColor White
    Write-Host "4. Rebuild the solution" -ForegroundColor White
}

Write-Host "`nPress any key to continue..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
