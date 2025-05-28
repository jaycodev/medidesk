using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models
{
    public class Cita
    {
        [Required]
        public int IdCita { get; set; }

        [Required]
        public int IdMedico { get; set; }

        [Required]
        public int IdPaciente { get; set; }

        [Required]
        public int IdEspecialidad { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required]
        public DateTime Fecha { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        [Required]
        public TimeSpan Hora { get; set; }

        public string TipoConsulta { get; set; }

        public string Sintomas { get; set; }

        public string Estado { get; set; } = "pendiente";
    }
}