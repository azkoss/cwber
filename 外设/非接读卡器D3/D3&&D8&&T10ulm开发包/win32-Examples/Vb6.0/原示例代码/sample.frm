VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "32λ��̬�����ʾ�� "
   ClientHeight    =   7170
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   6465
   BeginProperty Font 
      Name            =   "����"
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
      Caption         =   "�޸�����"
      BeginProperty Font 
         Name            =   "����"
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
      Caption         =   "�豸����"
      BeginProperty Font 
         Name            =   "����"
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
      Caption         =   "SAM������"
      BeginProperty Font 
         Name            =   "����"
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
      Caption         =   "�˳�"
      BeginProperty Font 
         Name            =   "����"
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
      Caption         =   "������"
      BeginProperty Font 
         Name            =   "����"
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
      Caption         =   "װ������"
      BeginProperty Font 
         Name            =   "����"
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
      Caption         =   "��ʼ��"
      BeginProperty Font 
         Name            =   "����"
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
Dim address As Integer  'д16�ֽ����� ��ַ
Dim size As Byte
Dim block As Integer  'ֵ���� ��ַ

Private Sub Command1_Click()
If icdev < 0 Then
  icdev = dc_init(100, 115200)   '�����ʼ������һ��������Ϊ115200
End If
If icdev < 0 Then
      List1.AddItem ("���� dc_init()��������!")
     Exit Sub
End If
List1.AddItem ("���� dc_init()�����ɹ�!")
End Sub


Private Sub Command2_Click()
hexkey = "ffffffffffff"
st = dc_load_key_hex(ByVal icdev, 0, 1, ByVal hexkey)  'װ��1���������뵽��д������ͺ���Ҫ
If st <> 0 Then                                        'У��1���������������Ӧ��
      List1.AddItem ("���� dc_Load_key_hex()��������!")
      Exit Sub
End If
List1.AddItem ("���� dc_Load_key_hex()�����ɹ�!")
End Sub

Private Sub Command3_Click()
cardmode = 0
sector = 1
address = 4
block = 5


st = dc_card(ByVal icdev, cardmode, snr)   'Ѱ������ cardmode=1 �ɶԿ�����������Ϊ0ʱ���ִ����HALT����
If st <> 0 And st <> 1 Then                '�򿨱����뿪��Ӧ��������ٴβ���
      List1.AddItem ("���� dc_card()��������,�����豸�����Ƿ�����")
      Exit Sub
End If
If st = 1 Then
       List1.AddItem ("û���ҵ���Ƭ")
      Exit Sub
End If
List1.AddItem ("��Ӧ���ڷ����˿�Ƭ")
List1.AddItem Str(snr)

DoEvents


st = dc_authentication(ByVal icdev, 0, sector)  '�˶Կ�����
If st <> 0 Then
      List1.AddItem "���� dc_authentication()��������!"
      Exit Sub
End If
List1.AddItem "���� dc_authentication()�����ɹ�!"
DoEvents

st = dc_read_hex(ByVal icdev, address, databuff32)   '�����ڵ�����
If st <> 0 Then
     List1.AddItem "���� dc_read_hex��������"
     Exit Sub

End If
List1.AddItem "���� dc_read_hex�����ɹ�"
List1.AddItem databuff32
DoEvents

data32 = "1234567890abcdef1234567890abcdef"
st = dc_write_hex(ByVal icdev, address, ByVal data32)  '����д�뿨Ƭ��
If st <> 0 Then
      List1.AddItem "���� dc_write_hex()��������!"
      Exit Sub
End If

List1.AddItem "���� dc_write_hex()�����ɹ�!"
DoEvents

''data32 = "1234567890abcdef"
data32 = "1234567890abcdef1234567890abcdef"

st = dc_check_writehex(icdev, snr, 0, address, data32) ''���ָ�������Ƿ��뿨������һ��
If st <> 0 Then
      List1.AddItem "���� dc_check_writehex()��������!"
      Exit Sub
End If
List1.AddItem "���� dc_check_writehex()�����ɹ�!"
DoEvents





'�����ǰ����������ݿ鵱��һ��ֵ��������
wvalue = 10000
st = dc_initval(ByVal icdev, block, ByVal wvalue)  '��ʼ��ֵ
If st <> 0 Then
      List1.AddItem "���� dc_initval()��������"
      Exit Sub
End If
List1.AddItem "���� dc_initval()�����ɹ�"
DoEvents

 


st = dc_increment(ByVal icdev, block, 888)    '��ֵ
If st <> 0 Then
      List1.AddItem "���� dc_increment()��������"
      Exit Sub
End If
List1.AddItem "���� dc_increment()�����ɹ�"
DoEvents

st = dc_decrement(ByVal icdev, block, 88)    '��ֵ
If st <> 0 Then
      List1.AddItem "���� dc_decrement()��������"
      Exit Sub
End If
List1.AddItem "���� dc_decrement()�����ɹ�"
DoEvents

st = dc_readval(ByVal icdev, block, rvalue)    '��ֵ
If st <> 0 Then
      List1.AddItem "���� dc_readval()��������"
      Exit Sub
End If
List1.AddItem "���� dc_readval()�����ɹ�"
List1.AddItem rvalue
DoEvents

'
st = dc_halt(ByVal icdev)                    '�ú���ִ�к���������뿪��Ӧ�������ٴβ���
If st <> 0 Then
      List1.AddItem "���� dc_halt()��������"
      Exit Sub
End If
List1.AddItem "���� dc_halt()�����ɹ�"

DoEvents

List1.AddItem "��д�豸����ͨ��!"

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
      List1.AddItem ("���� dc_cpureset��������")
      Exit Sub
End If
List1.AddItem ("���� dc_cpureset()�����ɹ�")
List1.AddItem data1


slen = 5
data1 = "0084000008"
st = dc_cpuapdu_hex(icdev, ByVal slen, ByVal data1, rlen, ByVal data2)
If st <> 0 Then
      List1.AddItem ("���� dc_cpuapdu��������")
      Exit Sub
End If
List1.AddItem ("���� dc_cpuapdu()�����ɹ�")
List1.AddItem data2

st = dc_cpudown(icdev)
If st <> 0 Then
      List1.AddItem ("���� dc_cpudown��������")
      Exit Sub
End If
List1.AddItem ("���� dc_cpudown�����ɹ�")


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
      List1.AddItem ("���� dc_beep()��������")
      Exit Sub
End If
List1.AddItem ("���� dc_beep()�����ɹ�")
DoEvents


data32 = "1122334455"
st = dc_swr_eepromhex(ByVal icdev, 100, 5, data32)
If st <> 0 Then
      List1.AddItem ("���� dc_swr_eepromhex()��������")
      Exit Sub
End If
List1.AddItem ("���� dc_swr_eepromhex()�����ɹ�")
DoEvents

st = dc_srd_eepromhex(ByVal icdev, 100, 5, databuff32)
If st <> 0 Then
      List1.AddItem ("���� dc_srd_eepromhex()��������")
      Exit Sub
End If
List1.AddItem ("���� dc_srd_eepromhex()�����ɹ�")
DoEvents




st = dc_disp_str(ByVal icdev, "a.b.c.d.e.F.12")
If st <> 0 Then
      List1.AddItem ("���� dc_disp_str()��������")
      Exit Sub
End If
List1.AddItem ("���� dc_disp_str()�����ɹ�")
DoEvents
End Sub

Private Sub Command7_Click()
Dim akey(6) As Byte
Dim bkey(6) As Byte
'��ʵ�޸�����Ĳ���������dc_write������дÿ������������飬dc_changeb3�Ƿ�װ��dc_write����
' dc_changeb3 (ByVal adr As Long, ByVal secer As Integer, ByRef KeyA As Byte, ByVal B0 As Integer, ByVal B1 As Integer, ByVal B2 As Integer, ByVal B3 As Integer, ByVal Bk As Integer, ByRef KeyB As Byte) As Integer

sector = 1
cardmode = 1
st = 0
st = dc_card(ByVal icdev, cardmode, snr)
If st <> 0 Then
      List1.AddItem "���� dc_card()��������!�����˳�"
      Exit Sub
End If

st = dc_authentication(ByVal icdev, 0, sector)
If st <> 0 Then
      List1.AddItem "���� dc_authentication()��������!���˳���"
      Exit Sub
End If
 

'�޸�����Ϳ���λ

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

'���п���λ����ֵ��֤��ԭ����Ϳ���λһ�£�һ�㽨�鲻Ҫ�Ĳ���3��4��5��6��7
st = dc_changeb3(ByVal icdev, sector, akey(0), 0, 0, 0, 1, 0, bkey(0))
If st <> 0 Then
    List1.AddItem "���� dc_changeb3()��������!���˳���"
    Exit Sub
End If
List1.AddItem "�޸�����ɹ�������"
End Sub



Private Sub Form_Load()

icdev = -1
End Sub

Private Sub Form_Unload(Cancel As Integer)
quit
End Sub
