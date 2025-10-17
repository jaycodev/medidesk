using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Doctors.Requests;
using Shared.DTOs.Patients.Requests;
using Shared.DTOs.Users.Requests;
using Web.Mappers;
using Web.Models.Users;
using Web.Services.Doctor;
using Web.Services.Patient;
using Web.Services.Specialty;
using Web.Services.User;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly ISpecialtyService _specialtyService;

        public UsersController(IUserService userService, IDoctorService doctorService, IPatientService patientService, ISpecialtyService specialtyService)
        {
            _userService = userService;
            _doctorService = doctorService;
            _patientService = patientService;
            _specialtyService = specialtyService;
        }

        private async Task LoadSpecialtiesAsync(int? selectedId = null)
        {
            ViewBag.Specialties = await _specialtyService.GetSelectListAsync(selectedId);
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecialties() => Json(await _specialtyService.GetListAsync());

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetListAsync();
            var viewModelList = users.Select(u => u.ToListViewModel()).ToList();
            return View(viewModelList);
        }

        public IActionResult Create() => View(new UserCreateViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (Success, Message) = await _userService.CreateAsync(model.ToCreateRequest());
            if (Success)
            {
                TempData["Success"] = "¡Usuario registrado correctamente!";
                return RedirectToAction("Index");
            }

            ViewBag.Message = Message;
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0) return RedirectToAction("Index");

            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null) return RedirectToAction("Index");

                var model = user.ToEditViewModel();

                if (user.Roles != null && user.Roles.Contains("medico"))
                {
                    var doc = await _doctorService.GetByIdAsync(id);
                    if (doc != null)
                    {
                        model.SpecialtyId = doc.SpecialtyId;
                        model.Status = doc.Status;
                    }

                    await LoadSpecialtiesAsync(model.SpecialtyId);
                }

                if (user.Roles != null && user.Roles.Contains("paciente"))
                {
                    var pat = await _patientService.GetByIdAsync(id);
                    if (pat != null)
                    {
                        model.BirthDate = pat.BirthDate;
                        model.BloodType = pat.BloodType;
                    }
                }

                return View(model);
            }
            catch
            {
                ViewBag.Message = "Ocurrió un error al cargar el usuario.";
                await LoadSpecialtiesAsync(null);
                return View(new UserEditViewModel { UserId = id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UserEditViewModel model)
        {
            if (id != model.UserId) return BadRequest();

            if (!ModelState.IsValid)
            {
                if ((model.SelectedRoleCombo ?? "").Contains("medico"))
                    await LoadSpecialtiesAsync(model.SpecialtyId);

                return View(model);
            }

            try
            {
                var updateUser = model.ToUpdateRequest();

                var putUserResp = await _userService.UpdateAsync(id, updateUser);
                if (!putUserResp.Success)
                {
                    ViewBag.Message = putUserResp.Message;
                    return View(model);
                }

                if (updateUser.Roles.Contains("medico"))
                {
                    if (model.SpecialtyId == null || model.SpecialtyId == 0 || model.Status == null)
                    {
                        ViewBag.Message = "Faltan campos requeridos para el rol médico.";
                        await LoadSpecialtiesAsync(model.SpecialtyId);
                        return View(model);
                    }

                    var updateDoctor = new UpdateDoctorRequest
                    {
                        SpecialtyId = model.SpecialtyId.Value,
                        Status = model.Status.Value
                    };

                    var putDocResp = await _doctorService.UpdateAsync(id, updateDoctor);
                    if (!putDocResp.Success)
                    {
                        ViewBag.Message = putDocResp.Message;
                        await LoadSpecialtiesAsync(model.SpecialtyId);
                        return View(model);
                    }
                }

                if (updateUser.Roles.Contains("paciente"))
                {
                    if (model.BirthDate == null)
                    {
                        ViewBag.Message = "Faltan campos requeridos para el rol paciente.";
                        return View(model);
                    }

                    var updatePatient = new UpdatePatientRequest
                    {
                        BirthDate = model.BirthDate.Value,
                        BloodType = model.BloodType
                    };

                    var putPatResp = await _patientService.UpdateAsync(id, updatePatient);
                    if (!putPatResp.Success)
                    {
                        ViewBag.Message = putPatResp.Message;
                        await LoadSpecialtiesAsync(model.SpecialtyId);
                        return View(model);
                    }
                }

                TempData["Success"] = "¡Médico actualizado correctamente!";
                return RedirectToAction("Index");
            }
            catch (ApplicationException aex)
            {
                ViewBag.Message = aex.Message;
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            await LoadSpecialtiesAsync(model.SpecialtyId);
            return View(model);
        }

        public async Task<ActionResult> Details(int id)
        {
            if (id == 0) return RedirectToAction("Index");

            var user = await _userService.GetByIdAsync(id);
            if (user == null) return RedirectToAction("Index");

            var model = user.ToDetailViewModel();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return RedirectToAction("Index");

            var (Success, Message) = await _userService.DeleteAsync(id);
            TempData[Success ? "Success" : "Error"] = Success ? "¡Usuario eliminado correctamente!" : Message;

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ExportToPdf()
        {
            var pdfBytes = await _userService.GeneratePdfAsync();
            return File(pdfBytes, "application/pdf", "Usuarios.pdf");
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var excelBytes = await _userService.GenerateExcelAsync();
            return File(excelBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Usuarios.xlsx");
        }
    }
}
