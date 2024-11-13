using DotNetNuke.Abstractions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.HearingLog.Components;

namespace tjc.Modules.HearingLog
{
    public partial class JudgeAdmin : HearingsLogModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public JudgeAdmin() => _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    var judges = RoleController.Instance.GetUsersByRole(PortalId, JudgeRole);
                    var jas = RoleController.Instance.GetUsersByRole(PortalId, JaRole);
                    drpJudge.DataSource = judges.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
                    drpJudge.DataTextField = "DisplayName";
                    drpJudge.DataValueField = "UserID";
                    drpJudge.DataBind();
                    drpJudge.Items.Insert(0, new ListItem("< Select Judge >", ""));
                    drpJA.DataSource = jas.OrderBy(x=>x.LastName).ThenBy(x=>x.FirstName);
                    drpJA.DataTextField = "DisplayName";
                    drpJA.DataValueField = "UserID";
                    drpJA.DataBind();
                    drpJA.Items.Insert(0, new ListItem("< Select JA >", ""));
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSaveJudge_Click(object sender, EventArgs e)
        {
            Int32.TryParse(drpJudge.SelectedValue, out int judgeUserId);
            Int32.TryParse(drpJA.SelectedValue, out int jaUserId);
            var ctl = new JudgeController();
            if (judgeUserId > 0)
                ctl.DeleteJudgeJaRef(judgeUserId);
            if (judgeUserId > 0 && jaUserId > 0)
            {
                JudgeJa judgeJa = ctl.GetJudgeJaRef(judgeUserId);
                if (judgeJa == null || judgeJa.JaUserID != jaUserId)
                {
                    ctl.CreateJudgeJaRef(judgeUserId, jaUserId);
                }
            }
            ctl.DeleteJacsJudgesByUserRef(judgeUserId,drpCounty.SelectedValue);
            foreach (ListItem item in chlJacsJudges.Items)
            {
                if (item.Selected)
                {
                    Int32.TryParse(item.Value, out int jacsUserId);
                    ctl.CreateJacsJudgeByUserRef(jacsUserId, judgeUserId);
                }
            }
        }

        protected void pnlJacsJudges_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void drpCounty_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateJacsJudges();
            if (drpJudge.SelectedIndex > 0)
            {
                Int32.TryParse(drpJudge.SelectedValue, out var judgeUserId);
                SelectJacsJudges(judgeUserId);
            }
        }

        protected void drpJudge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpJudge.SelectedIndex > 0)
            {
                var ctl = new JudgeController();
                Int32.TryParse(drpJudge.SelectedValue, out var judgeUserId);
                JudgeJa judgeJa = ctl.GetJudgeJaRef(judgeUserId);
                if (judgeJa != null)
                {
                    drpJA.SelectedValue = judgeJa.JaUserID.ToString();
                }
                else
                {
                    drpJA.SelectedIndex = 0;
                }
                if (judgeUserId > 0)
                {
                    PopulateJacsJudges(); 
                    SelectJacsJudges(judgeUserId);
                }
            }
            else
            {
                drpJA.SelectedIndex = 0;
                chlJacsJudges.ClearSelection();
            }
        }
        private void SelectJacsJudges(int judgeUserId)
        {
            if (drpJudge.SelectedIndex > 0 && drpCounty.SelectedIndex > 0)
            {
                var ctl = new JudgeController();
                var userJacsJudges = ctl.GetJacsJudgeByUserRef(judgeUserId, drpCounty.SelectedValue);
                foreach (JacsJudge jj in userJacsJudges)
                {
                    chlJacsJudges.Items.FindByValue(jj.JacsUserID.ToString()).Selected = true;
                }
            }
        }
        private void PopulateJacsJudges()
        {
            if (drpCounty.SelectedIndex > 0)
            {
                var ctl = new JudgeController();
                var jacsJudges = ctl.GetJacsJudgeByCounty(drpCounty.SelectedValue);
                chlJacsJudges.DataSource = jacsJudges.OrderBy(x => x.JudgeName);
                chlJacsJudges.DataTextField = "JudgeName";
                chlJacsJudges.DataValueField = "JacsUserID";
                chlJacsJudges.DataBind();
                Int32.TryParse(drpJudge.SelectedValue, out var judgeUserId);
                var existingJudges = ctl.GetExistingJacsJudges(drpCounty.SelectedValue, judgeUserId);
                foreach (var jj in existingJudges)
                {
                    var item = chlJacsJudges.Items.FindByValue(jj.JACSUserID.ToString());
                    item.Enabled = false;
                    item.Attributes.Add("title", string.Format("{0} is already assigned to {1}", item.Text, jj.Name));
                }
            }
            else
            {
                chlJacsJudges.Items.Clear();
            }
        }
    }
}