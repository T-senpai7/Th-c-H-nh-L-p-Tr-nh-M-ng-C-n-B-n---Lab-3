using Bai1;

namespace Lab3_LTMCB
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void btn_client_Click(object sender, EventArgs e)
        {
            new UClient().Show();
        }

        private void btn_server_Click(object sender, EventArgs e)
        {
            new UServer().Show();
            btn_server.Enabled = false;
        }
    }
}
