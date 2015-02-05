cd C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319

rem uninstall
installutil /u C:\VAPPCTComm\vappctcomm.exe
pause

rem reinstall
installutil C:\VAPPCTComm\vappctcomm.exe
pause

net start VAPPCTCommService
pause