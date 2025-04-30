using DotNetNuke.Web.Api;
using System.Web.Http;


namespace tjc.Modules.DigitalCourtReporting.Services
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
                moduleFolderName: "DCR",
                routeName: "action",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Modules.DigitalCourtReporting.Services" });

            mapRouteManager.MapHttpRoute(
                moduleFolderName: "DCR",
                routeName: "actionParam",
                url: "{controller}/{action}/{count}",
                defaults: new { caseNumber = RouteParameter.Optional },
                namespaces: new[] { "tjc.Modules.DigitalCourtReporting.Services" });
            mapRouteManager.MapHttpRoute(
               moduleFolderName: "Proceeding",
               routeName: "actionProceeding",
               url: "{controller}/{action}/{proceedingId}",
               defaults: new { caseNumber = RouteParameter.Optional },
               namespaces: new[] { "tjc.Modules.DigitalCourtReporting.Services" });
        }
    }
}