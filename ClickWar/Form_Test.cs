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


            /*RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateKey();

            string hexData = Util.EncoderDecoder.Encode("접속 주소", aes.Key);
            this.textBox1.Text = hexData;

            var chArr = BitConverter.ToString(aes.Key).Replace("-", "").ToCharArray();
            Array.Reverse(chArr);
            this.textBox2.Text = new string(chArr);*/
        }
    }
}
