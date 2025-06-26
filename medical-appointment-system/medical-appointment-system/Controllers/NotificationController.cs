using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using medical_appointment_system.Models;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
{
    public class NotificationController : Controller
    {
        NotificationService notificationService = new NotificationService();

        private User user;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            user = Session["user"] as User;
        }

        private List<Notification> ListadoPaciente(int id)
        {

        }

        public ActionResult Listado()
        {

            return View();
        }
    }
}