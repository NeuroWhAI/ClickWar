using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ClickWar
{
    public partial class Form_Main : Form, IStringReceiver
    {
        public Form_Main(string playerName)
        {
            // 더블 버퍼링
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            InitializeComponent();


            // 마우스 휠 이벤트 등록
            this.MouseWheel += Form_Main_MouseWheel;


            // Power way 버튼들을 목록에 등록
            m_powerWayButtons[0] = this.button_powerWayHere;
            m_powerWayButtons[1] = this.button_powerWayUp;
            m_powerWayButtons[2] = this.button_powerWayRight;
            m_powerWayButtons[3] = this.button_powerWayDown;
            m_powerWayButtons[4] = this.button_powerWayLeft;

            WhenPowerWayButtonClick(m_powerWayButtons[0]);


            // 컨트롤러에 DB와 게임 모델, 뷰 등록
            m_gameController.SetDB(m_db);
            m_gameController.SetGameMap(m_gameMap);
            m_gameController.SetGameViewer(m_gameViewer);


            // 창 이름 설정
            this.Text = "Click War - " + Application.ProductVersion;


            // 플레이어 이름 등록
            m_gameController.SetPlayerName(playerName);
            this.label_playerName.Text = string.Format("\"{0}\"", playerName);
        }

        //##################################################################################

        protected Button[] m_powerWayButtons = new Button[5];

        //##################################################################################

        protected Util.DBHelper m_db = new Util.DBHelper();

        protected Game.GameMap m_gameMap = new Game.GameMap();

        protected View.GameViewer m_gameViewer = new View.GameViewer();

        protected Controller.GameController m_gameController = new Controller.GameController();

        //##################################################################################

        protected readonly object m_lockObj = new object();
        protected bool m_runningThread = true;
        protected Thread m_gameThread = null;

        protected List<Action> m_threadWorkList = new List<Action>();

        //##################################################################################

        protected void ThreadJob()
        {
            while (m_runningThread)
            {
#if DEBUG
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();
#endif


                int workCount = m_threadWorkList.Count;

                for (int i = 0; i < workCount; ++i)
                {
                    if (m_threadWorkList[i] == null)
                        continue;


                    var job = m_threadWorkList[i];


#if DEBUG
                    System.Diagnostics.Stopwatch jobTimer = new System.Diagnostics.Stopwatch();
                    jobTimer.Start();

                    System.Diagnostics.Debug.Write(string.Format("Job{0}", i));
#endif


                    var task = Task.Factory.StartNew(job);
                    task.Wait(4000);


#if DEBUG
                    System.Diagnostics.Debug.WriteLine(string.Format(": {0}s", (double)jobTimer.ElapsedMilliseconds / 1000.0));
#endif


                    lock (m_lockObj)
                    {
                        m_threadWorkList[i] = null;
                    }


                    Thread.Sleep(10);
                }


#if DEBUG
                if(timer.ElapsedMilliseconds > 0)
                    System.Diagnostics.Debug.WriteLine(string.Format("{0}s", (double)timer.ElapsedMilliseconds / 1000.0));
#endif


                Thread.Sleep(32);
            }
        }

        protected bool AddThreadWork(Action work, int workNum)
        {
            while (m_threadWorkList.Count <= workNum)
            {
                lock(m_lockObj)
                {
                    m_threadWorkList.Add(null);
                }
            }


            if (m_threadWorkList[workNum] == null)
            {
                lock (m_lockObj)
                {
                    m_threadWorkList[workNum] = work;
                }


                return true;
            }
            

            return false;
        }

        //##################################################################################

        public void ResetGame(string playerName)
        {
            this.label_playerName.Text = string.Format("\"{0}\"", playerName);


            m_gameController.ResetGame(playerName, this.Size);


            this.timer_update.Start();
            this.timer_updateSlower.Start();
        }

        //##################################################################################

        private void Form_Main_Load(object sender, EventArgs e)
        {
            m_gameThread = new Thread(ThreadJob);
            m_gameThread.Start();


            // 폼이 게임 이벤트를 받을 수 있도록 등록.
            m_gameMap.WhenShutdownMessageChanged += WhenShutdownMessageChanged;


            m_db.Connect();

            m_gameController.Init(this.Size);


            this.timer_update.Start();
            this.timer_updateSlower.Start();
        }

        private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //m_db.WaitAllTask();

            if (m_gameThread != null)
            {
                m_runningThread = false;

                m_gameThread.Join();
                m_gameThread.Interrupt();
                m_gameThread = null;
            }

            Application.Exit();
        }

        //##################################################################################

        private void Form_Main_Paint(object sender, PaintEventArgs e)
        {
            m_gameController.DrawGame(e.Graphics, this.Size);
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void timer_updateSlower_Tick(object sender, EventArgs e)
        {
            // 동기화
            AddThreadWork(() => m_gameController.SyncGame(this.Size), 2); // 비동기 작업

            if (m_gameMap.ShutdownFlag)
                Application.Exit();

            // UI 갱신
            this.label_playerPower.Text = m_gameController.GetPlayerPower().ToString();

            // 랭킹 갱신
            UpdateRank();
        }

        //##################################################################################

        private void Form_Main_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 비동기 작업
                m_gameController.WhenLeftClick(this.AddThreadWork, 0,
                    e.Location, this.Size);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                ReadSignInput();
            }
        }

        private void Form_Main_MouseMove(object sender, MouseEventArgs e)
        {
            // 메인 포커스를 폼으로 하기위한 조치
            this.label1.Select();
            this.label1.Focus();


            m_gameController.WhenMouseMove(e.Location);
        }

        //##################################################################################

        private void Form_Main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    WhenPowerWayButtonClick(m_powerWayButtons[0]);
                    break;

                case Keys.W:
                    WhenPowerWayButtonClick(m_powerWayButtons[1]);
                    break;

                case Keys.D:
                    WhenPowerWayButtonClick(m_powerWayButtons[2]);
                    break;

                case Keys.S:
                    WhenPowerWayButtonClick(m_powerWayButtons[3]);
                    break;

                case Keys.A:
                    WhenPowerWayButtonClick(m_powerWayButtons[4]);
                    break;

                case Keys.Oemplus:
                    m_gameController.ChangeViewScale(4, this.Size);
                    break;

                case Keys.OemMinus:
                    m_gameController.ChangeViewScale(-4, this.Size);
                    break;

                case Keys.Enter:
                    ReadSignInput();
                    break;

                case Keys.Delete:
                    RemoveSign();
                    break;
            }
        }

        //##################################################################################

        protected void ReadSignInput()
        {
            int x, y;
            m_gameController.GetFocusedLocation(out x, out y);

            var tile = m_gameMap.GetTileAt(x, y);
            if (tile != null
                &&
                m_gameController.IsPlayerTerritory(x, y))
            {
                Form_Sign inputForm = new Form_Sign(this, x, y);
                inputForm.SetInputText(tile.Sign);
                inputForm.ShowDialog();
            }
        }

        protected void RemoveSign()
        {
            int x, y;
            m_gameController.GetFocusedLocation(out x, out y);

            var tile = m_gameMap.GetTileAt(x, y);
            if (tile != null
                &&
                m_gameController.IsPlayerTerritory(x, y))
            {
                AddThreadWork(() => m_gameController.SetSignAt("", x, y), 1);
            }
        }

        public void WhenReceiveSignInfo(string text, int tileX, int tileY)
        {
            AddThreadWork(() => m_gameController.SetSignAt(text, tileX, tileY), 1);
        }

        //##################################################################################

        private void button_scaleUp_Click(object sender, EventArgs e)
        {
            m_gameController.ChangeViewScale(4, this.Size);
        }

        private void button_scaleDown_Click(object sender, EventArgs e)
        {
            m_gameController.ChangeViewScale(-4, this.Size);
        }

        private void Form_Main_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                m_gameController.WhenMouseWheelRollDown(this.Size);
            }
            else if(e.Delta > 0)
            {
                m_gameController.WhenMouseWheelRollUp(this.Size);
            }
        }

        //##################################################################################

        private void button_logout_Click(object sender, EventArgs e)
        {
            // 갱신 정지
            this.timer_update.Stop();
            this.timer_updateSlower.Stop();


            // 채팅창 숨기기
            this.HideChatForm();


            // 로그인 화면 보여주기.
            this.Hide();

            foreach (Form form in Application.OpenForms)
            {
                if (form is Form_Login)
                {
                    form.Show();
                    return;
                }
            }

            Form_Login loginForm = new Form_Login();
            loginForm.Show();
        }

        private void button_reColor_Click(object sender, EventArgs e)
        {
            m_gameController.ReColorAll();
        }

        private void button_chat_Click(object sender, EventArgs e)
        {
            ShowChatForm();
        }

        //##################################################################################

        protected void ShowChatForm()
        {
            Form chatForm = null;

            // 채팅 화면 보여주기.
            foreach (Form form in Application.OpenForms)
            {
                if (form is Form_Chat)
                {
                    chatForm = form;

                    break;
                }
            }

            if (chatForm == null)
                chatForm = new Form_Chat();
            chatForm.Show();
            chatForm.Location = new Point(this.Location.X + this.DisplayRectangle.Width + 2, this.Location.Y);
        }

        protected void HideChatForm()
        {
            // 채팅 화면 숨기기.
            foreach (Form form in Application.OpenForms)
            {
                if (form is Form_Chat)
                {
                    form.Hide();

                    return;
                }
            }
        }

        //##################################################################################

        private void Form_Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                m_gameController.WhenRightDown(e.Location);
            }
        }

        private void Form_Main_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_gameController.WhenRightUp(e.Location);
        }

        private void Form_Main_MouseLeave(object sender, EventArgs e)
        {
            m_gameController.WhenRightUp(new Point(-1, -1));
        }

        //##################################################################################

        protected void WhenPowerWayButtonClick(object sender)
        {
            int index = 0;

            foreach (var btn in m_powerWayButtons)
            {
                if (btn == sender)
                {
                    btn.Enabled = false;

                    btn.BackColor = Color.Aqua;

                    m_gameController.SetPowerWay(index);
                }
                else
                {
                    btn.Enabled = true;

                    btn.BackColor = Color.FromKnownColor(KnownColor.Control);
                }

                ++index;
            }

            this.label1.Select();
            this.label1.Focus();
        }

        private void button_powerWayHere_Click(object sender, EventArgs e)
        {
            WhenPowerWayButtonClick(sender);
        }

        private void button_powerWayUp_Click(object sender, EventArgs e)
        {
            WhenPowerWayButtonClick(sender);
        }

        private void button_powerWayDown_Click(object sender, EventArgs e)
        {
            WhenPowerWayButtonClick(sender);
        }

        private void button_powerWayLeft_Click(object sender, EventArgs e)
        {
            WhenPowerWayButtonClick(sender);
        }

        private void button_powerWayRight_Click(object sender, EventArgs e)
        {
            WhenPowerWayButtonClick(sender);
        }

        //##################################################################################

        protected void ChangeRankHeight(int delta)
        {
            this.groupBox_rank.Size = new Size(this.groupBox_rank.Size.Width,
                54 + delta);

            this.listBox_rank.Size = new Size(this.listBox_rank.Size.Width,
                34 + delta);
        }

        private void listBox_rank_MouseEnter(object sender, EventArgs e)
        {
            var delta = this.listBox_rank.Items.Count * 14;
            if (delta > 256) delta = 256;

            ChangeRankHeight(delta);
        }

        private void listBox_rank_MouseLeave(object sender, EventArgs e)
        {
            ChangeRankHeight(0);

            this.Focus();
            this.Select();
        }

        private void listBox_rank_MouseMove(object sender, MouseEventArgs e)
        {
            // 리스트 박스가 갱신되더라도 선택된 아이템이 있다면 스크롤이 초기화되지 않는 현상을 이용한다.
            // 자동으로 그러기위해서 마우스를 누르지 않고 움직이기만 하더라도 해당 아이템을 선택하게 한다.

            int focusedIndex = this.listBox_rank.IndexFromPoint(e.Location);

            if (focusedIndex >= 0 && focusedIndex < this.listBox_rank.Items.Count)
            {
                this.listBox_rank.SelectedIndex = focusedIndex;
            }
        }

        //##################################################################################

        private void WhenShutdownMessageChanged(string msg, bool shutdownFlag)
        {
            MessageBox.Show(msg, "From server");
        }

        //##################################################################################

        private void UpdateRank()
        {
            this.listBox_rank.BeginUpdate();


            var rank = m_gameMap.GetRank();

            while (rank.Count > this.listBox_rank.Items.Count)
                this.listBox_rank.Items.Add("");
            while (rank.Count < this.listBox_rank.Items.Count)
                this.listBox_rank.Items.RemoveAt(0);

            int index = 0;
            foreach (var info in rank)
            {
                this.listBox_rank.Items[index] = string.Format("{0}: \"{1}\" #{2}",
                    index + 1, info.Key, info.Value);


                ++index;
            }


            this.listBox_rank.EndUpdate();
        }
    }
}
