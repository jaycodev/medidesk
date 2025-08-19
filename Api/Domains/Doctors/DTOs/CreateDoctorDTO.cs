using System.ComponentModel.DataAnnotations;

namespace Api.Domains.Doctors.DTOs
{
    public class CreateDoctorDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email is not a valid email address")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must have at least 8 characters, one uppercase, one number, and one symbol")]
        public string Password { get; set; }

        [RegularExpression(@"^$|^\d{9,}$", ErrorMessage = "Phone must have at least 9 numeric digits")]
        public string? Phone { get; set; }

        [Required]
        public int SpecialtyId { get; set; }
    }
}
