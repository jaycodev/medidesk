using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class UserCreateViewModel
    {
        [Display(Name = "Nombre(s)")]
        [Required(ErrorMessage = "Ingrese un(os) nombre(s)")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Apellido(s)")]
        [Required(ErrorMessage = "Ingrese un(os) apellido(s)")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Ingrese una contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo.")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe tener exactamente 9 dígitos y comenzar con 9")]
        public string? Phone { get; set; }
    }
}
