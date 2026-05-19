using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.jacs.Components;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class RoleAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetRoles(int p1)
        {
            List<RoleViewModel> roles = new List<RoleViewModel>();
            try
            {
                var ctl = new RoleController();
                var allRoles = ctl.GetRoles().Select(t => new RoleViewModel(t)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new RoleSearchResult
                {
                    data = allRoles,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new RoleSearchResult
                {
                    data = roles,
                    error = $"Failed to retrieve Roles: {ex.Message}"
                });
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeleteRole(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid role ID." });
                }
                var ctl = new RoleController();
                var role = ctl.GetRole(p1);
                if (role == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Role not found." });
                }
                ctl.DeleteRole(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Role deleted successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to delete role: {ex.Message}" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetRole(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new RoleResult { data = null, error = "Invalid role ID." });
                }
                var ctl = new RoleController();
                var role = ctl.GetRole(p1);
                if (role == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new RoleResult { data = null, error = "Role not found." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new RoleResult { data = role, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new RoleResult { data = null, error = $"Failed to retrieve role: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateRole(JObject p1)
        {
            try
            {
                var role = p1.ToObject<Role>();
                if (string.IsNullOrWhiteSpace(role.name) || string.IsNullOrWhiteSpace(role.guard_name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Role name and guard name are required." });
                }
                var ctl = new RoleController();
                role.created_at = DateTime.Now;
                role.updated_at = DateTime.Now;
                ctl.CreateRole(role);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Role created successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to create role: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateRole(JObject p1)
        {
            try
            {
                var role = p1.ToObject<Role>();
                if (role.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Role ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(role.name) || string.IsNullOrWhiteSpace(role.guard_name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Role name and guard name are required." });
                }
                var ctl = new RoleController();
                var existingRole = ctl.GetRole(role.id);
                if (existingRole == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Role not found." });
                }
                role.updated_at = DateTime.Now;
                ctl.UpdateRole(role);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Role updated successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to update role: {ex.Message}" });
            }
        }
        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 3:
                    return "name";
                case 4:
                    return "guard_name";
                default:
                    return "name";
            }
        }
    }
}