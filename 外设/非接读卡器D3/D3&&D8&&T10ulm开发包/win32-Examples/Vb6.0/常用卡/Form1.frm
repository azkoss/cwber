VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   5805
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   6285
   LinkTopic       =   "Form1"
   ScaleHeight     =   5805
   ScaleWidth      =   6285
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton Command8 
      Caption         =   "24c64"
      Height          =   375
      Left            =   4440
      TabIndex        =   8
      Top             =   5160
      Width           =   1215
   End
   Begin VB.CommandButton Command7 
      Caption         =   "24c01"
      Height          =   375
      Left            =   3120
      TabIndex        =   7
      Top             =   5160
      Width           =   1215
   End
   Begin VB.CommandButton Command6 
      Caption         =   "type b"
      Height          =   375
      Left            =   3120
      TabIndex        =   6
      Top             =   4680
      Width           =   1215
   End
   Begin VB.CommandButton Command5 
      Caption         =   "contact cpu"
      Height          =   375
      Left            =   4440
      TabIndex        =   5
      Top             =   4680
      Width           =   1215
   End
   Begin VB.CommandButton Command4 
      Caption         =   "4428"
      Height          =   375
      Left            =   480
      TabIndex        =   4
      Top             =   5160
      Width           =   1215
   End
   Begin VB.CommandButton Command3 
      Caption         =   "4442"
      Height          =   375
      Left            =   1800
      TabIndex        =   3
      Top             =   5160
      Width           =   1215
   End
   Begin VB.CommandButton Command2 
      Caption         =   "type a"
      Height          =   375
      Left            =   1800
      TabIndex        =   2
      Top             =   4680
      Width           =   1215
   End
   Begin VB.CommandButton Command1 
      Caption         =   "m1"
      Height          =   375
      Left            =   480
      TabIndex        =   1
      Top             =   4680
      Width           =   1215
   End
   Begin VB.ListBox List1 
      BackColor       =   &H00C0FFC0&
      Height          =   4155
      Left            =   480
      TabIndex        =   0
      Top             =   360
      Width           =   5175
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub Command1_Click()

Dim cardmode As Integer
Dim snr As Long
Dim hexkey As String
Dim sector As Integer
Dim databuff32 As String * 1024
Dim data32 As String
Dim address As Integer

List1.Clear
List1.AddItem ("/*** m1 card test begin ***/")
List1.AddItem ("")
List1.AddItem ("dc_init...")
icdev = dc_init(0, 115200)
If icdev < 0 Then
    List1.AddItem ("dc_init error")
    Exit Sub
End If
List1.AddItem ("dc_init ok")

st = dc_config_card(icdev, 65)

List1.AddItem ("dc_card...")
cardmode = 0
st = dc_card(icdev, cardmode, snr)
If st <> 0 Then
    List1.AddItem ("dc_card error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_card ok")
List1.AddItem (Hex(snr))

List1.AddItem ("dc_Load_key_hex...")
hexkey = "ffffffffffff"
st = dc_load_key_hex(icdev, 0, 0, hexkey)
If st <> 0 Then
      List1.AddItem ("dc_Load_key_hex ok")
       quit
      Exit Sub
End If
List1.AddItem ("dc_Load_key_hex error")
sector = 0
st = dc_authentication(icdev, 0, sector)
If st <> 0 Then
      List1.AddItem ("dc_autication eror!")
        quit
      Exit Sub
End If
List1.AddItem ("dc_autication ok")
 

List1.AddItem ("dc_write_hex...")
address = sector * 4 + 2
data32 = "1234567890abcdef1234567890abcdef"
st = dc_write_hex(icdev, address, data32)
If st <> 0 Then
      List1.AddItem ("dc_write_hex error")
        quit
      Exit Sub
End If
List1.AddItem ("dc_write_hex ok")

List1.AddItem ("dc_read_hex...")
st = dc_read_hex(icdev, address, databuff32)
If st <> 0 Then
     List1.AddItem ("dc_read_hex error")
      quit
     Exit Sub
End If
List1.AddItem ("dc_read_hex ok")
List1.AddItem (databuff32)
 
 List1.AddItem ("dc_write_hex...")
 address = sector * 4 + 2
data32 = "00000000000000000000000000000000"
st = dc_write_hex(icdev, address, data32)
If st <> 0 Then
      List1.AddItem ("dc_write_hex error")
        quit
      Exit Sub
End If
List1.AddItem ("dc_write_hex ok")

List1.AddItem ("dc_read_hex ")
st = dc_read_hex(icdev, address, databuff32)
If st <> 0 Then
     List1.AddItem ("dc_read_hex error")
     quit
     Exit Sub
End If
List1.AddItem ("dc_read_hex ok")
List1.AddItem (databuff32)

List1.AddItem ("")
List1.AddItem ("/*** m1 card test end ***/")
List1.AddItem ("")

st = dc_beep(icdev, 10)

quit
End Sub

Private Sub Command2_Click()

List1.Clear
Dim snr As Long
Dim rlen As Integer
Dim rdata As String * 1024
Dim temp As String

List1.AddItem ("dc init")
icdev = dc_init(100, 115200)
If icdev <= 0 Then
    List1.AddItem ("dc init error")
    Exit Sub
End If
List1.AddItem ("dc init ok")

st = dc_config_card(icdev, 65)

List1.AddItem ("dc card")
st = dc_card(icdev, 0, snr)
If st <> 0 Then
    List1.AddItem ("dc card error")
    quit
    Exit Sub
End If
List1.AddItem ("dc card ok")
List1.AddItem ("card id is" + (Hex(snr)))

List1.AddItem ("dc pro reset")
st = dc_pro_reset_hex(icdev, rlen, rdata)
If st <> 0 Then
    List1.AddItem ("dc pro reset error")
    quit
    Exit Sub
End If
List1.AddItem ("dc pro reset ok")
List1.AddItem ("reset information :" + rdata)

List1.AddItem ("dc pro command ")
st = dc_pro_commandhex(icdev, 5, "0084000008", rlen, rdata, 7)
If st <> 0 Then
    List1.AddItem ("dc pro command  error")
    quit
    Exit Sub
End If
List1.AddItem ("dc pro command ok")
List1.AddItem rdata
st = dc_beep(icdev, 10)
quit
    
End Sub

Private Sub Command3_Click()
Dim rbuff As String * 1024
Dim rlen As Integer

List1.Clear
List1.AddItem ("/*** 4442 card test begin ***/")
List1.AddItem ("")
List1.AddItem ("dc_init...")
icdev = dc_init(0, 115200)
If icdev < 0 Then
    List1.AddItem ("dc_init error")
    Exit Sub
End If
List1.AddItem ("dc_init ok")


List1.AddItem ("dc_readpincount_4442...")
st = dc_readpincount_4442(icdev)
If st < 0 Then
    List1.AddItem ("dc_readpincount_4442 error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_readpincount_4442 ok")
List1.AddItem ("pin count is" + Str(st))

List1.AddItem ("dc_verifypin_4442_hex...")
st = dc_verifypin_4442_hex(icdev, "ffffff")
If st <> 0 Then
    List1.AddItem ("dc_verifypin_4442_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_verifypin_4442_hex ok")

List1.AddItem ("dc_changepin_4442_hex...")
st = dc_changepin_4442_hex(icdev, "ffffff")
If st <> 0 Then
    List1.AddItem ("dc_changepin_4442_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_changepin_4442_hex ok")

List1.AddItem ("dc_readpin_4442_hex...")
st = dc_readpin_4442_hex(icdev, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_readpin_4442_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_readpin_4442_hex ok")
List1.AddItem ("pin is " + rbuff)

List1.AddItem ("dc_read_4442_hex...")
st = dc_read_4442_hex(icdev, 245, 6, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_read_4442_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_read_4442_hex ok")
List1.AddItem ("data is " + rbuff)

List1.AddItem ("dc_write_4442_hex...")
st = dc_write_4442_hex(icdev, 245, 6, "1234567890ab")
If st <> 0 Then
    List1.AddItem ("dc_write_4442_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_write_4442_hex ok")

List1.AddItem ("dc_read_4442_hex...")
st = dc_read_4442_hex(icdev, 245, 6, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_read_4442_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_read_4442_hex ok")
List1.AddItem ("data is " + rbuff)

List1.AddItem ("dc_write_4442_hex...")
st = dc_write_4442_hex(icdev, 245, 6, "ffffffffffff")
If st <> 0 Then
    List1.AddItem ("dc_write_4442_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_write_4442_hex ok")

List1.AddItem ("")
List1.AddItem ("/*** 4442 card test end ***/")
List1.AddItem ("")
st = dc_beep(icdev, 10)
quit
End Sub

Private Sub Command4_Click()
Dim rbuff As String * 1024
Dim rlen As Integer

List1.Clear
List1.AddItem ("/*** 4428 card test begin ***/")
List1.AddItem ("")
List1.AddItem ("dc_init...")
icdev = dc_init(0, 115200)
If icdev < 0 Then
    List1.AddItem ("dc_init error")
    Exit Sub
End If
List1.AddItem ("dc_init ok")


List1.AddItem ("dc_readpincount_4428...")
st = dc_readpincount_4428(icdev)
If st < 0 Then
    List1.AddItem ("dc_readpincount_4428 error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_readpincount_4428 ok")
List1.AddItem ("pin count is" + Str(st))

List1.AddItem ("dc_verifypin_4428_hex...")
st = dc_verifypin_4428_hex(icdev, "ffff")
If st <> 0 Then
    List1.AddItem ("dc_verifypin_4428_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_verifypin_4428_hex ok")

List1.AddItem ("dc_changepin_4428_hex...")
st = dc_changepin_4428_hex(icdev, "ffff")
If st <> 0 Then
    List1.AddItem ("dc_changepin_4428_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_changepin_4428_hex ok")

List1.AddItem ("dc_readpin_4428_hex...")
st = dc_readpin_4428_hex(icdev, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_readpin_4428_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_readpin_4428_hex ok")
List1.AddItem ("pin is " + rbuff)

List1.AddItem ("dc_read_4428_hex...")
st = dc_read_4428_hex(icdev, 245, 6, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_read_4428_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_read_4428_hex ok")
List1.AddItem ("data is " + rbuff)

List1.AddItem ("dc_write_4428_hex...")
st = dc_write_4428_hex(icdev, 245, 6, "1234567890ab")
If st <> 0 Then
    List1.AddItem ("dc_write_4428_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_write_4428_hex ok")

List1.AddItem ("dc_read_4428_hex...")
st = dc_read_4428_hex(icdev, 245, 6, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_read_4428_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_read_4428_hex ok")
List1.AddItem ("data is " + rbuff)

List1.AddItem ("dc_write_4428_hex...")
st = dc_write_4428_hex(icdev, 245, 6, "ffffffffffff")
If st <> 0 Then
    List1.AddItem ("dc_write_4428_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_write_4428_hex ok")

List1.AddItem ("")
List1.AddItem ("/*** 4428 card test end ***/")
List1.AddItem ("")
st = dc_beep(icdev, 10)
quit
End Sub

Private Sub Command5_Click()
Dim rbuff As String * 1024
Dim rlen As Integer
Dim cardmode As Integer
cardmode = 0

List1.Clear
List1.AddItem ("/*** contact card test begin ***/")
List1.AddItem ("")
List1.AddItem ("dc_init...")
icdev = dc_init(0, 115200)
If icdev < 0 Then
    List1.AddItem ("dc_init error")
    Exit Sub
End If
List1.AddItem ("dc_init ok")


List1.AddItem ("dc_setcpu...")
st = dc_setcpu(icdev, 12)
If st <> 0 Then
    List1.AddItem ("dc_setcpu error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_setcpu ok")


List1.AddItem ("dc_cpureset...")
st = dc_cpureset_hex(icdev, rlen, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_cpureset error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_cpureset ok")
List1.AddItem ((rbuff))

List1.AddItem ("dc_cpuapdu_hex...")
st = dc_cpuapdu_hex(icdev, 5, "0084000008", rlen, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_cpuapdu_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_cpuapdu_hex ok")
List1.AddItem ((rbuff))

List1.AddItem ("")
List1.AddItem ("/*** contact card test end ***/")
List1.AddItem ("")
st = dc_beep(icdev, 10)
quit
End Sub

Private Sub Command6_Click()
Dim snr As String * 1024
Dim rlen As Integer
Dim rdata As String * 1024
Dim temp As String

List1.Clear
List1.AddItem ("dc init")
icdev = dc_init(0, 115200)
If icdev <= 0 Then
    List1.AddItem ("dc init error")
    Exit Sub
End If
List1.AddItem ("dc init ok")

st = dc_config_card(icdev, 66)

List1.AddItem ("dc card")
st = dc_card_b_hex(icdev, snr)
If st <> 0 Then
    List1.AddItem ("dc card error")
    quit
    Exit Sub
End If
List1.AddItem ("dc card ok")
List1.AddItem ("reset information is" + ((snr)))

List1.AddItem ("dc pro command ")
st = dc_pro_commandhex(icdev, 5, "0084000008", rlen, rdata, 7)
If st <> 0 Then
    List1.AddItem ("dc pro command  error")
    quit
    Exit Sub
End If
List1.AddItem ("dc pro command ok")
List1.AddItem rdata
st = dc_beep(icdev, 10)
quit
    
End Sub


Private Sub Command7_Click()
Dim rbuff As String * 1024
Dim rlen As Integer

List1.Clear
icdev = dc_init(0, 115200)
If icdev < 0 Then
    List1.AddItem ("dc_init error")
    Exit Sub
End If
List1.AddItem ("dc_init ok")

List1.AddItem ("dc_write_24c01_hex...")
st = dc_write_24c_hex(icdev, 0, 6, "1234567890ab")
If st <> 0 Then
    List1.AddItem ("dc_write_24c01_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_write_24c01_hex ok")

List1.AddItem ("dc_read_24c01_hex...")
st = dc_read_24c_hex(icdev, 0, 6, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_read_24c01_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_read_24c01_hex ok")
List1.AddItem ("data is " + rbuff)

st = dc_beep(icdev, 10)
quit
End Sub

Private Sub Command8_Click()
Dim rbuff As String * 1024
Dim rlen As Integer

List1.Clear
icdev = dc_init(0, 115200)
If icdev < 0 Then
    List1.AddItem ("dc_init error")
    Exit Sub
End If
List1.AddItem ("dc_init ok")

List1.AddItem ("dc_write_24c64_hex...")
st = dc_write_24c64_hex(icdev, 0, 6, "1234567890ab")
If st <> 0 Then
    List1.AddItem ("dc_write_24c64_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_write_24c64_hex ok")

List1.AddItem ("dc_read_24c64_hex...")
st = dc_read_24c64_hex(icdev, 0, 6, rbuff)
If st <> 0 Then
    List1.AddItem ("dc_read_24c64_hex error")
    quit
    Exit Sub
End If
List1.AddItem ("dc_read_24c64_hex ok")
List1.AddItem ("data is " + rbuff)

st = dc_beep(icdev, 10)
quit
End Sub

Private Sub Form_Load()
icdev = -1
quit
End Sub

Private Sub Form_Unload(Cancel As Integer)
quit
Unload Me
End Sub
