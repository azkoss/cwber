using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace K100
{
    public partial class Form1 : Form
    {
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


        int hCom = 0;
        byte[] Info = new byte[500];
        byte Mac_Addr = 0x00;

        public Form1()
        {
            InitializeComponent();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("读卡器无公安部解密模块，无法读取身份证号", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strport = comboBox1.Text;
            int baudate = int.Parse(comboBox2.Text);
            byte[] _Version=new byte[20];
            int re = 0, i;
            hCom = M100A_CommOpenWithBaud(strport, baudate);
            if (hCom > 0)
            {
                if (checkBox1.Checked)
                {
                    for (i = 0; i < 16; i++)
                    {
                        re = M100A_AutoTestMachine(hCom, true, (byte)i, _Version, Info);
                        if (re == 0)
                        {
                            Mac_Addr = (byte)i;
                            break;
                        }
                    }
                    if (re == 0)
                        MessageBox.Show("设备连接成功");
                    else
                        MessageBox.Show("设备连接失败");
                }
                else
                {
                    re = M100A_Reset(hCom, false, 0, 0x34, _Version, Info);
                    if (re == 0)
                        MessageBox.Show("设备连接成功");
                    else
                        MessageBox.Show("设备连接失败");
                }
            }
            else
                MessageBox.Show("串口打开失败");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int re = M100A_CommClose(hCom);
            if (re == 0)
                MessageBox.Show("设备断开成功");
            else
                MessageBox.Show("设备断开失败");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int re = 0;
            if (checkBox1.Checked)
                re = M100A_EnterCard(hCom, true, Mac_Addr, 0x31, Info);
            else
                re = M100A_EnterCard(hCom, false, 0, 0x31, Info);
            if(re==0)
            {
                MessageBox.Show("进卡操作执行成功");
            }
            else
                MessageBox.Show("进卡操作执行失败");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int re = 0;
            if (checkBox1.Checked)
                re = M100A_MoveCard(hCom, true, Mac_Addr, 0x31, Info);
            else
                re = M100A_MoveCard(hCom, false, 0, 0x31, Info);
            if (re == 0)
            {
                MessageBox.Show("移动卡片操作执行成功");
            }
            else
                MessageBox.Show("移动卡片操作执行失败");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int re = 0;
            if (checkBox1.Checked)
                re = M100A_MoveCard(hCom, true, Mac_Addr, 0x30, Info);
            else
                re = M100A_MoveCard(hCom, false, 0, 0x30, Info);
            if (re == 0)
            {
                MessageBox.Show("移动卡片操作执行成功");
            }
            else
                MessageBox.Show("移动卡片操作执行失败");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int re = 0;
            if (checkBox1.Checked)
                re = M100A_S50DetectCard(hCom, true, Mac_Addr, Info);
            else
                re = M100A_S50DetectCard(hCom, false, 0, Info);
            if (re == 0)
            {
                MessageBox.Show("寻卡成功");
            }
            else
                MessageBox.Show("寻卡失败");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int[] length = new int[2];
            byte[] cardid = new byte[30];
            int re = 0, i = 0;
            string str="";
            if (checkBox1.Checked)
            {
                re = M100A_ReadICCardNum(hCom, true, Mac_Addr, length, cardid);
            }
            else
                re = M100A_ReadICCardNum(hCom, false, 0, length, cardid);
            if (re == 0)
            {
                if (length[0] % 2 == 0)
                {
                    for (i = 0; i < length[0] / 2; i++)
                    {
                        if (i != 0 && i % 2 == 0)
                            str += " ";
                        str += cardid[i].ToString("X2");
                    }
                }
                else
                {
                    for (i = 0; i < length[0] / 2; i++)
                    {
                        if (i != 0 && i % 2 == 0)
                            str += " ";
                        str += cardid[i].ToString("X2");
                    }
                    str += ((cardid[i] & 0xF0) / 16).ToString("X1");
                }
                MessageBox.Show("IC卡卡号为：" + str);
            }
            else
                MessageBox.Show("获取IC卡卡号失败，" + re);

        }

        private void button9_Click(object sender, EventArgs e)
        {
            int re = 0;
            if (checkBox1.Checked)
                re = M100A_MoveCard(hCom, true, Mac_Addr, 0x33, Info);
            else
                re = M100A_MoveCard(hCom, false, 0, 0x33, Info);
            if (re == 0)
            {
                MessageBox.Show("弹卡成功");
            }
            else
                MessageBox.Show("弹卡失败");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int re = 0;
            byte[] cardid = new byte[10];
            string str = "";
            if (checkBox1.Checked)
                re = M100A_S50GetCardID(hCom, true, Mac_Addr, cardid, Info);
            else
                re = M100A_S50GetCardID(hCom, false, 0, cardid, Info);
            if (re == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    str += cardid[i].ToString("X2");
                }
                MessageBox.Show(str);
            }
            else
                MessageBox.Show("获取卡号失败");
        }
    }
}
