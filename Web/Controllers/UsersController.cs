
using medical_appointment_system.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Web.Models.Specialties;
using Web.Models.User;
using Web.Models.Doctors;
using DocumentFormat.OpenXml.Drawing.Charts;
using Humanizer;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly HttpClient _http;

        public UsersController(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var users = new List<UserListDTO>();
            try
            {
                users = await _http.GetFromJsonAsync<List<UserListDTO>>("api/users") ?? new List<UserListDTO>();
            }
            catch
            {
                // Puedes manejar errores globales o mostrar mensaje en la vista
            }

            return View(users);
        }

        public IActionResult Create()
        {
            return View(new UserDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _http.PostAsJsonAsync("api/users", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Usuario registrado correctamente!";
                    return RedirectToAction("Index");
                }

                var content = await response.Content.ReadAsStringAsync();
                ViewBag.Message = ExtractErrorMessage(content);
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error al registrar el usuario.";
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetSpecialties()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<SpecialtyDTO>>("api/specialties");
                return Json(response ?? new List<SpecialtyDTO>());
            }
            catch
            {
                return Json(new List<SpecialtyDTO>());
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var user = await _http.GetFromJsonAsync<UserEditViewModel>($"api/users/{id}");
            if (user == null)
                return RedirectToAction("Index");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.Errors = errors;
                return View(model);
            }

            try
            {
                // 1. Primero, actualizas al usuario principal
                var response = await _http.PutAsJsonAsync($"api/users/{id}", model);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ViewBag.Message = ExtractErrorMessage(content);
                    return View(model);
                }

                // 2. Si incluye el rol de médico, creas el médico
                if (model.SelectedRoleCombo.Contains("medico"))
                {
                    CreateDoctorDTO doctorData = new CreateDoctorDTO { 
                        SpecialtyId = (int)model.SpecialtyId,
                        Status = (bool)model.Status,
                        
                    };
                    var doctorResponse = await _http.PutAsJsonAsync($"api/doctors/{id}", doctorData);

                    if (!doctorResponse.IsSuccessStatusCode)
                    {
                        var content = await doctorResponse.Content.ReadAsStringAsync();
                        ViewBag.Message = $"Error al guardar médico: {ExtractErrorMessage(content)}";
                        return View(model);
                    }
                }

                // 3. Si incluye el rol de paciente, creas el paciente
                if (model.SelectedRoleCombo.Contains("paciente"))
                {
                    var patientData = new
                    {
                        userId = id,
                        birthDate = model.BirthDate,
                        bloodType = model.BloodType
                    };

                    var patientResponse = await _http.PostAsJsonAsync("api/patients", patientData);

                    if (!patientResponse.IsSuccessStatusCode)
                    {
                        var content = await patientResponse.Content.ReadAsStringAsync();
                        ViewBag.Message = $"Error al guardar paciente: {ExtractErrorMessage(content)}";
                        return View(model);
                    }
                }

                TempData["Success"] = "¡Usuario actualizado correctamente!";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error al actualizar el usuario.";
                return View(model);
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var user = await _http.GetFromJsonAsync<UserDTO>($"api/users/{id}");
            if (user == null)
                return RedirectToAction("Index");

            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            try
            {
                var response = await _http.DeleteAsync($"api/users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Usuario eliminado correctamente!";
                    return RedirectToAction("Index");
                }

                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = ExtractErrorMessage(content);
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al eliminar el usuario.";
            }

            return RedirectToAction("Index");
        }

        private string ExtractErrorMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "No se pudo procesar la solicitud.";

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
