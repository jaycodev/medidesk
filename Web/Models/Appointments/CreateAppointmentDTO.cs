using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Appointments
{
    public class CreateAppointmentDTO
    {
        [Display(Name = "Doctor(a)")]
        [Required(ErrorMessage = "Seleccione un doctor(a)")]
        public int DoctorId { get; set; }

        [Display(Name = "Paciente")]
        [Required(ErrorMessage = "Seleccione un paciente")]
        public int PatientId { get; set; }

        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Fecha cita")]
        [Required(ErrorMessage = "Seleccione una fecha")]
        [DataType(DataType.Date)]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Use formato yyyy-MM-dd")]
        public string Date { get; set; } = string.Empty;

        [Display(Name = "Hora cita")]
        [Required(ErrorMessage = "Ingrese una hora")]
        [DataType(DataType.Time)]
        [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Use formato HH:mm")]
        public string Time { get; set; } = string.Empty;

        [Display(Name = "Tipo de consulta")]
        [Required(ErrorMessage = "Seleccione el tipo de Consulta")]
        public string ConsultationType { get; set; } = string.Empty;

        [Display(Name = "Sintomas")]
        [Required(ErrorMessage = "Ingrese los síntomas")]
        public string? Symptoms { get; set; }
    }
}
