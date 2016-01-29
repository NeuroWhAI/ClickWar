﻿using System;
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

        protected Point m_circlePos;
        protected float m_circleSize = -1.0f;

        protected float m_endSize = 128.0f;
        protected float m_scaleSpeed = 10.0f;

        protected Color m_color;

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

        public void Draw(Graphics g)
        {
            // 클릭효과 그리기 및 갱신
            if (m_circleSize > 0.0f)
            {
                using (Brush brh = new SolidBrush(Color.FromArgb(m_color.A - (int)(m_circleSize / m_endSize * (float)m_color.A),
                    m_color.R, m_color.G, m_color.B)))
                {
                    g.FillEllipse(brh, m_circlePos.X - m_circleSize / 2, m_circlePos.Y - m_circleSize / 2,
                        m_circleSize, m_circleSize);

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