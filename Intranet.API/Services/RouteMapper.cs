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
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "tjc.Modules/CourtCounsel",
                routeName: "actionMultiparam",
                url: "{controller}/{action}/{sectionId}/{itemIndex}/{direction}",
                defaults: new { sectionId = RouteParameter.Optional, itemIndex = RouteParameter.Optional, direction = RouteParameter.Optional },
                namespaces: new[] { "tjc.Modules.DocumentSubscription.Services" });

            mapRouteManager.MapHttpRoute(
                 moduleFolderName: "tjc.Modules/CourtCounsel",
                 routeName: "actionParam",
                 url: "{controller}/{action}/{caseNumber}",
                 defaults: new { caseNumber = RouteParameter.Optional },
                 namespaces: new[] { "tjc.Intranet.API.Services" });

            mapRouteManager.MapHttpRoute(
                moduleFolderName: "tjc.Modules/CourtCounsel",
                routeName: "action",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Intranet.API.Services" });

            mapRouteManager.MapHttpRoute(
                moduleFolderName: "tjc.Modules/CourtCounsel",
                routeName: "default",
                url: "{controller}/{caseNumber}",
                defaults: new { caseNumber = RouteParameter.Optional },
                namespaces: new[] { "tjc.Intranet.API.Services" });

            mapRouteManager.MapHttpRoute(
                moduleFolderName: "directory/data",
                routeName: "EmployeePersonalData",
                url: "{controller}/{action}/{emailAddress}",
                defaults: new { emailAddress = RouteParameter.Optional },
                namespaces: new[] { "tjc.Intranet.API.Services" });
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "directory",
                routeName: "EmployeeUpdates",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Intranet.API.Services" });
            mapRouteManager.MapHttpRoute(
               moduleFolderName: "directory/lists",
               routeName: "EmployeeLists",
               url: "{controller}/{action}/{employeeId}",
               defaults: new { employeeId = RouteParameter.Optional },
               namespaces: new[] { "tjc.Intranet.API.Services" });
            mapRouteManager.MapHttpRoute(
              moduleFolderName: "tjc.Modules/FamilySelfHelp",
              routeName: "Clients",
              url: "{controller}/{action}/{name}",
              defaults: new { name = RouteParameter.Optional },
              namespaces: new[] { "tjc.Intranet.API.Services.FamilySelfHelp" });
            mapRouteManager.MapHttpRoute(
               moduleFolderName: "tjc.Modules/Mediation",
               routeName: "CaseListItems",
               url: "{controller}/{action}/{count}",
               defaults: new { name = RouteParameter.Optional },
               namespaces: new[] { "tjc.Intranet.API.Services.Mediation" });
            mapRouteManager.MapHttpRoute(
              moduleFolderName: "tjc.Modules/Mediation",
              routeName: "AttorneyListItems",
              url: "{controller}/{action}/{count}",
              defaults: new { name = RouteParameter.Optional },
              namespaces: new[] { "tjc.Intranet.API.Services.Mediation" });
            mapRouteManager.MapHttpRoute(
               moduleFolderName: "tjc.Modules/Mediation/Services",
               routeName: "DeleteCase",
               url: "{controller}/{action}/{caseId}",
               defaults: new { name = RouteParameter.Optional },
               namespaces: new[] { "tjc.Intranet.API.Services.Mediation" });

        }
    }
}