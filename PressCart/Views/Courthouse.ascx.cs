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
    /// The courthouse list the public presentation cart form picks from. Moved here
    /// from the public PressCart module, where it sat behind an "Admin" link.
    /// </summary>
    public partial class Courthouse : PressCartModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
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
                    PopulateCourthouseList();
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

                var ctl = new CourthouseController(_hostSettings);
                Components.Courthouse courthouse = new Components.Courthouse();
                bool isNew = true;
                if (hdCourthouseId.Value != "")
                {
                    courthouse = ctl.GetCourthouse(Convert.ToInt32(hdCourthouseId.Value));
                    if (courthouse == null) return;
                    isNew = false;
                }
                courthouse.CourthouseName = txtCourthouseName.Text.Trim();
                courthouse.County = drpCounty.SelectedValue;

                if (isNew) ctl.CreateCourthouse(courthouse);
                else ctl.UpdateCourthouse(courthouse);

                ClearForm();
                PopulateCourthouseList();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptCourthouse_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                if (!CanManage) return;

                int courthouseId = Convert.ToInt32(e.CommandArgument);
                var ctl = new CourthouseController(_hostSettings);

                if (e.CommandName == "delete")
                {
                    // Courtrooms hang off the courthouse, and the public form reaches them
                    // through it, so removing one underneath them would strand them.
                    if (ctl.HasCourtrooms(courthouseId))
                    {
                        ShowMessage("That courthouse still has courtrooms. Delete or move them first.");
                    }
                    else
                    {
                        ctl.DeleteCourthouse(courthouseId);
                    }
                    PopulateCourthouseList();
                }
                if (e.CommandName == "edit")
                {
                    ClearForm();
                    Components.Courthouse courthouse = ctl.GetCourthouse(courthouseId);
                    if (courthouse == null)
                    {
                        PopulateCourthouseList();
                        return;
                    }
                    hdCourthouseId.Value = courthouseId.ToString();
                    txtCourthouseName.Text = courthouse.CourthouseName;
                    drpCounty.SelectedValue = courthouse.County;
                    ScriptManager.RegisterStartupScript(rptCourthouse, rptCourthouse.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptCourthouse_ItemCreated(object sender, RepeaterItemEventArgs e)
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
        protected void pnlCourthouse_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel"));
            methodInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { sender as UpdatePanel });
        }

        private void ShowMessage(string text)
        {
            lblMessage.Text = text;
            lblMessage.Visible = true;
        }

        private void ClearForm()
        {
            txtCourthouseName.Text = string.Empty;
            drpCounty.SelectedIndex = 0;
            hdCourthouseId.Value = string.Empty;
        }

        private void PopulateCourthouseList()
        {
            rptCourthouse.DataSource = new CourthouseController(_hostSettings).GetCourthouses();
            rptCourthouse.DataBind();
        }
    }
}
