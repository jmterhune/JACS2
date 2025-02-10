using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.TranscriptDatabase.Components;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;

namespace tjc.Modules.TranscriptDatabase.Services
{
    public class DesignationListItemController : DnnApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetDesignationListItems(int count)
        {
            List<DesignationListItemViewModel> caselistItems = new List<DesignationListItemViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            Boolean.TryParse(query["archived"], out bool archived);
            string county = query["county"].ToString();
            string firstName = query["firstName"].ToString();
            string lastName = query["lastName"].ToString();
            string caseNumber = query["caseNumber"].ToString();
            Int32.TryParse(query["order[0].column"], out int sortIndex);
            Int32.TryParse(query["length"], out int pageSize);
            Int32.TryParse(query["start"], out int recordOffset);
            Int32.TryParse(query["draw"], out int draw);
            string sortColumn = GetSortColumn(sortIndex);
            string sortDirection = query["order[0].dir"];
            try
            {
                var ctl = new DesignationController();
                filteredCount = ctl.GetDesignationListCount(firstName, lastName, caseNumber, county, archived);
                if (count == 0) { recordCount = filteredCount; }
                caselistItems = ctl.GetDesignationListPaged(firstName, lastName, caseNumber, county, archived, recordOffset, pageSize, sortColumn, sortDirection).Select(designationListItem => new DesignationListItemViewModel(designationListItem)).ToList();
                return Request.CreateResponse(new DesignationSearchResult { data = caselistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new DesignationSearchResult { data = caselistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [ActionName("Delete")]
        public HttpResponseMessage DeleteDesignation(int designationId)
        {
            try
            {
                var ctl = new DesignationController();
                 ctl.DeleteDesignation(designationId);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [ActionName("Archive")]
        public HttpResponseMessage ToggleArchiveStatus(int designationId)
        {
            try
            {
                var ctl = new DesignationController();
                ctl.ToggleArchiveStatus(designationId);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [ActionName("Acknowledge")]
        public HttpResponseMessage ToggleAcknowledgmentStatus(int designationId)
        {
            try
            {
                var ctl = new DesignationController();
                ctl.ToggleAcknowledgmentStatus(designationId);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public class DesignationSearchResult
        {
            public List<DesignationListItemViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }

        }
        private string GetSortColumn(int columnIndex)
        {
            string name = "DesignationID";
            switch (columnIndex)
            {
                case 2:
                    name = "DesignationID";
                    break;
                case 3:
                    name = "dLastName";
                    break;
                case 4:
                    name = "dFirstName";
                    break;
                case 5:
                    name = "CaseNumber";
                    break;
                case 6:
                    name = "County";
                    break;
                case 7:
                    name = "ServiceDate";
                    break;
                case 9:
                    name = "DueDate";
                    break;
                case 10:
                    name = "TranscriptFiled";
                    break;
                case 11:
                    name = "CreatedByUsername";
                    break;
                default:
                    name = "DesignationID";
                    break;
            }
            return name;
        }
    }
}
