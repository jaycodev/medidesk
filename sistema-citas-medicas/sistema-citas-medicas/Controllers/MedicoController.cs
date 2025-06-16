using ClosedXML.Excel;
using sistema_citas_medicas.Dao;
using sistema_citas_medicas.Dao.DaoImpl;
using sistema_citas_medicas.Models;
using sistema_citas_medicas.Servicio;
using System;
using System.Collections.Generic;
using System.IO;
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
            var existeCorreo = servicio.operacionesLectura("CONSULTAR_TODO", new Medico()).Any(m => m.Correo == objMedico.Correo);
            var existecontrasenia = servicio.operacionesLectura("CONSULTAR_TODO", new Medico()).Any(m => m.Contraseña == objMedico.Contraseña);

            if (existeCorreo)
            {
                ModelState.AddModelError("Correo", "El correo ya existe.");
            }
            if (existecontrasenia)
            {
                ModelState.AddModelError("Contraseña", "La Contraseña ya existe.");
            }

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
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar Médico. " + ex.Message;
                ModelState.AddModelError("", "No se pudo eliminar el Médico.");
            }
            return View(objMedico);
        }


        public ActionResult ExportarPdf()
        {
            var medico = servicio.operacionesLectura("CONSULTAR_TODO", new Medico());
            using (var ms = new System.IO.MemoryStream())
            {
                var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new iTextSharp.text.Paragraph("LISTA DE MÉDICOS"));
                doc.Add(new iTextSharp.text.Paragraph(" "));

                var tabla = new iTextSharp.text.pdf.PdfPTable(7);
                tabla.WidthPercentage = 100;
                tabla.AddCell("ID");
                tabla.AddCell("Nombre");
                tabla.AddCell("Apellido");
                tabla.AddCell("Correo");
                tabla.AddCell("Teléfono");
                tabla.AddCell("Rol");
                tabla.AddCell("Especialidad");

                foreach (var m in medico)
                {
                    tabla.AddCell(m.IdUsuario.ToString());
                    tabla.AddCell(m.Nombre);
                    tabla.AddCell(m.Apellido);
                    tabla.AddCell(m.Correo);
                    tabla.AddCell(m.Telefono ?? "");
                    tabla.AddCell(m.Rol);
                    tabla.AddCell(m.EspecialidadNombre);
                }

                doc.Add(tabla);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "Medicos.pdf");
            }
        }


        public ActionResult ExportarExcel()
        {
            var medicos = servicio.operacionesLectura("CONSULTAR_TODO", new Medico());

            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                var hoja = workbook.Worksheets.Add("Medicos");

                hoja.Cell(1, 1).Value = "ID";
                hoja.Cell(1, 2).Value = "Nombre";
                hoja.Cell(1, 3).Value = "Apellido";
                hoja.Cell(1, 4).Value = "Correo";
                hoja.Cell(1, 5).Value = "Teléfono";
                hoja.Cell(1, 6).Value = "Rol";
                hoja.Cell(1, 7).Value = "Especialidad";

                int fila = 2;
                foreach (var m in medicos)
                {
                    hoja.Cell(fila, 1).Value = m.IdUsuario;
                    hoja.Cell(fila, 2).Value = m.Nombre;
                    hoja.Cell(fila, 3).Value = m.Apellido;
                    hoja.Cell(fila, 4).Value = m.Correo;
                    hoja.Cell(fila, 5).Value = m.Telefono ?? "";
                    hoja.Cell(fila, 6).Value = m.Rol;
                    hoja.Cell(fila, 7).Value = m.EspecialidadNombre;
                    fila++;
                }

                hoja.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }

            stream.Position = 0; // Importante: rebobinar

            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Medicos.xlsx");
        }

    }
}