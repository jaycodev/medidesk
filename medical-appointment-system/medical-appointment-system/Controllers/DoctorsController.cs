using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using medical_appointment_system.Models;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
{
    public class DoctorsController : Controller
    {
        DoctorService doctorService = new DoctorService();
        SpecialtyService specialtyService = new SpecialtyService();

        private Doctor FindById(int id)
        {
            Doctor doctor = new Doctor { UserId = id };
            var result = doctorService.ExecuteRead("GET_BY_ID", doctor).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;

        }

        private void LoadSpecialties(int? selectedId = null)
        {
            ViewBag.Specialties = new SelectList(
                specialtyService.ExecuteRead("GET_ALL", new Specialty()),
                "SpecialtyId",
                "Name",
                selectedId
            );
        }

        public ActionResult Index()
        {
            return View(doctorService.ExecuteRead("GET_ALL", new Doctor()));
        }

        public ActionResult Create()
        {
            LoadSpecialties();

            return View(new Doctor());
        }

        [HttpPost]
        public ActionResult Create(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                LoadSpecialties(doctor.SpecialtyId);
                return View(doctor);
            }

            try
            {
                doctor.Role = "medico";
                doctor.Status = true;
                doctorService.ExecuteWrite("INSERT", doctor);
                TempData["Success"] = "¡Médico creado correctamente!";
                return RedirectToAction("Index");
            }
            catch (ApplicationException ex)
            {
                ViewBag.Message = ex.Message;
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            LoadSpecialties(doctor.SpecialtyId);

            return View(doctor);
        }

        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            Doctor doctor = FindById(id);

            LoadSpecialties(doctor.SpecialtyId);

            return View(doctor);
        }

        [HttpPost]
        public ActionResult Edit(Doctor doctor)
        {

            LoadSpecialties(doctor.SpecialtyId);

            int process = doctorService.ExecuteWrite("UPDATE", doctor);

            if (process >= 0)
            {
                TempData["Success"] = "¡Médico actualizado correctamente!";
                return RedirectToAction("Index");
            }

            return View(doctor);
        }

        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            return View(FindById(id));
        }

        public ActionResult ExportToPDF()
        {
            var medico = doctorService.ExecuteRead("GET_ALL", new Doctor());
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
                    tabla.AddCell(m.UserId.ToString());
                    tabla.AddCell(m.FirstName);
                    tabla.AddCell(m.LastName);
                    tabla.AddCell(m.Email);
                    tabla.AddCell(m.Phone ?? "");
                    tabla.AddCell(m.Role);
                    tabla.AddCell(m.SpecialtyName);
                }

                doc.Add(tabla);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "Medicos.pdf");
            }
        }

        public ActionResult ExportToExcel()
        {
            var medicos = doctorService.ExecuteRead("GET_ALL", new Doctor());

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
                    hoja.Cell(fila, 1).Value = m.UserId;
                    hoja.Cell(fila, 2).Value = m.FirstName;
                    hoja.Cell(fila, 3).Value = m.LastName;
                    hoja.Cell(fila, 4).Value = m.Email;
                    hoja.Cell(fila, 5).Value = m.Phone ?? "";
                    hoja.Cell(fila, 6).Value = m.Role;
                    hoja.Cell(fila, 7).Value = m.SpecialtyName;
                    fila++;
                }

                hoja.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }

            stream.Position = 0;

            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Medicos.xlsx");
        }
    }
}