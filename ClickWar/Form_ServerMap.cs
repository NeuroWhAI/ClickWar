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
    public partial class Form_ServerMap : Form
    {
        public Form_ServerMap()
        {
            InitializeComponent();
        }

        //##################################################################################

        protected Util.DBHelper m_db;

        //##################################################################################

        protected MongoDB.Bson.BsonArray GetServerList()
        {
            var serverListDoc = m_db.GetDocument("ServerList", "List");
            if (serverListDoc == null)
            {
                serverListDoc = m_db.CreateDocument("ServerList", "List",
                    new MongoDB.Bson.BsonDocument()
                    {
                        { "Servers", new MongoDB.Bson.BsonArray(0) }
                    });


                return null;
            }
            else
            {
                return serverListDoc["Servers"].AsBsonArray;
            }
        }

        //##################################################################################

        private void Form_ServerMap_Load(object sender, EventArgs e)
        {
            Reset();
        }

        public void Reset()
        {
            m_db = new Util.DBHelper();
            m_db.Connect();


            var serverArr = GetServerList();
            if (serverArr == null)
            {
                this.comboBox_server.Items.Clear();
                this.comboBox_server.Items.Add("No data");
            }
            else
            {
                this.comboBox_server.BeginUpdate();


                this.comboBox_server.Items.Clear();

                foreach (var serverInfo in serverArr)
                {
                    this.comboBox_server.Items.Add(serverInfo["Name"].AsString);
                }


                this.comboBox_server.EndUpdate();
            }

            if (this.comboBox_server.Items.Count > 0)
                this.comboBox_server.SelectedIndex = 0;
        }

        //##################################################################################

        private void button_enter_Click(object sender, EventArgs e)
        {
            string serverAddress = null;
            string addressKey = null;

            var serverArr = GetServerList();

            foreach (var serverInfo in serverArr)
            {
                if (serverInfo["Name"] == this.comboBox_server.Text)
                {
                    serverAddress = serverInfo["Address"].AsString;
                    addressKey = serverInfo["Key"].AsString;
                }
            }


            if (serverAddress != null && addressKey != null)
            {
                // 로그인 화면 띄우기
                this.Hide();

                foreach (Form form in Application.OpenForms)
                {
                    if (form is Form_Login)
                    {
                        ((Form_Login)form).ResetDB(serverAddress, addressKey);
                        form.Show();
                        return;
                    }
                }

                var loginForm = new Form_Login(serverAddress, addressKey);
                loginForm.Show();
            }
            else
            {
                MessageBox.Show("해당 서버가 존재하지 않습니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }
}
