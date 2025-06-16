using ClosedXML.Excel;
using sistema_citas_medicas.Models;
using sistema_citas_medicas.Servicio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace sistema_citas_medicas.Controllers
{
    public class UsuarioController : Controller
    {
        ServicioUsuario servicio = new ServicioUsuario();
        ServicioPaciente servicioPaciente = new ServicioPaciente();
        ServicioMedico servicioMedico = new ServicioMedico();

        public Medico BuscarIdMedico(int id)
        {
            Medico objMedico = new Medico { IdUsuario = id };
            var resultado = servicioMedico.operacionesLectura("CONSULTAR_X_ID", objMedico).FirstOrDefault();

            if (resultado != null)
            {
                return resultado;
            }
            return null;

        }

        private Paciente BuscarIdPaciente(int id)
        {
            Paciente objPaciente = new Paciente { IdUsuario = id };
            var resultado = servicioPaciente.operacionesLectura("CONSULTAR_X_ID", objPaciente).FirstOrDefault();

            if (resultado != null)
            {
                return resultado;
            }
            return null;
        }

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

            ViewBag.Medico = BuscarIdMedico(id);
            ViewBag.Paciente = BuscarIdPaciente(id);

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

        public ActionResult ExportarPDF()
        {
            var usuarios = servicio.operacionesLectura("CONSULTAR_TODO", new Usuario());
            using (var ms = new System.IO.MemoryStream())
            {
                var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new iTextSharp.text.Paragraph("LISTA DE USUARIOS"));
                doc.Add(new iTextSharp.text.Paragraph(" "));

                var tabla = new iTextSharp.text.pdf.PdfPTable(6);
                tabla.WidthPercentage = 100;
                tabla.AddCell("ID");
                tabla.AddCell("Nombre");
                tabla.AddCell("Apellido");
                tabla.AddCell("Correo");
                tabla.AddCell("Teléfono");
                tabla.AddCell("Rol");

                foreach (var u in usuarios)
                {
                    tabla.AddCell(u.IdUsuario.ToString());
                    tabla.AddCell(u.Nombre);
                    tabla.AddCell(u.Apellido);
                    tabla.AddCell(u.Correo);
                    tabla.AddCell(u.Telefono ?? "");
                    tabla.AddCell(u.Rol);
                }

                doc.Add(tabla);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "Usuarios.pdf");
            }
        }

        public ActionResult ExportarExcel()
        {
            var usuarios = servicio.operacionesLectura("CONSULTAR_TODO", new Usuario());

            var stream = new MemoryStream(); // <-- fuera del using

            using (var workbook = new XLWorkbook())
            {
                var hoja = workbook.Worksheets.Add("Usuarios");

                hoja.Cell(1, 1).Value = "ID";
                hoja.Cell(1, 2).Value = "Nombre";
                hoja.Cell(1, 3).Value = "Apellido";
                hoja.Cell(1, 4).Value = "Correo";
                hoja.Cell(1, 5).Value = "Teléfono";
                hoja.Cell(1, 6).Value = "Rol";

                int fila = 2;
                foreach (var u in usuarios)
                {
                    hoja.Cell(fila, 1).Value = u.IdUsuario;
                    hoja.Cell(fila, 2).Value = u.Nombre;
                    hoja.Cell(fila, 3).Value = u.Apellido;
                    hoja.Cell(fila, 4).Value = u.Correo;
                    hoja.Cell(fila, 5).Value = u.Telefono ?? "";
                    hoja.Cell(fila, 6).Value = u.Rol;
                    fila++;
                }

                hoja.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }

            stream.Position = 0; // Importante: rebobinar

            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Usuarios.xlsx");
        }

    }
}
