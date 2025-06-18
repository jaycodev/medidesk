using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using sistema_citas_medicas.Models;
using sistema_citas_medicas.Services;

namespace sistema_citas_medicas.Controllers
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

        public ActionResult Set()
        {
            int id = user.UserId;

            Schedule schedule = new Schedule { DoctorId = id };
            List<Schedule> currentSchedules = service.ExecuteRead("GET_BY_DOCTOR", schedule);

            var weekDays = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

            var horariosCompletos = weekDays.Select(day =>
            {
                var existing = currentSchedules.FirstOrDefault(h => h.Weekday.Equals(day, StringComparison.OrdinalIgnoreCase));

                if (existing != null)
                {
                    return existing;
                }
                else
                {
                    return new Schedule
                    {
                        Weekday = day,
                        DoctorId = id,
                        IsActive = false,
                        StartTime = TimeSpan.Zero,
                        EndTime = new TimeSpan(1, 0, 0)
                    };
                }
            }).ToList();

            return View(horariosCompletos);
        }

        [HttpPost]
        public ActionResult Set(List<Schedule> schedules)
        {
            try
            {
                string errorComment = string.Empty;

                foreach (var schedule in schedules)
                {
                    if (schedule.StartTime >= schedule.EndTime)
                    {
                        ModelState.AddModelError("StartTime", "La hora de inicio debe ser menor que la hora de fin.");
                        if (schedule.IsActive)
                            errorComment += ("En el dia " + schedule.Weekday + " la hora de inicio debe ser menor a la hora final\n");
                        continue;
                    }

                    int procesar = service.ExecuteWrite("INSERT_OR_UPDATE", schedule);
                }

                if (errorComment == string.Empty)
                {
                    TempData["Success"] = "Horarios actualizados correctamente!";
                    return RedirectToAction("Index");
                }
                else
                    TempData["Error"] = errorComment;
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