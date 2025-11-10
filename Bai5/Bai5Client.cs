using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Bai5Client : Form
    {
        // Network
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private Thread receiveThread;
        private bool isConnected = false;
        private string serverIP = "127.0.0.1";
        private int serverPort = 8080;
        private int currentUserId = -1;

        // UI Components
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
        private Panel pnlConnection;
        private TextBox txtServerIP;
        private TextBox txtServerPort;
        private Button btnConnect;
        private Button btnDisconnect;
        private Label lblServerIP;
        private Label lblServerPort;
        private RadioButton rdbRandomAll;
        private RadioButton rdbRandomUser;
        private Panel pnlRandomOptions;

        // Data
        private List<MonAnInfo> monAnList;
        private List<NguoiDungInfo> nguoiDungList;

        public Bai5Client()
        {
            monAnList = new List<MonAnInfo>();
            nguoiDungList = new List<NguoiDungInfo>();
            InitializeComponent();
            UpdateConnectionUI();
        }

        private void InitializeComponent()
        {
            Text = "Hôm nay ăn gì? - Client";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(700, 800);
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Padding = new Padding(0);

            // Connection Panel
            pnlConnection = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(700, 80),
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblServerIP = new Label
            {
                Text = "Server IP:",
                Location = new Point(20, 15),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };

            txtServerIP = new TextBox
            {
                Text = "127.0.0.1",
                Location = new Point(110, 12),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10)
            };

            lblServerPort = new Label
            {
                Text = "Port:",
                Location = new Point(280, 15),
                Size = new Size(50, 25),
                Font = new Font("Segoe UI", 10)
            };

            txtServerPort = new TextBox
            {
                Text = "8080",
                Location = new Point(340, 12),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 10)
            };

            btnConnect = new Button
            {
                Text = "Kết nối",
                Location = new Point(440, 10),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Click += BtnConnect_Click;

            btnDisconnect = new Button
            {
                Text = "Ngắt kết nối",
                Location = new Point(550, 10),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDisconnect.FlatAppearance.BorderSize = 0;
            btnDisconnect.Click += BtnDisconnect_Click;

            pnlConnection.Controls.Add(lblServerIP);
            pnlConnection.Controls.Add(txtServerIP);
            pnlConnection.Controls.Add(lblServerPort);
            pnlConnection.Controls.Add(txtServerPort);
            pnlConnection.Controls.Add(btnConnect);
            pnlConnection.Controls.Add(btnDisconnect);

            panelHeader = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(700, 100),
                BackColor = Color.FromArgb(255, 87, 34)
            };

            lblTitle = new Label
            {
                Text = "🍽️ HÔM NAY ĂN GÌ?",
                Location = new Point(0, 30),
                Size = new Size(700, 40),
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelHeader.Controls.Add(lblTitle);

            panelMain = new Panel
            {
                Location = new Point(30, 200),
                Size = new Size(640, 560),
                BackColor = Color.White
            };

            lblInstruction = new Label
            {
                Text = "Thêm món ăn yêu thích",
                Location = new Point(0, 0),
                Size = new Size(640, 25),
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

            // Random options
            pnlRandomOptions = new Panel
            {
                Location = new Point(0, 115),
                Size = new Size(640, 30),
                BackColor = Color.White
            };

            rdbRandomAll = new RadioButton
            {
                Text = "Chọn từ tất cả món ăn",
                Location = new Point(0, 5),
                Size = new Size(200, 25),
                Checked = true,
                Font = new Font("Segoe UI", 9)
            };

            rdbRandomUser = new RadioButton
            {
                Text = "Chọn từ món ăn của tôi",
                Location = new Point(220, 5),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9)
            };

            pnlRandomOptions.Controls.Add(rdbRandomAll);
            pnlRandomOptions.Controls.Add(rdbRandomUser);

            btnRandom = new Button
            {
                Text = "🎲 CHỌN MÓN NGẪU NHIÊN",
                Location = new Point(0, 155),
                Size = new Size(640, 55),
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
                Location = new Point(0, 230),
                Size = new Size(640, 120),
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
                Size = new Size(510, 50),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 87, 34),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblResultContributor = new Label
            {
                Text = "",
                Location = new Point(120, 60),
                Size = new Size(510, 30),
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
                Location = new Point(0, 370),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            btnReset = new Button
            {
                Text = "↻ Refresh",
                Location = new Point(550, 367),
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
                Location = new Point(0, 405),
                Size = new Size(640, 155),
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
            panelMain.Controls.Add(pnlRandomOptions);
            panelMain.Controls.Add(btnRandom);
            panelMain.Controls.Add(panelResult);
            panelMain.Controls.Add(lblCount);
            panelMain.Controls.Add(btnReset);
            panelMain.Controls.Add(lstFoods);

            Controls.Add(pnlConnection);
            Controls.Add(panelHeader);
            Controls.Add(panelMain);
        }

        private void UpdateConnectionUI()
        {
            txtServerIP.Enabled = !isConnected;
            txtServerPort.Enabled = !isConnected;
            btnConnect.Enabled = !isConnected;
            btnDisconnect.Enabled = isConnected;
            btnAdd.Enabled = isConnected;
            btnRandom.Enabled = isConnected;
            btnAddUser.Enabled = isConnected;
            btnAddSampleData.Enabled = isConnected;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                serverIP = txtServerIP.Text.Trim();
                if (!int.TryParse(txtServerPort.Text.Trim(), out serverPort))
                {
                    MessageBox.Show("Port không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                tcpClient = new TcpClient();
                IPAddress ipAddress = IPAddress.Parse(serverIP);
                IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, serverPort);
                
                tcpClient.Connect(iPEndPoint);
                networkStream = tcpClient.GetStream();
                isConnected = true;

                receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();

                UpdateConnectionUI();
                MessageBox.Show("Đã kết nối đến server thành công!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Request users và foods
                RequestUsers();
                RequestAllFoods();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể kết nối đến server: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                isConnected = false;
                UpdateConnectionUI();
            }
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            isConnected = false;
            try
            {
                if (networkStream != null)
                {
                    networkStream.Close();
                }
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
            }
            catch { }

            UpdateConnectionUI();
            MessageBox.Show("Đã ngắt kết nối", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[4096];
            StringBuilder messageBuilder = new StringBuilder();

            while (isConnected)
            {
                try
                {
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    messageBuilder.Append(data);

                    while (messageBuilder.ToString().Contains("\n"))
                    {
                        int newlineIndex = messageBuilder.ToString().IndexOf("\n");
                        string message = messageBuilder.ToString().Substring(0, newlineIndex);
                        messageBuilder.Remove(0, newlineIndex + 1);

                        ProcessServerMessage(message);
                    }
                }
                catch
                {
                    break;
                }
            }

            if (isConnected)
            {
                Invoke(new Action(() =>
                {
                    Disconnect();
                    MessageBox.Show("Mất kết nối với server", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }));
            }
        }

        private void ProcessServerMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(ProcessServerMessage), message);
                return;
            }

            string[] parts = message.Split('|');
            if (parts.Length == 0) return;

            string command = parts[0];

            switch (command)
            {
                case "ALL_FOODS":
                    if (parts.Length > 1)
                    {
                        ParseFoods(parts[1], true);
                    }
                    break;

                case "USER_FOODS":
                    if (parts.Length > 1)
                    {
                        ParseFoods(parts[1], false);
                    }
                    break;

                case "USERS":
                    if (parts.Length > 1)
                    {
                        ParseUsers(parts[1]);
                    }
                    break;

                case "RANDOM_FOOD":
                    if (parts.Length > 1 && parts[1] != "NONE")
                    {
                        ParseRandomFood(parts[1]);
                    }
                    else
                    {
                        MessageBox.Show("Không có món ăn nào", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;

                case "ADD_FOOD_SUCCESS":
                    RequestAllFoods();
                    txtNewFood.Clear();
                    txtHinhAnh.Clear();
                    MessageBox.Show("Đã thêm món ăn thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case "ADD_FOOD_ERROR":
                    MessageBox.Show($"Lỗi: {(parts.Length > 1 ? parts[1] : "Không xác định")}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case "ADD_USER_SUCCESS":
                    RequestUsers();
                    MessageBox.Show("Đã thêm người dùng thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case "ADD_USER_ERROR":
                    MessageBox.Show($"Lỗi: {(parts.Length > 1 ? parts[1] : "Không xác định")}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case "UPDATE_FOODS":
                    RequestAllFoods();
                    break;

                case "UPDATE_USERS":
                    RequestUsers();
                    break;

                case "ERROR":
                    MessageBox.Show($"Lỗi từ server: {(parts.Length > 1 ? parts[1] : "Không xác định")}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void ParseFoods(string data, bool isAll)
        {
            monAnList.Clear();
            if (string.IsNullOrEmpty(data)) return;

            string[] foods = data.Split(';');
            foreach (string foodStr in foods)
            {
                if (string.IsNullOrEmpty(foodStr)) continue;
                string[] parts = foodStr.Split(':');
                if (parts.Length >= 6)
                {
                    monAnList.Add(new MonAnInfo
                    {
                        IDMA = int.Parse(parts[0]),
                        TenMonAn = parts[1],
                        HinhAnh = parts[2],
                        IDNCC = int.Parse(parts[3]),
                        HoVaTen = parts[4],
                        QuyenHan = parts[5]
                    });
                }
            }

            UpdateFoodsList();
        }

        private void ParseUsers(string data)
        {
            nguoiDungList.Clear();
            if (string.IsNullOrEmpty(data)) return;

            string[] users = data.Split(';');
            foreach (string userStr in users)
            {
                if (string.IsNullOrEmpty(userStr)) continue;
                string[] parts = userStr.Split(':');
                if (parts.Length >= 3)
                {
                    nguoiDungList.Add(new NguoiDungInfo
                    {
                        IDNCC = int.Parse(parts[0]),
                        HoVaTen = parts[1],
                        QuyenHan = parts[2]
                    });
                }
            }

            cmbNguoiDung.DataSource = null;
            cmbNguoiDung.DataSource = nguoiDungList;
            cmbNguoiDung.DisplayMember = "HoVaTen";
            cmbNguoiDung.ValueMember = "IDNCC";

            if (cmbNguoiDung.Items.Count > 0)
            {
                cmbNguoiDung.SelectedIndex = 0;
                currentUserId = ((NguoiDungInfo)cmbNguoiDung.SelectedItem).IDNCC;
                SendMessage($"SET_USER|{currentUserId}");
            }
        }

        private void ParseRandomFood(string data)
        {
            string[] parts = data.Split(':');
            if (parts.Length >= 6)
            {
                var food = new MonAnInfo
                {
                    IDMA = int.Parse(parts[0]),
                    TenMonAn = parts[1],
                    HinhAnh = parts[2],
                    IDNCC = int.Parse(parts[3]),
                    HoVaTen = parts[4],
                    QuyenHan = parts[5]
                };

                lblResultFood.Text = food.TenMonAn;
                lblResultContributor.Text = $"Đóng góp bởi: {food.HoVaTen} ({food.QuyenHan})";

                if (!string.IsNullOrEmpty(food.HinhAnh))
                {
                    LoadFoodImage(food.HinhAnh);
                }
                else
                {
                    picResultImage.Image = null;
                }

                panelResult.BackColor = Color.FromArgb(255, 248, 225);

                foreach (ListViewItem item in lstFoods.Items)
                {
                    if (item.SubItems[1].Text == food.TenMonAn)
                    {
                        item.Selected = true;
                        item.EnsureVisible();
                        break;
                    }
                }
            }
        }

        private void UpdateFoodsList()
        {
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
        }

        private void LoadFoodImage(string imageName)
        {
            try
            {
                // Try to load from Images folder
                string imagePath = Path.Combine(AppContext.BaseDirectory, "Images", imageName);
                if (File.Exists(imagePath))
                {
                    picResultImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    picResultImage.Image = CreatePlaceholderImage(imageName);
                }
            }
            catch
            {
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

        private void SendMessage(string message)
        {
            if (!isConnected || networkStream == null) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                networkStream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi gửi tin nhắn: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RequestAllFoods()
        {
            SendMessage("GET_ALL_FOODS|");
        }

        private void RequestUsers()
        {
            SendMessage("GET_USERS|");
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
            if (!isConnected)
            {
                MessageBox.Show("Vui lòng kết nối đến server trước", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
            SendMessage($"ADD_FOOD|{monAnMoi}|{hinhAnh}|{selectedUser.IDNCC}");
        }

        private void BtnRandom_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Vui lòng kết nối đến server trước", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblResultFood.Text = "🎲";
            lblResultContributor.Text = "";
            picResultImage.Image = null;
            panelResult.BackColor = Color.FromArgb(255, 235, 59);
            Application.DoEvents();
            System.Threading.Thread.Sleep(500);

            if (rdbRandomAll.Checked)
            {
                SendMessage("RANDOM_FOOD_ALL|");
            }
            else
            {
                SendMessage("RANDOM_FOOD_USER|");
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                RequestAllFoods();
                RequestUsers();
            }
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Vui lòng kết nối đến server trước", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = new AddUserForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    SendMessage($"ADD_USER|{form.HoVaTen}|{form.QuyenHan}");
                }
            }
        }

        private void BtnAddSampleData_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng này chỉ có trên server. Vui lòng thêm dữ liệu mẫu từ server.", 
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isConnected)
            {
                Disconnect();
            }
            base.OnFormClosing(e);
        }
    }
}

