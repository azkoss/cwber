unit drv_unit;
 interface
var
   icdev:longint;//COM1,COM2,ParallelPort 公用设备句柄
   st:smallint;
  procedure quit;
 //comm function
 Function dc_init(prot:integer;baud:longint):longint; stdcall;
  far;external 'DCRF32.dll' name 'dc_init';
 Function dc_exit(icdev:longint):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_exit' ;
 Function dc_card(icdev:longint;mode:smallint;var snr:longword):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_card';
 Function dc_load_key_hex(icdev:longint;mode,secor:smallint;skey:pchar):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_load_key_hex';
 Function dc_authentication(icdev:longint;mode,secor:smallint):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_authentication';
 Function dc_read_hex(icdev:longint;adr:smallint;sdata :pchar):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_read_hex';
 Function dc_write_hex(icdev:longint;adr:smallint;sdata :pchar):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_write_hex';
 Function dc_halt(icdev:longint):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_halt';
 Function dc_reset(icdev:longint;msc:smallint):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_reset';
 Function dc_beep(icdev:longint;stime:smallint):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_beep';
 Function dc_disp_str(icdev:longint;sdata :pchar):smallint; stdcall;
  far;external 'DCRF32.dll' name 'dc_disp_str';
implementation

procedure quit;
  begin
    If icdev > 0 Then begin
       st := dc_reset(icdev, 10);
       st := dc_exit(icdev);
       icdev := -1;
    end;
end;
{
initialization
begin

end;

finalization
begin
end;
 }
end.