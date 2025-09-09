using System.ComponentModel.DataAnnotations;
using Shared.DTOs.Users.Requests;

namespace Shared.DTOs.Doctors.Requests
{
    public class CreateDoctorRequest : CreateUserRequest
    {
        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
