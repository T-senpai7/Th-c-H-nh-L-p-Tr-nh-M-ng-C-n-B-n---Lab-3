using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp
{
	public class Bai6Server : Form
	{
		private TextBox txtPort;
		private Button btnStart;
		private Button btnStop;
		private ListView lstLog;
		private ListView lstUsers;

		private Socket listenerSocket;
		private Thread acceptThread;
		private volatile bool isRunning = false;

		private class ClientCtx
		{
			public Socket Socket;
			public Thread Thread;
			public string Username;
			public StringBuilder Buffer = new StringBuilder();
		}

		private readonly object clientsLock = new object();
		private readonly Dictionary<string, ClientCtx> usernameToClient = new Dictionary<string, ClientCtx>(StringComparer.OrdinalIgnoreCase);
		private readonly List<ClientCtx> allClients = new List<ClientCtx>();

		public Bai6Server()
		{
			InitializeComponent();
			btnStop.Enabled = false;
		}

		private void InitializeComponent()
		{
			Text = "Bai6 - TCP Chat Server";
			Size = new Size(900, 650);
			StartPosition = FormStartPosition.CenterScreen;

			Label lblPort = new Label
			{
				Text = "Port:",
				Location = new Point(15, 15),
				Size = new Size(40, 24)
			};
			txtPort = new TextBox
			{
				Text = "8080",
				Location = new Point(60, 12),
				Size = new Size(100, 28)
			};
			btnStart = new Button
			{
				Text = "Start",
				Location = new Point(180, 10),
				Size = new Size(100, 32),
				BackColor = Color.FromArgb(40, 167, 69),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat
			};
			btnStart.FlatAppearance.BorderSize = 0;
			btnStart.Click += (s, e) => StartServer();

			btnStop = new Button
			{
				Text = "Stop",
				Location = new Point(290, 10),
				Size = new Size(100, 32),
				BackColor = Color.FromArgb(220, 53, 69),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat
			};
			btnStop.FlatAppearance.BorderSize = 0;
			btnStop.Click += (s, e) => StopServer();

			lstLog = new ListView
			{
				Location = new Point(15, 55),
				Size = new Size(650, 540),
				View = View.Details,
				FullRowSelect = true,
				GridLines = true
			};
			lstLog.Columns.Add("Time", 120);
			lstLog.Columns.Add("Log", 510);

			lstUsers = new ListView
			{
				Location = new Point(680, 55),
				Size = new Size(190, 540),
				View = View.Details,
				FullRowSelect = true,
				GridLines = true
			};
			lstUsers.Columns.Add("Users", 160);

			Controls.Add(lblPort);
			Controls.Add(txtPort);
			Controls.Add(btnStart);
			Controls.Add(btnStop);
			Controls.Add(lstLog);
			Controls.Add(lstUsers);

			FormClosing += (s, e) =>
			{
				if (isRunning) StopServer();
			};
		}

		private void StartServer()
		{
			if (isRunning) return;
			if (!int.TryParse(txtPort.Text.Trim(), out int port) || port <= 0 || port > 65535)
			{
				MessageBox.Show("Port không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			try
			{
				isRunning = true;
				btnStart.Enabled = false;
				btnStop.Enabled = true;

				// Thử bind nhiều lần nếu port đang được sử dụng
				const int maxAttempts = 10;
				int attempt = 0;
				Socket tempListener = null;
				Socket boundSocket = null;
				int boundPort = port;
				while (attempt < maxAttempts)
				{
					try
					{
						tempListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						// Cho phép tái sử dụng địa chỉ để giảm lỗi TIME_WAIT
						tempListener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
						tempListener.Bind(new IPEndPoint(IPAddress.Any, boundPort));
						tempListener.Listen(100);
						boundSocket = tempListener;
						break;
					}
					catch (SocketException se) when (se.SocketErrorCode == SocketError.AddressAlreadyInUse)
					{
						// Tự động thử port tiếp theo
						attempt++;
						boundPort++;
						try { tempListener?.Close(); } catch { }
						tempListener = null;
						continue;
					}
				}

				if (boundSocket == null)
				{
					throw new SocketException((int)SocketError.AddressAlreadyInUse);
				}

				listenerSocket = boundSocket;
				if (boundPort != port)
				{
					Log($"Port {port} đã bận. Tự động chuyển sang port {boundPort}.");
					if (InvokeRequired)
					{
						Invoke(new Action(() => txtPort.Text = boundPort.ToString()));
					}
					else
					{
						txtPort.Text = boundPort.ToString();
					}
				}

				Log($"Server listening on 0.0.0.0:{boundPort}");
				var host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (var ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						Log($"Clients can connect: {ip}:{boundPort}");
					}
				}

				acceptThread = new Thread(AcceptLoop) { IsBackground = true };
				acceptThread.Start();
			}
			catch (SocketException se)
			{
				isRunning = false;
				btnStart.Enabled = true;
				btnStop.Enabled = false;
				if (se.SocketErrorCode == SocketError.AddressAlreadyInUse)
				{
					MessageBox.Show("Port đang được sử dụng. Vui lòng chọn port khác (ví dụ 8081, 9090) hoặc dừng server đang chạy.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					MessageBox.Show($"Không thể mở cổng: {se.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void StopServer()
		{
			if (!isRunning) return;
			isRunning = false;

			try
			{
				lock (clientsLock)
				{
					foreach (var c in allClients.ToList())
					{
						try
						{
							c.Socket.Shutdown(SocketShutdown.Both);
						}
						catch { }
						try
						{
							c.Socket.Close();
						}
						catch { }
					}
					allClients.Clear();
					usernameToClient.Clear();
					UpdateUsersUI();
				}
			}
			catch { }

			try { listenerSocket?.Close(); } catch { }
			try { listenerSocket?.Dispose(); } catch { }
			listenerSocket = null;

			btnStart.Enabled = true;
			btnStop.Enabled = false;
			Log("Server stopped");
		}

		private void AcceptLoop()
		{
			while (isRunning)
			{
				try
				{
					var client = listenerSocket.Accept();
					var ctx = new ClientCtx { Socket = client, Username = "" };
					lock (clientsLock) { allClients.Add(ctx); }
					Log($"Client connected: {client.RemoteEndPoint}");
					ctx.Thread = new Thread(() => ClientLoop(ctx)) { IsBackground = true };
					ctx.Thread.Start();
				}
				catch
				{
					if (isRunning == false) break;
				}
			}
		}

		private void ClientLoop(ClientCtx ctx)
		{
			var socket = ctx.Socket;
			byte[] buffer = new byte[8192];
			try
			{
				while (isRunning && socket.Connected)
				{
					int n = socket.Receive(buffer);
					if (n <= 0) break;
					string data = Encoding.UTF8.GetString(buffer, 0, n);
					ctx.Buffer.Append(data);
					while (true)
					{
						string acc = ctx.Buffer.ToString();
						int idx = acc.IndexOf("\n", StringComparison.Ordinal);
						if (idx < 0) break;
						string line = acc.Substring(0, idx);
						ctx.Buffer.Remove(0, idx + 1);
						ProcessMessage(ctx, line);
					}
				}
			}
			catch (Exception ex)
			{
				Log($"Client error: {ex.Message}");
			}
			finally
			{
				HandleDisconnect(ctx);
			}
		}

		private void ProcessMessage(ClientCtx ctx, string message)
		{
			// Protocol:
			// JOIN|username
			// MSG_ALL|sender|text
			// MSG_PRIV|sender|toUser|text
			// FILE_ALL|sender|filename|mime|base64
			// FILE_PRIV|sender|toUser|filename|mime|base64
			try
			{
				string[] parts = message.Split('|');
				if (parts.Length == 0) return;
				string cmd = parts[0];

				switch (cmd)
				{
					case "JOIN":
						if (parts.Length >= 2)
						{
							string username = parts[1].Trim();
							if (string.IsNullOrWhiteSpace(username))
							{
								Send(ctx, "ERR|Username required");
								return;
							}
							lock (clientsLock)
							{
								if (usernameToClient.ContainsKey(username))
								{
									Send(ctx, "ERR|Username already taken");
									return;
								}
								ctx.Username = username;
								usernameToClient[username] = ctx;
							}
							Log($"User joined: {username}");
							Send(ctx, "JOIN_OK|");
							BroadcastUsers();
							BroadcastSystem($"[{username}] đã tham gia phòng chat");
						}
						break;

					case "MSG_ALL":
						if (parts.Length >= 3)
						{
							string sender = parts[1];
							string text = parts[2];
							Broadcast($"MSG|{sender}|{text}", exclude: null);
						}
						break;

					case "MSG_PRIV":
						if (parts.Length >= 4)
						{
							string sender = parts[1];
							string toUser = parts[2];
							string text = parts[3];
							SendPrivate(sender, toUser, $"MSG_PRIV|{sender}|{text}");
						}
						break;

					case "FILE_ALL":
						if (parts.Length >= 5)
						{
							string sender = parts[1];
							string filename = parts[2];
							string mime = parts[3];
							string base64 = parts[4];
							Broadcast($"FILE|{sender}|{filename}|{mime}|{base64}", exclude: null);
						}
						break;

					case "FILE_PRIV":
						if (parts.Length >= 6)
						{
							string sender = parts[1];
							string toUser = parts[2];
							string filename = parts[3];
							string mime = parts[4];
							string base64 = parts[5];
							SendPrivate(sender, toUser, $"FILE_PRIV|{sender}|{filename}|{mime}|{base64}");
						}
						break;

					default:
						Send(ctx, "ERR|Unknown command");
						break;
				}
			}
			catch (Exception ex)
			{
				Log($"Process error: {ex.Message}");
			}
		}

		private void SendPrivate(string sender, string toUser, string payload)
		{
			ClientCtx target = null;
			lock (clientsLock)
			{
				usernameToClient.TryGetValue(toUser, out target);
			}
			if (target != null)
			{
				Send(target, payload);
				// Optional echo to sender
				ClientCtx senderCtx = null;
				lock (clientsLock)
				{
					usernameToClient.TryGetValue(sender, out senderCtx);
				}
				if (senderCtx != null && !ReferenceEquals(senderCtx, target))
				{
					Send(senderCtx, payload);
				}
			}
		}

		private void Broadcast(string line, ClientCtx exclude)
		{
			List<ClientCtx> copy;
			lock (clientsLock) { copy = allClients.ToList(); }
			foreach (var c in copy)
			{
				if (exclude != null && ReferenceEquals(c, exclude)) continue;
				Send(c, line);
			}
		}

		private void BroadcastSystem(string text)
		{
			Broadcast($"SYS|{text}", exclude: null);
		}

		private void BroadcastUsers()
		{
			string[] users;
			lock (clientsLock) { users = usernameToClient.Keys.OrderBy(x => x).ToArray(); }
			string payload = "USERS|" + string.Join(",", users);
			Broadcast(payload, exclude: null);
			UpdateUsersUI();
		}

		private void UpdateUsersUI()
		{
			if (InvokeRequired)
			{
				Invoke(new Action(UpdateUsersUI));
				return;
			}
			lstUsers.Items.Clear();
			lock (clientsLock)
			{
				foreach (var u in usernameToClient.Keys.OrderBy(x => x))
				{
					lstUsers.Items.Add(new ListViewItem(u));
				}
			}
		}

		private void Send(ClientCtx ctx, string line)
		{
			try
			{
				if (ctx.Socket?.Connected == true)
				{
					byte[] data = Encoding.UTF8.GetBytes(line + "\n");
					ctx.Socket.Send(data);
				}
			}
			catch { }
		}

		private void HandleDisconnect(ClientCtx ctx)
		{
			try { ctx.Socket?.Close(); } catch { }
			bool hadUser = false;
			string username = ctx.Username;
			lock (clientsLock)
			{
				allClients.Remove(ctx);
				if (!string.IsNullOrEmpty(ctx.Username) && usernameToClient.ContainsKey(ctx.Username))
				{
					usernameToClient.Remove(ctx.Username);
					hadUser = true;
				}
			}
			if (hadUser)
			{
				Log($"User left: {username}");
				BroadcastUsers();
				BroadcastSystem($"[{username}] đã rời phòng chat");
			}
		}

		private void Log(string message)
		{
			if (InvokeRequired)
			{
				Invoke(new Action<string>(Log), message);
				return;
			}
			var item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"));
			item.SubItems.Add(message);
			lstLog.Items.Insert(0, item);
		}
	}
}


