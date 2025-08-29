using ClosedXML.Excel;
using medical_appointment_system.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Web.Models.Patients;

namespace Web.Controllers
{
    public class PatientsController : Controller
    {
        private readonly HttpClient _http;

        public PatientsController(IHttpClientFactory http) 
        {
            _http = http.CreateClient("ApiClient");
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var patients = await _http.GetFromJsonAsync<List<Patient>>("api/patient");
            return View(patients);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View(new PatientCreateDTO());
        }

        // POST: Patients/Create
        [HttpPost]
        public async Task<IActionResult> Create(PatientCreateDTO patient)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor complete los campos obligatorios.";
                return View(patient);
            }

            var response = await _http.PostAsJsonAsync("api/patient", patient);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "¡Paciente creado correctamente!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = "No se pudo crear el paciente. Intenta nuevamente.";
            return View(patient);
        }

        // GET: Patients/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _http.GetFromJsonAsync<Patient>($"api/patient/{id}");
            if (patient == null) return RedirectToAction(nameof(Index));

            var patienteUpdate=new PatientUpdateDTO
            {
                UserId = patient.UserId,
                BirthDate = patient.BirthDate,
                BloodType = patient.BloodType
            };

            return View(patienteUpdate);
        }

        // PUT: Patients/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(PatientUpdateDTO patient)
        {
            var response = await _http.PutAsJsonAsync($"api/patient", patient);

          

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "¡Paciente actualizado correctamente!";
                return RedirectToAction("Index");
            }

            ViewBag.Message = "No se pudo actualizar el paciente. Intenta nuevamente.";
            return View(patient);
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _http.GetFromJsonAsync<Patient>($"api/patient/{id}");
            if (patient == null) return RedirectToAction(nameof(Index));
            return View(patient);
        }

        // GET: Patients/ExportToExcel
        public async Task<IActionResult> ExportToExcel()
        {
            var patients = await _http.GetFromJsonAsync<List<Patient>>("api/patient");

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
