using Microsoft.AspNetCore.Mvc;
using Web.Models.Notifications;

namespace Web.Models.ViewComponents
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly HttpClient _http;
        public NotificationsViewComponent(IHttpClientFactory factory)
            => _http = factory.CreateClient("ApiClient");

        public async Task<IViewComponentResult> InvokeAsync(
            int? doctorId = null, int? patientId = null, int take = 10)
        {
            try
            {
                string url;
                if (doctorId.HasValue)
                    url = $"api/Notification/doctor/{doctorId.Value}?take={take}";
                else if (patientId.HasValue)
                    url = $"api/Notification/patient/{patientId.Value}?take={take}";
                else
                    // si tienes un endpoint "global", si no, devuelve vacío:
                    return View("Default", new List<NotificationDTO>());

                var list = await _http.GetFromJsonAsync<List<NotificationDTO>>(url) ?? new();
                // Ordena por fecha desc, por si el repo no lo hace:
                list = list.OrderByDescending(n => n.CreatedAt).ToList();
                return View("Default", list);
            }
            catch
            {
                return View("Default", new List<NotificationDTO>());
            }
        }
    }
}
