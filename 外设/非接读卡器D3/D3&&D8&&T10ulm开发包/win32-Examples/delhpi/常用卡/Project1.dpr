program Project1;

uses
  Forms,
  delphi in 'delphi.pas' {Form1},
  drv_unit in 'drv_unit.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.
