using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;
using System.Web.UI.WebControls;

namespace tjc.Modules.Transcriptionist
{
    /// <summary>Module settings: the link to the applicant-facing page on the jud12 site.</summary>
    public partial class Settings : TranscriptionistModuleSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (!Page.IsPostBack && Settings.Contains(TranscriptionistModuleBase.ExternalAppUrlSetting))
                {
                    txtExternalUrl.Text = Settings[TranscriptionistModuleBase.ExternalAppUrlSetting].ToString();
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
                string value = txtExternalUrl.Text.Trim();
                if (value.Length == 0)
                {
                    ModuleController.Instance.DeleteModuleSetting(ModuleId, TranscriptionistModuleBase.ExternalAppUrlSetting);
                }
                else if (TranscriptionistModuleBase.ValidExternalUrl(value) != null)
                {
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, TranscriptionistModuleBase.ExternalAppUrlSetting, value);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void valExternalUrl_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = TranscriptionistModuleBase.ValidExternalUrl(args.Value) != null;
        }
    }
}
