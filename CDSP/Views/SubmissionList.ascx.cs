/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using tjc.Modules.CDSPAdmin;
using tjc.Modules.CDSPAdmin.Components.Controllers;

namespace tjc.Modules.CDSPAdmin.Views
{
    public partial class SubmissionList : CDSPModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Emit the hidden __RequestVerificationToken field so the JS layer
                // can post to the module Web API (Submissions/Get, /SetCompleted).
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

                if (!IsPostBack)
                {
                    BindList();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindList()
        {
            var ctl = new SubmissionController();
            rptSubmissions.DataSource = ctl.GetAll()
                .OrderByDescending(s => s.CreatedDate)
                .ToList();
            rptSubmissions.DataBind();
        }
    }
}
