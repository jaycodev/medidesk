using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Controllers
{
    public class PacienteController : Controller
    {
         private IPacienteDao pacienteDao = new PacienteDao();
        public ActionResult Index()
        {
            var listaPacientes = pacienteDao.Listar();     
            return View(listaPacientes);
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Crear(PacienteViewModel model)
        {
            if (ModelState.IsValid)
            {
                pacienteDao.Registrar(model);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}