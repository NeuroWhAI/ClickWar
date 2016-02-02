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
    public partial class Form_Sign : Form
    {
        public Form_Sign(IStringReceiver receiver, int tileX, int tileY)
        {
            InitializeComponent();


            m_receiver = receiver;
            m_tileIndex = new Point(tileX, tileY);
        }

        //##################################################################################

        protected IStringReceiver m_receiver;
        protected Point m_tileIndex;

        //##################################################################################

        private void button_confirm_Click(object sender, EventArgs e)
        {
            m_receiver.WhenReceiveSignInfo(this.textBox_sign.Text, m_tileIndex.X, m_tileIndex.Y);

            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //##################################################################################

        public void SetInputText(string text)
        {
            this.textBox_sign.Text = text;

            this.textBox_sign.SelectAll();
        }
    }
}
