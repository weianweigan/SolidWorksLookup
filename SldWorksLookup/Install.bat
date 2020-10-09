set path=%~d0
cd %path%
cd /d %~dp0

RegAsm.exe SldWorksLookup.dll /codebase
pause
