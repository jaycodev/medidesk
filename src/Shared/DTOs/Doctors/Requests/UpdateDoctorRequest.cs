using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Doctors.Requests
{
    public class UpdateDoctorRequest
    {
        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
