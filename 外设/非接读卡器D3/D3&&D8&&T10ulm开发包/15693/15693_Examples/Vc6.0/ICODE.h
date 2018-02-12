// ICODE.h : main header file for the ICODE application
//

#if !defined(AFX_ICODE_H__55AC27EE_DCC7_47A5_8428_175FE0B009C7__INCLUDED_)
#define AFX_ICODE_H__55AC27EE_DCC7_47A5_8428_175FE0B009C7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CICODEApp:
// See ICODE.cpp for the implementation of this class
//

class CICODEApp : public CWinApp
{
public:
	CICODEApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CICODEApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CICODEApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_ICODE_H__55AC27EE_DCC7_47A5_8428_175FE0B009C7__INCLUDED_)
