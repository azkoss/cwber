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
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            //没有标题
            this.FormBorderStyle = FormBorderStyle.None;
            //任务栏不显示
            this.ShowInTaskbar = false;
            this.Height = int.Parse(WinFormDemo.Properties.Settings.Default.height); ;
            this.Width = int.Parse(WinFormDemo.Properties.Settings.Default.width);
            this.webBrowser1.Navigate(WinFormDemo.Properties.Settings.Default.url);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            keybords kb = new keybords();
            kb.Show();
        }
    }
}
