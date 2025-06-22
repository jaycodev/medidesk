using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using medical_appointment_system.Models;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
{
    public class SchedulesController : Controller
    {
        ScheduleService service = new ScheduleService();

        private User user;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            user = Session["user"] as User;
        }

        public ActionResult Index()
        {
            int id = user.UserId;

            Schedule schedule = new Schedule { DoctorId = id };
            List<Schedule> currentSchedules = service.ExecuteRead("GET_BY_DOCTOR", schedule);

            var weekDays = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

            var horariosCompletos = weekDays.SelectMany(day =>
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
                            DoctorId = id,
                            Weekday = day,
                            DayWorkShift = shift.ToLower(),
                            StartTime = TimeSpan.Zero,
                            EndTime = TimeSpan.Zero,
                            IsActive = false
                        };
                    }
                });

            }).ToList();

            return View(horariosCompletos);
        }

        [HttpPost]
        public ActionResult Index(List<Schedule> schedules)
        {
            try
            {
                string errorMessages = string.Empty;

                for (int i = 0; i < schedules.Count; i++)
                {
                    var schedule = schedules[i];

                    // Validación de rango horario inválido
                    if (schedule.IsActive && schedule.StartTime >= schedule.EndTime)
                    {
                        ModelState.AddModelError($"StartTime_{i}", $"[{schedule.Weekday}] La hora de inicio debe ser menor que la hora final.");
                        errorMessages += $"En el día {schedule.Weekday}, la hora de inicio debe ser menor que la hora final.\n";
                        continue;
                    }

                    // Validación de cruce entre turno mañana y tarde
                    if (i % 2 == 0 && i + 1 < schedules.Count)
                    {
                        var morningShift = schedules[i];
                        var afternoonShift = schedules[i + 1];

                        if (morningShift.IsActive && afternoonShift.IsActive)
                        {
                            bool horariosCruzan =
                                morningShift.EndTime > afternoonShift.StartTime ||
                                morningShift.StartTime >= afternoonShift.StartTime;

                            if (horariosCruzan)
                            {
                                ModelState.AddModelError($"StartTime_{i}", $"[{schedule.Weekday}] Cruce entre turno mañana y tarde.");
                                errorMessages += $"En el día {schedule.Weekday}, los horarios de mañana y tarde no deben cruzarse.\n";
                                continue;
                            }
                        }
                    }

                    // Procesar solo si está activo y válido
                    service.ExecuteWrite("INSERT_OR_UPDATE", schedule);
                }

                if (string.IsNullOrWhiteSpace(errorMessages))
                {
                    TempData["Success"] = "¡Horarios actualizados correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = errorMessages;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al procesar la solicitud: " + ex.Message);
                TempData["Error"] = ex.Message;
            }

            return View(schedules);
        }
    }
}