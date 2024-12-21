using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Bai.De1;

namespace Bai
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                using (Model1 studentDB = new Model1())
                {

                    List<Sinhvien> sinhViens = studentDB.Sinhvien.ToList();
                    List<Lop> lops = studentDB.Lop.ToList();

                    if (dtSinhVien.Columns.Count == 0)
                    {
                        dtSinhVien.Columns.Add("MaSV", "Mã SV");
                        dtSinhVien.Columns.Add("HoTenSV", "Họ Tên SV");
                        dtSinhVien.Columns.Add("NgaySinh", "Ngày Sinh");
                        dtSinhVien.Columns.Add("Lop", "Tên Lớp");
                    }


                    PopulateComboBox(lops);
                    PopulateGrid(sinhViens);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateComboBox(List<Lop> listLop)
        {
            cbolop.DataSource = listLop;
            cbolop.DisplayMember = "TenLop";
            cbolop.ValueMember = "MaLop";
        }

        private void PopulateGrid(List<Sinhvien> list)
        {
            dtSinhVien.Rows.Clear();
            foreach (var item in list)
            {
                int index = dtSinhVien.Rows.Add();
                dtSinhVien.Rows[index].Cells[0].Value = item.MaSV;
                dtSinhVien.Rows[index].Cells[1].Value = item.HotenSV;
                dtSinhVien.Rows[index].Cells[2].Value = item.NgaySinh.ToString("dd/MM/yyyy");
                dtSinhVien.Rows[index].Cells[3].Value = item.Lop?.TenLop ?? "N/A";
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Model1 studentDB = new Model1())
                {

                    Sinhvien newSinhVien = new Sinhvien
                    {
                        MaSV = txtMaSV.Text.Trim(),
                        HotenSV = txtHotenSV.Text.Trim(),
                        NgaySinh = dtNgaysinh.Value,
                        MaLop = cbolop.SelectedValue.ToString()
                    };

                    if (studentDB.Sinhvien.Any(s => s.MaSV == newSinhVien.MaSV))
                    {
                        MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng nhập mã khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    studentDB.Sinhvien.Add(newSinhVien);
                    studentDB.SaveChanges();

                    PopulateGrid(studentDB.Sinhvien.ToList());

                    MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtSinhVien.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dtSinhVien.SelectedRows[0];
                string maSV = selectedRow.Cells[0].Value.ToString();

                using (Model1 studentDB = new Model1())
                {
                    Sinhvien sinhVienToEdit = studentDB.Sinhvien.Find(maSV);
                    if (sinhVienToEdit != null)
                    {

                        sinhVienToEdit.HotenSV = txtHotenSV.Text.Trim();
                        sinhVienToEdit.NgaySinh = dtNgaysinh.Value;
                        sinhVienToEdit.MaLop = cbolop.SelectedValue.ToString();

                        studentDB.SaveChanges();


                        PopulateGrid(studentDB.Sinhvien.ToList());

                        MessageBox.Show("Sửa thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sinh viên không tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtSinhVien.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dtSinhVien.SelectedRows[0];
                string maSV = selectedRow.Cells[0].Value.ToString();

                using (Model1 studentDB = new Model1())
                {
                    Sinhvien sinhVienToDelete = studentDB.Sinhvien.Find(maSV);
                    if (sinhVienToDelete != null)
                    {
                        studentDB.Sinhvien.Remove(sinhVienToDelete);
                        studentDB.SaveChanges();

                        PopulateGrid(studentDB.Sinhvien.ToList());

                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow selectedRow = dtSinhVien.Rows[e.RowIndex];

                    txtMaSV.Text = selectedRow.Cells[0].Value?.ToString() ?? string.Empty;
                    txtHotenSV.Text = selectedRow.Cells[1].Value?.ToString() ?? string.Empty;
                    dtNgaysinh.Value = DateTime.TryParse(selectedRow.Cells[2].Value?.ToString(), out DateTime ngaySinh)
                        ? ngaySinh
                        : DateTime.Now;
                    cbolop.SelectedValue = selectedRow.Cells[3].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý sự kiện CellClick: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtSinhVien.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dtSinhVien.SelectedRows[0];
                string maSV = selectedRow.Cells[0].Value.ToString();

                using (Model1 studentDB = new Model1())
                {
                    Sinhvien sinhVienToEdit = studentDB.Sinhvien.Find(maSV);
                    if (sinhVienToEdit != null)
                    {
                       

                  
                        studentDB.SaveChanges();

                    
                        MessageBox.Show("Thông tin đã được lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sinh viên không tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtSinhVien.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần hủy lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dtSinhVien.SelectedRows[0];
                string maSV = selectedRow.Cells[0].Value.ToString();

                using (Model1 studentDB = new Model1())
                {
                    Sinhvien sinhVienToEdit = studentDB.Sinhvien.Find(maSV);
                    if (sinhVienToEdit != null)
                    {


                        MessageBox.Show("Đã hủy thao tác lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sinh viên không tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hủy lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                using (Model1 studentDB = new Model1())
                {
                    string tenSinhVien = txtTimKiem.Text.Trim();

                    var resultList = studentDB.Sinhvien
                        .Where(s => s.HotenSV.Contains(tenSinhVien))
                        .ToList();

                    PopulateGrid(resultList);

                    if (resultList.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
