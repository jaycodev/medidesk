using System.ComponentModel.DataAnnotations;

namespace Web.Models.Appointments
{
    public class AppointmentListDTO
    {
        [Display(Name = "Código")]
        public int AppointmentId { get; set; }

        [Display(Name = "Especialidad")]
        public string SpecialtyName { get; set; } = string.Empty;

        [Display(Name = "Doctor(a)")]
        public string DoctorName { get; set; } = string.Empty;

        [Display(Name = "Paciente")]
        public string PatientName { get; set; } = string.Empty;

        [Display(Name = "Tipo de Consulta")]
        public string ConsultationType { get; set; } = string.Empty;

        [Display(Name = "Fecha")]
        public string Date { get; set; } = string.Empty;

        [Display(Name = "Hora")]
        public string Time { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public string Status { get; set; } = string.Empty;
    }
}
