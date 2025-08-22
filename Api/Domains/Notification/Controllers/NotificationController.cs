using Api.Domains.Notification.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domains.Notification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotification _repository;

        public NotificationController(INotification repo)
        {
            _repository = repo;
        }

        [HttpGet("doctor/{doctorId}")]
        public IActionResult GetForDoctor(int doctorId)
        {
            var items = _repository.GetForDoctor(doctorId);
            return Ok(items);
        }

        [HttpGet("patient/{patientId}")]
        public IActionResult GetForPatient(int patientId)
        {
            var items = _repository.GetForPatient(patientId);
            return Ok(items);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var delete = _repository.Delete(id);
            return delete > 0
                ? Ok(new { message = "Se eliminó la notificación." })
                : NotFound(new { message = "No se encontró la notificación." });
        }
    }
}
