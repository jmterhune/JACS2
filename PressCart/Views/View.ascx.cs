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
using tjc.Modules.PressCart.Components;

namespace tjc.Modules.PressCart
{
    /// <summary>
    /// Lists the presentation cart requests submitted on the public site, filtered by
    /// handling status, with a CSV export of whatever the filter is showing.
    /// </summary>
    public partial class View : PressCartModuleBase
    {
        private const string AnyStatus = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // The list tabs lead to the division / courtroom / courthouse screens,
                // which only a user who can edit the module content may open.
                phManageTabs.Visible = CanManage;

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
            List<Components.PressCart> requests = GetFilteredRequests();
            rptRequests.DataSource = requests;
            rptRequests.DataBind();

            pnlEmpty.Visible = requests.Count == 0;
            ltEmpty.Text = drpStatusFilter.SelectedValue == AnyStatus
                ? "No presentation cart requests have been submitted yet."
                : "No requests have the status " + Server.HtmlEncode(drpStatusFilter.SelectedValue) + ".";
        }

        private List<Components.PressCart> GetFilteredRequests()
        {
            IEnumerable<Components.PressCart> requests = new PressCartController(_hostSettings).GetRequests();

            string status = drpStatusFilter.SelectedValue;
            if (!string.IsNullOrEmpty(status))
            {
                requests = requests.Where(r => string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            return requests.ToList();
        }

        /// <summary>
        /// Writes the listed requests out as CSV. Every field is quoted and embedded
        /// quotes are doubled, so a comma or a line break inside the special
        /// instructions does not break the row.
        /// </summary>
        private void WriteCsv(IEnumerable<Components.PressCart> requests)
        {
            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", new[]
            {
                "Request Id", "Submitted", "Status", "Status Notes",
                "Last Name", "First Name", "Email", "Phone",
                "Contact Name", "Contact Email", "Contact Phone",
                "Court Date", "Court Time", "County", "Division / Judge", "Courthouse", "Courtroom",
                "Cart Type", "Days For Hearing", "Training Required", "Special Instructions",
            }.Select(Quote)));

            foreach (Components.PressCart r in requests)
            {
                csv.AppendLine(string.Join(",", new[]
                {
                    r.PressCartId.ToString(),
                    r.CreatedOnDate.HasValue ? r.CreatedOnDate.Value.ToString("MM/dd/yyyy h:mm tt") : string.Empty,
                    r.Status,
                    r.StatusNotes,
                    r.LastName,
                    r.FirstName,
                    r.Email,
                    r.Phone,
                    r.ContactName,
                    r.ContactEmail,
                    r.ContactPhone,
                    r.CourtDate,
                    r.CourtTime,
                    r.County,
                    r.Judge,
                    r.Courthouse,
                    r.Courtroom,
                    r.CartType,
                    r.TrialLength.GetValueOrDefault() > 0 ? r.TrialLength.Value.ToString() : string.Empty,
                    r.TrainingRequired.GetValueOrDefault() ? "Yes" : "No",
                    r.Comments,
                }.Select(Quote)));
            }

            string fileName = string.Format("press-cart-requests-{0:yyyy-MM-dd}.csv", DateTime.Today);
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
