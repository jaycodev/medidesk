using Microsoft.AspNetCore.Mvc;
using Web.Models.Specialties;

namespace Web.Controllers
{
    public class SpecialtiesController : Controller
    {
        private readonly HttpClient _http;

        public SpecialtiesController(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        private async Task<SpecialtyDTO?> GetByIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/specialties/{id}");
            if (resp.IsSuccessStatusCode)
            {
                var specialty = await resp.Content.ReadFromJsonAsync<SpecialtyDTO>();
                return specialty;
            }
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var specialties = new List<SpecialtyDTO>();
            try
            {
                specialties = await _http.GetFromJsonAsync<List<SpecialtyDTO>>("api/specialties") ?? new List<SpecialtyDTO>();
            }
            catch
            {
            }

            return View(specialties);
        }

        public IActionResult Create()
        {
            return View(new CreateSpecialtyDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSpecialtyDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var resp = await _http.PostAsJsonAsync("api/specialties", dto);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Especialidad creada correctamente!";
                    return RedirectToAction("Index");
                }

                var content = await resp.Content.ReadAsStringAsync();
                ViewBag.Message = ExtractErrorMessage(content);
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
                return RedirectToAction("Index");

            var specialty = await GetByIdAsync(id);
            if (specialty == null)
                return RedirectToAction("Index");

            return View(specialty);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SpecialtyDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var toUpdate = new UpdateSpecialtyDTO
            {
                Name = dto.Name,
                Description = dto.Description
            };

            try
            {
                var resp = await _http.PutAsJsonAsync($"api/specialties/{dto.SpecialtyId}", toUpdate);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Especialidad actualizada correctamente!";
                    return RedirectToAction("Index");
                }

                var content = await resp.Content.ReadAsStringAsync();
                ViewBag.Message = ExtractErrorMessage(content);
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(dto);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return RedirectToAction("Index");

            var specialty = await GetByIdAsync(id);
            if (specialty == null)
                return RedirectToAction("Index");

            return View(specialty);
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
