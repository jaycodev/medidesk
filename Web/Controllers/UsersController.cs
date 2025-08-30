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
using Microsoft.AspNetCore.Mvc;
using Web.Models.Doctors;
using Web.Models.Specialties;
using Web.Models.User;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly HttpClient _http;

        public UsersController(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index(int id)
        {
            if(!int.TryParse(HttpContext.Session.GetString("UserId"), out int loggedUserId))
                return RedirectToAction("Login","Account");


            var users = new List<UserListDTO>();
            try
            {
                users = await _http.GetFromJsonAsync<List<UserListDTO>>($"api/users?id={loggedUserId}") ?? new List<UserListDTO>();
            }
            catch
            { }

            return View(users);
        }

        public IActionResult Create()
        {
            return View(new UserDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _http.PostAsJsonAsync("api/users", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Usuario registrado correctamente!";
                    return RedirectToAction("Index");
                }

                var content = await response.Content.ReadAsStringAsync();
                ViewBag.Message = ExtractErrorMessage(content);
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error al registrar el usuario.";
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetSpecialties()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<SpecialtyDTO>>("api/specialties");
                return Json(response ?? new List<SpecialtyDTO>());
            }
            catch
            {
                return Json(new List<SpecialtyDTO>());
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var user = await _http.GetFromJsonAsync<UserEditViewModel>($"api/users/{id}");
            if (user == null)
                return RedirectToAction("Index");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.Errors = errors;
                return View(model);
            }

            try
            {
                // 1. Primero, actualizas al usuario principal
                var response = await _http.PutAsJsonAsync($"api/users/{id}", model);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ViewBag.Message = ExtractErrorMessage(content);
                    return View(model);
                }

                if (model.SelectedRoleCombo.Contains("medico"))
                {
                    CreateDoctorDTO doctorData = new CreateDoctorDTO
                    {
                        SpecialtyId = (int)model.SpecialtyId,
                        Status = (bool)model.Status,

                    };
                    var doctorResponse = await _http.PutAsJsonAsync($"api/doctors/{id}", doctorData);

                    if (!doctorResponse.IsSuccessStatusCode)
                    {
                        var content = await doctorResponse.Content.ReadAsStringAsync();
                        ViewBag.Message = $"Error al guardar médico: {ExtractErrorMessage(content)}";
                        return View(model);
                    }
                }

                if (model.SelectedRoleCombo.Contains("paciente"))
                {
                    var patientData = new
                    {
                        birthDate = model.BirthDate,
                        bloodType = model.BloodType
                    };

                    var patientResponse = await _http.PutAsJsonAsync($"api/Patient/{id}", patientData);

                    if (!patientResponse.IsSuccessStatusCode)
                    {
                        var content = await patientResponse.Content.ReadAsStringAsync();
                        ViewBag.Message = $"Error al guardar paciente: {ExtractErrorMessage(content)}";
                        return View(model);
                    }
                }

                TempData["Success"] = "¡Usuario actualizado correctamente!";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error al actualizar el usuario.";
                return View(model);
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var user = await _http.GetFromJsonAsync<UserDTO>($"api/users/{id}");
            if (user == null)
                return RedirectToAction("Index");

            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            try
            {
                var response = await _http.DeleteAsync($"api/users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Usuario eliminado correctamente!";
                    return RedirectToAction("Index");
                }

                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = ExtractErrorMessage(content);
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al eliminar el usuario.";
            }

            return RedirectToAction("Index");
        }

        private string ExtractErrorMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "No se pudo procesar la solicitud.";

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


        public async Task<ActionResult> ExportToPDF()
        {
            var users = await _http.GetFromJsonAsync<List<UserListDTO>>("api/users") ?? new List<UserListDTO>();

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

                return File(ms.ToArray(), "application/pdf", "Users.pdf");
            }
        }

        public async Task<ActionResult> ExportToExcel()
        {
            var users = await _http.GetFromJsonAsync<List<UserListDTO>>("api/users") ?? new List<UserListDTO>();

            var stream = new MemoryStream();

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
                    sheet.Cell(row, 5).Value = d.Roles != null && d.Roles.Any() ? string.Join(", ", d.Roles) : "Sin roles";

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

