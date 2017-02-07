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
    public partial class keybords : Form
    {
        String str = "";
        public keybords()
        {
            InitializeComponent();
        }

        private void keybord_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            this.textBox1.Text += btn.Text;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "120")
            {
                System.Environment.Exit(0);
                Application.Exit();
            }
            else {
                label1.Text = "密码错误";
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
