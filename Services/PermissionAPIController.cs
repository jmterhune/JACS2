// PermissionAPIController.cs
using DotNetNuke.Entities.Users;
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
    public class PermissionAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetPermissions(int p1)
        {
            List<PermissionViewModel> permissions = new List<PermissionViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = query.ContainsKey("searchText") ? query["searchText"].ToString() : "";
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "name"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new PermissionController();
                filteredCount = ctl.GetPermissionsCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                var permissionsPaged = ctl.GetPermissionsPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection);
                if (permissionsPaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new PermissionSearchResult
                    {
                        data = permissions,
                        draw = draw,
                        recordsFiltered = filteredCount,
                        recordsTotal = recordCount,
                        error = "No permissions found."
                    });
                }
                permissions = permissionsPaged.Select(permission => new PermissionViewModel(permission)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new PermissionSearchResult
                {
                    data = permissions,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new PermissionSearchResult
                {
                    data = permissions,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = $"Failed to retrieve permissions: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeletePermission(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid permission ID." });
                }
                var ctl = new PermissionController();
                var permission = ctl.GetPermission(p1);
                if (permission == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Permission not found." });
                }
                ctl.DeletePermission(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Permission deleted successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to delete permission: {ex.Message}" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPermission(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new PermissionResult { data = null, error = "Invalid permission ID." });
                }
                var ctl = new PermissionController();
                var permission = ctl.GetPermission(p1);
                if (permission == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new PermissionResult { data = null, error = "Permission not found." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new PermissionResult { data = permission, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new PermissionResult { data = null, error = $"Failed to retrieve permission: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreatePermission(JObject p1)
        {
            try
            {
                var permission = p1.ToObject<Permission>();
                if (string.IsNullOrWhiteSpace(permission.name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Permission name is required." });
                }
                var ctl = new PermissionController();
                permission.created_at = DateTime.Now;
                permission.updated_at = DateTime.Now;
                ctl.CreatePermission(permission);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Permission created successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to create permission: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdatePermission(JObject p1)
        {
            try
            {
                var permission = p1.ToObject<Permission>();
                if (permission.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Permission ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(permission.name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Permission name is required." });
                }
                var ctl = new PermissionController();
                var existingPermission = ctl.GetPermission(permission.id);
                if (existingPermission == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Permission not found." });
                }
                permission.updated_at = DateTime.Now;
                ctl.UpdatePermission(permission);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Permission updated successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to update permission: {ex.Message}" });
            }
        }

        internal class PermissionSearchResult
        {
            public List<PermissionViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class PermissionResult
        {
            public Permission data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return "name";
                case 3:
                    return "guard_name";
                default:
                    return "name";
            }
        }
    }
}