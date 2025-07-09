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
    internal class CourtMotionAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourtMotions(int p1)
        {
            List<CourtMotionViewModel> courtMotions = new List<CourtMotionViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = !query.ContainsKey("searchText") ? "" : query["searchText"].ToString();
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "court_id"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new CourtMotionController();
                filteredCount = ctl.GetCourtMotionsCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                courtMotions = ctl.GetCourtMotionsPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(cm => new CourtMotionViewModel(cm)).ToList();
                return Request.CreateResponse(new CourtMotionSearchResult { data = courtMotions, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtMotionSearchResult { data = courtMotions, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage DeleteCourtMotion(long p1)
        {
            try
            {
                var ctl = new CourtMotionController();
                ctl.DeleteCourtMotion(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetCourtMotion(long p1)
        {
            try
            {
                var ctl = new CourtMotionController();
                CourtMotion courtMotionEntity = ctl.GetCourtMotion(p1);
                CourtMotionViewModel courtMotion = new CourtMotionViewModel { allowed=courtMotionEntity.allowed, id=courtMotionEntity.id, court_id=courtMotionEntity.court_id, motion_id=courtMotionEntity.motion_id };
                return Request.CreateResponse(new CourtMotionResult { data = courtMotion, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtMotionResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCourtMotion(JObject p1)
        {
            try
            {
                var ctl = new CourtMotionController();
                var courtMotion = p1.ToObject<CourtMotion>();
                courtMotion.created_at = DateTime.UtcNow;
                courtMotion.updated_at = DateTime.UtcNow;
                ctl.CreateCourtMotion(courtMotion);
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
        public HttpResponseMessage UpdateCourtMotion(JObject p1)
        {
            try
            {
                var ctl = new CourtMotionController();
                var courtMotion = p1.ToObject<CourtMotion>();
                courtMotion.updated_at = DateTime.UtcNow;
                ctl.UpdateCourtMotion(courtMotion);
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
        public HttpResponseMessage CreateCourt(JObject p1)
        {
            // Implementation for creating a court goes here
            return Request.CreateResponse(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateCourt(JObject p1)
        {
            // Implementation for updating a court goes here
            return Request.CreateResponse(System.Net.HttpStatusCode.OK);
        }

        internal class CourtMotionSearchResult
        {
            public List<CourtMotionViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class CourtMotionResult
        {
            public CourtMotionViewModel data { get; set; }
            public string error { get; set; }
        }
        internal class CourtMotionResults
        {
            public List<CourtMotionViewModel> data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            string fieldName = "court_id";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "court_id";
                    break;
                case 3:
                    fieldName = "motion_id";
                    break;
                case 4:
                    fieldName = "allowed";
                    break;
                default:
                    fieldName = "court_id";
                    break;
            }
            return fieldName;
        }
    }
}