using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Xml;
using BarcodeLib;

namespace WinFormDemo
{
    class PrintLpt
    {
        String xmlInfo = "";
        String s = "";
        private int width;
        private int height;
        private int x;
        private int y;
        


        public string S
        {
            get { return this.s; }
            set { this.s = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        //打印(挂号)
        public void PrintDataGH(String info,ref String errInfo)
        {
            this.xmlInfo = info;
            PrintDocument pd = new PrintDocument { PrintController = new StandardPrintController() };
            pd.PrinterSettings.PrinterName = Properties.Settings.Default.printerName;
            Margins margin = new Margins(20, 20, 20, 20);
            pd.DefaultPageSettings.Margins = margin;
            pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPageGH);
            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                errInfo = ex.Message;
                pd.PrintController.OnEndPrint(pd, new PrintEventArgs());
            }
        }
        //设置打印机开始打印事件处理函数(挂号)
        private void pd_PrintPageGH(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.xmlInfo);
            XmlNode node = doc.SelectSingleNode("//print_info");
            if (node != null)
            {
                int pos = 0;
                XmlNodeList itmes = node.ChildNodes;
                foreach (XmlNode xn in itmes)
                {
                    if (xn == null)
                        continue;
                    if (xn.Attributes != null)
                    {
                        String font = xn.Attributes["font"].Value;
                        int size = Int32.Parse("0" + xn.Attributes["size"].Value);
                        int x = Int32.Parse("0" + xn.Attributes["x"].Value);
                        int y = Int32.Parse("0" + xn.Attributes["y"].Value);
                        String value = xn.ChildNodes[0].InnerText;
                        pos = y;
                        Font f = new Font(font, size);
                        Brush b = new SolidBrush(Color.Black);
                        g.DrawString(value, f, b, x, y);
                    }
                }
                if (S != "")
                {
                    int width1 = Width;
                    int height1 = Height;

                    TYPE type;
                    type = TYPE.CODE128;
                    var code = S;
                    Image image;
                    GetBarcode(width1, height1, type, code, out image);
                    g.DrawImage(image,X,Y);
                }
                 
                if (node.Attributes != null)
                {
                    int width = Int32.Parse("0" + node.Attributes["width"].Value);
//                    Pen p = new Pen(Color.Blue, 2);
                    Pen p = new Pen(Color.White, 2);
                    g.DrawRectangle(p, 2, 2, width, pos + 30);
                }
            }
            g.Dispose();
        }


        public void PrintDataJF(String info, ref String errInfo)
        {
            this.xmlInfo = info;
            PrintDocument pd = new PrintDocument { PrintController = new StandardPrintController() };

            Margins margin = new Margins(20, 20, 20, 20);
            pd.DefaultPageSettings.Margins = margin;
            pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPageJF);
            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                errInfo = ex.Message;
                pd.PrintController.OnEndPrint(pd, new PrintEventArgs());
            }
        }
        //设置打印机开始打印事件处理函数
        private void pd_PrintPageJF(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.xmlInfo);
            XmlNode node = doc.SelectSingleNode("//print_info");
            if (node != null)
            {
                int pos = 0;
                XmlNodeList itmes = node.ChildNodes;
                foreach (XmlNode xn in itmes)
                {
                    if (xn == null)
                        continue;
                    if (xn.Attributes != null)
                    {
                        String font = xn.Attributes["font"].Value;
                        int size = Int32.Parse("0" + xn.Attributes["size"].Value);
                        int x = Int32.Parse("0" + xn.Attributes["x"].Value);
                        int y = Int32.Parse("0" + xn.Attributes["y"].Value);
                        String value = xn.ChildNodes[0].InnerText;
                        pos = y;
                        Font f = new Font(font, size);
                        Brush b = new SolidBrush(Color.Black);
                        g.DrawString(value, f, b, x, y);
                    }
                }
                if (node.Attributes != null)
                {
                    int width = Int32.Parse("0" + node.Attributes["width"].Value);
//                    Pen p = new Pen(Color.Blue, 2);
                    Pen p = new Pen(Color.White, 2);
                    g.DrawRectangle(p, 2, 2, width, pos + 30);
                }
            }
            g.Dispose();
        }

        #region 生成条形码
        /// <summary>  
        /// 生成条形码  
        /// </summary>  
        static byte[] GetBarcode(int width, int height, TYPE type, string code, out Image image)
        {
            Barcode b = new Barcode
            {
                BackColor = Color.White, //图片背景颜色  
                ForeColor = Color.Black, //条码颜色  
                IncludeLabel = true,
                Alignment = AlignmentPositions.CENTER,
                LabelPosition = LabelPositions.BOTTOMCENTER,
                ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg, //图片格式  
                LabelFont = new Font("verdana", 10f), //字体设置  
                Height = height, //图片高度  
                Width = width //图片宽度  
            };

            image = b.Encode(type, code);//生成图片  
            byte[] buffer = b.GetImageData(SaveTypes.GIF);//转换byte格式  
            return buffer;
        }
        #endregion  
    }
}
