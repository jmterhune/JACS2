using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.CourtRegistry.Components;
namespace tjc.Modules.CourtRegistry.Services
{
    public class ApplicationAPIController : DnnApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetApplicationListItems(int count)
        {
            List<ApplicationViewModel> caselistItems = new List<ApplicationViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            int applicationId = -1;
            int periodYear = -1;
            int statusId = -1;
            string firstName = string.Empty;
            string lastName = string.Empty;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            if (query.ContainsKey("applicationid"))
                Int32.TryParse(query["applicationid"], out applicationId);
            if (query.ContainsKey("year"))
                Int32.TryParse(query["year"], out  periodYear);
            if (query.ContainsKey("status"))
                Int32.TryParse(query["status"], out  statusId);
            if (query.ContainsKey("firstName"))
                firstName = query["firstName"].ToString();
            if (query.ContainsKey("lastName"))
                lastName = query["lastName"].ToString();
            Int32.TryParse(query["order[0].column"], out int sortIndex);
            Int32.TryParse(query["length"], out int pageSize);
            Int32.TryParse(query["start"], out int recordOffset);
            Int32.TryParse(query["draw"], out int draw);
            string sortColumn = GetSortColumn(sortIndex);
            string sortDirection = query["order[0].dir"];
            try
            {
                var ctl = new ApplicationController();
                filteredCount = ctl.GetApplicationListCount(applicationId, periodYear, firstName, lastName, statusId);
                if (count == 0) { recordCount = filteredCount; }
                caselistItems = ctl.GetDesignationListPaged(applicationId, periodYear, firstName, lastName, statusId, recordOffset, pageSize, sortColumn, sortDirection).Select(applicationListItem => new ApplicationViewModel(applicationListItem)).ToList();
                return Request.CreateResponse(new ApplicationSearchResult { data = caselistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ApplicationSearchResult { data = caselistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [ActionName("Delete")]
        public HttpResponseMessage DeleteApplication(int applicationId)
        {
            try
            {
                var ctl = new ApplicationController();
                ctl.DeleteApplication(applicationId);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public class ApplicationSearchResult
        {
            public List<ApplicationViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }

        }
        public class ApplicationResult
        {
            public int applicationId { get; set; }
            public string error { get; set; }

        }
        private string GetSortColumn(int columnIndex)
        {
            string name = "ApplicationID";
            switch (columnIndex)
            {
                case 1:
                    name = "ApplicationID";
                    break;
                case 2:
                    name = "Year";
                    break;
                case 3:
                    name = "LastName";
                    break;
                case 4:
                    name = "FirstName";
                    break;
                case 5:
                    name = "DateCreated";
                    break;
                case 6:
                    name = "DateReviewed";
                    break;
                case 7:
                    name = "YearsOnRegistry";
                    break;
                case 8:
                    name = "IsRenewal";
                    break;
                case 9:
                    name = "GuardianSignature";
                    break;
                case 10:
                    name = "Status";
                    break;
                default:
                    name = "ApplicationID";
                    break;
            }
            return name;
        }
    }
}
