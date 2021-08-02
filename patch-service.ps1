#!/usr/bin/env pwsh
param(
    [string]$installHome="${env:SystemDrive}/opt/virtualdesktop"
)

# Verify $installHome exists
New-Item -Force -ItemType Directory -Path "${installHome}" > $null

# Run MSBuild
msbuild "${PSScriptRoot}/VirtualDesktop.sln" /target:"clean;build" /property:Configuration=Debug
msbuild "${PSScriptRoot}/VirtualDesktop.sln" /target:"clean;build" /property:Configuration=Release

# Stop Service
sudo Stop-Service -Force -Name VirtualDesktopService

# Copy up Conf and Binary Files
if(!(Test-Path -Path "${PSScriptRoot}/VirtualDesktop/bin/Release/VirtualDesktop.conf")){ Copy-Item -Path "${PSScriptRoot}/VirtualDesktop/bin/Release/VirtualDesktop.conf" "${installHome}/" }
Copy-Item -Path "${PSScriptRoot}/VirtualDesktop/bin/Release/VirtualDesktop.exe" "${installHome}/"
Copy-Item -Path "${PSScriptRoot}/VirtualDesktopService/bin/Release/VirtualDesktopService.exe" "${installHome}/"

# Re-install Windows Service
sudo installutil /u "${installHome}/VirtualDesktopService.exe"
sudo installutil "${installHome}/VirtualDesktopService.exe"

# Start re-installed service back up
sudo Start-Service -Name VirtualDesktopService
