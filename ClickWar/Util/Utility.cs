using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar.Util
{
    public class Utility
    {
        public static Random Random = new Random();

        //##################################################################################

        public static Color GetRandomColor()
        {
            int green = Random.Next(0, 255);
            return Color.FromArgb(Random.Next(200, 255), green, Math.Abs(green - 255));
        }
    }
}
