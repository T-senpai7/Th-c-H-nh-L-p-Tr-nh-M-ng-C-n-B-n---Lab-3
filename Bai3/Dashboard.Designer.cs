namespace Bai3
{
    partial class Dashboard
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn_tcpsv = new Button();
            btn_tcpclient = new Button();
            SuspendLayout();
            // 
            // btn_tcpsv
            // 
            btn_tcpsv.Location = new Point(61, 49);
            btn_tcpsv.Name = "btn_tcpsv";
            btn_tcpsv.Size = new Size(112, 34);
            btn_tcpsv.TabIndex = 0;
            btn_tcpsv.Text = "TCP Server";
            btn_tcpsv.UseVisualStyleBackColor = true;
            btn_tcpsv.Click += btn_tcpsv_Click;
            // 
            // btn_tcpclient
            // 
            btn_tcpclient.Location = new Point(305, 49);
            btn_tcpclient.Name = "btn_tcpclient";
            btn_tcpclient.Size = new Size(112, 34);
            btn_tcpclient.TabIndex = 1;
            btn_tcpclient.Text = "TCP Client";
            btn_tcpclient.UseVisualStyleBackColor = true;
            btn_tcpclient.Click += btn_tcpclient_Click;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(492, 137);
            Controls.Add(btn_tcpclient);
            Controls.Add(btn_tcpsv);
            Name = "Dashboard";
            Text = "Dashboard";
            ResumeLayout(false);
        }

        #endregion

        private Button btn_tcpsv;
        private Button btn_tcpclient;
    }
}
