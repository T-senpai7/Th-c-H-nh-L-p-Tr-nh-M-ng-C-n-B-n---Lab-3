using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp
{
	internal static class Program
	{
		private static List<Form> openForms = new List<Form>();

		[STAThread]
		public static void Main()
		{
			ApplicationConfiguration.Initialize();

			Form menu = new Form();
			menu.Text = "Bai6 - Chat Room TCP - Menu";
			menu.Size = new Size(520, 300);
			menu.StartPosition = FormStartPosition.CenterScreen;
			menu.FormBorderStyle = FormBorderStyle.FixedDialog;
			menu.MaximizeBox = false;
			menu.MinimizeBox = true;
			menu.BackColor = Color.FromArgb(245, 245, 245);

			Label lblTitle = new Label();
			lblTitle.Text = "BÀI 6 - CHAT ROOM TCP (MULTI-CLIENT)";
			lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
			lblTitle.Size = new Size(480, 50);
			lblTitle.Location = new Point(20, 20);
			lblTitle.TextAlign = ContentAlignment.MiddleCenter;
			lblTitle.ForeColor = Color.FromArgb(52, 58, 64);

			Button btnServer = new Button();
			btnServer.Text = "Mở TCP Server";
			btnServer.Size = new Size(200, 70);
			btnServer.Location = new Point(40, 90);
			btnServer.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
			btnServer.BackColor = Color.FromArgb(13, 110, 253);
			btnServer.ForeColor = Color.White;
			btnServer.FlatStyle = FlatStyle.Flat;
			btnServer.FlatAppearance.BorderSize = 0;
			btnServer.Cursor = Cursors.Hand;
			btnServer.Click += (s, e) =>
			{
				var f = new Bai6Server();
				f.Show();
				openForms.Add(f);
				f.FormClosing += (sender, args) => openForms.Remove(f);
			};

			Button btnClient = new Button();
			btnClient.Text = "Mở TCP Client";
			btnClient.Size = new Size(200, 70);
			btnClient.Location = new Point(260, 90);
			btnClient.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
			btnClient.BackColor = Color.FromArgb(40, 167, 69);
			btnClient.ForeColor = Color.White;
			btnClient.FlatStyle = FlatStyle.Flat;
			btnClient.FlatAppearance.BorderSize = 0;
			btnClient.Cursor = Cursors.Hand;
			btnClient.Click += (s, e) =>
			{
				var f = new Bai6Client();
				f.Show();
				openForms.Add(f);
				f.FormClosing += (sender, args) => openForms.Remove(f);
			};

			Button btnExit = new Button();
			btnExit.Text = "Thoát";
			btnExit.Size = new Size(120, 40);
			btnExit.Location = new Point(190, 190);
			btnExit.Font = new Font("Segoe UI", 10F);
			btnExit.BackColor = Color.FromArgb(220, 53, 69);
			btnExit.ForeColor = Color.White;
			btnExit.FlatStyle = FlatStyle.Flat;
			btnExit.FlatAppearance.BorderSize = 0;
			btnExit.Cursor = Cursors.Hand;
			btnExit.Click += (s, e) => Application.Exit();

			menu.Controls.Add(lblTitle);
			menu.Controls.Add(btnServer);
			menu.Controls.Add(btnClient);
			menu.Controls.Add(btnExit);

			Application.Run(menu);
		}
	}
}


