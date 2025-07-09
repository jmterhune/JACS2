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
    public class CourtPermissionAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourtPermissions(int p1)
        {
            List<CourtPermissionViewModel> permissions = new List<CourtPermissionViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = !query.ContainsKey("searchText") ? "" : query["searchText"].ToString();
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "user_display_name"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new CourtPermissionController();
                filteredCount = ctl.GetCourtPermissionsCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                permissions = ctl.GetCourtPermissionsPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).ToList();
                return Request.CreateResponse(new CourtPermissionSearchResult { data = permissions, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtPermissionSearchResult { data = permissions, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteCourtPermission(long p1)
        {
            try
            {
                var ctl = new CourtPermissionController();
                ctl.DeleteCourtPermission(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtPermission(long p1)
        {
            try
            {
                var ctl = new CourtPermissionController();
                CourtPermission permission = ctl.GetCourtPermission(p1);
                return Request.CreateResponse(new CourtPermissionResult { data = permission, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtPermissionResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCourtPermission(JObject p1)
        {
            try
            {
                var ctl = new CourtPermissionController();
                var permission = p1.ToObject<CourtPermission>();
                permission.created_at = DateTime.Now;
                permission.updated_at = DateTime.Now;
                ctl.CreateCourtPermission(permission);
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
        public HttpResponseMessage UpdateCourtPermission(JObject p1)
        {
            try
            {
                var ctl = new CourtPermissionController();
                var permission = p1.ToObject<CourtPermission>();
                permission.updated_at = DateTime.Now;
                ctl.UpdateCourtPermission(permission);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetUsersForDropdown()
        {
            try
            {
                var ctl = new CourtPermissionController();
                var users = ctl.GetUsersForDropdown();
                return Request.CreateResponse(users);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetJudgesForDropdown()
        {
            try
            {
                var ctl = new CourtPermissionController();
                var judges = ctl.GetJudgesForDropdown();
                return Request.CreateResponse(judges);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal class CourtPermissionSearchResult
        {
            public List<CourtPermissionViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class CourtPermissionResult
        {
            public CourtPermission data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            string fieldName = "user_display_name";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "user_display_name";
                    break;
                case 3:
                    fieldName = "judge_name";
                    break;
                case 4:
                    fieldName = "active";
                    break;
                case 5:
                    fieldName = "permission";
                    break;
                default:
                    fieldName = "user_display_name";
                    break;
            }
            return fieldName;
        }
    }
}