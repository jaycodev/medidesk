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
            return View(new Specialty());
        }

        [HttpPost]
        public ActionResult Create(Specialty specialty)
        {
            if (!ModelState.IsValid)
            {
                return View(specialty);
            }

            try
            {
                int affectedRows = service.ExecuteWrite("INSERT", specialty);

                if (affectedRows > 0)
                {
                    TempData["Success"] = "¡Especialidad creada correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "No se pudo crear la especialidad. Intenta nuevamente.";
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
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
        public ActionResult Edit(Specialty specialty)
        {
            if (!ModelState.IsValid)
            {
                return View(specialty);
            }

            try
            {
                int affectedRows = service.ExecuteWrite("UPDATE", specialty);

                if (affectedRows > 0)
                {
                    TempData["Success"] = "¡Especialidad actualizada correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "No se pudo actualizar la especialidad. Intenta nuevamente.";
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(specialty);
        }

        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            return View(FindById(id));
        }
    }
}