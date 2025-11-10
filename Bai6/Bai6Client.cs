using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp
{
	// Class để lưu thông tin tin nhắn
	public class ChatMessage
	{
		public string Sender { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
		public MessageType Type { get; set; }
		public bool IsPrivate { get; set; }
		public bool IsOwnMessage { get; set; }
		public string FileName { get; set; }
		public Image ImageData { get; set; }
		public string FilePath { get; set; }
	}

	public enum MessageType
	{
		Text,
		File,
		System
	}

	public class Bai6Client : Form
	{
		private TextBox txtIP;
		private TextBox txtPort;
		private TextBox txtUsername;
		private Button btnConnect;
		private Button btnDisconnect;

		private Panel pnlChat;
		private ListBox lstChat;
		private ListBox lstUsers;
		private TextBox txtMessage;
		private Button btnSendAll;
		private Button btnSendPriv;
		private Button btnSendFileAll;
		private Button btnSendFilePriv;
		private Label lblOnlineUsers;

		private TcpClient tcpClient;
		private NetworkStream networkStream;
		private Thread receiveThread;
		private volatile bool isConnected = false;

		private List<ChatMessage> messages = new List<ChatMessage>();

		public Bai6Client()
		{
			InitializeComponent();
			UpdateUI();
		}

		private void InitializeComponent()
		{
			Text = "Chat Client - Messenger Style";
			Size = new Size(1000, 700);
			StartPosition = FormStartPosition.CenterScreen;
			BackColor = Color.FromArgb(240, 242, 245);

			// === Header Panel ===
			Panel pnlHeader = new Panel
			{
				Location = new Point(0, 0),
				Size = new Size(1000, 60),
				BackColor = Color.White,
				Dock = DockStyle.Top
			};

			Label lblIP = new Label
			{
				Text = "Server IP:",
				Location = new Point(15, 18),
				Size = new Size(70, 24),
				Font = new Font("Segoe UI", 9F),
				ForeColor = Color.FromArgb(65, 65, 65)
			};

			txtIP = new TextBox
			{
				Text = "127.0.0.1",
				Location = new Point(90, 15),
				Size = new Size(120, 28),
				Font = new Font("Segoe UI", 9F),
				BorderStyle = BorderStyle.FixedSingle
			};

			Label lblPort = new Label
			{
				Text = "Port:",
				Location = new Point(220, 18),
				Size = new Size(35, 24),
				Font = new Font("Segoe UI", 9F),
				ForeColor = Color.FromArgb(65, 65, 65)
			};

			txtPort = new TextBox
			{
				Text = "8080",
				Location = new Point(260, 15),
				Size = new Size(70, 28),
				Font = new Font("Segoe UI", 9F),
				BorderStyle = BorderStyle.FixedSingle
			};

			Label lblUser = new Label
			{
				Text = "Tên:",
				Location = new Point(345, 18),
				Size = new Size(35, 24),
				Font = new Font("Segoe UI", 9F),
				ForeColor = Color.FromArgb(65, 65, 65)
			};

			txtUsername = new TextBox
			{
				Text = "User" + Environment.TickCount % 1000,
				Location = new Point(385, 15),
				Size = new Size(130, 28),
				Font = new Font("Segoe UI", 9F),
				BorderStyle = BorderStyle.FixedSingle
			};

			btnConnect = new Button
			{
				Text = "Kết nối",
				Location = new Point(530, 12),
				Size = new Size(90, 36),
				BackColor = Color.FromArgb(0, 132, 255),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 9F, FontStyle.Bold),
				Cursor = Cursors.Hand
			};
			btnConnect.FlatAppearance.BorderSize = 0;
			btnConnect.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 112, 235);
			btnConnect.Click += (s, e) => ConnectServer();

			btnDisconnect = new Button
			{
				Text = "Ngắt",
				Location = new Point(630, 12),
				Size = new Size(90, 36),
				BackColor = Color.FromArgb(255, 59, 48),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 9F, FontStyle.Bold),
				Cursor = Cursors.Hand
			};
			btnDisconnect.FlatAppearance.BorderSize = 0;
			btnDisconnect.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 39, 28);
			btnDisconnect.Click += (s, e) => Disconnect();

			pnlHeader.Controls.Add(lblIP);
			pnlHeader.Controls.Add(txtIP);
			pnlHeader.Controls.Add(lblPort);
			pnlHeader.Controls.Add(txtPort);
			pnlHeader.Controls.Add(lblUser);
			pnlHeader.Controls.Add(txtUsername);
			pnlHeader.Controls.Add(btnConnect);
			pnlHeader.Controls.Add(btnDisconnect);

			// === Main Container ===
			Panel pnlMain = new Panel
			{
				Location = new Point(0, 60),
				Size = new Size(1000, 640),
				BackColor = Color.FromArgb(240, 242, 245),
				Dock = DockStyle.Fill
			};

			// === Chat Panel ===
			pnlChat = new Panel
			{
				Location = new Point(15, 15),
				Size = new Size(680, 520),
				BackColor = Color.White,
				BorderStyle = BorderStyle.None
			};

			// Custom painted ListBox for chat messages
			lstChat = new ListBox
			{
				Location = new Point(0, 0),
				Size = new Size(680, 520),
				BorderStyle = BorderStyle.None,
				BackColor = Color.White,
				Font = new Font("Segoe UI", 9.5F),
				DrawMode = DrawMode.OwnerDrawVariable,
				SelectionMode = SelectionMode.None
			};
			lstChat.DrawItem += LstChat_DrawItem;
			lstChat.MeasureItem += LstChat_MeasureItem;

			pnlChat.Controls.Add(lstChat);

			// === Users Panel ===
			Panel pnlUsers = new Panel
			{
				Location = new Point(710, 15),
				Size = new Size(260, 520),
				BackColor = Color.White,
				BorderStyle = BorderStyle.None
			};

			lblOnlineUsers = new Label
			{
				Text = "Online Users",
				Location = new Point(10, 10),
				Size = new Size(240, 30),
				Font = new Font("Segoe UI", 10F, FontStyle.Bold),
				ForeColor = Color.FromArgb(65, 65, 65)
			};

			lstUsers = new ListBox
			{
				Location = new Point(10, 45),
				Size = new Size(240, 465),
				BorderStyle = BorderStyle.None,
				BackColor = Color.White,
				Font = new Font("Segoe UI", 9.5F),
				DrawMode = DrawMode.OwnerDrawFixed,
				ItemHeight = 40
			};
			lstUsers.DrawItem += LstUsers_DrawItem;

			pnlUsers.Controls.Add(lblOnlineUsers);
			pnlUsers.Controls.Add(lstUsers);

			// === Message Input Panel ===
			Panel pnlInput = new Panel
			{
				Location = new Point(15, 545),
				Size = new Size(680, 80),
				BackColor = Color.White,
				BorderStyle = BorderStyle.None
			};

			txtMessage = new TextBox
			{
				Location = new Point(15, 12),
				Size = new Size(495, 28),
				Font = new Font("Segoe UI", 10F),
				BorderStyle = BorderStyle.FixedSingle
			};
			txtMessage.KeyPress += (s, e) =>
			{
				if (e.KeyChar == (char)Keys.Enter)
				{
					e.Handled = true;
					SendAll();
				}
			};

			btnSendAll = new Button
			{
				Text = "Gửi All",
				Location = new Point(520, 12),
				Size = new Size(75, 32),
				BackColor = Color.FromArgb(0, 132, 255),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 9F, FontStyle.Bold),
				Cursor = Cursors.Hand,
				UseVisualStyleBackColor = false
			};
			btnSendAll.FlatAppearance.BorderSize = 0;
			btnSendAll.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 112, 235);
			btnSendAll.Click += (s, e) => SendAll();

			btnSendPriv = new Button
			{
				Text = "Riêng",
				Location = new Point(600, 12),
				Size = new Size(70, 32),
				BackColor = Color.FromArgb(0, 132, 255),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 9F, FontStyle.Bold),
				Cursor = Cursors.Hand,
				UseVisualStyleBackColor = false
			};
			btnSendPriv.FlatAppearance.BorderSize = 0;
			btnSendPriv.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 112, 235);
			btnSendPriv.Click += (s, e) => SendPriv();

			btnSendFileAll = new Button
			{
				Text = "File All",
				Location = new Point(520, 48),
				Size = new Size(75, 28),
				BackColor = Color.FromArgb(0, 132, 255),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
				Cursor = Cursors.Hand,
				UseVisualStyleBackColor = false
			};
			btnSendFileAll.FlatAppearance.BorderSize = 0;
			btnSendFileAll.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 112, 235);
			btnSendFileAll.Click += (s, e) => SendFileAll();

			btnSendFilePriv = new Button
			{
				Text = "File Riêng",
				Location = new Point(600, 48),
				Size = new Size(70, 28),
				BackColor = Color.FromArgb(0, 132, 255),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
				Cursor = Cursors.Hand,
				UseVisualStyleBackColor = false
			};
			btnSendFilePriv.FlatAppearance.BorderSize = 0;
			btnSendFilePriv.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 112, 235);
			btnSendFilePriv.Click += (s, e) => SendFilePriv();

			pnlInput.Controls.Add(txtMessage);
			pnlInput.Controls.Add(btnSendAll);
			pnlInput.Controls.Add(btnSendPriv);
			pnlInput.Controls.Add(btnSendFileAll);
			pnlInput.Controls.Add(btnSendFilePriv);

			pnlMain.Controls.Add(pnlChat);
			pnlMain.Controls.Add(pnlUsers);
			pnlMain.Controls.Add(pnlInput);

			Controls.Add(pnlHeader);
			Controls.Add(pnlMain);

			FormClosing += (s, e) => { if (isConnected) Disconnect(); };
		}

		// Custom draw for user list with avatars
		private void LstUsers_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0) return;

			e.DrawBackground();
			string userName = lstUsers.Items[e.Index].ToString();

			// Draw avatar circle
			using (SolidBrush avatarBrush = new SolidBrush(GetUserColor(userName)))
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				e.Graphics.FillEllipse(avatarBrush, e.Bounds.X + 5, e.Bounds.Y + 5, 30, 30);

				// Draw initial
				string initial = userName.Length > 0 ? userName.Substring(0, 1).ToUpper() : "?";
				using (Font font = new Font("Segoe UI", 12F, FontStyle.Bold))
				using (SolidBrush textBrush = new SolidBrush(Color.White))
				{
					SizeF textSize = e.Graphics.MeasureString(initial, font);
					float x = e.Bounds.X + 20 - textSize.Width / 2;
					float y = e.Bounds.Y + 20 - textSize.Height / 2;
					e.Graphics.DrawString(initial, font, textBrush, x, y);
				}
			}

			// Draw username
			using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(65, 65, 65)))
			using (Font font = new Font("Segoe UI", 9.5F))
			{
				e.Graphics.DrawString(userName, font, textBrush, e.Bounds.X + 45, e.Bounds.Y + 12);
			}
		}

		// Custom draw for chat messages (bubble style)
		private void LstChat_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= messages.Count) return;

			e.DrawBackground();
			ChatMessage msg = messages[e.Index];

			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			int padding = 10;
			int bubbleRadius = 18;
			int maxWidth = (int)(e.Bounds.Width * 0.65);
			int maxImageWidth = 250;
			int maxImageHeight = 250;

			if (msg.Type == MessageType.System)
			{
				// System message (centered)
				using (Font font = new Font("Segoe UI", 8.5F, FontStyle.Italic))
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(142, 142, 147)))
				{
					SizeF textSize = g.MeasureString(msg.Content, font, maxWidth);
					float x = e.Bounds.X + (e.Bounds.Width - textSize.Width) / 2;
					g.DrawString(msg.Content, font, brush, new RectangleF(x, e.Bounds.Y + 5, textSize.Width, textSize.Height));
				}
			}
			else
			{
				// Regular message bubble
				bool isOwn = msg.IsOwnMessage;
				Color bubbleColor = isOwn ? Color.FromArgb(0, 132, 255) : Color.FromArgb(233, 233, 235);
				Color textColor = isOwn ? Color.White : Color.Black;

				int bubbleWidth = 0;
				int bubbleHeight = 0;
				int contentHeight = 0;

				// Calculate bubble size based on content type
				if (msg.Type == MessageType.File && msg.ImageData != null)
				{
					// Image message - calculate image dimensions
					float scale = 1.0f;
					if (msg.ImageData.Width > maxImageWidth || msg.ImageData.Height > maxImageHeight)
					{
						float scaleW = (float)maxImageWidth / msg.ImageData.Width;
						float scaleH = (float)maxImageHeight / msg.ImageData.Height;
						scale = Math.Min(scaleW, scaleH);
					}
					
					int imgWidth = (int)(msg.ImageData.Width * scale);
					int imgHeight = (int)(msg.ImageData.Height * scale);
					
					bubbleWidth = imgWidth + padding * 2;
					bubbleHeight = imgHeight + padding * 2 + 20; // +20 for filename
					contentHeight = imgHeight;
				}
				else
				{
					// Text message
					string displayText = msg.Type == MessageType.File 
						? $"File: {msg.FileName}" 
						: msg.Content;

					using (Font font = new Font("Segoe UI", 9.5F))
					{
						SizeF textSize = g.MeasureString(displayText, font, maxWidth);
						bubbleWidth = (int)textSize.Width + padding * 2;
						bubbleHeight = (int)textSize.Height + padding * 2;
						contentHeight = (int)textSize.Height;
					}
				}

				int bubbleX = isOwn 
					? e.Bounds.Right - bubbleWidth - 50 
					: e.Bounds.X + 50;
				int bubbleY = e.Bounds.Y + 5;

				// Draw avatar
				using (SolidBrush avatarBrush = new SolidBrush(GetUserColor(msg.Sender)))
				{
					int avatarX = isOwn ? e.Bounds.Right - 35 : e.Bounds.X + 10;
					g.FillEllipse(avatarBrush, avatarX, bubbleY, 30, 30);

					string initial = msg.Sender.Length > 0 ? msg.Sender.Substring(0, 1).ToUpper() : "?";
					using (Font avatarFont = new Font("Segoe UI", 10F, FontStyle.Bold))
					using (SolidBrush textBrush = new SolidBrush(Color.White))
					{
						SizeF initialSize = g.MeasureString(initial, avatarFont);
						float ix = avatarX + 15 - initialSize.Width / 2;
						float iy = bubbleY + 15 - initialSize.Height / 2;
						g.DrawString(initial, avatarFont, textBrush, ix, iy);
					}
				}

				// Draw bubble
				using (GraphicsPath path = GetRoundedRectPath(new Rectangle(bubbleX, bubbleY, bubbleWidth, bubbleHeight), bubbleRadius))
				using (SolidBrush bubbleBrush = new SolidBrush(bubbleColor))
				{
					g.FillPath(bubbleBrush, path);
				}

				// Draw content (image or text)
				if (msg.Type == MessageType.File && msg.ImageData != null)
				{
					// Draw image
					float scale = 1.0f;
					if (msg.ImageData.Width > maxImageWidth || msg.ImageData.Height > maxImageHeight)
					{
						float scaleW = (float)maxImageWidth / msg.ImageData.Width;
						float scaleH = (float)maxImageHeight / msg.ImageData.Height;
						scale = Math.Min(scaleW, scaleH);
					}
					
					int imgWidth = (int)(msg.ImageData.Width * scale);
					int imgHeight = (int)(msg.ImageData.Height * scale);

					Rectangle imgRect = new Rectangle(bubbleX + padding, bubbleY + padding, imgWidth, imgHeight);
					
					// Clip to rounded rectangle
					using (GraphicsPath clipPath = GetRoundedRectPath(imgRect, bubbleRadius - 5))
					{
						g.SetClip(clipPath);
						g.DrawImage(msg.ImageData, imgRect);
						g.ResetClip();
					}

					// Draw filename below image
					using (Font smallFont = new Font("Segoe UI", 7.5F))
					using (SolidBrush filenameBrush = new SolidBrush(textColor))
					{
						g.DrawString(msg.FileName, smallFont, filenameBrush, bubbleX + padding, bubbleY + padding + imgHeight + 2);
					}
				}
				else
				{
					// Draw text message
					string displayText = msg.Type == MessageType.File 
						? $"File: {msg.FileName}" 
						: msg.Content;

					using (Font font = new Font("Segoe UI", 9.5F))
					using (SolidBrush textBrush = new SolidBrush(textColor))
					{
						g.DrawString(displayText, font, textBrush, new RectangleF(bubbleX + padding, bubbleY + padding, maxWidth, contentHeight));
					}
				}

				// Draw sender name and time
				string timeText = msg.Timestamp.ToString("HH:mm");
				string senderInfo = isOwn ? timeText : $"{msg.Sender} · {timeText}";
				using (Font smallFont = new Font("Segoe UI", 7.5F))
				using (SolidBrush timeBrush = new SolidBrush(Color.FromArgb(142, 142, 147)))
				{
					int infoY = bubbleY + bubbleHeight + 2;
					int infoX = isOwn ? bubbleX + bubbleWidth - 40 : bubbleX;
					g.DrawString(senderInfo, smallFont, timeBrush, infoX, infoY);
				}

				// Draw private badge
				if (msg.IsPrivate)
				{
					using (Font badgeFont = new Font("Segoe UI", 7F, FontStyle.Bold))
					using (SolidBrush badgeBrush = new SolidBrush(Color.FromArgb(88, 86, 214)))
					{
						g.DrawString("RIENG", badgeFont, badgeBrush, bubbleX + 5, bubbleY - 12);
					}
				}
			}
		}

		private void LstChat_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= messages.Count)
			{
				e.ItemHeight = 30;
				return;
			}

			ChatMessage msg = messages[e.Index];

			if (msg.Type == MessageType.System)
			{
				e.ItemHeight = 25;
			}
			else if (msg.Type == MessageType.File && msg.ImageData != null)
			{
				// Image message - calculate height based on scaled image
				int maxImageWidth = 250;
				int maxImageHeight = 250;
				
				float scale = 1.0f;
				if (msg.ImageData.Width > maxImageWidth || msg.ImageData.Height > maxImageHeight)
				{
					float scaleW = (float)maxImageWidth / msg.ImageData.Width;
					float scaleH = (float)maxImageHeight / msg.ImageData.Height;
					scale = Math.Min(scaleW, scaleH);
				}
				
				int imgHeight = (int)(msg.ImageData.Height * scale);
				e.ItemHeight = imgHeight + 70; // +70 for padding, filename, timestamp
			}
			else
			{
				// Text message
				string displayText = msg.Type == MessageType.File 
					? $"File: {msg.FileName}" 
					: msg.Content;

				using (Font font = new Font("Segoe UI", 9.5F))
				{
					int maxWidth = (int)(lstChat.Width * 0.65);
					SizeF textSize = e.Graphics.MeasureString(displayText, font, maxWidth);
					e.ItemHeight = (int)textSize.Height + 50;
				}
			}
		}

		private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
		{
			GraphicsPath path = new GraphicsPath();
			int diameter = radius * 2;

			path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
			path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
			path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
			path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
			path.CloseFigure();

			return path;
		}

		private Color GetUserColor(string username)
		{
			Color[] colors = new Color[]
			{
				Color.FromArgb(255, 59, 48),   // Red
				Color.FromArgb(255, 149, 0),   // Orange
				Color.FromArgb(255, 204, 0),   // Yellow
				Color.FromArgb(52, 199, 89),   // Green
				Color.FromArgb(0, 199, 190),   // Teal
				Color.FromArgb(0, 132, 255),   // Blue
				Color.FromArgb(88, 86, 214),   // Purple
				Color.FromArgb(255, 45, 85)    // Pink
			};

			int hash = 0;
			foreach (char c in username)
			{
				hash = hash * 31 + c;
			}
			return colors[Math.Abs(hash) % colors.Length];
		}

		private void UpdateUI()
		{
			txtIP.Enabled = !isConnected;
			txtPort.Enabled = !isConnected;
			txtUsername.Enabled = !isConnected;
			btnConnect.Enabled = !isConnected;
			btnDisconnect.Enabled = isConnected;
			txtMessage.Enabled = isConnected;
			btnSendAll.Enabled = isConnected;
			btnSendPriv.Enabled = isConnected;
			btnSendFileAll.Enabled = isConnected;
			btnSendFilePriv.Enabled = isConnected;
		}

		private void AddChatMessage(ChatMessage msg)
		{
			messages.Add(msg);
			lstChat.Items.Add(msg);
			lstChat.TopIndex = lstChat.Items.Count - 1;
		}

		private void ConnectServer()
		{
			try
			{
				if (!IPAddress.TryParse(txtIP.Text.Trim(), out IPAddress ip))
				{
					MessageBox.Show("IP không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				if (!int.TryParse(txtPort.Text.Trim(), out int port))
				{
					MessageBox.Show("Port không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				string username = txtUsername.Text.Trim();
				if (string.IsNullOrWhiteSpace(username))
				{
					MessageBox.Show("Vui lòng nhập tên", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				tcpClient = new TcpClient();
				tcpClient.Connect(new IPEndPoint(ip, port));
				networkStream = tcpClient.GetStream();
				isConnected = true;
				UpdateUI();

				receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
				receiveThread.Start();

				SendLine($"JOIN|{username}");
				AddChatMessage(new ChatMessage
				{
					Content = "Đã kết nối server",
					Timestamp = DateTime.Now,
					Type = MessageType.System
				});
			}
			catch (Exception ex)
			{
				isConnected = false;
				UpdateUI();
				MessageBox.Show($"Kết nối thất bại: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Disconnect()
		{
			isConnected = false;
			try { networkStream?.Close(); } catch { }
			try { tcpClient?.Close(); } catch { }
			UpdateUI();
			AddChatMessage(new ChatMessage
			{
				Content = "Đã ngắt kết nối",
				Timestamp = DateTime.Now,
				Type = MessageType.System
			});
		}

		private void ReceiveLoop()
		{
			byte[] buffer = new byte[8192];
			StringBuilder sb = new StringBuilder();
			try
			{
				while (isConnected)
				{
					int n = networkStream.Read(buffer, 0, buffer.Length);
					if (n <= 0) break;
					string data = Encoding.UTF8.GetString(buffer, 0, n);
					sb.Append(data);
					while (true)
					{
						string acc = sb.ToString();
						int idx = acc.IndexOf("\n", StringComparison.Ordinal);
						if (idx < 0) break;
						string line = acc.Substring(0, idx);
						sb.Remove(0, idx + 1);
						ProcessServerLine(line);
					}
				}
			}
			catch
			{
			}
			finally
			{
				if (isConnected)
				{
					Invoke(new Action(() =>
					{
						Disconnect();
						MessageBox.Show("Mất kết nối với server", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}));
				}
			}
		}

		private void ProcessServerLine(string line)
		{
			if (InvokeRequired)
			{
				Invoke(new Action<string>(ProcessServerLine), line);
				return;
			}
			string[] parts = line.Split('|');
			if (parts.Length == 0) return;
			string cmd = parts[0];

			switch (cmd)
			{
				case "JOIN_OK":
					AddChatMessage(new ChatMessage
					{
						Content = "Đăng nhập phòng chat thành công",
						Timestamp = DateTime.Now,
						Type = MessageType.System
					});
					break;
				case "ERR":
					AddChatMessage(new ChatMessage
					{
						Content = "Lỗi: " + (parts.Length > 1 ? parts[1] : "Không xác định"),
						Timestamp = DateTime.Now,
						Type = MessageType.System
					});
					break;
				case "SYS":
					if (parts.Length > 1)
					{
						AddChatMessage(new ChatMessage
						{
							Content = parts[1],
							Timestamp = DateTime.Now,
							Type = MessageType.System
						});
					}
					break;
				case "USERS":
					if (parts.Length > 1)
					{
						lstUsers.Items.Clear();
						var users = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
						foreach (var u in users) lstUsers.Items.Add(u);
					}
					break;
				case "MSG":
					// MSG|sender|text
					if (parts.Length >= 3)
					{
						AddChatMessage(new ChatMessage
						{
							Sender = parts[1],
							Content = parts[2],
							Timestamp = DateTime.Now,
							Type = MessageType.Text,
							IsPrivate = false,
							IsOwnMessage = parts[1] == txtUsername.Text.Trim()
						});
					}
					break;
				case "MSG_PRIV":
					// MSG_PRIV|sender|text
					if (parts.Length >= 3)
					{
						AddChatMessage(new ChatMessage
						{
							Sender = parts[1],
							Content = parts[2],
							Timestamp = DateTime.Now,
							Type = MessageType.Text,
							IsPrivate = true,
							IsOwnMessage = parts[1] == txtUsername.Text.Trim()
						});
					}
					break;
				case "FILE":
					// FILE|sender|filename|mime|base64
					if (parts.Length >= 5) HandleIncomingFile(parts[1], parts[2], parts[3], parts[4], false);
					break;
				case "FILE_PRIV":
					// FILE_PRIV|sender|filename|mime|base64
					if (parts.Length >= 5) HandleIncomingFile(parts[1], parts[2], parts[3], parts[4], true);
					break;
			}
		}

		private void HandleIncomingFile(string sender, string filename, string mime, string base64, bool isPriv)
		{
			try
			{
				byte[] bytes = Convert.FromBase64String(base64);
				string folder = Path.Combine(AppContext.BaseDirectory, "Downloads");
				Directory.CreateDirectory(folder);
				string safeName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{sender}_{Path.GetFileName(filename)}";
				string path = Path.Combine(folder, safeName);
				File.WriteAllBytes(path, bytes);

				// Check if it's an image
				Image imageData = null;
				string ext = Path.GetExtension(filename).ToLowerInvariant();
				if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
				{
					try
					{
						using (MemoryStream ms = new MemoryStream(bytes))
						{
							imageData = Image.FromStream(ms);
						}
					}
					catch
					{
						// If image loading fails, just show as file
						imageData = null;
					}
				}

				AddChatMessage(new ChatMessage
				{
					Sender = sender,
					Content = $"Đã lưu tại: {path}",
					FileName = filename,
					Timestamp = DateTime.Now,
					Type = MessageType.File,
					IsPrivate = isPriv,
					IsOwnMessage = sender == txtUsername.Text.Trim(),
					ImageData = imageData,
					FilePath = path
				});
			}
			catch (Exception ex)
			{
				AddChatMessage(new ChatMessage
				{
					Content = "Lỗi nhận file: " + ex.Message,
					Timestamp = DateTime.Now,
					Type = MessageType.System
				});
			}
		}

		private void SendAll()
		{
			if (!isConnected) return;
			string text = txtMessage.Text.Trim();
			if (string.IsNullOrEmpty(text)) return;
			string sender = txtUsername.Text.Trim();
			SendLine($"MSG_ALL|{sender}|{text}");
			txtMessage.Clear();
		}

		private void SendPriv()
		{
			if (!isConnected) return;
			if (lstUsers.SelectedItem == null)
			{
				MessageBox.Show("Chọn một user ở danh sách để nhắn riêng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			string toUser = lstUsers.SelectedItem.ToString();
			string text = txtMessage.Text.Trim();
			if (string.IsNullOrEmpty(text)) return;
			string sender = txtUsername.Text.Trim();
			SendLine($"MSG_PRIV|{sender}|{toUser}|{text}");
			txtMessage.Clear();
		}

		private void SendFileAll()
		{
			if (!isConnected) return;
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "Hình ảnh hoặc TXT|*.jpg;*.jpeg;*.png;*.txt";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					SendFile(ofd.FileName, broadcast: true);
				}
			}
		}

		private void SendFilePriv()
		{
			if (!isConnected) return;
			if (lstUsers.SelectedItem == null)
			{
				MessageBox.Show("Chọn một user ở danh sách để gửi file riêng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "Hình ảnh hoặc TXT|*.jpg;*.jpeg;*.png;*.txt";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					SendFile(ofd.FileName, broadcast: false, toUser: lstUsers.SelectedItem.ToString());
				}
			}
		}

		private void SendFile(string path, bool broadcast, string toUser = "")
		{
			try
			{
				string ext = Path.GetExtension(path).ToLowerInvariant();
				string mime = ext switch
				{
					".jpg" => "image/jpeg",
					".jpeg" => "image/jpeg",
					".png" => "image/png",
					".txt" => "text/plain",
					_ => "application/octet-stream"
				};
				byte[] bytes = File.ReadAllBytes(path);
				string base64 = Convert.ToBase64String(bytes);
				string sender = txtUsername.Text.Trim();
				string filename = Path.GetFileName(path);
				if (broadcast)
				{
					SendLine($"FILE_ALL|{sender}|{filename}|{mime}|{base64}");
				}
				else
				{
					SendLine($"FILE_PRIV|{sender}|{toUser}|{filename}|{mime}|{base64}");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi gửi file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SendLine(string line)
		{
			if (!isConnected || networkStream == null) return;
			try
			{
				byte[] data = Encoding.UTF8.GetBytes(line + "\n");
				networkStream.Write(data, 0, data.Length);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi gửi dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}