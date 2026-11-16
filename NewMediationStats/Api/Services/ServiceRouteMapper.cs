using DotNetNuke.Web.Api;
using System.Web.Http;

namespace tjc.Modules.NewMediationStats.Api.Services
{
    /// <summary>
    /// Registers this module's Web API routes under its own folder (tjc.Modules/NewMediationStats) so
    /// they never overlap with the routes the original Mediation module gets from Intranet.API
    /// (tjc.Modules/Mediation). Route names are prefixed for the same reason: DNN route names
    /// must be unique across the whole site.
    /// </summary>
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        private const string Namespace = "tjc.Modules.NewMediationStats.Api.Services";

        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "tjc.Modules/NewMediationStats",
                routeName: "NewMediationStatsPosts",
                url: "{controller}/{action}",
                defaults: new { name = RouteParameter.Optional },
                namespaces: new[] { Namespace });
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "tjc.Modules/NewMediationStats",
                routeName: "NewMediationStatsCaseListItems",
                url: "{controller}/{action}/{count}",
                defaults: new { name = RouteParameter.Optional },
                namespaces: new[] { Namespace });
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "tjc.Modules/NewMediationStats",
                routeName: "NewMediationStatsAttorneyListItems",
                url: "{controller}/{action}/{count}",
                defaults: new { name = RouteParameter.Optional },
                namespaces: new[] { Namespace });
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "tjc.Modules/NewMediationStats/Services",
                routeName: "NewMediationStatsDeleteCase",
                url: "{controller}/{action}/{caseId}",
                defaults: new { name = RouteParameter.Optional },
                namespaces: new[] { Namespace });
        }
    }
}
