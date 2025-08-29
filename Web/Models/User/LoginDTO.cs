using System.ComponentModel.DataAnnotations;

namespace Web.Models.User
{
    public class LoginDTO
    {
        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Ingrese su contraseña")]
        public string Password { get; set; }
    }
}
    