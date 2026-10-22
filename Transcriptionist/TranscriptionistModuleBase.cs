using DotNetNuke.Abstractions;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;
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

        protected readonly IHostSettings _hostSettings;

        public TranscriptionistModuleBase()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
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

        // The dashboard, list, detail and edit controls are staff screens: module Admin or Manage permission.
        protected bool HasAccess()
        {
            return ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, "ADMIN")
                || ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, "MANAGE");
        }

        /// <summary>Sends anyone without access to the page's default view. True when the caller should stop.</summary>
        protected bool RedirectIfNoAccess()
        {
            if (HasAccess()) return false;
            Response.Redirect(DependencyProvider.GetRequiredService<INavigationManager>().NavigateURL(), true);
            return true;
        }

        #endregion
    }
}
