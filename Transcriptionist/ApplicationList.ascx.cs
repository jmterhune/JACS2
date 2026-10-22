using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using tjc.Modules.Transcriptionist.Components;

namespace tjc.Modules.Transcriptionist
{
    /// <summary>Every application, with name / county / status filters.</summary>
    public partial class ApplicationList : TranscriptionistModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (RedirectIfNoAccess()) return;

                if (!IsPostBack)
                {
                    lnkDashboard.NavigateUrl = EditUrl();

                    string external = ExternalAppUrl;
                    if (external != null)
                    {
                        lnkExternal.NavigateUrl = external;
                        lnkExternal.Visible = true;
                    }

                    BindAllApplications();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindAllApplications()
        {
            var allApps = new ApplicationController(_hostSettings).GetAll();

            if (!string.IsNullOrEmpty(txtSearchName.Text))
            {
                string name = txtSearchName.Text.ToLower();
                allApps = allApps.Where(a => (a.FirstName ?? "").ToLower().Contains(name) || (a.LastName ?? "").ToLower().Contains(name));
            }

            if (!string.IsNullOrEmpty(txtSearchCounty.Text))
            {
                string county = txtSearchCounty.Text.ToLower();
                allApps = allApps.Where(a => (a.County ?? "").ToLower().Contains(county));
            }

            if (!string.IsNullOrEmpty(ddlSearchStatus.SelectedValue))
            {
                allApps = allApps.Where(a => a.Status == ddlSearchStatus.SelectedValue);
            }

            gvAllApplications.DataSource = allApps.OrderByDescending(a => a.CreatedDate).ToList();
            gvAllApplications.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindAllApplications();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearchName.Text = "";
            txtSearchCounty.Text = "";
            ddlSearchStatus.SelectedValue = "";
            BindAllApplications();
        }

        protected string GetStatusBadgeClass(string status)
        {
            switch (status)
            {
                case "Approved": return "bg-success";
                case "Rejected": return "bg-danger";
                default: return "bg-secondary";
            }
        }

        protected string GetEditUrl(string tid)
        {
            return EditUrl("tid", tid, "edit");
        }

        protected string GetViewUrl(string tid)
        {
            return EditUrl("tid", tid, "detail");
        }
    }
}
