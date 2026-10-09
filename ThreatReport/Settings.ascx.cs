using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;

namespace tjc.Modules.ThreatReport
{
    /// <summary>
    /// Module settings shared by the incident list and the submit-incident modules: the role names that
    /// control access and notifications, and the pages the two modules link to.
    /// </summary>
    public partial class Settings : ThreatReportModuleSettingsBase
    {
        private static readonly string[] Keys = { "ViewerRole", "JudgesRole", "Jud12Role", "ViewTabID", "EditTabID" };

        private System.Web.UI.WebControls.TextBox[] Boxes
        {
            get { return new[] { txtViewerRole, txtJudgesRole, txtJud12Role, txtViewTab, txtEditTab }; }
        }

        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack) return;
                var boxes = Boxes;
                for (int i = 0; i < Keys.Length; i++)
                {
                    if (Settings.Contains(Keys[i])) boxes[i].Text = Settings[Keys[i]].ToString();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public override void UpdateSettings()
        {
            try
            {
                var modules = ModuleController.Instance;
                var boxes = Boxes;
                for (int i = 0; i < Keys.Length; i++)
                {
                    modules.UpdateTabModuleSetting(TabModuleId, Keys[i], boxes[i].Text.Trim());
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
