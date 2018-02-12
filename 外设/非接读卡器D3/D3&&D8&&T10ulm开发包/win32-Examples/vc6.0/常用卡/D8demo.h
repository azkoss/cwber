// D8demo.h : main header file for the D8DEMO application
//

#if !defined(AFX_D8DEMO_H__B0803F3F_C15C_48A6_B706_0CE436CF2AE2__INCLUDED_)
#define AFX_D8DEMO_H__B0803F3F_C15C_48A6_B706_0CE436CF2AE2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CD8demoApp:
// See D8demo.cpp for the implementation of this class
//

class CD8demoApp : public CWinApp
{
public:
	CD8demoApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CD8demoApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CD8demoApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_D8DEMO_H__B0803F3F_C15C_48A6_B706_0CE436CF2AE2__INCLUDED_)
