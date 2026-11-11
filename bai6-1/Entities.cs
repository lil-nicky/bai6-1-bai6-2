
namespace RetailSalesSystem.DTO
{
    public class Product
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
    }

    public class Customer
    {
        public int MaKH { get; set; }
        public string TenKH { get; set; }
        public string DienThoai { get; set; }
        public int DiemTichLuy { get; set; }
    }

    public class Employee
    {
        public int MaNV { get; set; }
        public string TenNV { get; set; }
        public string SoDienThoai { get; set; }
        public string ChucVu { get; set; }
    }

    public class CartItem
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public decimal ThanhTien
        {
            get { return SoLuong * DonGia; }
        }
    }
}
