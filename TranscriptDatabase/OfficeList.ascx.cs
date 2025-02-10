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
    public partial class OfficeList : TranscriptDatabaseModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public OfficeList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new OfficeController();
            rptOffice.DataSource = ctl.GetOffices();
            rptOffice.DataBind();
        }
        private void ClearForm()
        {
            hdOfficeId.Value = string.Empty;
            txtDescription.Text = string.Empty;
            drpDeliveryType.SelectedIndex = 0;
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
                    var deliveryTypes = Enumerations.GetValues<DeliveryTypes>();
                    foreach (DeliveryTypes deliveryType in deliveryTypes)
                    {
                        drpDeliveryType.Items.Add(new ListItem(Enumerations.GetEnumDescription(deliveryType), deliveryType.ToString()));
                    }

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptOffices_ItemCreated(object sender, RepeaterItemEventArgs e)
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

        protected void rptOffices_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int officeId = Convert.ToInt32(e.CommandArgument);
            var ctl = new OfficeController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteOffice(officeId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Office office = ctl.GetOffice(officeId);
                hdOfficeId.Value = officeId.ToString();
                txtDescription.Text = office.Description;
                drpDeliveryType.SelectedValue = office.DeliveryTypeID.ToString();
                ScriptManager.RegisterStartupScript(rptOffice, rptOffice.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }

        protected void pnlOffices_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new OfficeController();
            Office office = new Office();
            bool isNew = true;
            if (hdOfficeId.Value != "")
            {
                isNew = false;
                office = ctl.GetOffice(Convert.ToInt32(hdOfficeId.Value));
            }
            office.Description = txtDescription.Text;
            if (drpDeliveryType.SelectedIndex > 0)
                office.DeliveryTypeID = Int32.Parse(drpDeliveryType.SelectedValue);
            office.LastModifiedDate = DateTime.Now;
            office.LastModifiedByUser = UserId;
            if (isNew)
            {
                office.CreatedByUser = UserId;
                office.CreatedDate = DateTime.Now;
                ctl.CreateOffice(office);
            }
            else
            {
                ctl.UpdateOffice(office);
            }
            ClearForm();
            BindList();
        }
    }
}