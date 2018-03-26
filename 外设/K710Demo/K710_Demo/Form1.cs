using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace K710_Demo
{
    public partial class Form1 : Form
    {
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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string str = "";
            int nRet;

            for (int i = 1; i < 50; i++)
            {
                str = "COM" + i;
                ComHandle = D1000_CommOpen(i);
                if (ComHandle > 0)
                {
                    D1000_CommClose(ComHandle);
                    comboBox1.Items.Add(str);
                }
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            comboBox2.Items.Add("1200");
            comboBox2.Items.Add("2400");
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("38400");
            comboBox2.Items.Add("57600");
            comboBox2.Items.Add("115200");
            comboBox2.SelectedIndex = 3;
        }

        private string HexToString(byte[] by)
        {
            string str = "";

            for (int i = 0; by[i] != '\0'; i++)
            {
                char ch;
                ch = (char) by[i];
                str += ch;
            }
            return str;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte) 'D';
            sendcmd[1] = (byte) 'C';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 2);
            if (nRet == 0)
            {
                textBox1.Text = "发卡到取卡位置成功";
            }
            else
                textBox1.Text = "发卡到取卡位置失败";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int nRet = 1;
            byte[] Recv = new byte[200];

            ComHandle = D1000_CommOpenWithBaud(int.Parse(comboBox1.Text.Substring(3, 1)), int.Parse(comboBox2.Text));
            if (ComHandle > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    nRet = D1000_AutoTestMac(ComHandle, (byte) i);
                    if (nRet == 0)
                    {
                        MacAddress = (byte) i;
                        break;
                    }
                }

                if (nRet == 0)
                {
                    textBox1.Text = "设备连接成功";
                    button1.Enabled = false;
                    button2.Enabled = true;
                }
                else
                {
                    textBox1.Text = "设备连接失败";
                }
            }
            else
            {
                textBox1.Text = "串口打开失败";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int nRet = D1000_CommClose(ComHandle);
            if (nRet == 0)
            {
                button1.Enabled = true;
                button2.Enabled = false;
                textBox1.Text = "设备断开成功";
            }
            else
                textBox1.Text = "设备断开失败";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte) 'C';
            sendcmd[1] = (byte) 'P';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 2);
            if (nRet == 0)
            {
                textBox1.Text = "回收成功";
            }
            else
                textBox1.Text = "回收失败";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte) 'R';
            sendcmd[1] = (byte) 'S';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 2);
            if (nRet == 0)
            {
                textBox1.Text = "复位成功";
            }
            else
                textBox1.Text = "复位失败";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte) 'B';
            sendcmd[1] = (byte) 'E';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 2);
            if (nRet == 0)
            {
                textBox1.Text = "允许蜂鸣设置成功";
            }
            else
                textBox1.Text = "允许蜂鸣设置失败";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte) 'B';
            sendcmd[1] = (byte) 'D';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 2);
            if (nRet == 0)
            {
                textBox1.Text = "禁止蜂鸣设置成功";
            }
            else
                textBox1.Text = "禁止蜂鸣设置失败";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte) 'F';
            sendcmd[1] = (byte) 'C';
            sendcmd[2] = (byte) '6';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 3);
            if (nRet == 0)
            {
                textBox1.Text = "发卡到传感器2位置成功";
            }
            else
                textBox1.Text = "发卡到传感器2位置失败";
        }

        private void button14_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] _Version = new byte[50];

            nRet = D1000_GetSysVersion(ComHandle, MacAddress, _Version);
            if (nRet == 0)
            {
                MessageBox.Show(HexToString(_Version), "版本信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Text = "获取版本号成功";
            }
            else
                textBox1.Text = "获取版本号失败";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte) 'F';
            sendcmd[1] = (byte) 'C';
            sendcmd[2] = (byte) '7';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 3);
            if (nRet == 0)
            {
                textBox1.Text = "发卡到读卡位置成功";
            }
            else
                textBox1.Text = "发卡到读卡位置失败";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte) 'F';
            sendcmd[1] = (byte) 'C';
            sendcmd[2] = (byte) '4';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 3);
            if (nRet == 0)
            {
                textBox1.Text = "发卡到取卡位置成功";
            }
            else
                textBox1.Text = "发卡到取卡位置失败";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            int nRet;
            byte[] sendcmd = new byte[3];

            sendcmd[0] = (byte) 'F';
            sendcmd[1] = (byte) 'C';
            sendcmd[2] = (byte) '0';

            nRet = D1000_SendCmd(ComHandle, MacAddress, sendcmd, 3);
            if (nRet == 0)
            {
                textBox1.Text = "发卡到卡口外成功";
            }
            else
                textBox1.Text = "发卡到卡口外失败";
        }

        private void button15_Click(object sender, EventArgs e)
        {
            byte[] cardNo = new byte[5];
            string result = "";
            if (D1000_ReadCardNumber(ComHandle, MacAddress, cardNo) == 0)
            {
                foreach (byte b in cardNo)
                {
                    result += b.ToString("x2");
                }
                textBox1.Text = "读卡成功:" + result;
            }
            else
            {
                textBox1.Text = "读卡失败";
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            string result ="";
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
            textBox1.Text = result;
        }
    }
}

