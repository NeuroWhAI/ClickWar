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

            foreach (var brh in m_playerColor)
            {
                brh.Value.Dispose();
            }
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

        protected Dictionary<string, Brush> m_playerColor = new Dictionary<string, Brush>();

        //##################################################################################

        protected readonly Random m_random = new Random();

        protected Point m_focusedCoord = new Point(-1, -1);
        protected Brush m_piaColor = Brushes.Aqua;

        protected Font m_font = new Font(SystemFonts.DefaultFont.Name, 8, FontStyle.Regular);

        //##################################################################################

        protected List<CircleEffect> m_circleEffectList = new List<CircleEffect>();

        //##################################################################################

        public void OnClick(MouseEventArgs e, Util.DBHelper db, string playerName, int powerWay)
        {
            if (this.GameMap == null)
                return;


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
                        this.GameMap.AddPowerAt(db, x, y, 1);
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

            this.GameMap.SyncAll(db);
        }

        public void OnMove(MouseEventArgs e, Util.DBHelper db, string playerName)
        {
            if (this.GameMap == null)
                return;


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

        public int DrawMap(Graphics g, string playerName, Size screenSize)
        {
            if (this.GameMap == null)
                return -1;


            int width = this.GameMap.Width;
            int height = this.GameMap.Height;

            int startX = -Location.X / TileSize;
            int startY = -Location.Y / TileSize;

            if (startX < 0) startX = 0;
            if (startY < 0) startY = 0;

            int endX = startX + screenSize.Width / TileSize;
            int endY = startY + screenSize.Height / TileSize;


            // 타일 정보 그리기
            for (int w = startX; w <= endX; ++w)
            {
                for (int h = startY; h <= endY; ++h)
                {
                    var tile = this.GameMap.GetTileAt(w, h);

                    if (tile != null)
                    {
                        // 타일 색 채우기
                        Brush fillColor = null;

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
                                Color newColor = Color.FromArgb(m_random.Next(200, 255), m_random.Next(0, 255), m_random.Next(0, 255));
                                m_playerColor.Add(tile.Owner, new SolidBrush(newColor));
                            }
                            
                            fillColor = m_playerColor[tile.Owner];
                        }

                        if (fillColor != null)
                        {
                            g.FillRectangle(fillColor, Location.X + w * TileSize,
                                Location.Y + h * TileSize, TileSize, TileSize);
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


            // 격자 그리기
            g.DrawRectangle(Pens.Black, Location.X, Location.Y,
                width * TileSize, height * TileSize);

            /*for (int w = 1; w < width; ++w)
            {
                int lineX = Location.X + w * TileSize;

                g.DrawLine(Pens.Black, lineX, Location.Y, lineX, Location.Y + height * TileSize);
            }

            for (int h = 1; h < height; ++h)
            {
                int lineY = Location.Y + h * TileSize;

                g.DrawLine(Pens.Black, Location.X, lineY, Location.X + width * TileSize, lineY);
            }*/


            // 효과 그리기 및 갱신
            List<CircleEffect> eraseEffectList = new List<CircleEffect>();
            foreach (var circleEffect in m_circleEffectList)
            {
                circleEffect.Draw(g);

                // 효과가 끝났으면 삭제목록에 추가
                if (circleEffect.IsEnd)
                    eraseEffectList.Add(circleEffect);
            }

            foreach (var eraseEffect in eraseEffectList)
            {
                m_circleEffectList.Remove(eraseEffect);
            }


            return 0;
        }

        //##################################################################################

        public void AddCircleEffect(CircleEffect effect)
        {
            m_circleEffectList.Add(effect);
        }
    }
}
