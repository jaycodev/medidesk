using System.Web;
using System.Web.Mvc;

namespace medical_appointment_system.Filters
{
    public class AuthenticatedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sesion = HttpContext.Current.Session["user"];
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            string action = filterContext.ActionDescriptor.ActionName.ToLower();

            if (controller == "account" && action == "logout" && sesion == null)
            {
                filterContext.Result = new RedirectResult("~/account/login");
                return;
            }

            if (controller == "account" && action == "logout" && sesion != null)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (sesion == null && controller != "account")
            {
                filterContext.Result = new RedirectResult("~/account/login");
                return;
            }

            if (sesion != null && controller == "account" && action != "setactiverole")
            {
                filterContext.Result = new RedirectResult("~/");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}