using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class UserEditViewModel : IValidatableObject
    {
        [Display(Name = "Código")]
        public int UserId { get; set; }

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

        [Display(Name = "Teléfono")]
        [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe tener exactamente 9 dígitos y comenzar con 9")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Seleccione una combinación de rol(es)")]
        public string? SelectedRoleCombo { get; set; }

        public bool IsDoctor => (SelectedRoleCombo ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(r => r.Equals("medico", StringComparison.OrdinalIgnoreCase));

        public bool IsPatient => (SelectedRoleCombo ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(r => r.Equals("paciente", StringComparison.OrdinalIgnoreCase));

        public List<string> Roles { get; set; } = new List<string>();

        [Display(Name = "Especialidad")]
        public int? SpecialtyId { get; set; }

        [Display(Name = "Estado")]
        public bool? Status { get; set; }

        [Display(Name = "Fecha nacimiento")]
        [DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }

        [Display(Name = "Grupo sanguíneo")]
        public string? BloodType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsDoctor)
            {
                if (SpecialtyId == null || SpecialtyId == 0)
                {
                    yield return new ValidationResult("Seleccione una especialidad.", new[] { nameof(SpecialtyId) });
                }

                if (Status == null)
                {
                    yield return new ValidationResult("Seleccione un estado.", new[] { nameof(Status) });
                }
            }

            if (IsPatient)
            {
                if (BirthDate == null)
                {
                    yield return new ValidationResult("Ingrese una fecha de nacimiento.", new[] { nameof(BirthDate) });
                }
            }
        }
    }
}
