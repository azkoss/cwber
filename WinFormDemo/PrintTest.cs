using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormDemo
{
    public partial class PrintTest : Form
    {
        public PrintTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tmp1 = "";
            PrintLpt pr1 = new PrintLpt();
            int pos = 10;
            int mx_count = 0;
            string xml;
            xml = "<?xml version='1.0' encoding='utf-8'?>" + "<print_info width='300' height='500'>";
            xml += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos) + "' >就诊</tr>";
            xml += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos += 30) + "' >位置</tr>";
            xml += "<tr font='黑体' size='14' x='190' y='" + Convert.ToString(pos -= 15) + "' >上午1号</tr>";
            xml += "<tr font='黑体' size='14' x='80' y='" + Convert.ToString(pos) + "' >门诊3层</tr>";
            xml += "<tr font='黑体' size='12' x='70' y='" + Convert.ToString(pos += 45) + "' >海淀医院挂号单(自助)</tr>";
            xml += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 50) + "' >病案号:000089035800</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >神经内科门诊</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >号别:专科</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >姓名:张曙光</tr>";
            xml += "<tr font='黑体' size='11' x='140' y='" + Convert.ToString(pos) + "' >性别:男</tr>";
            xml += "<tr font='黑体' size='11' x='200' y='" + Convert.ToString(pos) + "' >60岁</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >身份:普通医保</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号费: 1.00元</tr>";
            xml += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >诊疗费:4.00元</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >自付:3.00元</tr>";
            xml += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >医保已支付:2.00元</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >垫付:0.00元</tr>";
            xml += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >实收:3.00元</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >机器编号:ZZ001</tr>";
            xml += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >流水号:956827</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
                  "' >挂号时间:2016-11-18 13:36:52</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >就诊时段:13:00--16:30</tr>";

            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝卡号:133****2634</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝支付:3.00元</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
                  "' >支付宝单号:20161207200080180828736</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
                  "' >流水号:ZZ00100001829097793687279172</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付状态:支付宝支付成功</tr>";
            xml += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号单作为退号凭证，遗失不补</tr>";
            xml += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >如需挂号收据，缴费时请提前告知收费员</tr>";
            xml += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >请患者到相应分诊台分诊报到机报到就诊</tr>";
            xml += "</print_info>";
            xml = xml.Replace("<print_info width='300' height='400'>",
                "<print_info width='300' height='" + (420 + mx_count * 35).ToString() + "'>");
            PrtJzDateCreatGuaHao(xml, "120305113800", Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text), Convert.ToInt32(textBox4.Text));
        }


        public void PrtJzDateCreatGuaHao(string s1, string s2, int width, int height, int x, int y)
        {
            string tmp1 = "";
            PrintLpt pr1 = new PrintLpt();
            int pos = 10;
            int mx_count = 0;
            decimal AllMoney = 0;
            //            s1 = "<?xml version='1.0' encoding='utf-8'?>" + "<print_info width='300' height='500'>";
            //            s1 += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos) + "' >就诊</tr>";
            //            s1 += "<tr font='黑体' size='14' x='35' y='" + Convert.ToString(pos += 30) + "' >位置</tr>";
            //            s1 += "<tr font='黑体' size='14' x='190' y='" + Convert.ToString(pos -= 15) + "' >上午1号</tr>";
            //            s1 += "<tr font='黑体' size='14' x='80' y='" + Convert.ToString(pos) + "' >门诊3层</tr>";
            //            s1 += "<tr font='黑体' size='12' x='70' y='" + Convert.ToString(pos += 45) + "' >海淀医院挂号单(自助)</tr>";
            //            s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 50) + "' >病案号:000089035800</tr>";
            //            //            s1 += "<tr font='黑体' size='13' x='280' y='" + Convert.ToString(pos) + "' >ID:" + PatId + "</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >神经内科门诊</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >号别:专科</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >姓名:张曙光</tr>";
            //            s1 += "<tr font='黑体' size='11' x='140' y='" + Convert.ToString(pos) + "' >性别:男</tr>";
            //            s1 += "<tr font='黑体' size='11' x='200' y='" + Convert.ToString(pos) + "' >60岁</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >身份:普通医保</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号费: 1.00元</tr>";
            //            s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >诊疗费:4.00元</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >自付:3.00元</tr>";
            //            s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >医保已支付:2.00元</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >垫付:0.00元</tr>";
            //            s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >实收:3.00元</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >机器编号:ZZ001</tr>";
            //            s1 += "<tr font='黑体' size='11' x='150' y='" + Convert.ToString(pos) + "' >流水号:956827</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
            //                  "' >挂号时间:2016-11-18 13:36:52</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >就诊时段:13:00--16:30</tr>";
            //
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝卡号:133****2634</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付宝支付:3.00元</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
            //                  "' >支付宝单号:20161207200080180828736</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) +
            //                  "' >流水号:ZZ00100001829097793687279172</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >支付状态:支付宝支付成功</tr>";

            //                s1 += "<tr font='黑体' size='12' x='10' y='" + Convert.ToString(pos += 10) + "' >---------------------------------------------</tr>";
            //                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联卡号：" + ZFBNum + "</tr>";
            //                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联支付：" + ShiShou + "</tr>";
            //                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联参考号：" + ZFBDanHao + "</tr>";
            //                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易流水号：" + ZFBStream + "</tr>";
            //                s1 += "<tr font='黑体' size='10' x='20' y='" + Convert.ToString(pos += 20) + "' >银联交易状态：" + ZFBStatus + "</tr>";
            //            s1 += "<tr font='黑体' size='11' x='35' y='" + Convert.ToString(pos += 30) + "' >挂号单作为退号凭证，遗失不补</tr>";
            //            s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >如需挂号收据，缴费时请提前告知收费员</tr>";
            //            s1 += "<tr font='黑体' size='10' x='35' y='" + Convert.ToString(pos += 30) + "' >请患者到相应分诊台分诊报到机报到就诊</tr>";
            //            s1 += "</print_info>";
            s1 = s1.Replace("<print_info width='300' height='400'>",
                "<print_info width='300' height='" + (420 + mx_count * 35).ToString() + "'>");
            pr1.S = s2;
            pr1.Width = width;
            pr1.Height = height;
            pr1.X = x;
            pr1.Y = y;
            pr1.PrintDataGH(s1, ref tmp1);
        }

        private void PrintTest_Load(object sender, EventArgs e)
        {
            this.reportViewer1.RefreshReport();
        }

        private void button2_Click(object sender, EventArgs e)
        {
//            reportViewer1
//            reportViewer1.Print();
        }


    }
}
