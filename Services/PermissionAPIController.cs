using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
            int sortIndex = 2; // Default sort index
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            string sortColumn = GetSortColumn(sortIndex);
            string sortDirection = "asc"; // Default sort direction

            // Check if order parameters exist
            if (query.ContainsKey("order[0].column"))
            {
                Int32.TryParse(query["order[0].column"], out sortIndex);
                sortColumn = GetSortColumn(sortIndex);
            }
            if (query.ContainsKey("order[0].dir"))
            {
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new PermissionController();
                filteredCount = ctl.GetPermissionsCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                permissions = ctl.GetPermissionsPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(permission => new PermissionViewModel(permission)).ToList();
                return Request.CreateResponse(new PermissionSearchResult { data = permissions, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new PermissionSearchResult { data = permissions, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeletePermission(long p1)
        {
            try
            {
                var ctl = new PermissionController();
                ctl.DeletePermission(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPermission(long p1)
        {
            try
            {
                var ctl = new PermissionController();
                Permission permission = ctl.GetPermission(p1);
                return Request.CreateResponse(new PermissionResult { data = permission, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new PermissionResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreatePermission(JObject p1)
        {
            try
            {
                var ctl = new PermissionController();
                var permission = p1.ToObject<Permission>();
                ctl.CreatePermission(permission);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdatePermission(JObject p1)
        {
            try
            {
                var ctl = new PermissionController();
                var permission = p1.ToObject<Permission>();
                ctl.UpdatePermission(permission);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
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
            string fieldName = "name";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "name";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}