using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace D3ReadWrite
{
    public partial class Form1 : Form
    {
        #region 调用非接读卡器dll
        [DllImport("dcrf32.dll")]
        public static extern int dc_init(Int16 port, long baud);  //初试化
        [DllImport("dcrf32.dll")]
        public static extern short dc_exit(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_reset(int icdev, uint sec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_config_card(int icdev, byte cardtype);
        [DllImport("dcrf32.dll")]
        public static extern short dc_request(int icdev, char _Mode, ref uint TagType);
        [DllImport("dcrf32.dll")]
        public static extern short dc_card(int icdev, char _Mode, ref ulong Snr);
        [DllImport("dcrf32.dll")]
        public static extern short dc_halt(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_anticoll(int icdev, char _Bcnt, ref ulong IcCardNo);
        [DllImport("dcrf32.dll")]
        public static extern short dc_beep(int icdev, uint _Msec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_authentication(int icdev, int _Mode, int _SecNr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_load_key(int icdev, int mode, int secnr, [In] byte[] nkey);  //密码装载到读写模块中
        [DllImport("dcrf32.dll")]
        public static extern short dc_load_key_hex(int icdev, int mode, int secnr, string nkey);  //密码装载到读写模块中

        [DllImport("dcrf32.dll")]
        public static extern short dc_write(int icdev, int adr, [In] byte[] sdata);  //向卡中写入数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_write(int icdev, int adr, [In] string sdata);  //向卡中写入数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_hex(int icdev, int adr, [In] string sdata);  //向卡中写入数据(转换为16进制)

        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [Out] byte[] sdata);

        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_read_hex(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据(转换为16进制)
        [DllImport("dcrf32.dll")]
        public static extern int a_hex(string oldValue, ref string newValue, Int16 len);  //普通字符转换成十六进制字符
        [DllImport("dcrf32.dll")]
        public static extern void hex_a(ref string oldValue, ref string newValue, int len);  //十六进制字符转换成普通字符

        private static int _icdev = -1;

        public static int IcDev
        {
            get { return _icdev; }
            set { _icdev = value; }
        }

        public static void InitIc()
        {
            //初始化串口1
            if (_icdev < 0)
            {
                _icdev = dc_init(100, 115200);
            }

            if (_icdev <= 0)
            {
                //                 Log.Info("Init Com Error!"+_icdev);
            }
            else
            {
                string a = "ffffffffffff";
            }
        }


        //关闭串口
        public void ExitIc()
        {
            dc_exit((Int16)_icdev);
        }

        //读卡
        public string ReadIc()
        {
            ulong icCardNo = 0;
            char str = (char)0;
            dc_card((Int16)_icdev, str, ref icCardNo);
            return icCardNo.ToString();
        }

        //器蜂鸣
        public static int Beep(int IcDev)
        {
            int st;
            st = dc_beep((Int16)IcDev, 50);
            return st;
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
        }
        int st;
        private void button1_Click(object sender, EventArgs e)
        {
            InitIc();
            label1.Text = _icdev.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string CardNo = "";
            try
            {
                ulong icCardNo = 0;
                char tt = (char) 0;
                uint ss = 0;
                st = dc_reset(_icdev, ss);
                Thread.Sleep(500);
                st = dc_card(_icdev, tt, ref icCardNo);
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
            label1.Text = CardNo;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i = dc_load_key(_icdev, Convert.ToInt32(comboBox1.Text), Convert.ToInt32(comboBox3.Text), Encoding.Default.GetBytes(comboBox2.Text));
            label1.Text = i.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StringBuilder data = new StringBuilder();
            dc_read(_icdev, Convert.ToInt32(comboBox4.Text), data);
            label1.Text = data.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int i = dc_write(_icdev, 0, "123456789");
            if (i == 0)
            {
                StringBuilder data = new StringBuilder();
                dc_read(_icdev, 0, data);
                label1.Text = "写入OK" + data;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int i = dc_authentication(_icdev, Convert.ToInt32(comboBox1.Text), Convert.ToInt32(comboBox3.Text));
            label1.Text = i.ToString();
        }
    }
}
