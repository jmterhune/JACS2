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
using System.Collections.Generic;
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
    public partial class GroupEdit : MediationStatisticsModuleBase
    {

        #region Methods

        private void PopulateCaseTypeGroupLists(GroupController ctl)
        {
            lsAvailableCaseType.DataSource = ctl.GetCaseTypesExcludedByGroup(GroupID);
            lsAvailableCaseType.DataBind();
            lsSelectedCaseType.DataSource = ctl.GetCaseTypesByGroup(GroupID);
            lsSelectedCaseType.DataBind();
        }
        private void PopulateAppearanceGroupLists(GroupController ctl)
        {
            lsAvailableAppearance.DataSource = ctl.GetAppearancesExcludedByGroup(GroupID);
            lsAvailableAppearance.DataBind();
            lsSelectedAppearance.DataSource = ctl.GetAppearancesByGroup(GroupID);
            lsSelectedAppearance.DataBind();
        }
        private void PopulateIssueGroupLists(GroupController ctl)
        {
            lsAvailableIssues.DataSource = ctl.GetIssuesExcludedByGroup(GroupID);
            lsAvailableIssues.DataBind();
            lsSelectedIssues.DataSource = ctl.GetIssuesByGroup(GroupID);
            lsSelectedIssues.DataBind();
        }

        private void SortAppearanceGroupList(GroupController ctl)
        {
            for (int i = 0; i < lsSelectedAppearance.Items.Count; i++)
            {
                ListItem item = lsSelectedAppearance.Items[i];
                Int32.TryParse(item.Value, out int appearanceId);
                AppearanceGroup appearanceGroup = ctl.GetAppearanceGroup(GroupID, appearanceId);
                appearanceGroup.SortOrder = i;
                appearanceGroup.LastModifiedById = UserId;
                if (appearanceGroup != null)
                {
                    ctl.UpdateAppearanceGroup(appearanceGroup);
                }
            }
        }
        private void SortCaseTypeGroupList(GroupController ctl)
        {
            for (int i = 0; i < lsSelectedCaseType.Items.Count; i++)
            {
                ListItem item = lsSelectedCaseType.Items[i];
                Int32.TryParse(item.Value, out int caseTypeId);
                CaseTypeGroup caseTypeGroup = ctl.GetCaseTypeGroup(GroupID, caseTypeId);
                caseTypeGroup.SortOrder = i;
                caseTypeGroup.LastModifiedById = UserId;
                if (caseTypeGroup != null)
                {
                    ctl.UpdateCaseTypeGroup(caseTypeGroup);
                }
            }
        }
        private void SortIssueGroupList(GroupController ctl)
        {
            for (int i = 0; i < lsSelectedIssues.Items.Count; i++)
            {
                ListItem item = lsSelectedIssues.Items[i];
                Int32.TryParse(item.Value, out int issueId);
                IssueGroup issueGroup = ctl.GetIssueGroup(GroupID, issueId);
                issueGroup.SortOrder = i;
                issueGroup.LastModifiedById = UserId;
                if (issueGroup != null)
                {
                    ctl.UpdateIssueGroup(issueGroup);
                }
            }
        }

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

                    if (GroupID > 0)
                    {
                        var ctl = new GroupController();
                        Group group = ctl.GetGroup(GroupID);
                        ltInfo.Text = string.Format(ltInfo.Text, group.Description);
                        PopulateCaseTypeGroupLists(ctl);
                        PopulateAppearanceGroupLists(ctl);
                        PopulateIssueGroupLists(ctl);
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            SortCaseTypeGroupList(ctl);
            SortAppearanceGroupList(ctl);
            SortIssueGroupList(ctl);
        }
        protected void pnlGroups_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void cmdAddCaseType_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            foreach (ListItem item in lsAvailableCaseType.Items)
            {
                if (item.Selected)
                {
                    Int32.TryParse(item.Value, out int caseTypeId);
                    CaseTypeGroup caseTypeGroup = new CaseTypeGroup { GroupId = GroupID, CaseTypeId = caseTypeId, SortOrder = lsSelectedCaseType.Items.Count, CreatedById = UserId, CreatedDate = DateTime.Now, LastModifiedById = UserId, LastModifiedDate = DateTime.Now };
                    ctl.CreateCaseTypeGroup(caseTypeGroup);
                }
            }
            PopulateCaseTypeGroupLists(ctl);

        }
        protected void cmdRemoveCaseType_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            foreach (ListItem item in lsSelectedCaseType.Items)
            {
                if (item.Selected)
                {
                    Int32.TryParse(item.Value, out int caseTypeId);
                    CaseTypeGroup caseTypeGroup = ctl.GetCaseTypeGroup(GroupID, caseTypeId);
                    if (caseTypeGroup != null)
                    {
                        ctl.DeleteCaseTypeGroup(caseTypeGroup);
                    }
                }
            }
            PopulateCaseTypeGroupLists(ctl);

        }
        protected void cmdAddAppearance_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            foreach (ListItem item in lsAvailableAppearance.Items)
            {
                if (item.Selected)
                {
                    Int32.TryParse(item.Value, out int appearanceId);
                    AppearanceGroup appearanceGroup = new AppearanceGroup { GroupId = GroupID, AppearanceId = appearanceId, SortOrder = lsSelectedAppearance.Items.Count, CreatedById = UserId, CreatedDate = DateTime.Now, LastModifiedById = UserId, LastModifiedDate = DateTime.Now };
                    ctl.CreateAppearanceGroup(appearanceGroup);
                }
            }
            PopulateAppearanceGroupLists(ctl);

        }
        protected void cmdRemoveAppearance_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            foreach (ListItem item in lsSelectedAppearance.Items)
            {
                if (item.Selected)
                {
                    Int32.TryParse(item.Value, out int appearanceId);
                    AppearanceGroup appearanceGroup = ctl.GetAppearanceGroup(GroupID, appearanceId);
                    if (appearanceGroup != null)
                    {
                        ctl.DeleteAppearanceGroup(appearanceGroup);
                    }
                }
            }
            PopulateAppearanceGroupLists(ctl);

        }
        protected void cmdAddIssue_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            foreach (ListItem item in lsAvailableIssues.Items)
            {
                if (item.Selected)
                {
                    Int32.TryParse(item.Value, out int issueId);
                    IssueGroup issueGroup = new IssueGroup { GroupId = GroupID, IssueId = issueId, SortOrder = lsSelectedIssues.Items.Count, CreatedById = UserId, CreatedDate = DateTime.Now, LastModifiedById = UserId, LastModifiedDate = DateTime.Now };
                    ctl.CreateIssueGroup(issueGroup);
                }
            }
            PopulateIssueGroupLists(ctl);

        }
        protected void cmdRemoveIssue_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            foreach (ListItem item in lsSelectedAppearance.Items)
            {
                if (item.Selected)
                {
                    Int32.TryParse(item.Value, out int appearanceId);
                    IssueGroup appearanceGroup = ctl.GetIssueGroup(GroupID, appearanceId);
                    if (appearanceGroup != null)
                    {
                        ctl.DeleteIssueGroup(appearanceGroup);
                    }
                }
            }
            PopulateIssueGroupLists(ctl);

        }
        protected void cmdMoveUpCaseType_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            Helper.MoveSelectedItemUp(lsSelectedCaseType);
            SortCaseTypeGroupList(ctl);
            PopulateCaseTypeGroupLists(ctl);
        }
        protected void cmdMoveDownCaseType_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            Helper.MoveSelectedItemDown(lsSelectedCaseType);
            SortCaseTypeGroupList(ctl);
            PopulateCaseTypeGroupLists(ctl);
        }

        protected void cmdMoveUpAppearance_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            Helper.MoveSelectedItemUp(lsSelectedAppearance);
            SortCaseTypeGroupList(ctl);
            PopulateCaseTypeGroupLists(ctl);
        }

        protected void cmdMoveDownAppearance_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            Helper.MoveSelectedItemDown(lsSelectedAppearance);
            SortCaseTypeGroupList(ctl);
            PopulateCaseTypeGroupLists(ctl);
        }

        protected void cmdMoveUpIssue_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            Helper.MoveSelectedItemUp(lsSelectedIssues);
            SortCaseTypeGroupList(ctl);
            PopulateCaseTypeGroupLists(ctl);
        }

        protected void cmdMoveDownIssue_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            Helper.MoveSelectedItemDown(lsSelectedIssues);
            SortCaseTypeGroupList(ctl);
            PopulateCaseTypeGroupLists(ctl);
        }

        #endregion


    }
}