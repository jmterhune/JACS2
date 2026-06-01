
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Users;
using System;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_event")]
    [PrimaryKey("EventID", AutoIncrement = true)]
    public class Event : EntityBase
    {
        public int EventID { get; set; }
        public int DesignationID { get; set; }
        public DateTime? HearingDate { get; set; }
        public int PresidingJudgeID { get; set; }
        public string HearingType { get; set; }
        public int CourtReporterID { get; set; }
        public int Pages { get; set; }
        public int DaysUntilComplete { get; set; }
        public int ScopistID { get; set; }
        public DateTime? ScopSent { get; set; }
        public DateTime? ScopReturned { get; set; }
        public int ScopPagesIn { get; set; }
        public int ScopPagesOut { get; set; }
        public int TranscriptionistID { get; set; }
        public DateTime? TransSent { get; set; }
        public DateTime? TransReturned { get; set; }
        public int TransPagesIn { get; set; }
        public int TransPagesOut { get; set; }
        public int EditorID { get; set; }
        public DateTime? EditSent { get; set; }
        public DateTime? EditReturned { get; set; }
        public int EditPagesIn { get; set; }
        public int EditPagesOut { get; set; }
        public int ProoferID { get; set; }
        public DateTime? ProofSent { get; set; }
        public DateTime? ProofReturned { get; set; }
        public int ProofPagesIn { get; set; }
        public int ProofPagesOut { get; set; }
        public int CompletedByUserID { get; set; }
        public DateTime? Completed { get; set; }
        public int CompletedPages { get; set; }
        public string Comments { get; set; }

    }
    [TableName("tjc_rec_event_list")]
    [PrimaryKey("EventID", AutoIncrement = false)]
    public class EventListItem : Event
    {
        public string CourtReporterName { get; set; }
        public string CreatedByName { get; set; }
        public string ScopistName { get; set; }
        public string TranscriptionistName { get; set; }
        public string ProoferName { get; set; }
        public string EditorName { get; set; }
        public string CompletedByName { get; set; }
        public string PresidingJudgeName { get; set; }
        [IgnoreColumn]
        public string HearingDateFormatted
        {
            get
            {
                if (HearingDate.HasValue)
                    return HearingDate.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string ScopSentFormatted
        {
            get
            {
                if (ScopSent.HasValue)
                    return ScopSent.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string ScopReturnedFormatted
        {
            get
            {
                if (ScopReturned.HasValue)
                    return ScopReturned.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string TransSentFormatted
        {
            get
            {
                if (TransSent.HasValue)
                    return TransSent.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string TransReturnedFormatted
        {
            get
            {
                if (TransReturned.HasValue)
                    return TransReturned.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string EditSentFormatted
        {
            get
            {
                if (EditSent.HasValue)
                    return EditSent.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string EditReturnedFormatted
        {
            get
            {
                if (EditReturned.HasValue)
                    return EditReturned.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string ProofSentFormatted
        {
            get
            {
                if (ProofSent.HasValue)
                    return ProofSent.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string ProofReturnedFormatted
        {
            get
            {
                if (ProofReturned.HasValue)
                    return ProofReturned.Value.ToShortDateString();
                return "";
            }
        }
        [IgnoreColumn]
        public string CompletedFormatted
        {
            get
            {
                if (Completed.HasValue)
                    return Completed.Value.ToShortDateString();
                return "";
            }
        }
    }
}
