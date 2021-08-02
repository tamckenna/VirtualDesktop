#!/usr/bin/env pwsh

# Get Project Root Directory
$projectRoot=Resolve-Path -Path "${PSScriptRoot}/../"

# Run MSBuild
msbuild "${projectRoot}/VirtualDesktop.sln" /target:"clean;build" /property:Configuration=Debug /verbosity:minimal
msbuild "${projectRoot}/VirtualDesktop.sln" /target:"clean;build" /property:Configuration=Release /verbosity:minimal
