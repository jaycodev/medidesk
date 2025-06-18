using System;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Filters
{
    public class AuthenticatedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sesion = HttpContext.Current.Session["user"];
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            if (controller == "Account" && action == "Logout" && sesion == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }

            if (controller == "Account" && action == "Logout" && sesion != null)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (sesion == null && controller != "Account")
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }

            if (sesion != null && controller == "Account")
            {
                filterContext.Result = new RedirectResult("~/Appointments/Dashboard");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}