using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Intranet.API.Services.ViewModels.Mediation;

namespace tjc.Intranet.API.Services.Mediation
{
    public class AttorneyListItemController : DnnApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAttorneyListItems(int count)
        {
            List<AttorneyListItemViewModel> attorneylistItems = new List<AttorneyListItemViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string firstName = query["firstName"].ToString();
            string lastName = query["lastName"].ToString();
            string firm = query["firm"].ToString();
            Int32.TryParse(query["order[0].column"], out int sortIndex);
            Int32.TryParse(query["length"], out int pageSize);
            Int32.TryParse(query["start"], out int recordOffset);
            Int32.TryParse(query["draw"], out int draw);
            string sortColumn = GetSortColumn(sortIndex);
            string sortDirection = query["order[0].dir"];
            try
            {
                var ctl = new Components.Mediation.AttorneyListItemController();
                filteredCount = ctl.GetAttorneyListCount(firstName, lastName, firm);
                if (count == 0) { recordCount = filteredCount; }
                attorneylistItems = ctl.GetAttorneyListPaged(firstName, lastName, firm, recordOffset, pageSize, sortColumn, sortDirection).Select(attorneylistItem => new AttorneyListItemViewModel(attorneylistItem)).ToList();
                return Request.CreateResponse(new MediationSearchResult { data = attorneylistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (System.Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new MediationSearchResult { data = attorneylistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        public class MediationSearchResult
        {
            public List<AttorneyListItemViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }

        }
        private string GetSortColumn(int columnIndex)
        {
            string name = "LastName";
            switch (columnIndex)
            {
                case 1:
                    name = "FirstName";
                    break;
                case 2:
                    name = "LastName";
                    break;
                case 3:
                    name = "Firm";
                    break;
                default:
                    name = "LastName";
                    break;
            }
            return name;
        }
    }
}
