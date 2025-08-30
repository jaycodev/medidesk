using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly HttpClient _http;

        public NotificationsController(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        [HttpPost]
        public async Task<JsonResult> Delete([FromBody] JsonElement payload)
        {
            if (!payload.TryGetProperty("id", out var prop) || !prop.TryGetInt32(out var id))
                return Json(new { success = false, message = "Id inválido" });

            try
            {
                var response = await _http.DeleteAsync($"api/notifications/{id}");

                if (response.IsSuccessStatusCode)
                    return Json(new { success = true });

                var content = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = content });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false, message = "Ocurrió un error inesperado" });
            }
        }
    }
}
