using System;
using System.Configuration;
using System.Data.SqlClient;

namespace HRMPayrollSystem.DAL
{
    public static class Database
    {
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["HRMDB"]?.ConnectionString;

        // Phương thức tạo kết nối
        public static SqlConnection CreateConnection()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception(
                    "Không tìm thấy chuỗi kết nối HRMDB trong App.config."
                );
            }

            return new SqlConnection(connectionString);
        }

        // Phương thức tương thích với HRDAL.cs
        public static SqlConnection GetConnection()
        {
            return CreateConnection();
        }
    }
}