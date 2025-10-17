using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Specialties.Requests;
using Web.Mappers;
using Web.Models.Specialties;
using Web.Services.Specialty;

namespace Web.Controllers
{
    public class SpecialtiesController : Controller
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        public async Task<IActionResult> Index()
        {
            var specialties = await _specialtyService.GetListAsync();
            var viewModelList = specialties.Select(s => s.ToViewModel()).ToList();
            return View(viewModelList);
        }

        public IActionResult Create() => View(new SpecialtyCreateViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(SpecialtyCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _specialtyService.CreateAsync(model.ToRequest());
            if (success)
            {
                TempData["Success"] = "¡Especialidad creada correctamente!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0) return RedirectToAction("Index");

            try
            {
                var specialty = await _specialtyService.GetByIdAsync(id);
                if (specialty == null) return RedirectToAction("Index");

                var model = specialty.ToEditViewModel();
                return View(model);
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error al cargar la especialidad";
                return View(new SpecialtyEditViewModel { SpecialtyId = id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SpecialtyEditViewModel model)
        {
            if (id != model.SpecialtyId) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var success = await _specialtyService.UpdateAsync(id, model.ToRequest());
            if (success)
            {
                TempData["Success"] = "¡Especialidad actualizada correctamente!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<ActionResult> Details(int id)
        {
            if (id == 0) return RedirectToAction("Index");

            var specialty = await _specialtyService.GetByIdAsync(id);
            if (specialty == null) return RedirectToAction("Index");

            var model = specialty.ToViewModel();
            return View(model);
        }
    }
}
