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
    public class CourtTypeAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourtTypes(int p1)
        {
            List<CourtTypeViewModel> courtTypes = new List<CourtTypeViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = !query.ContainsKey("searchText") ? "" : query["searchText"].ToString();
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "description"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new CourtTypeController();
                filteredCount = ctl.GetCourtTypesCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                courtTypes = ctl.GetCourtTypesPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(courtType => new CourtTypeViewModel(courtType)).ToList();
                return Request.CreateResponse(new CourtTypeSearchResult { data = courtTypes, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtTypeSearchResult { data = courtTypes, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtTypeDropDownItems()
        {
            List<KeyValuePair<long, string>> courtTypes = new List<KeyValuePair<long, string>>();

            try
            {
                var ctl = new CourtTypeController();
                courtTypes = ctl.GetCourtTypeDropDownItems();
                return Request.CreateResponse(new ListItemOptionResult { data = courtTypes, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ListItemOptionResult { data = courtTypes, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteCourtType(long p1)
        {
            try
            {
                var ctl = new CourtTypeController();
                ctl.DeleteCourtType(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court Type deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtType(long p1)
        {
            try
            {
                var ctl = new CourtTypeController();
                CourtType courtType = ctl.GetCourtType(p1);
                if (courtType == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CourtTypeResult { data = null, error = "Court Type not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new CourtTypeResult { data = courtType, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CourtTypeResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCourtType(JObject p1)
        {
            try
            {
                var ctl = new CourtTypeController();
                var courtType = p1.ToObject<CourtType>();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(courtType.description) || string.IsNullOrWhiteSpace(courtType.code))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Description and Code are required." });
                }

                courtType.created_at = DateTime.Now;
                courtType.updated_at = DateTime.Now;
                ctl.CreateCourtType(courtType);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court Type created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateCourtType(JObject p1)
        {
            try
            {
                var ctl = new CourtTypeController();
                var courtType = p1.ToObject<CourtType>();

                // Validate required fields
                if (courtType.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court Type ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(courtType.description) || string.IsNullOrWhiteSpace(courtType.code))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Description and Code are required." });
                }

                var existingCourtType = ctl.GetCourtType(courtType.id);
                if (existingCourtType == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court Type not found." });
                }

                courtType.updated_at = DateTime.Now;
                ctl.UpdateCourtType(courtType);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court Type updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        internal class CourtTypeSearchResult
        {
            public List<CourtTypeViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class CourtTypeResult
        {
            public CourtType data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return "code";
                case 3:
                    return "description";
                default:
                    return "description";
            }
        }
    }
}