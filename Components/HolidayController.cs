using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.jacs.Components
{
    internal class HolidayController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateHoliday(Holiday t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Holiday>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }

        public void DeleteHoliday(long holidayId)
        {
            var t = GetHoliday(holidayId);
            DeleteHoliday(t);
        }

        public void DeleteHoliday(Holiday t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Holiday>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Holiday> GetHolidays()
        {
            IEnumerable<Holiday> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Holiday>();
                t = rep.Get();
            }
            return t;
        }

        public Holiday GetHoliday(long holidayId)
        {
            Holiday t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Holiday>();
                t = rep.GetById(holidayId);
            }
            return t;
        }

        public void UpdateHoliday(Holiday t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Holiday>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }

        public IEnumerable<Holiday> GetHolidaysPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<Holiday>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_holiday_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetHolidaysCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_holiday_count",
                    searchTerm ?? string.Empty
                );
            }
        }
    }
}