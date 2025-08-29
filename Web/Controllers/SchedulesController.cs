using Web.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using medical_appointment_system.Models;

namespace Web.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SchedulesController(IHttpClientFactory http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        private LoggedUserDTO? GetLoggedUser()
        {
            var userJson = _httpContextAccessor.HttpContext!.Session.GetString("LoggedUser");
            if (string.IsNullOrEmpty(userJson)) return null;

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<LoggedUserDTO>(userJson, options);
        }

        public async Task<IActionResult> Index()
        {
            var user = GetLoggedUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int id = user.UserId;

            var response = await _http.GetAsync($"api/schedule/{id}");
            if (response.IsSuccessStatusCode)
            {
                var schedules = await response.Content.ReadFromJsonAsync<List<Schedule>>();
                return View(schedules);
            }

            TempData["Error"] = "No se encontraron horarios para el doctor.";
            return View(new List<Schedule>());
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<Schedule> schedules)
        {
            var response = await _http.PostAsJsonAsync("api/schedule", schedules);

            if (response.IsSuccessStatusCode)
            {
                var messages = await response.Content.ReadFromJsonAsync<List<string>>() ?? new();

                if (!messages.Any() || messages[0] == "✅ ¡Horarios actualizados correctamente!" ||
                    messages[0] == "ℹ️ No se realizaron cambios.")
                {
                    TempData["Success"] = string.Join(" | ", messages);
                }
                else
                {
                    TempData["Error"] = string.Join(" | ", messages);
                }

                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "No se pudieron actualizar los horarios. Intenta nuevamente.";
            return View(schedules);
        }
    }
}
