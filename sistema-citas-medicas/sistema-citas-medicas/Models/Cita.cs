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

        [Display(Name = "ID Médico")]
        [Required(ErrorMessage = "Seleccione un médico")]
        public int IdMedico { get; set; }

        [Display(Name = "Médico")]
        [Required(ErrorMessage = "Seleccione un médico")]
        public string NomMedico { get; set; }

        [Display(Name = "ID Paciente")]
        [Required(ErrorMessage = "Seleccione un paciente")]
        public int IdPaciente { get; set; }

        [Display(Name = "Paciente")]
        [Required(ErrorMessage = "Seleccione un paciente")]
        public string NomPaciente { get; set; }

        [Display(Name = "ID Especialidad")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public int IdEspecialidad { get; set; }

        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public string NomEspecialidad { get; set; }

        [Display(Name = "Fecha de cita")]
        [Required(ErrorMessage = "Ingrese una fecha de cita")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Display(Name = "Horario de cita")]
        [Required(ErrorMessage = "Ingrese un horario de cita")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Hora { get; set; }

        [Display(Name = "Tipo de consulta")]
        [Required(ErrorMessage = "Seleccione un tipo de consulta")]
        public string TipoConsulta { get; set; }

        [Display(Name = "Síntomas")]
        public string Sintomas { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "pendiente";

        public int IdUsuario { get; set; }
        public string TipoUsuario { get; set; }
    }
}