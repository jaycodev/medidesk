using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2010.Excel;
using medical_appointment_system.Models;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
{
    public class AppointmentsController : Controller
    {
        AppointmentService appointmentService = new AppointmentService();
        SpecialtyService specialtyService = new SpecialtyService();
        PatientService patientService = new PatientService();
        DoctorService doctorService = new DoctorService();

        private User user;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            user = Session["user"] as User;
        }

        private Appointment FindById(int id)
        {
            var filter = new Appointment { AppointmentId = id };
            var results = appointmentService.ExecuteRead("GET_BY_ID", filter);

            return results.FirstOrDefault();
        }

        private List<Appointment> GetAppointmentsByStatus(string status, string indicator)
        {
            return appointmentService.ExecuteRead(indicator, new Appointment
            {
                UserId = user.UserId,
                UserType = user.Role,
                Status = status
            });
        }

        public ActionResult Dashboard()
        {
            var list = GetAppointmentsByStatus(null, "GET_BY_USER_AND_STATUS");
            return View(list);
        }

        public ActionResult MyAppointments()
        {
            var list = GetAppointmentsByStatus("confirmada", "GET_BY_USER_AND_STATUS");
            return View(list);
        }

        public ActionResult Pending()
        {
            var list = GetAppointmentsByStatus("pendiente", "GET_BY_USER_AND_STATUS");
            return View(list);
        }

        public ActionResult Historial()
        {
            var list = GetAppointmentsByStatus(null, "GET_COMPLETED_OR_CANCELLED_BY_USER");
            return View(list);
        }

        public ActionResult Details(int id)
        {
            if (id == 0)
                return RedirectToAction("MyAppointments");

            return View(FindById(id));
        }

        public ActionResult Reserve()
        {
            ViewBag.Specialties = new SelectList(specialtyService.ExecuteRead("GET_ALL", new Specialty()), "SpecialtyId", "Name");

            return View(new Appointment());
        }

        public JsonResult GetDoctorsBySpecialty(int id)
        {
            var doctors = doctorService.ExecuteRead("GET_BY_SPECIALTY", new Doctor { SpecialtyId = id });
            var result = doctors.Select(d => new {
                d.UserId,
                FullName = $"{d.FirstName} {d.LastName}"
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public Tuple<TimeSpan, TimeSpan> GetDoctorScheduleByDay(int doctorId, DateTime date)
        {
            var result = appointmentService.ExecuteRead("GET_SCHEDULE_BY_DOCTOR_AND_DAY", new Appointment
            {
                DoctorId = doctorId,
                Date = date
            });

            if (result.Any())
            {
                var row = result.First();
                return Tuple.Create(row.StartTime, row.EndTime);
            }

            return null;
        }

        public JsonResult GetAvailableTimes(int doctorId, DateTime date)
        {
            var schedule = GetDoctorScheduleByDay(doctorId, date);

            if (schedule == null)
            {
                return Json(new { error = "El médico no tiene horario asignado ese día." }, JsonRequestBehavior.AllowGet);
            }

            var start = schedule.Item1;
            var end = schedule.Item2;

            var allTimes = new List<string>();
            for (var time = start; time < end; time = time.Add(TimeSpan.FromHours(1)))
            {
                allTimes.Add(time.ToString(@"hh\:mm"));
            }

            var appointments = appointmentService.ExecuteRead("GET_BY_DOCTOR_AND_DATE", new Appointment
            {
                DoctorId = doctorId,
                Date = date
            });

            var takenTimes = appointments.Select(a => a.Time.ToString(@"hh\:mm")).ToList();

            var available = allTimes.Select(t => new
            {
                Time = t,
                IsAvailable = !takenTimes.Contains(t)
            });

            return Json(available, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Reserve(Appointment appointment)
        {
            var user = Session["user"] as User;

            appointment.PatientId = user.UserId;
            appointment.Status = "pendiente";

            int appointmentId = appointmentService.ExecuteWrite("INSERT", appointment);

            if (appointmentId > 0)
                TempData["Success"] = "¡Cita reservada correctamente!";
            else
                TempData["Error"] = "No se pudo reservar la cita.";

            return RedirectToAction("Pending");
        }

        public ActionResult Cancel(int id)
        {
            if (id == 0)
                return RedirectToAction("MyAppointments");

            return View(FindById(id));
        }

        [HttpPost, ActionName("Cancel")]
        public ActionResult CancelConfirmed(int id)
        {
            var appointment = new Appointment { AppointmentId = id };

            int affected = appointmentService.ExecuteWrite("CANCEL", appointment);

            if (affected == 1)
            {
                TempData["Success"] = "La cita fue cancelada correctamente.";
            }
            else
            {
                TempData["Error"] = "Hubo un error al cancelar la cita.";
            }

            return RedirectToAction("Historial");
        }
    }
}