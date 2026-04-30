using DotNetNuke.Web.Api;
using System.Web.Http;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>
    /// Registers the Web API routes for the EmployeeDB module.
    /// DNN auto-discovers anything implementing IServiceRouteMapper at app start.
    /// All routes are scoped under <c>~/DesktopModules/EmployeeDB/API/</c>.
    /// </summary>
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            // Order matters — Web API matches in registration order.

            // 1. {controller}/{id} where id is numeric. Catches REST verbs:
            //      GET    Phones/2739
            //      PUT    Phones/2739
            //      DELETE Phones/2739
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "EmployeeDB",
                routeName: "EmployeeDB-id",
                url: "{controller}/{id}",
                defaults: null,
                constraints: new { id = @"^\d+$" },
                namespaces: new[] { "tjc.Modules.EmployeeDB.Components.Api" });

            // 2. {controller}/{action} — named subroutes like Phones/ForEmployee
            //    (the constraint on route #1 ensures we only land here when the
            //    second segment is non-numeric).
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "EmployeeDB",
                routeName: "EmployeeDB-action",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Modules.EmployeeDB.Components.Api" });

            // 3. {controller} on its own — used for POST /Phones (creates new row).
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "EmployeeDB",
                routeName: "EmployeeDB-controller",
                url: "{controller}",
                namespaces: new[] { "tjc.Modules.EmployeeDB.Components.Api" });
        }
    }
}
