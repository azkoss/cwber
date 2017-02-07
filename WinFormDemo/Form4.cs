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
using System.Threading;

namespace WinFormDemo
{
    public partial class Form4 : Form
    {
        ChromeWebBrowser browser = new ChromeWebBrowser();
        int st;
        public Form4()
        {
            InitializeComponent();
           
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            
            //没有标题
            this.FormBorderStyle = FormBorderStyle.None;
            //任务栏不显示
            this.ShowInTaskbar = false;
            this.Height = int.Parse(WinFormDemo.Properties.Settings.Default.height); ;
            this.Width = int.Parse(WinFormDemo.Properties.Settings.Default.width);
            OpenUrl(Properties.Settings.Default.url);
        }
        delegate void NewPageListener(string url, object request);
        public void OpenUrl(string url)
        {
            NewPageListener a = new NewPageListener(NewPage);
            this.Invoke(a, new object[] { url, null });
        }
        //初始化IC卡读卡器
        public int InitIC()
        {
            int flag = 0;
            DllClass.InitIc();
            flag = DllClass.IcDev;
           
            DllClass.Beep(DllClass.IcDev);
            return flag;
        }

        public void NewPage(string newUrl, object req)
        {

            CSharpBrowserSettings settings = new CSharpBrowserSettings();
            settings.CachePath = @"C:\temp\caches";


            browser.Initialize(settings);
            this.panel1.Controls.Add(browser);
            browser.Validate();
            browser.Dock = DockStyle.Fill;
            if (!newUrl.Contains("&"))
                browser.OpenUrl(newUrl);
            else
                browser.OpenUrl(req);

        }

        public int InitYBIntfDll()
        {
            var sOut = 0;
            sOut = DllClass.InitYBIntfDll();
            return sOut;

        }
        public StringBuilder YYT_YB_GET_PATI()
        {
            var sOut = new StringBuilder(10240);
            var s = "<?xml version='1.0' encoding='gb2312'?>" +
                      "<root>" +
                      "<commitdata>" +
                      "<data>";
            s += "<datarow hao_ming ='09' code_value ='' patient_name='' />"; 
            s += "</data>" +
                 "</commitdata>" +
                 "<returndata/>" +
                 "<operateinfo>" +
                 "<info method='YYT_YB_GET_PATI' opt_id='70000' opt_name='ZZJ01' opt_ip='80000001' opt_date='" + DateTime.Now.ToString("yyyy-MM-dd") +
                 "' guid='{" +
                 Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds) +
                 "}' token='AUTO-YYRMYY-20140701'  />" +
                 "</operateinfo>" +
                 "<result>" +
                 "<info />" +
                 "</result>" +
                 "</root>";
            DllClass.ChisYbRequestData(new StringBuilder("10002"), new StringBuilder("12345"),
                   new StringBuilder("YYT_YB_GET_PATI"), new StringBuilder(s), sOut);
            return sOut;
        }

        public StringBuilder YYT_YB_SF_CALC(string s)
        {
           
            string[] ss = s.Split('&');
            var sOut = new StringBuilder(10240);

            string s2 = "<?xml version='1.0' encoding='gb2312'?><root><commitdata><data><datarow patient_id='"+ss[0]+"' card_code='"+ss[1]+"' card_no='"+ss[2]+"' query_type='2'  times='"+ss[3]+"' start_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' end_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' pay_seq='"+ss[4]+"' times_order_no='"+ss[5]+"'/></data></commitdata><returndata/><operateinfo><info method='YYT_YB_SF_CALC' opt_id='70000' opt_name='ZZJ01' opt_ip='80000001' opt_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' guid='{" + Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds) + "}' token='AUTO-YYRMYY-" + DateTime.Now.ToString("yyyy-MM-dd") + "'  /></operateinfo><result><info /></result></root>";
            DllClass.ChisYbRequestData(new StringBuilder("10002"), new StringBuilder("12345"),new StringBuilder("YYT_YB_SF_CALC"), new StringBuilder(s2), sOut);
            return sOut;
        }

        public StringBuilder YYT_YB_SF_SAVE(string s)
        {
           
            string[] ss = s.Split('&');
            MessageBox.Show("测试");
            MessageBox.Show(s);
            var sOut = new StringBuilder(10240);
            string s2 = "<?xml version='1.0' encoding='gb2312'?><root><commitdata><data><datarow pay_seq='" + ss[0] + "' patient_id='" + ss[1] + "' card_code='20' card_no='" + ss[2] + "' responce_type='" + ss[3] + "'  times='" + ss[4] + "' charge_total='" + ss[5] + "' cash='" + ss[6] + "' zhzf='" + ss[7] + "' tczf='" + ss[8] + "' bk_card_no='' trade_no='" + ss[11] + "' stream_no='" + ss[10] + "' addition_no1='' trade_time='" + DateTime.Now.ToString("yyyy-MM-dd") + "' cheque_type='" + ss[9] + "' yb_flag='0' start_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' end_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' bank_type=''/></data></commitdata><returndata/><operateinfo><info method='YYT_SF_SAVE' opt_id='70000' opt_name='ZZJ01' opt_ip='800000001' opt_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' guid='" + ss[10] + "' token='AUTO-YYRMYY-" + DateTime.Now.ToString("yyyy-MM-dd") + "'  /></operateinfo><result><info /></result></root>";
            DllClass.ChisYbRequestData(new StringBuilder("10002"), new StringBuilder("12345"), new StringBuilder("YYT_YB_SF_SAVE"), new StringBuilder(s2), sOut);
            return sOut;
        }
        public String GetCardNo()
        {
           
            
            string CardNo = "";
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
            

        public void ShowMessage(string msg)
        {

            Shown sh = new Shown();
            sh.label1.Text = msg;
            sh.ShowDialog();

        }



    }
 }

