/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;

namespace tjc.Modules.Arbitrators.Views
{
    public partial class Settings : ArbitratorsModuleSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack) return;

                if (Settings.Contains("NotificationFromEmail"))
                    txtFromEmail.Text = Settings["NotificationFromEmail"].ToString();
                if (Settings.Contains("NotificationCcEmail"))
                    txtCcEmail.Text = Settings["NotificationCcEmail"].ToString();
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
                modules.UpdateModuleSetting(ModuleId, "NotificationFromEmail", txtFromEmail.Text);
                modules.UpdateModuleSetting(ModuleId, "NotificationCcEmail", txtCcEmail.Text);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
