using sistema_citas_medicas.Models;
using sistema_citas_medicas.Servicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Controllers
{
    public class UsuarioController : Controller
    {
        ServicioUsuario servicio = new ServicioUsuario();
        // GET: Usuarios
        public ActionResult Index()
        {
            
            List<Usuario> lista = servicio.operacionesLectura("CONSULTAR_TODO", new Usuario());
            return View(lista);
        }

        public Usuario BuscarID(int codigo)
        {
            Usuario objUsuario = new Usuario();
            objUsuario.IdUsuario = codigo;
            Usuario objID = servicio.operacionesLectura("CONSULTAR_X_ID", objUsuario).First();
            return objID;
        }

        [HttpGet]
        public ActionResult Crear()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Editar(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(BuscarID(id));
        }

        [HttpGet]
        public ActionResult Detalle(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(BuscarID(id));
        }

        [HttpGet]
        public ActionResult Eliminar(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(BuscarID(id));
        }

        [HttpPost]
        public ActionResult Crear(Usuario objUsu)
        {
            int procesar = servicio.operacionesEscritura("INSERTAR", objUsu);
            if (procesar >= 0)
            {
                TempData["Success"] = "¡Usuario creado exitosamente!";
                return RedirectToAction("Index");
            }
            
            return View(objUsu);
        }

        [HttpPost]
        public ActionResult Editar(Usuario objReg)
        {
            int procesar = servicio.operacionesEscritura("ACTUALIZAR", objReg);
            if (procesar >= 0)
            {
                TempData["Success"] = "¡Usuario actualizado correctamente!";
                return RedirectToAction("Index");
            }
            return View(objReg);
        }

        [HttpPost, ActionName("Eliminar")]
        public ActionResult Eliminar_Confirmacion(int id)
        {
            Usuario objUsu = BuscarID(id);

            try
            {
                int procesar = servicio.operacionesEscritura("ELIMINAR", objUsu);
                if (procesar >= 0)
                {
                    TempData["Success"] = "¡Usuario eliminado correctamente!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar el usuario. "+ex.Message;
                ModelState.AddModelError("", "No se pudo eliminar el usuario.");
            }
            return View(objUsu);
        }
    }
}
