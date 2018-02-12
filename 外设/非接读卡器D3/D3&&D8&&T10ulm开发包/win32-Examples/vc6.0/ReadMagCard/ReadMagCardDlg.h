// ReadMagCardDlg.h : header file
//

#if !defined(AFX_READMAGCARDDLG_H__FBC75536_D078_45C0_8F66_17E5EE574D26__INCLUDED_)
#define AFX_READMAGCARDDLG_H__FBC75536_D078_45C0_8F66_17E5EE574D26__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CReadMagCardDlg dialog

class CReadMagCardDlg : public CDialog
{
// Construction
public:
	CReadMagCardDlg(CWnd* pParent = NULL);	// standard constructor
	HANDLE icdev;
// Dialog Data
	//{{AFX_DATA(CReadMagCardDlg)
	enum { IDD = IDD_READMAGCARD_DIALOG };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CReadMagCardDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CReadMagCardDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButton2();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_READMAGCARDDLG_H__FBC75536_D078_45C0_8F66_17E5EE574D26__INCLUDED_)
