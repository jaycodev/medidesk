using System.Globalization;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Web.Models.Patients;

namespace Web.Controllers
{
    public class PatientsController : Controller
    {
        private readonly HttpClient _http;

        public PatientsController(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        private async Task<PatientDetailDTO?> GetByIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/patients/{id}");
            if (resp.IsSuccessStatusCode)
            {
                var patient = await resp.Content.ReadFromJsonAsync<PatientDetailDTO>();
                return patient;
            }

            return null;
        }

        public async Task<IActionResult> Index()
        {
            var patients = new List<PatientListDTO>();
            try
            {
                patients = await _http.GetFromJsonAsync<List<PatientListDTO>>("api/patients") ?? new List<PatientListDTO>();
            }
            catch
            { }

            return View(patients);
        }

        public async Task<ActionResult> Create()
        {
            return View(new CreatePatientDTO());
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreatePatientDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var resp = await _http.PostAsJsonAsync("api/patients", dto);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Paciente creado correctamente!";
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

            return View(dto);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (id == 0)
                return RedirectToAction("Index");

            var patient = await GetByIdAsync(id);
            if (patient == null)
                return RedirectToAction("Index");

            return View(patient);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(PatientDetailDTO dto)
        {
            var toUpdate = new UpdatePatientDTO
            {
                BirthDate = dto.BirthDate,
                BloodType = dto.BloodType
            };

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var resp = await _http.PutAsJsonAsync($"api/patients/{dto.UserId}", toUpdate);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "¡Paciente actualizado correctamente!";
                    return RedirectToAction("Index");
                }

                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Message = "Paciente no encontrado o no se pudo actualizar";
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

            var patient = await GetByIdAsync(id);
            if (patient == null)
                return RedirectToAction("Index");

            return View(patient);
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
            { }

            return content.Length > 300 ? content[..300] + "..." : content;
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var patients = await _http.GetFromJsonAsync<List<PatientListDTO>>("api/patient");

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
                    sheet.Cell(row, 5).Value = p.BirthDate.ToString("dd MMM yyyy") ?? "-";
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
