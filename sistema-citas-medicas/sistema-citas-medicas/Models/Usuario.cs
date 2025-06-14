using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sistema_citas_medicas.Models
{
    public class Usuario
    {
        [Display(Name = "Código")]
        public int IdUsuario { get; set; }

        [Display(Name = "Nombre(s)")]
        [Required(ErrorMessage = "Ingrese un(os) nombre(s)")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido(s)")]
        [Required(ErrorMessage = "Ingrese un(os) apellido(s)")]
        public string Apellido { get; set; }

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string Correo { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Ingrese una contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo.")]
        public string Contraseña { get; set; }

        [Display(Name = "Teléfono")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Ingrese solo números en el teléfono")]
        public string Telefono { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "Ingrese un rol")]
        public string Rol { get; set; }

        [Display(Name = "Foto de perfil")]
        public string FotoPerfil { get; set; }
    }
}