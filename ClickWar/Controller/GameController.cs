using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace ClickWar.Controller
{
    public class GameController
    {
        public GameController()
        {
            m_clickTimer.Start();
        }

        //##################################################################################

        protected int m_powerWayNum = 0;

        //##################################################################################

        protected string m_playerName = "";

        //##################################################################################

        protected Util.DBHelper m_db = null;
        protected Game.GameMap m_gameMap = null;
        protected View.GameViewer m_gameViewer = null;

        //##################################################################################

        protected View.ModelGraphic m_modelDrawer = new View.ModelGraphic();

        //##################################################################################

        protected Stopwatch m_clickTimer = new Stopwatch();
        protected bool m_bCanClick = true;
        protected int m_clickCount = 0, m_oldClickCount = 0;
        protected Point m_oldCursor;
        protected int m_oldPowerWay;
        protected string m_oldPlayerName;
        protected Size m_oldFormSize;

        //##################################################################################

        protected bool m_bMoveCam = false;
        protected Point m_cursor;

        //##################################################################################

        public void SetPowerWay(int number)
        {
            m_powerWayNum = number;
        }

        public void SetPlayerName(string playerName)
        {
            m_playerName = playerName;
        }

        public void SetDB(Util.DBHelper db)
        {
            m_db = db;
        }

        public void SetGameMap(Game.GameMap gameMap)
        {
            m_gameMap = gameMap;
        }

        public void SetGameViewer(View.GameViewer gameViewer)
        {
            m_gameViewer = gameViewer;
        }

        //##################################################################################

        public void Init(Size formSize)
        {
            // 뷰가 게임 이벤트를 받을 수 있도록 등록.
            m_gameMap.WhenTileCaptured += m_gameViewer.WhenTileCaptured;
            m_gameMap.WhenTileUnderAttack += m_gameViewer.WhenTileUnderAttack;
            m_gameMap.WhenTileUpgraded += m_gameViewer.WhenTileUpgraded;
            m_gameMap.WhenSignChanged += m_gameViewer.WhenSignChanged;
            
            
            m_gameViewer.Location = new Point(12, 76);
            m_gameViewer.TileSize = 48;


            m_gameMap.SyncAll(m_db);


            // 동기화 함으로서 발생하는 이벤트와 이펙트 제거
            m_gameViewer.ClearEffectAndEvent();


            // 화면을 플레이어 영토로 이동
            this.MoveScreenToPlayer(formSize);
        }

        public void ResetGame(string playerName, Size formSize)
        {
            SetPlayerName(playerName);
            

            m_gameMap.SyncAll(m_db);

            m_gameViewer.Location = new Point(12, 76);
            m_gameViewer.TileSize = 48;


            // 동기화 함으로서 발생하는 이벤트와 이펙트 제거
            m_gameViewer.ClearEffectAndEvent();


            // 화면을 플레이어 영토로 이동
            this.MoveScreenToPlayer(formSize);
        }

        //##################################################################################

        public void GetFocusedLocation(out int x, out int y)
        {
            x = (m_cursor.X - m_gameViewer.Location.X) / m_gameViewer.TileSize;
            y = (m_cursor.Y - m_gameViewer.Location.Y) / m_gameViewer.TileSize;
        }

        public void WhenLeftClick(Func<Action, int, bool> threadJobPusher, int threadJobNum,
            Point cursor, Size formSize)
        {
            // 초당 클릭 수 제한
            if (m_clickTimer.ElapsedMilliseconds >= 66)
            {
                // 타이머 리셋
                m_clickTimer.Restart();

                // 클릭 횟수 증가
                if (m_clickCount < 10)
                    ++m_clickCount;
                
                if (m_bCanClick)
                {
                    // 다음 갱신까지 클릭이벤트를 받지 않도록 설정.
                    m_bCanClick = false;

                    // 현재 상태를 저장해두어 스레드가 일을 할때 이 값을 사용하도록 한다.
                    m_oldClickCount = m_clickCount;
                    m_oldCursor = new Point(cursor.X, cursor.Y);
                    m_oldPowerWay = m_powerWayNum;
                    m_oldPlayerName = m_playerName;
                    m_oldFormSize = new Size(formSize.Width, formSize.Height);

                    // 클릭 처리 (비동기)
                    bool bSuccess = threadJobPusher(() =>
                    {
                        UpdateGameByLeftClick(m_oldCursor.X, m_oldCursor.Y, m_oldFormSize,
                            m_oldClickCount, m_oldPowerWay, m_oldPlayerName);
                        m_bCanClick = true;
                    }, threadJobNum);

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

        public void WhenRightDown(Point cursor)
        {
            m_bMoveCam = true;

            m_cursor = cursor;
        }

        public void WhenRightUp(Point cursor)
        {
            m_bMoveCam = false;

            m_cursor = cursor;
        }

        public void WhenMouseMove(Point cursor)
        {
            if (m_bMoveCam)
            {
                m_gameViewer.Location = new Point(m_gameViewer.Location.X + cursor.X - m_cursor.X,
                    m_gameViewer.Location.Y + cursor.Y - m_cursor.Y);
            }


            m_cursor = cursor;


            int x, y;
            this.GetFocusedLocation(out x, out y);

            var tile = m_gameMap.GetTileAt(x, y);

            if (tile != null)
            {
                m_gameViewer.SetFocusTileAt(x, y);


                Brush piaColor = null;

                if (m_gameMap.GetPlayerPower(m_playerName) <= 0)
                {
                    // 건국 모드
                    piaColor = Brushes.MediumPurple;
                }
                else if (tile.Owner == m_playerName)
                {
                    // 아군 지역
                    piaColor = Brushes.Green;
                }
                else if (tile.Owner.Length <= 0)
                {
                    // 중립 지역
                    piaColor = Brushes.Aqua;
                }
                else
                {
                    // 적군 지역
                    piaColor = Brushes.Red;
                }

                m_gameViewer.SetPiaColor(piaColor);
            }
            else
            {
                m_gameViewer.SetFocusTileAt(-1, -1);

                m_gameViewer.SetPiaColor(Brushes.Aqua);
            }
        }

        public void WhenMouseWheelRollUp(Size formSize)
        {
            ChangeViewScale(4, formSize);
        }

        public void WhenMouseWheelRollDown(Size formSize)
        {
            ChangeViewScale(-4, formSize);
        }

        //##################################################################################

        public void SetModelGraphic(int type)
        {
            switch (type)
            {
                case 0:
                    m_modelDrawer = new View.ModelGraphic();
                    break;

                case 1:
                    m_modelDrawer = new View.ModelGraphics.ColoringPlayerTileGraphic();
                    break;

                case 2:
                    m_modelDrawer = new View.ModelGraphics.SameEnemyColorGraphic();
                    break;

                case 3:
                    m_modelDrawer = new View.ModelGraphics.NotDrawSignGraphic();
                    break;
            }
        }

        public void DrawGame(Graphics g, Size formSize)
        {
            m_gameViewer.DrawMap(g, m_modelDrawer, m_gameMap, m_cursor, m_playerName, m_clickCount, formSize);
        }

        //##################################################################################

        public void SyncGame(Size formSize)
        {
            m_gameMap.SyncAll(m_db);

            // 화면에 보이는 영역만 동기화
            //Point startPos = new Point(), endPos = new Point();
            //m_gameViewer.GetIndexRect(m_gameMap.Width, m_gameMap.Height, formSize, ref startPos, ref endPos);
            //m_gameMap.SyncAllRect(m_db, startPos, endPos);
        }

        public void ReColorAll()
        {
            m_gameViewer.ReColorAll();
        }

        public void UpdateGameByLeftClick(int cursorX, int cursorY, Size formSize,
            int clickCount, int powerWayNum, string playerName)
        {
            int x = (cursorX - m_gameViewer.Location.X) / m_gameViewer.TileSize;
            int y = (cursorY - m_gameViewer.Location.Y) / m_gameViewer.TileSize;

            var tile = m_gameMap.GetTileAt(x, y);

            if (tile != null)
            {
                if (tile.Owner == playerName)
                {
                    if (m_powerWayNum == 0)
                    {
                        // 타일 강화
                        m_gameMap.AddPowerAt(m_db, x, y, clickCount);
                    }
                    else
                    {
                        // 타일 흡수
                        m_gameMap.AbsorbTile(m_db, x, y, powerWayNum, playerName);
                    }
                }
                else if (m_gameMap.GetPlayerPower(playerName) <= 0)
                {
                    const int buildCost = 100;

                    // 건국
                    m_gameMap.BuildCountry(m_db, x, y, playerName, buildCost);
                }
                else
                {
                    // 점령/공격
                    m_gameMap.AttackTile(m_db, x, y, playerName);
                }
            }
        }

        public void SetSignAt(string text, int tileX, int tileY)
        {
            m_gameMap.BuildSignToTile(m_db, tileX, tileY, text, m_playerName);
        }

        //##################################################################################

        public void MoveScreenToPlayer(Size formSize)
        {
            int widthIndex, heightIndex;
            m_gameMap.GetPlayerLocation(m_playerName, out widthIndex, out heightIndex);

            m_gameViewer.Location = new Point(formSize.Width / 2 - widthIndex * m_gameViewer.TileSize,
                formSize.Height / 2 - heightIndex * m_gameViewer.TileSize);
        }

        public void ChangeViewScale(int delta, Size formSize)
        {
            // 확대/축소에 따라 화면도 보던 곳을 추적.
            int camMoveX = delta * (m_gameViewer.Location.X - m_cursor.X) / m_gameViewer.TileSize;
            int camMoveY = delta * (m_gameViewer.Location.Y - m_cursor.Y) / m_gameViewer.TileSize;

            // 추적값을 계산한 뒤에 타일크기를 바꿔야 제대로 된다.
            
            if (delta < 0)
            {
                if (m_gameViewer.TileSize > -delta + 1)
                {
                    m_gameViewer.TileSize += delta;
                }
            }
            else
            {
                if (m_gameViewer.TileSize < 256)
                {
                    m_gameViewer.TileSize += delta;
                }
            }

            m_gameViewer.Location = new Point(m_gameViewer.Location.X + camMoveX, m_gameViewer.Location.Y + camMoveY);
        }

        public int GetPlayerPower()
        {
            return m_gameMap.GetPlayerPower(m_playerName);
        }

        public bool IsPlayerTerritory(int x, int y)
        {
            var tile = m_gameMap.GetTileAt(x, y);

            return (tile != null && tile.Owner == m_playerName);
        }
    }
}
