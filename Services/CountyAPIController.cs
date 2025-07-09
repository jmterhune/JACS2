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
using static tjc.Modules.jacs.Services.CourtAPIController;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class CountyAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCounties(int p1)
        {
            List<CountyViewModel> counties = new List<CountyViewModel>();
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
                var ctl = new CountyController();
                filteredCount = ctl.GetCountiesCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                counties = ctl.GetCountiesPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(county => new CountyViewModel(county)).ToList();
                return Request.CreateResponse(new CountySearchResult { data = counties, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CountySearchResult { data = counties, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCounties()
        {
            List<CountyViewModel> counties = new List<CountyViewModel>();

            try
            {
                var ctl = new CountyController();
                counties = ctl.GetCountys().Select(court => new CountyViewModel(court)).ToList();
                return Request.CreateResponse(new CountySearchResult { data = counties, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CountySearchResult { data = counties, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteCounty(long p1)
        {
            try
            {
                var ctl = new CountyController();
                ctl.DeleteCounty(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCounty(long p1)
        {
            try
            {
                var ctl = new CountyController();
                County county = ctl.GetCounty(p1);
                return Request.CreateResponse(new CountyResult { data = county, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CountyResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCounty(JObject p1)
        {
            try
            {
                var ctl = new CountyController();
                var county = p1.ToObject<County>();
                county.created_at = DateTime.Now;
                county.updated_at = DateTime.Now;
                ctl.CreateCounty(county);
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
        public HttpResponseMessage UpdateCounty(JObject p1)
        {
            try
            {
                var ctl = new CountyController();
                var county = p1.ToObject<County>();
                county.updated_at = DateTime.Now;
                ctl.UpdateCounty(county);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal class CountySearchResult
        {
            public List<CountyViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class CountyResult
        {
            public County data { get; set; }
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
                    fieldName = "code";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}