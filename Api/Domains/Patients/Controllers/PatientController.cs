using Api.Domains.Patients.DTOs;
using Api.Domains.Patients.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domains.Patients.Controllers
{
    [Route("api/patients")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _repository;

        public PatientController(IPatientRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var patients = _repository.GetList();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var patient = _repository.GetById(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatePatientDTO dto)
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

            return BadRequest("No se pudo crear el paciente");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdatePatientDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _repository.Update(id, dto);

            if (result > 0)
            {
                var updatedPatient = _repository.GetById(id);
                if (updatedPatient == null)
                    return NotFound("Paciente no encontrado después de actualizar.");

                return Ok(updatedPatient);
            }

            return NotFound("Paciente no encontrado o no se pudo actualizar");
        }
    }
}
