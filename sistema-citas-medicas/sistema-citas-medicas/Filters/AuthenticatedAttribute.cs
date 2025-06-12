using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;

namespace sistema_citas_medicas.Filters
{
    public class AuthenticatedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sesion = HttpContext.Current.Session["usuario"];
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            if (controller == "Cuenta" && action == "CerrarSesion" && sesion == null)
            {
                filterContext.Result = new RedirectResult("~/Cuenta/IniciarSesion");
                return;
            }

            if (controller == "Cuenta" && action == "CerrarSesion" && sesion != null)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (sesion == null && controller != "Cuenta")
            {
                filterContext.Result = new RedirectResult("~/Cuenta/IniciarSesion");
                return;
            }

            if (sesion != null && controller == "Cuenta")
            {
                filterContext.Result = new RedirectResult("~/Cita/TableroCitas");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}