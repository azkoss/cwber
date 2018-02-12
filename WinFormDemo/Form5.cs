using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using CCNET;
using Spire.Pdf;
using WinFormDemo.Properties;

namespace WinFormDemo
{
    /// <summary> 
    /// 在客户端HTML页面调用WebBrowser方法类 
    /// </summary> 
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    //[ComVisible(true)]
    public partial class Form5 : Form
    {
        int st;
        private volatile bool canStop = false;

        private delegate void CloseDevDelegate();

        public static List<Struce.JZINFO> jz = new List<Struce.JZINFO>(); //该病人的就诊详单

        public Form5()
        {
            InitializeComponent();
            this.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\bg.png");
            // nOpenPinpad();

            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        #region 非接读卡器

        /// <summary>
        /// 初始化读卡器
        /// </summary>
        /// <returns>成功返回>0</returns>
        public int InitIC()
        {
            int flag = 0;
            try
            {
                DllClass.InitIc();
                flag = DllClass.IcDev;
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 蜂鸣器
        /// </summary>
        public void Beep()
        {
            st = DllClass.IcDev;
            if (st > 0)
            {
                DllClass.Beep(st);
            }
        }

        /// <summary>
        /// 寻卡
        /// </summary>
        /// <returns></returns>
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
                    CardNo = "" + icCardNo;
                }
            }
            catch (Exception ex)
            {
                CardNo = "";
                MessageBox.Show(ex.Message);
            }
            return CardNo;
        }

        /// <summary>
        /// 将密码装入读写模块RAM中
        /// </summary>
        /// <param name="mode">密码验证模式:0——KEYSET0的KEYA,1——KEYSET1的KEYA,2——KEYSET2的KEYA,4——KEYSET0的KEYB,5——KEYSET1的KEYB,6——KEYSET2的KEYB</param>
        /// <param name="section">扇区号</param>
        /// <returns>成功则返回 0</returns>
        public int loadKey(int mode, int section)
        {
            byte[] password = new Byte[] { };
            return DllClass.dc_load_key(DllClass.IcDev, mode, section, password);
        }

        /// <summary>
        /// 核对密码函数
        /// </summary>
        /// <param name="mode">密码验证模式:0——KEYSET0的KEYA,1——KEYSET1的KEYA,2——KEYSET2的KEYA,4——KEYSET0的KEYB,5——KEYSET1的KEYB,6——KEYSET2的KEYB</param>
        /// <param name="section">扇区号</param>
        /// <param name="password">密码</param>
        /// <returns>成功则返回 0</returns>
        public int authentication(int mode, int section, string password)
        {
            if (DllClass.dc_load_key(DllClass.IcDev, mode, section, Encoding.Default.GetBytes(password)) == 0)
            {
                return DllClass.dc_authentication(DllClass.IcDev, mode, section);
            }
            else
            {
                return -100;
            }
        }

        /// <summary>
        /// 读卡函数--集成了校验密码
        /// </summary>
        /// <param name="mode">第几套密码?魏县使用的B套密码,传4</param>
        /// <param name="section">扇区号</param>
        /// <param name="addr">块号</param>
        /// <param name="password">扇区密码</param>
        /// <returns>调用成功返回卡号,调用失败返回读卡失败</returns>
        public string dcRead(int mode, int section, int addr, string password)
        {
            if (DllClass.dc_load_key(DllClass.IcDev, mode, section, Encoding.Default.GetBytes(password)) == 0)
            {
                Thread.Sleep(200);
                if (DllClass.dc_authentication(DllClass.IcDev, mode, section) == 0)
                {
                    Thread.Sleep(200);
                    StringBuilder data = new StringBuilder();
                    DllClass.dc_read(DllClass.IcDev, addr, data);
                    return data.ToString();
                }
                else
                {
                    return "读卡失败";
                }
            }
            else
            {
                return "读卡失败";
            }
        }

        /// <summary>
        /// 往卡中写数据--没有测试过
        /// </summary>
        /// <param name="addr">M1卡——块地址（1～63）,M1S70卡—块地址（1-255） ML卡——页地址（2～11）</param>
        /// <param name="data">写入的数据</param>
        /// <returns></returns>
        public int dcWrite(int addr, string data)
        {
            return DllClass.dc_write(DllClass.IcDev, addr, data);
        }

        public static string Decode(string strDecode)
        {
            string sResult = "";
            for (int i = 0; i < strDecode.Length / 2; i++)
            {
                sResult += (char) short.Parse(strDecode.Substring(i * 2, 2),
                    global::System.Globalization.NumberStyles.HexNumber);
            }
            return sResult;
        }

        #endregion

        #region 医保dll方法

        public int InitYBIntfDll()
        {
            var sOut = 0;
            sOut = DllClass.InitYBIntfDll();
            return sOut;
        }

        public void FreeYBIntfDll()
        {
            DllClass.FreeYBIntfDll();
        }

        public String YYT_YB_GET_PATI(string zzj_id, string business_type)
        {
            var sOut = new StringBuilder(10240);
            var s = "<?xml version='1.0' encoding='gb2312'?>" +
                    "<root>" +
                    "<commitdata>" +
                    "<data>";
            s += "<datarow hao_ming ='09' code_value ='' patient_name='' business_type='" + business_type + "' />";
            s += "</data>" +
                 "</commitdata>" +
                 "<returndata/>" +
                 "<operateinfo>" +
                 "<info method='YYT_YB_GET_PATI' opt_id='" + zzj_id + "' opt_name='" + zzj_id +
                 "' opt_ip='80000001' opt_date='" + DateTime.Now.ToString("yyyy-MM-dd") +
                 "' guid='{" +
                 Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds) +
                 "}' token='AUTO-YYRMYY-" + DateTime.Now.ToString("yyyy-MM-dd") + "'  />" +
                 "</operateinfo>" +
                 "<result>" +
                 "<info />" +
                 "</result>" +
                 "</root>";
            DllClass.ChisYbRequestData(new StringBuilder("10002"), new StringBuilder("12345"),
                new StringBuilder("YYT_YB_GET_PATI"), new StringBuilder(s), sOut);
            return sOut.ToString();
        }

        public String YYT_YB_SF_CALC(string s)
        {
            string[] ss = s.Split('&');
            var sOut = new StringBuilder(10240);

            string s2 = "<?xml version='1.0' encoding='gb2312'?><root><commitdata><data><datarow patient_id='" + ss[0] +
                        "' card_code='" + ss[1] + "' card_no='" + ss[2] + "' query_type='2'  times='" + ss[3] +
                        "' start_date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' end_date='" +
                        DateTime.Now.ToString("yyyy-MM-dd") + "' pay_seq='" + ss[4] + "' times_order_no='" + ss[5] +
                        "'/></data></commitdata><returndata/><operateinfo><info method='YYT_YB_SF_CALC' opt_id='" +
                        ss[6] + "' opt_name='" + ss[6] + "' opt_ip='80000001' opt_date='" +
                        DateTime.Now.ToString("yyyy-MM-dd") + "' guid='{" +
                        Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds) +
                        "}' token='AUTO-YYRMYY-" + DateTime.Now.ToString("yyyy-MM-dd") +
                        "'  /></operateinfo><result><info /></result></root>";
            DllClass.ChisYbRequestData(new StringBuilder("10002"), new StringBuilder("12345"),
                new StringBuilder("YYT_YB_SF_CALC"), new StringBuilder(s2), sOut);
            return sOut.ToString();
        }

        public String YYT_YB_SF_SAVE(string s)
        {
            string[] ss = s.Split('&');
            var sOut = new StringBuilder(10240);
            string s2 = "<?xml version='1.0' encoding='gb2312'?><root><commitdata><data><datarow pay_seq='" + ss[0] +
                        "' patient_id='" + ss[1] + "' card_code='20' card_no='" + ss[2] + "' responce_type='" + ss[3] +
                        "'  times='" + ss[4] + "' charge_total='" + ss[5] + "' cash='" + ss[6] + "' zhzf='" + ss[7] +
                        "' tczf='" + ss[8] + "' bk_card_no='' trade_no='" + ss[11] + "' times_order_no='" + ss[12] +
                        "' stream_no='" + ss[10] + "' addition_no1='' trade_time='" +
                        DateTime.Now.ToString("yyyy-MM-dd") + "' cheque_type='" + ss[9] + "' yb_flag='0' start_date='" +
                        DateTime.Now.ToString("yyyy-MM-dd") + "' end_date='" + DateTime.Now.ToString("yyyy-MM-dd") +
                        "' bank_type=''/></data></commitdata><returndata/><operateinfo><info method='YYT_SF_SAVE' opt_id='" +
                        ss[13] + "' opt_name='" + ss[13] + "' opt_ip='800000001' opt_date='" +
                        DateTime.Now.ToString("yyyy-MM-dd") + "' guid='{" +
                        Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds) +
                        "}' token='AUTO-YYRMYY-" + DateTime.Now.ToString("yyyy-MM-dd") +
                        "'  /></operateinfo><result><info /></result></root>";
            DllClass.ChisYbRequestData(new StringBuilder("10002"), new StringBuilder("12345"),
                new StringBuilder("YYT_YB_SF_SAVE"), new StringBuilder(s2), sOut);
            return sOut.ToString();
        }

        public String YYT_YB_GH_CALC(string s)
        {
            string[] ss = s.Split('&');
            var sOut = new StringBuilder(10240);
            string s2 = "<?xml version='1.0' encoding='gb2312'?><root><commitdata><data><datarow pay_seq='" + ss[0] +
                        "' patient_id='" + ss[1] + "' card_code='" + ss[2] + "' card_no='" + ss[3] +
                        "' responce_type='" +
                        ss[4] +
                        "' /></data></commitdata><returndata/><operateinfo><info method=' YYT_YB_GH_CALC' opt_id='" +
                        ss[5] + "' opt_name='" + ss[5] + "' opt_ip='80000001' opt_date='" +
                        DateTime.Now.ToString("yyyy-MM-dd") + "' guid='{" +
                        Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds) +
                        "}' token='AUTO-YYRMYY-" + DateTime.Now.ToString("yyyy-MM-dd") +
                        "'  /></operateinfo><result><info /></result></root>";
            DllClass.ChisYbRequestData(new StringBuilder("10002"), new StringBuilder("12345"),
                new StringBuilder("YYT_YB_GH_CALC"), new StringBuilder(s2), sOut);
            return sOut.ToString();
        }

        public String YYT_YB_GH_SAVE(string s)
        {
            string[] ss = s.Split('&');
            var sOut = new StringBuilder(10240);
//            string s2 = "<?xml version='1.0' encoding='gb2312'?><root><commitdata><data><datarow pay_seq='" + ss[0] +
//                        "' patient_id='" + ss[1] + "' card_code='20' card_no='" + ss[2] + "' responce_type='" + ss[3] +
//                        "'  times='" + ss[4] + "' charge_total='" + ss[5] + "' cash='" + ss[6] + "' zhzf='" + ss[7] +
//                        "' tczf='" + ss[8] + "' bk_card_no='' trade_no='" + ss[11] + "' times_order_no='" + ss[12] +
//                        "' stream_no='" + ss[10] + "' addition_no1='' trade_time='" +
//                        DateTime.Now.ToString("yyyy-MM-dd") + "' cheque_type='" + ss[9] + "' yb_flag='0' start_date='" +
//                        DateTime.Now.ToString("yyyy-MM-dd") + "' end_date='" + DateTime.Now.ToString("yyyy-MM-dd") +
//                        "' bank_type=''/></data></commitdata><returndata/><operateinfo><info method='YYT_SF_SAVE' opt_id='" +
//                        ss[13] + "' opt_name='" + ss[13] + "' opt_ip='800000001' opt_date='" +
//                        DateTime.Now.ToString("yyyy-MM-dd") + "' guid='{" +
//                        Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds) +
//                        "}' token='AUTO-YYRMYY-" + DateTime.Now.ToString("yyyy-MM-dd") +
//                        "'  /></operateinfo><result><info /></result></root>";
            string s2 =
                "<?xml version='1.0' encoding='gb2312'?><root><commitdata><data><datarow record_sn='" + ss[0] +
                "' pay_seq='" + ss[1] + "' responce_type='" + ss[2] + "' patient_id='" + ss[3] + "' card_code='" +
                ss[4] + "' card_no='" + ss[5] + "' charge_total='" + ss[6] + "' cash='" + ss[7] + "' zhzf='" + ss[8] +
                "' tczf='" + ss[9] + "'  record_id='" + ss[10] + "' bk_card_no='' trade_no='" + ss[11] +
                "' stream_no='" +
                ss[12] + "' addition_no1='' trade_time='" +
                DateTime.Now.ToString("yyyy-MM-dd") +
                "' cheque_type='c' gh_flag='1' bank_type=''/></data></commitdata><returndata/><operateinfo><info method='YYT_YB_GH_SAVE' opt_id='" +
                ss[13] + "' opt_name='" + ss[13] + "' opt_ip = '800000001' opt_date = '" +
                DateTime.Now.ToString("yyyy-MM-dd") + "' guid='{" +
                Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds) +
                "}' token='AUTO-YYRMYY-" + DateTime.Now.ToString("yyyy-MM-dd") +
                "' /></operateinfo><result><info /></result></root>";
            DllClass.ChisYbRequestData(new StringBuilder("10002"), new StringBuilder("12345"),
                new StringBuilder("YYT_YB_GH_SAVE"), new StringBuilder(s2), sOut);
            return sOut.ToString();
        }

        #endregion

        #region 密码键盘

        public void nOpenPinpad()
        {
            DllClass.nOpenPinpad();
            DllClass.nAUTHInstal(true);
            DllClass.Init();
        }

        public void nTextInput()
        {
            string str = "";
            canStop = false;
            try
            {
                Thread t = new Thread(delegate()
                {
                    while (!canStop)
                    {
                        str = DllClass.nTextInput();
                        if (str.Trim() != "")
                        {
                            object[] objects = new object[1];
                            objects[0] = str;

                            this.Invoke(
                                (EventHandler) delegate { this.webBrowser1.Document.InvokeScript("secr_c", objects); });
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
        }

        private void CloseDev()
        {
            int rRun = DllClass.KeybordClose();
        }

        public void keybord_close()
        {
            canStop = true;
            //CloseDev(); 
        }

        #endregion

        #region 调用默认打印机执行打印

        //缴费打印的方法
        public void PrtJzDateCreat(string CardCode = "", string PatName = "", string PatSex = "",
            string reponcename = "", string cardNo = "", string Mx = "", string zhzf = "", string tczf = "",
            string charge_total = "", string pay_charge_total = "", string idcard = "", string alipay_total = "",
            string trade_no = "", string stream_no = "", string alipay_status = "", string alipay_tuihuan_status = "",
            string yb_status = "", string pay_status = "", string OperAtor = "", string payType = "")
        {
            //MessageBox.Show(CardCode + "#" + PatName + "#" + PatSex + "#" + reponcename + "#" + cardNo + "#" + Mx + "#" + zhzf + "#" + tczf + "#" + charge_total + "#" + pay_charge_total + "#" + idcard + "#" + alipay_total + "#" + trade_no + "#" + stream_no + "#" + alipay_status + "#" + alipay_tuihuan_status + "#" + yb_status + "#" + pay_status + "#" + OperAtor);
            string tmp1 = "";
            PrintLpt pr1 = new PrintLpt();
            int pos = 0;
            int mx_count = 0;
            decimal AllMoney = 0;
            String strPrintData = "<?xml version='1.0' encoding='utf-8'?>" + "<print_info width='300' height='500'>";
            strPrintData += "<tr font='黑体' size='14' x='65' y='" + Convert.ToString(pos) + "' >北京市海淀医院(自助)</tr>";
            if (alipay_status.Contains("超时") || alipay_tuihuan_status.Contains("超时"))
            {
                strPrintData += "<tr font='黑体' size='20' x='50' y='" + Convert.ToString(pos += 25) + "'>交易失败</tr>";
                strPrintData += "<tr font='黑体' size='20' x='50' y='" + Convert.ToString(pos += 30) + "'>请去收费处咨询</tr>";
            }
            strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 30) + "' >姓名：" +
                            PatName.Trim() + "  性别：" + PatSex + "  身份：" + reponcename + "</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >就诊号：" + cardNo +
                            "</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >缴费日期：" +
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</tr>";
            if (Mx.Length == 0)
            {
                strPrintData += "<tr font='黑体' size='8' x='20' y='" + Convert.ToString(pos += 20) + "' >" + "获取缴费明细失败" +
                                "</tr>";
            }
            else
            {
                //缴费详单
                strPrintData += "<tr font='黑体' size='12' x='100' y='" + Convert.ToString(pos += 30) +
                                "' >收 费 明 细 单</tr>";
                strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
                                "' >---------------------------------------------</tr>";
                strPrintData += "<tr font='黑体' size='8' x='20' y='" + Convert.ToString(pos += 20) +
                                "' >类型 名称                     单位     单价   数量</tr>";
                string[] array = Mx.Split('|');
                if (array.Length > 0)
                {
                    foreach (string s in array)
                    {
                        string[] array1 = s.Split('&');
                        mx_count++;
                        pos += 20;
                        strPrintData += "<tr font='黑体' size='7' x='20' y='" + Convert.ToString(pos) + "' >" +
                                        array1[0] + "</tr>";
                        if (array1[1].Trim() != "")
                        {
                            strPrintData += "<tr font='黑体' size='7' x='170' y='" + Convert.ToString(pos) + "' >" +
                                            array1[1] + "</tr>";
                        }
                        strPrintData += "<tr font='黑体' size='7' x='245' y='" + Convert.ToString(pos) + "' >" +
                                        array1[2] + "</tr>";
                        strPrintData += "<tr font='黑体' size='7' x='288' y='" + Convert.ToString(pos) + "' >" +
                                        array1[3] + "</tr>";
                    }
                }
                if (CardCode == "20") //医保
                {
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >自付金额：" +
                                    pay_charge_total + " 元</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) +
                                    "' >医保个人账户支付金额：" + zhzf + " 元</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >基金支付：" +
                                    tczf + " 元</tr>";
                }
                if (CardCode != "20") //院内减免
                {
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >自付金额：" +
                                    pay_charge_total + " 元</tr>";
                    if (tczf != "" || tczf != "0")
                    {
                        strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) +
                                        "' >医院垫付：" + tczf + " 元</tr>";
                    }
                }
                strPrintData += "<tr font='黑体' size='12' x='20' y='" + Convert.ToString(pos += 20) + "' >总金额：" +
                                charge_total + " 元</tr>";
                strPrintData += "<tr font='黑体' size='8' x='20' y='" + Convert.ToString(pos += 20) +
                                "' >注：医保 标志 ①.无自付 ②.有自付 ③.全支付</tr>";
            }
            if (alipay_status.Contains("支付宝"))
            {
                if (alipay_status.Contains("退款"))
                {
                    strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
                                    "' >---------------------------------------------</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝卡号：" +
                                    idcard + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝支付：" +
                                    alipay_total + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝单号：" +
                                    trade_no + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" +
                                    stream_no + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝状态：" +
                                    alipay_tuihuan_status + "</tr>";
                }
                else
                {
                    strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
                                    "' >---------------------------------------------</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝卡号：" +
                                    idcard + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝支付：" +
                                    alipay_total + " 元</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝单号：" +
                                    trade_no + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" +
                                    stream_no + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付状态：" +
                                    alipay_status + "</tr>";
                }
            }
            else if (alipay_status.Contains("银联"))
            {
                strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
                                "' >---------------------------------------------</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联卡号：" +
                                idcard + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联支付：" +
                                alipay_total + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" +
                                stream_no + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易流水号：" +
                                trade_no + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易状态：" +
                                alipay_status + "</tr>";
            }
            else
            {
                strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
                                "' >---------------------------------------------</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" +
                                stream_no + "</tr>";
            }
            string str = pay_status;
            int n = str.Length;
            if (n > 25)
            {
                int m;
                if (n % 25 == 0)
                {
                    m = n / 25;
                }
                else
                {
                    m = n / 25 + 1;
                }
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >缴费状态：" +
                                yb_status + "</tr>";
                int i;
                for (i = 1; i < m - 1; i++)
                {
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >" +
                                    str.Substring(i * 25, 25) + "</tr>";
                }
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >" +
                                str.Substring(i * 25, n - i * 25) + "</tr>";
            }
            else
            {
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >缴费状态：" +
                                yb_status + "</tr>";
//                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >退费状态：" + alipay_tuihuan_status + "</tr>";
            }
            strPrintData += "<tr font='黑体' size='11' x='10' y='" + Convert.ToString(pos += 20) +
                            "' >*温馨提示：请保存此凭证，请勿遗失</tr>";
            strPrintData += "<tr font='黑体' size='11' x='10' y='" + Convert.ToString(pos += 20) + "' >*如需发票请去咨询台打印</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 30) + "' >机器名称：" + OperAtor +
                            "</tr>";
            strPrintData += "</print_info>";
            strPrintData = strPrintData.Replace("<print_info width='300' height='400'>",
                "<print_info width='300' height='" + (420 + mx_count * 35).ToString() + "'>");
            pr1.PrintDataJF(strPrintData, ref tmp1);
        }

        //挂号打印的方法
        public void PrtJzDateCreatGuaHao(string Position = "", string ampm = "", string RegNo = "",
            string PatBingAnHao = "", string PatId = "",
            string PatDept = "", string RegType = "", string PatName = "", string PatSex = "", string PatAge = "",
            string PatType = "", string GuaHaoFee = "", string ZhenLiaoFee = "", string ZiFu = "",
            string YiBaoZhiFu = "", string DianFu = "", string ShiShou = "", string OperAtor = "",
            string LiuShuiHao = "", string GuaHaoTime = "", string JzTime = "", string ZFBNum = "",
            string ZFBDanHao = "", string ZFBStream = "", string ZFBStatus = "")
        {
            //MessageBox.Show(Position + "#" + RegNo + "#" + PatBingAnHao + "#" + PatId + "#" + PatDept + "#" + RegType + "#" + PatName + "#" + PatSex + "#" + PatAge + "#" + PatType + "#" + GuaHaoFee + "#" + ZhenLiaoFee + "#" + ZiFu + "#" + YiBaoZhiFu + "#" + DianFu + "#" + ShiShou + "#" + GuaHaoYuan + "#" + LiuShuiHao + "#" + GuaHaoTime + "#" + JzTime);
            string tmp1 = "";
            PrintLpt pr1 = new PrintLpt();
            int pos = 10;
            int mx_count = 0;
            decimal AllMoney = 0;
            String strPrintData = "<?xml version='1.0' encoding='utf-8'?>" + "<print_info width='300' height='500'>";
            strPrintData += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos) + "' >就诊</tr>";
            strPrintData += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos += 30) + "' >位置</tr>";
            strPrintData += "<tr font='黑体' size='14' x='190' y='" + Convert.ToString(pos -= 15) + "' >" + ampm + RegNo +
                            "号</tr>";
            strPrintData += "<tr font='黑体' size='14' x='80' y='" + Convert.ToString(pos) + "' >" + Position + "</tr>";
            strPrintData += "<tr font='黑体' size='12' x='70' y='" + Convert.ToString(pos += 45) + "' >海淀医院挂号单(自助)</tr>";
            strPrintData += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 50) + "' >病案号:" +
                            PatBingAnHao + "</tr>";
            //            strPrintData += "<tr font='黑体' size='13' x='280' y='" + Convert.ToString(pos) + "' >ID:" + PatId + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >" + PatDept +
                            "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >号别:" + RegType +
                            "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >姓名:" + PatName +
                            "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='140' y='" + Convert.ToString(pos) + "' >性别:" + PatSex + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='200' y='" + Convert.ToString(pos) + "' >" + PatAge + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >身份:" + PatType +
                            "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号费:" + GuaHaoFee +
                            "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >诊疗费:" + ZhenLiaoFee +
                            "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >自付:" + ZiFu +
                            "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >医保已支付:" + YiBaoZhiFu +
                            "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >" + DianFu +
                            "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >实收:" + ShiShou +
                            "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >机器编号:" + OperAtor +
                            "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >流水号:" + LiuShuiHao +
                            "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号时间:" +
                            GuaHaoTime + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >" + JzTime +
                            "</tr>";
//            strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
//                                "' >---------------------------------------------</tr>";
            if (ZFBStatus.Contains("支付宝"))
            {
                if (ZFBStatus.Contains("退款"))
                {
                    strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
                                    "' >---------------------------------------------</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝卡号：" +
                                    ZFBNum + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝支付：" +
                                    ShiShou + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝单号：" +
                                    ZFBDanHao + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" +
                                    ZFBStream + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝状态：" +
                                    ZFBStatus + "</tr>";
                }
                else
                {
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝卡号:" +
                                    ZFBNum + "</tr>";
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝支付:" +
                                    ShiShou + "元</tr>";
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝单号:" +
                                    ZFBDanHao + "</tr>";
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >流水号:" +
                                    ZFBStream + "</tr>";
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付状态:" +
                                    ZFBStatus + "</tr>";
                }
            }
            if (ZFBStatus.Contains("银联"))
            {
                strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
                                "' >---------------------------------------------</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联卡号：" +
                                ZFBNum + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联支付：" +
                                ShiShou + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联参考号：" +
                                ZFBDanHao + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易流水号：" +
                                ZFBStream + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易状态：" +
                                ZFBStatus + "</tr>";
            }
//            strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
//                                "' >---------------------------------------------</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
                            "' >挂号单作为退号凭证，遗失不补</tr>";
            strPrintData += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) +
                            "' >如需挂号收据，缴费时请提前告知收费员</tr>";
            strPrintData += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) +
                            "' >请患者到相应分诊台分诊报到机报到就诊</tr>";
            strPrintData += "</print_info>";
            strPrintData = strPrintData.Replace("<print_info width='300' height='400'>",
                "<print_info width='300' height='" + (420 + mx_count * 35).ToString() + "'>");
            pr1.S = PatId;
            pr1.PrintDataGH(strPrintData, ref tmp1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="s1">打印的xml</param>
        /// <param name="s2">条形码</param>
        /// <param name="width">条码宽度</param>
        /// <param name="height">条码高度</param>
        /// <param name="x">条码x坐标</param>
        /// <param name="y">条码的y坐标</param>
        public void paint(string s1, string s2, int width, int height, int x, int y)
        {
            string tmp1 = "";
            PrintLpt pr1 = new PrintLpt();
            int pos = 10;
            int mx_count = 0;
            decimal AllMoney = 0;
            s1 = s1.Replace("<print_info width='300' height='400'>",
                "<print_info width='300' height='" + (4200 + mx_count * 35).ToString() + "'>");
            if (s2 != "")
            {
                pr1.S = s2;
                pr1.Width = width;
                pr1.Height = height;
                pr1.X = x;
                pr1.Y = y;
            }
            pr1.PrintDataGH(s1, ref tmp1);
        }

        #endregion

        #region 获取打印机状态

        #region 532 pm58t

        public void updatePrinterStatusToDb()
        {
            string status = getStatus_pm58t();
            string[] arr = status.Split(',');
            int effect = 0;
            DateTime timenow = DateTime.Now;
            string sqlstr = "update devstatus set Paper_End = '" + arr[3] +
                            "', Paper_Near_End = '无纸尽传感器', Ticket_Out = '没有出纸口检测功能', Paper_Jam = '无卡纸检测功能', Cover_Open = '" +
                            arr[1] +
                            "',UpdateTime = '" + timenow + "' where DevName = '" + Properties.Settings.Default.DevName +
                            "'";
            MySqlUtil sqlUtil = new MySqlUtil();
            sqlUtil.RunSql(sqlstr, out effect);
        }

        public string initDevice()
        {
            return DllClass.OpenPrinter() == 0 ? "打印机端口打开成功" : "打印机端口打开失败";
            Thread.Sleep(150);
        }

        public string getStatus_pm58t()
        {
            int i;
            string s = "";
            //GcRealtimeGetStatus(byte iStatus)函数说明
            //[返回值]
            //函数将返回打印机的状态字节iStatus，如果发生任何错误打印机都将返回-１。
            //[参数说明]
            //BYTE iStatus
            //用于在内存中存放打印机的状态字节。iStatus 的取值范围为1～5。
            //iStatus=1：打印机状态
            //iStatus=2：打印机离线状态
            //iStatus=3：打印机故障状态
            //iStatus=4：打印机纸检测状态
            //iStatus=5：送纸器状态
            i = DllClass.GcRealtimeGetStatus(1);
            if (i == -1)
            {
                s += i + "异常,";
            }
            else
            {
                //s += i + "正常,";
                string[] str = convert(i).Split(',');
                if (str[3] == "0")
                {
                    s += "联机,";
                }
                else
                {
                    s += "脱机,";
                }

                #region NonUse

                //                if (str[6] == "0")
                //                {
                //                    s += "进纸键断开,";
                //                }
                //                else
                //                {
                //                    s += "进纸键接通,";
                //                }

                #endregion
            }


            i = DllClass.GcRealtimeGetStatus(2);
            if (i == -1)
            {
                s += i + "异常,";
            }
            else
            {
                // s += i + "正常,";
                string[] str = convert(i).Split(','); //低位在前,高位在后
                if (str[2] == "0")
                {
                    s += "机头抬杠已关闭,";
                }
                else
                {
                    s += "机头抬杠已打开,";
                }

                #region NonUse

                //                if (str[3] == "0")
                //                {
                //                    s += "没有按键进纸,";
                //                }
                //                else
                //                {
                //                    s += "按键进纸中,";
                //                }
                //                if (str[5] == "0")
                //                {
                //                    s += "打印纸未用完,";
                //                }
                //                else
                //                {
                //                    s += "打印纸用完，停止打印,";
                //                }
                //                if (str[6] == "0")
                //                {
                //                    s += "没有错误,";
                //                }
                //                else
                //                {
                //                    s += "发生错误,";
                //                }

                #endregion
            }


            i = DllClass.GcRealtimeGetStatus(3);
            if (i == -1)
            {
                s += i + "异常,";
            }
            else
            {
                //s += i + "正常,";
                string[] str = convert(i).Split(',');

                #region NonUse

                //                if (str[2] == "0")
                //                {
                //                    s += "没有机械错误,";
                //                }
                //                else
                //                {
                //                    s += "发生机械错误,";
                //                }

                #endregion

                if (str[3] == "0")
                {
                    s += "没有自动切纸错误,";
                }
                else
                {
                    s += "发生自动切纸错误,";
                }

                #region NonUse

//                if (str[5] == "0")
                //                {
                //                    s += "没有不可恢复的错误,";
                //                }
                //                else
                //                {
                //                    s += "出现不可恢复的错误,";
                //                }
                //                if (str[6] == "0")
                //                {
                //                    s += "没有可自动恢复的错误,";
                //                }
                //                else
                //                {
                //                    s += "出现可自动恢复的错误,";
                //                }

                #endregion
            }


            i = DllClass.GcRealtimeGetStatus(4);
            if (i == -1)
            {
                s += i + "异常,";
            }
            else
            {
                //s += i + "正常,";
                string[] str = convert(i).Split(',');

                #region NonUse

//                if (str[2] == "0")
                //                {
                //                    s += "纸将尽检测器，纸张足够,";
                //                }
                //                else
                //                {
                //                    s += "纸将尽检测器检测到纸张接近末端,";
                //                }
                //                if (str[3] == "0")
                //                {
                //                    s += "纸将尽检测器，纸张足够,";
                //                }
                //                else
                //                {
                //                    s += "纸将尽检测器检测到纸张接近末端,";
                //                }

                #endregion

                if (str[5] == "0")
                {
                    s += "纸尽传感器：有纸,";
                }
                else
                {
                    //s += "纸尽传感器检测到卷纸末端,";
                    s += "纸尽传感器：缺纸,";
                }

                #region NonUse

//                if (str[6] == "0")
                //                {
                //                    s += "纸尽传感器：有纸,";
                //                }
                //                else
                //                {
                //                    s += "纸尽传感器检测到卷纸末端";
                //                }

                #endregion
            }

            #region NonUse

            //            i = DllClass.GcRealtimeGetStatus(5);
            //            if (i == -1)
            //            {
            //                s += i + "异常,";
            //            }
            //            else
            //            {
            //                //                s += i + "正常,";
            //                string[] str = convert(i).Split(',');
            //
            //            }

            #endregion

            return s;
        }

        public string convert(int i)
        {
            string j = Convert.ToString(i, 2);
            int i1 = Convert.ToInt32(j);
            string s = "";
            for (int k = 0; k < 8; k++)
            {
                s += Convert.ToString(i1 % 10) + ",";
                i1 /= 10;
            }
            return s;
        }

        #endregion

        #region K80 M216

        #endregion

        #region 迅普打印机

        private const Int32 C_PRINTER_PORT_COM = 0; // 打印机端口类型 串口
        private const Int32 C_PRINTER_PORT_LPT = 1; // 打印机端口类型 并口
        private const Int32 C_PRINTER_PORT_USB = 2; // 打印机端口类型 USB
        private const Int32 C_PRINTER_PORT_NET = 3; // (预留)打印机端口类型 网口

        private const Int32 C_PRINTER_PORT_VLPT = 4; // 打印机端口类型 虚拟并口

        // 常量定义-裁纸方式
        private const Int32 C_PRINTER_CUT_FULL = 0; // 全切

        private const Int32 C_PRINTER_CUT_PART = 1; // 半切

        //  常量定义-字符编码
        private const Int32 C_PRINTER_ENCODING_ASCII = 0; // ASCII编码

        private const Int32 C_PRINTER_ENCODING_GB2312 = 1; // GB2312编码

        private const Int32 C_PRINTER_ENCODING_GB18030 = 2; // GB18030编码

        // 常量定义-点阵大小
        private const Int32 C_PRINTER_FONT_24 = 0; // 24点阵汉字

        private const Int32 C_PRINTER_FONT_16 = 1; // 16点阵汉字

        // 常量定义-对齐方式
        private const Int32 C_PRINTER_ALIGN_LEFT = 0; // 居左对齐

        private const Int32 C_PRINTER_ALIGN_CENTER = 1; // 居中对齐

        private const Int32 C_PRINTER_ALIGN_RIGHT = 2; // 居右对齐

        //  常量定义-黑标参数
        private const Int32 C_PRINTER_BM_MOVE = 1; // 起始位置的设定值

        private const Int32 C_PRINTER_BM_CUT = 2; // 裁纸位置的设定值
        private const Int32 C_PRINTER_BM_FORWARD = 0; // 进纸的方向

        private const Int32 C_PRINTER_BM_BACK = 1; // 倒纸的方向

        //  常量定义-BMP位图模式
        private const Int32 C_PRINTER_BMP_8 = 0; // 8点单密度

        private const Int32 C_PRINTER_BMP_8D = 1; // 8点双密度
        private const Int32 C_PRINTER_BMP_24 = 32; // 24点单密度

        private const Int32 C_PRINTER_BMP_24D = 33; // 24点双密度

        // 一维条形码类型常量
        private const Int32 C_PRINTER_BARCODE_EAN13 = 2; // EAN13条形码

        private const Int32 C_PRINTER_BARCODE_CODE39 = 69; // CODE39条形码

        private const Int32 C_PRINTER_BARCODE_CODE128 = 73; // CODE128条形码

        // 一维条形码人工识别字符位置常量
        private const Int32 C_PRINTER_HRI_NONE = 0; // 不打印HRI字符

        private const Int32 C_PRINTER_HRI_TOP = 1; // 在条形码上方打印HRI字符
        private const Int32 C_PRINTER_HRI_BOTTOM = 2; // 在条形码下方打印HRI字符

        private const Int32 C_PRINTER_HRI_BOTH = 3; // 在条形码上方和下方打印HRI字符

        // 打印机状态类型常量
        private const Int32 C_PRINTER_STATUS_ONLINE = 1; // 在线离线状态

        private const Int32 C_PRINTER_STATUS_DOOR = 2; // 纸仓门状态
        private const Int32 C_PRINTER_STATUS_CUT = 3; // 裁纸器状态
        private const Int32 C_PRINTER_STATUS_PAPER = 4; // 纸卷状态
        private const Int32 C_PRINTER_STATUS_HEAT = 5; // (预留)发热片状态
        private const Int32 C_PRINTER_STATUS_BUFFER = 6; // (预留)缓存区状态
        private const Int32 C_PRINTER_STATUS_DRAWER = 7; // (预留)钱箱状态
        private const Int32 C_PRINTER_STATUS_JAM = 8; // 卡纸状态

        IntPtr hPrinter = IntPtr.Zero;

        public string xunPuPrinterGetStatus()
        {
            string status = "";
            if (hPrinter == IntPtr.Zero)
            {
                hPrinter = DllClass.Printer_Port_Open("", 0, C_PRINTER_PORT_USB);
            }
            int sta = 0;
            if (hPrinter.ToInt32() != -1)
            {
#if false //==离线检测===============================================
                sta = Printer_Query_Status(hPrinter, C_PRINTER_STATUS_ONLINE);
                if (0x10 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "请先打开打印机");
                }
                else if (0x16 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机联机");
                }
                else if (0x1e == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机脱机");
                }
                else
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机返回值错误");
                }

                //==抬杆检测===============================================
                sta = Printer_Query_Status(hPrinter, C_PRINTER_STATUS_DOOR);
                if (0x10 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "请先打开打印机");
                }
                else if (0x12 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机抬杆正常");
                }
                else if (0x16 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机抬杆异常");
                }
                else if (0x32 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机抬杆正常但缺纸");
                }
                else if (0x36 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机抬杆异常且缺纸");
                }
                else
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机返回值错误");
                }

                //==切刀检测===============================================
                sta = Printer_Query_Status(hPrinter, C_PRINTER_STATUS_CUT);
                if (0x10 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "请先打开打印机");
                }
                else if (0x12 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机切刀正常");
                }
                else if (0x16 == sta)
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机切刀异常");
                }
                else
                {
                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机返回值错误");
                }
#endif
                //==纸状态检测===============================================
                sta = DllClass.Printer_Query_Status(hPrinter, C_PRINTER_STATUS_PAPER);
                if (0x10 == sta)
                {
//                    MessageBox.Show(string.Format("0x{0:X00}", sta), "请先打开打印机");
                    status = "请先打开打印机";
                }
                else if (0x12 == sta)
                {
//                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机有纸且充足");
                    status = "打印机有纸且充足";
                }
                else if (0x1e == sta)
                {
//                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机有纸但将尽");
                    status = "打印机有纸但将尽";
                }
                else if (0x72 == sta)
                {
//                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机无纸但充足");
                    status = "打印机无纸但充足";
                }
                else if (0x7e == sta)
                {
//                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机无纸且将尽");
                    status = "打印机无纸且将尽";
                }
                else
                {
//                    MessageBox.Show(string.Format("0x{0:X00}", sta), "打印机返回值错误");
                    status = "打印机返回值错误";
                }
            }
            return status;
        }

        #endregion

        #endregion

        #region 电动医保医保读卡器

        public void DisAllowCardIn()
        {
            DllClass.DisableCardIn();
        }

        public void AllowCardIn()
        {
            DllClass.AllowCardIn();
        }

        public void MoveOutCard()
        {
            DllClass.MoveOutCard();
        }

        //ture表示有卡，false表示没卡
        public bool ReadStatus()
        {
            return DllClass.ReadStatus();
        }

        public void PowerOnIcCard()
        {
            DllClass.Sd();
        }

        public void PowerOffIcCard()
        {
            DllClass.Xd();
        }

        #endregion

        #region 发卡机接口

        #region CRT580

        public string Crt580ComOpen()
        {
            return !DllClass._ropen.Equals((object)IntPtr.Zero) ? "打开发卡机串口成功" : "打开发卡机串口失败,读卡器端口被占用，或者端口错误";
        }

        public string Crt580CheckDevice()
        {
            string result = "";
            DllClass.Fss5 s5 = DllClass.Fss5.停卡位为前端不持卡;
            DllClass.Fss4 s4 = (DllClass.Fss4)0;
            DllClass.Fss3 s3 = DllClass.Fss3.读卡器内无卡;
            DllClass.Fss2 s2 = DllClass.Fss2.发卡通道无卡;
            DllClass.Fss1 s1 = DllClass.Fss1.发卡箱内卡足;
            DllClass.Fss0 s0 = DllClass.Fss0.收卡箱卡不满;
            int i = DllClass.GetStatus(ref result, ref s5, ref s4, ref s3, ref s2, ref s1, ref s0);
            string s;
            if (i == 0)
            {
                s = s3 + "," + s2 + "," + s1 + "," + s0;
            }
            else
            {
                s = "发卡机状态获取失败";
            }
            return s;
            /*string result = "";
            DllClass.Fss5 s5 = DllClass.Fss5.停卡位为前端不持卡;
            DllClass.Fss4 s4 = (DllClass.Fss4)0;
            DllClass.Fss3 s3 = DllClass.Fss3.读卡器内无卡;
            DllClass.Fss2 s2 = DllClass.Fss2.发卡通道无卡;
            DllClass.Fss1 s1 = DllClass.Fss1.发卡箱内卡足;
            DllClass.Fss0 s0 = DllClass.Fss0.收卡箱卡不满;
            if (DllClass.GetStatus(ref result, ref s5, ref s4, ref s3, ref s2, ref s1, ref s0) != 0)
                return "发卡机状态获取失败";
            return "发卡机状态获取成功" + (object)"," + (string)(object)s5 + "," + (string)(object)s4 + "," + (string)(object)s3 + "," + (string)(object)s2 + "," + (string)(object)s1 + "," + (string)(object)s0;
*/
        }

        public string Crt580MoveInCard()
        {
            DllClass._toPosition = 0x31;
            DllClass._fromPosition = 0x30;
            return DllClass.CRT580_MoveCard(DllClass._ropen, DllClass.AddrH, DllClass.Addrl, DllClass._toPosition,
                       DllClass._fromPosition) != 0
                ? "发卡机走卡失败"
                : "发卡机走卡成功";
        }

        public string Crt580ReadCardNo()
        {
            string cardNo = "";
            byte _mode = 0x30;
            byte _track = 0x32;
            byte[] _TrackData = new byte[16];
            int _TrackDataLen = 0;
            int i = DllClass.MC_ReadTrack(DllClass._ropen, DllClass.AddrH, DllClass.Addrl, _mode, _track, _TrackData,
                ref _TrackDataLen); //读取磁卡卡号
            if (i == 0)
            {
                byte[] track2Data = _TrackData.Skip(5).Take(8).ToArray();
                cardNo = Ascii2Str(track2Data);
            }
            return cardNo;
        }

        public static string Ascii2Str(byte[] buf)
        {
            return System.Text.Encoding.ASCII.GetString(buf);
        }

        public string Crt580MoveOutCard()
        {
            DllClass._toPosition = 0x33;
            DllClass._fromPosition = 0x31;
            return DllClass.CRT580_MoveCard(DllClass._ropen, DllClass.AddrH, DllClass.Addrl, DllClass._toPosition,
                       DllClass._fromPosition) != 0
                ? "发卡机吐卡失败"
                : "发卡机吐卡成功";
        }

        public string Crt580RecyleCard()
        {
            return DllClass.BackCard() == 0 ? "回收卡成功" : "回收卡失败";
        }

        public string Crt580DeInit()
        {
            return DllClass.CommClose(Convert.ToUInt32(DllClass._ropen)) != 0 ? "关闭发卡机串口成功" : "关闭发卡机串口成功";
        }

        #endregion

        #region M100A

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_CommOpen(string strport);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_CommOpenWithBaud(string strport, int baudate);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_CommClose(int hCom);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_AutoTestMachine(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte[] _Version, byte[] Info);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_Reset(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte _PM, byte[] _Version, byte[] Info);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_EnterCard(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte EntryType, byte[] Info);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_MoveCard(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte _PM, byte[] Info);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_ReadICCardNum(int hCom, bool bHasMac_Addr, byte Mac_Addr, int[] length, byte[] IDNum);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_S50DetectCard(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte[] Info);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_S50GetCardID(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte[] _CardID, byte[] Info);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_CheckCardPosition(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte[] CardStates, byte[] Info);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_S50LoadSecKey(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte SectorAddr,byte keyType,byte[] key, byte[] Info);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_S50ReadBlock(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte SectorAddr, byte blockAddr, byte[] BlockData, byte[] Info);

        [DllImport("M100A_Dll.dll")]
        public static extern int M100A_S50WriteBlock(int hCom, bool bHasMac_Addr, byte Mac_Addr, byte SectorAddr, byte blockAddr, byte[] BlockData, byte[] Info);


        int hCom = 0;
        byte[] Info = new byte[500];
        byte Mac_Addr = 0x00;

        /// <summary>
        /// 初始化发卡机
        /// </summary>
        /// <param name="comPort">发卡机串口号</param>
        /// <param name="baudate">发卡机波特率:38400</param>
        /// <returns></returns>
        public string M100AComOpen(string comPort,int baudate)
        {
            hCom = M100A_CommOpenWithBaud(comPort, baudate);
            if (hCom > 0)
            {
                int re = 0, i;
                byte[] _Version = new byte[20];
                for (i = 0; i < 16; i++)
                {
                    re = M100A_Reset(hCom, false, 0, 0x34, _Version, Info);
                    if (re == 0)
                    {
                        Mac_Addr = (byte)i;
                        break;
                    }
                }
                return re == 0 ? "设备连接成功" : "设备连接失败";
            }
            else
            {
                return "串口打开失败";
            }
        }

        public string getM100AStatus()
        {
            byte[] status = new byte[2];
            string result = "";
            if (M100A_CheckCardPosition(hCom, false, 0, status, Info) == 0)
            {
                byte status1 = status[0];
                byte status2 = status[1];
                switch (status1)
                {
                    case 0x30:
                        result += "通道无卡,";
                        break;
                    case 0x31:
                        result += "读磁卡位置有卡,";
                        break;
                    case 0x32:
                        result += "IC卡位置有卡,";
                        break;
                    case 0x33:
                        result += "前端夹卡位置有卡,";
                        break;
                    case 0x34:
                        result += "前端不夹卡位置有卡,";
                        break;
                    case 0x35:
                        result += "卡不在标准位置,";
                        break;
                    case 0x36:
                        result += "卡片正在移动中,";
                        break;
                    case 0x37:
                        result += "射频卡位置有卡,";
                        break;
                }
                switch (status2)
                {
                    case 0x30:
                        result += "卡箱无卡";
                        break;
                    case 0x31:
                        result += "卡箱卡片不足";
                        break;
                    case 0x32:
                        result += "卡箱卡片足够";
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 移动卡到非接Ic卡位置
        /// </summary>
        /// <returns></returns>
        public string moveCardToIcPosition()
        {
            return M100A_MoveCard(hCom, false, 0, 0x31, Info) == 0 ? "移动卡片操作执行成功" : "移动卡片操作执行失败";
        }

        /// <summary>
        /// 回收卡
        /// </summary>
        /// <returns></returns>
        public string callbackCard()
        {
            byte[] Vercode = new byte[20];
            string info;
            return M100A_MoveCard(hCom, false, 0, 0x34, Info) == 0 ? "回收卡成功" : "回收卡失败";
        }

        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <returns></returns>
        public string getSerialNo()
        {
            int re = 0;
            byte[] cardid = new byte[10];
            string str = "";
            re = M100A_S50GetCardID(hCom, false, 0, cardid, Info);
            if (re == 0)
            {
                for (int i = 4; i >0 ; i--)
                {
                    str += cardid[i-1].ToString("X2");
                }
                return Convert.ToInt32(str,16).ToString();
            }
            else
                return "获取卡号失败";
        }

        /// <summary>
        /// 弹卡
        /// </summary>
        /// <returns></returns>
        public string outCard()
        {
            return M100A_MoveCard(hCom, false, 0, 0x33, Info) == 0 ? "弹卡成功" : "弹卡失败";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionAddr">扇区号</param>
        /// <param name="key">扇区密码:787332</param>
        /// <returns></returns>
        public string M100AloadKey(byte sectionAddr,string key)
        {
            return M100A_S50LoadSecKey(hCom, false, 0, sectionAddr, 0x31, Encoding.Default.GetBytes(key), Info) == 0 ? "密码校验OK" : "密码校验失败" ;
        }

        /// <summary>
        /// 读扇区数据
        /// </summary>
        /// <param name="sectionAddr">扇区号</param>
        /// <param name="blockAddr">块号</param>
        /// <returns></returns>
        public string M100AReadBlock(byte sectionAddr,byte blockAddr)
        {
            byte[] blockData = new byte[16];
            if (M100A_S50ReadBlock(hCom, false, 0, sectionAddr, blockAddr, blockData, Info) == 0)
            {
                return Encoding.Default.GetString(blockData);
            }
            else
            {
                return "扇区数据读取失败";
            }
        }

        /// <summary>
        /// 写扇区数据
        /// </summary>
        /// <param name="sectionAddr">扇区号</param>
        /// <param name="blockAddr">块号</param>
        /// <param name="key">密码</param>
        /// <returns></returns>
        public string M100AWriteBlock(byte sectionAddr, byte blockAddr,string key)
        {
            return M100A_S50WriteBlock(hCom, false, 0, sectionAddr, blockAddr, Encoding.Default.GetBytes(key), Info) == 0
                ? "写入成功"
                : "写入失败";
        }
        #endregion

        #endregion

        #region 灯带接口

        public string Ledinit()
        {
            this._Comm = new SerialPort();
            this._Comm.BaudRate = 9600;
            this._Comm.Parity = Parity.None;
            this._Comm.DataBits = 8;
            this._Comm.StopBits = StopBits.One;
            this._Comm.PortName = Settings.Default.Led_Port;
            this._Comm.ReceivedBytesThreshold = 1;
            string str;
            try
            {
                this._Comm.Open();
                str = "初始化串口成功";
            }
            catch (Exception ex)
            {
                str = "初始化串口失败";
            }
            return str;
        }

        /// <summary>
        ///主机控制命令格式：FE XX(0B关闭LED 1B关闭所有LED 2B闪烁LED 3B常亮LED 4B常亮所有LED) XX(01-07共7路指示灯) 73 73 FF
        ///控制板响应命令格式： FE 1C 73 73 73 FF (表示控制命令执行成功) FE 0C 73 73 73 FF (表示控制命令执行失败)
        /// </summary>
        /// <param name="ledNo">灯带序号,1--7</param>
        /// <param name="ledOrder">灯带命令,1点亮,2闪烁,3熄灭,4全灭,5,全亮</param>
        public void send(int ledNo, int ledOrder)
        {
            Converter converter = new Converter();
            for (int i = 1; i < 8; i++)
            {
                if (i == ledNo)
                {
                    byte[] sendbyte = new Byte[6];
                    if (ledOrder == 1) //点亮
                    {
                        sendbyte = new Byte[] {0XFE, 0X3B, converter.convert(ledNo), 0X73, 0X73, 0XFF};
                    }
                    else if (ledOrder == 2) //闪烁
                    {
                        sendbyte = new Byte[] {0XFE, 0X2B, converter.convert(ledNo), 0X73, 0X73, 0XFF};
                    }
                    else if (ledOrder == 3) //熄灭
                    {
                        sendbyte = new Byte[] {0XFE, 0X0B, converter.convert(ledNo), 0X73, 0X73, 0XFF};
                    }
                    else if (ledOrder == 4) //全部熄灭
                    {
                        sendbyte = new Byte[] {0XFE, 0X1B, converter.convert(ledNo), 0X73, 0X73, 0XFF};
                    }
                    else if (ledOrder == 5) //全部点亮
                    {
                        sendbyte = new Byte[] {0XFE, 0X4B, converter.convert(ledNo), 0X73, 0X73, 0XFF};
                    }
                    _Comm.Write(sendbyte, 0, 6);
                }
                Thread.Sleep(100);
            }
        }

        #endregion

        #region 二代身份证读卡器

        public int HDinit(int port)
        {
            return DllClass.HD_InitComm(port);
        }

        public int HDauth()
        {
            return DllClass.HD_Authenticate();
        }

        public int HDclose()
        {
            return DllClass.HD_CloseComm();
        }

        //海淀医院
        public string HDread()
        {
            int retval = 0;
            string pBmpFile = @"./zp.bmp";
            byte[] pName = new byte[100];
            byte[] pSex = new byte[100];
            byte[] pNation = new byte[100];
            byte[] pBirth = new byte[100];
            byte[] pAddress = new byte[400];
            byte[] pCertNo = new byte[100];
            byte[] pDepartment = new byte[100];
            byte[] pEffectData = new byte[100];
            byte[] pExpire = new byte[100];
            //            int port = Settings.Default.SFZport;
            int port = 1001;
            int init = this.HDinit(port);
            if (init == 0)
            {
                int auth = this.HDauth();
                if (auth == 0)
                {
                    retval = DllClass.HD_Read_BaseMsg(pBmpFile, pName, pSex, pNation, pBirth, pAddress, pCertNo,
                        pDepartment, pEffectData, pExpire);
                    if (retval != 0)
                    {
                        DllClass.HD_CloseComm();
                        return @"读卡失败";
                    }
                    string s1 = System.Text.Encoding.Default.GetString(pName);
                    string s2 = System.Text.Encoding.Default.GetString(pSex);
                    string s3 = System.Text.Encoding.Default.GetString(pNation);
                    string s4 = System.Text.Encoding.Default.GetString(pBirth);
                    string s5 = System.Text.Encoding.Default.GetString(pAddress);
                    string s6 = System.Text.Encoding.Default.GetString(pCertNo);
                    string s7 = System.Text.Encoding.Default.GetString(pDepartment);
                    string s8 = System.Text.Encoding.Default.GetString(pEffectData);
                    string s9 = System.Text.Encoding.Default.GetString(pExpire);
                    string s = s1.Replace("\0", "").Trim() + "," + s2.Replace("\0", "").Trim() + "," +
                               s3.Replace("\0", "").Trim() + "," + s4.Replace("\0", "").Trim() + "," +
                               s5.Replace("\0", "").Trim() + "," + s6.Replace("\0", "").Trim() + "," +
                               s7.Replace("\0", "").Trim() + "," + s8.Replace("\0", "").Trim() + "," +
                               s9.Replace("\0", "").Trim();
                    DllClass.HD_CloseComm();
                    return s;
                }
                else
                {
                    this.HDclose();
                    return @"请重放身份证";
                }
            }
            else
            {
                this.HDclose();
                return @"开usb失败";
            }
        }

        public string sfz_card_read()
        {
            var s = this.HDread();
            return s;
        }

        #endregion

        #region 读取txt文件，|表示换行

        public string readText(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string result = "";
            String line = "";
            while ((line = sr.ReadLine()) != null)
            {
                result += line + "|";
            }
//            MessageBox.Show(result);
            return result;
        }

        #endregion

        #region 顺义妇幼启动Lis程序

        public void StartLisPrint()
        {
            RunMutilInstanceProcess(1);
        }

        static void RunMutilInstanceProcess(int i)
        {
            string appPath = Properties.Settings.Default.exePath;
            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = appPath;
            process.Arguments = "process " + i.ToString();

            process.UseShellExecute = false;
            process.CreateNoWindow = true;

            process.RedirectStandardOutput = true;
            Process.Start(process);

            // string Result = p.StandardOutput.ReadToEnd();
            // Console.WriteLine("the console app output is {0}", Result);
            Console.WriteLine(" process {0} start", i);
        }

        #endregion

        #region 调用cmd执行lis打印接口

        /// <summary>
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdStr">病人id号</param>
        public bool RunCmd2(string cmdStr)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    string str = string.Format(@"""{0}"" {1} {2}", Properties.Settings.Default.LisPrintInterfacePath,
                        cmdStr, "&exit");

                    myPro.StandardInput.WriteLine(str);
                    myPro.StandardInput.AutoFlush = true;
                    myPro.WaitForExit();

                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }

        #endregion

        #region 手写输入法

        public void runHandInput()
        {
            string appPath = Application.StartupPath + "\\HandInput\\1.1.0.282\\handinput.exe";
            ProcessStartInfo process = new ProcessStartInfo
            {
                FileName = appPath,
                Arguments = "process 1",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            Process.Start(process);
        }

        private void RunCmd(string command)
        {
            //实例一个Process类，启动一个独立进程
            Process p = new Process
            {
//Process类有一个StartInfo属性
                StartInfo =
                {
                    FileName = "cmd.exe", //设定程序名
                    Arguments = "/c " + command, //设定程式执行参数
                    UseShellExecute = false, //关闭Shell的使用
                    RedirectStandardInput = true, //重定向标准输入
                    RedirectStandardOutput = true, //重定向标准输出 
                    RedirectStandardError = true, //重定向错误输出 
                    CreateNoWindow = true //设置不显示窗口
                }
            };
            p.Start(); //启动
            //也可以用这种方式输入要执行的命令
            //不过要记得加上Exit要不然下一行程式执行的时候会当机
            //p.StandardInput.WriteLine(command);
            //p.StandardInput.WriteLine("exit"); 
            //从输出流取得命令执行结果
            p.StandardOutput.ReadToEnd();
        }

        public void stopHandInput()
        {
            RunCmd("taskkill /f /im handinput.exe");
        }

        /*private string webtextboxid = "";
        //事件处理方法
        void frm_TransfEvent(string value)
        {
            HtmlElement textboxValue = webBrowser1.Document.All[webtextboxid];
            textboxValue.SetAttribute("value", value);
        }

        public void getHandWritingValue(string id)
        {
            webtextboxid = id;
            HandWrite frm = new HandWrite();
            //注册事件
            frm.TransfEvent += frm_TransfEvent;
            frm.ShowDialog();
        }*/

        #endregion

        #region 物理呼叫器

//        没调通
        public void InitWuLiHuJiaoQiPort()
        {
            this._Comm = new SerialPort();
            this._Comm.BaudRate = 9600;
            this._Comm.Parity = Parity.None;
            this._Comm.DataBits = 8;
            this._Comm.StopBits = StopBits.One;
            this._Comm.PortName = Settings.Default.WuLiHuJiaoQiPort;
            this._Comm.ReceivedBytesThreshold = 1;
            try
            {
                this._Comm.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("串口打开失败");
            }
        }

        //串口接收数据
        public string CommDataReceived()
        {
            int bytesToRead = this._Comm.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            this._Comm.Read(buffer, 0, bytesToRead);
            string str = "1";
            for (int index = 0; index < buffer.Length; ++index)
                str = str + buffer[index].ToString() + ",";
            return str;
        }

        /// <summary>
        ///顺呼发送：01  03  00  73  73  FF(主机地址+呼叫终端地址+顺呼命令+空数据+空数据+FF)
        ///顺乎返回：03  01  01  xx  xx  FF(呼叫终端地址+主机地址+显示命令+数据高位+数据低位+FF)
        ///重呼发送：01  03  02  00  00  FF(主机地址+呼叫终端地址+重呼命令+空数据+空数据+FF)
        ///重呼返回：03  01  01  xx  xx  FF(呼叫终端地址+主机地址+显示命令+数据高位+数据低位+FF)
        ///登录发送：01  03  06  73  73  FF(主机地址+呼叫终端地址+登陆命令+空数据+空数据+FF)
        ///登录返回：03  01  05  73  73  FF(呼叫器地址+主机地址+登陆成功命令+空数据+空数据+FF)
        ///退出发送：01  03  1A  73  73  FF(主机地址+呼叫终端地址+退出命令+空数据+空数据+FF)
        ///退出返回：03  01  07  73  73  FF(呼叫器地址+主机地址+登陆失败命令+空数据+空数据+FF)
        ///等候人数: 03  01  1c  xx  xx  FF(呼叫器地址+主机地址+显示命令+数据高位+数据低位+FF)
        /// </summary>
        /// <param name="i">物理呼叫器地址</param>
        /// <param name="i1">显示命令 当前号:1  等候人数:100</param>
        /// <param name="i2">要发送的数据 当前号:xxxx 等候人数:xx</param>
        public void CommDataSend(int i, int i1, int i2)
        {
            Converter converter = new Converter();
            byte[] buffer = new byte[6] {converter.convert(i), 1, converter.convert(i1), 0, 0, 0};
            if (i2 / 100 <= 0)
            {
                buffer[3] = (byte) 0;
                buffer[4] = converter.convert(i2 % 100);
            }
            else
            {
                buffer[3] = converter.convert(i2 / 100);
                buffer[4] = converter.convert(i2 % 100);
            }
            buffer[5] = byte.MaxValue;
            this._Comm.Write(buffer, 0, 6);
            Thread.Sleep(100);
        }

        #endregion

        #region 钞箱

        CCNET.Iccnet _B2bDevice;
        private int totalCash = 0;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Thread.Sleep(100);
                Thread.BeginCriticalRegion();
                Answer answer = _B2bDevice.RunCommand(CCNETCommand.Poll);
                BackgroundWorker backgroundWorker = (BackgroundWorker) sender;
                backgroundWorker.ReportProgress(0, answer);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Answer Ans = (Answer) e.UserState;
            label1.Text = Ans.Message;
            switch ((Bill_Validator_Sataus) Ans.Additional_Data)
            {
                case Bill_Validator_Sataus.Power_Up:
                    _B2bDevice.RunCommand(CCNETCommand.RESET);
                    break;
                case Bill_Validator_Sataus.Unit_Disabled:
                    Byte[] enable = {255, 255, 255, 255, 255, 255};
                    _B2bDevice.RunCommand(CCNETCommand.ENABLE_BILL_TYPES, enable);
                    break;
                case Bill_Validator_Sataus.Escrow_position:
                    _B2bDevice.RunCommand(CCNETCommand.STACK);
                    break;
                case Bill_Validator_Sataus.Bill_stacked:
                    int cash = 0;
                    switch (Ans.ReceivedData[4].ToString())
                    {
                        case "0":
                            cash = 1;
                            break;
                        case "1":
                            cash = 2;
                            break;
                        case "2":
                            cash = 5;
                            break;
                        case "3":
                            cash = 10;
                            break;
                        case "4":
                            cash = 20;
                            break;
                        case "5":
                            cash = 50;
                            break;
                        case "6":
                            cash = 100;
                            break;
                    }
                    totalCash += cash;
                    label2.Text = cash + "元,已压钞";
                    break;
                case Bill_Validator_Sataus.Bill_returned:
                    int cash1 = -1;
                    switch (Ans.ReceivedData[4].ToString())
                    {
                        case "0":
                            cash1 = 1;
                            break;
                        case "1":
                            cash1 = 2;
                            break;
                        case "2":
                            cash1 = 5;
                            break;
                        case "3":
                            cash1 = 10;
                            break;
                        case "4":
                            cash1 = 20;
                            break;
                        case "5":
                            cash1 = 50;
                            break;
                        case "6":
                            cash1 = 100;
                            break;
                    }
                    label2.Text = cash1 + "元,已退钞";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 开始收钞
        /// </summary>
        public void startPollCash()
        {
            totalCash = 0;
            label1.Text = "";
            label2.Text = "";
            Thread.Sleep(100);
            backgroundWorker1.RunWorkerAsync();
            Thread.Sleep(100);
        }

        /// <summary>
        /// 停止收钞
        /// </summary>
        public void stopPollCash()
        {
            Thread.Sleep(100);
            backgroundWorker1.CancelAsync();
            Thread.Sleep(100);
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        public string getCash()
        {
            return label1.Text + "," + totalCash + "元";
        }

        #endregion

        private void Form5_Load(object sender, EventArgs e)
        {
            //Crt580ComOpen();
            //InitWuLiHuJiaoQiPort();
            //没有标题
            this.FormBorderStyle = FormBorderStyle.None;
            //任务栏不显示
//            this.ShowInTaskbar = false;
            WindowState = FormWindowState.Maximized;
            Width = Convert.ToInt32(Properties.Settings.Default.width);
            Height = Convert.ToInt32(Properties.Settings.Default.height);
            webBrowser1.Width = Width;
            webBrowser1.Height = Height;
            webBrowser1.ScrollBarsEnabled = false;
            webBrowser1.ObjectForScripting = this;
            webBrowser1.Navigate(Properties.Settings.Default.url);

            if (!Properties.Settings.Default.showWebError)
            {
                webBrowser1.ScriptErrorsSuppressed = true; //屏蔽脚本错误
            }
//            this.TopMost = Properties.Settings.Default.topMost;

            if (!Properties.Settings.Default.testMode)
            {
                webBrowser1.IsWebBrowserContextMenuEnabled = false; //屏蔽右键
                label1.Hide();
                label2.Hide();
                button9.Hide();
                button10.Hide();
                button11.Hide();
                button12.Hide();
                button13.Hide();
                button14.Hide();
                button15.Hide();
                button16.Hide();
                button17.Hide();
                textBox1.Hide();
            }

//            Ledinit();
            _B2bDevice = new CCNET.Iccnet();
            bool devfound;
            //_B2bDevice.Port = New CCNET.COM_port
            //_B2bDevice.Port.RtsEnable = False
            //_B2bDevice.HandlePort = True
            //_B2bDevice.Port.BaudRate = 9600
            //_B2bDevice.Port.PortName = "COM3"
            //_B2bDevice.Device = CCNET.Device.Bill_to_Bill
            //_B2bDevice.Port.ReadTimeout = 100
            //devfound = True
            devfound = _B2bDevice.SearchAndConnect(CCNET.Device.Bill_Validator);
            if (devfound)
            {
                _B2bDevice.HandlePort = true;
                //backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                label1.Text = "Device was not detected";
            }
            //button1.Hide();
            //MessageBox.Show();
            //sfz_card_read();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            button1.FlatStyle = FlatStyle.Flat; //样式
            button1.ForeColor = Color.Transparent; //前景
            button1.BackColor = Color.Transparent; //去背景
            button1.FlatAppearance.BorderSize = 0; //去边线
        }

        //验证退出程序
        private void button1_Click_1(object sender, EventArgs e)
        {
            keybords kb = new keybords();
            kb.Show();
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            string s1;
            int pos = 50;
            s1 = "<?xml version='1.0' encoding='utf-8'?>" + "<print_info width='300' height='500'>";
            s1 += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos) + "' >就诊</tr>";
            s1 += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos += 30) + "' >位置</tr>";
            s1 += "<tr font='黑体' size='14' x='190' y='" + Convert.ToString(pos -= 15) + "' >上午1号</tr>";
            s1 += "<tr font='黑体' size='14' x='80' y='" + Convert.ToString(pos) + "' >门诊3层</tr>";
            s1 += "<tr font='黑体' size='12' x='70' y='" + Convert.ToString(pos += 45) + "' >海淀医院挂号单(自助)</tr>";
            s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 65) + "' >病案号:000089035800</tr>";
            //            s1 += "<tr font='黑体' size='13' x='280' y='" + Convert.ToString(pos) + "' >ID:" + PatId + "</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >神经内科门诊</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >号别:专科</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >姓名:张曙光</tr>";
            s1 += "<tr font='黑体' size='11' x='140' y='" + Convert.ToString(pos) + "' >性别:男</tr>";
            s1 += "<tr font='黑体' size='11' x='200' y='" + Convert.ToString(pos) + "' >60岁</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >身份:普通医保</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号费: 1.00元</tr>";
            s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >诊疗费:4.00元</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >自付:3.00元</tr>";
            s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >医保已支付:2.00元</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >垫付:0.00元</tr>";
            s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >实收:3.00元</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >机器编号:ZZ001</tr>";
            s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >流水号:956827</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
                  "' >挂号时间:2016-11-18 13:36:52</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >就诊时段:13:00--16:30</tr>";

            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝卡号:133****2634</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝支付:3.00元</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
                  "' >支付宝单号:20161207200080180828736</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
                  "' >流水号:ZZ00100001829097793687279172</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付状态:支付宝支付成功</tr>";

            s1 += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
                  "' >---------------------------------------------</tr>";
            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联卡号：" + 123 + "</tr>";
            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联支付：" + 123 + "</tr>";
            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联参考号：" + 123 + "</tr>";
            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易流水号：" + 123 + "</tr>";
            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易状态：" + 123 + "</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号单作为退号凭证，遗失不补</tr>";
            s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >如需挂号收据，缴费时请提前告知收费员</tr>";
            s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >请患者到相应分诊台分诊报到机报到就诊</tr>";
            s1 += "</print_info>";
            paint(s1, "123456789", 200, 40, 50, 130);
        }

        //获取配置文件信息
        public string getPropertiesValue(string propertiesName)
        {
            string propertiesValue = null;
            switch (propertiesName)
            {
                case "url":
                    propertiesValue = Properties.Settings.Default.url;
                    break;
                case "width":
                    propertiesValue = Properties.Settings.Default.width;
                    break;
                case "height":
                    propertiesValue = Properties.Settings.Default.height;
                    break;
                case "MySqlConnectStr":
                    propertiesValue = Properties.Settings.Default.MySqlConnectStr;
                    break;
                case "SSCardComPort":
                    propertiesValue = Properties.Settings.Default.SSCardComPort;
                    break;
                case "UID_Type":
                    propertiesValue = Properties.Settings.Default.UID_Type;
                    break;
                case "UID_Port":
                    propertiesValue = Properties.Settings.Default.UID_Port.ToString();
                    break;
                case "UID_BMPDIR":
                    propertiesValue = Properties.Settings.Default.UID_BMPDIR;
                    break;
                case "topMost":
                    propertiesValue = Properties.Settings.Default.topMost.ToString();
                    break;
                case "exePath":
                    propertiesValue = Properties.Settings.Default.exePath;
                    break;
                case "printerName":
                    propertiesValue = Properties.Settings.Default.printerName;
                    break;
                case "testMode":
                    propertiesValue = Properties.Settings.Default.testMode.ToString();
                    break;
                case "showWebError":
                    propertiesValue = Properties.Settings.Default.showWebError.ToString();
                    break;
                case "zzj_value":
                    propertiesValue = Properties.Settings.Default.zzj_value.ToString();
                    break;
            }

            return propertiesValue;
        }

        /// <summary>
        /// pdf打印，传ftp路径（大兴区医院）
        /// </summary>
        /// <param name="pdfFilePath">pdf文件的ftp地址</param>
        public void paintPdfFile(string pdfFilePath)
        {
            FtpUtil ftpUtil = new FtpUtil();
            ftpUtil.Download(pdfFilePath); //下载pdf文件，下载到output/pdfFiles/pdf文件名

            PdfDocument doc = new PdfDocument();
            string printFilePath = Application.StartupPath + "\\pdfFiles\\" +
                                   pdfFilePath.Split('/')[pdfFilePath.Split('/').Length - 1];
            doc.LoadFromFile(printFilePath);

            PrintDocument(doc, Properties.Settings.Default.printerName);
        }

        /// <summary>
        /// pdf打印，传pdf文件路径（展会）
        /// </summary>
        /// <param name="pdfFilePath">pdf文件的ftp地址</param>
        public void paintPdfFileTest(string pdfFilePath)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(pdfFilePath);
            PrintDocument(doc, Properties.Settings.Default.a5PrinterName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            paintPdfFile(Properties.Settings.Default.testImagePath);
        }

        private void PrintDocument(PdfDocument doc, string printername)
        {
            PrintDialog dialogPrint = new PrintDialog();

            doc.PrinterName = printername;
            doc.PageScaling = PdfPrintPageScaling.ActualSize;
            //PrintDocument类是实现打印功能的核心，它封装了打印有关的属性、事件、和方法
            PrintDocument printDoc = doc.PrintDocument;
            printDoc.PrintController = new StandardPrintController();
            // 获取PrinterSettings类的PrintDocument对象
            dialogPrint.Document = printDoc;

            printDoc.Print();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(xunPuPrinterGetStatus());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string s = HDread();
            MessageBox.Show(s);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string appPath = Application.StartupPath + "\\HandInput\\1.1.0.282\\handinput.exe";
            ProcessStartInfo process = new ProcessStartInfo
            {
                FileName = appPath,
                Arguments = "process 1",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            Process.Start(process);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            stopHandInput();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Crt580ReadCardNo());
        }

        private void _Comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(10000);
            string s = CommDataReceived();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MessageBox.Show(M100AComOpen("COM2",38400));
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show(moveCardToIcPosition());
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MessageBox.Show(getSerialNo());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            outCard();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            MessageBox.Show(M100AReadBlock(9, 0));
        }

        private void button15_Click(object sender, EventArgs e)
        {
            MessageBox.Show(getM100AStatus());
        }

        private void button16_Click(object sender, EventArgs e)
        {
            MessageBox.Show(callbackCard());
        }

        private void button17_Click(object sender, EventArgs e)
        {
            MessageBox.Show(M100AloadKey(9, "787332"));
        }

        private void button13_Click(object sender, EventArgs e)
        {
            MessageBox.Show(M100AWriteBlock(9, 0, textBox1.Text));
        }
    }
}