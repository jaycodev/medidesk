using System.Globalization;
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
using Shared.DTOs.Doctors.Requests;
using Shared.DTOs.Doctors.Responses;
using Web.Helpers;

namespace Web.Services.Doctor
{
    public class DoctorService : IDoctorService
    {
        private readonly HttpClient _http;

        public DoctorService(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        public async Task<List<DoctorListResponse>> GetListAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<DoctorListResponse>>("api/doctors") ?? new List<DoctorListResponse>();
            }
            catch
            {
                return new List<DoctorListResponse>();
            }
        }

        public async Task<DoctorResponse?> GetByIdAsync(int id)
        {
            try
            {
                var resp = await _http.GetAsync($"api/doctors/{id}");
                if (!resp.IsSuccessStatusCode) return null;
                return await resp.Content.ReadFromJsonAsync<DoctorResponse>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<DoctorBySpecialtyResponse>> GetBySpecialtyAsync(int specialtyId, int userId)
        {
            try
            {
                var resp = await _http.GetAsync($"api/doctors/by-specialty?specialtyId={specialtyId}&userId={userId}");
                if (!resp.IsSuccessStatusCode) return new List<DoctorBySpecialtyResponse>();
                return await resp.Content.ReadFromJsonAsync<List<DoctorBySpecialtyResponse>>()
                       ?? new List<DoctorBySpecialtyResponse>();
            }
            catch
            {
                return new List<DoctorBySpecialtyResponse>();
            }
        }
        public async Task<(bool Success, string Message)> CreateAsync(CreateDoctorRequest request)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync("api/doctors", request);
                if (resp.IsSuccessStatusCode) return (true, "Médico creado correctamente");

                var content = await resp.Content.ReadAsStringAsync();
                return (false, HttpHelper.ExtractErrorMessage(content));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int id, UpdateDoctorRequest request)
        {
            try
            {
                var resp = await _http.PutAsJsonAsync($"api/doctors/{id}", request);
                if (resp.IsSuccessStatusCode) return (true, "Médico actualizado correctamente");

                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return (false, "Médico no encontrado o no se pudo actualizar");

                return (false, await HttpHelper.ExtractErrorMessageAsync(resp));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<byte[]> GeneratePdfAsync()
        {
            var doctors = await GetListAsync();

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

                return ms.ToArray();
            }
        }

        public async Task<byte[]> GenerateExcelAsync()
        {
            var doctors = await GetListAsync();

            using (var ms = new MemoryStream())
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
                workbook.SaveAs(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }
    }
}
