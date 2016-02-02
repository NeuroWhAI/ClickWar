using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickWar
{
    public partial class Form_Chat : Form
    {
        public Form_Chat()
        {
            InitializeComponent();
        }

        //##################################################################################

        private void Form_Chat_Load(object sender, EventArgs e)
        {
            this.webBrowser_chat.Url = new Uri("http://www.gagalive.kr/gagalive.swf?chatroom=%7E%7E%7EClick_War_Chat_Room%21");
        }
    }
}
