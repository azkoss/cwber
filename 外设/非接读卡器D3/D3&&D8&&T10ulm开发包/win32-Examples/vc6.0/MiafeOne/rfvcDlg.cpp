// rfvcDlg.cpp : implementation file
//

#include "stdafx.h"
#include "rfvc.h"
#include "rfvcDlg.h"
#include "dcrf32.h"
#include "General.h"
#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//dc_HANDLE icdev;
HANDLE icdev;
unsigned long cardsnr;
/////////////////////////////////////////////////////////////////////////////
// CRfvcDlg dialog

CRfvcDlg::CRfvcDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CRfvcDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CRfvcDlg)
	
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CRfvcDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CRfvcDlg)
	DDX_Control(pDX, IDC_LIST1, m_list1);

	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CRfvcDlg, CDialog)
	//{{AFX_MSG_MAP(CRfvcDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON1, OnButton1)
	ON_BN_CLICKED(IDC_BUTTON2, OnButton2)
	ON_BN_CLICKED(IDC_BUTTON3, OnButton3)
	ON_BN_CLICKED(IDC_BUTTON4, OnButton4)
	ON_BN_CLICKED(IDC_BUTTON5, OnButton5)
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_BUTTON7, OnButton7)
	ON_BN_CLICKED(IDC_BUTTON8, OnButton8)
	ON_BN_CLICKED(IDC_BUTTON9, OnButton9)
	ON_BN_CLICKED(IDC_BUTTON11, OnButton11)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CRfvcDlg message handlers

BOOL CRfvcDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here
    icdev=0;	
	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CRfvcDlg::OnPaint() 
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
HCURSOR CRfvcDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}
void CRfvcDlg::show(CString datastr)
{

		m_list1.AddString(datastr);
		m_list1.SetCurSel(m_list1.GetCount()-1);
		MSG message;
		if (::PeekMessage(&message,NULL,0,0,PM_REMOVE))
		{
			::TranslateMessage(&message);
			::DispatchMessage(&message);
		}
}
void CRfvcDlg::OnButton1() 
{
	// TODO: Add your control notification handler code here

	//初始化串口1，       
	icdev=dc_init(1,115200);
	if((int)icdev<=0)
	{
	   show("Init Com Error!");
	}
	else
	{
      show("Init Com OK!");
	}
	dc_beep(icdev,10);
	return;

}

void CRfvcDlg::OnButton2() 
{
	// TODO: Add your control notification handler code here

	//下载密码，       
  int st;
  unsigned char keyA[6]={0xff,0xff,0xff,0xff,0xff,0xff};
  st=dc_load_key(icdev,0,32,keyA);
  if(st!=0)
	 show("Load Key Error!");
  else
	 show("Load Key Ok!");	
}

void CRfvcDlg::OnButton3() 
{
	// TODO: Add your control notification handler code here

	//寻卡函数，      
	int st;
	CString linstr;

	st=dc_card(icdev,0,&cardsnr);
    if(st!=0)
	{
   	   show("Find Card Error!");
       return;
	}
	else
	{
		show("Find Card Ok!");
	    linstr.Format("%d",cardsnr);
	    show(linstr);

	}

}

void CRfvcDlg::OnButton4() 
{
	// TODO: Add your control notification handler code here
	
	//核对密码       
	int st;
	st=dc_authentication(icdev,0,15);
    if(st!=0)
   	   show("Auth Card Error!");
    else
	   show("Auth Card Ok!");		


}

void CRfvcDlg::OnButton5() 
{
	// TODO: Add your control notification handler code here

	//写数据     
	int st;
	unsigned char databuff[]={0x44,0x33,0x33,0x66,0x33,0x44,0x33,0x66,0x36,0x34,0x33,0x44,0x33,0x66,0x36,0x34,0x00};//
	char databuff2[33];

	st=dc_write(icdev,61,databuff);
    if(st!=0)
	{
   	   show("Write Card Error!");
	   return;
	}
    else
	{
	   show("Write Card Ok!");	
       show(databuff);
	}
	st=dc_write_hex(icdev,254,databuff2);
    if(st!=0)
   	   show("Writehex Card Error!");
    else
	{
	   show("Writehex Card Ok!");	
       show(databuff2);
	}

}


void CRfvcDlg::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	dc_exit(icdev);
	CDialog::OnClose();
}

void CRfvcDlg::OnButton7() 
{
	// TODO: Add your control notification handler code here
    int st;
	st=dc_halt(icdev);
    if(st!=0)
   	   show("Halt Card Error!");
    else
	   show("Halt Card Ok!");	
	
}

void CRfvcDlg::OnButton8() 
{
	// TODO: Add your control notification handler code here
    // 设备函数          
	unsigned char key[]={'1','2','3','4','5','6','7','8','9','0','a','b','c','d','e','f',0};
    unsigned char sour[]={'a','b','c','d','e','f','1','2','3','4','5','6','8','7','9','0',0};

	unsigned char dest[16];
	int st;
    unsigned char addr;
	unsigned char len;
	unsigned char datastr[10];
    CString linstr;
	unsigned char linchar[17];
	unsigned char timestr[17];



    memset(linchar,0,16);
	st= dc_getver(icdev,linchar);//得设备号
	if (st!=0)
	{
		show("dc_getver error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_getver ok");
//	hex_a(linchar,linchar,8);
	show(linchar);
	
    linchar[0]=0x45;
    linchar[1]=0x75;
    linchar[3]=0x55;
    linchar[4]=0x65;
    linchar[2]=0x35;
	linchar[5]=0;
	st= dc_swr_eeprom(icdev,500,5,linchar);//写存储器EEPROM的函数（范围：0-1597）
	if (st!=0)
	{
		show("dc_swr_eeprom error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_swr_eeprom ok");

	linchar[0]=0;
	st= dc_srd_eeprom(icdev,500,5,linchar);//read
	if (st!=0)
	{
		show("dc_srd_eeprom error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_srd_eeprom ok");
	linchar[5]=0;
	show(linchar);

	linchar[0]='4';
    linchar[1]='5';
    linchar[2]='7';
    linchar[3]='5';
    linchar[4]='3';
    linchar[5]='5';
    linchar[6]='5';
    linchar[7]='5';
    linchar[8]='6';
    linchar[9]='5';
	linchar[10]=0;
	st= dc_swr_eepromhex(icdev,600,5,linchar);//
	if (st!=0)
	{
		show("dc_swr_eepromhex error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_swr_eepromhex ok");

	linchar[0]=0;
	st= dc_srd_eepromhex(icdev,600,5,linchar);
	if (st!=0)
	{
		show("dc_srd_eepromhex error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_srd_eepromhex ok");
	linchar[10]=0;
	show(linchar);
    
	linchar[0]='a';
    linchar[1]='.';
    linchar[2]='b';
    linchar[3]='.';
    linchar[4]='c';
    linchar[5]='d';
    linchar[6]='.';
    linchar[7]='0';
	st= dc_disp_str(icdev,(char *)linchar);//数码显示
	if (st!=0)
	{
		show("dc_disp_str error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_disp_str ok");

	st= dc_reset(icdev,100);//设备复位
	if (st!=0)
	{
		show("dc_reset error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_reset ok");

	st= dc_setbright(icdev,10);//设计亮度函数，0－15
	if (st!=0)
	{
		show("dc_setbright error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_setbright ok");

	st= dc_ctl_mode(icdev,1);//控制显示函数，0:计算机控制显示，1:读写器控制显示
	if (st!=0)
	{
		show("dc_ctl_mode error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_ctl_mode ok");

	st= dc_disp_mode(icdev,1);//显示格式函数，0，年-月-日，1:时-分-秒
	if (st!=0)
	{
		show("dc_disp_mode error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_disp_mode ok");

	st= dc_gettime( icdev,timestr); //从设备获取时间，格式：年，星期，月，日，时，分，秒 
	if (st!=0)
	{
		show("dc_gettime error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_gettime ok");
	timestr[7]=0;

	st= dc_settime( icdev,timestr );  //设计设备时间，格式：年，星期，月，日，时，分，秒
	if (st!=0)
	{
		show("dc_settime error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_settime ok");

	hex_a(timestr,linchar,7);
	show(linchar);

	
	st= dc_gettimehex( icdev,(char *)timestr); 
	if (st!=0)
	{
		show("dc_gettimehex error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_gettimehex ok");
	timestr[14]=0;
	show(timestr);

	st= dc_settimehex( icdev,(char *)timestr);
	if (st!=0)
	{
		show("dc_settimehex error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_settimehex ok");
	timestr[14]=0;
	show(timestr);
//	

    datastr[0]=0x80;
    datastr[1]=0x45;
    datastr[2]=0x8c;
    datastr[3]=0x00;
    datastr[4]=0x8a;
	datastr[5]=0x61;
    datastr[6]=0x01;
    addr=1;
	len=7;
	st= dc_high_disp(icdev, addr,len,datastr);//显示字符串操作
	if (st!=0)
	{
		show("dc_high_disp error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_high_disp ok");

    //des算法加解密函数 ，            
	st=dcdeshex(key,sour,dest,0);
    if (st!=0)
	{
		show("dcdeshex error");
		return;
	}
	show("dcdeshex ok");
	dest[16]=0;
	show(dest);

	show(sour);
    sour[0]=0;

	st=dcdeshex(key,dest,sour,1);
    if (st!=0)
	{
		show("dcdeshex error");
		return;
	}
	show("dcdeshex ok");
	sour[16]=0;
	show(sour);


}	

void CRfvcDlg::OnButton9() 
{ 
	//高级函数（不用执行dc_card）
	int st;
	CString linstr;
	unsigned char pieceaddr=128;
	unsigned long cardsnr=0;
	unsigned char wrdata[33]={'3','4','6','5','3','3','8','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3',0};
	unsigned char redata[33];
	//unsigned char cardsnrchar[16];


	st= dc_HL_authentication(icdev,0,0,0,32);
	if (st!=0)
	{
		show("dc_HL_authentication error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_HL_authentication ok");

	wrdata[16]=0;	
	st= dc_HL_write(icdev,0,pieceaddr,&cardsnr,wrdata);
	if (st!=0)
	{
		show("dc_HL_write error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_HL_write ok");
	linstr.Format("%d",cardsnr);
	show(linstr);
	cardsnr=0;
    
	st= dc_HL_read(icdev,0,pieceaddr,0,redata,&cardsnr);
	if (st!=0)
	{
		show("dc_HL_read error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_HL_read ok");
	linstr.Format("%d",cardsnr);
	show(linstr);
	redata[16]=0;
	show(redata);

    wrdata[16]='3';
	st= dc_HL_writehex(icdev,0,pieceaddr,&cardsnr,wrdata);
	if (st!=0)
	{
		show("dc_HL_writehex error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_HL_writehex ok");
	linstr.Format("%d",cardsnr);
	show(linstr);
      
	memset(redata,0,32);
	st= dc_HL_readhex(icdev,0,pieceaddr,0, redata,&cardsnr);
	if (st!=0)
	{
		show("dc_HL_readhex error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_HL_readhex ok");
	linstr.Format("%d",cardsnr);
	show(linstr);
	redata[32]=0;
	show(redata);
    
	
}


void CRfvcDlg::OnButton11() 
{
	//卡函数,
    unsigned char pieceaddr;
	unsigned long value;
    CString linstr;
    int st;
	unsigned char keya[7]={0xff,0xff,0xff,0xff,0xff,0xff};
	unsigned char keyb[7]={0xff,0xff,0xff,0xff,0xff,0xff};
	
	unsigned char datachar[40]={'3','4','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3','3'};
	
	
	int i;
	unsigned char databuff[17]={0};
	char databuff2[33]={0};




    //写数据      wfl   2003.10.28
	srand(time(NULL));
	for (i=0;i<16;i++)
	{
		databuff[i]=rand();
	}
	st=dc_write(icdev,61,databuff);
    if(st!=0)
	{
		show("Write Card Error!");
		return;
	}
    else
	{
		show("Write Card Ok!");	
		show(databuff);
	}
	st=dc_read_hex(icdev,61,databuff2);
    if(st!=0)
	{
		show("Readhex Card Error!");
		return;
    }
	else
	{
		show("Readhex Card Ok!");
		show(databuff2);		
	}	
	st=dc_write_hex(icdev,62,databuff2);
    if(st!=0)
		show("Writehex Card Error!");
    else
	{
		show("Writehex Card Ok!");	
		show(databuff2);
	}	
	st=dc_read(icdev,62,databuff);	
    if(st!=0)
		show("Read Card Error!");
    else
	{
		show("Read Card Ok!");
		memset(databuff2,0,32);
		hex_a(databuff,(unsigned char *)databuff2,16);
		show(databuff2);
	}
	
    st= dc_check_write(icdev,cardsnr,0,62,(unsigned char *)databuff);
	if (st!=0)
	{
		show("dc_check_write error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_check_write ok");
   st= dc_check_writehex(icdev,cardsnr,0,61,(unsigned char *)databuff2);
	if (st!=0)
	{
		show("dc_check_writehex error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_check_writehex ok");
	
    st= dc_changeb3(icdev,15,keya,0,0,0,1,0,keyb);//修改密码下列控制位参数值保证与原密码和控制位一致，一般建议不要改参数3，4，5，6，7
	if (st!=0)
	{
		show("dc_changeb3 error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_changeb3 ok");
	
	value=10000;
	pieceaddr=61;
    st= dc_initval(icdev, pieceaddr, value);
	if (st!=0)
	{
		show("dc_initval error");
	    linstr.Format("%d",st);
	    show(linstr);
		return;
	}
	show("dc_initval ok");

	value=20;
	st= dc_increment( icdev,pieceaddr,value);
	if (st!=0)
	{
		show("dc_increment error");
		return;
	}
	show("dc_increment ok");
	value=200;

    st= dc_decrement( icdev,pieceaddr,value);
	if (st!=0)
	{
		show("dc_decrement error");
		return;
	}
	show("dc_decrement ok");

    st= dc_readval(icdev,pieceaddr,&value);
	if (st!=0)
	{
		show("dc_readval error");
		return;
	}
	show("dc_readval ok");
	linstr.Format("%d",value);
	show(linstr);

return;
}

