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
        protected List<KeyValuePair<string, Color>> m_eventList = new List<KeyValuePair<string, Color>>();

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
                    using (Brush brh = new SolidBrush(eventInfo.Value))
                    {
                        g.DrawString(eventInfo.Key, m_font, brh,
                            location.X, location.Y - index * (m_font.Height + 2));
                    }

                    ++index;
                }


                if (m_timer.ElapsedMilliseconds > 4000)
                {
                    m_eventList.RemoveAt(m_eventList.Count - 1);

                    m_timer.Restart();
                }
            }
        }

        //##################################################################################

        public void AddEvent(string text, Color textColor)
        {
            m_eventList.Insert(0, new KeyValuePair<string, Color>(text, textColor));

            if (m_eventList.Count > 9)
                m_eventList.RemoveAt(m_eventList.Count - 1);
            else if (m_eventList.Count <= 1)
                m_timer.Restart();
        }

        public void ClearEvent()
        {
            m_eventList.Clear();
        }
    }
}
