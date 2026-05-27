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
    public partial class CertifiedInterpreterReport : ReportsModuleBase
    {
        private readonly INavigationManager _navigationManager;

        public CertifiedInterpreterReport()
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
                    // (Completed -> Eligible -> Not Yet Eligible -> Terminated).
                    // Within each bucket, rows are ordered by CertificationDate.
                    ViewState["SortExpression"] = "Status";
                    ViewState["SortDirection"]  = "ASC";
                    BindData();
                }
            }
            catch (Exception exc) { Exceptions.ProcessModuleLoadException(this, exc); }
        }

        private void BindData()
        {
            // includeInactive=true so terminated interpreters are listed for
            // historical reference. The row-data-bound handler tints them red
            // so they remain visually distinct from active rows.
            var data = new ReportController().GetCertifiedInterpreterSeniority(includeInactive: true);
            var sort = ViewState["SortExpression"] as string ?? "Status";
            var dir  = ViewState["SortDirection"]  as string ?? "ASC";

            System.Linq.IOrderedEnumerable<SeniorityRow> ordered;
            switch (sort)
            {
                // Default Status sort: rows ordered by bucket in legend order
                // (Completed -> Eligible -> Not Yet Eligible -> Terminated),
                // then by CertificationDate within each bucket.
                case "Status":
                    ordered = SortBy(data, BucketOrder, dir);
                    ordered = ordered.ThenBy(r => r.CertificationDate);
                    break;
                case "StartDate":         ordered = SortBy(data, r => r.StartDate,         dir); break;
                case "CertificationDate": ordered = SortBy(data, r => r.CertificationDate, dir); break;
                case "LastName":
                default:                  ordered = SortBy(data, r => r.LastName,          dir); break;
            }
            var final = (dir == "ASC")
                ? ordered.ThenBy(r => r.LastName).ThenBy(r => r.FirstName)
                : ordered.ThenByDescending(r => r.LastName).ThenByDescending(r => r.FirstName);

            grdReport.DataSource = final;
            grdReport.DataBind();
        }

        /// <summary>
        /// Color-codes each data row by status bucket so terminated employees
        /// (and milestone-eligibility tiers) are visually obvious. CSS lives
        /// in Reports/module.css alongside the .empdb-status-legend swatches.
        /// </summary>
        protected void grdReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var row = e.Row.DataItem as SeniorityRow;
            if (row == null) return;
            e.Row.CssClass = StatusCssClass(row.IsActive, row.CertificationDate);
        }

        /// <summary>Bucket label shown in the "Status" column.</summary>
        protected string DescribeStatus(object isActive, object certificationDate)
        {
            return StatusLabel(AsBool(isActive), AsDate(certificationDate));
        }

        // --- status bucket helpers ------------------------------------------
        // Interpreter incentive: anchored on CertificationDate (not HireDate).
        // 2-yr / 5-yr milestones. Four buckets:
        //   terminated   – IsActive=false (regardless of years)
        //   completed    – 5+ years since certification
        //   eligible     – 2 to <5 years (inside the incentive window)
        //   not-eligible – under 2 years (hasn't reached first milestone)
        private static string StatusLabel(bool? isActive, DateTime? anchor)
        {
            if (isActive == false) return "Terminated";
            var years = YearsSince(anchor);
            if (years >= 5) return "Completed";
            if (years >= 2) return "Eligible";
            return "Not Yet Eligible";
        }
        private static string StatusCssClass(bool? isActive, DateTime? anchor)
        {
            if (isActive == false) return "row-terminated";
            var years = YearsSince(anchor);
            if (years >= 5) return "row-completed";
            if (years >= 2) return "row-eligible";
            return "row-not-eligible";
        }
        // Sort key used by the default "Status" sort. Matches the legend order:
        //   0 = completed, 1 = eligible, 2 = not-eligible, 3 = terminated.
        private static int BucketOrder(SeniorityRow row)
        {
            if (row.IsActive == false) return 3;
            var years = YearsSince(row.CertificationDate);
            if (years >= 5) return 0;
            if (years >= 2) return 1;
            return 2;
        }
        private static int YearsSince(DateTime? anchor)
        {
            if (anchor == null) return 0;
            var today = DateTime.Today;
            var y = today.Year - anchor.Value.Year;
            if (today < anchor.Value.AddYears(y)) y--;
            return y;
        }
        private static bool? AsBool(object o)
        {
            if (o == null || o is DBNull) return null;
            if (o is bool b) return b;
            return bool.TryParse(o.ToString(), out var parsed) ? parsed : (bool?)null;
        }

        private static System.Linq.IOrderedEnumerable<T> SortBy<T, TKey>(
            System.Collections.Generic.IEnumerable<T> src, Func<T, TKey> key, string direction)
            => direction == "DESC" ? src.OrderByDescending(key) : src.OrderBy(key);

        protected void grdReport_Sorting(object sender, GridViewSortEventArgs e)
        {
            var prev = ViewState["SortExpression"] as string;
            var prevDir = ViewState["SortDirection"] as string ?? "ASC";
            ViewState["SortDirection"] = (prev == e.SortExpression && prevDir == "ASC") ? "DESC" : "ASC";
            ViewState["SortExpression"] = e.SortExpression;
            BindData();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            BindData();
            ReportExportHelper.ExportGridToExcel(grdReport, Response, "Certified_Interpreter_Seniority");
        }

        protected DateTime? AddYears(object dateObj, int years)
        {
            var d = AsDate(dateObj);
            return d?.AddYears(years);
        }
        protected bool HasYearsPassed(object dateObj, int years)
        {
            var d = AsDate(dateObj);
            if (d == null) return false;
            return DateTime.Today >= d.Value.AddYears(years);
        }
        protected string FormatNullableDate(object dateObj)
        {
            var d = AsDate(dateObj);
            return d.HasValue ? d.Value.ToString("MM/dd/yyyy") : "";
        }
        private static DateTime? AsDate(object o)
        {
            if (o == null || o is DBNull) return null;
            if (o is DateTime dt) return dt;
            return DateTime.TryParse(o.ToString(), out var parsed) ? parsed : (DateTime?)null;
        }
    }
}
