/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.PressCart.Components;

namespace tjc.Modules.PressCart
{
    /// <summary>
    /// The division / judge list the public presentation cart form picks from. Moved
    /// here from the public PressCart module, where it sat behind an "Admin" link.
    /// </summary>
    public partial class Division : PressCartModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                chkIsCircuit.InputAttributes.Add("class", "form-check-input");
                chkIsCircuit.LabelAttributes.Add("class", "form-check-label");

                if (!IsPostBack)
                {
                    if (!CanManage)
                    {
                        Response.Redirect(ListUrl, false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }
                    PopulateDivisionList();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CanManage || !Page.IsValid) return;

                var ctl = new DivisionController(_hostSettings);
                Components.Division division = new Components.Division();
                bool isNew = true;
                if (hdDivisionId.Value != "")
                {
                    division = ctl.GetDivision(Convert.ToInt32(hdDivisionId.Value));
                    if (division == null) return;
                    isNew = false;
                }
                division.DivisionName = txtDivisionName.Text.Trim();
                division.Judge = txtJudge.Text.Trim();
                division.County = drpCounty.SelectedValue;
                division.IsCircuit = chkIsCircuit.Checked;

                if (isNew) ctl.CreateDivision(division);
                else ctl.UpdateDivision(division);

                ClearForm();
                PopulateDivisionList();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptDivision_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                if (!CanManage) return;

                int divisionId = Convert.ToInt32(e.CommandArgument);
                var ctl = new DivisionController(_hostSettings);

                if (e.CommandName == "delete")
                {
                    ctl.DeleteDivision(divisionId);
                    PopulateDivisionList();
                }
                if (e.CommandName == "edit")
                {
                    ClearForm();
                    Components.Division division = ctl.GetDivision(divisionId);
                    if (division == null)
                    {
                        PopulateDivisionList();
                        return;
                    }
                    hdDivisionId.Value = divisionId.ToString();
                    txtDivisionName.Text = division.DivisionName;
                    txtJudge.Text = division.Judge;
                    chkIsCircuit.Checked = division.IsCircuit;
                    drpCounty.SelectedValue = division.County;
                    ScriptManager.RegisterStartupScript(rptDivision, rptDivision.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptDivision_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(Page);
                scriptMan.RegisterAsyncPostBackControl((LinkButton)e.Item.FindControl("cmdEdit"));
                scriptMan.RegisterAsyncPostBackControl((LinkButton)e.Item.FindControl("cmdDelete"));
            }
        }

        /// <summary>
        /// Re-registers the update panel after its own Unload. Without this the panel
        /// stops taking part in partial rendering once a row command has fired.
        /// </summary>
        protected void pnlDivision_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel"));
            methodInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { sender as UpdatePanel });
        }

        private void ClearForm()
        {
            txtDivisionName.Text = string.Empty;
            txtJudge.Text = string.Empty;
            drpCounty.SelectedIndex = 0;
            chkIsCircuit.Checked = false;
            hdDivisionId.Value = string.Empty;
        }

        private void PopulateDivisionList()
        {
            rptDivision.DataSource = new DivisionController(_hostSettings).GetDivisions();
            rptDivision.DataBind();
        }
    }
}
