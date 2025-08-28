using System.ComponentModel.DataAnnotations;

namespace Api.Domains.Appointment.DTOs
{
    public class UpdateAppointmentStatusDTO
    {
        [Required]
        [RegularExpression("pendiente|confirmada|cancelada|atendida",
            ErrorMessage = "El estado solo debe ser: pendiente | confirmada | cancelada | atendida")]
        public string Status { get; set; } = string.Empty;
    }
}
