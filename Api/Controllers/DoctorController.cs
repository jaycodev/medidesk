using Api.Repositories.Doctors;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Doctors.Requests;

namespace Api.Controllers
{
    [Route("api/doctors")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctors;

        public DoctorController(IDoctorRepository doctors)
        {
            _doctors = doctors;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var doctors = _doctors.GetList();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var doctor = _doctors.GetById(id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        [HttpGet("by-specialty")]
        public IActionResult GetBySpecialty([FromQuery] int specialtyId, [FromQuery] int userId)
        {
            var doctors = _doctors.GetBySpecialty(specialtyId, userId);
            return Ok(doctors);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateDoctorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (newId, error) = _doctors.Create(request);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(new { message = error });

            if (newId > 0)
            {
                var createdDoctor = _doctors.GetById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, createdDoctor);
            }

            return BadRequest("No se pudo crear el médico");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateDoctorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _doctors.Update(id, request);

            if (result > 0)
            {
                var updatedDoctor = _doctors.GetById(id);
                if (updatedDoctor == null)
                    return NotFound("Médico no encontrado después de actualizar.");

                return Ok(updatedDoctor);
            }

            return NotFound("Médico no encontrado o no se pudo actualizar");
        }
    }
}
