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
    public class AttorneyAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetAttorneys(int p1)
        {
            List<AttorneyViewModel> attorneys = new List<AttorneyViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string searchTerm = query["searchText"].ToString();
            Int32.TryParse(query["order[0].column"], out int sortIndex);
            Int32.TryParse(query["length"], out int pageSize);
            Int32.TryParse(query["start"], out int recordOffset);
            Int32.TryParse(query["draw"], out int draw);
            string sortColumn = GetSortColumn(sortIndex);
            string sortDirection = query["order[0].dir"];
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
               Attorney attorney= ctl.GetAttorney(p1);
                return Request.CreateResponse(new AttorneyResult { data = attorney, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new AttorneyResult { data = null, error = ex.Message });
            }
        }
        [HttpPost]
        public HttpResponseMessage CreateAttorney(JObject p1)
        {
            try
            {
                var ctl = new AttorneyController();
                var attorney = p1.ToObject<Attorney>();
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
        public HttpResponseMessage UpdateAttorney(JObject p1)
        {
            try
            {
                var ctl = new AttorneyController();
                
                var attorney = p1.ToObject<Attorney>();
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
