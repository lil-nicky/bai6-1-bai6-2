
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RetailSalesSystem.BUS;
using RetailSalesSystem.DTO;

namespace RetailSalesSystem.GUI
{
    public class MainForm : Form
    {
        private RetailBUS bus = new RetailBUS();

        private List<CartItem> cart = new List<CartItem>();

        private DataGridView dgvCart;
        private DataGridView dgvProduct;
        private DataGridView dgvCustomer;
        private DataGridView dgvEmployee;
        private DataGridView dgvTop;

        private TextBox txtCode;
        private TextBox txtQty;

        private TextBox txtSearch;
        private TextBox txtMaSP;
        private TextBox txtTenSP;
        private TextBox txtGia;
        private TextBox txtTon;

        private TextBox txtTenKH;
        private TextBox txtSDTKH;

        private TextBox txtTenNV;
        private TextBox txtSDTNV;
        private TextBox txtChucVu;

        private ComboBox cboNV;
        private ComboBox cboKH;

        private Label lblTotal;
        private Label lblToday;
        private Label lblMonth;

        private int currentKH = 0;
        private int currentNV = 0;

        public MainForm()
        {
            Text = "RETAIL SALES SYSTEM - QUẢN LÝ SIÊU THỊ MINI";
            Width = 1250;
            Height = 750;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 600);

            BuildInterface();
            LoadAll();
        }

        // ==================================
        // GIAO DIỆN CHUNG
        // ==================================

        private void BuildInterface()
        {
            TabControl tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;

            tabs.TabPages.Add(CreateSalesTab());
            tabs.TabPages.Add(CreateProductTab());
            tabs.TabPages.Add(CreateCustomerTab());
            tabs.TabPages.Add(CreateEmployeeTab());
            tabs.TabPages.Add(CreateReportTab());

            Controls.Add(tabs);
        }

        private TabPage CreateTab(string name)
        {
            return new TabPage(name)
            {
                BackColor = Color.WhiteSmoke
            };
        }

        private DataGridView CreateGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
        }

        private TextBox CreateTextBox(int width = 130)
        {
            return new TextBox
            {
                Width = width,
                Margin = new Padding(5)
            };
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Padding = new Padding(5, 8, 2, 2)
            };
        }

        private Button CreateButton(string text, Action action)
        {
            Button btn = new Button
            {
                Text = text,
                AutoSize = true,
                Height = 32,
                Margin = new Padding(5)
            };

            btn.Click += (s, e) =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Thông báo lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };

            return btn;
        }

        private FlowLayoutPanel CreatePanel()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = true,
                Padding = new Padding(5)
            };
        }

        // ==================================
        // TAB 1: BÁN HÀNG
        // ==================================

        private TabPage CreateSalesTab()
        {
            TabPage tab = CreateTab("Bán hàng");

            FlowLayoutPanel top = CreatePanel();

            txtCode = CreateTextBox(130);
            txtQty = CreateTextBox(60);
            txtQty.Text = "1";

            cboNV = new ComboBox();
            cboNV.Width = 170;
            cboNV.DropDownStyle = ComboBoxStyle.DropDownList;

            cboKH = new ComboBox();
            cboKH.Width = 200;
            cboKH.DropDownStyle = ComboBoxStyle.DropDownList;

            top.Controls.Add(CreateLabel("Mã sản phẩm:"));
            top.Controls.Add(txtCode);

            top.Controls.Add(CreateLabel("Số lượng:"));
            top.Controls.Add(txtQty);

            top.Controls.Add(CreateButton("THÊM VÀO GIỎ", AddToCart));

            top.Controls.Add(CreateLabel("Nhân viên:"));
            top.Controls.Add(cboNV);

            top.Controls.Add(CreateLabel("Khách hàng:"));
            top.Controls.Add(cboKH);

            tab.Controls.Add(top);

            dgvCart = CreateGrid();
            tab.Controls.Add(dgvCart);

            FlowLayoutPanel bottom = CreatePanel();
            bottom.Dock = DockStyle.Bottom;

            lblTotal = new Label
            {
                Text = "Tổng tiền: 0 VNĐ",
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Padding = new Padding(10)
            };

            bottom.Controls.Add(lblTotal);

            bottom.Controls.Add(
                CreateButton("XÓA DÒNG", RemoveCartItem));

            bottom.Controls.Add(
                CreateButton("LÀM TRỐNG GIỎ", ClearCart));

            bottom.Controls.Add(
                CreateButton("THANH TOÁN", Pay));

            tab.Controls.Add(bottom);

            return tab;
        }

        private void AddToCart()
        {
            string code = txtCode.Text.Trim();

            if (code == "")
                throw new Exception("Hãy nhập mã sản phẩm!");

            int qty;

            if (!int.TryParse(txtQty.Text, out qty) || qty <= 0)
                throw new Exception("Số lượng không hợp lệ!");

            DataTable dt = bus.ProductByCode(code);

            if (dt.Rows.Count == 0)
                throw new Exception("Không tìm thấy sản phẩm!");

            DataRow row = dt.Rows[0];

            int stock = Convert.ToInt32(row["SoLuongTon"]);

            CartItem oldItem =
                cart.FirstOrDefault(x => x.MaSP == code);

            int oldQty = oldItem == null ? 0 : oldItem.SoLuong;

            if (oldQty + qty > stock)
                throw new Exception("Không đủ số lượng tồn kho!");

            if (oldItem == null)
            {
                cart.Add(new CartItem
                {
                    MaSP = code,
                    TenSP = row["TenSP"].ToString(),
                    SoLuong = qty,
                    DonGia = Convert.ToDecimal(row["GiaBan"])
                });
            }
            else
            {
                oldItem.SoLuong += qty;
            }

            RefreshCart();

            txtCode.Clear();
            txtQty.Text = "1";
        }

        private void RefreshCart()
        {
            dgvCart.DataSource = null;

            dgvCart.DataSource = cart.Select(x => new
            {
                MaSP = x.MaSP,
                TenSP = x.TenSP,
                SoLuong = x.SoLuong,
                DonGia = x.DonGia,
                ThanhTien = x.ThanhTien
            }).ToList();

            decimal total = cart.Sum(x => x.ThanhTien);

            lblTotal.Text =
                "Tổng tiền: " + total.ToString("N0") + " VNĐ";
        }

        private void RemoveCartItem()
        {
            if (dgvCart.CurrentRow == null)
                return;

            string id = Convert.ToString(
                dgvCart.CurrentRow.Cells["MaSP"].Value);

            cart.RemoveAll(x => x.MaSP == id);

            RefreshCart();
        }

        private void ClearCart()
        {
            cart.Clear();
            RefreshCart();
        }

        private void Pay()
        {
            if (cart.Count == 0)
                throw new Exception("Giỏ hàng đang trống!");

            if (cboNV.SelectedValue == null)
                throw new Exception("Chưa có nhân viên bán hàng!");

            int maNV = Convert.ToInt32(cboNV.SelectedValue);

            int? maKH = null;

            if (cboKH.SelectedIndex > 0 &&
                cboKH.SelectedValue != null &&
                cboKH.SelectedValue != DBNull.Value)
            {
                maKH = Convert.ToInt32(cboKH.SelectedValue);
            }

            decimal total = cart.Sum(x => x.ThanhTien);

            DialogResult result = MessageBox.Show(
                "Xác nhận thanh toán?\n" +
                "Tổng tiền: " + total.ToString("N0") + " VNĐ",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            int maHD = bus.Checkout(maNV, maKH, cart);

            MessageBox.Show(
                "THANH TOÁN THÀNH CÔNG!\n\n" +
                "Mã hóa đơn: " + maHD +
                "\nTổng tiền: " + total.ToString("N0") + " VNĐ",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            cart.Clear();

            RefreshCart();
            LoadProducts();
            LoadCustomers();
            LoadCombos();
            LoadReports();
        }

        // ==================================
        // TAB 2: QUẢN LÝ SẢN PHẨM
        // ==================================

        private TabPage CreateProductTab()
        {
            TabPage tab = CreateTab("Sản phẩm");

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill
            };

            FlowLayoutPanel search = CreatePanel();

            txtSearch = CreateTextBox(220);

            search.Controls.Add(CreateLabel("Tìm kiếm:"));
            search.Controls.Add(txtSearch);

            search.Controls.Add(
                CreateButton("TÌM KIẾM", LoadProducts));

            search.Controls.Add(
                CreateButton("TẢI LẠI", () =>
                {
                    txtSearch.Clear();
                    LoadProducts();
                }));

            panel.Controls.Add(search);

            dgvProduct = CreateGrid();
            dgvProduct.Dock = DockStyle.Fill;
            dgvProduct.Top = 50;

            dgvProduct.SelectionChanged += (s, e) =>
            {
                if (dgvProduct.CurrentRow == null)
                    return;

                DataGridViewRow r = dgvProduct.CurrentRow;

                txtMaSP.Text = Convert.ToString(r.Cells["MaSP"].Value);
                txtTenSP.Text = Convert.ToString(r.Cells["TenSP"].Value);
                txtGia.Text = Convert.ToString(r.Cells["GiaBan"].Value);
                txtTon.Text = Convert.ToString(r.Cells["SoLuongTon"].Value);
            };

            panel.Controls.Add(dgvProduct);

            FlowLayoutPanel edit = CreatePanel();
            edit.Dock = DockStyle.Bottom;

            txtMaSP = CreateTextBox(90);
            txtTenSP = CreateTextBox(180);
            txtGia = CreateTextBox(100);
            txtTon = CreateTextBox(80);

            edit.Controls.Add(CreateLabel("Mã SP:"));
            edit.Controls.Add(txtMaSP);

            edit.Controls.Add(CreateLabel("Tên SP:"));
            edit.Controls.Add(txtTenSP);

            edit.Controls.Add(CreateLabel("Giá:"));
            edit.Controls.Add(txtGia);

            edit.Controls.Add(CreateLabel("Tồn kho:"));
            edit.Controls.Add(txtTon);

            edit.Controls.Add(CreateButton("THÊM", AddProduct));
            edit.Controls.Add(CreateButton("SỬA", UpdateProduct));
            edit.Controls.Add(CreateButton("XÓA", DeleteProduct));

            panel.Controls.Add(edit);

            tab.Controls.Add(panel);

            return tab;
        }

        private Product ReadProduct()
        {
            decimal price;
            int stock;

            if (!decimal.TryParse(txtGia.Text, out price))
                throw new Exception("Giá bán không hợp lệ!");

            if (!int.TryParse(txtTon.Text, out stock))
                throw new Exception("Số lượng tồn không hợp lệ!");

            return new Product
            {
                MaSP = txtMaSP.Text.Trim(),
                TenSP = txtTenSP.Text.Trim(),
                GiaBan = price,
                SoLuongTon = stock
            };
        }

        private void AddProduct()
        {
            bus.AddProduct(ReadProduct());

            LoadProducts();

            MessageBox.Show("Thêm sản phẩm thành công!");
        }

        private void UpdateProduct()
        {
            bus.UpdateProduct(ReadProduct());

            LoadProducts();

            MessageBox.Show("Cập nhật sản phẩm thành công!");
        }

        private void DeleteProduct()
        {
            string id = txtMaSP.Text.Trim();

            if (id == "")
                throw new Exception("Chọn sản phẩm cần xóa!");

            if (MessageBox.Show(
                "Bạn có chắc muốn xóa sản phẩm này?",
                "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bus.DeleteProduct(id);
                LoadProducts();
            }
        }

        // ==================================
        // TAB 3: KHÁCH HÀNG
        // ==================================

        private TabPage CreateCustomerTab()
        {
            TabPage tab = CreateTab("Khách hàng");

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill
            };

            dgvCustomer = CreateGrid();

            dgvCustomer.SelectionChanged += (s, e) =>
            {
                if (dgvCustomer.CurrentRow == null)
                    return;

                DataGridViewRow r = dgvCustomer.CurrentRow;

                currentKH = Convert.ToInt32(r.Cells["MaKH"].Value);

                txtTenKH.Text = Convert.ToString(r.Cells["TenKH"].Value);

                txtSDTKH.Text = Convert.ToString(r.Cells["DienThoai"].Value);
            };

            panel.Controls.Add(dgvCustomer);

            FlowLayoutPanel edit = CreatePanel();
            edit.Dock = DockStyle.Bottom;

            txtTenKH = CreateTextBox(200);
            txtSDTKH = CreateTextBox(140);

            edit.Controls.Add(CreateLabel("Tên khách hàng:"));
            edit.Controls.Add(txtTenKH);

            edit.Controls.Add(CreateLabel("Điện thoại:"));
            edit.Controls.Add(txtSDTKH);

            edit.Controls.Add(CreateButton("THÊM", AddCustomer));
            edit.Controls.Add(CreateButton("SỬA", UpdateCustomer));
            edit.Controls.Add(CreateButton("XÓA", DeleteCustomer));

            panel.Controls.Add(edit);

            tab.Controls.Add(panel);

            return tab;
        }

        private void AddCustomer()
        {
            Customer c = new Customer
            {
                TenKH = txtTenKH.Text.Trim(),
                DienThoai = txtSDTKH.Text.Trim()
            };

            bus.AddCustomer(c);

            LoadCustomers();
            LoadCombos();

            MessageBox.Show("Thêm khách hàng thành công!");
        }

        private void UpdateCustomer()
        {
            Customer c = new Customer
            {
                MaKH = currentKH,
                TenKH = txtTenKH.Text.Trim(),
                DienThoai = txtSDTKH.Text.Trim()
            };

            bus.UpdateCustomer(c);

            LoadCustomers();
            LoadCombos();

            MessageBox.Show("Cập nhật khách hàng thành công!");
        }

        private void DeleteCustomer()
        {
            if (currentKH <= 0)
                throw new Exception("Hãy chọn khách hàng!");

            if (MessageBox.Show(
                "Xóa khách hàng này?",
                "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bus.DeleteCustomer(currentKH);

                currentKH = 0;

                LoadCustomers();
                LoadCombos();
            }
        }

        // ==================================
        // TAB 4: NHÂN VIÊN
        // ==================================

        private TabPage CreateEmployeeTab()
        {
            TabPage tab = CreateTab("Nhân viên");

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill
            };

            dgvEmployee = CreateGrid();

            dgvEmployee.SelectionChanged += (s, e) =>
            {
                if (dgvEmployee.CurrentRow == null)
                    return;

                DataGridViewRow r = dgvEmployee.CurrentRow;

                currentNV = Convert.ToInt32(r.Cells["MaNV"].Value);

                txtTenNV.Text = Convert.ToString(r.Cells["TenNV"].Value);

                txtSDTNV.Text = Convert.ToString(r.Cells["SoDienThoai"].Value);

                txtChucVu.Text = Convert.ToString(r.Cells["ChucVu"].Value);
            };

            panel.Controls.Add(dgvEmployee);

            FlowLayoutPanel edit = CreatePanel();
            edit.Dock = DockStyle.Bottom;

            txtTenNV = CreateTextBox(180);
            txtSDTNV = CreateTextBox(130);
            txtChucVu = CreateTextBox(130);

            edit.Controls.Add(CreateLabel("Tên nhân viên:"));
            edit.Controls.Add(txtTenNV);

            edit.Controls.Add(CreateLabel("Điện thoại:"));
            edit.Controls.Add(txtSDTNV);

            edit.Controls.Add(CreateLabel("Chức vụ:"));
            edit.Controls.Add(txtChucVu);

            edit.Controls.Add(CreateButton("THÊM", AddEmployee));
            edit.Controls.Add(CreateButton("SỬA", UpdateEmployee));
            edit.Controls.Add(CreateButton("XÓA", DeleteEmployee));

            panel.Controls.Add(edit);

            tab.Controls.Add(panel);

            return tab;
        }

        private void AddEmployee()
        {
            Employee e = new Employee
            {
                TenNV = txtTenNV.Text.Trim(),
                SoDienThoai = txtSDTNV.Text.Trim(),
                ChucVu = txtChucVu.Text.Trim()
            };

            bus.AddEmployee(e);

            LoadEmployees();
            LoadCombos();

            MessageBox.Show("Thêm nhân viên thành công!");
        }

        private void UpdateEmployee()
        {
            Employee e = new Employee
            {
                MaNV = currentNV,
                TenNV = txtTenNV.Text.Trim(),
                SoDienThoai = txtSDTNV.Text.Trim(),
                ChucVu = txtChucVu.Text.Trim()
            };

            bus.UpdateEmployee(e);

            LoadEmployees();
            LoadCombos();

            MessageBox.Show("Cập nhật nhân viên thành công!");
        }

        private void DeleteEmployee()
        {
            if (currentNV <= 0)
                throw new Exception("Hãy chọn nhân viên!");

            if (MessageBox.Show(
                "Xóa nhân viên này?",
                "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bus.DeleteEmployee(currentNV);

                currentNV = 0;

                LoadEmployees();
                LoadCombos();
            }
        }

        // ==================================
        // TAB 5: BÁO CÁO
        // ==================================

        private TabPage CreateReportTab()
        {
            TabPage tab = CreateTab("Báo cáo");

            FlowLayoutPanel top = CreatePanel();

            lblToday = new Label
            {
                AutoSize = true,
                Font = new Font("Arial", 13, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                Padding = new Padding(10)
            };

            lblMonth = new Label
            {
                AutoSize = true,
                Font = new Font("Arial", 13, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Padding = new Padding(10)
            };

            top.Controls.Add(lblToday);
            top.Controls.Add(lblMonth);

            top.Controls.Add(
                CreateButton("CẬP NHẬT BÁO CÁO", LoadReports));

            tab.Controls.Add(top);

            dgvTop = CreateGrid();

            tab.Controls.Add(dgvTop);

            return tab;
        }

        // ==================================
        // TẢI DỮ LIỆU
        // ==================================

        private void LoadProducts()
        {
            if (dgvProduct != null)
                dgvProduct.DataSource =
                    bus.Products(txtSearch.Text.Trim());
        }

        private void LoadCustomers()
        {
            if (dgvCustomer != null)
                dgvCustomer.DataSource = bus.Customers();
        }

        private void LoadEmployees()
        {
            if (dgvEmployee != null)
                dgvEmployee.DataSource = bus.Employees();
        }

        private void LoadCombos()
        {
            if (cboNV != null)
            {
                DataTable dtNV = bus.Employees();

                cboNV.DataSource = dtNV;
                cboNV.DisplayMember = "TenNV";
                cboNV.ValueMember = "MaNV";

                if (dtNV.Rows.Count > 0)
                    cboNV.SelectedIndex = 0;
            }

            if (cboKH != null)
            {
                DataTable dtKH = bus.Customers();

                DataRow guest = dtKH.NewRow();

                guest["MaKH"] = DBNull.Value;
                guest["TenKH"] = "Khách lẻ";

                dtKH.Rows.InsertAt(guest, 0);

                cboKH.DataSource = dtKH;
                cboKH.DisplayMember = "TenKH";
                cboKH.ValueMember = "MaKH";

                cboKH.SelectedIndex = 0;
            }
        }

        private void LoadReports()
        {
            if (lblToday != null)
            {
                lblToday.Text =
                    "Doanh thu hôm nay: " +
                    bus.RevenueToday().ToString("N0") + " VNĐ";
            }

            if (lblMonth != null)
            {
                lblMonth.Text =
                    "Doanh thu tháng này: " +
                    bus.RevenueMonth().ToString("N0") + " VNĐ";
            }

            if (dgvTop != null)
                dgvTop.DataSource = bus.TopProducts();
        }

        private void LoadAll()
        {
            LoadProducts();
            LoadCustomers();
            LoadEmployees();
            LoadCombos();
            LoadReports();
            RefreshCart();
        }
    }
}
