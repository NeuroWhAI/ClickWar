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
        public Form_Login(string encryptedAddressDB, string key)
        {
            InitializeComponent();


            ResetDB(encryptedAddressDB, key);


            this.Text = "Click War - " + Application.ProductVersion;
        }

        //##################################################################################

        protected Util.DBHelper m_db = new Util.DBHelper();
        protected string m_encryptedAddress, m_key;

        //##################################################################################

        public void ResetDB(string encryptedAddressDB, string key)
        {
            m_encryptedAddress = encryptedAddressDB;
            m_key = key;

            m_db.Connect(encryptedAddressDB, key);
        }

        private void Form_Login_Load(object sender, EventArgs e)
        {
            // 자동 로그인 확인
            this.checkBox_autoLogin.Checked = Util.RegistryHelper.GetDataAsBool("AutoLoginFlag", false);
            if (this.checkBox_autoLogin.Checked)
            {
                // 인터페이스 비활성화
                DisableUI();
                
                // 레지스트리에서 로그인 정보 가져옴
                this.textBox_name.Text = Util.RegistryHelper.GetData("LoginName", "");

                var key = Util.RegistryHelper.GetData("LoginKey", "");
                this.textBox_password.Text = Util.EncoderDecoder.Decode(Util.RegistryHelper.GetData("LoginPass", ""),
                Enumerable.Range(0, key.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(key.Substring(x, 2), 16))
                     .ToArray());

                // 계정정보 확인
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
                // 인터페이스 비활성화
                DisableUI();

                // 계정정보 확인
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
            if (this.button_login.Enabled)
            {
                this.Hide();

                foreach (Form form in Application.OpenForms)
                {
                    if (form is Form_ServerMap)
                    {
                        form.Show();
                        return;
                    }
                }

                var serverMapForm = new Form_ServerMap();
                serverMapForm.Show();
            }
            else
            {
                Application.Exit();
            }
        }

        //##################################################################################

        protected void EnableUI()
        {
            this.textBox_name.Invoke(new MethodInvoker(() => this.textBox_name.Enabled = true));
            this.textBox_password.Invoke(new MethodInvoker(() => this.textBox_password.Enabled = true));
            this.button_login.Invoke(new MethodInvoker(() => this.button_login.Enabled = true));
        }

        protected void DisableUI()
        {
            this.textBox_name.Invoke(new MethodInvoker(() => this.textBox_name.Enabled = false));
            this.textBox_password.Invoke(new MethodInvoker(() => this.textBox_password.Enabled = false));
            this.button_login.Invoke(new MethodInvoker(() => this.button_login.Enabled = false));
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
                    ((Form_Main)form).ResetGame(m_encryptedAddress, m_key, this.textBox_name.Text);
                    form.Show();
                    return;
                }
            }

            Form_Main mainForm = new Form_Main(m_encryptedAddress, m_key, this.textBox_name.Text);
            mainForm.Show();
        }

        protected void WhenReceiveDocument(MongoDB.Bson.BsonDocument userDoc)
        {
            string userName = this.textBox_name.Text;
            string userPassword = this.textBox_password.Text;


            // 아이피 얻어옴
            var hostIPList = Util.Utility.GetHostIPList();

            // 아이피 배열 만듬
            MongoDB.Bson.BsonArray ipArr = new MongoDB.Bson.BsonArray(hostIPList);


            if (userDoc == null)
            {
                var selection = MessageBox.Show(@"해당 계정이 없습니다.
현재 정보로 가입하시겠습니까?
※ 자주 쓰시는 계정정보로 설정하시면 보안문제가 있을 수 있습니다.", "Login Error!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (selection == DialogResult.Yes)
                {
                    RijndaelManaged aes = new RijndaelManaged();
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.GenerateKey();

                    List<MongoDB.Bson.BsonDocument> overlapDocList;

                    // 계정 중복 확인
                    if (m_db.CheckCountIf("Users", "Unique", ipArr, out overlapDocList) > 0)
                    {
                        // 중복

                        var retryResult = MessageBox.Show(@"동일한 IP로 등록된 다른 계정이 존재하여 진행할 수 없습니다.
이전 계정을 삭제하고 이 정보로 다시 가입을 시도하시겠습니까?
※ 이전 계정이 소유한 타일은 전부 초기화됩니다.",
                            "Login Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (retryResult == DialogResult.Yes)
                        {
                            DeleteUser(overlapDocList);
                        }
                        else
                        {
                            EnableUI();
                            return;
                        }
                    }


                    // 개인정보수집 동의
                    if (MessageBox.Show(@"[안내]
계정의 중복을 확인하기 위해서
로그인시 현재 PC의 IP주소와 MAC주소를 DB로 전송합니다.
계정의 정보를 확인하는 것 외의 목적으로 사용되지 않습니다.
동의하시겠습니까?", "Agreement", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        EnableUI();
                        return;
                    }


                    // 계정정보 암호화
                    string encryptKey = BitConverter.ToString(aes.Key).Replace("-", "");
                    string encryptedPass = Util.EncoderDecoder.Encode(userPassword, aes.Key);

                    // 계정 생성
                    userDoc = m_db.CreateDocument("Users", userName,
                        new MongoDB.Bson.BsonDocument
                        {
                            { "Name", userName },
                            { "Pass", encryptedPass },
                            { "Key", encryptKey },
                            { "Unique", ipArr }
                        });

                    // 계정 생성 실패시
                    if (userDoc == null)
                    {
                        MessageBox.Show("계정을 생성할 수 없습니다.", "DB Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        EnableUI();
                        return;
                    }
                }
                else
                {
                    EnableUI();
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
                if (userDoc["Name"] != userName
                    ||
                    decryptedPass != userPassword)
                {
                    MessageBox.Show("로그인 정보가 올바르지 않습니다.", "Login Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    EnableUI();
                    return;
                }
                else
                {
                    // 계정의 아이피 정보 갱신
                    if (userDoc["Unique"] != ipArr)
                    {
                        userDoc["Unique"] = ipArr;

                        m_db.UpdateDocument("Users", userName, userDoc);
                    }

                    // 계정 중복 확인
                    List<MongoDB.Bson.BsonDocument> overlapDocList;
                    if (m_db.CheckCountIf("Users", "Unique", ipArr, out overlapDocList) > 1)
                    {
                        var retryResult = MessageBox.Show(@"동일한 IP로 등록된 다른 계정이 존재하여 진행할 수 없습니다.
이전 계정을 삭제하고 이 정보로 다시 가입을 시도하시겠습니까?
※ 이전 계정이 소유한 타일은 전부 초기화됩니다.",
    "Login Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (retryResult == DialogResult.Yes)
                        {
                            // 현재 계정은 삭제될 계정 목록에서 제거
                            overlapDocList.RemoveAll(delegate (MongoDB.Bson.BsonDocument doc)
                            {
                                return (doc["Name"] == userName);
                            });

                            
                            DeleteUser(overlapDocList);
                        }
                        else
                        {
                            EnableUI();
                            return;
                        }
                    }
                }
            }


            // 여기까지 왔으면 로그인 성공


            EnableUI();


            // 자동 로그인 정보 갱신
            this.Invoke(new MethodInvoker(() => this.UpdateAutoLogin()));


            // 게임 화면 띄우기
            this.Invoke(new MethodInvoker(() => this.SequenceToGame()));
        }

        protected void DeleteUser(List<MongoDB.Bson.BsonDocument> docList)
        {
            // 임시로 게임 상태를 저장하고 서버를 갱신할 게임맵을 생성.
            // db를 생성자에 넘겨줌으로서 동기화가 이루어진다.
            Game.GameMap tempGame = new Game.GameMap(m_db);

            // 이전 계정 삭제
            foreach (var doc in docList)
            {
                string deceasedName = doc["Name"].AsString;

                m_db.DeleteDocumentAsync("Users", deceasedName);

                // 해당 계정이 소유하는 타일 초기화
                tempGame.DeleteAllOf(m_db, deceasedName);
            }
        }
    }
}
