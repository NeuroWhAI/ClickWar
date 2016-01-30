namespace ClickWar
{
    partial class Form_Main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.timer_update = new System.Windows.Forms.Timer(this.components);
            this.timer_updateSlower = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_scaleDown = new System.Windows.Forms.Button();
            this.button_scaleUp = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_playerPower = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_playerName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_logout = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button_powerWayRight = new System.Windows.Forms.Button();
            this.button_powerWayLeft = new System.Windows.Forms.Button();
            this.button_powerWayDown = new System.Windows.Forms.Button();
            this.button_powerWayUp = new System.Windows.Forms.Button();
            this.button_powerWayHere = new System.Windows.Forms.Button();
            this.button_reColor = new System.Windows.Forms.Button();
            this.groupBox_rank = new System.Windows.Forms.GroupBox();
            this.listBox_rank = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox_rank.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer_update
            // 
            this.timer_update.Interval = 16;
            this.timer_update.Tick += new System.EventHandler(this.timer_update_Tick);
            // 
            // timer_updateSlower
            // 
            this.timer_updateSlower.Interval = 2000;
            this.timer_updateSlower.Tick += new System.EventHandler(this.timer_updateSlower_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_scaleDown);
            this.groupBox1.Controls.Add(this.button_scaleUp);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(90, 68);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "View";
            // 
            // button_scaleDown
            // 
            this.button_scaleDown.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_scaleDown.Location = new System.Drawing.Point(48, 24);
            this.button_scaleDown.Name = "button_scaleDown";
            this.button_scaleDown.Size = new System.Drawing.Size(36, 36);
            this.button_scaleDown.TabIndex = 2;
            this.button_scaleDown.Text = "-";
            this.button_scaleDown.UseVisualStyleBackColor = true;
            this.button_scaleDown.Click += new System.EventHandler(this.button_scaleDown_Click);
            // 
            // button_scaleUp
            // 
            this.button_scaleUp.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_scaleUp.Location = new System.Drawing.Point(6, 24);
            this.button_scaleUp.Name = "button_scaleUp";
            this.button_scaleUp.Size = new System.Drawing.Size(36, 36);
            this.button_scaleUp.TabIndex = 1;
            this.button_scaleUp.Text = "+";
            this.button_scaleUp.UseVisualStyleBackColor = true;
            this.button_scaleUp.Click += new System.EventHandler(this.button_scaleUp_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label_playerPower);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label_playerName);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(108, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(225, 68);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Profile";
            // 
            // label_playerPower
            // 
            this.label_playerPower.AutoSize = true;
            this.label_playerPower.Location = new System.Drawing.Point(65, 45);
            this.label_playerPower.Name = "label_playerPower";
            this.label_playerPower.Size = new System.Drawing.Size(74, 15);
            this.label_playerPower.TabIndex = 2;
            this.label_playerPower.Text = "Loading...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Power :";
            // 
            // label_playerName
            // 
            this.label_playerName.AutoSize = true;
            this.label_playerName.Location = new System.Drawing.Point(65, 24);
            this.label_playerName.Name = "label_playerName";
            this.label_playerName.Size = new System.Drawing.Size(74, 15);
            this.label_playerName.TabIndex = 2;
            this.label_playerName.Text = "Loading...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_reColor);
            this.groupBox3.Controls.Add(this.button_logout);
            this.groupBox3.Location = new System.Drawing.Point(339, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(187, 68);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Menu";
            // 
            // button_logout
            // 
            this.button_logout.Location = new System.Drawing.Point(6, 24);
            this.button_logout.Name = "button_logout";
            this.button_logout.Size = new System.Drawing.Size(88, 36);
            this.button_logout.TabIndex = 3;
            this.button_logout.Text = "Logout";
            this.button_logout.UseVisualStyleBackColor = true;
            this.button_logout.Click += new System.EventHandler(this.button_logout_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button_powerWayRight);
            this.groupBox4.Controls.Add(this.button_powerWayLeft);
            this.groupBox4.Controls.Add(this.button_powerWayDown);
            this.groupBox4.Controls.Add(this.button_powerWayUp);
            this.groupBox4.Controls.Add(this.button_powerWayHere);
            this.groupBox4.Location = new System.Drawing.Point(532, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(229, 68);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Power Way";
            // 
            // button_powerWayRight
            // 
            this.button_powerWayRight.Location = new System.Drawing.Point(187, 24);
            this.button_powerWayRight.Name = "button_powerWayRight";
            this.button_powerWayRight.Size = new System.Drawing.Size(36, 36);
            this.button_powerWayRight.TabIndex = 8;
            this.button_powerWayRight.Text = "→";
            this.button_powerWayRight.UseVisualStyleBackColor = true;
            this.button_powerWayRight.Click += new System.EventHandler(this.button_powerWayRight_Click);
            // 
            // button_powerWayLeft
            // 
            this.button_powerWayLeft.Location = new System.Drawing.Point(145, 24);
            this.button_powerWayLeft.Name = "button_powerWayLeft";
            this.button_powerWayLeft.Size = new System.Drawing.Size(36, 36);
            this.button_powerWayLeft.TabIndex = 7;
            this.button_powerWayLeft.Text = "←";
            this.button_powerWayLeft.UseVisualStyleBackColor = true;
            this.button_powerWayLeft.Click += new System.EventHandler(this.button_powerWayLeft_Click);
            // 
            // button_powerWayDown
            // 
            this.button_powerWayDown.Location = new System.Drawing.Point(103, 24);
            this.button_powerWayDown.Name = "button_powerWayDown";
            this.button_powerWayDown.Size = new System.Drawing.Size(36, 36);
            this.button_powerWayDown.TabIndex = 6;
            this.button_powerWayDown.Text = "↓";
            this.button_powerWayDown.UseVisualStyleBackColor = true;
            this.button_powerWayDown.Click += new System.EventHandler(this.button_powerWayDown_Click);
            // 
            // button_powerWayUp
            // 
            this.button_powerWayUp.Location = new System.Drawing.Point(61, 24);
            this.button_powerWayUp.Name = "button_powerWayUp";
            this.button_powerWayUp.Size = new System.Drawing.Size(36, 36);
            this.button_powerWayUp.TabIndex = 5;
            this.button_powerWayUp.Text = "↑";
            this.button_powerWayUp.UseVisualStyleBackColor = true;
            this.button_powerWayUp.Click += new System.EventHandler(this.button_powerWayUp_Click);
            // 
            // button_powerWayHere
            // 
            this.button_powerWayHere.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_powerWayHere.Location = new System.Drawing.Point(6, 24);
            this.button_powerWayHere.Name = "button_powerWayHere";
            this.button_powerWayHere.Size = new System.Drawing.Size(36, 36);
            this.button_powerWayHere.TabIndex = 4;
            this.button_powerWayHere.Text = "◎";
            this.button_powerWayHere.UseVisualStyleBackColor = true;
            this.button_powerWayHere.Click += new System.EventHandler(this.button_powerWayHere_Click);
            // 
            // button_reColor
            // 
            this.button_reColor.Location = new System.Drawing.Point(100, 24);
            this.button_reColor.Name = "button_reColor";
            this.button_reColor.Size = new System.Drawing.Size(81, 36);
            this.button_reColor.TabIndex = 4;
            this.button_reColor.Text = "ReColor";
            this.button_reColor.UseVisualStyleBackColor = true;
            this.button_reColor.Click += new System.EventHandler(this.button_reColor_Click);
            // 
            // groupBox_rank
            // 
            this.groupBox_rank.Controls.Add(this.listBox_rank);
            this.groupBox_rank.Location = new System.Drawing.Point(767, 12);
            this.groupBox_rank.Name = "groupBox_rank";
            this.groupBox_rank.Size = new System.Drawing.Size(227, 68);
            this.groupBox_rank.TabIndex = 4;
            this.groupBox_rank.TabStop = false;
            this.groupBox_rank.Text = "Rank";
            // 
            // listBox_rank
            // 
            this.listBox_rank.FormattingEnabled = true;
            this.listBox_rank.ItemHeight = 15;
            this.listBox_rank.Location = new System.Drawing.Point(6, 24);
            this.listBox_rank.Name = "listBox_rank";
            this.listBox_rank.ScrollAlwaysVisible = true;
            this.listBox_rank.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox_rank.Size = new System.Drawing.Size(215, 34);
            this.listBox_rank.TabIndex = 5;
            this.listBox_rank.MouseEnter += new System.EventHandler(this.listBox_rank_MouseEnter);
            this.listBox_rank.MouseLeave += new System.EventHandler(this.listBox_rank_MouseLeave);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.groupBox_rank);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Click War";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form_Main_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseDown);
            this.MouseLeave += new System.EventHandler(this.Form_Main_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox_rank.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer_update;
        private System.Windows.Forms.Timer timer_updateSlower;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_scaleDown;
        private System.Windows.Forms.Button button_scaleUp;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label_playerName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_playerPower;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_logout;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_powerWayRight;
        private System.Windows.Forms.Button button_powerWayLeft;
        private System.Windows.Forms.Button button_powerWayDown;
        private System.Windows.Forms.Button button_powerWayUp;
        private System.Windows.Forms.Button button_powerWayHere;
        private System.Windows.Forms.Button button_reColor;
        private System.Windows.Forms.GroupBox groupBox_rank;
        private System.Windows.Forms.ListBox listBox_rank;
    }
}

