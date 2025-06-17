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

        private Usuario usuario;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            usuario = Session["usuario"] as Usuario;
        }

        public ActionResult TableroCitas()
        {
            var filtro = new Cita
            {
                IdUsuario = usuario.IdUsuario,
                TipoUsuario = usuario.Rol,
                Estado = null
            };

            var lista = servicio.operacionesLectura("CONSULTAR_X_USUARIO_Y_ESTADO", filtro);
            return View(lista);
        }

        public ActionResult HistorialdeCitas()
        {
            var filtro = new Cita
            {
                IdUsuario = usuario.IdUsuario,
                TipoUsuario = usuario.Rol
            };

            var lista = servicio.operacionesLectura("CONSULTAR_CANCELADAS_Y_ATENDIDAS_X_USUARIO", filtro);
            return View(lista);
        }

        public ActionResult CitasPendiente()
        {
            var filtro = new Cita
            {
                IdUsuario = usuario.IdUsuario,
                TipoUsuario = usuario.Rol,
                Estado = "pendiente"
            };

            var lista = servicio.operacionesLectura("CONSULTAR_X_USUARIO_Y_ESTADO", filtro);
            return View(lista);
        }

        public ActionResult CitasConfirmadas()
        {
            var filtro = new Cita
            {
                IdUsuario = usuario.IdUsuario,
                TipoUsuario = usuario.Rol,
                Estado = "confirmada"
            };

            var lista = servicio.operacionesLectura("CONSULTAR_X_USUARIO_Y_ESTADO", filtro);
            return View(lista);
        }

        public Cita BuscarID(int codigo)
        {
            Cita objcita = new Cita();
            objcita.IdCita = codigo;
            Cita objID = servicio.operacionesLectura("CONSULTAR_X_ID", objcita).First();
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
