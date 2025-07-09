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
    public class RoleAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetRoles(int p1)
        {
            List<RoleViewModel> roles = new List<RoleViewModel>();
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
                var ctl = new RoleController();
                filteredCount = ctl.GetRolesCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                roles = ctl.GetRolesPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(role => new RoleViewModel(role)).ToList();
                return Request.CreateResponse(new RoleSearchResult { data = roles, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new RoleSearchResult { data = roles, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteRole(long p1)
        {
            try
            {
                var ctl = new RoleController();
                ctl.DeleteRole(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetRole(long p1)
        {
            try
            {
                var ctl = new RoleController();
                Role role = ctl.GetRole(p1);
                return Request.CreateResponse(new RoleResult { data = role, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new RoleResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateRole(JObject p1)
        {
            try
            {
                var ctl = new RoleController();
                var role = p1.ToObject<Role>();
                ctl.CreateRole(role);
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
        public HttpResponseMessage UpdateRole(JObject p1)
        {
            try
            {
                var ctl = new RoleController();
                var role = p1.ToObject<Role>();
                ctl.UpdateRole(role);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal class RoleSearchResult
        {
            public List<RoleViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class RoleResult
        {
            public Role data { get; set; }
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
                case 3:
                    fieldName = "guard_name";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}