using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace D1000
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_CommOpenWithBaud(int Port, int _data);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_CommClose(int ComHandle);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_SendCmd(int ComHandle, byte MacAddr, string p_Cmd, int CmdLen);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_Query(int ComHandle, byte MacAddr, byte[] StateInfo);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_SensorQuery(int ComHandle, byte MacAddr, byte[] StateInfo);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_AutoTestMac(int ComHandle, byte MacAddr);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_ReadRecycleCardNum(int ComHandle, byte MacAddr, byte[] szData);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_SetCardNum(int ComHandle, byte MacAddr, int nNum);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_DisEnableCount(int ComHandle, byte MacAddr);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_EnableCount(int ComHandle, byte MacAddr);

        [DllImport("D1000_DLL.dll")]
        public static extern int D1000_ReadCardNumber(int ComHandle, byte MacAddr, byte[] CardNumber);

        int hCom = 0;
        byte[] Info = new byte[500];
        byte Mac_Addr = 0x00;

        private void button1_Click(object sender, EventArgs e)
        {
            int strport = Convert.ToInt32(comboBox1.Text);
            int baudate = int.Parse(comboBox2.Text);
            byte[] _Version = new byte[20];
            int re = 0, i;
            hCom = D1000_CommOpenWithBaud(strport, baudate);
            if (hCom > 0)
            {
                MessageBox.Show("串口打开成功");
            }
            else
                MessageBox.Show("串口打开失败");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (D1000_CommClose(hCom) == 0)
            {
                MessageBox.Show("串口关闭成功");
            }
            else
            {
                MessageBox.Show("串口关闭失败");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (D1000_SendCmd(hCom, Mac_Addr, "FC7", 3) == 0)
            {
                MessageBox.Show("走卡到读卡位成功");
            }
            else
            {
                MessageBox.Show("走卡到读卡位失败");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            byte[] cardNumber = new byte[50];
            if (D1000_ReadCardNumber(hCom, Mac_Addr, cardNumber) == 0)
            {
                MessageBox.Show("卡号读取成功：" + cardNumber.ToString());
            }
            else
            {
                MessageBox.Show("卡号读取失败：");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (D1000_SendCmd(hCom, Mac_Addr, "FC0", 3) == 0)
            {
                MessageBox.Show("走卡到取卡位成功");
            }
            else
            {
                MessageBox.Show("走卡到取卡位失败");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (D1000_SendCmd(hCom, Mac_Addr, "CP", 2) == 0)
            {
                MessageBox.Show("回收卡成功");
            }
            else
            {
                MessageBox.Show("回收卡失败");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (D1000_SendCmd(hCom, Mac_Addr, "LT1", 3) == 0)
            {
                MessageBox.Show("发送亮灯指令成功");
            }
            else
            {
                MessageBox.Show("发送亮灯指令失败");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (D1000_SendCmd(hCom, Mac_Addr, "LT2", 3) == 0)
            {
                MessageBox.Show("发送灭灯指令成功");
            }
            else
            {
                MessageBox.Show("发送灭灯指令失败");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (D1000_SendCmd(hCom, Mac_Addr, "LT0", 3) == 0)
            {
                MessageBox.Show("发送灭灯指令成功");
            }
            else
            {
                MessageBox.Show("发送灭灯指令失败");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            byte[] info = new byte[3];
            if (D1000_Query(hCom, Mac_Addr, info) == 0)
            {
                MessageBox.Show("状态查询成功：" + info);
            }
            else
            {
                MessageBox.Show("状态查询失败：");
            }
        }
    }
}
