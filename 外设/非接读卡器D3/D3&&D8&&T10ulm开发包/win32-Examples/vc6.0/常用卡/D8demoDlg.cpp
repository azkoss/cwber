// D8demoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "D8demo.h"
#include "D8demoDlg.h"
#include "dcrf32.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CD8demoDlg dialog

CD8demoDlg::CD8demoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CD8demoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CD8demoDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CD8demoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CD8demoDlg)
	DDX_Control(pDX, IDC_LIST1, m_list1);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CD8demoDlg, CDialog)
	//{{AFX_MSG_MAP(CD8demoDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON1, OnButton1)
	ON_BN_CLICKED(IDC_BUTTON2, OnButton2)
	ON_BN_CLICKED(IDC_BUTTON3, OnButton3)
	ON_BN_CLICKED(IDC_BUTTON4, OnButton4)
	ON_BN_CLICKED(IDC_BUTTON5, OnButton5)
	ON_BN_CLICKED(IDC_BUTTON6, OnButton6)
	ON_BN_CLICKED(IDC_BUTTON7, OnButton7)
	ON_BN_CLICKED(IDC_BUTTON8, OnButton8)
//	ON_BN_CLICKED(IDC_BUTTON9, OnButton9)
//	ON_BN_CLICKED(IDC_BUTTON10, OnButton10)
//	ON_BN_CLICKED(IDC_BUTTON11, OnButton11)
//	ON_BN_CLICKED(IDC_BUTTON12, OnButton12)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CD8demoDlg message handlers

BOOL CD8demoDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	icdev = INVALID_HANDLE_VALUE;
	st = 0;
	// TODO: Add extra initialization here
	
	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CD8demoDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CD8demoDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CD8demoDlg::OnButton1() 
{
	// TODO: Add your control notification handler code here
	char  rbuff[1024]="\0"; 
	char sbuff[1024] = "\0";
	CString temp = "";
	unsigned char  urbuff[1024]="\0";
	m_list1.ResetContent();
	m_list1.AddString ("dc init ...");
	icdev = dc_init(100,115200);
	if(icdev < 0)
	{
		m_list1.AddString("dc init error");
		return;
	}
	m_list1.AddString("dc init ok");
	st = dc_config_card(icdev,65);
	m_list1.AddString("dc card ...");
	st = dc_card_hex(icdev,0,urbuff);
	if (st != 0)
	{
		m_list1.AddString("dc card error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("dc card ok ");
	temp.Format ("card id : %s",(char *)urbuff);
	m_list1.AddString( temp);
	m_list1.AddString("dc load key 'ffffffffffff' for sector 0...");
	memcpy(sbuff,"ffffffffffff",12);
	st = dc_load_key_hex(icdev,0,2,sbuff);
	if(st != 0)
	{
		m_list1.AddString("dc load key error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("dc load key ok");
	m_list1.AddString("dc authenticate key at sector 0...");
	st = dc_authentication(icdev,0,2);
	if(st != 0)
	{
		m_list1.AddString("dc authentication error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("dc authentication ok");
	m_list1.AddString("read info from block 2 of sector 0...");
	st = dc_read_hex(icdev,10,rbuff);
	if(st!=0)
	{
		m_list1.AddString("read info error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read info ok");
	temp.Format ("the info is : %s",(char *)rbuff);
	m_list1.AddString( temp);
	m_list1.AddString("write '11223344556677889900aabbccddeeff' into block 2 of sector 0");	
	memcpy(rbuff,"11223344556677889900aabbccddeeff",32);
	st = dc_write_hex(icdev,10,rbuff);
	if(st!=0)
	{
		m_list1.AddString("write data error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("write data ok");
	m_list1.AddString("read info from block 2 of sector 0");
	st = dc_read_hex(icdev,10,rbuff);
	if(st!=0)
	{
		m_list1.AddString("read info error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read info ok");
	temp.Format ("the info is : %s",(char *)rbuff);
	m_list1.AddString( temp);
	dc_beep(icdev,10);
	dc_exit(icdev);
}

void CD8demoDlg::OnButton2() 
{
	// TODO: Add your control notification handler code here
	char  rbuff[1024]="\0";
	char sbuff[1024]="\0";
	unsigned char rlen = 0;
    unsigned char urbuff[1024]="\0";
	m_list1.ResetContent();
	icdev = dc_init(100,115200);
	if(icdev < 0)
	{
		m_list1.AddString("dc init error");
		return;
	}
	m_list1.AddString("dc init ok");
	st = dc_config_card(icdev,65);
	st = dc_card_hex(icdev,0,urbuff);
	if (st != 0)
	{
		m_list1.AddString("dc card error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("dc card ok ");
	m_list1.AddString( "card id shows beneath :");
	m_list1.AddString( (char *)urbuff);
	st = dc_pro_resethex(icdev,&rlen,rbuff);
	if(st != 0)
	{
		m_list1.AddString("dc pro reset error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("dc pro reset ok");

	memcpy(sbuff,"0084000008",10);
	st = dc_pro_commandhex(icdev,5,sbuff,&rlen,rbuff,7);
	if(st != 0)
	{
		m_list1.AddString("dc pro command error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("get a random number ok,the number shows beneath");
	m_list1.AddString(rbuff);
	dc_beep(icdev,10);
	dc_exit(icdev);
}

void CD8demoDlg::OnButton3() 
{
	// TODO: Add your control notification handler code here
	char  rbuff[1024]="\0"; 
	char sbuff[1024]="\0";
	unsigned char rlen = 0;
	m_list1.ResetContent();
	icdev = dc_init(100,115200);
	if(icdev < 0)
	{
		m_list1.AddString("dc init error");
		return;
	}
	m_list1.AddString("dc init ok");
	st = dc_config_card(icdev,66);
	st = dc_card_b_hex(icdev,rbuff);
	if (st != 0)
	{
		m_list1.AddString("dc card error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("dc card ok ");
	m_list1.AddString( "card id shows beneath :");
	m_list1.AddString(  rbuff);
	memcpy(sbuff,"0084000008",10);
	st = dc_pro_commandhex(icdev,5,sbuff,&rlen,rbuff,7);
	if(st != 0)
	{
		m_list1.AddString("dc pro command error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("get a random number ok,the number shows beneath");
	m_list1.AddString(rbuff);
	dc_beep(icdev,10);
	dc_exit(icdev);
}


void CD8demoDlg::OnButton4() 
{
	// TODO: Add your control notification handler code here
	char recvbuff[1024]="\0";
	char sendbuff[1024]="\0";
	unsigned char  crlen=0;
	CString send="";
	m_list1.ResetContent();
	icdev = dc_init(100,115200);
	if(icdev <0)
	{
		m_list1.AddString ("dc init error");
		return;
	}
	m_list1.AddString ("dc init ok");
	st = dc_setcpu(icdev,12);
	if(st != 0)
	{
		m_list1.AddString("dc_setcpu error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("dc_setcpu ok");
	st = dc_cpureset_hex(icdev,&crlen,recvbuff);
	if(st != 0)
	{
		dc_exit(icdev);
		m_list1.AddString ("dc  cpu reset error");
		return;
	}
	m_list1.AddString ("dc  cpu reset ok");
	m_list1.AddString("the reset info shows beneath :");
	m_list1.AddString(  recvbuff);
	memcpy(sendbuff,"0084000008",10);
	st = dc_cpuapdusource_hex(icdev,5,sendbuff,&crlen,recvbuff);
	if(st != 0)
	{
		m_list1.AddString ("dc apdu error");
	}
	else
	{
		m_list1.AddString ("get a random number ok,the number shows beneath :");
		m_list1.AddString(recvbuff);
	}
	dc_beep(icdev,10);
	dc_exit(icdev);
}

void CD8demoDlg::OnButton5() 
{
	// TODO: Add your control notification handler code here
	unsigned char rbuff[1024]="\0"; 
	unsigned char sbuff[1024]="\0";
	char temp[1024]="\0";
	m_list1.ResetContent();
	icdev = dc_init(100,115200);
	if(icdev <0)
	{
		m_list1.AddString ("dc init error");
		return;
	}
	m_list1.AddString ("dc init ok");
	st = dc_readpincount_4428(icdev);
	if(st <0)
	{
		m_list1.AddString("get the pin error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("the pin count shwos beneath");
	memcpy(sbuff,"ffff",4);
	st = dc_verifypin_4428_hex(icdev,sbuff);
	if(st != 0)
	{
		m_list1.AddString("verify the pin failed");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("verify the pin ok");
	st = dc_changepin_4428_hex(icdev,sbuff);
	if(st != 0)
	{
		m_list1.AddString("change the pin to ffff error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("change the pin to ffff ok");
	st = dc_readpin_4428_hex(icdev,rbuff);
	if(st != 0)
	{
		m_list1.AddString("read the pinn error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read the pin ok ,the pin shows beneath:");
	m_list1.AddString((char *)rbuff);
	m_list1.AddString("read the data from 245 to 250 ...");
	st =dc_read_4428_hex(icdev, 245, 6, rbuff);
	if(st!=0)
	{
		m_list1.AddString("dc read error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read the data ok ,the data shows beneath");
	m_list1.AddString((char *)rbuff);
	memcpy(sbuff,"1234567890ab",12);
	m_list1.AddString("write '1234567890ab' from 245 to 250");
	st = dc_write_4428_hex(icdev, 245, 6, sbuff);
	if(st != 0)
	{
		m_list1.AddString("write data error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("write the data ok");
	m_list1.AddString("read the data from 245 to 250 ...");
	st = dc_read_4428_hex(icdev, 245, 6, rbuff);
	if(st!=0)
	{
		m_list1.AddString("dc read error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read the data ok ,the data shows beneath");
	m_list1.AddString((char *)rbuff);
	dc_beep(icdev,10);
	dc_exit(icdev);
}

void CD8demoDlg::OnButton6() 
{
	// TODO: Add your control notification handler code here
	unsigned char rbuff[1024]="\0";
	unsigned char sbuff[1024]="\0";
	char temp[1024]="\0";
	
	m_list1.ResetContent();
	icdev = dc_init(100,115200);
	
	if(icdev <0)
	{
		m_list1.AddString ("dc init error");
		return;
	}
	m_list1.AddString ("dc init ok");
	
	st = dc_readpincount_4442(icdev);
	if(st <0)
	{
		m_list1.AddString("get the pin error");
		dc_exit(icdev);
		return;
	}
	st+=48;
	m_list1.AddString("the pin count shwos beneath");
	m_list1.AddString((char * )&st);
	
//	memcpy(temp,,6);
	memcpy(sbuff,"ffffff",6);
	st = dc_verifypin_4442_hex(icdev,sbuff);
	if(st != 0)
	{
		m_list1.AddString("verify the pin failed");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("verify the pin ok");
	
	st = dc_changepin_4442_hex(icdev,sbuff);
	if(st != 0)
	{
		m_list1.AddString("change the pin to ffffff error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("change the pin to ffffff ok");
	
	st = dc_readpin_4442_hex(icdev,rbuff);
	if(st != 0)
	{
		m_list1.AddString("read the pinn error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read the pin ok ,the pin shows beneath:");
	m_list1.AddString((char *)rbuff);
	
	m_list1.AddString("read the data from 245 to 250 ...");
	st =dc_read_4442_hex(icdev, 245, 6, rbuff);
	if(st!=0)
	{
		m_list1.AddString("dc read error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read the data ok ,the data shows beneath");
	m_list1.AddString((char *)rbuff);
	
	//memcpy(temp,,12);
	memcpy(sbuff,"1234567890ab",12);
	m_list1.AddString("write '1234567890ab' from 245 to 250");
	st = dc_write_4442_hex(icdev, 245, 6, sbuff);
	if(st != 0)
	{
		m_list1.AddString("write data error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("write the data ok");
	
	m_list1.AddString("read the data from 245 to 250 ...");
	st =dc_read_4442_hex(icdev, 245, 6, rbuff);
	if(st!=0)
	{
		m_list1.AddString("dc read error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read the data ok ,the data shows beneath");
	m_list1.AddString((char *)rbuff);
	
	//memcpy(temp,"1234567890ab",12);
	memcpy(sbuff,"1234567890ab",12);
	m_list1.AddString("write '1234567890ab' from 245 to 250");
	st = dc_write_4442_hex(icdev, 245, 6, sbuff);
	if(st != 0)
	{
		m_list1.AddString("write data error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("write the data ok");
	dc_beep(icdev,10);
	dc_exit(icdev);
}

void CD8demoDlg::OnButton7() 
{
	unsigned char rdata[1024]="\0";
	int status = -1;
	char sdata[1024]="\0";
	CString temp ="";
	// TODO: Add your control notification handler code here
	m_list1.ResetContent();
	icdev = dc_init(100,115200);
	if(icdev <= 0)
	{
		m_list1.AddString("dc init error");
		return;
	}
	m_list1.AddString("dc init ok");
	st = dc_setcpu(icdev,12);//设置接触卡位为附卡座
	memcpy(sdata,"1234567890abcdef",16);
	st = dc_write_24c_hex(icdev,0,8,(unsigned char *)sdata);
	if(st !=0)
	{
		m_list1.AddString("write data error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("write into the area from 0 to 7 as '1234567890abcdef' ok");
	st = dc_read_24c_hex(icdev,0,8,rdata);
	if(st !=0)
	{
		m_list1.AddString("read data error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read data from 0 to 7  ok");
	temp.Format ("card id shows beneath : %s",(char *)rdata);
	m_list1.AddString("the data is " + temp);
	dc_beep(icdev,10);
	dc_exit(icdev);
}

void CD8demoDlg::OnButton8() 
{
	// TODO: Add your control notification handler code here
	unsigned char rdata[1024]="\0";
	int status = -1;
	char sdata[1024]="\0";
	CString temp="";
	// TODO: Add your control notification handler code here
	m_list1.ResetContent();
	icdev = dc_init(100,115200);
	if(icdev <= 0)
	{
		m_list1.AddString("dc init error");
		return;
	}
	m_list1.AddString("dc init ok");
	st = dc_setcpu(icdev,12);//设置接触卡位为附卡座
	memcpy(sdata,"1234567890abcdef",16);
	st = dc_write_24c64_hex(icdev,0,8,(unsigned char *)sdata);
	if(st !=0)
	{
		m_list1.AddString("write data error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("write into the area from 0 to 7 as '1234567890abcdef' ok");
	st = dc_read_24c64_hex(icdev,0,8,rdata);
	if(st !=0)
	{
		m_list1.AddString("read data error");
		dc_exit(icdev);
		return;
	}
	m_list1.AddString("read data from 0 to 7  ok");
	temp.Format ("card id shows beneath : %s",(char *)rdata);
	m_list1.AddString("the data is " + temp);
	dc_beep(icdev,10);
	dc_exit(icdev);
}

//void CD8demoDlg::OnButton9() 
//{
	// TODO: Add your control notification handler code here
//	unsigned char rdata[1024];
//	char * sbuff,* mkey;
//	sbuff = "fedcba98765432100123456789abcdef";
//	mkey = "0123456789abcdeffedcba9876543210";
//	int st = -1;
//	st = dc_tripledes_hex((unsigned char *)mkey,(unsigned char *)sbuff,rdata,0);
//	st = -1;
//}

//void CD8demoDlg::OnButton10() 
//{
	// TODO: Add your control notification handler code here
//	int st = -1;
//	unsigned char rdata[1024],* sbuff;
//	char rbuff[1024];
//	long snr;
	

//	icdev =dc_init(100,115200);

//	st = dc_card_hex(icdev,0,rdata);

//	st = dc_authentication(icdev,0,2);

//	mtest("123456789123456789f",19);
	

//	st = dc_read_hex(icdev,8,rbuff);

//	rbuff[19] = 0;


//	dc_exit(icdev);

//}

int CD8demoDlg::mtest(char *sbuff,int slen)
{
	char msbuff[128];
	memset(msbuff,48,32);
	memcpy(msbuff,sbuff,slen);
	st  = dc_write_hex(icdev,8,msbuff);
	return st;
}

void CD8demoDlg::OnButton11() 
{
	// TODO: Add your control notification handler code here
	char * key = "A81296541567CE19\0";
	char * sour = "abcdef1234567890\0";
	unsigned  char dest[128] = "\0";
	
	__int16 st = -1;
	
	st = dcdeshex((unsigned char *)key,(unsigned char *)sour,dest,1);
}

void CD8demoDlg::OnButton12() 
{
	// TODO: Add your control notification handler code here
/*	unsigned char key[128]={'A','8','1','2','9','6','5','4','1','5','6','7','C','E','1','9',0};
    unsigned char sour[128]={'a','b','c','d','e','f','1','2','3','4','5','6','8','7','9','0',0};
	unsigned char dest[128];
	int st;
	//	st=dcdeshex(key,sour,dest,0);
	//  if (st!=0)
	//{
	//	MessageBox("dcdeshex error");
	//	return;
	//}
	//	MessageBox("dcdeshex ok");
	//	dest[16]=0;
	//	MessageBox(dest);
	
	//	MessageBox(sour);
	//    sour[0]=0;
	
	st=dc_des_hex(key,sour,dest,0);
    if (st!=0)
	{
	//	MessageBox("dcdeshex error");
		return;
	}
	//MessageBox("dcdeshex ok");
	
		sour[16]=0;*/
	unsigned char rdata[1024];

	icdev =dc_init(100,115200);
	
	st = dc_card_double_hex(icdev,0,rdata);

	dc_exit(icdev);
}
