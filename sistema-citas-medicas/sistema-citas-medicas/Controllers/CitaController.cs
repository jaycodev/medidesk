using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using sistema_citas_medicas.Models;
using sistema_citas_medicas.Servicio;
using static ClosedXML.Excel.XLPredefinedFormat;

namespace sistema_citas_medicas.Controllers
{
    public class CitaController : Controller
    {
        ServicioCitas servicio = new ServicioCitas();
        ServicioEspecialidad servicioesp = new ServicioEspecialidad();
        ServicioPaciente serviciopac = new ServicioPaciente();
        ServicioMedico serviciomed = new ServicioMedico();


        public ActionResult TableroCitas()
        {
            Usuario usuario = Session["usuario"] as Usuario;
            List<Cita> listacitasporpaciente = new List<Cita>();

            if (usuario == null)
            {
                return RedirectToAction("IniciarSesion", "Cuenta");
            }

            Cita cita = new Cita();

            if (usuario.Rol == "pacientes")
            {
                cita.IdPaciente = usuario.IdUsuario;
                listacitasporpaciente = servicio.operacionesLectura("CONSULTAR_TODO_X_ID_PACIENTE", cita);
            }
            if (usuario.Rol == "medicos")
            {
                cita.IdMedico = usuario.IdUsuario;
                listacitasporpaciente = servicio.operacionesLectura("CONSULTAR_TODO_X_ID_MEDICO", cita);
            }
            else
            {
                listacitasporpaciente = servicio.operacionesLectura("CONSULTAR_TODO", cita);
            }

            return View(listacitasporpaciente);
        }

        public ActionResult HistorialdeCitas()
        {
            List<Cita> listacomplete = servicio.operacionesLectura("CONSULTAR_TODO", new Cita());
            return View(listacomplete);
        }

        public ActionResult CitasPendiente()
        {
            List<Cita> listacomplete = servicio.operacionesLectura("CONSULTAR_TODO_PENDIENTE", new Cita());
            return View(listacomplete);
        }

        public ActionResult CitasConfirmadas()
        {
            List<Cita> listacomplete = servicio.operacionesLectura("CONSULTAR_TODO_CONFIRMADA", new Cita());
            return View(listacomplete);
        }

        public Cita BuscarID(int codigo)
        {
            Cita objcita = new Cita();
            objcita.IdCita = codigo;
            Cita objID = servicio.operacionesLectura("CONSULTAR_TODOXID", objcita).First();
            return objID;
        }

        public ActionResult DetallesCita(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(BuscarID(id));
        }

        [HttpGet]
        public ActionResult ReservarCita()
        {
            ViewBag.TiposConsulta = new List<SelectListItem> { new SelectListItem { Text = "Consulta", Value = "consulta" }, new SelectListItem { Text = "Examen", Value = "examen" }, new SelectListItem { Text = "Operación", Value = "operacion" } };
            ViewBag.TiposEspecialidad = new SelectList(servicioesp.operacionesLectura("CONSULTAR_TODO", new Especialidad()), "IdEspecialidad", "Nombre");
            ViewBag.Medicos = new SelectList(serviciomed.operacionesLectura("CONSULTAR_TODO", new Medico()), "IdUsuario", "Nombre");
            ViewBag.Paciente = new SelectList(serviciopac.operacionesLectura("CONSULTAR_TODO", new Paciente()), "IdUsuario", "Nombre");
            return View(new Cita());
        }

        [HttpPost]
        public ActionResult ReservarCita(Cita citita)
        {
            ViewBag.TiposConsulta = new List<SelectListItem> { new SelectListItem { Text = "Consulta", Value = "consulta" }, new SelectListItem { Text = "Examen", Value = "examen" }, new SelectListItem { Text = "Operación", Value = "operacion" } };
            ViewBag.TiposEspecialidad = new SelectList(servicioesp.operacionesLectura("CONSULTAR_TODO", new Especialidad()), "IdEspecialidad", "Nombre");
            ViewBag.Medicos = new SelectList(serviciomed.operacionesLectura("CONSULTAR_TODO", new Medico()), "IdUsuario", "Nombre");
            ViewBag.Paciente = new SelectList(serviciopac.operacionesLectura("CONSULTAR_TODO", new Paciente()), "IdUsuario", "Nombre");

            int procesar = servicio.operacionesEscritura("INSERTAR", citita);

            if (procesar >= 0)
            {
                TempData["Success"] = "¡Cita creada exitosamente!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("TableroCitas",citita);
        }


        [HttpGet]
        public ActionResult EliminarCita(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("HistorialdeCitas");
            }
            return View(BuscarID(id));
        }

        [HttpPost, ActionName("EliminarCita")]
        public ActionResult EliminarCita_Confirmar(int id)
        {
            Cita cita = BuscarID(id);

            try
            {
                int procesar = servicio.operacionesEscritura("ELIMINAR", cita);
                if (procesar >= 0)
                {
                    return RedirectToAction("CitasPendiente");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo eliminar el cita.");
            }
            return RedirectToAction("CitasPendiente");
        }
    }
}
