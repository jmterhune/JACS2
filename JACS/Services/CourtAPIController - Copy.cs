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
    public class CourtAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourts(int p1)
        {
            List<CourtViewModel> courts = new List<CourtViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = query.ContainsKey("searchText") ? query["searchText"] : string.Empty;
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "description";
            string sortDirection = "asc";

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new CourtController();
                filteredCount = ctl.GetCourtsCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                courts = ctl.GetCourtsPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).ToList();
                return Request.CreateResponse(new CourtSearchResult { data = courts, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtSearchResult { data = courts, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourts()
        {
            List<Court> courts = new List<Court>();

            try
            {
                var ctl = new CourtController();
                courts = ctl.GetCourts().ToList();
                return Request.CreateResponse(new CourtResults { data = courts, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtResults { data = courts, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteCourt(long p1)
        {
            try
            {
                var ctl = new CourtController();
                ctl.DeleteCourt(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourt(long p1)
        {
            try
            {
                var ctl = new CourtController();
                Court court = ctl.GetCourt(p1);
                CourtViewModel courtViewModel = court != null ? new CourtViewModel(court) : null;
                if (courtViewModel == null)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound, new CourtResult { data = null, error = "Court not found" });
                }
                return Request.CreateResponse(new CourtResult { data = courtViewModel, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCourt(JObject p1)
        {
            try
            {
                var ctl = new CourtController();
                var court = p1.ToObject<CourtViewModel>();
                // Ensure required fields are set
                if (string.IsNullOrWhiteSpace(court.description) || court.county_id <= 0)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Description and County are required.");
                }
                ctl.CreateCourt(court);
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
        public HttpResponseMessage UpdateCourt(JObject p1)
        {
            try
            {
                var ctl = new CourtController();
                var court = p1.ToObject<CourtViewModel>();
                if (court.id <= 0)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Court ID is required for update.");
                }
                // Ensure required fields are set
                if (string.IsNullOrWhiteSpace(court.description) || court.county_id <= 0)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Description and County are required.");
                }
                var existingCourt = ctl.GetCourt(court.id);
                if (existingCourt == null)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound, "Court not found.");
                }
                ctl.UpdateCourt(court);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal class CourtSearchResult
        {
            public List<CourtViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }
        internal class CourtResults
        {
            public List<Court> data { get; set; }
            public string error { get; set; }
        }
        internal class CourtResult
        {
            public CourtViewModel data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return "description";
                case 3:
                    return "judge_name";
                case 4:
                    return "county_name";
                default:
                    return "description";
            }
        }
    }
}