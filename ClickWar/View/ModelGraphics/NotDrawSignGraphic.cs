using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar.View.ModelGraphics
{
    public class NotDrawSignGraphic : ModelGraphic
    {
        public NotDrawSignGraphic()
        {

        }

        //##################################################################################

        public override void DrawSign(Graphics g, Font font, string sign, int x, int y)
        {
            // empty
        }
    }
}
