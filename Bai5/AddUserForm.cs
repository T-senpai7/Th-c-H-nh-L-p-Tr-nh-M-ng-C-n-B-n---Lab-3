using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public class AddUserForm : Form
    {
        private TextBox txtHoVaTen;
        private ComboBox cmbQuyenHan;
        private Button btnOK;
        private Button btnCancel;
        private Label lblHoVaTen;
        private Label lblQuyenHan;

        public string HoVaTen { get; private set; }
        public string QuyenHan { get; private set; }

        public AddUserForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Thêm người dùng";
            Size = new Size(400, 200);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            lblHoVaTen = new Label
            {
                Text = "Họ và tên:",
                Location = new Point(20, 20),
                Size = new Size(100, 25)
            };

            txtHoVaTen = new TextBox
            {
                Location = new Point(130, 18),
                Size = new Size(230, 30),
                Font = new Font("Segoe UI", 10)
            };

            lblQuyenHan = new Label
            {
                Text = "Quyền hạn:",
                Location = new Point(20, 60),
                Size = new Size(100, 25)
            };

            cmbQuyenHan = new ComboBox
            {
                Location = new Point(130, 58),
                Size = new Size(230, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbQuyenHan.Items.AddRange(new[] { "Admin", "User" });
            cmbQuyenHan.SelectedIndex = 1;

            btnOK = new Button
            {
                Text = "OK",
                Location = new Point(180, 110),
                Size = new Size(80, 35),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(280, 110),
                Size = new Size(80, 35),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(100, 100, 100),
                FlatStyle = FlatStyle.Flat
            };

            Controls.Add(lblHoVaTen);
            Controls.Add(txtHoVaTen);
            Controls.Add(lblQuyenHan);
            Controls.Add(cmbQuyenHan);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);

            AcceptButton = btnOK;
            CancelButton = btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoVaTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            if (cmbQuyenHan.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn quyền hạn", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            HoVaTen = txtHoVaTen.Text.Trim();
            QuyenHan = cmbQuyenHan.SelectedItem.ToString();
        }
    }
}

