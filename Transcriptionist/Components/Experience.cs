using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.Transcriptionist.Components
{
    [TableName("tjc_cat_experience")]
    [PrimaryKey("ExperienceID", AutoIncrement = true)]
    internal class Experience
    {
        public long ExperienceID { get; set; }
        public long? TranscriptionistID { get; set; }
        public string CourtName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProceedingTypes { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
    }
}
