using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.JudicialReferral.Components.Models
{
    public enum Statuses
    {
        // 4-state workflow. Value 2 (legacy "Pending") is retired but reserved so
        // existing DB rows aren't silently remapped to another state.
        NewReferral = 1,
        ReferredToCounsel = 3,
        RetainedByJudge = 4,
        Completed = 5
    }

    public enum DivisionType
    {
        Criminal = 0,
        Civil = 1,
        Family = 2,
        Appeals = 3
    }

    [TableName("tjc_jr_referrals")]
    [PrimaryKey("ReferralId", AutoIncrement = true)]
    public class JudgeReferralInfo
    {
        public int ReferralId { get; set; }  // int (ReferralID)
        public int JaID { get; set; }  // int
        public int JudgeId { get; set; }  // int (JudgeID)
        public DateTime? JaCreatedDate { get; set; }  // date
        public DateTime? JudgeResponseDate { get; set; }  // date
        public DateTime? CounselReceivedDate { get; set; }  // date
        public int Status { get; set; }  // int
        public string CaseParties { get; set; }  // nvarchar(2000)
        public string CaseNumber { get; set; }  // nvarchar(25)
        public string MotionTitle { get; set; }  // nvarchar(50)
        public DateTime? MotionDate { get; set; }  // date
        public DateTime? RequestedCompletionDate { get; set; }  // date
        public bool CounselAssistance { get; set; }  // bit
        public string JudgeMotions { get; set; }  // nvarchar(2000)
        public int SelectedDivision { get; set; }  // int

        // Criminal
        public bool StatusOrderCriminal { get; set; }  // bit
        public DateTime? StatusOrderCriminalFiled { get; set; }  // date
        public bool MotionVacateCriminal { get; set; }  // bit
        public bool MotionCorrectCriminal { get; set; }  // bit
        public DateTime? MotionCorrectCriminalFiled { get; set; }  // date
        public bool MotionDirectedCriminal { get; set; }  // bit
        public string DirectedMotionsCriminal { get; set; }  // nvarchar(2000)
        public bool OtherMotionCriminal { get; set; }  // bit
        public string OtherMotionCriminalText { get; set; }  // nvarchar(50)
        public bool PretrialMotionCriminal { get; set; }  // bit
        public string PretrialMotionCriminalText { get; set; }  // nvarchar(50)
        public bool ResearchCriminal { get; set; }  // bit
        public string ResearchCriminalText { get; set; }  // nvarchar(50)

        // Civil
        public bool MotionDismissCivil { get; set; }  // bit
        public bool MotionSummaryJudgementCivil { get; set; }  // bit
        public bool MotionDiscoveryCivil { get; set; }  // bit
        public bool MotionAttorneyFeeCivil { get; set; }  // bit
        public bool OtherMotionCivil { get; set; }  // bit
        public string OtherMotionCivilText { get; set; }  // nvarchar(50)
        public bool ResearchMotionCivil { get; set; }  // bit
        public string ResearchMotionCivilText { get; set; }  // nvarchar(50)

        // Family
        public bool PetitionTimeShareFamily { get; set; }  // bit
        public bool PetitionChildSupportFamily { get; set; }  // bit
        public bool MotionDiscoveryFamily { get; set; }  // bit
        public bool MotionAttorneyFeeFamily { get; set; }  // bit
        public bool OtherMotionFamily { get; set; }  // bit
        public string OtherMotionFamilyText { get; set; }  // nvarchar(50)
        public bool ResearchMotionFamily { get; set; }  // bit
        public string ResearchMotionFamilyText { get; set; }  // nvarchar(50)

        // Appeals
        public string TypeOfAppeal { get; set; }  // nvarchar(50)

        [IgnoreColumn]
        public string JudgeName { get; set; }

        [IgnoreColumn]
        public string StatusName
        {
            // Labels match the production DLL's 5-state workflow.
            get
            {
                switch (Status)
                {
                    case 1: return "New";
                    case 3: return "Referred to Court Counsel";
                    case 4: return "Retained by Judge";
                    case 5: return "Completed";
                    default: return string.Empty;
                }
            }
        }
    }
}
