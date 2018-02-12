using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace D8_ULtralight
{
    public partial class Form1 : Form
    {
#region 
        [DllImport("dcrf32.dll")]
        public static extern int dc_init(Int16 port, Int32 baud);  //初试化
        [DllImport("dcrf32.dll")]
        public static extern short dc_exit(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_beep(int icdev, uint _Msec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_card_double_hex(int icdev, char _Mode, [MarshalAs(UnmanagedType.LPStr)] StringBuilder Snr);  //从卡中读数据(转换为16进制)

        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [Out] byte[] sdata);  //从卡中读数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据

        [DllImport("dcrf32.dll")]
        public static extern short dc_read_hex(int icdev, int adr, ref byte sdata);  //从卡中读数据(转换为16进制)
        [DllImport("dcrf32.dll")]
        public static extern short dc_read_hex(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数

        [DllImport("dcrf32.dll")]     
        public static extern short dc_write(int icdev, int adr, [In] string sdata);  //向卡中写入数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_hex(int icdev, int adr, [In] string sdata);  //向卡中写入数据(转换为16进制)
#endregion
        private int icdev=-1;
        private short st=-1;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
             if (icdev >0 )
             {
                 st = dc_exit(icdev);
                 icdev = -1;
             }
             icdev = dc_init(100, 115200);
            if (icdev<0)
            {
                MessageBox.Show("打开端口失败","提示");
                return;
            }
            st = dc_beep(icdev, 10);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (icdev>0)
            {
                st = dc_exit(icdev);
                icdev = -1;
            }
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StringBuilder strsnr=new StringBuilder(20);
            st = dc_card_double_hex(icdev, '0', strsnr);
            if (st!=0)
            {
                MessageBox.Show("寻卡失败","提示");
                return;
            }
            this.textBox1.Text = Convert.ToString(strsnr);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            short addr = 0;
            StringBuilder rbuff = new StringBuilder(32);
            addr = Convert.ToInt16(this.textBox2.Text);

            st = dc_read_hex(icdev, addr, rbuff);
            if (st!=0)
            {
                MessageBox.Show("读卡失败","提示");
                return;
            }
            string ss1 = rbuff.ToString();
            this.textBox3.Text = ss1.Substring(1,8);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            short addr = 0;
           // string wbuffer = String.Format("{0,-32}",textBox5.Text);
            string wbuffer = textBox5.Text;
            wbuffer = wbuffer.PadRight(32, '0');

            addr = Convert.ToInt16(this.textBox4.Text);

            st = dc_write_hex(icdev, addr, wbuffer);
            if (st != 0)
            {
                MessageBox.Show("写卡失败", "提示");
                return;
            }
            MessageBox.Show("写卡成功", "提示");
        }
    }
}