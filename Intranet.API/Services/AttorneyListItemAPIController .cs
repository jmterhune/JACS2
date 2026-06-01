using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Intranet.API.Components.Mediation;
using tjc.Intranet.API.Services.ViewModels.Mediation;

namespace tjc.Intranet.API.Services.Mediation
{
    [DnnAuthorize]
    public class AttorneyListItemController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetAttorneyListItems(int count)
        {
            List<AttorneyListItemViewModel> attorneylistItems = new List<AttorneyListItemViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string firstName = query["firstName"].ToString();
            string lastName = query["lastName"].ToString();
            string firm = query["firm"].ToString();
            Int32.TryParse(query["length"], out int pageSize);
            Int32.TryParse(query["start"], out int recordOffset);
            Int32.TryParse(query["draw"], out int draw);
            string sortColumn = "LastName"; // Default sort column
            string sortDirection = "asc"; // Default sort direction
            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }
            try
            {
                var ctl = new Components.Mediation.AttorneyListItemController();
                filteredCount = ctl.GetAttorneyListCount(firstName, lastName, firm);
                if (count == 0) { recordCount = filteredCount; }
                attorneylistItems = ctl.GetAttorneyListPaged(firstName, lastName, firm, recordOffset, pageSize, sortColumn, sortDirection).Select(attorneylistItem => new AttorneyListItemViewModel(attorneylistItem)).ToList();
                return Request.CreateResponse(new AttorneySearchResult { data = attorneylistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (System.Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new AttorneySearchResult { data = attorneylistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        public class AttorneySearchResult
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
        [HttpPost]
        public HttpResponseMessage CreateAttorney(AttorneyListItemViewModel attorneyViewItem)
        {
            var ctl = new Components.Mediation.AttorneyListItemController();
            AttorneyListItem attorney = new AttorneyListItem { Email = attorneyViewItem.Email, FirstName = attorneyViewItem.FirstName, LastName = attorneyViewItem.LastName, Phone = attorneyViewItem.Phone,Extension=attorneyViewItem.Extension, Address=attorneyViewItem.Address, City=attorneyViewItem.City, Firm=attorneyViewItem.Firm, State=attorneyViewItem.State, Zip=attorneyViewItem.Zip };
            try
            {
                ctl.CreateAttorney(attorney);
                bool result = attorney.AttorneyId > 0;
                if (result)
                {
                    return Request.CreateResponse(new AttorneyAddedResult { AttorneyId = attorney.AttorneyId });
                }
                return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public class AttorneyAddedResult
        {
            public int AttorneyId { get; set; }

        }
    }
}
