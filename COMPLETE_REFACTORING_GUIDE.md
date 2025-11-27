# COMPLETE REFACTORING GUIDE
## Vietnamese to English - Full Stack Refactoring

### ?? Scripts Created

| Script | Purpose | Order |
|--------|---------|-------|
| `RunMasterRefactor.ps1` | **MASTER SCRIPT** - Runs all steps automatically | RUN THIS! |
| `RefactorAllEntities.ps1` | Refactor Repository, Service, DTO layers | Step 1 |
| `RefactorViewModels.ps1` | Refactor ViewModel layer | Step 2 |
| `RenameFiles.ps1` | Rename physical files | Step 3 |
| `UpdateProjectFile.ps1` | Update .csproj references | Step 4 |

### ?? QUICK START (Recommended)

```powershell
# 1. Commit your code first!
git add .
git commit -m "Before complete refactoring"

# 2. Run the MASTER script
.\RunMasterRefactor.ps1

# That's it! The master script handles everything.
```

### ?? What Gets Refactored

#### Layer 1: Repository & Interface
- ? `IBookingRepository`, `BookingRepository`
- ? `IHallRepository`, `HallRepository`
- ? `IHallTypeRepository`, `HallTypeRepository`
- ? `IShiftRepository`, `ShiftRepository`
- ? `IDishRepository`, `DishRepository`
- ? `IServiceRepository`, `ServiceRepository`
- ? And all other repositories...

#### Layer 2: Service & Interface
- ? `IBookingService`, `BookingService`
- ? `IHallService`, `HallService`
- ? `IHallTypeService`, `HallTypeService`
- ? `IShiftService`, `ShiftService`
- ? And all other services...

#### Layer 3: DTOs
- ? `BookingDTO` (was PHIEUDATTIECDTO)
- ? `HallDTO` (was SANHDTO)
- ? `HallTypeDTO` (was LOAISANHDTO)
- ? `ShiftDTO` (was CADTO)
- ? `DishDTO` (was MONANDTO)
- ? `ServiceDTO` (was DICHVUDTO)
- ? And all other DTOs...

#### Layer 4: ViewModels
- ? `AddWeddingViewModel`
- ? `WeddingDetailViewModel`
- ? `InvoiceViewModel`
- ? `HallViewModel`
- ? `HallTypeViewModel`
- ? And all other ViewModels...

### ?? Property Mappings Reference

#### Booking (PhieuDatTiec)
```
MaPhieuDat ? BookingId
TenChuRe ? GroomName
TenCoDau ? BrideName
DienThoai ? Phone
NgayDatTiec ? BookingDate
NgayDaiTiec ? WeddingDate
TienDatCoc ? Deposit
SoLuongBan ? TableCount
SoBanDuTru ? ReserveTableCount
NgayThanhToan ? PaymentDate
TongTienHoaDon ? TotalInvoiceAmount
TienConLai ? RemainingAmount
```

#### Hall (Sanh)
```
MaSanh ? HallId
TenSanh ? HallName
SoLuongBanToiDa ? MaxTableCount
MaLoaiSanh ? HallTypeId
```

#### HallType (LoaiSanh)
```
MaLoaiSanh ? HallTypeId
TenLoaiSanh ? HallTypeName
DonGiaBanToiThieu ? MinTablePrice
```

#### Shift (Ca)
```
MaCa ? ShiftId
TenCa ? ShiftName
ThoiGianBatDauCa ? StartTime
ThoiGianKetThucCa ? EndTime
```

#### Dish (MonAn)
```
MaMonAn ? DishId
TenMonAn ? DishName
```

#### Service (DichVu)
```
MaDichVu ? ServiceId
TenDichVu ? ServiceName
```

#### Common Properties
```
DonGia ? UnitPrice
SoLuong ? Quantity
GhiChu ? Note
ThanhTien ? TotalAmount
```

### ?? Manual Steps (Run Individual Scripts)

If you prefer to run each step manually:

```powershell
# Step 1: Refactor entities (Repository, Service, DTO)
.\RefactorAllEntities.ps1

# Step 2: Refactor ViewModels
.\RefactorViewModels.ps1

# Step 3: Rename files
.\RenameFiles.ps1

# Step 4: Update .csproj
.\UpdateProjectFile.ps1
```

### ?? After Refactoring

#### 1. Close & Reopen Visual Studio
```
- Close all VS instances
- Reopen solution
- Reload project if prompted
```

#### 2. Clean & Rebuild
```
Build > Clean Solution
Build > Rebuild Solution
```

#### 3. Fix XAML Bindings
The scripts create `XAML_BINDING_COMPLETE_GUIDE.md` with all mappings.

Use Find & Replace in XAML files:
```xaml
<!-- Old -->
<TextBlock Text="{Binding TenChuRe}"/>

<!-- New -->
<TextBlock Text="{Binding GroomName}"/>
```

#### 4. Test Everything
- [ ] Test all CRUD operations
- [ ] Test all forms (Add, Edit, Delete)
- [ ] Test reports
- [ ] Test charts
- [ ] Test user management
- [ ] Test permissions

#### 5. Commit Changes
```bash
git add .
git commit -m "Refactor: Complete Vietnamese to English naming"
git push
```

### ?? Troubleshooting

#### Build Errors
1. Check Error List in Visual Studio
2. Common issues:
   - Missing using statements
   - XAML bindings not updated
   - File references in .csproj

#### XAML Errors
1. Open each .xaml file with errors
2. Use Find & Replace (Ctrl+H)
3. Replace old bindings with new ones

#### Rollback
If something goes wrong:

```powershell
# Restore from backup
$backupDir = "MasterBackup_YYYYMMDD_HHMMSS"  # Use your actual backup folder

# OR restore from Git
git checkout .
git clean -fd
```

### ?? File Naming Changes

All files will be renamed automatically:

| Old Name | New Name |
|----------|----------|
| `SANHDTO.cs` | `HallDTO.cs` |
| `LOAISANHDTO.cs` | `HallTypeDTO.cs` |
| `CADTO.cs` | `ShiftDTO.cs` |
| `MONANDTO.cs` | `DishDTO.cs` |
| `DICHVUDTO.cs` | `ServiceDTO.cs` |
| `PHIEUDATTIECDTO.cs` | `BookingDTO.cs` |
| `SanhRepository.cs` | `HallRepository.cs` |
| `SanhService.cs` | `HallService.cs` |
| ... and 60+ more files |

### ? Verification Checklist

After running all scripts:

- [ ] All files renamed successfully
- [ ] .csproj file updated
- [ ] Solution builds without errors
- [ ] All ViewModels work correctly
- [ ] All Views display data
- [ ] XAML bindings updated
- [ ] CRUD operations work
- [ ] Reports generate correctly
- [ ] No runtime errors
- [ ] Changes committed to Git

### ?? Success Criteria

You'll know refactoring is complete when:
1. ? Solution builds with 0 errors
2. ? Application runs without crashes
3. ? All features work as before
4. ? No Vietnamese property names in code
5. ? All XAML bindings use English names

### ?? Support

If you encounter issues:
1. Check Error List in Visual Studio
2. Review `XAML_BINDING_COMPLETE_GUIDE.md`
3. Check backup folder for rollback
4. Use Git to view changes: `git diff`

---

**Generated by**: Copilot Refactoring Assistant
**Date**: $(Get-Date -Format "yyyy-MM-dd")
**Version**: 1.0

Good luck with your refactoring! ??
