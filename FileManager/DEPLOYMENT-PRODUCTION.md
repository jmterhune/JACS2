# FileManager (intranet) – production deployment plan

Two parts go live together. The public part also closes a security hole, so it is the more urgent of the two.

| Part | Site | Folder | URL |
|---|---|---|---|
| Public (API + Download.ashx fix) | jud12.flcourts.org | `G:\Websites\jud12.flcourts` | https://www.jud12.flcourts.org/ |
| Intranet module | apps.jud12.flcourts.org | `K:\Intranet` | https://apps.jud12.flcourts.org/ |

Source: `tjc.Modules` commit `17ff3ab` (public) and `tjc.modules.local` commit `4851b48` (intranet) or later.

## What went wrong in dev/test, and what prevents it

| Problem seen | Prevention |
|---|---|
| Old `tjc.Modules.FileManager.dll` reappeared on the public test site, so the API route vanished (404 even for bad signatures) | Build once from the committed source, record the MD5, verify it after deploying **and again the next day**. Nothing else may deploy `tjc.Modules.FileManager.dll` to the public site (see "Freeze"). |
| `Could not establish trust relationship` | Check that the intranet server trusts the public site's certificate **before** deploying (step 0). Never set `AllowUntrustedCertificate` on live. |
| 404 from a wrong or misspelled share key | One key, chosen up front, written down in the table below; the intranet module setting must be exactly that key. |
| Secrets pasted into the wrong field / shared in chat | Generate a new live secret. Enter it only through ConfigEncryption on the two servers. Don't paste it into chat, tickets or module settings. |
| Encrypted `appSettings` can't be edited by hand | Use ConfigEncryption (decrypt, edit, re-encrypt) on each live `web.config`, and keep the backup. |

## Decisions to make before the change window

| Item | Value |
|---|---|
| Share key(s) | `____________` (letters, numbers, - and _ only) |
| Share folder on the public server | `____________` (outside the web root) |
| Intranet server's outbound IP as the public site sees it | `____________` |
| Who may view the intranet page (DNN roles) | `____________` |
| Change window / who is on call | `____________` |

## Freeze

From the moment the build is made until the check on the following day:
- Don't deploy anything from `tjc.Modules` to `G:\Websites\jud12.flcourts` that includes `tjc.Modules.FileManager.dll`, whether from Visual Studio, `deploy-standardization.ps1` or a restore.
- Tell anyone who deploys to that site what the correct MD5 is.

## 0. Pre-checks (no changes)

1. Both live `web.config` files and the public `bin` are backed up (step 2).
2. From the **intranet** server, in PowerShell:
   ```powershell
   Invoke-WebRequest https://www.jud12.flcourts.org/ -UseBasicParsing | Select StatusCode
   ```
   It must return 200 with no certificate error. If it fails, fix the CA/certificate trust first and stop here.
3. Confirm the public site's certificate covers the host used in `BaseUrl`, and that the intranet server resolves that name.
4. Confirm the share folder exists and the public site's app pool identity can read it (`icacls`).
5. List the rows in the public module's `tjc_rec_directory_access` table. After the fix, a user must be named on the row, or be in its role, to download. Make sure no one relies on the old open download.

## 1. Build

1. Check out the commits above on a clean tree.
2. Build both projects in Release **from their solutions** (`tjc.Modules\jud12 Modules.sln`, `tjc.modules.local\tjc.modules.local.sln`); packaging needs the solution's `packages` folder. Both projects reference DNN from the dev sites' `bin` folders, so the dev sites must be on this machine.
   - Public DLL: `tjc.Modules\FileManager\bin\tjc.Modules.FileManager.dll`, about 25 KB. The old pre-API build is about 15 KB (MD5 `3220FFFE…`); if that's what you have, the build didn't run.
3. Record `md5sum` of each DLL here:
   - Public `tjc.Modules.FileManager.dll`: `____________`
   - Intranet `tjc.Modules.FileManager.dll`: `____________`
4. Keep the exact DLL files; deploy those, not a rebuild.

## 2. Back up

On each live server, before changing anything:
- Public: copy `bin\tjc.Modules.FileManager.dll` to `bin\tjc.Modules.FileManager.dll.bak-<date>`, and `web.config` to `web.config.bak-<date>`.
- Intranet: copy `web.config` to `web.config.bak-<date>`, and `bin\tjc.Modules.FileManager.dll` if one exists.

## 3. Generate the live secret

Make a new value, 32+ characters, different from dev and test:

```
head -c 36 /dev/urandom | base64
```

Store it in the password manager. It goes in two places only.

## 4. Public site first

Deploy order: DLL, then config.

1. Copy the built DLL to `G:\Websites\jud12.flcourts\bin\`.
2. Using ConfigEncryption, add to `appSettings`, then re-encrypt:
   ```xml
   <add key="FileManagerRemote.Secret" value="<live secret>" />
   <add key="FileManagerRemote.Shares" value="<key>=<folder>" />
   ```
3. IIS IP restriction: allow only the intranet server's outbound address to `/API/FileManager/RemoteFiles`; deny everything else.
4. Verify the DLL hash matches step 1.
5. Verify the API (from any machine):
   - A request with a bad signature must return **401** (not 404; 404 means the route isn't registered).
   - A request with no headers must return **401**.
6. Verify the existing public FileManager module: a user with access downloads a file; a user without access does not.

## 5. Intranet site

1. Install `FileManager_00.00.01_Install.zip` (Host > Extensions), or copy the DLL then the `.ascx`/`.ashx`/`.css` files to `DesktopModules\tjc.modules\FileManager` and register through the install.
2. Using ConfigEncryption, add to `appSettings`, then re-encrypt:
   ```xml
   <add key="FileManagerRemote.Secret" value="<same live secret>" />
   <add key="FileManagerRemote.BaseUrl" value="https://www.jud12.flcourts.org/" />
   ```
   Do **not** add `FileManagerRemote.AllowUntrustedCertificate`.
3. Add the module to its page, set the share key to exactly the key from the table above (the name only, not the folder), and set the page/module view roles.

## 6. Verify

| Check | Expected |
|---|---|
| Page loads for an allowed user | File tree appears |
| Open a file | Downloads/opens, name correct (including names with spaces and `+`) |
| User outside the role | Page not visible; direct download URL returns 403 |
| Not signed in | Redirected to sign-in |
| Intranet log for `AllowUntrustedCertificate` warning | Absent |
| Public DNN log, `FileManager remote list` errors | Absent (they appear only when a share key or folder is wrong, and say which) |
| Public IIS log (`W3SVC<id>\u_ex*_x.log`), `RemoteFiles` lines | 200 for the module's calls |
| DLL hashes | Match step 1 |

## 7. Follow-up checks

- **Next business day:** confirm the public `tjc.Modules.FileManager.dll` hash again, and that the module still loads.
- **After any deploy to the public site:** repeat the 401 check in step 4.5.
- **Rotation:** change the secret at least yearly and whenever someone who knew it leaves. Change both servers together; the module shows an error until both match.

## Rollback

- **Intranet:** remove the module from its page (or disable its view roles) and restore the `web.config` backup. The public site is unaffected.
- **Public:** restore the backed-up DLL and `web.config`. Note that this puts the old `Download.ashx` behaviour back (no access check), so only do it if the site is down, and fix forward quickly.
