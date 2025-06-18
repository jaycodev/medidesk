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

            routes.Add("Default", new LowercaseRoute(
                url: "{controller}/{action}/{id}",
                routeHandler: new MvcRouteHandler()
            )
            {
                Defaults = new RouteValueDictionary(new { controller = "Appointments", action = "Dashboard", id = UrlParameter.Optional })
            });
        }
    }
}
