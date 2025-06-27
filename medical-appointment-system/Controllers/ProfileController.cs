using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using medical_appointment_system.Models;
using medical_appointment_system.Models.ViewModels;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
{
    public class ProfileController : Controller
    {
        UserService userService = new UserService();
        CloudinaryService cloudinaryService = new CloudinaryService();

        private void LoadAvatarsToViewBag()
        {
            string folderPath = Server.MapPath("~/Content/Img/Avatars/");
            string baseUrl = Url.Content("~/Content/Img/Avatars/");


            if (!Directory.Exists(folderPath))
            {
                ViewBag.Avatars = new List<string>();
                return;
            }

            var avatarFiles = Directory.GetFiles(folderPath)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
                .Select(f => baseUrl + Path.GetFileName(f))
                .ToList();

            ViewBag.Avatars = avatarFiles;
        }

        public ActionResult Index()
        {
            User userSession = Session["user"] as User;

            var modelProfile = new ProfileViewModel
            {
                User = userSession,
                ChangePassword = new ChangePasswordValidator
                {
                    UserId = userSession.UserId
                }
            };

            LoadAvatarsToViewBag();

            return View(modelProfile);
        }

        [HttpPost]
        public ActionResult Update(ProfileViewModel profile)
        {
            try
            {
                int affectedRows = userService.ExecuteWrite("UPDATE_PROFILE", profile.User);

                if (affectedRows > 0)
                {
                    var userSession = Session["user"] as User;
                    if (userSession != null && userSession.UserId == profile.User.UserId)
                    {
                        userSession.Phone = profile.User.Phone;
                        Session["user"] = userSession;
                    }

                    TempData["Success"] = "¡Perfil actualizado correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "No se pudo actualizar el perfil. Intenta nuevamente.";
                    LoadAvatarsToViewBag();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                LoadAvatarsToViewBag();
            }

            LoadAvatarsToViewBag();
            return View("Index", profile);
        }

        [HttpPost]
        public ActionResult ChangePassword(ProfileViewModel model)
        {
            User user = new User { UserId = model.ChangePassword.UserId, Password = model.ChangePassword.NewPassword };


            if (model.ChangePassword.NewPassword != model.ChangePassword.ConfirmPassword)
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                return RedirectToAction("Index");
            }

            try
            {
                user.Password = model.ChangePassword.NewPassword;

                int rows = userService.ExecuteWrite("UPDATE_PASSWORD", model.ChangePassword);
                if (rows > 0)
                {
                    TempData["Success"] = "Contraseña actualizada correctamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar la contraseña: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> EditProfilePicture(int userId, HttpPostedFileBase ProfilePictureFile)
        {
            if (ProfilePictureFile != null && ProfilePictureFile.ContentLength > 0)
            {
                string tempPath = null;
                try
                {
                    var fileName = Path.GetFileName(ProfilePictureFile.FileName);
                    tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_" + fileName);
                    ProfilePictureFile.SaveAs(tempPath);

                    string urlImagen = await cloudinaryService.UploadImageAsync(tempPath, "MedicalAppointmentsDB/UserPhotos", fileName);

                    User user = new User
                    {
                        UserId = userId,
                        ProfilePicture = urlImagen
                    };

                    int rows = userService.ExecuteWrite("UPDATE_PROFILE_PICTURE", user);
                    if (rows > 0)
                    {
                        User userSession = Session["user"] as User;
                        if (userSession != null && userSession.UserId == userId)
                        {
                            userSession.ProfilePicture = urlImagen;
                            Session["user"] = userSession;
                        }

                        return Json(new { success = true, message = "Foto de perfil actualizada correctamente." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "No se pudo actualizar la base de datos." });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al actualizar la foto: " + ex.Message });
                }
                finally
                {
                    if (!string.IsNullOrEmpty(tempPath) && System.IO.File.Exists(tempPath))
                        System.IO.File.Delete(tempPath);
                }
            }

            return Json(new { success = false, message = "Debe seleccionar una imagen válida." });
        }

        [HttpPost]
        public ActionResult SelectAvatar(int userId, string SelectedAvatar)
        {
            if (string.IsNullOrWhiteSpace(SelectedAvatar))
            {
                TempData["Error"] = "Debe seleccionar un avatar.";
                return RedirectToAction("Index");
            }

            try
            {
                var user = new User
                {
                    UserId = userId,
                    ProfilePicture = SelectedAvatar
                };

                int rows = userService.ExecuteWrite("UPDATE_PROFILE_PICTURE", user);

                if (rows > 0)
                {
                    TempData["Success"] = "Avatar seleccionado correctamente.";

                    var userSession = Session["user"] as User;
                    if (userSession != null && userSession.UserId == userId)
                    {
                        userSession.ProfilePicture = SelectedAvatar;
                        Session["user"] = userSession;
                    }
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
    }
}