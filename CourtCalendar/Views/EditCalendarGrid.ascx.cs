/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using tjc.Modules.CourtCalendar.Components;
using Calendar = tjc.Modules.CourtCalendar.Components.Calendar;

namespace tjc.Modules.CourtCalendar
{
    /// <summary>
    /// Drag-and-drop month grid for authoring a calendar's entries. Click a day to add,
    /// drag chips to reorder, drag across days to move (date only), Shift+drag to copy.
    /// The grid skeleton is rendered server-side (all seven days, every week of the
    /// month); the chips are loaded and maintained client-side via the CalendarAdmin
    /// Web API. All writes re-check CanManageDivision on the server.
    /// </summary>
    public partial class EditCalendarGrid : CourtCalendarModuleBase
    {
        private CalendarController Calendars { get { return new CalendarController(_hostSettings); } }
        private CategoryController Categories { get { return new CategoryController(_hostSettings); } }
        private DivisionController Divisions { get { return new DivisionController(_hostSettings); } }

        /// <summary>CalendarId exposed to the page script.</summary>
        public int GridCalendarId { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkHome.NavigateUrl = HomeUrl;

            var cal = CalendarIdParam > 0 ? Calendars.GetCalendar(CalendarIdParam) : null;
            var division = cal != null ? Divisions.GetDivision(cal.DivisionId) : null;
            if (cal == null || division == null || !CanManageDivision(division))
            {
                pnlMain.Visible = false;
                pnlDenied.Visible = true;
                return;
            }

            // Ensure jQuery + the DNN Services Framework (and its anti-forgery token)
            // are registered so the page script can post to the Web API.
            DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

            GridCalendarId = cal.CalendarId;
            lnkList.NavigateUrl = EditCalendarUrl(cal.CalendarId);
            ltTitle.Text = HttpUtility.HtmlEncode(
                (string.IsNullOrWhiteSpace(division.JudgeName) ? string.Empty : division.JudgeName + " – ")
                + cal.MonthName + " " + cal.Year + " – " + division.Name);

            var categories = Categories.GetCategories(division.DivisionId).ToList();
            ltCatOptions.Text = BuildCategoryOptions(categories);
            ltCatMap.Text = BuildCategoryMapJson(categories);
            ltGrid.Text = BuildGrid(cal);
        }

        // ---- Grid skeleton (chips are filled in by the page script) ----

        private string BuildGrid(Calendar cal)
        {
            var first = new DateTime(cal.Year, cal.Month, 1);
            var last = first.AddMonths(1).AddDays(-1);
            var names = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            var sb = new StringBuilder();
            sb.Append("<div class=\"cc-grid-scroll\">");
            sb.Append("<table class=\"cc-g-grid\"><thead><tr>");
            foreach (var n in names)
                sb.AppendFormat("<th>{0}</th>", n);
            sb.Append("</tr></thead><tbody>");

            for (var monday = Monday(first); monday <= last; monday = monday.AddDays(7))
            {
                sb.Append("<tr>");
                for (int i = 0; i < 7; i++)
                {
                    var date = monday.AddDays(i);
                    var iso = date.ToString("yyyy-MM-dd");
                    var inMonth = date.Month == cal.Month && date.Year == cal.Year;
                    sb.AppendFormat("<td class=\"cc-g-day{0}\" data-date=\"{1}\">", inMonth ? string.Empty : " cc-g-outmonth", iso);
                    sb.AppendFormat(
                        "<div class=\"cc-g-dayhead\"><span class=\"cc-g-daynum\">{0}</span>" +
                        "<button type=\"button\" class=\"cc-g-add\" data-date=\"{1}\" title=\"Add entry\" aria-label=\"Add entry on {1}\"><i class=\"fas fa-plus\" aria-hidden=\"true\"></i></button></div>",
                        date.Day, iso);
                    sb.AppendFormat("<ul class=\"cc-g-list\" data-date=\"{0}\"></ul>", iso);
                    sb.Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table></div>");
            return sb.ToString();
        }

        private static string BuildCategoryOptions(List<Category> categories)
        {
            var sb = new StringBuilder();
            sb.Append("<option value=\"0\">Default</option>");
            foreach (var c in categories.Where(c => !c.IsDefault))
                sb.AppendFormat("<option value=\"{0}\">{1}</option>", c.CategoryId, HttpUtility.HtmlEncode(c.Name ?? string.Empty));
            return sb.ToString();
        }

        private static string BuildCategoryMapJson(List<Category> categories)
        {
            // { "<id>": { "color": "#rrggbb", "name": "..." }, ... }  (0 = default/no color)
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"0\":{\"color\":\"\",\"name\":\"Default\"}");
            foreach (var c in categories)
            {
                var color = (!c.IsDefault && !string.IsNullOrWhiteSpace(c.Color)) ? c.Color.Trim() : string.Empty;
                sb.AppendFormat(",\"{0}\":{{\"color\":{1},\"name\":{2}}}",
                    c.CategoryId, JsStr(color), JsStr(c.Name ?? string.Empty));
            }
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>Minimal JSON string encoder for the category map.</summary>
        private static string JsStr(string s)
        {
            if (s == null) return "\"\"";
            var sb = new StringBuilder("\"");
            foreach (var ch in s)
            {
                switch (ch)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '"': sb.Append("\\\""); break;
                    case '<': sb.Append("\\u003c"); break;
                    case '>': sb.Append("\\u003e"); break;
                    case '&': sb.Append("\\u0026"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\n': sb.Append("\\n"); break;
                    default: sb.Append(ch); break;
                }
            }
            sb.Append("\"");
            return sb.ToString();
        }

        private static DateTime Monday(DateTime date)
        {
            int offset = ((int)date.DayOfWeek + 6) % 7; // Mon=0 ... Sun=6
            return date.Date.AddDays(-offset);
        }
    }
}
