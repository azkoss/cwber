// ICODEDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ICODE.h"
#include "ICODEDlg.h"
#include "dcrf32.h"
#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	//{{AFX_DATA(CAboutDlg)
	enum { IDD = IDD_ABOUTBOX };
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAboutDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	//{{AFX_MSG(CAboutDlg)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
		// No message handlers
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CICODEDlg dialog

CICODEDlg::CICODEDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CICODEDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CICODEDlg)
	m_StaAddr = 0;
	m_Blockno = 0;
	m_write = _T("");
	m_port = 0;
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CICODEDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CICODEDlg)
	DDX_Control(pDX, IDC_LIST1, m_list1);
	DDX_Text(pDX, IDC_EDIT1, m_StaAddr);
	DDV_MinMaxInt(pDX, m_StaAddr, 0, 27);
	DDX_Text(pDX, IDC_EDIT2, m_Blockno);
	DDV_MinMaxInt(pDX, m_Blockno, 1, 10);
	DDX_Text(pDX, IDC_EDIT3, m_write);
	DDV_MaxChars(pDX, m_write, 8);
	DDX_Text(pDX, IDC_EDIT4, m_port);
	DDV_MinMaxInt(pDX, m_port, 0, 100);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CICODEDlg, CDialog)
	//{{AFX_MSG_MAP(CICODEDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON1, OnINIT)
	ON_BN_CLICKED(IDC_FIND_SINGLE, OnFindSingle)
	ON_BN_CLICKED(IDC_FIND_MULTI, OnFindMulti)
	ON_BN_CLICKED(IDC_READ, OnRead)
	ON_BN_CLICKED(IDC_WRITE, OnWrite)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CICODEDlg message handlers

BOOL CICODEDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here
	m_Blockno=1;
	UpdateData(false);
	icdev=0;
	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CICODEDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CICODEDlg::OnPaint() 
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
HCURSOR CICODEDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CICODEDlg::OnINIT() 
{
	// TODO: Add your control notification handler code here
	UpdateData(true);
	icdev=dc_init(m_port,115200);   //init port,baud rate is 115200MHZ
	if((int)icdev<=0)
	{
		AfxMessageBox("Init port ERROR!");
		return;
	}
	else 
	{
		AfxMessageBox("Init port OK!");
	}
	int st=dc_config_card(icdev,'1');  //find 15693 card
}

void CICODEDlg::OnFindSingle() 
{
	// TODO: Add your control notification handler code here
	UpdateData(true);
	int st;
	unsigned char rlen[17]={0};
	unsigned char rbuffer[256];
	st= dc_inventory_hex(icdev,0x36,0,0,rlen,rbuffer);        //find single card
	if(st) 
	{
		AfxMessageBox("Find single card ERROR!");
		return;
	}	
	m_list1.AddString((char *)rbuffer);

		

}

void CICODEDlg::OnFindMulti() 
{
	// TODO: Add your control notification handler code here
	int st;
	unsigned char rlen[256];
	unsigned char rbuffer[256];
	st= dc_inventory_hex(icdev,0x16,0,0,rlen,rbuffer);        //find multi card
	if(st)
	{
		AfxMessageBox("Find multi card ERROR!");
		return;
	}
	m_list1.AddString((char *)rbuffer);
}

void CICODEDlg::OnRead() 
{
	int st;
	UpdateData(true);
	unsigned char UID[256];
	unsigned char rlen[256];
	unsigned char rbuffer[256];
	st=dc_inventory(icdev,0x36,0,0,rlen,UID);
	if(st)
	{
		AfxMessageBox("ERROR1!");
		return;
	}
	st=dc_readblock(icdev,0x22,m_StaAddr,m_Blockno,&UID[1],rlen,rbuffer);  //read block data
	if(st)
	{
		AfxMessageBox("Read data ERROR!");
		return;
	}
	unsigned char tmp[256];
	for(int i=0;i<m_Blockno;i++)
	{
		if(m_StaAddr+m_Blockno>28)
		{
			m_Blockno=m_Blockno-((m_StaAddr+m_Blockno)-28);
		}
		sprintf((char *)tmp,"BlockAddr:[%2d] Data:[%02X %02X %02X %02X]",m_StaAddr+i,rbuffer[i*4],rbuffer[i*4+1],rbuffer[i*4+2],rbuffer[i*4+3]);
		m_list1.AddString((const char *)tmp);
	}
}

void CICODEDlg::OnWrite() 
{
	// TODO: Add your control notification handler code here
	int st;
	unsigned char rlen[256];
	UpdateData(true);
	unsigned char UID[256];
	BYTE len=10;
	unsigned char WriteBuf[256];
	len=Str2Hex(m_write,(char *)WriteBuf);
	if(len==0)
	{
		AfxMessageBox("Write Block¡¡ERROR!");
		return;
	}
	st=dc_inventory(icdev,0x36,0,0,rlen,UID);
	if(st)
	{
		AfxMessageBox("ERROR1!");
		return;
	}
	st=dc_writeblock(icdev,0x22,m_StaAddr,1,&UID[1],4,WriteBuf);
	/*unsigned char data[16];
	CString Str;
	GetDlgItem(IDC_EDIT3)->GetWindowText(Str);
	memcpy(data,Str,Str.GetLength());
	st=dc_writeblock_hex(icdev,0x22,m_StaAddr,1,&UID[3],4,data);  //write block 1*/	
	if(st)
	{
		AfxMessageBox("Write data ERROR!");
		return;
	}
	else
	{
		AfxMessageBox("Write data OK!");
	}


	
}
HexChar(char c)
{
	if((c>='0')&&(c<='9'))
		return c-0x30;
	else if((c>='A')&&(c<='F'))
		return c-'A'+10;
	else if((c>='a')&&(c<='f'))
		return c-'a'+10;
	else 
		return 0x10;
}

int CICODEDlg::Str2Hex(CString str, char *data)
{	int t,t1;
	int rlen=0,len=str.GetLength();
	//data.SetSize(len/2);
	for(int i=0;i<len;)
	{
		char l,h=str[i];
		if(h==' ')
		{
			i++;
			continue;
		}
		i++;
		if(i>=len)
			break;
		l=str[i];
		t=HexChar(h);
		t1=HexChar(l);
		if((t==16)||(t1==16))
			break;
		else 
			t=t*16+t1;
		i++;
		data[rlen]=(char)t;
		rlen++;
	}
	return rlen;


}
