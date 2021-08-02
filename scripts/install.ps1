#!/usr/bin/env pwsh
param(
    [string]$name="VirtualDesktop",
    [string]$installHome="${env:SystemDrive}/opt/virtualdesktop",
    [string]$type="Release"
)

# Get Project Root Directory
$projectRoot=Resolve-Path -Path "${PSScriptRoot}/../"

# Verify $installHome exists
New-Item -Force -ItemType Directory -Path "${installHome}" > $null

# Stop Process if running
Stop-Process -Force -Name "${name}" 2>&1>$null

# Copy up Conf and Binary File
if(!(Test-Path -Path "${projectRoot}/${name}/bin/${type}/${name}.conf")){ Copy-Item -Path "${PSScriptRoot}/${name}/bin/Release/${name}.conf" "${installHome}/" }
Copy-Item -Path "${projectRoot}/${name}/bin/${type}/${name}.exe" "${installHome}/"

# Update Registry for Startup
$binaryPath=Resolve-Path -Path "${installHome}/${name}.exe"
$registryPath="HKLM:\Software\Microsoft\Windows\CurrentVersion\Run"
sudo "New-ItemProperty -Force -Path \""$registryPath\"" -Name \""$name\"" -Value '\""${binaryPath}\""' -PropertyType String"

# Start Process back up
Start-Process -FilePath "$binaryPath"

