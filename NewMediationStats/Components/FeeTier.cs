using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.NewMediationStats.Components
{
    /// <summary>The kinds of tier a fee schedule defines. Stored in FeeTier.TierType.</summary>
    internal static class FeeTierTypes
    {
        /// <summary>The session's single "Fee Range" / "Fee Amount" choice (combined income or county fee).</summary>
        public const string FeeAmount = "FeeAmount";
        /// <summary>Each party's own income range (revised family schedule).</summary>
        public const string PartyIncome = "PartyIncome";
        public const string FeePaid = "FeePaid";
        public const string FeeOwed = "FeeOwed";
    }

    /// <summary>Group ids the fee schedules are defined for. These are the ids the report procedures use.</summary>
    internal static class FeeGroups
    {
        public const int County = 2;
        public const int Family = 4;
    }

    [TableName("tjc_modules_med_fee_tiers")]
    [PrimaryKey("FeeTierId", AutoIncrement = true)]
    internal class FeeTier
    {
        public int FeeTierId { get; set; }  // int (identity PK)

        public int FeeScheduleId { get; set; }  // int

        public int GroupId { get; set; }  // int

        public string TierType { get; set; }  // nvarchar(20)

        /// <summary>The text stored on the session (for example "$140" or "$50K-$70K").</summary>
        public string Value { get; set; }  // nvarchar(50)

        /// <summary>The text shown to the user.</summary>
        public string Label { get; set; }  // nvarchar(100)

        public decimal? Amount { get; set; }  // decimal(9,2)

        public int SortOrder { get; set; }  // int

        public bool Active { get; set; }  // bit
    }
}
