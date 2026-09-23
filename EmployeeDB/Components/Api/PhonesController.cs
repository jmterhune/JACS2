using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

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
        // Mass-notification allowance, enforced both here and in
        // empdb-edit.js. Keep the two in sync if it ever changes.
        private const int MaxCallPhones = 5;
        private const int MaxTextPhones = 3; // 5 text/email slots minus 2 emails on Details
        private static readonly HashSet<string> SmsEligibleTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Mobile", "Work Cell" };

        private readonly IHostSettings _hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();
        private readonly PhoneController _phones;
        private readonly OfficeLocationController _locations;

        public PhonesController()
        {
            _phones = new PhoneController(_hostSettings);
            _locations = new OfficeLocationController(_hostSettings);
        }

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
        /// Defense-in-depth check for the mass-notification Call/Text
        /// allowance. The JS layer applies the same rules so the user gets
        /// immediate feedback, but the server is authoritative because anyone
        /// can poke the API directly.
        ///
        /// (The SwnCall / SwnText column names predate Crisis24 and are still
        /// the per-phone "use this for mass notification" markers — see
        /// Components\Helpers\Crisis24ExportBuilder.cs.)
        ///
        /// Returns an error message, or null if the row is OK to save.
        /// </summary>
        private string ValidateNotificationLimits(PhoneInfo item)
        {
            // SwnText only valid for Mobile / Work Cell.
            if (item.SwnText && !SmsEligibleTypes.Contains(item.PhoneType ?? string.Empty))
            {
                return "Text is only allowed for Mobile or Work Cell phones.";
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
            if (callCount > MaxCallPhones)
            {
                return "An employee can have at most " + MaxCallPhones + " phones with Call checked.";
            }
            if (textCount > MaxTextPhones)
            {
                return "An employee can have at most " + MaxTextPhones + " phones with Text checked (the two email addresses count toward the 5-text/email limit).";
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
            // PhoneId is 0 for an insert — ValidateNotificationLimits skips the "ignore
            // myself in the count" step in that case.
            var limitError = ValidateNotificationLimits(item);
            if (limitError != null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, limitError);
            try
            {
                _phones.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
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
                var limitError = ValidateNotificationLimits(item);
                if (limitError != null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, limitError);
                _phones.Update(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
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
                _phones.Delete(id);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
