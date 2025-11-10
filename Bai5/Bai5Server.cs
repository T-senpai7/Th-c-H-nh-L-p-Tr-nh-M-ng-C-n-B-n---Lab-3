using System;
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
    public partial class Bai5Server : Form
    {
        private ListView listViewLog = null!;
        private Button btnListen = null!;
        private Button btnStop = null!;
        private Socket? listenerSocket;
        private Thread? serverThread;
        private bool isListening = false;
        private List<ClientHandler> clients = new List<ClientHandler>();
        private DatabaseHelper database;
        private const int PORT = 8080;

        public Bai5Server()
        {
            database = new DatabaseHelper();
            InitializeComponent();
            btnStop.Enabled = false;
        }

        private void InitializeComponent()
        {
            this.Text = "Bai5 - TCP Server (Hôm nay ăn gì?)";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimizeBox = true;
            this.MaximizeBox = true;

            listViewLog = new ListView();
            listViewLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listViewLog.Location = new Point(12, 12);
            listViewLog.Size = new Size(876, 510);
            listViewLog.View = View.Details;
            listViewLog.FullRowSelect = true;
            listViewLog.GridLines = true;
            listViewLog.Columns.Add("Thời gian", 150);
            listViewLog.Columns.Add("Log", 726);

            btnListen = new Button();
            btnListen.Text = "Listen";
            btnListen.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnListen.Location = new Point(12, 530);
            btnListen.Size = new Size(120, 35);
            btnListen.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnListen.BackColor = Color.FromArgb(40, 167, 69);
            btnListen.ForeColor = Color.White;
            btnListen.FlatStyle = FlatStyle.Flat;
            btnListen.Click += BtnListen_Click;

            btnStop = new Button();
            btnStop.Text = "Stop";
            btnStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStop.Location = new Point(142, 530);
            btnStop.Size = new Size(120, 35);
            btnStop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStop.BackColor = Color.FromArgb(220, 53, 69);
            btnStop.ForeColor = Color.White;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Click += BtnStop_Click;

            this.Controls.Add(listViewLog);
            this.Controls.Add(btnListen);
            this.Controls.Add(btnStop);

            this.FormClosing += (s, e) =>
            {
                if (isListening)
                {
                    DialogResult result = MessageBox.Show(
                        "Server đang chạy. Bạn có muốn dừng server và đóng cửa sổ?",
                        "Xác nhận đóng Server",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        StopServer();
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            };
        }

        private void BtnListen_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            isListening = true;
            btnListen.Enabled = false;
            btnStop.Enabled = true;

            serverThread = new Thread(StartServer);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            isListening = false;
            
            foreach (var client in clients.ToList())
            {
                client.Close();
            }
            clients.Clear();

            try
            {
                if (listenerSocket != null)
                {
                    listenerSocket.Close();
                }
            }
            catch { }

            btnListen.Enabled = true;
            btnStop.Enabled = false;
            AddLog("Server stopped");
        }

        private void StartServer()
        {
            try
            {
                listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipepServer = new IPEndPoint(IPAddress.Any, PORT);
                
                try
                {
                    listenerSocket.Bind(ipepServer);
                    listenerSocket.Listen(10);
                    AddLog($"Server started on port {PORT}");
                    AddLog($"Server IP: {ipepServer.Address} (Listening on all interfaces)");
                    
                    // Hiển thị IP addresses có thể kết nối
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            AddLog($"Clients can connect to: {ip}:{PORT}");
                        }
                    }
                }
                catch (SocketException se) when (se.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    AddLog($"Lỗi: Port {PORT} đã được sử dụng bởi Server khác hoặc ứng dụng khác!");
                    AddLog("Vui lòng đóng Server đang chạy hoặc đổi port.");
                    isListening = false;
                    Invoke(new Action(() =>
                    {
                        btnListen.Enabled = true;
                        btnStop.Enabled = false;
                    }));
                    return;
                }

                while (isListening)
                {
                    try
                    {
                        Socket clientSocket = listenerSocket.Accept();
                        string clientInfo = clientSocket.RemoteEndPoint?.ToString() ?? "Unknown";
                        AddLog($"New client connected: {clientInfo}");
                        AddLog($"Total clients: {clients.Count + 1}");

                        ClientHandler handler = new ClientHandler(clientSocket, this, database);
                        clients.Add(handler);
                        handler.Start();
                    }
                    catch (Exception ex)
                    {
                        if (isListening)
                        {
                            AddLog($"Error accepting client: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"Server error: {ex.Message}");
                isListening = false;
                Invoke(new Action(() =>
                {
                    btnListen.Enabled = true;
                    btnStop.Enabled = false;
                }));
            }
        }

        public void AddLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddLog), message);
                return;
            }

            ListViewItem item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"));
            item.SubItems.Add(message);
            listViewLog.Items.Insert(0, item);
        }

        public void RemoveClient(ClientHandler client)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ClientHandler>(RemoveClient), client);
                return;
            }

            if (clients.Contains(client))
            {
                clients.Remove(client);
                AddLog($"Client disconnected. Total clients: {clients.Count}");
            }
        }

        public void BroadcastUpdate(string message)
        {
            foreach (var client in clients.ToList())
            {
                try
                {
                    client.SendMessage(message);
                }
                catch
                {
                    // Client đã disconnect
                }
            }
        }
    }

    public class ClientHandler
    {
        private Socket clientSocket = null!;
        private Bai5Server server = null!;
        private DatabaseHelper database = null!;
        private Thread? clientThread;
        private bool isConnected = true;
        private int currentUserId = -1; // ID của người dùng hiện tại

        public ClientHandler(Socket socket, Bai5Server server, DatabaseHelper database)
        {
            this.clientSocket = socket;
            this.server = server;
            this.database = database;
        }

        public void Start()
        {
            clientThread = new Thread(HandleClient);
            clientThread.IsBackground = true;
            clientThread.Start();
        }

        private void HandleClient()
        {
            try
            {
                byte[] buffer = new byte[4096];
                StringBuilder messageBuilder = new StringBuilder();

                while (isConnected && clientSocket.Connected)
                {
                    int bytesReceived = clientSocket.Receive(buffer);
                    if (bytesReceived == 0)
                    {
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    messageBuilder.Append(data);

                    while (messageBuilder.ToString().Contains("\n"))
                    {
                        int newlineIndex = messageBuilder.ToString().IndexOf("\n");
                        string message = messageBuilder.ToString().Substring(0, newlineIndex);
                        messageBuilder.Remove(0, newlineIndex + 1);

                        ProcessMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                server.AddLog($"Client error: {ex.Message}");
            }
            finally
            {
                Close();
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {
                string[] parts = message.Split('|');
                if (parts.Length == 0) return;

                string command = parts[0];
                server.AddLog($"Received: {command}");

                switch (command)
                {
                    case "SET_USER":
                        if (parts.Length > 1)
                        {
                            int.TryParse(parts[1], out currentUserId);
                            SendMessage("USER_SET|OK");
                        }
                        break;

                    case "GET_ALL_FOODS":
                        SendAllFoods();
                        break;

                    case "GET_USER_FOODS":
                        SendUserFoods();
                        break;

                    case "GET_USERS":
                        SendUsers();
                        break;

                    case "ADD_FOOD":
                        if (parts.Length > 3)
                        {
                            AddFood(parts[1], parts[2], parts[3]);
                        }
                        break;

                    case "ADD_USER":
                        if (parts.Length > 2)
                        {
                            AddUser(parts[1], parts[2]);
                        }
                        break;

                    case "RANDOM_FOOD_ALL":
                        SendRandomFood(true);
                        break;

                    case "RANDOM_FOOD_USER":
                        SendRandomFood(false);
                        break;

                    default:
                        SendMessage("ERROR|Unknown command");
                        break;
                }
            }
            catch (Exception ex)
            {
                server.AddLog($"Error processing message: {ex.Message}");
                SendMessage($"ERROR|{ex.Message}");
            }
        }

        private void SendAllFoods()
        {
            var foods = database.GetAllMonAn();
            StringBuilder response = new StringBuilder("ALL_FOODS|");
            foreach (var food in foods)
            {
                response.Append($"{food.IDMA}:{food.TenMonAn}:{food.HinhAnh ?? ""}:{food.IDNCC}:{food.HoVaTen}:{food.QuyenHan};");
            }
            SendMessage(response.ToString());
        }

        private void SendUserFoods()
        {
            if (currentUserId == -1)
            {
                SendMessage("ERROR|User not set");
                return;
            }

            var foods = database.GetMonAnByUser(currentUserId);
            StringBuilder response = new StringBuilder("USER_FOODS|");
            foreach (var food in foods)
            {
                response.Append($"{food.IDMA}:{food.TenMonAn}:{food.HinhAnh ?? ""}:{food.IDNCC}:{food.HoVaTen}:{food.QuyenHan};");
            }
            SendMessage(response.ToString());
        }

        private void SendUsers()
        {
            var users = database.GetAllNguoiDung();
            StringBuilder response = new StringBuilder("USERS|");
            foreach (var user in users)
            {
                response.Append($"{user.IDNCC}:{user.HoVaTen}:{user.QuyenHan};");
            }
            SendMessage(response.ToString());
        }

        private void AddFood(string tenMonAn, string hinhAnh, string idNCCStr)
        {
            if (int.TryParse(idNCCStr, out int idNCC))
            {
                bool success = database.AddMonAn(tenMonAn, hinhAnh, idNCC);
                if (success)
                {
                    SendMessage("ADD_FOOD_SUCCESS|");
                    // Broadcast update cho tất cả clients
                    server.BroadcastUpdate("UPDATE_FOODS|");
                    server.AddLog($"Food added: {tenMonAn} by user {idNCC}");
                }
                else
                {
                    SendMessage("ADD_FOOD_ERROR|Failed to add food");
                }
            }
            else
            {
                SendMessage("ADD_FOOD_ERROR|Invalid user ID");
            }
        }

        private void AddUser(string hoVaTen, string quyenHan)
        {
            bool success = database.AddNguoiDung(hoVaTen, quyenHan);
            if (success)
            {
                SendMessage("ADD_USER_SUCCESS|");
                // Broadcast update cho tất cả clients
                server.BroadcastUpdate("UPDATE_USERS|");
                server.AddLog($"User added: {hoVaTen} ({quyenHan})");
            }
            else
            {
                SendMessage("ADD_USER_ERROR|Failed to add user");
            }
        }

        private void SendRandomFood(bool fromAll)
        {
            MonAnInfo food;
            if (fromAll)
            {
                food = database.GetRandomMonAn();
            }
            else
            {
                if (currentUserId == -1)
                {
                    SendMessage("ERROR|User not set");
                    return;
                }
                food = database.GetRandomMonAnByUser(currentUserId);
            }

            if (food != null)
            {
                SendMessage($"RANDOM_FOOD|{food.IDMA}:{food.TenMonAn}:{food.HinhAnh ?? ""}:{food.IDNCC}:{food.HoVaTen}:{food.QuyenHan}");
            }
            else
            {
                SendMessage("RANDOM_FOOD|NONE");
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                if (clientSocket.Connected)
                {
                    byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                    clientSocket.Send(data);
                }
            }
            catch (Exception ex)
            {
                server.AddLog($"Error sending message: {ex.Message}");
            }
        }

        public void Close()
        {
            isConnected = false;
            try
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch { }
            server.RemoveClient(this);
        }
    }
}

