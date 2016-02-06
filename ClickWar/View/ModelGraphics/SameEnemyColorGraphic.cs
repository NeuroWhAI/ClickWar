using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar.View.ModelGraphics
{
    public class SameEnemyColorGraphic : ModelGraphic
    {
        public SameEnemyColorGraphic()
        {

        }

        //##################################################################################

        public override Brush GetBrushOfEnemyTile(Game.GameTile tile, Color color, out bool needRelease)
        {
            return base.GetBrushOfEnemyTile(tile, Color.OrangeRed, out needRelease);
        }
    }
}
