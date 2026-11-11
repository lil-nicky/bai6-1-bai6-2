
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using HRMPayrollSystem.DTO;

namespace HRMPayrollSystem.DAL
{
    public class HRDAL
    {
        // =====================================
        // LẤY DỮ LIỆU
        // =====================================

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

        // =====================================
        // PHÒNG BAN
        // =====================================

        public DataTable GetDepartments()
        {
            return GetData(
                "SELECT MaPB,TenPB,MoTa FROM PhongBan ORDER BY MaPB");
        }

        public void AddDepartment(Department d)
        {
            Execute(
                "INSERT INTO PhongBan(TenPB,MoTa) VALUES(@ten,@mota)",

                new SqlParameter("@ten", SqlDbType.NVarChar, 100)
                {
                    Value = d.TenPB
                },

                new SqlParameter("@mota", SqlDbType.NVarChar, 255)
                {
                    Value = (object)d.MoTa ?? DBNull.Value
                }
            );
        }

        public void UpdateDepartment(Department d)
        {
            Execute(
                @"UPDATE PhongBan
                  SET TenPB=@ten,MoTa=@mota
                  WHERE MaPB=@id",

                new SqlParameter("@id", SqlDbType.Int) { Value = d.MaPB },

                new SqlParameter("@ten", SqlDbType.NVarChar, 100)
                {
                    Value = d.TenPB
                },

                new SqlParameter("@mota", SqlDbType.NVarChar, 255)
                {
                    Value = (object)d.MoTa ?? DBNull.Value
                }
            );
        }

        public void DeleteDepartment(int id)
        {
            Execute(
                "DELETE FROM PhongBan WHERE MaPB=@id",
                new SqlParameter("@id", SqlDbType.Int) { Value = id }
            );
        }

        // =====================================
        // NHÂN VIÊN
        // =====================================

        public DataTable GetEmployees(string keyword, int maPB)
        {
            return GetData(
                @"SELECT
                    nv.MaNV,
                    nv.HoTen,
                    nv.NgaySinh,
                    nv.CCCD,
                    nv.MaPB,
                    pb.TenPB,
                    nv.LuongCoBan,
                    nv.HeSoLuong
                  FROM NhanVien nv
                  INNER JOIN PhongBan pb ON nv.MaPB=pb.MaPB
                  WHERE
                    (@kw=N'' OR nv.HoTen LIKE @like
                     OR nv.CCCD LIKE @like
                     OR CONVERT(VARCHAR(10),nv.MaNV) LIKE @like)
                    AND (@pb=0 OR nv.MaPB=@pb)
                  ORDER BY nv.MaNV",

                new SqlParameter("@kw", SqlDbType.NVarChar, 100)
                {
                    Value = keyword ?? ""
                },

                new SqlParameter("@like", SqlDbType.NVarChar, 110)
                {
                    Value = "%" + (keyword ?? "") + "%"
                },

                new SqlParameter("@pb", SqlDbType.Int)
                {
                    Value = maPB
                }
            );
        }

        public void AddEmployee(Employee e)
        {
            Execute(
                @"INSERT INTO NhanVien
                  (HoTen,NgaySinh,CCCD,MaPB,LuongCoBan,HeSoLuong)
                  VALUES
                  (@ten,@ns,@cccd,@pb,@luong,@heso)",

                new SqlParameter("@ten", SqlDbType.NVarChar, 100)
                {
                    Value = e.HoTen
                },

                new SqlParameter("@ns", SqlDbType.DateTime)
                {
                    Value = e.NgaySinh.HasValue
                        ? (object)e.NgaySinh.Value
                        : DBNull.Value
                },

                new SqlParameter("@cccd", SqlDbType.VarChar, 20)
                {
                    Value = (object)e.CCCD ?? DBNull.Value
                },

                new SqlParameter("@pb", SqlDbType.Int)
                {
                    Value = e.MaPB
                },

                new SqlParameter("@luong", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 2,
                    Value = e.LuongCoBan
                },

                new SqlParameter("@heso", SqlDbType.Float)
                {
                    Value = e.HeSoLuong
                }
            );
        }

        public void UpdateEmployee(Employee e)
        {
            Execute(
                @"UPDATE NhanVien SET
                    HoTen=@ten,
                    NgaySinh=@ns,
                    CCCD=@cccd,
                    MaPB=@pb,
                    LuongCoBan=@luong,
                    HeSoLuong=@heso
                  WHERE MaNV=@id",

                new SqlParameter("@id", SqlDbType.Int)
                {
                    Value = e.MaNV
                },

                new SqlParameter("@ten", SqlDbType.NVarChar, 100)
                {
                    Value = e.HoTen
                },

                new SqlParameter("@ns", SqlDbType.DateTime)
                {
                    Value = e.NgaySinh.HasValue
                        ? (object)e.NgaySinh.Value
                        : DBNull.Value
                },

                new SqlParameter("@cccd", SqlDbType.VarChar, 20)
                {
                    Value = (object)e.CCCD ?? DBNull.Value
                },

                new SqlParameter("@pb", SqlDbType.Int)
                {
                    Value = e.MaPB
                },

                new SqlParameter("@luong", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 2,
                    Value = e.LuongCoBan
                },

                new SqlParameter("@heso", SqlDbType.Float)
                {
                    Value = e.HeSoLuong
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

        // =====================================
        // CHẤM CÔNG DISCONNECTED MODEL
        // =====================================

        public DataTable LoadAttendance(string month)
        {
            using (SqlConnection cn = Database.GetConnection())
            {
                cn.Open();

                // DataAdapter lấy dữ liệu từ bảng chấm công.
                SqlDataAdapter adapter = new SqlDataAdapter(
                    @"SELECT MaCong,MaNV,ThangNam,SoNgayCong,SoGioOT
                      FROM BangChamCong
                      WHERE ThangNam=@thang",
                    cn);

                adapter.SelectCommand.Parameters.Add(
                    "@thang", SqlDbType.VarChar, 7).Value = month;

                // DataSet là mô hình ngắt kết nối.
                DataSet ds = new DataSet();

                adapter.Fill(ds, "BangChamCong");

                DataTable dt = ds.Tables["BangChamCong"];

                // Nạp toàn bộ nhân viên.
                DataTable employees = new DataTable();

                using (SqlDataAdapter empAdapter = new SqlDataAdapter(
                    @"SELECT MaNV,HoTen FROM NhanVien ORDER BY MaNV", cn))
                {
                    empAdapter.Fill(employees);
                }

                // Thêm tên nhân viên vào bảng chấm công trên bộ nhớ.
                dt.Columns.Add("HoTen", typeof(string));

                foreach (DataRow emp in employees.Rows)
                {
                    int maNV = Convert.ToInt32(emp["MaNV"]);

                    DataRow existing = null;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (Convert.ToInt32(row["MaNV"]) == maNV)
                        {
                            existing = row;
                            break;
                        }
                    }

                    if (existing != null)
                    {
                        existing["HoTen"] = emp["HoTen"];
                    }
                    else
                    {
                        DataRow newRow = dt.NewRow();

                        newRow["MaNV"] = maNV;
                        newRow["ThangNam"] = month;
                        newRow["SoNgayCong"] = 0;
                        newRow["SoGioOT"] = 0;
                        newRow["HoTen"] = emp["HoTen"];

                        dt.Rows.Add(newRow);
                    }
                }

                return dt;
            }
        }

        // Lưu dữ liệu bằng SqlCommandBuilder và adapter.Update().
        public int SaveAttendance(DataTable dt, string month)
        {
            using (SqlConnection cn = Database.GetConnection())
            {
                cn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(
                    @"SELECT MaCong,MaNV,ThangNam,SoNgayCong,SoGioOT
                      FROM BangChamCong
                      WHERE ThangNam=@thang",
                    cn);

                adapter.SelectCommand.Parameters.Add(
                    "@thang", SqlDbType.VarChar, 7).Value = month;

                using (SqlCommandBuilder builder =
                    new SqlCommandBuilder(adapter))
                {
                    // Kiểm tra chốt lương trước khi cập nhật.
                    using (SqlCommand check = new SqlCommand(
                        "SELECT COUNT(*) FROM BangLuong WHERE ThangNam=@thang",
                        cn))
                    {
                        check.Parameters.Add("@thang", SqlDbType.VarChar, 7)
                            .Value = month;

                        if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                            throw new Exception(
                                "Tháng này đã chốt lương, không thể sửa chấm công!");
                    }

                    // Xác nhận dữ liệu hợp lệ.
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted)
                            continue;

                        double days = Convert.ToDouble(row["SoNgayCong"]);
                        double ot = Convert.ToDouble(row["SoGioOT"]);

                        if (days < 0 || days > 31)
                            throw new Exception("Ngày công phải từ 0 đến 31.");

                        if (ot < 0)
                            throw new Exception("Giờ OT không được âm.");
                    }

                    // Bỏ cột HoTen vì không thuộc bảng BangChamCong.
                    dt.Columns.Remove("HoTen");

                    try
                    {
                        // Đồng bộ DataTable xuống SQL Server.
                        int result = adapter.Update(dt);

                        dt.AcceptChanges();

                        return result;
                    }
                    finally
                    {
                        // Khôi phục cột tên để hiển thị trên giao diện.
                        dt.Columns.Add("HoTen", typeof(string));

                        DataTable emp = GetData(
                            "SELECT MaNV,HoTen FROM NhanVien");

                        foreach (DataRow row in dt.Rows)
                        {
                            foreach (DataRow er in emp.Rows)
                            {
                                if (Convert.ToInt32(row["MaNV"]) ==
                                    Convert.ToInt32(er["MaNV"]))
                                {
                                    row["HoTen"] = er["HoTen"];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        // =====================================
        // TÍNH LƯƠNG
        // =====================================

        public int CloseSalary(string month)
        {
            using (SqlConnection cn = Database.GetConnection())
            {
                cn.Open();

                using (SqlTransaction trans =
                    cn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        // Không cho phép chốt lương trùng tháng.
                        using (SqlCommand check = new SqlCommand(
                            @"SELECT COUNT(*)
                              FROM BangLuong
                              WHERE ThangNam=@thang",
                            cn, trans))
                        {
                            check.Parameters.Add("@thang", SqlDbType.VarChar, 7)
                                .Value = month;

                            if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                                throw new Exception(
                                    "Tháng này đã được chốt lương!");
                        }

                        // Kiểm tra đã có chấm công cho nhân viên chưa.
                        using (SqlCommand check = new SqlCommand(
                            @"SELECT COUNT(*)
                              FROM NhanVien nv
                              LEFT JOIN BangChamCong cc
                                ON nv.MaNV=cc.MaNV
                                AND cc.ThangNam=@thang
                              WHERE cc.MaCong IS NULL",
                            cn, trans))
                        {
                            check.Parameters.Add("@thang", SqlDbType.VarChar, 7)
                                .Value = month;

                            if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                                throw new Exception(
                                    "Còn nhân viên chưa có dữ liệu chấm công tháng này!");
                        }

                        // Tính và ghi bảng lương cho toàn bộ nhân viên.
                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO BangLuong(MaNV,ThangNam,ThucLinh)
                              SELECT
                                nv.MaNV,
                                @thang,
                                CAST(
                                    (nv.LuongCoBan * nv.HeSoLuong / 22.0)
                                    * cc.SoNgayCong
                                    + (cc.SoGioOT * 50000)
                                AS DECIMAL(18,2))
                              FROM NhanVien nv
                              INNER JOIN BangChamCong cc
                                ON nv.MaNV=cc.MaNV
                              WHERE cc.ThangNam=@thang",
                            cn, trans))
                        {
                            cmd.Parameters.Add("@thang", SqlDbType.VarChar, 7)
                                .Value = month;

                            int count = cmd.ExecuteNonQuery();

                            if (count == 0)
                                throw new Exception(
                                    "Không có dữ liệu để tính lương!");

                            trans.Commit();

                            return count;
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        // =====================================
        // XEM BẢNG LƯƠNG
        // =====================================

        public DataTable GetSalary(string month)
        {
            return GetData(
                @"SELECT
                    bl.MaLuong,
                    nv.MaNV,
                    nv.HoTen,
                    pb.TenPB,
                    bl.ThangNam,
                    bl.ThucLinh
                  FROM BangLuong bl
                  INNER JOIN NhanVien nv ON bl.MaNV=nv.MaNV
                  INNER JOIN PhongBan pb ON nv.MaPB=pb.MaPB
                  WHERE bl.ThangNam=@thang
                  ORDER BY nv.MaNV",

                new SqlParameter("@thang", SqlDbType.VarChar, 7)
                {
                    Value = month
                }
            );
        }
    }
}
