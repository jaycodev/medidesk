using System.Globalization;
using System.Net;
using ClosedXML.Excel;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Appointments.Requests;
using Shared.DTOs.Appointments.Responses;
using Shared.DTOs.Doctors.Responses;
using Shared.DTOs.Schedules.Responses;

namespace Web.Services.Appointment
{
    public class AppointmentService : IAppointmentService
    {
        private readonly HttpClient _http;

        public AppointmentService(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        public async Task<List<AppointmentListResponse>> GetListAsync(string filter, int? userId = null, string? userRol = null)
        {
            try
            {
                string url = filter.ToLower() switch
                {
                    "all" => "api/appointments",
                    "all-by-user" => $"api/appointments/all-by-user?userId={userId}&userRol={userRol}",
                    "my" => $"api/appointments/my?userId={userId}&userRol={userRol}",
                    "pending" => $"api/appointments/pending?userId={userId}&userRol={userRol}",
                    "historial" => $"api/appointments/historial?userId={userId}&userRol={userRol}",
                    _ => "api/appointments"
                };

                return await _http.GetFromJsonAsync<List<AppointmentListResponse>>(url) ?? new List<AppointmentListResponse>();
            }
            catch
            {
                return new List<AppointmentListResponse>();
            }
        }

        public async Task<AppointmentResponse?> GetByIdAsync(int id)
        {
            try
            {
                var resp = await _http.GetAsync($"api/appointments/{id}");
                if (!resp.IsSuccessStatusCode) return null;
                return await resp.Content.ReadFromJsonAsync<AppointmentResponse>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<AppointmentTimeResponse>> GetByDateAsync(int doctorId, DateTime date)
        {
            try
            {
                var resp = await _http.GetAsync($"api/appointments/by-doctor-and-date?doctorId={doctorId}&date={date:yyyy-MM-dd}");
                if (!resp.IsSuccessStatusCode) return new List<AppointmentTimeResponse>();
                return await resp.Content.ReadFromJsonAsync<List<AppointmentTimeResponse>>() ?? new List<AppointmentTimeResponse>();
            }
            catch
            {
                return new List<AppointmentTimeResponse>();
            }
        }

        public async Task<HttpResponseMessage> ReserveAsync(CreateAppointmentRequest request)
        {
            try
            {
                return await _http.PostAsJsonAsync("api/appointments", request);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = ex.Message
                };
            }
        }

        public async Task<HttpResponseMessage> UpdateStatusAsync(int id, string status)
        {
            try
            {
                var dto = new { Status = status };
                return await _http.PutAsJsonAsync($"api/appointments/{id}", dto);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = ex.Message
                };
            }
        }

        public FileResult GeneratePdf(List<AppointmentListResponse> appointments, string title, string userRole)
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


                return new FileContentResult(ms.ToArray(), "application/pdf")
                {
                    FileDownloadName = $"{title.Replace(" ", "_")}.pdf"
                };
            }
        }

        public FileResult GenerateExcel(List<AppointmentListResponse> appointments, string title, string userRole)
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
            return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"{title.Replace(" ", "_")}.xlsx"
            };
        }
    }
}
