using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class HolidayController
    {
        public void CreateHoliday(Holiday t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                rep.Insert(t);
            }
        }
        public void DeleteHoliday(int holidayId)
        {
            var t = GetHoliday(holidayId);
            DeleteHoliday(t);
        }
        public void DeleteHoliday(Holiday t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Holiday> GetHolidays()
        {
            IEnumerable<Holiday> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                t = rep.Get();
            }
            return t;
        }
        public Holiday GetHoliday(int holidayId)
        {
            Holiday t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                t = rep.GetById(holidayId);
            }
            return t;
        }
        public void UpdateHoliday(Holiday t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Holiday>();
                rep.Update(t);
            }
        }
    }
}