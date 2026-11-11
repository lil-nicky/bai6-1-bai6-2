
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using HRMPayrollSystem.BUS;
using HRMPayrollSystem.DTO;

namespace HRMPayrollSystem.GUI
{
    public class MainForm : Form
    {
        private HRBUS bus = new HRBUS();

        private DataGridView dgvDepartment;
        private DataGridView dgvEmployee;
        private DataGridView dgvAttendance;
        private DataGridView dgvSalary;

        private TextBox txtTenPB;
        private TextBox txtMoTa;

        private TextBox txtSearch;
        private TextBox txtHoTen;
        private TextBox txtCCCD;
        private TextBox txtLuong;
        private TextBox txtHeSo;

        private DateTimePicker dtpNgaySinh;

        private ComboBox cboPhongBan;
        private ComboBox cboSearchPB;

        private DateTimePicker dtpMonth;

        private Label lblStatus;

        private int currentPB = 0;
        private int currentNV = 0;

        private DataTable attendanceTable;

        public MainForm()
        {
            Text = "HRM & PAYROLL SYSTEM - QUẢN LÝ NHÂN SỰ VÀ TÍNH LƯƠNG";
            Width = 1250;
            Height = 760;
            MinimumSize = new Size(1050, 600);
            StartPosition = FormStartPosition.CenterScreen;

            BuildInterface();
            LoadAll();
        }

        // =====================================
        // GIAO DIỆN CHUNG
        // =====================================

        private void BuildInterface()
        {
            TabControl tabs = new TabControl
            {
                Dock = DockStyle.Fill
            };

            tabs.TabPages.Add(CreateDepartmentTab());
            tabs.TabPages.Add(CreateEmployeeTab());
            tabs.TabPages.Add(CreateAttendanceTab());
            tabs.TabPages.Add(CreateSalaryTab());

            Controls.Add(tabs);
        }

        private TabPage NewTab(string name)
        {
            return new TabPage(name)
            {
                BackColor = Color.WhiteSmoke
            };
        }

        private DataGridView NewGrid()
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

        private TextBox NewText(int width = 150)
        {
            return new TextBox
            {
                Width = width,
                Margin = new Padding(5)
            };
        }

        private Label NewLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Padding = new Padding(5, 8, 3, 3)
            };
        }

        private FlowLayoutPanel NewPanel()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = true,
                Padding = new Padding(5)
            };
        }

        private Button NewButton(string text, Action action)
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

        // =====================================
        // TAB 1: PHÒNG BAN
        // =====================================

        private TabPage CreateDepartmentTab()
        {
            TabPage tab = NewTab("PHÒNG BAN");

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill
            };

            dgvDepartment = NewGrid();

            dgvDepartment.SelectionChanged += (s, e) =>
            {
                if (dgvDepartment.CurrentRow == null)
                    return;

                DataGridViewRow r = dgvDepartment.CurrentRow;

                currentPB = Convert.ToInt32(r.Cells["MaPB"].Value);

                txtTenPB.Text =
                    Convert.ToString(r.Cells["TenPB"].Value);

                txtMoTa.Text =
                    Convert.ToString(r.Cells["MoTa"].Value);
            };

            panel.Controls.Add(dgvDepartment);

            FlowLayoutPanel edit = NewPanel();
            edit.Dock = DockStyle.Bottom;

            txtTenPB = NewText(200);
            txtMoTa = NewText(250);

            edit.Controls.Add(NewLabel("Tên phòng ban:"));
            edit.Controls.Add(txtTenPB);

            edit.Controls.Add(NewLabel("Mô tả:"));
            edit.Controls.Add(txtMoTa);

            edit.Controls.Add(NewButton("THÊM", AddDepartment));
            edit.Controls.Add(NewButton("SỬA", UpdateDepartment));
            edit.Controls.Add(NewButton("XÓA", DeleteDepartment));

            panel.Controls.Add(edit);

            tab.Controls.Add(panel);

            return tab;
        }

        private void AddDepartment()
        {
            Department d = new Department
            {
                TenPB = txtTenPB.Text.Trim(),
                MoTa = txtMoTa.Text.Trim()
            };

            bus.AddDepartment(d);

            LoadDepartments();
            LoadDepartmentCombos();

            MessageBox.Show("Thêm phòng ban thành công!");
        }

        private void UpdateDepartment()
        {
            Department d = new Department
            {
                MaPB = currentPB,
                TenPB = txtTenPB.Text.Trim(),
                MoTa = txtMoTa.Text.Trim()
            };

            bus.UpdateDepartment(d);

            LoadDepartments();
            LoadDepartmentCombos();

            MessageBox.Show("Cập nhật phòng ban thành công!");
        }

        private void DeleteDepartment()
        {
            if (currentPB <= 0)
                throw new Exception("Hãy chọn phòng ban!");

            if (MessageBox.Show(
                "Bạn có chắc chắn muốn xóa phòng ban?",
                "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bus.DeleteDepartment(currentPB);

                currentPB = 0;

                LoadDepartments();
                LoadDepartmentCombos();
            }
        }

        // =====================================
        // TAB 2: NHÂN VIÊN
        // =====================================

        private TabPage CreateEmployeeTab()
        {
            TabPage tab = NewTab("NHÂN VIÊN");

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill
            };

            FlowLayoutPanel search = NewPanel();

            txtSearch = NewText(180);

            cboSearchPB = new ComboBox
            {
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            search.Controls.Add(NewLabel("Tìm kiếm:"));
            search.Controls.Add(txtSearch);

            search.Controls.Add(NewLabel("Phòng ban:"));
            search.Controls.Add(cboSearchPB);

            search.Controls.Add(
                NewButton("TÌM KIẾM", LoadEmployees));

            search.Controls.Add(
                NewButton("TẢI LẠI", () =>
                {
                    txtSearch.Clear();
                    cboSearchPB.SelectedIndex = 0;
                    LoadEmployees();
                }));

            panel.Controls.Add(search);

            dgvEmployee = NewGrid();

            dgvEmployee.Top = 50;

            dgvEmployee.SelectionChanged += (s, e) =>
            {
                if (dgvEmployee.CurrentRow == null)
                    return;

                DataGridViewRow r = dgvEmployee.CurrentRow;

                currentNV = Convert.ToInt32(r.Cells["MaNV"].Value);

                txtHoTen.Text =
                    Convert.ToString(r.Cells["HoTen"].Value);

                txtCCCD.Text =
                    Convert.ToString(r.Cells["CCCD"].Value);

                txtLuong.Text =
                    Convert.ToString(r.Cells["LuongCoBan"].Value);

                txtHeSo.Text =
                    Convert.ToString(r.Cells["HeSoLuong"].Value);

                if (r.Cells["NgaySinh"].Value != DBNull.Value)
                {
                    dtpNgaySinh.Value =
                        Convert.ToDateTime(r.Cells["NgaySinh"].Value);
                }

                cboPhongBan.SelectedValue =
                    Convert.ToInt32(r.Cells["MaPB"].Value);
            };

            panel.Controls.Add(dgvEmployee);

            FlowLayoutPanel edit = NewPanel();
            edit.Dock = DockStyle.Bottom;

            txtHoTen = NewText(160);
            txtCCCD = NewText(130);
            txtLuong = NewText(100);
            txtHeSo = NewText(70);

            dtpNgaySinh = new DateTimePicker
            {
                Width = 130,
                Format = DateTimePickerFormat.Short
            };

            cboPhongBan = new ComboBox
            {
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            edit.Controls.Add(NewLabel("Họ tên:"));
            edit.Controls.Add(txtHoTen);

            edit.Controls.Add(NewLabel("Ngày sinh:"));
            edit.Controls.Add(dtpNgaySinh);

            edit.Controls.Add(NewLabel("CCCD:"));
            edit.Controls.Add(txtCCCD);

            edit.Controls.Add(NewLabel("Lương CB:"));
            edit.Controls.Add(txtLuong);

            edit.Controls.Add(NewLabel("Hệ số:"));
            edit.Controls.Add(txtHeSo);

            edit.Controls.Add(NewLabel("Phòng ban:"));
            edit.Controls.Add(cboPhongBan);

            edit.Controls.Add(NewButton("THÊM", AddEmployee));
            edit.Controls.Add(NewButton("SỬA / CHUYỂN PB", UpdateEmployee));
            edit.Controls.Add(NewButton("XÓA", DeleteEmployee));

            panel.Controls.Add(edit);

            tab.Controls.Add(panel);

            return tab;
        }

        private Employee ReadEmployee()
        {
            decimal luong;
            double heso;

            if (!decimal.TryParse(txtLuong.Text, out luong))
                throw new Exception("Lương cơ bản không hợp lệ!");

            if (!double.TryParse(txtHeSo.Text, out heso))
                throw new Exception("Hệ số lương không hợp lệ!");

            if (cboPhongBan.SelectedValue == null)
                throw new Exception("Chưa chọn phòng ban!");

            return new Employee
            {
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value,
                CCCD = txtCCCD.Text.Trim(),
                MaPB = Convert.ToInt32(cboPhongBan.SelectedValue),
                LuongCoBan = luong,
                HeSoLuong = heso
            };
        }

        private void AddEmployee()
        {
            Employee e = ReadEmployee();

            bus.AddEmployee(e);

            LoadEmployees();

            MessageBox.Show("Thêm nhân viên thành công!");
        }

        private void UpdateEmployee()
        {
            Employee e = ReadEmployee();

            e.MaNV = currentNV;

            bus.UpdateEmployee(e);

            LoadEmployees();

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
            }
        }

        // =====================================
        // TAB 3: CHẤM CÔNG
        // =====================================

        private TabPage CreateAttendanceTab()
        {
            TabPage tab = NewTab("CHẤM CÔNG");

            FlowLayoutPanel top = NewPanel();

            dtpMonth = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "MM/yyyy",
                ShowUpDown = true,
                Width = 120
            };

            top.Controls.Add(NewLabel("Tháng chấm công:"));
            top.Controls.Add(dtpMonth);

            top.Controls.Add(
                NewButton("TẢI BẢNG CHẤM CÔNG", LoadAttendance));

            top.Controls.Add(
                NewButton("LƯU CHẤM CÔNG", SaveAttendance));

            dgvAttendance = NewGrid();

            dgvAttendance.ReadOnly = false;

            dgvAttendance.AllowUserToAddRows = false;

            dgvAttendance.CellBeginEdit += (s, e) =>
            {
                if (dgvAttendance.Columns[e.ColumnIndex].Name != "SoNgayCong" &&
                    dgvAttendance.Columns[e.ColumnIndex].Name != "SoGioOT")
                {
                    e.Cancel = true;
                }
            };

            tab.Controls.Add(dgvAttendance);
            tab.Controls.Add(top);

            return tab;
        }

        private string GetMonth()
        {
            return dtpMonth.Value.ToString("MM/yyyy");
        }

        private void LoadAttendance()
        {
            string month = GetMonth();

            attendanceTable = bus.LoadAttendance(month);

            dgvAttendance.DataSource = attendanceTable;

            dgvAttendance.ReadOnly = false;

            dgvAttendance.Columns["MaCong"].ReadOnly = true;
            dgvAttendance.Columns["MaNV"].ReadOnly = true;
            dgvAttendance.Columns["ThangNam"].ReadOnly = true;
            dgvAttendance.Columns["HoTen"].ReadOnly = true;

            dgvAttendance.Columns["SoNgayCong"].ReadOnly = false;
            dgvAttendance.Columns["SoGioOT"].ReadOnly = false;

            dgvAttendance.Columns["MaCong"].HeaderText = "Mã công";
            dgvAttendance.Columns["MaNV"].HeaderText = "Mã NV";
            dgvAttendance.Columns["HoTen"].HeaderText = "Họ tên";
            dgvAttendance.Columns["ThangNam"].HeaderText = "Tháng";
            dgvAttendance.Columns["SoNgayCong"].HeaderText = "Số ngày công";
            dgvAttendance.Columns["SoGioOT"].HeaderText = "Số giờ OT";

            // Nếu tháng đã chốt lương thì khóa lưới.
            DataTable salary = bus.Salary(month);

            bool locked = salary.Rows.Count > 0;

            dgvAttendance.ReadOnly = locked;

            if (locked)
            {
                MessageBox.Show(
                    "Tháng này đã chốt lương. Dữ liệu chấm công đã bị khóa!");
            }
        }

        private void SaveAttendance()
        {
            if (attendanceTable == null)
                throw new Exception("Hãy tải bảng chấm công trước!");

            dgvAttendance.EndEdit();

            int count = bus.SaveAttendance(
                attendanceTable,
                GetMonth());

            MessageBox.Show(
                "Lưu chấm công thành công!\nSố dòng được cập nhật: " + count);

            // Tải lại để lấy MaCong Identity vừa được SQL Server sinh.
            LoadAttendance();
        }

        // =====================================
        // TAB 4: TÍNH LƯƠNG
        // =====================================

        private TabPage CreateSalaryTab()
        {
            TabPage tab = NewTab("TÍNH LƯƠNG");

            FlowLayoutPanel top = NewPanel();

            top.Controls.Add(NewLabel("Tháng tính lương:"));

            DateTimePicker salaryMonth = new DateTimePicker
            {
                Name = "salaryMonth",
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "MM/yyyy",
                ShowUpDown = true,
                Width = 120
            };

            top.Controls.Add(salaryMonth);

            top.Controls.Add(
                NewButton("XEM BẢNG LƯƠNG", () =>
                {
                    LoadSalary(salaryMonth.Value.ToString("MM/yyyy"));
                }));

            top.Controls.Add(
                NewButton("CHỐT LƯƠNG THÁNG", () =>
                {
                    CloseSalary(salaryMonth.Value.ToString("MM/yyyy"));
                }));

            dgvSalary = NewGrid();

            tab.Controls.Add(dgvSalary);
            tab.Controls.Add(top);

            return tab;
        }

        private void LoadSalary(string month)
        {
            dgvSalary.DataSource = bus.Salary(month);

            if (dgvSalary.Columns.Contains("MaLuong"))
                dgvSalary.Columns["MaLuong"].HeaderText = "Mã lương";

            if (dgvSalary.Columns.Contains("MaNV"))
                dgvSalary.Columns["MaNV"].HeaderText = "Mã nhân viên";

            if (dgvSalary.Columns.Contains("HoTen"))
                dgvSalary.Columns["HoTen"].HeaderText = "Họ tên";

            if (dgvSalary.Columns.Contains("TenPB"))
                dgvSalary.Columns["TenPB"].HeaderText = "Phòng ban";

            if (dgvSalary.Columns.Contains("ThangNam"))
                dgvSalary.Columns["ThangNam"].HeaderText = "Tháng";

            if (dgvSalary.Columns.Contains("ThucLinh"))
                dgvSalary.Columns["ThucLinh"].HeaderText = "Thực lĩnh";
        }

        private void CloseSalary(string month)
        {
            if (MessageBox.Show(
                "Bạn chắc chắn muốn chốt lương tháng " + month + "?",
                "Xác nhận chốt lương",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            int count = bus.CloseSalary(month);

            MessageBox.Show(
                "CHỐT LƯƠNG THÀNH CÔNG!\n" +
                "Tháng: " + month +
                "\nSố nhân viên: " + count,
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LoadSalary(month);
        }

        // =====================================
        // TẢI DỮ LIỆU
        // =====================================

        private void LoadDepartments()
        {
            dgvDepartment.DataSource = bus.Departments();
        }

        private void LoadDepartmentCombos()
        {
            DataTable dt1 = bus.Departments();

            cboPhongBan.DataSource = dt1.Copy();
            cboPhongBan.DisplayMember = "TenPB";
            cboPhongBan.ValueMember = "MaPB";

            DataTable dt2 = bus.Departments();

            DataRow all = dt2.NewRow();

            all["MaPB"] = 0;
            all["TenPB"] = "Tất cả phòng ban";

            dt2.Rows.InsertAt(all, 0);

            cboSearchPB.DataSource = dt2;
            cboSearchPB.DisplayMember = "TenPB";
            cboSearchPB.ValueMember = "MaPB";
        }

        private void LoadEmployees()
        {
            if (dgvEmployee == null)
                return;

            int maPB = cboSearchPB.SelectedValue == null
                ? 0
                : Convert.ToInt32(cboSearchPB.SelectedValue);

            dgvEmployee.DataSource = bus.Employees(
                txtSearch.Text.Trim(),
                maPB);
        }

        private void LoadAll()
        {
            LoadDepartments();
            LoadDepartmentCombos();
            LoadEmployees();
        }
    }
}
