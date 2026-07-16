/*
' Copyright (c) 2024  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Web.Api;

namespace tjc.Modules.ExpertWitness.Components.Api
{
    /// <summary>
    /// Registers the Web API routes for the ExpertWitness module. DNN auto-discovers
    /// anything implementing IServiceRouteMapper at app start. All routes are scoped
    /// under <c>~/DesktopModules/ExpertWitness/API/</c>.
    /// </summary>
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            // Order matters — Web API matches in registration order.

            // 1. {controller}/{id} where id is numeric. Catches GET/PUT/DELETE by id.
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "ExpertWitness",
                routeName: "ExpertWitness-id",
                url: "{controller}/{id}",
                defaults: null,
                constraints: new { id = @"^\d+$" },
                namespaces: new[] { "tjc.Modules.ExpertWitness.Components.Api" });

            // 2. {controller}/{action} — named subroutes like Types/All.
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "ExpertWitness",
                routeName: "ExpertWitness-action",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Modules.ExpertWitness.Components.Api" });

            // 3. {controller} on its own — used for POST /Types (creates new row).
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "ExpertWitness",
                routeName: "ExpertWitness-controller",
                url: "{controller}",
                namespaces: new[] { "tjc.Modules.ExpertWitness.Components.Api" });
        }
    }
}
