using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Xml;

namespace WinFormDemo
{
    class PrintLpt1
    {
        String xmlInfo = "";
        //打印
        public void PrintData(String info, ref String errInfo)
        {
            this.xmlInfo = info;
            PrintDocument pd = new PrintDocument { PrintController = new StandardPrintController() };
            Margins margin = new Margins(20, 20, 20, 20);
            pd.DefaultPageSettings.Margins = margin;
            pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
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
        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.xmlInfo);
            XmlNode node = doc.SelectSingleNode("//print_info");
//            XmlNode node1 = doc.SelectSingleNode("//print_tiaoma");
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
                    Pen p = new Pen(Color.Blue, 2);
                   // g.DrawRectangle(p, 2, 2, width, pos + 30);
                }
            }
            /*if (node1 != null)
            {
                XmlNodeList itmes = node.ChildNodes;
                foreach (XmlNode xn in itmes)
                {
                    if (xn == null)
                        continue;
                    if (xn.Attributes != null)
                    {
                        String value = xn.ChildNodes[0].InnerText;
                        BarCodeClass barcode = new BarCodeClass();
                        Bitmap bitmap = barcode.CreateBarCode(value);
                        Point ulCorner = new Point(100, 100);
                        Point urCorner = new Point(550, 100);
                        Point llCorner = new Point(150, 250);
                        Point[] destPara = { ulCorner, urCorner, llCorner };

                        // Draw image to screen.
                        e.Graphics.DrawImage(bitmap, destPara);
//                        g.DrawPath(bitmap);
                    }
                }
            }*/
            g.Dispose();
        }
    }
}
