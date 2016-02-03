using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace ClickWar.View
{
    public class GameEventViewer
    {
        public GameEventViewer()
        {
            
        }

        ~GameEventViewer()
        {
            m_font.Dispose();
        }

        //##################################################################################

        protected Stopwatch m_timer = new Stopwatch();
        protected List<Tuple<string, Color, int>> m_eventList = new List<Tuple<string, Color, int>>();
        protected float m_alpha = 255.0f;

        protected Font m_font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

        //##################################################################################

        public void Draw(Graphics g, Point location)
        {
            if (m_eventList.Count > 0)
            {
                var tempEventList = m_eventList.ToList();

                int index = 0;
                foreach (var eventInfo in tempEventList)
                {
                    using (Brush brh = new SolidBrush(Color.FromArgb(eventInfo.Item2.ToArgb() & 0x00ffffff | ((int)m_alpha << 24))))
                    {
                        string text = string.Format("{0}", eventInfo.Item1);

                        if (eventInfo.Item3 > 1)
                            text = string.Format("{0} ({1})", eventInfo.Item1, eventInfo.Item3);

                        g.DrawString(text, m_font, brh,
                            location.X, location.Y - index * (m_font.Height + 2));
                    }

                    ++index;
                }
            }


            if (m_alpha > 0.0f && m_timer.ElapsedMilliseconds > 4000)
            {
                m_alpha -= 1.0f;

                if (m_alpha < 0.0f)
                {
                    m_alpha = 0.0f;

                    m_timer.Reset();
                }
            }
        }

        //##################################################################################

        public void AddEvent(string text, Color textColor)
        {
            int count = 1;

            if (m_eventList.Count > 0)
            {
                if (m_eventList[0].Item1 == text)
                {
                    count = m_eventList[0].Item3 + 1;

                    m_eventList.RemoveAt(0);
                }
            }

            m_eventList.Insert(0, new Tuple<string, Color, int>(text, textColor, count));


            if (m_eventList.Count > 9)
                m_eventList.RemoveAt(m_eventList.Count - 1);

            m_timer.Restart();
            m_alpha = 255.0f;
        }

        public void ClearEvent()
        {
            m_eventList.Clear();
        }
    }
}
