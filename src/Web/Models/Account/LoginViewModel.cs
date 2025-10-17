using System.ComponentModel.DataAnnotations;

namespace Web.Models.Account
{
    public class LoginViewModel
    {
        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Ingrese su contraseña")]
        public string Password { get; set; } = string.Empty;
    }
}
