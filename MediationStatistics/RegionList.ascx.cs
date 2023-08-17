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

using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using System;
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
    public partial class RegionList : MediationStatisticsModuleBase
    {

        #region Methods
        private void BindList()
        {
            var ctl = new RegionController();
            rptRegion.DataSource = ctl.GetRegions();
            rptRegion.DataBind();
        }
        private void ClearForm()
        {
            hdRegionId.Value = string.Empty;
            txtRegion.Text = string.Empty;
            chkActive.Checked = false;
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                chkActive.InputAttributes.Add("class", "form-check-input");
                chkActive.LabelAttributes.Add("class", "form-check-label");

                if (!IsPostBack)
                {
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
            var ctl = new RegionController();
            Region region = new Region();
            bool isNew = true;
            if (hdRegionId.Value != "")
            {
                isNew = false;
                region = ctl.GetRegion(Convert.ToInt32(hdRegionId.Value));
            }
            region.Description = txtRegion.Text;
            region.Active = chkActive.Checked;
            region.LastModifiedDate = DateTime.Now;
            region.LastModifiedById = UserId;
            if (isNew)
            {
                region.CreatedById = UserId;
                region.CreatedDate = DateTime.Now;
                ctl.CreateRegion(region);
            }
            else
            {
                ctl.UpdateRegion(region);
            }
            ClearForm();
            BindList();
        }
        protected void pnlRegions_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptRegion_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int regionId = Convert.ToInt32(e.CommandArgument);
            var ctl = new RegionController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteRegion(regionId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Region region = ctl.GetRegion(regionId);
                hdRegionId.Value = regionId.ToString();
                txtRegion.Text = region.Description;
                chkActive.Checked = region.Active;
                ScriptManager.RegisterStartupScript(rptRegion, rptRegion.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptRegion_ItemCreated(object sender, RepeaterItemEventArgs e)
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