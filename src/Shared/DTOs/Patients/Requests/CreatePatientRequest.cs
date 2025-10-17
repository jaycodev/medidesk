using System.ComponentModel.DataAnnotations;
using Shared.DTOs.Users.Requests;

namespace Shared.DTOs.Patients.Requests
{
    public class CreatePatientRequest : CreateUserRequest
    {
        [Required]
        public DateOnly BirthDate { get; set; }

        public string? BloodType { get; set; }
    }
}
