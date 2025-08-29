using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using Web.Models.User;

namespace Web.Filters
{
    public class AuthenticatedAttribute : ActionFilterAttribute
    {
        private static readonly Dictionary<string, List<string>> RolePermissions = new(StringComparer.OrdinalIgnoreCase)
        {
            ["administrador"] = new List<string>
            {
                "appointments/allappointments",
                "appointments/details",
                "appointments/home",
                "appointments/exportallappointmentstopdf",
                "appointments/exportallappointmentstoexcel",
                "doctors/*",
                "patients/*",
                "specialties/*",
                "users/*",
                "profile/*",
                "error/show"
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
                "appointments/exportmyappointmentstopdf",
                "appointments/exportpendingappointmentstopdf",
                "appointments/exporthistorialappointmentstopdf",
                "appointments/exportmyappointmentstoexcel",
                "appointments/exportpendingappointmentstoexcel",
                "appointments/exporthistorialappointmentstoexcel",
                "schedules/*",
                "profile/*",
                "notifications/delete",
                "error/show"
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
                "appointments/exportmyappointmentstopdf",
                "appointments/exportpendingappointmentstopdf",
                "appointments/exporthistorialappointmentstopdf",
                "appointments/exportmyappointmentstoexcel",
                "appointments/exportpendingappointmentstoexcel",
                "appointments/exporthistorialappointmentstoexcel",
                "appointments/exporttoexcel",
                "profile/*",
                "notifications/delete",
                "error/show"
            }
        };

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var session = httpContext.Session;

            var userJson = session.GetString("LoggedUser");
            LoggedUserDTO? sessionUser = null;

            if (!string.IsNullOrEmpty(userJson))
                sessionUser = JsonSerializer.Deserialize<LoggedUserDTO>(userJson);

            string controller = context.RouteData.Values["controller"]?.ToString()?.ToLower() ?? "";
            string action = context.RouteData.Values["action"]?.ToString()?.ToLower() ?? "";

            if (controller == "account" && action == "logout" && sessionUser == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (controller == "account" && action == "logout" && sessionUser != null)
            {
                base.OnActionExecuting(context);
                return;
            }

            if (sessionUser == null && controller != "account")
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (sessionUser != null && controller == "account")
            {
                if (action == "setactiverole")
                {
                    if (sessionUser.Roles == null || sessionUser.Roles.Count <= 1)
                    {
                        context.Result = RedirectToError(403);
                        return;
                    }

                    base.OnActionExecuting(context);
                    return;
                }

                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            if (sessionUser != null)
            {
                if (string.IsNullOrEmpty(sessionUser.ActiveRole) && sessionUser.Roles?.Count > 1)
                {
                    base.OnActionExecuting(context);
                    return;
                }

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
                        context.Result = RedirectToError(403);
                        return;
                    }
                }
                else
                {
                    context.Result = RedirectToError(403);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }

        private RedirectToActionResult RedirectToError(int code)
        {
            return new RedirectToActionResult("Show", "Error", new { code });
        }
    }
}
