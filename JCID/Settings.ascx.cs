using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;
using System.Web.UI.WebControls;

namespace tjc.Modules.JCID
{
    /// <summary>Module settings: the link to the applicant-facing JCID page on the jud12 (public) site.</summary>
    public partial class Settings : JCIDModuleSettingsBase
    {
        public override void LoadSettings()
        {
            try
            {
                if (!Page.IsPostBack && Settings.Contains(JCIDModuleBase.ExternalUrlSetting))
                {
                    txtExternalUrl.Text = Settings[JCIDModuleBase.ExternalUrlSetting].ToString();
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
                    ModuleController.Instance.DeleteModuleSetting(ModuleId, JCIDModuleBase.ExternalUrlSetting);
                }
                else if (JCIDModuleBase.ValidExternalUrl(value) != null)
                {
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, JCIDModuleBase.ExternalUrlSetting, value);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void valExternalUrl_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = JCIDModuleBase.ValidExternalUrl(args.Value) != null;
        }
    }
}
