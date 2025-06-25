using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace medical_appointment_system
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            var httpException = exception as HttpException;
            int code = httpException?.GetHttpCode() ?? 500;

            if (Response.StatusCode != code)
            {
                Server.ClearError();
                Response.Redirect($"/Error/Show?code={code}");
            }
        }

    }
}
