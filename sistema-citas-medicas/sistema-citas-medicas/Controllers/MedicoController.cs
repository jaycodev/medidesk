using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Controllers
{
    public class MedicoController : Controller
    {
        IMedicoDao dao = new MedicoDaoImpl();
        public ActionResult Index(string txtnom = "")
        {
            Medico objmed = new Medico();

            objmed.Nombre = txtnom;

            List<Medico> lista = dao.consultarTodo("CONSULTAR_TODO", objmed);
            return View(lista);
        }
    }
}