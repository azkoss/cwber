using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using NHTSS.ClientApp.RWIC.MDI;
using System.Text;


namespace NHTSS.ClientApp.RWIC
{
	/// <summary>
	/// Form1 ��ժҪ˵����
	/// </summary>
	public class FormRWIC : System.Windows.Forms.Form
	{
		#region ��USB�ӿڵ�ʹ��(PHILIPH��)
		[DllImport("dcrf32.dll")]	
		public static extern int dc_init(Int16 port,long baud);  //���Ի�
		[DllImport("dcrf32.dll")]	
		public static extern short dc_exit(int icdev);
		[DllImport("dcrf32.dll")]	
		public static extern short dc_reset(int icdev,uint sec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_config_card(int icdev, byte cardtype);
		[DllImport("dcrf32.dll")]	
		public static extern short dc_request(int icdev,char _Mode,ref uint TagType);
		[DllImport("dcrf32.dll")]	
		public static extern short dc_card(int icdev,char _Mode,ref ulong Snr);
		[DllImport("dcrf32.dll")]	
		public static extern short dc_halt(int icdev);
		[DllImport("dcrf32.dll")]	
		public static extern  short dc_anticoll(int icdev,char _Bcnt,ref ulong IcCardNo);
		[DllImport("dcrf32.dll")]	
		public static extern  short dc_beep(int icdev,uint _Msec);
		[DllImport("dcrf32.dll")]	
		public static extern short dc_authentication(int icdev,int _Mode,int _SecNr);
		
		[DllImport("dcrf32.dll")]	
		public static extern short dc_load_key(int  icdev, int mode,int secnr,[In] byte[] nkey );  //����װ�ص���дģ����
		[DllImport("dcrf32.dll")]	
		public static extern short dc_load_key_hex(int  icdev, int mode,int secnr, string nkey );  //����װ�ص���дģ����

		[DllImport("dcrf32.dll")]	
		public static extern short dc_write ( int  icdev, int adr, [In] byte[] sdata);  //����д������
		[DllImport("dcrf32.dll")]	
		public static extern short dc_write ( int  icdev, int adr, [In] string sdata);  //����д������
		[DllImport("dcrf32.dll")]	
		public static extern short dc_write_hex ( int  icdev, int adr,[In] string sdata);  //����д������(ת��Ϊ16����)

		[DllImport("dcrf32.dll")]	
		public static extern short dc_read ( int  icdev, int adr,[Out] byte[] sdata ); 

		[DllImport("dcrf32.dll")]	
		public static extern short dc_read ( int  icdev, int adr,[MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata );  //�ӿ��ж�����
		[DllImport("dcrf32.dll")]	
		public static extern short dc_read_hex ( int  icdev, int adr,[MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata );  //�ӿ��ж�����(ת��Ϊ16����)
		[DllImport("dcrf32.dll")]	
		public static extern int a_hex (string  oldValue,ref string newValue,Int16 len);  //��ͨ�ַ�ת����ʮ�������ַ�
		[DllImport("dcrf32.dll")]	
		public static extern void hex_a (ref string  oldValue,ref string newValue,int len);  //ʮ�������ַ�ת������ͨ�ַ�


		#endregion

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mItemIcManager;
		private System.Windows.Forms.MenuItem mItemExit;


		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormRWIC()
		{
			//
			// Windows ���������֧���������
			//
			InitializeComponent();

			//
			// TODO: �� InitializeComponent ���ú�����κι��캯������
			//
		}

		/// <summary>
		/// ������������ʹ�õ���Դ��
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows ������������ɵĴ���
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.mItemIcManager = new System.Windows.Forms.MenuItem();
			this.mItemExit = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mItemIcManager,
																					  this.mItemExit});
			// 
			// mItemIcManager
			// 
			this.mItemIcManager.Index = 0;
			this.mItemIcManager.Text = "IC������(&M)";
			this.mItemIcManager.Click += new System.EventHandler(this.mItemIcManager_Click);
			// 
			// mItemExit
			// 
			this.mItemExit.Index = 1;
			this.mItemExit.Text = "�˳�(&E)";
			this.mItemExit.Click += new System.EventHandler(this.mItemExit_Click);
			// 
			// FormRWIC
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.IsMdiContainer = true;
			this.Menu = this.mainMenu1;
			this.Name = "FormRWIC";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "IC����д����";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormRWIC_Load);
			this.Closed += new System.EventHandler(this.FormRWIC_Closed);

		}
		#endregion

		private int _icdev = -1;

		#region IC�����������ԣ�PHILIPS����
		public int IcDev
		{
			get { return _icdev; }
			set { _icdev = value; }
		}

		public void InitIC()
		{
			//��ʼ������1
			if (_icdev < 0)
			{
				_icdev = dc_init(0,115200);
			}
			
			if(_icdev<=0)
			{
				//MessageBox.Show("Init Com Error!"+_icdev.ToString());
			}
			else
			{
				string a = "ffffffffffff";
				//MessageBox.Show("Init Com OK!");
				//int s =dc_load_key_hex(_icdev,0,0,a);
				//MessageBox.Show(s.ToString());
			}
		}


		//�رմ���
		public void ExitIC()
		{
			dc_exit((Int16)_icdev);
		}

		//����
		public string ReadIc()
		{
			uint st=4;
			ulong icCardNo=0;
			char str=(char )0;
		    dc_card((Int16)_icdev,str,ref icCardNo);
			return icCardNo.ToString();
		}

		//������
		public void Beep()
		{
			dc_beep((Int16)_icdev,50);
		}

		#endregion


		/// <summary>
		/// Ӧ�ó��������ڵ㡣
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new FormRWIC());
		}

		private void FormRWIC_Closed(object sender, System.EventArgs e)
		{
			//��������
			ExitIC();
		}

		private void FormRWIC_Load(object sender, System.EventArgs e)
		{
			//���Ի�IC��
			InitIC();
			mItemIcManager_Click(null, null);
		}

		private void mItemIcManager_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				FormIcManager IcManager = new FormIcManager();
				IcManager.MdiParent = this;
				IcManager.Show();
				IcManager.Activate();
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void mItemExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
