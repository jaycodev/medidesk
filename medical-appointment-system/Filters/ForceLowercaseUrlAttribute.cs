using System.Web.Mvc;

namespace medical_appointment_system.Filters
{
    public class ForceLowercaseUrlAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var url = request.Url.ToString();
            var lowercaseUrl = url.ToLowerInvariant();

            if (url != lowercaseUrl)
            {
                filterContext.Result = new RedirectResult(lowercaseUrl, true);
            }
        }
    }
}