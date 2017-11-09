using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Ink;

namespace WinFormDemo
{
    /// <summary>
    /// 手写输入，net4.0抛异常，2.0不会
    /// </summary>
    public partial class HandWrite : Form
    {
        InkCollector ic;
        RecognizerContext rct;
        // Recognizer rc;
        string FullCACText;
        private string writeString { get; set; }
        public delegate void TransfDelegate(String value);
        public event TransfDelegate TransfEvent; 

        public HandWrite()
        {
            InitializeComponent();
        }

        public HandWrite(string writing)
        {
            InitializeComponent();
            writeString = writing;
        }

        private void HandWrite_Load(object sender, EventArgs e)
        {
            //没有标题
            this.FormBorderStyle = FormBorderStyle.None;
            //任务栏不显示
            this.ShowInTaskbar = false;
            pictureBox1.Width = Width;
            pictureBox1.Height = Height;
            textBox1.Width = Width - 500;
            textBox1.Location = new Point(30, Height- 150);
            button1.Location = new Point(Width - 450, Height -150);
            button2.Location = new Point(Width - 250, Height -150);
            button3.Location = new Point(Width - 350, Height - 150);
            ic = new InkCollector(pictureBox1.Handle);
            this.ic.Stroke += new InkCollectorStrokeEventHandler(ic_Stroke);

            ic.Enabled = true;
            ink_();

            //   this.ic.Stroke += new InkCollectorStrokeEventHandler(ic_Stroke);
            this.rct.RecognitionWithAlternates += new RecognizerContextRecognitionWithAlternatesEventHandler(rct_RecognitionWithAlternates);

            rct.Strokes = ic.Ink.Strokes;
        }


        void rct_RecognitionWithAlternates(object sender, RecognizerContextRecognitionWithAlternatesEventArgs e)
        {

            string ResultString = "";
            RecognitionAlternates alts;

            if (e.RecognitionStatus == RecognitionStatus.NoError)
            {
                alts = e.Result.GetAlternatesFromSelection();

                foreach (RecognitionAlternate alt in alts)
                {
                    ResultString = ResultString + alt.ToString() + " ";
                }
            }
            FullCACText = ResultString.Trim();
            Control.CheckForIllegalCrossThreadCalls = false;
            textBox1.Text = FullCACText;
            returnString(FullCACText);
            Control.CheckForIllegalCrossThreadCalls = true;

        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        private void returnString(string str)
        {
            string[] str_temp = str.Split(' ');
            string str_temp1 = "shibie_";
            string str_temp2 = "";
            if (str_temp.Length > 0)
            {
                for (int i = 0; i < str_temp.Length; i++)
                {
                    str_temp2 = str_temp1 + i.ToString();
                    Control[] con_temp = Controls.Find(str_temp2, true);
                    if (con_temp.Length > 0)
                    {
                        ((Button)(con_temp[0])).Text = str_temp[i];
                    }
                }
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ic_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            rct.StopBackgroundRecognition();
            rct.Strokes.Add(e.Stroke);
            rct.CharacterAutoCompletion = RecognizerCharacterAutoCompletionMode.Full;
            rct.BackgroundRecognizeWithAlternates(0);
        }
        /// <summary>
        ///
        /// </summary>
        private void ink_()
        {
            Recognizers recos = new Recognizers();
            Recognizer chineseReco = recos.GetDefaultRecognizer();

            rct = chineseReco.CreateRecognizerContext();
        }

        /// <summary>
        /// 重写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!ic.CollectingInk)
            {
                Strokes strokesToDelete = ic.Ink.Strokes;
                rct.StopBackgroundRecognition();
                ic.Ink.DeleteStrokes(strokesToDelete);
                rct.Strokes = ic.Ink.Strokes;
                ic.Ink.DeleteStrokes();//清除手写区域笔画;
                pictureBox1.Refresh();//刷新手写区域
            }

        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            TransfEvent(textBox1.Text);
            this.Close();
        }

        /// <summary>
        /// 识别手写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";

            textBox1.SelectedText = ic.Ink.Strokes.ToString();
        }

    }
}
