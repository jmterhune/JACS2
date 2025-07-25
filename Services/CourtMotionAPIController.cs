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
    public class CourtMotionAPIController : DnnApiController
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
        public HttpResponseMessage DeleteCourtMotion(long p1)
        {
            try
            {
                var ctl = new CourtMotionController();
                ctl.DeleteCourtMotion(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court Motion deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtMotion(long p1)
        {
            try
            {
                var ctl = new CourtMotionController();
                CourtMotion courtMotionEntity = ctl.GetCourtMotion(p1);
                if (courtMotionEntity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CourtMotionResult { data = null, error = "Court Motion not found" });
                }
                CourtMotionViewModel courtMotion = new CourtMotionViewModel { allowed = courtMotionEntity.allowed, id = courtMotionEntity.id, court_id = courtMotionEntity.court_id, motion_id = courtMotionEntity.motion_id };
                return Request.CreateResponse(HttpStatusCode.OK, new CourtMotionResult { data = courtMotion, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CourtMotionResult { data = null, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtMotionDropDownItems(long p1, bool p2)
        {
            try
            {
                var ctl = new CourtMotionController();
                List<KeyValuePair<long, string>> courtMotions = ctl.GetCourtMotionDropDownItems(p1, p2);
                return Request.CreateResponse(new ListItemOptionResult { data = courtMotions, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ListItemOptionResult { data = null, error = ex.Message });
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
                if (courtMotion.court_id <= 0 || courtMotion.motion_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court ID and Motion ID are required." });
                }
                courtMotion.created_at = DateTime.UtcNow;
                courtMotion.updated_at = DateTime.UtcNow;
                ctl.CreateCourtMotion(courtMotion);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court Motion created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
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
                if (courtMotion.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court Motion ID is required for update." });
                }
                if (courtMotion.court_id <= 0 || courtMotion.motion_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court ID and Motion ID are required." });
                }
                var existingCourtMotion = ctl.GetCourtMotion(courtMotion.id);
                if (existingCourtMotion == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court Motion not found." });
                }
                courtMotion.updated_at = DateTime.UtcNow;
                ctl.UpdateCourtMotion(courtMotion);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court Motion updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAvailableMotionDropDownItems(long p1, string p2 = "")
        {
            try
            {
                var ctl = new CourtMotionController();
                List<long> excluded = string.IsNullOrEmpty(p2) ? new List<long>() : p2.Split(',').Select(long.Parse).ToList();
                var motions = ctl.GetAvailableMotionDropDownItems(p1, excluded);
                return Request.CreateResponse(new ListItemOptionResult { data = motions, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ListItemOptionResult { data = null, error = ex.Message });
            }
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