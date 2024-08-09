using Newtonsoft.Json;
using System;
using System.Web;

namespace tjc.Modules.HearingLog.Components.Services.ViewModels
{
    public class LogItemViewModel
    {
        public LogItemViewModel(){}
        public LogItemViewModel(HearingLog hearingLog)
        {
            LogID=hearingLog.LogID;
            CalendarID = hearingLog.CalendarID;
            JudgeID = hearingLog.JudgeID;
            County = hearingLog.County;
            CaseName = hearingLog.CaseName;
            CaseNumber = hearingLog.CaseNumber;
            DIN=hearingLog.DIN;
            MotionTitle = hearingLog.MotionTitle;
            DraftedBy = hearingLog.DraftedBy;
            CourtNotes = HttpUtility.HtmlAttributeEncode(hearingLog.CourtNotes);
            DelayReason = HttpUtility.HtmlAttributeEncode(hearingLog.DelayReason);
            Status = (int)hearingLog.Status;
            OrderSigned = hearingLog.OrderSigned.HasValue?hearingLog.OrderSigned.Value.ToShortDateString():"";
            HearingDate = hearingLog.HearingDate.ToShortDateString();
            SixtiethDayDate=hearingLog.SixtiethDayDate.ToShortDateString();
            CreatedByID=hearingLog.CreatedByID;
        }
        [JsonProperty("logid")]
        public int LogID { get; set; }
        [JsonProperty("ordersigned")]
        public string OrderSigned { get; set; }
        [JsonProperty("hearingdate")]
        public string HearingDate { get; set; }
        [JsonProperty("sixtiethdaydate")]
        public string SixtiethDayDate { get; set; }     
        [JsonProperty("county")]
        public string County { get; set; }
        [JsonProperty("casename")]
        public string CaseName { get; set; }
        [JsonProperty("casenumber")]
        public string CaseNumber { get; set; }
        [JsonProperty("din")]
        public string DIN { get; set; }
        [JsonProperty("motiontitle")]
        public string MotionTitle { get; set; }
        [JsonProperty("draftedby")]
        public string DraftedBy { get; set; }
        [JsonProperty("courtnotes")]
        public string CourtNotes { get; set; }
        [JsonProperty("delayreason")]
        public string DelayReason { get; set; }
        [JsonProperty("createdbyid")]
        public int CreatedByID { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("calendarid")]
        public int CalendarID { get; set; }
        [JsonProperty("judgeid")]
        public string JudgeID { get; set; }

    }
}