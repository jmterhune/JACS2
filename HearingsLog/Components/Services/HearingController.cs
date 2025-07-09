using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.HearingLog.Components.Services.ViewModels;
namespace tjc.Modules.HearingLog.Components.Services
{
    public class HearingController : DnnApiController
    {

        [HttpPut]
        [ValidateAntiForgeryToken]
        [DnnAuthorize]
        [ActionName("update-hearing")]
        public HttpResponseMessage UpdateHearingLog(LogItemViewModel hearingViewItem)
        {
            var ctl = new Components.HearingController();
            try
            {
                var query = Request.GetQueryNameValuePairs()
                  .ToDictionary(kv => kv.Key, kv => kv.Value,
                       StringComparer.OrdinalIgnoreCase);
                string jaRole = query["jaRole"].ToString();
                var user = UserController.Instance.GetCurrentUserInfo();
                HearingLog hearing = ctl.GetHearing(hearingViewItem.LogID);
                hearing.HearingDate = DateTime.Parse(hearingViewItem.HearingDate);
                hearing.Status = (StatusType)hearingViewItem.Status;

                if (!string.IsNullOrEmpty(hearingViewItem.OrderSigned))
                {
                    hearing.OrderSigned = DateTime.Parse(hearingViewItem.OrderSigned);
                    hearing.Status = StatusType.Archived;
                }
                else
                {
                    hearing.Status = StatusType.New;
                }
                hearing.DIN = hearingViewItem.DIN;
                hearing.CaseName = hearingViewItem.CaseName;
                hearing.CaseNumber = hearingViewItem.CaseNumber;
                hearing.CourtNotes = hearingViewItem.CourtNotes;
                hearing.DelayReason = hearingViewItem.DelayReason;
                hearing.DraftedBy = hearingViewItem.DraftedBy;
                hearing.MotionTitle = hearingViewItem.MotionTitle;
                if (user != null)
                {
                    if (user.IsInRole(jaRole))
                    {
                        var jCtl = new JudgeController();
                        var jaJudge = jCtl.GetJaJudgeRef(user.UserID);
                        if (jaJudge != null)
                        {
                            hearing.LastModifiedByID = jaJudge.JudgeUserID;
                        }
                        else { hearing.LastModifiedByID = user.UserID; }
                    }
                    else { hearing.LastModifiedByID = user.UserID; }

                }
                hearing.LastModifiedDate = DateTime.Now;
                ctl.UpdateHearing(hearing);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnAuthorize]
        [ActionName("add-hearing")]
        public HttpResponseMessage AddHearingLog(LogItemViewModel hearingViewItem)
        {
            var ctl = new Components.HearingController();
            try
            {
                var query = Request.GetQueryNameValuePairs()
                  .ToDictionary(kv => kv.Key, kv => kv.Value,
                       StringComparer.OrdinalIgnoreCase);
                string jaRole = query["jaRole"].ToString();
                var user = UserController.Instance.GetCurrentUserInfo();
                HearingLog hearing = new HearingLog
                {
                    HearingDate = DateTime.Parse(hearingViewItem.HearingDate),
                    Status = (StatusType)hearingViewItem.Status,
                    DIN = hearingViewItem.DIN,
                    CaseName = hearingViewItem.CaseName,
                    CaseNumber = hearingViewItem.CaseNumber,
                    CourtNotes = hearingViewItem.CourtNotes,
                    DelayReason = hearingViewItem.DelayReason,
                    County = hearingViewItem.County,
                    DraftedBy = hearingViewItem.DraftedBy,
                    MotionTitle = hearingViewItem.MotionTitle,
                    CreatedByID = user.UserID,
                    JudgeID = user.UserID.ToString(),
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                };

                if (!string.IsNullOrEmpty(hearingViewItem.OrderSigned))
                {
                    hearing.OrderSigned = DateTime.Parse(hearingViewItem.OrderSigned);
                    hearing.Status = StatusType.Archived;
                }
                else
                {
                    hearing.Status = StatusType.New;
                }
                if (user != null)
                {
                    if (user.IsInRole(jaRole))
                    {
                        var jCtl = new JudgeController();
                        var jaJudge = jCtl.GetJaJudgeRef(user.UserID);
                        if (jaJudge != null)
                        {
                            hearing.LastModifiedByID = jaJudge.JudgeUserID;
                        }
                        else { hearing.LastModifiedByID = user.UserID; }
                    }
                    else { hearing.LastModifiedByID = user.UserID; }

                }
                ctl.CreateHearing(hearing);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnAuthorize]
        [ActionName("exclude-hearings")]
        public HttpResponseMessage ExcludeHearings(string logIds)
        {
            var ctl = new Components.HearingController();
            try
            {
                var query = Request.GetQueryNameValuePairs()
                  .ToDictionary(kv => kv.Key, kv => kv.Value,
                       StringComparer.OrdinalIgnoreCase);
                string jaRole = query["jaRole"].ToString();
                var user = UserController.Instance.GetCurrentUserInfo();
                var logs=logIds.Split(',');
                foreach (var log in logs) { 

                HearingLog hearing = ctl.GetHearing(Int32.Parse(log));
                if (hearing.Status == StatusType.Excluded)
                {
                    if (hearing.OrderSigned.HasValue)
                        hearing.Status = StatusType.Archived;
                    hearing.Status = StatusType.New;
                }
                else
                {
                    hearing.Status = StatusType.Excluded;
                }
                if (user != null)
                {
                    if (user.IsInRole(jaRole))
                    {
                        var jCtl = new JudgeController();
                        var jaJudge = jCtl.GetJaJudgeRef(user.UserID);
                        if (jaJudge != null)
                        {
                            hearing.LastModifiedByID = jaJudge.JudgeUserID;
                        }
                        else { hearing.LastModifiedByID = user.UserID; }
                    }
                    else { hearing.LastModifiedByID = user.UserID; }

                }
                hearing.LastModifiedDate = DateTime.Now;
                ctl.UpdateHearing(hearing);
                }
                
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [DnnAuthorize]
        [ActionName("toggle-excluded")]
        public HttpResponseMessage UpdateExcludedStatus(int logId)
        {
            var ctl = new Components.HearingController();
            try
            {
                var query = Request.GetQueryNameValuePairs()
                    .ToDictionary(kv => kv.Key, kv => kv.Value,
                    StringComparer.OrdinalIgnoreCase);
                string jaRole = query["jaRole"].ToString();

                var user = UserController.Instance.GetCurrentUserInfo();
                HearingLog hearing = ctl.GetHearing(logId);
                if (hearing.Status == StatusType.Excluded)
                {
                    if (hearing.OrderSigned.HasValue)
                        hearing.Status = StatusType.Archived;
                    hearing.Status = StatusType.New;
                }
                else
                {
                    hearing.Status = StatusType.Excluded;
                }
                if (user != null)
                {
                    if (user.IsInRole(jaRole))
                    {
                        var jCtl = new JudgeController();
                        var jaJudge = jCtl.GetJaJudgeRef(user.UserID);
                        if (jaJudge != null)
                        {
                            hearing.LastModifiedByID = jaJudge.JudgeUserID;
                        }
                        else { hearing.LastModifiedByID = user.UserID; }
                    }
                    else { hearing.LastModifiedByID = user.UserID; }

                }
                hearing.LastModifiedDate = DateTime.Now;
                ctl.UpdateHearing(hearing);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        [DnnAuthorize]
        [ActionName("import-hearings")]
        public HttpResponseMessage ImportHearings()
        {
            var ctl = new Components.HearingController();
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

                DateTime.TryParse(query["startDate"], out DateTime startDate);
                DateTime.TryParse(query["endDate"], out DateTime endDate);
                string jaRole = query["jaRole"].ToString();
                var user = UserController.Instance.GetCurrentUserInfo();
                int UserId = 0;
                if (user != null)
                {
                    UserId = user.UserID;
                    if (user.IsInRole(jaRole))
                    {
                        var jCtl = new JudgeController();
                        var jaJudge = jCtl.GetJaJudgeRef(user.UserID);
                        if (jaJudge != null)
                        {
                            UserId = jaJudge.JudgeUserID;
                        }
                    }
                }
                ctl.ImportHearings(UserId, startDate, endDate);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        [DnnAuthorize]
        public HttpResponseMessage GetLogItems(int count)
        {
            List<LogItemViewModel> loglistItems = new List<LogItemViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs()
                   .ToDictionary(kv => kv.Key, kv => kv.Value,
                        StringComparer.OrdinalIgnoreCase);
            Int32.TryParse(query["status"], out int status);
            DateTime.TryParse(query["startDate"], out DateTime startDate);
            DateTime.TryParse(query["endDate"], out DateTime endDate);
            string selectedJudgeValue = string.Empty;
            string jaRole = query["jaRole"].ToString();
            if (query.ContainsKey("selectedJudge"))
                selectedJudgeValue = query["selectedJudge"].ToString();
            int selectedJudge = selectedJudgeValue == string.Empty ? -1 : Int32.Parse(selectedJudgeValue);
            string searchText = query["searchText"].ToString();
            Int32.TryParse(query["length"], out int pageSize);
            Int32.TryParse(query["start"], out int recordOffset);
            Int32.TryParse(query["draw"], out int draw);
            string sortColumn = "HearingDate"; // Default sort column
            string sortDirection = "asc"; // Default sort direction
            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }
            try
            {
                var user = UserController.Instance.GetCurrentUserInfo();
                int userId = 0;
                if (user != null)
                {
                    userId = user.UserID;
                    if (user.IsInRole(jaRole))
                    {
                        var jCtl = new JudgeController();
                        var ja = jCtl.GetJaJudgeRef(user.UserID);
                        if (ja != null)
                            userId = ja.JudgeUserID;
                    }
                }
                var ctl = new Components.HearingController();
                // Rest of the method remains unchanged
                // ...
                return Request.CreateResponse(new LogSearchResult { data = loglistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (System.Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new LogSearchResult { data = loglistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        [DnnAuthorize]
        public HttpResponseMessage GetCourtCounselItems(int count)
        {
            List<CourtCounselViewModel> loglistItems = new List<CourtCounselViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs()
                   .ToDictionary(kv => kv.Key, kv => kv.Value,
                        StringComparer.OrdinalIgnoreCase);
            DateTime.TryParse(query["startDate"], out DateTime startDate);
            DateTime.TryParse(query["endDate"], out DateTime endDate);
            string selectedJudgeValue = string.Empty;
            string jaRole = query["jaRole"].ToString();
            if (query.ContainsKey("selectedJudge"))
                selectedJudgeValue = query["selectedJudge"].ToString();
            int selectedJudge = selectedJudgeValue == string.Empty ? -1 : Int32.Parse(selectedJudgeValue);
            string searchText = query["searchText"].ToString();
            Int32.TryParse(query["length"], out int pageSize);
            Int32.TryParse(query["start"], out int recordOffset);
            Int32.TryParse(query["draw"], out int draw);
            string sortColumn = "DateReceived"; // Default sort column
            string sortDirection = "asc"; // Default sort direction
            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetCcSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }
            try
            {
                var user = UserController.Instance.GetCurrentUserInfo();
                int userId = 0;
                if (user != null)
                {
                    userId = user.UserID;
                    if (user.IsInRole(jaRole))
                    {
                        var jCtl = new JudgeController();
                        var ja = jCtl.GetJaJudgeRef(user.UserID);
                        if (ja != null)
                            userId = ja.JudgeUserID;
                    }
                }
                var ctl = new Components.CourtCounselController();
                // Rest of the method remains unchanged
                // ...
                return Request.CreateResponse(new CourtCounselSearchResult { data = loglistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (System.Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtCounselSearchResult { data = loglistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        private string GetSortColumn(int columnIndex)
        {
            string name = "HearingDate";
            switch (columnIndex)
            {
                case 1:
                    name = "OrderSigned";
                    break;
                case 2:
                    name = "HearingDate";
                    break;
                case 3:
                    name = "SixtiethDayDate";
                    break;
                case 4:
                    name = "County";
                    break;
                case 5:
                    name = "CaseName";
                    break;
                case 6:
                    name = "CaseNumber";
                    break;
                case 7:
                    name = "DIN";
                    break;
                case 8:
                    name = "MotionTitle";
                    break;
                case 9:
                    name = "DraftedBy";
                    break;
                case 10:
                    name = "JudgeID";
                    break;
                default:
                    name = "HearingDate";
                    break;
            }
            return name;
        }

        private string GetCcSortColumn(int columnIndex)
        {
            string name = "DateReceived";
            switch (columnIndex)
            {
                case 0:
                    name = "MotionFiled";
                    break;
                case 2:
                    name = "DateReceived";
                    break;
                case 3:
                    name = "CaseName";
                    break;
                case 4:
                    name = "CaseNumber";
                    break;
                case 5:
                    name = "CaseType";
                    break;
                case 6:
                    name = "CaseStatus";
                    break;
                case 7:
                    name = "Attorney";
                    break;
                default:
                    name = "DateReceived";
                    break;
            }
            return name;
        }

        internal class LogSearchResult
        {
            public List<LogItemViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }

        }
        internal class CourtCounselSearchResult
        {
            public List<CourtCounselViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }

        }
    }
}
