using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Mappers;
using Web.Models.Account;
using Web.Models.Profile;
using Web.Services.Profile;

public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _env;

    public ProfileController(IProfileService profileService, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
    {
        _profileService = profileService;
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

    private UserSession? GetLoggedUser()
    {
        var userJson = _httpContextAccessor.HttpContext!.Session.GetString("UserSession");
        return string.IsNullOrEmpty(userJson)
            ? null
            : JsonConvert.DeserializeObject<UserSession>(userJson);
    }

    [HttpGet]
    public IActionResult Index()
    {
        var user = GetLoggedUser();
        if (user == null) return RedirectToAction("Login", "Account");

        LoadAvatarsToViewBag();

        var model = ProfileMapper.ToViewModel(user);

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

        bool success = await _profileService.ChangePasswordAsync(model.ChangePassword);
        TempData[success ? "Success" : "Error"] =
            success ? "Contraseña actualizada correctamente." : "Error al actualizar la contraseña.";

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
            string? uploadedUrl = await _profileService.UploadProfilePictureAsync(profilePictureFile);
            if (uploadedUrl == null)
            {
                TempData["Error"] = "Error al subir la foto.";
                return RedirectToAction("Index");
            }

            bool success = await _profileService.UpdateProfilePictureAsync(userId, uploadedUrl);

            if (success)
            {
                var user = GetLoggedUser();
                if (user != null)
                {
                    user.ProfilePicture = uploadedUrl;
                    HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(user));
                }

                TempData["Success"] = "Foto de perfil actualizada correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo actualizar la base de datos.";
            }
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
            bool success = await _profileService.UpdateProfilePictureAsync(userId, SelectedAvatar);

            if (success)
            {
                var user = GetLoggedUser();
                if (user != null)
                {
                    user.ProfilePicture = SelectedAvatar;
                    HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(user));
                }

                TempData["Success"] = "Avatar seleccionado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo actualizar el avatar.";
            }
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

        bool success = await _profileService.UpdateProfileAsync(profile.UserId, profile.Phone);

        if (success)
        {
            var user = GetLoggedUser();
            if (user != null)
            {
                user.Phone = profile.Phone;
                HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(user));
            }

            TempData["Success"] = "Teléfono actualizado correctamente.";
        }
        else
        {
            TempData["Error"] = "No se pudo actualizar el teléfono.";
        }

        return RedirectToAction("Index");
    }
}
