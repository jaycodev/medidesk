﻿using System.ComponentModel.DataAnnotations;

namespace Web.Models.Schedules
{
    public class ScheduleViewModel
    {
        [Display(Name = "Médico")]
        [Required(ErrorMessage = "Ingrese un médico")]
        public int DoctorId { get; set; }

        [Display(Name = "Día de la semana")]
        [Required(ErrorMessage = "Ingrese un día")]
        [StringLength(10)]
        public string Weekday { get; set; } = string.Empty;

        [Display(Name = "Turno")]
        public string DayWorkShift { get; set; } = string.Empty;

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
