using Api.Queries;
using Api.Repositories.Appointments;
using Api.Repositories.Doctors;
using Api.Repositories.Patients;
using Api.Repositories.Specialties;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Appointments.Requests;

namespace Api.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _appointments;
        private readonly IDoctorRepository _doctors;
        private readonly ISpecialtyRepository _specialties;
        private readonly IPatientRepository _patients;

        public AppointmentController(IAppointmentRepository appointments, IDoctorRepository doctors,
        ISpecialtyRepository specialties, IPatientRepository patients)
        {
            _appointments = appointments;
            _doctors = doctors;
            _specialties = specialties;
            _patients = patients;
        }

        [HttpGet("all-by-user")]
        public IActionResult GetAllByUser([FromQuery] AppointmentQuery query)
        {
            if (query.UserId <= 0 || string.IsNullOrWhiteSpace(query.UserRole))
                return BadRequest("Se requiere userId y userRol válidos.");

            var list = _appointments.GetAppointmentsByStatus(query);
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] ListQuery listQuery, [FromQuery] AppointmentQuery query)
        {
            return Ok(_appointments.GetAll(listQuery, query));
        }

        [HttpGet("my")]
        public IActionResult GetMyAppointments([FromQuery] AppointmentQuery query)
        {
            if (query.UserId <= 0 || string.IsNullOrWhiteSpace(query.UserRole))
                return BadRequest("Se requiere userId y userRol válidos.");

            query = new AppointmentQuery()
            {
                Status = "confirmada"
            };

            var list = _appointments.GetAppointmentsByStatus(query);
            return Ok(list);
        }

        [HttpGet("historial")]
        public IActionResult GetHistorial([FromQuery] int userId, [FromQuery] string userRol)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(userRol))
                return BadRequest("Se requiere userId y userRol válidos.");

            var list = _appointments.GetHistorial(userId, userRol);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _appointments.GetById(id);
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

            var times = _appointments.GetByDoctorAndDate(doctorId, date);

            return Ok(times);
        }

        [HttpPost]
        public IActionResult Reserve([FromBody] CreateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_doctors.GetById(request.DoctorId) is null)
                return BadRequest(new { message = "El doctor no existe." });

            if (_patients.GetById(request.PatientId) is null)
                return BadRequest(new { message = "El paciente no existe." });

            if (_specialties.GetById(request.SpecialtyId) is null)
                return BadRequest(new { message = "La especialidad no existe." });

            var newId = _appointments.Reserve(request);
            if (newId > 0)
            {
                var created = _appointments.GetById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, created);
            }

            return BadRequest("No se pudo crear la cita");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _appointments.Update(id, request);

            if (result > 0)
            {
                var updated = _appointments.GetById(id);
                if (updated == null)
                    return NotFound("Cita no encontrada después de actualizar.");

                return Ok(updated);
            }

            return NotFound("Cita no encontrada o no se pudo actualizar");
        }
    }
}
