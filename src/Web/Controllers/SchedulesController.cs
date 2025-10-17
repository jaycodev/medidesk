using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Models.Account;
using Web.Models.Schedules;
using Web.Services.Schedule;

namespace Web.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SchedulesController(IScheduleService scheduleService, IHttpContextAccessor httpContextAccessor)
        {
            _scheduleService = scheduleService;
            _httpContextAccessor = httpContextAccessor;
        }

        private UserSession? GetLoggedUser()
        {
            var userJson = _httpContextAccessor.HttpContext!.Session.GetString("UserSession");
            if (string.IsNullOrEmpty(userJson)) return null;

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<UserSession>(userJson, options);
        }

        public async Task<IActionResult> Index()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Account");

            var schedules = await _scheduleService.GetByIdAsync(user.UserId);

            if (!schedules.Any())
                TempData["Error"] = "No se encontraron horarios para el doctor.";

            var viewModels = schedules.ToViewModelList();
            return View(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<ScheduleViewModel> schedules)
        {
            var requests = schedules.ToRequestList();
            var messages = await _scheduleService.UpdateAsync(requests);

            if (!messages.Any() || messages[0].Contains("✅") || messages[0].Contains("ℹ️"))
                TempData["Success"] = string.Join(" | ", messages);
            else
                TempData["Error"] = string.Join(" | ", messages);

            return RedirectToAction(nameof(Index));
        }
    }
}
