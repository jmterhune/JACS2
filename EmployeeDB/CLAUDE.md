# EmployeeDB Module — Working Context

This is the C# / DAL2 rewrite of the legacy VB EmployeeDB DNN module. Use this file as the entry point when working on this module in a fresh Claude Code session.

> User memory already covers DNN deploy paths, Bootstrap 5 + Porto conventions, App_LocalResources placement, no-card-wrappers, no spacing utilities, and the dev-vs-test deploy rule. Don't restate those here — assume they apply.

## Identity

| | |
|---|---|
| Source root | `D:\OneDrive - jud12fl\SourceCode\tjc.modules.local\EmployeeDB` |
| Solution | `tjc.modules.local.sln` |
| Namespace | `tjc.Modules.EmployeeDB` |
| Assembly | `tjc.Modules.EmployeeDB.dll` |
| Install path | `DesktopModules/tjc.modules/EmployeeDB` |
| businessControllerClass | `tjc.Modules.EmployeeDB.Components.FeatureController, tjc.Modules.EmployeeDB` |
| Target framework | .NET 4.8 / DNN 9.11 |

## Tech stack — what we use here vs. what the legacy module used

- **DAL2 (PetaPoco)** — replaced LINQ-to-SQL DBML. Every controller follows the `DataContext.Instance() + GetRepository<T>()` pattern. Stored procs / functions from the legacy module were rewritten as inline SQL.
- **Crisis24 Person/HR feed** — replaced the Send Word Now (SWN) web service on 2026-09-09. `Components/Helpers/Crisis24ExportBuilder.cs` builds the CSV, `Components/Helpers/Crisis24Uploader.cs` pushes it over SFTP via WinSCPnet, and `Components/Api/Crisis24Controller.cs` exposes POST `Crisis24/Export` (**Crisis24 Export** button — builds + uploads, no download) and GET `Crisis24/Preview` (**Preview Crisis24 File** button — same builder, same filename, same `Crisis24Uploader.Encode`, downloaded instead of sent; works with no SFTP config, used to get the layout approved by Crisis24). Configured with web.config appSettings (`Crisis24FTP` plus optional overrides), NOT module settings. The retired SWN client, its generated WCF proxy and the `SwnList` view are parked in `_Retired-SWN/` and are out of the build.
- **DataTables.js** for grids (Telerik RadGrid is gone). Buttons extension powers the export on `DetailsList.ascx`.
- **Bootstrap modal-xl + iframe** for `EditEmployee` (replaces the legacy popup window).
- **`postMessage` channel** between the iframe and parent for save/delete/cancel handoff — code-behind reads `?modal=1` from querystring (`InModal` property) and emits the right exit script.

## Database

- **New DB:** `intranet.jud12.local` on `CAM-4HQ8144`
- **Old DB (migrated FROM):** `intranet_613` (legacy `Emp_*` tables) — same server, cross-DB references work
- **Tables consumed:** `tjc_employee`, `tjc_employee_class`, `tjc_employee_eeo`, `tjc_employee_emergency_contact`, `tjc_employee_group_membership`, `tjc_employee_job_group`, `tjc_employee_office_location`, `tjc_employee_phone`, `tjc_employee_position_history`, `tjc_employee_race`, `tjc_employee_service_history` (`tjc_employee_swn_interface_log` is no longer read — the SWN client that wrote it is retired)
- **Lookups (read-only):** `tjc_gl_group`, `tjc_gl_counties`
- **Views:** `tjc_employee_list`, `tjc_employee_phone_list`, `tjc_employee_eeo_list`
- **Module-owned NEW table:** `tjc_employee_assigned_item` — created by `Providers/DataProviders/SqlDataProvider/00.00.01.SqlDataProvider`. The other tables already exist; do NOT generate CREATE TABLE for them.
- **Audit columns on every writable table:** `CreatedDate`, `CreatedById`, `LastModifiedDate`, `LastModifiedById`. Every Insert/Update sets `*Date = DateTime.Now` and `*ById = UserId`.

### Schema renames vs. legacy (the most-bitten ones)

| Old `Emp_Employees` | New `tjc_employee` |
|---|---|
| `DivisionUnitId` | `DepartmentId` (FK → `tjc_gl_group`) |
| `JobCategoryId` | `JobGroupId` |
| `EmailWork` | `Email` |
| `EmailHome` | `PersonalEmail` |
| `Title` | `JobTitle` |
| `DateOfBirth` | `BirthDate` |
| `TerminatedDate` | `TerminationDate` |
| `Active` | `IsActive` |
| `Address1` + `Address2` | `Address` (concatenated `, `) |
| `Location` (string) | `OfficeLocationId` (FK) + `LocationName` (denormalized copy) |
| `County` (string) | `CountyId` (FK) |
| `PhotoUrl` | `PhotoFileId` (legacy paths NOT migrated; new records use DNN FileId) |
| `PhoneHome`/`PhoneCell`/`Phone`/`Pager`/`Extension` | normalized into `tjc_employee_phone` rows |

Phone migration mapping: `PhoneHome → 'Home'`, `PhoneCell → 'Mobile'`, `Phone → 'Work'` (with Extension), `Pager → 'Pager'`. Only inserted if non-empty.

### Out of scope / dropped

- `PhotoUrl` migration (deferred — old path-based URLs don't map to DNN FileIDs; migrated rows have NULL photo)
- `BBPin`, `BBPinLabel`
- `Emp_Divisions` (obsolete; `DepartmentId` now FKs `tjc_gl_group`)
- `emp_people_first` (legacy staging)
- `back/EditEmployee.ascx`, `WebUserControl.ascx` (backup/placeholder files)

## Project layout

```
EmployeeDB/
├─ EmployeeDB.csproj             (auto-deploys to dev on build)
├─ EmployeeDB.dnn                (manifest — 10 moduleControls)
├─ EmployeeDBModuleBase.cs       (PortalModuleBase + ReportUrl, EmployeeId from QS, role helpers, _navigationManager)
├─ Components/
│  ├─ Models/        (16 PetaPoco entity classes — [TableName], [PrimaryKey], [Cacheable] on lookups)
│  ├─ Controllers/   (17 controllers — Employee, JobClass, JobGroup, Race, OfficeLocation, Phone,
│  │                  EmergencyContact, GroupMembership, PositionHistory, ServiceHistory, Eeo,
│  │                  AssignedItem, Group (RO), County (RO), EmployeeReport)
│  ├─ FeatureController.cs       (IPortable, ISearchable, IUpgradeable)
│  └─ Helpers/Crisis24*.cs       (Person/HR feed CSV builder + SFTP uploader)
├─ _Retired-SWN/                 (the removed Send Word Now client, WCF proxy and SwnList view — kept for reference, excluded from the build and from DeployToSite)
├─ Views/                        (10 .ascx — see below)
├─ App_LocalResources/           (one .resx per view)
├─ Migrations/migrate-employeedb.sql  (intranet_613 → intranet.jud12.local — already run)
└─ Providers/DataProviders/SqlDataProvider/  (only creates tjc_employee_assigned_item)
```

### Views (10)

| View | Role |
|---|---|
| `EmployeeList.ascx` | Main admin landing — tabs: Employees / JobGroups / Classes / Races / OfficeLocations / Details + Crisis24 Export button |
| `EditEmployee.ascx` | Tabbed edit form (Bootstrap nav-tabs): Details / Groups / Employment History / Photo / Phones / Emergency Contacts / Access. Renders inside an XL modal+iframe when launched from `EmployeeList` (`?modal=1`) |
| `Directory.ascx` | Searchable list with tooltip popup |
| `DetailsList.ascx` | 28-column list with DataTables Buttons export |
| `DetailPopUp.ascx` | Tooltip partial |
| `EEOSetup.ascx` | Date range + preview grid → Publish |
| `Settings.ascx` | Stores `Employee_ReportUrl` |
| `Birthdays.ascx` | HTML report with DataTables (replaces Crystal Reports) |
| `TerminatedEmployees.ascx` | Date range report |
| `SelectUserId.ascx` | DNN user picker popup |

## Conventions you MUST follow in this module

1. **URLs for modal launches:** never use `EditUrl(...)` — it wraps in `javascript:dnnModal.show('https://...')` which breaks JS strings. Use `_navigationManager.NavigateURL(TabId, "Edit", ...)` and emit JSON-encoded via `HttpUtility.JavaScriptStringEncode`. See `EmployeeList.ascx.cs::GetEditEmployeeUrlJson()`.
2. **Repeater Edit buttons → plain `<a onclick="...">`, not `LinkButton`.** LinkButtons inside Repeaters trigger full postbacks unless every item registers as an async trigger via `ItemDataBound` — much simpler to do client-side modal population.
3. **Settings:** `LoadSettings` reads from the merged `Settings` property, not `TabModuleSettings` (different hash → setting won't appear). `UpdateSettings` writes via `ModuleController.UpdateModuleSetting`.
4. **Form fields look extra-tall?** DNN's `base-1.min.css` ships `.form-control { height: calc(2.25rem + 2px) }`. EditEmployee scopes a `<style>` override `#EmployeeEditForm .form-control { height: auto; min-height: 38px; }` — preserve that block when editing the view.
5. **Bootstrap 5 Tab init in iframes:** the inline JS in `EditEmployee` explicitly news-up `bootstrap.Tab` instances after DOM ready — Porto's auto-init doesn't reach inside the iframe.
6. **DNN doesn't re-read `.dnn` at runtime.** If you change `businessControllerClass`, manifest edits ALONE won't take effect — also UPDATE `DesktopModules.BusinessControllerClass` in the host DB. (We hit this on CourtCounsel; same trap applies here.)
7. **All audit fields are required (NOT NULL).** Every `Insert`/`Update` controller method MUST stamp `CreatedDate / CreatedById / LastModifiedDate / LastModifiedById`. The base controller exposes `UserId` for this.

## Build / deploy commands

Auto-deploy happens on every build. To pin to test instead:

```powershell
$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
  -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1

# Dev (default — fires on any build):
& $msbuild "EmployeeDB.csproj" /p:Configuration=Debug /v:minimal /nologo

# Test (explicit, only on user request):
& $msbuild "EmployeeDB.csproj" /p:Configuration=Debug `
    /p:DeploySitePath="M:\Websites\Intranet.jud12.local" `
    /p:DevSiteUrl="https://apps.test.jud12.local/" `
    /v:minimal /nologo /t:Rebuild
# Then touch web.config to recycle the test pool:
(Get-Item "M:\Websites\Intranet.jud12.local\web.config").LastWriteTime = Get-Date
```

## Known good state (as of last session)

- **Module installs cleanly** in dev under DNN 9.11. The "Value cannot be null. Parameter name: type" install error was caused by a manifest assembly-name mismatch and is fixed (assembly is now `tjc.Modules.EmployeeDB`, not `EmployeeDB`).
- **Data migration done:** 548 employees + 82 follow-up adds, 978 phones, 358 emergency contacts, 367 group memberships imported from `intranet_613`.
- **Build is clean** — `System.IdentityModel` reference + `using System.Web.Caching;` already added for `[Cacheable]` models.
- **EmployeeList iframe modal:** working (Edit buttons populate via client-side JS, Save closes modal via `postMessage`).
- **Settings:** load and save correctly (read from merged `Settings`, write via `UpdateModuleSetting`).
- **Crisis24 export** needs the `Crisis24FTP` appSetting in the site web.config. As of 2026-09-09 dev holds a PLACEHOLDER value (`sftp://USERNAME@HOST.crisis24.com/inbound/`) because Crisis24 has not supplied the host/account yet; `Crisis24Uploader.IsConfigured` treats placeholder markers (USERNAME, HOST.CRISIS24.COM, EXAMPLE.COM, CHANGEME, TODO) as unconfigured, so the button reports a clean "not set up yet" message instead of hanging on a connect timeout. WinSCPnet.dll + WinSCP.exe must be present in the site bin.

## Reference patterns in sibling modules

| Pattern | Where to look |
|---|---|
| DAL2 controller / repository pattern | `JudicialReferral/Components/Controllers/JudgeReferralController.cs`, `CourtCounsel/Components/Controllers/HistoryController.cs` |
| Tabbed admin landing | `CourtCounsel/Views/Admin.ascx` |
| Responsive edit-form layout | `CourtCounsel/Views/EditHistory.ascx` (`col-12 col-lg-*` rows) |
| DataTables list with paging/sorting | `JudicialReferral/Views/View.ascx` |
| Inline modal + confirm | `JudicialReferral/Views/Review.ascx` |
| Collapsible search card | `JudicialReferral/Views/View.ascx` |
| Server-side Excel export (HTML-as-XLS) | `CourtCounsel/Views/DataSheet.ascx.cs::cmdExport_Click` |
| Auto-deploy MSBuild target | `JudicialReferral/JudicialReferral.csproj` `DeployToSite` target |

## When you start the next conversation

Useful first steps for the new session:

1. `Read EmployeeDB/CLAUDE.md` (this file) for context.
2. Skim `EmployeeDB/EmployeeDB.dnn` to confirm module controls.
3. If touching the DB schema, remember the `tjc_employee_assigned_item` install script lives at `Providers/DataProviders/SqlDataProvider/00.00.01.SqlDataProvider`.
4. If touching the Crisis24 feed, read the column rules in the header comment of `Components/Helpers/Crisis24ExportBuilder.cs` first — they come straight from the Crisis24 Person Implementation Guide (rev 2024-04-23), and a record missing a required field fails ingestion silently on their side.
5. If touching `EmployeeList` Edit buttons, do NOT convert them back to `LinkButton` — see convention #2 above.
