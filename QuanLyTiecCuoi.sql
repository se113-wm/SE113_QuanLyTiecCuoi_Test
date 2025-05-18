-- =============================
-- CSDL QUẢN LÝ TIỆC CƯỚI
-- =============================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'QuanLyTiecCuoi')
BEGIN
    ALTER DATABASE QuanLyTiecCuoi SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyTiecCuoi;
END
GO

CREATE DATABASE QuanLyTiecCuoi;
GO

USE QuanLyTiecCuoi;
GO

ALTER DATABASE QuanLyTiecCuoi COLLATE SQL_Latin1_General_CP1_CS_AS

-- BẢNG LOAISANH
CREATE TABLE LOAISANH (
    MaLoaiSanh INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiSanh NVARCHAR(40),
    DonGiaBanToiThieu MONEY
);

-- BẢNG SANH
CREATE TABLE SANH (
    MaSanh INT IDENTITY(1,1) PRIMARY KEY,
    MaLoaiSanh INT,
    TenSanh NVARCHAR(40),
    SoLuongBanToiDa INT,
    GhiChu NVARCHAR(100),
    FOREIGN KEY (MaLoaiSanh) REFERENCES LOAISANH(MaLoaiSanh)
);

-- BẢNG CA
CREATE TABLE CA (
    MaCa INT IDENTITY(1,1) PRIMARY KEY,
    TenCa NVARCHAR(40),
    ThoiGianBatDauCa TIME,
    ThoiGianKetThucCa TIME
);

-- BẢNG PHIEUDATTIEC
CREATE TABLE PHIEUDATTIEC (
    MaPhieuDat INT IDENTITY(1,1) PRIMARY KEY,
    TenChuRe NVARCHAR(40),
    TenCoDau NVARCHAR(40),
    DienThoai VARCHAR(10),
    NgayDatTiec SMALLDATETIME,
    NgayDaiTiec SMALLDATETIME,
    MaCa INT,
    MaSanh INT,
    TienDatCoc MONEY,
    SoLuongBan INT,
    SoBanDuTru INT,
    NgayThanhToan SMALLDATETIME,
    DonGiaBanTiec MONEY,
    TongTienBan MONEY,
    TongTienDV MONEY,
    TongTienHoaDon MONEY,
    TienConLai MONEY,
    ChiPhiPhatSinh MONEY,
    TienPhat MONEY,
    FOREIGN KEY (MaCa) REFERENCES CA(MaCa),
    FOREIGN KEY (MaSanh) REFERENCES SANH(MaSanh)
);

-- BẢNG MONAN
CREATE TABLE MONAN (
    MaMonAn INT IDENTITY(1,1) PRIMARY KEY,
    TenMonAn NVARCHAR(40),
    DonGia MONEY,
    GhiChu NVARCHAR(100)
);

-- BẢNG THUCDON
CREATE TABLE THUCDON (
    MaPhieuDat INT,
    MaMonAn INT,
    SoLuong INT,
    DonGia MONEY,
    ThuTuLenMon INT,
    GhiChu NVARCHAR(100),
    PRIMARY KEY (MaPhieuDat, MaMonAn),
    FOREIGN KEY (MaPhieuDat) REFERENCES PHIEUDATTIEC(MaPhieuDat),
    FOREIGN KEY (MaMonAn) REFERENCES MONAN(MaMonAn)
);

-- BẢNG DICHVU
CREATE TABLE DICHVU (
    MaDichVu INT IDENTITY(1,1) PRIMARY KEY,
    TenDichVu NVARCHAR(40),
    DonGia MONEY,
    GhiChu NVARCHAR(100)
);

-- BẢNG CHITIETDV
CREATE TABLE CHITIETDV (
    MaPhieuDat INT,
    MaDichVu INT,
    SoLuong INT,
    DonGia MONEY,
    ThanhTien MONEY,
    GhiChu NVARCHAR(100),
    PRIMARY KEY (MaPhieuDat, MaDichVu),
    FOREIGN KEY (MaPhieuDat) REFERENCES PHIEUDATTIEC(MaPhieuDat),
    FOREIGN KEY (MaDichVu) REFERENCES DICHVU(MaDichVu)
);

-- BẢNG BAO CAO DOANH THU
CREATE TABLE BAOCAODS (
    Thang INT,
    Nam INT,
    TongDoanhThu MONEY,
    PRIMARY KEY (Thang, Nam)
);

CREATE TABLE CTBAOCAODS (
    Ngay INT,
    Thang INT,
    Nam INT,
    SoLuongTiec INT,
    DoanhThu MONEY,
    TiLe DECIMAL(5,2),
    PRIMARY KEY (Ngay, Thang, Nam),
    FOREIGN KEY (Thang, Nam) REFERENCES BAOCAODS(Thang, Nam)
);

-- BẢNG THAM SỐ
CREATE TABLE THAMSO (
    KiemTraPhat BIT,
    TiLePhat DECIMAL(5,2),
    TiLeTienDatCocToiThieu DECIMAL(5,2),
    TiLeSoBanDatTruocToiThieu DECIMAL (5,2)
);

-- BẢNG PHÂN QUYỀN
CREATE TABLE CHUCNANG (
    MaChucNang VARCHAR(10) PRIMARY KEY,
    TenChucNang NVARCHAR(100),
    TenManHinhDuocLoad NVARCHAR(100)
);

CREATE TABLE NHOMNGUOIDUNG (
    MaNhom VARCHAR(10) PRIMARY KEY,
    TenNhom NVARCHAR(100)
);

CREATE TABLE PHANQUYEN (
    MaNhom VARCHAR(10),
    MaChucNang VARCHAR(10),
    PRIMARY KEY (MaNhom, MaChucNang),
    FOREIGN KEY (MaNhom) REFERENCES NHOMNGUOIDUNG(MaNhom),
    FOREIGN KEY (MaChucNang) REFERENCES CHUCNANG(MaChucNang)
);

CREATE TABLE NGUOIDUNG (
    TenDangNhap VARCHAR(50) PRIMARY KEY,
    MatKhauHash VARCHAR(256) NOT NULL,
    HoTen NVARCHAR(100),
    Email VARCHAR(100),
    TrangThai BIT DEFAULT 1,
    MaNhom VARCHAR(10),
    FOREIGN KEY (MaNhom) REFERENCES NHOMNGUOIDUNG(MaNhom)
);

INSERT INTO NHOMNGUOIDUNG (MaNhom, TenNhom)
VALUES ('ADMIN', N'Quản trị viên');

INSERT INTO NHOMNGUOIDUNG (MaNhom, TenNhom)
VALUES ('STAFF', N'Nhân viên');

INSERT INTO NGUOIDUNG (TenDangNhap, MatKhauHash, HoTen, Email, TrangThai, MaNhom)
VALUES ('Fartiel', 'db69fc039dcbd2962cb4d28f5891aae1', N'Đặng Phú Thiện', '23521476@gm.uit.edu.vn', 1, 'ADMIN');

INSERT INTO NGUOIDUNG (TenDangNhap, MatKhauHash, HoTen, Email, TrangThai, MaNhom)
VALUES ('Neith', '978aae9bb6bee8fb75de3e4830a1be46', N'Đặng Phú Thiện', '23521476@gm.uit.edu.vn', 1, 'STAFF');

INSERT INTO CHUCNANG (MaChucNang, TenChucNang, TenManHinhDuocLoad) VALUES 
('Home', N'Trang chủ', N'HomeView'),
('HallType', N'sảnh', N'HallTypeView'),
('Hall', N'tiệc', N'HallView'),
('Shift', N'Ca làm việc', N'ShiftView'),
('Food', N'Món ăn', N'FoodView'),
('Service', N'Dịch vụ', N'ServiceView'),
('Wedding', N'Tiệc cưới', N'WeddingView'),
('Report', N'Báo cáo', N'ReportView'),
('Parameter', N'Tham số', N'ParameterView'),
('Permission', N'Phân quyền', N'PermissionView'),
('User', N'Người dùng', N'UserView');

INSERT INTO PHANQUYEN (MaNhom, MaChucNang) VALUES
('ADMIN', 'Home')
INSERT INTO PHANQUYEN (MaNhom, MaChucNang) VALUES
('ADMIN', 'Report')
INSERT INTO PHANQUYEN (MaNhom, MaChucNang) VALUES
('ADMIN', 'HallType')
INSERT INTO PHANQUYEN (MaNhom, MaChucNang) VALUES
('ADMIN', 'Hall')
INSERT INTO PHANQUYEN (MaNhom, MaChucNang) VALUES
('ADMIN', 'Wedding')
INSERT INTO PHANQUYEN (MaNhom, MaChucNang) VALUES
('ADMIN', 'Permission')

INSERT INTO PHANQUYEN (MaNhom, MaChucNang) VALUES
('STAFF', 'Home')
INSERT INTO PHANQUYEN (MaNhom, MaChucNang) VALUES
('STAFF', 'User')

INSERT INTO LOAISANH (TenLoaiSanh, DonGiaBanToiThieu)
VALUES 
    (N'A', 1000000),
    (N'B', 1100000),
    (N'C', 1200000),
    (N'D', 1400000),
    (N'E', 1600000);

INSERT INTO SANH (MaLoaiSanh, TenSanh, SoLuongBanToiDa, GhiChu)
VALUES
    (1, N'Ruby', 30, N'Gần cửa chính'),
    (1, N'Sapphire', 28, N'Có sân khấu lớn'),
    (2, N'Diamond', 25, N'Không gian mở'),
    (2, N'Gold', 22, N'Phù hợp tiệc nhỏ'),
    (3, N'Silver', 24, N'Thiết kế hiện đại'),
    (3, N'Platinum', 26, N'View đẹp'),
    (4, N'Emerald', 20, N'Ánh sáng tự nhiên'),
    (4, N'Opal', 18, N'Màu sắc ấm cúng'),
    (5, N'Pearl', 15, N'Cho tiệc thân mật'),
    (5, N'Crystal', 16, N'Thiết kế sang trọng');
