VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   4890
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   7740
   LinkTopic       =   "Form1"
   ScaleHeight     =   4890
   ScaleWidth      =   7740
   StartUpPosition =   3  'Windows Default
   Begin VB.TextBox Text4 
      Height          =   375
      Left            =   1800
      TabIndex        =   12
      Top             =   600
      Width           =   615
   End
   Begin VB.TextBox Text3 
      Height          =   495
      Left            =   2160
      MaxLength       =   8
      TabIndex        =   11
      Top             =   3960
      Width           =   4335
   End
   Begin VB.ListBox List1 
      Height          =   1815
      Left            =   2160
      TabIndex        =   10
      Top             =   1920
      Width           =   4335
   End
   Begin VB.TextBox Text2 
      Height          =   285
      Left            =   5160
      TabIndex        =   9
      Top             =   1410
      Width           =   615
   End
   Begin VB.TextBox Text1 
      Height          =   285
      Left            =   3360
      TabIndex        =   7
      Top             =   1410
      Width           =   495
   End
   Begin VB.CommandButton Command6 
      Caption         =   "Exit"
      Height          =   495
      Left            =   5880
      TabIndex        =   5
      Top             =   1200
      Width           =   975
   End
   Begin VB.CommandButton Command5 
      Caption         =   "Write Data"
      Height          =   495
      Left            =   960
      TabIndex        =   4
      Top             =   3960
      Width           =   1095
   End
   Begin VB.CommandButton Command4 
      Caption         =   "Read Data"
      Height          =   495
      Left            =   960
      TabIndex        =   3
      Top             =   1320
      Width           =   1095
   End
   Begin VB.CommandButton Command3 
      Caption         =   "Find Multi Card"
      Height          =   495
      Left            =   5640
      TabIndex        =   2
      Top             =   480
      Width           =   1455
   End
   Begin VB.CommandButton Command2 
      Caption         =   "Find Single Card"
      Height          =   495
      Left            =   3960
      TabIndex        =   1
      Top             =   480
      Width           =   1575
   End
   Begin VB.CommandButton Command1 
      Caption         =   "Init"
      Height          =   495
      Left            =   960
      TabIndex        =   0
      Top             =   480
      Width           =   735
   End
   Begin VB.Label Label3 
      Caption         =   "PortNo(0~19,100)"
      Height          =   255
      Left            =   2520
      TabIndex        =   13
      Top             =   600
      Width           =   1335
   End
   Begin VB.Label Label2 
      Caption         =   "BlockNum(1~10)"
      Height          =   255
      Left            =   3960
      TabIndex        =   8
      Top             =   1440
      Width           =   1215
   End
   Begin VB.Label Label1 
      Caption         =   "StartAddr(0~27)"
      Height          =   255
      Left            =   2160
      TabIndex        =   6
      Top             =   1440
      Width           =   1215
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Dim icdev As Long

Private Sub Command1_Click()
If icdev > 0 Then
    st = dc_exit(icdev)
    icdev = 0
End If

If icdev < 0 Then
  icdev = dc_init(CInt(Text4.Text), 115200)   'init com1£¬baud rate is 115200MHZ
End If

If icdev < 0 Then
      List1.AddItem ("Init port Error!")
      
     Exit Sub
End If
List1.AddItem ("Init port OK!")

 st = dc_config_card(icdev, &H31)     'find 15693 card,&H31 can be exchanged by 49
    If st <> 0 Then
     List1.AddItem ("config card Error!")
         Exit Sub
    End If
End Sub

Private Sub Command2_Click()   'find single card
Dim rlen As Byte
    st = dc_inventory_hex(icdev, &H36, 0, 0, rlen, UID)
    If st <> 0 Then
    
       List1.AddItem ("Find single card ERROR!")
    
       Exit Sub
    End If
    
     List1.AddItem (UID)

End Sub

Private Sub Command3_Click()   'find multi card
Dim rlen As Byte
st = dc_inventory_hex(icdev, &H16, 0, 0, rlen, UID)
    If st <> 0 Then
        
        List1.AddItem ("Find multi card ERROR!")
        
            
        Exit Sub
    End If
    List1.AddItem (UID)

End Sub

Private Sub Command4_Click()   'read data
Dim i As Integer
Dim rlen As Byte
Dim rbuffer As String * 256
m = CInt(Text1.Text)            'start addresss
n = CInt(Text2.Text)           'block number
st = dc_readblock_hex(icdev, &H22, m, n, Mid(UID, 3, 16), rlen, rbuffer)  'get datas except the front one byte,two datas
If st <> 0 Then
st = dc_exit(icdev)
    List1.AddItem ("Read data ERROR!")
    st = dc_exit(icdev)
    Exit Sub
End If
If m + n > 28 Then
        n = n - ((m + n) - 28)
End If
For i = 0 To n - 1
   
    List1.AddItem ("BlockAddr" & m + i & ":" & Mid(rbuffer, 1 + i * 8, 8))
    
Next i
End Sub

Private Sub Command5_Click()
Dim rbuffer As String * 8        'write data from the start address CInt(Text1.Text)
rbuffer = Text3.Text
st = dc_writeblock_hex(icdev, &H22, CInt(Text1.Text), 1, Mid(UID, 3, 16), 4, CStr(rbuffer))
If st <> 0 Then
    st = dc_exit(icdev)
        List1.AddItem ("Write data ERROR")
        
        Exit Sub
End If
  List1.AddItem ("Write data OK")
End Sub

Private Sub Command6_Click()  'exit
If icdev > 0 Then
        st = dc_exit(icdev)
    icdev = 0
 End If
 Unload Me
End Sub

Private Sub Form_Load()
    icdev = -1
End Sub

Private Sub Form_Unload(Cancel As Integer)
If icdev > 0 Then
    dc_exit (icdev)
End If

End Sub
