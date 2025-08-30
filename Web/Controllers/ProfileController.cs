using Microsoft.AspNetCore.Mvc;
using Web.Models; // tus ViewModels
using System.Net.Http.Headers;
using Newtonsoft.Json;
using medical_appointment_system.Models.ViewModels;
using Web.Models.User;

namespace Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;        

        public ProfileController(IHttpClientFactory http, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = http.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        private LoggedUserDTO? GetLoggedUser()
        {
            var userJson = _httpContextAccessor.HttpContext!.Session.GetString("LoggedUser");
            if (string.IsNullOrEmpty(userJson)) return null;

            return JsonConvert.DeserializeObject<LoggedUserDTO>(userJson);
        }

        public async Task<IActionResult> Index()
        {
            // Ejemplo: obtener usuario logueado (puede venir de Claims o TempData)
            var user = GetLoggedUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

        var modelProfile = new ProfileViewModel
            {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = "999999",
            ProfilePicture = user.ProfilePicture,
            ChangePassword = new ChangePasswordValidator { UserId = user.UserId }
            };

            return View(modelProfile);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProfileViewModel profile)
        {
            var json = JsonConvert.SerializeObject(profile);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/users/{profile.UserId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "¡Perfil actualizado correctamente!";
               // HttpContext.Session.SetString("UserProfile", JsonConvert.SerializeObject(profile));
                return RedirectToAction("Index");
            }

            TempData["Error"] = "No se pudo actualizar el perfil.";
            return View("Index", profile);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ProfileViewModel model)
        {
            if (model.ChangePassword.NewPassword != model.ChangePassword.ConfirmPassword)
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                return RedirectToAction("Index");
            }

            var json = JsonConvert.SerializeObject(model.ChangePassword);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/users/{model.ChangePassword.UserId}/password", content);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Contraseña actualizada correctamente.";
            else
                TempData["Error"] = "Error al actualizar la contraseña.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditProfilePicture(int userId, IFormFile ProfilePictureFile)
        {
            if (ProfilePictureFile == null || ProfilePictureFile.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar una imagen válida.";
                return RedirectToAction("Index");
            }

            using var content = new MultipartFormDataContent();
            var fileStream = ProfilePictureFile.OpenReadStream();
            content.Add(new StreamContent(fileStream), "file", ProfilePictureFile.FileName);
            content.Add(new StringContent("MedicalAppointmentsDB/UserPhotos"), "folder");
            content.Add(new StringContent(ProfilePictureFile.FileName), "fileName");

            var response = await _httpClient.PostAsync("api/cloudinary/upload", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al subir la foto.";
                return RedirectToAction("Index");
            }
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            // Guardar en BD
            var userUpdate = new { UserId = userId, ProfilePicture = (string)result.Url };
            var userJson = JsonConvert.SerializeObject(userUpdate);
            var updateContent = new StringContent(userJson, System.Text.Encoding.UTF8, "application/json");

            var updateResponse = await _httpClient.PutAsync($"api/users/{userId}", updateContent);

            if (updateResponse.IsSuccessStatusCode)
            {
                TempData["Succes"] = "Foto de perfil actualizada correctamente.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "No se pudo actualizar la base de datos.";
            return RedirectToAction("Index");
        }
    }
}
