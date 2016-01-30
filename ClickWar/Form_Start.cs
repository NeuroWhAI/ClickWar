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
    public partial class Form_Start : Form
    {
        public Form_Start()
        {
            InitializeComponent();


            this.label_underText.Text = Application.ProductName + " - " + Application.ProductVersion;
        }

        //##################################################################################

        private void Form_Start_Load(object sender, EventArgs e)
        {
            this.timer_update.Start();
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1.0)
            {
                this.Opacity += 0.02;

                if (this.Opacity >= 1.0)
                {
                    this.Opacity = 1.0;

                    this.timer_update.Stop();

                    
                    Util.DBHelper db = new Util.DBHelper();
                    db.Connect();

                    var noticeDoc = db.GetDocument("Server", "Notice");
                    if (noticeDoc == null)
                    {
                        noticeDoc = db.CreateDocument("Server", "Notice",
                            new MongoDB.Bson.BsonDocument()
                            {
                                { "Title", "" },
                                { "Message", "" }
                            });
                    }
                    else
                    {
                        var title = noticeDoc["Title"].AsString;
                        var msg = noticeDoc["Message"].AsString;

                        // 메세지가 있으면
                        if (title.Length > 0 || msg.Length > 0)
                        {
                            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }


                    Form_Login loginForm = new Form_Login();
                    loginForm.Show();

                    this.Hide();
                }
            }
        }
    }
}
