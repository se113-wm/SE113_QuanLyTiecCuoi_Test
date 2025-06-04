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
    TenLoaiSanh NVARCHAR(40) UNIQUE NOT NULL,
    DonGiaBanToiThieu MONEY
);

-- BẢNG SANH
CREATE TABLE SANH (
    MaSanh INT IDENTITY(1,1) PRIMARY KEY,
    MaLoaiSanh INT,
    TenSanh NVARCHAR(40) UNIQUE NOT NULL,
    SoLuongBanToiDa INT,
    GhiChu NVARCHAR(100),
    FOREIGN KEY (MaLoaiSanh) REFERENCES LOAISANH(MaLoaiSanh)
);

-- BẢNG CA
CREATE TABLE CA (
    MaCa INT IDENTITY(1,1) PRIMARY KEY,
    TenCa NVARCHAR(40) UNIQUE NOT NULL,
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
    TenMonAn NVARCHAR(40) UNIQUE NOT NULL,
    DonGia MONEY,
    GhiChu NVARCHAR(100)
);

-- BẢNG THUCDON
CREATE TABLE THUCDON (
    MaPhieuDat INT,
    MaMonAn INT,
    SoLuong INT,
    DonGia MONEY,
    ThuTuLenMon INT UNIQUE NOT NULL,
    GhiChu NVARCHAR(100),
    PRIMARY KEY (MaPhieuDat, MaMonAn),
    FOREIGN KEY (MaPhieuDat) REFERENCES PHIEUDATTIEC(MaPhieuDat),
    FOREIGN KEY (MaMonAn) REFERENCES MONAN(MaMonAn)
);

-- BẢNG DICHVU
CREATE TABLE DICHVU (
    MaDichVu INT IDENTITY(1,1) PRIMARY KEY,
    TenDichVu NVARCHAR(40) UNIQUE NOT NULL,
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
    TenThamSo NVARCHAR(5) PRIMARY KEY,
	GiaTri DECIMAL(5,2)
);

-- BẢNG PHÂN QUYỀN
CREATE TABLE CHUCNANG (
    MaChucNang VARCHAR(10) PRIMARY KEY,
    TenChucNang NVARCHAR(100),
    TenManHinhDuocLoad NVARCHAR(100)
);

CREATE TABLE NHOMNGUOIDUNG (
    MaNhom VARCHAR(10) PRIMARY KEY,
    TenNhom NVARCHAR(100) UNIQUE NOT NULL
);

CREATE TABLE PHANQUYEN (
    MaNhom VARCHAR(10),
    MaChucNang VARCHAR(10),
    PRIMARY KEY (MaNhom, MaChucNang),
    FOREIGN KEY (MaNhom) REFERENCES NHOMNGUOIDUNG(MaNhom),
    FOREIGN KEY (MaChucNang) REFERENCES CHUCNANG(MaChucNang)
);

CREATE TABLE NGUOIDUNG (
    MaNguoiDung INT PRIMARY KEY IDENTITY(1,1),
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhauHash VARCHAR(256) NOT NULL,
    HoTen NVARCHAR(100),
    Email VARCHAR(100),
    MaNhom VARCHAR(10),
    FOREIGN KEY (MaNhom) REFERENCES NHOMNGUOIDUNG(MaNhom)
);

INSERT INTO NHOMNGUOIDUNG (MaNhom, TenNhom)
VALUES ('ADMIN', N'Quản trị viên');

INSERT INTO NHOMNGUOIDUNG (MaNhom, TenNhom)
VALUES ('STAFF', N'Nhân viên');
INSERT INTO NHOMNGUOIDUNG (MaNhom, TenNhom)
VALUES ('gr1', N'Nhân viên1');
INSERT INTO NHOMNGUOIDUNG (MaNhom, TenNhom)
VALUES ('gr2', N'Nhân viên2');
INSERT INTO NHOMNGUOIDUNG (MaNhom, TenNhom)
VALUES ('gr3', N'Nhân viên3');

INSERT INTO NGUOIDUNG (TenDangNhap, MatKhauHash, HoTen, Email, MaNhom)
VALUES ('Fartiel', 'db69fc039dcbd2962cb4d28f5891aae1', N'Đặng Phú Thiện', '23521476@gm.uit.edu.vn', 'ADMIN');

INSERT INTO NGUOIDUNG (TenDangNhap, MatKhauHash, HoTen, Email, MaNhom)
VALUES ('Neith', '978aae9bb6bee8fb75de3e4830a1be46', N'Đặng Phú Thiện', '23521476@gm.uit.edu.vn', 'STAFF');

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

-- Gán toàn bộ chức năng cho nhóm ADMIN
INSERT INTO PHANQUYEN (MaNhom, MaChucNang) VALUES
('ADMIN', 'Home'),
('ADMIN', 'HallType'),
('ADMIN', 'Hall'),
('ADMIN', 'Shift'),
('ADMIN', 'Food'),
('ADMIN', 'Service'),
('ADMIN', 'Wedding'),
('ADMIN', 'Report'),
('ADMIN', 'Parameter'),
('ADMIN', 'Permission'),
('ADMIN', 'User');


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

INSERT INTO LOAISANH (TenLoaiSanh, DonGiaBanToiThieu) VALUES
(N'F', 1800000),
(N'G', 2000000),
(N'H', 2200000),
(N'I', 2400000),
(N'J', 2600000),
(N'K', 2800000),
(N'L', 3000000),
(N'M', 3200000),
(N'N', 3400000),
(N'O', 3600000),
(N'P', 3800000),
(N'Q', 4000000),
(N'R', 4200000),
(N'S', 4400000),
(N'T', 4600000),
(N'U', 4800000),
(N'V', 5000000),
(N'W', 5200000),
(N'X', 5400000),
(N'Y', 5600000),
(N'Z', 5800000),
(N'AA', 6000000),
(N'AB', 6200000),
(N'AC', 6400000),
(N'AD', 6600000),
(N'AE', 6800000),
(N'AF', 7000000),
(N'AG', 7200000),
(N'AH', 7400000),
(N'AI', 7600000);

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



INSERT INTO DICHVU (TenDichVu, DonGia, GhiChu) VALUES
(N'Âm thanh', 2000000, N'Dịch vụ âm thanh tiêu chuẩn'),
(N'Ánh sáng', 2500000, N'Hệ thống ánh sáng chuyên nghiệp'),
(N'Ca sĩ biểu diễn', 5000000, N'Ca sĩ nổi tiếng biểu diễn 1 tiết mục'),
(N'Ban nhạc sống', 7000000, N'Ban nhạc chơi trực tiếp'),
(N'Trang trí hoa tươi', 3000000, N'Trang trí sân khấu và bàn tiệc'),
(N'Trang trí bóng bay', 1000000, N'Trang trí theo yêu cầu'),
(N'Nhóm múa khai tiệc', 4000000, N'Múa mở màn lễ cưới'),
(N'MC chuyên nghiệp', 3500000, N'Dẫn chương trình toàn buổi tiệc'),
(N'Xe hoa', 1500000, N'Xe hoa đưa đón cô dâu chú rể'),
(N'Pháo điện', 800000, N'Tạo hiệu ứng sân khấu'),
(N'Thiết kế phông nền', 1200000, N'Phông nền chụp hình chuyên nghiệp'),
(N'Chụp hình cưới', 4500000, N'Nhiếp ảnh gia chuyên nghiệp'),
(N'Quay phim lễ cưới', 5000000, N'Quay toàn bộ buổi lễ và dựng video'),
(N'Làm album cưới', 3000000, N'Thiết kế album cưới in màu'),
(N'Trang điểm cô dâu', 2000000, N'Trang điểm tại nhà hoặc tại tiệc'),
(N'Cho thuê váy cưới', 2500000, N'Váy cưới cao cấp, nhiều mẫu mã'),
(N'Dịch vụ giữ trẻ', 1000000, N'Nhân viên giữ trẻ tại tiệc'),
(N'Bắn pháo hoa lạnh', 6000000, N'Tạo điểm nhấn hoành tráng'),
(N'Dịch vụ rước dâu bằng voi', 10000000, N'Dịch vụ đặc biệt, cần đặt trước'),
(N'Trang trí bàn gallery', 1500000, N'Bàn gallery đón khách sang trọng');