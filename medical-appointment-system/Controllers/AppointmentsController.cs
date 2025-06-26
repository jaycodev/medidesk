using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Web.Mvc;
using System.Web.WebPages;
using Antlr.Runtime.Tree;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
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
        NotificationService notificationService = new NotificationService();

        private User user;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            user = Session["user"] as User;
        }

        private Appointment GetAppointmentIds(int id)
        {
            var filter = new Appointment { AppointmentId = id };
            var result = appointmentService.ExecuteRead("GET_IDS_BY_ID", filter);
            return result.FirstOrDefault();
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
                UserRol = user.ActiveRole,
                Status = status
            });
        }

        public ActionResult Home()
        {
            var list = GetAppointmentsByStatus(null, "GET_BY_USER_AND_STATUS");
            return View(list);
        }

        public ActionResult AllAppointments()
        {
            var list = appointmentService.ExecuteRead("GET_ALL", new Appointment());
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
                return RedirectToAction("Home");

            return View(FindById(id));
        }

        public ActionResult Reserve()
        {
            ViewBag.Specialties = new SelectList(specialtyService.ExecuteRead("GET_ALL", new Specialty()), "SpecialtyId", "Name");

            return View(new Appointment());
        }

        public JsonResult GetDoctorsBySpecialty(int id)
        {
            var doctors = doctorService.ExecuteRead("GET_BY_SPECIALTY", new Doctor
            {
                SpecialtyId = id,
                UserId = user.UserId
            });

            var result = doctors.Select(d => new
            {
                d.UserId,
                FullName = $"{d.FirstName} {d.LastName}"
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public List<Appointment> GetDoctorScheduleByDay(int doctorId, DateTime date)
        {
            var result = appointmentService.ExecuteRead("GET_SCHEDULE_BY_DOCTOR_AND_DAY", new Appointment
            {
                DoctorId = doctorId,
                Date = date
            });

            var shifts = new List<Appointment>();

            foreach (var row in result)
            {
                shifts.Add(new Appointment
                {
                    DayWorkShift = row.DayWorkShift,
                    StartTime = row.StartTime,
                    EndTime = row.EndTime
                });
            }

            return shifts;
        }

        public JsonResult GetAvailableTimes(int doctorId, DateTime date)
        {
            var shifts = GetDoctorScheduleByDay(doctorId, date);

            if (shifts == null || !shifts.Any())
            {
                return Json(new { error = "El médico no tiene horario asignado ese día." }, JsonRequestBehavior.AllowGet);
            }

            var allTimes = new List<string>();

            foreach (var shift in shifts)
            {
                for (var time = shift.StartTime; time < shift.EndTime; time = time.Add(TimeSpan.FromHours(1)))
                {
                    allTimes.Add(time.ToString(@"hh\:mm"));
                }
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

            int affectedRows = appointmentService.ExecuteWrite("INSERT", appointment);

            if (affectedRows > 0)
                TempData["Success"] = "¡Cita reservada correctamente!";
            else
                TempData["Error"] = "No se pudo reservar la cita.";

            return RedirectToAction("Pending");
        }

        public ActionResult Confirm(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = FindById(id);

            if (appointment == null || appointment.Status?.ToLower() != "pendiente")
            {
                TempData["Error"] = "Solo se pueden confirmar citas pendientes.";
                return RedirectToAction("Pending");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Confirm")]
        public ActionResult ConfirmConfirmed(int id)
        {
            var appointment = new Appointment { AppointmentId = id };

            int affectedRows = appointmentService.ExecuteWrite("CONFIRM", appointment);

            if (affectedRows == 1)
            {
                TempData["Success"] = "La cita fue confirmada correctamente.";
            }
            else
            {
                TempData["Error"] = "Hubo un error al confirmar la cita.";
            }

            return RedirectToAction("MyAppointments");
        }

        public ActionResult Attend(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = FindById(id);

            if (appointment == null || appointment.Status?.ToLower() != "confirmada")
            {
                TempData["Error"] = "Solo se pueden atender citas confirmadas.";
                return RedirectToAction("MyAppointments");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Attend")]
        public ActionResult AttendConfirmed(int id)
        {
            var appointment = new Appointment { AppointmentId = id };

            int affectedRows = appointmentService.ExecuteWrite("ATTEND", appointment);

            if (affectedRows == 1)
            {
                TempData["Success"] = "La cita fue atendida correctamente.";
            }
            else
            {
                TempData["Error"] = "Hubo un error al atender la cita.";
            }

            return RedirectToAction("Historial");
        }

        public ActionResult Cancel(int id)
        {
            if (id == 0)
                return RedirectToAction("Home");

            var appointment = FindById(id);

            var status = appointment?.Status?.ToLower();

            if (appointment == null || status == "cancelada" || status == "atendida")
            {
                TempData["Error"] = "Solo se pueden cancelar citas pendientes o confirmadas.";
                return RedirectToAction("Pending");
            }

            return View(appointment);
        }

        [HttpPost, ActionName("Cancel")]
        public ActionResult CancelConfirmed(int id)
        {
            var appointment = new Appointment { AppointmentId = id };

            int affectedRows = appointmentService.ExecuteWrite("CANCEL", appointment);

            if (affectedRows == 1)
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