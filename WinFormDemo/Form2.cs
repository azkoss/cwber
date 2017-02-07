using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Sashulin;
using Sashulin.common;
using WinFormDemo.CommUtil;


namespace WinFormDemo
{
    /// <summary> 
    /// 在客户端HTML页面调用WebBrowser方法类 
    /// </summary> 
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class Form2 : Form
    {
        public string uid;
        public string room_id;
        ChromeWebBrowser browser = new ChromeWebBrowser();
        //获取当前鼠标下可视化控件的函数
        [DllImport("user32.dll")]
        public static extern int WindowFromPoint(int xPoint, int yPoint);
        //获取指定句柄的父级函数
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        //获取屏幕的大小
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        private static extern int GetSystemMetrics(int mVal);

        private IntPtr CurrentHandle;//记录鼠标当前状态下控件的句柄
        private int WindowFlag;//标记是否对窗体进行拉伸操作
        private int intOriHeight;
        int ti = 0; //用来控制ico图片索引 
        private Icon ico1 = new Icon(@".\images/01.ico");
        private Icon ico2 = new Icon(@".\images/02.ico");//透明的图标
        public Form2()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            this.TopMost = true;
            CheckForIllegalCrossThreadCalls = false; 

        }
        private void Form2_Load(object sender, EventArgs e)
        {
            this.Height = int.Parse(WinFormDemo.Properties.Settings.Default.height); ;
            this.Width = int.Parse(WinFormDemo.Properties.Settings.Default.width);
            this.timer1.Interval = 1000;
            this.timer1.Start();
            OpenUrl(Properties.Settings.Default.url);

            intOriHeight = this.Height;
            ChangePosition(Screen.PrimaryScreen.Bounds.Width - 966, 50);
            JudgeWinMouPosition.Enabled = true;         //计时器JudgeWinMouPosition开始工作
            this.notifyIcon1.Icon = ico1;
            this.timer3.Enabled = false;
        }
        public void ShowMessage(string msg)
        {
            Shown sh = new Shown();
           
            sh.label1.Text = msg;
            sh.ShowDialog(this);
  
          
        }

        delegate void NewPageListener(string url, object request);
        public void OpenUrl(string url)
        {
            NewPageListener a = new NewPageListener(NewPage);
            this.Invoke(a, new object[] { url, null });
        }

        public void NewPage(string newUrl, object req)
        {
          
            CSharpBrowserSettings settings = new CSharpBrowserSettings();
            settings.CachePath = @"C:\temp\caches";
            
           
            browser.Initialize(settings);
            this.panel1.Controls.Add(browser);
            browser.Validate();
            browser.Dock = DockStyle.Fill;
            if (!newUrl.Contains("&"))
                browser.OpenUrl(newUrl);
            else
                browser.OpenUrl(req);
            
        }
        public int OriHeight
        {
            get { return intOriHeight; }
        }

        public IntPtr MouseNowPosition(int x, int y)
        {
            IntPtr OriginalHandle;                              //声明保存原始句柄的变量
            OriginalHandle = ((IntPtr)WindowFromPoint(x, y));   //获取包含鼠标原始位置的窗口的句柄
            CurrentHandle = OriginalHandle;                     //设置当前句柄
            while (OriginalHandle != ((IntPtr)0))               //循环判断鼠标是否移动
            {
                CurrentHandle = OriginalHandle;                 //记录当前的句柄
                OriginalHandle = GetParent(CurrentHandle);      //更新原始句柄
            }
            return CurrentHandle;                               //返回当前的句柄
        }

        private void JudgeWinMouPosition_Tick(object sender, EventArgs e)
        {

            if (this.Top < 3)                       //当本窗体距屏幕的上边距小于3px时
            {
                if (this.Handle == MouseNowPosition(Cursor.Position.X, Cursor.Position.Y))//当鼠标在该窗体上时
                {
                    WindowFlag = 1;                //设定当前的窗体状态
                    HideWindow.Enabled = false;     //设定计时器HideWindow为不可用状态
                    this.Top = 0;                 //设定窗体上边缘与容器工作区上边缘之间的距离

                }
                else                              //当鼠标没在窗体上时
                {
                    WindowFlag = 1;                //设定当前的窗体状态
                    HideWindow.Enabled = true;      //启动计时器HideWindow
                }
            }                                     //当本窗体距屏幕的上边距大于3px时
            else
            {
                //当本窗体在屏幕的最左端或者最右端、最下端时
                if (this.Left < 3 || (this.Left + this.Width) > (GetSystemMetrics(0) - 3) || (this.Top + this.Height) > (Screen.AllScreens[0].Bounds.Height - 3))
                {
                    if (this.Left < 3)              //当窗体处于屏幕左侧时
                    {
                        if (this.Handle == MouseNowPosition(Cursor.Position.X, Cursor.Position.Y))    //当鼠标在该窗体上时
                        {
                            this.Height = Screen.AllScreens[0].Bounds.Height - 20;
                            this.Top = 3;
                            WindowFlag = 2;        //设定当前的窗体状态
                            HideWindow.Enabled = false;//设定计时器HideWindow为不可用状态
                            this.Left = 0;         //设定窗体的左边缘与容器工作区的左边缘之间的距离
                        }
                        else                      //当鼠标没在该窗体上时
                        {
                            WindowFlag = 2;        //设定当前的窗体状态
                            HideWindow.Enabled = true;//设定计时器HideWindow为可用状态

                        }
                    }
                    if ((this.Left + this.Width) > (GetSystemMetrics(0) - 3)) //当窗体处于屏幕的最右侧时
                    {
                        if (this.Handle == MouseNowPosition(Cursor.Position.X, Cursor.Position.Y))//当鼠标处于窗体上时
                        {
                            this.Height = Screen.AllScreens[0].Bounds.Height - 20;
                            this.Top = 3;
                            WindowFlag = 3;        //设定当前的窗体状态
                            HideWindow.Enabled = false; //设定计时器HideWindow为不可用状态
                            this.Left = GetSystemMetrics(0) - this.Width;//设定该窗体与容器工作区左边缘之间的距离
                        }
                        else                          //当鼠标离开窗体时
                        {
                            WindowFlag = 3;            //设定当前的窗体状态
                            HideWindow.Enabled = true;  //设定计时器HideWindow为可用状态
                        }
                    }

                    //当窗体距屏幕最下端的距离小于3px时
                    if ((this.Top + this.Height) > (Screen.AllScreens[0].Bounds.Height - 3))
                    {
                        if (this.Handle == MouseNowPosition(Cursor.Position.X, Cursor.Position.Y)) //当鼠标在该窗体上时
                        {
                            WindowFlag = 4;           //设定当前的窗体状态
                            HideWindow.Enabled = false;//设定计时器HideWindow为不可用状态
                            this.Top = Screen.AllScreens[0].Bounds.Height - this.Height;//设定该窗体与容器工作区上边缘之间的距离
                        }
                        else
                        {
                            if ((this.Left > this.Width + 3) && (GetSystemMetrics(0) - this.Right) > 3)
                            {
                                WindowFlag = 4;           //设定当前的窗体状态
                                HideWindow.Enabled = true; //设定计时器HideWindow为可用状态
                            }
                        }
                    }
                }
            }
        }

        private void HideWindow_Tick(object sender, EventArgs e)
        {
            switch (BaseUtil.ParseInt(WindowFlag.ToString())) //判断当前窗体处于那个状态
            {
                case 1:                                 //当窗体处于最上端时
                    if (this.Top < 3)                   //当窗体与容器工作区的上边缘的距离小于5px时
                        this.Top = -(this.Height - 2);  //设定当前窗体距容器工作区上边缘的值
                    break;
                case 2:                                 //当窗体处于最左端时
                    if (this.Left < 3)                  //当窗体与容器工作区的左边缘的距离小于5px时
                        this.Left = -(this.Width - 2);  //设定当前窗体据容器工作区左边缘的值
                    break;
                case 3:                                 //当窗体处于最右端时
                    if ((this.Left + this.Width) > (GetSystemMetrics(0) - 3))   //当窗体与容器工作区的右边缘的距离小于5px时
                        this.Left = GetSystemMetrics(0) - 2;                    //设定当前窗体距容器工作区左边缘的值
                    break;
                case 4:                                 //当窗体处于最低端时
                    if (this.Bottom > Screen.AllScreens[0].Bounds.Height - 3)   //当窗体与容器工作区的下边缘的距离小于5px时
                        this.Top = Screen.AllScreens[0].Bounds.Height - 5;      //设定当前窗体距容器工作区上边缘之间的距离
                    break;

            }


        }
        //当窗体离开左右隐藏区域时，窗体回复原有高度
        private void Form_Caller_LocationChanged(object sender, EventArgs e)
        {

            if (this.Left > 3 && this.Right < (GetSystemMetrics(0) - 3))
            {
                if (this.Height == Screen.AllScreens[0].Bounds.Height - 20)
                {
                    this.Height = OriHeight;
                }
            }

        }
        private void Form_Caller_FormClosing(object sender, FormClosingEventArgs e)
        {


            if (MessageBox.Show("您确认退出吗？", "提示信息", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    MySqlConnection MyCon = new MySqlConnection(Properties.Settings.Default.MySqlConnectStr);
                    string uid = MyVal.uid;
                    string room_id = MyVal.room_id;
                    string sql = "delete from client_uid where uid='" + uid + "'";
                    MyCon.Open();
                    MySqlCommand DBComm = new MySqlCommand(sql, MyCon);

                    try
                    {

                        if (DBComm.ExecuteNonQuery() == 1)
                        {
                            //log.Info("清除数据成功，执行SQL=" + sql);
                        }
                        else
                        {
                            //log.Info("清除登录数据失败，执行SQL：" + sql);
                        }


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                    finally
                    {
                        DBComm.Dispose();
                        MyCon.Close();
                    }

                    Dispose();
                    Application.Exit();
              
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
            else
            {

                e.Cancel = true;
            }


        }






        private void timer1_Tick(object sender, EventArgs e)
        {
            this.toolStripStatusLabel3.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }






        private void timer3_Tick(object sender, EventArgs e)
        {
            //如果ti=0则让任务栏图标变为透明的图标并且退出 
            if (ti < 1)
            {
                this.notifyIcon1.Icon = ico2;
                ti++;
                return;
            }
            //如果ti!=0,就让任务栏图标变为ico1,并将ti置为0; 
            else
                this.notifyIcon1.Icon = ico1;
            ti = 0;

        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.notifyIcon1.Icon = ico1;
            this.timer3.Enabled = false;

            this.Top = 10;

        }



        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

       
        public void DialogShow(string Caption, string Message)
        {
            MessageBox.Show(Message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void ChangePosition(int x, int y)
        {
            this.Location = new Point(x, y);   //为当前窗体定位


        }
        public void SetValue(string str)
        {
            string[] str2 = str.Split('*');
            MyVal.uid = str2[0];
            MyVal.room_id = str2[2];
            int pos = str2[2].IndexOf("|");
            
            if (pos != -1)
            {
                string[] pca = str2[2].Split('|');
                toolStripStatusLabel1.Text = "第" + pca[0] + "诊室 " + "第" + pca[1] + "诊台";
            }
            else
            {
                toolStripStatusLabel1.Text = "第" + room_id + "诊室";
            }
            toolStripStatusLabel2.Text = str2[1];

        }



        private void 隐藏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangePosition(Screen.PrimaryScreen.Bounds.Width - 460, 0);
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_MinimumSizeChanged(object sender, EventArgs e)
        {
            MessageBox.Show("111");
        }

        

    }
    public class MyVal
    {
        public static string uid;
        public static string room_id;
    } 

}
