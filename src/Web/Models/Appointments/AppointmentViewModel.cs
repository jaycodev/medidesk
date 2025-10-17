using System.ComponentModel.DataAnnotations;

namespace Web.Models.Appointments
{
    public class AppointmentViewModel
    {
        [Display(Name = "Código")]
        public int AppointmentId { get; set; }

        [Display(Name = "Especialidad")]
        public string SpecialtyName { get; set; } = string.Empty;

        [Display(Name = "Médico")]
        public string DoctorName { get; set; } = string.Empty;

        [Display(Name = "Paciente")]
        public string PatientName { get; set; } = string.Empty;

        [Display(Name = "Tipo consulta")]
        public string ConsultationType { get; set; } = string.Empty;

        [Display(Name = "Fecha cita")]
        public DateOnly Date { get; set; }

        [Display(Name = "Horario cita")]
        public TimeSpan Time { get; set; }

        [Display(Name = "Síntomas")]
        public string? Symptoms { get; set; }

        [Display(Name = "Estado")]
        public string Status { get; set; } = string.Empty;
    }
}
