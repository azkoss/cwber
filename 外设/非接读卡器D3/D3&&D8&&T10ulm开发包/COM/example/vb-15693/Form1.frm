VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "15693"
   ClientHeight    =   5265
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   6345
   LinkTopic       =   "Form1"
   ScaleHeight     =   5265
   ScaleWidth      =   6345
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton Cmd_Test 
      Caption         =   "Test"
      Height          =   495
      Left            =   4320
      TabIndex        =   1
      Top             =   720
      Width           =   1335
   End
   Begin VB.ListBox List1 
      Height          =   4155
      Left            =   600
      TabIndex        =   0
      Top             =   600
      Width           =   3495
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub Cmd_Test_Click()
Dim st As Integer
Dim UID As String * 16
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

'Request single card
Dim rlen As Variant
st = rd.dc_inventory(&H36, 0, 0, rlen)
If st <> 0 Then
    List1.AddItem "dc_inventory error!"
    rd.dc_exit
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_inventory ok!"
List1.AddItem rd.get_bstrRBuffer
List1.AddItem rd.get_bstrRBuffer_asc

'get system information
UID = Mid(rd.get_bstrRBuffer_asc, 3, (rlen - 1) * 2)
rd.put_bstrSBuffer_asc = UID
st = rd.dc_get_systeminfo(&H22, rlen)
If st <> 0 Then
    List1.AddItem "dc_get_systeminfo error!"
    rd.dc_exit
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_get_systeminfo ok!"
List1.AddItem rd.get_bstrRBuffer
List1.AddItem rd.get_bstrRBuffer_asc

'Read
rd.put_bstrSBuffer_asc = UID
st = rd.dc_readblock(&H22, 1, 10, rlen)
If st <> 0 Then
    List1.AddItem "dc_readblock error!"
    rd.dc_exit
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_readblock ok!"
List1.AddItem rd.get_bstrRBuffer
List1.AddItem rd.get_bstrRBuffer_asc

'Write  TI
rd.put_bstrSBuffer_asc = UID
rd.put_bstrSBufferEx_asc = "12345678"
st = rd.dc_writeblock(&H43 Or &H80, 1, 1, 4)
If st <> 0 Then
    List1.AddItem "dc_writeblock error!"
    rd.dc_exit
    Set rd = Nothing
    Exit Sub
End If
List1.AddItem "dc_writeblock ok!"

'Write ICODE II
'rd.put_bstrSBuffer_asc = UID
'rd.put_bstrSBufferEx_asc = "12345678"
'st = rd.dc_writeblock(&H22, 1, 1, 4)
'If st <> 0 Then
'    List1.AddItem "dc_writeblock error!"
'    rd.dc_exit
'    Set rd = Nothing
'    Exit Sub
'End If
'List1.AddItem "dc_writeblock ok!"


End Sub
