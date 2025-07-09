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
    public class CourtTemplateAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourtTemplates(int p1)
        {
            List<CourtTemplateViewModel> courtTemplates = new List<CourtTemplateViewModel>();
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
                var ctl = new CourtTemplateController();
                filteredCount = ctl.GetCourtTemplatesCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                courtTemplates = ctl.GetCourtTemplatesPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).ToList();
                return Request.CreateResponse(new CourtTemplateSearchResult { data = courtTemplates, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtTemplateSearchResult { data = courtTemplates, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteCourtTemplate(long p1)
        {
            try
            {
                var ctl = new CourtTemplateController();
                ctl.DeleteCourtTemplate(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtTemplate(long p1)
        {
            try
            {
                var ctl = new CourtTemplateController();
                CourtTemplate courtTemplate = ctl.GetCourtTemplate(p1);
                return Request.CreateResponse(new CourtTemplateResult { data = courtTemplate, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtTemplateResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCourtTemplate(JObject p1)
        {
            try
            {
                var ctl = new CourtTemplateController();
                var courtTemplate = p1.ToObject<CourtTemplate>();
                courtTemplate.created_at = DateTime.UtcNow;
                courtTemplate.updated_at = DateTime.UtcNow;
                ctl.CreateCourtTemplate(courtTemplate);
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
        public HttpResponseMessage UpdateCourtTemplate(JObject p1)
        {
            try
            {
                var ctl = new CourtTemplateController();
                var courtTemplate = p1.ToObject<CourtTemplate>();
                courtTemplate.updated_at = DateTime.UtcNow;
                ctl.UpdateCourtTemplate(courtTemplate);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal class CourtTemplateSearchResult
        {
            public List<CourtTemplateViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class CourtTemplateResult
        {
            public CourtTemplate data { get; set; }
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
                    fieldName = "judge_name";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}