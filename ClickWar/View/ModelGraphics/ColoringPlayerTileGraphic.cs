using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar.View.ModelGraphics
{
    public class ColoringPlayerTileGraphic : ModelGraphic
    {
        public ColoringPlayerTileGraphic()
        {

        }

        //##################################################################################

        public override Brush GetBrushOfFriendTile(Game.GameTile tile, Color color, out bool needRelease)
        {
            return base.GetBrushOfEnemyTile(tile, Color.YellowGreen, out needRelease);
        }
    }
}
