using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Patients.Requests
{
    public class UpdatePatientRequest
    {
        [Required]
        public DateOnly BirthDate { get; set; }

        public string? BloodType { get; set; }
    }
}
