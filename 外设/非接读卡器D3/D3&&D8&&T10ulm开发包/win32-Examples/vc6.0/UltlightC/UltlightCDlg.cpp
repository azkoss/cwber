// UltlightCDlg.cpp : implementation file
//

#include "stdafx.h"
#include "UltlightC.h"
#include "UltlightCDlg.h"
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
// CUltlightCDlg dialog

CUltlightCDlg::CUltlightCDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CUltlightCDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CUltlightCDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CUltlightCDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CUltlightCDlg)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CUltlightCDlg, CDialog)
	//{{AFX_MSG_MAP(CUltlightCDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON1, OnButton1)
	ON_BN_CLICKED(IDC_BUTTON3, OnButton3)
	ON_BN_CLICKED(IDC_BUTTON2, OnButton2)
	ON_BN_CLICKED(IDC_BUTTON4, OnButton4)
	ON_BN_CLICKED(IDC_BUTTON5, OnButton5)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CUltlightCDlg message handlers

BOOL CUltlightCDlg::OnInitDialog()
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
	SetDlgItemText(IDC_EDIT1,"49454D4B41455242214E4143554F5946");
	((CComboBox *)GetDlgItem(IDC_COMBO1))->AddString("usb");
	((CComboBox *)GetDlgItem(IDC_COMBO1))->AddString("com1");
	((CComboBox *)GetDlgItem(IDC_COMBO1))->AddString("com2");
	((CComboBox *)GetDlgItem(IDC_COMBO1))->SetCurSel(0);
	((CComboBox *)GetDlgItem(IDC_COMBO2))->AddString("115200");
	((CComboBox *)GetDlgItem(IDC_COMBO2))->AddString("9600");
	((CComboBox *)GetDlgItem(IDC_COMBO2))->SetCurSel(0);
	SetDlgItemText(IDC_EDIT2,"4");
	SetDlgItemText(IDC_EDIT4,"11223344");
	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CUltlightCDlg::OnSysCommand(UINT nID, LPARAM lParam)
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

void CUltlightCDlg::OnPaint() 
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
HCURSOR CUltlightCDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CUltlightCDlg::OnButton1() 
{
	// TODO: Add your control notification handler code here
	int st = -1;
	unsigned char snr[50] = "\0";
	st = dc_reset(icdev,10);
	st = dc_card_double(icdev,0,snr);
	if(st)
	{
		MessageBox("寻卡失败");
		return;
	}
	MessageBox("寻卡成功");
    return;
}

void CUltlightCDlg::OnButton3() 
{
	// TODO: Add your control notification handler code here
	
	__int16 port = -1;
	long baud = -1;
	int isel = ((CComboBox *)GetDlgItem(IDC_COMBO1))->GetCurSel();
	if(!isel)
	{
		port = 100;
	}
	else
	{
		port = isel - 1;
	}
	baud = GetDlgItemInt(IDC_COMBO2);
	icdev = dc_init(port,baud);
	if((int)icdev < 0)
	{
		MessageBox("打开端口失败");
		return;
	}
	MessageBox("打开端口成功");
	return;
}

void CUltlightCDlg::OnButton2() 
{
	// TODO: Add your control notification handler code here
	unsigned char key[33] = _T("");
	int nRet(0);
	CString m_Key;
	GetDlgItemText(IDC_EDIT1,m_Key);
	memcpy(key, m_Key, 32);
	nRet = dc_auth_ulc_hex(icdev, key);
	if (nRet)
	{
		AfxMessageBox("验证失败");
		return;
	}
	AfxMessageBox("验证成功");
	return;
}

void CUltlightCDlg::OnButton4() 
{
	// TODO: Add your control notification handler code here
	int nPage(0);
	int nRet(0);
	char data[33] = "\0";
	CString strData = "";
	nPage = GetDlgItemInt(IDC_EDIT2);
	nRet = dc_read_hex(icdev, nPage, data);	
	if (nRet)
	{
		strData.Format("读卡失败(page = %d)", nPage);
		AfxMessageBox(strData);
		return;
	}
	data[8] = '\0';
	SetDlgItemText(IDC_EDIT3,data);
}

void CUltlightCDlg::OnButton5() 
{
	// TODO: Add your control notification handler code here
	int m_nIndex(0);
	unsigned char pageData[33] = "";
	int nRet(0);
	CString strTip = _T("");
	CString m_pageData;
	m_nIndex = GetDlgItemInt(IDC_EDIT2);
	GetDlgItemText(IDC_EDIT4,m_pageData);
	if (m_pageData.GetLength() != 8)
	{
		AfxMessageBox("写入长度必须为8字符");
		return;
	}
	memset(pageData,0x30,32);
	memcpy(pageData, m_pageData, 8);
	nRet = dc_write_hex(icdev, m_nIndex, (char *)pageData);
	if (nRet)
	{
		strTip.Format("写卡失败(page = %d)", m_nIndex);
		AfxMessageBox(strTip);
		return;
	}
	AfxMessageBox("写卡成功");
	return;
}
