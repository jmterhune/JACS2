/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Web.Api;

namespace tjc.Modules.CourtCalendar.Services
{
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            // Authoring actions post a JSON body, so support both {controller}/{action}
            // and the {id} form.
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "CourtCalendar",
                routeName: "courtcalendar-admin",
                url: "{controller}/{action}",
                namespaces: new[] { "tjc.Modules.CourtCalendar.Services" });

            mapRouteManager.MapHttpRoute(
                moduleFolderName: "CourtCalendar",
                routeName: "courtcalendar-admin-id",
                url: "{controller}/{action}/{id}",
                namespaces: new[] { "tjc.Modules.CourtCalendar.Services" });
        }
    }
}
