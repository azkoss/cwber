// ICODEDlg.h : header file
//

#if !defined(AFX_ICODEDLG_H__EA4D59ED_4960_43BE_BDA7_162CB3C7FACA__INCLUDED_)
#define AFX_ICODEDLG_H__EA4D59ED_4960_43BE_BDA7_162CB3C7FACA__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CICODEDlg dialog

class CICODEDlg : public CDialog
{
// Construction
public:
	int Str2Hex(CString str,char *data);
	CICODEDlg(CWnd* pParent = NULL);	// standard constructor
	
// Dialog Data
	//{{AFX_DATA(CICODEDlg)
	enum { IDD = IDD_ICODE_DIALOG };
	CListBox	m_list1;
	int		m_StaAddr;
	int		m_Blockno;
	CString	m_write;
	int		m_port;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CICODEDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;
	HANDLE icdev;
	// Generated message map functions
	//{{AFX_MSG(CICODEDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnINIT();
	afx_msg void OnFindSingle();
	afx_msg void OnFindMulti();
	afx_msg void OnRead();
	afx_msg void OnWrite();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_ICODEDLG_H__EA4D59ED_4960_43BE_BDA7_162CB3C7FACA__INCLUDED_)
