using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace tjc.Modules.Transcriptionist.Components
{
    // Not [Cacheable]: the rows are written by the jud12 site too, and a cache here would keep showing
    // applications as they were when this site last read them.
    [TableName("tjc_cat_applications")]
    [PrimaryKey("TranscriptionistID", AutoIncrement = true)]
    internal class Application
    {
        public long TranscriptionistID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string HomeAddress { get; set; }
        public string County { get; set; }
        public string OfficePhone { get; set; }
        public string ResidencePhone { get; set; }
        public string CellPhone { get; set; }
        public string TollFreePhone { get; set; }
        public string Employer { get; set; }
        public string EmployerAddress { get; set; }
        public string Website { get; set; }
        public string SendMailLocation { get; set; }
        public string EmailAddress { get; set; }
        public string TranscriptionTraining { get; set; }
        public string Certifications { get; set; }
        public int? MonthsExperience { get; set; }
        public string ExperienceType { get; set; }
        public string ExpeditedTranscriptAbility { get; set; }
        public string SoftwareExperience { get; set; }
        public string TurnAroundTime { get; set; }
        public string RuleFamiliarity { get; set; }
        public bool? JacsVendor { get; set; }
        public bool? ShowInternet { get; set; }
        public string Signature { get; set; }
        public DateTime? PacketMailedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? ApprovedByUserID { get; set; }
        public DateTime? RejectedDate { get; set; }
        public int? RejectedByUserID { get; set; }
        public string RejectionReason { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserID { get; set; }

        [IgnoreColumn]
        public string Status
        {
            get
            {
                if (RejectedDate.HasValue) return "Rejected";
                if (ApprovedDate.HasValue) return "Approved";
                return "Pending";
            }
        }

        [IgnoreColumn]
        public string FullName
        {
            get
            {
                var nameParts = new[] { LastName?.Trim(), FirstName?.Trim(), MiddleName?.Trim() }
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
                if (nameParts.Count == 0) return string.Empty;
                string rest = string.Join(" ", nameParts.Skip(1));
                return string.IsNullOrEmpty(rest) ? nameParts[0] : $"{nameParts[0]}, {rest}";
            }
        }
    }
}
