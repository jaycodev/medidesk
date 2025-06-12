using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sistema_citas_medicas.Models;
using sistema_citas_medicas.Models.ViewModels;
using sistema_citas_medicas.Servicio;

namespace sistema_citas_medicas.Controllers
{
    public class CuentaController : Controller
    {
        ServicioUsuario servicio = new ServicioUsuario();

        public ActionResult IniciarSesion()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IniciarSesion(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Usuario objUsuario = new Usuario
            {
                Correo = model.Email,
                Contraseña = model.Password
            };

            Usuario userLogged = servicio.operacionesLectura("LOGIN", objUsuario).FirstOrDefault();
            if (userLogged != null)
            {
                Session["usuario"] = userLogged;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Mensaje = "Correo o contraseña incorrectos";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CerrarSesion()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("IniciarSesion", "Cuenta");
        }

        public ActionResult Registrar()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(Usuario model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                int procesar = servicio.operacionesEscritura("INSERTAR", model);

                if (procesar >= 0)
                {
                    TempData["Success"] = "¡Usuario creado exitosamente!";
                    return RedirectToAction("IniciarSesion", "Cuenta");
                }

                ViewBag.Mensaje = "Hubo un error al crear el usuario.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ViewBag.Mensaje = "El correo electrónico ya está registrado. Por favor, usa otro.";
                }
                else
                {
                    ViewBag.Mensaje = "Ocurrió un error al procesar tu solicitud. Intenta nuevamente.";
                }
            }
            catch
            {
                ViewBag.Mensaje = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(model);
        }
    }
}