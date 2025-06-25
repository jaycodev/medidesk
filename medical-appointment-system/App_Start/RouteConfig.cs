using System.Web.Mvc;
using System.Web.Routing;
using medical_appointment_system.App_Start;

namespace medical_appointment_system
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "DefaultIndexShortcut",
                url: "{controller}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );

            routes.Add("Default", new LowercaseRoute(
                url: "{controller}/{action}/{id}",
                routeHandler: new MvcRouteHandler()
            )
            {
                Defaults = new RouteValueDictionary(new { controller = "Appointments", action = "Home", id = UrlParameter.Optional })
            });
        }
    }
}
