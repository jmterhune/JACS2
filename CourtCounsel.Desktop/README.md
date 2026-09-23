# Court Counsel Desktop

A standalone WPF (.NET 9) desktop port of the `CourtCounsel` DNN module, with the
same screens and functionality, talking directly to the same SQL Server
database (`tjc_cc_*` tables in the `intranet` database) — no DNN, no web
server.

## Settings are admin-only

The **Settings** screen (connection string, Admin Group, template text) is
only visible and usable to an administrator — checked via
`AuthorizationService.CanEditSettings`, which is true for a member of the
local **Administrators** group or of **Domain Admins** (matched on the
universal `-512` SID suffix, so no domain name is hard-coded).

That check deliberately inspects the **linked elevated token**, not just
the process token. UAC strips Administrators/Domain Admins out of the token
for a non-elevated run, and .NET drops those deny-only SIDs from
`WindowsIdentity.Groups`, so every naive check reports `false` for an admin
who simply double-clicked the app. Reading the linked token gives the real
answer and means admins don't have to right-click → Run as administrator
just to open Settings. A standard user has no linked token, which is the
correct `false`.

Anyone who isn't an admin:

- doesn't see the Settings button in the nav bar at all,
- if somehow navigated there anyway, can't actually save (`SettingsViewModel`
  refuses the write),
- on first run with no connection string configured, sees a plain
  "Not Configured — contact your system administrator" screen instead of
  Settings.

Settings are stored **machine-wide**, not per-user, at:
```
%ProgramData%\CourtCounselDesktop\settings.json
```
(`C:\ProgramData\CourtCounselDesktop\settings.json`). The connection string
is encrypted in that file (AES-256-GCM, `Services/ConnectionStringProtector.cs`)
so it isn't sitting in plain text — but the encryption key is embedded in
the app itself, so this protects against casual viewing (Notepad, a stray
screenshot, `grep`ing AppData) rather than a targeted reverse-engineering
attack. The real security boundary is still the SQL login itself: keep it
scoped to only what this app needs.

**Deploying with settings already configured:** since this is one shared,
machine-wide file, provisioning a new machine is just: install the app,
then either (a) have an admin launch it once and fill in Settings on that
machine, or (b) have IT copy an already-produced `settings.json` into
`%ProgramData%\CourtCounselDesktop\` as part of the deployment/imaging
process — the same encrypted file works on any machine, since the
encryption key isn't tied to a user or machine (unlike Windows DPAPI,
which was deliberately not used for this reason). For extra enforcement
beyond the in-app gate, IT can tighten the NTFS ACLs on that `ProgramData`
folder so only administrators have write access — the app doesn't set this
itself. Note that if you do restrict the ACL, saving from Settings then
requires actually running elevated (membership alone gets you into the
screen; writing through a restricted ACL needs the elevated token) — the
Settings screen says so explicitly if Windows refuses the write.

If you configured a connection string with an earlier build of this app
(back when it was per-user, in `%AppData%\CourtCounselDesktop\settings.json`,
plain text), that file is still read automatically as a read-only fallback
if the new `ProgramData` file doesn't exist yet — so existing setups keep
working — but it's never written back to automatically; only an admin
saving from Settings produces the new file.

## First run (as an admin, before any settings exist)

1. Build and launch (F5 in Visual Studio, or `dotnet run` from this folder).
2. The app opens on **Settings** (no connection string yet). Enter a SQL
   Server connection string, e.g.:
   ```
   Server=YOURSQLSERVER;Database=intranet;Integrated Security=True;TrustServerCertificate=True
   ```
   Click **Test Connection**, then **Save**.
3. Optionally set an **Admin Group** — a Windows/AD group name. Members of
   that group get access to the Admin (lookup-management) screen — this is
   a separate, app-level concept from the administrators-only Settings gate
   above. Leave it blank to let everyone into Admin.
4. Restart, or just navigate to Search.

## Deploying to another PC

Build the self-contained single-file exe — the .NET runtime is bundled, so
target machines need **no prerequisites**:

```bash
dotnet publish -c Release -p:PublishProfile=FolderProfile
```

Output: `bin\Release\selfcontained\CourtCounsel.Desktop.exe` (~63 MB, one
file; the `.pdb` beside it is debug symbols and isn't needed).

Then copy **two** things to the target PC:

| From | To on target PC |
|---|---|
| `bin\Release\selfcontained\CourtCounsel.Desktop.exe` | anywhere, e.g. `C:\Program Files\CourtCounsel\` |
| `C:\ProgramData\CourtCounselDesktop\settings.json` | `C:\ProgramData\CourtCounselDesktop\settings.json` (same path) |

That's it — launch and it's already configured. The encrypted connection
string decrypts on any machine because the key is embedded in the exe
rather than tied to a user or machine.

**If `C:\ProgramData\CourtCounselDesktop\settings.json` doesn't exist yet**
on your machine, launch the app as an admin, open **Settings**, and click
**Save** once. That writes it (encrypting the connection string on the
way). Settings configured by an older build of this app lived in
`%AppData%\CourtCounselDesktop\settings.json` in plain text — still read
as a fallback, but not deployable, so do the Save step to produce the real
file.

To push it with SCCM/GPO, treat it as a two-item copy job (exe to Program
Files, `settings.json` to ProgramData). If you tighten the ProgramData ACL
so only admins can write it, note that saving from the Settings screen
then requires actually running elevated.

## Screens

Search, Data Entry, Reports, Data Sheet (with Excel export), Admin
(lookup-table CRUD: Case Types, Attorneys, Counties, Phases, Requestors,
Actions, Time Spent), Settings, and Update Case Name — the same set as the
web module's `Views/` folder.

## Known deviations from the web module

A handful of things were simplified rather than pixel/byte-for-byte ported,
since they were presentation details rather than functionality:

- The case-number widget doesn't mask keystrokes live (no `jquery.mask`
  equivalent); it validates and formats on Save instead. The final
  `County-Year-Type-Sequence[-Suffix]` format is identical.
- The Attorney Case List's "Completed" column is always shown, rather than
  conditionally hidden based on the status filter (`DataGridColumn` isn't
  part of the visual tree, so binding its visibility needs a proxy object
  that wasn't worth adding).
- Case-name search results don't highlight the matched substring.
- The Data Sheet export writes the same Excel-flavored HTML `.xls` file the
  web module produces (opens fine in Excel) rather than a native `.xlsx`.

## Architecture

MVVM, hand-rolled (no third-party MVVM framework): `ViewModels/` +
view-model-first navigation via `NavigationService` and DataTemplates in
`App.xaml`. `Data/` is plain ADO.NET via Dapper (`Microsoft.Data.SqlClient`),
mirroring the exact queries in the web module's `HistoryController` and
lookup controllers — see each repository's comments for the DNN
counterpart it replaces.
