using medical_appointment_system.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly HttpClient _http;

        public SchedulesController(IHttpClientFactory http)
        {
            _http = http.CreateClient("ApiClient");
        }

        // GET: Schedules
        public async Task<IActionResult> Index()
        {
            int id;
            /* var user = HttpContext.Session.Get<User>("user");
             if (user == null)
             {
                 return RedirectToAction("Login", "Auth");
             }*/
            id = 2;// user.UserId;

            var response = await _http.GetAsync($"api/schedule/{id}");
            if (response.IsSuccessStatusCode)
            {
                var schedules = await response.Content.ReadFromJsonAsync<List<Schedule>>();
                return View(schedules);
            }

            TempData["Error"] = "No se encontraron horarios para el doctor.";
            return View(new List<Schedule>());
        }

        // POST: Schedules
        [HttpPost]
        public async Task<IActionResult> Index(List<Schedule> schedules)
        {
            var response = await _http.PostAsJsonAsync("api/schedule", schedules);

            if (response.IsSuccessStatusCode)
            {
                var messages = await response.Content.ReadFromJsonAsync<List<string>>();

                if (messages.Count()==0 || messages[0]== "✅ ¡Horarios actualizados correctamente!" ||
                    messages[0] == "ℹ️ No se realizaron cambios.")
                TempData["Success"] = string.Join(" | ", messages);
                else
                    TempData["Error"] = string.Join(" | ", messages);

                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "No se pudieron actualizar los horarios. Intenta nuevamente.";
            return View(schedules);
        }
    }
}
