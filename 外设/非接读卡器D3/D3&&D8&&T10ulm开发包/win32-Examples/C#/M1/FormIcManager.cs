using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Timer = System.Timers.Timer;
using System.Runtime.InteropServices;
using NHTSS.ClientApp.RWIC;
using System.Text;

namespace NHTSS.ClientApp.RWIC.MDI
{
	/// <summary>
	/// FormIcManager ��ժҪ˵����
	/// </summary>
	public class FormIcManager : System.Windows.Forms.Form
	{
		[DllImport("dcrf32.dll")]	
		public static extern int dc_init(Int16 port,long baud);  //���Ի�
		[DllImport("dcrf32.dll")]	
		public static extern int dc_exit(int icdev);
		[DllImport("dcrf32.dll")]	
		public static extern int dc_reset(int icdev,uint sec);
		[DllImport("dcrf32.dll")]	
		public static extern int dc_request(int icdev,char _Mode,ref uint TagType);
		[DllImport("dcrf32.dll")]	
		public static extern int dc_card(int icdev,char _Mode,ref ulong Snr);
		[DllImport("dcrf32.dll")]	
		public static extern int dc_halt(int icdev);
		[DllImport("dcrf32.dll")]	
		public static extern  int dc_anticoll(int icdev,char _Bcnt,ref ulong IcCardNo);
		[DllImport("dcrf32.dll")]	
		public static extern  int dc_beep(int icdev,uint _Msec);
		[DllImport("dcrf32.dll")]	
		public static extern int dc_authentication(int icdev,int _Mode,int _SecNr);
		
		[DllImport("dcrf32.dll")]	
		public static extern int dc_load_key(long  icdev, int mode,int secnr,[In] byte[] nkey );  //����װ�ص���дģ����
		[DllImport("dcrf32.dll")]	
		public static extern int dc_load_key_hex(long  icdev, int mode,int secnr, string nkey );  //����װ�ص���дģ����

		[DllImport("dcrf32.dll")]	
		public static extern int dc_write ( int  icdev, int adr, [In] byte[] sdata);  //����д������
		[DllImport("dcrf32.dll")]	
		public static extern int dc_write_hex ( int  icdev, int adr,[In] byte[] sdata);  //����д������(ת��Ϊ16����)

		[DllImport("dcrf32.dll")]	
		public static extern int dc_read ( int  icdev, int adr,[Out] byte[] sdata );  //�ӿ��ж�����
		[DllImport("dcrf32.dll")]	
		public static extern int dc_read_hex ( int  icdev, int adr, [Out] byte[] sdata );  //�ӿ��ж�����(ת��Ϊ16����)
		[DllImport("dcrf32.dll")]	
		public static extern int a_hex (string  oldValue,ref string newValue,Int16 len);  //��ͨ�ַ�ת����ʮ�������ַ�
		[DllImport("dcrf32.dll")]	
		public static extern void hex_a (ref string  oldValue,ref string newValue,int len);  //ʮ�������ַ�ת������ͨ�ַ�



		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

		public FormIcManager()
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
				if(components != null)
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
			this.components = new System.ComponentModel.Container();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.button1 = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.ItemHeight = 12;
			this.listBox1.Location = new System.Drawing.Point(16, 32);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(488, 460);
			this.listBox1.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(536, 184);
			this.button1.Name = "button1";
			this.button1.TabIndex = 5;
			this.button1.Text = "����";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// FormIcManager
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.listBox1);
			this.Name = "FormIcManager";
			this.Text = "FormIcManager";
			this.Load += new System.EventHandler(this.FormIcManager_Load);
			this.ResumeLayout(false);

		}
		#endregion

		#region "����/��������"
		private FormRWIC _frmParent = null;
		
		//private System.Windows.Forms.Timer _timer = null;
		//private System.Timers.Timer _timer = null ;
		
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{

			
			int st;
			//1.��ʼ��
			if (_frmParent.IcDev > 0)
			{
				listBox1.Items.Add("��ʼ���ɹ���");
			}
			else
			{
				listBox1.Items.Add("��ʼ��ʧ�ܣ�");
				return;
			}

			//����


			st = FormRWIC.dc_beep(_frmParent.IcDev,10);
			if (st == 0)
			{
				listBox1.Items.Add("�����ɹ���");
			}
			else
			{
				listBox1.Items.Add("����ʧ�ܣ�");
				return;
			}

			ulong icCardNo=0;
			char tt=(char )0;
			uint ss=5;
			st = FormRWIC.dc_reset(_frmParent.IcDev,ss);
            st = FormRWIC.dc_config_card(_frmParent.IcDev, 65);
			st = FormRWIC.dc_card(_frmParent.IcDev, tt, ref icCardNo);
			if (icCardNo != 0)
			{
				listBox1.Items.Add("Ѱ���ɹ������ں�Ϊ��"+icCardNo);
			}
			else
			{
				listBox1.Items.Add("Ѱ��ʧ�ܣ�");
				return;
			}

			byte[] hexkey =new byte[6]{0xff,0xff,0xff,0xff,0xff,0xff};
		
			st = FormRWIC.dc_load_key(_frmParent.IcDev,0, 0, hexkey);

			//�˶�����
			int sector = 0;
			st = FormRWIC.dc_authentication(_frmParent.IcDev, 0, sector);
			if (st == 0)
			{
				listBox1.Items.Add("��֤����ɹ���");
			}
			else
			{
				listBox1.Items.Add("��֤����ʧ�ܣ�");
				return;
			}

			int address = 1;
			byte[] data32;
			data32  =new byte[16]{0x6A, 0xC2, 0x92, 0xFA, 0xA1, 0x31, 0x5B, 0x4D,0x6A, 0xC2, 0x92, 0xFA, 0xA1, 0x31, 0x5B, 0x4D};//= "12345678901234561234567890123456";
			
		
			string data32_hex = "".PadLeft(32, ' ');


			address = sector*4 + 2;
		

			//д������

			data32_hex="�й�d8�������";
			st = FormRWIC.dc_write( _frmParent.IcDev, address,data32_hex );
			if (st == 0)
			{
				listBox1.Items.Add("д���ɹ�������Ϊ��"+data32_hex);
			}
			else
			{
				listBox1.Items.Add("д��ʧ�ܣ�");
				return;
			}


			
			StringBuilder temp =	new StringBuilder(64);
			StringBuilder temp1 =	new StringBuilder(64);
			byte[] TempBit = new byte[256];
			char[] databur = new char[32];
			
			string StrTemp = temp.ToString();
			//char a ;

			//��������
			string databuff32 = string.Empty;
			st = FormRWIC.dc_read( _frmParent.IcDev, address,temp1);
			if (st == 0)
			{
				listBox1.Items.Add("�����ɹ���������Ϊ��"+ temp1);
			}
		

			try 
			{
				st = FormRWIC.dc_read_hex( _frmParent.IcDev, address, temp);
				
				if (st == 0)
				{
					listBox1.Items.Add("�����ɹ���������Ϊ��"+ temp);
				}
				else
				{
					listBox1.Items.Add("����ʧ�ܣ�");
					return;
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

		}



		private void FormIcManager_Load(object sender, System.EventArgs e)
		{
			_frmParent = this.MdiParent as FormRWIC;
			MessageBox.Show(_frmParent.IcDev.ToString());
		}

	}
	
}
