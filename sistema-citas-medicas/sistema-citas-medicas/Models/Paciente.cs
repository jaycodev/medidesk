using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models
{
    public class Paciente : Usuario
    {
        [Display(Name = "Fecha de nacimiento")]
        [Required(ErrorMessage = "Ingrese una fecha de nacimiento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaNacimiento { get; set; }

        [Display(Name = "Grupo sanguíneo")]
        [Required(ErrorMessage = "Ingrese un grupo sanguíneo")]
        public string GrupoSanguineo { get; set; }
    }
}