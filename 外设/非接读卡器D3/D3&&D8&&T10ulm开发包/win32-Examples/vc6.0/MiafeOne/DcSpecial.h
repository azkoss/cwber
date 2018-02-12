#define STX						0x02
#define ETX						0x03
#define ACK						0x06
#define DLE						0x10
#define NAK						0x15

#define CMD_HL_DEC				0x30
#define CMD_HL_INC				0x31
#define CMD_HL_W				0x32
#define CMD_HL_R				0x33
#define CMD_HL_INITVAL			0x34
#define CMD_DISP				0x35
#define CMD_BEEP				0x36
#define CMD_LIGHT				0x58
#define CMD_R_EEPROM			0x37
#define CMD_W_EEPROM			0x38
#define CMD_SET_TIME			0x39
#define CMD_GET_TIME			0x3a
#define CMD_HL_AUTH				0x40
#define CMD_REQUEST				0x41
#define CMD_ANTICOLL			0x42
#define CMD_SELECT				0x43
#define CMD_AUTH				0x44
#define CMD_HALT				0x45
#define CMD_READ				0x46
#define CMD_WRITE				0x47
#define CMD_INC					0x48
#define CMD_DEC					0x49
#define CMD_RESTOR				0x4a
#define CMD_TRANSFER			0x4b
#define CMD_LOAD_KEY			0x4c
#define CMD_RESET				0x4e
#define CMD_STATUS				0x4f
#define CMD_SET_BIT				0x50
#define CMD_CLR_BIT				0x51
#define CMD_CONFIG				0x52
#define CMD_CHECK_W				0x53
#define CMD_SET_PORT			0x54    //not use
#define CMD_GET_PORT			0x55    //not use
#define CMD_AUTH_2				0x56
#define CMD_DEC_TRAN			0x57
#define CMD_HIGH_DISP			0x7f
#define CMD_READER_VER			0x80
#define CMD_DISP_OLD			0x56
#define CMD_BEEP_OLD			0x57
#define CMD_CPUPDOWN			0x7C
#define CMD_CPUAPDU				0x7d
#define CMD_CPURESET			0x7e
#define CMD_SET_DELTIME			0x81
#define CMD_SET_ENCRYPT			0x82
#define CMD_SET_CPU				0x83
#define CMD_READ_ALL			0x96
#define CMD_WRITE_ALL			0x97
#define CMD_SET_AUTOFLAG		0x86
#define CMD_READ_ONE			0xa6
#define CMD_WRITE_ONE			0xa7
#define CMD_ANTICOLL2			0xa2
#define CMD_SELECT2				0xa3

typedef struct _PORT_TABLE
{
	dc_ULONG ulRef;
	dc_CHAR szPort[16];
} PORT_TABLE, *LPPORT_TABLE;

#if defined(DC_VER_WINDOWS)
static PORT_TABLE g_SupportPortList[] =
{
	{ 0, "\\\\.\\COM1"}, { 1, "\\\\.\\COM2"}, { 2, "\\\\.\\COM3"}, { 3, "\\\\.\\COM4"},
	{ 4, "\\\\.\\COM5"}, { 5, "\\\\.\\COM6"}, { 6, "\\\\.\\COM7"}, { 7, "\\\\.\\COM8"},
	{ 8, "\\\\.\\COM9"}, { 9, "\\\\.\\COM10"}
};
#elif defined(DC_VER_LINUX)
static PORT_TABLE g_SupportPortList[] =
{
	{ 0, "/dev/ttyS0"}, { 1, "/dev/ttyS1"}, { 2, "/dev/ttyS2"}, { 3, "/dev/ttyS3"},
	{ 4, "/dev/ttyS4"}, { 5, "/dev/ttyS5"}, { 6, "/dev/ttyS6"}, { 7, "/dev/ttyS7"},
	{ 8, "/dev/ttyS8"}, { 9, "/dev/ttyS9"}
};
#endif

#define g_SupportPortListSize sizeof(g_SupportPortList)/sizeof(g_SupportPortList[0])

static dc_ULONG g_SupportBaudList[] = {9600, 19200, 38400, 57600, 115200};

#define g_SupportBaudListSize sizeof(g_SupportBaudList)/sizeof(g_SupportBaudList[0])

#if defined(DC_VER_LINUX)
static dc_ULONG g_SupportBaudListForLinux[] = {B9600, B19200, B38400, B57600, B115200};
#define NULL 0
#endif
