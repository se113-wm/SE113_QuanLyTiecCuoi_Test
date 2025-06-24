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
    TiLe DECIMAL(7,2),
    PRIMARY KEY (Ngay, Thang, Nam),
    FOREIGN KEY (Thang, Nam) REFERENCES BAOCAODS(Thang, Nam)
);

-- BẢNG THAM SỐ
CREATE TABLE THAMSO (
    TenThamSo NVARCHAR(100) PRIMARY KEY,
	GiaTri DECIMAL(5,2)
);

INSERT INTO THAMSO (TenThamSo, GiaTri) VALUES
('KiemTraPhat', 1),         -- BIT: 0 = tắt, 1 = bật (giá trị mặc định có thể là 0)
('TiLePhat', 0.01),         -- DECIMAL(5,2)
('TiLeTienDatCocToiThieu', 0.15), -- DECIMAL(5,2)
('TiLeSoBanDatTruocToiThieu', 0.85); -- DECIMAL(5,2)


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

GO
CREATE TRIGGER TRG_Update_TongTien_From_ThucDon
ON THUCDON
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Lấy danh sách các MaPhieuDat bị ảnh hưởng
    DECLARE @AffectedPhieuDats TABLE (MaPhieuDat INT);
    
    INSERT INTO @AffectedPhieuDats
    SELECT MaPhieuDat FROM inserted
    UNION
    SELECT MaPhieuDat FROM deleted;
    
    -- Cập nhật lại các phiếu đặt tiệc liên quan
    UPDATE p
    SET 
        TongTienBan = p.SoLuongBan * (p.DonGiaBanTiec + ISNULL((SELECT SUM(td.DonGia * td.SoLuong) 
                                                                   FROM THUCDON td 
                                                                   WHERE td.MaPhieuDat = p.MaPhieuDat), 0)),
        TongTienDV = ISNULL((SELECT SUM(ctdv.ThanhTien) 
                           FROM CHITIETDV ctdv 
                           WHERE ctdv.MaPhieuDat = p.MaPhieuDat), 0),
        TongTienHoaDon = p.SoLuongBan * (p.DonGiaBanTiec + ISNULL((SELECT SUM(td.DonGia * td.SoLuong) 
                                                                      FROM THUCDON td 
                                                                      WHERE td.MaPhieuDat = p.MaPhieuDat), 0)) +
                       ISNULL((SELECT SUM(ctdv.ThanhTien) 
                              FROM CHITIETDV ctdv 
                              WHERE ctdv.MaPhieuDat = p.MaPhieuDat), 0) +
                       ISNULL(p.ChiPhiPhatSinh, 0),
        TienConLai = p.SoLuongBan * (p.DonGiaBanTiec + ISNULL((SELECT SUM(td.DonGia * td.SoLuong) 
                                                                  FROM THUCDON td 
                                                                  WHERE td.MaPhieuDat = p.MaPhieuDat), 0)) +
                    ISNULL((SELECT SUM(ctdv.ThanhTien) 
                           FROM CHITIETDV ctdv 
                           WHERE ctdv.MaPhieuDat = p.MaPhieuDat), 0) +
                    ISNULL(p.ChiPhiPhatSinh, 0) +
                    ISNULL(p.TienPhat, 0) -
                    ISNULL(p.TienDatCoc, 0)
    FROM PHIEUDATTIEC p
    JOIN SANH s ON p.MaSanh = s.MaSanh
    JOIN LOAISANH ls ON s.MaLoaiSanh = ls.MaLoaiSanh
    WHERE p.MaPhieuDat IN (SELECT MaPhieuDat FROM @AffectedPhieuDats);
END;

GO
CREATE TRIGGER TRG_Update_TongTien_From_ChiTietDV
ON CHITIETDV
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Lấy danh sách các MaPhieuDat bị ảnh hưởng
    DECLARE @AffectedPhieuDats TABLE (MaPhieuDat INT);
    
    INSERT INTO @AffectedPhieuDats
    SELECT MaPhieuDat FROM inserted
    UNION
    SELECT MaPhieuDat FROM deleted;
    
    -- Cập nhật lại các phiếu đặt tiệc liên quan
    UPDATE p
    SET 
        TongTienDV = ISNULL((SELECT SUM(ctdv.ThanhTien) 
                           FROM CHITIETDV ctdv 
                           WHERE ctdv.MaPhieuDat = p.MaPhieuDat), 0),
        TongTienHoaDon = p.TongTienBan + 
                        ISNULL((SELECT SUM(ctdv.ThanhTien) 
                               FROM CHITIETDV ctdv 
                               WHERE ctdv.MaPhieuDat = p.MaPhieuDat), 0) +
                        ISNULL(p.ChiPhiPhatSinh, 0),
        TienConLai = p.TongTienBan + 
                    ISNULL((SELECT SUM(ctdv.ThanhTien) 
                           FROM CHITIETDV ctdv 
                           WHERE ctdv.MaPhieuDat = p.MaPhieuDat), 0) +
                    ISNULL(p.ChiPhiPhatSinh, 0) +
                    ISNULL(p.TienPhat, 0) -
                    ISNULL(p.TienDatCoc, 0)
    FROM PHIEUDATTIEC p
    WHERE p.MaPhieuDat IN (SELECT MaPhieuDat FROM @AffectedPhieuDats);
END;

--GO
--CREATE TRIGGER TRG_Update_TongTien_When_LoaiSanh_Changes
--ON LOAISANH
--AFTER UPDATE
--AS
--BEGIN
--    SET NOCOUNT ON;
    
--    -- Kiểm tra xem DonGiaBanToiThieu có thay đổi không
--    IF UPDATE(DonGiaBanToiThieu)
--    BEGIN
--        -- Cập nhật lại TongTienBan và các trường liên quan trong PHIEUDATTIEC
--        -- cho các phiếu đặt tiệc còn ở tương lai
--        UPDATE p
--        SET 
--            p.TongTienBan = p.SoLuongBan * i.DonGiaBanToiThieu,
--            p.TongTienHoaDon = (p.SoLuongBan * i.DonGiaBanToiThieu) + 
--                              ISNULL(p.TongTienDV, 0) + 
--                              ISNULL(p.ChiPhiPhatSinh, 0),
--            p.TienConLai = (p.SoLuongBan * i.DonGiaBanToiThieu) + 
--                           ISNULL(p.TongTienDV, 0) + 
--                           ISNULL(p.ChiPhiPhatSinh, 0) + 
--                           ISNULL(p.TienPhat, 0) - 
--                           ISNULL(p.TienDatCoc, 0)
--        FROM PHIEUDATTIEC p
--        INNER JOIN SANH s ON p.MaSanh = s.MaSanh
--        INNER JOIN inserted i ON s.MaLoaiSanh = i.MaLoaiSanh
--        INNER JOIN deleted d ON i.MaLoaiSanh = d.MaLoaiSanh
--        WHERE 
--            i.DonGiaBanToiThieu <> d.DonGiaBanToiThieu
--            AND p.NgayDaiTiec >= CAST(GETDATE() AS DATE);  -- Chỉ cập nhật nếu ngày đãi tiệc chưa tới
--    END
--END;


GO
CREATE TRIGGER TRG_Update_TongTien_PhieuDat
ON PHIEUDATTIEC
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Cập nhật Đơn giá, Tổng tiền bàn, dịch vụ, hóa đơn
    UPDATE p
    SET 
        TongTienBan = p.SoLuongBan * (
            p.DonGiaBanTiec + ISNULL((
                SELECT SUM(td.DonGia * td.SoLuong)
                FROM THUCDON td
                WHERE td.MaPhieuDat = p.MaPhieuDat
            ), 0)
        ),
        TongTienDV = ISNULL((
            SELECT SUM(ctdv.ThanhTien)
            FROM CHITIETDV ctdv
            WHERE ctdv.MaPhieuDat = p.MaPhieuDat
        ), 0),
        TongTienHoaDon = 
            p.SoLuongBan * (
                p.DonGiaBanTiec + ISNULL((
                    SELECT SUM(td.DonGia * td.SoLuong)
                    FROM THUCDON td
                    WHERE td.MaPhieuDat = p.MaPhieuDat
                ), 0)
            ) +
            ISNULL((
                SELECT SUM(ctdv.ThanhTien)
                FROM CHITIETDV ctdv
                WHERE ctdv.MaPhieuDat = p.MaPhieuDat
            ), 0) +
            ISNULL(p.ChiPhiPhatSinh, 0)
    FROM PHIEUDATTIEC p
    JOIN SANH s ON p.MaSanh = s.MaSanh
    JOIN LOAISANH ls ON s.MaLoaiSanh = ls.MaLoaiSanh
    WHERE p.MaPhieuDat IN (SELECT MaPhieuDat FROM inserted);

    -- Lấy giá trị tham số phạt
    DECLARE @TiLePhat FLOAT = ISNULL((SELECT TOP 1 GiaTri FROM THAMSO WHERE TenThamSo = 'TiLePhat'), 0);
    DECLARE @KiemTraPhat FLOAT = ISNULL((SELECT TOP 1 GiaTri FROM THAMSO WHERE TenThamSo = 'KiemTraPhat'), 0);

    -- Cập nhật Tiền phạt và Tiền còn lại
    UPDATE p
    SET 
        TienPhat = 
            CASE 
                WHEN i.NgayThanhToan IS NOT NULL AND i.NgayThanhToan > i.NgayDaiTiec THEN
                    DATEDIFF(DAY, i.NgayDaiTiec, i.NgayThanhToan) *
                    @TiLePhat * @KiemTraPhat *
                    (p.TongTienHoaDon - ISNULL(i.TienDatCoc, 0))
                ELSE 0
            END,
        TienConLai =
            p.TongTienHoaDon +
            CASE 
                WHEN i.NgayThanhToan IS NOT NULL AND i.NgayThanhToan > i.NgayDaiTiec THEN
                    DATEDIFF(DAY, i.NgayDaiTiec, i.NgayThanhToan) *
                    @TiLePhat * @KiemTraPhat *
                    (p.TongTienHoaDon - ISNULL(i.TienDatCoc, 0))
                ELSE 0
            END - ISNULL(i.TienDatCoc, 0)
    FROM PHIEUDATTIEC p
    JOIN inserted i ON p.MaPhieuDat = i.MaPhieuDat
    WHERE i.NgayThanhToan IS NOT NULL;
END;

GO
CREATE TRIGGER TRG_Update_BaoCaoDS
ON PHIEUDATTIEC
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Changes TABLE (
        Ngay INT, 
        Thang INT, 
        Nam INT, 
        DoanhThuChange MONEY, 
        SoLuongTiecChange INT
    );

   -- Xử lý bản ghi bị xóa
	INSERT INTO @Changes (Ngay, Thang, Nam, DoanhThuChange, SoLuongTiecChange)
	SELECT 
		DAY(NgayDaiTiec),
		MONTH(NgayDaiTiec),
		YEAR(NgayDaiTiec),
		-1 * (
			ISNULL(TienDatCoc, 0) +
			CASE WHEN NgayThanhToan IS NOT NULL THEN ISNULL(TienConLai, 0) ELSE 0 END
		),
		-1
	FROM DELETED
	WHERE NgayDaiTiec IS NOT NULL;

	-- Xử lý bản ghi thêm hoặc cập nhật
	INSERT INTO @Changes (Ngay, Thang, Nam, DoanhThuChange, SoLuongTiecChange)
	SELECT 
		DAY(NgayDaiTiec),
		MONTH(NgayDaiTiec),
		YEAR(NgayDaiTiec),
		ISNULL(TienDatCoc, 0) +
		CASE WHEN NgayThanhToan IS NOT NULL THEN ISNULL(TienConLai, 0) ELSE 0 END,
		1
	FROM INSERTED
	WHERE NgayDaiTiec IS NOT NULL;


    -- Tổng hợp thay đổi
    DECLARE @AggregatedChanges TABLE (
        Ngay INT,
        Thang INT,
        Nam INT,
        TotalDoanhThu MONEY,
        TotalSoLuongTiec INT
    );

    INSERT INTO @AggregatedChanges
    SELECT 
        Ngay,
        Thang,
        Nam,
        SUM(DoanhThuChange),
        SUM(SoLuongTiecChange)
    FROM @Changes
    GROUP BY Ngay, Thang, Nam;

    -- Đảm bảo bản ghi BAOCAODS tồn tại
    MERGE BAOCAODS AS target
    USING (
        SELECT DISTINCT Thang, Nam FROM @AggregatedChanges
    ) AS source
    ON target.Thang = source.Thang AND target.Nam = source.Nam
    WHEN NOT MATCHED THEN
        INSERT (Thang, Nam, TongDoanhThu)
        VALUES (source.Thang, source.Nam, 0);

    -- Cập nhật CTBAOCAODS
    MERGE CTBAOCAODS AS target
    USING @AggregatedChanges AS source
    ON target.Ngay = source.Ngay AND target.Thang = source.Thang AND target.Nam = source.Nam
    WHEN MATCHED THEN
        UPDATE SET 
            SoLuongTiec = target.SoLuongTiec + source.TotalSoLuongTiec,
            DoanhThu = target.DoanhThu + source.TotalDoanhThu
    WHEN NOT MATCHED AND source.TotalSoLuongTiec > 0 THEN
        INSERT (Ngay, Thang, Nam, SoLuongTiec, DoanhThu, TiLe)
        VALUES (
            source.Ngay,
            source.Thang,
            source.Nam,
            source.TotalSoLuongTiec,
            source.TotalDoanhThu,
            0
        );

    -- Cập nhật tổng doanh thu tháng
    UPDATE b
	SET TongDoanhThu = (
		SELECT SUM(DoanhThu)
		FROM CTBAOCAODS c
		WHERE c.Thang = b.Thang AND c.Nam = b.Nam
	)
	FROM BAOCAODS b
	WHERE EXISTS (
		SELECT 1
		FROM @AggregatedChanges ac
		WHERE ac.Thang = b.Thang AND ac.Nam = b.Nam
	);

    -- Cập nhật lại toàn bộ tỉ lệ TiLe trong tháng có thay đổi
    UPDATE c
    SET TiLe = CASE 
                WHEN b.TongDoanhThu = 0 THEN 0
                ELSE TRY_CAST(c.DoanhThu AS FLOAT) * 100.0 / NULLIF(TRY_CAST(b.TongDoanhThu AS FLOAT), 0)
              END
    FROM CTBAOCAODS c
    JOIN BAOCAODS b ON c.Thang = b.Thang AND c.Nam = b.Nam
    WHERE EXISTS (
        SELECT 1 FROM @AggregatedChanges ac
        WHERE ac.Thang = c.Thang AND ac.Nam = c.Nam
    );
END;


GO
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
('STAFF', 'Home'),
('STAFF', 'HallType'),
('STAFF', 'Hall'),
('STAFF', 'Shift'),
('STAFF', 'Food'),
('STAFF', 'Service'),
('STAFF', 'Wedding'),
('STAFF', 'Report'),
('STAFF', 'Parameter'),
('STAFF', 'Permission'),
('STAFF', 'User');

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

INSERT INTO SANH (MaLoaiSanh, TenSanh, SoLuongBanToiDa, GhiChu)
VALUES
    (6, N'Jade', 32, N'Trang trí phong cách Á Đông'),
    (6, N'Amber', 28, N'Ánh sáng vàng ấm áp'),
    (7, N'Topaz', 30, N'Có hệ thống âm thanh cao cấp'),
    (7, N'Garnet', 25, N'Phù hợp tiệc cưới truyền thống'),
    (8, N'Aquamarine', 22, N'Tông màu xanh biển nhẹ nhàng'),
    (8, N'Citrine', 24, N'Không gian trẻ trung'),
    (9, N'Peridot', 20, N'Trang trí cây xanh tự nhiên'),
    (9, N'Tanzanite', 18, N'Màu sắc độc đáo'),
    (10, N'Zircon', 26, N'Có khu vực chụp ảnh riêng'),
    (10, N'Turquoise', 28, N'Phong cách Bohemian'),
    (11, N'Onyx', 30, N'Trang trí đen trắng sang trọng'),
    (11, N'Lapis Lazuli', 25, N'Tông màu xanh hoàng gia'),
    (12, N'Malachite', 22, N'Họa tiết thiên nhiên'),
    (12, N'Moonstone', 24, N'Ánh sáng dịu nhẹ'),
    (13, N'Sunstone', 20, N'Tràn ngập ánh sáng'),
    (13, N'Labradorite', 18, N'Hiệu ứng ánh kim độc đáo'),
    (14, N'Rhodonite', 26, N'Kết hợp gỗ và đá tự nhiên'),
    (14, N'Amazonite', 28, N'Phong cách vintage'),
    (15, N'Kunzite', 30, N'Sắc hồng lãng mạn'),
    (15, N'Spinel', 25, N'Trang trí pha lê cao cấp');



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

-- CA
INSERT INTO CA (TenCa, ThoiGianBatDauCa, ThoiGianKetThucCa)
VALUES (N'Ca sáng', '09:00', '15:00');
INSERT INTO CA (TenCa, ThoiGianBatDauCa, ThoiGianKetThucCa)
VALUES (N'Ca tối', '17:00', '23:00');

-- MONAN
INSERT INTO MONAN (TenMonAn, DonGia, GhiChu)
VALUES 
(N'Súp cua', 50000, N'Món khai vị'),
(N'Cá hấp xì dầu', 120000, N'Món chính');
INSERT INTO MONAN (TenMonAn, DonGia, GhiChu)
VALUES 
(N'Bò kho', 66175, N'Món đặc biệt'),
(N'Cơm tấm', 65451, N'Món khai vị'),
(N'Lẩu gà lá é', 56685, N'Món đặc biệt'),
(N'Gà rang muối', 187835, N'Món chính'),
(N'Gỏi cuốn', 68192, N'Món chính'),
(N'Vịt quay', 152310, N'Món khai vị'),
(N'Bánh hỏi', 105045, N'Món đặc biệt'),
(N'Cá chiên', 163277, N'Món đặc biệt'),
(N'Canh chua', 171184, N'Món tráng miệng'),
(N'Tôm chiên', 87469, N'Món tráng miệng'),
(N'Cháo gà', 51317, N'Món tráng miệng'),
(N'Gỏi xoài', 67096, N'Món chính'),
(N'Bún chả', 62076, N'Món tráng miệng'),
(N'Mực nướng', 71844, N'Món khai vị'),
(N'Gà luộc', 78276, N'Món phụ')

--thứ tự
--5 phiếu đã thanh toán trước ngày 24/06
--5 phiếu chưa thanh toán vào ngày 24/06
--5 phiếu chưa thanh toán trước ngày 24/06
--5 phiếu chưa thanh toán sau ngày 24/06

INSERT INTO PHIEUDATTIEC (TenChuRe, TenCoDau, DienThoai, NgayDatTiec, NgayDaiTiec,
    MaCa, MaSanh, TienDatCoc, SoLuongBan, SoBanDuTru, DonGiaBanTiec)
VALUES 
(N'Nguyễn Văn Đức', N'Trần Thị Nguyệt', '0912345678', '2025-05-10', '2025-06-20', 1, 1, 7000000, 27, 1, 1000000),
(N'Lê Văn Chí', N'Phạm Thị Tố', '0923456789', '2025-05-15', '2025-06-19', 2, 2, 7500000, 26, 1, 1000000),

(N'Nguyễn Văn Anh', N'Trần Thị Bé', '0912345678', '2025-05-10', '2025-06-24', 1, 1, 9000000, 27, 1, 1000000),

(N'Nguyễn Tự Tuấn', N'Trần Liễu My', '0912345678', '2025-05-10', '2025-06-20', 1, 1, 10000000, 27, 1, 1000000),

(N'Nguyễn Đại Lương', N'Trần Nữ Uyên', '0912345678', '2025-05-10', '2025-06-26', 1, 1, 9000000, 27, 1, 1000000);


INSERT INTO PHIEUDATTIEC (TenChuRe, TenCoDau, DienThoai, NgayDatTiec, NgayDaiTiec,
    MaCa, MaSanh, TienDatCoc, SoLuongBan, SoBanDuTru, DonGiaBanTiec)
VALUES 
(N'Hoàng Văn Mai', N'Ngô Thị Phấn', '0934567890', '2025-05-20', '2025-06-18', 1, 3, 8000000, 24, 0, 1100000),

(N'Lê Văn Cường', N'Phạm Thị Dương', '0923456789', '2025-05-15', '2025-06-24', 2, 2, 15000000, 26, 1, 1000000),
(N'Hoàng Văn E', N'Ngô Thị F', '0934567890', '2025-05-20', '2025-06-24', 1, 3, 8000000, 24, 0, 1100000),

(N'Lê Trọng Nghĩa', N'Phạm Tuyết Lan', '0923456789', '2025-05-15', '2025-06-19', 2, 2, 10500000, 26, 1, 1000000),

(N'Lê Tâm Việt', N'Phạm Xuân Nhi', '0923456789', '2025-05-15', '2025-06-27', 2, 2, 15000000, 26, 1, 1000000);

INSERT INTO PHIEUDATTIEC (TenChuRe, TenCoDau, DienThoai, NgayDatTiec, NgayDaiTiec,
    MaCa, MaSanh, TienDatCoc, SoLuongBan, SoBanDuTru, DonGiaBanTiec)
VALUES 
(N'Phan Văn Tài', N'Đỗ Thị Hồng', '0945678901', '2025-05-25', '2025-06-17', 2, 4, 9000000, 24, 0, 1100000),

(N'Phan Văn Giang', N'Đỗ Thị Hướng', '0945678901', '2025-05-25', '2025-06-24', 2, 4, 9000000, 22, 0, 1100000),

(N'Hoàng Mạnh Sơn', N'Ngô Quế Đài', '0934567890', '2025-05-20', '2025-06-18', 1, 3, 12000000, 24, 0, 1100000),
(N'Phan Chính Huy', N'Đỗ Mỹ Thanh', '0945678901', '2025-05-25', '2025-06-17', 2, 4, 9000000, 22, 0, 1100000),

(N'Hoàng Bảo Phát', N'Ngô Sương Vy', '0934567890', '2025-05-20', '2025-07-17', 1, 3, 8000000, 24, 0, 1100000);

INSERT INTO PHIEUDATTIEC (TenChuRe, TenCoDau, DienThoai, NgayDatTiec, NgayDaiTiec,
    MaCa, MaSanh, TienDatCoc, SoLuongBan, SoBanDuTru, DonGiaBanTiec)
VALUES 
(N'Trịnh Văn Tuấn', N'Bùi Thị Trinh', '0956789012', '2025-05-28', '2025-06-16', 1, 5, 9600000, 23, 1, 1200000),

(N'Trịnh Văn Yên', N'Bùi Thị Kiều', '0956789012', '2025-05-28', '2025-06-24', 1, 5, 9600000, 23, 1, 1200000),

(N'Trịnh Chương Sĩ', N'Bùi Liên Yến', '0956789012', '2025-05-28', '2025-06-16', 1, 5, 9600000, 23, 1, 1200000),

(N'Phan An Bình', N'Đỗ Oanh Ý', '0945678901', '2025-05-25', '2025-07-12', 2, 4, 9000000, 22, 0, 1100000),
(N'Trịnh Ninh Xuyên', N'Bùi Ánh Châu', '0956789012', '2025-05-28', '2025-07-11', 1, 5, 9600000, 23, 1, 1200000);

-- CẬP NHẬT thông tin thanh toán cho 5 phiếu
UPDATE PHIEUDATTIEC
SET NgayThanhToan = '2025-06-21', ChiPhiPhatSinh = 1000000
WHERE MaPhieuDat = 1;

UPDATE PHIEUDATTIEC
SET NgayThanhToan = '2025-06-22', ChiPhiPhatSinh = 1200000
WHERE MaPhieuDat = 2;

UPDATE PHIEUDATTIEC
SET NgayThanhToan = '2025-06-23', ChiPhiPhatSinh = 1500000
WHERE MaPhieuDat = 6;

UPDATE PHIEUDATTIEC
SET NgayThanhToan = '2025-06-23', ChiPhiPhatSinh = 800000
WHERE MaPhieuDat = 11;

UPDATE PHIEUDATTIEC
SET NgayThanhToan = '2025-06-22', ChiPhiPhatSinh = 1000000
WHERE MaPhieuDat = 16;


INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(1, 1, 1, 50000, N'Súp cua - khai vị'),
(1, 2, 1, 120000, N'Cá hấp xì dầu - món chính'),
(1, 4, 1, 65451, N'Cơm tấm - món phụ'),
(1, 6, 2, 187835, N'Gà rang muối - món chính (x2 mỗi bàn)'),
(1, 11, 1, 171184, N'Canh chua - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(1, 1, 1, 2000000, 2000000, N'Âm thanh tiêu chuẩn'),
(1, 5, 1, 3000000, 3000000, N'Trang trí hoa tươi'),
(1, 8, 1, 3500000, 3500000, N'MC dẫn chương trình');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(2, 3, 1, 66175, N'Bò kho - món đặc biệt'),
(2, 5, 1, 56685, N'Lẩu gà lá é'),
(2, 7, 2, 68192, N'Gỏi cuốn (x2 mỗi bàn)'),
(2, 13, 1, 51317, N'Cháo gà - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(2, 2, 1, 2500000, 2500000, N'Hệ thống ánh sáng'),
(2, 10, 2, 800000, 1600000, N'Pháo điện mỗi tiết mục'),
(2, 12, 1, 4500000, 4500000, N'Chụp hình cưới');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(3, 8, 1, 152310, N'Vịt quay - khai vị'),
(3, 9, 1, 105045, N'Bánh hỏi'),
(3, 10, 1, 163277, N'Cá chiên - món đặc biệt'),
(3, 14, 1, 67096, N'Gỏi xoài - món chính'),
(3, 15, 1, 62076, N'Bún chả - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(3, 3, 1, 5000000, 5000000, N'Ca sĩ biểu diễn'),
(3, 6, 1, 1000000, 1000000, N'Trang trí bóng bay'),
(3, 9, 1, 1500000, 1500000, N'Xe hoa đưa đón');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(4, 1, 1, 50000, N'Súp cua'),
(4, 6, 2, 187835, N'Gà rang muối (x2 mỗi bàn)'),
(4, 11, 1, 171184, N'Canh chua'),
(4, 16, 1, 71844, N'Mực nướng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(4, 4, 1, 7000000, 7000000, N'Ban nhạc sống'),
(4, 7, 1, 4000000, 4000000, N'Nhóm múa khai tiệc'),
(4, 11, 1, 1200000, 1200000, N'Thiết kế phông nền');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(5, 2, 1, 120000, N'Cá hấp xì dầu'),
(5, 6, 1, 187835, N'Gà rang muối'),
(5, 12, 1, 87469, N'Tôm chiên - tráng miệng'),
(5, 13, 1, 51317, N'Cháo gà - tráng miệng'),
(5, 17, 1, 78276, N'Gà luộc - món phụ');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(5, 5, 1, 3000000, 3000000, N'Trang trí hoa tươi'),
(5, 8, 1, 3500000, 3500000, N'MC chuyên nghiệp'),
(5, 14, 1, 3000000, 3000000, N'Làm album cưới');



--5 phiếu diễn ra vào ngày 24/06


INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(6, 1, 1, 50000, N'Súp cua - khai vị'),
(6, 2, 1, 120000, N'Cá hấp xì dầu - món chính'),
(6, 4, 1, 65451, N'Cơm tấm - món phụ'),
(6, 6, 2, 187835, N'Gà rang muối - món chính (x2 mỗi bàn)'),
(6, 11, 1, 171184, N'Canh chua - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(6, 1, 1, 2000000, 2000000, N'Âm thanh tiêu chuẩn'),
(6, 5, 1, 3000000, 3000000, N'Trang trí hoa tươi'),
(6, 8, 1, 3500000, 3500000, N'MC dẫn chương trình');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(7, 3, 1, 66175, N'Bò kho - món đặc biệt'),
(7, 5, 1, 56685, N'Lẩu gà lá é'),
(7, 7, 2, 68192, N'Gỏi cuốn (x2 mỗi bàn)'),
(7, 13, 1, 51317, N'Cháo gà - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(7, 2, 1, 2500000, 2500000, N'Hệ thống ánh sáng'),
(7, 10, 2, 800000, 1600000, N'Pháo điện mỗi tiết mục'),
(7, 12, 1, 4500000, 4500000, N'Chụp hình cưới');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(8, 8, 1, 152310, N'Vịt quay - khai vị'),
(8, 9, 1, 105045, N'Bánh hỏi'),
(8, 10, 1, 163277, N'Cá chiên - món đặc biệt'),
(8, 14, 1, 67096, N'Gỏi xoài - món chính'),
(8, 15, 1, 62076, N'Bún chả - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(8, 3, 1, 5000000, 5000000, N'Ca sĩ biểu diễn'),
(8, 6, 1, 1000000, 1000000, N'Trang trí bóng bay'),
(8, 9, 1, 1500000, 1500000, N'Xe hoa đưa đón');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(9, 1, 1, 50000, N'Súp cua'),
(9, 6, 2, 187835, N'Gà rang muối (x2 mỗi bàn)'),
(9, 11, 1, 171184, N'Canh chua'),
(9, 16, 1, 71844, N'Mực nướng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(9, 4, 1, 7000000, 7000000, N'Ban nhạc sống'),
(9, 7, 1, 4000000, 4000000, N'Nhóm múa khai tiệc'),
(9, 11, 1, 1200000, 1200000, N'Thiết kế phông nền');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(10, 2, 1, 120000, N'Cá hấp xì dầu'),
(10, 6, 1, 187835, N'Gà rang muối'),
(10, 12, 1, 87469, N'Tôm chiên - tráng miệng'),
(10, 13, 1, 51317, N'Cháo gà - tráng miệng'),
(10, 17, 1, 78276, N'Gà luộc - món phụ');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(10, 5, 1, 3000000, 3000000, N'Trang trí hoa tươi'),
(10, 8, 1, 3500000, 3500000, N'MC chuyên nghiệp'),
(10, 14, 1, 3000000, 3000000, N'Làm album cưới');

--5 phiếu đã thanh toán trước ngày 24/06
-- THÊM 5 PHIẾU ĐẶT TIỆC (chưa thanh toán)


INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(11, 1, 1, 50000, N'Súp cua - khai vị'),
(11, 2, 1, 120000, N'Cá hấp xì dầu - món chính'),
(11, 4, 1, 65451, N'Cơm tấm - món phụ'),
(11, 6, 2, 187835, N'Gà rang muối - món chính (x2 mỗi bàn)'),
(11, 11, 1, 171184, N'Canh chua - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(11, 1, 1, 2000000, 2000000, N'Âm thanh tiêu chuẩn'),
(11, 5, 1, 3000000, 3000000, N'Trang trí hoa tươi'),
(11, 8, 1, 3500000, 3500000, N'MC dẫn chương trình');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(12, 3, 1, 66175, N'Bò kho - món đặc biệt'),
(12, 5, 1, 56685, N'Lẩu gà lá é'),
(12, 7, 2, 68192, N'Gỏi cuốn (x2 mỗi bàn)'),
(12, 13, 1, 51317, N'Cháo gà - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(12, 2, 1, 2500000, 2500000, N'Hệ thống ánh sáng'),
(12, 10, 2, 800000, 1600000, N'Pháo điện mỗi tiết mục'),
(12, 12, 1, 4500000, 4500000, N'Chụp hình cưới');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(13, 8, 1, 152310, N'Vịt quay - khai vị'),
(13, 9, 1, 105045, N'Bánh hỏi'),
(13, 10, 1, 163277, N'Cá chiên - món đặc biệt'),
(13, 14, 1, 67096, N'Gỏi xoài - món chính'),
(13, 15, 1, 62076, N'Bún chả - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(13, 3, 1, 5000000, 5000000, N'Ca sĩ biểu diễn'),
(13, 6, 1, 1000000, 1000000, N'Trang trí bóng bay'),
(13, 9, 1, 1500000, 1500000, N'Xe hoa đưa đón');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(14, 1, 1, 50000, N'Súp cua'),
(14, 6, 2, 187835, N'Gà rang muối (x2 mỗi bàn)'),
(14, 11, 1, 171184, N'Canh chua'),
(14, 16, 1, 71844, N'Mực nướng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(14, 4, 1, 7000000, 7000000, N'Ban nhạc sống'),
(14, 7, 1, 4000000, 4000000, N'Nhóm múa khai tiệc'),
(14, 11, 1, 1200000, 1200000, N'Thiết kế phông nền');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(15, 2, 1, 120000, N'Cá hấp xì dầu'),
(15, 6, 1, 187835, N'Gà rang muối'),
(15, 12, 1, 87469, N'Tôm chiên - tráng miệng'),
(15, 13, 1, 51317, N'Cháo gà - tráng miệng'),
(15, 17, 1, 78276, N'Gà luộc - món phụ');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(15, 5, 1, 3000000, 3000000, N'Trang trí hoa tươi'),
(15, 8, 1, 3500000, 3500000, N'MC chuyên nghiệp'),
(15, 14, 1, 3000000, 3000000, N'Làm album cưới');

--5 phiếu diễn ra sau ngày 24/06


INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(16, 1, 1, 50000, N'Súp cua - khai vị'),
(16, 2, 1, 120000, N'Cá hấp xì dầu - món chính'),
(16, 4, 1, 65451, N'Cơm tấm - món phụ'),
(16, 6, 2, 187835, N'Gà rang muối - món chính (x2 mỗi bàn)'),
(16, 11, 1, 171184, N'Canh chua - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(16, 1, 1, 2000000, 2000000, N'Âm thanh tiêu chuẩn'),
(16, 5, 1, 3000000, 3000000, N'Trang trí hoa tươi'),
(16, 8, 1, 3500000, 3500000, N'MC dẫn chương trình');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(17, 3, 1, 66175, N'Bò kho - món đặc biệt'),
(17, 5, 1, 56685, N'Lẩu gà lá é'),
(17, 7, 2, 68192, N'Gỏi cuốn (x2 mỗi bàn)'),
(17, 13, 1, 51317, N'Cháo gà - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(17, 2, 1, 2500000, 2500000, N'Hệ thống ánh sáng'),
(17, 10, 2, 800000, 1600000, N'Pháo điện mỗi tiết mục'),
(17, 12, 1, 4500000, 4500000, N'Chụp hình cưới');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(18, 8, 1, 152310, N'Vịt quay - khai vị'),
(18, 9, 1, 105045, N'Bánh hỏi'),
(18, 10, 1, 163277, N'Cá chiên - món đặc biệt'),
(18, 14, 1, 67096, N'Gỏi xoài - món chính'),
(18, 15, 1, 62076, N'Bún chả - tráng miệng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(18, 3, 1, 5000000, 5000000, N'Ca sĩ biểu diễn'),
(18, 6, 1, 1000000, 1000000, N'Trang trí bóng bay'),
(18, 9, 1, 1500000, 1500000, N'Xe hoa đưa đón');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(19, 1, 1, 50000, N'Súp cua'),
(19, 6, 2, 187835, N'Gà rang muối (x2 mỗi bàn)'),
(19, 11, 1, 171184, N'Canh chua'),
(19, 16, 1, 71844, N'Mực nướng');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(19, 4, 1, 7000000, 7000000, N'Ban nhạc sống'),
(19, 7, 1, 4000000, 4000000, N'Nhóm múa khai tiệc'),
(19, 11, 1, 1200000, 1200000, N'Thiết kế phông nền');

INSERT INTO THUCDON (MaPhieuDat, MaMonAn, SoLuong, DonGia, GhiChu)
VALUES 
(20, 2, 1, 120000, N'Cá hấp xì dầu'),
(20, 6, 1, 187835, N'Gà rang muối'),
(20, 12, 1, 87469, N'Tôm chiên - tráng miệng'),
(20, 13, 1, 51317, N'Cháo gà - tráng miệng'),
(20, 17, 1, 78276, N'Gà luộc - món phụ');
INSERT INTO CHITIETDV (MaPhieuDat, MaDichVu, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(20, 5, 1, 3000000, 3000000, N'Trang trí hoa tươi'),
(20, 8, 1, 3500000, 3500000, N'MC chuyên nghiệp'),
(20, 14, 1, 3000000, 3000000, N'Làm album cưới');

