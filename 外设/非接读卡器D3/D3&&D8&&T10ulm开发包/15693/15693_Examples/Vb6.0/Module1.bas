Attribute VB_Name = "Module1"
Option Explicit
Global icdev As Long
Global st As Integer
Global UID As String * 256
Declare Function dc_init Lib "dcrf32.dll" (ByVal port%, ByVal baud As Long) As Long
Declare Function dc_exit Lib "dcrf32.dll" (ByVal icdev As Long) As Integer
Declare Function dc_config_card Lib "dcrf32.dll" (ByVal icdev As Long, ByVal cardtype As Byte) As Integer
Declare Function dc_inventory Lib "dcrf32.dll" (ByVal icdev As Long, ByVal flags As Byte, ByVal AFI As Byte, ByVal masklen As Byte, ByRef rlen As Byte, ByVal rbuffer$) As Integer
Declare Function dc_inventory_hex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal flags As Byte, ByVal AFI As Byte, ByVal masklen As Byte, ByRef rlen As Byte, ByVal rbuffer$) As Integer
Declare Function dc_readblock Lib "dcrf32.dll" (ByVal icdev As Long, ByVal flags As Byte, ByVal startblock%, ByVal blocknum%, ByVal UID$, ByRef rlen As Byte, ByVal rbuffer$) As Integer
Declare Function dc_readblock_hex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal flags As Byte, ByVal startblock%, ByVal blocknum%, ByVal UID$, ByRef rlen As Byte, ByVal rbuffer$) As Integer
Declare Function dc_writeblock_hex Lib "dcrf32.dll" (ByVal icdev As Long, ByVal flags As Byte, ByVal startblock%, ByVal blocknum%, ByVal UID$, ByVal rlen As Byte, ByVal rbuffer$) As Integer
Declare Function dc_writeblock Lib "dcrf32.dll" (ByVal icdev As Long, ByVal flags As Byte, ByVal startblock%, ByVal blocknum%, ByVal UID$, ByVal rlen As Byte, ByVal rbuffer$) As Integer
