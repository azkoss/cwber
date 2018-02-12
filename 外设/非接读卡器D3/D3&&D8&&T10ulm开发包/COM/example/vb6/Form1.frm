VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   5190
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   5310
   LinkTopic       =   "Form1"
   ScaleHeight     =   5190
   ScaleWidth      =   5310
   StartUpPosition =   3  'Windows Default
   Begin VB.ListBox List1 
      Height          =   4545
      Left            =   240
      TabIndex        =   1
      Top             =   240
      Width           =   3375
   End
   Begin VB.CommandButton Command1 
      Caption         =   "≤‚ ‘"
      Height          =   495
      Left            =   3840
      TabIndex        =   0
      Top             =   240
      Width           =   1215
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub Command1_Click()

Dim st As Integer
Dim rd As COMRD800Lib.RD800
Set rd = New COMRD800Lib.RD800
List1.Clear

st = rd.dc_init(100, 115200)
If st <= 0 Then
    List1.AddItem "dc_init error!"
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_init ok!"

Dim lSnr As Variant
st = rd.dc_card(0, lSnr)
If st <> 0 Then
    List1.AddItem "dc_card error!"
    rd.dc_exit
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_card ok!"
List1.AddItem lSnr

rd.put_bstrSBuffer_asc = "FFFFFFFFFFFF"
st = rd.dc_load_key(0, 0)
If st < 0 Then
    List1.AddItem "dc_load_key error!"
    rd.dc_exit
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_load_key ok!"

st = rd.dc_authentication(0, 0)
If st < 0 Then
    List1.AddItem "dc_authentication error!"
    rd.dc_exit
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_authentication ok!"

rd.put_bstrSBuffer_asc = "31323334353637383930313233343536"
st = rd.dc_write(2)
If st < 0 Then
    List1.AddItem "dc_write error!"
    rd.dc_exit
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_write ok!"

st = rd.dc_read(2)
If st < 0 Then
    List1.AddItem "dc_read error!"
    rd.dc_exit
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_read ok!"
List1.AddItem rd.get_bstrRBuffer
List1.AddItem rd.get_bstrRBuffer_asc

st = rd.dc_exit()
If st < 0 Then
    List1.AddItem "dc_exit error!"
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_exit ok!"

Set rd = Nothing

End Sub
