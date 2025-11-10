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
    public partial class TCPClient : Form
    {
        private NetworkStream ns;
        private TcpClient tcpClient;
        private Byte[] bytes;
        private IPAddress ipAddress;

        public TCPClient()
        {
            InitializeComponent();
            btn_discnt.Enabled = false;
        }

        private void btn_cnt_Click(object sender, EventArgs e)
        {
            tcpClient = new TcpClient();
            ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, 8080);
            tcpClient.Connect(iPEndPoint);
            ns = tcpClient.GetStream();
            btn_cnt.Enabled = false;
            btn_discnt.Enabled = true;
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            try 
            {
                if (tcpClient == null || !tcpClient.Connected)
                {
                    MessageBox.Show("Please connect to the server first.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
            Byte[] bytes = System.Text.Encoding.ASCII.GetBytes(tb_chat.Text + "\n");
            ns.Write(bytes, 0, bytes.Length);
        }

        private void btn_discnt_Click(object sender, EventArgs e)
        {
            Byte[] Data = System.Text.Encoding.ASCII.GetBytes("quit\n");
            ns.Write(Data, 0, Data.Length);
            ns.Close();
            tcpClient.Close();
            btn_cnt.Enabled = true;
        }
    }
}
