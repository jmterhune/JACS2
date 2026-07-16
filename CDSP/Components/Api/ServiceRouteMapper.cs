/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Web.Api;

namespace tjc.Modules.CDSPAdmin.Components.Api
{
    /// <summary>
    /// Registers the module's Web API routes. DNN auto-discovers anything
    /// implementing IServiceRouteMapper at application start. Routes live under
    /// <c>~/DesktopModules/CDSPAdmin/API/</c> — e.g. Submissions/Get, Submissions/SetCompleted.
    /// </summary>
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "CDSPAdmin",
                routeName: "CDSPAdmin",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Modules.CDSPAdmin.Components.Api" });
        }
    }
}
