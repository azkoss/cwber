VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "32位动态库调用示例 "
   ClientHeight    =   7170
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   6465
   BeginProperty Font 
      Name            =   "宋体"
      Size            =   9
      Charset         =   134
      Weight          =   700
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   7170
   ScaleWidth      =   6465
   StartUpPosition =   2  'CenterScreen
   WhatsThisHelp   =   -1  'True
   Begin VB.CommandButton Command7 
      Caption         =   "修改密码"
      BeginProperty Font 
         Name            =   "宋体"
         Size            =   12
         Charset         =   134
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   420
      Left            =   4560
      TabIndex        =   7
      Top             =   2640
      Width           =   1455
   End
   Begin VB.CommandButton Command6 
      Caption         =   "设备函数"
      BeginProperty Font 
         Name            =   "宋体"
         Size            =   12
         Charset         =   134
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   495
      Left            =   4560
      TabIndex        =   6
      Top             =   3240
      Width           =   1455
   End
   Begin VB.CommandButton Command5 
      Caption         =   "SAM卡操作"
      BeginProperty Font 
         Name            =   "宋体"
         Size            =   12
         Charset         =   134
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   420
      Left            =   4560
      TabIndex        =   5
      Top             =   3960
      Width           =   1455
   End
   Begin VB.CommandButton Command4 
      Caption         =   "退出"
      BeginProperty Font 
         Name            =   "宋体"
         Size            =   12
         Charset         =   134
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   420
      Left            =   4560
      TabIndex        =   4
      Top             =   6000
      Width           =   1455
   End
   Begin VB.CommandButton Command3 
      Caption         =   "卡函数"
      BeginProperty Font 
         Name            =   "宋体"
         Size            =   12
         Charset         =   134
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   420
      Left            =   4560
      TabIndex        =   3
      Top             =   1920
      Width           =   1455
   End
   Begin VB.CommandButton Command2 
      Caption         =   "装载密码"
      BeginProperty Font 
         Name            =   "宋体"
         Size            =   12
         Charset         =   134
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   420
      Left            =   4560
      TabIndex        =   2
      Top             =   1200
      Width           =   1455
   End
   Begin VB.ListBox List1 
      BackColor       =   &H00FFFFC0&
      Height          =   6360
      Left            =   360
      TabIndex        =   1
      Top             =   360
      Width           =   3615
   End
   Begin VB.CommandButton Command1 
      Caption         =   "初始化"
      BeginProperty Font 
         Name            =   "宋体"
         Size            =   12
         Charset         =   134
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   420
      Left            =   4560
      TabIndex        =   0
      Top             =   480
      Width           =   1455
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Dim hexkey As String * 12
Dim snr, tagtype As Long
Dim data32 As String * 32
Dim databuff32 As String * 32
Dim alldata As String * 3000
Dim rvalue As Long
Dim wvalue As Long
Dim cardmode As Integer
Dim loadmode As Integer
Dim sector As Integer
Dim displaydata(8) As Byte
Dim address As Integer  '写16字节数据 地址
Dim size As Byte
Dim block As Integer  '值操作 地址

Private Sub Command1_Click()
If icdev < 0 Then
  icdev = dc_init(100, 115200)   '这里初始化串口一，波特率为115200
End If
If icdev < 0 Then
      List1.AddItem ("调用 dc_init()函数出错!")
     Exit Sub
End If
List1.AddItem ("调用 dc_init()函数成功!")
End Sub


Private Sub Command2_Click()
hexkey = "ffffffffffff"
st = dc_load_key_hex(ByVal icdev, 0, 1, ByVal hexkey)  '装载1扇区的密码到读写器，这和后面要
If st <> 0 Then                                        '校验1扇区的密码是相对应的
      List1.AddItem ("调用 dc_Load_key_hex()函数出错!")
      Exit Sub
End If
List1.AddItem ("调用 dc_Load_key_hex()函数成功!")
End Sub

Private Sub Command3_Click()
cardmode = 0
sector = 1
address = 4
block = 5


st = dc_card(ByVal icdev, cardmode, snr)   '寻卡函数 cardmode=1 可对卡连续操作，为0时如果执行了HALT操作
If st <> 0 And st <> 1 Then                '则卡必须离开感应区后才能再次操作
      List1.AddItem ("调用 dc_card()函数出错,请检查设备连接是否正常")
      Exit Sub
End If
If st = 1 Then
       List1.AddItem ("没有找到卡片")
      Exit Sub
End If
List1.AddItem ("感应区内发现了卡片")
List1.AddItem Str(snr)

DoEvents


st = dc_authentication(ByVal icdev, 0, sector)  '核对卡密码
If st <> 0 Then
      List1.AddItem "调用 dc_authentication()函数出错!"
      Exit Sub
End If
List1.AddItem "调用 dc_authentication()函数成功!"
DoEvents

st = dc_read_hex(ByVal icdev, address, databuff32)   '读卡内的数据
If st <> 0 Then
     List1.AddItem "调用 dc_read_hex函数出错"
     Exit Sub

End If
List1.AddItem "调用 dc_read_hex函数成功"
List1.AddItem databuff32
DoEvents

data32 = "1234567890abcdef1234567890abcdef"
st = dc_write_hex(ByVal icdev, address, ByVal data32)  '数据写入卡片中
If st <> 0 Then
      List1.AddItem "调用 dc_write_hex()函数出错!"
      Exit Sub
End If

List1.AddItem "调用 dc_write_hex()函数成功!"
DoEvents

''data32 = "1234567890abcdef"
data32 = "1234567890abcdef1234567890abcdef"

st = dc_check_writehex(icdev, snr, 0, address, data32) ''检测指定数据是否与卡中数据一致
If st <> 0 Then
      List1.AddItem "调用 dc_check_writehex()函数出错!"
      Exit Sub
End If
List1.AddItem "调用 dc_check_writehex()函数成功!"
DoEvents





'以下是把扇区的数据块当作一个值块来操作
wvalue = 10000
st = dc_initval(ByVal icdev, block, ByVal wvalue)  '初始化值
If st <> 0 Then
      List1.AddItem "调用 dc_initval()函数出错"
      Exit Sub
End If
List1.AddItem "调用 dc_initval()函数成功"
DoEvents

 


st = dc_increment(ByVal icdev, block, 888)    '加值
If st <> 0 Then
      List1.AddItem "调用 dc_increment()函数出错"
      Exit Sub
End If
List1.AddItem "调用 dc_increment()函数成功"
DoEvents

st = dc_decrement(ByVal icdev, block, 88)    '减值
If st <> 0 Then
      List1.AddItem "调用 dc_decrement()函数出错"
      Exit Sub
End If
List1.AddItem "调用 dc_decrement()函数成功"
DoEvents

st = dc_readval(ByVal icdev, block, rvalue)    '读值
If st <> 0 Then
      List1.AddItem "调用 dc_readval()函数出错"
      Exit Sub
End If
List1.AddItem "调用 dc_readval()函数成功"
List1.AddItem rvalue
DoEvents

'
st = dc_halt(ByVal icdev)                    '该函数执行后，如果卡不离开感应区不能再次操作
If st <> 0 Then
      List1.AddItem "调用 dc_halt()函数出错"
      Exit Sub
End If
List1.AddItem "调用 dc_halt()函数成功"

DoEvents

List1.AddItem "读写设备测试通过!"

End Sub

Private Sub Command4_Click()
quit
Unload Me
End Sub

Private Sub Command5_Click()
Dim data1 As String * 1024
Dim data2 As String * 1024
Dim slen As Byte
Dim rlen As Byte

st = dc_cpureset_hex(ByVal icdev, rlen, data1)
If st <> 0 Then
      List1.AddItem ("调用 dc_cpureset函数出错")
      Exit Sub
End If
List1.AddItem ("调用 dc_cpureset()函数成功")
List1.AddItem data1


slen = 5
data1 = "0084000008"
st = dc_cpuapdu_hex(icdev, ByVal slen, ByVal data1, rlen, ByVal data2)
If st <> 0 Then
      List1.AddItem ("调用 dc_cpuapdu函数出错")
      Exit Sub
End If
List1.AddItem ("调用 dc_cpuapdu()函数成功")
List1.AddItem data2

st = dc_cpudown(icdev)
If st <> 0 Then
      List1.AddItem ("调用 dc_cpudown函数出错")
      Exit Sub
End If
List1.AddItem ("调用 dc_cpudown函数成功")


End Sub

Private Sub Command6_Click()
Dim dispdata(8) As Byte
Dim ctt(7) As Byte
dispdata(0) = &H44
dispdata(1) = &H46
dispdata(2) = &H5
dispdata(3) = &H5
dispdata(4) = &H85
dispdata(5) = &H5
dispdata(6) = &H5F

st = dc_gettime(icdev, ctt(0))
If st <> 0 Then
      List1.AddItem "Gettime Error"
      Exit Sub
End If
List1.AddItem "gettime ok"



ctt(0) = &H4
ctt(1) = &H2
ctt(2) = &H5
ctt(3) = &H15
ctt(4) = &H4
ctt(5) = &H3
ctt(6) = &H2
st = dc_settime(icdev, ctt(0))
If st <> 0 Then
    List1.AddItem "settime Error"
    Exit Sub
End If
List1.AddItem "settime ok"
Exit Sub

    


st = dc_high_disp(icdev, 1, 5, dispdata(0))


st = dc_beep(ByVal icdev, 10)
If st <> 0 Then
      List1.AddItem ("调用 dc_beep()函数出错")
      Exit Sub
End If
List1.AddItem ("调用 dc_beep()函数成功")
DoEvents


data32 = "1122334455"
st = dc_swr_eepromhex(ByVal icdev, 100, 5, data32)
If st <> 0 Then
      List1.AddItem ("调用 dc_swr_eepromhex()函数出错")
      Exit Sub
End If
List1.AddItem ("调用 dc_swr_eepromhex()函数成功")
DoEvents

st = dc_srd_eepromhex(ByVal icdev, 100, 5, databuff32)
If st <> 0 Then
      List1.AddItem ("调用 dc_srd_eepromhex()函数出错")
      Exit Sub
End If
List1.AddItem ("调用 dc_srd_eepromhex()函数成功")
DoEvents




st = dc_disp_str(ByVal icdev, "a.b.c.d.e.F.12")
If st <> 0 Then
      List1.AddItem ("调用 dc_disp_str()函数出错")
      Exit Sub
End If
List1.AddItem ("调用 dc_disp_str()函数成功")
DoEvents
End Sub

Private Sub Command7_Click()
Dim akey(6) As Byte
Dim bkey(6) As Byte
'其实修改密码的操作就是用dc_write函数来写每个扇区的密码块，dc_changeb3是封装了dc_write操作
' dc_changeb3 (ByVal adr As Long, ByVal secer As Integer, ByRef KeyA As Byte, ByVal B0 As Integer, ByVal B1 As Integer, ByVal B2 As Integer, ByVal B3 As Integer, ByVal Bk As Integer, ByRef KeyB As Byte) As Integer

sector = 1
cardmode = 1
st = 0
st = dc_card(ByVal icdev, cardmode, snr)
If st <> 0 Then
      List1.AddItem "调用 dc_card()函数出错!。请退出"
      Exit Sub
End If

st = dc_authentication(ByVal icdev, 0, sector)
If st <> 0 Then
      List1.AddItem "调用 dc_authentication()函数出错!请退出。"
      Exit Sub
End If
 

'修改密码和控制位

akey(0) = &HFF 'A0
akey(1) = &HFF 'A1
akey(2) = &HFF 'A2
akey(3) = &HFF 'A3
akey(4) = &HFF 'A4
akey(5) = &HFF 'A5

bkey(0) = &HFF 'B0
bkey(1) = &HFF 'B1
bkey(2) = &HFF 'B2
bkey(3) = &HFF 'B3
bkey(4) = &HFF 'B4
bkey(5) = &HFF 'B5

'下列控制位参数值保证与原密码和控制位一致，一般建议不要改参数3，4，5，6，7
st = dc_changeb3(ByVal icdev, sector, akey(0), 0, 0, 0, 1, 0, bkey(0))
If st <> 0 Then
    List1.AddItem "调用 dc_changeb3()函数出错!请退出。"
    Exit Sub
End If
List1.AddItem "修改密码成功！！！"
End Sub



Private Sub Form_Load()

icdev = -1
End Sub

Private Sub Form_Unload(Cancel As Integer)
quit
End Sub
