using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar
{
    public interface IStringReceiver
    {
        void WhenReceiveSignInfo(string text, int tileX, int tileY);
    }
}
