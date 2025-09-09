using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Appointments.Requests
{
    public class UpdateAppointmentRequest
    {
        [Required]
        [RegularExpression("pendiente|confirmada|cancelada|atendida",
        ErrorMessage = "El estado solo debe ser: pendiente | confirmada | cancelada | atendida")]
        public string Status { get; set; } = string.Empty;
    }
}
