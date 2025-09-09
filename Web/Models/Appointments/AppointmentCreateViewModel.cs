using System.ComponentModel.DataAnnotations;

namespace Web.Models.Appointments
{
    public class AppointmentCreateViewModel
    {
        [Display(Name = "Médico")]
        [Required(ErrorMessage = "Seleccione un médico")]
        public int DoctorId { get; set; }

        [Display(Name = "Paciente")]
        [Required(ErrorMessage = "Seleccione un paciente")]
        public int PatientId { get; set; }

        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Fecha cita")]
        [Required(ErrorMessage = "Ingrese una fecha de cita")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? Date { get; set; }

        [Display(Name = "Horario cita")]
        [Required(ErrorMessage = "Ingrese un horario de cita")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Time { get; set; }

        [Display(Name = "Tipo consulta")]
        [Required(ErrorMessage = "Seleccione un tipo de consulta")]
        public string ConsultationType { get; set; } = string.Empty;

        [Display(Name = "Sintomas")]
        public string? Symptoms { get; set; }
    }
}
