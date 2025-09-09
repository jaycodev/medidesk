using System.ComponentModel.DataAnnotations;
using Web.Models.Account;

namespace Web.Models.Profile
{
    public class ProfileViewModel
    {
        [Display(Name = "Código")]
        public int UserId { get; set; }

        [Display(Name = "Nombre(s)")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Apellido(s)")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe tener exactamente 9 dígitos y comenzar con 9")]
        public string? Phone { get; set; }

        [Display(Name = "Foto perfil")]
        public string? ProfilePicture { get; set; }

        public ChangePasswordViewModel ChangePassword { get; set; } = new ChangePasswordViewModel();
    }
}
