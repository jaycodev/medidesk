    using medical_appointment_system.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Globalization;
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
                var resp = await _http.GetAsync($"api/appointment/{id}");
                if (resp.IsSuccessStatusCode)
                    return await resp.Content.ReadFromJsonAsync<AppointmentDetailDTO>();
                return null;
            }

            private async Task LoadDoctorsAsync(int? selectedId = null)
            {
                try
                {
                    var docs = await _http.GetFromJsonAsync<List<DoctorListDTO>>("api/doctors") ?? new();
                    ViewBag.Doctors = new SelectList(
                        docs.Select(d => new {
                            Id = d.UserId,
                            Name = $"{d.FirstName} {d.LastName}".Trim()
                        }),
                        "Id", "Name", selectedId
                    );
                }
                catch
                {
                    ViewBag.Doctors = new SelectList(Enumerable.Empty<SelectListItem>());
                }
            }

            private async Task LoadPatientsAsync(int? selectedId = null)
            {
                try
                {
                    var pats = await _http.GetFromJsonAsync<List<Patient>>("api/patients") ?? new();
                    ViewBag.Patients = new SelectList(
                        pats.Select(p => new {
                            Id = p.UserId,
                            Name = $"{p.FirstName} {p.LastName}".Trim()
                        }),
                        "Id", "Name", selectedId
                    );
                }
                catch
                {
                    ViewBag.Patients = new SelectList(Enumerable.Empty<SelectListItem>());
                }
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

            private void LoadConsultationTypes(string? selected = null)
            {
                ViewBag.ConsultationTypes = new SelectList(new[]
                {
                    new SelectListItem("Consulta",  "consulta"),
                    new SelectListItem("Examen",    "examen"),
                    new SelectListItem("Operación", "operacion"),
                }, "Value", "Text", selected);
            }

            private void LoadStatusList(string? selected = null)
            {
                ViewBag.StatusList = new SelectList(new[]
                {
                    new SelectListItem("Pendiente", "pendiente"),
                    new SelectListItem("Confirmada", "confirmada"),
                    new SelectListItem("Cancelada", "cancelada"),
                    new SelectListItem("Atendida", "atendida"),
                }, "Value", "Text", selected);
            }

            private async Task LoadAllSelectsAsync(
                int? doctorId = null, int? patientId = null, int? specialtyId = null, string? type = null)
            {
                await LoadDoctorsAsync(doctorId);
                await LoadPatientsAsync(patientId);
                await LoadSpecialtiesAsync(specialtyId);
                LoadConsultationTypes(type);
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

            public async Task<IActionResult> Index()
            {
                var list = new List<AppointmentListDTO>();
                try
                {
                    list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>("api/appointment")
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

            [HttpGet]
            public async Task<IActionResult> Create()
            {
                var vm = new CreateAppointmentDTO
                {
                    Date = DateTime.Today.ToString("yyyy-MM-dd"),
                    Time = DateTime.Now.ToString("HH:mm")
                };
                await LoadAllSelectsAsync();
                return View(vm);
            }

            [HttpPost]
            public async Task<IActionResult> Create(CreateAppointmentDTO dto)
            {
                if (!ModelState.IsValid)
                {
                    await LoadAllSelectsAsync(dto.DoctorId, dto.PatientId, dto.SpecialtyId, dto.ConsultationType);
                    return View(dto);
                }

                try
                {
                    var resp = await _http.PostAsJsonAsync("api/appointment", dto);

                    if (resp.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "¡Cita creada correctamente!";
                        return RedirectToAction(nameof(Index));
                    }

                    var content = await resp.Content.ReadAsStringAsync();
                    ViewBag.Message = ExtractErrorMessage(content);
                }
                catch
                {
                    ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
                }

                await LoadAllSelectsAsync(dto.DoctorId, dto.PatientId, dto.SpecialtyId, dto.ConsultationType);
                return View(dto);
            }

            [HttpGet]
            public async Task<IActionResult> Edit(int id)
            {
                if (id == 0) return RedirectToAction(nameof(Index));

                var item = await GetByIdAsync(id);
                if (item == null) return RedirectToAction(nameof(Index));

                LoadStatusList(item.Status);
                return View(item);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [FromForm] UpdateAppointmentStatusDTO form)
            {
                if (!ModelState.IsValid)
                {
                    var vm = await GetByIdAsync(id);
                    LoadStatusList(form.Status);
                    return View(vm!);
                }

                HttpResponseMessage resp;
                try
                {
                    resp = await _http.PutAsJsonAsync($"api/appointment/{id}", form);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"No se pudo contactar a la API: {ex.Message}";
                    var vmErr = await GetByIdAsync(id);
                    LoadStatusList(form.Status);
                    return View(vmErr!);
                }

                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = $"¡Cita actualizada a '{form.Status}'!";
                    return RedirectToAction(nameof(Index));
                }

                var raw = await resp.Content.ReadAsStringAsync();
                ViewBag.Message = $"Error {(int)resp.StatusCode}: {resp.ReasonPhrase}. {ExtractErrorMessage(raw)}";
                var vm2 = await GetByIdAsync(id);
                LoadStatusList(form.Status);
                return View(vm2!);
            }

            private async Task HydrateAsync(AppointmentDetailDTO dto)
                {
                    var full = await GetByIdAsync(dto.AppointmentId);
                    if (full != null)
                    {
                        dto.DoctorName  = full.DoctorName;
                        dto.PatientName = full.PatientName;
                        dto.Date        = full.Date;
                        dto.Time        = full.Time;
                    }
                }
            }
    }
