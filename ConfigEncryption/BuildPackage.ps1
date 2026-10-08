# Builds ConfigEncryption and creates the DNN install package in .\install.
param(
    [string]$Configuration = "Release",
    [string]$DnnBin = "D:\websites\apps.jud12.flcourts.org\bin"
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"

& $msbuild "$root\ConfigEncryption.csproj" /restore /p:Configuration=$Configuration /p:DnnBin=$DnnBin /v:minimal
if ($LASTEXITCODE -ne 0) { throw "Build failed" }

$version = "01.00.00"
$stage = Join-Path ([IO.Path]::GetTempPath()) ("ConfigEncryption_" + [guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $stage | Out-Null
try {
    Compress-Archive -Path "$root\admin\personaBar\*" -DestinationPath "$stage\PersonaBarResources.zip"
    Copy-Item "$root\bin\$Configuration\net48\Tjc.Modules.ConfigEncryption.dll" $stage
    Copy-Item "$root\ConfigEncryption.dnn" $stage

    $installDir = Join-Path $root "install"
    New-Item -ItemType Directory -Force -Path $installDir | Out-Null
    $package = Join-Path $installDir "ConfigEncryption_${version}_Install.zip"
    if (Test-Path $package) { Remove-Item $package }
    Compress-Archive -Path "$stage\*" -DestinationPath $package
    Write-Host "Created $package"
}
finally {
    Remove-Item $stage -Recurse -Force
}
