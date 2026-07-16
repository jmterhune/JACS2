
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
        public int EventID { get; set; }  // int
        public int DesignationID { get; set; }  // int
        public DateTime? HearingDate { get; set; }  // smalldatetime
        public int PresidingJudgeID { get; set; }  // int
        public string HearingType { get; set; }  // nvarchar(50)
        public int CourtReporterID { get; set; }  // int
        public int Pages { get; set; }  // int
        public int DaysUntilComplete { get; set; }  // int
        public int ScopistID { get; set; }  // int
        public DateTime? ScopSent { get; set; }  // smalldatetime
        public DateTime? ScopReturned { get; set; }  // smalldatetime
        public int ScopPagesIn { get; set; }  // int
        public int ScopPagesOut { get; set; }  // int
        public int TranscriptionistID { get; set; }  // int
        public DateTime? TransSent { get; set; }  // smalldatetime
        public DateTime? TransReturned { get; set; }  // smalldatetime
        public int TransPagesIn { get; set; }  // int
        public int TransPagesOut { get; set; }  // int
        public int EditorID { get; set; }  // int
        public DateTime? EditSent { get; set; }  // smalldatetime
        public DateTime? EditReturned { get; set; }  // smalldatetime
        public int EditPagesIn { get; set; }  // int
        public int EditPagesOut { get; set; }  // int
        public int ProoferID { get; set; }  // int
        public DateTime? ProofSent { get; set; }  // smalldatetime
        public DateTime? ProofReturned { get; set; }  // smalldatetime
        public int ProofPagesIn { get; set; }  // int
        public int ProofPagesOut { get; set; }  // int
        public int CompletedByUserID { get; set; }  // int
        public DateTime? Completed { get; set; }  // smalldatetime
        public int CompletedPages { get; set; }  // int
        public string Comments { get; set; }  // nvarchar(MAX)

    }
    [TableName("tjc_rec_event_list")]
    [PrimaryKey("EventID", AutoIncrement = false)]
    public class EventListItem : Event
    {
        public string CourtReporterName { get; set; }  // nvarchar(102)
        public string CreatedByName { get; set; }  // nvarchar(128)
        public string ScopistName { get; set; }  // nvarchar(152)
        public string TranscriptionistName { get; set; }  // nvarchar(152)
        public string ProoferName { get; set; }  // nvarchar(152)
        public string EditorName { get; set; }  // nvarchar(152)
        public string CompletedByName { get; set; }  // nvarchar(128)
        public string PresidingJudgeName { get; set; }  // nvarchar(152)
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
