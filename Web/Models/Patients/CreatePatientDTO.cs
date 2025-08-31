using System.ComponentModel.DataAnnotations;

namespace Web.Models.Patients
{
    public class CreatePatientDTO
    {
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
        [RegularExpression(@"^$|^\d{9,}$", ErrorMessage = "Ingrese al menos 9 dígitos numéricos")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Ingrese una fecha de nacimiento")]
        [Display(Name = "Fecha nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? BirthDate { get; set; }

        [Display(Name = "Grupo sanguíneo")]
        public string? BloodType { get; set; }
    }
}
