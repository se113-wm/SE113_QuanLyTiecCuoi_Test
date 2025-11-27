# XAML Binding Reference Guide

This file contains all the XAML binding replacements you need to make manually.

## How to use:

1. Open each .xaml file in Visual Studio
2. Use Find \& Replace (Ctrl+H) for each binding below
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

* Use Find \& Replace in entire solution (Ctrl+Shift+H)
* Check "Match whole word" option
* Preview changes before replacing all
* Test each view after updating bindings
