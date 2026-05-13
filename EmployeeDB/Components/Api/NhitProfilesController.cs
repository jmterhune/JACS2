using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>
    /// REST endpoints for the saved-profile dropdown on the New Hire IT
    /// Worksheet. A profile bundles every non-employee-unique field on the
    /// form so HR can re-use the same defaults across multiple new hires.
    ///
    /// Reads are open to module viewers (the dropdown is part of the form).
    /// Saves and deletes are gated to site administrators because a profile
    /// affects every subsequent worksheet that loads it.
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class NhitProfilesController : DnnApiController
    {
        private readonly NhitProfileController _profiles = new NhitProfileController();

        private bool IsSiteAdmin
        {
            get
            {
                if (UserInfo == null) return false;
                if (UserInfo.IsSuperUser) return true;
                var role = PortalSettings == null ? null : PortalSettings.AdministratorRoleName;
                return !string.IsNullOrEmpty(role) && UserInfo.IsInRole(role);
            }
        }

        private HttpResponseMessage Forbid(string message = "Site administrator access required.")
        {
            return Request.CreateErrorResponse(HttpStatusCode.Forbidden, message);
        }

        /// <summary>List for the profile dropdown — returns every profile
        /// header (name + id) without the heavy SelectedItemIds payload.
        /// Use Get(id) for the full profile when one is selected.</summary>
        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try
            {
                // Strip the IgnoreColumn list so the dropdown payload stays small.
                var rows = _profiles.GetAll()
                    .Select(p => new { p.NhitProfileId, p.ProfileName })
                    .OrderBy(p => p.ProfileName);
                return Request.CreateResponse(HttpStatusCode.OK, rows);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var item = _profiles.GetById(id);
                if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(NhitProfileInfo item)
        {
            if (!IsSiteAdmin) return Forbid();
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (string.IsNullOrWhiteSpace(item.ProfileName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Profile Name is required");
            try
            {
                _profiles.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, NhitProfileInfo item)
        {
            if (!IsSiteAdmin) return Forbid();
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (string.IsNullOrWhiteSpace(item.ProfileName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Profile Name is required");
            try
            {
                item.NhitProfileId = id;
                _profiles.Update(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            if (!IsSiteAdmin) return Forbid();
            try
            {
                _profiles.Delete(id);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
