using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text.Json;
using Web.Models.Appointments;
using Web.Models.Doctors;
using Web.Models.Specialties;
using Web.Models.User;
using ClosedXML.Excel;


namespace Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentsController(IHttpClientFactory httpFactory, IHttpContextAccessor httpContextAccessor)
        {
            _http = httpFactory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        private LoggedUserDTO? GetLoggedUser()
        {
            var userJson = _httpContextAccessor.HttpContext!.Session.GetString("LoggedUser");
            if (string.IsNullOrEmpty(userJson)) return null;

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<LoggedUserDTO>(userJson, options);
        }

        private async Task<AppointmentDetailDTO?> GetByIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/appointments/{id}");
            if (resp.IsSuccessStatusCode)
                return await resp.Content.ReadFromJsonAsync<AppointmentDetailDTO>();
            return null;
        }

        private async Task LoadSpecialtiesAsync(int? selectedId = null)
        {
            try
            {
                var specs = await _http.GetFromJsonAsync<List<SpecialtyDTO>>("api/specialties") ?? new();
                ViewBag.Specialties = new SelectList(
                    specs.Select(s => new { Id = s.SpecialtyId, Name = s.Name }),
                    "Id", "Name", selectedId
                );
            }
            catch
            {
                ViewBag.Specialties = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        public async Task<IActionResult> Home()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                    $"api/appointments/all-by-user?userId={user.UserId}&userRol={user.ActiveRole}")
                    ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> AllAppointments()
        {
            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>("api/appointments")
                       ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> MyAppointments()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                           $"api/appointments/my?userId={user.UserId}&userRol={user.ActiveRole}")
                       ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> Pending()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                           $"api/appointments/pending?userId={user.UserId}&userRol={user.ActiveRole}")
                       ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> Historial()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = new List<AppointmentListDTO>();
            try
            {
                list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                           $"api/appointments/historial?userId={user.UserId}&userRol={user.ActiveRole}")
                       ?? new List<AppointmentListDTO>();
            }
            catch { }

            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return RedirectToAction(nameof(Index));

            var item = await GetByIdAsync(id);
            if (item == null) return RedirectToAction(nameof(Index));

            return View(item);
        }

        public async Task<IActionResult> Reserve()
        {
            await LoadSpecialtiesAsync();
            return View(new CreateAppointmentDTO());
        }

        public async Task<JsonResult> GetDoctorsBySpecialty(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/doctors/by-specialty?specialtyId={id}&userId=3");
                if (!response.IsSuccessStatusCode)
                    return Json(new { error = "No se pudieron obtener los médicos" });

                var doctors = await response.Content.ReadFromJsonAsync<List<DoctorBySpecialtyDTO>>()
                                ?? new List<DoctorBySpecialtyDTO>();

                return Json(doctors);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private async Task<List<ScheduleDTO>?> GetDoctorScheduleByDayAsync(int doctorId, DateTime date)
        {
            try
            {
                var response = await _http.GetAsync(
                    $"api/appointments/schedule-by-doctor-and-day?doctorId={doctorId}&date={date.ToString("yyyy-MM-dd")}");

                if (!response.IsSuccessStatusCode) return null;

                var schedule = await response.Content.ReadFromJsonAsync<List<ScheduleDTO>>();
                return schedule;
            }
            catch
            {
                return null;
            }
        }

        public async Task<JsonResult> GetAvailableTimes(int doctorId, DateTime date)
        {
            if (doctorId <= 0)
                return Json(new { error = "Se requiere un doctorId válido." });

            var shifts = await GetDoctorScheduleByDayAsync(doctorId, date);
            if (shifts == null || !shifts.Any())
                return Json(new { error = "El médico no tiene horario asignado ese día." });

            var allTimes = new List<string>();
            foreach (var shift in shifts)
            {
                for (var time = shift.StartTime; time < shift.EndTime; time = time.Add(TimeSpan.FromHours(1)))
                {
                    allTimes.Add(time.ToString(@"hh\:mm"));
                }
            }

            var appointmentsResponse = await _http.GetAsync(
                $"api/appointments/by-doctor-and-date?doctorId={doctorId}&date={date:yyyy-MM-dd}");

            var appointments = appointmentsResponse.IsSuccessStatusCode
                ? await appointmentsResponse.Content.ReadFromJsonAsync<List<AppointmentTimeDTO>>()
                : new List<AppointmentTimeDTO>();

            var takenTimes = appointments?.Select(a => a.Time.ToString(@"hh\:mm")).ToList() ?? new List<string>();

            var available = allTimes.Select(t => new
            {
                Time = t,
                IsAvailable = !takenTimes.Contains(t)
            }).ToList();

            return Json(available);
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(CreateAppointmentDTO dto)
        {
            var user = GetLoggedUser();
            if (user == null || user.ActiveRole?.ToLower() != "paciente")
            {
                TempData["Error"] = "Solo un paciente puede reservar citas.";
                return RedirectToAction("Home");
            }

            dto.PatientId = user.UserId;

            var resp = await _http.PostAsJsonAsync("api/appointments", dto);

            if (resp.IsSuccessStatusCode)
                TempData["Success"] = "¡Cita reservada correctamente!";
            else
                TempData["Error"] = "No se pudo reservar la cita.";

            return RedirectToAction("Pending");
        }

        public async Task<IActionResult> Confirm(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = await GetByIdAsync(id);
            if (appointment == null || appointment.Status?.ToLower() != "pendiente")
            {
                TempData["Error"] = "Solo se pueden confirmar citas pendientes.";
                return RedirectToAction("Pending");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmConfirmed(int id)
        {
            var dto = new { Status = "confirmada" };

            var resp = await _http.PutAsJsonAsync($"api/appointments/{id}", dto);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Success"] = "La cita fue confirmada correctamente.";
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Error"] = "Cita no encontrada.";
            }
            else
            {
                var content = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = ExtractErrorMessage(content);
            }

            return RedirectToAction("MyAppointments");
        }

        public async Task<IActionResult> Attend(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = await GetByIdAsync(id);
            if (appointment == null || appointment.Status?.ToLower() != "confirmada")
            {
                TempData["Error"] = "Solo se pueden atender citas confirmadas.";
                return RedirectToAction("MyAppointments");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Attend")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AttendConfirmed(int id)
        {
            var dto = new { Status = "atendida" };

            var resp = await _http.PutAsJsonAsync($"api/appointments/{id}", dto);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Success"] = "La cita fue atendida correctamente.";
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Error"] = "Cita no encontrada.";
            }
            else
            {
                var content = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = ExtractErrorMessage(content);
            }

            return RedirectToAction("Historial");
        }

        public async Task<IActionResult> Cancel(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = await GetByIdAsync(id);
            var status = appointment?.Status?.ToLower();

            if (appointment == null || status == "cancelada" || status == "atendida")
            {
                TempData["Error"] = "Solo se pueden cancelar citas pendientes o confirmadas.";
                return RedirectToAction("Pending");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var dto = new { Status = "cancelada" };

            var resp = await _http.PutAsJsonAsync($"api/appointments/{id}", dto);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Success"] = "La cita fue cancelada correctamente.";
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Error"] = "Cita no encontrada.";
            }
            else
            {
                var content = await resp.Content.ReadAsStringAsync();
                TempData["Error"] = ExtractErrorMessage(content);
            }

            return RedirectToAction("Historial");
        }

        public async Task<IActionResult> ExportAllAppointmentsToPDF()
        {
            var list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>("api/appointments") ?? new List<AppointmentListDTO>();
            var user = GetLoggedUser();
            var role = user?.ActiveRole ?? "administrador";
            return ExportAppointmentsToPdf(list, "Lista de citas", role);
        }

        public async Task<IActionResult> ExportMyAppointmentsToPDF()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                $"api/appointments/my?userId={user.UserId}&userRol={user.ActiveRole}")
                ?? new List<AppointmentListDTO>();

            return ExportAppointmentsToPdf(list, "Mis citas", user.ActiveRole);
        }

        public async Task<IActionResult> ExportPendingAppointmentsToPDF()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                $"api/appointments/pending?userId={user.UserId}&userRol={user.ActiveRole}")
                ?? new List<AppointmentListDTO>();

            return ExportAppointmentsToPdf(list, "Citas pendientes", user.ActiveRole);
        }

        public async Task<IActionResult> ExportHistorialAppointmentsToPDF()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                $"api/appointments/historial?userId={user.UserId}&userRol={user.ActiveRole}")
                ?? new List<AppointmentListDTO>();

            return ExportAppointmentsToPdf(list, "Historial de citas", user.ActiveRole);
        }

        public async Task<IActionResult> ExportAllAppointmentsToExcel()
        {
            var list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>("api/appointments")
                       ?? new List<AppointmentListDTO>();
            var user = GetLoggedUser();
            var role = user?.ActiveRole ?? "administrador";
            return ExportAppointmentsToExcel(list, "Lista de citas", role);
        }

        public async Task<IActionResult> ExportMyAppointmentsToExcel()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                $"api/appointments/my?userId={user.UserId}&userRol={user.ActiveRole}")
                ?? new List<AppointmentListDTO>();

            return ExportAppointmentsToExcel(list, "Mis citas", user.ActiveRole);
        }

        public async Task<IActionResult> ExportPendingAppointmentsToExcel()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                $"api/appointments/pending?userId={user.UserId}&userRol={user.ActiveRole}")
                ?? new List<AppointmentListDTO>();

            return ExportAppointmentsToExcel(list, "Citas pendientes", user.ActiveRole);
        }

        public async Task<IActionResult> ExportHistorialAppointmentsToExcel()
        {
            var user = GetLoggedUser();
            if (user == null) return RedirectToAction("Login", "Auth");

            var list = await _http.GetFromJsonAsync<List<AppointmentListDTO>>(
                $"api/appointments/historial?userId={user.UserId}&userRol={user.ActiveRole}")
                ?? new List<AppointmentListDTO>();

            return ExportAppointmentsToExcel(list, "Historial de citas", user.ActiveRole);
        }

        private FileResult ExportAppointmentsToPdf(List<AppointmentListDTO> appointments, string title, string userRole)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf, PageSize.A4);
                document.SetMargins(36, 36, 36, 36);

                PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                const float smallFontSize = 9f;
                const float headerFontSize = 10f;
                const float titleFontSize = 14f;

                var now = DateTime.Now;
                string date = now.ToString("dd MMM yyyy");
                string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                var headerTable = new Table(new float[] { 2f, 6f, 2f })
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .SetMarginTop(5)
                    .SetMarginBottom(10);

                var dateCell = new Cell()
                    .Add(new Paragraph(date).SetFont(regularFont).SetFontSize(smallFontSize))
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetPaddingTop(4)
                    .SetPaddingBottom(4);

                var titleCell = new Cell()
                    .Add(new Paragraph(title).SetFont(boldFont).SetFontSize(titleFontSize))
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPaddingTop(4)
                    .SetPaddingBottom(4);

                var timeCell = new Cell()
                    .Add(new Paragraph(time).SetFont(regularFont).SetFontSize(smallFontSize))
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetPaddingTop(4)
                    .SetPaddingBottom(4);

                headerTable.AddCell(dateCell);
                headerTable.AddCell(titleCell);
                headerTable.AddCell(timeCell);

                document.Add(headerTable);

                bool hideDoctor = string.Equals(userRole, "medico", StringComparison.OrdinalIgnoreCase);
                bool hidePatient = string.Equals(userRole, "paciente", StringComparison.OrdinalIgnoreCase);

                var headers = new List<string> { "Código", "Especialidad" };
                if (!hideDoctor) headers.Add("Médico");
                if (!hidePatient) headers.Add("Paciente");
                headers.AddRange(new[] { "Tipo consulta", "Fecha cita", "Horario cita", "Estado" });

                float[] columnWidths = Enumerable.Repeat(1f, headers.Count).ToArray();
                var table = new Table(UnitValue.CreatePercentArray(columnWidths))
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .SetMarginTop(5);

                var headerColor = new DeviceRgb(0x0a, 0x76, 0xd8);

                foreach (var h in headers)
                {
                    var p = new Paragraph(h)
                        .SetFont(boldFont)
                        .SetFontSize(headerFontSize)
                        .SetFontColor(ColorConstants.WHITE);

                    var cell = new Cell()
                        .Add(p)
                        .SetBackgroundColor(headerColor)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(5);

                    table.AddHeaderCell(cell);
                }

                foreach (var a in appointments)
                {
                    table.AddCell(new Cell().Add(new Paragraph(a.AppointmentId.ToString()).SetFont(regularFont).SetFontSize(smallFontSize)).SetPadding(4));

                    table.AddCell(new Cell().Add(new Paragraph(a.SpecialtyName ?? string.Empty).SetFont(regularFont).SetFontSize(smallFontSize)).SetPadding(4));

                    if (!hideDoctor)
                        table.AddCell(new Cell().Add(new Paragraph(a.DoctorName ?? string.Empty).SetFont(regularFont).SetFontSize(smallFontSize)).SetPadding(4));

                    if (!hidePatient)
                        table.AddCell(new Cell().Add(new Paragraph(a.PatientName ?? string.Empty).SetFont(regularFont).SetFontSize(smallFontSize)).SetPadding(4));

                    var consultationType = (a.ConsultationType ?? string.Empty).ToLower();
                    var consultationDisplay = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(consultationType);
                    table.AddCell(new Cell().Add(new Paragraph(consultationDisplay).SetFont(regularFont).SetFontSize(smallFontSize)).SetPadding(4));

                    var fecha = a.Date.ToString("dd MMM yyyy");
                    table.AddCell(new Cell().Add(new Paragraph(fecha).SetFont(regularFont).SetFontSize(smallFontSize)).SetPadding(4));

                    DateTime startTime;
                    try
                    {
                        startTime = DateTime.Today.Add(a.Time);
                    }
                    catch
                    {
                        if (TimeSpan.TryParse(a.Time.ToString() ?? "00:00", out var ts))
                            startTime = DateTime.Today.Add(ts);
                        else
                            startTime = DateTime.Today;
                    }
                    var endTime = startTime.AddHours(1);
                    var timeRange = $"{startTime:hh:mm tt} - {endTime:hh:mm tt}";
                    table.AddCell(new Cell().Add(new Paragraph(timeRange).SetFont(regularFont).SetFontSize(smallFontSize)).SetPadding(4));

                    var status = (a.Status ?? "Desconocido").Trim().ToLower();
                    string statusText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(status);
                    DeviceRgb statusRgb;
                    switch (status)
                    {
                        case "confirmada":
                            statusRgb = new DeviceRgb(13, 110, 253);
                            break;
                        case "pendiente":
                            statusRgb = new DeviceRgb(255, 193, 7);
                            break;
                        case "cancelada":
                            statusRgb = new DeviceRgb(220, 53, 69);
                            break;
                        case "atendida":
                            statusRgb = new DeviceRgb(25, 135, 84);
                            break;
                        default:
                            statusRgb = (DeviceRgb)ColorConstants.DARK_GRAY;
                            break;
                    }

                    var statusPara = new Paragraph(statusText).SetFont(boldFont).SetFontSize(smallFontSize).SetFontColor(statusRgb);
                    table.AddCell(new Cell().Add(statusPara).SetPadding(4).SetTextAlignment(TextAlignment.CENTER));
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", $"{title.Replace(" ", "_")}.pdf");
            }
        }

        private FileResult ExportAppointmentsToExcel(List<AppointmentListDTO> appointments, string title, string userRole)
        {
            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Citas");

                var now = DateTime.Now;
                string date = now.ToString("dd MMM yyyy");
                string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                bool hideDoctor = string.Equals(userRole, "medico", StringComparison.OrdinalIgnoreCase);
                bool hidePatient = string.Equals(userRole, "paciente", StringComparison.OrdinalIgnoreCase);

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
                    sheet.Cell(row, col++).Value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a.ConsultationType?.ToLower() ?? string.Empty);
                    sheet.Cell(row, col++).Value = a.Date.ToString("dd MMM yyyy");

                    DateTime startTime;
                    try
                    {
                        startTime = DateTime.Today.Add(a.Time);
                    }
                    catch
                    {
                        if (TimeSpan.TryParse(a.Time.ToString() ?? "00:00", out var ts))
                            startTime = DateTime.Today.Add(ts);
                        else
                            startTime = DateTime.Today;
                    }
                    var endTime = startTime.AddHours(1);
                    sheet.Cell(row, col++).Value = $"{startTime:hh:mm tt} - {endTime:hh:mm tt}";

                    var status = a.Status?.Trim().ToLower() ?? "desconocido";
                    var statusCell = sheet.Cell(row, col);
                    statusCell.Value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(status);
                    statusCell.Style.Font.Bold = true;

                    var statusColor = XLColor.Gray;
                    switch (status)
                    {
                        case "confirmada": statusColor = XLColor.FromHtml("#0d6efd"); break;
                        case "pendiente": statusColor = XLColor.FromHtml("#ffc107"); break;
                        case "cancelada": statusColor = XLColor.FromHtml("#dc3545"); break;
                        case "atendida": statusColor = XLColor.FromHtml("#198754"); break;
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

        private string ExtractErrorMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "No se pudo procesar la petición.";

            try
            {
                if (content.Contains("\"message\""))
                {
                    var start = content.IndexOf("\"message\"", StringComparison.OrdinalIgnoreCase);
                    var colon = content.IndexOf(':', start);
                    var trimmed = content.Substring(colon + 1).Trim().Trim('"', ' ', '}');
                    return trimmed;
                }
                if (content.Contains("\"error\""))
                {
                    var start = content.IndexOf("\"error\"", StringComparison.OrdinalIgnoreCase);
                    var colon = content.IndexOf(':', start);
                    var trimmed = content.Substring(colon + 1).Trim().Trim('"', ' ', '}');
                    return trimmed;
                }
            }
            catch { }

            return content.Length > 300 ? content[..300] + "..." : content;
        }
    }
}
