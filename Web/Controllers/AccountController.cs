using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Models.Account;
using Web.Models.Patients;
using Web.Services.Account;
using Web.Services.Patient;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IPatientService _patientService;
        private readonly IHttpContextAccessor _httpContext;

        public AccountController(
            IAccountService accountService,
            IPatientService patientService,
            IHttpContextAccessor httpContext)
        {
            _accountService = accountService;
            _patientService = patientService;
            _httpContext = httpContext;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var request = model.ToLoginRequest();

            var (success, apiUser, message) = await _accountService.LoginAsync(request);

            if (!success || apiUser == null)
            {
                ViewBag.Message = message ?? "Correo y/o contraseña incorrectos";
                return View(model);
            }

            var user = apiUser.ToUserSession();

            if (user.Roles != null && user.Roles.Count == 1)
                user.ActiveRole = user.Roles.First();

            var userJson = JsonSerializer.Serialize(user);
            _httpContext.HttpContext!.Session.SetString("UserSession", userJson);

            _httpContext.HttpContext.Session.SetString("UserId", user.UserId.ToString());
            _httpContext.HttpContext.Session.SetString("Email", user.Email ?? string.Empty);
            _httpContext.HttpContext.Session.SetString("FirstName", user.FirstName ?? string.Empty);
            _httpContext.HttpContext.Session.SetString("LastName", user.LastName ?? string.Empty);
            _httpContext.HttpContext.Session.SetString("FullName", user.FullName ?? string.Empty);
            _httpContext.HttpContext.Session.SetString("Roles", string.Join(",", user.Roles ?? new List<string>()));
            _httpContext.HttpContext.Session.SetString("ProfilePicture", user.ProfilePicture ?? string.Empty);
            _httpContext.HttpContext.Session.SetString("Phone", user.Phone ?? string.Empty);
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

        public IActionResult Register()
        {
            return View(new PatientCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PatientCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var request = model.ToCreateRequest();
            var (Success, Message) = await _patientService.CreateAsync(request);

            if (Success)
            {
                TempData["Success"] = "¡Cuenta creada correctamente!";
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Message = Message;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetActiveRole(string role)
        {
            var userJson = _httpContext.HttpContext!.Session.GetString("UserSession");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToAction("Login");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var user = JsonSerializer.Deserialize<UserSession>(userJson, options);

            if (user != null && user.Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase)))
            {
                user.ActiveRole = role;
            }
            else if (user != null && user.Roles.Any())
            {
                user.ActiveRole = user.Roles.First();
            }

            var updatedUserJson = JsonSerializer.Serialize(user);
            _httpContext.HttpContext.Session.SetString("UserSession", updatedUserJson);

            if (!string.IsNullOrEmpty(user!.ActiveRole))
                _httpContext.HttpContext.Session.SetString("ActiveRole", user.ActiveRole);

            return RedirectToAction("Home", "Appointments");
        }
    }
}
