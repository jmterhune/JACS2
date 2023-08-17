/*
' Copyright (c) 2023  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Services.Exceptions;
using System;
using tjc.Modules.EmployeeDB.Components;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace tjc.Modules.EmployeeDB
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from EmployeeDBModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class LocationView : EmployeeDBModuleBase
    {
        #region Members
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }

                    PopulateOfficeLocationList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptOfficeLocations_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int officeOfficeLocationId = Convert.ToInt32(e.CommandArgument);
            var ctl = new OfficeLocationController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteOfficeLocation(officeOfficeLocationId);
                PopulateOfficeLocationList();
            }
            if (e.CommandName == "edit")
            {
                OfficeLocation officeOfficeLocation = ctl.GetOfficeLocation(officeOfficeLocationId);

                hdOfficeLocationId.Value = officeOfficeLocationId.ToString();
                txtDescription.Text = officeOfficeLocation.Description;
                ScriptManager.RegisterStartupScript(rptOfficeLocations, rptOfficeLocations.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new OfficeLocationController();
            OfficeLocation officeOfficeLocation = new OfficeLocation();
            bool isNew = true;
            if (hdOfficeLocationId.Value != "")
            {
                isNew = false;
                officeOfficeLocation = ctl.GetOfficeLocation(Convert.ToInt32(hdOfficeLocationId.Value));
            }
            officeOfficeLocation.Description = txtDescription.Text;
            officeOfficeLocation.LastModifiedDate = DateTime.Now;
            officeOfficeLocation.LastModifiedById = UserId;
            if (isNew)
            {
                officeOfficeLocation.CreatedById = UserId;
                officeOfficeLocation.CreatedDate = DateTime.Now;
                ctl.CreateOfficeLocation(officeOfficeLocation);
            }
            else
            {
                ctl.UpdateOfficeLocation(officeOfficeLocation);
            }
            ClearForm();
            PopulateOfficeLocationList();
        }
        protected void pnlOfficeLocations_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void rptOfficeLocations_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
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

        #region Methods
        private void ClearForm()
        {
            hdOfficeLocationId.Value = string.Empty;
            txtDescription.Text = string.Empty;
        }
        private void PopulateOfficeLocationList()
        {
            var ctl = new OfficeLocationController();
            rptOfficeLocations.DataSource = ctl.GetOfficeLocations();
            rptOfficeLocations.DataBind();
        }
        #endregion

    }
}