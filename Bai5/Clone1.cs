using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public class Bai6Form : Form
    {
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelMain;
        private Label lblInstruction;
        private TextBox txtNewFood;
        private TextBox txtHinhAnh;
        private ComboBox cmbNguoiDung;
        private Button btnAdd;
        private Button btnRandom;
        private Panel panelResult;
        private Label lblResultFood;
        private PictureBox picResultImage;
        private Label lblResultContributor;
        private ListView lstFoods;
        private Button btnReset;
        private Button btnAddUser;
        private Button btnAddSampleData;
        private Label lblCount;
        private DatabaseHelper dbHelper;
        private List<MonAnInfo> monAnList;
        private List<NguoiDungInfo> nguoiDungList;

        public Bai6Form()
        {
            dbHelper = new DatabaseHelper();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Hôm nay ăn gì?";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(650, 700);
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Padding = new Padding(0);

            panelHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(650, 100),
                BackColor = Color.FromArgb(255, 87, 34)
            };

            lblTitle = new Label
            {
                Text = "🍽️ HÔM NAY ĂN GÌ?",
                Location = new Point(0, 30),
                Size = new Size(650, 40),
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelHeader.Controls.Add(lblTitle);

            panelMain = new Panel
            {
                Location = new Point(30, 120),
                Size = new Size(590, 540),
                BackColor = Color.White
            };

            lblInstruction = new Label
            {
                Text = "Thêm món ăn yêu thích",
                Location = new Point(0, 0),
                Size = new Size(590, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtNewFood = new TextBox
            {
                Location = new Point(0, 35),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tên món ăn"
            };
            txtNewFood.KeyPress += TxtNewFood_KeyPress;

            txtHinhAnh = new TextBox
            {
                Location = new Point(210, 35),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tên file hình ảnh"
            };

            cmbNguoiDung = new ComboBox
            {
                Location = new Point(370, 35),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnAdd = new Button
            {
                Text = "Thêm",
                Location = new Point(500, 35),
                Size = new Size(90, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            btnAddUser = new Button
            {
                Text = "Thêm người dùng",
                Location = new Point(0, 75),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddUser.FlatAppearance.BorderSize = 0;
            btnAddUser.Click += BtnAddUser_Click;

            btnAddSampleData = new Button
            {
                Text = "Thêm dữ liệu mẫu",
                Location = new Point(130, 75),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddSampleData.FlatAppearance.BorderSize = 0;
            btnAddSampleData.Click += BtnAddSampleData_Click;

            btnRandom = new Button
            {
                Text = "🎲 CHỌN MÓN NGẪU NHIÊN",
                Location = new Point(0, 115),
                Size = new Size(590, 55),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 87, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRandom.FlatAppearance.BorderSize = 0;
            btnRandom.Click += BtnRandom_Click;

            panelResult = new Panel
            {
                Location = new Point(0, 190),
                Size = new Size(590, 120),
                BackColor = Color.FromArgb(255, 248, 225),
                BorderStyle = BorderStyle.FixedSingle
            };

            picResultImage = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblResultFood = new Label
            {
                Text = "...",
                Location = new Point(120, 10),
                Size = new Size(460, 50),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 87, 34),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblResultContributor = new Label
            {
                Text = "",
                Location = new Point(120, 60),
                Size = new Size(460, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleLeft
            };

            panelResult.Controls.Add(picResultImage);
            panelResult.Controls.Add(lblResultFood);
            panelResult.Controls.Add(lblResultContributor);

            lblCount = new Label
            {
                Text = "Danh sách món ăn",
                Location = new Point(0, 330),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            btnReset = new Button
            {
                Text = "↻ Reset",
                Location = new Point(500, 327),
                Size = new Size(90, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(100, 100, 100),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btnReset.Click += BtnReset_Click;

            lstFoods = new ListView
            {
                Location = new Point(0, 365),
                Size = new Size(590, 175),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            lstFoods.Columns.Add("ID", 50);
            lstFoods.Columns.Add("Tên món ăn", 200);
            lstFoods.Columns.Add("Hình ảnh", 150);
            lstFoods.Columns.Add("Người đóng góp", 150);
            lstFoods.Columns.Add("Quyền hạn", 100);

            panelMain.Controls.Add(lblInstruction);
            panelMain.Controls.Add(txtNewFood);
            panelMain.Controls.Add(txtHinhAnh);
            panelMain.Controls.Add(cmbNguoiDung);
            panelMain.Controls.Add(btnAdd);
            panelMain.Controls.Add(btnAddUser);
            panelMain.Controls.Add(btnAddSampleData);
            panelMain.Controls.Add(btnRandom);
            panelMain.Controls.Add(panelResult);
            panelMain.Controls.Add(lblCount);
            panelMain.Controls.Add(btnReset);
            panelMain.Controls.Add(lstFoods);

            Controls.Add(panelHeader);
            Controls.Add(panelMain);
        }

        private void TxtNewFood_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnAdd_Click(sender, e);
                e.Handled = true;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string monAnMoi = txtNewFood.Text.Trim();
            string hinhAnh = txtHinhAnh.Text.Trim();

            if (string.IsNullOrWhiteSpace(monAnMoi))
            {
                MessageBox.Show("Vui lòng nhập tên món ăn", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewFood.Focus();
                return;
            }

            if (cmbNguoiDung.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn người đóng góp", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbNguoiDung.Focus();
                return;
            }

            var selectedUser = (NguoiDungInfo)cmbNguoiDung.SelectedItem;

            if (monAnList.Any(mon => mon.TenMonAn.Equals(monAnMoi, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"Món '{monAnMoi}' đã có trong danh sách", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNewFood.Clear();
                txtNewFood.Focus();
                return;
            }

            if (dbHelper.AddMonAn(monAnMoi, hinhAnh, selectedUser.IDNCC))
            {
                LoadData();
                txtNewFood.Clear();
                txtHinhAnh.Clear();
                txtNewFood.Focus();
                FlashControl(btnAdd, Color.FromArgb(46, 125, 50));
                MessageBox.Show($"Đã thêm món '{monAnMoi}' thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm món ăn", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRandom_Click(object sender, EventArgs e)
        {
            if (monAnList.Count == 0)
            {
                MessageBox.Show("Danh sách món ăn trống", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblResultFood.Text = "🎲";
            lblResultContributor.Text = "";
            picResultImage.Image = null;
            panelResult.BackColor = Color.FromArgb(255, 235, 59);
            Application.DoEvents();
            System.Threading.Thread.Sleep(500);

            var monDuocChon = dbHelper.GetRandomMonAn();
            if (monDuocChon != null)
            {
                lblResultFood.Text = monDuocChon.TenMonAn;
                lblResultContributor.Text = $"Đóng góp bởi: {monDuocChon.HoVaTen} ({monDuocChon.QuyenHan})";
                
                // Load hình ảnh nếu có
                if (!string.IsNullOrEmpty(monDuocChon.HinhAnh))
                {
                    LoadFoodImage(monDuocChon.HinhAnh);
                }
                else
                {
                    picResultImage.Image = null;
                }

                panelResult.BackColor = Color.FromArgb(255, 248, 225);

                // Highlight trong ListView
                foreach (ListViewItem item in lstFoods.Items)
                {
                    if (item.SubItems[1].Text == monDuocChon.TenMonAn)
                    {
                        item.Selected = true;
                        item.EnsureVisible();
                        break;
                    }
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Reset về danh sách mặc định?\nTất cả dữ liệu hiện tại sẽ bị xóa!",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Xóa file database và tạo lại (sử dụng đường dẫn đầy đủ từ DatabaseHelper)
                try
                {
                    var dbPath = dbHelper.DatabaseFilePath;
                    if (!string.IsNullOrEmpty(dbPath) && File.Exists(dbPath))
                    {
                        File.Delete(dbPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể xóa file database: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                dbHelper = new DatabaseHelper();
                LoadData();
                lblResultFood.Text = "...";
                lblResultContributor.Text = "";
                picResultImage.Image = null;
                txtNewFood.Clear();
                txtHinhAnh.Clear();
            }
        }

        private void LoadData()
        {
            monAnList = dbHelper.GetAllMonAn();
            nguoiDungList = dbHelper.GetAllNguoiDung();

            // Cập nhật ComboBox người dùng
            cmbNguoiDung.DataSource = nguoiDungList;
            cmbNguoiDung.DisplayMember = "HoVaTen";
            cmbNguoiDung.ValueMember = "IDNCC";

            // Cập nhật ListView
            lstFoods.Items.Clear();
            foreach (var mon in monAnList)
            {
                var item = new ListViewItem(mon.IDMA.ToString());
                item.SubItems.Add(mon.TenMonAn);
                item.SubItems.Add(string.IsNullOrEmpty(mon.HinhAnh) ? "Không có" : mon.HinhAnh);
                item.SubItems.Add(string.IsNullOrEmpty(mon.HoVaTen) ? "Không xác định" : mon.HoVaTen);
                item.SubItems.Add(string.IsNullOrEmpty(mon.QuyenHan) ? "Không xác định" : mon.QuyenHan);
                item.Tag = mon;
                lstFoods.Items.Add(item);
            }

            // Cập nhật label hiển thị số lượng
            if (monAnList.Count == 0)
            {
                lblCount.Text = "Danh sách món ăn (0 món) - Database trống";
                lblCount.ForeColor = Color.FromArgb(255, 87, 34);
            }
            else
            {
                lblCount.Text = $"Danh sách món ăn ({monAnList.Count} món)";
                lblCount.ForeColor = Color.FromArgb(100, 100, 100);
            }

            // Hiển thị thông báo nếu chưa có người dùng
            if (nguoiDungList.Count == 0)
            {
                MessageBox.Show("Chưa có người dùng nào trong hệ thống.\nVui lòng thêm người dùng trước khi thêm món ăn.", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadFoodImage(string imageName)
        {
            try
            {
                string imagesFolder = dbHelper?.ImagesFolderPath ?? Path.Combine(AppContext.BaseDirectory, "Images");
                string imagePath = Path.Combine(imagesFolder, imageName);
                if (File.Exists(imagePath))
                {
                    picResultImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    // Tạo hình ảnh placeholder nếu không tìm thấy file
                    picResultImage.Image = CreatePlaceholderImage(imageName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải hình ảnh: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                picResultImage.Image = CreatePlaceholderImage(imageName);
            }
        }

        private Image CreatePlaceholderImage(string imageName)
        {
            var bitmap = new Bitmap(100, 100);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.LightGray);
                using (var font = new Font("Arial", 8))
                using (var brush = new SolidBrush(Color.DarkGray))
                {
                    string text = string.IsNullOrEmpty(imageName) ? "No Image" : imageName;
                    var textSize = g.MeasureString(text, font);
                    g.DrawString(text, font, brush, 
                        (100 - textSize.Width) / 2, 
                        (100 - textSize.Height) / 2);
                }
            }
            return bitmap;
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            using (var form = new AddUserForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (dbHelper.AddNguoiDung(form.HoVaTen, form.QuyenHan))
                    {
                        LoadData();
                        MessageBox.Show("Đã thêm người dùng thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra khi thêm người dùng", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnAddSampleData_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Thêm dữ liệu mẫu vào database?\nĐiều này sẽ thêm một số người dùng và món ăn mẫu.",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    dbHelper.AddSampleData();
                    LoadData();
                    MessageBox.Show("Đã thêm dữ liệu mẫu thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void FlashControl(Control control, Color flashColor)
        {
            Color originalColor = control.BackColor;
            control.BackColor = flashColor;
            await System.Threading.Tasks.Task.Delay(150);
            control.BackColor = originalColor;
        }
    }
}