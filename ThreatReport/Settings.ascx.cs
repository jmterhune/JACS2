using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;

namespace tjc.Modules.ThreatReport
{
    /// <summary>
    /// Module settings: the names of the notification role on this site and on the jud12 site.
    /// </summary>
    public partial class Settings : ThreatReportModuleSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (Settings.Contains("ViewerRole")) txtViewerRole.Text = Settings["ViewerRole"].ToString();
                    if (Settings.Contains("Jud12Role")) txtJud12Role.Text = Settings["Jud12Role"].ToString();
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
                modules.UpdateTabModuleSetting(TabModuleId, "ViewerRole", txtViewerRole.Text.Trim());
                modules.UpdateTabModuleSetting(TabModuleId, "Jud12Role", txtJud12Role.Text.Trim());
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
