using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Models.Appointments;
using Web.Models.Doctors;
using Web.Models.Specialties;

namespace Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly HttpClient _http;

        public AppointmentsController(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        private async Task<AppointmentDetailDTO?> GetByIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/appointments/{id}");
            if (resp.IsSuccessStatusCode)
                return await resp.Content.ReadFromJsonAsync<AppointmentDetailDTO>();
            return null;
        }

        private async Task LoadSpecialtiesAsync(int? selectedId = null)
        {
            try
            {
                var specs = await _http.GetFromJsonAsync<List<SpecialtyDTO>>("api/specialties") ?? new();
                ViewBag.Specialties = new SelectList(
                    specs.Select(s => new { Id = s.SpecialtyId, Name = s.Name }),
                    "Id", "Name", selectedId
                );
            }
            catch
            {
                ViewBag.Specialties = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        public async Task<IActionResult> Home()
        {
            int userId = 2; 
            string userRol = "medico";

            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                    $"api/appointments/all-by-user?userId={userId}&userRol={userRol}")
                    ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> AllAppointments()
        {
            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>("api/appointments")
                       ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> MyAppointments()
        {
            int userId = 2;
            string userRol = "medico";

            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                           $"api/appointments/my?userId={userId}&userRol={userRol}")
                       ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> Pending()
        {
            int userId = 2;
            string userRol = "medico";

            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                           $"api/appointments/pending?userId={userId}&userRol={userRol}")
                       ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> Historial()
        {
            int userId = 2;
            string userRol = "medico";

            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                           $"api/appointments/historial?userId={userId}&userRol={userRol}")
                       ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return RedirectToAction(nameof(Index));

            var item = await GetByIdAsync(id);
            if (item == null) return RedirectToAction(nameof(Index));

            return View(item);
        }

        public async Task<IActionResult> Reserve()
        {
            await LoadSpecialtiesAsync();
            return View(new CreateAppointmentDTO());
        }

        public async Task<JsonResult> GetDoctorsBySpecialty(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/doctors/by-specialty?specialtyId={id}&userId=3");
                if (!response.IsSuccessStatusCode)
                    return Json(new { error = "No se pudieron obtener los médicos" });

                var doctors = await response.Content.ReadFromJsonAsync<List<DoctorBySpecialtyDTO>>()
                                ?? new List<DoctorBySpecialtyDTO>();

                return Json(doctors);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private async Task<List<ScheduleDTO>?> GetDoctorScheduleByDayAsync(int doctorId, DateTime date)
        {
            try
            {
                var response = await _http.GetAsync(
                    $"api/appointments/schedule-by-doctor-and-day?doctorId={doctorId}&date={date.ToString("yyyy-MM-dd")}");

                if (!response.IsSuccessStatusCode) return null;

                var schedule = await response.Content.ReadFromJsonAsync<List<ScheduleDTO>>();
                return schedule;
            }
            catch
            {
                return null;
            }
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

            var appointmentsResponse = await _http.GetAsync(
                $"api/appointments/by-doctor-and-date?doctorId={doctorId}&date={date:yyyy-MM-dd}");

            var appointments = appointmentsResponse.IsSuccessStatusCode
                ? await appointmentsResponse.Content.ReadFromJsonAsync<List<AppointmentTimeDTO>>()
                : new List<AppointmentTimeDTO>();

            var takenTimes = appointments?.Select(a => a.Time.ToString(@"hh\:mm")).ToList() ?? new List<string>();

            var available = allTimes.Select(t => new
            {
                Time = t,
                IsAvailable = !takenTimes.Contains(t)
            }).ToList();

            return Json(available);
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(CreateAppointmentDTO dto)
        {
            dto.PatientId = 3;

            var resp = await _http.PostAsJsonAsync("api/appointments", dto);

            if (resp.IsSuccessStatusCode)
                TempData["Success"] = "¡Cita reservada correctamente!";
            else
                TempData["Error"] = "No se pudo reservar la cita.";

            return RedirectToAction("Pending");
        }

        public async Task<IActionResult> Confirm(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = await GetByIdAsync(id);
            if (appointment == null || appointment.Status?.ToLower() != "pendiente")
            {
                TempData["Error"] = "Solo se pueden confirmar citas pendientes.";
                return RedirectToAction("Pending");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmConfirmed(int id)
        {
            var dto = new { Status = "confirmada" };

            var resp = await _http.PutAsJsonAsync($"api/appointments/{id}", dto);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Success"] = "La cita fue confirmada correctamente.";
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Error"] = "Cita no encontrada.";
            }
            else
            {
                var content = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = ExtractErrorMessage(content);
            }

            return RedirectToAction("MyAppointments");
        }

        public async Task<IActionResult> Attend(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = await GetByIdAsync(id);
            if (appointment == null || appointment.Status?.ToLower() != "confirmada")
            {
                TempData["Error"] = "Solo se pueden atender citas confirmadas.";
                return RedirectToAction("MyAppointments");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Attend")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AttendConfirmed(int id)
        {
            var dto = new { Status = "atendida" };

            var resp = await _http.PutAsJsonAsync($"api/appointments/{id}", dto);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Success"] = "La cita fue atendida correctamente.";
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Error"] = "Cita no encontrada.";
            }
            else
            {
                var content = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = ExtractErrorMessage(content);
            }

            return RedirectToAction("Historial");
        }

        public async Task<IActionResult> Cancel(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = await GetByIdAsync(id);
            var status = appointment?.Status?.ToLower();

            if (appointment == null || status == "cancelada" || status == "atendida")
            {
                TempData["Error"] = "Solo se pueden cancelar citas pendientes o confirmadas.";
                return RedirectToAction("Pending");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var dto = new { Status = "cancelada" };

            var resp = await _http.PutAsJsonAsync($"api/appointments/{id}", dto);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Success"] = "La cita fue cancelada correctamente.";
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Error"] = "Cita no encontrada.";
            }
            else
            {
                var content = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = ExtractErrorMessage(content);
            }

            return RedirectToAction("Historial");
        }

        private string ExtractErrorMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "No se pudo procesar la petición.";

            try
            {
                if (content.Contains("\"message\""))
                {
                    var start = content.IndexOf("\"message\"", StringComparison.OrdinalIgnoreCase);
                    var colon = content.IndexOf(':', start);
                    var trimmed = content.Substring(colon + 1).Trim().Trim('"', ' ', '}');
                    return trimmed;
                }
                if (content.Contains("\"error\""))
                {
                    var start = content.IndexOf("\"error\"", StringComparison.OrdinalIgnoreCase);
                    var colon = content.IndexOf(':', start);
                    var trimmed = content.Substring(colon + 1).Trim().Trim('"', ' ', '}');
                    return trimmed;
                }
            }
            catch { }

            return content.Length > 300 ? content[..300] + "..." : content;
        }
    }
}
