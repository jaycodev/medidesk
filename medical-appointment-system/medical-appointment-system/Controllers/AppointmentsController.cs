using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
            ViewBag.Doctors = new SelectList(doctorService.ExecuteRead("GET_ALL", new Doctor()), "UserId", "FirstName");

            return View(new Appointment());
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