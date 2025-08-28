using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Api.Domains.Appointment.DTOs
{
    public class CreateAppointmentDTO
    {
        [Required] 
        public int DoctorId { get; set; }
        [Required] 
        public int PatientId { get; set; }
        [Required] 
        public int SpecialtyId { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date debe ser yyyy-MM-dd")]
        [DefaultValue("2000-01-20")]
        public string Date { get; set; } = "2000-01-20";
        [Required]
        [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "La hora debe ser HH:mm")]
        [DefaultValue("00:00")]
        public string Time { get; set; } = "00:00";

        [Required, MaxLength(20)]
        [RegularExpression(@"^(examen|consulta|operacion)$",
            ErrorMessage = "ConsultationType debe ser: examen, consulta u operacion")]
        [DefaultValue("consulta")]
        public string ConsultationType { get; set; } = "consulta";
        public string? Symptoms { get; set; }
    }
}
