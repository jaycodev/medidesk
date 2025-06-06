using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Controllers
{
    public class CuentaController : Controller
    {
        public ActionResult IniciarSesion()
        {
            return View();
        }
    }
}