using System.Net;
using System.Net.Sockets;

namespace Bai2
{
    public partial class Bai2 : Form
    {
        private Thread serverThread;
        private Socket clientSocket;
        private Socket listenerSocket;
        public Bai2()
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
                do
                {
                    BytesRecv = clientSocket.Receive(recv);
                    text += System.Text.Encoding.ASCII.GetString(recv, 0, BytesRecv);
                }
                while (text[text.Length - 1] != '\n');

                listViewCommand.Items.Add(new ListViewItem(text));
            }

            listenerSocket.Close();
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
