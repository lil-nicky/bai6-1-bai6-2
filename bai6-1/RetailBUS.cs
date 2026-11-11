
using System;
using System.Collections.Generic;
using System.Data;
using RetailSalesSystem.DAL;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.BUS
{
    public class RetailBUS
    {
        private RetailDAL dal = new RetailDAL();

        public DataTable Products(string keyword)
        {
            return dal.GetProducts(keyword);
        }

        public DataTable Customers()
        {
            return dal.GetCustomers();
        }

        public DataTable Employees()
        {
            return dal.GetEmployees();
        }

        public DataTable ProductByCode(string code)
        {
            return dal.GetProductByCode(code);
        }

        // SẢN PHẨM
        public void AddProduct(Product p)
        {
            ValidateProduct(p);
            dal.InsertProduct(p);
        }

        public void UpdateProduct(Product p)
        {
            ValidateProduct(p);
            dal.UpdateProduct(p);
        }

        public void DeleteProduct(string id)
        {
            dal.DeleteProduct(id);
        }

        private void ValidateProduct(Product p)
        {
            if (string.IsNullOrWhiteSpace(p.MaSP))
                throw new Exception("Mã sản phẩm không được trống!");

            if (string.IsNullOrWhiteSpace(p.TenSP))
                throw new Exception("Tên sản phẩm không được trống!");

            if (p.GiaBan <= 0)
                throw new Exception("Giá bán phải lớn hơn 0!");

            if (p.SoLuongTon < 0)
                throw new Exception("Số lượng tồn không được âm!");
        }

        // KHÁCH HÀNG
        public void AddCustomer(Customer c)
        {
            if (string.IsNullOrWhiteSpace(c.TenKH))
                throw new Exception("Tên khách hàng không được trống!");

            dal.InsertCustomer(c);
        }

        public void UpdateCustomer(Customer c)
        {
            if (c.MaKH <= 0 || string.IsNullOrWhiteSpace(c.TenKH))
                throw new Exception("Thông tin khách hàng không hợp lệ!");

            dal.UpdateCustomer(c);
        }

        public void DeleteCustomer(int id)
        {
            dal.DeleteCustomer(id);
        }

        // NHÂN VIÊN
        public void AddEmployee(Employee e)
        {
            if (string.IsNullOrWhiteSpace(e.TenNV))
                throw new Exception("Tên nhân viên không được trống!");

            dal.InsertEmployee(e);
        }

        public void UpdateEmployee(Employee e)
        {
            if (e.MaNV <= 0 || string.IsNullOrWhiteSpace(e.TenNV))
                throw new Exception("Thông tin nhân viên không hợp lệ!");

            dal.UpdateEmployee(e);
        }

        public void DeleteEmployee(int id)
        {
            dal.DeleteEmployee(id);
        }

        // THANH TOÁN
        public int Checkout(int maNV, int? maKH, List<CartItem> cart)
        {
            if (cart == null || cart.Count == 0)
                throw new Exception("Giỏ hàng đang trống!");

            decimal total = 0;

            foreach (CartItem item in cart)
            {
                if (item.SoLuong <= 0)
                    throw new Exception("Số lượng phải lớn hơn 0!");

                total += item.ThanhTien;
            }

            return dal.Checkout(maNV, maKH, cart, total);
        }

        // BÁO CÁO
        public decimal RevenueToday()
        {
            return dal.RevenueToday();
        }

        public decimal RevenueMonth()
        {
            return dal.RevenueMonth();
        }

        public DataTable TopProducts()
        {
            return dal.TopProducts();
        }
    }
}
