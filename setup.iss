[Setup]
AppName=LostKit
AppVersion=1.0
DefaultDirName={pf}\LostKit
DefaultGroupName=LostKit
OutputDir=.
OutputBaseFilename=LostKitSetup

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{commondesktop}\LostKit"; Filename: "{app}\LostKit.exe"

[Run]
Filename: "{app}\LostKit.exe"; Description: "Launch LostKit"; Flags: nowait postinstall skipifsilent