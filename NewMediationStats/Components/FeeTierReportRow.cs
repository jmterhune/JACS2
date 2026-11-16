using System;

namespace tjc.Modules.NewMediationStats.Components
{
    /// <summary>
    /// One row of the Fee Tiers report: a fee value within a schedule and case group, with how many parties
    /// paid it, were owed it for held mediations, and were owed it after a failure to appear.
    /// </summary>
    public class FeeTierReportRow
    {
        public int FeeScheduleId { get; set; }

        public string ScheduleName { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int GroupId { get; set; }

        public string CaseTypeGroup { get; set; }

        public string FeeValue { get; set; }

        /// <summary>Dollar amount of the tier, or null when the value is not a tier of that schedule (for example an old-style amount entered after a new schedule took effect).</summary>
        public decimal? Amount { get; set; }

        public int PaidCount { get; set; }

        public int OwedHeldCount { get; set; }

        public int OwedFtaCount { get; set; }

        public int SortOrder { get; set; }

        public decimal? PaidTotal { get { return Amount.HasValue ? Amount.Value * PaidCount : (decimal?)null; } }

        public decimal? OwedHeldTotal { get { return Amount.HasValue ? Amount.Value * OwedHeldCount : (decimal?)null; } }

        public decimal? OwedFtaTotal { get { return Amount.HasValue ? Amount.Value * OwedFtaCount : (decimal?)null; } }

        /// <summary>True when the fee value is not part of the schedule it was reported under.</summary>
        public bool NotInSchedule { get { return SortOrder >= 99; } }

        public string Note { get { return NotInSchedule ? "not in schedule" : string.Empty; } }
    }
}
