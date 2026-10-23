/*
' Copyright (c) 2017  12th Judicial Circuit
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.JCID
{
    public class JCIDModuleBase : PortalModuleBase
    {
        public const string ExternalUrlSetting = "ExternalUrl";
        public const string PaymentUrlSetting = "PaymentUrl";

        protected readonly IHostSettings _hostSettings;

        public JCIDModuleBase()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }

        /// <summary>The application being worked on, from the "rid" query string. -1 when absent or invalid.</summary>
        public int RequestId
        {
            get
            {
                var qs = Request.QueryString["rid"];
                if (qs != null && Int32.TryParse(qs, out int requestId))
                    return requestId;
                return -1;
            }
        }

        /// <summary>
        /// The public (jud12) JCID applicant page, from the ExternalUrl setting. Null when the setting is blank
        /// or is not an absolute http(s) address, so a bad value never becomes a link.
        /// </summary>
        public string ExternalUrl
        {
            get { return ValidExternalUrl(Settings.Contains(ExternalUrlSetting) ? Settings[ExternalUrlSetting] as string : null); }
        }

        public static string ValidExternalUrl(string value)
        {
            Uri uri;
            value = (value ?? "").Trim();
            if (value.Length > 0 && Uri.TryCreate(value, UriKind.Absolute, out uri)
                && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
            {
                return uri.AbsoluteUri;
            }
            return null;
        }

        /// <summary>
        /// Tamper-proof encoding of a request id, matching the public site's Utilities.Encryptor, for the "eid"
        /// value the public JCID Form reads when an applicant follows a reject-correction link.
        /// </summary>
        public string EncryptRequestId(int requestId)
        {
            return new Components.Encryptor().QueryStringEncode(requestId.ToString());
        }

        /// <summary>
        /// Absolute address of an uploaded applicant photo on the jud12 site: the site part of ExternalUrl plus
        /// the photo's stored relative path. Returns the raw value unchanged when ExternalUrl is not set (so the
        /// image still shows if this module ever runs on the same site as the files).
        /// </summary>
        public string ExternalPhotoUrl(string photoUrl)
        {
            if (string.IsNullOrWhiteSpace(photoUrl)) return photoUrl;
            string app = ExternalUrl;
            if (app == null) return photoUrl;
            string path = photoUrl.Replace('\\', '/').TrimStart('/');
            return new Uri(new Uri(app), "/" + path).AbsoluteUri;
        }

        /// <summary>
        /// The payment-page URL template on the public (jud12) site, from the PaymentUrl setting. The full
        /// address is stored with "{id}" where the request id goes, e.g.
        /// https://.../Attorney/JCID-Application/ctl/payment/mid/395/rid/{id}. Null when the setting is blank.
        /// </summary>
        public string PaymentUrlTemplate
        {
            get
            {
                string v = Settings.Contains(PaymentUrlSetting) ? Settings[PaymentUrlSetting] as string : null;
                return string.IsNullOrWhiteSpace(v) ? null : v.Trim();
            }
        }

        /// <summary>
        /// The public payment page for an application, built from the PaymentUrl template with "{id}" replaced by
        /// the request id. Null when the setting is blank, so the copy-payment link is hidden until it is set.
        /// </summary>
        public string ExternalPaymentUrl(int requestId)
        {
            string template = PaymentUrlTemplate;
            if (template == null) return null;
            return template.Replace("{id}", requestId.ToString()).Replace("{ID}", requestId.ToString());
        }

        /// <summary>The public form, pre-filled for a rejected applicant to correct, or null when ExternalUrl is not set.</summary>
        public string ExternalFormFixUrl(int requestId)
        {
            string app = ExternalUrl;
            return app == null ? null : AppendQuery(app, "ctl=form&eid=" + EncryptRequestId(requestId));
        }

        private static string AppendQuery(string url, string query)
        {
            return url + (url.IndexOf('?') >= 0 ? "&" : "?") + query;
        }
    }
}
