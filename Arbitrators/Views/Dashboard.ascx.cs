/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.Arbitrators.Components;

namespace tjc.Modules.Arbitrators.Views
{
    /// <summary>Admin dashboard: this year's counts and the latest pending applications.</summary>
    public partial class Dashboard : ArbitratorsModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (IsPostBack) return;

                lnkList.NavigateUrl = ListUrl;

                string external = ExternalAppUrl;
                if (!string.IsNullOrEmpty(external))
                {
                    lnkExternal.NavigateUrl = external;
                    lnkExternal.Visible = true;
                }

                var applications = new ArbitratorApplicationController(_hostSettings).GetApplications().ToList();
                LoadDashboardStats(applications);
                BindPendingApplications(applications);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void LoadDashboardStats(List<ArbitratorApplication> applications)
        {
            var thisYear = applications.Where(a => a.CreatedOnDate.Year == DateTime.Today.Year).ToList();

            litTotalApplications.Text = thisYear.Count.ToString();
            litPending.Text = thisYear.Count(a => a.Status == ApplicationStatus.Pending).ToString();
            litApproved.Text = thisYear.Count(a => a.Status == ApplicationStatus.Approved).ToString();
            litDenied.Text = thisYear.Count(a => a.Status == ApplicationStatus.Denied).ToString();
        }

        private void BindPendingApplications(List<ArbitratorApplication> applications)
        {
            var pending = applications
                .Where(a => a.Status == ApplicationStatus.Pending)
                .OrderByDescending(a => a.CreatedOnDate)
                .Take(10)
                .ToList();

            rptPendingApplications.DataSource = pending;
            rptPendingApplications.DataBind();
            rptPendingApplications.Visible = pending.Count > 0;
            litNoPending.Visible = pending.Count == 0;
        }
    }
}
