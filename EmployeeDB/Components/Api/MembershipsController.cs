using DotNetNuke.Security;
using DotNetNuke.Web.Api;
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
    /// REST endpoints for the Groups tab on the Edit Employee page.
    /// The tab's UI is a dual-list (Selected / Available) — the GET payload
    /// pre-splits the groups so the JS doesn't have to re-derive Available
    /// every render.
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class MembershipsController : DnnApiController
    {
        private readonly GroupController _groups = new GroupController();
        private readonly GroupMembershipController _memberships = new GroupMembershipController();

        public class MembershipState
        {
            public List<GroupInfo> Selected { get; set; }
            public List<GroupInfo> Available { get; set; }
        }

        public class SaveBody
        {
            public int EmployeeId { get; set; }
            public List<int> GroupIds { get; set; }
        }

        [HttpGet]
        [ActionName("ForEmployee")]
        public HttpResponseMessage ForEmployee(int employeeId)
        {
            try
            {
                var all = _groups.GetAll().OrderBy(g => g.GroupName, StringComparer.OrdinalIgnoreCase).ToList();
                if (employeeId <= 0)
                {
                    // New employee — nothing assigned yet, so everything is available.
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new MembershipState { Selected = new List<GroupInfo>(), Available = all });
                }
                var selectedIds = new HashSet<int>(_memberships.GetForEmployee(employeeId).Select(m => m.GroupId));
                var selected = all.Where(g => selectedIds.Contains(g.GroupID)).ToList();
                var available = all.Where(g => !selectedIds.Contains(g.GroupID)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new MembershipState { Selected = selected, Available = available });
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }

        /// <summary>Replace the full group-membership set for an employee.
        /// The body's <c>GroupIds</c> list is the authoritative new set —
        /// everything not in the list is removed; new entries are added.</summary>
        [HttpPost]
        [ActionName("Save")]
        public HttpResponseMessage Save(SaveBody body)
        {
            if (body == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (body.EmployeeId <= 0) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "EmployeeId required");
            try
            {
                var desired = body.GroupIds == null
                    ? new HashSet<int>()
                    : new HashSet<int>(body.GroupIds.Where(id => id > 0));

                var existing = _memberships.GetForEmployee(body.EmployeeId).Select(m => m.GroupId).ToList();

                // Diff — remove anything no longer wanted, add anything new.
                foreach (var gid in existing.Where(id => !desired.Contains(id)))
                    _memberships.DeleteMembership(gid, body.EmployeeId);
                foreach (var gid in desired.Where(id => !existing.Contains(id)))
                    _memberships.AddMembership(gid, body.EmployeeId, UserInfo.UserID);

                return Request.CreateResponse(HttpStatusCode.OK, new { saved = desired.Count });
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); }
        }
    }
}
