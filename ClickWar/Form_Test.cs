using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace ClickWar
{
    public partial class Form_Test : Form
    {
        public Form_Test()
        {
            InitializeComponent();


#if DEBUG
            /*string key = "";
            this.textBox1.Text = Util.EncoderDecoder.EncodeEx("mongodb://hacker:fuckyou231@ds053295.mongolab.com:53295/click_war_hack", out key);
            this.textBox2.Text = key;*/
#endif
        }
    }
}
