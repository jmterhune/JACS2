using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;

namespace tjc.Modules.ProcessServer
{
    /// <summary>
    /// Module settings for the internal Process Server admin module: the roles that gate the
    /// management and approval screens, the notification from-address, and the enrollment window
    /// that determines the application year. The payment-gateway settings stay on the public module.
    /// </summary>
    public partial class Settings : ProcessServerModuleSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (Settings.Contains("AdminRole"))
                        txtAdminRole.Text = Settings["AdminRole"].ToString();
                    if (Settings.Contains("ChiefJudgeRole"))
                        txtChiefJudgeRole.Text = Settings["ChiefJudgeRole"].ToString();
                    if (Settings.Contains("CourtCounselRole"))
                        txtCourtCounselRole.Text = Settings["CourtCounselRole"].ToString();
                    if (Settings.Contains("NotificationEmail"))
                        txtNotificationEmail.Text = Settings["NotificationEmail"].ToString();
                    if (Settings.Contains("EnrollmentStart"))
                        txtEnrollmentStart.Text = Settings["EnrollmentStart"].ToString();
                    if (Settings.Contains("EnrollmentEnd"))
                        txtEnrollmentEnd.Text = Settings["EnrollmentEnd"].ToString();
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
                modules.UpdateModuleSetting(ModuleId, "AdminRole", txtAdminRole.Text);
                modules.UpdateModuleSetting(ModuleId, "ChiefJudgeRole", txtChiefJudgeRole.Text);
                modules.UpdateModuleSetting(ModuleId, "CourtCounselRole", txtCourtCounselRole.Text);
                modules.UpdateModuleSetting(ModuleId, "NotificationEmail", txtNotificationEmail.Text);
                modules.UpdateModuleSetting(ModuleId, "EnrollmentStart", txtEnrollmentStart.Text);
                modules.UpdateModuleSetting(ModuleId, "EnrollmentEnd", txtEnrollmentEnd.Text);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
