using System;
using System.Linq;
using System.Web.Mvc;
using sistema_citas_medicas.Models;
using sistema_citas_medicas.Servicio;

namespace sistema_citas_medicas.Controllers
{
    public class PacienteController : Controller
    {
        ServicioPaciente servicio = new ServicioPaciente();

        private Paciente BuscarId(int id)
        {
            Paciente objPaciente = new Paciente { IdUsuario = id };
            var resultado = servicio.operacionesLectura("CONSULTAR_X_ID", objPaciente).FirstOrDefault();

            if (resultado != null)
            {
                return resultado;
            }
            return null;
        }

        public ActionResult Index()
        {
            var listaPacientes = servicio.operacionesLectura("CONSULTAR_TODO", new Paciente());
            return View(listaPacientes);
        }

        public ActionResult Crear()
        {
            return View(new Paciente());
        }

        [HttpPost]
        public ActionResult Crear(Paciente objPaciente)
        {
            if (!ModelState.IsValid)
            {
                return View(objPaciente);
            }

            try
            {
                servicio.operacionesEscritura("INSERTAR", objPaciente);
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

        public ActionResult Editar(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            return View(BuscarId(id));
        }

        [HttpPost]
        public ActionResult Editar(Paciente objPaciente)
        {
            int procesar = servicio.operacionesEscritura("ACTUALIZAR", objPaciente);

            if (procesar >= 0)
            {
                TempData["Success"] = "¡Paciente actualizado correctamente!";
                return RedirectToAction("Index");
            }

            return View(objPaciente);
        }

        public ActionResult Detalle(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            return View(BuscarId(id));
        }
    }
}