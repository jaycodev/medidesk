using Microsoft.AspNetCore.Mvc;
using Web.Mappers;
using Web.Models.Patients;
using Web.Models.Users;
using Web.Services.Patient;

namespace Web.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            var patients = await _patientService.GetListAsync();
            var viewModelList = patients.Select(p => p.ToListViewModel()).ToList();
            return View(viewModelList);
        }

        public IActionResult Create() => View(new PatientCreateViewModel());

        [HttpPost]
        public async Task<ActionResult> Create(PatientCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var request = model.ToCreateRequest();
            var (Success, Message) = await _patientService.CreateAsync(request);

            if (Success)
            {
                TempData["Success"] = "¡Paciente creado correctamente!";
                return RedirectToAction("Index");
            }

            ViewBag.Message = Message;
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return RedirectToAction("Index");

            var model = patient.ToEditViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, PatientEditViewModel model)
        {
            if (id != model.UserId) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var toUpdate = model.ToUpdateRequest();
            var (Success, Message) = await _patientService.UpdateAsync(id, toUpdate);

            if (Success)
            {
                TempData["Success"] = "¡Paciente actualizado correctamente!";
                return RedirectToAction("Index");
            }

            ViewBag.Message = Message;
            return View(model);
        }

        public async Task<ActionResult> Details(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return RedirectToAction("Index");

            var model = patient.ToDetailViewModel();
            return View(model);
        }

        public async Task<ActionResult> ExportToPdf()
        {
            var pdfBytes = await _patientService.GeneratePdfAsync();
            return File(pdfBytes, "application/pdf", "Pacientes.pdf");
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var excelBytes = await _patientService.GenerateExcelAsync();
            return File(excelBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Pacientes.xlsx");
        }
    }
}
