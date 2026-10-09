/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using tjc.Modules.Arbitrators.Components;

namespace tjc.Modules.Arbitrators.Views
{
    public partial class List : ArbitratorsModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                    BindList();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindList()
        {
            var ctl = new ArbitratorApplicationController(_hostSettings);
            rptApplications.DataSource = ctl.GetApplications();
            rptApplications.DataBind();
        }

        protected string StatusBadgeClass(object status)
        {
            switch ((ApplicationStatus)Enum.Parse(typeof(ApplicationStatus), status.ToString()))
            {
                case ApplicationStatus.Pending: return "bg-primary";
                case ApplicationStatus.Approved: return "bg-success";
                case ApplicationStatus.Denied: return "bg-warning text-dark";
                case ApplicationStatus.Removed: return "bg-dark";
                default: return "bg-secondary";
            }
        }
    }
}
