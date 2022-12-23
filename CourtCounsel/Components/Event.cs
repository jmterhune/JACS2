using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_events")]
    //setup the primary key for table
    [PrimaryKey("EventId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Scope("AssignmentId")]
    internal class Event : EntityBase
    {
        public long EventId { get; set; }
        public long AssignmentId { get; set; }
        public string UserName { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long ReminderMinutesBeforeStart { get; set; }
        public bool IsReminderOn { get; set; }
        public bool IsAllDay { get; set; }
        public string ExternalId { get; set; }
        [IgnoreColumn]
        public int ReminderDays { get { return (int)ReminderMinutesBeforeStart / 1440; } }
    }
    internal class CalendarEvent
    {
        public string EventList { get; set; }
        public bool Muted { get; set; }
        public int Day { get; set; }
        public string DayOfWeek { get; set; }
        public string WeekBreak
        {
            get
            {
                if (DayOfWeek != "Saturday")
                { return ""; }
                else { return "<div class=\"w-100\"></div>"; }
            }
        }
    }
    [TableName("court_counsel_event_list")]
    //setup the primary key for table
    internal class EventListItem
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
    internal class EventDTO : EntityBase
    {
        public long EventId { get; set; }
        public long AssignmentId { get; set; }
        public string UserName { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long ReminderMinutesBeforeStart { get; set; }
        public bool IsReminderOn { get; set; }
        public bool IsAllDay { get; set; }
        public string ExternalId { get; set; }
    }
}
