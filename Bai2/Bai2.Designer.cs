namespace Bai2
{
    partial class Bai2
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
            btn_listen = new Button();
            listViewCommand = new ListView();
            SuspendLayout();
            // 
            // btn_listen
            // 
            btn_listen.Location = new Point(536, 12);
            btn_listen.Name = "btn_listen";
            btn_listen.Size = new Size(112, 34);
            btn_listen.TabIndex = 1;
            btn_listen.Text = "Listen";
            btn_listen.UseVisualStyleBackColor = true;
            btn_listen.Click += btn_listen_Click;
            // 
            // listViewCommand
            // 
            listViewCommand.Location = new Point(12, 12);
            listViewCommand.Name = "listViewCommand";
            listViewCommand.Size = new Size(518, 421);
            listViewCommand.TabIndex = 2;
            listViewCommand.UseCompatibleStateImageBehavior = false;
            listViewCommand.View = View.List;
            // 
            // Bai2
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(660, 445);
            Controls.Add(listViewCommand);
            Controls.Add(btn_listen);
            Name = "Bai2";
            Text = "Bai2";
            ResumeLayout(false);
        }

        #endregion

        private Button btn_listen;
        private ListView listViewCommand;
    }
}
