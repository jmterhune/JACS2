using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Intranet.API.Components
{
    public class Event : EntityBase
    {
        public long EventId { get; set; }
        public long AssignmentId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate{ get; set; }
        public string UserName { get; set; }
        public bool IsReminderOn { get; set; }
        public long ReminderMinutesBeforeStart { get; set; }
        public bool IsAllDay { get; set; }
        public string ExternalId { get; set; }
        [IgnoreColumn]
        public int ReminderDays { get { return (int)ReminderMinutesBeforeStart / 1440; } }
    }
    //setup the primary key for table
    public class EventListItem
    {
        public long EventId { get; set; }
        public long AssignmentId { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public string UserName { get; set; }
        public string CaseTypeName { get; set; }
        public DateTime DateReceived { get; set; }
        public string Subject { get; set; }
        public DateTime StartDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
