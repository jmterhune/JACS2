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
                var ctl = new AttorneyController();
                filteredCount = ctl.GetAttorneysCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                attorneys = ctl.GetAttorneysPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(attorney => new AttorneyViewModel(attorney)).ToList();
                return Request.CreateResponse(new AttorneySearchResult { data = attorneys, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new AttorneySearchResult { data = attorneys, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetAttorneyDropDownItems()
        {
            List<AttorneyViewModel> attorneys = new List<AttorneyViewModel>();
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string searchTerm = !query.ContainsKey("q") ? "" : query["q"].ToString();

            try
            {
                var ctl = new AttorneyController();
                attorneys = ctl.GetAttorneyDropDownItems(searchTerm).Select(atty=>new AttorneyViewModel(atty)).ToList();
                return Request.CreateResponse(new AttorneySearchResult { data = attorneys,  error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new AttorneySearchResult { data = attorneys,  error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteAttorney(long p1)
        {
            try
            {
                var ctl = new AttorneyController();
                ctl.DeleteAttorney(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAttorney(long p1)
        {
            try
            {
                var ctl = new AttorneyController();
                Attorney attorney = ctl.GetAttorney(p1);
                return Request.CreateResponse(new AttorneyResult { data = attorney, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new AttorneyResult { data = null, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSiteUser(string p1)
        {
            try
            {
                var ctl = new AttorneyController();
                SiteUser user = ctl.GetSiteUser(0, p1);
                return Request.CreateResponse(new SiteUserResult { data = SiteUserMapper.ToViewModel(user), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new SiteUserResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateAttorney(JObject p1)
        {
            try
            {
                var ctl = new AttorneyController();
                var attorney = p1.ToObject<Attorney>();
                attorney.created_at = DateTime.Now;
                attorney.updated_at = DateTime.Now;
                ctl.CreateAttorney(attorney);
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
        public HttpResponseMessage UpdateAttorney(JObject p1)
        {
            try
            {
                var ctl = new AttorneyController();
                var attorney = p1.ToObject<Attorney>();
                attorney.updated_at = DateTime.Now;
                ctl.UpdateAttorney(attorney);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
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

        private string GetSortColumn(int columnIndex)
        {
            string fieldName = "name";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "enabled";
                    break;
                case 3:
                    fieldName = "name";
                    break;
                case 4:
                    fieldName = "bar_num";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}