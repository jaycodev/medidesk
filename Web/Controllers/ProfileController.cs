using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Domains.Users.DTOs;
using Web.Models.User;
using medical_appointment_system.Models.ViewModels;

namespace Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;

        public ProfileController(IHttpClientFactory http, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _httpClient = http.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        private void LoadAvatarsToViewBag()
        {
            string folderPath = Path.Combine(_env.WebRootPath, "img", "avatars");
            string baseUrl = Url.Content("~/img/avatars/");

            if (!Directory.Exists(folderPath))
            {
                ViewBag.Avatars = new List<string>();
                return;
            }

            ViewBag.Avatars = Directory.GetFiles(folderPath)
                .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                .Select(f => baseUrl + Path.GetFileName(f))
                .ToList();
        }

        private LoggedUserDTO? GetLoggedUser()
        {
            var userJson = _httpContextAccessor.HttpContext!.Session.GetString("LoggedUser");
            return string.IsNullOrEmpty(userJson)
                ? null
                : JsonConvert.DeserializeObject<LoggedUserDTO>(userJson);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            LoadAvatarsToViewBag();

            var model = new ProfileViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                ProfilePicture = user.ProfilePicture,
                ChangePassword = new ChangePasswordValidator { UserId = user.UserId }
            };

            return View(model);
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
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/users/{model.ChangePassword.UserId}/password", content);

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode
                ? "Contraseña actualizada correctamente."
                : "Error al actualizar la contraseña.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditProfilePicture(int userId, IFormFile profilePictureFile)
        {
            if (profilePictureFile == null || profilePictureFile.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar una imagen válida.";
                return RedirectToAction("Index");
            }

            try
            {
                using var content = new MultipartFormDataContent
                {
            { new StreamContent(profilePictureFile.OpenReadStream()), "file", profilePictureFile.FileName },
            { new StringContent("MedicalAppointmentsDB/UserPhotos"), "folder" },
            { new StringContent(profilePictureFile.FileName), "fileName" }
                };

                var response = await _httpClient.PostAsync("api/cloudinary/upload", content);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Error al subir la foto.";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json)!;
                string uploadedUrl = result["url"];

                bool success = await UpdateUserProfilePictureAsync(userId, uploadedUrl);
                TempData[success ? "Success" : "Error"] = success
                    ? "Foto de perfil actualizada correctamente."
                    : "No se pudo actualizar la base de datos.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error inesperado al actualizar la foto: " + ex.Message;
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> SelectAvatar(int userId, string SelectedAvatar)
        {
            if (string.IsNullOrWhiteSpace(SelectedAvatar))
            {
                TempData["Error"] = "Debe seleccionar un avatar.";
                return RedirectToAction("Index");
            }

            try
            {
                bool success = await UpdateUserProfilePictureAsync(userId, SelectedAvatar);
                TempData[success ? "Success" : "Error"] = success
                    ? "Avatar seleccionado correctamente."
                    : "No se pudo actualizar el avatar.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al seleccionar el avatar: " + ex.Message;
            }

            return RedirectToAction("Index");
        }




        [HttpPost]
        public async Task<IActionResult> UpdatePhone(ProfileViewModel profile)
        {
            if (string.IsNullOrWhiteSpace(profile.Phone))
            {
                TempData["Error"] = "El número de teléfono no puede estar vacío.";
                return RedirectToAction("Index");
            }

            var json = JsonConvert.SerializeObject(profile.Phone);
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/users/{profile.UserId}/profile", content);

            if (response.IsSuccessStatusCode)
            {
                var user = GetLoggedUser();
                if (user != null)
                {
                    user.Phone = profile.Phone;
                    HttpContext.Session.SetString("LoggedUser", JsonConvert.SerializeObject(user));
                }

                TempData["Success"] = "Teléfono actualizado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo actualizar el teléfono.";
            }

            return RedirectToAction("Index");
        }

        private async Task<bool> UpdateUserProfilePictureAsync(int userId, string pictureUrl)
        {
            var userUpdate = new UpdateProfilePictureDTO { ProfilePictureUrl = pictureUrl };
            using var updateContent = new StringContent(JsonConvert.SerializeObject(userUpdate), System.Text.Encoding.UTF8, "application/json");

            var updateResponse = await _httpClient.PutAsync($"api/users/{userId}/profile-picture", updateContent);

            if (!updateResponse.IsSuccessStatusCode) return false;

            var user = GetLoggedUser();
            if (user != null)
            {
                user.ProfilePicture = pictureUrl;
                HttpContext.Session.SetString("LoggedUser", JsonConvert.SerializeObject(user));
            }

            return true;
        }
    }
}
