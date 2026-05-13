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
    /// REST endpoints for managing the New Hire IT Worksheet's checkbox
    /// catalog (Software / Intranet / Judicial categories).
    ///
    /// Reads are open to any module viewer (everyone uses the form);
    /// writes are restricted to site administrators so a regular HR clerk
    /// can't accidentally edit the catalog while filling out a worksheet.
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class NhitItemsController : DnnApiController
    {
        private readonly NhitItemController _items = new NhitItemController();

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

        [HttpGet]
        [ActionName("Active")]
        public HttpResponseMessage Active()
        {
            try { return Request.CreateResponse(HttpStatusCode.OK, _items.GetActive()); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try { return Request.CreateResponse(HttpStatusCode.OK, _items.GetAll()); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var item = _items.GetById(id);
                if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(NhitItemInfo item)
        {
            if (!IsSiteAdmin) return Forbid();
            var err = Validate(item);
            if (err != null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, err);
            try
            {
                // Default IsActive = true on insert if the client didn't say
                // — most callers will leave it alone and we want new items
                // visible immediately.
                item.IsActive = true;
                _items.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, NhitItemInfo item)
        {
            if (!IsSiteAdmin) return Forbid();
            var err = Validate(item);
            if (err != null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, err);
            try
            {
                item.NhitItemId = id;
                _items.Update(item, UserInfo.UserID);
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
                _items.Delete(id);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        private static readonly string[] AllowedCategories = new[] { "Software", "Intranet", "Judicial" };

        private static string Validate(NhitItemInfo item)
        {
            if (item == null) return "Body required";
            if (string.IsNullOrWhiteSpace(item.Name)) return "Name is required";
            if (string.IsNullOrWhiteSpace(item.Category)) return "Category is required";
            // Case-insensitive match — but normalize to canonical casing
            // so the category column is consistent for filtering.
            var match = AllowedCategories.FirstOrDefault(c =>
                string.Equals(c, item.Category, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                return "Category must be one of: " + string.Join(", ", AllowedCategories);
            }
            item.Category = match;
            return null;
        }
    }
}
