using System;
using System.Linq;
using System.Web.Mvc;
using sistema_citas_medicas.Models;
using sistema_citas_medicas.Models.ViewModels;
using sistema_citas_medicas.Services;

namespace sistema_citas_medicas.Controllers
{
    public class AccountController : Controller
    {
        UserService userService = new UserService();
        PatientService patientService = new PatientService();

        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = new User
            {
                Email = model.Email,
                Password = model.Password
            };

            User userLogged = userService.ExecuteRead("LOGIN", user).FirstOrDefault();
            if (userLogged != null)
            {
                Session["user"] = userLogged;
                return RedirectToAction("Dashboard", "Appointments");
            }
            else
            {
                ViewBag.Message = "Correo o contraseña incorrectos";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Register()
        {
            return View(new Patient());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Patient model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.Role = "paciente";
                patientService.ExecuteWrite("INSERT", model);
                
                TempData["Success"] = "¡Usuario creado correctamente!";
                return RedirectToAction("Login", "Account");
            }
            catch (ApplicationException ex)
            {
                ViewBag.Message = ex.Message;
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(model);
        }
    }
}