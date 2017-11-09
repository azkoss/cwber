
#define ERR	     	-1
#define ACKOUTTIME	-2
#define ACKERR		-3
#define COMMOUTTIME	-4
#define STXERR      -5
#define OK	    	0

#define ERRCARD	2
#define ERRRDCARD	3
#define ERRWRCARD	4
#define ERRCARDSEC	5
#define ERRCARDKEY	6
#define ERRCARDLOCKED 7
#define ERRMSG 8
#define ERRRFCARD	9
#define ERRFORMAT	10
#define ERROVERFLOW	11

#define NOCARD	1
#define UNKNOWCARD	12
#define ERRCARDPOSITION	14

#define PAC_ADDRESS	1021

#define ENQ  0x05//请求连接通信线路(询问).
#define ACK  0x06//确认(握手).
#define NAK  0x15//通信忙.
#define EOT  0x04//通信结束(传送结束).
#define CAN  0x18//解除通信(取消).
#define STX  0x02//数据包起始符(起始字符).
#define ETX  0x03//数据包结束符(终结符).
#define US   0x1F//数据分隔符.

int APIENTRY GetSysVerion(HANDLE ComHandle, char *strVerion);
HANDLE APIENTRY CommOpen(char *Port);
HANDLE APIENTRY CommOpenWithBaut(char *Port, unsigned int _data);
HANDLE APIENTRY CommOpenWithBautForVB(HANDLE ComHandle, BYTE _Bauddata);
int APIENTRY CommClose(HANDLE ComHandle);
int APIENTRY CommSetting(HANDLE ComHandle,char *ComSeting);
////////////////////////////////////////////////////////////////////////////////////////

int APIENTRY MC_ReadTrack(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _mode, BYTE _track, BYTE _TrackData[], int *_TrackDataLen);
int APIENTRY MC_UploadTrack(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _mode, BYTE _track, BYTE _TrackData[], int *_TrackDataLen);
int APIENTRY MC_ClearTrack(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);

int APIENTRY CRT580_Reset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY CRT580_CardSetting(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl,BYTE _CardIn, BYTE _StopPosition);
int APIENTRY CRT580_GetStatus(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE *_SS5,BYTE *_SS4, BYTE *_SS3, BYTE *_SS2, BYTE *_SS1, BYTE *_SS0);
int APIENTRY CRT580_SensorStatus(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl,BYTE *_PH1,BYTE *_PH2,BYTE *_PH3,BYTE *_PH4,BYTE *_PH5,BYTE *_PH6,BYTE *_KSW,BYTE *_CTSW, BYTE *_PSS1,BYTE *_PSS2, BYTE *_PSS3, BYTE *_PSS4, BYTE *_PSS5);
int APIENTRY CRT580_MoveCard(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _ToPosition, BYTE _FromPosition);
int APIENTRY CRT580_SetBaudRate(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Bauddata);

int APIENTRY CRT_IC_CardOpen(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY CRT_IC_CardClose(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY CRT_IC_DetectCard(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl,BYTE *_CardType,BYTE *_CardInfor);

int APIENTRY RF_DetectCard(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY RF_GetCardID(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _CardID[4]);
int APIENTRY RF_ChangeSecKey(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Sec, BYTE _KEY[6]);
int APIENTRY RF_LoadSecKey(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Sec, BYTE _KEYType, BYTE _KEY[6]);
int APIENTRY RF_ReadBlock(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Sec, BYTE _Block, BYTE _BlockData[16]);
int APIENTRY RF_ChangeSecKey(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Sec, BYTE _KEY[6]);
int APIENTRY RF_WriteBlock(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Sec, BYTE _Block, BYTE _BlockData[16]);

int APIENTRY RF_InitValue(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Sec, BYTE _Block, BYTE _Data[4]);
int APIENTRY RF_ReadValue(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Sec, BYTE _Block, BYTE _Data[4]);
int APIENTRY RF_Decrement(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Sec, BYTE _Block, BYTE _Data[4]);
int APIENTRY RF_Increment(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Sec, BYTE _Block, BYTE _Data[4]);

int APIENTRY IC24CXX_DetectCard(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl,BYTE *_CardType);
int APIENTRY IC24CXX_ReadBlock(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _CardType, int _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY IC24CXX_WriteBlock(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _CardType, int _Address, BYTE _dataLen, BYTE _BlockData[]);

int APIENTRY CPU_Reset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE *_CPUType,BYTE _exData[], int *_exdataLen);
int APIENTRY CPU_WarmReset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE *_CPUType,BYTE _exData[], int *_exdataLen);
int APIENTRY CPU_T0_C_APDU(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, int _dataLen, BYTE _APDUData[], BYTE _exData[], int *_exdataLen);
int APIENTRY CPU_T1_C_APDU(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, int _dataLen, BYTE _APDUData[], BYTE _exData[], int *_exdataLen);

int APIENTRY SIM_Reset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl,BYTE _VOLTAGE,BYTE _SIMNo, BYTE *_SIMTYPE,BYTE _exData[], int *_exdataLen);
int APIENTRY SIM_T0_C_APDU(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE SIMNo, int _dataLen, BYTE _APDUData[], BYTE _exData[], int *_exdataLen);
int APIENTRY SIM_T1_C_APDU(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE SIMNo, int _dataLen, BYTE _APDUData[], BYTE _exData[], int *_exdataLen);
int APIENTRY SIM_CardClose(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);

int APIENTRY SLE4442_Reset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY SLE4442_Read(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY SLE4442_ReadP(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _BlockData[32]);
int APIENTRY SLE4442_ReadS(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _BlockData[4]);
int APIENTRY SLE4442_VerifyPWD(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _PWData[3]);
int APIENTRY SLE4442_Write(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY SLE4442_WriteP(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _BlockData[32]);
int APIENTRY SLE4442_WritePWD(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _PWData[3]);

int APIENTRY SLE4428_Reset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY SLE4428_Read(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, int _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY SLE4428_ReadP(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, int _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY SLE4428_VerifyPWD(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _PWData[2]);
int APIENTRY SLE4428_WritePWD(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _PWDataOld[2],BYTE _PWDataNew[2]);
int APIENTRY SLE4428_Write(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, int _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY SLE4428_WriteP(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, int _Address, BYTE _dataLen, BYTE _BlockData[]);

int APIENTRY AT45D041_Reset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY AT45D041_Read(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, int  _Address, BYTE _BlockData[]);
int APIENTRY AT45D041_Write(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, int  _Address, BYTE _BlockData[264]);

int APIENTRY AT88SC102_Reset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY AT88SC102_Security1Clear(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Index, BYTE _Address, BYTE _dataLen);
int APIENTRY AT88SC102_Security2ClearApp1(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl,BYTE _Security2App1PW[6]);
int APIENTRY AT88SC102_Security2ClearApp2(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl,BYTE _EC2,BYTE _Security2App2PW[4]);
int APIENTRY AT88SC102_Read(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Index, BYTE _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY AT88SC102_VerifyPWD(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _PWData[]);
int APIENTRY AT88SC102_Write(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Index, BYTE _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY AT88SC102_WritePWD(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _PWIndex,BYTE _PWData[]);
int APIENTRY AT88SC102_InitSecurity2(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _CtrlMode);
int APIENTRY AT88SC102_DisableEC2(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);

int APIENTRY AT88SC1604_Clear(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Index, int _Address, BYTE _dataLen);
int APIENTRY AT88SC1604_Personalization(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl,BYTE _data);
int APIENTRY AT88SC1604_Read(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Index, int _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY AT88SC1604_Reset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY AT88SC1604_VerifyPWD(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _PWIndex,BYTE _PWData[]);
int APIENTRY AT88SC1604_Write(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Index, int _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY AT88SC1604_WritePWD(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _PWIndex,BYTE _PWData[]);

int APIENTRY AT88SC1608_Reset(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY AT88SC1608_VerifyPWD(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _PWIndex, BYTE _PWData[]);
int APIENTRY AT88SC1608_Read(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Index, BYTE _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY AT88SC1608_Write(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Index, BYTE _Address, BYTE _dataLen, BYTE _BlockData[]);
int APIENTRY AT88SC1608_ReadFUSE(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE * _PER, BYTE * _CMA, BYTE * _FAB);
int APIENTRY AT88SC1608_WriteFUSE(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl);
int APIENTRY AT88SC1608_InitAuth(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Q[8]);
int APIENTRY AT88SC1608_VerifyAuth(HANDLE ComHandle,BYTE _AddrH, BYTE _Addrl, BYTE _Q[8]);
