-- =============================
-- WEDDING MANAGEMENT DATABASE
-- =============================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'WeddingManagement')
BEGIN
ALTER DATABASE WeddingManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE WeddingManagement;
END
GO

CREATE DATABASE WeddingManagement;
GO

USE WeddingManagement;
GO

ALTER DATABASE WeddingManagement COLLATE SQL_Latin1_General_CP1_CS_AS

-- HALL TYPE TABLE
CREATE TABLE HallType (
HallTypeId INT IDENTITY(1,1) PRIMARY KEY,
HallTypeName NVARCHAR(40) UNIQUE NOT NULL,
MinTablePrice MONEY
);

-- HALL TABLE
CREATE TABLE Hall (
HallId INT IDENTITY(1,1) PRIMARY KEY,
HallTypeId INT,
HallName NVARCHAR(40) UNIQUE NOT NULL,
MaxTableCount INT,
Note NVARCHAR(100),
FOREIGN KEY (HallTypeId) REFERENCES HallType(HallTypeId)
);

-- SHIFT TABLE
CREATE TABLE Shift (
ShiftId INT IDENTITY(1,1) PRIMARY KEY,
ShiftName NVARCHAR(40) UNIQUE NOT NULL,
StartTime TIME,
EndTime TIME
);

-- BOOKING TABLE
CREATE TABLE Booking (
BookingId INT IDENTITY(1,1) PRIMARY KEY,
GroomName NVARCHAR(40),
BrideName NVARCHAR(40),
Phone VARCHAR(10),
BookingDate SMALLDATETIME,
WeddingDate SMALLDATETIME,
ShiftId INT,
HallId INT,
Deposit MONEY,
TableCount INT,
ReserveTableCount INT,
PaymentDate SMALLDATETIME,
TablePrice MONEY,
TotalTableAmount MONEY,
TotalServiceAmount MONEY,
TotalInvoiceAmount MONEY,
RemainingAmount MONEY,
AdditionalCost MONEY,
PenaltyAmount MONEY,
FOREIGN KEY (ShiftId) REFERENCES Shift(ShiftId),
FOREIGN KEY (HallId) REFERENCES Hall(HallId)
);

-- DISH TABLE
CREATE TABLE Dish (
DishId INT IDENTITY(1,1) PRIMARY KEY,
DishName NVARCHAR(40) UNIQUE NOT NULL,
UnitPrice MONEY,
Note NVARCHAR(100)
);

-- MENU TABLE
CREATE TABLE Menu (
BookingId INT,
DishId INT,
Quantity INT,
UnitPrice MONEY,
Note NVARCHAR(100),
PRIMARY KEY (BookingId, DishId),
FOREIGN KEY (BookingId) REFERENCES Booking(BookingId),
FOREIGN KEY (DishId) REFERENCES Dish(DishId)
);

-- SERVICE TABLE
CREATE TABLE Service (
ServiceId INT IDENTITY(1,1) PRIMARY KEY,
ServiceName NVARCHAR(40) UNIQUE NOT NULL,
UnitPrice MONEY,
Note NVARCHAR(100)
);

-- SERVICE DETAIL TABLE
CREATE TABLE ServiceDetail (
BookingId INT,
ServiceId INT,
Quantity INT,
UnitPrice MONEY,
TotalAmount MONEY,
Note NVARCHAR(100),
PRIMARY KEY (BookingId, ServiceId),
FOREIGN KEY (BookingId) REFERENCES Booking(BookingId),
FOREIGN KEY (ServiceId) REFERENCES Service(ServiceId)
);

-- REVENUE REPORT TABLE
CREATE TABLE RevenueReport (
Month INT,
Year INT,
TotalRevenue MONEY,
PRIMARY KEY (Month, Year)
);

CREATE TABLE RevenueReportDetail (
Day INT,
Month INT,
Year INT,
WeddingCount INT,
Revenue MONEY,
Ratio DECIMAL(7,2),
PRIMARY KEY (Day, Month, Year),
FOREIGN KEY (Month, Year) REFERENCES RevenueReport(Month, Year)
);

-- PARAMETER TABLE
CREATE TABLE Parameter (
ParameterName NVARCHAR(100) PRIMARY KEY,
Value DECIMAL(5,2)
);

INSERT INTO Parameter (ParameterName, Value) VALUES
('EnablePenalty', 1), -- BIT: 0 = disabled, 1 = enabled (default value can be 0)
('PenaltyRate', 0.01), -- DECIMAL(5,2)
('MinDepositRate', 0.15), -- DECIMAL(5,2)
('MinReserveTableRate', 0.85); -- DECIMAL(5,2)

-- PERMISSION TABLES
CREATE TABLE Permission (
PermissionId VARCHAR(10) PRIMARY KEY,
PermissionName NVARCHAR(100),
LoadedScreenName NVARCHAR(100)
);

CREATE TABLE UserGroup (
GroupId VARCHAR(10) PRIMARY KEY,
GroupName NVARCHAR(100) UNIQUE NOT NULL
);

CREATE TABLE GroupPermission (
GroupId VARCHAR(10),
PermissionId VARCHAR(10),
PRIMARY KEY (GroupId, PermissionId),
FOREIGN KEY (GroupId) REFERENCES UserGroup(GroupId),
FOREIGN KEY (PermissionId) REFERENCES Permission(PermissionId)
);

CREATE TABLE AppUser (
UserId INT PRIMARY KEY IDENTITY(1,1),
Username VARCHAR(50) UNIQUE NOT NULL,
PasswordHash VARCHAR(256) NOT NULL,
FullName NVARCHAR(100),
Email VARCHAR(100),
PhoneNumber VARCHAR(15),
Address NVARCHAR(200),
BirthDate DATE,
Gender NVARCHAR(10),
GroupId VARCHAR(10),
FOREIGN KEY (GroupId) REFERENCES UserGroup(GroupId)
);

GO
CREATE TRIGGER TRG_Update_TotalAmount_From_Menu
ON Menu
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
SET NOCOUNT ON;

    -- Get list of affected BookingIds
    DECLARE @AffectedBookings TABLE (BookingId INT);

    INSERT INTO @AffectedBookings
    SELECT BookingId FROM inserted
    UNION
    SELECT BookingId FROM deleted;

    -- Update affected booking records
    UPDATE b
    SET
        TotalTableAmount = b.TableCount * (b.TablePrice + ISNULL((SELECT SUM(m.UnitPrice * m.Quantity)
                                                                   FROM Menu m
                                                                   WHERE m.BookingId = b.BookingId), 0)),
        TotalServiceAmount = ISNULL((SELECT SUM(sd.TotalAmount)
                           FROM ServiceDetail sd
                           WHERE sd.BookingId = b.BookingId), 0),
        TotalInvoiceAmount = b.TableCount * (b.TablePrice + ISNULL((SELECT SUM(m.UnitPrice * m.Quantity)
                                                                      FROM Menu m
                                                                      WHERE m.BookingId = b.BookingId), 0)) +
                       ISNULL((SELECT SUM(sd.TotalAmount)
                              FROM ServiceDetail sd
                              WHERE sd.BookingId = b.BookingId), 0) +
                       ISNULL(b.AdditionalCost, 0),
        RemainingAmount = b.TableCount * (b.TablePrice + ISNULL((SELECT SUM(m.UnitPrice * m.Quantity)
                                                                  FROM Menu m
                                                                  WHERE m.BookingId = b.BookingId), 0)) +
                    ISNULL((SELECT SUM(sd.TotalAmount)
                           FROM ServiceDetail sd
                           WHERE sd.BookingId = b.BookingId), 0) +
                    ISNULL(b.AdditionalCost, 0) +
                    ISNULL(b.PenaltyAmount, 0) -
                    ISNULL(b.Deposit, 0)
    FROM Booking b
    JOIN Hall h ON b.HallId = h.HallId
    JOIN HallType ht ON h.HallTypeId = ht.HallTypeId
    WHERE b.BookingId IN (SELECT BookingId FROM @AffectedBookings);

END;

GO
CREATE TRIGGER TRG_Update_TotalAmount_From_ServiceDetail
ON ServiceDetail
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
SET NOCOUNT ON;

    -- Get list of affected BookingIds
    DECLARE @AffectedBookings TABLE (BookingId INT);

    INSERT INTO @AffectedBookings
    SELECT BookingId FROM inserted
    UNION
    SELECT BookingId FROM deleted;

    -- Update affected booking records
    UPDATE b
    SET
        TotalServiceAmount = ISNULL((SELECT SUM(sd.TotalAmount)
                           FROM ServiceDetail sd
                           WHERE sd.BookingId = b.BookingId), 0),
        TotalInvoiceAmount = b.TotalTableAmount +
                        ISNULL((SELECT SUM(sd.TotalAmount)
                               FROM ServiceDetail sd
                               WHERE sd.BookingId = b.BookingId), 0) +
                        ISNULL(b.AdditionalCost, 0),
        RemainingAmount = b.TotalTableAmount +
                    ISNULL((SELECT SUM(sd.TotalAmount)
                           FROM ServiceDetail sd
                           WHERE sd.BookingId = b.BookingId), 0) +
                    ISNULL(b.AdditionalCost, 0) +
                    ISNULL(b.PenaltyAmount, 0) -
                    ISNULL(b.Deposit, 0)
    FROM Booking b
    WHERE b.BookingId IN (SELECT BookingId FROM @AffectedBookings);

END;

GO
CREATE TRIGGER TRG_Update_TotalAmount_Booking
ON Booking
AFTER INSERT, UPDATE
AS
BEGIN
SET NOCOUNT ON;

    -- Update Unit price, Total table amount, service amount, invoice amount
    UPDATE b
    SET
        TotalTableAmount = b.TableCount * (
            b.TablePrice + ISNULL((
                SELECT SUM(m.UnitPrice * m.Quantity)
                FROM Menu m
                WHERE m.BookingId = b.BookingId
            ), 0)
        ),
        TotalServiceAmount = ISNULL((
            SELECT SUM(sd.TotalAmount)
            FROM ServiceDetail sd
            WHERE sd.BookingId = b.BookingId
        ), 0),
        TotalInvoiceAmount =
            b.TableCount * (
                b.TablePrice + ISNULL((
                    SELECT SUM(m.UnitPrice * m.Quantity)
                    FROM Menu m
                    WHERE m.BookingId = b.BookingId
                ), 0)
            ) +
            ISNULL((
                SELECT SUM(sd.TotalAmount)
                FROM ServiceDetail sd
                WHERE sd.BookingId = b.BookingId
            ), 0) +
            ISNULL(b.AdditionalCost, 0)
    FROM Booking b
    JOIN Hall h ON b.HallId = h.HallId
    JOIN HallType ht ON h.HallTypeId = ht.HallTypeId
    WHERE b.BookingId IN (SELECT BookingId FROM inserted);

    -- Get penalty parameter values
    DECLARE @PenaltyRate FLOAT = ISNULL((SELECT TOP 1 Value FROM Parameter WHERE ParameterName = 'PenaltyRate'), 0);
    DECLARE @EnablePenalty FLOAT = ISNULL((SELECT TOP 1 Value FROM Parameter WHERE ParameterName = 'EnablePenalty'), 0);

    -- Update Penalty amount and Remaining amount
    UPDATE b
    SET
        PenaltyAmount =
            CASE
                WHEN i.PaymentDate IS NOT NULL AND i.PaymentDate > i.WeddingDate THEN
                    DATEDIFF(DAY, i.WeddingDate, i.PaymentDate) *
                    @PenaltyRate * @EnablePenalty *
                    (b.TotalInvoiceAmount - ISNULL(i.Deposit, 0))
                ELSE 0
            END,
        RemainingAmount =
            b.TotalInvoiceAmount +
            CASE
                WHEN i.PaymentDate IS NOT NULL AND i.PaymentDate > i.WeddingDate THEN
                    DATEDIFF(DAY, i.WeddingDate, i.PaymentDate) *
                    @PenaltyRate * @EnablePenalty *
                    (b.TotalInvoiceAmount - ISNULL(i.Deposit, 0))
                ELSE 0
            END - ISNULL(i.Deposit, 0)
    FROM Booking b
    JOIN inserted i ON b.BookingId = i.BookingId
    WHERE i.PaymentDate IS NOT NULL;

END;

GO
CREATE TRIGGER TRG_Update_RevenueReport
ON Booking
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
SET NOCOUNT ON;

    DECLARE @Changes TABLE (
        Day INT,
        Month INT,
        Year INT,
        RevenueChange MONEY,
        WeddingCountChange INT
    );

    -- Handle deleted records
    INSERT INTO @Changes (Day, Month, Year, RevenueChange, WeddingCountChange)
    SELECT
        DAY(WeddingDate),
        MONTH(WeddingDate),
        YEAR(WeddingDate),
        -1 * (
            ISNULL(Deposit, 0) +
            CASE WHEN PaymentDate IS NOT NULL THEN ISNULL(RemainingAmount, 0) ELSE 0 END
        ),
        -1
    FROM DELETED
    WHERE WeddingDate IS NOT NULL;

    -- Handle inserted or updated records
    INSERT INTO @Changes (Day, Month, Year, RevenueChange, WeddingCountChange)
    SELECT
        DAY(WeddingDate),
        MONTH(WeddingDate),
        YEAR(WeddingDate),
        ISNULL(Deposit, 0) +
        CASE WHEN PaymentDate IS NOT NULL THEN ISNULL(RemainingAmount, 0) ELSE 0 END,
        1
    FROM INSERTED
    WHERE WeddingDate IS NOT NULL;

    -- Aggregate changes
    DECLARE @AggregatedChanges TABLE (
        Day INT,
        Month INT,
        Year INT,
        TotalRevenue MONEY,
        TotalWeddingCount INT
    );

    INSERT INTO @AggregatedChanges
    SELECT
        Day,
        Month,
        Year,
        SUM(RevenueChange),
        SUM(WeddingCountChange)
    FROM @Changes
    GROUP BY Day, Month, Year;

    -- Ensure RevenueReport record exists
    MERGE RevenueReport AS target
    USING (
        SELECT DISTINCT Month, Year FROM @AggregatedChanges
    ) AS source
    ON target.Month = source.Month AND target.Year = source.Year
    WHEN NOT MATCHED THEN
        INSERT (Month, Year, TotalRevenue)
        VALUES (source.Month, source.Year, 0);

    -- Update RevenueReportDetail
    MERGE RevenueReportDetail AS target
    USING @AggregatedChanges AS source
    ON target.Day = source.Day AND target.Month = source.Month AND target.Year = source.Year
    WHEN MATCHED THEN
        UPDATE SET
            WeddingCount = target.WeddingCount + source.TotalWeddingCount,
            Revenue = target.Revenue + source.TotalRevenue
    WHEN NOT MATCHED AND source.TotalWeddingCount > 0 THEN
        INSERT (Day, Month, Year, WeddingCount, Revenue, Ratio)
        VALUES (
            source.Day,
            source.Month,
            source.Year,
            source.TotalWeddingCount,
            source.TotalRevenue,
            0
        );

    -- Update monthly total revenue
    UPDATE r
    SET TotalRevenue = (
        SELECT SUM(Revenue)
        FROM RevenueReportDetail rd
        WHERE rd.Month = r.Month AND rd.Year = r.Year
    )
    FROM RevenueReport r
    WHERE EXISTS (
        SELECT 1
        FROM @AggregatedChanges ac
        WHERE ac.Month = r.Month AND ac.Year = r.Year
    );

    -- Update all Ratios in affected months
    UPDATE rd
    SET Ratio = CASE
                WHEN r.TotalRevenue = 0 THEN 0
                ELSE TRY_CAST(rd.Revenue AS FLOAT) * 100.0 / NULLIF(TRY_CAST(r.TotalRevenue AS FLOAT), 0)
              END
    FROM RevenueReportDetail rd
    JOIN RevenueReport r ON rd.Month = r.Month AND rd.Year = r.Year
    WHERE EXISTS (
        SELECT 1 FROM @AggregatedChanges ac
        WHERE ac.Month = rd.Month AND ac.Year = rd.Year
    );

END;

GO
INSERT INTO UserGroup (GroupId, GroupName)
VALUES ('ADMIN', N'Administrator');

INSERT INTO UserGroup (GroupId, GroupName)
VALUES ('STAFF', N'Staff');
INSERT INTO UserGroup (GroupId, GroupName)
VALUES ('gr1', N'Staff 1');
INSERT INTO UserGroup (GroupId, GroupName)
VALUES ('gr2', N'Staff 2');
INSERT INTO UserGroup (GroupId, GroupName)
VALUES ('gr3', N'Staff 3');

INSERT INTO AppUser (Username, PasswordHash, FullName, Email, GroupId)
VALUES ('Fartiel', 'db69fc039dcbd2962cb4d28f5891aae1', N'Đặng Phú Thiện', '23521476@gm.uit.edu.vn', 'ADMIN');

INSERT INTO AppUser (Username, PasswordHash, FullName, Email, GroupId)
VALUES ('Neith', '978aae9bb6bee8fb75de3e4830a1be46', N'Đặng Phú Thiện', '23521476@gm.uit.edu.vn', 'STAFF');

INSERT INTO Permission (PermissionId, PermissionName, LoadedScreenName) VALUES
('Home', N'Home', N'HomeView'),
('HallType', N'Hall Type', N'HallTypeView'),
('Hall', N'Hall', N'HallView'),
('Shift', N'Shift', N'ShiftView'),
('Food', N'Food', N'FoodView'),
('Service', N'Service', N'ServiceView'),
('Wedding', N'Wedding', N'WeddingView'),
('Report', N'Report', N'ReportView'),
('Parameter', N'Parameter', N'ParameterView'),
('Permission', N'Permission', N'PermissionView'),
('User', N'User', N'UserView');

-- Grant all permissions to ADMIN group
INSERT INTO GroupPermission (GroupId, PermissionId) VALUES
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

INSERT INTO GroupPermission (GroupId, PermissionId) VALUES
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

INSERT INTO HallType (HallTypeName, MinTablePrice)
VALUES
(N'A', 1000000),
(N'B', 1100000),
(N'C', 1200000),
(N'D', 1400000),
(N'E', 1600000);

INSERT INTO HallType (HallTypeName, MinTablePrice) VALUES
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

INSERT INTO Hall (HallTypeId, HallName, MaxTableCount, Note)
VALUES
(1, N'Ruby', 30, N'Near main entrance'),
(1, N'Sapphire', 28, N'Large stage available'),
(2, N'Diamond', 25, N'Open space'),
(2, N'Gold', 22, N'Suitable for small parties'),
(3, N'Silver', 24, N'Modern design'),
(3, N'Platinum', 26, N'Beautiful view'),
(4, N'Emerald', 20, N'Natural lighting'),
(4, N'Opal', 18, N'Warm color scheme'),
(5, N'Pearl', 15, N'For intimate parties'),
(5, N'Crystal', 16, N'Luxurious design');

INSERT INTO Hall (HallTypeId, HallName, MaxTableCount, Note)
VALUES
(6, N'Jade', 32, N'Asian style decoration'),
(6, N'Amber', 28, N'Warm golden lighting'),
(7, N'Topaz', 30, N'Premium sound system'),
(7, N'Garnet', 25, N'Traditional wedding style'),
(8, N'Aquamarine', 22, N'Light blue tone'),
(8, N'Citrine', 24, N'Youthful space'),
(9, N'Peridot', 20, N'Natural plant decoration'),
(9, N'Tanzanite', 18, N'Unique colors'),
(10, N'Zircon', 26, N'Separate photo area'),
(10, N'Turquoise', 28, N'Bohemian style'),
(11, N'Onyx', 30, N'Elegant black and white'),
(11, N'Lapis Lazuli', 25, N'Royal blue tone'),
(12, N'Malachite', 22, N'Nature patterns'),
(12, N'Moonstone', 24, N'Soft lighting'),
(13, N'Sunstone', 20, N'Full of light'),
(13, N'Labradorite', 18, N'Unique shimmer effect'),
(14, N'Rhodonite', 26, N'Wood and natural stone'),
(14, N'Amazonite', 28, N'Vintage style'),
(15, N'Kunzite', 30, N'Romantic pink'),
(15, N'Spinel', 25, N'Premium crystal decoration');

INSERT INTO Service (ServiceName, UnitPrice, Note) VALUES
(N'Sound system', 2000000, N'Standard sound service'),
(N'Lighting', 2500000, N'Professional lighting system'),
(N'Singer performance', 5000000, N'Famous singer performs 1 song'),
(N'Live band', 7000000, N'Live band performance'),
(N'Fresh flower decoration', 3000000, N'Stage and table decoration'),
(N'Balloon decoration', 1000000, N'Custom decoration'),
(N'Opening dance group', 4000000, N'Wedding opening dance'),
(N'Professional MC', 3500000, N'MC for entire party'),
(N'Wedding car', 1500000, N'Bride and groom transportation'),
(N'Electric confetti', 800000, N'Stage effects'),
(N'Backdrop design', 1200000, N'Professional photo backdrop'),
(N'Wedding photography', 4500000, N'Professional photographer'),
(N'Wedding videography', 5000000, N'Full event recording and editing'),
(N'Wedding album', 3000000, N'Designed color-printed album'),
(N'Bridal makeup', 2000000, N'Makeup at home or venue'),
(N'Wedding dress rental', 2500000, N'Premium dresses, various styles'),
(N'Childcare service', 1000000, N'Childcare staff at party'),
(N'Cold fireworks', 6000000, N'Spectacular highlight'),
(N'Elephant bride procession', 10000000, N'Special service, advance booking'),
(N'Gallery table decoration', 1500000, N'Elegant guest welcome table');

-- SHIFTS
INSERT INTO Shift (ShiftName, StartTime, EndTime)
VALUES (N'Morning shift', '09:00', '15:00');
INSERT INTO Shift (ShiftName, StartTime, EndTime)
VALUES (N'Evening shift', '17:00', '23:00');

-- DISHES
INSERT INTO Dish (DishName, UnitPrice, Note)
VALUES
(N'Crab soup', 50000, N'Appetizer'),
(N'Steamed fish with soy sauce', 120000, N'Main course');
INSERT INTO Dish (DishName, UnitPrice, Note)
VALUES
(N'Beef stew', 66175, N'Special dish'),
(N'Broken rice', 65451, N'Appetizer'),
(N'Chicken hotpot with lemon basil', 56685, N'Special dish'),
(N'Salt roasted chicken', 187835, N'Main course'),
(N'Fresh spring rolls', 68192, N'Main course'),
(N'Roasted duck', 152310, N'Appetizer'),
(N'Rice vermicelli', 105045, N'Special dish'),
(N'Fried fish', 163277, N'Special dish'),
(N'Sour soup', 171184, N'Dessert'),
(N'Fried shrimp', 87469, N'Dessert'),
(N'Chicken porridge', 51317, N'Dessert'),
(N'Mango salad', 67096, N'Main course'),
(N'Bun cha', 62076, N'Dessert'),
(N'Grilled squid', 71844, N'Appetizer'),
(N'Boiled chicken', 78276, N'Side dish');

-- Order:
-- 5 bookings paid before June 24
-- 5 bookings unpaid on June 24
-- 5 bookings unpaid before June 24
-- 5 bookings unpaid after June 24

INSERT INTO Booking (GroomName, BrideName, Phone, BookingDate, WeddingDate,
ShiftId, HallId, Deposit, TableCount, ReserveTableCount, TablePrice)
VALUES
(N'Nguyen Van Duc', N'Tran Thi Nguyet', '0912345678', '2025-05-10', '2025-06-20 10:00:00', 1, 1, 7000000, 27, 1, 1000000),
(N'Le Van Chi', N'Pham Thi To', '0923456789', '2025-05-15', '2025-06-19 18:00:00', 2, 2, 7500000, 26, 1, 1000000),

(N'Nguyen Van Anh', N'Tran Thi Be', '0912345678', '2025-05-10', '2025-06-24 10:00:00', 1, 1, 9000000, 27, 1, 1000000),

(N'Nguyen Tu Tuan', N'Tran Lieu My', '0912345678', '2025-05-10', '2025-06-20 18:00:00', 2, 1, 10000000, 27, 1, 1000000),

(N'Nguyen Dai Luong', N'Tran Nu Uyen', '0912345678', '2025-05-10', '2025-06-26 10:00:00', 1, 1, 9000000, 27, 1, 1000000);

INSERT INTO Booking (GroomName, BrideName, Phone, BookingDate, WeddingDate,
ShiftId, HallId, Deposit, TableCount, ReserveTableCount, TablePrice)
VALUES
(N'Hoang Van Mai', N'Ngo Thi Phan', '0934567890', '2025-05-20', '2025-06-18 10:00:00', 1, 3, 8000000, 24, 0, 1100000),

(N'Le Van Cuong', N'Pham Thi Duong', '0923456789', '2025-05-15', '2025-06-24 18:00:00', 2, 2, 15000000, 26, 1, 1000000),
(N'Hoang Van E', N'Ngo Thi F', '0934567890', '2025-05-20', '2025-06-24 10:00:00', 1, 3, 8000000, 24, 0, 1100000),

(N'Le Trong Nghia', N'Pham Tuyet Lan', '0923456789', '2025-05-15', '2025-06-19 10:00:00', 1, 2, 10500000, 26, 1, 1000000),

(N'Le Tam Viet', N'Pham Xuan Nhi', '0923456789', '2025-05-15', '2025-06-27 18:00:00', 2, 2, 15000000, 26, 1, 1000000);

INSERT INTO Booking (GroomName, BrideName, Phone, BookingDate, WeddingDate,
ShiftId, HallId, Deposit, TableCount, ReserveTableCount, TablePrice)
VALUES
(N'Phan Van Tai', N'Do Thi Hong', '0945678901', '2025-05-25', '2025-06-17 18:00:00', 2, 4, 9000000, 24, 0, 1100000),

(N'Phan Van Giang', N'Do Thi Huong', '0945678901', '2025-05-25', '2025-06-24 18:00:00', 2, 4, 9000000, 22, 0, 1100000),

(N'Hoang Manh Son', N'Ngo Que Dai', '0934567890', '2025-05-20', '2025-06-18 18:00:00', 2, 3, 12000000, 24, 0, 1100000),
(N'Phan Chinh Huy', N'Do My Thanh', '0945678901', '2025-05-25', '2025-06-17 10:00:00', 1, 4, 9000000, 22, 0, 1100000),

(N'Hoang Bao Phat', N'Ngo Suong Vy', '0934567890', '2025-05-20', '2025-07-17 10:00:00', 1, 3, 8000000, 24, 0, 1100000);

INSERT INTO Booking (GroomName, BrideName, Phone, BookingDate, WeddingDate,
ShiftId, HallId, Deposit, TableCount, ReserveTableCount, TablePrice)
VALUES
(N'Trinh Van Tuan', N'Bui Thi Trinh', '0956789012', '2025-05-28', '2025-06-16 10:00:00', 1, 5, 9600000, 23, 1, 1200000),

(N'Trinh Van Yen', N'Bui Thi Kieu', '0956789012', '2025-05-28', '2025-06-24 10:00:00', 1, 5, 9600000, 23, 1, 1200000),

(N'Trinh Chuong Si', N'Bui Lien Yen', '0956789012', '2025-05-28', '2025-06-16 18:00:00', 2, 5, 9600000, 23, 1, 1200000),

(N'Phan An Binh', N'Do Oanh Y', '0945678901', '2025-05-25', '2025-07-12 18:00:00', 2, 4, 9000000, 22, 0, 1100000),
(N'Trinh Ninh Xuyen', N'Bui Anh Chau', '0956789012', '2025-05-28', '2025-07-11 10:00:00', 1, 5, 9600000, 23, 1, 1200000);

-- UPDATE payment information for 5 bookings
UPDATE Booking
SET PaymentDate = '2025-06-21', AdditionalCost = 1000000
WHERE BookingId = 1;

UPDATE Booking
SET PaymentDate = '2025-06-22', AdditionalCost = 1200000
WHERE BookingId = 2;

UPDATE Booking
SET PaymentDate = '2025-06-23', AdditionalCost = 1500000
WHERE BookingId = 6;

UPDATE Booking
SET PaymentDate = '2025-06-23', AdditionalCost = 800000
WHERE BookingId = 11;

UPDATE Booking
SET PaymentDate = '2025-06-22', AdditionalCost = 1000000
WHERE BookingId = 16;

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(1, 1, 1, 50000, N'Crab soup - appetizer'),
(1, 2, 1, 120000, N'Steamed fish with soy sauce - main course'),
(1, 4, 1, 65451, N'Broken rice - side dish'),
(1, 6, 2, 187835, N'Salt roasted chicken - main course (x2 per table)'),
(1, 11, 1, 171184, N'Sour soup - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(1, 1, 1, 2000000, 2000000, N'Standard sound system'),
(1, 5, 1, 3000000, 3000000, N'Fresh flower decoration'),
(1, 8, 1, 3500000, 3500000, N'MC host');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(2, 3, 1, 66175, N'Beef stew - special dish'),
(2, 5, 1, 56685, N'Chicken hotpot with lemon basil'),
(2, 7, 2, 68192, N'Fresh spring rolls (x2 per table)'),
(2, 13, 1, 51317, N'Chicken porridge - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(2, 2, 1, 2500000, 2500000, N'Lighting system'),
(2, 10, 2, 800000, 1600000, N'Electric confetti per performance'),
(2, 12, 1, 4500000, 4500000, N'Wedding photography');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(3, 8, 1, 152310, N'Roasted duck - appetizer'),
(3, 9, 1, 105045, N'Rice vermicelli'),
(3, 10, 1, 163277, N'Fried fish - special dish'),
(3, 14, 1, 67096, N'Mango salad - main course'),
(3, 15, 1, 62076, N'Bun cha - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(3, 3, 1, 5000000, 5000000, N'Singer performance'),
(3, 6, 1, 1000000, 1000000, N'Balloon decoration'),
(3, 9, 1, 1500000, 1500000, N'Wedding car transportation');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(4, 1, 1, 50000, N'Crab soup'),
(4, 6, 2, 187835, N'Salt roasted chicken (x2 per table)'),
(4, 11, 1, 171184, N'Sour soup'),
(4, 16, 1, 71844, N'Grilled squid');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(4, 4, 1, 7000000, 7000000, N'Live band'),
(4, 7, 1, 4000000, 4000000, N'Opening dance group'),
(4, 11, 1, 1200000, 1200000, N'Backdrop design');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(5, 2, 1, 120000, N'Steamed fish with soy sauce'),
(5, 6, 1, 187835, N'Salt roasted chicken'),
(5, 12, 1, 87469, N'Fried shrimp - dessert'),
(5, 13, 1, 51317, N'Chicken porridge - dessert'),
(5, 17, 1, 78276, N'Boiled chicken - side dish');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(5, 5, 1, 3000000, 3000000, N'Fresh flower decoration'),
(5, 8, 1, 3500000, 3500000, N'Professional MC'),
(5, 14, 1, 3000000, 3000000, N'Wedding album');

-- 5 bookings on June 24

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(6, 1, 1, 50000, N'Crab soup - appetizer'),
(6, 2, 1, 120000, N'Steamed fish with soy sauce - main course'),
(6, 4, 1, 65451, N'Broken rice - side dish'),
(6, 6, 2, 187835, N'Salt roasted chicken - main course (x2 per table)'),
(6, 11, 1, 171184, N'Sour soup - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(6, 1, 1, 2000000, 2000000, N'Standard sound system'),
(6, 5, 1, 3000000, 3000000, N'Fresh flower decoration'),
(6, 8, 1, 3500000, 3500000, N'MC host');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(7, 3, 1, 66175, N'Beef stew - special dish'),
(7, 5, 1, 56685, N'Chicken hotpot with lemon basil'),
(7, 7, 2, 68192, N'Fresh spring rolls (x2 per table)'),
(7, 13, 1, 51317, N'Chicken porridge - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(7, 2, 1, 2500000, 2500000, N'Lighting system'),
(7, 10, 2, 800000, 1600000, N'Electric confetti per performance'),
(7, 12, 1, 4500000, 4500000, N'Wedding photography');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(8, 8, 1, 152310, N'Roasted duck - appetizer'),
(8, 9, 1, 105045, N'Rice vermicelli'),
(8, 10, 1, 163277, N'Fried fish - special dish'),
(8, 14, 1, 67096, N'Mango salad - main course'),
(8, 15, 1, 62076, N'Bun cha - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(8, 3, 1, 5000000, 5000000, N'Singer performance'),
(8, 6, 1, 1000000, 1000000, N'Balloon decoration'),
(8, 9, 1, 1500000, 1500000, N'Wedding car transportation');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(9, 1, 1, 50000, N'Crab soup'),
(9, 6, 2, 187835, N'Salt roasted chicken (x2 per table)'),
(9, 11, 1, 171184, N'Sour soup'),
(9, 16, 1, 71844, N'Grilled squid');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(9, 4, 1, 7000000, 7000000, N'Live band'),
(9, 7, 1, 4000000, 4000000, N'Opening dance group'),
(9, 11, 1, 1200000, 1200000, N'Backdrop design');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(10, 2, 1, 120000, N'Steamed fish with soy sauce'),
(10, 6, 1, 187835, N'Salt roasted chicken'),
(10, 12, 1, 87469, N'Fried shrimp - dessert'),
(10, 13, 1, 51317, N'Chicken porridge - dessert'),
(10, 17, 1, 78276, N'Boiled chicken - side dish');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(10, 5, 1, 3000000, 3000000, N'Fresh flower decoration'),
(10, 8, 1, 3500000, 3500000, N'Professional MC'),
(10, 14, 1, 3000000, 3000000, N'Wedding album');

-- 5 bookings paid before June 24

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(11, 1, 1, 50000, N'Crab soup - appetizer'),
(11, 2, 1, 120000, N'Steamed fish with soy sauce - main course'),
(11, 4, 1, 65451, N'Broken rice - side dish'),
(11, 6, 2, 187835, N'Salt roasted chicken - main course (x2 per table)'),
(11, 11, 1, 171184, N'Sour soup - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(11, 1, 1, 2000000, 2000000, N'Standard sound system'),
(11, 5, 1, 3000000, 3000000, N'Fresh flower decoration'),
(11, 8, 1, 3500000, 3500000, N'MC host');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(12, 3, 1, 66175, N'Beef stew - special dish'),
(12, 5, 1, 56685, N'Chicken hotpot with lemon basil'),
(12, 7, 2, 68192, N'Fresh spring rolls (x2 per table)'),
(12, 13, 1, 51317, N'Chicken porridge - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(12, 2, 1, 2500000, 2500000, N'Lighting system'),
(12, 10, 2, 800000, 1600000, N'Electric confetti per performance'),
(12, 12, 1, 4500000, 4500000, N'Wedding photography');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(13, 8, 1, 152310, N'Roasted duck - appetizer'),
(13, 9, 1, 105045, N'Rice vermicelli'),
(13, 10, 1, 163277, N'Fried fish - special dish'),
(13, 14, 1, 67096, N'Mango salad - main course'),
(13, 15, 1, 62076, N'Bun cha - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(13, 3, 1, 5000000, 5000000, N'Singer performance'),
(13, 6, 1, 1000000, 1000000, N'Balloon decoration'),
(13, 9, 1, 1500000, 1500000, N'Wedding car transportation');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(14, 1, 1, 50000, N'Crab soup'),
(14, 6, 2, 187835, N'Salt roasted chicken (x2 per table)'),
(14, 11, 1, 171184, N'Sour soup'),
(14, 16, 1, 71844, N'Grilled squid');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(14, 4, 1, 7000000, 7000000, N'Live band'),
(14, 7, 1, 4000000, 4000000, N'Opening dance group'),
(14, 11, 1, 1200000, 1200000, N'Backdrop design');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(15, 2, 1, 120000, N'Steamed fish with soy sauce'),
(15, 6, 1, 187835, N'Salt roasted chicken'),
(15, 12, 1, 87469, N'Fried shrimp - dessert'),
(15, 13, 1, 51317, N'Chicken porridge - dessert'),
(15, 17, 1, 78276, N'Boiled chicken - side dish');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(15, 5, 1, 3000000, 3000000, N'Fresh flower decoration'),
(15, 8, 1, 3500000, 3500000, N'Professional MC'),
(15, 14, 1, 3000000, 3000000, N'Wedding album');

-- 5 bookings after June 24

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(16, 1, 1, 50000, N'Crab soup - appetizer'),
(16, 2, 1, 120000, N'Steamed fish with soy sauce - main course'),
(16, 4, 1, 65451, N'Broken rice - side dish'),
(16, 6, 2, 187835, N'Salt roasted chicken - main course (x2 per table)'),
(16, 11, 1, 171184, N'Sour soup - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(16, 1, 1, 2000000, 2000000, N'Standard sound system'),
(16, 5, 1, 3000000, 3000000, N'Fresh flower decoration'),
(16, 8, 1, 3500000, 3500000, N'MC host');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(17, 3, 1, 66175, N'Beef stew - special dish'),
(17, 5, 1, 56685, N'Chicken hotpot with lemon basil'),
(17, 7, 2, 68192, N'Fresh spring rolls (x2 per table)'),
(17, 13, 1, 51317, N'Chicken porridge - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(17, 2, 1, 2500000, 2500000, N'Lighting system'),
(17, 10, 2, 800000, 1600000, N'Electric confetti per performance'),
(17, 12, 1, 4500000, 4500000, N'Wedding photography');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(18, 8, 1, 152310, N'Roasted duck - appetizer'),
(18, 9, 1, 105045, N'Rice vermicelli'),
(18, 10, 1, 163277, N'Fried fish - special dish'),
(18, 14, 1, 67096, N'Mango salad - main course'),
(18, 15, 1, 62076, N'Bun cha - dessert');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(18, 3, 1, 5000000, 5000000, N'Singer performance'),
(18, 6, 1, 1000000, 1000000, N'Balloon decoration'),
(18, 9, 1, 1500000, 1500000, N'Wedding car transportation');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(19, 1, 1, 50000, N'Crab soup'),
(19, 6, 2, 187835, N'Salt roasted chicken (x2 per table)'),
(19, 11, 1, 171184, N'Sour soup'),
(19, 16, 1, 71844, N'Grilled squid');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(19, 4, 1, 7000000, 7000000, N'Live band'),
(19, 7, 1, 4000000, 4000000, N'Opening dance group'),
(19, 11, 1, 1200000, 1200000, N'Backdrop design');

INSERT INTO Menu (BookingId, DishId, Quantity, UnitPrice, Note)
VALUES
(20, 2, 1, 120000, N'Steamed fish with soy sauce'),
(20, 6, 1, 187835, N'Salt roasted chicken'),
(20, 12, 1, 87469, N'Fried shrimp - dessert'),
(20, 13, 1, 51317, N'Chicken porridge - dessert'),
(20, 17, 1, 78276, N'Boiled chicken - side dish');
INSERT INTO ServiceDetail (BookingId, ServiceId, Quantity, UnitPrice, TotalAmount, Note)
VALUES
(20, 5, 1, 3000000, 3000000, N'Fresh flower decoration'),
(20, 8, 1, 3500000, 3500000, N'Professional MC'),
(20, 14, 1, 3000000, 3000000, N'Wedding album');
