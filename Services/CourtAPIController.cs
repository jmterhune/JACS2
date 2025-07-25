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
        public HttpResponseMessage GetCourtDropDownItems()
        {
            List<KeyValuePair<long, string>> courts = new List<KeyValuePair<long, string>>();

            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                string searchTerm = !query.ContainsKey("q") ? "" : query["q"].ToString();

                var ctl = new CourtController();
                courts = ctl.GetCourtDropDownItems(searchTerm);
                return Request.CreateResponse(new ListItemOptionResult { data = courts, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ListItemOptionResult { data = courts, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtsUnassigned()
        {
            try
            {
                var courtController = new CourtController();
                var judgeController = new JudgeController();
                var assignedCourtIds = judgeController.GetJudges()
                    .Where(j => j.court_id.HasValue)
                    .Select(j => j.court_id.Value)
                    .Distinct()
                    .ToList();

                var courts = courtController.GetCourts()
                    .Where(c => !assignedCourtIds.Contains(c.id))
                    .Select(c => new
                    {
                        id = c.id,
                        description = c.description
                    }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { data = courts, error = "" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteCourt(long p1)
        {
            try
            {
                var ctl = new CourtController();
                ctl.DeleteCourt(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourt(long p1)
        {
            try
            {
                var ctl = new CourtController();
                Court court = ctl.GetCourt(p1);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CourtResult { data = null, error = "Court not found" });
                }
                CourtViewModel courtViewModel = new CourtViewModel(court);
                return Request.CreateResponse(HttpStatusCode.OK, new CourtResult { data = courtViewModel, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CourtResult { data = null, error = ex.Message });
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
                if (string.IsNullOrWhiteSpace(court.description) || court.county_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Description and County are required." });
                }
                ctl.CreateCourt(court);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
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
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(court.description) || court.county_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Description and Countyare required." });
                }
                var existingCourt = ctl.GetCourt(court.id);
                if (existingCourt == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                ctl.UpdateCourt(court);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
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