@echo off
setlocal
cd /d "%~dp0" || exit /b 1
if not exist "%~dp0RegAsm.exe" exit /b 2
if not exist "%~dp0SldWorksLookup.dll" exit /b 3
"%~dp0RegAsm.exe" "%~dp0SldWorksLookup.dll" /codebase
if errorlevel 1 exit /b %errorlevel%
exit /b 0
