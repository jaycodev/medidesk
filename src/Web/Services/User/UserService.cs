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
using Shared.DTOs.Users.Requests;
using Shared.DTOs.Users.Responses;
using Web.Helpers;

namespace Web.Services.User
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpClientFactory httpFactory, IHttpContextAccessor httpContextAccessor)
        {
            _http = httpFactory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserResponse?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<UserResponse>($"api/users/{id}");
        }

        public async Task<List<UserListResponse>> GetListAsync(int? loggedUserId = null)
        {
            if (loggedUserId == null)
            {
                if (int.TryParse(_httpContextAccessor.HttpContext?.Session.GetString("UserId"), out var id))
                    loggedUserId = id;
            }

            return await _http.GetFromJsonAsync<List<UserListResponse>>($"api/users?id={loggedUserId}") ?? new();
        }

        public async Task<(bool Success, string Message)> CreateAsync(CreateUserRequest request)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync("api/users", request);
                if (resp.IsSuccessStatusCode) return (true, "Usuario creado correctamente");

                var content = await resp.Content.ReadAsStringAsync();
                return (false, HttpHelper.ExtractErrorMessage(content));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(int id, UpdateUserRequest request)
        {
            try
            {
                var resp = await _http.PutAsJsonAsync($"api/users/{id}", request);
                if (resp.IsSuccessStatusCode) return (true, "Usuario actualizado correctamente");

                var content = await resp.Content.ReadAsStringAsync();
                return (false, await HttpHelper.ExtractErrorMessageAsync(resp));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var resp = await _http.DeleteAsync($"api/users/{id}");
                if (resp.IsSuccessStatusCode) return (true, "Usuario eliminado correctamente");

                var content = await resp.Content.ReadAsStringAsync();
                return (false, HttpHelper.ExtractErrorMessage(content));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<byte[]> GeneratePdfAsync()
        {
            var users = await GetListAsync();

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
                    .Add(new Paragraph("Lista de usuarios").SetFont(boldFont).SetFontSize(titleFontSize))
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
                string[] headers = { "Código", "Nombre(s)", "Apellido(s)", "Correo electrónico", "Telefono", "Rol(es)" };

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

                foreach (var d in users)
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
                        .Add(new Paragraph(d.Phone ?? string.Empty).SetFont(regularFont).SetFontSize(smallFontSize))
                        .SetPadding(4));

                    table.AddCell(new Cell()
                         .Add(new Paragraph(d.Roles != null && d.Roles.Any() ? string.Join(", ", d.Roles) : "Sin roles").SetFont(regularFont).SetFontSize(smallFontSize))
                         .SetPadding(4));

                }

                document.Add(table);
                document.Close();

                return ms.ToArray();
            }
        }

        public async Task<byte[]> GenerateExcelAsync()
        {
            var users = await GetListAsync();

            using (var ms = new MemoryStream())
            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Users");

                var now = DateTime.Now;
                string date = now.ToString("dd MMM yyyy");
                string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                sheet.Cell(1, 1).Value = date;
                sheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                sheet.Cell(1, 1).Style.Font.FontSize = 10;

                sheet.Range("B1:E1").Merge();
                sheet.Cell(1, 2).Value = "Lista de Usuarios";
                sheet.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cell(1, 2).Style.Font.Bold = true;
                sheet.Cell(1, 2).Style.Font.FontSize = 14;

                sheet.Cell(1, 6).Value = time;
                sheet.Cell(1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cell(1, 6).Style.Font.FontSize = 10;

                string[] headers = { "Código", "Nombre(s)", "Apellido(s)", "Correo electrónico", "Telefono", "Rol(es)" };
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
                foreach (var d in users)
                {
                    sheet.Cell(row, 1).Value = d.UserId;
                    sheet.Cell(row, 2).Value = d.FirstName;
                    sheet.Cell(row, 3).Value = d.LastName;
                    sheet.Cell(row, 4).Value = d.Email;
                    sheet.Cell(row, 5).Value = d.Phone;
                    sheet.Cell(row, 6).Value = d.Roles != null && d.Roles.Any() ? string.Join(", ", d.Roles) : "Sin roles";

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
