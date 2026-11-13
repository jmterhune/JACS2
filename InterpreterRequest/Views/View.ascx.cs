/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using tjc.Modules.InterpreterRequest.Components;

namespace tjc.Modules.InterpreterRequest
{
    /// <summary>
    /// Lists the interpreter requests submitted on the public site, filtered by
    /// handling status, with a CSV export of whatever the filter is showing.
    /// </summary>
    public partial class View : InterpreterRequestModuleBase
    {
        private const string AnyStatus = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    BindStatusFilter();
                    BindList();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void drpStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindList();
        }

        protected void cmdExport_Click(object sender, EventArgs e)
        {
            try
            {
                WriteCsv(GetFilteredRequests());
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>Exposed to the markup so a row's badge matches its status.</summary>
        protected string StatusBadgeClass(object status)
        {
            return SubmissionStatus.BadgeClass(status as string);
        }

        private void BindStatusFilter()
        {
            drpStatusFilter.Items.Clear();
            drpStatusFilter.Items.Add(new ListItem("All statuses", AnyStatus));
            foreach (string status in SubmissionStatus.All)
            {
                drpStatusFilter.Items.Add(new ListItem(status, status));
            }
        }

        private void BindList()
        {
            List<InterpreterRequestItem> requests = GetFilteredRequests();
            rptRequests.DataSource = requests;
            rptRequests.DataBind();

            pnlEmpty.Visible = requests.Count == 0;
            ltEmpty.Text = drpStatusFilter.SelectedValue == AnyStatus
                ? "No interpreter requests have been submitted yet."
                : "No interpreter requests have the status " + Server.HtmlEncode(drpStatusFilter.SelectedValue) + ".";
        }

        private List<InterpreterRequestItem> GetFilteredRequests()
        {
            IEnumerable<InterpreterRequestItem> requests = new InterpreterRequestItemController(_hostSettings).GetRequests();

            string status = drpStatusFilter.SelectedValue;
            if (!string.IsNullOrEmpty(status))
            {
                requests = requests.Where(r => string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            return requests.ToList();
        }

        /// <summary>
        /// Writes the listed requests out as CSV. Every field is quoted and embedded
        /// quotes are doubled, so a comma or a line break inside a comment does not
        /// break the row.
        /// </summary>
        private void WriteCsv(IEnumerable<InterpreterRequestItem> requests)
        {
            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", new[]
            {
                "Request Id", "Submitted", "Status", "Status Notes", "Form Language",
                "Interpreter Language", "Case Number", "Person Needing", "Person Last Name",
                "Person First Name", "Event Type", "Date Needed", "Time Needed", "Estimated Length",
                "County", "Courtroom", "Appearance Method", "Judge", "Requestor Last Name",
                "Requestor First Name", "Requestor Phone", "Requestor Email", "Requestor Is Attorney",
            }.Select(Quote)));

            foreach (InterpreterRequestItem r in requests)
            {
                csv.AppendLine(string.Join(",", new[]
                {
                    r.InterpreterRequestId.ToString(),
                    r.CreatedOnDate.ToString("MM/dd/yyyy h:mm tt"),
                    r.Status,
                    r.StatusNotes,
                    r.IsSpanishForm ? "Spanish" : "English",
                    r.InterpreterLanguage,
                    r.CaseNumber,
                    r.PersonNeeding,
                    r.PersonLastName,
                    r.PersonFirstName,
                    r.EventType,
                    r.DateNeeded,
                    r.TimeNeeded,
                    r.DurationNeeded,
                    r.County,
                    r.Courtroom,
                    r.AppearanceMethod,
                    r.JudgeName,
                    r.RequestorLastName,
                    r.RequestorFirstName,
                    r.RequestorPhone,
                    r.RequestorEmail,
                    r.IsAttorney ? "Yes" : "No",
                }.Select(Quote)));
            }

            string fileName = string.Format("interpreter-requests-{0:yyyy-MM-dd}.csv", DateTime.Today);
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(csv.ToString());
            Response.End();
        }

        private static string Quote(string value)
        {
            return "\"" + (value ?? string.Empty).Replace("\"", "\"\"") + "\"";
        }
    }
}
