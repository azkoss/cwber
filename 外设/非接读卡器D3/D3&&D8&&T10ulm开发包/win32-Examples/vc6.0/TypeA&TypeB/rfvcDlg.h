// rfvcDlg.h : header file
//

#if !defined(AFX_RFVCDLG_H__3A6CB469_C07B_11D4_B20D_0080AD85208E__INCLUDED_)
#define AFX_RFVCDLG_H__3A6CB469_C07B_11D4_B20D_0080AD85208E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CRfvcDlg dialog

class CRfvcDlg : public CDialog
{
// Construction
public:
	CRfvcDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	//{{AFX_DATA(CRfvcDlg)
	enum { IDD = IDD_RFVC_DIALOG };
	CListBox	m_list;
	CString	m_command;
	CString	m_result;
	CString	m_AB;
	int		m_port;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CRfvcDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CRfvcDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButton1();
	afx_msg void OnButton3();
	afx_msg void OnClose();
	afx_msg void OnButtonReset();
	afx_msg void OnButtonCommand();
	afx_msg void OnButton9();
	afx_msg void OnButton8();
	afx_msg void OnButton11();
	afx_msg void OnButton12();
	afx_msg void OnDblclkList1();
	afx_msg void OnButton2();
	afx_msg void OnButtonCommand2();
	afx_msg void OnButton4();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_RFVCDLG_H__3A6CB469_C07B_11D4_B20D_0080AD85208E__INCLUDED_)
