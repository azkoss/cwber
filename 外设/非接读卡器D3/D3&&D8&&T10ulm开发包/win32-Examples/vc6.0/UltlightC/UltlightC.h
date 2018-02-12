// UltlightC.h : main header file for the ULTLIGHTC application
//

#if !defined(AFX_ULTLIGHTC_H__0C4000BE_B5D6_4164_90A2_80A267913138__INCLUDED_)
#define AFX_ULTLIGHTC_H__0C4000BE_B5D6_4164_90A2_80A267913138__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CUltlightCApp:
// See UltlightC.cpp for the implementation of this class
//

class CUltlightCApp : public CWinApp
{
public:
	CUltlightCApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CUltlightCApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CUltlightCApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_ULTLIGHTC_H__0C4000BE_B5D6_4164_90A2_80A267913138__INCLUDED_)
