namespace Bai1
{
    partial class UClient
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
            tb_ip = new TextBox();
            tb_port = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            btn_send = new Button();
            tb_chat = new TextBox();
            SuspendLayout();
            // 
            // tb_ip
            // 
            tb_ip.Location = new Point(73, 59);
            tb_ip.Name = "tb_ip";
            tb_ip.Size = new Size(320, 31);
            tb_ip.TabIndex = 0;
            // 
            // tb_port
            // 
            tb_port.Location = new Point(544, 59);
            tb_port.Name = "tb_port";
            tb_port.Size = new Size(147, 31);
            tb_port.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(73, 19);
            label1.Name = "label1";
            label1.Size = new Size(136, 25);
            label1.TabIndex = 2;
            label1.Text = "IP Remote Host";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(544, 19);
            label2.Name = "label2";
            label2.Size = new Size(44, 25);
            label2.TabIndex = 3;
            label2.Text = "Port";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(73, 113);
            label3.Name = "label3";
            label3.Size = new Size(48, 25);
            label3.TabIndex = 5;
            label3.Text = "Chat";
            // 
            // btn_send
            // 
            btn_send.Location = new Point(73, 391);
            btn_send.Name = "btn_send";
            btn_send.Size = new Size(112, 34);
            btn_send.TabIndex = 6;
            btn_send.Text = "Gửi";
            btn_send.UseVisualStyleBackColor = true;
            btn_send.Click += btn_send_Click;
            // 
            // tb_chat
            // 
            tb_chat.Location = new Point(73, 150);
            tb_chat.Multiline = true;
            tb_chat.Name = "tb_chat";
            tb_chat.Size = new Size(618, 235);
            tb_chat.TabIndex = 7;
            tb_chat.TextChanged += tb_chat_TextChanged;
            // 
            // UClient
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tb_chat);
            Controls.Add(btn_send);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tb_port);
            Controls.Add(tb_ip);
            Name = "UClient";
            Text = "UDP Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_ip;
        private TextBox tb_port;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btn_send;
        private TextBox tb_chat;
    }
}