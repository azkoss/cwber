﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Net.FtpClient;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Spire.Pdf;

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
            this.BackgroundImage = Image.FromFile(@".\images\bg.png");
            // nOpenPinpad();
        }


        #region 非接读卡器
        //初始化IC卡读卡器
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

        public string getSectionData(int section)
        {
            string sectionData = "";
            StringBuilder temp = new StringBuilder(64);
//            st = DllClass.dc_reset(DllClass.IcDev, 0);
            Thread.Sleep(500);
            if (DllClass.dc_read_hex(DllClass.IcDev, 1, temp) == 0)
            {
                sectionData = Decode(Convert.ToString(temp));
            }
            MessageBox.Show(sectionData);
            return sectionData;
        }

        public static string Decode(string strDecode)
        {
            string sResult = "";
            for (int i = 0; i < strDecode.Length / 2; i++)
            {
                sResult += (char)short.Parse(strDecode.Substring(i * 2, 2), global::System.Globalization.NumberStyles.HexNumber);
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

        public String YYT_YB_GET_PATI(string zzj_id,string business_type)
        {
            var sOut = new StringBuilder(10240);
            var s = "<?xml version='1.0' encoding='gb2312'?>" +
                    "<root>" +
                    "<commitdata>" +
                    "<data>";
            s += "<datarow hao_ming ='09' code_value ='' patient_name='' business_type='" +business_type+"' />";
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
                        "' patient_id='" + ss[1] + "' card_code='" + ss[2] + "' card_no='" + ss[3] + "' responce_type='" +
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
                "' tczf='" + ss[9] + "'  record_id='" + ss[10] + "' bk_card_no='' trade_no='" + ss[11] + "' stream_no='" +
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
                        strPrintData += "<tr font='黑体' size='7' x='20' y='" + Convert.ToString(pos) + "' >" + array1[0] + "</tr>";
                        if (array1[1].Trim() != "")
                        {
                            strPrintData += "<tr font='黑体' size='7' x='170' y='" + Convert.ToString(pos) + "' >" + array1[1] + "</tr>";
                        }
                        strPrintData += "<tr font='黑体' size='7' x='245' y='" + Convert.ToString(pos) + "' >" + array1[2] + "</tr>";
                        strPrintData += "<tr font='黑体' size='7' x='288' y='" + Convert.ToString(pos) + "' >" + array1[3] + "</tr>";
                    }
                }
                if (CardCode == "20") //医保
                {
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >自付金额：" + pay_charge_total + " 元</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >医保个人账户支付金额：" + zhzf + " 元</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >基金支付：" + tczf + " 元</tr>";
                }
                if (CardCode != "20") //院内减免
                {
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >自付金额：" + pay_charge_total + " 元</tr>";
                    if (tczf != "" || tczf != "0")
                    {
                        strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >医院垫付：" + tczf + " 元</tr>";
                    }
                }
                strPrintData += "<tr font='黑体' size='12' x='20' y='" + Convert.ToString(pos += 20) + "' >总金额：" + charge_total + " 元</tr>";
                strPrintData += "<tr font='黑体' size='8' x='20' y='" + Convert.ToString(pos += 20) + "' >注：医保 标志 ①.无自付 ②.有自付 ③.全支付</tr>";
            }
            if (alipay_status.Contains("支付宝"))
            {
                if (alipay_status.Contains("退款"))
                {
                    strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) + "' >---------------------------------------------</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝卡号：" + idcard + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝支付：" + alipay_total + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝单号：" + trade_no + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" + stream_no + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝状态：" + alipay_tuihuan_status + "</tr>";
                }
                else
                {
                    strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) + "' >---------------------------------------------</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝卡号：" + idcard + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝支付：" + alipay_total + " 元</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝单号：" + trade_no + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" + stream_no + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付状态：" + alipay_status + "</tr>";
                }
            }
            else if (alipay_status.Contains("银联"))
            {
                strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) + "' >---------------------------------------------</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联卡号：" + idcard + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联支付：" + alipay_total + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" + stream_no + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易流水号：" + trade_no + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易状态：" + alipay_status + "</tr>";
            }
            else
            {
                strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) + "' >---------------------------------------------</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" + stream_no + "</tr>";
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
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >缴费状态：" + yb_status + "</tr>";
                int i;
                for (i = 1; i < m - 1; i++)
                {
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >" + str.Substring(i * 25, 25) + "</tr>";
                }
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >" + str.Substring(i * 25, n - i * 25) + "</tr>";
            }
            else
            {
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >缴费状态：" + yb_status + "</tr>";
//                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >退费状态：" + alipay_tuihuan_status + "</tr>";
            }
            strPrintData += "<tr font='黑体' size='11' x='10' y='" + Convert.ToString(pos += 20) + "' >*温馨提示：请保存此凭证，请勿遗失</tr>";
            strPrintData += "<tr font='黑体' size='11' x='10' y='" + Convert.ToString(pos += 20) + "' >*如需发票请去咨询台打印</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 30) + "' >机器名称：" + OperAtor + "</tr>";
            strPrintData += "</print_info>";
            strPrintData = strPrintData.Replace("<print_info width='300' height='400'>", "<print_info width='300' height='" + (420 + mx_count * 35).ToString() + "'>");
            pr1.PrintDataJF(strPrintData, ref tmp1);
        }

        //挂号打印的方法
        public void PrtJzDateCreatGuaHao(string Position = "", string ampm = "", string RegNo = "", string PatBingAnHao = "", string PatId = "",
            string PatDept = "", string RegType = "", string PatName = "", string PatSex = "", string PatAge = "",
            string PatType = "", string GuaHaoFee = "", string ZhenLiaoFee = "", string ZiFu = "",
            string YiBaoZhiFu = "", string DianFu = "", string ShiShou = "", string OperAtor = "",
            string LiuShuiHao = "", string GuaHaoTime = "", string JzTime = "",string ZFBNum = "", string ZFBDanHao = "", string ZFBStream = "", string ZFBStatus = "")
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
            strPrintData += "<tr font='黑体' size='14' x='190' y='" + Convert.ToString(pos -= 15) + "' >" + ampm + RegNo + "号</tr>";
            strPrintData += "<tr font='黑体' size='14' x='80' y='" + Convert.ToString(pos) + "' >" + Position + "</tr>";
            strPrintData += "<tr font='黑体' size='12' x='70' y='" + Convert.ToString(pos += 45) + "' >海淀医院挂号单(自助)</tr>";
            strPrintData += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 50) + "' >病案号:" + PatBingAnHao + "</tr>";
            //            strPrintData += "<tr font='黑体' size='13' x='280' y='" + Convert.ToString(pos) + "' >ID:" + PatId + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >" + PatDept + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >号别:" + RegType + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >姓名:" + PatName + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='140' y='" + Convert.ToString(pos) + "' >性别:" + PatSex + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='200' y='" + Convert.ToString(pos) + "' >" + PatAge + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >身份:" + PatType + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号费:" + GuaHaoFee + "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >诊疗费:" + ZhenLiaoFee + "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >自付:" + ZiFu + "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >医保已支付:" + YiBaoZhiFu + "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >" + DianFu + "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >实收:" + ShiShou + "元</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >机器编号:" + OperAtor + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >流水号:" + LiuShuiHao + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号时间:" + GuaHaoTime + "</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >" + JzTime + "</tr>";
//            strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
//                                "' >---------------------------------------------</tr>";
            if (ZFBStatus.Contains("支付宝"))
            {
                if (ZFBStatus.Contains("退款"))
                {
                    strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) + "' >---------------------------------------------</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝卡号：" + ZFBNum + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝支付：" + ShiShou + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝单号：" + ZFBDanHao + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >流水号：" + ZFBStream + "</tr>";
                    strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >支付宝状态：" + ZFBStatus + "</tr>";
                }
                else
                {
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝卡号:" + ZFBNum + "</tr>";
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝支付:" + ShiShou + "元</tr>";
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝单号:" + ZFBDanHao + "</tr>";
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >流水号:" + ZFBStream + "</tr>";
                    strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付状态:" + ZFBStatus + "</tr>";
                }
            }
            if (ZFBStatus.Contains("银联"))
            {
                strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) + "' >---------------------------------------------</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联卡号：" + ZFBNum + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联支付：" + ShiShou + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联参考号：" + ZFBDanHao + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易流水号：" + ZFBStream + "</tr>";
                strPrintData += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易状态：" + ZFBStatus + "</tr>";
            }
//            strPrintData += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) +
//                                "' >---------------------------------------------</tr>";
            strPrintData += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号单作为退号凭证，遗失不补</tr>";
            strPrintData += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >如需挂号收据，缴费时请提前告知收费员</tr>";
            strPrintData += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >请患者到相应分诊台分诊报到机报到就诊</tr>";
            strPrintData += "</print_info>";
            strPrintData = strPrintData.Replace("<print_info width='300' height='400'>", "<print_info width='300' height='" + (420 + mx_count * 35).ToString() + "'>");
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
//                        s1 = "<?xml version='1.0' encoding='utf-8'?>" + "<print_info width='300' height='500'>";
//                        s1 += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos) + "' >就诊</tr>";
//                        s1 += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos += 30) + "' >位置</tr>";
//                        s1 += "<tr font='黑体' size='14' x='190' y='" + Convert.ToString(pos -= 15) + "' >上午1号</tr>";
//                        s1 += "<tr font='黑体' size='14' x='80' y='" + Convert.ToString(pos) + "' >门诊3层</tr>";
//                        s1 += "<tr font='黑体' size='12' x='70' y='" + Convert.ToString(pos += 45) + "' >海淀医院挂号单(自助)</tr>";
//                        s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 50) + "' >病案号:000089035800</tr>";
//                        //            s1 += "<tr font='黑体' size='13' x='280' y='" + Convert.ToString(pos) + "' >ID:" + PatId + "</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >神经内科门诊</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >号别:专科</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >姓名:张曙光</tr>";
//                        s1 += "<tr font='黑体' size='11' x='140' y='" + Convert.ToString(pos) + "' >性别:男</tr>";
//                        s1 += "<tr font='黑体' size='11' x='200' y='" + Convert.ToString(pos) + "' >60岁</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >身份:普通医保</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号费: 1.00元</tr>";
//                        s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >诊疗费:4.00元</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >自付:3.00元</tr>";
//                        s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >医保已支付:2.00元</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >垫付:0.00元</tr>";
//                        s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >实收:3.00元</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >机器编号:ZZ001</tr>";
//                        s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >流水号:956827</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
//                              "' >挂号时间:2016-11-18 13:36:52</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >就诊时段:13:00--16:30</tr>";
//            
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝卡号:133****2634</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝支付:3.00元</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
//                              "' >支付宝单号:20161207200080180828736</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
//                              "' >流水号:ZZ00100001829097793687279172</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付状态:支付宝支付成功</tr>";
//
//                            s1 += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) + "' >---------------------------------------------</tr>";
//                            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联卡号：" + ZFBNum + "</tr>";
//                            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联支付：" + ShiShou + "</tr>";
//                            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联参考号：" + ZFBDanHao + "</tr>";
//                            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易流水号：" + ZFBStream + "</tr>";
//                            s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易状态：" + ZFBStatus + "</tr>";
//                        s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号单作为退号凭证，遗失不补</tr>";
//                        s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >如需挂号收据，缴费时请提前告知收费员</tr>";
//                        s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >请患者到相应分诊台分诊报到机报到就诊</tr>";
//                        s1 += "</print_info>";
            s1 = s1.Replace("<print_info width='300' height='400'>",
                "<print_info width='300' height='" + (420 + mx_count * 35).ToString() + "'>");
            if(s2!="")
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
            string sqlstr = "update devstatus set Paper_End = '" + arr[3] + "', Paper_Near_End = '无纸尽传感器', Ticket_Out = '没有出纸口检测功能', Paper_Jam = '无卡纸检测功能', Cover_Open = '" + arr[1] +
                            "',UpdateTime = '" + timenow + "' where DevName = '" + Properties.Settings.Default.DevName + "'";
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

        private void Form5_Load(object sender, EventArgs e)
        {
            //没有标题
            this.FormBorderStyle = FormBorderStyle.None;
            //任务栏不显示
            this.ShowInTaskbar = false;
            this.Height = int.Parse(WinFormDemo.Properties.Settings.Default.height);
            this.Width = int.Parse(WinFormDemo.Properties.Settings.Default.width);
            webBrowser1.ScrollBarsEnabled = false;
            webBrowser1.ObjectForScripting = this;
            webBrowser1.Navigate(Properties.Settings.Default.url);
            webBrowser1.IsWebBrowserContextMenuEnabled = false; //屏蔽右键
            if (!Properties.Settings.Default.showWebError)      
            {
                webBrowser1.ScriptErrorsSuppressed = true;          //屏蔽脚本错误
            }
//            this.TopMost = Properties.Settings.Default.topMost;

            if (!Properties.Settings.Default.testMode)
            {
                button2.Hide();
                button3.Hide();
            }

            //button1.Hide();
            //MessageBox.Show();
            //sfz_card_read();
        }

        

//        private void button1_Click(object sender, EventArgs e)
//        {
//            PrtJzDateCreatGuaHao("三层西南侧", "下午", "35", "000089035800", "120305113800", "神经内科门诊", "专科", "张曙光", "男", "60岁", "普通医保",
//                "1.00", "4.00", "3.00", "2.00", "0.00", "3.00", "ZZ001", "956827", "2016-11-18 13:36:52",
//                "13:00--16:30","133****2634","20161207200080180828736","ZZ00100001829097793687279172","支付宝支付成功");
//        }


        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            button1.FlatStyle = FlatStyle.Flat;//样式
            button1.ForeColor = Color.Transparent;//前景
            button1.BackColor = Color.Transparent;//去背景
            button1.FlatAppearance.BorderSize = 0;//去边线
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartLisPrint();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            DllClass.AutoEnlargeKeyC(true);
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
            
                s1 += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) + "' >---------------------------------------------</tr>";
                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联卡号：" + 123 + "</tr>";
                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联支付：" + 123 + "</tr>";
                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联参考号：" + 123 + "</tr>";
                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易流水号：" + 123 + "</tr>";
                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易状态：" + 123 + "</tr>";
            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号单作为退号凭证，遗失不补</tr>";
            s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >如需挂号收据，缴费时请提前告知收费员</tr>";
            s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >请患者到相应分诊台分诊报到机报到就诊</tr>";
            s1 += "</print_info>";
            paint(s1,"123456789",200,40,50,130);
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
            ftpUtil.Download(pdfFilePath);    //下载pdf文件，下载到output/pdfFiles/pdf文件名

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
    }
}