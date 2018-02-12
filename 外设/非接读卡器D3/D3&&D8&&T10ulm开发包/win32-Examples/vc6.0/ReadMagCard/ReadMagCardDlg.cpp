// ReadMagCardDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ReadMagCard.h"
#include "ReadMagCardDlg.h"
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
// CReadMagCardDlg dialog

CReadMagCardDlg::CReadMagCardDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CReadMagCardDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CReadMagCardDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CReadMagCardDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CReadMagCardDlg)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CReadMagCardDlg, CDialog)
	//{{AFX_MSG_MAP(CReadMagCardDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
//	ON_BN_CLICKED(IDC_BUTTON1, OnButton1)
	ON_BN_CLICKED(IDC_BUTTON2, OnButton2)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CReadMagCardDlg message handlers

BOOL CReadMagCardDlg::OnInitDialog()
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
	((CButton *)GetDlgItem(IDC_RADIO3))->SetCheck(1);
	((CComboBox *)GetDlgItem(IDC_COMBO1))->AddString("9600");
	((CComboBox *)GetDlgItem(IDC_COMBO1))->AddString("115200");
	((CComboBox *)GetDlgItem(IDC_COMBO1))->SetCurSel(0);
	SetDlgItemText(IDC_EDIT4,"100");
	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CReadMagCardDlg::OnSysCommand(UINT nID, LPARAM lParam)
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

void CReadMagCardDlg::OnPaint() 
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
HCURSOR CReadMagCardDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

// void CReadMagCardDlg::OnButton1() 
// {
// 	// TODO: Add your control notification handler code here
// 	int st = -1;
// 	short port;
// 	long baud;
// 	if(((CButton *)GetDlgItem(IDC_RADIO1))->GetCheck())
// 	{
// 		port = 0;
// 	}
// 	if(((CButton *)GetDlgItem(IDC_RADIO2))->GetCheck())
// 	{
// 		port = 1;
// 	}
// 	if(((CButton *)GetDlgItem(IDC_RADIO3))->GetCheck())
// 	{
// 		port = 100;
// 	}
// 	int isel = ((CComboBox*)GetDlgItem(IDC_COMBO1))->GetCurSel();
// 	if(!isel)
// 		baud = 9600;
// 	else if(isel)
// 		baud = 115200;
// 	icdev = dc_init(port,baud);
// 	if(icdev < 0)
// 	{
// 		MessageBox("打开端口错误");
// 		return;
// 	}
// 	unsigned char ctimeout = 100;
// 	unsigned char pTrack2Data[100]="\0";
// 	unsigned long pTrack2Len = 0;
// 	unsigned char pTrack3Data[100]="\0";
// 	unsigned long pTrack3Len = 0;
// 	st = dc_readmagcard(icdev,ctimeout,pTrack2Data,&pTrack2Len,pTrack3Data,&pTrack3Len);
// 	if(st)
// 	{
// 		MessageBox("获取磁条卡信息错误");
// 		return;
// 	}
// 	SetDlgItemText(IDC_EDIT2,(const char *)pTrack2Data);
// 	SetDlgItemText(IDC_EDIT3,(const char *)pTrack3Data);
// 	dc_beep(icdev,10);
// 	dc_exit(icdev);
// }

void CReadMagCardDlg::OnButton2() 
{
	// TODO: Add your control notification handler code here
	int st = -1;
	short port;
	long baud;
	if(((CButton *)GetDlgItem(IDC_RADIO1))->GetCheck())
	{
		port = 0;
	}
	if(((CButton *)GetDlgItem(IDC_RADIO2))->GetCheck())
	{
		port = 1;
	}
	if(((CButton *)GetDlgItem(IDC_RADIO3))->GetCheck())
	{
		port = 100;
	}
	int isel = ((CComboBox*)GetDlgItem(IDC_COMBO1))->GetCurSel();
	if(!isel)
		baud = 9600;
	else if(isel)
		baud = 115200;
	icdev = dc_init(port,baud);
	if(icdev < 0)
	{
		MessageBox("打开端口错误");
		return;
	}
	unsigned char ctimeout = 0;
    unsigned char pTrack1Data[100]="\0"; 
	unsigned long pTrack1Len = 0;
	unsigned char pTrack2Data[100]="\0";
	unsigned long pTrack2Len = 0;
	unsigned char pTrack3Data[100]="\0";
	unsigned long pTrack3Len = 0;
	BOOL b;
	ctimeout = GetDlgItemInt(IDC_EDIT4,&b,FALSE);
    st = dc_readmagcardall(icdev,ctimeout,pTrack1Data,&pTrack1Len,pTrack2Data,&pTrack2Len,pTrack3Data,&pTrack3Len);
	if(st)
	{
		MessageBox("获取磁条卡信息错误");
		return;
	}
	SetDlgItemText(IDC_EDIT1,(const char *)pTrack1Data);
	SetDlgItemText(IDC_EDIT2,(const char *)pTrack2Data);
	SetDlgItemText(IDC_EDIT3,(const char *)pTrack3Data);
	dc_beep(icdev,10);
	dc_exit(icdev);
}
