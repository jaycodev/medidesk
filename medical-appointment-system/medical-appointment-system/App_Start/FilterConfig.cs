using System.Web.Mvc;
using medical_appointment_system.Filters;

namespace medical_appointment_system
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticatedAttribute());
            filters.Add(new ForceLowercaseUrlAttribute());
        }
    }
}
