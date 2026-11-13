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
using tjc.Modules.LPP_Complaint.Components;

namespace tjc.Modules.LPP_Complaint
{
    /// <summary>
    /// Lists the Local Professionalism Panel complaints submitted on the public site,
    /// filtered by handling status, with a CSV export of whatever the filter is showing.
    /// </summary>
    public partial class View : LPP_ComplaintModuleBase
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
                WriteCsv(GetFilteredComplaints());
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
            List<LppComplaint> complaints = GetFilteredComplaints();
            rptComplaints.DataSource = complaints;
            rptComplaints.DataBind();

            pnlEmpty.Visible = complaints.Count == 0;
            ltEmpty.Text = drpStatusFilter.SelectedValue == AnyStatus
                ? "No complaints have been submitted yet."
                : "No complaints have the status " + Server.HtmlEncode(drpStatusFilter.SelectedValue) + ".";
        }

        private List<LppComplaint> GetFilteredComplaints()
        {
            IEnumerable<LppComplaint> complaints = new LppComplaintController(_hostSettings).GetComplaints();

            string status = drpStatusFilter.SelectedValue;
            if (!string.IsNullOrEmpty(status))
            {
                complaints = complaints.Where(c => string.Equals(c.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            return complaints.ToList();
        }

        /// <summary>
        /// Writes the listed complaints out as CSV. Every field is quoted and embedded
        /// quotes are doubled, so the commas and line breaks that the incident
        /// description is full of do not break the row.
        /// </summary>
        private void WriteCsv(IEnumerable<LppComplaint> complaints)
        {
            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", new[]
            {
                "Complaint Id", "Submitted", "Status", "Status Notes",
                "Referring Party", "Referrer Email", "Referrer Address", "Referrer City",
                "Referrer State", "Referrer Zip", "Referrer Phone", "Relationship",
                "Attorney Name", "Bar Number", "Attorney Address", "Attorney City",
                "Attorney State", "Attorney Zip", "Attorney Phone", "Attorney Email",
                "Nature of Incident", "Violation Date", "County", "Incident Description", "Signed",
            }.Select(Quote)));

            foreach (LppComplaint c in complaints)
            {
                csv.AppendLine(string.Join(",", new[]
                {
                    c.LppComplaintId.ToString(),
                    c.CreatedOnDate.ToString("MM/dd/yyyy h:mm tt"),
                    c.Status,
                    c.StatusNotes,
                    c.ReferrerName,
                    c.ReferrerEmail,
                    c.ReferrerAddress,
                    c.ReferrerCity,
                    c.ReferrerState,
                    c.ReferrerZip,
                    c.ReferrerPhone,
                    c.Relationship,
                    c.AttorneyName,
                    c.AttorneyBarNumber,
                    c.AttorneyAddress,
                    c.AttorneyCity,
                    c.AttorneyState,
                    c.AttorneyZip,
                    c.AttorneyPhone,
                    c.AttorneyEmail,
                    c.NatureSummary,
                    c.ViolationDate,
                    c.County,
                    c.IncidentDescription,
                    c.SignedBy,
                }.Select(Quote)));
            }

            string fileName = string.Format("lpp-complaints-{0:yyyy-MM-dd}.csv", DateTime.Today);
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
