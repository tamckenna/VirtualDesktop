#!/usr/bin/env pwsh
param(
    [string]$installHome="${env:SystemDrive}/opt/virtualdesktop",
    [string]$serviceName="VirtualDesktopService",
    [string]$serviceDisplayName="Virtual Desktop Service",
    [string]$serviceDescription="A service managing Virtual Desktop hot keys."
)

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
$exePath=Resolve-Path -Path "${installHome}/VirtualDesktopService.exe"
sudo Stop-Service -Name "$serviceName" 2>&1>$null
sudo Remove-Service -Name "$serviceName" 2>&1>$null
sudo New-Service -Name "'$serviceName'" -DisplayName "'${serviceDisplayName}'" -Description "'${serviceDescription}'" -BinaryPathName "'${exePath}'" -StartupType Automatic >$null

# Start re-installed service back up
sudo Start-Service -Name "$serviceName" >$null
