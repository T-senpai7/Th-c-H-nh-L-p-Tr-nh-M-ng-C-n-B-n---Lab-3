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

namespace Bai1
{
    public partial class UServer : Form
    {
        public UServer()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_listen_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Thread thdUDPserver = new Thread(new ThreadStart(StartServer));
            thdUDPserver.Start();
            btn_listen.Enabled = false;
        }

        private void StartServer()
        {
            UdpClient udpClient;
            if (int.TryParse(tb_port.Text, out int port))
            {
                udpClient = new UdpClient(port);
            }
            else
            {
                MessageBox.Show("Vui lòng nhập định dạng số cho Port", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btn_listen.Enabled = true;
                return;
            } 
                
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string ReturnData = Encoding.UTF8.GetString(receiveBytes);
                string mess = RemoteIpEndPoint.Address.ToString() + ":" + ReturnData.ToString();
                InfoMessage(mess);
            }
        }

        private void InfoMessage(string mess)
        {
            rtb_chat.AppendText(" " + mess + "\n");
        }
    }
}
