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
                if (!Page.IsPostBack)
                {
                    if (Settings.Contains(JCIDModuleBase.ExternalUrlSetting))
                        txtExternalUrl.Text = Settings[JCIDModuleBase.ExternalUrlSetting].ToString();
                    if (Settings.Contains(JCIDModuleBase.PaymentUrlSetting))
                        txtPaymentUrl.Text = Settings[JCIDModuleBase.PaymentUrlSetting].ToString();
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

                // Payment URL is a template containing {id}, so it is stored as entered rather than URL-validated.
                string payment = txtPaymentUrl.Text.Trim();
                if (payment.Length == 0)
                {
                    ModuleController.Instance.DeleteModuleSetting(ModuleId, JCIDModuleBase.PaymentUrlSetting);
                }
                else
                {
                    ModuleController.Instance.UpdateModuleSetting(ModuleId, JCIDModuleBase.PaymentUrlSetting, payment);
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
