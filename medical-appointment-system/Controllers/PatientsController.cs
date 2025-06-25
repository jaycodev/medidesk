using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using medical_appointment_system.Models;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
{
    public class PatientsController : Controller
    {
        PatientService service = new PatientService();

        private Patient FindById(int id)
        {
            Patient patient = new Patient { UserId = id };
            var result = service.ExecuteRead("GET_BY_ID", patient).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }

        public ActionResult Index()
        {
            return View(service.ExecuteRead("GET_ALL", new Patient()));
        }

        public ActionResult Create()
        {
            return View(new Patient());
        }

        [HttpPost]
        public ActionResult Create(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return View(patient);
            }

            try
            {
                patient.Roles = new List<string> { "paciente" };
                int affectedRows = service.ExecuteWrite("INSERT", patient);

                if (affectedRows > 0)
                {
                    TempData["Success"] = "¡Paciente creado correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "No se pudo crear el paciente. Intenta nuevamente.";
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

            return View(patient);
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
        public ActionResult Edit(Patient patient)
        {
            try
            {
                int affectedRows = service.ExecuteWrite("UPDATE", patient);

                if (affectedRows > 0)
                {
                    TempData["Success"] = "¡Paciente actualizado correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "No se pudo actualizar el paciente. Intenta nuevamente.";
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(patient);
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