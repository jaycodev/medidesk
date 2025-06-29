using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using medical_appointment_system.Models;
using medical_appointment_system.Services;
using System.Globalization;

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

        public ActionResult ExportToExcel()
        {
            var patients = service.ExecuteRead("GET_ALL", new Patient());

            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Pacientes");

                var now = DateTime.Now;
                string date = now.ToString("dd MMM yyyy");
                string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                sheet.Cell(1, 1).Value = date;
                sheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                sheet.Cell(1, 1).Style.Font.FontSize = 10;

                sheet.Range("B1:E1").Merge();
                sheet.Cell(1, 2).Value = "Lista de pacientes";
                sheet.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cell(1, 2).Style.Font.Bold = true;
                sheet.Cell(1, 2).Style.Font.FontSize = 14;

                sheet.Cell(1, 6).Value = time;
                sheet.Cell(1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cell(1, 6).Style.Font.FontSize = 10;

                var headers = new[] { "Código", "Nombre(s)", "Apellido(s)", "Correo electrónico", "Fecha nacimiento", "Grupo sanguíneo" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = sheet.Cell(3, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0a76d8");
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Font.FontSize = 10;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.Black;
                }

                int row = 4;
                foreach (var p in patients)
                {
                    sheet.Cell(row, 1).Value = p.UserId;
                    sheet.Cell(row, 2).Value = p.FirstName;
                    sheet.Cell(row, 3).Value = p.LastName;
                    sheet.Cell(row, 4).Value = p.Email;
                    sheet.Cell(row, 5).Value = p.BirthDate?.ToString("dd MMM yyyy") ?? "-";
                    sheet.Cell(row, 6).Value = p.BloodType;

                    for (int c = 1; c <= 6; c++)
                    {
                        var cell = sheet.Cell(row, c);
                        cell.Style.Font.FontSize = 9;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = XLColor.Black;
                    }

                    row++;
                }

                sheet.Columns().AdjustToContents();
                workbook.SaveAs(stream);
            }

            stream.Position = 0;

            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Pacientes.xlsx");
        }
    }
}