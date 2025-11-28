# H??ng d?n s? d?ng Script Refactor

## M?c ?ích
Script này t? ??ng thay th? t?t c? tên bi?n, thu?c tính, class t? ti?ng Vi?t không d?u sang ti?ng Anh trong toàn b? project.

## Cách s? d?ng

### B??c 1: Commit code hi?n t?i (Khuy?n ngh?)
Tr??c khi ch?y script, hãy commit t?t c? thay ??i vào Git:
```bash
git add .
git commit -m "Before refactoring Vietnamese to English"
```

### B??c 2: Ch?y script
1. M? PowerShell v?i quy?n Administrator
2. Di chuy?n ??n th? m?c project:
   ```powershell
   cd "D:\1. UIT\HK5\SE113.Q12\SE113_QuanLyTiecCuoi_Test"
   ```

3. Cho phép ch?y script (n?u ch?a):
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

4. Ch?y script:
   ```powershell
   .\RefactorToEnglish.ps1
   ```

5. Khi ???c h?i v? backup, nh?n **Y** ?? t?o b?n sao l?u (khuy?n ngh?)

### B??c 3: Ki?m tra k?t qu?
Script s? hi?n th?:
- S? file ???c x? lý
- S? thay ??i trong m?i file
- T?ng s? thay ??i
- V? trí th? m?c backup (n?u có)

### B??c 4: Build và test
1. M? Visual Studio
2. Build solution (Ctrl + Shift + B)
3. S?a các l?i compile n?u có
4. Test ?ng d?ng

### B??c 5: Review thay ??i
S? d?ng Git ?? xem nh?ng thay ??i:
```bash
git diff
```

Ho?c s? d?ng Git GUI trong Visual Studio.

## Các thay ??i ???c th?c hi?n

### DTO Classes
- `SANHDTO` ? `HallDTO`
- `LOAISANHDTO` ? `HallTypeDTO`
- `CADTO` ? `ShiftDTO`
- `MONANDTO` ? `DishDTO`
- `DICHVUDTO` ? `ServiceDTO`
- `THUCDONDTO` ? `MenuDTO`
- `CHITIETDVDTO` ? `ServiceDetailDTO`
- `PHIEUDATTIECDTO` ? `BookingDTO`

### Hall (S?nh)
- `MaSanh` ? `HallId`
- `TenSanh` ? `HallName`
- `SoLuongBanToiDa` ? `MaxTableCount`
- `MaLoaiSanh` ? `HallTypeId`
- `Sanh` ? `Hall`

### HallType (Lo?iS?nh)
- `MaLoaiSanh` ? `HallTypeId`
- `TenLoaiSanh` ? `HallTypeName`
- `DonGiaBanToiThieu` ? `MinTablePrice`
- `LoaiSanh` ? `HallType`

### Shift (Ca)
- `MaCa` ? `ShiftId`
- `TenCa` ? `ShiftName`
- `ThoiGianBatDauCa` ? `StartTime`
- `ThoiGianKetThucCa` ? `EndTime`
- `Ca` ? `Shift`

### Dish (MonAn)
- `MaMonAn` ? `DishId`
- `TenMonAn` ? `DishName`
- `MonAn` ? `Dish`

### Service (DichVu)
- `MaDichVu` ? `ServiceId`
- `TenDichVu` ? `ServiceName`
- `DichVu` ? `Service`

### Booking (PhieuDatTiec)
- `MaPhieuDat` ? `BookingId`
- `TenChuRe` ? `GroomName`
- `TenCoDau` ? `BrideName`
- `DienThoai` ? `Phone`
- `NgayDatTiec` ? `BookingDate`
- `NgayDaiTiec` ? `WeddingDate`
- `TienDatCoc` ? `Deposit`
- `SoLuongBan` ? `TableCount`
- `SoBanDuTru` ? `ReserveTableCount`
- `NgayThanhToan` ? `PaymentDate`
- `DonGiaBanTiec` ? `TablePrice`
- `TongTienBan` ? `TotalTableAmount`
- `TongTienDV` ? `TotalServiceAmount`
- `TongTienHoaDon` ? `TotalInvoiceAmount`
- `TienConLai` ? `RemainingAmount`
- `ChiPhiPhatSinh` ? `AdditionalCost`
- `TienPhat` ? `PenaltyAmount`

### Menu & ServiceDetail
- `SoLuong` ? `Quantity`
- `DonGia` ? `UnitPrice`
- `GhiChu` ? `Note`
- `ThanhTien` ? `TotalAmount`

## Nh?ng file KHÔNG ???c thay ??i
Script t? ??ng b? qua:
- Th? m?c `bin/`, `obj/`, `packages/`, `.vs/`, `.git/`
- File auto-generated: `*.Designer.cs`, `*.Context.cs`, `Model1.cs`
- File EDMX và các file ???c generate t? Entity Framework

## Khôi ph?c n?u có l?i

### Cách 1: S? d?ng Backup
N?u b?n ?ã ch?n t?o backup, copy toàn b? file t? th? m?c `Backup_[timestamp]` v? project.

### Cách 2: S? d?ng Git
```bash
git checkout .
git clean -fd
```

## L?u ý quan tr?ng

1. **XAML Bindings**: Script không th? t? ??ng s?a các binding trong XAML. B?n c?n tìm và s?a th? công:
   - M? m?i file `.xaml`
   - Tìm ki?m các binding c? (ví d?: `{Binding MaSanh}`)
   - Thay th? b?ng tên m?i (`{Binding HallId}`)

2. **Database**: Script ch? s?a code C#. Database c?a b?n v?n gi? nguyên tên ti?ng Vi?t. Entity Framework s? map t? ??ng.

3. **Resource Files**: N?u có file `.resx`, c?n ki?m tra và s?a th? công.

4. **Comments**: Script không s?a comments, b?n có th? c?p nh?t sau n?u c?n.

## H? tr?

N?u g?p v?n ??:
1. Ki?m tra Git diff ?? xem thay ??i
2. Build solution và ??c error messages
3. Khôi ph?c t? backup ho?c Git
4. Ch?y l?i script ho?c s?a th? công

## Checklist sau khi ch?y script

- [ ] Ch?y script thành công
- [ ] Build solution không có l?i
- [ ] Ki?m tra các ViewModel
- [ ] Ki?m tra các View (XAML bindings)
- [ ] Test ch?c n?ng CRUD
- [ ] Test các form thêm/s?a/xóa
- [ ] Test báo cáo
- [ ] Commit thay ??i vào Git

---
**T?o b?i**: Copilot AI Assistant
**Ngày**: 2025
