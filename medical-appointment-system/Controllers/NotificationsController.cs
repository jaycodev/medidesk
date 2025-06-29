using System.Web.Mvc;
using medical_appointment_system.Models;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
{
    public class NotificationsController : Controller
    {
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var service = new NotificationService();
            var noti = new Notification { NotificationId = id };
            service.ExecuteWrite("DELETE_BY_ID", noti);
            return Json(new { success = true });
        }
    }
}