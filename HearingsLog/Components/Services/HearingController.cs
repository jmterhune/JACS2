using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using tjc.Modules.HearingLog.Components.Services.ViewModels;
namespace tjc.Modules.HearingLog.Components.Services
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class HearingController : DnnApiController
    {

        [HttpPut]
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
        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public HttpResponseMessage GetLogItems(int count)
        {
            List<LogItemViewModel> loglistItems = new List<LogItemViewModel>();
            int recordCount = count;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs()
                   .ToDictionary(kv => kv.Key, kv => kv.Value,
                        StringComparer.OrdinalIgnoreCase);
            Int32.TryParse(query["status"], out int status);
            DateTime.TryParse(query["cutoffDate"], out DateTime cutoffDate);
            string selectedJudgeValue = string.Empty;
            string jaRole = query["jaRole"].ToString();
            if (query.ContainsKey("selectedJudge"))
                selectedJudgeValue = query["selectedJudge"].ToString();
            int selectedJudge = selectedJudgeValue == string.Empty ? -1 : Int32.Parse(selectedJudgeValue);
            string searchText = query["searchText"].ToString();
            Int32.TryParse(query["order[0].column"], out int sortIndex);
            Int32.TryParse(query["length"], out int pageSize);
            Int32.TryParse(query["start"], out int recordOffset);
            Int32.TryParse(query["draw"], out int draw);
            string sortColumn = GetSortColumn(sortIndex);
            string sortDirection = query["order[0].dir"];
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
                if (selectedJudge >= 0)
                {
                    filteredCount = ctl.GetHearingLogCount(userId, status, cutoffDate, searchText, selectedJudge);
                    if (count == 0) { recordCount = filteredCount; }
                    loglistItems = ctl.GetHearingLogPaged(userId, status, cutoffDate, searchText, selectedJudge, recordOffset, pageSize, sortColumn, sortDirection).Select(loglistItem => new LogItemViewModel(loglistItem)).ToList();
                }
                else
                {
                    if (string.IsNullOrEmpty(searchText))
                    {
                        filteredCount = ctl.GetHearingLogCount(userId, status, cutoffDate);
                    }
                    else
                    {
                        filteredCount = ctl.GetHearingLogCount(userId, status, cutoffDate, searchText);
                    }
                    if (count == 0) { recordCount = filteredCount; }
                    if (string.IsNullOrEmpty(searchText))
                    {
                        loglistItems = ctl.GetHearingLogPaged(userId, status, cutoffDate, recordOffset, pageSize, sortColumn, sortDirection).Select(loglistItem => new LogItemViewModel(loglistItem)).ToList();
                    }
                    else
                    {
                        loglistItems = ctl.GetHearingLogPaged(userId, status, cutoffDate, searchText, recordOffset, pageSize, sortColumn, sortDirection).Select(loglistItem => new LogItemViewModel(loglistItem)).ToList();
                    }
                }
                return Request.CreateResponse(new LogSearchResult { data = loglistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (System.Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new LogSearchResult { data = loglistItems, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        internal class LogSearchResult
        {
            public List<LogItemViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }

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

    }
}
