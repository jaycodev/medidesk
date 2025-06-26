using medical_appointment_system.Models;
using medical_appointment_system.Models.Validators;
using medical_appointment_system.Models.ViewModels;
using medical_appointment_system.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace medical_appointment_system.Controllers
{
    public class ProfileController : Controller
    {
        UserService userService = new UserService();
        CloudinaryService cloudinaryService = new CloudinaryService();

        private User FindUserById(int id)
        {
            User user = new Patient { UserId = id };
            User result = userService.ExecuteRead("GET_BY_ID", user).First();

            if (result != null)
            {
                result.SelectedRoleCombo = string.Join(",", result.Roles);
                System.Diagnostics.Debug.WriteLine("SelectedRoleCombo: " + result.SelectedRoleCombo);

                return result;
            }

            return null;
        }

        // GET: Profile
        public ActionResult ProfileUser()
        {

            User userSession = Session["user"] as User;

            User user = null;
            if (userSession == null || (user = FindUserById(userSession.UserId)) == null)
            {
                return RedirectToAction("Index");
            }

            var modelProfile = new ProfileViewModel
            {
                User = user,
                ChangePassword = new ChangePasswordValidator
                {
                    UserId = user.UserId
                },
                ChangePhone = new ChangePhoneValidator
                {
                    UserId = user.UserId
                }
            };

            string folderPath = Server.MapPath("~/Content/Img/Avatars/");
            string baseUrl = Url.Content("~/Content/Img/Avatars/");

            var avatarFiles = Directory.GetFiles(folderPath)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
                .Select(f => baseUrl + Path.GetFileName(f))
                .ToList();

            ViewBag.Avatars = avatarFiles;

            return View(modelProfile);
        }

        [HttpPost]
        public ActionResult ChangePassword(ProfileViewModel model)
        {

            User user = new User { UserId = model.ChangePassword.UserId, Password = model.ChangePassword.NewPassword };


            if (model.ChangePassword.NewPassword != model.ChangePassword.ConfirmPassword)
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                return RedirectToAction("ProfileUser");
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

            return RedirectToAction("ProfileUser");
        }


        [HttpPost]
        public ActionResult EditPhone(ProfileViewModel model)
        {
            try
            {
                var user = new User
                {
                    UserId = model.ChangePhone.UserId,
                    Phone = model.ChangePhone.Phone
                };

                int rows = userService.ExecuteWrite("UPDATE_PHONE", user);
                if (rows > 0)
                {
                    TempData["Success"] = "Número de teléfono actualizado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo actualizar el teléfono.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el teléfono: " + ex.Message;
            }

            return RedirectToAction("ProfileUser");
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
                return RedirectToAction("ProfileUser");
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

                    // Actualizar la sesión si es el usuario logueado
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

            return RedirectToAction("ProfileUser");
        }
    }
}