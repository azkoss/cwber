//************************************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//**  �ļ���ʼ(ͷ�ļ�)****************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//**��ֹͷ�ļ����ض���****************************************************************************************************************************************/
#ifndef __SDK_H__
#define __SDK_H__
//************************************************************************************************************************************************************/
//**����DLL����ǰ׺*******************************************************************************************************************************************/
#ifdef DLL_API 
#else
	#define DLL_API extern "C" __declspec(dllimport)
#endif
//************************************************************************************************************************************************************/
//**��������**************************************************************************************************************************************************/
#define C_VERSION							109				// SDK�汾�ţ�3λ���֣�
// ��������-�˿�����
#define C_PRINTER_PORT_COM					0				// ��ӡ���˿����� ����
#define C_PRINTER_PORT_LPT					1				// ��ӡ���˿����� ����
#define C_PRINTER_PORT_USB					2				// ��ӡ���˿����� USB
#define C_PRINTER_PORT_NET					3				// (Ԥ��)��ӡ���˿����� ����
#define C_PRINTER_PORT_VLPT					4				// ��ӡ���˿����� ���Ⲣ��
// ��������-��ֽ��ʽ
#define C_PRINTER_CUT_FULL					0				// ȫ��
#define C_PRINTER_CUT_PART					1				// ����
//  ��������-�ַ�����
#define C_PRINTER_ENCODING_ASCII			0				// ASCII����
#define C_PRINTER_ENCODING_GB2312			1				// GB2312����
#define C_PRINTER_ENCODING_GB18030			2				// GB18030����
// ��������-�����С
#define C_PRINTER_FONT_24					0				// 24������
#define C_PRINTER_FONT_16					1				// 16������
// ��������-���뷽ʽ
#define C_PRINTER_ALIGN_LEFT				0				// �������
#define C_PRINTER_ALIGN_CENTER				1				// ���ж���
#define C_PRINTER_ALIGN_RIGHT				2				// ���Ҷ���
//  ��������-�ڱ����
#define C_PRINTER_BM_MOVE					1				// ��ʼλ�õ��趨ֵ
#define C_PRINTER_BM_CUT					2				// ��ֽλ�õ��趨ֵ
#define C_PRINTER_BM_FORWARD				0				// ��ֽ�ķ���
#define C_PRINTER_BM_BACK					1				// ��ֽ�ķ���
//  ��������-BMPλͼģʽ
#define C_PRINTER_BMP_8						0				// 8�㵥�ܶ�
#define C_PRINTER_BMP_8D					1				// 8��˫�ܶ�
#define C_PRINTER_BMP_24					32				// 24�㵥�ܶ�
#define C_PRINTER_BMP_24D					33				// 24��˫�ܶ�
// һά���������ͳ���
#define C_PRINTER_BARCODE_EAN13				2				// EAN13������
#define C_PRINTER_BARCODE_CODE39			69				// CODE39������
#define C_PRINTER_BARCODE_CODE128			73				// CODE128������
// һά�������˹�ʶ���ַ�λ�ó���
#define C_PRINTER_HRI_NONE					0				// ����ӡHRI�ַ�
#define C_PRINTER_HRI_TOP					1				// ���������Ϸ���ӡHRI�ַ�
#define C_PRINTER_HRI_BOTTOM				2				// ���������·���ӡHRI�ַ�
#define C_PRINTER_HRI_BOTH					3				// ���������Ϸ����·���ӡHRI�ַ�
// ��ӡ��״̬���ͳ���
#define C_PRINTER_STATUS_ONLINE				1				// ��������״̬
#define C_PRINTER_STATUS_DOOR				2				// ֽ����״̬
#define C_PRINTER_STATUS_CUT				3				// ��ֽ��״̬
#define C_PRINTER_STATUS_PAPER				4				// ֽ��״̬
#define C_PRINTER_STATUS_HEAT				5				// (Ԥ��)����Ƭ״̬
#define C_PRINTER_STATUS_BUFFER				6				// (Ԥ��)������״̬
#define C_PRINTER_STATUS_DRAWER				7				// (Ԥ��)Ǯ��״̬
#define C_PRINTER_STATUS_JAM				8				// ��ֽ״̬
//************************************************************************************************************************************************************/
//**��������**************************************************************************************************************************************************/
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
//**  �ļ�����  **********************************************************************************************************************************************/
//************************************************************************************************************************************************************/
//************************************************************************************************************************************************************/
