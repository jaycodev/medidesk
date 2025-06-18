using System.ComponentModel.DataAnnotations;

namespace sistema_citas_medicas.Models
{
    public class User
    {
        [Display(Name = "Código")]
        public int UserId { get; set; }

        [Display(Name = "Nombre(s)")]
        [Required(ErrorMessage = "Ingrese un(os) nombre(s)")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido(s)")]
        [Required(ErrorMessage = "Ingrese un(os) apellido(s)")]
        public string LastName { get; set; }

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Ingrese una contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo.")]
        public string Password { get; set; }

        [Display(Name = "Teléfono")]
        [RegularExpression(@"^\d{9,}$", ErrorMessage = "Ingrese al menos 9 dígitos numéricos")]
        public string Phone { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "Ingrese un rol")]
        public string Role { get; set; }

        [Display(Name = "Foto perfil")]
        public string ProfilePicture { get; set; }
    }
}