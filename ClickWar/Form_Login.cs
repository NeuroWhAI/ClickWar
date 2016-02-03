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
    public partial class Form_Login : Form
    {
        public Form_Login()
        {
            InitializeComponent();


            this.Text = "Click War - " + Application.ProductVersion;
        }

        //##################################################################################

        protected Util.DBHelper m_db = new Util.DBHelper();

        //##################################################################################

        private void Form_Login_Load(object sender, EventArgs e)
        {
            m_db.Connect();


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
                    if (int.Parse(versionData[i]) < int.Parse(versionDoc["v" + i].AsString))
                    {
                        updateExist = true;

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


            // 자동 로그인 확인
            this.checkBox_autoLogin.Checked = Util.RegistryHelper.GetDataAsBool("AutoLoginFlag", false);
            if (this.checkBox_autoLogin.Checked)
            {
                this.textBox_name.Enabled = false;
                this.textBox_password.Enabled = false;
                this.button_login.Enabled = false;
                
                this.textBox_name.Text = Util.RegistryHelper.GetData("LoginName", "");

                var key = Util.RegistryHelper.GetData("LoginKey", "");
                this.textBox_password.Text = Util.EncoderDecoder.Decode(Util.RegistryHelper.GetData("LoginPass", ""),
                Enumerable.Range(0, key.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(key.Substring(x, 2), 16))
                     .ToArray());

                m_db.GetDocumentAsync("Users", this.textBox_name.Text, this.WhenReceiveDocument);
            }
        }

        private void Form_Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            //m_db.WaitAllTask();
        }

        private void button_login_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (this.textBox_name.Text.Length <= 0)
            {
                this.textBox_name.Text = "뭐지What";
                this.textBox_password.Text = "abcd1234";
            }
#endif

            if (this.textBox_name.Text.Length > 0
                ||
                this.textBox_password.Text.Length > 0)
            {
                this.textBox_name.Enabled = false;
                this.textBox_password.Enabled = false;
                this.button_login.Enabled = false;

                m_db.GetDocumentAsync("Users", this.textBox_name.Text, this.WhenReceiveDocument);
            }
            else
            {
                MessageBox.Show("아이디와 비밀번호를 입력해주세요.", "Login Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //##################################################################################

        protected void UpdateAutoLogin()
        {
            if (this.checkBox_autoLogin.Checked)
            {
                Util.RegistryHelper.SetData("LoginName", this.textBox_name.Text);


                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateKey();

                string hexData = Util.EncoderDecoder.Encode(this.textBox_password.Text, aes.Key);
                Util.RegistryHelper.SetData("LoginPass", hexData);

                Util.RegistryHelper.SetData("LoginKey", BitConverter.ToString(aes.Key).Replace("-", ""));
            }
            else
            {
                Util.RegistryHelper.SetData("LoginName", "");
                Util.RegistryHelper.SetData("LoginPass", "");
                Util.RegistryHelper.SetData("LoginKey", "");
            }

            Util.RegistryHelper.SetData<bool>("AutoLoginFlag", this.checkBox_autoLogin.Checked);
        }

        protected void SequenceToGame()
        {
            this.Hide();

            foreach (Form form in Application.OpenForms)
            {
                if (form is Form_Main)
                {
                    ((Form_Main)form).ResetGame(this.textBox_name.Text);
                    form.Show();
                    return;
                }
            }

            Form_Main mainForm = new Form_Main(this.textBox_name.Text);
            mainForm.Show();
        }

        protected void WhenReceiveDocument(MongoDB.Bson.BsonDocument userDoc)
        {
            this.textBox_name.Invoke(new MethodInvoker(() => this.textBox_name.Enabled = true));
            this.textBox_password.Invoke(new MethodInvoker(() => this.textBox_password.Enabled = true));
            this.button_login.Invoke(new MethodInvoker(() => this.button_login.Enabled = true));


            if (userDoc == null)
            {
                var selection = MessageBox.Show(@"해당 계정이 없습니다.
현재 정보로 가입하시겠습니까?
주의, 자주 쓰시는 계정정보로 설정하시면 보안문제가 있을 수 있습니다.", "Login Error!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (selection == DialogResult.Yes)
                {
                    // 계정정보 암호화
                    RijndaelManaged aes = new RijndaelManaged();
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.GenerateKey();

                    string encryptKey = BitConverter.ToString(aes.Key).Replace("-", "");
                    string encryptedPass = Util.EncoderDecoder.Encode(this.textBox_password.Text, aes.Key);

                    // 계정 생성
                    userDoc = m_db.CreateDocument("Users", this.textBox_name.Text,
                        new MongoDB.Bson.BsonDocument
                        {
                                { "Name", this.textBox_name.Text },
                                { "Pass", encryptedPass },
                                { "Key", encryptKey }
                        });

                    // 계정 생성 실패시
                    if (userDoc == null)
                    {
                        MessageBox.Show("계정을 생성할 수 없습니다.", "DB Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                // 암호화된 암호 해독
                string encryptKey = userDoc["Key"].AsString;
                string decryptedPass = Util.EncoderDecoder.Decode(userDoc["Pass"].AsString,
                    Enumerable.Range(0, encryptKey.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(encryptKey.Substring(x, 2), 16))
                    .ToArray());

                // 로그인 정보 대조
                if (userDoc["Name"] != this.textBox_name.Text
                    ||
                    decryptedPass != this.textBox_password.Text)
                {
                    MessageBox.Show("로그인 정보가 올바르지 않습니다.", "Login Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
            }


            // 자동 로그인 정보 갱신
            this.Invoke(new MethodInvoker(() => this.UpdateAutoLogin()));


            // 게임 화면 띄우기
            this.Invoke(new MethodInvoker(() => this.SequenceToGame()));
        }
    }
}
