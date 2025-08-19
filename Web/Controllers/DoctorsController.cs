using System.Globalization;
using ClosedXML.Excel;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using medical_appointment_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Models.Doctors;
using iText.Layout.Borders;

namespace Web.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly HttpClient _http;

        public DoctorsController(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        private async Task<DoctorDetailDTO?> GetByIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/doctors/{id}");
            if (resp.IsSuccessStatusCode)
            {
                var doc = await resp.Content.ReadFromJsonAsync<DoctorDetailDTO>();
                return doc;
            }

            return null;
        }

        private async Task LoadSpecialtiesAsync(int? selectedId = null)
        {
            try
            {
                var specialties = await _http.GetFromJsonAsync<List<Specialty>>("api/specialties");
                ViewBag.Specialties = new SelectList(specialties ?? new List<Specialty>(), "SpecialtyId", "Name", selectedId);
            }
            catch
            {
                ViewBag.Specialties = new SelectList(new List<Specialty>(), "SpecialtyId", "Name", selectedId);
            }
        }

        public async Task<IActionResult> Index()
        {
            var doctors = new List<DoctorListDTO>();
            try
            {
                doctors = await _http.GetFromJsonAsync<List<DoctorListDTO>>("api/doctors") ?? new List<DoctorListDTO>();
            }
            catch
            {
            }

            return View(doctors);
        }

        public async Task<ActionResult> Create()
        {
            await LoadSpecialtiesAsync();
            return View(new CreateDoctorDTO());
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateDoctorDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadSpecialtiesAsync(dto.SpecialtyId);
                return View(dto);
            }

            try
            {
                var resp = await _http.PostAsJsonAsync("api/doctors", dto);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Médico creado correctamente!";
                    return RedirectToAction("Index");
                }

                var content = await resp.Content.ReadAsStringAsync();
                ViewBag.Message = ExtractErrorMessage(content);
            }
            catch (ApplicationException ex)
            {
                ViewBag.Message = ex.Message;
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            await LoadSpecialtiesAsync(dto.SpecialtyId);
            return View(dto);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (id == 0)
                return RedirectToAction("Index");

            var doctor = await GetByIdAsync(id);
            if (doctor == null)
                return RedirectToAction("Index");

            await LoadSpecialtiesAsync(doctor.SpecialtyId);

            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = true, Text = "Activo" },
                new { Value = false, Text = "Inactivo" }
            }, "Value", "Text", doctor.Status);

            return View(doctor);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(DoctorDetailDTO dto)
        {
            await LoadSpecialtiesAsync(dto.SpecialtyId);

            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = true, Text = "Activo" },
                new { Value = false, Text = "Inactivo" }
            }, "Value", "Text", dto.Status);

            var toUpdate = new UpdateDoctorDTO();
            toUpdate.SpecialtyId = dto.SpecialtyId;
            toUpdate.Status = dto.Status;

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var resp = await _http.PutAsJsonAsync($"api/doctors/{dto.UserId}", toUpdate);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Médico actualizado correctamente!";
                    return RedirectToAction("Index");
                }

                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Message = "Médico no encontrado o no se pudo actualizar";
                }
                else
                {
                    var content = await resp.Content.ReadAsStringAsync();
                    ViewBag.Message = ExtractErrorMessage(content);
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(dto);
        }

        public async Task<ActionResult> Details(int id)
        {
            if (id == 0)
                return RedirectToAction("Index");

            var doc = await GetByIdAsync(id);
            if (doc == null)
                return RedirectToAction("Index");

            return View(doc);
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
            catch
            {

            }

            return content.Length > 300 ? content[..300] + "..." : content;
        }

        public async Task<ActionResult> ExportToPDF()
        {
            var doctors = await _http.GetFromJsonAsync<List<DoctorListDTO>>("api/doctors") ?? new List<DoctorListDTO>();

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
                    .Add(new Paragraph("Lista de médicos").SetFont(boldFont).SetFontSize(titleFontSize))
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

                float[] columnWidths = { 1.5f, 2f, 2f, 3f, 2f, 2f };
                var table = new Table(UnitValue.CreatePercentArray(columnWidths))
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .SetMarginTop(5);

                var headerColor = new DeviceRgb(0x0a, 0x76, 0xd8);
                string[] headers = { "Código", "Nombre(s)", "Apellido(s)", "Correo electrónico", "Especialidad", "Estado" };

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

                foreach (var d in doctors)
                {
                    table.AddCell(new Cell()
                        .Add(new Paragraph(d.UserId.ToString()).SetFont(regularFont).SetFontSize(smallFontSize))
                        .SetPadding(4));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(d.FirstName ?? string.Empty).SetFont(regularFont).SetFontSize(smallFontSize))
                        .SetPadding(4));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(d.LastName ?? string.Empty).SetFont(regularFont).SetFontSize(smallFontSize))
                        .SetPadding(4));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(d.Email ?? string.Empty).SetFont(regularFont).SetFontSize(smallFontSize))
                        .SetPadding(4));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(d.SpecialtyName ?? string.Empty).SetFont(regularFont).SetFontSize(smallFontSize))
                        .SetPadding(4));

                    string statusText = d.Status ? "Activo" : "Inactivo";
                    var statusRgb = d.Status ? new DeviceRgb(40, 167, 69) : new DeviceRgb(220, 53, 69);

                    var statusParagraph = new Paragraph(statusText)
                        .SetFont(boldFont)
                        .SetFontSize(smallFontSize)
                        .SetFontColor(statusRgb);

                    table.AddCell(new Cell()
                        .Add(statusParagraph)
                        .SetPadding(4)
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                document.Add(table);

                document.Close();

                return File(ms.ToArray(), "application/pdf", "Medicos.pdf");
            }
        }

        public async Task<ActionResult> ExportToExcel()
        {
            var doctors = await _http.GetFromJsonAsync<List<DoctorListDTO>>("api/doctors") ?? new List<DoctorListDTO>();

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
