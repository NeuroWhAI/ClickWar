using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ClickWar
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool bSuccess;
            string mutexName = "ClickWar_Mutex_Prevent_Multiple";
            Mutex mutex = new Mutex(true, mutexName, out bSuccess);

#if DEBUG
            bSuccess = true;
#endif

            if (bSuccess)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form_Start());

                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("프로그램이 이미 실행 중입니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}
