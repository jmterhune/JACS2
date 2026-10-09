/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    public partial class CaseTypes : CourtRegistryModuleBase
    {
        private readonly IHostSettings _hostSettings;

        public CaseTypes()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }

        private void BindList()
        {
            var ctl = new CaseTypeController(_hostSettings);
            rptCaseTypes.DataSource = ctl.GetCaseTypes().OrderBy(c => c.CaseTypeName);
            rptCaseTypes.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                    BindList();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptCaseTypes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int.TryParse(e.CommandArgument.ToString(), out int caseTypeId);
            var ctl = new CaseTypeController(_hostSettings);
            if (e.CommandName == "delete" && caseTypeId > 0)
            {
                ctl.DeleteCaseType(caseTypeId);
                BindList();
            }
            else if (e.CommandName == "edit" && caseTypeId > 0)
            {
                var caseType = ctl.GetCaseType(caseTypeId);
                if (caseType != null)
                {
                    hdCaseTypeID.Value = caseType.CaseTypeID.ToString();
                    txtCaseTypeName.Text = caseType.CaseTypeName;
                    chkActive.Checked = caseType.Active;
                    ltModalScript.Text = "<script>(function(){function s(){if(typeof bootstrap!=='undefined'&&bootstrap.Modal){bootstrap.Modal.getOrCreateInstance(document.getElementById('caseTypeModal')).show();}else if(typeof jQuery!=='undefined'){jQuery('#caseTypeModal').modal('show');}}if(document.readyState!=='loading'){s();}else{document.addEventListener('DOMContentLoaded',s);}})();</script>";
                }
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new CaseTypeController(_hostSettings);
            if (int.TryParse(hdCaseTypeID.Value, out int caseTypeId) && caseTypeId > 0)
            {
                var caseType = ctl.GetCaseType(caseTypeId);
                caseType.CaseTypeName = txtCaseTypeName.Text.Trim();
                caseType.Active = chkActive.Checked;
                ctl.UpdateCaseType(caseType);
            }
            else
            {
                ctl.CreateCaseType(new CaseType
                {
                    CaseTypeName = txtCaseTypeName.Text.Trim(),
                    Active = chkActive.Checked
                });
            }
            BindList();
        }
    }
}
