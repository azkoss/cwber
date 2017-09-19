//************************************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//**  文件开始(头文件)****************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//**防止头文件多重定义****************************************************************************************************************************************/
#ifndef __SDK_H__
#define __SDK_H__
//************************************************************************************************************************************************************/
//**定义DLL函数前缀*******************************************************************************************************************************************/
#ifdef DLL_API 
#else
	#define DLL_API extern "C" __declspec(dllimport)
#endif
//************************************************************************************************************************************************************/
//**常量定义**************************************************************************************************************************************************/
#define C_VERSION							109				// SDK版本号（3位数字）
// 常量定义-端口类型
#define C_PRINTER_PORT_COM					0				// 打印机端口类型 串口
#define C_PRINTER_PORT_LPT					1				// 打印机端口类型 并口
#define C_PRINTER_PORT_USB					2				// 打印机端口类型 USB
#define C_PRINTER_PORT_NET					3				// (预留)打印机端口类型 网口
#define C_PRINTER_PORT_VLPT					4				// 打印机端口类型 虚拟并口
// 常量定义-裁纸方式
#define C_PRINTER_CUT_FULL					0				// 全切
#define C_PRINTER_CUT_PART					1				// 半切
//  常量定义-字符编码
#define C_PRINTER_ENCODING_ASCII			0				// ASCII编码
#define C_PRINTER_ENCODING_GB2312			1				// GB2312编码
#define C_PRINTER_ENCODING_GB18030			2				// GB18030编码
// 常量定义-点阵大小
#define C_PRINTER_FONT_24					0				// 24点阵汉字
#define C_PRINTER_FONT_16					1				// 16点阵汉字
// 常量定义-对齐方式
#define C_PRINTER_ALIGN_LEFT				0				// 居左对齐
#define C_PRINTER_ALIGN_CENTER				1				// 居中对齐
#define C_PRINTER_ALIGN_RIGHT				2				// 居右对齐
//  常量定义-黑标参数
#define C_PRINTER_BM_MOVE					1				// 起始位置的设定值
#define C_PRINTER_BM_CUT					2				// 裁纸位置的设定值
#define C_PRINTER_BM_FORWARD				0				// 进纸的方向
#define C_PRINTER_BM_BACK					1				// 倒纸的方向
//  常量定义-BMP位图模式
#define C_PRINTER_BMP_8						0				// 8点单密度
#define C_PRINTER_BMP_8D					1				// 8点双密度
#define C_PRINTER_BMP_24					32				// 24点单密度
#define C_PRINTER_BMP_24D					33				// 24点双密度
// 一维条形码类型常量
#define C_PRINTER_BARCODE_EAN13				2				// EAN13条形码
#define C_PRINTER_BARCODE_CODE39			69				// CODE39条形码
#define C_PRINTER_BARCODE_CODE128			73				// CODE128条形码
// 一维条形码人工识别字符位置常量
#define C_PRINTER_HRI_NONE					0				// 不打印HRI字符
#define C_PRINTER_HRI_TOP					1				// 在条形码上方打印HRI字符
#define C_PRINTER_HRI_BOTTOM				2				// 在条形码下方打印HRI字符
#define C_PRINTER_HRI_BOTH					3				// 在条形码上方和下方打印HRI字符
// 打印机状态类型常量
#define C_PRINTER_STATUS_ONLINE				1				// 在线离线状态
#define C_PRINTER_STATUS_DOOR				2				// 纸仓门状态
#define C_PRINTER_STATUS_CUT				3				// 裁纸器状态
#define C_PRINTER_STATUS_PAPER				4				// 纸卷状态
#define C_PRINTER_STATUS_HEAT				5				// (预留)发热片状态
#define C_PRINTER_STATUS_BUFFER				6				// (预留)缓存区状态
#define C_PRINTER_STATUS_DRAWER				7				// (预留)钱箱状态
#define C_PRINTER_STATUS_JAM				8				// 卡纸状态
//************************************************************************************************************************************************************/
//**函数声明**************************************************************************************************************************************************/
DLL_API	HANDLE _stdcall Printer_Port_Open(LPCSTR strPortName,LONG lBaudRate, LONG lPortType);
DLL_API BOOL _stdcall Printer_Port_Close(HANDLE hPrinterID);
DLL_API BOOL _stdcall Printer_Control_CutPaper(HANDLE hPrinterID, BYTE iType, BYTE iLines);
DLL_API BOOL _stdcall Printer_Control_CashDraw(HANDLE hPrinterID, BYTE iNum, BYTE iTimeOn, BYTE iTimeOff);
DLL_API BOOL _stdcall Printer_Control_BlackMark(HANDLE hPrinterID);
DLL_API BOOL _stdcall Printer_Control_FeedLines(HANDLE hPrinterID, BYTE iDotLines);
DLL_API BOOL _stdcall Printer_Control_BackLines(HANDLE hPrinterID, BYTE iDotLines);
DLL_API BOOL _stdcall Printer_Set_Reset(HANDLE hPrinterID);
DLL_API BOOL _stdcall Printer_Set_Encoding(HANDLE hPrinterID, LONG lEncoding);
DLL_API BOOL _stdcall Printer_Set_Font(HANDLE hPrinterID, LONG iFont, BOOL bBold, BOOL bDoubleWidth, BOOL bDoubleHeight, BOOL bUnderLine);
DLL_API BOOL _stdcall Printer_Set_FontSize(HANDLE hPrinterID, BYTE iWidth, BYTE iHeight);
DLL_API BOOL _stdcall Printer_Set_OppositeColor(HANDLE hPrinterID, BOOL bOppsite);
DLL_API BOOL _stdcall Printer_Set_FontBold(HANDLE hPrinterID, BOOL bBold);
DLL_API BOOL _stdcall Printer_Set_AlignType(HANDLE hPrinterID, LONG lAlignType);
DLL_API BOOL _stdcall Printer_Set_LeftMargin(HANDLE hPrinterID, BYTE iLeftMargin);
DLL_API BOOL _stdcall Printer_Set_RightMargin(HANDLE hPrinterID, BYTE iRightMargin);
DLL_API BOOL _stdcall Printer_Set_LineSpace(HANDLE hPrinterID, BYTE iSpace);
DLL_API BOOL _stdcall Printer_Set_BlackMarkValue(HANDLE hPrinterID, BYTE iType, BYTE iDir, LONG lOffset);
DLL_API BOOL _stdcall Printer_Set_BmpInFlash(HANDLE hPrinterID, BYTE iNumber, LPCSTR lpFilePath);
DLL_API BOOL _stdcall Printer_Output_String(HANDLE hPrinterID, LPCSTR lpStr);
DLL_API BOOL _stdcall Printer_Output_Data(HANDLE hPrinterID, LPCSTR lpBuffer, LONG iLength);
DLL_API BOOL _stdcall Printer_Output_File(HANDLE hPrinterID, LPCSTR lpFilePath);
DLL_API BOOL _stdcall Printer_Output_Logo(HANDLE hPrinterID, BYTE iNumber);
DLL_API BOOL _stdcall Printer_Output_Bitmap(HANDLE hPrinterID, BYTE iType, LPCSTR lpFilePath);
DLL_API BOOL _stdcall Printer_Output_BmpInFlash(HANDLE hPrinterID, BYTE iNumber, BYTE iType);
DLL_API BOOL _stdcall Printer_Output_Barcode(HANDLE hPrinterID, LONG iType, LONG iWidth, LONG iHeight, LONG iHRI, LPCSTR lpString);
DLL_API BOOL _stdcall Printer_Output_QR(HANDLE hPrinterID, LONG iWidth, LPCSTR lpString);
DLL_API BYTE _stdcall Printer_Query_Status(HANDLE hPrinterID,LONG iType);
DLL_API LONG _stdcall Printer_Query_Version(void);
//************************************************************************************************************************************************************/
#endif
//************************************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//**  文件结束  **********************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//************************************************************************************************************************************************************/
