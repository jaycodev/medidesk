using Api.Domains.Schedules.DTOs;
using Api.Domains.Schedules.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domains.Schedules.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ISchedule scheduleDATA;

        public ScheduleController(ISchedule schedule)
        {
            scheduleDATA = schedule;
        }

        [HttpGet("{idDoctor}")]
        public IActionResult GetShedulesById(int idDoctor)
        {
            List<ScheduleDTO> fullschedules = scheduleDATA.GetListSchedulesByIdDoctor(idDoctor);

            return Ok(fullschedules);
        }


        [HttpPost]
        public IActionResult UpdateSchedules(List<ScheduleDTO> schedules)
        {
            try
            {
                List<string> messages = scheduleDATA.CreateOrUpdateSchedules(schedules);
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
