using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormDemo
{
    public partial class print : Form
    {
        public static List<Struce.JZINFO> jz = new List<Struce.JZINFO>();//该病人的就诊详单
        public print()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
//            Form5.PrtJzDateCreat(CardCode.Text, PatName.Text, PatSex.Text, cardNo.Text, Mx.Text, zhzf.Text,
//                charge_total.Text, pay_charge_total.Text, idcard.Text, alipay_total.Text, trade_no.Text, stream_no.Text,
//                alipay_status.Text, alipay_tuihuan_status.Text, yb_status.Text, pay_status.Text, OperAtor.Text);
            PrintLpt print = new PrintLpt();
            string tmp1 = "";
            string strPrintData = "<?xml version='1.0' encoding='utf-8'?><print_info width='300' height='500'>";
            strPrintData += "<tr font='黑体' size='14' x='10' y='10' >北京海淀医院(自助)</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='40' >姓名：孙安成   性别：男    身份：  </tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='60' >就诊号：140626018700</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='80' >缴费日期：2016-10-08 12:02:35</tr>";
            strPrintData += "<tr font='黑体' size='12' x='100' y='110' >收 费 明 细 单</tr>";
            strPrintData += "<tr font='黑体' size='12' x='10' y='120' >---------------------------------------------</tr>";
            strPrintData += "<tr font='黑体' size='8' x='10' y='140' >类型 名称                     单位       单价 数量</tr>";
            strPrintData += "<tr font='黑体' size='7' x='10' y='160' >① 注射器(2ml)</tr>";
            strPrintData += "<tr font='黑体' size='7' x='170' y='160' ></tr>";
            strPrintData += "<tr font='黑体' size='7' x='245' y='160' >0.5000</tr>";
            strPrintData += "<tr font='黑体' size='7' x='283' y='160' >1</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='180' >自付金额：0.5 元</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='200' >医院减免金额：0 元</tr>";
            strPrintData += "<tr font='黑体' size='12' x='20' y='220' >总金额：0.5 元</tr>";
            strPrintData += "<tr font='黑体' size='8' x='20' y='240' >注：医保 标志 ①.无自付 ②.有自付 ③.全支付</tr>";
            strPrintData += "<tr font='黑体' size='12' x='10' y='250' >---------------------------------------------</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='270' >支付宝卡号：140626018700</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='290' >支付宝支付：0.5</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='310' >支付宝订单号：2016100821001004200279659752</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='330' >流水号：14062601870020161008HH0207</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='350' >支付状态：支付宝支付成功</tr>"; 
            strPrintData += "<tr font='黑体' size='10' x='20' y='370' >缴费状态：成功</tr>";
            strPrintData += "<tr font='黑体' size='11' x='10' y='390' >*温馨提示：请保存此凭证，请勿遗失</tr>";
            strPrintData += "<tr font='黑体' size='11' x='10' y='410' >*如需发票请去咨询台打印</tr>";
            strPrintData += "<tr font='黑体' size='10' x='20' y='440' >机器名称：ZZJ01</tr>";
            strPrintData += "</print_info>";
//           string strPrintData = "<?xml version='1.0' encoding='utf-8'?><print_info width='300' height='500'><tr font='黑体' size='14' x='10' y='10' >北京海淀医院(自助)</tr><tr font='黑体' size='10' x='20' y='40' >姓名：孙安成   性别：男    身份：  </tr><tr font='黑体' size='10' x='20' y='60' >就诊号：140626018700</tr><tr font='黑体' size='10' x='20' y='80' >缴费日期：2016-10-08 12:02:35</tr><tr font='黑体' size='12' x='100' y='110' >收 费 明 细 单</tr><tr font='黑体' size='12' x='10' y='120' >---------------------------------------------</tr><tr font='黑体' size='8' x='10' y='140' >类型 名称                     单位       单价 数量</tr><tr font='黑体' size='7' x='10' y='160' >① 注射器(2ml)</tr><tr font='黑体' size='7' x='170' y='160' > </tr><tr font='黑体' size='7' x='245' y='160' >0.5000</tr><tr font='黑体' size='7' x='283' y='160' >1</tr><tr font='黑体' size='10' x='20' y='180' >自付金额：0.5 元</tr><tr font='黑体' size='10' x='20' y='200' >医院减免金额：0 元</tr><tr font='黑体' size='12' x='20' y='220' >总金额：0.5 元</tr><tr font='黑体' size='8' x='20' y='240' >注：医保 标志 ①.无自付 ②.有自付 ③.全支付</tr><tr font='黑体' size='12' x='10' y='250' >---------------------------------------------</tr><tr font='黑体' size='10' x='20' y='270' >支付宝卡号：140626018700</tr><tr font='黑体' size='10' x='20' y='290' >支付宝支付：0.5</tr><tr font='黑体' size='10' x='20' y='310' >支付宝订单号：2016100821001004200279659752</tr><tr font='黑体' size='10' x='20' y='330' >流水号：14062601870020161008HH0207</tr><tr font='黑体' size='10' x='20' y='350' >支付状态：支付宝支付成功</tr><tr font='黑体' size='10' x='20' y='370' >缴费状态：成功</tr><tr font='黑体' size='11' x='10' y='390' >*温馨提示：请保存此凭证，请勿遗失</tr><tr font='黑体' size='11' x='10' y='410' >*如需发票请去咨询台打印</tr><tr font='黑体' size='10' x='20' y='440' >机器名称：ZZJ01</tr></print_info>";
//           strPrintData = strPrintData.Replace("<print_info width='300' height='400'>", "<print_info width='300' height='" + (400 + jz.Count * 28).ToString() + "'>");
            print.PrintDataJF(strPrintData, ref tmp1);
        }
    }
}
