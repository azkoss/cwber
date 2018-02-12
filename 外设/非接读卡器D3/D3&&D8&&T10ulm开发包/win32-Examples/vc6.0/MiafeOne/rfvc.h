// rfvc.h : main header file for the RFVC application
//

#if !defined(AFX_RFVC_H__3A6CB467_C07B_11D4_B20D_0080AD85208E__INCLUDED_)
#define AFX_RFVC_H__3A6CB467_C07B_11D4_B20D_0080AD85208E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CRfvcApp:
// See rfvc.cpp for the implementation of this class
//

class CRfvcApp : public CWinApp
{
public:
	CRfvcApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CRfvcApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CRfvcApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_RFVC_H__3A6CB467_C07B_11D4_B20D_0080AD85208E__INCLUDED_)
