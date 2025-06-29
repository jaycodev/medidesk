using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Services.Description;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using medical_appointment_system.Models;
using medical_appointment_system.Services;
using System.Globalization;

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
                doctor.Roles = new List<string> { "medico" };
                int affectedRows = doctorService.ExecuteWrite("INSERT", doctor);

                if (affectedRows > 0)
                {
                    TempData["Success"] = "¡Médico creado correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "No se pudo crear el médico. Intenta nuevamente.";
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

            try
            {
                int affectedRows = doctorService.ExecuteWrite("UPDATE", doctor);

                if (affectedRows > 0)
                {
                    TempData["Success"] = "¡Médico actualizado correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "No se pudo actualizar el médico. Intenta nuevamente.";
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
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
            var doctors = doctorService.ExecuteRead("GET_ALL", new Doctor());
            using (var ms = new System.IO.MemoryStream())
            {
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var smallFont = FontFactory.GetFont("Arial", 9, Font.NORMAL);
                var boldFont = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.WHITE);
                var titleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);

                var now = DateTime.Now;
                string date = now.ToString("dd MMM yyyy");
                string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                PdfPTable headerTable = new PdfPTable(3);
                headerTable.WidthPercentage = 100;
                headerTable.SpacingBefore = 5f;
                headerTable.SpacingAfter = 10f;
                headerTable.SetWidths(new float[] { 2f, 6f, 2f });

                PdfPCell dateCell = new PdfPCell(new Phrase($"{date}", smallFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    PaddingTop = 4,
                    PaddingBottom = 4
                };
                PdfPCell titleCell = new PdfPCell(new Phrase("Lista de médicos", titleFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 4,
                    PaddingBottom = 4
                };
                PdfPCell timeCell = new PdfPCell(new Phrase($"{time}", smallFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    PaddingTop = 4,
                    PaddingBottom = 4
                };

                headerTable.AddCell(dateCell);
                headerTable.AddCell(titleCell);
                headerTable.AddCell(timeCell);
                doc.Add(headerTable);

                float[] columnWidths = { 1.5f, 2f, 2f, 3f, 2f, 2f };
                var table = new PdfPTable(6)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 5f
                };
                table.SetWidths(columnWidths);

                var headerColor = new BaseColor(0x0a, 0x76, 0xd8);
                string[] headers = { "Código", "Nombre(s)", "Apellido(s)", "Correo electrónico", "Especialidad", "Estado" };
                foreach (var h in headers)
                {
                    var cell = new PdfPCell(new Phrase(h, boldFont))
                    {
                        BackgroundColor = headerColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                foreach (var d in doctors)
                {
                    table.AddCell(new PdfPCell(new Phrase(d.UserId.ToString(), smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(d.FirstName, smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(d.LastName, smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(d.Email, smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(d.SpecialtyName, smallFont)) { Padding = 4 });

                    string statusText = d.Status ? "Activo" : "Inactivo";
                    var statusColor = d.Status ? new BaseColor(40, 167, 69) : new BaseColor(220, 53, 69);
                    var statusFont = FontFactory.GetFont("Arial", 9, Font.BOLD, statusColor);
                    table.AddCell(new PdfPCell(new Phrase(statusText, statusFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_CENTER });
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "Medicos.pdf");
            }
        }

        public ActionResult ExportToExcel()
        {
            var doctors = doctorService.ExecuteRead("GET_ALL", new Doctor());

            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Médicos");

                var now = DateTime.Now;
                string date = now.ToString("dd MMM yyyy");
                string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                sheet.Cell(1, 1).Value = date;
                sheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                sheet.Cell(1, 1).Style.Font.FontSize = 10;

                sheet.Range("B1:E1").Merge();
                sheet.Cell(1, 2).Value = "Lista de médicos";
                sheet.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cell(1, 2).Style.Font.Bold = true;
                sheet.Cell(1, 2).Style.Font.FontSize = 14;

                sheet.Cell(1, 6).Value = time;
                sheet.Cell(1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cell(1, 6).Style.Font.FontSize = 10;

                var headers = new[] { "Código", "Nombre(s)", "Apellido(s)", "Correo electrónico", "Especialidad", "Estado" };
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
                foreach (var d in doctors)
                {
                    sheet.Cell(row, 1).Value = d.UserId;
                    sheet.Cell(row, 2).Value = d.FirstName;
                    sheet.Cell(row, 3).Value = d.LastName;
                    sheet.Cell(row, 4).Value = d.Email;
                    sheet.Cell(row, 5).Value = d.SpecialtyName;

                    var statusCell = sheet.Cell(row, 6);
                    statusCell.Value = d.Status ? "Activo" : "Inactivo";
                    statusCell.Style.Font.FontColor = d.Status ? XLColor.FromHtml("#28a745") : XLColor.FromHtml("#dc3545");
                    statusCell.Style.Font.Bold = true;

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

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Medicos.xlsx"
            );
        }
    }
}