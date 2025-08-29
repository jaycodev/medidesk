using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Web.Models.User;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContext;

        public AccountController(IHttpClientFactory httpFactory, IHttpContextAccessor httpContext)
        {
            _http = httpFactory.CreateClient("ApiClient");
            _httpContext = httpContext;
        }

        public IActionResult Login()
        {
            return View(new LoginDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _http.PostAsJsonAsync("api/users/login", model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Correo o contraseña incorrectos";
                return View(model);
            }

            var user = await response.Content.ReadFromJsonAsync<LoggedUserDTO>();

            if (user == null)
            {
                ViewBag.Message = "Error al procesar el login";
                return View(model);
            }

            if (user.Roles != null && user.Roles.Count == 1)
            {
                user.ActiveRole = user.Roles.First();
            }

            var userJson = JsonSerializer.Serialize(user);
            _httpContext.HttpContext!.Session.SetString("LoggedUser", userJson);

            _httpContext.HttpContext.Session.SetString("UserId", user.UserId.ToString());
            _httpContext.HttpContext.Session.SetString("Email", user.Email ?? string.Empty);
            _httpContext.HttpContext.Session.SetString("FirstName", user.FirstName ?? string.Empty);
            _httpContext.HttpContext.Session.SetString("LastName", user.LastName ?? string.Empty);
            _httpContext.HttpContext.Session.SetString("FullName", user.FullName ?? string.Empty);
            _httpContext.HttpContext.Session.SetString("Roles", string.Join(",", user.Roles ?? new List<string>()));
            _httpContext.HttpContext.Session.SetString("ProfilePicture", user.ProfilePicture ?? string.Empty);
            if (!string.IsNullOrEmpty(user.ActiveRole))
            {
                _httpContext.HttpContext.Session.SetString("ActiveRole", user.ActiveRole);
            }

            return RedirectToAction("Home", "Appointments");
        }

        public IActionResult Logout()
        {
            _httpContext.HttpContext!.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetActiveRole(string role)
        {
            var userJson = _httpContext.HttpContext!.Session.GetString("LoggedUser");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToAction("Login");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var user = JsonSerializer.Deserialize<LoggedUserDTO>(userJson, options);

            if (user != null && user.Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase)))
            {
                user.ActiveRole = role;
            }
            else if (user != null && user.Roles.Any())
            {
                user.ActiveRole = user.Roles.First();
            }

            var updatedUserJson = JsonSerializer.Serialize(user);
            _httpContext.HttpContext.Session.SetString("LoggedUser", updatedUserJson);

            if (!string.IsNullOrEmpty(user.ActiveRole))
                _httpContext.HttpContext.Session.SetString("ActiveRole", user.ActiveRole);

            return RedirectToAction("Home", "Appointments");
        }
    }
}
