using System.ComponentModel.DataAnnotations;

namespace Web.Models.Patients
{
    public class PatientEditViewModel
    {
        [Display(Name = "Código")]
        public int UserId { get; set; }

        [Display(Name = "Nombre(s)")]
        public string? FirstName { get; set; }

        [Display(Name = "Apellido(s)")]
        public string? LastName { get; set; }

        [Display(Name = "Correo electrónico")]
        public string? Email { get; set; }

        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Ingrese una fecha de nacimiento")]
        [Display(Name = "Fecha nacimiento")]
        public DateOnly BirthDate { get; set; }

        [Display(Name = "Grupo sanguíneo")]
        public string? BloodType { get; set; }
    }
}
