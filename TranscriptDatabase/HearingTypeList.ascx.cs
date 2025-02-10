using DotNetNuke.Abstractions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.TranscriptDatabase.Components;

namespace tjc.Modules.TranscriptDatabase
{
    public partial class HearingTypeList : TranscriptDatabaseModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public HearingTypeList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new HearingTypeController();
            rptHearing.DataSource = ctl.GetHearingTypes();
            rptHearing.DataBind();
        }
        private void ClearForm()
        {
            hdHearingTypeId.Value = string.Empty;
            txtHearingType.Text = string.Empty;
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptHearing_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton cmdEdit = (LinkButton)e.Item.FindControl("cmdEdit");
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
                scriptMan.RegisterAsyncPostBackControl(cmdEdit);
            }
        }

        protected void rptHearing_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int hearingTypeId = Convert.ToInt32(e.CommandArgument);
            var ctl = new HearingTypeController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteHearingType(hearingTypeId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                HearingType hearingType = ctl.GetHearingType(hearingTypeId);
                hdHearingTypeId.Value = hearingTypeId.ToString();
                txtHearingType.Text = hearingType.HearingTypeName;
                ScriptManager.RegisterStartupScript(rptHearing, rptHearing.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }

        protected void pnlHearings_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new HearingTypeController();
            HearingType hearingType = new HearingType();
            bool isNew = true;
            if (hdHearingTypeId.Value != "")
            {
                isNew = false;
                hearingType = ctl.GetHearingType(Convert.ToInt32(hdHearingTypeId.Value));
            }
            hearingType.HearingTypeName = txtHearingType.Text;
            hearingType.LastModifiedDate = DateTime.Now;
            hearingType.LastModifiedByUser = UserId;
            if (isNew)
            {
                hearingType.CreatedByUser = UserId;
                hearingType.CreatedDate = DateTime.Now;
                ctl.CreateHearingType(hearingType);
            }
            else
            {
                ctl.UpdateHearingType(hearingType);
            }
            ClearForm();
            BindList();
        }
    }
}