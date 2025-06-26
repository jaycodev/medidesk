using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using medical_appointment_system.Models;

namespace medical_appointment_system.Filters
{
    public class AuthenticatedAttribute : ActionFilterAttribute
    {
        private static readonly Dictionary<string, List<string>> RolePermissions = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["administrador"] = new List<string>
            {
                "appointments/allappointments",
                "appointments/details",
                "appointments/home",
                "doctors/*",
                "patients/*",
                "specialties/*",
                "users/*",
                "profile/*"
            },
            ["medico"] = new List<string>
            {
                "appointments/home",
                "appointments/myappointments",
                "appointments/pending",
                "appointments/historial",
                "appointments/confirm",
                "appointments/attend",
                "appointments/cancel",
                "appointments/details",
                "appointments/exporttopdf",
                "appointments/exporttoexcel",
                "schedules/*",
                "profile/*"
            },
            ["paciente"] = new List<string>
            {
                "appointments/home",
                "appointments/reserve",
                "appointments/getdoctorsbyspecialty",
                "appointments/getavailabletimes",
                "appointments/myappointments",
                "appointments/pending",
                "appointments/historial",
                "appointments/details",
                "appointments/exporttopdf",
                "appointments/exporttoexcel",
                "profile/*"
            }
        };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionUser = HttpContext.Current.Session["user"] as User;
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            string action = filterContext.ActionDescriptor.ActionName.ToLower();

            if (controller == "account" && action == "logout" && sessionUser == null)
            {
                filterContext.Result = new RedirectResult("~/account/login");
                return;
            }

            if (controller == "account" && action == "logout" && sessionUser != null)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (sessionUser == null && controller != "account")
            {
                filterContext.Result = new RedirectResult("~/account/login");
                return;
            }

            if (sessionUser != null && controller == "account")
            {
                if (action == "setactiverole")
                {
                    if (sessionUser.Roles == null || sessionUser.Roles.Count <= 1)
                    {
                        filterContext.Result = new RedirectResult("~/");
                        return;
                    }

                    base.OnActionExecuting(filterContext);
                    return;
                }

                filterContext.Result = new RedirectResult("~/");
                return;
            }

            if (sessionUser != null)
            {
                string role = sessionUser.ActiveRole?.ToLower();
                if (!string.IsNullOrEmpty(role) && RolePermissions.ContainsKey(role))
                {
                    var allowed = RolePermissions[role];
                    string currentPath = $"{controller}/{action}";

                    bool hasAccess = allowed.Any(p =>
                        p.Equals(currentPath, StringComparison.OrdinalIgnoreCase) ||
                        (p.EndsWith("/*") && p.StartsWith(controller + "/", StringComparison.OrdinalIgnoreCase))
                    );

                    if (!hasAccess)
                    {
                        filterContext.Result = new RedirectResult("~/");
                        return;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}