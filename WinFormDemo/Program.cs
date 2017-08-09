using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sashulin;

namespace WinFormDemo
{
    
    static class Program
    {
        public static string[] mainArgs;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
//            if (Properties.Settings.Default.testMode == true)
//            {
//                Application.Run(new PdfPrintTest());
//            }
//            else
//            {
                Application.Run(new Form5());
//            }
            
//            Application.Run(new PrintTest());
//            Application.Run(new TiaoXingMa());
//            Application.Run(new PdfPrintTest());
        }
    }
}
