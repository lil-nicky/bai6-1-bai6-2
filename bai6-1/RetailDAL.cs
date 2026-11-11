
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.DAL
{
    public class RetailDAL
    {
        // =========================
        // LẤY DỮ LIỆU
        // =========================

        public DataTable GetData(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection cn = Database.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable GetProducts(string keyword)
        {
            return GetData(
                @"SELECT MaSP, TenSP, GiaBan, SoLuongTon
                  FROM SanPham
                  WHERE MaSP LIKE @kw OR TenSP LIKE @kw
                  ORDER BY TenSP",
                new SqlParameter("@kw", SqlDbType.NVarChar, 200)
                {
                    Value = "%" + (keyword ?? "") + "%"
                });
        }

        public DataTable GetCustomers()
        {
            return GetData(
                "SELECT MaKH, TenKH, DienThoai, DiemTichLuy FROM KhachHang");
        }

        public DataTable GetEmployees()
        {
            return GetData(
                "SELECT MaNV, TenNV, SoDienThoai, ChucVu FROM NhanVien");
        }

        public DataTable GetProductByCode(string maSP)
        {
            return GetData(
                "SELECT * FROM SanPham WHERE MaSP=@id",
                new SqlParameter("@id", SqlDbType.VarChar, 20)
                {
                    Value = maSP
                });
        }

        // =========================
        // THỰC THI SQL
        // =========================

        private void Execute(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection cn = Database.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // =========================
        // CRUD SẢN PHẨM
        // =========================

        public void InsertProduct(Product p)
        {
            Execute(
                @"INSERT INTO SanPham
                  (MaSP,TenSP,GiaBan,SoLuongTon)
                  VALUES(@id,@name,@price,@stock)",

                new SqlParameter("@id", SqlDbType.VarChar, 20) { Value = p.MaSP },
                new SqlParameter("@name", SqlDbType.NVarChar, 150) { Value = p.TenSP },
                new SqlParameter("@price", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 2,
                    Value = p.GiaBan
                },
                new SqlParameter("@stock", SqlDbType.Int) { Value = p.SoLuongTon }
            );
        }

        public void UpdateProduct(Product p)
        {
            Execute(
                @"UPDATE SanPham
                  SET TenSP=@name,
                      GiaBan=@price,
                      SoLuongTon=@stock
                  WHERE MaSP=@id",

                new SqlParameter("@id", SqlDbType.VarChar, 20) { Value = p.MaSP },
                new SqlParameter("@name", SqlDbType.NVarChar, 150) { Value = p.TenSP },
                new SqlParameter("@price", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 2,
                    Value = p.GiaBan
                },
                new SqlParameter("@stock", SqlDbType.Int) { Value = p.SoLuongTon }
            );
        }

        public void DeleteProduct(string id)
        {
            Execute(
                "DELETE FROM SanPham WHERE MaSP=@id",
                new SqlParameter("@id", SqlDbType.VarChar, 20) { Value = id }
            );
        }

        // =========================
        // CRUD KHÁCH HÀNG
        // =========================

        public void InsertCustomer(Customer c)
        {
            Execute(
                @"INSERT INTO KhachHang(TenKH,DienThoai)
                  VALUES(@name,@phone)",

                new SqlParameter("@name", SqlDbType.NVarChar, 100)
                {
                    Value = c.TenKH
                },
                new SqlParameter("@phone", SqlDbType.VarChar, 15)
                {
                    Value = string.IsNullOrWhiteSpace(c.DienThoai)
                        ? (object)DBNull.Value : c.DienThoai
                }
            );
        }

        public void UpdateCustomer(Customer c)
        {
            Execute(
                @"UPDATE KhachHang
                  SET TenKH=@name,DienThoai=@phone
                  WHERE MaKH=@id",

                new SqlParameter("@id", SqlDbType.Int) { Value = c.MaKH },
                new SqlParameter("@name", SqlDbType.NVarChar, 100) { Value = c.TenKH },
                new SqlParameter("@phone", SqlDbType.VarChar, 15)
                {
                    Value = string.IsNullOrWhiteSpace(c.DienThoai)
                        ? (object)DBNull.Value : c.DienThoai
                }
            );
        }

        public void DeleteCustomer(int id)
        {
            Execute(
                "DELETE FROM KhachHang WHERE MaKH=@id",
                new SqlParameter("@id", SqlDbType.Int) { Value = id }
            );
        }

        // =========================
        // CRUD NHÂN VIÊN
        // =========================

        public void InsertEmployee(Employee e)
        {
            Execute(
                @"INSERT INTO NhanVien(TenNV,SoDienThoai,ChucVu)
                  VALUES(@name,@phone,@role)",

                new SqlParameter("@name", SqlDbType.NVarChar, 100)
                {
                    Value = e.TenNV
                },
                new SqlParameter("@phone", SqlDbType.VarChar, 15)
                {
                    Value = string.IsNullOrWhiteSpace(e.SoDienThoai)
                        ? (object)DBNull.Value : e.SoDienThoai
                },
                new SqlParameter("@role", SqlDbType.NVarChar, 50)
                {
                    Value = (object)e.ChucVu ?? DBNull.Value
                }
            );
        }

        public void UpdateEmployee(Employee e)
        {
            Execute(
                @"UPDATE NhanVien
                  SET TenNV=@name,
                      SoDienThoai=@phone,
                      ChucVu=@role
                  WHERE MaNV=@id",

                new SqlParameter("@id", SqlDbType.Int) { Value = e.MaNV },
                new SqlParameter("@name", SqlDbType.NVarChar, 100) { Value = e.TenNV },
                new SqlParameter("@phone", SqlDbType.VarChar, 15)
                {
                    Value = string.IsNullOrWhiteSpace(e.SoDienThoai)
                        ? (object)DBNull.Value : e.SoDienThoai
                },
                new SqlParameter("@role", SqlDbType.NVarChar, 50)
                {
                    Value = (object)e.ChucVu ?? DBNull.Value
                }
            );
        }

        public void DeleteEmployee(int id)
        {
            Execute(
                "DELETE FROM NhanVien WHERE MaNV=@id",
                new SqlParameter("@id", SqlDbType.Int) { Value = id }
            );
        }

        // =========================
        // THANH TOÁN SQLTRANSACTION
        // =========================

        public int Checkout(
            int maNV,
            int? maKH,
            List<CartItem> cart,
            decimal tongTien)
        {
            using (SqlConnection cn = Database.GetConnection())
            {
                cn.Open();

                using (SqlTransaction trans =
                    cn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        int maHD;

                        // 1. TẠO HÓA ĐƠN
                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO HoaDon
                              (NgayLap,MaNV,MaKH,TongTien)
                              VALUES(GETDATE(),@nv,@kh,@total);
                              SELECT CAST(SCOPE_IDENTITY() AS INT);",
                            cn, trans))
                        {
                            cmd.Parameters.Add("@nv", SqlDbType.Int).Value = maNV;

                            cmd.Parameters.Add("@kh", SqlDbType.Int).Value =
                                maKH.HasValue
                                ? (object)maKH.Value
                                : DBNull.Value;

                            SqlParameter total =
                                cmd.Parameters.Add("@total", SqlDbType.Decimal);

                            total.Precision = 18;
                            total.Scale = 2;
                            total.Value = tongTien;

                            maHD = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 2. THÊM CHI TIẾT VÀ TRỪ TỒN KHO
                        foreach (CartItem item in cart)
                        {
                            // Trừ tồn kho có kiểm tra số lượng.
                            using (SqlCommand cmd = new SqlCommand(
                                @"UPDATE SanPham
                                  SET SoLuongTon=SoLuongTon-@qty
                                  WHERE MaSP=@sp
                                  AND SoLuongTon>=@qty",
                                cn, trans))
                            {
                                cmd.Parameters.Add("@qty", SqlDbType.Int)
                                    .Value = item.SoLuong;

                                cmd.Parameters.Add("@sp", SqlDbType.VarChar, 20)
                                    .Value = item.MaSP;

                                int affected = cmd.ExecuteNonQuery();

                                if (affected != 1)
                                {
                                    throw new Exception(
                                        "Sản phẩm " + item.MaSP +
                                        " không đủ hàng trong kho!");
                                }
                            }

                            // Thêm chi tiết hóa đơn.
                            using (SqlCommand cmd = new SqlCommand(
                                @"INSERT INTO ChiTietHoaDon
                                  (MaHD,MaSP,SoLuong,DonGia)
                                  VALUES(@hd,@sp,@qty,@price)",
                                cn, trans))
                            {
                                cmd.Parameters.Add("@hd", SqlDbType.Int)
                                    .Value = maHD;

                                cmd.Parameters.Add("@sp", SqlDbType.VarChar, 20)
                                    .Value = item.MaSP;

                                cmd.Parameters.Add("@qty", SqlDbType.Int)
                                    .Value = item.SoLuong;

                                SqlParameter price =
                                    cmd.Parameters.Add("@price", SqlDbType.Decimal);

                                price.Precision = 18;
                                price.Scale = 2;
                                price.Value = item.DonGia;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3. CỘNG ĐIỂM TÍCH LŨY
                        if (maKH.HasValue)
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                @"UPDATE KhachHang
                                  SET DiemTichLuy =
                                  DiemTichLuy + CONVERT(INT,FLOOR(@total/100000.0))
                                  WHERE MaKH=@kh",
                                cn, trans))
                            {
                                SqlParameter total =
                                    cmd.Parameters.Add("@total", SqlDbType.Decimal);

                                total.Precision = 18;
                                total.Scale = 2;
                                total.Value = tongTien;

                                cmd.Parameters.Add("@kh", SqlDbType.Int)
                                    .Value = maKH.Value;

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception("Không tìm thấy khách hàng!");
                            }
                        }

                        // 4. XÁC NHẬN GIAO DỊCH
                        trans.Commit();

                        return maHD;
                    }
                    catch
                    {
                        // BẤT KỲ LỖI NÀO: HỦY TOÀN BỘ GIAO DỊCH
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        // =========================
        // BÁO CÁO DOANH THU
        // =========================

        public decimal RevenueToday()
        {
            using (SqlConnection cn = Database.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT ISNULL(SUM(TongTien),0)
                  FROM HoaDon
                  WHERE NgayLap >= CONVERT(DATE,GETDATE())
                  AND NgayLap < DATEADD(DAY,1,CONVERT(DATE,GETDATE()))",
                cn))
            {
                cn.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public decimal RevenueMonth()
        {
            using (SqlConnection cn = Database.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT ISNULL(SUM(TongTien),0)
                  FROM HoaDon
                  WHERE YEAR(NgayLap)=YEAR(GETDATE())
                  AND MONTH(NgayLap)=MONTH(GETDATE())",
                cn))
            {
                cn.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        // =========================
        // TOP 5 SẢN PHẨM BÁN CHẠY
        // =========================

        public DataTable TopProducts()
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = Database.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 5
                    sp.MaSP,
                    sp.TenSP,
                    SUM(ct.SoLuong) AS TongSoLuong
                  FROM ChiTietHoaDon ct
                  INNER JOIN SanPham sp
                    ON ct.MaSP=sp.MaSP
                  GROUP BY sp.MaSP,sp.TenSP
                  ORDER BY SUM(ct.SoLuong) DESC",
                cn))
            {
                cn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }
    }
}
