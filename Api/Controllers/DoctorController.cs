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
            try
            {
                var doctors = _doctors.GetList();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var doctor = _doctors.GetById(id);
                if (doctor == null)
                    return NotFound(new { message = "Médico no encontrado" });

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("by-specialty")]
        public IActionResult GetBySpecialty([FromQuery] int specialtyId, [FromQuery] int userId)
        {
            try
            {
                var doctors = _doctors.GetBySpecialty(specialtyId, userId);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateDoctorRequest request)
        {
            try
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

                return BadRequest(new { message = "No se pudo crear el médico" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateDoctorRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = _doctors.Update(id, request);

                if (result > 0)
                {
                    var updatedDoctor = _doctors.GetById(id);
                    if (updatedDoctor == null)
                        return NotFound(new { message = "Médico no encontrado después de actualizar." });

                    return Ok(updatedDoctor);
                }

                return NotFound(new { message = "Médico no encontrado o no se pudo actualizar" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }
    }
}
