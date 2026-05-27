/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.Reports.Components;

namespace tjc.Modules.Reports.EmployeeReports
{
    /// <summary>
    /// DROP Participants report — see EmployeeDB\Documentation\DROP Participants.xlsx
    /// for the legacy version this replaces. Lists every employee where
    /// tjc_employee.DropEntryDate is non-NULL, sorted by entry date.
    /// </summary>
    public partial class DropParticipantsReport : ReportsModuleBase
    {
        private readonly INavigationManager _navigationManager;

        public DropParticipantsReport()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    lnkBack.NavigateUrl = _navigationManager.NavigateURL();
                    // Default sort: status bucket in legend order
                    // (Completed (Retired) -> In DROP -> Terminated).
                    // Within each bucket, rows are ordered by DropEntryDate.
                    ViewState["SortExpression"] = "Status";
                    ViewState["SortDirection"]  = "ASC";
                    BindData();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>Re-bind the grid honoring the current sort column / direction.</summary>
        private void BindData()
        {
            var data = new ReportController().GetDropParticipants(includeInactive: true);
            var sort = ViewState["SortExpression"] as string ?? "Status";
            var dir  = ViewState["SortDirection"]  as string ?? "ASC";

            // Map SortExpression -> projection. Wrap each in a typed Func so
            // OrderBy sees the underlying type (DateTime?, string, etc.) for
            // correct null-handling and ordering.
            System.Linq.IOrderedEnumerable<DropParticipantRow> ordered;
            switch (sort)
            {
                // Default Status sort: rows ordered by bucket in legend order
                // (Completed (Retired) -> In DROP -> Terminated), then by
                // DropEntryDate within each bucket.
                case "Status":
                    ordered = SortBy(data, BucketOrder, dir);
                    ordered = ordered.ThenBy(r => r.DropEntryDate);
                    break;
                case "JobTitle":        ordered = SortBy(data, r => r.JobTitle,        dir); break;
                case "DropEntryDate":   ordered = SortBy(data, r => r.DropEntryDate,   dir); break;
                case "DropExitDate":    ordered = SortBy(data, r => r.DropExitDate,    dir); break;
                case "TerminationDate": ordered = SortBy(data, r => r.TerminationDate, dir); break;
                case "DropLeavePayout": ordered = SortBy(data, r => r.DropLeavePayout, dir); break;
                case "LastName":
                default:                ordered = SortBy(data, r => r.LastName,        dir); break;
            }
            // Stable tiebreaker so equal primary keys group consistently.
            var final = (dir == "ASC")
                ? ordered.ThenBy(r => r.LastName).ThenBy(r => r.FirstName)
                : ordered.ThenByDescending(r => r.LastName).ThenByDescending(r => r.FirstName);

            grdReport.DataSource = final;
            grdReport.DataBind();
        }

        private static System.Linq.IOrderedEnumerable<T> SortBy<T, TKey>(
            System.Collections.Generic.IEnumerable<T> src,
            Func<T, TKey> key,
            string direction)
            => direction == "DESC" ? src.OrderByDescending(key) : src.OrderBy(key);

        protected void grdReport_Sorting(object sender, GridViewSortEventArgs e)
        {
            var prev = ViewState["SortExpression"] as string;
            var prevDir = ViewState["SortDirection"] as string ?? "ASC";
            // Same column clicked again -> flip direction. New column -> ASC.
            ViewState["SortDirection"] = (prev == e.SortExpression && prevDir == "ASC") ? "DESC" : "ASC";
            ViewState["SortExpression"] = e.SortExpression;
            BindData();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Re-bind so the export reflects the current sort, then stream the
            // already-rendered cells out as an Excel-readable HTML table
            // (no GridView.RenderControl -> avoids Page.VerifyRenderingInServerForm).
            BindData();
            ReportExportHelper.ExportGridToExcel(grdReport, Response, "DROP_Participants");
        }

        /// <summary>
        /// Color-codes each data row by DROP status bucket so completed
        /// retirees, current DROP participants, and terminated employees
        /// are visually distinct. CSS lives in Reports/module.css alongside
        /// the .empdb-status-legend swatches.
        /// </summary>
        protected void grdReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var row = e.Row.DataItem as DropParticipantRow;
            if (row == null) return;
            e.Row.CssClass = StatusCssClass(row.IsActive, row.DropExitDate);
        }

        /// <summary>Bucket label shown in the "Status" column.</summary>
        protected string DescribeStatus(object isActive, object dropExitDate)
        {
            return StatusLabel(AsBool(isActive), AsDate(dropExitDate));
        }

        // --- status bucket helpers ------------------------------------------
        // DROP has 3 buckets (every row already has a DROP entry date, so
        // "not yet eligible" doesn't apply):
        //   completed   – IsActive=false AND DropExitDate is set (retired
        //                 through the program)
        //   eligible    – IsActive=true (currently in DROP, still working)
        //   terminated  – IsActive=false AND no DropExitDate (left before
        //                 completing the program — atypical)
        private static string StatusLabel(bool? isActive, DateTime? dropExitDate)
        {
            if (isActive == true) return "In DROP";
            return dropExitDate.HasValue ? "Completed" : "Terminated";
        }
        private static string StatusCssClass(bool? isActive, DateTime? dropExitDate)
        {
            if (isActive == true) return "row-eligible";
            return dropExitDate.HasValue ? "row-completed" : "row-terminated";
        }
        // Sort key used by the default "Status" sort. Matches the legend order:
        //   0 = Completed (Retired), 1 = In DROP, 2 = Terminated.
        private static int BucketOrder(DropParticipantRow row)
        {
            if (row.IsActive == true) return 1;
            return row.DropExitDate.HasValue ? 0 : 2;
        }
        private static bool? AsBool(object o)
        {
            if (o == null || o is DBNull) return null;
            if (o is bool b) return b;
            return bool.TryParse(o.ToString(), out var parsed) ? parsed : (bool?)null;
        }
        private static DateTime? AsDate(object o)
        {
            if (o == null || o is DBNull) return null;
            if (o is DateTime dt) return dt;
            return DateTime.TryParse(o.ToString(), out var parsed) ? parsed : (DateTime?)null;
        }
    }
}
