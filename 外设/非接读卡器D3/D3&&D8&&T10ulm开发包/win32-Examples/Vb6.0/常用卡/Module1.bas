Attribute VB_Name = "Module1"
Option Explicit
Global icdev As Long
Global st As Integer


'comm function
Declare Function dc_init Lib "dcrf32.dll" (ByVal port%, ByVal baud&) As Long
Declare Function dc_exit Lib "dcrf32.dll" (ByVal icdev&) As Integer
Declare Function dc_card Lib "dcrf32.dll" (ByVal icdev&, ByVal mode%, snr&) As Integer
Declare Function dc_beep Lib "dcrf32.dll" (ByVal icdev&, ByVal mtime%) As Integer
Declare Function dc_reset Lib "dcrf32.dll" (ByVal icdev&, ByVal mtime%) As Integer
Declare Function dc_config_card Lib "dcrf32.dll" (ByVal icdev&, ByVal cardtype%) As Integer

Declare Function dc_load_key_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal mode%, ByVal secnr%, ByVal nkey$) As Integer
Declare Function dc_authentication Lib "crf32.dll" (ByVal icdev&, ByVal mode%, ByVal scenr%) As Integer
Declare Function dc_read_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal adr%, ByVal sdata$) As Integer
Declare Function dc_write_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal adr%, ByVal sdata$) As Integer

Declare Function dc_pro_reset_hex Lib "dcrf32.dll" (ByVal icdev&, rlen%, ByVal sdata$) As Integer
Declare Function dc_pro_commandhex Lib "dcrf32.dll" (ByVal icdev&, ByVal slen%, ByVal sendbuffer$, rlen%, _
               ByVal databuffer$, ByVal timeout%) As Integer
                                    
Declare Function dc_card_b_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal snr$) As Integer


Declare Function dc_setcpu Lib "dcrf32.dll" (ByVal icdev&, ByVal position%) As Integer
Declare Function dc_cpureset_hex Lib "dcrf32.dll" (ByVal icdev&, rlen%, ByVal databuffer$) As Integer
Declare Function dc_cpuapdu_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal slen%, ByVal sendbuffer$, _
                                  rlen%, ByVal databuffer$) As Integer
                                 
Declare Function dc_read_4442_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal offset%, ByVal lenth%, ByVal buffer$) As Integer
Declare Function dc_write_4442_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal offset%, ByVal lenth%, ByVal buffer$) As Integer
Declare Function dc_verifypin_4442_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal passwd$) As Integer
Declare Function dc_readpin_4442_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal passwd$) As Integer
Declare Function dc_readpincount_4442 Lib "dcrf32.dll" (ByVal icdev&) As Integer
Declare Function dc_changepin_4442_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal passwd$) As Integer

Declare Function dc_read_4428_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal offset%, ByVal lenth%, ByVal buffer$) As Integer
Declare Function dc_write_4428_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal offset%, ByVal lenth%, ByVal buffer$) As Integer
Declare Function dc_verifypin_4428_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal passwd$) As Integer
Declare Function dc_readpin_4428_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal passwd$) As Integer
Declare Function dc_readpincount_4428 Lib "dcrf32.dll" (ByVal icdev&) As Integer
Declare Function dc_changepin_4428_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal passwd$) As Integer
Declare Function dc_read_24c_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal offset%, ByVal lenth%, ByVal buffer$) As Integer
Declare Function dc_write_24c_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal offset%, ByVal lenth%, ByVal buffer$) As Integer
Declare Function dc_read_24c64_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal offset%, ByVal lenth%, ByVal buffer$) As Integer
Declare Function dc_write_24c64_hex Lib "dcrf32.dll" (ByVal icdev&, ByVal offset%, ByVal lenth%, ByVal buffer$) As Integer

                                 

Sub quit()
    If icdev > 0 Then
       st = dc_exit(icdev)
       icdev = -1
    End If
End Sub


