using DotNetNuke.Abstractions;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Web;
using tjc.Modules.Transcriptionist.Components;

namespace tjc.Modules.Transcriptionist
{
    public class TranscriptionistModuleBase : PortalModuleBase
    {
        public const string ExternalAppUrlSetting = "ExternalAppUrl";

        // The *ByUserID columns are shared with the jud12 site, whose user ids mean different people than
        // this site's, so the staff user's id is not written. 0 marks "changed on the intranet".
        protected const int IntranetUserId = 0;

        protected readonly INavigationManager _navigationManager;

        protected readonly IHostSettings _hostSettings;

        public TranscriptionistModuleBase()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        // Plain URLs from the navigation manager rather than EditUrl: EditUrl() with no key resolves to the "edit"
        // control (here the add-applicant page), and for some controls it returns a popup javascript call.

        /// <summary>The dashboard (the module's default view).</summary>
        public string DashboardUrl { get { return _navigationManager.NavigateURL(); } }

        /// <summary>The full application list (the "list" control).</summary>
        public string ListUrl { get { return _navigationManager.NavigateURL(TabId, "list", "mid=" + ModuleId); } }

        /// <summary>The edit control with no "tid": the add-applicant form.</summary>
        public string AddUrl { get { return _navigationManager.NavigateURL(TabId, "edit", "mid=" + ModuleId); } }

        public string EditApplicationUrl(string transcriptionistId)
        {
            return _navigationManager.NavigateURL(TabId, "edit", "mid=" + ModuleId, "tid=" + transcriptionistId);
        }

        public string DetailUrl(string transcriptionistId)
        {
            return _navigationManager.NavigateURL(TabId, "detail", "mid=" + ModuleId, "tid=" + transcriptionistId);
        }

        /// <summary>The application being worked on, from the "tid" query string. 0 when absent or invalid.</summary>
        protected long TranscriptionistID
        {
            get
            {
                long id;
                return long.TryParse(Request.QueryString["tid"], out id) && id > 0 ? id : 0;
            }
        }

        /// <summary>
        /// The applicant-facing page on the jud12 (external) site, from the ExternalAppUrl setting. Null when the
        /// setting is blank or is not an absolute http(s) address, so a bad value never becomes a link.
        /// </summary>
        public string ExternalAppUrl
        {
            get { return ValidExternalUrl(Settings.Contains(ExternalAppUrlSetting) ? Settings[ExternalAppUrlSetting] as string : null); }
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
        /// Address of an uploaded document on the jud12 site: that site's address from ExternalAppUrl plus the
        /// portal files folder and the file's stored path. Null when ExternalAppUrl is not set.
        /// </summary>
        internal string ExternalFileUrl(string fullPath)
        {
            string app = ExternalAppUrl;
            if (app == null || string.IsNullOrWhiteSpace(fullPath)) return null;

            string path = fullPath.Replace('\\', '/').TrimStart('/');
            if (!path.StartsWith("Portals/", StringComparison.OrdinalIgnoreCase)) path = "Portals/0/" + path;

            string escaped = string.Join("/", path.Split('/').Select(Uri.EscapeDataString));
            return new Uri(new Uri(app), "/" + escaped).AbsoluteUri;
        }

        /// <summary>A link to an attachment's file on the jud12 site, or just its name when ExternalAppUrl is not set.</summary>
        protected string GetFileLink(object dataItem)
        {
            var attachment = (Attachment)dataItem;
            string name = HttpUtility.HtmlEncode(attachment.Filename ?? "");
            string url = ExternalFileUrl(attachment.FullPath);
            return url == null
                ? name
                : $"<a href=\"{HttpUtility.HtmlAttributeEncode(url)}\" target=\"_blank\" rel=\"noopener noreferrer\">{name}</a>";
        }

        // Column sizes in tjc_cat_applications; a longer value would make the update fail.
        protected static string Limit(string value, int max)
        {
            return value.Length > max ? value.Substring(0, max) : value;
        }

        #region Permission Checks

        // Who may open these screens is set by the page's View permission (the module is on its own page), the
        // same as the other intranet admin modules, so the module's own Edit/Admin permissions are not used.
        // This only turns away anyone who is not signed in.
        protected bool HasAccess()
        {
            return UserId > 0;
        }

        /// <summary>Sends anyone without access to the page's default view. True when the caller should stop.</summary>
        protected bool RedirectIfNoAccess()
        {
            if (HasAccess()) return false;
            Response.Redirect(DashboardUrl, true);
            return true;
        }

        #endregion
    }
}
