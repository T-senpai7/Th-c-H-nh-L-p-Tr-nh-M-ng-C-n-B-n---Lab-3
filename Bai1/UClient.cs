using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai1
{
    public partial class UClient : Form
    {
        public UClient()
        {
            InitializeComponent();
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            UdpClient udpClient = new UdpClient();
            Byte[] sendBytes = Encoding.UTF8.GetBytes(tb_chat.Text);
            int.TryParse(tb_port.Text, out int port);
            udpClient.Send(sendBytes, sendBytes.Length, tb_ip.Text, port);
        }

        private void tb_chat_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
