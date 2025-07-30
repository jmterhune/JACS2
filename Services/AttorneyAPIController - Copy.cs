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
using tjc.Modules.jacs.Services.Mappers;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class AttorneyAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetAttorneys(int p1)
        {
            List<AttorneyViewModel> attorneys = new List<AttorneyViewModel>();
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
                var ctl = new AttorneyController();
                filteredCount = ctl.GetAttorneysCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                var attorneysPaged = ctl.GetAttorneysPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection);
                if (attorneysPaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new AttorneySearchResult
                    {
                        data = attorneys,
                        draw = draw,
                        recordsFiltered = filteredCount,
                        recordsTotal = recordCount,
                        error = "No attorneys found."
                    });
                }
                attorneys = attorneysPaged.Select(attorney => new AttorneyViewModel(attorney)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new AttorneySearchResult
                {
                    data = attorneys,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new AttorneySearchResult
                {
                    data = attorneys,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = $"Failed to retrieve attorneys: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAttorneyDropDownItems()
        {
            List<KeyValuePair<long, string>> attorneys = new List<KeyValuePair<long, string>>();
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string searchTerm = query.ContainsKey("q") ? query["q"].ToString() : "";

            try
            {
                var ctl = new AttorneyController();
                attorneys = ctl.GetAttorneyDropDownItems(searchTerm);
                return Request.CreateResponse(HttpStatusCode.OK, new ListItemOptionResult { data = attorneys, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ListItemOptionResult { data = attorneys, error = $"Failed to retrieve attorney dropdown items: {ex.Message}" });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteAttorney(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid attorney ID." });
                }
                var ctl = new AttorneyController();
                var attorney = ctl.GetAttorney(p1);
                if (attorney == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Attorney not found." });
                }
                ctl.DeleteAttorney(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Attorney deleted successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to delete attorney: {ex.Message}" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAttorney(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new AttorneyResult { data = null, error = "Invalid attorney ID." });
                }
                var ctl = new AttorneyController();
                var attorney = ctl.GetAttorney(p1);
                if (attorney == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new AttorneyResult { data = null, error = "Attorney not found." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new AttorneyResult { data = attorney, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new AttorneyResult { data = null, error = $"Failed to retrieve attorney: {ex.Message}" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSiteUser(string p1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(p1))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new SiteUserResult { data = null, error = "Bar number is required." });
                }
                var ctl = new AttorneyController();
                var user = ctl.GetSiteUser(0, p1);
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new SiteUserResult { data = null, error = "User not found for the provided bar number." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new SiteUserResult { data = SiteUserMapper.ToViewModel(user), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new SiteUserResult { data = null, error = $"Failed to retrieve site user: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateAttorney(JObject p1)
        {
            try
            {
                var attorney = p1.ToObject<Attorney>();
                if (string.IsNullOrWhiteSpace(attorney.name) || string.IsNullOrWhiteSpace(attorney.bar_num) || attorney.UserId < 0 || (attorney.emails != null && attorney.emails.Any(e => string.IsNullOrWhiteSpace(e))))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name, bar number, valid user ID, and at least one valid email are required." });
                }
                var ctl = new AttorneyController();
                attorney.created_at = DateTime.Now;
                attorney.updated_at = DateTime.Now;
                ctl.CreateAttorney(attorney);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Attorney created successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to create attorney: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateAttorney(JObject p1)
        {
            try
            {
                var attorney = p1.ToObject<Attorney>();
                if (attorney.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Attorney ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(attorney.name) || string.IsNullOrWhiteSpace(attorney.bar_num) || attorney.UserId < 0 || (attorney.emails != null && attorney.emails.Any(e => string.IsNullOrWhiteSpace(e))))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name, bar number, valid user ID, and at least one valid email are required." });
                }
                var ctl = new AttorneyController();
                var existingAttorney = ctl.GetAttorney(attorney.id);
                if (existingAttorney == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Attorney not found." });
                }
                attorney.updated_at = DateTime.Now;
                ctl.UpdateAttorney(attorney);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Attorney updated successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to update attorney: {ex.Message}" });
            }
        }

        internal class AttorneySearchResult
        {
            public List<AttorneyViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class MatchingNameResult
        {
            public IEnumerable<AttorneyViewModel> data { get; set; }
            public string error { get; set; }
        }

        internal class AttorneyResult
        {
            public Attorney data { get; set; }
            public string error { get; set; }
        }

        internal class SiteUserResult
        {
            public SiteUserViewModel data { get; set; }
            public string error { get; set; }
        }

        internal class ListItemOptionResult
        {
            public List<KeyValuePair<long, string>> data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return "enabled";
                case 3:
                    return "name";
                case 4:
                    return "bar_num";
                default:
                    return "name";
            }
        }
    }
}