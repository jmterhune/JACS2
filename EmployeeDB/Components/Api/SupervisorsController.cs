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
    /// REST endpoints for the Supervisors admin tab on EmployeeList and the
    /// supporting typeahead search used by that tab.
    ///
    /// Routes (resolved by ServiceRouteMapper):
    ///   GET    Supervisors/All
    ///   GET    Supervisors/SearchEmployees?q=...
    ///   POST   Supervisors            body: { EmployeeId, IsActive? }
    ///   PUT    Supervisors/{id}       body: { IsActive }
    ///   DELETE Supervisors/{id}       refuses 409 if currently assigned
    ///
    /// Write paths require the HR Admin role (or site admin / super user).
    /// Reads are gated to module View access — the dropdown on EditEmployee
    /// calls into the EmployeeController directly, but the admin tab on
    /// EmployeeList uses these endpoints.
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class SupervisorsController : DnnApiController
    {
        private readonly SupervisorController _supervisors = new SupervisorController();
        private readonly EmployeeController   _employees   = new EmployeeController();

        /// <summary>True for HR Admins (configurable via the HrAdminRole
        /// module setting, default "HR Admin") plus site admins and super
        /// users. Mirrors the gate on the existing HR-only admin tabs.</summary>
        private bool IsHrAdmin
        {
            get
            {
                if (UserInfo == null) return false;
                if (UserInfo.IsSuperUser) return true;

                var settings = ActiveModule == null ? null : ActiveModule.ModuleSettings;
                var roleName = "HR Admin";
                if (settings != null && settings.Contains("HrAdminRole"))
                {
                    var v = settings["HrAdminRole"] as string;
                    if (!string.IsNullOrWhiteSpace(v)) roleName = v.Trim();
                }
                if (UserInfo.IsInRole(roleName)) return true;

                var portalAdmin = PortalSettings?.AdministratorRoleName;
                return !string.IsNullOrEmpty(portalAdmin) && UserInfo.IsInRole(portalAdmin);
            }
        }

        private HttpResponseMessage Forbid(string message = "HR Admin access required.")
        {
            return Request.CreateErrorResponse(HttpStatusCode.Forbidden, message);
        }

        [HttpGet]
        [ActionName("All")]
        public HttpResponseMessage All()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _supervisors.GetAll());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Returns the employees currently assigned to a supervisor
        /// (the people whose <c>SupervisorId</c> equals the supervisor's
        /// <c>EmployeeId</c>). Powers the assignees modal opened from the
        /// users icon on each row of the Supervisors admin tab.</summary>
        [HttpGet]
        [ActionName("Assignees")]
        public HttpResponseMessage Assignees(int id)
        {
            try
            {
                var sup = _supervisors.GetById(id);
                if (sup == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                var rows = _supervisors.GetAssignees(sup.EmployeeId)
                    .Select(e => new {
                        EmployeeId  = e.EmployeeId,
                        FirstName   = e.FirstName,
                        LastName    = e.LastName,
                        DisplayName = (e.LastName ?? string.Empty).Trim() + ", " + (e.FirstName ?? string.Empty).Trim(),
                        JobTitle    = e.JobTitle,
                        IsActive    = e.IsActive ?? false
                    })
                    .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, rows);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Typeahead employee lookup for the "Add Supervisor" UI.
        /// Returns up to 20 employees whose first / last name (or "Last,
        /// First") matches the query, excluding anyone already on the
        /// supervisor roster so the user can't try to add a duplicate.</summary>
        [HttpGet]
        [ActionName("SearchEmployees")]
        public HttpResponseMessage SearchEmployees(string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return Request.CreateResponse(HttpStatusCode.OK, new object[0]);

                // EmployeeId is UNIQUE in tjc_supervisor, so this set captures
                // every employee that's already on the roster (active or not).
                var alreadySupervisors = new System.Collections.Generic.HashSet<int>(
                    _supervisors.GetAll().Select(r => r.EmployeeId));

                var matches = _employees.SearchByName(q, 20)
                    .Where(e => !alreadySupervisors.Contains(e.EmployeeId))
                    .Select(e => new {
                        EmployeeId  = e.EmployeeId,
                        FirstName   = e.FirstName,
                        LastName    = e.LastName,
                        DisplayName = (e.LastName ?? string.Empty).Trim() + ", " + (e.FirstName ?? string.Empty).Trim(),
                        IsActive    = e.IsActive ?? false,
                        JobTitle    = e.JobTitle
                    })
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, matches);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post(SupervisorInfo item)
        {
            if (!IsHrAdmin) return Forbid();
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (item.EmployeeId <= 0)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "EmployeeId is required");
            try
            {
                // Refuse a duplicate cleanly — the DB has a UNIQUE on
                // EmployeeId but we'd rather return 409 than expose a
                // SqlException through the 500 error path.
                if (_supervisors.GetByEmployeeId(item.EmployeeId) != null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict,
                        "This employee is already on the supervisor list.");
                }
                // New supervisor rows default to Active unless the client
                // explicitly passed IsActive = false.
                if (!item.IsActive) item.IsActive = true;
                _supervisors.Create(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, item);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Toggles IsActive on an existing supervisor row. The
        /// controller layer pins EmployeeId to the existing value so a
        /// stray payload can't repoint the row at a different employee.</summary>
        [HttpPut]
        public HttpResponseMessage Put(int id, SupervisorInfo item)
        {
            if (!IsHrAdmin) return Forbid();
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            try
            {
                var existing = _supervisors.GetById(id);
                if (existing == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                item.SupervisorId = id;
                _supervisors.Update(item, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, _supervisors.GetById(id));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            if (!IsHrAdmin) return Forbid();
            try
            {
                var existing = _supervisors.GetById(id);
                if (existing == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                // Refuse the delete cleanly when the supervisor is still
                // assigned to one or more employees. The admin should
                // either deactivate (PUT IsActive=false) so the row stays
                // for historical reference, or reassign the dependents
                // first.
                var dependents = _supervisors.CountAssignedEmployees(existing.EmployeeId);
                if (dependents > 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict,
                        "This supervisor is still assigned to " + dependents
                        + " employee record(s) and cannot be deleted. "
                        + "Reassign those employees first, or deactivate the supervisor instead.");
                }

                _supervisors.Delete(id);
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
