using System.ComponentModel.DataAnnotations;

namespace Web.Models.Appointments
{
    public class AppointmentDetailDTO
    {
        [Display(Name = "Código")]
        public int AppointmentId { get; set; }

        [Display(Name = "Especialidad")]
        public string SpecialtyName { get; set; }

        [Display(Name = "Doctor(a)")]
        public string DoctorName { get; set; }

        [Display(Name = "Paciente")]
        public string PatientName { get; set; }

        [Display(Name = "Tipo de Consulta")]
        public string ConsultationType { get; set; }

        [Display(Name = "Fecha")]
        public string Date { get; set; }

        [Display(Name = "Hora")]
        public string Time { get; set; }

        [Display(Name = "Estado")]
        public string Status { get; set; }

        [Display(Name = "Síntomas")]
        public string? Symptoms { get; set; }
    }
}
