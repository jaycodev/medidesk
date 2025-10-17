using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Mappers;
using Web.Models.Doctors;
using Web.Services.Doctor;
using Web.Services.Specialty;

namespace Web.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly ISpecialtyService _specialtyService;

        public DoctorsController(IDoctorService doctorService, ISpecialtyService specialtyService)
        {
            _doctorService = doctorService;
            _specialtyService = specialtyService;
        }

        private async Task LoadSpecialtiesAsync(int? selectedId = null)
        {
            ViewBag.Specialties = await _specialtyService.GetSelectListAsync(selectedId);
        }

        private void LoadStatusList(object? selected = null)
        {
            ViewBag.StatusList = new SelectList(new[]
            {
                new { Value = true, Text = "Activo" },
                new { Value = false, Text = "Inactivo" }
            }, "Value", "Text", selected);
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorService.GetListAsync();
            var viewModelList = doctors.Select(d => d.ToListViewModel()).ToList();
            return View(viewModelList);
        }

        public async Task<ActionResult> Create()
        {
            await LoadSpecialtiesAsync();
            LoadStatusList();
            return View(new DoctorCreateViewModel());
        }


        [HttpPost]
        public async Task<ActionResult> Create(DoctorCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadSpecialtiesAsync(model.SpecialtyId);
                LoadStatusList(model.Status);
                return View(model);
            }

            var request = model.ToCreateRequest();
            var (Success, Message) = await _doctorService.CreateAsync(request);

            if (Success)
            {
                TempData["Success"] = "¡Médico creado correctamente!";
                return RedirectToAction("Index");
            }

            ViewBag.Message = Message;
            await LoadSpecialtiesAsync(model.SpecialtyId);
            LoadStatusList(model.Status);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0) return RedirectToAction("Index");

            try
            {
                var doctor = await _doctorService.GetByIdAsync(id);
                if (doctor == null) return RedirectToAction("Index");

                var model = doctor.ToEditViewModel();

                await LoadSpecialtiesAsync(model.SpecialtyId);
                LoadStatusList(model.Status);

                return View(model);
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error al cargar el médico.";
                await LoadSpecialtiesAsync(null);
                LoadStatusList();
                return View(new DoctorEditViewModel { UserId = id });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, DoctorEditViewModel model)
        {
            if (id != model.UserId) return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadSpecialtiesAsync(model.SpecialtyId);
                LoadStatusList(model.Status);
                return View(model);
            }

            LoadStatusList(model.Status);

            var toUpdate = model.ToUpdateRequest();
            var (Success, Message) = await _doctorService.UpdateAsync(id, toUpdate);

            if (Success)
            {
                TempData["Success"] = "¡Médico actualizado correctamente!";
                return RedirectToAction("Index");
            }

            ViewBag.Message = Message;
            await LoadSpecialtiesAsync(model.SpecialtyId);
            return View(model);
        }

        public async Task<ActionResult> Details(int id)
        {
            if (id == 0) return RedirectToAction("Index");

            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null) return RedirectToAction("Index");

            var model = doctor.ToDetailViewModel();
            return View(model);
        }

        public async Task<ActionResult> ExportToPdf()
        {
            var pdfBytes = await _doctorService.GeneratePdfAsync();
            return File(pdfBytes, "application/pdf", "Medicos.pdf");
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var excelBytes = await _doctorService.GenerateExcelAsync();
            return File(excelBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Medicos.xlsx");
        }
    }
}
