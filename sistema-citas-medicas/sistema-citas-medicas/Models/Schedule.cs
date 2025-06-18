using System;
using System.ComponentModel.DataAnnotations;

namespace sistema_citas_medicas.Models
{
    public class Schedule
    {
        [Display(Name = "Código")]
        public int ScheduleId { get; set; }

        [Display(Name = "Médico")]
        [Required(ErrorMessage = "Ingrese un médico")]
        public int DoctorId { get; set; }

        [Display(Name = "Día de la semana")]
        [Required(ErrorMessage = "Ingrese un día")]
        [StringLength(10)]
        public string Weekday { get; set; }

        [Display(Name = "Hora de inicio")]
        [Required(ErrorMessage = "Ingrese una hora de inicio")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Hora de fin")]
        [Required(ErrorMessage = "Ingrese una hora de fin")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
