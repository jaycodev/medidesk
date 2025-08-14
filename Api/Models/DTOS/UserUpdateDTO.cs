using System.ComponentModel.DataAnnotations;

namespace Api.Models.DTOS
{
    public class UserUpdateDTO
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

        [Display(Name = "Teléfono")]
        [RegularExpression(@"^\d{9,}$", ErrorMessage = "Ingrese al menos 9 dígitos numéricos")]
        public string Phone { get; set; }

    }
}
