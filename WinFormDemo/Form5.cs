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
using System.Runtime.Serialization.Formatters.Binary;
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
                    return data.Remove(16,data.Length-16).ToString();
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


        public int hCom = 0;
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
            if (hCom == 0)
            {
                hCom = M100A_CommOpenWithBaud(comPort, baudate);  
            }
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
            Thread.Sleep(100);
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
            Thread.Sleep(100);
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
            Thread.Sleep(100);
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
            Thread.Sleep(100);
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
            Thread.Sleep(100);
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
            Thread.Sleep(100);
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
            Thread.Sleep(100);
            byte[] blockData = new byte[16];
            if (M100A_S50ReadBlock(hCom, false, 0, sectionAddr, blockAddr, blockData, Info) == 0)
            {
                return Encoding.Default.GetString(blockData).Replace("\0", "");
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
        /// <param name="data">数据</param>
        /// <returns></returns>
        public string M100AWriteBlock(byte sectionAddr, byte blockAddr,string data)
        {
            Thread.Sleep(100);
            return M100A_S50WriteBlock(hCom, false, 0, sectionAddr, blockAddr, Encoding.Default.GetBytes(data), Info) == 0
                ? "写入成功"
                : "写入失败";
        }
        #endregion

        #region K710
        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_CommOpen(int Port);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_CommOpenWithBaud(int Port, int Baudate);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_CommClose(int ComHandle);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_GetDllVersion(int ComHandle, char[] strVersion);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_GetSysVersion(int ComHandle, byte MacAddr, byte[] strVersion);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_SendCmd(int ComHandle, byte MacAddr, byte[] p_Cmd, int len);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_Query(int ComHandle, byte MacAddr, byte[] StateInfo);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_SensorQuery(int ComHandle, byte MacAddr, byte[] StateInfo);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_AutoTestMac(int ComHandle, byte MacAddr);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_ReadRecycleCardNum(int ComHandle, byte MacAddr, byte[] szData);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_SetCardNum(int ComHandle, byte MacAddr, int Num);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_DisEnableCount(int ComHandle, byte MacAddr);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_EnableCount(int ComHandle, byte MacAddr);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_ReadCardNumber(int ComHandle, byte MacAddr, byte[] CardNumber);

        byte MacAddress = 0;
        int ComHandle = 0;

        public string K710ComOpen()
        {
            int nRet = 1;
            byte[] Recv = new byte[200];
            if (ComHandle == 0)
            {
                ComHandle = D1000_CommOpenWithBaud(int.Parse(Properties.Settings.Default.K710ComPort.Substring(3, 1)), 9600);
            }
            if (ComHandle > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    nRet = D1000_AutoTestMac(ComHandle, (byte)i);
                    if (nRet == 0)
                    {
                        MacAddress = (byte)i;
                        break;
                    }
                }

                if (nRet == 0)
                {
                    return "设备连接成功";
                }
                else
                {
                    return "设备连接失败";
                }
            }
            else
            {
                return "串口打开失败";
            }
        }

        public string K710RecyleCard()
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte)'C';
            sendcmd[1] = (byte)'P';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 2);
            if (nRet == 0)
            {
                return "回收成功";
            }
            else
                return "回收失败";
        }

        public string K710Reset()
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte)'R';
            sendcmd[1] = (byte)'S';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 2);
            if (nRet == 0)
            {
                return "复位成功";
            }
            else
                return "复位失败";
        }

        public string K710AllowBeep()
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte)'B';
            sendcmd[1] = (byte)'E';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 2);
            if (nRet == 0)
            {
                return "允许蜂鸣设置成功";
            }
            else
                return "允许蜂鸣设置失败";
        }

        public string K710DisAllowBeep()
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte)'B';
            sendcmd[1] = (byte)'D';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 2);
            if (nRet == 0)
            {
                return "禁止蜂鸣设置成功";
            }
            else
                return "禁止蜂鸣设置失败";
        }

        public string K710MoveToReadCardPosition()
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte)'F';
            sendcmd[1] = (byte)'C';
            sendcmd[2] = (byte)'7';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 3);
            if (nRet == 0)
            {
                return "发卡到读卡位置成功";
            }
            else
                return "发卡到读卡位置失败";
        }

        public string K710OutCard()
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte)'F';
            sendcmd[1] = (byte)'C';
            sendcmd[2] = (byte)'4';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 3);
            if (nRet == 0)
            {
                return "发卡到取卡位置成功";
            }
            else
                return "发卡到取卡位置失败";
        }

        public string K710ReadCardNo()
        {
            byte[] cardNo = new byte[5];
            string result = "";
            if (D1000_ReadCardNumber(ComHandle, MacAddress, cardNo) == 0)
            {
                foreach (byte b in cardNo)
                {
                    result += b.ToString("x2");
                }
                return "读卡成功:" + result;
            }
            else
            {
                return "读卡失败";
            }
        }

        public string K710GetStatus()
        {
            string result = "";
            byte[] StateInfo = new byte[4];
            if (D1000_SensorQuery(ComHandle, MacAddress, StateInfo) == 0)
            {
                result += "发卡机状态获取成功:";
                switch (StateInfo[0])
                {
                    case 0x38: //保留

                        break;
                    case 0x34: //命令不能执行
                        result += "命令不能执行&请点击“复位”,";
                        break;
                    case 0x32: //准备卡失败
                        result += "准备卡失败&请点击“复位”,";
                        break;
                    case 0x31: //正在准备卡
                        result += "正在准备卡,";
                        break;
                    case 0x30: //机器空闲
                        result += "机器空闲,";
                        break;
                }

                switch (StateInfo[1])
                {
                    case 0x38: //正在发卡
                        result += "正在发卡,";
                        break;

                    case 0x34: //正在收卡
                        result += "正在收卡,";
                        break;

                    case 0x32: //发卡出错
                        result += "发卡过程出错&请点击复位,";
                        break;

                    case 0x31: //收卡出错
                        result += "收卡过程出错&请点击复位,";
                        break;

                    case 0x30: //没有任何动作
                        result += "没有任何动作,";
                        break;
                }
                switch (StateInfo[2])
                {
                    case 0x39:
                        result += "回收卡箱已满&";

                        result += "卡箱预空,";

                        break;
                    case 0x38: //无捕卡

                        result += "回收卡箱已满&";

                        result += "卡箱非预空,";
                        break;

                    case 0x34: //重叠卡
                        result += "重叠卡,";
                        break;

                    case 0x32: //卡堵塞
                        result += "卡堵塞,";
                        break;

                    case 0x31: //卡箱预空
                        result += "卡箱预空,";
                        break;

                    case 0x30: //卡箱为非预空状态
                        result += "卡箱为非预空状态,";
                        break;

                }

                switch (StateInfo[3])
                {
                    case 0x3E: //只有一张卡，在传感器２-３位置
                        result += "只有一张卡在传感器２-３位置";
                        break;

                    case 0x3B: //只有一张卡，在传感器１-２位置
                        result += "只有一张卡在传感器１-２位置";
                        break;

                    case 0x39: //只有一张卡，在传感器１位置
                        result += "只有一张卡在传感器１位置";
                        break;

                    case 0x38: //卡箱是空的已经无任何卡片
                        result += "卡箱是空的已经无任何卡片";
                        break;

                    case 0x36: //在传感器２和３的位置
                        result += "卡在传感器２和３的位置";
                        break;

                    case 0x34: //在传感器３位置，预发卡位置
                        result += "卡在传感器３预发卡位置";
                        break;

                    case 0x33: //在传感器１和２的位置(读卡位置)
                        result += "卡在传感器１和２的位置(读卡位置)";
                        break;

                    case 0x32: //在传感器２的位置
                        result += "在传感器２的位置";
                        break;

                    case 0x31: //在卡口位置传感器１位置(取卡位置)
                        result += "卡在卡口位置传感器１位置(取卡位置)";
                        break;

                }
            }
            else
            {
                result = "发卡机状态获取失败";
            }
            return result;
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

        #region FormLoad

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

        #endregion
        

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

        private void button18_Click(object sender, EventArgs e)
        {
            paint("<?xml version='1.0' encoding='utf-8'?><print_info width='300' height='500'><tr font='宋体' size='16' x='270' y='10' >北京电力医院处方笺1</tr><tr font='宋体' size='10' x='95' y='50' >定点医疗机构编码: 06110001</tr><tr font='黑体' size='16' x='290' y='50' >医保</tr><tr font='宋体' size='10' x='580' y='50' >医疗保险</tr><tr font='宋体' size='10' x='95' y='70' >处方号: 15478355</tr><tr font='宋体' size='12' x='430' y='70' >心内门诊</tr><tr font='宋体' size='10' x='580' y='70' >门诊号: 00104109</tr><tr font='宋体' size='10' x='95' y='100' >姓名: 风来景</tr><tr font='宋体' size='10' x='200' y='100' >性别:女</tr><tr font='宋体' size='10' x='270' y='100' >年龄:59岁</tr><tr font='宋体' size='10' x='350' y='100' >单位: 北京京丰印刷厂</tr><tr font='黑体' size='10' x='80' y='110' >______________________________________________________________________________________</tr><tr font='黑体' size='10' x='77' y='123' >|</tr><tr font='黑体' size='10' x='77' y='136' >|</tr><tr font='黑体' size='10' x='77' y='149' >|</tr><tr font='黑体' size='10' x='77' y='162' >|</tr><tr font='黑体' size='10' x='77' y='175' >|</tr><tr font='黑体' size='10' x='77' y='188' >|</tr><tr font='宋体' size='10' x='80' y='125' >临床诊断:</tr><tr font='宋体' size='10' x='95' y='145' >高血压,骨质疏</tr><tr font='宋体' size='10' x='95' y='160' >松,结膜炎,糖尿</tr><tr font='宋体' size='10' x='95' y='175' >病,糖尿病性视网</tr><tr font='宋体' size='10' x='95' y='190' >膜病变,行动不</tr><tr font='宋体' size='10' x='95' y='205' >变,周围神经病</tr><tr font='宋体' size='10' x='95' y='520' >过敏试验:</tr><tr font='黑体' size='10' x='215' y='123' >|</tr><tr font='黑体' size='10' x='215' y='136' >|</tr><tr font='黑体' size='10' x='215' y='149' >|</tr><tr font='黑体' size='10' x='215' y='162' >|</tr><tr font='黑体' size='10' x='215' y='175' >|</tr><tr font='黑体' size='10' x='215' y='188' >|</tr><tr font='黑体' size='10' x='215' y='201' >|</tr><tr font='黑体' size='10' x='215' y='214' >|</tr><tr font='黑体' size='10' x='215' y='227' >|</tr><tr font='黑体' size='10' x='215' y='240' >|</tr><tr font='黑体' size='10' x='225' y='130' >localPicture:F://PxTwo.png</tr><tr font='黑体' size='10' x='235' y='170' >阿莫西林胶囊(啊啊肯定是看看里)(1片*7片/盒)</tr><tr font='黑体' size='10' x='560' y='170' >8盒</tr><tr font='黑体' size='10' x='620' y='170' >233.92有自付</tr><tr font='黑体' size='10' x='235' y='230' >用法:2片/次</tr><tr font='黑体' size='10' x='400' y='230' >1次/日</tr><tr font='黑体' size='10' x='480' y='230' >口服</tr><tr font='黑体' size='10' x='520' y='230' >空腹或进餐服用</tr><tr font='黑体' size='10' x='225' y='250' >base64Picture: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dFJpZ2h0bG9uZwAAAAAAAAALY3JvcFJlY3RUb3Bsb25nAAAAAAA4QklNA+0AAAAAABAAYAAAAAEAAgBgAAAAAQACOEJJTQQmAAAAAAAOAAAAAAAAAAAAAD+AAAA4QklNBA0AAAAAAAQAAAAeOEJJTQQZAAAAAAAEAAAAHjhCSU0D8wAAAAAACQAAAAAAAAAAAQA4QklNJxAAAAAAAAoAAQAAAAAAAAACOEJJTQP1AAAAAABIAC9mZgABAGxmZgAGAAAAAAABAC9mZgABAKGZmgAGAAAAAAABADIAAAABAFoAAAAGAAAAAAABADUAAAABAC0AAAAGAAAAAAABOEJJTQP4AAAAAABwAAD/////////////////////////////A+gAAAAA/////////////////////////////wPoAAAAAP////////////////////////////8D6AAAAAD/////////////////////////////A+gAADhCSU0ECAAAAAAAEAAAAAEAAAJAAAACQAAAAAA4QklNBB4AAAAAAAQAAAAAOEJJTQQaAAAAAAM3AAAABgAAAAAAAAAAAAAATQAAAEsAAAABADEAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAEsAAABNAAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAAEAAAAAAABudWxsAAAAAgAAAAZib3VuZHNPYmpjAAAAAQAAAAAAAFJjdDEAAAAEAAAAAFRvcCBsb25nAAAAAAAAAABMZWZ0bG9uZwAAAAAAAAAAQnRvbWxvbmcAAABNAAAAAFJnaHRsb25nAAAASwAAAAZzbGljZXNWbExzAAAAAU9iamMAAAABAAAAAAAFc2xpY2UAAAASAAAAB3NsaWNlSURsb25nAAAAAAAAAAdncm91cElEbG9uZwAAAAAAAAAGb3JpZ2luZW51bQAAAAxFU2xpY2VPcmlnaW4AAAANYXV0b0dlbmVyYXRlZAAAAABUeXBlZW51bQAAAApFU2xpY2VUeXBlAAAAAEltZyAAAAAGYm91bmRzT2JqYwAAAAEAAAAAAABSY3QxAAAABAAAAABUb3AgbG9uZwAAAAAAAAAATGVmdGxvbmcAAAAAAAAAAEJ0b21sb25nAAAATQAAAABSZ2h0bG9u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3gCfuWV9VWlv1c6dJkuoY7/OG/8A78tO4xTYfBp/IqH1b/8AE/03v+q0/wDUNSU6SSSSSn//0/VUkkklKSSSSUpJJJJSO8E02Aclp/IqP1b0+r/TR4YtI+5jVomIM8d1Gr0vTb6O30oGzZG3b+bt2+3akpmkkkkp/9k4QklNBCEAAAAAAFUAAAABAQAAAA8AQQBkAG8AYgBlACAAUABoAG8AdABvAHMAaABvAHAAAAATAEEAZABvAGIAZQAgAFAAaABvAHQAbwBzAGgAbwBwACAAQwBTADYAAAABADhCSU0EBgAAAAAABwAEAAAAAQEA/+EMtWh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8APD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS4zLWMwMTEgNjYuMTQ1NjYxLCAyMDEyLzAyLzA2LTE0OjU2OjI3ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1sbnM6ZGM9Imh0dHA6Ly9wdXJsLm9yZy9kYy9lbGVtZW50cy8xLjEvIiB4bWxuczpwaG90b3Nob3A9Imh0dHA6Ly9ucy5hZG9iZS5jb20vcGhvdG9zaG9wLzEuMC8iIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyIgeG1wTU06RG9jdW1lbnRJRD0iMTJENDAyQkQ1NEQ4MzNBMkRCREFCNEU4NTc0MkVEODQiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6Q0YwMUJFRTIwNDFCRTgxMUJBRUNERDc3ODg2NTVBQzgiIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0iMTJENDAyQkQ1NEQ4MzNBMkRCREFCNEU4NTc0MkVEODQiIGRjOmZvcm1hdD0iaW1hZ2UvanBlZyIgcGhvdG9zaG9wOkNvbG9yTW9kZT0iMyIgeG1wOkNyZWF0ZURhdGU9IjIwMTgtMDItMjZUMjM6MDI6MTQrMDg6MDAiIHhtcDpNb2RpZnlEYXRlPSIyMDE4LTAyLTI2VDIzOjEzOjExKzA4OjAwIiB4bXA6TWV0YWRhdGFEYXRlPSIyMDE4LTAyLTI2VDIzOjEzOjExKzA4OjAwIj4gPHhtcE1NOkhpc3Rvcnk+IDxyZGY6U2VxPiA8cmRmOmxpIHN0RXZ0OmFjdGlvbj0ic2F2ZWQiIHN0RXZ0Omluc3RhbmNlSUQ9InhtcC5paWQ6Q0YwMUJFRTIwNDFCRTgxMUJBRUNERDc3ODg2NTVBQzgiIHN0RXZ0OndoZW49IjIwMTgtMDItMjZUMjM6MTM6MTErMDg6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCBDUzYgKFdpbmRvd3MpIiBzdEV2dDpjaGFuZ2VkPSIvIi8+IDwvcmRmOlNlcT4gPC94bXBNTTpIaXN0b3J5PiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8P3hwYWNrZXQgZW5kPSJ3Ij8+/+4ADkFkb2JlAGQAAAAAAf/bAIQABgQEBAUEBgUFBgkGBQYJCwgGBggLDAoKCwoKDBAMDAwMDAwQDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAEHBwcNDA0YEBAYFA4ODhQUDg4O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</tr></print_info>","",0,0,0,0);
        }

        protected Bitmap Base64StringToImage(string strbase64)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(strbase64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);

                //                bmp.Save(@"d:\test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp.Save(@"d:\"test.bmp", ImageFormat.Bmp);  
                //bmp.Save(@"d:\"test.gif", ImageFormat.Gif);  
                //bmp.Save(@"d:\"test.png", ImageFormat.Png);  
                ms.Close();
                return bmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }  
    }
}