using System.Web.Mvc;
using System.Web.Routing;

namespace medical_appointment_system
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Appointments", action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}
