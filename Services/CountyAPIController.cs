// CountyAPIController.cs
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
        public HttpResponseMessage GetCountyDropDownItems()
        {
            List<KeyValuePair<long, string>> counties = new List<KeyValuePair<long, string>>();

            try
            {
                var ctl = new CountyController();
                counties = ctl.GetCountyDropDownItems();
                return Request.CreateResponse(new ListItemOptionResult { data = counties, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ListItemOptionResult { data = counties, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteCounty(long p1)
        {
            try
            {
                var ctl = new CountyController();
                ctl.DeleteCounty(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "County deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCounty(long p1)
        {
            try
            {
                var ctl = new CountyController();
                County county = ctl.GetCounty(p1);
                if (county == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CountyResult { data = null, error = "County not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new CountyResult { data = county, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CountyResult { data = null, error = ex.Message });
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
                if (string.IsNullOrWhiteSpace(county.name) || string.IsNullOrWhiteSpace(county.code))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name and Code are required." });
                }
                county.created_at = DateTime.Now;
                county.updated_at = DateTime.Now;
                ctl.CreateCounty(county);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "County created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
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
                if (county.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "County ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(county.name) || string.IsNullOrWhiteSpace(county.code))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name and Code are required." });
                }
                var existingCounty = ctl.GetCounty(county.id);
                if (existingCounty == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "County not found." });
                }
                county.updated_at = DateTime.Now;
                ctl.UpdateCounty(county);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "County updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
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