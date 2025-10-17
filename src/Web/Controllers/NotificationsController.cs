using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Notification;

namespace Web.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<JsonResult> Delete([FromBody] JsonElement payload)
        {
            if (!payload.TryGetProperty("id", out var prop) || !prop.TryGetInt32(out var id))
                return Json(new { success = false, message = "Id inválido" });

            var success = await _notificationService.DeleteAsync(id);

            if (success)
                return Json(new { success = true });

            return Json(new { success = false, message = "Ocurrió un error al eliminar la notificación" });
        }
    }
}
