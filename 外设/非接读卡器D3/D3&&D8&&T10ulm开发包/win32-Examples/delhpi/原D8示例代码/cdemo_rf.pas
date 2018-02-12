unit cdemo_rf;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls;

type
  Tfrm_demo_rf = class(TForm)
    List1: TListBox;
    Button1: TButton;
    Button2: TButton;
    Button3: TButton;
    Button4: TButton;
    Button5: TButton;
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure Button4Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure Button5Click(Sender: TObject);

  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  frm_demo_rf:Tfrm_demo_rf;
  hexkey:pchar;
  hexkey2:pchar;
  tagtype :Longint;
  snr:longword;
  data32:pchar;
  databuff32,datadevstr:pchar;
  rvalue:Longint;
  wvalue:Longint;
  cardmode:Integer;
  loadmode:Integer;
  sector:Integer;
  address:Integer;
  size:Byte;
  tempint:longword;
  st:smallint;
  readdata:array[0..32]of char;
implementation

uses drv_unit;

{$R *.DFM}

procedure Tfrm_demo_rf.Button1Click(Sender: TObject);
begin

  icdev := dc_init(0, 115200);
  If icdev < 0 Then begin
      List1.items.Add('调用 dc_init()函数出错!');
     exit;
 end;
 List1.Items.add('调用 dc_init()函数成功!');
end;

procedure Tfrm_demo_rf.Button2Click(Sender: TObject);
begin
 hexkey :='ffffffffffff';
 //hexkey := 'a0a1a2a3a4a5';
 st := dc_load_key_hex(icdev, 0, 0,pchar(hexkey));
 If st <> 0 Then begin
      List1.items.add('调用 dc_Load_key_hex()函数出错!');
      exit;
 end;

 List1.items.add('调用 dc_Load_key_hex()函数成功!');

 end;

procedure Tfrm_demo_rf.Button3Click(Sender: TObject);

begin
 cardmode := 1;//MODE为1可反复寻卡, MODE为0卡必须拿开才可重新操作!
 address  := 2 ;
 sector   := 0 ;
 st := dc_beep(icdev, 10);
 If st <> 0 Then begin
      List1.items.Add('调用 dc_beep()函数出错');
      Exit;
 End;
 List1.items.add('调用 dc_beep()函数成功');
 application.ProcessMessages;

 st := dc_disp_str(icdev,'123.45');
 If st <> 0 Then begin
      List1.items.Add('调用 dc_disp_str()函数出错');
      Exit;
 End;
 List1.items.add('调用 dc_disp_str()函数成功');
 application.ProcessMessages;

 st := dc_card(icdev,cardmode,tempint);
 If st <> 0 Then begin
      List1.items.add('调用 dc_card()函数出错');
      Exit;
 End;
 List1.items.add('调用 dc_card()函数成功');
 List1.items.add('序列号:'+inttostr(tempint));

 st := dc_authentication(icdev, 0, sector);
 If st <> 0 Then begin
      List1.Items.Add('调用 dc_authentication()函数出错!');
      Exit;
 End;
 List1.Items.Add('调用 dc_authentication()函数成功!');

 databuff32:='00112233445566778899AABBCCDDEEFF';
 st := dc_write_hex(icdev, address, databuff32);
 If st <> 0 Then begin
     List1.Items.add('调用 dc_write_hex函数出错');
     Exit;
 End;
 List1.Items.add('调用 dc_write_hex函数成功');
 //getmem(datadevStr,33);
 st := dc_read_hex(icdev, address,readdata);
 If st <> 0 Then begin
     List1.Items.add('调用 dc_read_hex函数出错');
     //freemem(datadevstr);
     Exit;
 End;
 List1.Items.add('调用 dc_read_hex函数成功');
 List1.items.add('数据为:'+readdata);
// freemem(datadevstr);

 st := dc_halt(icdev);
 If st <> 0 Then begin
      List1.Items.add('调用 dc_halt()函数出错');
      Exit;
 End;
 List1.Items.add('调用 dc_halt()函数成功');
 List1.Items.add('读写设备测试通过!');

end;

procedure Tfrm_demo_rf.Button5Click(Sender: TObject);
begin
//下面例子将2扇区的A套密码从ffffffffffff
//改为112233445566,然后再改回ffffffffffff
//该该密码程序针对的卡型是MIFARE ONE
 cardmode := 1 ;
 sector   := 2 ;

 hexkey :='ffffffffffff';
 st := dc_load_key_hex(icdev, 0, 2,pchar(hexkey));
 If st <> 0 Then begin
      List1.items.add('调用 dc_Load_key_hex()函数出错!');
      exit;
 end;
 List1.items.add('调用 dc_Load_key_hex()函数成功!');

 st := dc_card(icdev,cardmode,tempint);
 If st <> 0 Then begin
      List1.items.add('调用 dc_card()函数出错');

      Exit;
 End;
 List1.items.add('调用 dc_card()函数成功');

 st := dc_authentication(icdev, 0, sector);
 If st <> 0 Then begin
      List1.Items.Add('调用 dc_authentication()函数出错!');
      Exit;
 End;
 List1.Items.Add('调用 dc_authentication()函数成功!');

//现在把A密码改为112233445566,B密码写为FFFFFFFFFFFF
  data32 := '112233445566FF078069FFFFFFFFFFFF';
  st := dc_write_hex(icdev,2*4+3,data32);
 If st <> 0 Then begin
      List1.Items.Add('调用 dc_write_hex()函数出错!');
      Exit;
 End;
    List1.Items.Add('A密码改为了112233445566');
 //上面两个写函数完成后,密码改为了112233445566


 //现在重新装入密码,才能正确操作
 hexkey :='112233445566';
 st := dc_load_key_hex(icdev, 0, 2,pchar(hexkey));
 If st <> 0 Then begin
      List1.items.add('调用 dc_Load_key_hex()函数出错!');
      exit;
 end;
 List1.items.add('调用 dc_Load_key_hex()函数成功!');

  st := dc_card(icdev,cardmode,tempint);
 If st <> 0 Then begin
      List1.items.add('调用 dc_card()函数出错');

      Exit;
 End;
 List1.items.add('调用 dc_card()函数成功');

 st := dc_authentication(icdev, 0, sector);
 If st <> 0 Then begin
      List1.Items.Add('调用 dc_authentication()函数出错!');
      Exit;
 End;
 List1.Items.Add('调用 dc_authentication()函数成功!');

//现在开始把A密码改回ffffffffffff, b密码改回ffffffffffff
  data32 := 'ffffffffffffff078069ffffffffffff';
  st := dc_write_hex(icdev,2*4+3,data32);
 If st <> 0 Then begin
      List1.Items.Add('调用 dc_write_hex()函数出错!');
      Exit;
 End;

 //现在把密码改回了  ffffffffffff
   List1.Items.Add('密码改回了ffffffffffff');
   List1.Items.Add('修改密码操作完成!');
end;

procedure Tfrm_demo_rf.Button4Click(Sender: TObject);
begin
quit();
close
end;


end.
