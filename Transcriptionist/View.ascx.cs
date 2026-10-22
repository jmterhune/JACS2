using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using tjc.Modules.Transcriptionist.Components;

namespace tjc.Modules.Transcriptionist
{
    /// <summary>Admin dashboard: this year's counts and the latest pending applications.</summary>
    public partial class View : TranscriptionistModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (RedirectIfNoAccess()) return;

                if (!IsPostBack)
                {
                    lnkList.NavigateUrl = EditUrl("list");

                    string external = ExternalAppUrl;
                    if (external != null)
                    {
                        lnkExternal.NavigateUrl = external;
                        lnkExternal.Visible = true;
                    }

                    var applications = new ApplicationController(_hostSettings).GetAll().ToList();
                    LoadDashboardStats(applications);
                    BindPendingApplications(applications);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void LoadDashboardStats(System.Collections.Generic.List<Application> applications)
        {
            var thisYear = applications.Where(a => a.CreatedDate?.Year == DateTime.Today.Year).ToList();

            litTotalApplications.Text = thisYear.Count.ToString();
            litPending.Text = thisYear.Count(a => a.Status == "Pending").ToString();
            litApproved.Text = thisYear.Count(a => a.Status == "Approved").ToString();
            litRejected.Text = thisYear.Count(a => a.Status == "Rejected").ToString();
        }

        private void BindPendingApplications(System.Collections.Generic.List<Application> applications)
        {
            rptPendingApplications.DataSource = applications
                .Where(a => a.Status == "Pending")
                .OrderByDescending(a => a.CreatedDate)
                .Take(10)
                .ToList();
            rptPendingApplications.DataBind();
        }

        protected string GetEditUrl(object dataItem)
        {
            return EditUrl("tid", ((Application)dataItem).TranscriptionistID.ToString(), "edit");
        }

        protected string GetViewUrl(object dataItem)
        {
            return EditUrl("tid", ((Application)dataItem).TranscriptionistID.ToString(), "detail");
        }
    }
}
