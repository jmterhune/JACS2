using DotNetNuke.Web.Api;
using System.Web.Http;

namespace tjc.Modules.TranscriptDatabase.Services
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
                moduleFolderName: "TranscriptDatabase",
                routeName: "action",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Modules.TranscriptDatabase.Services" });
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "TranscriptDatabase",
                routeName: "actionParam",
                url: "{controller}/{action}/{count}",
                defaults: new { caseNumber = RouteParameter.Optional },
                namespaces: new[] { "tjc.Modules.TranscriptDatabase.Services" });
            mapRouteManager.MapHttpRoute(
               moduleFolderName: "TranscriptDelete",
               routeName: "Delete",
               url: "{controller}/{action}/{designationId}",
               namespaces: new[] { "tjc.Modules.TranscriptDatabase.Services" });
            mapRouteManager.MapHttpRoute(
              moduleFolderName: "TranscriptToggle",
              routeName: "Toggle",
              url: "{controller}/{action}/{designationId}",
              namespaces: new[] { "tjc.Modules.TranscriptDatabase.Services" });
        }
    }
}