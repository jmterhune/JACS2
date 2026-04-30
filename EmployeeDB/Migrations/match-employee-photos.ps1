# --------------------------------------------------------------------------
# Match employee photo files in M:\Websites\Intranet.jud12.local\Portals\0\
# Images\Staff to employees in tjc_employee, by normalized name comparison.
#
# Reasoning: the test DB inherited FileIds from production but the test
# portal's DNN Files table is a different set, so most current FileId values
# resolve to unrelated transcripts/attachments. We re-derive the link:
#   1. Read every file from the Staff directory.
#   2. For each file, strip non-alphanumerics from the base name to make a
#      normalized key. Same normalization is applied to candidate
#      lastname/firstname combinations per employee. Examples:
#        "Smith, John.jpg"   -> "smithjohn"
#        "Smith-John.jpg"    -> "smithjohn"
#        "Smith_J.jpg"       -> "smithj"
#        "Artman Smith-Michelle.jpg" -> "artmansmithmichelle"
#   3. Build a hashtable keyed on normalized name -> { filename, on-disk-path }.
#   4. For each employee, generate candidate keys in confidence order
#      (LastFirst, FirstLast, LastInitial, etc.) and pick the first one
#      that resolves to exactly one disk file.
#   5. Look up that file in DNN's Files table for its FileId; insert a row
#      if the file is on disk but not registered.
#   6. Output a CSV report of every employee + matched file (or no match).
#
# Modes:
#   .\match-employee-photos.ps1                # dry run, writes report only
#   .\match-employee-photos.ps1 -Apply         # also runs the SQL updates
# --------------------------------------------------------------------------

[CmdletBinding()]
param(
    [switch]$Apply,
    [ValidateSet('dev','test')]
    [string]$Env = 'test'
)

$ErrorActionPreference = 'Stop'

# Connection + folder bindings per environment.
#   dev  - D:\websites\apps.jud12.flcourts.org    (localhost SQL)
#   test - M:\Websites\Intranet.jud12.local        (10.212.72.62 SQL)
# Both DBs are named "intranet.jud12.local" and use the same login.
if ($Env -eq 'dev') {
    $server   = '.'
    $database = 'intranet.jud12.local'
    $user     = 'intranet_web_user'
    $pw       = 'intranet_web_user'
    $staffDir = 'D:\websites\apps.jud12.flcourts.org\Portals\0\Images\Staff'
    $reportPath = Join-Path $PSScriptRoot 'photo-match-report.dev.csv'
} else {
    $server   = '10.212.72.62'
    $database = 'intranet.jud12.local'
    $user     = 'intranet_web_user'
    $pw       = 'intranet_web_user'
    $staffDir = 'M:\Websites\Intranet.jud12.local\Portals\0\Images\Staff'
    $reportPath = Join-Path $PSScriptRoot 'photo-match-report.csv'
}
$portalId      = 0

# Resolve FolderID for Images/Staff/ on this database (FolderID can drift
# between DNN portals; cheaper to look it up than hardcode).
$folderLookup = Invoke-Sqlcmd -ServerInstance $server -Database $database `
    -Username $user -Password $pw `
    -Query "SELECT TOP 1 FolderID FROM dbo.Folders WHERE FolderPath = 'Images/Staff/' AND PortalID = $portalId"
if (-not $folderLookup) { throw "Could not find FolderID for Images/Staff/ on $server.$database" }
$staffFolderId = [int]$folderLookup.FolderID
Write-Host ("Env: {0}  Server: {1}  DB: {2}  StaffDir: {3}  FolderID: {4}" -f $Env, $server, $database, $staffDir, $staffFolderId)

# ------------- Normalization helper --------------
function Normalize([string]$s) {
    if ([string]::IsNullOrWhiteSpace($s)) { return '' }
    # Strip everything that isn't a letter or a digit, then lowercase.
    return ([regex]::Replace($s, '[^A-Za-z0-9]', '')).ToLowerInvariant()
}

# ------------- Pull employees --------------
$empSql = @"
SELECT EmployeeId, LastName, FirstName, MiddleInitial, FileId,
       (SELECT FileName FROM dbo.Files WHERE FileId = e.FileId) AS CurrentFileName
FROM dbo.tjc_employee e
WHERE IsEmployee = 1
ORDER BY LastName, FirstName
"@
$employees = Invoke-Sqlcmd -ServerInstance $server -Database $database `
    -Username $user -Password $pw -Query $empSql

Write-Host "Pulled $($employees.Count) employees"

# ------------- Pull existing DNN Files for Staff folder --------------
$filesSql = "SELECT FileId, FileName FROM dbo.Files WHERE FolderID = $staffFolderId"
$dnnFiles = Invoke-Sqlcmd -ServerInstance $server -Database $database `
    -Username $user -Password $pw -Query $filesSql

$dnnByFilename = @{}
foreach ($f in $dnnFiles) {
    # Filenames are case-sensitive on disk but Windows file system is not --
    # canonicalise on lowercase for the lookup.
    $key = $f.FileName.ToLowerInvariant()
    $dnnByFilename[$key] = $f.FileId
}
Write-Host "DNN Files table has $($dnnFiles.Count) entries for FolderID $staffFolderId"

# ------------- Read Staff dir --------------
$diskFiles = Get-ChildItem $staffDir -File | Where-Object {
    $_.Extension -match '(?i)^\.(jpg|jpeg|png|gif|bmp)$'
}
Write-Host "Staff directory has $($diskFiles.Count) image files"

# Build normalized lookup. Multiple disk files can share a normalized key
# (rare but possible -- e.g. two photo updates of the same person with a
# version suffix). Track collisions so we don't produce ambiguous matches.
$diskByNorm = @{}
foreach ($df in $diskFiles) {
    $base = [System.IO.Path]::GetFileNameWithoutExtension($df.Name)
    $norm = Normalize $base
    if ([string]::IsNullOrEmpty($norm)) { continue }
    if (-not $diskByNorm.ContainsKey($norm)) { $diskByNorm[$norm] = @() }
    $diskByNorm[$norm] += [PSCustomObject]@{
        FileName    = $df.Name
        FullPath    = $df.FullName
        Size        = $df.Length
        Extension   = $df.Extension
        LastWrite   = $df.LastWriteTime
    }
}

# ------------- Match per employee --------------
$results = New-Object System.Collections.Generic.List[object]
$inserts = New-Object System.Collections.Generic.List[object]   # disk-only files to register

foreach ($emp in $employees) {
    $last  = ($emp.LastName  -as [string]).Trim()
    $first = ($emp.FirstName -as [string]).Trim()
    $mi    = ($emp.MiddleInitial -as [string]).Trim()

    # Strip out anything inside double quotes (nicknames like Allyson "Ally")
    $firstClean = [regex]::Replace($first, '"[^"]*"', '').Trim()

    # Pull a "first only" -- first whitespace-separated token of the cleaned
    # first name, since some records have "Mary Jane" etc.
    $firstFirstToken = if ($firstClean) { ($firstClean -split '\s+')[0] } else { '' }

    $firstInit = if ($firstFirstToken) { $firstFirstToken.Substring(0, 1) } else { '' }

    # Strip common name suffixes (Jr., Sr., II, III, IV) from the last name
    # before normalizing. Files generally don't include these in the name.
    $lastNoSuffix = [regex]::Replace($last, '(?i),?\s*(jr|sr|i{2,3}|iv|v)\.?\s*$', '').Trim()

    # First segment of a hyphenated / comma'd / spaced last name. Some legacy
    # files were named with only the first portion (e.g. "Adekoya_A.jpg" for
    # employee "Adekoya-Fitzgerald, Arlean").
    $lastFirstSeg = if ($lastNoSuffix) { ($lastNoSuffix -split '[-,\s]+')[0] } else { '' }

    # Candidate keys, ordered by confidence (most specific first).
    # Each fallback layer widens the search; the first hit wins.
    $candidates = New-Object System.Collections.Generic.List[hashtable]
    function Add-Candidate {
        param($List, [string]$Key, [string]$Why)
        if (-not [string]::IsNullOrEmpty($Key)) {
            $List.Add(@{ key = $Key; why = $Why }) | Out-Null
        }
    }

    # Strongest: full last + full first
    Add-Candidate $candidates (Normalize "$last$firstClean")              'Last+First'
    Add-Candidate $candidates (Normalize "$last$firstFirstToken")         'Last+First1'
    Add-Candidate $candidates (Normalize "$firstClean$last")              'First+Last'
    Add-Candidate $candidates (Normalize "$firstFirstToken$last")         'First1+Last'

    # With middle initial
    Add-Candidate $candidates (Normalize "$last$firstFirstToken$mi")      'Last+First1+MI'

    # Suffix stripped (Jr., Sr., II, III)
    if ($lastNoSuffix -ne $last) {
        Add-Candidate $candidates (Normalize "$lastNoSuffix$firstClean")       'LastNoSfx+First'
        Add-Candidate $candidates (Normalize "$lastNoSuffix$firstFirstToken")  'LastNoSfx+First1'
        Add-Candidate $candidates (Normalize "$firstClean$lastNoSuffix")       'First+LastNoSfx'
        Add-Candidate $candidates (Normalize "$firstFirstToken$lastNoSuffix")  'First1+LastNoSfx'
    }

    # First segment only (handles hyphenated and comma'd last names like
    # Adekoya-Fitzgerald, Allen-Armour, Allen, Jr.)
    if ($lastFirstSeg -ne $lastNoSuffix -and $lastFirstSeg -ne $last) {
        Add-Candidate $candidates (Normalize "$lastFirstSeg$firstClean")        'LastSeg+First'
        Add-Candidate $candidates (Normalize "$lastFirstSeg$firstFirstToken")   'LastSeg+First1'
        Add-Candidate $candidates (Normalize "$firstClean$lastFirstSeg")        'First+LastSeg'
        Add-Candidate $candidates (Normalize "$firstFirstToken$lastFirstSeg")   'First1+LastSeg'
        Add-Candidate $candidates (Normalize "$lastFirstSeg$firstInit")         'LastSeg+Initial'
        Add-Candidate $candidates (Normalize "$firstInit$lastFirstSeg")         'Initial+LastSeg'
    }

    # Initial-only forms (full last name)
    Add-Candidate $candidates (Normalize "$last$firstInit")               'Last+Initial'
    Add-Candidate $candidates (Normalize "$firstInit$last")               'Initial+Last'
    if ($lastNoSuffix -ne $last) {
        Add-Candidate $candidates (Normalize "$lastNoSuffix$firstInit")   'LastNoSfx+Initial'
        Add-Candidate $candidates (Normalize "$firstInit$lastNoSuffix")   'Initial+LastNoSfx'
    }

    # Last name only (least specific, only used if it resolves to a single file)
    Add-Candidate $candidates (Normalize "$last")                         'Last only'

    $matchEntry = $null
    $matchWhy   = $null
    foreach ($c in $candidates) {
        if ($diskByNorm.ContainsKey($c.key)) {
            $hits = $diskByNorm[$c.key]
            if ($hits.Count -eq 1) {
                # Unambiguous -- pick it.
                $matchEntry = $hits[0]
                $matchWhy   = $c.why
                break
            } elseif ($hits.Count -gt 1 -and $c.why -ne 'Last only') {
                # Multiple files normalize to the same key (e.g. .jpg + .JPG
                # duplicates). Prefer the most-recently modified -- usually
                # the one DNN actually serves.
                $matchEntry = $hits | Sort-Object LastWrite -Descending | Select-Object -First 1
                $matchWhy   = "$($c.why) (resolved $($hits.Count) candidates by mtime)"
                break
            }
        }
    }

    # Resolve to an existing FileId, or queue an insert.
    $newFileId = $null
    $newFileName = $null
    if ($matchEntry) {
        $newFileName = $matchEntry.FileName
        $key = $matchEntry.FileName.ToLowerInvariant()
        if ($dnnByFilename.ContainsKey($key)) {
            $newFileId = $dnnByFilename[$key]
        } else {
            # Need to register this on-disk file in DNN. Add to insert queue.
            $inserts.Add([PSCustomObject]@{
                FileName  = $matchEntry.FileName
                FullPath  = $matchEntry.FullPath
                Size      = $matchEntry.Size
                Extension = $matchEntry.Extension
                LastWrite = $matchEntry.LastWrite
            }) | Out-Null
            $newFileId = $null   # filled in after insert
        }
    }

    $results.Add([PSCustomObject]@{
        EmployeeId      = $emp.EmployeeId
        LastName        = $last
        FirstName       = $first
        OldFileId       = $emp.FileId
        OldFileName     = $emp.CurrentFileName
        NewFileName     = $newFileName
        NewFileId       = $newFileId
        MatchReason     = $matchWhy
        Status          = if ($matchEntry) {
                              if ($newFileId) { 'matched-existing' } else { 'matched-needs-register' }
                          } else { 'no-match' }
    }) | Out-Null
}

# ------------- Insert disk-only files into Files table --------------
# Batch them so we don't make one round-trip per row.
if ($inserts.Count -gt 0) {
    Write-Host "$($inserts.Count) on-disk files need to be registered in DNN Files"

    if ($Apply) {
        # De-dupe (an employee twin might pick the same file).
        $unique = $inserts | Sort-Object FileName -Unique
        Write-Host "  ($($unique.Count) unique filenames after dedup)"

        foreach ($u in $unique) {
            $contentType = switch ($u.Extension.ToLowerInvariant()) {
                '.jpg'  { 'image/jpeg' }
                '.jpeg' { 'image/jpeg' }
                '.png'  { 'image/png' }
                '.gif'  { 'image/gif' }
                '.bmp'  { 'image/bmp' }
                default { 'application/octet-stream' }
            }
            $extNoDot = $u.Extension.TrimStart('.')

            # SQL escape: replace single quotes with two single quotes.
            $fnEsc = $u.FileName -replace "'", "''"

            # NOTE: Files.Folder is a COMPUTED column (derived from
            # Folders.FolderPath via FolderID), so it can't be inserted into.
            $sql = @"
DECLARE @FolderId INT = $staffFolderId;
DECLARE @PortalId INT = $portalId;
IF NOT EXISTS (SELECT 1 FROM dbo.Files WHERE FolderID = @FolderId AND FileName = N'$fnEsc')
BEGIN
    INSERT INTO dbo.Files (
        PortalId, FileName, Extension, Size, ContentType, FolderID,
        CreatedByUserID, CreatedOnDate, LastModifiedByUserID, LastModifiedOnDate,
        UniqueId, VersionGuid, LastModificationTime,
        StartDate, EnablePublishPeriod, PublishedVersion, HasBeenPublished
    )
    VALUES (
        @PortalId, N'$fnEsc', N'$extNoDot', $($u.Size), N'$contentType', @FolderId,
        -1, SYSUTCDATETIME(), -1, SYSUTCDATETIME(),
        NEWID(), NEWID(), '$($u.LastWrite.ToString("yyyy-MM-ddTHH:mm:ss"))',
        SYSUTCDATETIME(), 0, 1, 0
    );
END
"@
            Invoke-Sqlcmd -ServerInstance $server -Database $database `
                -Username $user -Password $pw -Query $sql | Out-Null
        }

        # Refresh dnnByFilename so we can re-resolve newly inserted files.
        $dnnFiles = Invoke-Sqlcmd -ServerInstance $server -Database $database `
            -Username $user -Password $pw -Query $filesSql
        $dnnByFilename = @{}
        foreach ($f in $dnnFiles) { $dnnByFilename[$f.FileName.ToLowerInvariant()] = $f.FileId }

        # Re-fill NewFileId on the matched-needs-register results.
        foreach ($r in $results) {
            if ($r.Status -eq 'matched-needs-register') {
                $key = $r.NewFileName.ToLowerInvariant()
                if ($dnnByFilename.ContainsKey($key)) {
                    $r.NewFileId = $dnnByFilename[$key]
                    $r.Status = 'matched-existing'
                }
            }
        }
    } else {
        Write-Host "  (dry run -- pass -Apply to register them and update employees)"
    }
}

# ------------- Apply UPDATEs --------------
$toUpdate = $results | Where-Object { $_.Status -eq 'matched-existing' -and $_.NewFileId -and $_.NewFileId -ne $_.OldFileId }
$noChange = $results | Where-Object { $_.NewFileId -eq $_.OldFileId -and $_.NewFileId }
$noMatch  = $results | Where-Object { $_.Status -eq 'no-match' }

Write-Host ""
Write-Host "===== Summary ====="
Write-Host "Matched existing FileId already correct: $($noChange.Count)"
Write-Host "Will update FileId:                       $($toUpdate.Count)"
Write-Host "No match found:                           $($noMatch.Count)"
Write-Host "Total employees:                          $($results.Count)"
Write-Host ""

if ($Apply -and $toUpdate.Count -gt 0) {
    Write-Host "Applying $($toUpdate.Count) UPDATEs..."
    foreach ($r in $toUpdate) {
        $sql = "UPDATE dbo.tjc_employee SET FileId = $($r.NewFileId), LastModifiedDate = SYSUTCDATETIME(), LastModifiedById = -1 WHERE EmployeeId = $($r.EmployeeId);"
        Invoke-Sqlcmd -ServerInstance $server -Database $database `
            -Username $user -Password $pw -Query $sql | Out-Null
    }
    Write-Host "Done."
}

# ------------- Write CSV report --------------
$results | Export-Csv -Path $reportPath -NoTypeInformation -Encoding UTF8
Write-Host "Wrote report: $reportPath"
