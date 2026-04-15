using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.CourtCounsel.Components.Models
{
    [TableName("tjc_cc_history")]
    [PrimaryKey("logId", AutoIncrement = true)]
    public class HistoryInfo
    {
        public int LogId { get; set; }
        public DateTime DateReceived { get; set; }
        public string CaseNumber { get; set; }
        public string PartyName { get; set; }
        public string CaseType { get; set; }
        public DateTime? DateDue { get; set; }
        public string RequestedBy { get; set; }
        public string Responsible { get; set; }
        public string County { get; set; }
        public string Description { get; set; }
        public string Phase { get; set; }
        public string Action { get; set; }
        public string FollowUp { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string TimeSpent { get; set; }
        public string Comments { get; set; }
        public string StatusName { get; set; }
        public DateTime? MotionFiled { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [IgnoreColumn]
        public CurrentStatus Status
        {
            get
            {
                if (DateCompleted.HasValue)
                    return CurrentStatus.Complete;
                if (DateReceived > DateTime.Now)
                    return CurrentStatus.Inactive;
                return CurrentStatus.Active;
            }
        }

        [IgnoreColumn]
        public int StatusSort
        {
            get { return (int)Status; }
        }

        public enum CurrentStatus
        {
            Active = 0,
            Inactive = 1,
            Complete = 2
        }
    }
}
