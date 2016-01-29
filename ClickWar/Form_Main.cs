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


            this.Text = "Click War - " + Application.ProductVersion;


            m_playerName = playerName;
            this.label_playerName.Text = string.Format("\"{0}\"", playerName);
        }

        //##################################################################################

        protected Util.DBHelper m_db = new Util.DBHelper();

        protected Game.GameMap m_gameMap = null;

        protected View.MapViewer m_mapViewer = new View.MapViewer();

        //##################################################################################

        private bool m_bCanClick = true;

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
                while (m_threadWorkList.Count > 0)
                {
                    lock(m_lockObj)
                    {
                        m_threadWorkList[0]();
                        m_threadWorkList.RemoveAt(0);

                        m_bCanClick = true;
                    }

                    Thread.Sleep(10);
                }


                Thread.Sleep(32);
            }
        }

        protected void AddThreadWork(Action work)
        {
            lock(m_lockObj)
            {
                m_threadWorkList.Add(work);
            }
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

            m_runningThread = false;
            m_gameThread.Join();

            Application.Exit();
        }

        //##################################################################################

        private void Form_Main_Paint(object sender, PaintEventArgs e)
        {
            m_mapViewer.DrawMap(e.Graphics, m_playerName, this.Size);
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            this.Invalidate();

            //m_bCanClick = true;
        }

        private void timer_updateSlower_Tick(object sender, EventArgs e)
        {
            // 동기화
            //m_gameMap.SyncAll(m_db);
            AddThreadWork(() => m_gameMap.SyncAll(m_db));

            // UI 갱신
            this.label_playerPower.Text = m_gameMap.GetPlayerPower(m_playerName).ToString();
        }

        //##################################################################################

        private void Form_Main_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && m_bCanClick)
            {
                //m_mapViewer.OnClick(e, m_db, m_playerName);
                AddThreadWork(() => m_mapViewer.OnClick(e, m_db, m_playerName));

                // 다음 갱신까지 클릭이벤트를 받지 않도록 설정.
                lock(m_lockObj)
                {
                    m_bCanClick = false;
                }

                // 자동 동기화 타이머 리셋
                this.timer_updateSlower.Stop();
                this.timer_updateSlower.Start();
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

        private void button_scaleUp_Click(object sender, EventArgs e)
        {
            if (m_mapViewer.TileSize < 256)
            {
                m_mapViewer.TileSize += 4;
            }
        }

        private void button_scaleDown_Click(object sender, EventArgs e)
        {
            if (m_mapViewer.TileSize > 5)
            {
                m_mapViewer.TileSize -= 4;
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
    }
}
