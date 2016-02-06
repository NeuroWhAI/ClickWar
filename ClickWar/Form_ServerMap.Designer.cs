namespace ClickWar
{
    partial class Form_ServerMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ServerMap));
            this.comboBox_server = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_enter = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox_server
            // 
            this.comboBox_server.FormattingEnabled = true;
            this.comboBox_server.Location = new System.Drawing.Point(77, 6);
            this.comboBox_server.Name = "comboBox_server";
            this.comboBox_server.Size = new System.Drawing.Size(233, 23);
            this.comboBox_server.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server :";
            // 
            // button_enter
            // 
            this.button_enter.Location = new System.Drawing.Point(12, 40);
            this.button_enter.Name = "button_enter";
            this.button_enter.Size = new System.Drawing.Size(149, 33);
            this.button_enter.TabIndex = 5;
            this.button_enter.Text = "Enter";
            this.button_enter.UseVisualStyleBackColor = true;
            this.button_enter.Click += new System.EventHandler(this.button_enter_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(161, 40);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(149, 33);
            this.button_cancel.TabIndex = 6;
            this.button_cancel.Text = "Exit";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // Form_ServerMap
            // 
            this.AcceptButton = this.button_enter;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(322, 85);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_enter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_server);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form_ServerMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server Map";
            this.Load += new System.EventHandler(this.Form_ServerMap_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_server;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_enter;
        private System.Windows.Forms.Button button_cancel;
    }
}