using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.NewMediationStats.Components
{
    /// <summary>
    /// A set of fee amounts and income tiers that applies from an effective date. The schedule that applies
    /// to a session is the latest one whose effective date is on or before the session's mediation date.
    /// A schedule with no effective date is not yet in effect.
    /// </summary>
    [TableName("tjc_modules_med_fee_schedules")]
    [PrimaryKey("FeeScheduleId", AutoIncrement = true)]
    internal class FeeSchedule : EntityBase
    {
        public int FeeScheduleId { get; set; }  // int (identity PK)

        public string Name { get; set; }  // nvarchar(100)

        public DateTime? EffectiveDate { get; set; }  // date, NULL = not yet in effect

        public string Notes { get; set; }  // nvarchar(500)
    }
}
