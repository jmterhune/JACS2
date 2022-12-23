using DotNetNuke.Web.Api;
using System.Web.Http;

namespace tjc.Intranet.API.Services
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
            //mapRouteManager.MapHttpRoute(
            //    moduleFolderName: "tjc.Modules/CourtCounsel",
            //    routeName: "actionMultiparam",
            //    url: "{controller}/{action}/{sectionId}/{itemIndex}/{direction}",
            //    defaults: new { sectionId = RouteParameter.Optional, itemIndex = RouteParameter.Optional, direction = RouteParameter.Optional },
            //    namespaces: new[] { "tjc.Modules.DocumentSubscription.Services" });

            mapRouteManager.MapHttpRoute(
                 moduleFolderName: "CourtCounsel",
                 routeName: "actionParam",
                 url: "{controller}/{action}/{caseNumber}",
                 defaults: new { caseNumber = RouteParameter.Optional },
                 namespaces: new[] { "tjc.Intranet.API.Services" });

            mapRouteManager.MapHttpRoute(
                moduleFolderName: "CourtCounsel",
                routeName: "action",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Intranet.API.Services" });

            mapRouteManager.MapHttpRoute(
                moduleFolderName: "CourtCounsel",
                routeName: "default",
                url: "{controller}/{caseNumber}",
                defaults: new { caseNumber = RouteParameter.Optional },
                namespaces: new[] { "tjc.Intranet.API.Services" });


        }
    }

}