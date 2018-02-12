<HTML>
<HEAD><TITLE>测试网站</TITLE></HEAD>
<BODY>
<OBJECT id=rd codeBase=/comRD800.dll classid="clsid:638B238E-EB84-4933-B3C8-854B86140668"></OBJECT>

<script language=javascript>

function OnTest()
{
///////////////////////////////////////////////////////////////////////////////
//以下为非接触式读写器函数调用例子 for javascript
//注意个别函数（例如dc_disp_str）只有当设备满足要求（例如有数码显示）时才有效
//更详细的帮助可参照32位动态库对应的函数使用说明
///////////////////////////////////////////////////////////////////////////////

var st; //主要用于返回值
var lSnr; //本用于取序列号，但在javascript只是当成dc_card函数的一个临时变量

//初始化端口
//第一个参数为0表示COM1，为1表示COM2，以此类推
//第二个参数为通讯波特率
st = rd.dc_init(100, 115200);
if(st <= 0) //返回值小于等于0表示失败
{
    alert("dc_init error!");
	return;
}
alert("dc_init ok!");

//寻卡，能返回在工作区域内某张卡的序列号
//第一个参数一般设置为0，表示IDLE模式，一次只对一张卡操作
//第二个参数在javascript只是当成dc_card函数的一个临时变量，仅在vbscript中调用后能正确返回序列号
st = rd.dc_card(0, lSnr);
if(st != 0) //返回值小于0表示失败
{
    alert("dc_card error!");
    rd.dc_exit();
	return;
}
alert("dc_card ok!");
alert(rd.get_bstrRBuffer); //序列号为rd.get_bstrRBuffer，一般有不可显示字符出现
alert(rd.get_bstrRBuffer_asc); //序列号十六进制ascll码字符串表示为rd.get_bstrRBuffer_asc

//将密码装入读写模块RAM中
//第一个参数为装入密码模式
//第二个参数为扇区号
rd.put_bstrSBuffer_asc = "FFFFFFFFFFFF"; //在调用dc_load_key必须前先设置属性rd.put_bstrSBuffer或rd.put_bstrSBuffer_asc
st = rd.dc_load_key(0, 0);
if(st != 0) //返回值小于0表示失败
{
    alert("dc_load_key error!");
    rd.dc_exit();
    return;
}
alert("dc_load_key ok!");

//核对密码函数
//第一个参数为密码验证模式
//第二个参数为扇区号
st = rd.dc_authentication(0, 0);
if(st != 0) //返回值小于0表示失败
{
    alert("dc_authentication error!");
    rd.dc_exit();
    return;
}
alert("dc_authentication ok!");

//向卡中写入数据，一次必须写一个块
//第一个参数为块地址
//在调用dc_write必须前先设置属性rd.put_bstrSBuffer或rd.put_bstrSBuffer_asc
rd.put_bstrSBuffer_asc = "31323334353637383930313233343536";
st = rd.dc_write(2);
if(st != 0) //返回值小于0表示失败
{
    alert("dc_write error!");
    rd.dc_exit();
    return;
}
alert("dc_write ok!");

//读取卡中数据，一次必须读一个块
//第一个参数为块地址
//取出的数据放在属性放在rd.put_bstrSBuffer（正常表示）和rd.put_bstrSBuffer_asc（十六进制ascll码字符串表示）中
st = rd.dc_read(2);
if(st != 0) //返回值小于0表示失败
{
    alert("dc_read error!");
    rd.dc_exit();
    return;
}
alert("dc_read ok!");
alert(rd.get_bstrRBuffer);
alert(rd.get_bstrRBuffer_asc);

//设置读写器时间
//04年 星期四 9月 9日 11点 20分 50秒
rd.put_bstrSBuffer_asc = "04040909112050";
st = rd.dc_settime();
if(st != 0) //返回值小于0表示失败
{
    alert("dc_settime error!");
    rd.dc_exit();
    return;
}
alert("dc_settime ok!");

//在读写器的数码管上显示数字
rd.put_bstrSBuffer = "12345678\0"; //显示12345678，注意字符串后必须附加一个ascll为0x00结束符号;
st = rd.dc_disp_str();
if(st != 0) //返回值小于0表示失败
{
    alert("dc_disp_str error!");
    rd.dc_exit();
    return;
}
alert("dc_disp_str ok!");

//蜂鸣
//第一个参数为蜂鸣时间，单位是10毫秒
st = rd.dc_beep(50);
if(st != 0) //返回值小于0表示失败
{
    alert("dc_beep error!");
    rd.dc_exit();
    return;
}
alert("dc_beep ok!");

//关闭端口
st = rd.dc_exit();
if(st != 0) //返回值小于0表示失败
{
    alert("dc_exit error!");
    return;
}
alert("dc_exit ok!");

}

</script>

<!-- Insert HTML here -->
<INPUT id=test type=button value=test onclick=OnTest()></BODY>
</BODY>
</HTML>
