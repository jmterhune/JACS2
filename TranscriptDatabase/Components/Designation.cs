using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_designation")]
    [PrimaryKey("DesignationID", AutoIncrement = true)]
    [Cacheable("Designations", CacheItemPriority.Default, 20)]
    public class Designation : EntityBase
    {
        public int DesignationID { get; set; }

        public string dLastName { get; set; }

        public string dFirstName { get; set; }

        public string dMiddleName { get; set; }

        public string County { get; set; }

        public string LowerTribunalCaseNumber { get; set; }

        public string AppellateCaseNumber { get; set; }

        public int AttorneyID { get; set; }

        public DateTime? ServiceDate { get; set; }

        public DateTime? ReceiptDate { get; set; }

        public DateTime? DueDate { get; set; }

        public bool PublicDefenderAppointed { get; set; }

        public bool DeclaredIndigent { get; set; }

        public bool CourtAppointedCounsel { get; set; }

        public DateTime? TranscriptFiled { get; set; }

        public bool AcknowledgmentFiled { get; set; }

        public string Comment { get; set; }

        public int? Attorney2ID { get; set; }

        public int? Attorney3ID { get; set; }

        public int? Attorney4ID { get; set; }

        public bool Archived { get; set; }
    }
    [TableName("tjc_rec_designation_list")]
    [PrimaryKey("DesignationID", AutoIncrement = true)]
    public class DesignationListItem : Designation
    {
        public string CreatedByUsername { get; set; }
        public string CaseNumber { get; set; }
    }
}