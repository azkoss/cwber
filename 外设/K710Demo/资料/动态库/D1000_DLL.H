#ifndef __D1000_DLL
#define __D1000_DLL
#include <windows.h>
/************************************************************************
*	文件功能：D1000动态库程序
*	动态库版本：V2.4
*	Author: rgh		Date: 12-10-18
************************************************************************/
/////////////////////////////////////////////////////////////////
//////////////////// DLL内部错误索引/////////////////////////////
//CommOpen(101-104)
#define Bad_CommOpen -101//端口打开错.
#define Bad_CommSet -102//端口参数设置错,(102)还代表CommSet函数错

#define Bad_CommTimeouts -999
#define Bad_CommQueue -998
//CommClose(105)
#define Bad_CommClose -105//端口关闭错.

//Send(106)
#define Bad_SendByte -106
#define Bad_SendData_Len -107

//Receive(106-107)
#define Bad_ReceiveData_ReadFile -103
#define Bad_ReceiveData_Len -104

// 公共错误
#define Bad_Parameter -200//传入参数错误(超越定义)

#define OK		0
#define ERR		-1
#define ACK		0x06
#define ENQ		0x05


//----函数声明--------------------------------------
int __stdcall CommTimeouts(HANDLE ComHandle,
				 DWORD ReadIntervalTimeout,
				 DWORD ReadTotalTimeoutMultiplier,
				 DWORD ReadTotalTimeoutConstant,
				 DWORD WriteTotalTimeoutMultiplier,
				 DWORD WriteTotalTimeoutConstant);
int __stdcall CommQueue(HANDLE ComHandle,DWORD InQueue,DWORD OutQueue);
int __stdcall SendData(HANDLE ComHandle,BYTE *DataBuf, DWORD Number);
int __stdcall ReceiveData(HANDLE ComHandle,BYTE *ReceData, DWORD Number);
int __stdcall InQueCount(HANDLE ComHandle);
int __stdcall OutQueCount(HANDLE ComHandle);
int __stdcall PurgeIn(HANDLE ComHandle);
int __stdcall PurgeOut(HANDLE ComHandle);
int __stdcall CommSetting(HANDLE ComHandle,char *ComSeting);
BYTE CalXOR(unsigned char strOrder[],unsigned int Len);
int __stdcall D1000_AckEnq(HANDLE ComHandle, unsigned char MacAddr);

//--------导出的函数-----------------------------------------------
HANDLE __stdcall D1000_CommOpen(int Port);
HANDLE __stdcall D1000_CommOpenWithBaud(int Port, unsigned int _data);
int __stdcall D1000_CommClose(HANDLE ComHandle);
int __stdcall D1000_GetDllVersion(HANDLE ComHandle, char *strVersion);
int __stdcall D1000_GetSysVersion(HANDLE ComHandle, unsigned char MacAddr, char *strVersion);
int __stdcall D1000_SendCmd(HANDLE ComHandle, unsigned char MacAddr, char *p_Cmd, int CmdLen);
int __stdcall D1000_Query(HANDLE ComHandle, unsigned char MacAddr, BYTE StateInfo[3]);
int __stdcall D1000_SensorQuery(HANDLE ComHandle, unsigned char MacAddr, BYTE StateInfo[4]);
int __stdcall D1000_AutoTestMac(HANDLE ComHandle, unsigned char MacAddr);
int __stdcall D1000_ReadRecycleCardNum(HANDLE ComHandle, unsigned char MacAddr, BYTE szData[4]);
int __stdcall D1000_SetCardNum(HANDLE ComHandle, unsigned char MacAddr, int nNum);
int __stdcall D1000_DisEnableCount(HANDLE ComHandle, unsigned char MacAddr);
int __stdcall D1000_EnableCount(HANDLE ComHandle, unsigned char MacAddr);
int __stdcall D1000_ReadCardNumber(HANDLE ComHandle, unsigned char MacAddr, BYTE CardNumber[50]);

#endif