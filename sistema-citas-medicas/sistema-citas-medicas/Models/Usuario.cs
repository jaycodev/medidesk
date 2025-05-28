using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models
{
    public class Usuario
    {
        [Display(Name = " ID")]
        [Required(ErrorMessage = "El campo ID es obligatorio")]
        public int idUsuario { get; set; }

        [Display(Name = " Nombres")]
        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        public string nombre { get; set; }

        [Display(Name = " Apellidos")]
        [Required(ErrorMessage = "El campo Apellido es obligatorio")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "Ingrese un correo")]
        public string correo { get; set; }

        [Required(ErrorMessage = "Ingrese una contraseña")]
        public string contraseña { get; set; }


        public string telefono { get; set; }

        [Required]
        public string rol { get; set; }

        public string fotoPerfil { get; set; }
    }
}