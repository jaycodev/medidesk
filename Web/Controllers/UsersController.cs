
using medical_appointment_system.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Web.Models.Specialties;
using Web.Models.User;

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

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var user = await _http.GetFromJsonAsync<User>($"api/users/{id}");
            if (user == null)
                return RedirectToAction("Index");

            return View(user);
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


        [HttpPost]
        public async Task<IActionResult> Edit(int id, User model)
        {
            if (!ModelState.IsValid)
                return View(model);
            
            try
            {
                var response = await _http.PutAsJsonAsync($"api/users/{id}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Usuario actualizado correctamente!";
                    return RedirectToAction("Index");
                }

                var content = await response.Content.ReadAsStringAsync();
                ViewBag.Message = ExtractErrorMessage(content);
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error al actualizar el usuario.";
            }

            return View(model);
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
