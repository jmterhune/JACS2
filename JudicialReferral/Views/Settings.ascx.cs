/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;

namespace tjc.Modules.JudicialReferral.Views
{
    public partial class Settings : JudicialReferralSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Settings.Contains("JudgeRole"))
                        txtJudgeRole.Text = Settings["JudgeRole"].ToString();
                    if (Settings.Contains("JaRole"))
                        txtJaRole.Text = Settings["JaRole"].ToString();
                    if (Settings.Contains("CounselRole"))
                        txtCounselRole.Text = Settings["CounselRole"].ToString();
                    if (Settings.Contains("CourtCounselEmail"))
                        txtCounselEmail.Text = Settings["CourtCounselEmail"].ToString();
                    if (Settings.Contains("FolderName"))
                        txtFolderName.Text = Settings["FolderName"].ToString();
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
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "JudgeRole", txtJudgeRole.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "JaRole", txtJaRole.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "CounselRole", txtCounselRole.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "CourtCounselEmail", txtCounselEmail.Text);
                ModuleController.Instance.UpdateModuleSetting(ModuleId, "FolderName", txtFolderName.Text);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
