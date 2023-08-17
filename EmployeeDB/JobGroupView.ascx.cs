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
    public partial class JobGroupView : EmployeeDBModuleBase
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

                    PopulateJobGroupList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptJobGroups_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int jobGroupId = Convert.ToInt32(e.CommandArgument);
            var ctl = new JobGroupController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteJobGroup(jobGroupId);
                PopulateJobGroupList();
            }
            if (e.CommandName == "edit")
            {
                JobGroup jobGroup = ctl.GetJobGroup(jobGroupId);

                hdJobGroupId.Value = jobGroupId.ToString();
                txtDescription.Text = jobGroup.Description;
                ScriptManager.RegisterStartupScript(rptJobGroups, rptJobGroups.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new JobGroupController();
            JobGroup jobGroup = new JobGroup();
            bool isNew = true;
            if (hdJobGroupId.Value != "")
            {
                isNew = false;
                jobGroup = ctl.GetJobGroup(Convert.ToInt32(hdJobGroupId.Value));
            }
            jobGroup.Description = txtDescription.Text;
            jobGroup.LastModifiedDate = DateTime.Now;
            jobGroup.LastModifiedById = UserId;
            if (isNew)
            {
                jobGroup.CreatedById = UserId;
                jobGroup.CreatedDate = DateTime.Now;
                ctl.CreateJobGroup(jobGroup);
            }
            else
            {
                ctl.UpdateJobGroup(jobGroup);
            }
            ClearForm();
            PopulateJobGroupList();
        }

        protected void pnlJobGroups_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void rptJobGroups_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
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
            hdJobGroupId.Value = string.Empty;
            txtDescription.Text = string.Empty;
        }
        private void PopulateJobGroupList()
        {
            var ctl = new JobGroupController();
            rptJobGroups.DataSource = ctl.GetJobGroups();
            rptJobGroups.DataBind();
        }
        #endregion

    }
}