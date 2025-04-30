using DotNetNuke.Web.Api;
using System.Web.Http;
namespace tjc.Modules.CourtRegistry.Services
{

    /// <summary>
    /// The ServiceRouteMapper tells the DNN Web API Framework what routes this module uses
    /// </summary>
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        /// <summary>
        /// RegisterRoutes is used to register the module's routes
        /// </summary>
        /// <param name="mapRouteManager"></param>
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "CourtRegistry",
                routeName: "action",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Modules.CourtRegistry.Services" });

            mapRouteManager.MapHttpRoute(
                moduleFolderName: "CourtRegistry",
                routeName: "actionParam",
                url: "{controller}/{action}/{count}",
                namespaces: new[] { "tjc.Modules.CourtRegistry.Services" });

            mapRouteManager.MapHttpRoute(
               moduleFolderName: "CourtRegistryDelete",
               routeName: "Delete",
               url: "{controller}/{action}/{applicationid}",
               namespaces: new[] { "tjc.Modules.CourtRegistry.Services" });
        }
    }
}