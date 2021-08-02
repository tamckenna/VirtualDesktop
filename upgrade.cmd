@echo off
"pwsh.exe" -c "%~dp0\scripts\compile.ps1 %*"
"pwsh.exe" -c "%~dp0\scripts\install.ps1 %*"
