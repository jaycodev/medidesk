using Api.Repositories.Doctors;
using Api.Repositories.Schedules;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Schedules.Requests;

namespace Api.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _schedules;
        private readonly IDoctorRepository _doctors;

        public ScheduleController(IScheduleRepository schedules, IDoctorRepository doctors)
        {
            _schedules = schedules;
            _doctors = doctors;
        }

        [HttpGet("{doctorId}")]
        public IActionResult GetList(int doctorId)
        {
            var fullschedules = _schedules.GetList(doctorId);
            return Ok(fullschedules);
        }

        [HttpGet("by-date")]
        public IActionResult GetByDoctorAndDate([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            if (doctorId <= 0)
                return BadRequest("Se requiere un doctorId válido.");

            var doctor = _doctors.GetById(doctorId);
            if (doctor == null)
                return NotFound("El doctor no existe.");

            var schedule = _schedules.GetByDate(doctorId, date);

            return Ok(schedule);
        }

        [HttpPost]
        public IActionResult CreateOrUpdate(List<ScheduleRequest> schedules)
        {
            try
            {
                var messages = _schedules.CreateOrUpdate(schedules);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud: " + ex.Message });
            }
        }
    }
}
