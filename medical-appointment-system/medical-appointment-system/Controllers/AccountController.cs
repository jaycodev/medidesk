using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using medical_appointment_system.Models;
using medical_appointment_system.Models.ViewModels;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
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
                if (userLogged.Roles?.Count == 1)
                {
                    userLogged.ActiveRole = userLogged.Roles.First();
                }

                Session["user"] = userLogged;
                return RedirectToAction("Home", "Appointments");
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
                model.Roles = new List<string> { "paciente" };
                int affectedRows = patientService.ExecuteWrite("INSERT", model);

                if (affectedRows > 0)
                {
                    TempData["Success"] = "¡Cuenta creada correctamente!";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.Message = "No se pudo crear la cuenta. Intenta nuevamente.";
                }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetActiveRole(string role)
        {
            var user = Session["user"] as User;

            if (user != null && user.Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase)))
            {
                user.ActiveRole = role;
                Session["user"] = user;
            }
            else if (user != null && user.Roles.Any())
            {
                user.ActiveRole = user.Roles.First();
                Session["user"] = user;
            }

            return RedirectToAction("Home", "Appointments");
        }
    }
}