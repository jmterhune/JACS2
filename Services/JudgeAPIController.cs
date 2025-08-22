using DotNetNuke.Entities.Portals;
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
    public class JudgeAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetJudges(int p1)
        {
            List<JudgeViewModel> judges = new List<JudgeViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = !query.ContainsKey("searchText") ? "" : query["searchText"].ToString();
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
                var ctl = new JudgeController();
                filteredCount = ctl.GetJudgesCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                judges = ctl.GetJudgesPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).ToList();
                return Request.CreateResponse(new JudgeSearchResult { data = judges, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new JudgeSearchResult { data = judges, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetJudgeDropDownItems(string p1, int p2)
        {
            List<KeyValuePair<long, string>> userOptions = new List<KeyValuePair<long, string>>();
            try
            {
                string judgeRole = p1;
                PortalSettings ps = PortalController.Instance.GetCurrentSettings() as PortalSettings;
                int portalId = ps.PortalId;
                var users = DotNetNuke.Security.Roles.RoleController.Instance.GetUsersByRole(portalId, judgeRole);
                var ctl = new JudgeController();
                var existingJudges = new List<long>();
                if (p2==1)
                {
                    var user = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                    if (user == null || user.UserID <= 0)
                    {
                        existingJudges = ctl.GetFilteredJudges(user.UserID).Select(j => j.id).ToList();
                    }
                }
                else
                {
                    existingJudges = ctl.GetJudges().Select(j => j.id).ToList();
                }
                var judgeUsers = users
                    .Where(u => !existingJudges.Contains(u.UserID))
                    .Select(u => new
                        KeyValuePair<long, string>(u.UserID, u.DisplayName)
                    ).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { data = judgeUsers, error = "" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetJudgeCourtDropDownItems(long p1)
        {
            List<KeyValuePair<long, string>> courts = new List<KeyValuePair<long, string>>();
            try
            {
                var user = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                if (user.IsAdmin)
                    p1 = 0;
                var ctl = new JudgeController();
                courts = ctl.GetJudgeCourtDropDownItems(p1);
                return Request.CreateResponse(new ListItemOptionResult { data = courts, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ListItemOptionResult { data = courts, error = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage DeleteJudge(long p1)
        {
            try
            {
                var ctl = new JudgeController();
                ctl.DeleteJudge(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Judge deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetJudge(long p1)
        {
            try
            {
                var ctl = new JudgeController();
                Judge judge = ctl.GetJudge(p1);
                if (judge == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new JudgeResult { data = null, error = "Judge not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new JudgeResult { data = new JudgeViewModel(judge), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new JudgeResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateJudge(JObject p1)
        {
            try
            {
                var ctl = new JudgeController();
                var judge = p1.ToObject<Judge>();
                if (judge.id <= 0 || string.IsNullOrWhiteSpace(judge.name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Judge ID and Name are required." });
                }
                judge.created_at = DateTime.Now;
                judge.updated_at = DateTime.Now;
                ctl.CreateJudge(judge);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Judge created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateJudge(JObject p1)
        {
            try
            {
                var ctl = new JudgeController();
                var judge = p1.ToObject<Judge>();
                if (judge.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Judge ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(judge.name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name is required." });
                }
                var existingJudge = ctl.GetJudge(judge.id);
                if (existingJudge == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Judge not found." });
                }
                judge.updated_at = DateTime.Now;
                ctl.UpdateJudge(judge);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Judge updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        internal class JudgeSearchResult
        {
            public List<JudgeViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class JudgeResult
        {
            public JudgeViewModel data { get; set; }
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
                    fieldName = "phone";
                    break;
                case 4:
                    fieldName = "court_name";
                    break;
                case 5:
                    fieldName = "title";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}