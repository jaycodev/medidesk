using System.ComponentModel.DataAnnotations;

namespace medical_appointment_system.Models.ViewModels
{
    public class LoginViewModel
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