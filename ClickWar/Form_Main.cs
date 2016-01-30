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
    public partial class Form_Main : Form
    {
        public Form_Main(string playerName)
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            InitializeComponent();


            this.MouseWheel += Form_Main_MouseWheel;


            m_powerWayButtons[0] = this.button_powerWayHere;
            m_powerWayButtons[1] = this.button_powerWayUp;
            m_powerWayButtons[2] = this.button_powerWayRight;
            m_powerWayButtons[3] = this.button_powerWayDown;
            m_powerWayButtons[4] = this.button_powerWayLeft;

            WhenPowerWayButtonClick(m_powerWayButtons[0]);


            this.Text = "Click War - " + Application.ProductVersion;


            m_playerName = playerName;
            this.label_playerName.Text = string.Format("\"{0}\"", playerName);
        }

        //##################################################################################

        protected Button[] m_powerWayButtons = new Button[5];
        protected int m_powerWayNum = 0;

        //##################################################################################

        protected Util.DBHelper m_db = new Util.DBHelper();

        protected Game.GameMap m_gameMap = null;

        protected View.MapViewer m_mapViewer = new View.MapViewer();

        //##################################################################################

        private bool m_bCanClick = true;
        protected int m_clickCount = 0, m_prevClickCount = 0;

        protected string m_playerName = "";

        //##################################################################################

        protected readonly object m_lockObj = new object();
        protected bool m_runningThread = true;
        protected Thread m_gameThread = null;

        protected List<Action> m_threadWorkList = new List<Action>();

        //##################################################################################

        protected bool m_bMoveCam = false;
        protected Point m_prevCursor;

        //##################################################################################

        protected void ThreadJob()
        {
            while (m_runningThread)
            {
                int workCount = m_threadWorkList.Count;

                for (int i = 0; i < workCount; ++i)
                {
                    if (m_threadWorkList[i] == null)
                        continue;


                    var job = m_threadWorkList[i];

                    job();


                    lock (m_lockObj)
                    {
                        m_threadWorkList[i] = null;
                    }


                    Thread.Sleep(10);
                }


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
            m_playerName = playerName;
            this.label_playerName.Text = string.Format("\"{0}\"", playerName);


            m_gameMap.SyncAll(m_db);

            m_mapViewer.TileSize = 48;


            this.timer_update.Start();
            this.timer_updateSlower.Start();
        }

        //##################################################################################

        private void Form_Main_Load(object sender, EventArgs e)
        {
            m_gameThread = new Thread(ThreadJob);
            m_gameThread.Start();


            m_gameMap = new Game.GameMap();


            // 뷰가 게임 이벤트를 받을 수 있도록 등록.
            m_gameMap.WhenTileCaptured += m_mapViewer.WhenTileCaptured;
            m_gameMap.WhenTileUnderAttack += m_mapViewer.WhenTileUnderAttack;
            m_gameMap.WhenTileUpgraded += m_mapViewer.WhenTileUpgraded;

            // 폼이 게임 이벤트를 받을 수 있도록 등록.
            m_gameMap.WhenShutdownMessageChanged += WhenShutdownMessageChanged;


            m_db.Connect();

            m_mapViewer.GameMap = m_gameMap;
            m_mapViewer.Location = new Point(12, 76);
            m_mapViewer.TileSize = 48;


            m_gameMap.SyncAll(m_db);


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
            m_mapViewer.DrawMap(e.Graphics, m_playerName, m_clickCount, this.Size);
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void timer_updateSlower_Tick(object sender, EventArgs e)
        {
            // 동기화
            AddThreadWork(() => m_gameMap.SyncAll(m_db), 0); // 비동기 작업

            if (m_gameMap.ShutdownFlag)
                Application.Exit();

            // UI 갱신
            this.label_playerPower.Text = m_gameMap.GetPlayerPower(m_playerName).ToString();

            // 랭킹 갱신
            UpdateRank();
        }

        //##################################################################################

        private void Form_Main_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 클릭 횟수 증가
                if (m_clickCount < 10)
                    ++m_clickCount;

                if (m_bCanClick)
                {
                    // 다음 갱신까지 클릭이벤트를 받지 않도록 설정.
                    m_bCanClick = false;

                    // 클릭 처리 (비동기)
                    m_prevClickCount = m_clickCount;
                    bool bSuccess = AddThreadWork(() => {
                        m_mapViewer.OnClick(e, m_prevClickCount, m_db, m_playerName, m_powerWayNum, this.Size);
                        m_bCanClick = true;
                    }, 1);

                    // 클릭처리작업 추가 성공시
                    if (bSuccess)
                    {
                        // 클릭 횟수 초기화
                        m_clickCount = 0;
                    }
                    else
                    {
                        // 클릭작업 추가에 실패했으므로 재시도 할 수 있도록 설정.
                        m_bCanClick = true;
                    }
                }
            }
        }

        private void Form_Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_bMoveCam)
            {
                m_mapViewer.Location = new Point(m_mapViewer.Location.X + e.X - m_prevCursor.X,
                    m_mapViewer.Location.Y + e.Y - m_prevCursor.Y);

                m_prevCursor = e.Location;
            }

            m_mapViewer.OnMove(e, m_db, m_playerName);
        }

        //##################################################################################

        protected void ViewScaleChange(int delta)
        {
            if (delta < 0)
            {
                if (m_mapViewer.TileSize > -delta + 1)
                {
                    m_mapViewer.TileSize += delta;
                }
            }
            else
            {
                if (m_mapViewer.TileSize < 256)
                {
                    m_mapViewer.TileSize += delta;
                }
            }

            // 확대/축소에 따라 화면도 보던 곳을 추적.
            int camMoveX = delta * (m_mapViewer.Location.X - this.Size.Width / 2) / m_mapViewer.TileSize;
            int camMoveY = delta * (m_mapViewer.Location.Y - this.Size.Height / 2) / m_mapViewer.TileSize;

            m_mapViewer.Location = new Point(m_mapViewer.Location.X + camMoveX, m_mapViewer.Location.Y + camMoveY);
        }

        private void button_scaleUp_Click(object sender, EventArgs e)
        {
            ViewScaleChange(4);
        }

        private void button_scaleDown_Click(object sender, EventArgs e)
        {
            ViewScaleChange(-4);
        }

        private void Form_Main_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                ViewScaleChange(-4);
            }
            else if(e.Delta > 0)
            {
                ViewScaleChange(4);
            }
        }

        //##################################################################################

        private void button_logout_Click(object sender, EventArgs e)
        {
            // 갱신 정지
            this.timer_update.Stop();
            this.timer_updateSlower.Stop();


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
            m_mapViewer.ReColorAll();
        }

        //##################################################################################

        private void Form_Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                m_bMoveCam = true;

                m_prevCursor = e.Location;
            }
        }

        private void Form_Main_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_bMoveCam = false;
        }

        private void Form_Main_MouseLeave(object sender, EventArgs e)
        {
            m_bMoveCam = false;
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

                    m_powerWayNum = index;
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


            this.listBox_rank.Items.Clear();


            var rank = m_gameMap.GetRank();

            int index = 0;
            foreach (var info in rank)
            {
                this.listBox_rank.Items.Add(string.Format("{0}: \"{1}\" #{2}",
                    index + 1, info.Key, info.Value));


                ++index;
            }


            this.listBox_rank.EndUpdate();
        }
    }
}
