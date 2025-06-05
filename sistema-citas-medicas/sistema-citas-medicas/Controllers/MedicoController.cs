using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;
using sistema_citas_medicas.Servicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace sistema_citas_medicas.Controllers
{
    public class MedicoController : Controller
    {
        
        ServicioMedico servicio = new ServicioMedico();
        ServicioEspecialidad servEsp = new ServicioEspecialidad();

        Especialidad objeEsp = new Especialidad();

        //metodos
        public Medico BuscarId(int id)
        {
            Medico objMedico = new Medico { IdUsuario = id };
            var resultado = servicio.operacionesLectura("CONSULTAR_X_ID", objMedico).FirstOrDefault();

            if (resultado != null)
            {
                return resultado;
            }
            return null;

        }
        //vistas
        public ActionResult Index(string txtnom = "")
        {
            Medico objmed = new Medico();
            objmed.Nombre = txtnom;
            List<Medico> lista = servicio.operacionesLectura("CONSULTAR_TODO", objmed);
            return View(lista);
        }
        [HttpGet]
        public ActionResult Crear()
        {
            ViewBag.especialidades = new SelectList(
                servEsp.operacionesLectura("CONSULTAR_TODO", objeEsp), 
                "IdEspecialidad", 
                "Nombre");
            return View();
        }
        [HttpPost]
        public ActionResult Crear(Medico objMedico)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.especialidades = new SelectList(
                servEsp.operacionesLectura("CONSULTAR_TODO", objeEsp), 
                "IdEspecialidad", 
                "Nombre",
                servEsp.operacionesLectura("CONSULTAR_TODO", objeEsp));
            }

            int procesar = servicio.operacionesEscritura("INSERTAR", objMedico);
            if(procesar >= 0)
            {
                TempData["success"] = "¡Médico creado exitosamente!";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "No se pudo crear el médico.";
            return View(objMedico);
        }

        [HttpGet]
        public ActionResult Editar(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            Medico objMedico = BuscarId(id);
            ViewBag.especialidades = new SelectList(
                    servEsp.operacionesLectura("CONSULTAR_TODO", objeEsp),
                    "IdEspecialidad",
                    "Nombre",
                    objMedico.IdEspecialidad);

            return View(objMedico);

        }
        [HttpPost]
        public ActionResult Editar(Medico objMedico)
        {
            
            ViewBag.especialidades = new SelectList(
                servEsp.operacionesLectura("CONSULTAR_TODO", objeEsp),
                "IdEspecialidad",
                "Nombre",
                objMedico.IdEspecialidad);

            
            int procesar = servicio.operacionesEscritura("ACTUALIZAR", objMedico);

            if (procesar >= 0)
            {

                TempData["Success"] = "¡Médico actualizado correctamente!";
                return RedirectToAction("Index");
            }
            return View(objMedico);

        }
        [HttpGet]
        public ActionResult Detalle(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(BuscarId(id));
        }
        [HttpGet]
        public ActionResult Eliminar(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(BuscarId(id));
        }

        [HttpPost, ActionName("Eliminar")]
        public ActionResult Eliminar_Confirmacion(int id)
        {
            Medico objMedico = BuscarId(id);
            

            try
            {
                int procesar = servicio.operacionesEscritura("ELIMINAR", objMedico);
                if (procesar >= 0)
                {
                    TempData["Success"] = "¡Médico eliminado correctamente!";
                    return RedirectToAction("Index");
                }
                TempData["Error"] = "No se pudo eliminar el médico.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar Médico. " + ex.Message;
                ModelState.AddModelError("", "No se pudo eliminar el Médico.");
            }
            return View(objMedico);
        }
    }
}