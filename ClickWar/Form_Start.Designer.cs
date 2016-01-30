namespace ClickWar
{
    partial class Form_Start
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Start));
            this.timer_update = new System.Windows.Forms.Timer(this.components);
            this.label_underText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer_update
            // 
            this.timer_update.Interval = 16;
            this.timer_update.Tick += new System.EventHandler(this.timer_update_Tick);
            // 
            // label_underText
            // 
            this.label_underText.AutoSize = true;
            this.label_underText.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label_underText.Location = new System.Drawing.Point(12, 232);
            this.label_underText.Name = "label_underText";
            this.label_underText.Size = new System.Drawing.Size(0, 15);
            this.label_underText.TabIndex = 1;
            // 
            // Form_Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::ClickWar.Properties.Resources.loading;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(512, 256);
            this.Controls.Add(this.label_underText);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_Start";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form_Start";
            this.TransparencyKey = System.Drawing.SystemColors.WindowFrame;
            this.Load += new System.EventHandler(this.Form_Start_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer_update;
        private System.Windows.Forms.Label label_underText;
    }
}