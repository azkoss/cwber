// UltlightCDlg.h : header file
//

#if !defined(AFX_ULTLIGHTCDLG_H__9D02B6B7_51E3_452D_9C58_C4D3907C92E8__INCLUDED_)
#define AFX_ULTLIGHTCDLG_H__9D02B6B7_51E3_452D_9C58_C4D3907C92E8__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CUltlightCDlg dialog

class CUltlightCDlg : public CDialog
{
// Construction
public:
	HANDLE icdev;
	CUltlightCDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	//{{AFX_DATA(CUltlightCDlg)
	enum { IDD = IDD_ULTLIGHTC_DIALOG };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CUltlightCDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CUltlightCDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButton1();
	afx_msg void OnButton3();
	afx_msg void OnButton2();
	afx_msg void OnButton4();
	afx_msg void OnButton5();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_ULTLIGHTCDLG_H__9D02B6B7_51E3_452D_9C58_C4D3907C92E8__INCLUDED_)
