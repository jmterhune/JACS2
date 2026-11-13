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
    /// The courtroom list the public presentation cart form picks from. Moved here
    /// from the public PressCart module, where it sat behind an "Admin" link.
    /// </summary>
    public partial class Courtroom : PressCartModuleBase
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
                    PopulateCourtroomList();
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

                var ctl = new CourtroomController(_hostSettings);
                Components.Courtroom courtroom = new Components.Courtroom();
                bool isNew = true;
                if (hdCourtroomId.Value != "")
                {
                    courtroom = ctl.GetCourtroom(Convert.ToInt32(hdCourtroomId.Value));
                    if (courtroom == null) return;
                    isNew = false;
                }
                courtroom.CourtroomName = txtCourtroomName.Text.Trim();
                courtroom.CourthouseId = Convert.ToInt32(drpCourthouse.SelectedValue);

                if (isNew) ctl.CreateCourtroom(courtroom);
                else ctl.UpdateCourtroom(courtroom);

                ClearForm();
                PopulateCourtroomList();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptCourtroom_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                if (!CanManage) return;

                int courtroomId = Convert.ToInt32(e.CommandArgument);
                var ctl = new CourtroomController(_hostSettings);

                if (e.CommandName == "delete")
                {
                    ctl.DeleteCourtroom(courtroomId);
                    PopulateCourtroomList();
                }
                if (e.CommandName == "edit")
                {
                    ClearForm();
                    Components.Courtroom courtroom = ctl.GetCourtroom(courtroomId);
                    if (courtroom == null)
                    {
                        PopulateCourtroomList();
                        return;
                    }
                    hdCourtroomId.Value = courtroomId.ToString();
                    txtCourtroomName.Text = courtroom.CourtroomName;
                    drpCourthouse.SelectedValue = courtroom.CourthouseId.ToString();
                    ScriptManager.RegisterStartupScript(rptCourtroom, rptCourtroom.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptCourtroom_ItemCreated(object sender, RepeaterItemEventArgs e)
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
        protected void pnlCourtroom_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel"));
            methodInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { sender as UpdatePanel });
        }

        private void ClearForm()
        {
            txtCourtroomName.Text = string.Empty;
            drpCourthouse.SelectedIndex = 0;
            hdCourtroomId.Value = string.Empty;
        }

        private void PopulateCourthouseList()
        {
            drpCourthouse.DataTextField = "CourthouseName";
            drpCourthouse.DataValueField = "CourthouseId";
            drpCourthouse.DataSource = new CourthouseController(_hostSettings).GetCourthouses();
            drpCourthouse.DataBind();
        }

        private void PopulateCourtroomList()
        {
            rptCourtroom.DataSource = new CourtroomController(_hostSettings).GetCourtrooms();
            rptCourtroom.DataBind();
        }
    }
}
