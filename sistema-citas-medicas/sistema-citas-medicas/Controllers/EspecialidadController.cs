using sistema_citas_medicas.Models;
using sistema_citas_medicas.Servicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Controllers
{
    public class EspecialidadController : Controller
    {
        ServicioEspecialidad servicio = new ServicioEspecialidad();

        // vistas
        public ActionResult Index()
        {
            List<Especialidad> lista = servicio.operacionesLectura("CONSULTAR_TODO", new Especialidad());
            return View(lista);
        }

        public ActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Crear(Especialidad objesp)
        {
            int procesar = servicio.operacionesEscritura("INSERTAR", objesp);
            if (procesar >= 0)
            {
                TempData["Success"] = "¡Especialidad agregada exitosamente!";
                return RedirectToAction("Index");
            }

            return View(objesp);
        }
        public ActionResult Editar(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(BuscarID(id));
        }

        [HttpPost]
        public ActionResult Editar(Especialidad objESp)
        {
            int procesar = servicio.operacionesEscritura("ACTUALIZAR", objESp);
            if (procesar >= 0)
            {
                TempData["Success"] = "¡Especialidad actualizado correctamente!";
                return RedirectToAction("Index");
            }
            return View(objESp);
        }
        public ActionResult Detalle(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(BuscarID(id));
        }
        public ActionResult Eliminar(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(BuscarID(id));
        }
        [HttpPost, ActionName("Eliminar")]
        public ActionResult Eliminar_Confirmacion(int id)
        {
            Especialidad objESp = BuscarID(id);

            try
            {
                int procesar = servicio.operacionesEscritura("ELIMINAR", objESp);
                if (procesar >= 0)
                {
                    TempData["Success"] = "¡Especialidad eliminado correctamente!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar la Especialidad. " + ex.Message;
                ModelState.AddModelError("", "No se pudo eliminar la Especialidad.");
            }
            return View(objESp);
        }
        //metodos

        public Especialidad BuscarID(int id)
        {
            Especialidad objEsp = new Especialidad();
            objEsp.IdEspecialidad = id;
            Especialidad objID = servicio.operacionesLectura("CONSULTAR_X_ID", objEsp).FirstOrDefault();
            return objID;
        }
    }
}