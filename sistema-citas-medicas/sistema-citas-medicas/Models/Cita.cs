using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models
{
    public class Cita
    {
        [Display(Name = "ID cita")]
        public int IdCita { get; set; }

        [Display(Name = "Médico")]
        [Required(ErrorMessage = "Ingrese un médico")]
        public int IdMedico { get; set; }

        [Display(Name = "Paciente")]
        [Required(ErrorMessage = "Ingrese un paciente")]
        public int IdPaciente { get; set; }

        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "Ingrese una especialidad")]
        public int IdEspecialidad { get; set; }

        [Display(Name = "Fecha de cita")]
        [Required(ErrorMessage = "Ingrese una fecha de cita")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Hora de cita")]
        [Required(ErrorMessage = "Ingrese una hora de cita")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Hora { get; set; }

        [Display(Name = "Tipo de consulta")]
        [Required(ErrorMessage = "Ingrese un tipo de consulta")]
        public string TipoConsulta { get; set; }

        [Display(Name = "Síntomas")]
        [Required(ErrorMessage = "Ingrese unos síntomas")]
        public string Sintomas { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "pendiente";
    }
}