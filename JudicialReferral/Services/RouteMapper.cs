using DotNetNuke.Web.Api;

namespace tjc.Modules.JudicialReferral.Services
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                "JudicialReferral",
                "default",
                "{controller}/{action}",
                new[] { "tjc.Modules.JudicialReferral.Services" });
        }
    }
}
