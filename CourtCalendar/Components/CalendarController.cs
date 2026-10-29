/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// Calendar, week-header and entry data access for the intranet authoring app
    /// (Jud12 connection). Includes strike/unstrike of entries.
    /// </summary>
    public class CalendarController
    {
        private const string CONN_JUD12 = "Jud12";
        private readonly IHostSettings _hostSettings;

        public CalendarController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        // ---- Calendars ----

        public IEnumerable<Calendar> GetCalendars(int divisionId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Calendar>();
                return rep.Find("WHERE DivisionId = @0 ORDER BY Year DESC, Month DESC", divisionId).ToList();
            }
        }

        public Calendar GetCalendar(int calendarId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Calendar>().GetById(calendarId);
            }
        }

        public Calendar GetCalendar(int divisionId, int year, int month)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Calendar>();
                return rep.Find("WHERE DivisionId = @0 AND Year = @1 AND Month = @2", divisionId, year, month).FirstOrDefault();
            }
        }

        public int CreateCalendar(Calendar c)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Calendar>().Insert(c);
            }
            return c.CalendarId;
        }

        public void UpdateCalendar(Calendar c)
        {
            c.LastModifiedOnDate = DateTime.Now;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Calendar>().Update(c);
            }
        }

        public void DeleteCalendar(int calendarId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text, "DELETE FROM tjc_cc_entries WHERE CalendarId = @0", calendarId);
                ctx.Execute(System.Data.CommandType.Text, "DELETE FROM tjc_cc_week_headers WHERE CalendarId = @0", calendarId);
                var rep = ctx.GetRepository<Calendar>();
                var c = rep.GetById(calendarId);
                if (c != null) rep.Delete(c);
            }
        }

        // ---- Week headers ----

        public IEnumerable<WeekHeader> GetWeekHeaders(int calendarId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<WeekHeader>();
                return rep.Find("WHERE CalendarId = @0 ORDER BY WeekStartDate, SortOrder", calendarId).ToList();
            }
        }

        public WeekHeader GetWeekHeader(int weekHeaderId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<WeekHeader>().GetById(weekHeaderId);
            }
        }

        public void SaveWeekHeader(WeekHeader w)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<WeekHeader>();
                if (w.WeekHeaderId > 0) rep.Update(w); else rep.Insert(w);
            }
        }

        public void DeleteWeekHeader(int weekHeaderId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<WeekHeader>();
                var w = rep.GetById(weekHeaderId);
                if (w != null) rep.Delete(w);
            }
        }

        // ---- Entries ----

        public IEnumerable<CalendarEntry> GetEntries(int calendarId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<CalendarEntry>();
                return rep.Find("WHERE CalendarId = @0 ORDER BY EntryDate, SortOrder, Timespan", calendarId).ToList();
            }
        }

        public CalendarEntry GetEntry(int entryId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<CalendarEntry>().GetById(entryId);
            }
        }

        public void SaveEntry(CalendarEntry e)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<CalendarEntry>();
                if (e.EntryId > 0) rep.Update(e); else rep.Insert(e);
            }
        }

        public void DeleteEntry(int entryId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<CalendarEntry>();
                var e = rep.GetById(entryId);
                if (e != null) rep.Delete(e);
            }
        }

        /// <summary>Toggles the stricken state of an entry; returns the new state.</summary>
        public bool SetStricken(int entryId, bool stricken, int userId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<CalendarEntry>();
                var e = rep.GetById(entryId);
                if (e == null) return false;
                e.IsStricken = stricken;
                e.StrickenByUserId = stricken ? userId : 0;
                e.StrickenOnDate = stricken ? (DateTime?)DateTime.Now : null;
                rep.Update(e);
                return stricken;
            }
        }
    }
}
