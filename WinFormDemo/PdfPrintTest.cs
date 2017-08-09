using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Spire.Pdf;

namespace WinFormDemo
{
    public partial class PdfPrintTest : Form
    {
        public PdfPrintTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*FtpUtil ftpUtil = new FtpUtil();
            ftpUtil.Download(Properties.Settings.Default.testImagePath);
            pictureBox1.LoadAsync(Properties.Settings.Default.testImagePath);*/

            /*PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(Properties.Settings.Default.testImagePath);
            //doc.PrinterName = Properties.Settings.Default.printerName;
            doc.PrintDocument.PrintController = new StandardPrintController();
            string[] marInts = Properties.Settings.Default.printMargins.Split(',');
            doc.PrintDocument.DefaultPageSettings.Margins = new Margins(Convert.ToInt32(marInts[0]), Convert.ToInt32(marInts[1]), Convert.ToInt32(marInts[2]), Convert.ToInt32(marInts[3]));
            doc.PrintDocument.Print();*/

//            string strBinPath = GetFilePath();

            /*Process pr = new Process();

            pr.StartInfo.FileName = "d:\\1708889.pdf";//文件全称-包括文件后缀

            pr.StartInfo.CreateNoWindow = true;

            pr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            pr.StartInfo.Verb = "Print";

            pr.Start(); */

            string filePath = Properties.Settings.Default.testImagePath;
            pdfPrint(filePath);
        }

        private static string GetFilePath()
        {

            string filepath = AppDomain.CurrentDomain.BaseDirectory;

            int binPos = filepath.IndexOf("bin");

            string binPath = filepath.Remove(binPos);

            return binPath;

        }

        private void pdfPrint(string filePath)
        {
            PrintDocument pd = new PrintDocument();
            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = true;
            startInfo.FileName = filePath;
            startInfo.Verb = "print";
            startInfo.Arguments = @"/p /h \" + filePath + "\"\"" + pd.PrinterSettings.PrinterName + "\"";
            p.StartInfo = startInfo;
            p.Start();
            p.WaitForExit();
        }
    }
}
