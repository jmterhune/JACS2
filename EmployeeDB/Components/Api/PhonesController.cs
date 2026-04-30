using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;
using tjc.Modules.EmployeeDB.Components.SWN;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>
    /// REST endpoints for the Phones tab on the Edit Employee page.
    ///
    /// Routes (all rooted under /DesktopModules/EmployeeDB/API/):
    ///   GET    Phones/ForEmployee?employeeId=N    — list phones for an employee
    ///   GET    Phones/{id}                        — fetch one phone
    ///   POST   Phones                             — create
    ///   PUT    Phones/{id}                        — update
    ///   DELETE Phones/{id}                        — delete
    ///
    /// Authorization: same security level as the Edit page (View access on the
    /// module is enough — HR Admin checks happen in the page-level code-behind).
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class PhonesController : DnnApiController
    {
        // SWN limits enforced both here and in empdb-edit.js. Keep these two
        // in sync if the per-employee allowance ever changes.
        private const int MaxSwnCall = 5;
        private const int MaxSwnText = 3; // 5 text/email slots minus 2 emails on Details
        private static readonly HashSet<string> SmsEligibleTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Mobile", "Work Cell" };

        private readonly PhoneController _phones = new PhoneController();
        private readonly OfficeLocationController _locations = new OfficeLocationController();
        private readonly EmployeeController _employees = new EmployeeController();

        /// <summary>Builds a fresh OfficeLocationId -> Description lookup. Cheap
        /// enough to do per-request — only used by the Phones tab and there are
        /// only a handful of office locations.</summary>
        private Dictionary<int, string> BuildLocationLookup()
        {
            return _locations.GetAll().ToDictionary(l => l.OfficeLocationId, l => l.Description);
        }

        private static void StampLocationName(PhoneInfo p, Dictionary<int, string> lookup)
        {
            if (p == null) return;
            if (p.OfficeLocationId.HasValue && lookup.TryGetValue(p.OfficeLocationId.Value, out var name))
                p.LocationName = name;
            else
                p.LocationName = "";
        }

        /// <summary>
        /// Re-syncs the affected employee to SWN after a phone Create / Update /
        /// Delete. The local DB write is the source of truth — if the SWN call
        /// fails (network blip, bad credentials, SWN service down) we don't
        /// roll back the DB change; we surface the SWN error string so the
        /// caller can render it via the X-Swn-Warning response header.
        ///
        /// Returns null on success (or when SWN isn't configured for this
        /// module — then it's a no-op). Returns a single-line warning when
        /// the sync ran but reported errors / failures.
        /// </summary>
        private string TrySyncEmployee(int employeeId)
        {
            try
            {
                if (employeeId <= 0) return null;

                var creds = SwnSettings.Read(ActiveModule?.ModuleSettings);
                // Don't try to sync if SWN credentials aren't filled in. The
                // module may legitimately be running with SWN disabled, and
                // the user shouldn't see a warning every time they edit a
                // phone in that mode.
                if (!creds.IsConfigured) return null;

                var emp = _employees.GetEmployee(employeeId);
                if (emp == null) return "Employee #" + employeeId + " not found for SWN sync.";

                var swn = new SWNServiceRequests(creds.Username, creds.Password);
                var resp = swn.AddUpdateContact(emp);

                if (resp == null) return null;
                if (!resp.HasErrors) return null;

                // Roll the SWN error messages into a single delimited string —
                // the client surfaces this as a Noty warning, so we want to
                // keep it short and human-readable rather than a JSON dump.
                var sb = new StringBuilder();
                if (resp.MessageList != null)
                {
                    foreach (var m in resp.MessageList)
                    {
                        if (m == null || m.MessageType != SWNResponseMessageType.Failure) continue;
                        if (sb.Length > 0) sb.Append("; ");
                        sb.Append(m.MessageText ?? "(unknown error)");
                    }
                }
                if (sb.Length == 0) sb.Append("SWN reported errors but no message text.");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                // We never want a SWN failure to take the API call down with
                // it. Return the message so the client can show it and move on.
                return ex.Message;
            }
        }

        /// <summary>
        /// Decorate the response with an X-Swn-Warning header so the JS api
        /// wrapper can pick it up and surface a Noty toast. We use a header
        /// (rather than embedding the warning in the body) so existing
        /// callers that read the body-as-PhoneInfo don't need to change.
        /// </summary>
        private static void AttachSwnWarning(HttpResponseMessage resp, string warning)
        {
            if (resp == null || string.IsNullOrEmpty(warning)) return;
            // Strip CR/LF — header values must be a single line. The SWN
            // service occasionally embeds newlines in long error messages.
            var clean = warning.Replace("\r", " ").Replace("\n", " ");
            resp.Headers.TryAddWithoutValidation("X-Swn-Warning", clean);
        }

        /// <summary>
        /// Defense-in-depth check for the SWN Call/Text allowance. The JS layer
        /// applies the same rules so the user gets immediate feedback, but the
        /// server is authoritative because anyone can poke the API directly.
        /// Returns an error message, or null if the row is OK to save.
        /// </summary>
        private string ValidateSwnLimits(PhoneInfo item)
        {
            // SwnText only valid for Mobile / Work Cell.
            if (item.SwnText && !SmsEligibleTypes.Contains(item.PhoneType ?? string.Empty))
            {
                return "SWN Text is only allowed for Mobile or Work Cell phones.";
            }

            var existing = _phones.GetForEmployee(item.EmployeeId) ?? Enumerable.Empty<PhoneInfo>();

            // Skip the row currently being saved; its proposed flags are
            // counted as the seed for the running totals below.
            int callCount = item.SwnCall ? 1 : 0;
            int textCount = item.SwnText ? 1 : 0;
            foreach (var row in existing)
            {
                if (item.PhoneId > 0 && row.PhoneId == item.PhoneId) continue;
                if (row.SwnCall) callCount++;
                if (row.SwnText) textCount++;
            }
            if (callCount > MaxSwnCall)
            {
                return "An employee can have at most " + MaxSwnCall + " phones with SWN Call checked.";
            }
            if (textCount > MaxSwnText)
            {
                return "An employee can have at most " + MaxSwnText + " phones with SWN Text checked (the two email addresses count toward the SWN 5-text/email limit).";
            }
            return null;
        }

        [HttpGet]
        [ActionName("ForEmployee")]
        public HttpResponseMessage ForEmployee(int employeeId)
        {
            try
            {
                var rows = _phones.GetForEmployee(employeeId).ToList();
                var lookup = BuildLocationLookup();
                foreach (var p in rows) StampLocationName(p, lookup);
                return Request.CreateResponse(HttpStatusCode.OK, rows);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(long id)
        {
            try
            {
                var item = _phones.GetById(id);
                if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                StampLocationName(item, BuildLocationLookup());
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post(PhoneInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (item.EmployeeId <= 0) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "EmployeeId required");
            // PhoneId is 0 for an insert — ValidateSwnLimits skips the "ignore
            // myself in the count" step in that case.
            var swnError = ValidateSwnLimits(item);
            if (swnError != null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, swnError);
            try
            {
                _phones.Create(item, UserInfo.UserID);
                // Push the new phone to SWN. Failures show as a warning header
                // but don't roll back the DB insert — the row is saved and a
                // full Sync can recover the SWN side later.
                var warning = TrySyncEmployee(item.EmployeeId);
                var resp = Request.CreateResponse(HttpStatusCode.OK, item);
                AttachSwnWarning(resp, warning);
                return resp;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        public HttpResponseMessage Put(long id, PhoneInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            try
            {
                item.PhoneId = id;
                // EmployeeId comes from the body; if the caller forgot it, fall
                // back to the existing row so the validator can find siblings.
                if (item.EmployeeId <= 0)
                {
                    var existing = _phones.GetById(id);
                    if (existing == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                    item.EmployeeId = existing.EmployeeId;
                }
                var swnError = ValidateSwnLimits(item);
                if (swnError != null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, swnError);
                _phones.Update(item, UserInfo.UserID);
                var warning = TrySyncEmployee(item.EmployeeId);
                var resp = Request.CreateResponse(HttpStatusCode.OK, item);
                AttachSwnWarning(resp, warning);
                return resp;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(long id)
        {
            try
            {
                // Capture the EmployeeId BEFORE the row is deleted so we know
                // which contact to re-sync afterwards. The SWN AddUpdateContact
                // call rebuilds the contact's phone list from the (post-delete)
                // DB, so the deleted phone effectively disappears from SWN.
                var existing = _phones.GetById(id);
                int empId = existing == null ? 0 : existing.EmployeeId;

                _phones.Delete(id);

                var warning = TrySyncEmployee(empId);
                // 204 No Content has no body; we still want to surface the
                // warning header so the JS layer can show it.
                var resp = Request.CreateResponse(HttpStatusCode.NoContent);
                AttachSwnWarning(resp, warning);
                return resp;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
