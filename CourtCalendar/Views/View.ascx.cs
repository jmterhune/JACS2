/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using tjc.Modules.CourtCalendar.Components;

namespace tjc.Modules.CourtCalendar
{
    public partial class View : CourtCalendarModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (IsAdmin)
                {
                    pnlAdminTools.Visible = true;
                    lnkManageJudges.NavigateUrl = ManageJudgesUrl;
                    lnkManageDivisions.NavigateUrl = ManageDivisionsUrl;
                }

                var divCtl = new DivisionController(_hostSettings);
                List<Division> divisions = IsAdmin
                    ? divCtl.GetDivisions().ToList()
                    : divCtl.GetDivisionsForUser(UserId).ToList();

                ltContent.Text = BuildContent(divisions);
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private string BuildContent(List<Division> divisions)
        {
            if (!Request.IsAuthenticated)
                return "<div class=\"alert alert-warning\">Please sign in to manage calendars.</div>";

            if (divisions.Count == 0)
                return IsAdmin
                    ? "<div class=\"alert alert-info\">No divisions yet. Use <strong>Manage Divisions</strong> to add one.</div>"
                    : "<div class=\"alert alert-info\">You are not assigned to any divisions. Contact Court Administration if this is unexpected.</div>";

            var calCtl = new CalendarController(_hostSettings);
            var sb = new StringBuilder();
            sb.Append("<h2 class=\"cc-h2\">Court Calendars</h2>");

            foreach (var d in divisions.OrderBy(x => x.SortOrder).ThenBy(x => x.Name))
            {
                sb.Append("<div class=\"cc-division-card card mb-3\"><div class=\"card-body\">");
                sb.AppendFormat("<div class=\"cc-division-head\"><h3 class=\"cc-h3\">{0}</h3>", Enc(d.Name));
                if (!string.IsNullOrWhiteSpace(d.JudgeName))
                    sb.AppendFormat("<span class=\"cc-judge\">{0}</span>", Enc(d.JudgeName));
                sb.Append("</div>");

                sb.Append("<div class=\"cc-division-actions mb-2\">");
                sb.AppendFormat("<a class=\"btn btn-sm btn-primary\" href=\"{0}\"><i class=\"fas fa-plus\" aria-hidden=\"true\"></i> New calendar</a> ", Attr(NewCalendarUrl(d.DivisionId)));
                sb.AppendFormat("<a class=\"btn btn-sm btn-outline-secondary\" href=\"{0}\"><i class=\"fas fa-palette\" aria-hidden=\"true\"></i> Categories</a>", Attr(ManageCategoriesUrl(d.DivisionId)));
                sb.Append("</div>");

                var cals = calCtl.GetCalendars(d.DivisionId).ToList();
                if (cals.Count == 0)
                {
                    sb.Append("<p class=\"text-muted\">No calendars yet.</p>");
                }
                else
                {
                    sb.Append("<table class=\"table table-sm cc-cal-table\"><thead><tr><th>Month</th><th>Status</th><th></th></tr></thead><tbody>");
                    foreach (var c in cals)
                    {
                        var status = c.Published
                            ? "<span class=\"badge bg-success\">Published</span>"
                            : "<span class=\"badge bg-secondary\">Draft</span>";
                        sb.AppendFormat(
                            "<tr><td>{0} {1}</td><td>{2}</td><td class=\"text-end\"><a class=\"btn btn-sm btn-outline-primary\" href=\"{3}\"><i class=\"fas fa-pen\" aria-hidden=\"true\"></i> Edit</a></td></tr>",
                            Enc(c.MonthName), c.Year, status, Attr(EditCalendarUrl(c.CalendarId)));
                    }
                    sb.Append("</tbody></table>");
                }
                sb.Append("</div></div>");
            }
            return sb.ToString();
        }

        private static string Enc(string s) { return HttpUtility.HtmlEncode(s ?? string.Empty); }
        private static string Attr(string s) { return HttpUtility.HtmlAttributeEncode(s ?? string.Empty); }
    }
}
