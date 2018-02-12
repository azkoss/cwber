using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Sashulin;

namespace WinFormDemo
{
    
    static class Program
    {
        public static System.Threading.Mutex Run;
        public static string[] mainArgs;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool noRun = false;
            Run = new System.Threading.Mutex(true, "WinFormDemo", out noRun);

            if (noRun)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form5());
            }
            else
            {
                MessageBox.Show("WinFormDemo程序已启动！", "提示");
                Application.Exit();
            }
        }
    }
}
