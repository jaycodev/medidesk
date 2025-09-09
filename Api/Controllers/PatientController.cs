using Api.Repositories.Patients;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Patients.Requests;

namespace Api.Controllers
{
    [Route("api/patients")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patients;

        public PatientController(IPatientRepository patients)
        {
            _patients = patients;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var patients = _patients.GetList();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var patient = _patients.GetById(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatePatientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (newId, error) = _patients.Create(request);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(new { message = error });

            if (newId > 0)
            {
                var createdDoctor = _patients.GetById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, createdDoctor);
            }

            return BadRequest("No se pudo crear el paciente");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdatePatientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _patients.Update(id, request);

            if (result > 0)
            {
                var updatedPatient = _patients.GetById(id);
                if (updatedPatient == null)
                    return NotFound("Paciente no encontrado después de actualizar.");

                return Ok(updatedPatient);
            }

            return NotFound("Paciente no encontrado o no se pudo actualizar");
        }
    }
}
