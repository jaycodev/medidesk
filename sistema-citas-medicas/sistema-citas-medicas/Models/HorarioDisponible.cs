using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models
{
    public class HorarioDisponible
    {
        [Required]
        public int IdHorario { get; set; }

        [Required]
        public int IdMedico { get; set; }

        [Required]
        [StringLength(10)]
        public string DiaSemana { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan HoraFin { get; set; }
    }
}