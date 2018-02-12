// rfvcDlg.cpp : implementation file
//

#include "stdafx.h"
#include "rfvc.h"
#include "rfvcDlg.h"
#include "dcrf32.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif
unsigned char SCID;
HANDLE icdev;
/////////////////////////////////////////////////////////////////////////////
// CRfvcDlg dialog

CRfvcDlg::CRfvcDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CRfvcDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CRfvcDlg)
	m_command = _T("");
	m_result = _T("");
	m_AB = _T("TYPE IS A");
	m_port = 0;
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CRfvcDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CRfvcDlg)
	DDX_Control(pDX, IDC_LIST1, m_list);
	DDX_Text(pDX, IDC_EDIT1, m_command);
	DDX_Text(pDX, IDC_EDIT2, m_result);
	DDX_Text(pDX, IDC_STATIC_AB, m_AB);
	DDX_Text(pDX, IDC_EDIT_PORT, m_port);
	DDV_MinMaxInt(pDX, m_port, 0, 200);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CRfvcDlg, CDialog)
	//{{AFX_MSG_MAP(CRfvcDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON1, OnButton1)
	ON_BN_CLICKED(IDC_BUTTON3, OnButton3)
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_BUTTON_RESET, OnButtonReset)
	ON_BN_CLICKED(IDC_BUTTON_COMMAND, OnButtonCommand)
	ON_BN_CLICKED(IDC_BUTTON9, OnButton9)
	ON_BN_CLICKED(IDC_BUTTON8, OnButton8)
	ON_BN_CLICKED(IDC_BUTTON11, OnButton11)
	ON_BN_CLICKED(IDC_BUTTON12, OnButton12)
	ON_LBN_DBLCLK(IDC_LIST1, OnDblclkList1)
	ON_BN_CLICKED(IDC_BUTTON2, OnButton2)
	ON_BN_CLICKED(IDC_BUTTON_COMMAND2, OnButtonCommand2)
	ON_BN_CLICKED(IDC_BUTTON4, OnButton4)
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
	m_port=100;
	m_list.AddString("00a40000023f00");
	m_list.AddString("00a40000023f01");
	m_list.AddString("00a40000020002");
	m_list.AddString("00B0000064");
	m_list.AddString("00d60000640102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f6061626364");
	m_list.AddString("00d600006488888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888");
    UpdateData(FALSE);

    

    


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

void CRfvcDlg::OnButton1() 
{
	// TODO: Add your control notification handler code here
	UpdateData(TRUE);

	icdev=dc_init(m_port,115200);  //100±íÊ¾USB¿Ú
	if((long)icdev<=0)
	   AfxMessageBox("Init Com Error!");
	else
      AfxMessageBox("Init Com OK!");
   
	dc_beep(icdev,10);
	return;

}



void CRfvcDlg::OnButton3() 
{
	// TODO: Add your control notification handler code here
	int st;
	unsigned long cardsnr;
	unsigned char sss[5];
	unsigned __int16 ttt;
	st=dc_request(icdev,1,&ttt);
    if(st!=0)
	{
   	   AfxMessageBox("request Card Error!");
	   return;
    }
	st=dc_anticoll(icdev,0,&cardsnr);
	if(st!=0)
	{
   	   AfxMessageBox("anticoll Error!");
	   return;
    }
	st=dc_select(icdev,cardsnr,sss);
	if(st!=0)
	{
   	   AfxMessageBox("dc_select Error!");
	   return;
    }
	AfxMessageBox("ok");
}




void CRfvcDlg::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	dc_exit(icdev);
	CDialog::OnClose();
}


void CRfvcDlg::OnButtonReset() 
{
	// TODO: Add your control notification handler code here
	unsigned char crlen[2],recbuff[300];
	int st;
	st=dc_pro_resethex(icdev,crlen,(char *)recbuff);
    if(st!=0)
	{
   	   AfxMessageBox("dc_pro_reset Card Error!");
	   return;
	}
	m_result=recbuff;
	UpdateData(false);
	return;

}


void CRfvcDlg::OnButtonCommand() 
{
	// TODO: Add your control notification handler code here
	unsigned char crlen[2],recbuff[1024];
    unsigned char tempbuff[1024],sendbuff[1024];
	int st;
	int clen;
	UpdateData(TRUE);
	clen=m_command.GetLength();
	memcpy(tempbuff,m_command,clen);
	a_hex(tempbuff,&sendbuff[0],clen);
	clen=clen/2;
	st=dc_pro_commandlink(icdev,clen,sendbuff,crlen,recbuff,7,40);
    if(st!=0)
	{
   	   sprintf((char *)tempbuff,"dc_pro_command Card Error code=%d",st);
	   AfxMessageBox((char *)tempbuff);
       return;
	}
    hex_a(recbuff,tempbuff,crlen[0]);
	tempbuff[crlen[0]*2]=0;
	m_result=tempbuff;
	UpdateData(false);
	return;	
}


void CRfvcDlg::OnButton9() 
{
	// TODO: Add your control notification handler code here
	int st;
	UpdateData(TRUE);

	if(m_AB=="TYPE IS B")
	{
	    st=dc_config_card(icdev,'A');
		m_AB="TYPE IS A";
	} 
	else
	{
		st=dc_config_card(icdev,'B');
		m_AB="TYPE IS B";

	}
    if(st!=0)
	{
   	   AfxMessageBox("dc_config_card Card Error!");
       return;
	}
    UpdateData(false);
   	
	AfxMessageBox("dc_config_card Card OK!");
	return;
	
}

void CRfvcDlg::OnButton8() 
{
    int st;
    unsigned char tempbuff[1024],recbuff[1024];
   	UpdateData(TRUE);
   st=dc_request_b(icdev,0,0,0,recbuff);  //5000059D4A000000000000410000
                                          //5000059D4A00000000000041
   if(st!=0)
   {
   	   AfxMessageBox("dc_request_b Card Error!");
       return;
   }  
    hex_a(recbuff,tempbuff,14);
	tempbuff[14*2]=0;
	m_result=tempbuff;   	
    UpdateData(false);
	return;
}


void CRfvcDlg::OnButton11() 
{
	// TODO: Add your control notification handler code here
    int st,clen;
    unsigned char tempbuff[300],recbuff[300];
	UpdateData(TRUE);
	clen=m_result.GetLength();
	if(clen==0)
	{
		AfxMessageBox("please find type b card first!");
		return;	
	}
	memcpy(tempbuff,m_result,clen);
	a_hex(tempbuff,recbuff,clen);
	//50211D040C0F435AFF006185
    st=dc_attrib(icdev,&recbuff[1],0);     //5000059D4A000000000000410000
	//5000059D4A00000000000041
	if(st<0)
	{
		AfxMessageBox("dc_attrib Card Error!");
		return;
	}  
	UpdateData(FALSE);
    AfxMessageBox("dc_attrib Card ok!");
	
	return;		
}

void CRfvcDlg::OnButton12() 
{
	// TODO: Add your control notification handler code here
	UpdateData(TRUE);
	if(m_command.GetLength()!=0)
	   m_list.AddString(m_command);

	
}

void CRfvcDlg::OnDblclkList1() 
{
	// TODO: Add your control notification handler code here
    int msel;
    UpdateData(TRUE);
	msel=m_list.GetCurSel();
	m_list.GetText(msel,m_command);
	UpdateData(FALSE);


}

void CRfvcDlg::OnButton2() 
{
	// TODO: Add your control notification handler code here
	int st;
	st=dc_reset(icdev,1);
	if(st<0)
	{
   	   AfxMessageBox("dc_reset Error!");
       return;	
	}
    AfxMessageBox("dc_reset ok!");


}

void CRfvcDlg::OnButtonCommand2() 
{
	// TODO: Add your control notification handler code here
	unsigned char crlen[2],recbuff[1024];
    unsigned char tempbuff[1024],sendbuff[1024];
	int st;
	int clen;
	UpdateData(TRUE);
	clen=m_command.GetLength();
	memcpy(tempbuff,m_command,clen);
	a_hex(tempbuff,&sendbuff[0],clen);
	clen=clen/2;
	st=dc_pro_commandsource(icdev,clen,sendbuff,crlen,recbuff,7);
	if(st!=0)
	{
		sprintf((char *)tempbuff,"dc_pro_command Card Error code=%d",st);
		AfxMessageBox((char *)tempbuff);
		return;
	}
	hex_a(recbuff,tempbuff,crlen[0]);
	tempbuff[crlen[0]*2]=0;
	m_result=tempbuff;
	UpdateData(false);
	return;		
}

void CRfvcDlg::OnButton4() 
{
	// TODO: Add your control notification handler code here
	int st;
	unsigned long cardsnr;
	unsigned char sss[5];
	unsigned __int16 ttt;
	st=dc_request(icdev,0,&ttt);
    if(st!=0)
	{
		AfxMessageBox("request Card Error!");
		return;
    }	
}
