@echo off

set PJ_D_ROOT=%~dp0..

rem External Tools: powershell (from PowerShell)
set POWERSHELL=powershell
rem External Tools: 7z (from 7-Zip)
set SEVENZIP=7z

rem Detect powershell
%POWERSHELL% /? 1>nul 2>&1
if errorlevel 1 (
   echo %POWERSHELL%: NOT FOUND
   exit /b 1
)
echo %POWERSHELL%: Found in path

rem Detect 7z
%SEVENZIP% 1>nul 2>&1
if errorlevel 1 (
   echo %SEVENZIP%: NOT FOUND
   exit /b 1
)
echo %SEVENZIP%: Found in path

cd "%PJ_D_ROOT%"

rem %POWERSHELL% -ExecutionPolicy Unrestricted -File .\build.ps1 -Configuration Release -Target Publish-NuGet-Package -ScriptArgs '--revision="100"'
%POWERSHELL% -ExecutionPolicy Unrestricted -File .\build.ps1