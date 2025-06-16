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
        ServicioUsuario servicioUsuario = new ServicioUsuario();
        ServicioPaciente servicioPaciente = new ServicioPaciente();

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

            Usuario userLogged = servicioUsuario.operacionesLectura("LOGIN", objUsuario).FirstOrDefault();
            if (userLogged != null)
            {
                Session["usuario"] = userLogged;
                return RedirectToAction("TableroCitas", "Cita");
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
            return View(new Paciente());
        }

        public ActionResult Crear(Paciente objPaciente)
        {
            if (!ModelState.IsValid)
            {
                return View(objPaciente);
            }

            try
            {
                servicioPaciente.operacionesEscritura("INSERTAR", objPaciente);
                TempData["Success"] = "¡Paciente creado correctamente!";
                return RedirectToAction("Index");
            }
            catch (ApplicationException ex)
            {
                ViewBag.MensajeError = ex.Message;
            }
            catch (Exception)
            {
                ViewBag.MensajeError = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(objPaciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(Paciente model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                int procesar = servicioPaciente.operacionesEscritura("INSERTAR", model);

                TempData["Success"] = "¡Usuario creado exitosamente!";
                return RedirectToAction("IniciarSesion", "Cuenta");
            }
            catch (ApplicationException ex)
            {
                ViewBag.Mensaje = ex.Message;
            }
            catch (Exception)
            {
                ViewBag.Mensaje = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(model);
        }
    }
}