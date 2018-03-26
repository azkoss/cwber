using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormDemo
{
    public partial class base64Image : Form
    {
        public base64Image()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
//            richTextBox1.Text = ImgToBase64String("D:\\1.jpg");
            pictureBox1.Image = Base64StringToImage(richTextBox1.Text);
        }

        public static Image showImage(byte[] picData, int picWidth, int picHeight, int kv)
        {
            Bitmap btS = new Bitmap(new MemoryStream(picData));
            //去掉点边
            picWidth = picWidth - 4;
            picHeight = picHeight - 4;
            //这里只实现页宽自适应
            //double ka = btS.Width / btS.Height;
            //double kb = picWidth / picHeight;
            if ((double)btS.Width / btS.Height < (double)picWidth / picHeight)//出滚动条
                //if (ka<kb)
            {
                picWidth = picWidth - 25;
            }
            picHeight = picWidth * btS.Height / btS.Width;
            picWidth = (int)(picWidth * kv / 100.0);
            picHeight = (int)(picHeight * kv / 100.0);
            Bitmap btD = new Bitmap(picWidth, picHeight);
            Graphics gr = Graphics.FromImage(btD);
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.DrawImage(btS, 0, 0, picWidth, picHeight);
            gr.Dispose();
            btS.Dispose();
            return btD;
        }

        protected string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }  

        protected Bitmap Base64StringToImage(string strbase64)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(strbase64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);

//                bmp.Save(@"d:\test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp.Save(@"d:\"test.bmp", ImageFormat.Bmp);  
                //bmp.Save(@"d:\"test.gif", ImageFormat.Gif);  
                //bmp.Save(@"d:\"test.png", ImageFormat.Png);  
                ms.Close();
                return bmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }  
    }
}
