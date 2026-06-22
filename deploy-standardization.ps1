# Deploy standardization-refactor changes to dev and test DNN sites.
# Maps each source module to its deployed DesktopModules folder name,
# copies modified .ascx/.ascx.cs/.ascx.designer.cs/.css/.js files,
# and copies built DLLs from each module's bin/Debug to the site bin.

param(
    [string]$BranchBase = "master",
    [switch]$DevOnly,
    [switch]$TestOnly,
    [switch]$WhatIf,
    [string[]]$SkipDllFor = @()
)

$ErrorActionPreference = "Stop"
$src = "D:\OneDrive - jud12fl\SourceCode\tjc.modules.local"
$devSite = "D:\websites\apps.jud12.flcourts.org"
$testSite = "M:\Websites\Intranet.jud12.local"

# Mapping: source folder name -> DesktopModules\tjc.modules\<folder>
# AudioRequest is intentionally excluded — not in the solution and not deployed.
$folderMap = @{
    "CourtCounsel"              = "CourtCounsel"
    "CourtRegistry"             = "CourtRegistry"
    "DigitalCourtReporting"     = "DigitalCourtReporting"
    "DocketInmateCompare"       = "DocketInmateCompare"
    "EmployeeDB"                = "EmployeeDB"
    "ExpertWitness"             = "ExpertWitness"
    "FamilySelfHelp"            = "FamilySelfHelp"
    "HearingsLog"               = "HearingsLog"
    "JudgeVacation"             = "JudgeVacation"
    "JudicialReferral"          = "JudicialReferral"
    "ManateeJacsCaseMaintenace" = "JacsMaintenance"
    "MediationStatistics"       = "Mediation"
    "PretrialServices"          = "PretrialServices"
    "PretrialServicesSarasota"  = "PretrialServicesSarasota"
    "ProSeLog"                  = "ProSeLog"
    "Purchasing"                = "Purchasing"
    "Record-Destruction"        = "DestructionLog"
    "Reports"                   = "EmployeeReports"
    "ThreatReport"              = "ThreatReport"
    "TranscriptDatabase"        = "TranscriptDatabase"
    "ZoomConnector"             = "ZoomConnector"
}

# Get modified files in this branch via git
Push-Location $src
$diffArgs = @("diff", "--name-only", "--diff-filter=AM", "$BranchBase...HEAD")
$changedFiles = & git @diffArgs 2>$null
Pop-Location

if (-not $changedFiles) {
    Write-Host "No changed files vs $BranchBase."
    exit 0
}

# Group changed files by source module folder
$byModule = @{}
foreach ($f in $changedFiles) {
    $module = ($f -split "/")[0]
    if (-not $folderMap.ContainsKey($module)) { continue }
    if (-not $byModule.ContainsKey($module)) { $byModule[$module] = @() }
    $byModule[$module] += $f
}

function Deploy-Module {
    param($Module, $Files, $SitePath, $Label)
    $target = $folderMap[$Module]
    if (-not $target) {
        Write-Host "[$Label] SKIP $Module (no deploy mapping)" -ForegroundColor Yellow
        return
    }
    $targetPath = Join-Path $SitePath "DesktopModules\tjc.modules\$target"
    if (-not (Test-Path $targetPath)) {
        Write-Host "[$Label] SKIP $Module -> $target (target folder not present)" -ForegroundColor Yellow
        return
    }

    $copied = 0
    foreach ($f in $Files) {
        # Source path
        $srcFile = Join-Path $src ($f -replace "/", "\")
        if (-not (Test-Path $srcFile)) { continue }

        # Skip server-side .cs (no need to deploy raw .cs files; DLL handles it)
        # but keep .ascx.designer.cs/.ascx.cs as they are part of the DNN package
        $ext = [System.IO.Path]::GetExtension($f).ToLower()
        $deployableExts = @(".ascx",".ashx",".css",".js",".resx",".dnn",".html")
        if (-not ($deployableExts -contains $ext)) {
            # Skip .cs files - they're compiled into the DLL
            continue
        }

        # Relative path within the source module
        $relWithinModule = ($f -replace "/", "\") -replace "^$([regex]::Escape($Module))\\", ""
        $destFile = Join-Path $targetPath $relWithinModule
        $destDir = [System.IO.Path]::GetDirectoryName($destFile)
        if (-not (Test-Path $destDir)) {
            if (-not $WhatIf) { New-Item -ItemType Directory -Path $destDir -Force | Out-Null }
        }
        if ($WhatIf) {
            Write-Host "[$Label] WOULD COPY $srcFile -> $destFile"
        } else {
            Copy-Item -LiteralPath $srcFile -Destination $destFile -Force
            $copied++
        }
    }

    # Copy DLL from bin\Debug (or bin) if present, unless this module's build failed.
    # Module-folder name != assembly name in several cases (e.g. Record-Destruction ->
    # tjc.Modules.RecordDestruction.dll, ManateeJacsCaseMaintenace -> tjc.Modules.JacsCaseMaint.dll),
    # so resolve by reading AssemblyName from the .csproj.
    if ($SkipDllFor -contains $Module) {
        Write-Host ("[$Label] {0,-26} -> {1,-26} ({2} files, DLL SKIPPED)" -f $Module, $target, $copied)
        return
    }
    $csproj = Get-ChildItem (Join-Path $src $Module) -Filter "*.csproj" -ErrorAction SilentlyContinue | Select-Object -First 1
    $asmName = $null
    if ($csproj) {
        $csprojXml = [xml](Get-Content $csproj.FullName -Raw)
        $asmName = $csprojXml.Project.PropertyGroup | ForEach-Object { $_.AssemblyName } | Where-Object { $_ } | Select-Object -First 1
    }
    if (-not $asmName) { $asmName = "tjc.Modules.$Module" }
    $dllName = "$asmName.dll"
    $dllSrc = Join-Path $src "$Module\bin\Debug\$dllName"
    if (-not (Test-Path $dllSrc)) {
        $dllSrc = Join-Path $src "$Module\bin\$dllName"
    }
    if (Test-Path $dllSrc) {
        $dllDest = Join-Path $SitePath "bin\$dllName"
        if ($WhatIf) {
            Write-Host "[$Label] WOULD COPY DLL $dllSrc -> $dllDest"
        } else {
            Copy-Item -LiteralPath $dllSrc -Destination $dllDest -Force
            $copied++
        }
    }

    Write-Host ("[$Label] {0,-26} -> {1,-26} ({2} files)" -f $Module, $target, $copied)
}

$targets = @()
if (-not $TestOnly) { $targets += @{ Path = $devSite;  Label = "DEV " } }
if (-not $DevOnly)  { $targets += @{ Path = $testSite; Label = "TEST" } }

foreach ($t in $targets) {
    if (-not (Test-Path $t.Path)) {
        Write-Host "[$($t.Label)] SKIP - site path not present: $($t.Path)" -ForegroundColor Yellow
        continue
    }
    Write-Host "=== Deploying to $($t.Label) ($($t.Path)) ==="
    foreach ($module in ($byModule.Keys | Sort-Object)) {
        Deploy-Module -Module $module -Files $byModule[$module] -SitePath $t.Path -Label $t.Label
    }
}

Write-Host "Done."
