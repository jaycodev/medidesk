using Api.Domains.Appointments.DTOs;
using Api.Domains.Appointments.Repositories;
using Api.Domains.Doctors.Repositories;
using Api.Domains.Patients.Repositories;
using Api.Domains.Specialties.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domains.Appointments.Controllers
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

        [HttpGet("my")]
        public IActionResult GetMyAppointments([FromQuery] int userId, [FromQuery] string userRol)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(userRol))
                return BadRequest("Se requiere userId y userRol válidos.");

            var list = _repository.GetAppointmentsByStatus(userId, userRol, "confirmada");
            return Ok(list);
        }

        [HttpGet("pending")]
        public IActionResult GetPendingAppointments([FromQuery] int userId, [FromQuery] string userRol)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(userRol))
                return BadRequest("Se requiere userId y userRol válidos.");

            var list = _repository.GetAppointmentsByStatus(userId, userRol, "pendiente");
            return Ok(list);
        }

        [HttpGet("historial")]
        public IActionResult GetHistorial([FromQuery] int userId, [FromQuery] string userRol)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(userRol))
                return BadRequest("Se requiere userId y userRol válidos.");

            var list = _repository.GetHistorial(userId, userRol);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _repository.GetById(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpGet("by-doctor-and-date")]
        public IActionResult GetByDoctorAndDate([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            if (doctorId <= 0)
                return BadRequest("Se requiere un doctorId válido.");

            var doctor = _doctors.GetById(doctorId);
            if (doctor == null)
                return NotFound("El doctor no existe.");

            var times = _repository.GetByDoctorAndDate(doctorId, date);

            return Ok(times);
        }

        [HttpGet("schedule-by-doctor-and-day")]
        public IActionResult GetScheduleByDoctorAndDay([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            if (doctorId <= 0)
                return BadRequest("Se requiere un doctorId válido.");

            var doctor = _doctors.GetById(doctorId);
            if (doctor == null)
                return NotFound("El doctor no existe.");

            var schedule = _repository.GetScheduleByDoctorAndDay(doctorId, date);

            return Ok(schedule);
        }

        [HttpPost]
        public IActionResult Reserve([FromBody] CreateAppointmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_doctors.GetById(dto.DoctorId) is null)
                return BadRequest(new { message = "El doctor no existe." });

            if (_patients.GetById(dto.PatientId) is null)
                return BadRequest(new { message = "El paciente no existe." });

            if (_specialties.GetById(dto.SpecialtyId) is null)
                return BadRequest(new { message = "La especialidad no existe." });

            var newId = _repository.Reserve(dto);
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
