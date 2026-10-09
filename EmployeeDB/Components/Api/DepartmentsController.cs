using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection;
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
    /// REST endpoints for the Departments admin tab on EmployeeList.
    ///
    /// Departments live in the cross-module <c>tjc_gl_group</c> table, so
    /// write operations are gated to DNN site administrators (or super
    /// users) only. The <c>[DnnModuleAuthorize]</c> attribute enforces
    /// View-level access for anyone hitting the endpoint, but each handler
    /// also re-checks <see cref="UserInfo"/> against
    /// <see cref="DotNetNuke.Entities.Portals.PortalSettings.AdministratorRoleName"/>
    /// so a non-admin can't reach the write methods even if they manage to
    /// craft the request.
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class DepartmentsController : DnnApiController
    {
        private readonly IHostSettings _hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();
        private readonly GroupController _groups;

        public DepartmentsController()
        {
            _groups = new GroupController(_hostSettings);
        }

        private bool IsSiteAdmin
        {
            get
            {
                if (UserInfo == null) return false;
                if (UserInfo.IsSuperUser) return true;
                var role = PortalSettings?.AdministratorRoleName;
                return !string.IsNullOrEmpty(role) && UserInfo.IsInRole(role);
            }
        }

        private HttpResponseMessage Forbid(string message = "Site administrator access required.")
        {
            return Request.CreateErrorResponse(HttpStatusCode.Forbidden, message);
        }

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            // Listing is allowed for any authorized module viewer — the
            // dropdown that picks a Department is used everywhere on the
            // Edit page, not just by site admins.
            try { return Request.CreateResponse(HttpStatusCode.OK,
                _groups.GetAll().OrderBy(x => x.GroupName)); }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var item = _groups.GetById(id);
                if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPost]
        public HttpResponseMessage Post(GroupInfo item)
        {
            if (!IsSiteAdmin) return Forbid();
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (string.IsNullOrWhiteSpace(item.GroupName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Group Name is required");
            try
            {
                _groups.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, GroupInfo item)
        {
            if (!IsSiteAdmin) return Forbid();
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (string.IsNullOrWhiteSpace(item.GroupName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Group Name is required");
            try
            {
                item.GroupID = id;
                // IsSwnGroup lost its editor when the Send Word Now export was
                // retired, so the posted body never carries it. Carry the
                // stored value forward rather than letting the model default
                // silently clear the column on every department save.
                var stored = _groups.GetById(id);
                if (stored != null) item.IsSwnGroup = stored.IsSwnGroup;
                _groups.Update(item, UserInfo.UserID);
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
                var dependents = _groups.CountDependents(id);
                if (dependents > 0)
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict,
                        "This department is still referenced by " + dependents
                        + " employee or membership record(s) and cannot be deleted.");
                _groups.Delete(id);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
