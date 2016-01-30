using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ClickWar.View
{
    public class MapViewer
    {
        public MapViewer()
        {
            this.Location = new Point(0, 0);
            this.TileSize = 1;

            this.GameMap = null;
        }

        public MapViewer(Game.GameMap map)
        {
            this.Location = new Point(0, 0);
            this.TileSize = 1;

            this.GameMap = map;
        }

        ~MapViewer()
        {
            m_font.Dispose();

            /*foreach (var brh in m_playerColor)
            {
                brh.Value.Dispose();
            }*/
        }

        //##################################################################################

        public Point Location
        { get; set; }

        public int TileSize
        { get; set; }

        //##################################################################################

        public Game.GameMap GameMap
        { get; set; }

        //##################################################################################

        protected Dictionary<string, Color> m_playerColor = new Dictionary<string, Color>();

        //##################################################################################

        protected Point m_focusedCoord = new Point(-1, -1);
        protected Brush m_piaColor = Brushes.Aqua;

        protected Font m_font = new Font(SystemFonts.DefaultFont.Name, 8, FontStyle.Regular);

        //##################################################################################

        protected List<CircleEffect> m_circleEffectList = new List<CircleEffect>();

        //##################################################################################

        protected Point m_cursor = new Point(0, 0);

        //##################################################################################

        public void OnClick(MouseEventArgs e, int clickCount, Util.DBHelper db, string playerName, int powerWay, Size screenSize)
        {
            if (this.GameMap == null)
                return;


            m_cursor = e.Location;


            int x = (e.X - Location.X) / TileSize;
            int y = (e.Y - Location.Y) / TileSize;

            var tile = this.GameMap.GetTileAt(x, y);

            if (tile != null)
            {
                if (tile.Owner == playerName)
                {
                    if (powerWay == 0)
                    {
                        // 타일 강화
                        this.GameMap.AddPowerAt(db, x, y, clickCount);
                    }
                    else
                    {
                        // 타일 흡수
                        this.GameMap.AbsorbTile(db, x, y, powerWay, playerName);
                    }
                }
                else if (this.GameMap.GetPlayerPower(playerName) <= 0)
                {
                    const int buildCost = 100;

                    // 주인없는 땅이면 즉시 건국하고
                    // 있으면 buildCost보다 낮은 땅일시 그만큼 소모하고 건국
                    if (tile.Power < buildCost)
                    {
                        this.GameMap.SetOwnerAt(db, x, y, playerName);
                        this.GameMap.AddPowerAt(db, x, y, buildCost - tile.Power);
                    }
                }
                else
                {
                    // 점령/공격
                    this.GameMap.AttackTile(db, x, y, playerName);
                }
            }

            //this.GameMap.SyncAll(db);
            Point startPos = new Point(), endPos = new Point();
            GetIndexRect(screenSize, ref startPos, ref endPos);
            this.GameMap.SyncTileRect(db, startPos, endPos);
        }

        public void OnMove(MouseEventArgs e, Util.DBHelper db, string playerName)
        {
            if (this.GameMap == null)
                return;


            m_cursor = e.Location;


            int x = (e.X - Location.X) / TileSize;
            int y = (e.Y - Location.Y) / TileSize;

            var tile = this.GameMap.GetTileAt(x, y);

            if (tile != null)
            {
                m_focusedCoord.X = x;
                m_focusedCoord.Y = y;

                if (this.GameMap.GetPlayerPower(playerName) <= 0)
                {
                    // 건국 모드
                    m_piaColor = Brushes.MediumPurple;
                }
                else if (tile.Owner == playerName)
                {
                    // 아군 지역
                    m_piaColor = Brushes.Green;
                }
                else if (tile.Owner.Length <= 0)
                {
                    // 중립 지역
                    m_piaColor = Brushes.Aqua;
                }
                else
                {
                    // 적군 지역
                    m_piaColor = Brushes.Red;
                }
            }
            else
            {
                m_focusedCoord.X = -1;
                m_focusedCoord.Y = -1;

                m_piaColor = Brushes.Aqua;
            }
        }

        //##################################################################################

        public void WhenTileUnderAttack(int x, int y)
        {
            var effect = new CircleEffect();
            effect.Start(Location.X + x * TileSize + TileSize / 2, Location.Y + y * TileSize + TileSize / 2,
                6.0f, 100.0f, Color.OrangeRed);

            AddCircleEffect(effect);
        }

        public void WhenTileCaptured(int x, int y)
        {
            var effect = new CircleEffect();
            effect.Start(Location.X + x * TileSize + TileSize / 2, Location.Y + y * TileSize + TileSize / 2,
                12.0f, 200.0f, Color.AliceBlue);

            AddCircleEffect(effect);
        }

        public void WhenTileUpgraded(int x, int y)
        {
            var effect = new CircleEffect();
            effect.Start(Location.X + x * TileSize + TileSize / 2, Location.Y + y * TileSize + TileSize / 2,
                10.0f, 64.0f, Color.Yellow);

            AddCircleEffect(effect);
        }

        //##################################################################################

        public int DrawMap(Graphics g, string playerName, int clickCount, Size screenSize)
        {
            if (this.GameMap == null)
                return -1;


            int width = this.GameMap.Width;
            int height = this.GameMap.Height;

            Point startPos = new Point(), endPos = new Point();
            GetIndexRect(screenSize, ref startPos, ref endPos);


            // 타일 정보 그리기
            for (int w = startPos.X; w <= endPos.X; ++w)
            {
                for (int h = startPos.Y; h <= endPos.Y; ++h)
                {
                    var tile = this.GameMap.GetTileAt(w, h);

                    if (tile != null)
                    {
                        // 타일 색 채우기
                        Brush fillColor = null;
                        bool needDispose = false;

                        if (w == m_focusedCoord.X && h == m_focusedCoord.Y)
                        {
                            // 포커스된 타일
                            fillColor = m_piaColor;
                        }
                        else if (tile.Owner == playerName)
                        {
                            // 아군 지역
                            fillColor = Brushes.GreenYellow;
                        }
                        else if (tile.Owner.Length > 0)
                        {
                            // 적군 지역

                            // 처음 그리는 유저라면 색을 만듬.
                            if (!m_playerColor.ContainsKey(tile.Owner))
                            {
                                m_playerColor.Add(tile.Owner, Util.Utility.GetRandomColor());
                            }

                            Color color = m_playerColor[tile.Owner];

                            int alpha = 120 + tile.Power / 2;
                            if (alpha < 120) alpha = 120;
                            else if (alpha > 255) alpha = 255;

                            color = Color.FromArgb(color.ToArgb() & 0x00ffffff | (alpha << 24));

                            fillColor = new SolidBrush(color);
                            needDispose = true;
                        }

                        if (fillColor != null)
                        {
                            g.FillRectangle(fillColor, Location.X + w * TileSize,
                                Location.Y + h * TileSize, TileSize, TileSize);

                            if (needDispose)
                                fillColor.Dispose();
                        }


                        // 타일 공간이 어느정도 있어야 세부정보를 표시함.
                        if (TileSize > 30)
                        {
                            if (tile.Owner.Length > 0)
                            {
                                g.DrawString(string.Format("\"{0}\"", tile.Owner), m_font,
                                    Brushes.Black,
                                    Location.X + w * TileSize + TileSize / 2,
                                    Location.Y + h * TileSize + TileSize / 2 - 6,
                                    new StringFormat()
                                    {
                                        Alignment = StringAlignment.Center,
                                        LineAlignment = StringAlignment.Center
                                    });
                            }

                            if (tile.Power != 0)
                            {
                                g.DrawString(string.Format("#{0}", tile.Power), m_font,
                                    Brushes.Black,
                                    Location.X + w * TileSize + TileSize / 2,
                                    Location.Y + h * TileSize + TileSize / 2 + 9,
                                    new StringFormat()
                                    {
                                        Alignment = StringAlignment.Center,
                                        LineAlignment = StringAlignment.Center
                                    });
                            }
                        }
                    }
                }
            }


            // 격자 그리기
            g.DrawRectangle(Pens.Black, Location.X, Location.Y,
                width * TileSize, height * TileSize);


            // 영토 경계선 그리기
            for (int w = startPos.X; w <= endPos.X; ++w)
            {
                for (int h = startPos.Y; h <= endPos.Y; ++h)
                {
                    var tile = this.GameMap.GetTileAt(w, h);

                    if (tile == null)
                        continue;


                    int[] nearX = new int[]
                    {
                        w, w + 1//, w, w - 1
                    };
                    int[] nearY = new int[]
                    {
                        h - 1, h//, h + 1, h
                    };

                    for (int i = 0; i < 2; ++i)
                    {
                        var nearTile = this.GameMap.GetTileAt(nearX[i], nearY[i]);

                        if (nearTile != null && nearTile.Owner != tile.Owner)
                        {
                            switch (i)
                            {
                                case 0:
                                    g.DrawLine(Pens.Black,
                                        Location.X + w * TileSize, Location.Y + h * TileSize,
                                        Location.X + w * TileSize + TileSize, Location.Y + h * TileSize);
                                    break;

                                case 1:
                                    g.DrawLine(Pens.Black,
                                        Location.X + w * TileSize + TileSize, Location.Y + h * TileSize,
                                        Location.X + w * TileSize + TileSize, Location.Y + h * TileSize + TileSize);
                                    break;
                            }
                        }
                    }
                }
            }


            // 효과 그리기 및 갱신
            var circleEffectListClone = m_circleEffectList.ToArray();
            foreach (var circleEffect in circleEffectListClone)
            {
                circleEffect.Draw(g);

                // 효과가 끝났으면 삭제목록에 추가
                if (circleEffect.IsEnd)
                    m_circleEffectList.Remove(circleEffect);
            }


            // 클릭 카운트 표시
            using (Pen pen = new Pen(Color.DarkBlue, 3.0f))
            {
                g.DrawPie(pen, m_cursor.X - 16.0f, m_cursor.Y - 16.0f, 32.0f, 32.0f,
                    65.0f, clickCount * 36.0f);
            }


            return 0;
        }

        //##################################################################################

        public void AddCircleEffect(CircleEffect effect)
        {
            m_circleEffectList.Add(effect);
        }

        public void ReColorAll()
        {
            List<string> nameList = new List<string>(); 

            foreach(var colorInfo in m_playerColor)
            {
                //colorInfo.Value.Dispose();

                nameList.Add(colorInfo.Key);
            }

            m_playerColor.Clear();

            foreach (var name in nameList)
            {
                //m_playerColor[name] = new SolidBrush(Util.Utility.GetRandomColor());
                m_playerColor[name] = Util.Utility.GetRandomColor();
            }
        }

        public void GetIndexRect(Size screenSize, ref Point startPos, ref Point endPos)
        {
            int width = this.GameMap.Width;
            int height = this.GameMap.Height;

            int startX = -Location.X / TileSize;
            int startY = -Location.Y / TileSize;

            if (startX < 0) startX = 0;
            if (startY < 0) startY = 0;

            int endX = startX + screenSize.Width / TileSize;
            int endY = startY + screenSize.Height / TileSize;

            if (endX >= width) endX = width - 1;
            if (endY >= height) endY = height - 1;


            startPos.X = startX;
            startPos.Y = startY;

            endPos.X = endX;
            endPos.Y = endY;
        }
    }
}
