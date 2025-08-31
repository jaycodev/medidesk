using System.ComponentModel.DataAnnotations;

namespace Web.Models.Patients
{
    public class UpdatePatientDTO
    {
        [Required]
        public DateOnly BirthDate { get; set; }

        [Required]
        public string? BloodType { get; set; }
    }
}
