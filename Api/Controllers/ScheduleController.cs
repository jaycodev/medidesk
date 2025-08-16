using Api.Data.Contract;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IGenericContract<Schedule> scheduleDATA;

        public ScheduleController(IGenericContract<Schedule> schedule)
        {
            scheduleDATA = schedule;
        }

        [HttpGet("{idDoctor}")]
        public IActionResult GetById(int idDoctor) {
            Schedule schedule = new Schedule { DoctorId = idDoctor }; //doctor ID
            List<Schedule> currentSchedules = scheduleDATA.ExecuteRead("GET_BY_DOCTOR", schedule);
            
            if (currentSchedules != null && currentSchedules.Any())
            {
                var weekDays = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

                var fullSchedules = weekDays.SelectMany(day =>
                {
                    var existing = currentSchedules.FindAll(h => h.Weekday.Equals(day, StringComparison.OrdinalIgnoreCase)).ToList();
                    var dayWorkShifts = new[] { "Mañana", "Tarde" };

                    return dayWorkShifts.Select(shift =>
                    {
                        var existingShift = existing.FirstOrDefault(h => h.DayWorkShift == shift.ToLower());
                        if (existingShift != null)
                        {
                            return existingShift;
                        }
                        else
                        {
                            return new Schedule
                            {
                                DoctorId = idDoctor,
                                Weekday = day,
                                DayWorkShift = shift.ToLower(),
                                StartTime = TimeSpan.Zero,
                                EndTime = TimeSpan.Zero,
                                IsActive = false
                            };
                        }
                    });

                }).ToList();

                return Ok(fullSchedules);
            }
            return NotFound(new { message = "No se encontraron horarios para el doctor especificado." });
        }


        [HttpPost]
        public IActionResult UpdateSchedules(List<Schedule> schedules)
        {
            try
            {
                var errors = new List<string>();

                // Validaciones
                for (int i = 0; i < schedules.Count; i++)
                {
                    var schedule = schedules[i];

                    if (schedule.IsActive && schedule.StartTime >= schedule.EndTime)
                    {
                        errors.Add($"En el día {schedule.Weekday}, la hora de inicio debe ser menor que la hora final.");
                        continue;
                    }

                    if (i % 2 == 0 && i + 1 < schedules.Count)
                    {
                        var morningShift = schedules[i];
                        var afternoonShift = schedules[i + 1];

                        if (morningShift.IsActive && afternoonShift.IsActive)
                        {
                            bool doSchedulesOverlap =
                                morningShift.EndTime > afternoonShift.StartTime ||
                                morningShift.StartTime >= afternoonShift.StartTime;

                            if (doSchedulesOverlap)
                            {
                                errors.Add($"En el día {schedule.Weekday}, los horarios de mañana y tarde no deben cruzarse.");
                                continue;
                            }
                        }
                    }
                }

                // Si hay errores → no guardar nada
                if (errors.Any())
                {
                    return BadRequest(new { errors });
                }

                // Persistencia (solo si todo está OK)
                int totalAffected = schedules.Sum(s => scheduleDATA.ExecuteWrite("INSERT_OR_UPDATE", s));

                return Ok(new
                {
                    message = totalAffected > 0
                        ? "¡Horarios actualizados correctamente!"
                        : "No se realizaron cambios."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Ocurrió un error al procesar la solicitud: " + ex.Message });
            }
        }

    }
}
