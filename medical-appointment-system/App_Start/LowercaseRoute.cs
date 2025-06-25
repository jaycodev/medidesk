using System.Web.Routing;

namespace medical_appointment_system.App_Start
{
    public class LowercaseRoute : Route
    {
        public LowercaseRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler) { }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var path = base.GetVirtualPath(requestContext, values);
            if (path != null)
            {
                path.VirtualPath = path.VirtualPath.ToLowerInvariant();
            }
            return path;
        }
    }
}