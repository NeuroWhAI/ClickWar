using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar.View
{
    public class CircleEffect
    {
        public CircleEffect()
        {

        }

        //##################################################################################

        protected Point m_circlePos = new Point(0, 0);
        protected float m_circleSize = -1.0f;

        protected float m_endSize = 128.0f;
        protected float m_scaleSpeed = 10.0f;

        protected Color m_color;

        //##################################################################################

        public void AddLocation(int deltaX, int deltaY)
        {
            m_circlePos.X += deltaX;
            m_circlePos.Y += deltaY;
        }

        //##################################################################################

        public void Start(int x, int y, float scaleUpSpeed, float endSize, Color color,
            float startSize = 4.0f)
        {
            m_circlePos.X = x;
            m_circlePos.Y = y;

            m_circleSize = startSize;


            m_endSize = endSize;
            m_scaleSpeed = scaleUpSpeed;


            m_color = color;
        }

        public void Draw(Graphics g, float scale)
        {
            // 클릭효과 그리기 및 갱신
            if (m_circleSize > 0.0f)
            {
                using (Brush brh = new SolidBrush(Color.FromArgb(m_color.A - (int)(m_circleSize / m_endSize * (float)m_color.A),
                    m_color.R, m_color.G, m_color.B)))
                {
                    int scaledSize = (int)(scale * m_circleSize);

                    g.FillEllipse(brh, m_circlePos.X - scaledSize / 2,
                        m_circlePos.Y - scaledSize / 2,
                        scaledSize, scaledSize);

                    m_circleSize += m_scaleSpeed;
                    if (m_circleSize > m_endSize)
                        m_circleSize = -1.0f;
                }
            }
        }

        public bool IsEnd
        {
            get { return (m_circleSize < 0.0f); }
        }
    }
}
