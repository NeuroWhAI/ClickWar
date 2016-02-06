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

        protected Util.DBHelper m_db = new Util.DBHelper();

        //##################################################################################

        private void Form_Start_Load(object sender, EventArgs e)
        {
            m_db.Connect();


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


                    CheckUpdate();
                    

                    // 일반공지 확인 후 표시
                    var noticeDoc = m_db.GetDocument("Server", "Notice");
                    if (noticeDoc == null)
                    {
                        noticeDoc = m_db.CreateDocument("Server", "Notice",
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


                    // 서버선택 화면으로 넘어감
                    var serverMapForm = new Form_ServerMap();
                    serverMapForm.Show();

                    this.Hide();
                }
            }
        }

        //##################################################################################

        protected void CheckUpdate()
        {
            // 업데이트 확인
            string[] versionData = new string[]
            {
                "", "", "", ""
            };

            int index = 0;

            foreach (char ch in Application.ProductVersion)
            {
                if (ch == '.')
                {
                    ++index;

                    if (index >= 4)
                        break;
                }
                else
                {
                    versionData[index] += ch;
                }
            }

            var versionDoc = m_db.GetDocument("Publish", "Publish");
            if (versionDoc == null)
            {
                versionDoc = m_db.CreateDocument("Publish", "Publish",
                    new MongoDB.Bson.BsonDocument
                    {
                        { "v0", versionData[0] },
                        { "v1", versionData[1] },
                        { "v2", versionData[2] },
                        { "v3", versionData[3] },
                        { "Download", "http://blog.naver.com/tlsehdgus321" }
                    });
            }
            else
            {
                bool updateExist = false;

                for (int i = 0; i < 4; ++i)
                {
                    int thisVersion = int.Parse(versionData[i]);
                    int serverVersion = int.Parse(versionDoc["v" + i].AsString);

                    if (thisVersion < serverVersion)
                    {
                        updateExist = true;

                        break;
                    }
                    else if (thisVersion > serverVersion)
                    {
                        break;
                    }
                }

                if (updateExist)
                {
                    var dlgResult = MessageBox.Show("업데이트가 있습니다.\n다운로드 하시겠습니까?", "Info",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dlgResult == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(versionDoc["Download"].AsString);

                        Application.Exit();
                    }
                    else
                    {
                        MessageBox.Show("최신버전의 클라이언트로만 접속할 수 있습니다.", "Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        Application.Exit();
                    }
                }
            }
        }
    }
}
