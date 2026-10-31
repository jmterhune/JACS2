/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.CourtCalendar.Components;

namespace tjc.Modules.CourtCalendar.Services
{
    /// <summary>
    /// Intranet-only Web API behind the drag-and-drop calendar grid. Every action
    /// requires that the signed-in user may manage the entry's division (module admin,
    /// or the division's assigned judge/JA). Writes go to the public (jud12) database
    /// through the CalendarController's "Jud12" connection.
    /// </summary>
    public class CalendarAdminController : DnnApiController
    {
        private IHostSettings HostSettings =>
            System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();

        private CalendarController Calendars => new CalendarController(HostSettings);
        private DivisionController Divisions => new DivisionController(HostSettings);

        // ---- Actions ----

        [HttpGet]
        [DnnAuthorize]
        public HttpResponseMessage Entries(int id)
        {
            return Guarded(() =>
            {
                if (id <= 0) return Bad("Missing calendar.");
                if (!CanManage(DivisionForCalendar(id))) return Denied();
                var list = Calendars.GetEntries(id).Select(EntryJson).ToList();
                return Ok(list);
            });
        }

        [HttpPost]
        [DnnAuthorize]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage AddEntry(EntryDto dto)
        {
            return Guarded(() =>
            {
                if (dto == null || dto.CalendarId <= 0) return Bad("Missing calendar.");
                var division = DivisionForCalendar(dto.CalendarId);
                if (!CanManage(division)) return Denied();

                var en = new CalendarEntry
                {
                    CalendarId = dto.CalendarId,
                    CreatedByUserId = UserInfo.UserID,
                    CreatedOnDate = DateTime.Now,
                };
                Apply(dto, en);
                Calendars.SaveEntry(en);
                return Ok(EntryJson(en));
            });
        }

        [HttpPost]
        [DnnAuthorize]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage SaveEntry(EntryDto dto)
        {
            return Guarded(() =>
            {
                if (dto == null || dto.EntryId <= 0) return Bad("Missing entry.");
                var en = Calendars.GetEntry(dto.EntryId);
                if (en == null) return Bad("Entry not found.");
                if (!CanManage(DivisionForCalendar(en.CalendarId))) return Denied();

                var order = en.SortOrder;   // editing an entry must not change its position
                Apply(dto, en);
                en.SortOrder = order;
                Calendars.SaveEntry(en);
                return Ok(EntryJson(en));
            });
        }

        [HttpPost]
        [DnnAuthorize]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage MoveEntry(MoveDto dto)
        {
            return Guarded(() =>
            {
                if (dto == null || dto.EntryId <= 0) return Bad("Missing entry.");
                DateTime date;
                if (!TryDate(dto.Date, out date)) return Bad("Bad date.");
                var en = Calendars.GetEntry(dto.EntryId);
                if (en == null) return Bad("Entry not found.");
                if (!CanManage(DivisionForCalendar(en.CalendarId))) return Denied();

                var moved = Calendars.MoveEntry(dto.EntryId, date, dto.SortOrder);
                return Ok(EntryJson(moved));
            });
        }

        [HttpPost]
        [DnnAuthorize]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CopyEntry(MoveDto dto)
        {
            return Guarded(() =>
            {
                if (dto == null || dto.EntryId <= 0) return Bad("Missing entry.");
                DateTime date;
                if (!TryDate(dto.Date, out date)) return Bad("Bad date.");
                var en = Calendars.GetEntry(dto.EntryId);
                if (en == null) return Bad("Entry not found.");
                if (!CanManage(DivisionForCalendar(en.CalendarId))) return Denied();

                var copy = Calendars.CopyEntryToDate(dto.EntryId, date, dto.SortOrder, UserInfo.UserID);
                return Ok(EntryJson(copy));
            });
        }

        [HttpPost]
        [DnnAuthorize]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage ReorderDay(ReorderDto dto)
        {
            return Guarded(() =>
            {
                if (dto == null || dto.CalendarId <= 0) return Bad("Missing calendar.");
                DateTime date;
                if (!TryDate(dto.Date, out date)) return Bad("Bad date.");
                if (!CanManage(DivisionForCalendar(dto.CalendarId))) return Denied();

                Calendars.ReorderDay(dto.CalendarId, date, dto.EntryIds ?? new List<int>());
                return Ok(new { ok = true });
            });
        }

        [HttpPost]
        [DnnAuthorize]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage DeleteEntry(IdDto dto)
        {
            return Guarded(() =>
            {
                if (dto == null || dto.EntryId <= 0) return Bad("Missing entry.");
                var en = Calendars.GetEntry(dto.EntryId);
                if (en == null) return Ok(new { ok = true });
                if (!CanManage(DivisionForCalendar(en.CalendarId))) return Denied();

                Calendars.DeleteEntry(dto.EntryId);
                return Ok(new { ok = true });
            });
        }

        [HttpPost]
        [DnnAuthorize]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage SetStricken(StrikeDto dto)
        {
            return Guarded(() =>
            {
                if (dto == null || dto.EntryId <= 0) return Bad("Missing entry.");
                var en = Calendars.GetEntry(dto.EntryId);
                if (en == null) return Bad("Entry not found.");
                if (!CanManage(DivisionForCalendar(en.CalendarId))) return Denied();

                var state = Calendars.SetStricken(dto.EntryId, dto.Stricken, UserInfo.UserID);
                return Ok(new { isStricken = state });
            });
        }

        // ---- Helpers ----

        private void Apply(EntryDto dto, CalendarEntry en)
        {
            DateTime date;
            if (TryDate(dto.Date, out date)) en.EntryDate = date;
            en.AllDay = dto.AllDay;
            en.Timespan = dto.AllDay ? string.Empty : (dto.Timespan ?? string.Empty).Trim();
            en.ShortTitle = (dto.ShortTitle ?? string.Empty).Trim();
            en.TitleBold = dto.TitleBold;
            en.TitleItalic = dto.TitleItalic;
            en.Detail = (dto.Detail ?? string.Empty).Trim();
            en.CategoryId = dto.CategoryId;
            en.SortOrder = dto.SortOrder;
        }

        private Division DivisionForCalendar(int calendarId)
        {
            var cal = Calendars.GetCalendar(calendarId);
            return cal == null ? null : Divisions.GetDivision(cal.DivisionId);
        }

        private bool CanManage(Division division)
        {
            if (division == null) return false;
            if (ActiveModule != null && ModulePermissionController.CanEditModuleContent(ActiveModule)) return true;
            var judge = new JudgeController(HostSettings).GetJudge(division.JudgeId);
            return judge != null && (judge.JudgeUserId == UserInfo.UserID || judge.JaUserId == UserInfo.UserID);
        }

        private static bool TryDate(string s, out DateTime date)
        {
            return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        private static object EntryJson(CalendarEntry e)
        {
            if (e == null) return null;
            return new
            {
                entryId = e.EntryId,
                calendarId = e.CalendarId,
                entryDate = e.EntryDate.ToString("yyyy-MM-dd"),
                timespan = e.Timespan ?? string.Empty,
                allDay = e.AllDay,
                shortTitle = e.ShortTitle ?? string.Empty,
                titleBold = e.TitleBold,
                titleItalic = e.TitleItalic,
                detail = e.Detail ?? string.Empty,
                categoryId = e.CategoryId,
                sortOrder = e.SortOrder,
                isStricken = e.IsStricken,
            };
        }

        private HttpResponseMessage Ok(object body) => Request.CreateResponse(HttpStatusCode.OK, body);
        private HttpResponseMessage Bad(string msg) => Request.CreateErrorResponse(HttpStatusCode.BadRequest, msg);
        private HttpResponseMessage Denied() => Request.CreateErrorResponse(HttpStatusCode.Forbidden, "You cannot manage this calendar.");

        private HttpResponseMessage Guarded(Func<HttpResponseMessage> action)
        {
            try { return action(); }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message);
            }
        }

        // ---- DTOs ----

        public class EntryDto
        {
            public int EntryId { get; set; }
            public int CalendarId { get; set; }
            public string Date { get; set; }
            public string Timespan { get; set; }
            public bool AllDay { get; set; }
            public string ShortTitle { get; set; }
            public bool TitleBold { get; set; }
            public bool TitleItalic { get; set; }
            public string Detail { get; set; }
            public int CategoryId { get; set; }
            public int SortOrder { get; set; }
        }

        public class MoveDto
        {
            public int EntryId { get; set; }
            public string Date { get; set; }
            public int SortOrder { get; set; }
        }

        public class ReorderDto
        {
            public int CalendarId { get; set; }
            public string Date { get; set; }
            public List<int> EntryIds { get; set; }
        }

        public class IdDto { public int EntryId { get; set; } }

        public class StrikeDto
        {
            public int EntryId { get; set; }
            public bool Stricken { get; set; }
        }
    }
}
