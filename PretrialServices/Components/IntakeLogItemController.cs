using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.PretrialServices.Components
{
    internal class IntakeLogItemController
    {
        public void CreateIntakeLogItem(IntakeLogItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IntakeLogItem>();
                rep.Insert(t);
            }
        }

        public void DeleteIntakeLogItem(long logId)
        {
            var t = GetIntakeLogItem(logId);
            DeleteIntakeLogItem(t);
        }

        public void DeleteIntakeLogItem(IntakeLogItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IntakeLogItem>();
                rep.Delete(t);
            }
        }

        public IEnumerable<IntakeLogItem> GetIntakeLogItems()
        {
            IEnumerable<IntakeLogItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IntakeLogItem>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<IntakeLogItem> GetIntakeLogItemsByCounty(int countyId)
        {
            IEnumerable<IntakeLogItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IntakeLogItem>();
                t = rep.Find("Where CountyId = @0",countyId);
            }
            return t;
        }
        public IntakeLogItem GetIntakeLogItemByDate( DateTime intakeDate)
        {
            IntakeLogItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IntakeLogItem>();
                t = rep.Find("Where IntakeDate = @0", intakeDate).FirstOrDefault();
            }
            return t;
        }
        public IntakeLogItem GetIntakeLogItemByCountyAndDate(int countyId,DateTime intakeDate)
        {
            IntakeLogItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IntakeLogItem>();
                t = rep.Find("Where CountyId = @0 And IntakeDate = @1",countyId, intakeDate).FirstOrDefault();
            }
            return t;
        }
        public IntakeLogItem GetIntakeLogItem(long logId)
        {
            IntakeLogItem t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IntakeLogItem>();
                t = rep.GetById(logId);
            }
            return t;
        }

        public void UpdateIntakeLogItem(IntakeLogItem t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IntakeLogItem>();
                rep.Update(t);
            }
        }

    }
}
