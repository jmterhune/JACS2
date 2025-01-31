
using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_event")]
    [PrimaryKey("EventID", AutoIncrement = true)]
    [Cacheable("Events", CacheItemPriority.Default, 20)]
    internal class Event:EntityBase
    {
        public int EventID { get; set; }

        public int? DesignationID { get; set; }

        public DateTime? HearingDate { get; set; }

        public string PresidingJudge { get; set; }

        public string HearingType { get; set; }

        public int? CourtReporterID { get; set; }

        public int? Pages { get; set; }

        public int? DaysUntilComplete { get; set; }

        public string ScopName { get; set; }

        public DateTime? ScopSent { get; set; }

        public DateTime? ScopReturned { get; set; }

        public int? ScopPagesIn { get; set; }

        public int? ScopPagesOut { get; set; }

        public string TransName { get; set; }

        public DateTime? TransSent { get; set; }

        public DateTime? TransReturned { get; set; }

        public int? TransPagesIn { get; set; }

        public int? TransPagesOut { get; set; }

        public string EditName { get; set; }

        public DateTime? EditSent { get; set; }

        public DateTime? EditReturned { get; set; }

        public int? EditPagesIn { get; set; }

        public int? EditPagesOut { get; set; }

        public string ProofName { get; set; }

        public DateTime? ProofSent { get; set; }

        public DateTime? ProofReturned { get; set; }

        public int? ProofPagesIn { get; set; }

        public int? ProofPagesOut { get; set; }

        public DateTime? Completed { get; set; }

        public int? CompletedPages { get; set; }

        public string Comments { get; set; }
    }
}
