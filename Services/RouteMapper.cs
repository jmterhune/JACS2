using DotNetNuke.Web.Api;
using System.Web.Http;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services
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
                moduleFolderName: "JACS",
                routeName: "posts",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Modules.jacs.Services" });

            mapRouteManager.MapHttpRoute(
                moduleFolderName: "JACS",
                routeName: "paramCount",
                url: "{controller}/{action}/{p1}",
                namespaces: new[] { "tjc.Modules.jacs.Services" });
            mapRouteManager.MapHttpRoute(
               moduleFolderName: "JACS",
               routeName: "paramCount2",
               url: "{controller}/{action}/{p1}/{p2}",
               namespaces: new[] { "tjc.Modules.jacs.Services" });
            mapRouteManager.MapHttpRoute(
               moduleFolderName: "JACS",
               routeName: "paramCount3",
               url: "{controller}/{action}/{p1}/{p2}/{p3}",
               namespaces: new[] { "tjc.Modules.jacs.Services" });

        }
    }
}