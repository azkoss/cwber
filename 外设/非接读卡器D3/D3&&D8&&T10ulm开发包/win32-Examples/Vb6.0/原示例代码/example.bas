Attribute VB_Name = "Module1"
Option Explicit
Global icdev As Long
Global st As Integer


'comm function
Declare Function add_s Lib "dcrf32.dll" (ByVal i%) As Integer

Declare Function dc_init Lib "dcrf32.dll" (ByVal port%, ByVal baud As Long) As Long
Declare Function dc_exit Lib "dcrf32.dll" (ByVal icdev As Long) As Integer
Declare Function dc_request Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode%, tagtype As Long) As Integer
Declare Function dc_anticoll Lib "dcrf32.dll" (ByVal icdev As Long, ByVal bcnt%, snr As Long) As Integer
Declare Function dc_select Lib "dcrf32.dll" (ByVal icdev As Long, ByVal snr As Long, size As Byte) As Integer
Declare Function dc_card Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode%, snr As Long) As Integer
Declare Function dc_load_key Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode%, ByVal secnr%, ByRef nkey As Byte) As Integer
Declare Function dc_load_key_hex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode%, ByVal secnr%, ByVal nkey As String) As Integer
Declare Function dc_authentication Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode%, ByVal scenr%) As Integer
Declare Function dc_read Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, ByVal sdata$) As Integer
Declare Function dc_read_hex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, ByVal sdata$) As Integer
Declare Function dc_write Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, ByVal sdata$) As Integer
Declare Function dc_write_hex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, ByVal sdata$) As Integer
Declare Function dc_changeb3 Lib "dcrf32.dll" (ByVal adr As Long, ByVal secer As Integer, ByRef KeyA As Byte, ByVal B0 As Integer, ByVal B1 As Integer, ByVal B2 As Integer, ByVal B3 As Integer, ByVal Bk As Integer, ByRef KeyB As Byte) As Integer
Declare Function dc_read_allhex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal sdata$) As Integer
Declare Function dc_write_allhex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal sdata$) As Integer
Declare Function dc_set_autoflag Lib "dcrf32.dll" (ByVal icdev As Long, ByVal flag%) As Integer
Declare Function dc_check_writehex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal cardid As Long, ByVal mode As Integer, ByVal adr%, ByVal sdata$) As Integer


Declare Function dc_HL_initval Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode As Integer, ByVal adr%, ByVal value As Long, ByRef snr As Long) As Integer
Declare Function dc_HL_increment Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode As Integer, ByVal adr%, ByVal value As Long, ByVal snr As Long, value As Long, ByRef snr As Long) As Integer
Declare Function dc_HL_decrement Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode As Integer, ByVal adr%, ByVal value As Long, ByVal snr As Long, value As Long, ByRef snr As Long) As Integer

'
Declare Function dc_initval Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, ByVal value As Long) As Integer
Declare Function dc_readval Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, value As Long) As Integer
Declare Function dc_increment Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, ByVal value As Long) As Integer
Declare Function dc_decrement Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, ByVal value As Long) As Integer
Declare Function dc_restore Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%) As Integer
Declare Function dc_transfer Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%) As Integer
Declare Function dc_halt Lib "dcrf32.dll" (ByVal icdev As Long) As Integer
 
'CPU card
Declare Function dc_cpureset_hex Lib "dcrf32.dll" (ByVal icdev As Long, ByRef rlen As Byte, ByVal sdata$) As Integer
Declare Function dc_cpudown Lib "dcrf32.dll" (ByVal icdev As Long) As Integer
Declare Function dc_cpuapdu_hex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal slen%, ByVal senddata$, ByRef rlen As Byte, ByVal recdata As String) As Integer
'device fuction
Declare Function dc_srd_eepromhex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, ByVal lenth%, ByVal sdata$) As Integer
Declare Function dc_swr_eepromhex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal adr%, ByVal lenth%, ByVal sdata$) As Integer


Declare Function dc_gettime Lib "dcrf32.dll" (ByVal icdev As Long, ctime As Byte) As Integer
Declare Function dc_settime Lib "dcrf32.dll" (ByVal icdev As Long, ctime As Byte) As Integer


Declare Function dc_ctl_mode Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode%) As Integer
Declare Function dc_disp_mode Lib "dcrf32.dll" (ByVal icdev As Long, ByVal mode%) As Integer
Declare Function dc_reset Lib "dcrf32.dll" (ByVal icdev As Long, ByVal msec%) As Integer
Declare Function dc_disp_str Lib "dcrf32.dll" (ByVal icdev As Long, ByVal sdata$) As Integer
Declare Function dc_high_disp Lib "dcrf32.dll" (ByVal icdev As Long, ByVal offset As Integer, ByVal displen As Integer, ByRef dispdata As Byte) As Integer
Declare Function dc_light Lib "dcrf32.dll" (ByVal icdev As Long, ByVal onoff%) As Integer
Declare Function dc_beep Lib "dcrf32.dll" (ByVal icdev As Long, ByVal time1 As Integer) As Integer


Sub quit()
    If icdev > 0 Then
       st = dc_exit(icdev)
       icdev = -1
    End If
End Sub


