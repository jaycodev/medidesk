using System;
using System.ComponentModel.DataAnnotations;

namespace medical_appointment_system.Models
{
    public class Appointment
    {
        [Display(Name = "Código")]
        public int AppointmentId { get; set; }

        [Display(Name = "Código Médico")]
        [Required(ErrorMessage = "Seleccione un médico")]
        public int DoctorId { get; set; }

        [Display(Name = "Médico")]
        [Required(ErrorMessage = "Seleccione un médico")]
        public string DoctorName { get; set; }

        [Display(Name = "Código paciente")]
        [Required(ErrorMessage = "Seleccione un paciente")]
        public int PatientId { get; set; }

        [Display(Name = "Paciente")]
        [Required(ErrorMessage = "Seleccione un paciente")]
        public string PatientName { get; set; }

        [Display(Name = "Código especialidad")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public string SpecialtyName { get; set; }

        [Display(Name = "Fecha cita")]
        [Required(ErrorMessage = "Ingrese una fecha de cita")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "Horario cita")]
        [Required(ErrorMessage = "Ingrese un horario de cita")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Time { get; set; }

        [Display(Name = "Tipo consulta")]
        [Required(ErrorMessage = "Seleccione un tipo de consulta")]
        public string ConsultationType { get; set; }

        [Display(Name = "Síntomas")]
        public string Symptoms { get; set; }

        [Display(Name = "Estado")]
        public string Status { get; set; } = "pendiente";

        public int UserId { get; set; }

        public string UserType { get; set; }
    }
}