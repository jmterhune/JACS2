# Fire-and-forget GET to the dev site after a module DLL is dropped into bin\.
# First request after a bin\ change triggers IIS to recycle the app pool, so this
# is what actually picks up the new assembly. The request runs in a detached,
# hidden PowerShell so the build finishes immediately.

param(
    [string]$Url = $env:DEV_SITE_URL
)

if ([string]::IsNullOrWhiteSpace($Url)) { $Url = 'https://www.dnndev.me/' }

$args = @(
    '-NoProfile',
    '-Command',
    "try { Invoke-WebRequest -Uri '$Url' -UseBasicParsing -TimeoutSec 120 -ErrorAction SilentlyContinue | Out-Null } catch {}"
)

Start-Process -WindowStyle Hidden -FilePath powershell.exe -ArgumentList $args | Out-Null
Write-Host "Ping dispatched: $Url"
