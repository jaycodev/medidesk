using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text;
using medical_appointment_system.Models;
using medical_appointment_system.Services;
using System.IO;
using System.Globalization;
using ClosedXML.Excel;

namespace medical_appointment_system.Controllers
{
    public class AppointmentsController : Controller
    {
        AppointmentService appointmentService = new AppointmentService();
        SpecialtyService specialtyService = new SpecialtyService();
        PatientService patientService = new PatientService();
        DoctorService doctorService = new DoctorService();
        NotificationService notificationService = new NotificationService();

        private User user;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            user = Session["user"] as User;
        }

        private Appointment GetAppointmentIds(int id)
        {
            var filter = new Appointment { AppointmentId = id };
            var result = appointmentService.ExecuteRead("GET_IDS_BY_ID", filter);
            return result.FirstOrDefault();
        }

        private Appointment FindById(int id)
        {
            var filter = new Appointment { AppointmentId = id };
            var results = appointmentService.ExecuteRead("GET_BY_ID", filter);

            return results.FirstOrDefault();
        }

        private List<Appointment> GetAppointmentsByStatus(string status, string indicator)
        {
            return appointmentService.ExecuteRead(indicator, new Appointment
            {
                UserId = user.UserId,
                UserRol = user.ActiveRole,
                Status = status
            });
        }

        public ActionResult Home()
        {
            var list = GetAppointmentsByStatus(null, "GET_BY_USER_AND_STATUS");
            return View(list);
        }

        public ActionResult AllAppointments()
        {
            var list = appointmentService.ExecuteRead("GET_ALL", new Appointment());
            return View(list);
        }

        public ActionResult MyAppointments()
        {
            var list = GetAppointmentsByStatus("confirmada", "GET_BY_USER_AND_STATUS");
            return View(list);
        }

        public ActionResult Pending()
        {
            var list = GetAppointmentsByStatus("pendiente", "GET_BY_USER_AND_STATUS");
            return View(list);
        }

        public ActionResult Historial()
        {
            var list = GetAppointmentsByStatus(null, "GET_COMPLETED_OR_CANCELLED_BY_USER");
            return View(list);
        }

        public ActionResult Details(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            return View(FindById(id));
        }

        public ActionResult Reserve()
        {
            ViewBag.Specialties = new SelectList(specialtyService.ExecuteRead("GET_ALL", new Specialty()), "SpecialtyId", "Name");

            return View(new Appointment());
        }

        public JsonResult GetDoctorsBySpecialty(int id)
        {
            var doctors = doctorService.ExecuteRead("GET_BY_SPECIALTY", new Doctor
            {
                SpecialtyId = id,
                UserId = user.UserId
            });

            var result = doctors.Select(d => new
            {
                d.UserId,
                FullName = $"{d.FirstName} {d.LastName}"
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public List<Appointment> GetDoctorScheduleByDay(int doctorId, DateTime date)
        {
            var result = appointmentService.ExecuteRead("GET_SCHEDULE_BY_DOCTOR_AND_DAY", new Appointment
            {
                DoctorId = doctorId,
                Date = date
            });

            var shifts = new List<Appointment>();

            foreach (var row in result)
            {
                shifts.Add(new Appointment
                {
                    DayWorkShift = row.DayWorkShift,
                    StartTime = row.StartTime,
                    EndTime = row.EndTime
                });
            }

            return shifts;
        }

        public JsonResult GetAvailableTimes(int doctorId, DateTime date)
        {
            var shifts = GetDoctorScheduleByDay(doctorId, date);

            if (shifts == null || !shifts.Any())
            {
                return Json(new { error = "El médico no tiene horario asignado ese día." }, JsonRequestBehavior.AllowGet);
            }

            var allTimes = new List<string>();

            foreach (var shift in shifts)
            {
                for (var time = shift.StartTime; time < shift.EndTime; time = time.Add(TimeSpan.FromHours(1)))
                {
                    allTimes.Add(time.ToString(@"hh\:mm"));
                }
            }

            var appointments = appointmentService.ExecuteRead("GET_BY_DOCTOR_AND_DATE", new Appointment
            {
                DoctorId = doctorId,
                Date = date
            });

            var takenTimes = appointments.Select(a => a.Time.ToString(@"hh\:mm")).ToList();

            var available = allTimes.Select(t => new
            {
                Time = t,
                IsAvailable = !takenTimes.Contains(t)
            });

            return Json(available, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Reserve(Appointment appointment)
        {
            var user = Session["user"] as User;

            appointment.PatientId = user.UserId;
            appointment.Status = "pendiente";

            int affectedRows = appointmentService.ExecuteWrite("INSERT", appointment);

            if (affectedRows > 0)
                TempData["Success"] = "¡Cita reservada correctamente!";
            else
                TempData["Error"] = "No se pudo reservar la cita.";

            return RedirectToAction("Pending");
        }

        public ActionResult Confirm(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = FindById(id);

            if (appointment == null || appointment.Status?.ToLower() != "pendiente")
            {
                TempData["Error"] = "Solo se pueden confirmar citas pendientes.";
                return RedirectToAction("Pending");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Confirm")]
        public ActionResult ConfirmConfirmed(int id)
        {
            var appointment = new Appointment { AppointmentId = id };

            int affectedRows = appointmentService.ExecuteWrite("CONFIRM", appointment);

            if (affectedRows == 1)
            {
                TempData["Success"] = "La cita fue confirmada correctamente.";
            }
            else
            {
                TempData["Error"] = "Hubo un error al confirmar la cita.";
            }

            return RedirectToAction("MyAppointments");
        }

        public ActionResult Attend(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = FindById(id);

            if (appointment == null || appointment.Status?.ToLower() != "confirmada")
            {
                TempData["Error"] = "Solo se pueden atender citas confirmadas.";
                return RedirectToAction("MyAppointments");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Attend")]
        public ActionResult AttendConfirmed(int id)
        {
            var appointment = new Appointment { AppointmentId = id };

            int affectedRows = appointmentService.ExecuteWrite("ATTEND", appointment);

            if (affectedRows == 1)
            {
                TempData["Success"] = "La cita fue atendida correctamente.";
            }
            else
            {
                TempData["Error"] = "Hubo un error al atender la cita.";
            }

            return RedirectToAction("Historial");
        }

        public ActionResult Cancel(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = FindById(id);

            var status = appointment?.Status?.ToLower();

            if (appointment == null || status == "cancelada" || status == "atendida")
            {
                TempData["Error"] = "Solo se pueden cancelar citas pendientes o confirmadas.";
                return RedirectToAction("Pending");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Cancel")]
        public ActionResult CancelConfirmed(int id)
        {
            var appointment = new Appointment { AppointmentId = id };

            int affectedRows = appointmentService.ExecuteWrite("CANCEL", appointment);

            if (affectedRows == 1)
            {
                TempData["Success"] = "La cita fue cancelada correctamente.";
            }
            else
            {
                TempData["Error"] = "Hubo un error al cancelar la cita.";
            }

            return RedirectToAction("Historial");
        }

        public ActionResult ExportAllAppointmentsToPDF()
        {
            var list = appointmentService.ExecuteRead("GET_ALL", new Appointment());
            return ExportToPDF(list, "Lista de citas", "administrador");
        }

        public ActionResult ExportMyAppointmentsToPDF()
        {
            var list = GetAppointmentsByStatus("confirmada", "GET_BY_USER_AND_STATUS");
            return ExportToPDF(list, "Mis citas", user.ActiveRole);
        }

        public ActionResult ExportPendingAppointmentsToPDF()
        {
            var list = GetAppointmentsByStatus("pendiente", "GET_BY_USER_AND_STATUS");
            return ExportToPDF(list, "Citas pendientes", user.ActiveRole);
        }

        public ActionResult ExportHistorialAppointmentsToPDF()
        {
            var list = GetAppointmentsByStatus(null, "GET_COMPLETED_OR_CANCELLED_BY_USER");
            return ExportToPDF(list, "Historial de citas", user.ActiveRole);
        }
        public ActionResult ExportAllAppointmentsToExcel()
        {
            var list = appointmentService.ExecuteRead("GET_ALL", new Appointment());
            return ExportToExcel(list, "Lista de citas", "administrador");
        }

        public ActionResult ExportMyAppointmentsToExcel()
        {
            var list = GetAppointmentsByStatus("confirmada", "GET_BY_USER_AND_STATUS");
            return ExportToExcel(list, "Mis citas", user.ActiveRole);
        }

        public ActionResult ExportPendingAppointmentsToExcel()
        {
            var list = GetAppointmentsByStatus("pendiente", "GET_BY_USER_AND_STATUS");
            return ExportToExcel(list, "Citas pendientes", user.ActiveRole);
        }

        public ActionResult ExportHistorialAppointmentsToExcel()
        {
            var list = GetAppointmentsByStatus(null, "GET_COMPLETED_OR_CANCELLED_BY_USER");
            return ExportToExcel(list, "Historial de citas", user.ActiveRole);
        }

        private FileResult ExportToPDF(List<Appointment> appointments, string title, string userRole)
        {
            using (var ms = new MemoryStream())
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

                var headerTable = new PdfPTable(3) { WidthPercentage = 100, SpacingBefore = 5f, SpacingAfter = 10f };
                headerTable.SetWidths(new float[] { 2f, 6f, 2f });

                headerTable.AddCell(new PdfPCell(new Phrase(date, smallFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, PaddingTop = 4, PaddingBottom = 4 });
                headerTable.AddCell(new PdfPCell(new Phrase(title, titleFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, PaddingTop = 4, PaddingBottom = 4 });
                headerTable.AddCell(new PdfPCell(new Phrase(time, smallFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, PaddingTop = 4, PaddingBottom = 4 });

                doc.Add(headerTable);

                bool hideDoctor = userRole == "medico";
                bool hidePatient = userRole == "paciente";

                var headers = new List<string> { "Código", "Especialidad" };
                if (!hideDoctor) headers.Add("Médico");
                if (!hidePatient) headers.Add("Paciente");
                headers.AddRange(new[] { "Tipo consulta", "Fecha cita", "Horario cita", "Estado" });

                var table = new PdfPTable(headers.Count) { WidthPercentage = 100, SpacingBefore = 5f };
                table.SetWidths(Enumerable.Repeat(1f, headers.Count).ToArray());

                var headerColor = new BaseColor(0x0a, 0x76, 0xd8);
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

                foreach (var a in appointments)
                {
                    table.AddCell(new PdfPCell(new Phrase(a.AppointmentId.ToString(), smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(a.SpecialtyName, smallFont)) { Padding = 4 });
                    if (!hideDoctor) table.AddCell(new PdfPCell(new Phrase(a.DoctorName, smallFont)) { Padding = 4 });
                    if (!hidePatient) table.AddCell(new PdfPCell(new Phrase(a.PatientName, smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a.ConsultationType.ToLower()), smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(a.Date.ToString("dd MMM yyyy"), smallFont)) { Padding = 4 });

                    var startTime = DateTime.Today.Add(a.Time);
                    var endTime = startTime.AddHours(1);
                    var timeRange = $"{startTime:hh:mm tt} - {endTime:hh:mm tt}";
                    table.AddCell(new PdfPCell(new Phrase(timeRange, smallFont)) { Padding = 4 });

                    var status = a.Status?.Trim().ToLower();
                    string statusText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(status ?? "Desconocido");
                    BaseColor statusColor;

                    switch (status)
                    {
                        case "confirmada":
                            statusColor = new BaseColor(13, 110, 253);
                            break;
                        case "pendiente":
                            statusColor = new BaseColor(255, 193, 7);
                            break;
                        case "cancelada":
                            statusColor = new BaseColor(220, 53, 69);
                            break;
                        case "atendida":
                            statusColor = new BaseColor(25, 135, 84);
                            break;
                        default:
                            statusColor = BaseColor.DARK_GRAY;
                            break;
                    }

                    var statusFont = FontFactory.GetFont("Arial", 9, Font.BOLD, statusColor);
                    table.AddCell(new PdfPCell(new Phrase(statusText, statusFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_CENTER });
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", $"{title.Replace(" ", "_")}.pdf");
            }
        }

        private FileResult ExportToExcel(List<Appointment> appointments, string title, string userRole)
        {
            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Citas");

                var now = DateTime.Now;
                string date = now.ToString("dd MMM yyyy");
                string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                bool hideDoctor = userRole == "medico";
                bool hidePatient = userRole == "paciente";

                var headers = new List<string> { "Código", "Especialidad" };
                if (!hideDoctor) headers.Add("Médico");
                if (!hidePatient) headers.Add("Paciente");
                headers.AddRange(new[] { "Tipo consulta", "Fecha cita", "Horario cita", "Estado" });

                int totalCols = headers.Count;

                sheet.Cell(1, 1).Value = date;
                sheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                sheet.Cell(1, 1).Style.Font.FontSize = 10;

                if (totalCols >= 3)
                {
                    sheet.Range(sheet.Cell(1, 2), sheet.Cell(1, totalCols - 1)).Merge();
                    sheet.Cell(1, 2).Value = title;
                    sheet.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    sheet.Cell(1, 2).Style.Font.Bold = true;
                    sheet.Cell(1, 2).Style.Font.FontSize = 14;
                }
                else
                {
                    sheet.Cell(1, 2).Value = title;
                    sheet.Cell(1, 2).Style.Font.Bold = true;
                    sheet.Cell(1, 2).Style.Font.FontSize = 14;
                }

                sheet.Cell(1, totalCols).Value = time;
                sheet.Cell(1, totalCols).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cell(1, totalCols).Style.Font.FontSize = 10;

                for (int i = 0; i < headers.Count; i++)
                {
                    var cell = sheet.Cell(3, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0a76d8");
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontSize = 10;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.Black;
                }

                int row = 4;
                foreach (var a in appointments)
                {
                    int col = 1;
                    sheet.Cell(row, col++).Value = a.AppointmentId;
                    sheet.Cell(row, col++).Value = a.SpecialtyName;
                    if (!hideDoctor) sheet.Cell(row, col++).Value = a.DoctorName;
                    if (!hidePatient) sheet.Cell(row, col++).Value = a.PatientName;
                    sheet.Cell(row, col++).Value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a.ConsultationType.ToLower());
                    sheet.Cell(row, col++).Value = a.Date.ToString("dd MMM yyyy");

                    var startTime = DateTime.Today.Add(a.Time);
                    var endTime = startTime.AddHours(1);
                    sheet.Cell(row, col++).Value = $"{startTime:hh:mm tt} - {endTime:hh:mm tt}";

                    var status = a.Status?.Trim().ToLower() ?? "desconocido";
                    var statusCell = sheet.Cell(row, col);
                    statusCell.Value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(status);
                    statusCell.Style.Font.Bold = true;

                    var statusColor = XLColor.Gray;
                    switch (status)
                    {
                        case "confirmada":
                            statusColor = XLColor.FromHtml("#0d6efd");
                            break;
                        case "pendiente":
                            statusColor = XLColor.FromHtml("#ffc107");
                            break;
                        case "cancelada":
                            statusColor = XLColor.FromHtml("#dc3545");
                            break;
                        case "atendida":
                            statusColor = XLColor.FromHtml("#198754");
                            break;
                    }

                    statusCell.Style.Font.FontColor = statusColor;

                    for (int c = 1; c <= headers.Count; c++)
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
                $"{title.Replace(" ", "_")}.xlsx"
            );
        }
    }
}