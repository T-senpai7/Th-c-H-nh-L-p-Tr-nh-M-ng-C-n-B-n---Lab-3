namespace Bai1
{
    partial class UServer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tb_port = new TextBox();
            label1 = new Label();
            btn_listen = new Button();
            rtb_chat = new RichTextBox();
            SuspendLayout();
            // 
            // tb_port
            // 
            tb_port.Location = new Point(158, 54);
            tb_port.Name = "tb_port";
            tb_port.Size = new Size(135, 31);
            tb_port.TabIndex = 0;
            tb_port.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(94, 54);
            label1.Name = "label1";
            label1.Size = new Size(44, 25);
            label1.TabIndex = 1;
            label1.Text = "Port";
            // 
            // btn_listen
            // 
            btn_listen.Location = new Point(547, 54);
            btn_listen.Name = "btn_listen";
            btn_listen.Size = new Size(112, 34);
            btn_listen.TabIndex = 3;
            btn_listen.Text = "Listen";
            btn_listen.UseVisualStyleBackColor = true;
            btn_listen.Click += btn_listen_Click;
            // 
            // rtb_chat
            // 
            rtb_chat.Location = new Point(94, 115);
            rtb_chat.Name = "rtb_chat";
            rtb_chat.Size = new Size(565, 323);
            rtb_chat.TabIndex = 4;
            rtb_chat.Text = "";
            // 
            // UServer
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(rtb_chat);
            Controls.Add(btn_listen);
            Controls.Add(label1);
            Controls.Add(tb_port);
            Name = "UServer";
            Text = "UDP Server";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_port;
        private Label label1;
        private Button btn_listen;
        private RichTextBox rtb_chat;
    }
}