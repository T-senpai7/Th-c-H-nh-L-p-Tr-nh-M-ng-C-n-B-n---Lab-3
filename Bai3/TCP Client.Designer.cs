namespace Bai3
{
    partial class TCPClient
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
            tb_chat = new TextBox();
            btn_cnt = new Button();
            btn_send = new Button();
            btn_discnt = new Button();
            SuspendLayout();
            // 
            // tb_chat
            // 
            tb_chat.Location = new Point(12, 12);
            tb_chat.Multiline = true;
            tb_chat.Name = "tb_chat";
            tb_chat.Size = new Size(498, 252);
            tb_chat.TabIndex = 0;
            // 
            // btn_cnt
            // 
            btn_cnt.Location = new Point(516, 12);
            btn_cnt.Name = "btn_cnt";
            btn_cnt.Size = new Size(112, 34);
            btn_cnt.TabIndex = 1;
            btn_cnt.Text = "Connect";
            btn_cnt.UseVisualStyleBackColor = true;
            btn_cnt.Click += btn_cnt_Click;
            // 
            // btn_send
            // 
            btn_send.Location = new Point(516, 70);
            btn_send.Name = "btn_send";
            btn_send.Size = new Size(112, 34);
            btn_send.TabIndex = 2;
            btn_send.Text = "Send";
            btn_send.UseVisualStyleBackColor = true;
            btn_send.Click += btn_send_Click;
            // 
            // btn_discnt
            // 
            btn_discnt.Location = new Point(516, 120);
            btn_discnt.Name = "btn_discnt";
            btn_discnt.Size = new Size(112, 34);
            btn_discnt.TabIndex = 3;
            btn_discnt.Text = "Disconnect";
            btn_discnt.UseVisualStyleBackColor = true;
            btn_discnt.Click += btn_discnt_Click;
            // 
            // TCPClient
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 276);
            Controls.Add(btn_discnt);
            Controls.Add(btn_send);
            Controls.Add(btn_cnt);
            Controls.Add(tb_chat);
            Name = "TCPClient";
            Text = "TCP Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_chat;
        private Button btn_cnt;
        private Button btn_send;
        private Button btn_discnt;
    }
}