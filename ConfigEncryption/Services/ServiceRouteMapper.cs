using DotNetNuke.Web.Api;

namespace Tjc.Modules.ConfigEncryption.Services
{
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("PersonaBar", "ConfigEncryption", "{controller}/{action}",
                new[] { "Tjc.Modules.ConfigEncryption.Services" });
        }
    }
}
