using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.JudicialReferral.Components.Models
{
    public enum Statuses
    {
        NewReferral = 1,
        MotionSet = 2,
        ReferredToCounsel = 3,
        RetainedByJudge = 4,
        ReceivedAssigned = 5,
        Completed = 6
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
        public int ReferralId { get; set; }
        public int JaID { get; set; }
        public int JudgeId { get; set; }
        public DateTime? JaCreatedDate { get; set; }
        public DateTime? JudgeResponseDate { get; set; }
        public DateTime? CounselReceivedDate { get; set; }
        public int Status { get; set; }
        public string CaseParties { get; set; }
        public string CaseNumber { get; set; }
        public string MotionTitle { get; set; }
        public DateTime? MotionDate { get; set; }
        public DateTime? RequestedCompletionDate { get; set; }
        public bool CounselAssistance { get; set; }
        public string JudgeMotions { get; set; }
        public int SelectedDivision { get; set; }

        // Criminal
        public bool StatusOrderCriminal { get; set; }
        public DateTime? StatusOrderCriminalFiled { get; set; }
        public bool MotionVacateCriminal { get; set; }
        public bool MotionCorrectCriminal { get; set; }
        public DateTime? MotionCorrectCriminalFiled { get; set; }
        public bool MotionDirectedCriminal { get; set; }
        public string DirectedMotionsCriminal { get; set; }
        public bool OtherMotionCriminal { get; set; }
        public string OtherMotionCriminalText { get; set; }
        public bool PretrialMotionCriminal { get; set; }
        public string PretrialMotionCriminalText { get; set; }
        public bool ResearchCriminal { get; set; }
        public string ResearchCriminalText { get; set; }

        // Civil
        public bool MotionDismissCivil { get; set; }
        public bool MotionSummaryJudgementCivil { get; set; }
        public bool MotionDiscoveryCivil { get; set; }
        public bool MotionAttorneyFeeCivil { get; set; }
        public bool OtherMotionCivil { get; set; }
        public string OtherMotionCivilText { get; set; }
        public bool ResearchMotionCivil { get; set; }
        public string ResearchMotionCivilText { get; set; }

        // Family
        public bool PetitionTimeShareFamily { get; set; }
        public bool PetitionChildSupportFamily { get; set; }
        public bool MotionDiscoveryFamily { get; set; }
        public bool MotionAttorneyFeeFamily { get; set; }
        public bool OtherMotionFamily { get; set; }
        public string OtherMotionFamilyText { get; set; }
        public bool ResearchMotionFamily { get; set; }
        public string ResearchMotionFamilyText { get; set; }

        // Appeals
        public string TypeOfAppeal { get; set; }

        [IgnoreColumn]
        public string JudgeName { get; set; }

        [IgnoreColumn]
        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case 1: return "New";
                    case 2: return "Motion Type Set";
                    case 3: return "Referred to Court Counsel";
                    case 4: return "Retained by Judge";
                    case 5: return "Received & Assigned";
                    case 6: return "Complete";
                    default: return string.Empty;
                }
            }
        }
    }
}
