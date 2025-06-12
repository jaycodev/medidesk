using System.Web;
using System.Web.Mvc;
using sistema_citas_medicas.Filters;

namespace sistema_citas_medicas
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticatedAttribute());
        }
    }
}
