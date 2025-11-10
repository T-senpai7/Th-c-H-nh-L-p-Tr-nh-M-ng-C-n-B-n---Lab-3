using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        private Button btnServer = null!;
        private Button btnClient = null!;
        private Label lblTitle = null!;
        private Panel panelMain = null!;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Bai5 - Hôm nay ăn gì?";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            lblTitle = new Label();
            lblTitle.Text = "BÀI 5: HÔM NAY ĂN GÌ?\n1 Server - Multi Client";
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(255, 87, 34);
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(480, 100);
            lblTitle.Location = new Point(10, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            panelMain = new Panel();
            panelMain.Size = new Size(480, 250);
            panelMain.Location = new Point(10, 140);
            panelMain.BackColor = Color.White;
            panelMain.BorderStyle = BorderStyle.FixedSingle;

            btnServer = new Button();
            btnServer.Text = "TCP Server";
            btnServer.Size = new Size(200, 80);
            btnServer.Location = new Point(30, 30);
            btnServer.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnServer.BackColor = Color.FromArgb(40, 167, 69);
            btnServer.ForeColor = Color.White;
            btnServer.FlatStyle = FlatStyle.Flat;
            btnServer.FlatAppearance.BorderSize = 0;
            btnServer.Cursor = Cursors.Hand;
            btnServer.Click += BtnServer_Click;

            btnClient = new Button();
            btnClient.Text = "TCP Client";
            btnClient.Size = new Size(200, 80);
            btnClient.Location = new Point(250, 30);
            btnClient.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnClient.BackColor = Color.FromArgb(33, 150, 243);
            btnClient.ForeColor = Color.White;
            btnClient.FlatStyle = FlatStyle.Flat;
            btnClient.FlatAppearance.BorderSize = 0;
            btnClient.Cursor = Cursors.Hand;
            btnClient.Click += BtnClient_Click;

            Label lblInfo = new Label();
            lblInfo.Text = "Chọn Server để quản lý database và chấp nhận kết nối từ clients.\nChọn Client để kết nối đến server và sử dụng ứng dụng.";
            lblInfo.Font = new Font("Segoe UI", 10F);
            lblInfo.ForeColor = Color.FromArgb(100, 100, 100);
            lblInfo.AutoSize = false;
            lblInfo.Size = new Size(440, 60);
            lblInfo.Location = new Point(20, 130);
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;

            panelMain.Controls.Add(btnServer);
            panelMain.Controls.Add(btnClient);
            panelMain.Controls.Add(lblInfo);

            this.Controls.Add(lblTitle);
            this.Controls.Add(panelMain);
        }

        private void BtnServer_Click(object? sender, EventArgs e)
        {
            var serverForm = new Bai5Server();
            serverForm.Show();
            btnServer.Enabled = false;
            serverForm.FormClosed += (s, args) => { btnServer.Enabled = true; };
        }

        private void BtnClient_Click(object? sender, EventArgs e)
        {
            var clientForm = new Bai5Client();
            clientForm.Show();
        }
    }
}

