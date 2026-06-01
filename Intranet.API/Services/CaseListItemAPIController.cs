using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Intranet.API.Components.Mediation;
using tjc.Intranet.API.Services.ViewModels.Mediation;

namespace tjc.Intranet.API.Services.Mediation
{
    [DnnAuthorize]
    public class CaseListItemController : DnnApiController
    {

        [HttpGet]
        public HttpResponseMessage GetCaseListItems(int count)
        {
            List<CaseListItemViewModel> caselistItems = new List<CaseListItemViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs()
                   .ToDictionary(kv => kv.Key, kv => kv.Value,
                        StringComparer.OrdinalIgnoreCase);
            Int32.TryParse(query["region"], out int regionId);
            Int32.TryParse(query["group"], out int groupId);
            string firstName = query["firstName"].ToString();
            string lastName = query["lastName"].ToString();
            string businessName = query["businessName"].ToString();
            string cdspNumber = query["cdspNumber"].ToString();
            string caseNumber = query["caseNumber"].ToString();
            Int32.TryParse(query["length"], out int pageSize);
            Int32.TryParse(query["start"], out int recordOffset);
            Int32.TryParse(query["draw"], out int draw);
            string sortColumn = "ListNumber"; // Default sort column
            string sortDirection = "asc"; // Default sort direction
            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }
            try
            {
                var ctl = new Components.Mediation.CaseListItemController();
                filteredCount = ctl.GetCaseListCount(groupId, regionId, caseNumber, cdspNumber, firstName, lastName, businessName);
                if (count == 0) { recordCount = filteredCount; }
                caselistItems = ctl.GetCaseListPaged(groupId, regionId, caseNumber, cdspNumber, firstName, lastName, businessName, recordOffset, pageSize, sortColumn, sortDirection).Select(caselistItem => new CaseListItemViewModel(caselistItem)).ToList();
                return Request.CreateResponse(new CaseSearchResult { data = caselistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (System.Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CaseSearchResult { data = caselistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        [HttpDelete]
        [DnnAuthorize(StaticRoles = "Mediation")]
        public HttpResponseMessage DeleteCase(int caseId)
        {
            try
            {
                var ctl = new Components.Mediation.CaseListItemController();
                ctl.DeleteCase(caseId);
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        public class CaseSearchResult
        {
            public List<CaseListItemViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }

        }
        private string GetSortColumn(int columnIndex)
        {
            string name = "ListNumber";
            switch (columnIndex)
            {
                case 1:
                    name = "ListNumber";
                    break;
                case 2: 
                    name = "Region"; 
                    break;
                case 3: 
                    name = "[Group]";
                    break;
                case 4:
                    name = "PartyOne";
                    break;
                case 5:
                    name = "PartyTwo";
                    break;
                case 6:
                    name = "CreatedDate";
                    break;
                default:
                    name = "ListNumber";
                    break;
            }
            return name;
        }

    }
}
