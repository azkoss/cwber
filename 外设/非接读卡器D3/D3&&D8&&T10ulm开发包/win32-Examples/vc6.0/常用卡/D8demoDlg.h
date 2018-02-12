// D8demoDlg.h : header file
//

#if !defined(AFX_D8DEMODLG_H__DF2E4ACB_B48A_43B6_8E12_E6D1CCBBDAB3__INCLUDED_)
#define AFX_D8DEMODLG_H__DF2E4ACB_B48A_43B6_8E12_E6D1CCBBDAB3__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CD8demoDlg dialog

class CD8demoDlg : public CDialog
{
// Construction
public:
	int mtest(char *sbuff,int slen);
	CD8demoDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	//{{AFX_DATA(CD8demoDlg)
	enum { IDD = IDD_D8DEMO_DIALOG };
	CListBox	m_list1;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CD8demoDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CD8demoDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnButton1();
	afx_msg void OnButton2();
	afx_msg void OnButton3();
	afx_msg void OnButton4();
	afx_msg void OnButton5();
	afx_msg void OnButton6();
	afx_msg void OnButton7();
	afx_msg void OnButton8();
//	afx_msg void OnButton9();
//	afx_msg void OnButton10();
	afx_msg void OnButton11();
	afx_msg void OnButton12();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
		HANDLE icdev ;
int st;
};



//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_D8DEMODLG_H__DF2E4ACB_B48A_43B6_8E12_E6D1CCBBDAB3__INCLUDED_)
