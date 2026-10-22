using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dnn.PersonaBar.Library;
using Dnn.PersonaBar.Library.Attributes;
using DotNetNuke.Instrumentation;
using DotNetNuke.Web.Api;
using Tjc.Modules.ConfigEncryption.Components;

namespace Tjc.Modules.ConfigEncryption.Services
{
    [MenuPermission(Scope = ServiceScope.Host)]
    public class ConfigProtectionController : PersonaBarApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ConfigProtectionController));

        public class SectionRequest
        {
            public string Section { get; set; }
            public string Provider { get; set; }
        }

        /// GET: API/PersonaBar/ConfigProtection/GetStatus
        [HttpGet]
        public HttpResponseMessage GetStatus()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Sections = ConfigProtectionManager.GetStatus(),
                    Providers = ConfigProtectionManager.Providers,
                    DefaultProvider = ConfigProtectionManager.RsaProvider,
                });
            }
            catch (Exception ex)
            {
                return Fail(ex);
            }
        }

        /// POST: API/PersonaBar/ConfigProtection/Protect
        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Protect(SectionRequest request)
        {
            try
            {
                if (request == null) throw new ArgumentException("No request body.");
                ConfigProtectionManager.Protect(request.Section, request.Provider);
                Logger.Info("web.config section '" + request.Section + "' encrypted by " + UserInfo.Username);
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = true });
            }
            catch (Exception ex)
            {
                return Fail(ex);
            }
        }

        /// POST: API/PersonaBar/ConfigProtection/Unprotect
        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Unprotect(SectionRequest request)
        {
            try
            {
                if (request == null) throw new ArgumentException("No request body.");
                ConfigProtectionManager.Unprotect(request.Section);
                Logger.Info("web.config section '" + request.Section + "' decrypted by " + UserInfo.Username);
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = true });
            }
            catch (Exception ex)
            {
                return Fail(ex);
            }
        }

        private HttpResponseMessage Fail(Exception ex)
        {
            Logger.Error(ex);
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Success = false, Message = ex.Message });
        }
    }
}
