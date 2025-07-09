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
        public HttpResponseMessage DeleteCourtType(long p1)
        {
            try
            {
                var ctl = new CourtTypeController();
                ctl.DeleteCourtType(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtType(long p1)
        {
            try
            {
                var ctl = new CourtTypeController();
                CourtType courtType = ctl.GetCourtType(p1);
                return Request.CreateResponse(new CourtTypeResult { data = courtType, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtTypeResult { data = null, error = ex.Message });
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
                courtType.created_at = DateTime.Now;
                courtType.updated_at = DateTime.Now;
                ctl.CreateCourtType(courtType);
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
        public HttpResponseMessage UpdateCourtType(JObject p1)
        {
            try
            {
                var ctl = new CourtTypeController();
                var courtType = p1.ToObject<CourtType>();
                courtType.updated_at = DateTime.Now;
                ctl.UpdateCourtType(courtType);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
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
            string fieldName = "description";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "description";
                    break;
                default:
                    fieldName = "description";
                    break;
            }
            return fieldName;
        }
    }
}