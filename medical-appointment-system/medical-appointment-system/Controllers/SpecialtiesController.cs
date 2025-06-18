using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using medical_appointment_system.Models;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
{
    public class SpecialtiesController : Controller
    {
        SpecialtyService service = new SpecialtyService();

        private Specialty FindById(int id)
        {
            Specialty specialty = new Specialty { SpecialtyId = id };
            var result = service.ExecuteRead("GET_BY_ID", specialty).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }

        public ActionResult Index()
        {
            List<Specialty> list = service.ExecuteRead("GET_ALL", new Specialty());
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Specialty specialty)
        {
            int process = service.ExecuteWrite("INSERT", specialty);
            if (process >= 0)
            {
                TempData["Success"] = "¡Especialidad agregada exitosamente!";
                return RedirectToAction("Index");
            }

            return View(specialty);
        }

        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            return View(FindById(id));
        }

        [HttpPost]
        public ActionResult Edit(Specialty objSpec)
        {
            int procesar = service.ExecuteWrite("UPDATE", objSpec);
            if (procesar >= 0)
            {
                TempData["Success"] = "¡Especialidad actualizado correctamente!";
                return RedirectToAction("Index");
            }
            return View(objSpec);
        }

        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(FindById(id));
        }

        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(FindById(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Specialty specialty = FindById(id);

            try
            {
                int procesar = service.ExecuteWrite("DELETE", specialty);
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
            return View(specialty);
        }
    }
}