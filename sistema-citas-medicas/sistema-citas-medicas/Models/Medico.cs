using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models
{
    public class Medico : Usuario
    {
        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "Ingrese una especialidad")]
        public int IdEspecialidad { get; set; }

        [Display(Name = "Especialidad")]
        public string EspecialidadNombre { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }
    }
}