# tjc.modules.local

DNN modules for the Twelfth Judicial Circuit intranet. These rules apply to every module in this directory.

## Environments

| Environment | URL | Site folder |
|---|---|---|
| Dev | https://www.dnndev.me/ | `D:\websites\apps.jud12.flcourts.org` |
| Test | https://apps.test.jud12.local/ | `M:\Websites\Intranet.jud12.local` |
| **Live, don't touch** | https://apps.jud12.flcourts.org/ | `K:\Intranet` |

- **Never make changes to, or browse, the live site without a specific instruction to do so.** That includes viewing pages, running read-only queries, and copying files. The dev site folder is named `apps.jud12.flcourts.org` and the dev DNN `PortalAlias` table lists the live hostname, so never build a URL from the folder name or the database. For browser testing use only the dev and test URLs above.
- The dev site may have been stopped and can take a long time on its first load. Wait for it.
- Module test pages for JACS Maintenance (ManateeJacsCaseMaintenace), all under https://apps.test.jud12.local/Utilities/:
  - Case maintenance view (View.ascx): `Manatee-JACS-Utility`
  - Excluded Attorneys and Attorney Status tabs (ExcludedAttorneyList.ascx): `Excluded-Attorneys`
  - Dev uses the same paths under https://www.dnndev.me/.
- Pages require a DNN sign-in. Anonymous requests redirect to `/Landing-Page`.
- Deploying a module: copy the built DLL to the site's `bin` folder, and the `.ascx` and other resource files to `DesktopModules\tjc.modules\<DeployFolder>`. The folder mapping from source project to deploy folder is in `deploy-standardization.ps1`. Back up the existing files first, and copy the DLL before the `.ascx` files.
