namespace HRMPayrollSystem.DTO
{
    public class Department
    {
        public int MaPB { get; set; }
        public string TenPB { get; set; }
        public string MoTa { get; set; }
    }

    public class Employee
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public System.DateTime? NgaySinh { get; set; }
        public string CCCD { get; set; }
        public int MaPB { get; set; }
        public decimal LuongCoBan { get; set; }
        public double HeSoLuong { get; set; }
    }

    public class Attendance
    {
        public int MaCong { get; set; }
        public int MaNV { get; set; }
        public string ThangNam { get; set; }
        public double SoNgayCong { get; set; }
        public double SoGioOT { get; set; }
    }

    public class Salary
    {
        public int MaLuong { get; set; }
        public int MaNV { get; set; }
        public string ThangNam { get; set; }
        public decimal ThucLinh { get; set; }
    }
}