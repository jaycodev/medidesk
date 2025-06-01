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
        public int IdUsuario { get; set; }

        [Display(Name = " Nombres")]
        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        public string Nombre { get; set; }

        [Display(Name = " Apellidos")]
        [Required(ErrorMessage = "El campo Apellido es obligatorio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Ingrese un correo")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Ingrese una contraseña")]
        public string Contraseña { get; set; }


        public string Telefono { get; set; }

        [Required]
        public string Rol { get; set; }

        public string FotoPerfil { get; set; }
    }
}