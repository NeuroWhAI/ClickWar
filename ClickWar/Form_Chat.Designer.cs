namespace ClickWar
{
    partial class Form_Chat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Chat));
            this.webBrowser_chat = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser_chat
            // 
            this.webBrowser_chat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser_chat.Location = new System.Drawing.Point(0, 0);
            this.webBrowser_chat.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser_chat.Name = "webBrowser_chat";
            this.webBrowser_chat.Size = new System.Drawing.Size(322, 233);
            this.webBrowser_chat.TabIndex = 0;
            // 
            // Form_Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 233);
            this.Controls.Add(this.webBrowser_chat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form_Chat";
            this.Text = "Chat";
            this.Load += new System.EventHandler(this.Form_Chat_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser_chat;
    }
}