using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sashulin;
using Sashulin.common;
using WinFormDemo.CommUtil;
using System.Threading;


namespace WinFormDemo
{
    /// <summary> 
    /// 在客户端HTML页面调用WebBrowser方法类 
    /// </summary> 
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    //[ComVisible(true)]
    public partial class Form3 : Form
    {
        int st;
        private volatile bool canStop = false;
        public Form3()
        {
            
            InitializeComponent();

        }

        private void Form3_Load(object sender, EventArgs e)
        {
           //没有标题
            this.FormBorderStyle = FormBorderStyle.None; 
            //任务栏不显示
            this.ShowInTaskbar = false;
            this.Height = int.Parse(WinFormDemo.Properties.Settings.Default.height); ;
            this.Width = int.Parse(WinFormDemo.Properties.Settings.Default.width);
            this.Left = 0;
            this.Top =0;
            this.webBrowser1.Navigate(WinFormDemo.Properties.Settings.Default.url);

            DllClass.InitIc();
            //1.初始化
            st = DllClass.IcDev;
            GetCardNo();
            


        }
        public void Beep()
        {
            st = DllClass.IcDev;
            if (st > 0)
            {
                DllClass.Beep(st);
            }
        }

        public String GetCardNo()
        {
            string CardNo = "";
            canStop = false;
            try
            {
                Thread t = new Thread(delegate()
                {

                    while (!canStop)
                    {
                        ulong icCardNo = 0;
                        char tt = (char)0;
                        uint ss = 0;
                        st = DllClass.dc_reset(DllClass.IcDev, ss);
                        Thread.Sleep(500);
                        st = DllClass.dc_card(DllClass.IcDev, tt, ref icCardNo);
                        if (icCardNo != 0)
                        {
                            Beep();
                            CardNo = "H" + icCardNo;
                            object[] objects = new object[1];
                            objects[0] = CardNo;

                            this.Invoke((EventHandler)delegate
                            {
                                this.webBrowser1.Document.InvokeScript("fill_input", objects);
                            });
                            Thread.Sleep(2000);
                        }
                       
                    }
                    Thread.Sleep(500);
                    //DllClass.KeybordClose();


                });
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



            try
            {
                ulong icCardNo = 0;
                char tt = (char) 0;
                uint ss = 0;
                st = DllClass.dc_reset(DllClass.IcDev, ss);
                Thread.Sleep(500);
                st = DllClass.dc_card(DllClass.IcDev, tt, ref icCardNo);
                if (icCardNo != 0)
                {

                    CardNo = "H" + icCardNo;

                }
                
            }
            catch (Exception ex)
            {
                CardNo = "";
                MessageBox.Show(ex.Message);

            }
            return CardNo;
        }



      

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            keybords kb = new keybords();
            kb.Show();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScrollBarsEnabled = false;
            webBrowser1.ObjectForScripting = this;
        }

    }


}
