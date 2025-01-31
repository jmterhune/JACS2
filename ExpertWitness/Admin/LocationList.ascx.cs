/*
' Copyright (c) 2023  12th Judicial Circuit
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN Location OF
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
using tjc.Modules.ExpertWitness.Components;
namespace tjc.Modules.ExpertWitness
{
    public partial class LocationList : ExpertWitnessModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion

        #region Methods
        public LocationList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new LocationController();
            rptLocation.DataSource = ctl.GetLocations();
            rptLocation.DataBind();
        }
        private void ClearForm()
        {
            hdLocationId.Value = string.Empty;
            txtLocationName.Text = string.Empty;
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
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
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
            var ctl = new LocationController();
            Components.Location Location = new Components.Location();
            bool isNew = true;
            if (hdLocationId.Value != "")
            {
                isNew = false;
                Location = ctl.GetLocation(Convert.ToInt32(hdLocationId.Value));
            }
            Location.LocationName = txtLocationName.Text;
            Location.ModifiedDate = DateTime.Now;
            Location.ModifiedBy = UserInfo.Username;
            if (isNew)
            {
                Location.CreatedBy = UserInfo.Username;
                Location.CreatedDate = DateTime.Now;
                ctl.CreateLocation(Location);
            }
            else
            {
                ctl.UpdateLocation(Location);
            }
            ClearForm();
            BindList();
        }
        protected void pnlLocations_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptLocation_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int LocationId = Convert.ToInt32(e.CommandArgument);
            var ctl = new LocationController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteLocation(LocationId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Components.Location Location = ctl.GetLocation(LocationId);
                hdLocationId.Value = LocationId.ToString();
                txtLocationName.Text = Location.LocationName;
                ScriptManager.RegisterStartupScript(rptLocation, rptLocation.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptLocation_ItemCreated(object sender, RepeaterItemEventArgs e)
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