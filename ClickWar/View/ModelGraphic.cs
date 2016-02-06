using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ClickWar.View
{
    public class ModelGraphic
    {
        public ModelGraphic()
        {

        }

        //##################################################################################

        public virtual Brush GetBrushOfFriendTile(Game.GameTile tile, Color color, out bool needRelease)
        {
            needRelease = true;
            return new SolidBrush(color);
        }

        public virtual Brush GetBrushOfEnemyTile(Game.GameTile tile, Color color, out bool needRelease)
        {
            // 타일의 힘이 약할수록 색이 투명해짐.
            int alpha = 120 + tile.Power / 2;
            if (alpha < 120) alpha = 120;
            else if (alpha > 255) alpha = 255;

            color = Color.FromArgb(color.ToArgb() & 0x00ffffff | (alpha << 24));


            needRelease = true;
            return new SolidBrush(color);
        }

        //##################################################################################

        public virtual void DrawSign(Graphics g, Font font, string sign, int x, int y)
        {
            var boxSize = TextRenderer.MeasureText(sign, font);
            int boxWidth = boxSize.Width + 4;
            int boxHeight = boxSize.Height + 8;

            g.FillPie(Brushes.SandyBrown,
                x - 16,
                y - 14,
                32,
                28,
                270 - 20,
                40);

            g.FillRectangle(Brushes.SandyBrown,
                x - boxWidth / 2,
                y - boxHeight - 8,
                boxWidth,
                boxHeight);

            g.DrawString(sign, font, Brushes.Black,
                x,
                y - boxHeight - 4,
                new StringFormat()
                {
                    Alignment = StringAlignment.Center
                });
        }

        //##################################################################################

        public virtual void DrawTileInfoSimply(Graphics g, Font font, Game.GameTile tile,
            int x, int y)
        {
            string infoText;

            if (tile != null && tile.HaveOwner)
                infoText = string.Format("Owner: {0}\nPower: {1}", tile.Owner, tile.Power);
            else
                infoText = "[Empty]";

            g.DrawString(infoText, font, Brushes.Black,
                x, y);
        }

        //##################################################################################

        public virtual void DrawClickCount(Graphics g, int count, int x, int y)
        {
            using (Pen pen = new Pen(Color.DarkBlue, 3.0f))
            {
                g.DrawPie(pen, x, y, 32.0f, 32.0f,
                    65.0f, count * 36.0f);
            }
        }
    }
}
