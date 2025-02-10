/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.TranscriptDatabase.Components;
using DotNetNuke.Common;
using System.Collections;
using DotNetNuke.Common.Lists;
using System.Collections.Generic;
namespace tjc.Modules.TranscriptDatabase
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from TranscriptDatabaseModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class AttorneyList : TranscriptDatabaseModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public AttorneyList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindDropDowns()
        {
            var ctl = new ListController();
            IEnumerable<ListEntryInfo> states = ctl.GetListEntryInfoItems("Region", "Country.US");
            drpState.DataSource = states;
            drpState.DataTextField = "Text";
            drpState.DataValueField = "Value";
            drpState.DataBind();

            var lCtl = new OfficeController();
            IEnumerable<Office> offices=lCtl.GetOffices();
            drpOffice.DataSource = offices;
            drpOffice.DataTextField = "Description";
            drpOffice.DataValueField= "OfficeID";
            drpOffice.DataBind();
        }
        private void BindList()
        {
            var ctl = new AttorneyController();
            rptAttorney.DataSource = ctl.GetAttorneys();
            rptAttorney.DataBind();
        }
        private void ClearForm()
        {
            hdAttorneyId.Value = string.Empty;
            txtAddress.Text = string.Empty;
            txtAddress2.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtMiddleName.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtZip.Text = string.Empty;
            drpOffice.SelectedIndex = 0;
            drpState.SelectedIndex = 0;
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
                    BindDropDowns();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptAttorneys_ItemCreated(object sender, RepeaterItemEventArgs e)
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

        protected void rptAttorneys_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int attorneyId = Convert.ToInt32(e.CommandArgument);
            var ctl = new AttorneyController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteAttorney(attorneyId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Attorney attorney = ctl.GetAttorney(attorneyId);
                hdAttorneyId.Value = attorneyId.ToString();
                txtAddress.Text = attorney.Address1;
                txtCity.Text = attorney.City;
                txtAddress2.Text = attorney.Address2;
                txtZip.Text = attorney.ZipCode;
                txtFirstName.Text = attorney.FirstName;
                txtLastName.Text = attorney.LastName;
                txtMiddleName.Text = attorney.MiddleName;
                drpOffice.SelectedValue = attorney.OfficeID.ToString();
                drpState.SelectedValue = attorney.State;
                ScriptManager.RegisterStartupScript(rptAttorney, rptAttorney.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }

        protected void pnlAttorneys_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new AttorneyController();
            Attorney attorney = new Attorney();
            bool isNew = true;
            if (hdAttorneyId.Value != "")
            {
                isNew = false;
                attorney = ctl.GetAttorney(Convert.ToInt32(hdAttorneyId.Value));
            }
            attorney.Address1 = txtAddress.Text;
            attorney.Address2 = txtAddress2.Text;
            attorney.FirstName = txtFirstName.Text;
            attorney.LastName = txtLastName.Text;
            attorney.MiddleName = txtMiddleName.Text;
            attorney.City = txtCity.Text;
            attorney.ZipCode = txtZip.Text;
            if (drpOffice.SelectedIndex > 0)
                attorney.OfficeID = Int32.Parse(drpOffice.SelectedValue);
            attorney.State = drpState.SelectedValue;
            attorney.LastModifiedDate = DateTime.Now;
            attorney.LastModifiedByUser = UserId;
            if (isNew)
            {
                attorney.CreatedByUser = UserId;
                attorney.CreatedDate = DateTime.Now;
                ctl.CreateAttorney(attorney);
            }
            else
            {
                ctl.UpdateAttorney(attorney);
            }
            ClearForm();
            BindList();
        }
    }
}