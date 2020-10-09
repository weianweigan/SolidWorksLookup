set path=%~d0
cd %path%
cd /d %~dp0

RegAsm.exe SldWorks.TestRunner.Addin.dll /u
pause
