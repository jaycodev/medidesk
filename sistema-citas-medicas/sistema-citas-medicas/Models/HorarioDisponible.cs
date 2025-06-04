using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models
{
    public class HorarioDisponible
    {
        [Display(Name = "ID horario")]
        public int IdHorario { get; set; }

        [Display(Name = "Médico")]
        [Required(ErrorMessage = "Ingrese un médico")]
        public int IdMedico { get; set; }

        [Display(Name = "Día de la semana")]
        [Required(ErrorMessage = "Ingrese un día")]
        [StringLength(10)]
        public string DiaSemana { get; set; }

        [Display(Name = "Hora de inicio")]
        [Required(ErrorMessage = "Ingrese una hora de inicio")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan HoraInicio { get; set; }

        [Display(Name = "Hora de fin")]
        [Required(ErrorMessage = "Ingrese una hora de fin")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan HoraFin { get; set; }

        // Nueva propiedad: no requerida, valor por defecto true
        public bool Habilita { get; set; } = true;
    }
}