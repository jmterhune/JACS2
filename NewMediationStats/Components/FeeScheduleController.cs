using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.NewMediationStats.Components
{
    internal class FeeScheduleController
    {
        private readonly IHostSettings _hostSettings;

        public FeeScheduleController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<FeeSchedule> GetSchedules()
        {
            IEnumerable<FeeSchedule> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<FeeSchedule>();
                t = rep.Get();
            }
            // Effective schedules oldest first, schedules not yet in effect last.
            return t.OrderBy(s => s.EffectiveDate.HasValue ? 0 : 1).ThenBy(s => s.EffectiveDate).ThenBy(s => s.FeeScheduleId).ToList();
        }

        public FeeSchedule GetSchedule(int feeScheduleId)
        {
            if (feeScheduleId <= 0)
                return null;
            FeeSchedule t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<FeeSchedule>();
                t = rep.GetById(feeScheduleId);
            }
            return t;
        }

        public void UpdateSchedule(FeeSchedule t)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<FeeSchedule>();
                rep.Update(t);
            }
        }

        /// <summary>The schedule in effect on a date, or 0 if none is.</summary>
        public int GetScheduleIdForDate(DateTime date)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                return ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT ISNULL(dbo.tjc_modules_med_fee_schedule_id(@0), 0)", date);
            }
        }

        public IEnumerable<FeeTier> GetTiers(int feeScheduleId, int groupId, string tierType)
        {
            IEnumerable<FeeTier> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<FeeTier>();
                t = rep.Find("WHERE FeeScheduleId = @0 AND GroupId = @1 AND TierType = @2 AND Active = 1 ORDER BY SortOrder, FeeTierId", feeScheduleId, groupId, tierType);
            }
            return t.ToList();
        }

        public IEnumerable<FeeTier> GetAllTiers()
        {
            IEnumerable<FeeTier> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                var rep = ctx.GetRepository<FeeTier>();
                t = rep.Find("ORDER BY FeeScheduleId, GroupId DESC, TierType, SortOrder, FeeTierId");
            }
            return t.ToList();
        }

        public IEnumerable<FeeTierReportRow> GetFeeTierReport(DateTime startDate, DateTime endDate)
        {
            IEnumerable<FeeTierReportRow> t;
            using (IDataContext ctx = DataContext.Instance(_hostSettings))
            {
                t = ctx.ExecuteQuery<FeeTierReportRow>(System.Data.CommandType.StoredProcedure, "tjc_modules_med_fee_tier_report", startDate, endDate);
            }
            return t.ToList();
        }
    }
}
