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

        /// <summary>
        /// Copies an existing entry onto the following day and returns the new
        /// entry, or null when the source is missing. The copy is never stricken.
        /// </summary>
        public CalendarEntry CopyEntryToNextDay(int entryId, int userId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<CalendarEntry>();
                var src = rep.GetById(entryId);
                if (src == null) return null;
                var copy = CloneForDate(src, src.EntryDate.AddDays(1), userId);
                rep.Insert(copy);
                return copy;
            }
        }

        /// <summary>
        /// Inserts a copy of the template entry on each of the given dates (used to
        /// replicate one entry across a day range). Returns the number inserted.
        /// </summary>
        public int ReplicateEntry(CalendarEntry template, IEnumerable<DateTime> dates, int userId)
        {
            if (template == null || dates == null) return 0;
            int count = 0;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<CalendarEntry>();
                foreach (var d in dates)
                {
                    rep.Insert(CloneForDate(template, d.Date, userId));
                    count++;
                }
            }
            return count;
        }

        private static CalendarEntry CloneForDate(CalendarEntry src, DateTime date, int userId)
        {
            return new CalendarEntry
            {
                CalendarId = src.CalendarId,
                EntryDate = date.Date,
                Timespan = src.Timespan,
                AllDay = src.AllDay,
                ShortTitle = src.ShortTitle,
                TitleBold = src.TitleBold,
                TitleItalic = src.TitleItalic,
                Detail = src.Detail,
                CategoryId = src.CategoryId,
                SortOrder = src.SortOrder,
                IsStricken = false,
                StrickenByUserId = 0,
                StrickenOnDate = null,
                CreatedByUserId = userId,
                CreatedOnDate = DateTime.Now,
            };
        }

        /// <summary>
        /// Moves an entry to a different date and position (drag-and-drop in the grid
        /// view). Only the date and order change. Returns the updated entry or null.
        /// </summary>
        public CalendarEntry MoveEntry(int entryId, DateTime newDate, int newSortOrder)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<CalendarEntry>();
                var en = rep.GetById(entryId);
                if (en == null) return null;
                en.EntryDate = newDate.Date;
                en.SortOrder = newSortOrder;
                rep.Update(en);
                return en;
            }
        }

        /// <summary>
        /// Sets SortOrder on the given entries to match their position in the list,
        /// after confirming each belongs to the calendar and date (grid reorder).
        /// </summary>
        public void ReorderDay(int calendarId, DateTime date, IList<int> orderedEntryIds)
        {
            if (orderedEntryIds == null || orderedEntryIds.Count == 0) return;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<CalendarEntry>();
                for (int i = 0; i < orderedEntryIds.Count; i++)
                {
                    var en = rep.GetById(orderedEntryIds[i]);
                    if (en == null || en.CalendarId != calendarId || en.EntryDate.Date != date.Date) continue;
                    if (en.SortOrder == i) continue;
                    en.SortOrder = i;
                    rep.Update(en);
                }
            }
        }

        /// <summary>
        /// Copies an entry onto another date, changing only the date (Shift+drag in the
        /// grid). Everything else - time, title, styling, detail, category, stricken
        /// state - is preserved. Returns the new entry or null when the source is gone.
        /// </summary>
        public CalendarEntry CopyEntryToDate(int entryId, DateTime date, int newSortOrder, int userId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<CalendarEntry>();
                var src = rep.GetById(entryId);
                if (src == null) return null;
                var copy = new CalendarEntry
                {
                    CalendarId = src.CalendarId,
                    EntryDate = date.Date,
                    Timespan = src.Timespan,
                    AllDay = src.AllDay,
                    ShortTitle = src.ShortTitle,
                    TitleBold = src.TitleBold,
                    TitleItalic = src.TitleItalic,
                    Detail = src.Detail,
                    CategoryId = src.CategoryId,
                    SortOrder = newSortOrder,
                    IsStricken = src.IsStricken,
                    StrickenByUserId = src.StrickenByUserId,
                    StrickenOnDate = src.StrickenOnDate,
                    CreatedByUserId = userId,
                    CreatedOnDate = DateTime.Now,
                };
                rep.Insert(copy);
                return copy;
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
