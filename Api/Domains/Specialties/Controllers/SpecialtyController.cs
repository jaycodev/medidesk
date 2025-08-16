using Api.Domains.Specialties.DTOs;
using Api.Domains.Specialties.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domains.Specialties.Controllers
{
    [Route("api/specialties")]
    [ApiController]
    public class SpecialtyController : ControllerBase
    {
        private readonly ISpecialtyRepository _repository;

        public SpecialtyController(ISpecialtyRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var specialties = _repository.GetList();
            return Ok(specialties);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var specialty = _repository.GetById(id);
            if (specialty == null)
                return NotFound();

            return Ok(specialty);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateSpecialtyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newId = _repository.Create(dto);

            if (newId > 0)
            {
                var createdSpecialty = _repository.GetById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, createdSpecialty);
            }

            return BadRequest("No se pudo crear la especialidad");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateSpecialtyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _repository.Update(id, dto);

            if (result > 0)
            {
                var updatedSpecialty = _repository.GetById(id);
                if (updatedSpecialty == null)
                    return NotFound("Especialidad no encontrada después de actualizar.");

                return Ok(updatedSpecialty);
            }

            return NotFound("Especialidad no encontrada o no se pudo actualizar");
        }
    }
}
