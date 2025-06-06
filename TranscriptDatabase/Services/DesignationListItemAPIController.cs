using DotNetNuke.Entities.Users;
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
    [DnnAuthorize]
    public class DesignationListItemController : DnnApiController
    {
        [HttpGet]
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
        [ActionName("GetMatchingNames")]
        public HttpResponseMessage GetMatchingNames()
        {
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string lastName = query["lastName"].ToString();
            try
            {
                var ctl = new DesignationController();
                IEnumerable<NameMatchViewModel> matchingNames = ctl.GetMatchingNames(lastName);
                return Request.CreateResponse(new MatchingNameResult { data = matchingNames, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new MatchingNameResult { data = null, error = ex.Message });
            }
        }
        [HttpPost]
        [ActionName("CreateDesignation")]
        public HttpResponseMessage CreateDesignation(DesignationViewModel designationViewModel)
        {
            var ctl = new Components.DesignationController();
            var attorneys = designationViewModel.Attorneys.Split(',');
            DateTime dueDate = DateTime.Now.AddDays(30);
            if (DateTime.TryParse(designationViewModel.ReceiptDate, out DateTime receiptdate))
                dueDate = receiptdate.AddDays(30);
            string adminRole = designationViewModel.AdminRole;
            Designation designation = new Designation
            {
                dFirstName = designationViewModel.FirstName,
                dLastName = designationViewModel.LastName,
                dMiddleName = designationViewModel.MiddleName,
                County = designationViewModel.County,
                LowerTribunalCaseNumber = designationViewModel.TribunalCaseNumber,
                AppellateCaseNumber = designationViewModel.AppellateCaseNumber,
                ServiceDate = DateTime.Parse(designationViewModel.ServiceDate),
                ReceiptDate = DateTime.Parse(designationViewModel.ReceiptDate),
                CreatedByUserID = designationViewModel.CreatedByUserID,
                CreatedDate = DateTime.Now,
                DueDate = dueDate,
                LastModifiedByUserID = designationViewModel.CreatedByUserID,
                LastModifiedDate = DateTime.Now
            };
            try
            {
                ctl.CreateDesignation(designation);
                foreach (string attorneyId in attorneys)
                {
                    ctl.CreateDesignationAttorney(designation.DesignationID, Int32.Parse(attorneyId));
                }
                bool result = designation.DesignationID > 0;
                if (result)
                {
                    AddDueDate(designation);
                    int portalId = PortalSettings.PortalId;
                    UserInfo userinfo = UserController.Instance.GetUserById(portalId, designation.CreatedByUserID);
                    Notifications.NotifiyRecordingManager(portalId, designation.DesignationID,userinfo.Email,adminRole,designation.DisplayName,designation.County);
                    var dCtl = new DesignationController();
                    designationViewModel.DesignationID = designation.DesignationID;
                    return Request.CreateResponse(new DesignationResult { designationId = designationViewModel.DesignationID, error = null });
                }
                return Request.CreateResponse(new DesignationResult { designationId =-1, error = "Unable to Create Designation" });
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                return Request.CreateResponse(new DesignationResult { designationId = -1, error =exc.Message });
            }
        }
        [HttpGet]
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
        public class MatchingNameResult
        {
            public IEnumerable<NameMatchViewModel> data { get; set; }
            public string error { get; set; }
        }
        public class DesignationResult
        {
            public int designationId { get; set; }
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
                case 8:
                    name = "AcknowledgmentFiled";
                    break;
                case 9:
                    name = "DueDate";
                    break;
                case 10:
                    name = "TranscriptFiled";
                    break;
                case 11:
                    name = "CreatedByName";
                    break;
                default:
                    name = "DesignationID";
                    break;
            }
            return name;
        }
        private void AddDueDate(Designation designation)
        {
            var ctl = new CalendarController();
            Components.Calendar calendar = new Components.Calendar
            {
                CreatedByUserID = designation.CreatedByUserID,
                CreatedDate = designation.CreatedDate,
                LastModifiedByUserID = designation.LastModifiedByUserID,
                LastModifiedDate = designation.LastModifiedDate,
                DesignationID = designation.DesignationID,
                StartTime = designation.DueDate.Value,
                EndTime = designation.DueDate.Value,
                EventTypeID = (int)EventTypes.dueDate,
                RequestOutstanding = false,
                Subject = designation.CalendarName
            };
            ctl.CreateCalendar(calendar);
        }
    }
}
