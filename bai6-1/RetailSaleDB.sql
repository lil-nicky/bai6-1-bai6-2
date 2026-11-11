/* ==========================================
   BAI 6.1 - RETAIL SALES SYSTEM
   DATABASE: RetailSalesDB
   ========================================== */

IF DB_ID(N'RetailSalesDB') IS NULL
    CREATE DATABASE RetailSalesDB;
GO

USE RetailSalesDB;
GO

/* 1. BANG NHAN VIEN */
IF OBJECT_ID(N'dbo.NhanVien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhanVien
    (
        MaNV INT IDENTITY(1,1) PRIMARY KEY,
        HoTen NVARCHAR(100) NOT NULL,
        NgaySinh DATE NULL,
        SDT VARCHAR(15) NULL,
        DiaChi NVARCHAR(255) NULL,
        NgayVaoLam DATE NOT NULL DEFAULT GETDATE(),
        TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Đang làm'
    );
END
GO

/* 2. BANG KHACH HANG */
IF OBJECT_ID(N'dbo.KhachHang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.KhachHang
    (
        MaKH INT IDENTITY(1,1) PRIMARY KEY,
        HoTen NVARCHAR(100) NOT NULL,
        SDT VARCHAR(15) NULL,
        DiaChi NVARCHAR(255) NULL,
        Email VARCHAR(100) NULL
    );
END
GO

/* 3. BANG SAN PHAM */
IF OBJECT_ID(N'dbo.SanPham', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SanPham
    (
        MaSP INT IDENTITY(1,1) PRIMARY KEY,
        TenSP NVARCHAR(150) NOT NULL,
        DonViTinh NVARCHAR(30) NOT NULL DEFAULT N'Cái',
        DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
        SoLuongTon INT NOT NULL DEFAULT 0 CHECK (SoLuongTon >= 0),
        TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Đang bán'
    );
END
GO

/* 4. BANG HOA DON */
IF OBJECT_ID(N'dbo.HoaDon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HoaDon
    (
        MaHD INT IDENTITY(1,1) PRIMARY KEY,
        MaNV INT NOT NULL,
        MaKH INT NULL,
        NgayLap DATETIME NOT NULL DEFAULT GETDATE(),
        TongTien DECIMAL(18,2) NOT NULL DEFAULT 0,
        GhiChu NVARCHAR(255) NULL,

        CONSTRAINT FK_HoaDon_NhanVien
            FOREIGN KEY (MaNV) REFERENCES dbo.NhanVien(MaNV),

        CONSTRAINT FK_HoaDon_KhachHang
            FOREIGN KEY (MaKH) REFERENCES dbo.KhachHang(MaKH)
    );
END
GO

/* 5. BANG CHI TIET HOA DON */
IF OBJECT_ID(N'dbo.ChiTietHoaDon', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChiTietHoaDon
    (
        MaHD INT NOT NULL,
        MaSP INT NOT NULL,
        SoLuong INT NOT NULL CHECK (SoLuong > 0),
        DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),
        ThanhTien AS (SoLuong * DonGia) PERSISTED,

        CONSTRAINT PK_ChiTietHoaDon
            PRIMARY KEY (MaHD, MaSP),

        CONSTRAINT FK_CTHD_HoaDon
            FOREIGN KEY (MaHD) REFERENCES dbo.HoaDon(MaHD),

        CONSTRAINT FK_CTHD_SanPham
            FOREIGN KEY (MaSP) REFERENCES dbo.SanPham(MaSP)
    );
END
GO

/* ==========================================
   DU LIEU MAU
   Chi them neu bang dang rong
   ========================================== */

IF NOT EXISTS (SELECT 1 FROM dbo.NhanVien)
BEGIN
    INSERT INTO dbo.NhanVien
        (HoTen, NgaySinh, SDT, DiaChi)
    VALUES
        (N'Nguyễn Văn An', '2000-01-15', '0901000001', N'Hà Nội'),
        (N'Trần Thị Bình', '2001-05-20', '0901000002', N'Hà Nội');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.KhachHang)
BEGIN
    INSERT INTO dbo.KhachHang (HoTen, SDT, DiaChi, Email)
    VALUES
        (N'Lê Văn Nam', '0912000001', N'Hà Nội', 'nam@gmail.com'),
        (N'Phạm Thị Hoa', '0912000002', N'Hải Phòng', 'hoa@gmail.com');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SanPham)
BEGIN
    INSERT INTO dbo.SanPham
        (TenSP, DonViTinh, DonGia, SoLuongTon)
    VALUES
        (N'Nước suối', N'Chai', 5000, 100),
        (N'Mì tôm', N'Gói', 4000, 200),
        (N'Sữa tươi', N'Hộp', 8000, 150),
        (N'Bánh quy', N'Gói', 15000, 80),
        (N'Cà phê', N'Gói', 12000, 60);
END
GO

/* KIEM TRA */
SELECT * FROM dbo.NhanVien;
SELECT * FROM dbo.KhachHang;
SELECT * FROM dbo.SanPham;
SELECT * FROM dbo.HoaDon;
SELECT * FROM dbo.ChiTietHoaDon;
GO
