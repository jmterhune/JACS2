using DotNetNuke.Web.Api;
using System.Web.Http;

namespace tjc.Modules.HearingLog.Components.Services
{
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        /// <summary>
        /// RegisterRoutes is used to register the module's routes
        /// </summary>
        /// <param name="mapRouteManager"></param>
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
               moduleFolderName: "HearingsLog",
               routeName: "HearingLogCount",
               url: "{controller}/{action}/{count}",
               defaults: new { name = RouteParameter.Optional },
               namespaces: new[] { "tjc.Modules.HearingLog.Components.Services" });
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "HearingsLog",
                routeName: "HearingLogItems",
                url: "{controller}/{action}",
                defaults: new { name = RouteParameter.Optional },
                namespaces: new[] { "tjc.Modules.HearingLog.Components.Services" });

            mapRouteManager.MapHttpRoute(
               moduleFolderName: "ExcludeLog",
               routeName: "ExcludeItem",
               url: "{controller}/{action}/{logId}",
               defaults: new { name = RouteParameter.Optional },
               namespaces: new[] { "tjc.Modules.HearingLog.Components.Services" });

        }
    }
}