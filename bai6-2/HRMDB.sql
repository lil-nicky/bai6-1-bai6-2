/* ==========================================
   BAI 6.2 - HRM & PAYROLL SYSTEM
   DATABASE: HRMDB
   ========================================== */

IF DB_ID(N'HRMDB') IS NULL
    CREATE DATABASE HRMDB;
GO

USE HRMDB;
GO

/* 1. BANG PHONG BAN */
IF OBJECT_ID(N'dbo.PhongBan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhongBan
    (
        MaPB INT IDENTITY(1,1) PRIMARY KEY,
        TenPB NVARCHAR(100) NOT NULL,
        MoTa NVARCHAR(255) NULL
    );
END
GO

/* 2. BANG NHAN VIEN */
IF OBJECT_ID(N'dbo.NhanVien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhanVien
    (
        MaNV INT IDENTITY(1,1) PRIMARY KEY,
        HoTen NVARCHAR(100) NOT NULL,
        NgaySinh DATETIME NULL,
        CCCD VARCHAR(20) NULL,
        MaPB INT NOT NULL,
        LuongCoBan DECIMAL(18,2) NOT NULL,
        HeSoLuong FLOAT NOT NULL DEFAULT 1.0,

        CONSTRAINT UQ_NhanVien_CCCD UNIQUE (CCCD),

        CONSTRAINT CK_NhanVien_Luong
            CHECK (LuongCoBan > 0),

        CONSTRAINT CK_NhanVien_HeSo
            CHECK (HeSoLuong > 0),

        CONSTRAINT FK_NhanVien_PhongBan
            FOREIGN KEY (MaPB) REFERENCES dbo.PhongBan(MaPB)
    );
END
GO

/* 3. BANG CHAM CONG */
IF OBJECT_ID(N'dbo.BangChamCong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BangChamCong
    (
        MaCong INT IDENTITY(1,1) PRIMARY KEY,
        MaNV INT NOT NULL,
        ThangNam VARCHAR(7) NOT NULL,
        SoNgayCong FLOAT NOT NULL DEFAULT 0,
        SoGioOT FLOAT NOT NULL DEFAULT 0,

        CONSTRAINT CK_ChamCong_Ngay
            CHECK (SoNgayCong >= 0 AND SoNgayCong <= 31),

        CONSTRAINT CK_ChamCong_OT
            CHECK (SoGioOT >= 0),

        CONSTRAINT UQ_ChamCong_NhanVien_Thang
            UNIQUE (MaNV, ThangNam),

        CONSTRAINT FK_ChamCong_NhanVien
            FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV)
    );
END
GO

/* 4. BANG LUONG */
IF OBJECT_ID(N'dbo.BangLuong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BangLuong
    (
        MaLuong INT IDENTITY(1,1) PRIMARY KEY,
        MaNV INT NOT NULL,
        ThangNam VARCHAR(7) NOT NULL,
        ThucLinh DECIMAL(18,2) NOT NULL DEFAULT 0,

        CONSTRAINT UQ_BangLuong_NhanVien_Thang
            UNIQUE (MaNV, ThangNam),

        CONSTRAINT FK_BangLuong_NhanVien
            FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV)
    );
END
GO

/* ==========================================
   DU LIEU MAU
   ========================================== */

IF NOT EXISTS (SELECT 1 FROM dbo.PhongBan)
BEGIN
    INSERT INTO dbo.PhongBan (TenPB, MoTa)
    VALUES
        (N'Phòng Kế toán', N'Quản lý tài chính và kế toán'),
        (N'Phòng Nhân sự', N'Quản lý nhân sự'),
        (N'Phòng Kỹ thuật', N'Phát triển và bảo trì hệ thống');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.NhanVien)
BEGIN
    INSERT INTO dbo.NhanVien
        (HoTen, NgaySinh, CCCD, MaPB, LuongCoBan, HeSoLuong)
    VALUES
        (N'Nguyễn Văn An', '2000-01-15', '001200000001', 1, 8000000, 1.5),
        (N'Trần Thị Bình', '2001-05-20', '001200000002', 2, 7500000, 1.3),
        (N'Lê Văn Cường', '1999-08-10', '001200000003', 3, 9000000, 1.7);
END
GO

/* TAO CHAM CONG MAU CHO THANG 11/2026 */
IF NOT EXISTS
(
    SELECT 1
    FROM dbo.BangChamCong
    WHERE ThangNam = '11/2026'
)
BEGIN
    INSERT INTO dbo.BangChamCong
        (MaNV, ThangNam, SoNgayCong, SoGioOT)
    SELECT MaNV, '11/2026', 22, 5
    FROM dbo.NhanVien;
END
GO

/* ==========================================
   KIEM TRA DU LIEU
   ========================================== */

SELECT * FROM dbo.PhongBan;
SELECT * FROM dbo.NhanVien;
SELECT * FROM dbo.BangChamCong;
SELECT * FROM dbo.BangLuong;
GO

/* ==========================================
   KIEM TRA TINH LUONG THANG 11/2026
   ========================================== */

SELECT
    NV.MaNV,
    NV.HoTen,
    PB.TenPB,
    NV.LuongCoBan,
    NV.HeSoLuong,
    CC.ThangNam,
    CC.SoNgayCong,
    CC.SoGioOT,
    CAST(
        (NV.LuongCoBan * NV.HeSoLuong / 22.0)
        * CC.SoNgayCong
        + (CC.SoGioOT * 50000)
        AS DECIMAL(18,2)
    ) AS ThucLinh
FROM dbo.NhanVien NV
INNER JOIN dbo.PhongBan PB
    ON NV.MaPB = PB.MaPB
INNER JOIN dbo.BangChamCong CC
    ON NV.MaNV = CC.MaNV
WHERE CC.ThangNam = '11/2026';
GO
