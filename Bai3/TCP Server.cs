using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai3
{
    public partial class TCPServer : Form
    {
        private Thread serverThread;
        private Socket clientSocket;
        private Socket listenerSocket;
        public TCPServer()
        {
            InitializeComponent();
        }

        void StartUnsafeThread()
        {
            int BytesRecv = 0;
            Byte[] recv = new Byte[1];
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipepServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            listenerSocket.Bind(ipepServer);
            listenerSocket.Listen(-1);
            clientSocket = listenerSocket.Accept();

            listViewCommand.Items.Add(new ListViewItem("New client connected"));

            while (clientSocket.Connected)
            {
                string text = "";
                BytesRecv = 0;

                while (true)
                {
                    BytesRecv = clientSocket.Receive(recv);
                    if (BytesRecv == 0)
                    {
                        clientSocket.Close();
                        listenerSocket.Close();
                        btn_listen.Enabled = true;
                        return;
                    }

                    text += Encoding.ASCII.GetString(recv, 0, BytesRecv);
                    if (text.EndsWith("\n"))
                        break;
                }
                if (Text.Equals("quit\n"))
                {
                    listViewCommand.Items.Add(new ListViewItem("Client disconnected"));
                    break;
                }
                listViewCommand.Items.Add(new ListViewItem("Client: " + text.Trim()));
            }
        }

        private void btn_listen_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            serverThread = new Thread(new ThreadStart(StartUnsafeThread));
            serverThread.Start();
            btn_listen.Enabled = false;
        }
    }
}
