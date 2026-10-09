/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;
using tjc.Modules.FileManager.Components;

namespace tjc.Modules.FileManager.Views
{
    public partial class Settings : ModuleSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack) return;

                if (Settings.Contains("ShareKey"))
                    txtShareKey.Text = Settings["ShareKey"].ToString();

                // Report only whether the connection to the public site is configured, never its values.
                lblStatus.Text = new RemoteFileClient().IsConfigured
                    ? "Connection to the public site is configured."
                    : "Connection to the public site is NOT configured. Set " + RemoteFileClient.BaseUrlSetting
                      + " and " + RemoteFileClient.SecretSetting + " in web.config.";
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
                if (!Page.IsValid) return;

                ModuleController.Instance.UpdateModuleSetting(ModuleId, "ShareKey", txtShareKey.Text.Trim());
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
