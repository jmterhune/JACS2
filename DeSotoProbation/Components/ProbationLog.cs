using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_desoto_probation_log")]
    //setup the primary key for table
    [PrimaryKey("RecordID", AutoIncrement = true)]
    internal class ProbationLog
    {
        public long RecordID { get; set; }
        public int? Month { get; set; }
        public string CaseNumber { get; set; }
        public int? AddressID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? EmailID { get; set; }
        public int? PhonePrimaryID { get; set; }
        public int? PhoneSecondaryID { get; set; }
        public string SourceOfIncome { get; set; }
        public decimal PaymentAmount { get; set; }
        public string ReasonNoPayment { get; set; }
        public decimal PaymentPlanBalance { get; set; }
        public bool? PaidMonthlyFees { get; set; }
        public string ReasonNoMonthlyFees { get; set; }
        public decimal PtiFeeBalance { get; set; }
        public string ReasonNoClassEnrollment { get; set; }
        public string ClassesCompleted { get; set; }
        public bool? WorkProgramRequired { get; set; }
        public int? WorkProgramDaysOrdered { get; set; }
        public int? WorkProgramDaysCompleted { get; set; }
        public bool? CommunityServiceRequired { get; set; }
        public int? CommunityServiceHoursOrdered { get; set; }
        public int? CommunityServiceHoursCompleted { get; set; }
        public string CommunityServiceLocation { get; set; }
        public string CommunityServiceReasonNoStart { get; set; }
        public bool? LeoContact { get; set; }
        public string LeoContactDates { get; set; }
        public string LeoCountyArrested { get; set; }
        public string LeoCharges { get; set; }
        public DateTime? ProbateCompletionDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedBy { get; set; }
    }
}