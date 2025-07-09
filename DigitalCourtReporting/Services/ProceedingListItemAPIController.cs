using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Mail;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.DigitalCourtReporting.Components;
using tjc.Modules.DigitalCourtReporting.Services.ViewModels;

namespace tjc.Modules.DigitalCourtReporting.Services
{
    public class ProceedingListItemController : DnnApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetProceedingListItems(int count)
        {
            List<ProceedingListItemViewModel> proceedinglistItems = new List<ProceedingListItemViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string searchText = !query.ContainsKey("searchText") || query["searchText"] == "null" ? "" : query["searchText"].ToString();
            Int32.TryParse(query.ContainsKey("listType") ? query["listType"] : "0", out int listTypeId);
            Int32.TryParse(query.ContainsKey("searchType") ? query["searchType"] : "0", out int searchTypeId);
            ListTypes listType = (ListTypes)listTypeId;
            SearchTypes searchType = (SearchTypes)searchTypeId;
            Int32.TryParse(query.ContainsKey("countyId") ? query["countyId"] : "0", out int countyId);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);

            string sortColumn = "RequestDateFormatted"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new ProceedingController();
                filteredCount = ctl.GetProceedingsCount(listTypeId, searchTypeId, searchText, countyId);
                if (count == 0) { recordCount = filteredCount; }
                proceedinglistItems = ctl.GetProceedingsPaged(listTypeId, searchTypeId, searchText, countyId, recordOffset, pageSize, sortColumn, sortDirection).Select(proceedingListItem => new ProceedingListItemViewModel(proceedingListItem)).ToList();
                return Request.CreateResponse(new ProceedingSearchResult { data = proceedinglistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ProceedingSearchResult { data = proceedinglistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("Delete")]
        public HttpResponseMessage DeleteProceeding(int proceedingId)
        {
            try
            {
                var ctl = new ProceedingController();
                ctl.DeleteProceeding(proceedingId);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        internal class ProceedingSearchResult
        {
            public List<ProceedingListItemViewModel> data { get; set; }
            public int draw { get; set; }
            public int recordsFiltered { get; set; }
            public int recordsTotal { get; set; }
            public object error { get; set; }
        }

        public class ProceedingResult
        {
            public int proceedingId { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            string name = "RequestDateFormatted";
            switch (columnIndex)
            {
                case 1:
                    name = "RequestDateFormatted";
                    break;
                case 2:
                    name = "Requestor";
                    break;
                case 3:
                    name = "CaseName";
                    break;
                case 4:
                    name = "CaseNumber";
                    break;
                case 5:
                    name = "ProceedingDate";
                    break;
                default:
                    name = "RequestDateFormatted";
                    break;
            }
            return name;
        }
    }
}