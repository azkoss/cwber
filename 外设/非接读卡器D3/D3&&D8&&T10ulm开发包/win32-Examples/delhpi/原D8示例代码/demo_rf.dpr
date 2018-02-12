program demo_rf;

uses
  Forms,
  cdemo_rf in 'cdemo_rf.pas' {frm_demo_rf},
  drv_unit in 'drv_unit.pas';

{$R *.RES}

begin
  Application.Initialize;
  Application.CreateForm(Tfrm_demo_rf, frm_demo_rf);
  Application.Run;
end.
