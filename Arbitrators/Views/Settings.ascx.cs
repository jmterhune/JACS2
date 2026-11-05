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
                if (Settings.Contains("ExternalAppUrl"))
                    txtExternalUrl.Text = Settings["ExternalAppUrl"].ToString();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void valExternalUrl_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            Uri parsed;
            args.IsValid = Uri.TryCreate((args.Value ?? string.Empty).Trim(), UriKind.Absolute, out parsed) &&
                (parsed.Scheme == Uri.UriSchemeHttp || parsed.Scheme == Uri.UriSchemeHttps);
        }

        public override void UpdateSettings()
        {
            try
            {
                var modules = ModuleController.Instance;
                modules.UpdateModuleSetting(ModuleId, "NotificationFromEmail", txtFromEmail.Text);
                modules.UpdateModuleSetting(ModuleId, "NotificationCcEmail", txtCcEmail.Text);
                modules.UpdateModuleSetting(ModuleId, "ExternalAppUrl", txtExternalUrl.Text.Trim());
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
