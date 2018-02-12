unit drv_unit;
 interface
var
   icdev:longint;//COM1,COM2,ParallelPort 公用设备句柄
   st:smallint;
  procedure quit;
 //comm function
 Function dc_init(prot:integer;baud:longint):longint; stdcall;
  far;external './DCRF32.dll' name 'dc_init';
 Function dc_exit(icdev:longint):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_exit' ;
 Function dc_card(icdev:longint;mode:smallint;var snr:longword):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_card';
 Function dc_card_double_hex(icdev:longint;mode:smallint;buffer:pchar):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_card_double_hex';
 Function dc_load_key_hex(icdev:longint;mode,secor:smallint;skey:pchar):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_load_key_hex';
 Function dc_authentication(icdev:longint;mode,secor:smallint):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_authentication';
 function dc_authentication_pass_hex(icdev:longint;mode,secor:smallint;skey:pchar):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_authentication_pass_hex';
 Function dc_read_hex(icdev:longint;adr:smallint;sdata :pchar):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_read_hex';
 Function dc_write_hex(icdev:longint;adr:smallint;sdata :pchar):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_write_hex';
 Function dc_halt(icdev:longint):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_halt';
 Function dc_reset(icdev:longint;msc:smallint):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_reset';
 Function dc_beep(icdev:longint;stime:smallint):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_beep';
 Function dc_disp_str(icdev:longint;sdata :pchar):smallint; stdcall;
  far;external './DCRF32.dll' name 'dc_disp_str';

  function  dc_config_card(icdev:longint;cardtype:byte):smallint;stdcall;
  far;external './DCRF32.dll' name 'dc_config_card';
 Function dc_pro_resethex(handle:longint; var rlen:byte; rdata:pchar):smallint;
stdcall; far; external './DCRF32.dll' name 'dc_pro_resethex';

Function dc_pro_commandlink_hex(handle:longint; slen:byte; sdata:pchar; var rlen:byte; rdata:pchar; timeout:byte; fg:byte):smallint;
stdcall; far; external './DCRF32.dll' name 'dc_pro_commandlink_hex';

function dc_card_b_hex(icdev:LongInt;rbuff:PChar):Smallint;
  stdcall;far;external './DCRF32.dll' name 'dc_card_b_hex';

function  dc_pro_commandhex(icdev:LongInt;slen:Byte;sendbuff:PChar;var rlen:Byte;recvbuff:PChar;timeout:Byte):Smallint;
  stdcall;far;external './DCRF32.dll' name 'dc_pro_commandhex';

function dc_setcpu(icdev:LongInt;_type:Byte):Smallint;
  stdcall;far;external './DCRF32.dll' name 'dc_setcpu';

  function dc_cpureset_hex(icdev:LongInt;var rlen:byte;rdata:PChar):Smallint;
  stdcall;far;external './DCRF32.dll' name 'dc_cpureset_hex';

  function dc_cpuapdu_hex(icdev:LongInt;slen:Byte;sendbuffer:PChar;var rlen:Byte;databuff:pchar):Smallint;
  stdcall;far;external './DCRF32.dll' name 'dc_cpuapdu_hex';


function dc_read_4428_hex(icdev:LongInt;offset:Smallint;lenth:smallint;buffer:pchar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_read_4428_hex';
function dc_write_4428_hex(icdev:LongInt;offset:Smallint;lenth:Smallint;buffer:PChar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_write_4428_hex';
function dc_verifypin_4428_hex(icdev:LongInt;passwd:PChar):smallint;
stdcall;far;external './DCRF32.dll' name 'dc_verifypin_4428_hex';
function dc_readpin_4428_hex(icdev:LongInt;passwd:PChar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_readpin_4428_hex';
function dc_readpincount_4428(icdev:LongInt):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_readpincount_4428';

function dc_changepin_4428_hex(icdev:LongInt;passwd:PChar):SmallInt;
stdcall;far;external './DCRF32.dll' name 'dc_changepin_4428_hex';

function dc_read_4442_hex(icdev:LongInt;offset:Smallint;lenth:smallint;buffer:pchar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_read_4442_hex';
function dc_write_4442_hex(icdev:LongInt;offset:Smallint;lenth:Smallint;buffer:PChar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_write_4442_hex';
function dc_verifypin_4442_hex(icdev:LongInt;passwd:PChar):smallint;
stdcall;far;external './DCRF32.dll' name 'dc_verifypin_4442_hex';
function dc_readpin_4442_hex(icdev:LongInt;passwd:PChar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_readpin_4442_hex';
function dc_readpincount_4442(icdev:LongInt):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_readpincount_4442';

function dc_changepin_4442_hex(icdev:LongInt;passwd:PChar):SmallInt;
stdcall;far;external './DCRF32.dll' name 'dc_changepin_4442_hex';

function dc_checkCard (icdev:LongInt):Smallint ;
stdcall ;far;external './DCRF32.dll' name 'dc_CheckCard';
function dc_read_24c_hex(icdev:LongInt;offset:Smallint;lenth:smallint;buffer:pchar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_read_24c_hex';
function dc_write_24c_hex(icdev:LongInt;offset:Smallint;lenth:Smallint;buffer:PChar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_write_24c_hex';
function dc_read_24c64_hex(icdev:LongInt;offset:Smallint;lenth:smallint;buffer:pchar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_read_24c64_hex';
function dc_write_24c64_hex(icdev:LongInt;offset:Smallint;lenth:Smallint;buffer:PChar):Smallint;
stdcall;far;external './DCRF32.dll' name 'dc_write_24c64_hex';

implementation

procedure quit;
  begin
    If icdev > 0 Then begin
       st := dc_reset(icdev, 10);
       st := dc_exit(icdev);
       icdev := -1;
    end;
end;

end.
