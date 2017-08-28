using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text;
using WinFormDemo.Properties;

namespace WinFormDemo
{
    #region 密码键盘
    //the enums from IPinpad.h
    enum EPORT
    {
        eCOM = 0x0     // Serial("COM1:9600,N,8,1")
      ,
        eUSB_FTDI = 0x1     // only for FTDI chip with baudrate, such as EPP("FT232R USB UART:9600,N,8,1")
            ,
        eUSB = 0x2     // Windows(such as "VID_23AB&PID_0002","VID_23AB&PID_0002&REV_0900","USB\\VID_23AB&PID_0002") 
            // Linux  ("lp0")

            ,
        eHID = 0x3     // Windows(such as "VID_23AB&PID_1003","VID_23AB&PID_1003&REV_0100","HID\\VID_23AB&PID_1003")
            // Linux  ("hiddev0")

            ,
        ePC_SC = 0x20     // only for windows PC/SC()
            ,
        eLPT = 0x40     // LPT("LPT1")
            ,
        eTCPIP = 0x80     // TCP(such as "127.0.0.1:36860")

            , eCOMBINE = 0x100     // It is combined with master device, and must behind master device instantiated
    }

    enum EPIN_TYPE
    {
        ePIN_UNKNOWN = 0x0   //unknown type(DLL will try to detect pinpad's type,when finish do that you cann't change other command pinpad)

      ,
        ePIN_EPP = 0x10000   //EPP command type(Bxx Cxx Exx Vxx)
            ,
        ePIN_VISA = 0x20000   //VISA command type(Dxx)
            ,
        ePIN_PCI = 0x40000   //PCI command type(Hxx)
            ,
        ePIN_WOSA = 0x80000   //WOSA command type(Fxx)

            ,
        ePIN_VISA_3X = ePIN_VISA + 0x2  //VISA_3X
            ,
        ePIN_EPP_BR = ePIN_EPP + 0x4  //EPP_BR(B7/C60)
            , ePIN_WOSA_3X = ePIN_WOSA + 0x8  //WOSA_3X
    }

    enum ECRYPT
    {
        CRYPT_DESECB = (0x0001)  //DES ECB
      ,
        CRYPT_DESCBC = (0x0002)  //DES CBC
            ,
        CRYPT_DESCFB = (0x0004)  //DES CFB
            ,
        CRYPT_RSA = (0x0008)  //RSA
            ,
        CRYPT_ECMA = (0x0010)  //ECMA
            ,
        CRYPT_DESMAC = (0x0020)  //DES MAC
            ,
        CRYPT_TRIDESECB = (0x0040)  //TDES ECB
            ,
        CRYPT_TRIDESCBC = (0x0080)  //TDES CBC
            ,
        CRYPT_TRIDESCFB = (0x0100)  //TDES CFB
            ,
        CRYPT_TRIDESMAC = (0x0200)  //TDES MAC
            ,
        CRYPT_MAAMAC = (0x0400)  //MAA MAC
            ,
        CRYPT_SM4ECB = (0x1000)	//SM4 ECB
            ,
        CRYPT_SM4MAC = (0x2000)	//SM4 CBC
            ,
        CRYPT_SM4CBC = (0x4000)	//SM4 MAC
            ,
        CRYPT_OFB = (0x10000) //AES OFB
            ,
        CRYPT_CFB = (0x20000) //AES CFB
            ,
        CRYPT_PCBC = (0x40000) //AES PCBC
            , CRYPT_CTR = (0x80000) //AES CTR
    }  //Crypt mode

    enum EPINFORMAT
    {
        FORMAT_ARITHMETIC = (0x0000)      //arithmetic choose
      ,
        FORMAT_IBM3624 = (0x0001)      //IBM3624
            ,
        FORMAT_ANSI = (0x0002)      //ANSI 9.8
            ,
        FORMAT_ISO0 = (0x0004)      //ISO9564 0	
            ,
        FORMAT_ISO1 = (0x0008)      //ISO9564 1
            ,
        FORMAT_ECI2 = (0x0010)      //ECI2
            ,
        FORMAT_ECI3 = (0x0020)      //ECI3
            ,
        FORMAT_VISA = (0x0040)      //VISA/VISA2
            ,
        FORMAT_DIEBOLD = (0x0080)      //DIEBOLD
            ,
        FORMAT_DIEBOLDCO = (0x0100)      //DIEBOLDCO
            ,
        FORMAT_VISA3 = (0x0200)      //VISA3
            ,
        FORMAT_BANKSYS = (0x0400)      //Bank system
            ,
        FORMAT_EMV = (0x0800)      //EMV
            ,
        FORMAT_ISO3 = (0x2000)      //ISO9564 3	
            , FORMAT_AP = (0x4000)      //AP
    }

    enum EMAC
    {
        MAC_X9 = 0x00      //X9.9
      ,
        MAC_X919 = 0x01      //X9.19
            ,
        MAC_PSAM = 0x02      //PSAM
            ,
        MAC_PBOC = 0x03      //PBOC
            ,
        MAC_CBC = 0x04      //CBC(ISO 16609)
            ,
        MAC_BANKSYS = 0x05      //Bank system
            ,
        AES_CMAC = 0x06      //AES-CMAC-PRF-128
            ,
        AES_XCBC = 0x07      //AES-XCBC-PRF-128
            ,
        SM4MAC_PBOC = 0x08      //PBOC
            , SM4MAC_BANKSYS = 0x09      //Bank system
    }

    enum EKEYATTR
    {
        ATTR_SPECIAL = (0x0000)  //special key(UID | UAK | KBPK | IMK)
      ,
        ATTR_SM = (0x0008)  //

            ,
        ATTR_DK = (0x0001)  //DATA KEY(WFS_PIN_USECRYPT)
            ,
        ATTR_PK = (0x0002)  //PIN KEY(WFS_PIN_USEFUNCTION)
            ,
        ATTR_AK = (0x0004)  //MAC KEY(WFS_PIN_USEMACING)
            ,
        ATTR_MK = (0x0020)  //MASTER KEY / MK only for MK(WFS_PIN_USEKEYENCKEY)
            ,
        ATTR_IV = (0x0080)  //IV KEY(WFS_PIN_USESVENCKEY)

            ,
        ATTR_NODUPLICATE = (0x0040)  //All the key value must diffent(WFS_PIN_USENODUPLICATE)
            ,
        ATTR_ANSTR31 = (0x0400)  //ANSTR31 MASTER KEY(WFS_PIN_USEANSTR31MASTER)
            ,
        ATTR_PINLOCAL = (0x10000)  //pin local offset(WFS_PIN_USEPINLOCAL)

            ,
        ATTR_RSAPUBLIC = (0x20000)  //RSA public(WFS_PIN_USERSAPUBLIC)
            ,
        ATTR_RSAPRIVATE = (0x40000)  //RSA private(WFS_PIN_USERSAPRIVATE)
            ,
        ATTR_RSA_VERIFY = (0x8000000)  //RSA public verify(WFS_PIN_USERSAPUBLICVERIFY)
            ,
        ATTR_RSA_SIGN = (0x10000000)  //RSA private sign(WFS_PIN_USERSAPRIVATESIGN)

            ,
        ATTR_CHIPINFO = (0x100000)  //WFS_PIN_USECHIPINFO
            ,
        ATTR_CHIPPIN = (0x200000)  //WFS_PIN_USECHIPPIN
            ,
        ATTR_CHIPPS = (0x400000)  //WFS_PIN_USECHIPPS
            ,
        ATTR_CHIPMAC = (0x800000)  //WFS_PIN_USECHIPMAC
            ,
        ATTR_CHIPLT = (0x1000000)  //WFS_PIN_USECHIPLT
            ,
        ATTR_CHIPMACLZ = (0x2000000)  //WFS_PIN_USECHIPMACLZ
            ,
        ATTR_CHIPMACAZ = (0x4000000)  //WFS_PIN_USECHIPMACAZ

            ,
        ATTR_MPK = (ATTR_MK | ATTR_PK)  //MASTER KEY only for PIN KEY
            ,
        ATTR_MDK = (ATTR_MK | ATTR_DK)  //MASTER KEY only for DATA KEY
            ,
        ATTR_MAK = (ATTR_MK | ATTR_AK)  //MASTER KEY only for MAC KEY
            ,
        ATTR_MIV = (ATTR_MK | ATTR_IV)  //MASTER KEY only for IV

            ,
        ATTR_WK = (ATTR_PK | ATTR_DK | ATTR_AK)
            , ATTR_MWK = (ATTR_MK | ATTR_PK | ATTR_DK | ATTR_AK)
    }

    enum EKEYMODE
    {
        KEY_SET = 0x30  // It's equivalent to "combine" at some pinpad
      ,
        KEY_XOR = 0x31
            ,
        KEY_XOR2 = 0x32
            , KEY_XOR3 = 0x33
    }

    enum EKCVMODE
    {
        KCVNONE = 0x0       //no KCV
      ,
        KCVSELF = 0x1       //key encrypt itself(first 8 char)
            , KCVZERO = 0x2       //key encrypt 00000000
    }

    enum ENTRYMODE
    {
        ENTRY_MODE_CLOSE = 0x0
      ,
        ENTRY_MODE_TEXT = 0x1
            ,
        ENTRY_MODE_PIN = 0x2
            , ENTRY_MODE_KEY = 0x3
    }

    enum ESOUND
    {
        SOUND_CLOSE = 0x0
      ,
        SOUND_OPEN = 0x1
            , SOUND_KEEP = 0x2
    }

    enum EINSTALL_AUTH
    {
        AUTH_FORBID = 0x30      //forbid(disable) RemoveInstallAuth, don't use this RemoveInstallAuth
      ,
        AUTH_REMOVE = 0x31      //remove RemoveInstallAuth 
            , AUTH_INSTALL = 0x32      //install RemoveInstallAuth
    }

    enum EPSW_ID
    {
        PSW_ID1 = 0x1       //password id 1
      ,
        PSW_ID2 = 0x2       //password id 2
            ,
        PSW_ID3 = 0x3       //password id 3
            ,
        PSW_ID4 = 0x4       //password id 4 (Password1 for RemoveInstallAuth)
            , PSW_ID5 = 0x5       //password id 5 (Password2 for RemoveInstallAuth)
    }

    enum EPSW
    {
        PSW_OLD = 0x30      // use the PIN buffer
      , PSW_NEW = 0x31      // use the KEY buffer
    } 
    #endregion
    

    class DllClass
    {

        #region 调用非接读卡器dll
        //        private static readonly ILog Log = LogManager.GetLogger("DcrfLog");
        [DllImport("dcrf32.dll")]
        public static extern int dc_init(Int16 port, Int32 baud); //初试化

        [DllImport("dcrf32.dll")]
        public static extern short dc_exit(int icdev);

        [DllImport("dcrf32.dll")]
        public static extern short dc_card(int icdev, char mode, ref ulong snr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_beep(int icdev, uint msec);

        [DllImport("dcrf32.dll")]
        public static extern short dc_reset(int icdev, uint sec);

        [DllImport("dcrf32.dll")]
        public static extern int dc_authentication(int icdev, int _Mode, int _SecNr);

        [DllImport("dcrf32.dll")]
        public static extern int dc_load_key(int icdev, int mode, int secnr, [In] byte[] nkey);  //密码装载到读写模块中

        [DllImport("dcrf32.dll")]
        public static extern int dc_load_key_hex(int icdev, int mode, int secnr, string nkey);  //密码装载到读写模块中

        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据

        [DllImport("dcrf32.dll")]
        public static extern short dc_read_hex(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据(转换为16进制)
        
        [DllImport("dcrf32.dll")]
        public static extern int a_hex(string oldValue, ref string newValue, Int16 len);  //普通字符转换成十六进制字符
        
        [DllImport("dcrf32.dll")]
        public static extern void hex_a(ref string oldValue, ref string newValue, int len);  //十六进制字符转换成普通字符

        private Container components = null;
        private static int _icdev = -1;

        public void Dcrf(Container components)
        {
            this.components = components;
        }

        public static int IcDev
        {
            get { return _icdev; }
            set { _icdev = value; }
        }

        public static void InitIc()
        {
            //初始化串口1
            if (_icdev < 0)
            {
                _icdev = dc_init(100, 115200);
            }

            if (_icdev <= 0)
            {
                //                 Log.Info("Init Com Error!"+_icdev);
            }
            else
            {
                string a = "ffffffffffff";
            }
        }


        //关闭串口
        public void ExitIc()
        {
            dc_exit((Int16)_icdev);
        }

        //读卡
        public string ReadIc()
        {
            ulong icCardNo = 0;
            char str = (char)0;
            dc_card((Int16)_icdev, str, ref icCardNo);
            return icCardNo.ToString();
        }

        //器蜂鸣
        public static int Beep(int IcDev)
        {
            int st;
            st = dc_beep((Int16)IcDev, 50);
            return st;
        }
        #endregion
        

        #region 调用创自读卡器dll
        [DllImport("CRT_310.dll")]
        public static extern UInt32 CommOpen(string port);

        //按指定的波特率打开串口
        [DllImport("CRT_310.dll")]
        public static extern long CommOpenWithBaut(string port, byte _BaudOption);

        //关闭串口
        [DllImport("CRT_310.dll")]
        public static extern int CommClose(UInt32 ComHandle);


        [DllImport("CRT_310.dll")]
        public static extern int GetErrCode(ref int Errorcode);

        //读卡机序列号
        [DllImport("CRT_310.dll")]
        public static extern int CRT310_ReadSnr(UInt32 ComHandle, byte[] _SNData, ref byte _dataLen);

        //复位读卡机//0x30=不弹卡 0x31=前端弹卡 0x32=后端弹卡
        [DllImport("CRT_310.dll")]
        public static extern int CRT310_Reset(UInt32 ComHandle, byte _Eject);

        //停卡位置
        [DllImport("CRT_310.dll")]
        public static extern int CRT310_CardPosition(UInt32 ComHandle, byte _Position);


        //移动卡
        [DllImport("CRT_310.dll")]
        public static extern int CRT310_MovePosition(UInt32 ComHandle, int _Position);


        //进卡控制
        [DllImport("CRT_310.dll")]
        public static extern int CRT310_CardSetting(UInt32 ComHandle, byte _CardIn, byte _EnableBackIn);


        //从读卡机读取状态信息
        [DllImport("CRT_310.dll")]
        public static extern int CRT310_GetStatus(UInt32 ComHandle, ref byte _CardStatus, ref byte _frontStatus,
            ref byte _RearStatus);

        //IC CARD POWER ON
        [DllImport("CRT_310.dll")]
        public static extern int CRT_IC_CardOpen(UInt32 ComHandle);

        //IC CARD POWER OFF
        [DllImport("CRT_310.dll")]
        public static extern int CRT_IC_CardClose(UInt32 ComHandle);



        static UInt32 SSCardHdle = CommOpen(Settings.Default.SSCardComPort);
        //        获得串口句柄
        public static uint SsCardGetHdle()
        {
            return SSCardHdle;
        }

        //        上电
        public static void Sd()
        {
            CRT_IC_CardOpen(SsCardGetHdle());
        }

        //        下电
        public static void Xd()
        {
            CRT_IC_CardClose(SsCardGetHdle());
        }

        //        禁止进卡
        public static void DisableCardIn()
        {
            CRT310_CardSetting(SsCardGetHdle(), 0x1, 0x1);
        }

        //        允许进卡
        public static void AllowCardIn()
        {
            CRT310_CardSetting(SsCardGetHdle(), 0x3, 0x1);
        }

        //        弹卡
        public static void MoveOutCard()
        {
            CRT310_MovePosition(SsCardGetHdle(), 0x2);
        }

        //        重置
        public static void ResetDevice()
        {
            CRT310_Reset(SsCardGetHdle(), 1);
        }

        //        返回ture表示有卡，返回false表示没卡
        public static bool ReadStatus()
        {
            Boolean b = false;
            if (SsCardGetHdle() != 0)
            {
                byte _CardStatus, _frontSetting, _RearSetting;
                _CardStatus = 0;
                _frontSetting = 0;
                _RearSetting = 0;
                var i = CRT310_GetStatus(SsCardGetHdle(), ref _CardStatus, ref _frontSetting, ref _RearSetting);
                if (i == 0)
                {
                    switch (_CardStatus)
                    {
                        case 70:
                            b = true;
                            break;
                        case 72:
                            b = true;
                            break;
                        case 73:
                            b = true;
                            break;
                        case 74:
                            b = true;
                            break;
                        case 75:
                            b = true;
                            break;
                        case 76:
                            b = true;
                            break;
                        case 77:
                            b = true;
                            break;
                        case 78:
                            //                            b = false;
                            break;
                    }
                }
                else
                {
                    b = true;
                }
            }
            else
            {
                b = true;
            }
            return b;
        }

        //移动到ic卡操作位（没用）
        public static void MoveToIcPostion()
        {
            CRT310_MovePosition(SsCardGetHdle(), 0x4);
        }
	#endregion


        #region 调用医保dll
        [DllImport("ChisYBIntf.dll")]
        public static extern int InitYBIntfDll();

        [DllImport("ChisYBIntf.dll")]
        public static extern void FreeYBIntfDll();

        [DllImport("ChisYBIntf.dll")]
        public static extern int ChisYbRequestData(StringBuilder aUsername, StringBuilder aPassword,
            StringBuilder aBusinessType, StringBuilder aRequestData, StringBuilder aResponseData);
        #endregion


        #region 二代证dll
        [DllImport("cw_UserIDcard.dll")]
        private static extern int uid_open(int port, int baud = 9600, int date = 8, int stop = 1, char parity = 'N');
        [DllImport("cw_UserIDcard.dll")]
        private static extern int uid_close();
        [DllImport("cw_UserIDcard.dll")]
        private static extern int uid_GetDeviceStatus(StringBuilder Desc);
        [DllImport("cw_UserIDcard.dll")]
        public static extern int uid_GetCardStatus();
        [DllImport("cw_UserIDcard.dll")]
        public static extern int uid_ReadCard(StringBuilder buf1);
        [DllImport("cw_UserIDcard.dll")]
        private static extern int uid_SavePhoto(string FileName);
        [DllImport("cw_UserIDcard.dll")]
        private static extern int uid_setDriverType(string DriverType);

        private static StringBuilder strBuf = new StringBuilder(512);
        public static string[] user = new string[10];//保存用户信息
        public const int name = 0;//姓名
        public const int sex = 1;//性别
        public const int nation = 2;//民族
        public const int Birthday = 3;//生日
        public const int adress = 4;//地址
        public const int id = 5;//身份证号
        public const int organ = 6;//发证地址
        public const int start = 7;//有效期起始
        public const int end = 8;//有效期结束
        public static string bmpfilename;


        public static void clear()
        {
            for (int i = 0; i != 9; i++)
                user[i] = "";
            bmpfilename = "";
        }
        public static bool open()
        {
            try
            {
                clear();
                if (0 != uid_setDriverType(Properties.Settings.Default.UID_Type))
                {
                    //                    MainWindow.log.Error("Client.driver.UserIDCard.open:设置身份证类型参数错误");
                    return false;
                }
                int ret = uid_open(Properties.Settings.Default.UID_Port);
                switch (ret)
                {
                    case 0:
                        //                        MainWindow.log.Info("Client.driver.UserIDCard.open:打开端口成功");
                        return true;
                    case 1:
                        //                        MainWindow.log.Error("Client.driver.UserIDCard.open:类型读取错误");
                        return false;
                    case 2:
                        //                        MainWindow.log.Error("Client.driver.UserIDCard.open:波特率读取错误");
                        return false;
                    case 3:
                        //                        MainWindow.log.Error("Client.driver.UserIDCard.open:端口号读取错误");
                        return false;
                    case 4:
                        //                        MainWindow.log.Error("Client.driver.UserIDCard.open:打开端口失败");
                        return false;
                    default:
                        //                        MainWindow.log.Error("Client.driver.UserIDCard.open:未知错误-" + ret.ToString());
                        return false;
                }
            }
            catch (Exception ex)
            {
                //                MainWindow.log.Fatal("Client.driver.UserIDCard.open:" + ex.Message);
                return false;
            }
        }
        public static bool close()
        {
            try
            {
                clear();
                uid_close();
                //                MainWindow.log.Info("Client.driver.UserIDCard.close:串口关闭");
                return true;
            }
            catch (Exception ex)
            {
                //                MainWindow.log.Fatal("Client.driver.UserIDCard.close:" + ex.Message);
                return false;
            }
        }
        public static bool GetDeviceStatus()
        {
            try
            {
                StringBuilder s1 = new StringBuilder(64);
                int ret = uid_GetDeviceStatus(s1);
                switch (ret)
                {
                    case 0:
                        //                        MainWindow.log.Info("Client.driver.UserIDCard.GetDeviceStatus:状态正常");
                        return true;
                    case 52:
                        //                        MainWindow.log.Error("Client.driver.UserIDCard.GetDeviceStatus:设备故障");
                        return false;
                    case 82:
                        //                        MainWindow.log.Error("Client.driver.UserIDCard.GetDeviceStatus:通信失败");
                        return false;
                    default:
                        //                        MainWindow.log.Error("Client.driver.UserIDCard.GetDeviceStatus:未知错误-" + ret.ToString() + "," + s1.ToString());
                        return false;
                }
            }
            catch (Exception ex)
            {
                //                MainWindow.log.Fatal("Client.driver.LISTPRINT.GetDeviceStatus:" + ex.Message);
                return false;
            }
        }
        public static string ReadCard()//-1:故障，1：无卡，0：成功
        {
            string s = "";
            try
            {
                clear();
                int ret = uid_GetCardStatus();
                if (ret == 54)
                {
                    s = "无卡";
                    return s;
                }
                else if (ret == 53)
                {
                    ret = uid_ReadCard(strBuf);
                    if (ret != 0)
                    {
                        s = "设备故障";
                        return s;
                    }
                    user = strBuf.ToString().Split(',');
                    bmpfilename = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Properties.Settings.Default.UID_BMPDIR + user[id];
                    ret = uid_SavePhoto(bmpfilename);
                    if (ret == 0)
                    {
                        s = strBuf.ToString();
                        return s;
                        //                        bmpfilename = bmpfilename + ".bmp";
                        //                        BusinessData.ID.id = user[id];
                        //                        BusinessData.ID.name = user[name];//姓名
                        //                        BusinessData.ID.sex = user[sex];//性别
                        //                        BusinessData.ID.nation = user[nation];//民族
                        //                        BusinessData.ID.Birthday = user[Birthday];//生日
                        //                        BusinessData.ID.adress = user[adress];//地址
                        //                        BusinessData.ID.id = user[id];//身份证号
                        //                        BusinessData.ID.organ = user[organ];//发证地址
                        //                        BusinessData.ID.start = user[start];//有效期起始
                        //                        BusinessData.ID.end = user[end];//有效期结束
                        //                        BusinessData.ID.bmpfilename = bmpfilename + ".bmp";
                    }
                    else if (ret == 50)
                    {
                        s = "设备故障";
                        return s;
                    }
                    else if (ret == 51)
                    {
                        s = "设备故障";
                        return s;
                    }
                    else
                    {
                        s = "设备故障";
                        return s;
                    }
                }
                else if (ret == 82)
                {
                    s = "设备故障";
                    return s;
                }
                else
                {
                    s = "设备故障";
                    return s;
                }
            }
            catch (Exception ex)
            {
                s = "设备故障";
                return s;
            }
        }
        #endregion


        #region 银联dll
        /// <summary>
        /// //交易接口
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [DllImport("c:\\gmc\\posinf.dll", EntryPoint = "YS_Trans")]
        public static extern int YS_Trans([MarshalAs(UnmanagedType.LPStr)] StringBuilder str1, [MarshalAs(UnmanagedType.LPStr)] StringBuilder str2, [MarshalAs(UnmanagedType.LPStr)] StringBuilder state);

        /// <summary>
        /// //打开读卡器
        /// </summary>
        /// <param name="outstr"></param>
        /// <returns></returns>
        [DllImport("c:\\gmc\\posinf.dll", EntryPoint = "StartCardReader")]
        public static extern int StartCardReader([MarshalAs(UnmanagedType.LPStr)] StringBuilder outstr);

        /// <summary>
        /// //吐卡函数
        /// </summary>
        /// <param name="outstr"></param>
        /// <returns></returns>
        [DllImport("c:\\gmc\\posinf.dll", EntryPoint = "EjectCard")]
        public static extern int EjectCard([MarshalAs(UnmanagedType.LPStr)] StringBuilder outstr);

        /// <summary>
        /// //取读卡器状态
        /// </summary>
        /// <param name="outstr"></param>
        /// <returns></returns>
        [DllImport("c:\\gmc\\posinf.dll", EntryPoint = "GetCardReaderStatus")]
        public static extern int GetCardReaderStatus([MarshalAs(UnmanagedType.LPStr)] StringBuilder outstr);


        /// <summary>
        /// //密码键盘状态检测
        /// </summary>
        /// <param name="outstr"></param>
        /// <returns></returns>
        [DllImport("c:\\gmc\\posinf.dll", EntryPoint = "GetKeyBoardStatus")]
        public static extern int GetKeyBoardStatus([MarshalAs(UnmanagedType.LPStr)] StringBuilder outstr);

        /// <summary>
        /// //使用密码键盘输入明文
        /// </summary>
        /// <param name="outstr"></param>
        /// <returns></returns>
        [DllImport("c:\\gmc\\posinf.dll", EntryPoint = "EnterKey")]
        public static extern int EnterKey([MarshalAs(UnmanagedType.LPStr)] StringBuilder outstr);

        /// <summary>
        /// //关闭密码键盘
        /// </summary>
        /// <param name="outstr"></param>
        /// <returns></returns>
        [DllImport("c:\\gmc\\posinf.dll", EntryPoint = "CloseKey")]
        public static extern int CloseKey([MarshalAs(UnmanagedType.LPStr)] StringBuilder outstr);

        #endregion


        #region 华大身份证dll
        [DllImport("HDstdapi.dll")]
        public static extern int HD_InitComm(int port);

        [DllImport("HDstdapi.dll")]
        public static extern int HD_CloseComm();

        [DllImport("HDstdapi.dll")]
        public static extern int HD_Authenticate();

        [DllImport("HDstdapi.DLL", EntryPoint = "HD_Read_BaseMsg")]
        public static extern int HD_Read_BaseMsg(string pBmpFile, byte[] pName, byte[] pSex, byte[] pNation, byte[] pBirth, byte[] pAddress, byte[] pCertNo, byte[] pDepartment, byte[] pEffectData, byte[] pExpire); //读卡内信息///
        #endregion


        #region CUSTOM K80 dll
        [DllImport("CePrintLib.dll")]
        public static extern int CeInitUSBLayer(UInt32 phUsbContext, out uint dwSysErr);

        [DllImport("CePrintLib.dll")]
        public static extern int CeDeInitUSBLayer(IntPtr hUsbContext);

        [DllImport("CePrintLib.dll")]
        public static extern int CeOpenUSBDev(IntPtr hUSBContext, out string szDevName, out uint dwSysErr);

        [DllImport("CePrintLib.dll")]
        public static extern int CeWriteUSBDev(IntPtr hUSBContext, UInt16[] lpData, int dwSizData, ref int dwByteWritten, out uint dwSysErr);

        [DllImport("CePrintLib.dll")]
        public static extern int CeGetSts(IntPtr hPortContext, out uint dwStatus, out uint dwSysErr);        //Get printer Status
        #endregion

        #region  532打印机 兼容PM58T

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcOpenPrinter(int iPort, int baud, int hedshk);

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcClosePrinter();

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcPrintString(string szStr);

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcSetAbsoluPrintPosition(int nL, int nH);

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcPrintBitmap(string szBmpFile, int m);

        //行间距

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcSetLineSpace(int iSpace);

        //设定左边空白量, 左边空白量设置为 [(nL + nH ´ 256)* 0.125 毫米]
        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcSetLeftMargin(int nL, int nH);
        //行起点

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcSetBeginningPosition(int nNum);

        //部分切纸 1,49

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcSelectCutPaper(int mNum, int nNum);

        //打印一行

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcPrintFeedLine();

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcRealtimeGetStatus(byte iStatus);

        //int W
        //参数W 设定放大字符宽度的倍数，取值范围为０～７，默认值为０。
        //int H
        //参数H 设定放大字符高度的倍数，取值范围为０～７，默认值为０。

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcSelectCharacterSize(int W, int H);

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcSelectHRIPosition(int nNum);     //0:不打印条码号,1:条形码上方打印条码号,2:条形码下方打印条码号

        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcSetBarCodeHeight(int iNum);      //条形码打印高度,1--255,默认162;

        // int m
        //参数m 的取值范围为０～８或６５～７５。如果m 取０～８参数n 将被忽略。 CODE128(m=73)
        //int n
        //参数n的取值范围为１～２５５。
        //char * string
        //参数string表示一串要打印成条形码的规定范围内的字符。
        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcPrintBarCode(int m, int n, string str);  //打印条码


        [DllImport("ThLPrinterDLL.dll")]
        public static extern int GcSetBarCodeWidth(int nNum);       //条形码打印宽度,取值范围2--6,默认为3

        public static int OpenPrinter()
        {
            int i = Properties.Settings.Default.EpsonPrintPort;
            return GcOpenPrinter(i, 115200, 0);
        }

        public static int ClosePrinter()
        {
            return GcClosePrinter();
        }

        public static int PrintString(string str)
        {
            return GcPrintString(str);
        }

        public static void PrintString()
        {
            GcPrintFeedLine();
        }

        //获取打印机状态
        public static string GetPrinterStatus()
        {
            int i;
            string s = "";
            i = DllClass.GcRealtimeGetStatus(1);    //00010110
            if (i == -1)
            {
                s += "异常,";
            }
            else
            {
                s += "正常,";
            }
            i = DllClass.GcRealtimeGetStatus(2);
            if (i == -1)
            {
                s += "异常,";
            }
            else
            {
                s += "正常,";
            }
            i = DllClass.GcRealtimeGetStatus(3);
            if (i == -1)
            {
                s += "异常,";
            }
            else
            {
                s += "正常,";
            }
            i = DllClass.GcRealtimeGetStatus(4);
            if (i == -1)
            {
                s += "异常,";
            }
            else
            {
                s += "正常,";
            }
            i = DllClass.GcRealtimeGetStatus(5);
            if (i == -1)
            {
                s += "异常,";
            }
            else
            {
                s += "正常,";
            }
            return s;
        }
        #endregion


        #region 创自发卡器dll
        static IntPtr _ropen = new IntPtr(0);
        static Byte _toPosition = 0x31;
        static Byte _fromPosition = 0x30;
        static Byte AddrH = 0x30;
        static Byte Addrl = 0x30;
        private static byte _ss5;
        private static byte _ss4;
        private static byte _ss3;
        private static byte _ss2;
        private static byte _ss1;
        private static byte _ss0;
        private static byte _cardType;
        private static byte _cardInfor;

        [DllImport("CRT_580.dll")]
        private static extern IntPtr CommOpenWithBaut(string port, Int32 data);
        [DllImport("CRT_580.dll")]
        private static extern int CommClose(IntPtr comHandle);
        [DllImport("CRT_580.dll")]
        private static extern int CRT580_MoveCard(IntPtr comHandle, Byte addrH, Byte addrl, Byte toPosition, Byte fromPosition);
        [DllImport("CRT_580.dll")]
        private static extern int CRT580_GetStatus(IntPtr comHandle, Byte addrH, Byte addrl, ref Byte ss5, ref Byte ss4, ref Byte ss3, ref Byte ss2, ref Byte ss1, ref Byte ss0);
        [DllImport("CRT_580.dll")]
        private static extern int CRT580_Reset(IntPtr comHandle, Byte addrH, Byte addrl);
        [DllImport("CRT_580.dll")]
        private static extern int CRT_IC_DetectCard(IntPtr comHandle, Byte addrH, Byte addrl, ref Byte cardType, ref Byte cardInfor);
        [DllImport("CRT_580.dll")]
        private static extern int RF_GetCardID(IntPtr ComHandle, Byte _AddrH, Byte _Addrl, Byte[] _CardID);
        [DllImport("CRT_580.dll")]
        private static extern int RF_ReadBlock(IntPtr ComHandle, IntPtr _AddrH, Byte _Addrl, Byte _Sec, Byte _Block, Byte[] _BlockData);
        [DllImport("CRT_580.dll")]
        private static extern int IC24CXX_ReadBlock(IntPtr ComHandle, Byte _AddrH, Byte _Addrl, Byte _CardType, int _Address, Byte _dataLen, ref Byte[] _BlockData);
        [DllImport("CRT_580.dll")]
        private static extern int MC_ReadTrack(IntPtr ComHandle, Byte _AddrH, Byte _Addrl, Byte mode, int track, Byte[] TrackData, ref int TrackDataLen);
        public enum Fss5
        {
            停卡位为前端不持卡,
            停卡位为前端持卡,
            停卡位为射频卡卡机内操作位,
            停卡位为ic卡操作位
        }

        public enum Fss4
        {
            读卡器卡口禁止进卡 = 1,
            读卡器卡口允许磁卡进卡,
            读卡器卡口允许ic卡进卡,
            读卡器卡口允许磁信号进卡
        }

        public enum Fss3
        {
            读卡器内无卡,
            读卡器内有卡射频卡操作位,
            读卡器ic卡操作位有卡,
            读卡器卡口持卡位有卡,
            读卡器卡口不持卡位有卡,
            读卡器有长卡,
            读卡器有短卡,
            后端持卡位置有卡,
            后端不持卡位置有卡
        }

        public enum Fss2
        {
            发卡通道无卡,
            发卡通道预发卡位置有卡,
            发卡通道扩展操作位置有卡,
            发卡通道非正常位置有卡
        }

        public enum Fss1
        {
            发卡箱内卡足,
            发卡箱内卡少,
            发卡箱内无卡
        }

        public enum Fss0
        {
            收卡箱卡不满,
            收卡箱卡已满
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        public static int GetStatus(ref string result, ref Fss5 s5, ref Fss4 s4, ref Fss3 s3, ref Fss2 s2, ref Fss1 s1, ref Fss0 s0)
        {
            int retmc = CRT580_GetStatus(_ropen, AddrH, Addrl, ref _ss5, ref _ss4, ref _ss3, ref _ss2, ref _ss1, ref _ss0);
            if (retmc == 0)
            {
                s5 = (Fss5)Convert.ToInt32(_ss5.ToString("x2").Substring(1, 1));
                s4 = (Fss4)Convert.ToInt32(_ss4.ToString("x2").Substring(1, 1));
                s3 = (Fss3)Convert.ToInt32(_ss3.ToString("x2").Substring(1, 1));
                s2 = (Fss2)Convert.ToInt32(_ss2.ToString("x2").Substring(1, 1));
                s1 = (Fss1)Convert.ToInt32(_ss1.ToString("x2").Substring(1, 1));
                s0 = (Fss0)Convert.ToInt32(_ss0.ToString("x2").Substring(1, 1));
                result = "发卡机读取状态信息成功";
                return 0;
            }
            else
            {
                result = "发卡机读取状态信息失败";
                return -1;
            }
        }
        /// <summary>
        /// 移动卡
        /// </summary>
        /// <returns></returns>
        public static int MoveCardIn(ref string result)
        {
            _toPosition = 0x31;
            _fromPosition = 0x30;
            int retmc = CRT580_MoveCard(_ropen, AddrH, Addrl, _toPosition, _fromPosition);
            if (retmc == 0)
            {
                result = "发卡机走卡成功";
                return 0;
            }
            else
            {
                result = "发卡机走卡失败";
                return -1;
            }
        }
        /// <summary>
        /// 出卡
        /// </summary>
        /// <returns></returns>
        public static int OutCard(ref string result)
        {
            _toPosition = 0x33;
            _fromPosition = 0x31;
            int retmc = CRT580_MoveCard(_ropen, AddrH, Addrl, _toPosition, _fromPosition);
            if (retmc == 0)
            {
                result = "发卡机吐卡成功";
                return 0;
            }
            else
            {
                result = "发卡机吐卡失败";
                return -1;
            }
        }
        /// <summary>
        /// 回收卡
        /// </summary>
        /// <returns></returns>
        public static int BackCard()
        {
            _toPosition = 0x35;
            _fromPosition = 0x31;
            CRT580_MoveCard(_ropen, AddrH, Addrl, _toPosition, _fromPosition);
            return 0;
        }
        /// <summary>
        /// 测试卡片类型
        /// </summary>
        /// <returns></returns>
        public static int CRT_IC_DetectCard(ref string result)
        {
            int retmc = CRT_IC_DetectCard(_ropen, AddrH, Addrl, ref _cardType, ref _cardInfor);
            if (retmc == 0)
            {
                result = "测试发卡机中的卡片类型成功:" + Convert.ToInt32(_cardType.ToString("x2").Substring(1, 1)) + " " + Convert.ToInt32(_cardInfor.ToString("x2").Substring(1, 1));
                return 0;
            }
            else
            {
                result = "测试发卡机中的卡片类型失败";
                return -1;
            }
        }
        #endregion


        #region 金属密码键盘区开始
        public const int KEY_INVALID = 0XFFFF;
        public const byte[] CONST_BYTE = null;
        public const String DLL_PATH = "PinpadC.dll";//modify DLL DLL_PATH
        public const String PORT_INFO = "COM2:9600,N,8,1";//modify PINPAD COM INFO
        public const EPORT ePORT = EPORT.eCOM;//modify PINPAD COM INFO

        [DllImport("PinpadC.dll", EntryPoint = "AutoEnlargeKeyC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static void AutoEnlargeKeyC(System.Boolean bEnable);

        [DllImport(DLL_PATH, EntryPoint = "Open", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int Open(EPORT eDevice, EPIN_TYPE eType, string lpDescription);

        [DllImport(DLL_PATH, EntryPoint = "Init", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int Init(byte nMode = 0);

        [DllImport(DLL_PATH, EntryPoint = "Soft_Asc2Hex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int Soft_Asc2Hex(byte[] pHex, String pAscii, int dwLen); //ASCII to Hex(nReturn Hex length

        [DllImport(DLL_PATH, EntryPoint = "Soft_Hex2Asc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int Soft_Hex2Asc(byte[] pAscii, byte[] pHex, int dwLen); //Hex to ASCII

        [DllImport(DLL_PATH, EntryPoint = "AutoUpdatePassword", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int AutoUpdatePassword(EPSW_ID ePswId, String lpOldPSW, String lpNewPSW, String lpKeyboardCodes);

        [DllImport(DLL_PATH, EntryPoint = "RI_AutoRemoveInstall", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int RI_AutoRemoveInstall(EINSTALL_AUTH eMode, String lpPSW1, String lpPSW2, String lpKeyboardCodes);

        [DllImport(DLL_PATH, EntryPoint = "OpenKeyboardAndSound", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int OpenKeyboardAndSound(ESOUND eSound, ENTRYMODE eMode, int dwDisableKey = 0, int dwDisableFDK = 0);

        [DllImport(DLL_PATH, EntryPoint = "ReadText", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int ReadText(byte[] lpText, ref byte dwOutLen, int dwTimeOut = 2000);

        [DllImport(DLL_PATH, EntryPoint = "LoadKey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int LoadKey(int wKeyId, EKEYATTR dwKeyAttr, byte[] lpKey, int iKeyLen, int wEnKey = KEY_INVALID, EKEYMODE eMode = EKEYMODE.KEY_SET, EKCVMODE eKCV = EKCVMODE.KCVNONE, byte[] lpKCVRet = CONST_BYTE);

        [DllImport(DLL_PATH, EntryPoint = "ReadKeyAttribute", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int ReadKeyAttribute(int wKeyId, ref EKEYATTR lpKeyAttr, EKCVMODE eKCV = EKCVMODE.KCVNONE, byte[] lpKCVRet = CONST_BYTE);

        [DllImport(DLL_PATH, EntryPoint = "SetKCVLength", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int SetKCVLength(byte KCVL);

        [DllImport(DLL_PATH, EntryPoint = "StartPinInput", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int StartPinInput(ESOUND eSound, byte MaxLen = 6, byte MinLen = 4, bool bAutoEnd = true, byte TimeOut = 30);

        [DllImport(DLL_PATH, EntryPoint = "GetPinBlock", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int GetPinBlock(int wKeyId, byte PinLen, EPINFORMAT ePinFormat, String lpCardNo, byte[] pPinBlock, int wEnKeyId = KEY_INVALID, byte Padding = 0x0F);

        [DllImport(DLL_PATH, EntryPoint = "CalcMAC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int CalcMAC(int wKeyId, EMAC eMac, byte[] lpDataIn, int wInLen, byte[] lpOutData, int wMK1IV = KEY_INVALID);

        [DllImport(DLL_PATH, EntryPoint = "DESCrypt", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int DESCrypt(int wKeyId, ECRYPT eMode, byte[] lpDataIn, int wInLen, byte[] lpOutData, bool bEncrypt = true, int wMK1IV = KEY_INVALID);

        [DllImport(DLL_PATH, EntryPoint = "Close", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        // public static extern IntPtr KeybordClose();
        public extern static int KeybordClose();
        /*****************************************************************************
      * Function Description:  open the pinpad for use
      * Parameter: 
      * Return Value: 
      *****************************************************************************/
        public static int nOpenPinpad()
        {
            int nRet = 0;//nReturn value
            string str = "";
            //Set keys extending
            //Console.WriteLine("AutoEnlargeKeyC\n");
            str += "AutoEnlargeKeyC\n";
            AutoEnlargeKeyC(true);

            //Open pinpad
            nRet = Open(ePORT, EPIN_TYPE.ePIN_UNKNOWN, PORT_INFO);
            //Console.WriteLine("Open nRet is [{0}]\n", nRet);
            str += "Open nRet is " + nRet + "\n";
            /*if (0 != nRet)
            {
                Console.WriteLine("Open fail ,maybe the PINPAD COM INFO is wrong OR  the COM is using!\n\nThe program will going to close!\n");
                Console.Read();//input char to end the program
                return;
            }*/
            return nRet;
        }

        /*****************************************************************************
        * Function Description:  update the password and auth install or remove the pinpad , some pinpad unsport
        * Parameter: 
        *           install:the install or remove operation , true for insatll ,false for remove
        * Return Value: 
        *****************************************************************************/
        public static void nAUTHInstal(bool install)
        {
            int nRet = 0;
            String strDefaultPasword = "0000000000000000";//default password of pinpad
            String strPassWord4 = "4444444444444444";
            String strPassWord5 = "5555555555555555";
            //can't set the password1~5 as the same value as each other
            //while set g_pAutoEnlargeKey as TRUE, password 1-3 will be entered as "1111111111111111","2222222222222222","3333333333333333" in function Init()
            //password4,5 should not set the same value with password1~3

            //Console.WriteLine("Now is going to [{0}]", install ? "install" : "remove");

            //if the password in the pinpad is not the default,this functuin will not return success
            nRet = AutoUpdatePassword(EPSW_ID.PSW_ID4, strDefaultPasword, strPassWord4, null);
            //Console.WriteLine("AutoUpdatePassword PSW_ID4 nRet is [{0}]", nRet);

            //if the password in the pinpad is not the default,this functuin will not return success
            nRet = AutoUpdatePassword(EPSW_ID.PSW_ID5, strDefaultPasword, strPassWord5, null);
            //Console.WriteLine("AutoUpdatePassword PSW_ID5 nRet is [{0}]", nRet);

            nRet = RI_AutoRemoveInstall(install ? EINSTALL_AUTH.AUTH_INSTALL : EINSTALL_AUTH.AUTH_REMOVE, strPassWord4, strPassWord5, null);
            //Console.WriteLine("RI_AutoRemoveInstall nRet is [{0}]\n", nRet);
        }

        /*****************************************************************************
        * Function Description:  start text input and output the pinpad input 
        * Parameter: 
        * Return Value
        *****************************************************************************/
        public static string nTextInput()
        {
            int nRet = 0;
            int nInputLen = 18; ;//input length
            byte byLen = 0;
            string str = "";
            byte[] bypText = new byte[10];

            //open text input open sound
            nRet = OpenKeyboardAndSound(ESOUND.SOUND_OPEN, ENTRYMODE.ENTRY_MODE_TEXT);
            //Console.WriteLine("Open Keyboard And Sound nRet is [{0}]\n", nRet);

            //Console.WriteLine("Test the pinpad button, plase check your KeyCodes.\nPlease input 18 char by pinpad!<<");


            ReadText(bypText, ref byLen, 500);

            for (int i = 0; i < byLen && 0 < nInputLen; i++)
            {
                nInputLen--;
                switch (bypText[i])
                {
                    case 0x1B:
                        //Console.WriteLine("You pressed the [CANCEL] button.");
                        str = "CANCEL";
                        break;
                    case 0x08:
                        //Console.WriteLine("You pressed the [CLEAR] button.");
                        str = "CLEAR";
                        break;
                    case 0x0D:
                        // Console.WriteLine("You pressed the [ENTER] button.");
                        str = "ENTER";
                        break;
                    case 0x2F:
                        //Console.WriteLine("You pressed the [BLACK] button.");
                        str = "BLACK";
                        break;
                    case 0x2E:
                        //Console.WriteLine("You pressed the [.] button.");
                        str = ".";
                        break;
                    case 0x7F:
                        // Console.WriteLine("You pressed the [00] button.");
                        str = "00";
                        break;
                    default:
                        if (0x80 == bypText[i] || 0x81 == bypText[i] || 0x82 == bypText[i])
                        {
                            //Console.WriteLine("Input error, maybe the input is TIMEOUT or the button is stuck.\n");
                            str = "输入错误,超时,密码键盘关闭";
                            //Console.WriteLine("The input is going to close.\n");
                            nInputLen = 0;
                        }
                        else if (0 == bypText[i])//no input, for some pinpad
                        {
                            nInputLen++;
                        }
                        else
                        {
                            //Console.WriteLine("You pressed the [{0}] button", (char)bypText[i]);
                            str = ((char)bypText[i]).ToString();
                        }
                        break;
                }
            }

            //close the input
            nRet = OpenKeyboardAndSound(ESOUND.SOUND_CLOSE, ENTRYMODE.ENTRY_MODE_CLOSE);
            return str;
            //Console.WriteLine("Open Keyboard And Sound SOUND_CLOSE  ENTRY_MODE_CLOSE nRet is [{0}]\n", nRet);
        }

        /*****************************************************************************
       * Function Description:  load  master key , pin key , mac key and data key into pinpad
       * Parameter: 
       * Return Value: 
       *****************************************************************************/
        void nLoadKey()
        {
            int nRet = 0;
            int nKeyLen = 0;
            byte byKcvLen = 8;
            string strKeyAsc = "";              //key asc string
            byte[] bypKeyHex = new byte[48 + 1];//key hex string , for  LoadKey , the '1' is for the string end '\0'
            byte[] bypKcvHex = new byte[8 + 1];//kcv hex string , from LoadKey
            byte[] bypKcvAsc = new byte[16 + 1];//kcv asc string , for show

            nRet = SetKCVLength(byKcvLen);
            Console.WriteLine("Set KCV Length [{0}] nRet is [{1}]\n", byKcvLen, nRet);

            //load master key 1
            Console.WriteLine("LoadKey MASTER KEY 1 ...");
            strKeyAsc = "CE31B0C2D38034706861B0AE86CE91D0";
            //transform key value from ASCII TO HEX
            nKeyLen = Soft_Asc2Hex(bypKeyHex, strKeyAsc, strKeyAsc.Length);
            //load master key 1 , EnKeyId is 0xFFFF(load clear key) ,set key , KCV calculate encrypt zero
            nRet = LoadKey(1, EKEYATTR.ATTR_MK, bypKeyHex, nKeyLen, 0xFFFF, EKEYMODE.KEY_SET, EKCVMODE.KCVZERO, bypKcvHex);
            //transform KCV value from HEX TO ASCII
            Soft_Hex2Asc(bypKcvAsc, bypKcvHex, byKcvLen);
            //show KCV
            Console.WriteLine("LoadKey MASTER KEY 1 nRet is [{0}]  KCV is [{1}]\n", nRet, System.Text.Encoding.Default.GetString(bypKcvAsc).TrimEnd('\0'));

            //load pin key 2 , master key 1
            Console.WriteLine("LoadKey PIN KEY 2 MASTER KEY 1 ...");
            strKeyAsc = "358DA0A6507D8464E34A4EEDA6CD7740";
            nKeyLen = Soft_Asc2Hex(bypKeyHex, strKeyAsc, strKeyAsc.Length);
            //load PIN key 2 , EnKeyId is 1 ,set key , KCV calculate encrypt zero
            nRet = LoadKey(2, EKEYATTR.ATTR_PK, bypKeyHex, nKeyLen, 1, EKEYMODE.KEY_SET, EKCVMODE.KCVZERO, bypKcvHex);
            Soft_Hex2Asc(bypKcvAsc, bypKcvHex, byKcvLen);
            Console.WriteLine("LoadKey PIN KEY 2 nRet is [{0}]  KCV is [{1}]\n", nRet, System.Text.Encoding.Default.GetString(bypKcvAsc).TrimEnd('\0'));

            //load mac key 3 , master key 1
            Console.WriteLine("LoadKey MAC KEY 3 MASTER KEY 1 ...");
            strKeyAsc = "863A1602089DFB5B";
            nKeyLen = Soft_Asc2Hex(bypKeyHex, strKeyAsc, strKeyAsc.Length);
            nRet = LoadKey(3, EKEYATTR.ATTR_AK, bypKeyHex, nKeyLen, 1, EKEYMODE.KEY_SET, EKCVMODE.KCVZERO, bypKcvHex);
            Soft_Hex2Asc(bypKcvAsc, bypKcvHex, byKcvLen);
            Console.WriteLine("LoadKey MAC KEY 3 nRet is [{0}]  KCV is [{1}]\n", nRet, System.Text.Encoding.Default.GetString(bypKcvAsc).TrimEnd('\0'));

            //load mac key 4 , master key 1
            Console.WriteLine("LoadKey DATA KEY 4 MASTER KEY 1 ...");
            strKeyAsc = "987DA0A6507D8464E34A4EEDA6CD7740";
            nKeyLen = Soft_Asc2Hex(bypKeyHex, strKeyAsc, strKeyAsc.Length);
            nRet = LoadKey(4, EKEYATTR.ATTR_DK, bypKeyHex, nKeyLen, 1, EKEYMODE.KEY_SET, EKCVMODE.KCVZERO, bypKcvHex);
            Soft_Hex2Asc(bypKcvAsc, bypKcvHex, byKcvLen);
            Console.WriteLine("LoadKey DATA KEY 4 nRet is [{0}]  KCV is [{1}]\n", nRet, System.Text.Encoding.Default.GetString(bypKcvAsc).TrimEnd('\0'));
        }

        /*****************************************************************************
         * Function Description:  Read Key 1 Attribute
         * Parameter: 
         * Return Value: 
         *****************************************************************************/
        void nReadKeyAttribute()
        {
            int nRet = 0;
            EKEYATTR lpKeyAttr = EKEYATTR.ATTR_AK;
            byte[] bypKcvHex = new byte[8 + 1];//kcv hex string , from ReadKeyAttribute
            byte[] bypKcvAsc = new byte[16 + 1];//kcv asc string , for show

            //Read Key Attribute
            nRet = ReadKeyAttribute(1, ref lpKeyAttr, EKCVMODE.KCVZERO, bypKcvHex);
            //transform KCV value from HEX TO ASCII
            Soft_Hex2Asc(bypKcvAsc, bypKcvHex, 8);
            Console.WriteLine("ReadKeyAttribute nRet is [{0}] KCV is [{1}]\n", nRet, System.Text.Encoding.Default.GetString(bypKcvAsc).TrimEnd('\0'));
        }

        /*****************************************************************************
        * Function Description:  start pin input , get the input from pinpad (digit as "*" or "0") and cul the pinblock
        * Parameter: 
        * Return Value: 
        *****************************************************************************/
        void nPinBlock()
        {
            int nRet = 0;
            int byPinLen = 0; // input pin length , for GetPinBlock
            byte byLen = 0;
            string strCardNumber = "";//card number , for GetPinBlock
            byte[] bypPinInput = new byte[16 + 1];
            byte[] bypPinBlockHex = new byte[8 + 1];//pinblock hex string , from GetPinBlock
            byte[] bypPinBlockAsc = new byte[16 + 1];//pinblock ascii string , for show
            int i = 0;

            //open pin input , pen sound , max len is 6 , min len is 4 , auto ended if the input len is equal the max len 
            nRet = StartPinInput(ESOUND.SOUND_OPEN, 6, 4, true);
            Console.WriteLine("StartPinInput nRet is [{0}]", nRet);

            Console.WriteLine("Please input PIN.<<");

            nRet = 0;//for end the pin inpt , press the CANCEL buntton or ENTER buntton with len bewtten min len and max len

            while (0 <= nRet)//the pin input will end when input ENTER or the StartPinInput auto end it
            {
                ReadText(bypPinInput, ref byLen, 500);

                for (i = 0; i < byLen; i++)
                {
                    if (0x0D == bypPinInput[i])//the pin input will end when input ENTER or the StartPinInput auto end it
                    {
                        Console.WriteLine("You pressed the [ENTER] button.");
                        if (byPinLen >= 4)
                        {
                            Console.WriteLine("The pin input is end.");
                            nRet = -1;//break the input,end the pin input and calculate pinblock
                            break;
                        }
                        Console.WriteLine("The input pin is insufficient please continue the pin input.");
                    }
                    else if (0x1B == bypPinInput[i])
                    {
                        Console.WriteLine("You pressed the [CANCEL] button.");
                        Console.WriteLine("The input is going to close.");
                        nRet = -2;//break the input,end the pin input with no pinblock
                        break;
                    }
                    else if (0x80 == bypPinInput[i] || 0x81 == bypPinInput[i] || 0x82 == bypPinInput[i])
                    {
                        Console.WriteLine("Input error, maybe the input is TIMEOUT or the button is stuck.\n");
                        Console.WriteLine("The input is going to close.\n");
                        nRet = -3;//end the pin input with no pinblock
                        break;
                    }
                    else if (0x08 == bypPinInput[i])
                    {
                        Console.WriteLine("You pressed the [CLEAR] or [BACKSPACE] button.");
                        //close the input and reopen the pin input
                        OpenKeyboardAndSound(ESOUND.SOUND_CLOSE, ENTRYMODE.ENTRY_MODE_CLOSE);
                        Console.WriteLine("Open Keyboard And Sound SOUND_CLOSE  ENTRY_MODE_CLOSE nRet is [{0}]\n", nRet);
                        StartPinInput(ESOUND.SOUND_OPEN, 6, 4, true);
                        Console.WriteLine("StartPinInput  nRet is [{0}]\n", nRet);

                        byPinLen = 0;
                    }
                    else if (0x20 == bypPinInput[i])
                    {
                        Console.WriteLine("You pressed the [BLACK] button.");
                    }
                    else if (0x2E == bypPinInput[i])
                    {
                        Console.WriteLine("You pressed the [.] button.");
                    }
                    else if (0x7F == bypPinInput[i])
                    {
                        Console.WriteLine("You pressed the [00] button.");
                    }
                    else//count the PIN input, some pinpad's PIN return code can change
                    {
                        Console.WriteLine("You pressed the [{0}] button", (char)bypPinInput[i]);
                        byPinLen++;
                    }
                }
            }
            //close the input
            nRet = OpenKeyboardAndSound(ESOUND.SOUND_CLOSE, ENTRYMODE.ENTRY_MODE_CLOSE);
            Console.WriteLine("Open Keyboard And Sound SOUND_CLOSE  ENTRY_MODE_CLOSE nRet is [{0}]\n", nRet);

            if (-1 == nRet)//the pin input was ended by ENTER , get the pinblock
            {
                strCardNumber = "6217000130000004332";
                //calculate pinblock , use key 2 , pin len is byPinLen , pinblock format is FORMAT_ISO0 , the key's enkey is 1
                nRet = GetPinBlock(2, (byte)byPinLen, EPINFORMAT.FORMAT_ISO0, strCardNumber, bypPinBlockHex);
                //transform pinblock from HEX TO ASCII
                Soft_Hex2Asc(bypPinBlockAsc, bypPinBlockHex, 8);
                Console.WriteLine("GetPinBlock nRet is [{0}] PINBLOCK is [{1}]\n", nRet, System.Text.Encoding.Default.GetString(bypPinBlockAsc).TrimEnd('\0'));

            }
        }

        /*****************************************************************************
        * Function Description:  calculate MAC by pinpad
        * Parameter: 
        * Return Value: 
        *****************************************************************************/
        void nMAC()
        {
            int nRet = 0;
            int nDataLen = 0;//length of data for calculate MAC
            byte[] bypDataHex = new byte[2048];  //data hex string , for CalcMAC
            byte[] bypMacHex = new byte[8 + 1]; //MAC hex string , form CalcMAC
            byte[] bypMAacAsc = new byte[16 + 1];//MAC asc string , for show

            string data = "0200702006C020C098111962170001300000043320000000000000000001000337051000010012376217000130000004332D220262023310200000303030303232363431303531333031353436323030303131353642EA1CEBCF168FE32600000000000000001322000001000500";
            //transform data from ASCII TO HEX
            nDataLen = Soft_Asc2Hex(bypDataHex, data, data.Length);
            //calculate ndataLen length cpDataHex use MAC_BANKSYS , with key 3 and no IV
            nRet = CalcMAC(3, EMAC.MAC_BANKSYS, bypDataHex, nDataLen, bypMacHex);
            //transform MAC from HEX TO ASCII
            Soft_Hex2Asc(bypMAacAsc, bypMacHex, 8);
            Console.WriteLine("MAC nRet is [{0}] MAC is [{1}]\n", nRet, System.Text.Encoding.Default.GetString(bypMAacAsc).TrimEnd('\0'));
        }

        /*****************************************************************************
        * Function Description:  encrypt and decrypt by pinpad
        * Parameter: 
        * Return Value: 
        *****************************************************************************/
        void nCrypt()
        {
            int nRet = 0;
            int nDataLen = 0;
            byte[] bypDataHex = new byte[16 + 1];//data hex string , for crypt
            byte[] bypResultHex = new byte[16 + 1]; //crypt result hex string , from crypt , the len should increase with the data len
            byte[] bypResultAsc = new byte[32 + 1];//crypt result asc string , for show , the len should increase with the data len
            String strDataAsc = "";

            //encypt the data
            strDataAsc = "EF34A0A6507D8464E34A4EEDA6CD7740";
            Console.WriteLine("ENCRYPT [{0}]", strDataAsc);
            nDataLen = Soft_Asc2Hex(bypDataHex, strDataAsc, strDataAsc.Length);
            //Encrypt cpDataHex with key 4 and no IV by CRYPT_DESECB
            nRet = DESCrypt(4, ECRYPT.CRYPT_DESECB, bypDataHex, nDataLen, bypResultHex, true);
            Soft_Hex2Asc(bypResultAsc, bypResultHex, bypResultHex.Length - 1);
            Console.WriteLine("nRet is [{0}] nciphertext is [{1}]\n", nRet, System.Text.Encoding.Default.GetString(bypResultAsc).TrimEnd('\0'));

            //decrypt the data
            Console.WriteLine("DECRYPT [{0}]", System.Text.Encoding.Default.GetString(bypResultAsc).TrimEnd('\0'));
            bypResultHex.CopyTo(bypDataHex, 0);
            nDataLen = bypDataHex.Length - 1;
            //decrypt cpDataHex with key 4 and no IV by CRYPT_DESECB
            nRet = DESCrypt(4, ECRYPT.CRYPT_DESECB, bypDataHex, nDataLen, bypResultHex, false);
            Soft_Hex2Asc(bypResultAsc, bypResultHex, bypResultHex.Length - 1);
            Console.WriteLine("nRet is [{0}] cleartext is [{1}]\n", nRet, System.Text.Encoding.Default.GetString(bypResultAsc).TrimEnd('\0'));
        }
        #endregion
        
    }



}