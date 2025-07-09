using DotNetNuke.Security;
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
    [DnnAuthorize]
    public class AttorneyAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetAttorneyListItems(int count)
        {
            List<AttorneyViewModel> attorneyListItems = new List<AttorneyViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            int barNumber = -1;
            string firstName = string.Empty;
            string lastName = string.Empty;
            string email = string.Empty;
            string lawFirm = string.Empty;

            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            if (query.ContainsKey("barnumber"))
                Int32.TryParse(query["barnumber"], out barNumber);
            if (query.ContainsKey("firstName"))
                firstName = query["firstName"].ToString();
            if (query.ContainsKey("lastName"))
                lastName = query["lastName"].ToString();
            if (query.ContainsKey("email"))
                email = query["email"].ToString();
            if (query.ContainsKey("lawFirm"))
                lawFirm = query["lawFirm"].ToString();
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            string sortColumn = "AttorneyID"; // Default sort column
            string sortDirection = "asc"; // Default sort direction
            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }
            try
            {
                var ctl = new AttorneyController();
                filteredCount = ctl.GetAttorneyListCount(barNumber, firstName, lastName, email, lawFirm);
                if (count == 0) { recordCount = filteredCount; }
                attorneyListItems = ctl.GetAttorneyListPaged(barNumber, firstName, lastName, email, lawFirm, recordOffset, pageSize, sortColumn, sortDirection).Select(attorneyListItem => new AttorneyViewModel(attorneyListItem)).ToList();
                return Request.CreateResponse(new AttorneySearchResult { data = attorneyListItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new AttorneySearchResult { data = attorneyListItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        [HttpGet]
        [ActionName("Delete")]
        public HttpResponseMessage DeleteAttorney(int attorneyId)
        {
            try
            {
                var ctl = new AttorneyController();
                ctl.DeleteAttorney(attorneyId);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        [ActionName("GetAttorney")]
        public HttpResponseMessage GetAttorney(int attorneyId)
        {
            try
            {
                var ctl = new AttorneyController();
                AttorneyViewModel attorney = new AttorneyViewModel(ctl.GetAttorney(attorneyId));
                return Request.CreateResponse(new AttorneyGetResult { attorney= attorney , error=null});
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new AttorneyGetResult { attorney = null, error = ex.Message});
            }
        }
        [HttpPost]
        internal HttpResponseMessage SaveAttorney(AttorneyViewModel attorney)
        {
            try
            {
                var ctl = new AttorneyController();
                if (attorney.AttorneyID == 0)
                {
                    ctl.CreateAttorney(new Attorney { BarNumber = attorney.BarNumber, LastName = attorney.LastName, FirstName = attorney.FirstName, Email = attorney.Email, Phone = attorney.Phone, Cell = attorney.Cell, Fax = attorney.Fax, LawFirm = attorney.LawFirm });
                }
                else
                {
                    ctl.UpdateAttorney(new Attorney { AttorneyID = attorney.AttorneyID, BarNumber = attorney.BarNumber, LastName = attorney.LastName, FirstName = attorney.FirstName, Email = attorney.Email, Phone = attorney.Phone, Cell = attorney.Cell, Fax = attorney.Fax, LawFirm = attorney.LawFirm });
                }
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
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
        internal class AttorneyResult
        {
            public int attorneyId { get; set; }
            public string error { get; set; }

        }
        internal class AttorneyGetResult
        {
            public AttorneyViewModel attorney { get; set; }
            public string error { get; set; }

        }
        private string GetSortColumn(int columnIndex)
        {
            string name = "AttorneyID";
            switch (columnIndex)
            {
                case 1:
                    name = "AttorneyID";
                    break;
                case 2:
                    name = "BarNumber";
                    break;
                case 3:
                    name = "LastName";
                    break;
                case 4:
                    name = "FirstName";
                    break;
                case 5:
                    name = "Email";
                    break;
                case 6:
                    name = "Phone";
                    break;
                case 7:
                    name = "Cell";
                    break;
                case 8:
                    name = "Fax";
                    break;
                case 9:
                    name = "LawFirm";
                    break;
                default:
                    name = "AttorneyID";
                    break;
            }
            return name;
        }
    }
}
