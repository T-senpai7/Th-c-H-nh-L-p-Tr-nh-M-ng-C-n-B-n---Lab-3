namespace Bai3
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void btn_tcpsv_Click(object sender, EventArgs e)
        {
            new TCPServer().Show();
            btn_tcpsv.Enabled = false;
        }

        private void btn_tcpclient_Click(object sender, EventArgs e)
        {
            new TCPClient().Show();
        }
    }
}
