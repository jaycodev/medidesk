using Api.Repositories.Specialties;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Specialties.Requests;

namespace Api.Controllers
{
    [Route("api/specialties")]
    [ApiController]
    public class SpecialtyController : ControllerBase
    {
        private readonly ISpecialtyRepository _specialties;

        public SpecialtyController(ISpecialtyRepository specialties)
        {
            _specialties = specialties;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var specialties = _specialties.GetList();
            return Ok(specialties);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var specialty = _specialties.GetById(id);
            if (specialty == null)
                return NotFound();

            return Ok(specialty);
        }

        [HttpPost]
        public IActionResult Create([FromBody] SpecialtyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newId = _specialties.Create(request);

            if (newId > 0)
            {
                var createdSpecialty = _specialties.GetById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, createdSpecialty);
            }

            return BadRequest("No se pudo crear la especialidad");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] SpecialtyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _specialties.Update(id, request);

            if (result > 0)
            {
                var updatedSpecialty = _specialties.GetById(id);
                if (updatedSpecialty == null)
                    return NotFound("Especialidad no encontrada después de actualizar.");

                return Ok(updatedSpecialty);
            }

            return NotFound("Especialidad no encontrada o no se pudo actualizar");
        }
    }
}
