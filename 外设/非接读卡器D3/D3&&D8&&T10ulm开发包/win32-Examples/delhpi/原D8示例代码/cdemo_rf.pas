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
      List1.items.Add('���� dc_init()��������!');
     exit;
 end;
 List1.Items.add('���� dc_init()�����ɹ�!');
end;

procedure Tfrm_demo_rf.Button2Click(Sender: TObject);
begin
 hexkey :='ffffffffffff';
 //hexkey := 'a0a1a2a3a4a5';
 st := dc_load_key_hex(icdev, 0, 0,pchar(hexkey));
 If st <> 0 Then begin
      List1.items.add('���� dc_Load_key_hex()��������!');
      exit;
 end;

 List1.items.add('���� dc_Load_key_hex()�����ɹ�!');

 end;

procedure Tfrm_demo_rf.Button3Click(Sender: TObject);

begin
 cardmode := 1;//MODEΪ1�ɷ���Ѱ��, MODEΪ0�������ÿ��ſ����²���!
 address  := 2 ;
 sector   := 0 ;
 st := dc_beep(icdev, 10);
 If st <> 0 Then begin
      List1.items.Add('���� dc_beep()��������');
      Exit;
 End;
 List1.items.add('���� dc_beep()�����ɹ�');
 application.ProcessMessages;

 st := dc_disp_str(icdev,'123.45');
 If st <> 0 Then begin
      List1.items.Add('���� dc_disp_str()��������');
      Exit;
 End;
 List1.items.add('���� dc_disp_str()�����ɹ�');
 application.ProcessMessages;

 st := dc_card(icdev,cardmode,tempint);
 If st <> 0 Then begin
      List1.items.add('���� dc_card()��������');
      Exit;
 End;
 List1.items.add('���� dc_card()�����ɹ�');
 List1.items.add('���к�:'+inttostr(tempint));

 st := dc_authentication(icdev, 0, sector);
 If st <> 0 Then begin
      List1.Items.Add('���� dc_authentication()��������!');
      Exit;
 End;
 List1.Items.Add('���� dc_authentication()�����ɹ�!');

 databuff32:='00112233445566778899AABBCCDDEEFF';
 st := dc_write_hex(icdev, address, databuff32);
 If st <> 0 Then begin
     List1.Items.add('���� dc_write_hex��������');
     Exit;
 End;
 List1.Items.add('���� dc_write_hex�����ɹ�');
 //getmem(datadevStr,33);
 st := dc_read_hex(icdev, address,readdata);
 If st <> 0 Then begin
     List1.Items.add('���� dc_read_hex��������');
     //freemem(datadevstr);
     Exit;
 End;
 List1.Items.add('���� dc_read_hex�����ɹ�');
 List1.items.add('����Ϊ:'+readdata);
// freemem(datadevstr);

 st := dc_halt(icdev);
 If st <> 0 Then begin
      List1.Items.add('���� dc_halt()��������');
      Exit;
 End;
 List1.Items.add('���� dc_halt()�����ɹ�');
 List1.Items.add('��д�豸����ͨ��!');

end;

procedure Tfrm_demo_rf.Button5Click(Sender: TObject);
begin
//�������ӽ�2������A�������ffffffffffff
//��Ϊ112233445566,Ȼ���ٸĻ�ffffffffffff
//�ø����������ԵĿ�����MIFARE ONE
 cardmode := 1 ;
 sector   := 2 ;

 hexkey :='ffffffffffff';
 st := dc_load_key_hex(icdev, 0, 2,pchar(hexkey));
 If st <> 0 Then begin
      List1.items.add('���� dc_Load_key_hex()��������!');
      exit;
 end;
 List1.items.add('���� dc_Load_key_hex()�����ɹ�!');

 st := dc_card(icdev,cardmode,tempint);
 If st <> 0 Then begin
      List1.items.add('���� dc_card()��������');

      Exit;
 End;
 List1.items.add('���� dc_card()�����ɹ�');

 st := dc_authentication(icdev, 0, sector);
 If st <> 0 Then begin
      List1.Items.Add('���� dc_authentication()��������!');
      Exit;
 End;
 List1.Items.Add('���� dc_authentication()�����ɹ�!');

//���ڰ�A�����Ϊ112233445566,B����дΪFFFFFFFFFFFF
  data32 := '112233445566FF078069FFFFFFFFFFFF';
  st := dc_write_hex(icdev,2*4+3,data32);
 If st <> 0 Then begin
      List1.Items.Add('���� dc_write_hex()��������!');
      Exit;
 End;
    List1.Items.Add('A�����Ϊ��112233445566');
 //��������д������ɺ�,�����Ϊ��112233445566


 //��������װ������,������ȷ����
 hexkey :='112233445566';
 st := dc_load_key_hex(icdev, 0, 2,pchar(hexkey));
 If st <> 0 Then begin
      List1.items.add('���� dc_Load_key_hex()��������!');
      exit;
 end;
 List1.items.add('���� dc_Load_key_hex()�����ɹ�!');

  st := dc_card(icdev,cardmode,tempint);
 If st <> 0 Then begin
      List1.items.add('���� dc_card()��������');

      Exit;
 End;
 List1.items.add('���� dc_card()�����ɹ�');

 st := dc_authentication(icdev, 0, sector);
 If st <> 0 Then begin
      List1.Items.Add('���� dc_authentication()��������!');
      Exit;
 End;
 List1.Items.Add('���� dc_authentication()�����ɹ�!');

//���ڿ�ʼ��A����Ļ�ffffffffffff, b����Ļ�ffffffffffff
  data32 := 'ffffffffffffff078069ffffffffffff';
  st := dc_write_hex(icdev,2*4+3,data32);
 If st <> 0 Then begin
      List1.Items.Add('���� dc_write_hex()��������!');
      Exit;
 End;

 //���ڰ�����Ļ���  ffffffffffff
   List1.Items.Add('����Ļ���ffffffffffff');
   List1.Items.Add('�޸�����������!');
end;

procedure Tfrm_demo_rf.Button4Click(Sender: TObject);
begin
quit();
close
end;


end.
