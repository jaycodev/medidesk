using Api.Domains.Doctors.DTOs;
using Api.Domains.Doctors.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domains.Doctors.Controllers
{
    [Route("api/doctors")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _repository;

        public DoctorController(IDoctorRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var doctors = _repository.GetList();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var doctor = _repository.GetById(id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        [HttpGet("by-specialty")]
        public IActionResult GetBySpecialty([FromQuery] int specialtyId, [FromQuery] int userId)
        {
            var doctors = _repository.GetBySpecialty(specialtyId, userId);
            return Ok(doctors);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateDoctorDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (newId, error) = _repository.Create(dto);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(new { message = error });

            if (newId > 0)
            {
                var createdDoctor = _repository.GetById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, createdDoctor);
            }

            return BadRequest("No se pudo crear el médico");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateDoctorDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _repository.Update(id, dto);

            if (result > 0)
            {
                var updatedDoctor = _repository.GetById(id);
                if (updatedDoctor == null)
                    return NotFound("Médico no encontrado después de actualizar.");

                return Ok(updatedDoctor);
            }

            return NotFound("Médico no encontrado o no se pudo actualizar");
        }
    }
}
