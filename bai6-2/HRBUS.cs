
using System;
using System.Data;
using HRMPayrollSystem.DAL;
using HRMPayrollSystem.DTO;

namespace HRMPayrollSystem.BUS
{
    public class HRBUS
    {
        private HRDAL dal = new HRDAL();

        // PHÒNG BAN
        public DataTable Departments()
        {
            return dal.GetDepartments();
        }

        public void AddDepartment(Department d)
        {
            if (string.IsNullOrWhiteSpace(d.TenPB))
                throw new Exception("Tên phòng ban không được để trống!");

            dal.AddDepartment(d);
        }

        public void UpdateDepartment(Department d)
        {
            if (d.MaPB <= 0 ||
                string.IsNullOrWhiteSpace(d.TenPB))
                throw new Exception("Thông tin phòng ban không hợp lệ!");

            dal.UpdateDepartment(d);
        }

        public void DeleteDepartment(int id)
        {
            dal.DeleteDepartment(id);
        }

        // NHÂN VIÊN
        public DataTable Employees(string keyword, int maPB)
        {
            return dal.GetEmployees(keyword, maPB);
        }

        public void AddEmployee(Employee e)
        {
            ValidateEmployee(e);
            dal.AddEmployee(e);
        }

        public void UpdateEmployee(Employee e)
        {
            if (e.MaNV <= 0)
                throw new Exception("Chưa chọn nhân viên!");

            ValidateEmployee(e);
            dal.UpdateEmployee(e);
        }

        public void DeleteEmployee(int id)
        {
            dal.DeleteEmployee(id);
        }

        private void ValidateEmployee(Employee e)
        {
            if (string.IsNullOrWhiteSpace(e.HoTen))
                throw new Exception("Họ tên không được để trống!");

            if (e.MaPB <= 0)
                throw new Exception("Chưa chọn phòng ban!");

            if (e.LuongCoBan <= 0)
                throw new Exception("Lương cơ bản phải lớn hơn 0!");

            if (e.HeSoLuong <= 0)
                throw new Exception("Hệ số lương phải lớn hơn 0!");
        }

        // CHẤM CÔNG
        public DataTable LoadAttendance(string month)
        {
            return dal.LoadAttendance(month);
        }

        public int SaveAttendance(DataTable dt, string month)
        {
            if (dt == null || dt.Rows.Count == 0)
                throw new Exception("Không có dữ liệu chấm công!");

            return dal.SaveAttendance(dt, month);
        }

        // CHỐT LƯƠNG
        public int CloseSalary(string month)
        {
            return dal.CloseSalary(month);
        }

        // XEM BẢNG LƯƠNG
        public DataTable Salary(string month)
        {
            return dal.GetSalary(month);
        }
    }
}
