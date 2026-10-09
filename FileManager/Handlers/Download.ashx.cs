/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using tjc.Modules.FileManager.Components;

namespace tjc.Modules.FileManager.Handlers
{
    /// <summary>
    /// Streams a file from the public site to a user who may view the FileManager module that lists it.
    /// The caller names only the module and a relative path; the share comes from that module's own
    /// settings, so a user can't ask for a share other than the one the module is set up to show.
    /// </summary>
    public class Download : IHttpHandler
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(Download));
        private const int BufferSize = 81920;

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            string path = request.QueryString["p"];
            if (!int.TryParse(request.QueryString["tabid"], out int tabId) ||
                !int.TryParse(request.QueryString["mid"], out int moduleId) ||
                !IsSafeRelativePath(path))
            {
                Fail(response, 404);
                return;
            }

            ModuleInfo module = ModuleController.Instance.GetModule(moduleId, tabId, false);
            PortalSettings portal = PortalSettings.Current;
            if (module == null || module.IsDeleted || portal == null || module.PortalID != portal.PortalId ||
                module.DesktopModule == null ||
                !string.Equals(module.DesktopModule.ModuleName, "FileManager", StringComparison.OrdinalIgnoreCase))
            {
                Fail(response, 404);
                return;
            }

            if (!ModulePermissionController.CanViewModule(module))
            {
                Fail(response, 403);
                return;
            }

            string share = Convert.ToString(module.ModuleSettings["ShareKey"]);
            var client = new RemoteFileClient();
            if (string.IsNullOrWhiteSpace(share) || !client.IsConfigured)
            {
                Fail(response, 404);
                return;
            }

            UserInfo user = UserController.Instance.GetCurrentUserInfo();
            Logger.InfoFormat("FileManager download: user={0} module={1} share={2} path={3}", user.Username, moduleId, share, path);

            try
            {
                using (HttpResponseMessage remote = client.OpenFile(share, path))
                {
                    if (!remote.IsSuccessStatusCode)
                    {
                        Fail(response, remote.StatusCode == System.Net.HttpStatusCode.NotFound ? 404 : 502);
                        return;
                    }

                    Stream(remote, path, response);
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                Exceptions.LogException(ex);
                Fail(response, 502);
            }
        }

        private static void Stream(HttpResponseMessage remote, string path, HttpResponse response)
        {
            string fileName = path.Substring(path.LastIndexOf('/') + 1);

            response.Clear();
            response.BufferOutput = false;
            response.ContentType = MimeMapping.GetMimeMapping(fileName);
            response.AddHeader("Content-Disposition", new ContentDisposition { FileName = fileName, Inline = true }.ToString());
            response.AddHeader("X-Content-Type-Options", "nosniff");
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();
            if (remote.Content.Headers.ContentLength.HasValue)
            {
                response.AddHeader("Content-Length", remote.Content.Headers.ContentLength.Value.ToString());
            }

            using (System.IO.Stream source = remote.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult())
            {
                var buffer = new byte[BufferSize];
                int read;
                while (response.IsClientConnected && (read = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    response.OutputStream.Write(buffer, 0, read);
                }
            }

            response.Flush();
        }

        /// <summary>Rejects anything that isn't a plain relative path like "folder/file.pdf".</summary>
        private static bool IsSafeRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path) || path.Length > 1024 || path[0] == '/' ||
                path.IndexOfAny(new[] { '\\', ':', '\0' }) >= 0)
            {
                return false;
            }

            foreach (string segment in path.Split('/'))
            {
                if (segment.Length == 0 || segment == "." || segment == "..")
                {
                    return false;
                }
            }

            return true;
        }

        private static void Fail(HttpResponse response, int statusCode)
        {
            response.Clear();
            response.StatusCode = statusCode;
            response.ContentType = "text/plain";
            response.Write(statusCode == 403 ? "You do not have permission to view this file." : "File not available.");
            response.End();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
