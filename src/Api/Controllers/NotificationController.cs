using Api.Repositories.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notifications;

        public NotificationController(INotificationRepository notifications)
        {
            _notifications = notifications;
        }

        [HttpGet("doctor/{doctorId}")]
        public IActionResult GetForDoctor(int doctorId)
        {
            var items = _notifications.GetForDoctor(doctorId);
            return Ok(items);
        }

        [HttpGet("patient/{patientId}")]
        public IActionResult GetForPatient(int patientId)
        {
            var items = _notifications.GetForPatient(patientId);
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var delete = _notifications.Delete(id);
            return delete > 0
                ? Ok(new { message = "Se eliminó la notificación." })
                : NotFound(new { message = "No se encontró la notificación." });
        }
    }
}
