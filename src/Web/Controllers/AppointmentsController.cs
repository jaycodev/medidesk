using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Appointments.Requests;
using Shared.DTOs.Appointments.Responses;
using Shared.DTOs.Schedules.Responses;
using Web.Helpers;
using Web.Mappers;
using Web.Models.Account;
using Web.Models.Appointments;
using Web.Services.Appointment;
using Web.Services.Doctor;
using Web.Services.Schedule;
using Web.Services.Specialty;


namespace Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly ISpecialtyService _specialtyService;
        private readonly IScheduleService _scheduleService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentsController(IAppointmentService appointmentService, IDoctorService doctorService, ISpecialtyService specialtyService, IScheduleService scheduleService, IHttpContextAccessor httpContextAccessor)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _specialtyService = specialtyService;
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

        private async Task LoadSpecialtiesAsync(int? selectedId = null)
        {
            ViewBag.Specialties = await _specialtyService.GetSelectListAsync(selectedId);
        }

        public async Task<IActionResult> Home()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Account");

            var list = await _appointmentService.GetListAsync("all-by-user", user.UserId, user.ActiveRole);
            var viewModelList = list.Select(a => a.ToListViewModel()).ToList();
            return View(viewModelList);
        }

        public async Task<IActionResult> AllAppointments()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Account");

            var list = await _appointmentService.GetListAsync("all", user.UserId, user.ActiveRole);
            var viewModelList = list.Select(a => a.ToListViewModel()).ToList();
            return View(viewModelList);
        }

        public async Task<IActionResult> MyAppointments()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Account");

            var list = await _appointmentService.GetListAsync("my", user.UserId, user.ActiveRole);
            var viewModelList = list.Select(a => a.ToListViewModel()).ToList();
            return View(viewModelList);
        }

        public async Task<IActionResult> Pending()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Account");

            var list = await _appointmentService.GetListAsync("pending", user.UserId, user.ActiveRole);
            var viewModelList = list.Select(a => a.ToListViewModel()).ToList();
            return View(viewModelList);
        }

        public async Task<IActionResult> Historial()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Account");

            var list = await _appointmentService.GetListAsync("historial", user.UserId, user.ActiveRole);
            var viewModelList = list.Select(a => a.ToListViewModel()).ToList();
            return View(viewModelList);
        }

        public async Task<ActionResult> Details(int id)
        {
            if (id == 0) return RedirectToAction("Home");

            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null) return RedirectToAction("Home");

            var model = appointment.ToViewModel();
            return View(model);
        }

        public async Task<IActionResult> Reserve()
        {
            await LoadSpecialtiesAsync();
            return View(new AppointmentCreateViewModel());
        }

        public async Task<JsonResult> GetDoctorsBySpecialty(int id)
        {
            try
            {
                var doctors = await _doctorService.GetBySpecialtyAsync(id, userId: 3);
                if (doctors == null || !doctors.Any())
                    return Json(new { error = "No se pudieron obtener los médicos" });

                return Json(doctors);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private async Task<List<ScheduleByDateResponse>?> GetDoctorScheduleByDayAsync(int doctorId, DateTime date)
        {
            return await _scheduleService.GetByDateAsync(doctorId, date);
        }

        public async Task<JsonResult> GetAvailableTimes(int doctorId, DateTime date)
        {
            if (doctorId <= 0)
                return Json(new { error = "Se requiere un doctorId válido." });

            var shifts = await GetDoctorScheduleByDayAsync(doctorId, date);
            if (shifts == null || !shifts.Any())
                return Json(new { error = "El médico no tiene horario asignado ese día." });

            var allTimes = new List<string>();
            foreach (var shift in shifts)
            {
                for (var time = shift.StartTime; time < shift.EndTime; time = time.Add(TimeSpan.FromHours(1)))
                {
                    allTimes.Add(time.ToString(@"hh\:mm"));
                }
            }

            var appointments = await _appointmentService.GetByDateAsync(doctorId, date);
            var takenTimes = appointments?.Select(a => a.Time.ToString(@"hh\:mm")).ToList() ?? new List<string>();

            var available = allTimes.Select(t => new
            {
                Time = t,
                IsAvailable = !takenTimes.Contains(t)
            }).ToList();

            return Json(available);
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(AppointmentCreateViewModel model)
        {
            var user = GetLoggedUser();
            if (user == null || user.ActiveRole?.ToLower() != "paciente")
            {
                TempData["Error"] = "Solo un paciente puede reservar citas.";
                return RedirectToAction("Home");
            }

            model.PatientId = user.UserId;
            var request = model.ToCreateRequest();
            var resp = await _appointmentService.ReserveAsync(request);

            if (resp.IsSuccessStatusCode)
                TempData["Success"] = "¡Cita reservada correctamente!";
            else
                TempData["Error"] = "No se pudo reservar la cita.";

            return RedirectToAction("Pending");
        }

        public async Task<IActionResult> Confirm(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null || appointment.Status?.ToLower() != "pendiente")
            {
                TempData["Error"] = "Solo se pueden confirmar citas pendientes.";
                return RedirectToAction("Pending");
            }

            var model = appointment.ToViewModel();
            return View(model);
        }

        [HttpPost, ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmConfirmed(int id)
        {
            var resp = await _appointmentService.UpdateStatusAsync(id, "confirmada");

            if (resp.IsSuccessStatusCode)
                TempData["Success"] = "La cita fue confirmada correctamente.";
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                TempData["Error"] = "Cita no encontrada.";
            else
            {
                var content = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = HttpHelper.ExtractErrorMessage(content);
            }

            return RedirectToAction("MyAppointments");
        }

        public async Task<IActionResult> Attend(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null || appointment.Status?.ToLower() != "confirmada")
            {
                TempData["Error"] = "Solo se pueden atender citas confirmadas.";
                return RedirectToAction("MyAppointments");
            }

            var model = appointment.ToViewModel();
            return View(model);
        }

        [HttpPost, ActionName("Attend")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AttendConfirmed(int id)
        {
            var resp = await _appointmentService.UpdateStatusAsync(id, "atendida");

            if (resp.IsSuccessStatusCode)
                TempData["Success"] = "La cita fue atendida correctamente.";
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                TempData["Error"] = "Cita no encontrada.";
            else
            {
                var content = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = HttpHelper.ExtractErrorMessage(content);
            }

            return RedirectToAction("Historial");
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            var status = appointment?.Status?.ToLower();

            if (appointment == null || status == "cancelada" || status == "atendida")
            {
                TempData["Error"] = "Solo se pueden cancelar citas pendientes o confirmadas.";
                return RedirectToAction("Pending");
            }

            var model = appointment.ToViewModel();
            return View(model);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var resp = await _appointmentService.UpdateStatusAsync(id, "cancelada");

            if (resp.IsSuccessStatusCode)
                TempData["Success"] = "La cita fue cancelada correctamente.";
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                TempData["Error"] = "Cita no encontrada.";
            else
            {
                var content = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = HttpHelper.ExtractErrorMessage(content);
            }

            return RedirectToAction("Historial");
        }

        public async Task<IActionResult> ExportToPdf(string filter)
        {
            var user = GetLoggedUser();
            int? userId = user?.UserId;
            string? userRol = user?.ActiveRole;

            var list = await _appointmentService.GetListAsync(filter, userId, userRol);
            var role = userRol ?? "administrador";
            string title = filter.ToLower() switch
            {
                "all" => "Lista de citas",
                "my" => "Mis citas",
                "pending" => "Citas pendientes",
                "historial" => "Historial de citas",
                _ => "Citas"
            };

            return _appointmentService.GeneratePdf(list, title, role);
        }

        public async Task<IActionResult> ExportToExcel(string filter)
        {
            var user = GetLoggedUser();
            int? userId = user?.UserId;
            string? userRol = user?.ActiveRole;

            var list = await _appointmentService.GetListAsync(filter, userId, userRol);
            var role = userRol ?? "administrador";
            string title = filter.ToLower() switch
            {
                "all" => "Lista de citas",
                "my" => "Mis citas",
                "pending" => "Citas pendientes",
                "historial" => "Historial de citas",
                _ => "Citas"
            };

            return _appointmentService.GenerateExcel(list, title, role);
        }
    }
}
