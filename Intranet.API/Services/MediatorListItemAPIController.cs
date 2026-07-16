using DotNetNuke.Security;
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

    public class MediatorListItemController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetMediatorListItems(int count)
        {
            List<MediatorListItemViewModel> mediatorlistItems = new List<MediatorListItemViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string firstName = query.ContainsKey("firstName") ? query["firstName"] ?? "" : "";
            string lastName = query.ContainsKey("lastName") ? query["lastName"] ?? "" : "";
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
                var ctl = new Components.Mediation.MediatorListItemController();
                filteredCount = ctl.GetMediatorListCount(firstName, lastName);
                if (count == 0) { recordCount = filteredCount; }
                mediatorlistItems = ctl.GetMediatorListPaged(firstName, lastName, recordOffset, pageSize, sortColumn, sortDirection).Select(mediatorlistItem => new MediatorListItemViewModel(mediatorlistItem)).ToList();
                return Request.CreateResponse(new MediationSearchResult { data = mediatorlistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (System.Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new MediationSearchResult { data = mediatorlistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        [HttpPost]
        [ActionName("add-mediator")]
        public HttpResponseMessage CreateMediator(MediatorListItemViewModel mediatorViewItem)
        {
            var ctl = new Components.Mediation.MediatorListItemController();
            MediatorListItem mediator = new MediatorListItem { Email = mediatorViewItem.Email, FirstName = mediatorViewItem.FirstName, LastName = mediatorViewItem.LastName, Phone = mediatorViewItem.Phone };
            try
            {
                ctl.CreateMediator(mediator);
                bool result = mediator.MediatorId > 0;
                if (result)
                {
                    return Request.CreateResponse(new MediationAddedResult { MediationId = mediator.MediatorId });
                }
                return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public class MediationSearchResult
        {
            public List<MediatorListItemViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }

        }
        public class MediationAddedResult
        {
            public int MediationId { get; set; }

        }
        private string GetSortColumn(int columnIndex)
        {
            string name;
            switch (columnIndex)
            {
                case 1:
                    name = "FirstName";
                    break;
                case 2:
                    name = "LastName";
                    break;
                case 3:
                    name = "MediatorName";
                    break;

                default:
                    name = "LastName";
                    break;
            }
            return name;
        }
    }
}
