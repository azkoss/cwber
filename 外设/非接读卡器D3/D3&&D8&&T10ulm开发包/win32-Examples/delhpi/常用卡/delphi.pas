unit delphi;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls;

type
  TForm1 = class(TForm)
    lst1: TListBox;
    btn1: TButton;
    btn2: TButton;
    btn3: TButton;
    btn4: TButton;
    btn5: TButton;
    btn6: TButton;
    btn7: TButton;
    btn8: TButton;
    procedure btn3Click(Sender: TObject);
      procedure clearText;
    procedure btn4Click(Sender: TObject);
    procedure btn5Click(Sender: TObject);
    procedure btn6Click(Sender: TObject);
    procedure btn2Click(Sender: TObject);
    procedure btn1Click(Sender: TObject);
    procedure btn7Click(Sender: TObject);
    procedure btn8Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }

  end;

var
  Form1: TForm1;

implementation

uses drv_unit;

{$R *.dfm}


procedure TForm1.btn3Click(Sender: TObject);
var
 atqb:PChar;
  rdata:PChar;
  rlen:Byte;
begin
  clearText;

  icdev := dc_init(100,115200);
  if icdev < 0 then
  begin
      lst1.items.Add('dc init error');
      Exit;
  end;
  lst1.Items.add('dc init ok');

  st := dc_config_card(icdev,$42);

  GetMem(atqb,128);
  st := dc_card_b_hex(icdev,atqb);
  if st <> 0 then
  begin

    lst1.Items.Add('card b error');
    lst1.Items.Add('erroe code '+IntToStr(st));
    quit;
    Exit
  end;
  lst1.Items.Add('card b ok');
  lst1.Items.Add('reset infomation '+ atqb ); 

  GetMem(rdata,128);
  st := dc_pro_commandhex(icdev,5,'0084000008',rlen,rdata,7);
  if st <> 0 then
   begin
    lst1.Items.Add('get a random number error ');
    lst1.Items.Add('errer code'+IntToStr(st));
    quit;
    Exit
  end;
  lst1.items.add('get a random number ok');
  lst1.Items.Add('random number is : '+rdata);
  dc_beep(icdev,10);
  quit;
end;


procedure TForm1.clearText;
  var
  i,j:integer;   
  begin   
      j:=lst1.items.count;
      i:=0;   
      while   i<j   do
      begin
        lst1.Items.Delete(0);
      inc(i);   
      end;   
  end;

procedure TForm1.btn4Click(Sender: TObject);
var
  st:smallint;
  rlen:Byte;
  rdata:pchar;
begin
  clearText;
icdev := dc_init(100,115200);
if icdev<0 then begin
    lst1.Items.Add('init error ...');
     Exit;
end;
    lst1.Items.Add('init ok ');

    st := dc_setcpu(icdev,12);
    if st <> 0 then begin
    lst1.Items.Add('set cpu address error ');
    quit;
    Exit;
    end;
    lst1.Items.Add('set cpu address ok');

    getmem(rdata, 256);
    st := dc_cpureset_hex(icdev,rlen,rdata);
    if st <> 0 then begin
      lst1.Items.Add('cpu reset error');
      quit;
      Exit;
    end;
    lst1.Items.Add('cpu reset ok');
    lst1.Items.Add('reset data '+rdata);

    GetMem(rdata,1024);
    st := dc_cpuapdu_hex(icdev,5,'0084000008',rlen,rdata);
    if st <> 0 then begin
      lst1.Items.Add('get a random number error ');
      quit;
      Exit;
    end;
    lst1.Items.Add('get a random number ok ');
    lst1.Items.Add('random number is '+ rdata);
    dc_beep(icdev,10);
    quit;
end;

procedure TForm1.btn5Click(Sender: TObject);
var
  rdata:PChar;
begin
clearText;

icdev := dc_init(100,115200);
if icdev < 0 then
  begin
    lst1.Items.Add('init error');
    Exit;
  end;
lst1.Items.Add('init ok');

lst1.Items.Add('get the pin count..');
st := dc_readpincount_4428(icdev);
  if st < 0 then
    begin
      lst1.Items.Add('get the pin count error');
      quit;
      Exit;
    end;
lst1.Items.Add('get the pin count ok');
lst1.Items.Add('pin count is '+inttostr(st));

st := dc_verifypin_4428_hex(icdev,'FFFF');
if st <> 0 then
  begin
    lst1.items.add('verify the pin error');
    quit;
    Exit;
  end;
lst1.Items.Add('verify the pin ok');

lst1.Items.Add('change the pin');
st := dc_changepin_4428_hex(icdev,'FFFF');
if st <> 0 then
begin
  lst1.Items.Add('change the pin to ffff error');
  quit;
  Exit;
end;
lst1.Items.Add('change the pin to ffff ok');

getmem(rdata,256);
st := dc_readpin_4428_hex(icdev,rdata);
if st <> 0 then
begin
  lst1.Items.Add('read the pin error');
  quit;
  Exit;
end;
lst1.Items.Add('read the pin ok ');
lst1.Items.Add('the pin is ' + rdata);

lst1.Items.Add('read the data from $f5 to $fa');
getmem(rdata,256);
st := dc_read_4428_hex(icdev,$f5,6,rdata);
if st <> 0 then
begin
  lst1.Items.Add('read the data error');
  quit;
  Exit;
end;
lst1.Items.Add('read the data ok');
lst1.Items.Add('the data is :' + rdata);

lst1.Items.Add('write the data from $f5 to $fa');

st := dc_write_4428_hex(icdev,$f5,6,'123456789012');
if st <> 0 then
begin
  lst1.Items.Add('write the data error');
  quit;
  Exit;
end;
lst1.Items.Add('write the data ok');

lst1.Items.Add('read the data from $f5 to $fa');
getmem(rdata,256);
st := dc_read_4428_hex(icdev,$f5,6,rdata);
if st <> 0 then
begin
  lst1.Items.Add('read the data error');
  quit;
  exit;
end;
lst1.Items.Add('read the data ok');
lst1.Items.Add('the data is :' + rdata);

lst1.Items.Add('write the data from $f5 to $fa');

st := dc_write_4428_hex(icdev,$f5,6,'ffffffffffff');
if st <> 0 then
begin
  lst1.Items.Add('write the data error');
  quit;
  Exit;
end;
lst1.Items.Add('write the data ok');
dc_beep(icdev,10);
quit;
  end;

procedure TForm1.btn6Click(Sender: TObject);
var
  rdata:PChar;
begin
clearText;

icdev := dc_init(100,115200);
if icdev < 0 then
  begin
    lst1.Items.Add('init error');
    Exit;
  end;
lst1.Items.Add('init ok');

st := dc_readpincount_4442(icdev);
  if st < 0 then
    begin
      lst1.Items.Add('get the pin count error');
      quit;
      Exit;
    end;
lst1.Items.Add('get the pin count ok');
lst1.Items.Add('pin count is '+inttostr(st));

st := dc_verifypin_4442_hex(icdev,'FFFFFF');
if st <> 0 then
  begin
    lst1.items.add('verify the pin error');
    quit;
    Exit;
  end;
lst1.Items.Add('verify the pin ok');

st := dc_changepin_4442_hex(icdev,'FFFFFF');
if st <> 0 then
begin
  lst1.Items.Add('change the pin to ffffff error');
  quit;
  Exit;
end;
lst1.Items.Add('change the pin to ffffff ok');

getmem(rdata,256);
st := dc_readpin_4442_hex(icdev,rdata);
if st <> 0 then
begin
  lst1.Items.Add('read the pin error');
  quit;
  Exit;
end;
lst1.Items.Add('read the pin ok ');
lst1.Items.Add('the pin is ' + rdata);

lst1.Items.Add('read the data from $f5 to $fa');
getmem(rdata,256);
st := dc_read_4442_hex(icdev,$f5,6,rdata);
if st <> 0 then
begin
  lst1.Items.Add('read the data error');
  quit;
  Exit;
end;
lst1.Items.Add('read the data ok');
lst1.Items.Add('the data is :' + rdata);

lst1.Items.Add('write the data from $f5 to $fa as 123456789012');

st := dc_write_4442_hex(icdev,$f5,6,'123456789012');
if st <> 0 then
begin
  lst1.Items.Add('write the data error');
  quit;
  Exit;
end;
lst1.Items.Add('write the data ok');

lst1.Items.Add('read the data from $f5 to $fa');
getmem(rdata,256);
st := dc_read_4442_hex(icdev,$f5,6,rdata);
if st <> 0 then
begin
  lst1.Items.Add('read the data error');
  quit;
  exit;
end;
lst1.Items.Add('read the data ok');
lst1.Items.Add('the data is :' + rdata);

lst1.Items.Add('write the data from $f5 to $fa as ffffffffffff');

st := dc_write_4442_hex(icdev,$f5,6,'ffffffffffff');
if st <> 0 then
begin
  lst1.Items.Add('write the data error');
  quit;
  Exit;
end;
lst1.Items.Add('write the data ok');
dc_beep(icdev,10);
quit;
  end;

procedure TForm1.btn2Click(Sender: TObject);
var
  st:smallint;
   cardmode:Integer;
     tempint:longword;
 rlen:Byte;
 readdata:array[0..256]of char;
 senddata:PChar;
  rdata: pchar;

begin
  clearText;
   icdev := dc_init(100, 115200);
  If icdev < 0 Then begin
      lst1.items.Add('dc init error!');
      exit;
 end;
 lst1.Items.add('dc init ok');

   cardmode := 0;
 dc_config_card(icdev,$41);


 st := dc_card(icdev,cardmode,tempint);
 If st <> 0 Then begin
      lst1.items.add('dc card error');
       quit;exit;
 End;
 lst1.items.add('dc card ok');
 lst1.items.add('card id : '+inttohex((tempint),8));
 
  getmem(rdata, 256);
    st := dc_pro_resethex(icdev, rlen, rdata);

  If st <> 0 Then begin
      lst1.items.add('dc_pro_resst error');
       quit;exit;
 End;
 lst1.items.add('dc_pro_rest ok');
 lst1.items.add('the length of reset information:'+inttostr(rlen));
 lst1.items.add('the reset data : '+rdata);;

  senddata :='0084000008' ;
st := dc_pro_commandlink_hex(icdev,5,senddata,rlen,readdata,7,56,);
if st<> 0 then begin
  lst1.Items.Add('get a random number error ');
  lst1.Items.Add('error code '+IntToStr(st));
   quit;exit;
end;
  lst1.Items.Add('get a random number ok ');
lst1.Items.Add('the random number is '+ readdata);
dc_beep(icdev,10);
  quit;
end;

procedure TForm1.btn1Click(Sender: TObject);
var
  hexkey:pchar;
  databuff32,datadevstr:pchar;
  cardmode:Integer;
  sector:Integer;
  address:Integer;
  tempint:longword;
  st:smallint;
  readdata:array[0..32]of char;
  buffer:pchar;

begin
  clearText;
   icdev := dc_init(100, 115200);     //³õÊ¼»¯usb¿Ú

  If icdev < 0 Then begin
      lst1.items.Add('dc_init error!');
     exit;
 end;
 lst1.Items.add('dc_init ok!');
 
  hexkey :='ffffffffffff';
 //hexkey := 'a0a1a2a3a4a5';
 st := dc_load_key_hex(icdev, 0, 0,pchar(hexkey));
 If st <> 0 Then begin
      lst1.items.add('dc_Load_key_hex error!');
      quit;
      exit;
 end;

 lst1.items.add('dc_Load_key_hex ok');
 
  cardmode := 1;
 address  := 2 ;
 sector   := 0 ;

  dc_config_card(icdev,$41);

  GetMem(buffer,256);
 st := dc_card_double_hex(icdev,cardmode,buffer);
 If st <> 0 Then begin
      lst1.items.add('dc_card error');
      quit;
      Exit;
 End;
 lst1.items.add('dc_card ok ');
 lst1.items.add('card id :'+buffer);

 st := dc_authentication_pass_hex(icdev, 0, sector,hexkey);
 If st <> 0 Then begin
      lst1.Items.Add('dc_authentication error!');
      quit;
      Exit;
 End;
 lst1.Items.Add('dc_authentication ok!');

 databuff32:='00112233445566778899AABBCCDDEEFF';
 lst1.Items.add('write 00112233445566778899AABBCCDDEEFF');
 st := dc_write_hex(icdev, address, databuff32);
 If st <> 0 Then begin
     lst1.Items.add('dc_write_hex  error');
     quit;
     Exit;
 End;
 lst1.Items.add('dc_write_hex ok');

 getmem(datadevStr,33);
 st := dc_read_hex(icdev, address,readdata);
 If st <> 0 Then begin
     lst1.Items.add('dc_read_hex error');
     freemem(datadevstr);
     quit;
     Exit;
 End;
 lst1.Items.add('dc_read_hex ok');
 lst1.items.add('the data is :'+readdata);
 freemem(datadevstr);
 dc_beep(icdev,10);
 quit;

end;


procedure TForm1.btn7Click(Sender: TObject);
var
  rdata:PChar;

begin
  clearText;
icdev := dc_init(100,115200);
if(icdev <= 0) then begin
  lst1.Items.Add('dc init error');
  Exit;
end;
lst1.Items.Add('dc card ok');

st:= dc_setcpu(icdev,12);

  lst1.Items.Add('write the data from 0 to 5 as 123456789012');

st := dc_write_24c_hex(icdev,0,6,'123456789012');
if st <> 0 then
begin
  lst1.Items.Add('write the data error');
  quit;
  Exit;
end;
lst1.Items.Add('write the data ok');

lst1.Items.Add('read the data from 0 to 5');
getmem(rdata,256);
st := dc_read_24c_hex(icdev,0,6,rdata);
if st <> 0 then
begin
  lst1.Items.Add('read the data error');
  quit;
  exit;
end;
lst1.Items.Add('read the data ok');
lst1.Items.Add('the data is :' + rdata);
dc_beep(icdev,10);
quit;
end;

procedure TForm1.btn8Click(Sender: TObject);
var
  rdata:PChar;

begin
  clearText;
icdev := dc_init(100,115200);
if(icdev <= 0) then begin
  lst1.Items.Add('dc init error');
  Exit;
end;
lst1.Items.Add('dc card ok');

st:= dc_setcpu(icdev,12);



      lst1.Items.Add('write the data from 0 to 5 as 123456789012');

st := dc_write_24c64_hex(icdev,0,6,'123456789012');
if st <> 0 then
begin
  lst1.Items.Add('write the data error');
  quit;
  Exit;
end;
lst1.Items.Add('write the data ok');

lst1.Items.Add('read the data from 0 to 5');
getmem(rdata,256);
st := dc_read_24c64_hex(icdev,0,6,rdata);
if st <> 0 then
begin
  lst1.Items.Add('read the data error');
  quit;
  exit;
end;
lst1.Items.Add('read the data ok');
lst1.Items.Add('the data is :' + rdata);
dc_beep(icdev,10);
quit;
end;

end.


