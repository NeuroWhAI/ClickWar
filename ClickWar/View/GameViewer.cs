using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ClickWar.View
{
    public class GameViewer
    {
        public GameViewer()
        {
            this.Location = new Point(0, 0);
            this.TileSize = 1;
        }

        ~GameViewer()
        {
            m_font.Dispose();
        }

        //##################################################################################

        protected GameEventViewer m_eventViewer = new GameEventViewer();

        //##################################################################################

        protected Point m_location;
        public Point Location
        {
            get { return m_location; }
            set
            {
                if (m_location != null)
                {
                    int deltaX = value.X - m_location.X;
                    int deltaY = value.Y - m_location.Y;

                    foreach (var effect in m_circleEffectList)
                    {
                        effect.AddLocation(deltaX, deltaY);
                    }
                }

                m_location = value;
            }
        }

        public int TileSize
        { get; set; } = 48;

        //##################################################################################

        protected Dictionary<string, Color> m_playerColor = new Dictionary<string, Color>();

        //##################################################################################

        protected Point m_focusedCoord = new Point(-1, -1);
        protected Brush m_piaColor = Brushes.Aqua;

        protected Font m_font = new Font(SystemFonts.DefaultFont.Name, 8, FontStyle.Regular);

        //##################################################################################

        protected List<CircleEffect> m_circleEffectList = new List<CircleEffect>();

        //##################################################################################

        public void SetFocusTileAt(int x, int y)
        {
            m_focusedCoord.X = x;
            m_focusedCoord.Y = y;
        }

        public void SetPiaColor(Brush brh)
        {
            m_piaColor = brh;
        }

        //##################################################################################

        public void WhenTileUnderAttack(int x, int y, Game.GameTile tile)
        {
            var effect = new CircleEffect();
            effect.Start(Location.X + x * TileSize + TileSize / 2, Location.Y + y * TileSize + TileSize / 2,
                24.0f, 512.0f, Color.OrangeRed);

            AddCircleEffect(effect);


            /*if (tile.HaveOwner)
            {
                m_eventViewer.AddEvent(string.Format("\"{0}\"님의 타일 에너지 감소!", tile.Owner),
                    Color.Orange);
            }*/
        }

        public void WhenTileCaptured(int x, int y, Game.GameTile tile)
        {
            var effect = new CircleEffect();
            effect.Start(Location.X + x * TileSize + TileSize / 2, Location.Y + y * TileSize + TileSize / 2,
                48.0f, 1024.0f, Color.AliceBlue);

            AddCircleEffect(effect);


            if (tile.HaveOwner)
            {
                m_eventViewer.AddEvent(string.Format("\"{0}\"님이 영토를 차지했습니다!", tile.Owner),
                    Color.Red);
            }
        }

        public void WhenTileUpgraded(int x, int y, Game.GameTile tile)
        {
            var effect = new CircleEffect();
            effect.Start(Location.X + x * TileSize + TileSize / 2, Location.Y + y * TileSize + TileSize / 2,
                6.0f, 64.0f, Color.Yellow);

            AddCircleEffect(effect);


            /*if (tile.HaveOwner)
            {
                m_eventViewer.AddEvent(string.Format("\"{0}\"님이 영토를 강화했습니다!", tile.Owner),
                    Color.Red);
            }*/
        }

        public void WhenSignChanged(int x, int y, Game.GameTile tile)
        {
            var effect = new CircleEffect();
            effect.Start(Location.X + x * TileSize + TileSize / 2, Location.Y + y * TileSize + TileSize / 2,
                80.0f, 2048.0f, Color.WhiteSmoke);

            AddCircleEffect(effect);


            if (tile.HaveOwner && tile.HaveSign)
            {
                m_eventViewer.AddEvent(string.Format("\"{0}\": \"{1}\"", tile.Owner, tile.Sign),
                    Color.Black);
            }
        }

        //##################################################################################

        public int DrawMap(Graphics g, Game.GameMap gameMap, Point cursor,
            string playerName, int clickCount, Size screenSize)
        {
            if (gameMap == null)
                return -1;


            const int minDetailTileSize = 30;


            int width = gameMap.Width;
            int height = gameMap.Height;

            Point startPos = new Point(), endPos = new Point();
            GetIndexRect(gameMap.Width, gameMap.Height, screenSize, ref startPos, ref endPos);


            // 타일 정보 그리기
            for (int h = startPos.Y; h <= endPos.Y; ++h)
            {
                var tempTile = gameMap.GetTileAt(startPos.X, h);

                string prevOwner = "";
                if (tempTile != null)
                    prevOwner = tempTile.Owner;
                int beginX = startPos.X;
                bool bNotEnd = true;

                for (int w = startPos.X; w <= endPos.X; ++w)
                {
                    var tile = gameMap.GetTileAt(w, h);

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
                        if (TileSize > minDetailTileSize)
                        {
                            if (tile.Owner != prevOwner)
                            {
                                if (prevOwner.Length > 0)
                                {
                                    int gap = w + beginX - 1;
                                    this.DrawOwnerName(g, prevOwner,
                                       gap / 2, h, (gap % 2 != 0) ? this.TileSize / 2 : 0);
                                }

                                prevOwner = tile.Owner;
                                beginX = w;

                                bNotEnd = false;
                            }
                            else
                            {
                                bNotEnd = true;
                            }

                            if (tile.Power != 0)
                            {
                                this.DrawTilePower(g, tile.Power, w, h);
                            }
                        }
                    } // if (tile != null)
                }


                if (prevOwner.Length > 0 // 이전 이름이 있고
                    &&
                    TileSize > minDetailTileSize // 그릴 수 있는 정도의 배율이고
                    &&
                    (bNotEnd || beginX == endPos.X)) // 이전 이름을 못 그렸다면
                {
                    if (beginX == endPos.X) ++beginX;
                    int gap = endPos.X + beginX - 1;

                    this.DrawOwnerName(g, prevOwner,
                        gap / 2, h, (gap % 2 != 0) ? this.TileSize / 2 : 0);
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
                    var tile = gameMap.GetTileAt(w, h);

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
                        var nearTile = gameMap.GetTileAt(nearX[i], nearY[i]);

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


            // 표지판 그리기
            if (TileSize > minDetailTileSize)
            {
                for (int w = startPos.X; w <= endPos.X; ++w)
                {
                    for (int h = startPos.Y; h <= endPos.Y; ++h)
                    {
                        var tile = gameMap.GetTileAt(w, h);

                        if (tile == null)
                            continue;


                        if (tile.HaveSign)
                        {
                            int locationX = Location.X + w * TileSize + TileSize / 2;
                            int locationY = Location.Y + h * TileSize + TileSize / 2;

                            var boxSize = TextRenderer.MeasureText(tile.Sign, m_font);
                            int boxWidth = boxSize.Width + 4;
                            int boxHeight = boxSize.Height + 8;

                            g.FillPie(Brushes.SandyBrown,
                                locationX - 16,
                                locationY - 14,
                                32,
                                28,
                                270 - 20,
                                40);

                            g.FillRectangle(Brushes.SandyBrown,
                                locationX - boxWidth / 2,
                                locationY - boxHeight - 8,
                                boxWidth,
                                boxHeight);

                            g.DrawString(tile.Sign, m_font, Brushes.Black,
                                locationX,
                                locationY - boxHeight - 4,
                                new StringFormat()
                                {
                                    Alignment = StringAlignment.Center
                                });
                        }
                    }
                }
            }


            // 효과 그리기 및 갱신
            var circleEffectListClone = m_circleEffectList.ToArray();
            foreach (var circleEffect in circleEffectListClone)
            {
                circleEffect.Draw(g, (float)TileSize / 48.0f);

                // 효과가 끝났으면 삭제목록에 추가
                if (circleEffect.IsEnd)
                    m_circleEffectList.Remove(circleEffect);
            }


            // 이벤트 그리기
            m_eventViewer.Draw(g, new Point(8, 200));


            // 클릭 카운트 표시
            using (Pen pen = new Pen(Color.DarkBlue, 3.0f))
            {
                g.DrawPie(pen, cursor.X - 16.0f, cursor.Y - 16.0f, 32.0f, 32.0f,
                    65.0f, clickCount * 36.0f);
            }


            return 0;
        }

        protected void DrawOwnerName(Graphics g, string name, int w, int h,
            int offsetX = 0)
        {
            g.DrawString(string.Format("\"{0}\"", name), m_font,
                Brushes.Black,
                Location.X + w * TileSize + TileSize / 2 + offsetX,
                Location.Y + h * TileSize + TileSize / 2 - 6,
                new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
        }

        protected void DrawTilePower(Graphics g, int power, int w, int h)
        {
            g.DrawString(string.Format("#{0}", power), m_font,
                Brushes.Black,
                Location.X + w * TileSize + TileSize / 2,
                Location.Y + h * TileSize + TileSize / 2 + 9,
                new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
        }

        //##################################################################################

        public void AddCircleEffect(CircleEffect effect)
        {
            m_circleEffectList.Add(effect);
        }

        public void ClearEffectAndEvent()
        {
            m_circleEffectList.Clear();
            m_eventViewer.ClearEvent();
        }

        public void ReColorAll()
        {
            List<string> nameList = new List<string>(); 

            foreach(var colorInfo in m_playerColor)
            {
                nameList.Add(colorInfo.Key);
            }

            m_playerColor.Clear();

            foreach (var name in nameList)
            {
                m_playerColor[name] = Util.Utility.GetRandomColor();
            }
        }

        public void GetIndexRect(int boardWidth, int boardHeight, Size screenSize, ref Point startPos, ref Point endPos)
        {
            int startX = -Location.X / TileSize;
            int startY = -Location.Y / TileSize;

            int endX = startX + screenSize.Width / TileSize + 2;
            int endY = startY + screenSize.Height / TileSize + 2;

            if (startX < 0) startX = 0;
            if (startY < 0) startY = 0;

            if (endX < 0) endX = 0;
            else if (endX >= boardWidth) endX = boardWidth - 1;
            if (endY < 0) endY = 0;
            else if (endY >= boardHeight) endY = boardHeight - 1;


            startPos.X = startX;
            startPos.Y = startY;

            endPos.X = endX;
            endPos.Y = endY;
        }
    }
}
