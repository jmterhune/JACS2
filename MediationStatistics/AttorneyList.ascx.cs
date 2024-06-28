/*
' Copyright (c) 2023  12th Judicial Circuit
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
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.MediationStatistics.Components;

namespace tjc.Modules.MediationStatistics
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from MediationStatisticsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class AttorneyList : MediationStatisticsModuleBase
    {
        private readonly INavigationManager _navigationManager;

        #region Methods
        public AttorneyList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
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
            txtCity.Text = string.Empty;
            txtExtension.Text = string.Empty;
            txtFirm.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtPhone.Text = string.Empty;
            drpState.SelectedIndex = 0;
            txtZip.Text = string.Empty;
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
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
            attorney.FirstName = txtFirstName.Text;
            attorney.LastName = txtLastName.Text;
            attorney.Phone = txtPhone.Text;
            attorney.Firm=txtFirm.Text;
            attorney.Email = txtEmail.Text;
            attorney.Address = txtAddress.Text;
            attorney.Extension = txtExtension.Text;
            attorney.City = txtCity.Text;
            attorney.State = drpState.SelectedValue;
            attorney.Zip = txtZip.Text;
            attorney.LastModifiedDate = DateTime.Now;
            attorney.LastModifiedById = UserId;
            if (isNew)
            {
                attorney.CreatedById = UserId;
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
        protected void pnlAttorneys_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptAttorney_ItemCommand(object source, RepeaterCommandEventArgs e)
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
                txtFirstName.Text = attorney.FirstName;
                txtLastName.Text = attorney.LastName;
                txtFirm.Text = attorney.Firm;
                txtEmail.Text = attorney.Email;
                txtPhone.Text = attorney.Phone;
                txtExtension.Text = attorney.Extension;
                txtAddress.Text = attorney.Address;
                txtCity.Text = attorney.City;
                drpState.SelectedValue = attorney.State;
                txtZip.Text = attorney.Zip;
                ScriptManager.RegisterStartupScript(rptAttorney, rptAttorney.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
 protected void rptAttorney_ItemCreated(object sender, RepeaterItemEventArgs e)
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
        #endregion

       
    }
}