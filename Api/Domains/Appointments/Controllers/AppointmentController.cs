using Api.Domains.Appointment.DTOs;
using Api.Domains.Appointment.Repositories;
using Api.Domains.Doctors.Repositories;
using Api.Domains.Patients.Repositories;
using Api.Domains.Specialties.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domains.Appointment.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _repository;
        private readonly IDoctorRepository _doctors;
        private readonly ISpecialtyRepository _specialties;
        private readonly IPatient _patients;

        public AppointmentController(IAppointmentRepository repo, IDoctorRepository doctors,
        ISpecialtyRepository specialties, IPatient patients)
        {
            _repository = repo;
            _doctors = doctors;
            _specialties = specialties;
            _patients = patients;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _repository.GetById(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateAppointmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_doctors.GetById(dto.DoctorId) is null)
                return BadRequest(new { message = "El doctor no existe." });

            if (_patients.GetById(dto.PatientId) is null)
                return BadRequest(new { message = "El paciente no existe." });

            if (_specialties.GetById(dto.SpecialtyId) is null)
                return BadRequest(new { message = "La especialidad no existe." });

            var newId = _repository.Create(dto);
            if (newId > 0)
            {
                var created = _repository.GetById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, created);
            }

            return BadRequest("No se pudo crear la cita");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateAppointmentStatusDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _repository.Update(id, dto);

            if (result > 0)
            {
                var updated = _repository.GetById(id);
                if (updated == null)
                    return NotFound("Cita no encontrada después de actualizar.");

                return Ok(updated);
            }

            return NotFound("Cita no encontrada o no se pudo actualizar");
        }
    }
}
